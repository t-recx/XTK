using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TS
{
    public static class IO
    {
        public static Tileset Load(string Filename, GraphicsDevice GraphDevice)
        {
            Tileset Ts = new Tileset();
            XmlSerializer serializer = new XmlSerializer(typeof(Tileset));

            FileStream fs = new FileStream(Filename, FileMode.Open);
            XmlReader reader = new XmlTextReader(fs);

            Ts = (Tileset)serializer.Deserialize(reader);

            if (Ts.Items != null)
            {
                foreach (TilesetItem item in Ts.Items)
                {
                    if (item.Sprites == null)
                        item.Sprites = new List<Texture2D>();

                    foreach (string location in item.SpritesLocation)
                    {
                        FileStream fsSprite = new FileStream(location, FileMode.Open);

                        item.Sprites.Add(Texture2D.FromStream(GraphDevice, fsSprite));
                    }
                }
            }

            return Ts;
        }

        public static int Save(string Filename, Tileset Ts)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Tileset));

            TextWriter tw = new StreamWriter(Filename);

            xs.Serialize(tw, Ts);

            tw.Close();

            return 0;
        }
    }
}

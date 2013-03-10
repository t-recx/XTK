using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class TilesetItem
    {
        public string AssetName;

        public List<TilesetItemAttribute> Attributes;

        public int MSToNextFrame;
        public List<string> SpritesLocation;

        [XmlIgnore]
        public List<Texture2D> Sprites;
    }
}

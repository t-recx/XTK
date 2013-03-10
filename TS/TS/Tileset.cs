using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TS
{
    public class Tileset
    {
        public string Name;
        public List<TilesetItem> Items = new List<TilesetItem>();

        public Texture2D GetPicture(string assetName, int index)
        {
            TilesetItem item = Items.Find(i => i.AssetName == assetName);

            if (item == null || item.Sprites.Count - 1 < index)
                return null;

            return item.Sprites[index];
        }

        public Texture2D GetPicture(string assetName, string attribute, object value, int index)
        {
            TilesetItem item = Items.Find(i => i.AssetName == assetName && i.Attributes.Exists(a => a.Name == attribute && a.Value.ToString() == value.ToString()));

            if (item == null || item.Sprites.Count - 1 < index)
                return null;

            return item.Sprites[index];
        }

        public Texture2D GetPicture(string assetName, string attribute, object value)
        {
            return GetPicture(assetName, attribute, value, 0);
        }

        public Texture2D GetPicture(string assetName)
        {
            return GetPicture(assetName, 0);
        }
    }
}

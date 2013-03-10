using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XTK
{
    public class TextChangingEventArgs : EventArgs
    {
        public string Text;
        public bool Cancel = false;

        public TextChangingEventArgs(string text)
        {
            Text = text;
        }
    }

    public delegate void TextChangingEventHandler(Object sender, TextChangingEventArgs e);
}

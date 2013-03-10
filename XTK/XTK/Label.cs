using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XTK
{
    public class Label : Control
    {
        public Label(Form formowner)
            : base(formowner)
        {
        }

        public override void Draw()
        {
            Drawing.DrawString(spriteBatch, Theme.Font, Text, OwnerX + X, OwnerY + Y, ForeColor, Z - 0.00001f);

            base.Draw();
        }
    }
}

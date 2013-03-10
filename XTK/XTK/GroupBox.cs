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
    public class GroupBox : Control
    {
        private List<Texture2D> frame = null;

        public List<Texture2D> Frame
        {
            get
            {
                if (frame != null)
                    return frame;

                return Theme.GroupFrame;
            }
            set
            {
                frame = value;
            }
        }

        public GroupBox(Form formowner)
            : base(formowner)
        {
        }

        public override void Draw()
        {
            if (Frame != null)
            {
                Drawing.DrawFrame(spriteBatch, Frame, OwnerX + X,
                    OwnerY + Y + Drawing.GetFontHeight(Theme.Font) / 2, Width, Height - Drawing.GetFontHeight(Theme.Font) / 2, Z);

                Drawing.DrawRectangle(spriteBatch, Frame[(int)FramePart.Background], Color.White,
                    OwnerX + X + Width / 2 - (int)Theme.Font.MeasureString(Text).X / 2, 
                    OwnerY + Y,
                    (int)Theme.Font.MeasureString(Text).X, Drawing.GetFontHeight(Theme.Font), Z - 0.0018f);
            }

            Drawing.DrawCenteredText(spriteBatch, Theme.Font, Text, OwnerX + X + Width / 2, OwnerY + Y, ForeColor, null, Z - 0.002f);

            if (Controls != null)
                for (int i = 0; i < Controls.Count; i++)
                    Controls[i].Z = Z - 0.0021f;
        }
    }
}

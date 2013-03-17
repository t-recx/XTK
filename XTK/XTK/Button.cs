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
    public class Button : Control
    {
        public bool Pressed = false;

        private Texture2D image = null;

        private List<Texture2D> frame = null;
        private List<Texture2D> framePressed = null;

        public List<Texture2D> Frame
        {
            get
            {
                if (frame != null)
                    return frame;

                return Theme.Frame;
            }
            set
            {
                frame = value;
            }
        }

        public List<Texture2D> FramePressed
        {
            get
            {
                if (framePressed != null)
                    return framePressed;

                return Theme.FramePressed;
            }
            set
            {
                framePressed = value;
            }
        }

        public Texture2D Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }

        private bool PressedWithMouse = false;

        public bool DepressImageOnPress = true;
        public bool DepressTextOnPress = true;

        public Button(Form formowner)
            : base(formowner)
        {
        }

        public override void Draw()
        {
            if (Frame != null)
            {
                if (Pressed)
                    Drawing.DrawFrame(spriteBatch, FramePressed, OwnerX + X, OwnerY + Y, Width, Height, Z);
                else
                    Drawing.DrawFrame(spriteBatch, Frame, OwnerX + X, OwnerY + Y, Width, Height, Z);
            }

            if (Text != null)
                Drawing.DrawCenteredText(spriteBatch, Font, Text, OwnerX + X + Width / 2 + (Pressed && DepressTextOnPress ? 1 : 0), 
                    OwnerY + Y + Height / 2 - Drawing.GetFontHeight(Font) / 2
                    + (Pressed && DepressTextOnPress ? 1 : 0) - 1, ForeColor, null, Z - 0.001f);

            if (Image != null)
                Drawing.Draw(spriteBatch, Image,
                    new Vector2(OwnerX + X + Width / 2 - Image.Width / 2 + (Pressed && DepressImageOnPress ? 1 : 0), OwnerY + Y + Height / 2 - Image.Height / 2 +
                        (Pressed && DepressImageOnPress ? 1 : 0)), Color.White, Z - 0.001f);

            //base.Draw();
        }

        public override bool ReceiveMessage(MessageEnum message, object msgTag)
        {
            switch (message)
            {
                case MessageEnum.MouseLeftDown:
                    if (Focused && OwnerHasFocus)
                    {
                        if (MouseIsOnControl())
                        {
                            Pressed = true;
                            PressedWithMouse = true;
                        }
                    }
                    break;

                case MessageEnum.KeyDown:
                    if (Focused && OwnerHasFocus)
                    {
                        if (((Keys)msgTag) == Keys.Enter)
                        {
                            Pressed = true;
                            PressedWithMouse = false;
                        }
                    }
                    break;

                case MessageEnum.KeyUp:
                    if (Focused && OwnerHasFocus && Pressed)
                    {
                        if (((Keys)msgTag) == Keys.Enter)
                        {
                            Pressed = false;
                            
                            PerformClick();
                        }
                    }
                    break;

                case MessageEnum.UnFocus:
                case MessageEnum.MouseLeftClick:
                case MessageEnum.MouseRightClick:
                    Pressed = false;
                    PressedWithMouse = false;
                    break;

                case MessageEnum.Logic:
                    if (PressedWithMouse && !MouseIsOnControl())
                    {
                        Pressed = false;
                        //PressedWithMouse = false;
                    }
                    break;
            }

            return base.ReceiveMessage(message, msgTag);
        }
    }
}

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
using GameResolution;

namespace XTK
{
    public class PictureBox : Control
    {
        private PictureBoxSizeMode sizeMode = PictureBoxSizeMode.Normal;

        private Texture2D image;

        protected int imageWidth = 0, imageHeight = 0;
        protected int imageX = 0, imageY = 0;

        public PictureBoxSizeMode SizeMode
        {
            get
            {
                return sizeMode;
            }
            set
            {
                if (sizeMode != value)
                {
                    sizeMode = value;
                    SetInternalImageRepresentation();
                }
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
                if (image != value)
                {
                    image = value;
                    SetInternalImageRepresentation();
                }
            }
        }

        public PictureBox(Form formowner)
            : base(formowner)
        {
            this.Init += new EventHandler(PictureBox_Init);
        }

        void PictureBox_Init(object sender, EventArgs e)
        {
            SetInternalImageRepresentation();
        }

        private void SetInternalImageRepresentation()
        {
            imageX = OwnerX + X;
            imageY = OwnerY + Y;

            switch (SizeMode)
            {
                case PictureBoxSizeMode.Normal:
                    imageWidth = Image.Width;
                    imageHeight = Image.Height;

                    if (imageWidth > Width)
                        imageWidth = Width;
                    if (imageHeight > Height)
                        imageHeight = Height;
                    break;

                case PictureBoxSizeMode.Stretch:
                    imageWidth = Width;
                    imageHeight = Height;
                    break;

                case PictureBoxSizeMode.AutoSize:
                    Width = Image.Width;
                    Height = Image.Height;
                    imageWidth = Width;
                    imageHeight = Height;
                    break;

                case PictureBoxSizeMode.CenterImage:
                    imageX = imageX + Width / 2 - imageWidth / 2;
                    imageY = imageY + Height / 2 - imageHeight / 2;
                    break;
            }
        }

        public override void Draw()
        {
            Drawing.Draw(spriteBatch, Image, imageX, imageY, imageWidth, imageHeight, Color.White, SizeMode == PictureBoxSizeMode.Stretch, Z - 0.0001f);

 	        base.Draw();
        }
    }

    public enum PictureBoxSizeMode
    {
        Normal = 0,
        Stretch = 1,
        AutoSize = 2,
        CenterImage = 3
    }
}

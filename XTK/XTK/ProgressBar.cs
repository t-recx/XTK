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
    public class ProgressBar : Control
    {
        private int minimum = 0, maximum = 100, value;

        private int paddingLeft = 0, paddingRight = 0, paddingUp = 0, paddingDown = 0;

        public int Minimum
        {
            get
            {
                return minimum;
            }
            set
            {
                minimum = value;
            }
        }

        public int Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                maximum = value;
            }
        }

        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public ProgressBar(Form formowner)
            : base(formowner)
        {
            this.Init += new EventHandler(ProgressBar_Init);
        }

        void ProgressBar_Init(object sender, EventArgs e)
        {
            paddingUp = Theme.TileSet.GetPicture("ProgressBar", "Part", "Up").Height;
            paddingDown = Theme.TileSet.GetPicture("ProgressBar", "Part", "Down").Height;
            paddingLeft = Theme.TileSet.GetPicture("ProgressBar", "Part", "Left").Width;
            paddingRight = Theme.TileSet.GetPicture("ProgressBar", "Part", "Right").Width;
        }

        private int GetBarWidth()
        {
            int bWidth = 0;

            bWidth = (int)((Width - (paddingLeft + paddingRight)) * 
                (Value / (double)(Maximum - Minimum)));

            return bWidth;
        }

        public override void Draw()
        {
            Drawing.DrawFrame(spriteBatch, Theme.ProgressBarFrame, OwnerX + X, OwnerY + Y, Width, Height, Z - 0.00001f);
            Drawing.DrawRectangle(spriteBatch, Theme.Dot, ForeColor, 
                OwnerX + X + paddingLeft + 1, OwnerY + Y + paddingUp + 1,
                GetBarWidth() - 1, Height - paddingDown - paddingUp - 1, Z - 0.0002f);

 	        base.Draw();
        }
    }
}

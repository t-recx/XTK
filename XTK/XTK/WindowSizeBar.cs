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
    public class WindowSizeBar : Control
    {
        Button LeftEdge, RightEdge, BottomEdge;

        private int mX, mY;
        private int oX, oY, oW, oH;

        public int EdgeHeight = 8;

        public WindowSizeBar(Form formowner)
            : base(formowner)
        {
            LeftEdge = new Button(formowner) { Height = EdgeHeight };
            RightEdge = new Button(formowner) { Height = EdgeHeight };
            BottomEdge = new Button(formowner) { Height = EdgeHeight };

            LeftEdge.Parent = RightEdge.Parent = BottomEdge.Parent = this;

            Controls.AddRange(new Control [] {LeftEdge, RightEdge, BottomEdge });

            this.Init += new EventHandler(WindowSizeBar_Init);
            Owner.WidthChanged += new EventHandler(Owner_SizeChanged);
            Owner.HeightChanged += new EventHandler(Owner_SizeChanged);
            LeftEdge.MouseLeftDown += new EventHandler(Edge_MouseLeftDown); 
            RightEdge.MouseLeftDown += new EventHandler(Edge_MouseLeftDown);
            BottomEdge.MouseLeftDown += new EventHandler(Edge_MouseLeftDown);
        }

        void Owner_SizeChanged(object sender, EventArgs e)
        {
            AdjustInnerControls();
        }

        void Edge_MouseLeftDown(object sender, EventArgs e)
        {
            if ((sender as Control).Tag == null)
                (sender as Control).Tag = false;

            int pW = Owner.Width, pH = Owner.Height;
            bool mouseDown = (bool)(sender as Control).Tag;

            if (!mouseDown)
            {
                mX = WindowManager.MouseX;
                mY = WindowManager.MouseY;
                oX = Owner.X;
                oY = Owner.Y;
                oW = Owner.Width;
                oH = Owner.Height;

                (sender as Control).Tag = true;
            }

            if (pW != Owner.Width || pH != Owner.Height)
                AdjustInnerControls();
        }

        void WindowSizeBar_Init(object sender, EventArgs e)
        {
            AdjustInnerControls();
            LeftEdge.Frame = RightEdge.Frame = BottomEdge.Frame = Theme.WindowEdgeFrame;
            LeftEdge.FramePressed = RightEdge.FramePressed = BottomEdge.FramePressed = Theme.WindowEdgeFrame;

            LeftEdge.Cursor = Theme.ResizeCursorDownLeft;
            RightEdge.Cursor = Theme.ResizeCursorDownRight;
            BottomEdge.Cursor = Theme.ResizeCursorDown;
        }

        public void AdjustInnerControls()
        {
            this.X = 0;
            this.Y = Owner.Height- EdgeHeight;
            this.Width = Owner.Width;
            this.Height = EdgeHeight;

            int EdgeWidth = 0;

            if (Width > 60)
            {
                EdgeWidth = 30;
                BottomEdge.Visible = true;
            }
            else
            {
                EdgeWidth = Width / 2;
                BottomEdge.Visible = false;
            }

            LeftEdge.Width = RightEdge.Width = EdgeWidth;
            BottomEdge.Width = Width - EdgeWidth * 2;

            LeftEdge.X = 0;
            LeftEdge.Y = 0;
            RightEdge.X = Owner.Width - RightEdge.Width;
            RightEdge.Y = 0;
            BottomEdge.X = LeftEdge.X + LeftEdge.Width;
            BottomEdge.Y = 0;
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void Update()
        {
            if (!WindowManager.MouseLeftPressed)
                LeftEdge.Tag = RightEdge.Tag = BottomEdge.Tag = false;

            if (LeftEdge.Tag != null && (bool)LeftEdge.Tag)
            {
                Owner.X = oX - (mX - WindowManager.MouseX);
                Owner.Width = oW + (mX - WindowManager.MouseX);
                Owner.Height = oH - (mY - WindowManager.MouseY);
            }
            else if (RightEdge.Tag != null && (bool)RightEdge.Tag)
            {
                Owner.Width = oW - (mX - WindowManager.MouseX);
                Owner.Height = oH - (mY - WindowManager.MouseY);
            }
            else if (BottomEdge.Tag != null && (bool)BottomEdge.Tag)
            {
                Owner.Height = oH - (mY - WindowManager.MouseY);
            }

            base.Update();
        } 
    }
}

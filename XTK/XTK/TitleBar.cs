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
    public class TitleBar : Control
    {
        public int TitleBarHeight = 23;

        private bool mouseDown = false;
        private int mX, mY;

        Button btnCloseWindow;
        Button btnMaximizeWindow;
        Button btnMinimizeWindow;
        Button btnWindowOptions;

        public Color TextColorFocused = Color.White;
        public Color TextColorUnfocused = Color.Black;

        private int OwnerPreviousX, OwnerPreviousY, OwnerPreviousWidth, OwnerPreviousHeight;

        public bool MaximizeWindowButtonVisible
        {
            get
            {
                return btnMaximizeWindow.Visible;
            }
            set
            {
                btnMaximizeWindow.Visible = value;

                AdjustControlsCoordinatesAndSize();
            }
        }

        public bool MinimizeWindowButtonVisible
        {
            get
            {
                return btnMinimizeWindow.Visible;
            }
            set
            {
                btnMinimizeWindow.Visible = value;

                AdjustControlsCoordinatesAndSize();
            }
        }

        public bool CloseWindowButtonVisible
        {
            get
            {
                return btnCloseWindow.Visible;
            }
            set
            {
                btnCloseWindow.Visible = value;

                AdjustControlsCoordinatesAndSize();
            }
        }

        public bool WindowOptionsButtonVisible
        {
            get
            {
                return btnWindowOptions.Visible;
            }
            set
            {
                btnWindowOptions.Visible = value;

                AdjustControlsCoordinatesAndSize();
            }
        }

        public event EventHandler OnMaximizeButtonClick;

        public TitleBar(Form formowner)
            : base(formowner)
        {
            btnCloseWindow = new Button(formowner) { Width = 15, Height = 15 };
            btnMaximizeWindow = new Button(formowner) { Width = 15, Height = 15 };
            btnMinimizeWindow = new Button(formowner) { Width = 15, Height = 15 };
            btnWindowOptions = new Button(formowner) { Width = 15, Height = 15 };
            btnCloseWindow.Parent = btnMaximizeWindow.Parent = btnMinimizeWindow.Parent = btnWindowOptions.Parent = this;
            Controls.AddRange(new Control [] { btnCloseWindow, btnMaximizeWindow, btnMinimizeWindow, btnWindowOptions });

            btnCloseWindow.Click += new EventHandler(btnCloseWindow_Click);
            this.MouseLeftDown += new EventHandler(TitleBar_MouseLeftDown);
            this.Init += new EventHandler(TitleBar_Init);
            btnMaximizeWindow.Click += new EventHandler(btnMaximizeWindow_Click);
            Owner.WidthChanged += new EventHandler(Owner_SizeChanged);
            Owner.HeightChanged += new EventHandler(Owner_SizeChanged);
        }

        void btnMaximizeWindow_Click(object sender, EventArgs e)
        {
            if (OnMaximizeButtonClick != null)
                OnMaximizeButtonClick(this, new EventArgs());
            else
                HandleWindowStateChange();
        }

        private void HandleWindowStateChange()
        {
            if (Owner.WindowState != FormWindowState.Maximized)
            {
                OwnerPreviousX = Owner.X;
                OwnerPreviousY = Owner.Y;
                OwnerPreviousWidth = Owner.Width;
                OwnerPreviousHeight = Owner.Height;

                Owner.X = Owner.Y = 0;
                Owner.Width = WindowManager.MaximizedWindowWidth;
                Owner.Height = WindowManager.MaximizedWindowHeight + Owner.EdgeHeight;

                Owner.WindowState = FormWindowState.Maximized;
            }
            else
            {
                Owner.X = OwnerPreviousX;
                Owner.Y = OwnerPreviousY;
                Owner.Width = OwnerPreviousWidth;
                Owner.Height = OwnerPreviousHeight;

                Owner.WindowState = FormWindowState.Normal;
            }

            SetMaximizeButtonImage(); 
        }

        private void SetMaximizeButtonImage()
        {
            if (Owner.WindowState == FormWindowState.Maximized)
                btnMaximizeWindow.Image = Theme.UnMaximize;
            else if (Owner.WindowState == FormWindowState.Normal)
                btnMaximizeWindow.Image = Theme.Maximize;
        }

        void Owner_SizeChanged(object sender, EventArgs e)
        {
            AdjustControlsCoordinatesAndSize();
        }

        private void AdjustControlsCoordinatesAndSize()
        {
            int buttonX;

            Width = Owner.Width;
            Height = TitleBarHeight;

            btnWindowOptions.X = 4;
            buttonX = Width - 4 - btnCloseWindow.Width;
            btnCloseWindow.X = buttonX;
            if (btnCloseWindow.Visible)
                buttonX -= btnCloseWindow.Width;
            btnMaximizeWindow.X = buttonX;
            if (btnMaximizeWindow.Visible)
                buttonX -= btnMaximizeWindow.Width;
            btnMinimizeWindow.X = buttonX;
            btnCloseWindow.Y = btnMaximizeWindow.Y = btnMinimizeWindow.Y = btnWindowOptions.Y = 4;
        }

        void TitleBar_Init(object sender, EventArgs e)
        {
            Font = Theme.FontBold;

            btnCloseWindow.Image = Theme.CloseIcon;
            btnMaximizeWindow.Image = Theme.Maximize;
            btnMinimizeWindow.Image = Theme.Minimize;
            btnWindowOptions.Image = Theme.WindowOptions;
        }

        void btnCloseWindow_Click(object sender, EventArgs e)
        {
            Owner.Visible = false;
        }

        void TitleBar_MouseLeftDown(object sender, EventArgs e)
        {
            if (!mouseDown && Owner.WindowState != FormWindowState.Maximized)
            {
                mX = WindowManager.MouseX - OwnerX;
                mY = WindowManager.MouseY - OwnerY;

                mouseDown = true;
            }
        }

        public override void Draw()
        {
            List<Texture2D> TitleBarFrame;

            if (Owner.Focused)
                TitleBarFrame = Theme.TitleBarFocusedFrame;
            else
                TitleBarFrame = Theme.TitleBarUnfocusedFrame;

            Drawing.DrawFrame(spriteBatch, TitleBarFrame, Owner.X + X, Owner.Y + Y, Owner.Width, TitleBarHeight, Z - 0.00001f);
            Drawing.DrawCenteredText(spriteBatch, Font, Text, Owner.X + X + Width / 2, Owner.Y + Y + Height / 2 - Drawing.GetFontHeight(Font) / 2 + 1, 
                Owner.Focused ? TextColorFocused : TextColorUnfocused, null, Z - 0.001f);

            //base.Draw();
        }

        public override void Update()
        {
            if (!WindowManager.MouseLeftPressed)
                mouseDown = false;

            if (mouseDown && Owner.Focused)
            {
                this.Owner.X = (WindowManager.MouseX - mX);
                this.Owner.Y = (WindowManager.MouseY - mY);
            }

            base.Update();
        }
    }
}

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
    public class Form : Control
    {
        public FormWindowState WindowState = FormWindowState.Normal;

        protected TitleBar titleBar;
        protected WindowSizeBar windowSizeBar;

        private bool resizable = true;

        private bool useTitleBar = true;

        public bool UseTitleBar
        {
            get
            {
                return useTitleBar;
            }
            set
            {
                if (value != useTitleBar)
                {
                    useTitleBar = value;
                    SetTitleBar();
                }
            }
        }

        public bool Resizable
        {
            get
            {
                return resizable;
            }
            set
            {
                if (resizable != value)
                {
                    resizable = value;
                    SetWindowSizeBar();
                }
            }
        }

        public int EdgeHeight
        {
            get
            {
                return windowSizeBar.EdgeHeight;
            }
        }

        public bool MaximizeWindowButtonVisible
        {
            get
            {
                return titleBar.MaximizeWindowButtonVisible;
            }
            set
            {
                titleBar.MaximizeWindowButtonVisible = value;
            }
        }

        public bool MinimizeWindowButtonVisible
        {
            get
            {
                return titleBar.MinimizeWindowButtonVisible;
            }
            set
            {
                titleBar.MinimizeWindowButtonVisible = value;
            }
        }

        public bool CloseWindowButtonVisible
        {
            get
            {
                return titleBar.CloseWindowButtonVisible;
            }
            set
            {
                titleBar.CloseWindowButtonVisible = value;
            }
        }

        public bool WindowOptionsButtonVisible
        {
            get
            {
                return titleBar.WindowOptionsButtonVisible;
            }
            set
            {
                titleBar.WindowOptionsButtonVisible = value;
            }
        }

        public Form(WindowManager windowmanager)
        {
            titleBar = new TitleBar(this);
            titleBar.ForeColor = Color.White;
            windowSizeBar = new WindowSizeBar(this);

            WindowManager = windowmanager;
            this.TextChanged += new EventHandler(Form_TextChanged);
        }

        void Form_TextChanged(object sender, EventArgs e)
        {
            titleBar.Text = Text;
        }

        public override bool ReceiveMessage(MessageEnum message, object msgTag)
        {
            if (message == MessageEnum.Focus)
                if (WindowManager != null)
                    WindowManager.BringToFront(this);

            return base.ReceiveMessage(message, msgTag);
        }

        protected void SetTitleBar()
        {
            if (UseTitleBar)
            {
                if (!Controls.Contains(titleBar))
                {
                    TransposeY(titleBar.TitleBarHeight);

                    Controls.Add(titleBar);
                }
            }
            else
            {
                if (Controls.Contains(titleBar))
                {
                    TransposeY(-titleBar.TitleBarHeight);

                    Controls.Remove(titleBar);
                }
            }
        }

        protected void SetWindowSizeBar()
        {
            if (Resizable)
            {
                if (!Controls.Contains(windowSizeBar))
                {
                    Height += windowSizeBar.EdgeHeight;
                    MinimumHeight += windowSizeBar.EdgeHeight;

                    Controls.Add(windowSizeBar);
                }
            }
            else
            {
                if (Controls.Contains(windowSizeBar))
                {
                    Height -= windowSizeBar.EdgeHeight;
                    MinimumHeight -= windowSizeBar.EdgeHeight;
                    Controls.Remove(windowSizeBar);
                }
            }
        }

        public override void InitControl()
        {
            SetTitleBar();
            SetWindowSizeBar();

            if (Controls != null)
                foreach (Control ctrl in Controls)
                    ctrl.InitControl();

            base.InitControl();
        }

        private void TransposeY(int addedY)
        {
            foreach (Control control in Controls)
                control.Y += addedY;

            Height += addedY;
            MinimumHeight += addedY;
        }

        public override void Draw()
        {
            Drawing.DrawRectangle(spriteBatch, Theme.Dot, Color.Black, X, Y, Width, 1, Z - 0.002f);
            Drawing.DrawRectangle(spriteBatch, Theme.Dot, Color.Black, X, Y + Height - 1, Width, 1, Z - 0.002f);
            Drawing.DrawRectangle(spriteBatch, Theme.Dot, Color.Black, X, Y, 1, Height, Z - 0.002f);
            Drawing.DrawRectangle(spriteBatch, Theme.Dot, Color.Black, X + Width - 1, Y, 1, Height, Z - 0.002f);

            base.Draw();
        }
    }

    public enum FormBorderStyle
    {
        None = 0,
        FixedSingle = 1
    }

    public enum FormWindowState
    {
        Normal = 0,
        Maximized = 1,
        Minimized = 2
    }
}

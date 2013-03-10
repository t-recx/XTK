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
    public class ScrollbarHorizontal : Control
    {
        private int btnWidth = 16;

        protected Button btnLeft;
        protected Button btnRight;
        protected InnerScrollbarHorizontal innerScrollBarHorizontal;

        private int MsToNextIndex = 300;
        private double TotalMilliseconds = 0;

        private bool ScrolledMouseLeftDown;

        public int TotalNrItems
        {
            get
            {
                return innerScrollBarHorizontal.TotalNrItems;
            }
            set
            {
                innerScrollBarHorizontal.TotalNrItems = value;
            }
        }

        public int NrItemsPerPage
        {
            get
            {
                return innerScrollBarHorizontal.NrItemsPerPage;
            }
            set
            {
                innerScrollBarHorizontal.NrItemsPerPage = value;
            }
        }

        public int Index
        {
            get
            {
                return innerScrollBarHorizontal.Index;
            }
            set
            {
                innerScrollBarHorizontal.Index = value;
            }
        }

        public double WidthPerItem
        {
            get
            {
                return innerScrollBarHorizontal.WidthPerItem;
            }
            set
            {
                innerScrollBarHorizontal.WidthPerItem = value;
            }
        }

        public event EventHandler IndexChanged;

        public ScrollbarHorizontal(Form formowner)
            : base(formowner)
        {
            btnLeft = new Button(formowner) { X = Width - btnWidth * 2, Y = 0, Width = btnWidth, Height = Height, Tag = (double)0 };
            btnRight = new Button(formowner) { X =  Width - btnWidth, Y = 0, Width = btnWidth, Height = Height, Tag = (double)0 };
            innerScrollBarHorizontal = new InnerScrollbarHorizontal(formowner) { X = 0, Y = 0, Width = Width - (btnLeft.Width + btnRight.Width), Height = Height };

            btnLeft.MouseLeftPressed += new EventHandler(btn_MouseLeftPressed);
            btnLeft.MouseLeftDown += new EventHandler(btn_MouseLeftDown);
            btnLeft.MouseLeftUp += new EventHandler(btn_MouseLeftUp);
            btnRight.MouseLeftPressed += new EventHandler(btn_MouseLeftPressed);
            btnRight.MouseLeftDown += new EventHandler(btn_MouseLeftDown);
            btnRight.MouseLeftUp += new EventHandler(btn_MouseLeftUp);
            btnLeft.Click += new EventHandler(btn_Click);
            btnRight.Click += new EventHandler(btn_Click);
            innerScrollBarHorizontal.IndexChanged += new EventHandler(innerScrollBarHorizontal_IndexChanged);
            innerScrollBarHorizontal.WidthChanged += new EventHandler(innerScrollBarHorizontal_SizeChanged);
            innerScrollBarHorizontal.HeightChanged += new EventHandler(innerScrollBarHorizontal_SizeChanged);

            btnLeft.Image = Theme.ArrowLeft;
            btnRight.Image = Theme.ArrowRight;

            btnLeft.Parent = btnRight.Parent = innerScrollBarHorizontal.Parent = this;

            Controls.Add(innerScrollBarHorizontal);
            Controls.Add(btnRight);
            Controls.Add(btnLeft);

            this.Init += new EventHandler(ScrollbarHorizontal_Init);
        }

        void innerScrollBarHorizontal_SizeChanged(object sender, EventArgs e)
        {
            innerScrollBarHorizontal.ConfigureBar();
        }

        void btn_MouseLeftUp(object sender, EventArgs e)
        {
            ScrolledMouseLeftDown = false;
        }

        void btn_Click(object sender, EventArgs e)
        {
            if (!ScrolledMouseLeftDown)
                Index += sender == btnLeft ? -1 : 1;
        }

        void btn_MouseLeftPressed(object sender, EventArgs e)
        {
            (sender as Button).Tag = (double)0;
            ScrolledMouseLeftDown = false;
        }

        void ScrollbarHorizontal_Init(object sender, EventArgs e)
        {
            InitInnerControls();
        }

        void btn_MouseLeftDown(object sender, EventArgs e)
        {
            Button btn = (sender as Button);

            if (TotalMilliseconds - (double)btn.Tag > MsToNextIndex)
            {
                Index += sender == btnLeft ? -1 : 1;

                btn.Tag = TotalMilliseconds;
                ScrolledMouseLeftDown = true;
            }
        }

        void innerScrollBarHorizontal_IndexChanged(object sender, EventArgs e)
        {
            if (IndexChanged != null)
                IndexChanged(this, new EventArgs());
        }

        private void InitInnerControls()
        {
            btnLeft.X = Width - btnWidth * 2;
            btnRight.X = Width - btnWidth;
            innerScrollBarHorizontal.X = 0;
            btnLeft.Width = btnWidth;
            btnRight.Width = btnWidth;
            innerScrollBarHorizontal.Width = Width - btnWidth * 2;
            btnLeft.Y = 0;
            btnRight.Y = 0;
            innerScrollBarHorizontal.Y = 0;
            innerScrollBarHorizontal.Height = Height;
            btnLeft.Height = btnRight.Height = Height;

            btnLeft.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            btnRight.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            innerScrollBarHorizontal.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;

            innerScrollBarHorizontal.ConfigureBar();
        }

        public override void Update()
        {
            TotalMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;

            base.Update();
        }
    }
}

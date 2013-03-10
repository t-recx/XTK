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
    public class ScrollbarVertical : Control
    {
        private int btnHeight = 16;

        protected Button btnUp;
        protected Button btnDown;
        protected InnerScrollbarVertical innerScrollBarVertical;

        private int MsToNextIndex = 300;
        private double TotalMilliseconds = 0;

        private bool ScrolledMouseLeftDown;

        public int TotalNrItems
        {
            get
            {
                return innerScrollBarVertical.TotalNrItems;
            }
            set
            {
                innerScrollBarVertical.TotalNrItems = value;
            }
        }

        public int NrItemsPerPage
        {
            get
            {
                return innerScrollBarVertical.NrItemsPerPage;
            }
            set
            {
                innerScrollBarVertical.NrItemsPerPage = value;
            }
        }

        public int Index
        {
            get
            {
                return innerScrollBarVertical.Index;
            }
            set
            {
                innerScrollBarVertical.Index = value;
            }
        }

        public double HeightPerItem
        {
            get
            {
                return innerScrollBarVertical.HeightPerItem;
            }
            set
            {
                innerScrollBarVertical.HeightPerItem = value;
            }
        }

        public event EventHandler IndexChanged;

        public ScrollbarVertical(Form formowner)
            : base(formowner)
        {
            btnUp = new Button(formowner) { X = 0, Y = Height - btnHeight * 2, Width = Width, Height = btnHeight, Tag = (double)0 };
            btnDown = new Button(formowner) { X = 0, Y = Height - btnHeight, Width = Width, Height = btnHeight, Tag = (double)0 };
            innerScrollBarVertical = new InnerScrollbarVertical(formowner) { X = 0, Y = 0, Width = Width, Height = Height - (btnUp.Height + btnDown.Height)};

            btnUp.MouseLeftPressed += new EventHandler(btn_MouseLeftPressed);
            btnUp.MouseLeftDown += new EventHandler(btn_MouseLeftDown);
            btnUp.MouseLeftUp += new EventHandler(btn_MouseLeftUp);
            btnDown.MouseLeftPressed += new EventHandler(btn_MouseLeftPressed);
            btnDown.MouseLeftDown += new EventHandler(btn_MouseLeftDown);
            btnDown.MouseLeftUp += new EventHandler(btn_MouseLeftUp);
            btnUp.Click += new EventHandler(btn_Click);
            btnDown.Click += new EventHandler(btn_Click);
            innerScrollBarVertical.IndexChanged += new EventHandler(innerScrollBarVertical_IndexChanged);
            innerScrollBarVertical.WidthChanged += new EventHandler(innerScrollBarVertical_SizeChanged);
            innerScrollBarVertical.HeightChanged += new EventHandler(innerScrollBarVertical_SizeChanged);

            btnUp.Image = Theme.ArrowUp;
            btnDown.Image = Theme.ArrowDown;

            btnUp.Parent = btnDown.Parent = innerScrollBarVertical.Parent = this;

            Controls.Add(innerScrollBarVertical);
            Controls.Add(btnDown);
            Controls.Add(btnUp);

            this.Init += new EventHandler(ScrollbarVertical_Init);
        }

        void innerScrollBarVertical_SizeChanged(object sender, EventArgs e)
        {
            innerScrollBarVertical.ConfigureBar();
        }

        void btn_MouseLeftUp(object sender, EventArgs e)
        {
            ScrolledMouseLeftDown = false;
        }

        void btn_Click(object sender, EventArgs e)
        {
            if (!ScrolledMouseLeftDown)
                Index += sender == btnUp ? -1 : 1;
        }

        void btn_MouseLeftPressed(object sender, EventArgs e)
        {
            (sender as Button).Tag = (double)0;
            ScrolledMouseLeftDown = false;
        }

        void ScrollbarVertical_Init(object sender, EventArgs e)
        {
            InitInnerControls();
        }

        void btn_MouseLeftDown(object sender, EventArgs e)
        {
            Button btn = (sender as Button);

            if (TotalMilliseconds - (double)btn.Tag > MsToNextIndex)
            {
                Index += sender == btnUp ? -1 : 1;

                btn.Tag = TotalMilliseconds;
                ScrolledMouseLeftDown = true;
            }
        }

        void innerScrollBarVertical_IndexChanged(object sender, EventArgs e)
        {
            if (IndexChanged != null)
                IndexChanged(this, new EventArgs());
        }

        private void InitInnerControls()
        {
            btnUp.X = btnDown.X = innerScrollBarVertical.X = 0;
            btnUp.Width = btnDown.Width = innerScrollBarVertical.Width = Width;
            btnUp.Y = Height - btnHeight * 2;
            btnDown.Y = Height - btnHeight;
            innerScrollBarVertical.Y = 0;
            innerScrollBarVertical.Height = Height - btnHeight * 2;
            btnUp.Height = btnDown.Height = btnHeight;

            innerScrollBarVertical.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnUp.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnDown.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            innerScrollBarVertical.ConfigureBar();
        }

        public override void Update()
        {
            TotalMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;

            base.Update();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTK
{
    public class Scrollbar : Control
    {
        private int btnSize = 16;

        protected Button btnBack;
        protected Button btnNext;

        protected InnerScrollbar innerScrollBar;

        private int MsToNextIndex = 300;
        private double TotalMilliseconds = 0;

        private bool ScrolledMouseLeftDown;

        public ScrollBarType Type;

        public int TotalNrItems
        {
            get
            {
                return innerScrollBar.TotalNrItems;
            }
            set
            {
                innerScrollBar.TotalNrItems = value;
            }
        }

        public int NrItemsPerPage
        {
            get
            {
                return innerScrollBar.NrItemsPerPage;
            }
            set
            {
                innerScrollBar.NrItemsPerPage = value;
            }
        }

        public int Index
        {
            get
            {
                return innerScrollBar.Index;
            }
            set
            {
                innerScrollBar.Index = value;
            }
        }

        public double SizePerItem
        {
            get
            {
                return innerScrollBar.SizePerItem;
            }
            set
            {
                innerScrollBar.SizePerItem = value;
            }
        }

        public event EventHandler IndexChanged;

        public Scrollbar(Form formowner)
            : base(formowner)
        {
            btnBack = new Button(formowner);
            btnNext = new Button(formowner);
            innerScrollBar = new InnerScrollbar(formowner);


            btnBack.MouseLeftPressed += new EventHandler(btn_MouseLeftPressed);
            btnBack.MouseLeftDown += new EventHandler(btn_MouseLeftDown);
            btnBack.MouseLeftUp += new EventHandler(btn_MouseLeftUp);
            btnNext.MouseLeftPressed += new EventHandler(btn_MouseLeftPressed);
            btnNext.MouseLeftDown += new EventHandler(btn_MouseLeftDown);
            btnNext.MouseLeftUp += new EventHandler(btn_MouseLeftUp);
            btnBack.Click += new EventHandler(btn_Click);
            btnNext.Click += new EventHandler(btn_Click);
            innerScrollBar.IndexChanged += new EventHandler(innerScrollBar_IndexChanged);
            innerScrollBar.WidthChanged += new EventHandler(innerScrollBar_SizeChanged);
            innerScrollBar.HeightChanged += new EventHandler(innerScrollBar_SizeChanged);

            btnBack.Parent = btnNext.Parent = innerScrollBar.Parent = this;

            Controls.Add(innerScrollBar);
            Controls.Add(btnBack);
            Controls.Add(btnNext);

            this.Init += new EventHandler(Scrollbar_Init);
        }

        void innerScrollBar_SizeChanged(object sender, EventArgs e)
        {
            innerScrollBar.ConfigureBar();
        }

        void btn_MouseLeftUp(object sender, EventArgs e)
        {
            ScrolledMouseLeftDown = false;
        }

        void btn_Click(object sender, EventArgs e)
        {
            if (!ScrolledMouseLeftDown)
                Index += sender == btnBack ? -1 : 1;
        }

        void btn_MouseLeftPressed(object sender, EventArgs e)
        {
            (sender as Button).Tag = (double)0;
            ScrolledMouseLeftDown = false;
        }

        void Scrollbar_Init(object sender, EventArgs e)
        {
            if (Type == ScrollBarType.Horizontal)
            {
                btnBack.X = Width - btnSize * 2;
                btnBack.Y = 0;

                btnNext.X = Width - btnSize;
                btnNext.Y = 0;

                btnBack.Width = btnNext.Width = btnSize;
                btnBack.Height = btnNext.Height = Height;

                innerScrollBar.X = innerScrollBar.Y = 0;
                innerScrollBar.Width = Width - (btnBack.Width + btnNext.Width);
                innerScrollBar.Height = Height;
            }
            else
            {
                btnBack.X = btnNext.X = 0;
                btnBack.Y = Height - btnSize * 2;
                btnNext.Y = Height - btnSize;
                btnBack.Width = btnNext.Width = Width;
                btnBack.Height = btnNext.Height = btnSize;

                innerScrollBar.X = innerScrollBar.Y = 0;
                innerScrollBar.Width = Width;
                innerScrollBar.Height = Height - (btnBack.Height + btnNext.Height);
            }
            btnBack.Tag = btnNext.Tag = (double)0;
            innerScrollBar.Type = Type;

            btnBack.Image = Type == ScrollBarType.Horizontal ? Theme.ArrowLeft : Theme.ArrowUp;
            btnNext.Image = Type == ScrollBarType.Horizontal ? Theme.ArrowRight : Theme.ArrowDown;

            InitInnerControls();
        }

        void btn_MouseLeftDown(object sender, EventArgs e)
        {
            Button btn = (sender as Button);

            if (TotalMilliseconds - (double)btn.Tag > MsToNextIndex)
            {
                Index += sender == btnBack ? -1 : 1;

                btn.Tag = TotalMilliseconds;
                ScrolledMouseLeftDown = true;
            }
        }

        void innerScrollBar_IndexChanged(object sender, EventArgs e)
        {
            if (IndexChanged != null)
                IndexChanged(this, new EventArgs());
        }

        private void InitInnerControls()
        {
            if (Type == ScrollBarType.Horizontal)
            {
                btnBack.X = Width - btnSize * 2;
                btnNext.X = Width - btnSize;
                innerScrollBar.X = 0;
                btnBack.Width = btnSize;
                btnNext.Width = btnSize;
                innerScrollBar.Width = Width - btnSize * 2;
                btnBack.Y = 0;
                btnNext.Y = 0;
                innerScrollBar.Y = 0;
                innerScrollBar.Height = Height;
                btnBack.Height = btnNext.Height = Height;

                btnBack.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                btnNext.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            }
            else
            {
                btnBack.X = btnNext.X = innerScrollBar.X = 0;
                btnBack.Width = btnNext.Width = innerScrollBar.Width = Width;
                btnBack.Y = Height - btnSize * 2;
                btnNext.Y = Height - btnSize;
                innerScrollBar.Y = 0;
                innerScrollBar.Height = Height - btnSize * 2;
                btnBack.Height = btnNext.Height = btnSize;

                btnBack.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                btnNext.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            }

            innerScrollBar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            innerScrollBar.ConfigureBar();
        }

        public override void Update()
        {
            TotalMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;

            base.Update();
        }
    }
}

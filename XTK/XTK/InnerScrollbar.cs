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
    public class InnerScrollbar : Control
    {
        private int totalNrItems;
        private int nrItemsPerPage;
        private int index = 0;

        private double sizePerItem = 1;

        private bool mouseDown = false;
        private int sbPosWhenMouseDown = 0;
        private int mousePosWhenMouseDown = 0;
        private int indexWhenMouseDown = 0;

        private double TotalMilliseconds = 0;

        private double LastUpdateNextPage = 0;
        private int MsToNextPage = 300;

        public ScrollBarType Type;

        public int TotalNrItems
        {
            get
            {
                return totalNrItems;
            }
            set
            {
                if (totalNrItems != value)
                {
                    totalNrItems = value;

                    ConfigureBar();
                }
            }
        }

        public int NrItemsPerPage
        {
            get
            {
                return nrItemsPerPage;
            }
            set
            {
                if (nrItemsPerPage != value)
                {
                    nrItemsPerPage = value;

                    ConfigureBar();
                }
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                if (value != index)
                {
                    int _si = index;

                    index = value;

                    if (index < 0)
                        index = 0;
                    else
                    {
                        if (TotalNrItems >= NrItemsPerPage)
                        {
                            if (index > TotalNrItems - NrItemsPerPage)
                                index = TotalNrItems - NrItemsPerPage;
                        }
                        else
                            index = 0;
                    }

                    if (IndexChanged != null && _si != index)
                        IndexChanged(this, new EventArgs());

                    ConfigureBar();
                }
            }
        }

        public double SizePerItem
        {
            get
            {
                return sizePerItem;
            }
            set
            {
                if (value != sizePerItem)
                {
                    sizePerItem = value;

                    ConfigureBar();
                }
            }
        }

        public event EventHandler IndexChanged;

        Button Bar;

        public InnerScrollbar(Form formowner)
            : base(formowner)
        {
            Bar = new Button(formowner);

            Bar.FramePressed = Theme.Frame;
            Bar.MouseLeftPressed += new EventHandler(Bar_MouseLeftPressed);
            Bar.Parent = this;
            Bar.DepressImageOnPress = false;

            Controls.Add(Bar);
            this.Init += new EventHandler(InnerScrollbar_Init);
        }

        void InnerScrollbar_Init(object sender, EventArgs e)
        {
            Bar.Image = Type == ScrollBarType.Horizontal ? Theme.HorizontalHandle : Theme.VerticalHandle;
        }

        void Bar_MouseLeftPressed(object sender, EventArgs e)
        {
            if (!mouseDown)
            {
                sbPosWhenMouseDown = GetScrollBarPos();
                mousePosWhenMouseDown = GetMousePos();
                indexWhenMouseDown = Index;
                mouseDown = true;
            }
        }

        private int GetMousePos()
        {
            return Type == ScrollBarType.Horizontal ? WindowManager.MouseX : WindowManager.MouseY;
        }

        public override void Update()
        {
            TotalMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;

            if (!WindowManager.MouseLeftPressed)
                mouseDown = false;

            if (mouseDown)
                Index = (int)(indexWhenMouseDown + (GetMousePos() - mousePosWhenMouseDown) / SizePerItem);

            base.Update();
        }

        private int GetScrollBarSize()
        {
            int sbSize;
            int opposedSize = Type == ScrollBarType.Horizontal ? Width : Height;

            if (NrItemsPerPage > TotalNrItems)
                sbSize = opposedSize;
            else
                sbSize = (int)(opposedSize - (TotalNrItems - NrItemsPerPage) * SizePerItem);

            return sbSize;
        }

        private int GetScrollBarPos()
        {
            int sbPos;

            if (NrItemsPerPage > totalNrItems)
                sbPos = 0;
            else
                sbPos = (int)(Index * SizePerItem);

            return sbPos;
        }

        public void ConfigureBar()
        {
            int sizeUsed = Type == ScrollBarType.Horizontal ? Width : Height;

            if (sizeUsed > 0 && (TotalNrItems - NrItemsPerPage) * SizePerItem > Width)
                SizePerItem = (double)sizeUsed / (double)TotalNrItems;

            if (Type == ScrollBarType.Horizontal)
            {
                Bar.Width = GetScrollBarSize();
                Bar.Height = Height;
                Bar.X = GetScrollBarPos();
                Bar.Y = 0;
            }
            else
            {
                Bar.Width = Width;
                Bar.Height = GetScrollBarSize();
                Bar.X = 0;
                Bar.Y = GetScrollBarPos();
            }
        }

        public override bool ReceiveMessage(MessageEnum message, object msgTag)
        {
            switch (message)
            {
                case MessageEnum.MouseLeftDown:
                    if (Focused && MouseIsOnControl())
                    {
                        if (TotalMilliseconds - LastUpdateNextPage > MsToNextPage)
                        {
                            if (Type == ScrollBarType.Horizontal)
                            {
                                if ((WindowManager.MouseX < OwnerX + Bar.X) || (WindowManager.MouseX > OwnerX + Bar.X + Bar.Width))
                                    Index += ((WindowManager.MouseX < OwnerX + Bar.X) ? -1 : 1) * NrItemsPerPage;
                            }
                            else
                            {
                                if ((WindowManager.MouseY < OwnerY + Bar.Y) || (WindowManager.MouseY > OwnerY + Bar.Y + Bar.Height))
                                    Index += ((WindowManager.MouseY < OwnerY + Bar.Y) ? -1 : 1) * NrItemsPerPage;
                            }

                            LastUpdateNextPage = TotalMilliseconds;
                        }
                    }
                    break;
            }

            return base.ReceiveMessage(message, msgTag);
        }
    }

    public enum ScrollBarType
    {
        Horizontal = 0,
        Vertical = 1
    }
}

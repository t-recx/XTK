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
    public class InnerScrollbarHorizontal : Control
    {
        private int totalNrItems;
        private int nrItemsPerPage;
        private int index = 0;

        private double widthPerItem = 1;

        private bool mouseDown = false;
        private int sbXWhenMouseDown = 0;
        private int mouseXWhenMouseDown = 0;
        private int indexWhenMouseDown = 0;

        private double TotalMilliseconds = 0;

        private double LastUpdateNextPage = 0;
        private int MsToNextPage = 300;

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
                    else if (index > TotalNrItems - NrItemsPerPage)
                        index = TotalNrItems - NrItemsPerPage;
                        
                    if (IndexChanged != null && _si != index)
                        IndexChanged(this, new EventArgs());

                    ConfigureBar();
                }
            }
        }

        public double WidthPerItem
        {
            get
            {
                return widthPerItem;
            }
            set
            {
                if (value != widthPerItem)
                {
                    widthPerItem = value;

                    ConfigureBar();
                }
            }
        }

        public event EventHandler IndexChanged;

        Button Bar;

        public InnerScrollbarHorizontal(Form formowner)
            : base(formowner)
        {
            Bar = new Button(formowner);

            Bar.FramePressed = Theme.Frame;
            Bar.MouseLeftPressed += new EventHandler(Bar_MouseLeftPressed);
            Bar.Parent = this;
            Bar.DepressImageOnPress = false;

            Controls.Add(Bar);
            this.Init += new EventHandler(InnerScrollbarHorizontal_Init);
        }

        void InnerScrollbarHorizontal_Init(object sender, EventArgs e)
        {
            Bar.Image = Theme.HorizontalHandle;
        }

        void Bar_MouseLeftPressed(object sender, EventArgs e)
        {
            if (!mouseDown)
            {
                sbXWhenMouseDown = GetScrollBarX();
                mouseXWhenMouseDown = WindowManager.MouseX;
                indexWhenMouseDown = Index;
                mouseDown = true;
            }
        }

        public override void Update()
        {
            TotalMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;

            if (!WindowManager.MouseLeftPressed)
                mouseDown = false;

            if (mouseDown)
                Index = (int)(indexWhenMouseDown + (WindowManager.MouseX - mouseXWhenMouseDown) / WidthPerItem);

            base.Update();
        }
        
        private int GetScrollBarWidth()
        {
            int sbHeight;

            if (NrItemsPerPage > TotalNrItems)
                sbHeight = Width;
            else
                sbHeight = (int)(Width - (TotalNrItems - NrItemsPerPage) * WidthPerItem);

            return sbHeight;
        }

        private int GetScrollBarX()
        {
            int sbX;

            if (NrItemsPerPage > totalNrItems)
                sbX = 0;
            else
                sbX = (int)(Index * WidthPerItem);

            return sbX;
        }

        public void ConfigureBar()
        {
            if (Width > 0 && (TotalNrItems - NrItemsPerPage) * WidthPerItem > Width)
                WidthPerItem = (double)Width / (double)TotalNrItems;

            Bar.Width = GetScrollBarWidth();
            Bar.Height = Height;
            Bar.X = GetScrollBarX();
            Bar.Y = 0;
        }

        public override bool ReceiveMessage(MessageEnum message, object msgTag)
        {
            switch (message)
            {
                case MessageEnum.MouseLeftDown:
                    if (Focused && MouseIsOnControl() && ((WindowManager.MouseX < OwnerX + Bar.X) || (WindowManager.MouseX > OwnerX + Bar.X + Bar.Width)))
                    {
                        if (TotalMilliseconds - LastUpdateNextPage > MsToNextPage)
                        {
                            Index += ((WindowManager.MouseX < OwnerX + Bar.X) ? -1 : 1) * NrItemsPerPage;
                            LastUpdateNextPage = TotalMilliseconds;
                        }
                    }
                    break;
            }

            return base.ReceiveMessage(message, msgTag);
        }
    }
}

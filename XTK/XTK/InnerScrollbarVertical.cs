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
    public class InnerScrollbarVertical : Control
    {
        private int totalNrItems;
        private int nrItemsPerPage;
        private int index = 0;

        private double heightPerItem = 1;

        private bool mouseDown = false;
        private int sbYWhenMouseDown = 0;
        private int mouseYWhenMouseDown = 0;
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

        public double HeightPerItem
        {
            get
            {
                return heightPerItem;
            }
            set
            {
                if (value != heightPerItem)
                {
                    heightPerItem = value;

                    ConfigureBar();
                }
            }
        }

        public event EventHandler IndexChanged;

        Button Bar;

        public InnerScrollbarVertical(Form formowner)
            : base(formowner)
        {
            Bar = new Button(formowner);

            Bar.FramePressed = Theme.Frame;
            Bar.MouseLeftPressed += new EventHandler(Bar_MouseLeftPressed);
            Bar.Parent = this;
            Bar.DepressImageOnPress = false;

            Controls.Add(Bar);
            this.Init += new EventHandler(InnerScrollbarVertical_Init);
        }

        void InnerScrollbarVertical_Init(object sender, EventArgs e)
        {
            Bar.Image = Theme.VerticalHandle;
        }

        void Bar_MouseLeftPressed(object sender, EventArgs e)
        {
            if (!mouseDown)
            {
                sbYWhenMouseDown = GetScrollBarY();
                mouseYWhenMouseDown = WindowManager.MouseY;
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
                Index = (int)(indexWhenMouseDown + (WindowManager.MouseY - mouseYWhenMouseDown) / HeightPerItem);

            base.Update();
        }
        
        private int GetScrollBarHeight()
        {
            int sbHeight;

            if (NrItemsPerPage > TotalNrItems)
                sbHeight = Height;
            else
                sbHeight = (int)(Height - (TotalNrItems - NrItemsPerPage) * HeightPerItem);

            return sbHeight;
        }

        private int GetScrollBarY()
        {
            int sbY;

            if (NrItemsPerPage > totalNrItems)
                sbY = 0;
            else
                sbY = (int)(Index * HeightPerItem);

            return sbY;
        }

        public void ConfigureBar()
        {
            if (Height > 0 && (TotalNrItems - NrItemsPerPage) * HeightPerItem > Height)
                HeightPerItem = (double)Height / (double)TotalNrItems;

            Bar.Width = Width;
            Bar.Height = GetScrollBarHeight();
            Bar.X = 0;
            Bar.Y = GetScrollBarY();
        }

        public override bool ReceiveMessage(MessageEnum message, object msgTag)
        {
            switch (message)
            {
                case MessageEnum.MouseLeftDown:
                    if (Focused && MouseIsOnControl() && ((WindowManager.MouseY < OwnerY + Bar.Y) || (WindowManager.MouseY > OwnerY + Bar.Y + Bar.Height)))
                    {
                        if (TotalMilliseconds - LastUpdateNextPage > MsToNextPage)
                        {
                            Index += ((WindowManager.MouseY < OwnerY + Bar.Y) ? -1 : 1) * NrItemsPerPage;
                            LastUpdateNextPage = TotalMilliseconds;
                        }
                    }
                    break;
            }

            return base.ReceiveMessage(message, msgTag);
        }
    }
}

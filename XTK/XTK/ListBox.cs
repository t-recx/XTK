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
    public class ListBox : Control
    {
        protected ScrollbarHorizontal horizontalScrollbar;
        protected ScrollbarVertical verticalScrollbar;
        public InnerListBox innerListBox;

        private bool useVerticalScrollBar = true, useHorizontalScrollBar = false;

        private int MsToNextIndex = 150;
        private double LastUpdateIndexChange;

        public bool UseVerticalScrollBar
        {
            get
            {
                return useVerticalScrollBar;
            }
            set
            {
                useVerticalScrollBar = value;

                ConfigureCoordinatesAndSizes();
            }
        }

        public bool UseHorizontalScrollBar
        {
            get
            {
                return useHorizontalScrollBar;
            }
            set
            {
                useHorizontalScrollBar = value;

                ConfigureCoordinatesAndSizes();
            }
        }

        public object SelectedValue
        {
            get
            {
                return innerListBox.SelectedValue;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return innerListBox.SelectedIndex;
            }
            set
            {
                if (innerListBox.SelectedIndex != value)
                {
                    innerListBox.SelectedIndex = value;

                    if (SelectedIndexChanged != null)
                        SelectedIndexChanged(this, new EventArgs());
                }
            }
        }

        public List<ListBoxItem> Items
        {
            get
            {
                return innerListBox.Items;
            }
            set
            {
                innerListBox.Items = value;
                ConfigureCoordinatesAndSizes();
            }
        }

        public event EventHandler SelectedIndexChanged;
        public event EventHandler ListItemClick;

        public ListBox(Form formowner)
            : base(formowner)
        {
            horizontalScrollbar = new ScrollbarHorizontal(formowner);
            verticalScrollbar = new ScrollbarVertical(formowner);
            innerListBox = new InnerListBox(formowner);

            innerListBox.Click += new EventHandler(innerListBox_Click);

            horizontalScrollbar.Parent = verticalScrollbar.Parent = innerListBox.Parent = this;

            horizontalScrollbar.IndexChanged += new EventHandler(horizontalScrollbar_IndexChanged);
            verticalScrollbar.IndexChanged += new EventHandler(verticalScrollbar_IndexChanged);

            innerListBox.SelectedIndexChanged += new EventHandler(innerListBox_SelectedIndexChanged);

            this.Controls.Add(innerListBox);
            this.Controls.Add(horizontalScrollbar);
            this.Controls.Add(verticalScrollbar);

            this.Init += new EventHandler(ListBox_Init);
            this.WidthChanged += new EventHandler(ListBox_SizeChanged);
            this.HeightChanged += new EventHandler(ListBox_SizeChanged);
            this.KeyDown += new ControlKeyEventHandler(ListBox_KeyDown);
            innerListBox.KeyDown += new ControlKeyEventHandler(ListBox_KeyDown);
        }

        void innerListBox_Click(object sender, EventArgs e)
        {
            if (ListItemClick != null)
                ListItemClick(this, new EventArgs());
        }

        void innerListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, new EventArgs());
        }

        void ListBox_KeyDown(object sender, ControlKeyEventArgs e)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - LastUpdateIndexChange > MsToNextIndex)
            {
                if (e.Key == Keys.Down)
                    SelectedIndex++;
                if (e.Key == Keys.PageDown)
                    SelectedIndex += innerListBox.NrItemsVisible;
                if (e.Key == Keys.Up)
                    SelectedIndex--;
                if (e.Key == Keys.PageUp)
                    SelectedIndex -= innerListBox.NrItemsVisible;
                if (e.Key == Keys.Left)
                    horizontalScrollbar.Index--;
                if (e.Key == Keys.Right)
                    horizontalScrollbar.Index++;

                if (SelectedIndex < verticalScrollbar.Index)
                    verticalScrollbar.Index= SelectedIndex;
                if (SelectedIndex >= verticalScrollbar.Index + verticalScrollbar.NrItemsPerPage)
                    verticalScrollbar.Index= SelectedIndex - innerListBox.NrItemsVisible + 1;

                LastUpdateIndexChange = gameTime.TotalGameTime.TotalMilliseconds;
            }
        }

        void verticalScrollbar_IndexChanged(object sender, EventArgs e)
        {
            innerListBox.ScrollY = verticalScrollbar.Index;
        }

        void horizontalScrollbar_IndexChanged(object sender, EventArgs e)
        {
            innerListBox.ScrollX = horizontalScrollbar.Index;
        }

        void ListBox_SizeChanged(object sender, EventArgs e)
        {
            ConfigureCoordinatesAndSizes();
        }

        void ListBox_Init(object sender, EventArgs e)
        {
            ConfigureCoordinatesAndSizes();
        }

        public override void Draw()
        {
            Drawing.DrawFrame(spriteBatch, Theme.ListFrame, OwnerX + X, OwnerY + Y, Width, Height, Z - 0.001f);

            base.Draw();
        }

        public override void Update()
        {
            horizontalScrollbar.Z = verticalScrollbar.Z = Z - 0.0015f;
            innerListBox.Z = Z - 0.001f;

            base.Update();
        }

        public void ConfigureCoordinatesAndSizes()
        {
            innerListBox.X = 2;
            innerListBox.Y =  2;

            verticalScrollbar.X = Width - 16 - 2;
            verticalScrollbar.Y = 2;
            verticalScrollbar.Width = 16;
            verticalScrollbar.Height = Height - 4;
            horizontalScrollbar.X = 2;
            horizontalScrollbar.Y = Height - 16 - 2;
            horizontalScrollbar.Width = Width - 4;
            horizontalScrollbar.Height = 16;

            if (UseVerticalScrollBar)
                innerListBox.Width = Width - verticalScrollbar.Width - 4 - 1;
            else
                innerListBox.Width = Width - 4;

            if (UseHorizontalScrollBar)
                innerListBox.Height = Height - horizontalScrollbar.Height - 4 - 1;
            else
                innerListBox.Height = Height - 4;

            if (UseVerticalScrollBar && UseHorizontalScrollBar)
            {
                horizontalScrollbar.Width -= 16;
                verticalScrollbar.Height -= 16;
            }

            horizontalScrollbar.Visible = UseHorizontalScrollBar;
            verticalScrollbar.Visible = UseVerticalScrollBar;

            verticalScrollbar.NrItemsPerPage = innerListBox.NrItemsVisible;
            verticalScrollbar.TotalNrItems = innerListBox.Items.Count;

            if (Items != null && Items.Count > 0)
            {
                horizontalScrollbar.TotalNrItems = 0;
                horizontalScrollbar.NrItemsPerPage = 0;

                for (int i = 0; i < Items.Count; i++)
			    {
			        if (innerListBox.Items[i].Text.Length > horizontalScrollbar.TotalNrItems)
                        horizontalScrollbar.TotalNrItems = innerListBox.Items[i].Text.Length;
                    if (innerListBox.Items[i].NrCharsVisible > horizontalScrollbar.NrItemsPerPage)
                        horizontalScrollbar.NrItemsPerPage = innerListBox.Items[i].NrCharsVisible;
			    }
            }

            innerListBox.UpdateNrCharsVisible();
        }

        public void AddItem(ListBoxItem item)
        {
            innerListBox.AddItem(item);
            ConfigureCoordinatesAndSizes();
        }

        public void RemoveItem(ListBoxItem item)
        {
            innerListBox.RemoveItem(item);
            ConfigureCoordinatesAndSizes();
        }
    }
}

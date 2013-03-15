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
    public class TextBox : Control
    {
        protected Scrollbar horizontalScrollbar;
        protected Scrollbar verticalScrollbar;
        protected InnerTextBox innerTextBox;

        private bool useVerticalScrollBar = true, useHorizontalScrollBar = true;

        public int CursorX
        {
            get
            {
                return innerTextBox.CursorX;
            }
            set
            {
                innerTextBox.CursorX = value;
            }
        }

        public int CursorY
        {
            get
            {
                return innerTextBox.CursorY;
            }
            set
            {
                innerTextBox.CursorY = value;
            }
        }

        public bool UseVerticalScrollBar
        {
            get
            {
                if (!MultiLine)
                    return false;

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
                if (!MultiLine)
                    return false;

                return useHorizontalScrollBar;
            }
            set
            {
                useHorizontalScrollBar = value;

                ConfigureCoordinatesAndSizes();
            }
        }

        public bool MultiLine
        {
            get
            {
                return innerTextBox.MultiLine;
            }
            set
            {
                innerTextBox.MultiLine = value;
            }
        }

        public new string Text
        {
            get
            {
                return innerTextBox.Text;
            }
            set
            {
                innerTextBox.Text = value;
            }
        }

        public new event EventHandler TextChanged;
        public event TextChangingEventHandler TextChanging;

        public TextBox(Form formowner)
            : base(formowner)
        {
            horizontalScrollbar = new Scrollbar(formowner) { Type = ScrollBarType.Horizontal };
            verticalScrollbar = new Scrollbar(formowner) { Type = ScrollBarType.Vertical };
            innerTextBox = new InnerTextBox(formowner);

            horizontalScrollbar.Parent = verticalScrollbar.Parent = innerTextBox.Parent = this;

            horizontalScrollbar.IndexChanged += new EventHandler(horizontalScrollbar_IndexChanged);
            verticalScrollbar.IndexChanged += new EventHandler(verticalScrollbar_IndexChanged);
            innerTextBox.TextChanged += new EventHandler(innerTextBox_TextChanged);
            innerTextBox.ScrollChanged += new EventHandler(innerTextBox_ScrollChanged);
            innerTextBox.TextChanging += new TextChangingEventHandler(innerTextBox_TextChanging);
            
            this.Controls.Add(innerTextBox);
            this.Controls.Add(horizontalScrollbar);
            this.Controls.Add(verticalScrollbar);

            this.Init += new EventHandler(TextBox_Init);
            this.TextChanged += new EventHandler(TextBox_TextChanged);
            this.WidthChanged += new EventHandler(TextBox_SizeChanged);
            this.HeightChanged += new EventHandler(TextBox_SizeChanged);
        }

        void innerTextBox_TextChanging(object sender, TextChangingEventArgs e)
        {
            if (TextChanging != null)
                TextChanging(this, e);
        }

        void TextBox_SizeChanged(object sender, EventArgs e)
        {
            ConfigureInternalTextCounters();
        }

        void innerTextBox_ScrollChanged(object sender, EventArgs e)
        {
            horizontalScrollbar.Index = innerTextBox.ScrollX;
            verticalScrollbar.Index = innerTextBox.ScrollY;
        }

        void innerTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Initialized)
                ConfigureInternalTextCounters();
        }

        void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (TextChanged != null)
                TextChanged(this, new EventArgs());
        }

        void TextBox_Init(object sender, EventArgs e)
        {
            ConfigureCoordinatesAndSizes();
        }

        public override void Draw()
        {
            Drawing.DrawFrame(spriteBatch, Theme.TextBoxFrame, OwnerX + X,
                OwnerY + Y, Width, Height, Z - 0.001f);

            horizontalScrollbar.Z = verticalScrollbar.Z = Z - 0.0015f;
            innerTextBox.Z = Z - 0.001f;

            base.Draw();
        }

        private void ConfigureInternalTextCounters()
        {
            verticalScrollbar.NrItemsPerPage = innerTextBox.NrItemsVisible;
            verticalScrollbar.TotalNrItems = innerTextBox.NrLines;

            if (innerTextBox.internalText != null && innerTextBox.internalText.Count > 0)
            {
                horizontalScrollbar.TotalNrItems = 0;
                horizontalScrollbar.NrItemsPerPage = 0;

                for (int i = 0; i < innerTextBox.internalText.Count; i++)
                {
                    if (innerTextBox.internalText[i].Text.Length > horizontalScrollbar.TotalNrItems)
                        horizontalScrollbar.TotalNrItems = innerTextBox.internalText[i].Text.Length;
                    if (innerTextBox.internalText[i].NrCharsVisible > horizontalScrollbar.NrItemsPerPage)
                        horizontalScrollbar.NrItemsPerPage = innerTextBox.internalText[i].NrCharsVisible;
                }
            }
        }

        private void ConfigureCoordinatesAndSizes()
        {
            innerTextBox.X = 2;
            innerTextBox.Y = 2;
            innerTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            verticalScrollbar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            verticalScrollbar.X = Width - 16 - 2;
            verticalScrollbar.Y = 2;
            verticalScrollbar.Width = 16;
            verticalScrollbar.Height = Height - 4;
            horizontalScrollbar.X = 2;
            horizontalScrollbar.Y = Height - 16 - 2;
            horizontalScrollbar.Width = Width - 4;
            horizontalScrollbar.Height = 16;
            horizontalScrollbar.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            if (UseVerticalScrollBar)
                innerTextBox.Width = Width - verticalScrollbar.Width - 4 - 1;
            else
                innerTextBox.Width = Width - 4;

            if (UseHorizontalScrollBar)
                innerTextBox.Height = Height - horizontalScrollbar.Height - 4 - 1;
            else
                innerTextBox.Height = Height - 4;

            if (UseVerticalScrollBar && UseHorizontalScrollBar)
            {
                horizontalScrollbar.Width -= 16;
                verticalScrollbar.Height -= 16;
            }

            horizontalScrollbar.Visible = UseHorizontalScrollBar;
            verticalScrollbar.Visible = UseVerticalScrollBar;

            ConfigureInternalTextCounters();
        }

        void verticalScrollbar_IndexChanged(object sender, EventArgs e)
        {
            innerTextBox.ScrollY = verticalScrollbar.Index;
        }

        void horizontalScrollbar_IndexChanged(object sender, EventArgs e)
        {
            innerTextBox.ScrollX = horizontalScrollbar.Index;
        }
    }
}

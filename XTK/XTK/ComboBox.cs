using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XTK
{
    public class ComboBox : Control
    {
        protected InnerTextBox innerTextBox;
        protected ListBox listBox;
        protected Button btnExpand;

        protected int InitialWidth;
        protected int InitialHeight;

        protected int ListBoxHeight = 64;

        protected bool ShowingListBox = false;

        private int MsToNextIndex = 150;
        private double LastUpdateIndexChange;

        public object SelectedValue
        {
            get
            {
                return listBox.SelectedValue;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return listBox.SelectedIndex;
            }
            set
            {
                listBox.SelectedIndex = value;
            }
        }

        public List<ListBoxItem> Items
        {
            get
            {
                return listBox.Items;
            }
            set
            {
                listBox.Items = value;
            }
        }

        public event EventHandler SelectedIndexChanged;

        public ComboBox(Form formowner)
            : base(formowner)
        {
            btnExpand = new Button(formowner);
            listBox = new ListBox(formowner);
            innerTextBox = new InnerTextBox(formowner);
            btnExpand.Parent = listBox.Parent = innerTextBox.Parent = this;

            innerTextBox.MultiLine = false;
            btnExpand.Image = Theme.ComboButton;
            btnExpand.Click += new EventHandler(btnExpand_Click);

            listBox.ListItemClick += new EventHandler(listBox_ListItemClick);
            listBox.SelectedIndexChanged += new EventHandler(listBox_SelectedIndexChanged);
            listBox.KeyDown += new ControlKeyEventHandler(listBox_KeyDown);

            this.Controls.AddRange(new Control[] { btnExpand, listBox, innerTextBox });

            this.Init += new EventHandler(ComboBox_Init);
            this.WidthChanged += new EventHandler(ComboBox_SizeChanged);
            this.HeightChanged += new EventHandler(ComboBox_SizeChanged);
            this.KeyDown += new ControlKeyEventHandler(ComboBox_KeyDown);
            innerTextBox.KeyDown += new ControlKeyEventHandler(ComboBox_KeyDown);
        }

        void listBox_ListItemClick(object sender, EventArgs e)
        {
            SelectListItem();
        }

        void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, new EventArgs());

            //SelectListItem();
        }

        private void SelectListItem()
        {
            ToggleListBox(false);
            innerTextBox.Text = listBox.Items[listBox.SelectedIndex].Text;
            innerTextBox.ScrollX = 0;
            innerTextBox.CursorX = 0;
        }

        public override bool ReceiveMessage(MessageEnum message, object msgTag)
        {
            if (message == MessageEnum.MouseLeftPressed)
                if (!MouseIsOnControl(false))
                    ToggleListBox(false);

            return base.ReceiveMessage(message, msgTag);
        }

        void listBox_KeyDown(object sender, ControlKeyEventArgs e)
        {
            if (e.Key == Keys.Enter)
            {
                SelectListItem();
            }
        }

        void btnExpand_Click(object sender, EventArgs e)
        {
            btnExpand.Focused = false;

            ToggleListBox(!ShowingListBox);
        }

        void ComboBox_KeyDown(object sender, ControlKeyEventArgs e)
        {
            if (e.Key == Keys.Up || e.Key == Keys.Down)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds - LastUpdateIndexChange > MsToNextIndex)
                {
                    SelectedIndex += e.Key == Keys.Up ? -1 : 1;

                    LastUpdateIndexChange = gameTime.TotalGameTime.TotalMilliseconds;
                }
            }
        }

        void ComboBox_SizeChanged(object sender, EventArgs e)
        {
            if (!ShowingListBox)
            {
                InitialWidth = this.Width;
                InitialHeight = this.Height;
            }
        }

        void ComboBox_Init(object sender, EventArgs e)
        {
            ConfigureCoordinatesAndSizes();
        }

        public override void Draw()
        {
            Drawing.DrawFrame(spriteBatch, Theme.ComboFrame, OwnerX + X, OwnerY + Y, 
                InitialWidth, InitialHeight, Z - 0.001f);

            base.Draw();
        }

        public override void Update()
        {
            btnExpand.Z = Z - 0.0015f;
            listBox.Z = Z - 0.0015f;

            base.Update();
        }

        public void ToggleListBox(bool Show)
        {
            ShowingListBox = Show;

            listBox.Visible = Show;

            if (Show)
            {
                listBox.Y = InitialHeight;
                InitialHeight = Height;
                listBox.Focus();
                btnExpand.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                innerTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            }
            else
            {
                btnExpand.Anchor = AnchorStyles.Top | AnchorStyles.Left; 
                innerTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            }

            Height = (Show ? ListBoxHeight : 0) + InitialHeight;

            if (!Show)
            {
                btnExpand.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
                innerTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            }
        }

        private void ConfigureCoordinatesAndSizes()
        {
            listBox.X = 0;
            listBox.Y = InitialHeight;
            listBox.Width = InitialWidth;
            listBox.Height = ListBoxHeight;
            listBox.Visible = false;
            listBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            btnExpand.Y = 2;
            btnExpand.Height = InitialHeight - 4;
            btnExpand.Width = btnExpand.Height;
            btnExpand.X = InitialWidth - btnExpand.Width - 2;
            btnExpand.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;

            innerTextBox.Y = 2;
            innerTextBox.X = 2;
            innerTextBox.Width = InitialWidth - btnExpand.Width - 2 - 3;
            innerTextBox.Height = InitialHeight - 4;
            innerTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class InnerListBox : Control
    {
        public List<ListBoxItem> items = new List<ListBoxItem>();

        private int selectedIndex;

        private int scrollX, scrollY;

        private int nrItemsVisible = 0;

        private int fontHeight;

        private int TextXPadding = 4;

        public int NrItemsVisible
        {
            get
            {
                if (nrItemsVisible <= 0)
                    UpdateNrItemsVisible();

                return nrItemsVisible;
            }
            set
            {
                nrItemsVisible = value;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;

                    if (selectedIndex < 0)
                        selectedIndex = 0;
                    else if (Items != null && selectedIndex > Items.Count - 1)
                        selectedIndex = Items.Count - 1;

                    if (SelectedIndexChanged != null)
                        SelectedIndexChanged(this, new EventArgs());
                }
            }
        }

        public object SelectedValue
        {
            get
            {
                if (Items.Count > 0 && SelectedIndex < Items.Count)
                    return Items[SelectedIndex];

                return null;
            }
        }

        public int ScrollX
        {
            get
            {
                return scrollX;
            }
            set
            {
                scrollX = value;

                UpdateNrCharsVisible();
            }
        }

        public int ScrollY
        {
            get
            {
                return scrollY;
            }
            set
            {
                scrollY = value;
            }
        }

        private int FontHeight
        {
            get
            {
                if (fontHeight <= 0)
                    fontHeight = Drawing.GetFontHeight(Theme.Font);

                return fontHeight;
            }
        }

        public List<ListBoxItem> Items
        {
            get
            {
                return items;
            }
            set
            {
                if (value != items)
                {
                    items = value;
                    UpdateControlList();
                }
            }
        }

        public event EventHandler SelectedIndexChanged;

        public InnerListBox(Form formowner)
            : base(formowner)
        {
            this.WidthChanged += new EventHandler(InnerListBox_WidthChanged);
            this.HeightChanged += new EventHandler(InnerListBox_HeightChanged);
            this.Init += new EventHandler(InnerListBox_Init);
            this.Click += new EventHandler(InnerListBox_Click);
        }

        void InnerListBox_Click(object sender, EventArgs e)
        {
            for (int i = ScrollY; i < ScrollY + NrItemsVisible && i < Items.Count; i++)
            {
                if (WindowManager.MouseX >= X + OwnerX && WindowManager.MouseX <= X + OwnerX + Width &&
                    WindowManager.MouseY >= Y + OwnerY + (i - scrollY) * FontHeight &&
                    WindowManager.MouseY <= Y + OwnerY + (i - scrollY) * FontHeight + FontHeight)
                    SelectedIndex = i;
            }
        }

        void InnerListBox_Init(object sender, EventArgs e)
        {
            UpdateNrItemsVisible();
            UpdateNrCharsVisible();
        }

        void InnerListBox_HeightChanged(object sender, EventArgs e)
        {
            UpdateNrItemsVisible();
        }

        void InnerListBox_WidthChanged(object sender, EventArgs e)
        {
            UpdateNrCharsVisible();
        }
        
        public void UpdateNrCharsVisible()
        {
            if (Items != null)
                for (int i = 0; i < Items.Count; i++)
                {
                    string itemText = Items[i].Text;
                    int xPadding = TextXPadding + Items[i].TextXPadding;

                    if (itemText.Length - ScrollX <= 0)
                    {
                        Items[i].NrCharsVisible = 0;
                        continue;
                    }

                    itemText = itemText.Substring(ScrollX, itemText.Length - ScrollX);

                    Items[i].NrCharsVisible = itemText.Length;

                    if (Items[i].Ctrl != null)
                        xPadding += Items[i].Ctrl.Width;

                    while (Theme.Font.MeasureString(itemText.Substring(0, Items[i].NrCharsVisible)).X + xPadding > Width &&
                        Items[i].NrCharsVisible > 0)
                    {
                        if (Items[i].NrCharsVisible - 1 >= 0)
                        {
                            Items[i].NrCharsVisible--;
                            itemText = itemText.Substring(0, Items[i].NrCharsVisible);
                        }
                    }
                }
        }

        public override void Draw()
        {
            SetListBoxItemsControlVisibility(false);

            for (int i = ScrollY; i < ScrollY + NrItemsVisible && i < Items.Count; i++)
            {
                int itemXPadding = 0;
                string itemText = Items[i].Text;

                if (i == SelectedIndex)
                    Drawing.DrawRectangle(spriteBatch, Theme.SelectedListItemBackground, Color.White, X + OwnerX,
                        Y + OwnerY + (i - ScrollY) * FontHeight, Width, FontHeight, Z - 0.001f);

                if (itemText.Length < ScrollX)
                    continue;

                itemText = itemText.Substring(ScrollX, Items[i].NrCharsVisible);
                itemXPadding = TextXPadding + Items[i].TextXPadding;

                if (Items[i].Ctrl != null)
                {
                    Items[i].Ctrl.X = itemXPadding;
                    Items[i].Ctrl.Y = (i - ScrollY) * FontHeight + Items[i].Ctrl.Height / 2 - FontHeight / 2 + 3;
                    Items[i].Ctrl.Z = Z - 0.0016f;
                    Items[i].Ctrl.Visible = true;
                    itemXPadding += Items[i].Ctrl.Width;
                }

                Drawing.DrawString(spriteBatch, Theme.Font, itemText, X + OwnerX + itemXPadding,
                    Y + OwnerY + (i - ScrollY) * FontHeight, ForeColor, Z - 0.0015f);
            }

            base.Draw();
        }

        public void UpdateNrItemsVisible()
        {
            NrItemsVisible = (int)((double)Height / (double)FontHeight);
        }

        public void UpdateControlList()
        {
            if (Controls == null)
                Controls = new List<Control>();

            Controls.Clear();

            if (Items != null)
                foreach (ListBoxItem item in Items)
                {
                    if (item.Ctrl != null && !Controls.Contains(item.Ctrl))
                        Controls.Add(item.Ctrl);
                }
        }

        private void SetListBoxItemsControlVisibility(bool visible)
        {
            foreach (ListBoxItem item in Items)
            {
                if (item.Ctrl != null)
                    item.Ctrl.Visible = visible;
            }
        }

        public void AddItem(ListBoxItem item)
        {
            Items.Add(item);

            if (item.Ctrl != null)
                Controls.Add(item.Ctrl);
        }

        public void RemoveItem(ListBoxItem item)
        {
            if (item.Ctrl != null)
                Controls.Remove(item.Ctrl);

            Items.Remove(item);
        }
    }

    public class ListBoxItem
    {
        public object Value;
        public string Text;
        public int NrCharsVisible = -1;
        public int TextXPadding = 0;
        public Control Ctrl = null;
        public object tag = null;

        public ListBoxItem(object value, string text, int textxpadding, Control ctrl)
        {
            Value = value;
            Text = text;
            TextXPadding = textxpadding;
            Ctrl = ctrl;
        }

        public ListBoxItem()
        {

        }
    }
}

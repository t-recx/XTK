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
    public class CheckedListBox : ListBox
    {
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
                ConfigureCheckBoxes();
                innerListBox.UpdateControlList();
            }
        }

        public void AddItem(ListBoxItem item, bool _checked)
        {
            item.Ctrl = GetNewCheckBox(_checked);

            innerListBox.AddItem(item);
            ConfigureCoordinatesAndSizes();
        }

        public void RemoveItem(ListBoxItem item)
        {
            innerListBox.RemoveItem(item);
            ConfigureCoordinatesAndSizes();
        }

        public void SetItemCheck(ListBoxItem item, bool _checked)
        {
            if (item.Ctrl != null && item.Ctrl is CheckBox)
                (item.Ctrl as CheckBox).Checked = _checked;
        }

        public List<ListBoxItem> GetCheckedItems()
        {
            return Items.FindAll(li => li.Ctrl != null && li.Ctrl is CheckBox && (li.Ctrl as CheckBox).Checked);
        }

        private CheckBox GetNewCheckBox(bool _checked)
        {
            return new CheckedListBoxCheckBox(Owner) { Parent = innerListBox.Parent, Width = Drawing.GetFontHeight(Font), Height = Drawing.GetFontHeight(Font), Checked = _checked };
        }

        private void ConfigureCheckBoxes()
        {
            foreach (ListBoxItem item in Items)
                item.Ctrl = GetNewCheckBox(false);
        }

        public CheckedListBox(Form formowner) 
            : base(formowner)
        {

        }
    }

    public class CheckedListBoxCheckBox : CheckBox
    {
        protected override Texture2D GetControlImage()
        {
            if (Checked)
                return Theme.CheckedListBoxCheckBoxChecked;
            else
                return Theme.CheckedListBoxCheckBoxUnchecked;
        }

        public CheckedListBoxCheckBox(Form formowner)
            : base(formowner)
        {

        }
    }
}

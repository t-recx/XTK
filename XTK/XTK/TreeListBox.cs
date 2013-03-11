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
    public class TreeListBox : Control
    {
        private ListBox lstBox;
        private List<TreeListBoxItem> items;

        public List<TreeListBoxItem> Items
        {
            get
            {
                return items;
            }
            set
            {
                if (items != value)
                {
                    items = value;
                    TranslateToLstBox();
                }
            }
        }

        public object SelectedValue
        {
            get
            {
                return lstBox.SelectedValue;
            }
        }

        public int SelectedIndex
        {
            get
            {
                if (Items != null && lstBox.Items != null && lstBox.Items.Count > 0 &&
                    lstBox.SelectedIndex >= 0 && 
                    lstBox.SelectedIndex < lstBox.Items.Count)
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (lstBox.Items[lstBox.SelectedIndex].tag is TreeListBoxItem &&
                            lstBox.Items[lstBox.SelectedIndex].tag == Items[i])
                            return i;
                    }
                }

                return -1;
            }
        }

        public bool UseVerticalScrollBar
        {
            get
            {
                return lstBox.UseVerticalScrollBar;
            }
            set
            {
                lstBox.UseVerticalScrollBar = value;

                lstBox.ConfigureCoordinatesAndSizes();
            }
        }

        public bool UseHorizontalScrollBar
        {
            get
            {
                return lstBox.UseHorizontalScrollBar;
            }
            set
            {
                lstBox.UseHorizontalScrollBar = value;

                lstBox.ConfigureCoordinatesAndSizes();
            }
        }

        public event EventHandler SelectedIndexChanged;

        public TreeListBox(Form formowner) 
            : base(formowner)
        {
            lstBox = new ListBox(formowner) { Parent = this, Width = Width, Height = Height };
            lstBox.SelectedIndexChanged += new EventHandler(lstBox_SelectedIndexChanged);

            Controls.Add(lstBox);

            this.WidthChanged += new EventHandler(TreeListBox_SizeChanged);
            this.HeightChanged += new EventHandler(TreeListBox_SizeChanged);
        }

        void lstBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, new EventArgs());
        }

        void TreeListBox_SizeChanged(object sender, EventArgs e)
        {
            lstBox.Width = Width;
            lstBox.Height = Height;
            lstBox.ConfigureCoordinatesAndSizes();
        }

        private void TranslateToLstBox()
        {
            lstBox.Items = GetListBoxItems(Items, null, 4);
            lstBox.ConfigureCoordinatesAndSizes();
        }

        private Control GetExpandCollapseButton(TreeListBoxItem tLBIOwner)
        {
            CheckBox chkExpCol = null;

            if (Items.Exists(li => li.Parent == tLBIOwner))
            {
                chkExpCol = new CheckBox(Owner) { Width = 12, Height = 12, Tag = tLBIOwner, Parent = lstBox.innerListBox };

                chkExpCol.CheckBoxUnchecked = Theme.TreeListBoxExpand;
                chkExpCol.CheckBoxChecked = Theme.TreeListBoxCollapse;

                chkExpCol.Checked = tLBIOwner.Expanded;

                chkExpCol.CheckStateChanged += new EventHandler(chkExpCol_CheckStateChanged);
            }

            return chkExpCol;
        }

        void chkExpCol_CheckStateChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Tag != null && (sender as CheckBox).Tag is TreeListBoxItem)
            {
                TreeListBoxItem tLBI = ((sender as CheckBox).Tag as TreeListBoxItem);

                tLBI.Expanded = !tLBI.Expanded;

                TranslateToLstBox();
            }
        }

        private List<ListBoxItem> GetListBoxItems(List<TreeListBoxItem> treeItems, TreeListBoxItem parent, int textXPadding)
        {
            List<ListBoxItem> lstItems = new List<ListBoxItem>();

            foreach (TreeListBoxItem treeItem in treeItems)
            {
                if (treeItem.Parent == parent)
                {
                    lstItems.Add(new ListBoxItem(treeItem.Value, treeItem.Text, textXPadding, GetExpandCollapseButton(treeItem)));
                    lstItems[lstItems.Count - 1].tag = treeItem;

                    if (treeItem.Expanded)
                    {
                        List<ListBoxItem> lstSub = GetListBoxItems(treeItems, treeItem, textXPadding + textXPadding +
                            lstItems[lstItems.Count - 1].Ctrl.Width);
                        lstItems.AddRange(lstSub);
                    }
                }
            }

            return lstItems;
        }

        public void AddItem(TreeListBoxItem item)
        {
            Items.Add(item);
            TranslateToLstBox();
            lstBox.ConfigureCoordinatesAndSizes();
        }

        public void RemoveItem(TreeListBoxItem item)
        {
            Items.Remove(item);
            lstBox.Items.Remove(lstBox.Items.Find(li => li.tag == item));
            lstBox.ConfigureCoordinatesAndSizes();
        }
    }

    public class TreeListBoxItem : ListBoxItem
    {
        private TreeListBoxItem _parent;

        public bool Expanded = false;

        public TreeListBoxItem Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (_parent != value)
                    _parent = value;
            }
        }

        public TreeListBoxItem(object value, string text, TreeListBoxItem parent, bool expanded)
        {
            Value = value;
            Text = text;
            Parent = parent;
            Expanded = expanded;
        }
    }
}

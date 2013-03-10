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
using XTK;

namespace XTKExample
{
    class frmListBoxes : Form
    {
        Label lblTreeList;
        TreeListBox treeListBox;

        Label lblCheckedList;
        CheckedListBox checkedListBox;

        Label lblList;
        ListBox listBox;

        private List<TreeListBoxItem> GetTestItemList()
        {
            List<TreeListBoxItem> lst = new List<TreeListBoxItem>();

            for (int i = 0; i < 16; i++)
            {
                lst.Add(new TreeListBoxItem(i, "Item #" + (i + 1), null, false));

                TreeListBoxItem Parent = lst[lst.Count - 1];

                for (int x = 0; x < 2; x++)
                {
                    lst.Add(new TreeListBoxItem(x, "SubItem " + (char)(x + 65), Parent, false));
                    TreeListBoxItem SubParent = lst[lst.Count - 1];

                    for (int y = 0; y < 3; y++)
                    {
                        lst.Add(new TreeListBoxItem(x, "SubSubItem " + y+1, SubParent, false));
                    }
                }
            }

            return lst;
        }

        private List<ListBoxItem> GetItemList()
        {
            List<ListBoxItem> lst = new List<ListBoxItem>();

            for (int i = 0; i < 16; i++)
                lst.Add(new ListBoxItem(i, "Item #" + (i + 1), 0, null));

            return lst;
        }

        public frmListBoxes(WindowManager windowmanager)
            : base(windowmanager)
        {
            WindowManager = windowmanager;

            lblTreeList = new Label(this) { X = 10 , Y = 5, Height = 10, Width = 100, Text = "TreeListBox", BackColor = new Color(170, 170, 170) };
            treeListBox = new TreeListBox(this) { X = 10, Y = 20, Height = 100, Width = 100, Items = GetTestItemList() };

            Controls.AddRange(new Control[] { lblTreeList, treeListBox});

            lblCheckedList = new Label(this) { X = 10 + 110, Y = 5, Height = 10, Width = 100, Text = "CheckListBox", BackColor = new Color(170, 170, 170) };
            checkedListBox = new CheckedListBox(this) { X = 10 + 110, Y = 20, Height = 100, Width = 100, Items = GetItemList() };

            Controls.AddRange(new Control[] { lblCheckedList, checkedListBox });

            lblList = new Label(this) { X = 10 + 220, Y = 5, Height = 10, Width = 100, Text = "ListBox", BackColor = new Color(170, 170, 170) };
            listBox = new ListBox(this) { X = 10 + 220, Y = 20, Height = 100, Width = 100, Items = GetItemList() };

            Controls.AddRange(new Control[] { lblList, listBox });
        }
    }
}

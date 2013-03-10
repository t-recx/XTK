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
    public class frmTest : Form
    {
        Button btnOK, btnCancel;
        CheckBox chkOption1, chkOption2, chkOption3;
        RadioButton rdbOption1, rdbOption2, rdbOption3;
        GroupBox grpChecks, grpRadios;
        Label lblCombo;
        ComboBox cmbTest;
        Label lblList;
        ListBox lstTest;
        Label lblNUD;
        NumericUpDown nudTest;

        private List<ListBoxItem> GetTestItemList()
        {
            List<ListBoxItem> lst = new List<ListBoxItem>();

            for (int i = 0; i < 16; i++)
                lst.Add(new ListBoxItem(i, "Item #" + (i + 1), 0, null));

            return lst;
        }

        public frmTest(WindowManager windowmanager) : base(windowmanager)
        {
            WindowManager = windowmanager;

            btnOK = new Button(this) { X = 230 - 40 - 80 - 10, Y = 240 - 25, Width = 60, Height = 20, Text = "Okay", Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            btnCancel = new Button(this) { X = 230 - 40 - 20 - 10, Y = 240 - 25, Width = 60, Height = 20, Text = "Cancel", Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            btnOK.Click += new EventHandler(btnOK_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            Controls.AddRange(new Control[] { btnOK, btnCancel });

            grpChecks = new GroupBox(this) { X = 10, Y = 10, Width = 100, Height = 90, Text = "CheckBoxes" };

            chkOption1 = new CheckBox(this) { X = 10, Y = 20, Width = 60, Height = 20, Text = "Check #1", Parent = grpChecks };
            chkOption2 = new CheckBox(this) { X = 10, Y = 40, Width = 60, Height = 20, Text = "Check #2", Parent = grpChecks };
            chkOption3 = new CheckBox(this) { X = 10, Y = 60, Width = 60, Height = 20, Text = "Check #3", Parent = grpChecks };

            grpChecks.Controls.AddRange(new Control[] { chkOption1, chkOption2, chkOption3 });

            Controls.Add(grpChecks);

            grpRadios = new GroupBox(this) { X = 20 + 100, Y = 10, Width = 100, Height = 90, Text = "RadioButtons" };

            rdbOption1 = new RadioButton(this) { X = 10, Y = 20, Width = 60, Height = 20, Text = "Radio #1", Parent = grpRadios };
            rdbOption2 = new RadioButton(this) { X = 10, Y = 40, Width = 60, Height = 20, Text = "Radio #2", Parent = grpRadios };
            rdbOption3 = new RadioButton(this) { X = 10, Y = 60, Width = 60, Height = 20, Text = "Radio #3", Parent = grpRadios };

            grpRadios.Controls.AddRange(new Control[] { rdbOption1, rdbOption2, rdbOption3 });

            Controls.Add(grpRadios);

            lblCombo = new Label(this) { X = 10, Y = 105, Height = 10, Width = 100, Text = "ComboBox", BackColor = new Color(170, 170, 170) };
            cmbTest = new ComboBox(this) { X = 10, Y = 120, Height = 20, Width = 100, Items = GetTestItemList() };

            Controls.AddRange(new Control[] { lblCombo, cmbTest });

            lblList = new Label(this) { X = 10 + 110, Y = 105, Height = 10, Width = 100, Text = "ListBox", BackColor = new Color(170, 170, 170) };
            lstTest = new ListBox(this) { X = 10 + 110, Y = 120, Height = 84, Width = 100, Items = GetTestItemList() };

            Controls.AddRange(new Control[] { lblList, lstTest });

            lblNUD = new Label(this) { X = 10, Y = 140, Height = 10, Width = 100, Text = "NumericUpDown", BackColor = new Color(170, 170, 170) };
            nudTest = new NumericUpDown(this) { X = 10, Y = 155, Height = 20, Width = 100 };

            Controls.AddRange(new Control[] { lblNUD, nudTest });
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            lstTest.RemoveItem(lstTest.Items[lstTest.Items.Count - 1]);
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            lstTest.AddItem(new ListBoxItem(0, "kfldjlk", 0, new CheckBox(this) { Parent = lstTest, Width = 15, Height = 15 }));
        }
    }
}

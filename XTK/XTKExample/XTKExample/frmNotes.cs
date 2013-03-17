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
    public class frmNotes : Form
    {
        Tab tabDocuments;
        TabPage tabPage1;
        TabPage tabPage2;
        TextBox txtTest;

        public frmNotes(WindowManager windowmanager)
            : base(windowmanager)
        {
            WindowManager = windowmanager;

            tabDocuments = new Tab(this)
            {
                X = 10,
                Y = 10,
                Width = 300,
                Height = 220,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            tabPage1 = new TabPage(this)
            {
                Parent = tabDocuments,
                Text = "Document #1"
            };

            tabPage2 = new TabPage(this)
            {
                Parent = tabDocuments,
                Text = "Document #2"
            };

            tabDocuments.TabPages.Add(tabPage1);
            tabDocuments.TabPages.Add(tabPage2);
 
            txtTest = new TextBox(this) { X = 10, Y = 10, Width = 280, Height = 180, MultiLine = true, Parent = tabPage1,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top };

            tabPage1.Controls.Add(txtTest);

            Controls.Add(tabDocuments);
        }
    }
}

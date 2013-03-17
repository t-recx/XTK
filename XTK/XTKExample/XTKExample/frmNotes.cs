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

        private void CreateTabPages()
        {
            for (int i = 0; i < 5; i++)
            {
                TabPage tabPage = new TabPage(this)
                {
                    Parent = tabDocuments,
                    Text = "Document #" + (i + 1)
                };

                TextBox txtPage = new TextBox(this)
                {
                    X = 10,
                    Y = 10,
                    Width = 280,
                    Height = 180,
                    MultiLine = true,
                    Parent = tabPage,
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
                };

                tabPage.Controls.Add(txtPage);

                tabDocuments.TabPages.Add(tabPage);
            }
        }

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

            CreateTabPages();

            Controls.Add(tabDocuments);
        }
    }
}

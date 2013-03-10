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
        TextBox txtTest;

        public frmNotes(WindowManager windowmanager)
            : base(windowmanager)
        {
            WindowManager = windowmanager;

            txtTest = new TextBox(this) { X = 0, Y = 0, Width = 320, Height = 240, MultiLine = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top };

            Controls.Add(txtTest);
        }
    }
}

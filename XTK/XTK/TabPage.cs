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
    public class TabPage : Control
    {
        private bool selected = false;
        public int TextPadding = 14;

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    Visible = value;

                    if (SelectedChanged != null)
                        SelectedChanged(this, new EventArgs());
                }
            }
        }

        public event EventHandler SelectedChanged;

        public TabPage(Form formowner)
            : base(formowner)
        {
            Visible = Selected;
        }
    }
}

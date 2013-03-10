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

namespace XTK
{
    public class CheckBox : Button
    {
        private Texture2D checkBoxChecked = null;
        private Texture2D checkBoxUnchecked = null;

        private bool _checked = false;

        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                if (_checked != value)
                {
                    _checked = value;

                    if (CheckStateChanged != null)
                        CheckStateChanged(this, new EventArgs());
                }
            }
        }

        public Texture2D CheckBoxChecked
        {
            get
            {
                if (checkBoxChecked == null)
                    return Theme.CheckBoxChecked;

                return checkBoxChecked;
            }
            set
            {
                checkBoxChecked = value;
            }
        }

        public Texture2D CheckBoxUnchecked
        {
            get
            {
                if (checkBoxUnchecked == null)
                    return Theme.CheckBoxUnchecked;

                return checkBoxUnchecked;
            }
            set
            {
                checkBoxUnchecked = value;
            }
        }

        public event EventHandler CheckStateChanged;

        public CheckBox(Form formowner)
            : base(formowner)
        {
            this.Click += new EventHandler(CheckBox_Click);
        }

        void CheckBox_Click(object sender, EventArgs e)
        {
            Checked = !Checked;
        }

        protected virtual Texture2D GetControlImage()
        {
            if (Checked)
                return CheckBoxChecked;
            else
                return CheckBoxUnchecked;
        }

        public override void Draw()
        {
            Drawing.Draw(spriteBatch, GetControlImage(), new Vector2(OwnerX + X, OwnerY + Y), Color.White, Z);
            Drawing.DrawTextShadow(spriteBatch, Font, Text, OwnerX + X + GetControlImage().Width + 1, OwnerY + Y + GetControlImage().Height / 2 - Drawing.GetFontHeight(Font) / 2, ForeColor, null, Z);
            //base.Draw();
        }
    }
}

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
    public class RadioButton : CheckBox
    {
        public RadioButton LastCheckedRadioButton = null;

        public RadioButton(Form formowner)
            : base(formowner)
        {
            this.CheckStateChanged += new EventHandler(RadioButton_CheckStateChanged);
        }

        void RadioButton_CheckStateChanged(object sender, EventArgs e)
        {
            if (LastCheckedRadioButton == this && !Checked)
                Checked = true;

            if (Parent != null && Parent.Controls != null && Parent.Controls.Count > 0 && Checked)
            {
                foreach (Control ctrl in Parent.Controls)
                {
                    if (ctrl is RadioButton)
                    {
                        (ctrl as RadioButton).LastCheckedRadioButton = this;

                        if (ctrl != this)
                            (ctrl as RadioButton).Checked = false;
                    }
                }
            }
        }

        protected override Texture2D GetControlImage()
        {
            if (Checked)
                return Theme.RadioButtonChecked;
            else
                return Theme.RadioButtonUnchecked;
        }
    }
}

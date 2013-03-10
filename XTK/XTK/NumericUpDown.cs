using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XTK
{
    public class NumericUpDown : Control
    {
        protected InnerTextBox innerTextBox;
        protected Button btnUp;
        protected Button btnDown;

        private decimal _value = 0;
        private decimal increment = 1;
        private decimal maximum = 100;
        private decimal minimum = 0;

        private int MsToNextIndex = 150;
        private double TotalMilliseconds = 0;

        private string previousText;

        public decimal Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                if (maximum != value)
                {
                    maximum = value;

                    CheckValueBoundaries();
                }
            }
        }

        public decimal Minimum
        {
            get
            {
                return minimum;
            }
            set
            {
                if (minimum != value)
                {
                    minimum = value;

                    CheckValueBoundaries();
                }
            }
        }

        public decimal Increment
        {
            get
            {
                return increment;
            }
            set
            {
                if (increment != value)
                    increment = value;
            }
        }

        public decimal Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;

                    CheckValueBoundaries();

                    if (ValueChanged != null)
                        ValueChanged(this, new EventArgs());

                    UpdateTextBox();
                }
            }
        }

        public event EventHandler ValueChanged;

        public NumericUpDown(Form formowner)
            : base(formowner)
        {
            btnUp = new Button(formowner);
            btnDown = new Button(formowner);
            innerTextBox = new InnerTextBox(formowner);
            btnUp.Parent = btnDown.Parent = innerTextBox.Parent = this;

            innerTextBox.MultiLine = false;
            btnUp.Image = Theme.NumericUpDownUpButton;
            btnDown.Image = Theme.NumericUpDownDownButton;

            btnUp.MouseLeftDown += new EventHandler(btn_MouseLeftDown);
            btnDown.MouseLeftDown += new EventHandler(btn_MouseLeftDown);
            btnUp.MouseLeftPressed += new EventHandler(btn_MouseLeftPressed);
            btnDown.MouseLeftPressed += new EventHandler(btn_MouseLeftPressed);

            this.Controls.AddRange(new Control[] { btnUp, btnDown, innerTextBox });

            this.Init += new EventHandler(NumericUpDown_Init);

            innerTextBox.TextChanged += new EventHandler(innerTextBox_TextChanged);
        }

        void innerTextBox_TextChanged(object sender, EventArgs e)
        {
            string t = innerTextBox.Text.Replace(Environment.NewLine, "");

            if (previousText != t)
            {
                if (t.Trim().Length > 0)
                {
                    if (!Regex.Match(t, @"^[-+]?\d+(,\d+)?$").Success)
                        UpdateTextBox();
                    else
                        Value = Convert.ToDecimal(t);
                }

                previousText = t;
            }
        }

        void btn_MouseLeftPressed(object sender, EventArgs e)
        {
            (sender as Button).Tag = (double)0;
        }

        private void UpdateTextBox()
        {
            innerTextBox.Text = Value.ToString();
        }

        private void CheckValueBoundaries()
        {
            if (Value > Maximum) Value = Maximum;
            if (Value < Minimum) Value = Minimum;
        }

        void btn_MouseLeftDown(object sender, EventArgs e)
        {
            Button btn = (sender as Button);

            if (TotalMilliseconds - (double)btn.Tag > MsToNextIndex)
            {
                Value += Increment * (sender == btnDown ? -1 : 1);

                btn.Tag = TotalMilliseconds;
            }
        }

        void NumericUpDown_Init(object sender, EventArgs e)
        {
            ConfigureCoordinatesAndSizes();
            UpdateTextBox();
        }

        public override void Draw()
        {
            Drawing.DrawFrame(spriteBatch, Theme.NumericUpDownFrame, OwnerX + X, OwnerY + Y, 
                Width, Height, Z - 0.001f);

            base.Draw();
        }

        public override void Update()
        {
            TotalMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;

            btnUp.Z = Z - 0.0015f;
            btnDown.Z = Z - 0.0015f;

            base.Update();
        }

        private void ConfigureCoordinatesAndSizes()
        {
            btnUp.Y = 2;
            btnUp.Height = (Height - 4) / 2;
            btnUp.Width = btnUp.Height * 2;
            btnUp.X = Width - btnUp.Width - 2;
            btnUp.Anchor = AnchorStyles.Right;

            btnDown.Y = 2 + btnUp.Height;
            btnDown.Height = (Height - 4) / 2;
            btnDown.Width = btnDown.Height * 2;
            btnDown.X = Width - btnDown.Width - 2;
            btnDown.Anchor = AnchorStyles.Right;

            innerTextBox.Y = 2;
            innerTextBox.X = 2;
            innerTextBox.Width = Width - btnUp.Width - 2 - 3;
            innerTextBox.Height = Height - 4;
            innerTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
        }
    }
}

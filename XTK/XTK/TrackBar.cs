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
    public class TrackBar : Control
    {
        private TrackBarOrientation orientation;
        public int TickFrequency = 1;
        public TickStyle tickStyle = TickStyle.BottomRight;
        public int SmallChange = 1;
        public int LargeChange = 5;
        public int minimum = 0;
        public int maximum = 10;
        private int _value;

        private int sliderSize = 11;
        private int tickSize = 4;

        protected Button btnSlider;

        private bool mouseDown = false;
        private int sbPosWhenMouseDown = 0;
        private int mousePosWhenMouseDown = 0;
        private int valueWhenMouseDown = 0;

        private double TotalMilliseconds = 0;

        private int MsToNextIndex = 150;
        private double LastUpdateIndexChange;

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                CapValue();

                SetSliderPosAccordingToValue();

                if (ValueChanged != null)
                    ValueChanged(this, new EventArgs());
            }
        }

        public int Minimum
        {
            get
            {
                return minimum;
            }
            set
            {
                minimum = value;

                CapValue();
            }
        }

        public int Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                maximum = value;

                CapValue();
            }
        }

        public TrackBarOrientation Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;

                ConfigureSlider();
            }
        }

        public TickStyle TickStyle
        {
            get
            {
                return tickStyle;
            }
            set
            {
                tickStyle = value;

                ConfigureSlider();
            }
        }

        public int TickSize
        {
            get
            {
                return tickSize;
            }
            set
            {
                tickSize = value;

                ConfigureSlider();
            }
        }

        public DrawTickRoutine AlternativeTickDraw;
        public event EventHandler ValueChanged;

        public TrackBar(Form formowner)
            : base(formowner)
        {
            btnSlider = new Button(formowner);
            btnSlider.Parent = this;
            btnSlider.MouseLeftPressed += new EventHandler(btnSlider_MouseLeftPressed);
            btnSlider.Frame = btnSlider.FramePressed = Theme.SliderButtonFrame;
            btnSlider.KeyDown += new ControlKeyEventHandler(TrackBar_KeyDown);
            Controls.Add(btnSlider);

            this.WidthChanged += new EventHandler(TrackBar_SizeChanged);
            this.HeightChanged += new EventHandler(TrackBar_SizeChanged);
            this.KeyDown += new ControlKeyEventHandler(TrackBar_KeyDown);
            this.MouseLeftDown += new EventHandler(TrackBar_MouseLeftDown);

            ConfigureSlider();
        }

        int ChangeValueMouseDown;
        bool mouseDownLargeChange = false;

        void TrackBar_MouseLeftDown(object sender, EventArgs e)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - LastUpdateIndexChange > MsToNextIndex)
            {
                if (!mouseDownLargeChange)
                {
                    if (Orientation == TrackBarOrientation.Horizontal)
                    {
                        if (WindowManager.MouseX < OwnerX + X + btnSlider.X)
                            ChangeValueMouseDown = -LargeChange;
                        else if (WindowManager.MouseX > OwnerX + X + btnSlider.X + btnSlider.Width)
                            ChangeValueMouseDown = LargeChange;
                    }
                    else if (Orientation == TrackBarOrientation.Vertical)
                    {
                        if (WindowManager.MouseY < OwnerY + Y + btnSlider.Y)
                            ChangeValueMouseDown = LargeChange;
                        else if (WindowManager.MouseY > OwnerY + Y + btnSlider.Y + btnSlider.Height)
                            ChangeValueMouseDown = -LargeChange;
                    }

                    mouseDownLargeChange = true;
                }

                if (Orientation == TrackBarOrientation.Horizontal)
                {
                    if (WindowManager.MouseX < OwnerX + X + btnSlider.X && ChangeValueMouseDown < 0)
                        Value += ChangeValueMouseDown;
                    else if (WindowManager.MouseX > OwnerX + X + btnSlider.X + btnSlider.Width && ChangeValueMouseDown > 0)
                        Value += ChangeValueMouseDown;
                }
                else if (Orientation == TrackBarOrientation.Vertical)
                {
                    if (WindowManager.MouseY < OwnerY + Y + btnSlider.Y && ChangeValueMouseDown > 0)
                        Value += ChangeValueMouseDown;
                    else if (WindowManager.MouseY > OwnerY + Y + btnSlider.Y + btnSlider.Height && ChangeValueMouseDown < 0)
                        Value += ChangeValueMouseDown;
                }

                LastUpdateIndexChange = gameTime.TotalGameTime.TotalMilliseconds;
            }
        }

        void TrackBar_KeyDown(object sender, ControlKeyEventArgs e)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - LastUpdateIndexChange > MsToNextIndex)
            {
                if ((e.Key == Keys.Down && Orientation == TrackBarOrientation.Vertical) ||
                    (e.Key == Keys.Up && Orientation == TrackBarOrientation.Horizontal) ||
                    e.Key == Keys.Left)
                    Value--;
                else if ((e.Key == Keys.Up && Orientation == TrackBarOrientation.Vertical) ||
                    (e.Key == Keys.Down && Orientation == TrackBarOrientation.Horizontal) ||
                    e.Key == Keys.Right)
                    Value++;
                else if (e.Key == Keys.PageDown && Orientation == TrackBarOrientation.Vertical)
                    Value -= LargeChange;
                else if (e.Key == Keys.PageUp && Orientation == TrackBarOrientation.Vertical)
                    Value += LargeChange;
                else if (e.Key == Keys.PageDown && Orientation == TrackBarOrientation.Horizontal)
                    Value += LargeChange;
                else if (e.Key == Keys.PageUp && Orientation == TrackBarOrientation.Horizontal)
                    Value -= LargeChange;

                LastUpdateIndexChange = gameTime.TotalGameTime.TotalMilliseconds;
            }
        }

        void TrackBar_SizeChanged(object sender, EventArgs e)
        {
            ConfigureSlider();
        }

        private void CapValue()
        {
            if (Value < minimum)
                Value = minimum;

            if (Value > maximum)
                Value = maximum;
        }

        void btnSlider_MouseLeftPressed(object sender, EventArgs e)
        {
            if (!mouseDown)
            {
                sbPosWhenMouseDown = GetTickPos(Value - Minimum);
                mousePosWhenMouseDown = GetWindowManagerMousePos();
                valueWhenMouseDown = Value;
                mouseDown = true;
            }
        }

        public override void Update()
        {
            TotalMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;

            if (!WindowManager.MouseLeftPressed)
            {
                mouseDown = false;
                mouseDownLargeChange = false;
            }

            if (mouseDown)
            {
                int mSub;

                if (Orientation == TrackBarOrientation.Horizontal)
                    mSub = GetWindowManagerMousePos() - mousePosWhenMouseDown;
                else
                    mSub = mousePosWhenMouseDown - GetWindowManagerMousePos();

                Value = (int)(valueWhenMouseDown + (mSub + sliderSize / 2) / GetTickIntervalSize());
            }

            base.Update();
        }

        public override void Draw()
        {
            btnSlider.Z = Z - 0.001f;

            if (TickStyle != XTK.TickStyle.None)
            {
                for (int i = 0; i < Maximum - Minimum + 1; i++)
                {
                    DrawTick(i);
                }
            }

            if (Orientation == TrackBarOrientation.Horizontal)
                Drawing.DrawFrame(spriteBatch, Theme.SliderBarFrame, OwnerX + X, OwnerY + Y + Height / 2 - 2, Width, 4, Z - 0.000001f);
            else
                Drawing.DrawFrame(spriteBatch, Theme.SliderBarFrame, OwnerX + X + Width / 2 - 2, OwnerY + Y, 4, Height, Z - 0.000001f);

            base.Draw();
        }

        protected void DrawTick(int i)
        {
            if (AlternativeTickDraw != null)
            {
                AlternativeTickDraw(i);
                return;
            }

            int tX = 0, tY = 0;
            int tW = 0, tH = 0;

            if (TickStyle == XTK.TickStyle.BottomRight || TickStyle == XTK.TickStyle.Both)
            {
                if (Orientation == TrackBarOrientation.Horizontal)
                {
                    tX = GetTickPos(i);
                    tY = Height - TickSize;
                    tW = 1;
                    tH = TickSize;
                }
                else
                {
                    tX = Width - TickSize;
                    tY = GetTickPos(i);
                    tW = TickSize;
                    tH = 1;
                }

                Drawing.DrawRectangle(spriteBatch, Theme.Dot, ForeColor,
                    OwnerX + X + tX, OwnerY + Y + tY, tW, tH, Z - 0.00001f);
            }

            if (TickStyle == XTK.TickStyle.TopLeft || TickStyle == XTK.TickStyle.Both)
            {
                if (Orientation == TrackBarOrientation.Horizontal)
                {
                    tX = GetTickPos(i);
                    tY = 0;
                    tW = 1;
                    tH = TickSize;
                }
                else
                {
                    tX = 0;
                    tY = GetTickPos(i);
                    tW = TickSize;
                    tH = 1;
                }

                Drawing.DrawRectangle(spriteBatch, Theme.Dot, ForeColor,
                    OwnerX + X + tX, OwnerY + Y + tY, tW, tH, Z - 0.00001f);
            }
        }

        private void ConfigureSlider()
        {
            btnSlider.X = 0;
            btnSlider.Y = 0;

            if (Orientation == TrackBarOrientation.Horizontal)
            {
                btnSlider.Width = sliderSize;

                if (TickStyle == XTK.TickStyle.None)
                    btnSlider.Height = Height;
                else if (TickStyle == XTK.TickStyle.BottomRight || TickStyle == XTK.TickStyle.TopLeft)
                    btnSlider.Height = Height - TickSize - 1;
                else if (TickStyle == XTK.TickStyle.Both)
                {
                    btnSlider.Y = TickSize + 1;
                    btnSlider.Height = Height - TickSize * 2 - 2;
                }

                if (TickStyle == XTK.TickStyle.TopLeft)
                    btnSlider.Y = TickSize + 1;
            }
            else
            {
                btnSlider.Height = sliderSize;

                if (TickStyle == XTK.TickStyle.None)
                    btnSlider.Width = Width;
                else if (TickStyle == XTK.TickStyle.BottomRight || TickStyle == XTK.TickStyle.TopLeft)
                    btnSlider.Width = Width - TickSize - 1;
                else if (TickStyle == XTK.TickStyle.Both)
                {
                    btnSlider.X = TickSize + 1;
                    btnSlider.Width = Width - TickSize * 2 - 2;
                }

                if (TickStyle == XTK.TickStyle.TopLeft)
                    btnSlider.X = TickSize + 1;
            }

            SetSliderPosAccordingToValue();
        }

        private void SetSliderPosAccordingToValue()
        {
            if (Orientation == TrackBarOrientation.Horizontal)
                btnSlider.X = GetTickPos(Value - Minimum) - btnSlider.Width / 2;
            else
                btnSlider.Y = Height - GetTickPos(Value - Minimum) + btnSlider.Height / 2 - (Height - (Maximum - Minimum) * GetTickIntervalSize());
        }

        private int GetTickIntervalSize()
        {
            int tIS = (int)((GetControlSize() - sliderSize) / (decimal)(Maximum - Minimum));

            if (tIS <= 0)
                tIS = 1;

            return tIS;
        }

        private int GetControlSize()
        {
            if (Orientation == TrackBarOrientation.Horizontal)
                return Width;

            return Height;
        }

        private int GetWindowManagerMousePos()
        {
            if (Orientation == TrackBarOrientation.Horizontal)
                return WindowManager.MouseX;

            return WindowManager.MouseY;
        }

        private int GetTickPos(int position)
        {
            return GetTickIntervalSize() * position + sliderSize / 2;
        }
    }

    public delegate void DrawTickRoutine(int i);

    public enum TrackBarOrientation
    {
        Horizontal = 0,
        Vertical = 1
    }

    public enum TickStyle
    {
        None = 0,
        TopLeft = 1,
        BottomRight = 2,
        Both = 3
    }
}

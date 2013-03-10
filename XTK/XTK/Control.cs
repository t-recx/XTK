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
    public class Control
    {
        protected decimal x, y;
        protected float ?z;
        protected decimal width, height;

        public int TabOrder;

        protected bool visible = true;
        protected bool enabled;
        private bool focused = false;

        private WindowManager windowManager;

        private Control parent;
        private Form owner;

        public List<Control> Controls;

        private bool usingCustomFont = false;
        private SpriteFont font;

        private Color ?foreColor = null;
        private Color ?backColor = null;

        public object Tag;
        protected string text;

        private Theme theme;

        public Texture2D Cursor = null;

        private AnchorStyles anchor = AnchorStyles.Top | AnchorStyles.Left;

        private int previousParentWidth, previousParentHeight;

        protected bool Initialized = false;

        protected int minimumWidth = 0, minimumHeight = 0, maximumWidth = Int32.MaxValue, maximumHeight = Int32.MaxValue;

        private int VisibleWidth, VisibleHeight;

        public int MinimumWidth
        {
            get
            {
                return minimumWidth;
            }
            set
            {
                if (minimumWidth != value)
                    minimumWidth = value;
            }
        }

        public int MinimumHeight
        {
            get
            {
                return minimumHeight;
            }
            set
            {
                if (minimumHeight != value)
                    minimumHeight = value;
            }
        }

        public int MaximumWidth
        {
            get
            {
                return maximumWidth;
            }
            set
            {
                if (maximumWidth != value)
                    maximumWidth = value;
            }
        }

        public int MaximumHeight
        {
            get
            {
                return maximumHeight;
            }
            set
            {
                if (maximumHeight != value)
                    maximumHeight = value;
            }
        }

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;

                //List<Control> lowerCtrls = GetAllDescendingControls(this);

                //foreach (Control c in lowerCtrls)
                //    c.Visible = value;
            }
        }

        public int X
        {
            get
            {
                return (int)x;
            }
            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return (int)y;
            }
            set
            {
                y = value;
            }
        }

        public float Z
        {
            get
            {
                if (z.HasValue)
                    return z.Value;
                if (Parent != null)
                    return Parent.Z - 0.0001f;
                if (Owner != null)
                    return Owner.Z - 0.0001f;

                return 0;
            }
            set
            {
                if (z != value)
                {
                    z = value;

                    if (ZChanged != null)
                        ZChanged(this, new EventArgs());
                }
            }
        }

        public int OwnerX
        {
            get
            {
                int ownerX = 0;

                if (Owner != null)
                    ownerX += Owner.X;

                Control p = Parent;

                while (p != null && p != Owner)
                {
                    ownerX += p.X + (p.Owner != Owner ? p.OwnerX : 0);
                    p = p.Parent;
                }

                return ownerX;
            }
        }

        public int OwnerY
        {
            get
            {
                int ownerY = 0;

                if (Owner != null)
                    ownerY += Owner.Y;

                Control p = Parent;

                while (p != null && p != Owner)
                {
                    ownerY += p.Y + (p.Owner != Owner ? p.OwnerY : 0);
                    p = p.Parent;
                }

                return ownerY;
            }
        }

        public bool OwnerHasFocus
        {
            get
            {
                if (Owner != null)
                    return Owner.Focused;

                return Focused;
            }
        }

        public bool Focused
        {
            get
            {
                return focused;
            }
            set
            {
                if (value != focused)
                {
                    focused = value;

                    if (focused)
                        if (GotFocus != null)
                            GotFocus(this, new EventArgs());
                    else
                        if (LostFocus != null)
                            LostFocus(this, new EventArgs());

                    //if (Parent != null && !(Parent is Form))
                    //    Parent.Focused = value;
                }
            }
        }

        public virtual string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (text != value)
                {
                    text = value;

                    if (TextChanged != null)
                        TextChanged(this, new EventArgs());
                }
            }
        }

        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (enabled != value)
                {
                    enabled = value;

                    if (EnabledChanged != null)
                        EnabledChanged(this, new EventArgs());
                }
            }
        }

        public Color ForeColor
        {
            get
            {
                if (foreColor.HasValue)
                    return foreColor.Value;
                if (Theme != null)
                    return Theme.ForeColor;

                return Color.Gray;
            }
            set
            {
                foreColor = value;
            }
        }

        public Color BackColor
        {
            get
            {
                if (backColor.HasValue)
                    return backColor.Value;
                if (Theme != null)
                    return Theme.BackColor;

                return Color.Gray;
            }
            set
            {
                backColor = value;
            }
        }

        public SpriteFont Font
        {
            get
            {
                if (usingCustomFont)
                    return font;
                    
                return Theme.Font;
            }
            set
            {
                font = value;
                usingCustomFont = true;
            }
        }

        public int Width
        {
            get
            {
                return (int)width;
            }
            set
            {
                if (width != value)
                {
                    width = value;
                    CheckControlBoundaries();
                    CheckVisibility();
                    RaiseWidthChanged();
                }
            }
        }

        public int Height
        {
            get
            {
                return (int)height;
            }
            set
            {
                if (height != value)
                {
                    height = value;
                    CheckControlBoundaries();
                    CheckVisibility();
                    RaiseHeightChanged();
                }
            }
        }

        public Control Parent
        {
            get
            {
                if (parent != null)
                    return parent;
                if (Owner != null)
                    return Owner;

                return null;
            }
            set
            {
                if (value != parent)
                {
                    parent = value;
                    parent.WidthChanged += new EventHandler(parent_SizeChanged);
                    parent.HeightChanged += new EventHandler(parent_SizeChanged);

                    SetPreviousParentSize();
                }
            }
        }

        public WindowManager WindowManager
        {
            get
            {
                if (Owner != null)
                    return Owner.WindowManager;

                if (windowManager != null)
                    return windowManager;

                return null;
            }
            set
            {
                windowManager = value;
            }
        }

        public Theme Theme
        {
            get
            {
                if (theme != null)
                    return theme;

                if (Owner != null)
                    return Owner.Theme;

                if (WindowManager != null)
                    return WindowManager.Theme;

                return null;
            }
            set
            {
                theme = value;
            }
        }

        protected GraphicsDevice graphicsDevice
        {
            get
            {
                return WindowManager.graphicsDevice;
            }
        }

        protected SpriteBatch spriteBatch
        {
            get
            {
                return WindowManager.spriteBatch;
            }
        }

        protected GameTime gameTime
        {
            get
            {
                return WindowManager.gameTime;
            }
        }

        protected GameResolution.VirtualScreen virtualScreen
        {
            get
            {
                return WindowManager.virtualScreen;
            }
        }

        public AnchorStyles Anchor
        {
            get
            {
                return anchor;
            }
            set
            {
                anchor = value;
            }
        }

        public Form Owner
        {
            get
            {
                return owner;
            }
            set
            {
                if (owner != value)
                {
                    owner = value;
                    owner.WidthChanged += new EventHandler(parent_SizeChanged);
                    owner.HeightChanged += new EventHandler(parent_SizeChanged);

                    SetPreviousParentSize();
                }
            }
        }

        public event EventHandler Init;
        public event EventHandler GotFocus;
        public event EventHandler LostFocus;
        public event EventHandler Click;
        public event EventHandler MouseLeftDown;
        public event EventHandler MouseRightDown;
        public event EventHandler MouseLeftPressed;
        public event EventHandler MouseRightPressed;
        public event EventHandler MouseLeftUp;
        public event EventHandler MouseRightUp;
        public event EventHandler TextChanged;
        public event EventHandler EnabledChanged;
        public event EventHandler HeightChanged;
        public event EventHandler WidthChanged;
        public event EventHandler ZChanged;
        public event ControlKeyEventHandler KeyDown;
        public event ControlKeyEventHandler KeyUp;

        public Control()
        {
            Controls = new List<Control>();
        }

        public Control(Form controlowner)
        {
            Owner = controlowner;

            Controls = new List<Control>();
        }

        private void CheckVisibility()
        {
            int parentVisibleWidth, parentVisibleHeight;

            if (this is Form)
            {
                parentVisibleWidth = windowManager.ScreenWidth;
                parentVisibleHeight = windowManager.ScreenHeight;
            }
            else
            {
                parentVisibleWidth = Parent.Width;
                parentVisibleHeight = Parent.Height;
            }

            VisibleWidth = Width;
            VisibleHeight = Height;
            
            if (X + Width > parentVisibleWidth)
                VisibleWidth = parentVisibleWidth - X;
            if (Y + Height > parentVisibleHeight)
                VisibleHeight = parentVisibleHeight - Y;

            if (Controls != null)
            {
                foreach (Control c in Controls)
                    c.CheckVisibility();
            }
        }

        private void CheckControlBoundaries()
        {
            if (Width > MaximumWidth)
                Width = MaximumWidth;
            if (Width < MinimumWidth)
                Width = MinimumWidth;
            if (Height > MaximumHeight)
                Height = MaximumHeight;
            if (Height < MinimumHeight)
                Height = MinimumHeight;
        }

        private void RaiseWidthChanged()
        {
            if (WidthChanged != null)
                WidthChanged(this, new EventArgs());
        }

        private void RaiseHeightChanged()
        {
            if (HeightChanged != null)
                HeightChanged(this, new EventArgs());
        }

        void parent_SizeChanged(object sender, EventArgs e)
        {
            if (sender == Parent)
            {
                AnchorAlignment();
                SetPreviousParentSize();
            }
        }

        private void SetPreviousParentSize()
        {
            previousParentWidth = Parent.Width;
            previousParentHeight = Parent.Height;
        }

        public virtual void AnchorAlignment()
        {
            if (!Initialized)
                return;

            bool anchorTop = (Anchor & AnchorStyles.Top) == AnchorStyles.Top;
            bool anchorBottom = (Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom;
            bool anchorLeft = (Anchor & AnchorStyles.Left) == AnchorStyles.Left;
            bool anchorRight = (Anchor & AnchorStyles.Right) == AnchorStyles.Right;

            int prevWidth = (int)width, prevHeight = (int)height;

            if (anchorTop && anchorBottom)
                height += ((decimal)(Parent.Height - previousParentHeight));
            else if (anchorBottom)
                y += ((decimal)(Parent.Height - previousParentHeight));

            if (anchorLeft && anchorRight)
                width += ((decimal)(Parent.Width - previousParentWidth));
            else if (anchorRight)
                x += ((decimal)(Parent.Width - previousParentWidth));

            if (!anchorLeft && !anchorRight)
                x += ((decimal)(Parent.Width - previousParentWidth) / (decimal)2);

            if (!anchorTop && !anchorBottom)
                y += ((decimal)(Parent.Height - previousParentHeight) / (decimal)2);

            CheckControlBoundaries();

            if (prevHeight != (int)height)
                RaiseHeightChanged();
            if (prevWidth != (int)width)
                RaiseWidthChanged();
        }

        public virtual void InitControl()
        {
            if (Controls != null)
                foreach (Control control in Controls)
                {
                    if (this is Form)
                        control.Owner = (Form)this;
                    else
                        control.Owner = Owner;

                    control.InitControl();
                }

            Initialized = true;
        }

        public virtual void Draw()
        {
            if (Theme != null)
                Drawing.DrawRectangle(spriteBatch, Theme.Dot, BackColor, OwnerX + X, OwnerY + Y, Width, Height, Z);
        }

        public virtual void Update()
        {
            if (((this is Form && Focused) || (Owner != null && Owner.Focused)) && Cursor != null && MouseIsOnControl())
                WindowManager.MouseCursor = Cursor;
        }

        public virtual bool ReceiveMessage(MessageEnum message, object msgTag)
        {
            bool StopSpreading = false;

            switch (message)
            {
                case MessageEnum.Init:
                    if (Init != null)
                        Init(this, new EventArgs());
                    break;

                case MessageEnum.MouseLeftDown:
                    if (MouseIsOnControl() && Focused)
                    {
                        if (MouseLeftDown != null)
                            MouseLeftDown(this, new EventArgs());

                        StopSpreading = true;
                    }
                    break;

                case MessageEnum.MouseRightDown:
                    if (MouseIsOnControl() && Focused)
                    {
                        if (MouseRightDown != null)
                            MouseRightDown(this, new EventArgs());

                        StopSpreading = true;
                    }
                    break;

                case MessageEnum.MouseLeftClick:
                case MessageEnum.MouseRightClick:
                    if (MouseIsOnControl() && Focused)
                    {
                        PerformClick();

                        StopSpreading = true;
                    }
                    break;

                case MessageEnum.MouseLeftPressed:
                    if (MouseIsOnControl())
                    {
                        if (Controls != null)
                        {
                            foreach (Control control in Controls)
                                Messages.SendMessage(control, message, msgTag);
                        }

                        Focus();

                        if (MouseLeftPressed != null)
                            MouseLeftPressed(this, new EventArgs());

                        return true;
                    }
                    break;

                case MessageEnum.MouseRightPressed:
                    if (MouseIsOnControl())
                    {
                        if (MouseRightPressed != null)
                            MouseRightPressed(this, new EventArgs());

                        StopSpreading = true;
                    }
                    break;

                case MessageEnum.MouseLeftUp:
                    if (MouseLeftUp != null)
                        MouseLeftUp(this, new EventArgs());
                    break;

                case MessageEnum.MouseRightUp:
                    if (MouseRightUp != null)
                        MouseRightUp(this, new EventArgs());
                    break;

                case MessageEnum.UnFocus:
                    Focused = false;
                    break;

                case MessageEnum.Focus:
                    Focused = true;
                    return true;

                case MessageEnum.Draw:
                    Draw();
                    break;

                case MessageEnum.Logic:
                    Update();
                    break;

                case MessageEnum.KeyDown:
                    if (Focused && (this is Form || Owner.Focused))
                    {
                        if (KeyDown != null)
                            KeyDown(this, new ControlKeyEventArgs((Keys)msgTag));
                    }
                    break;

                case MessageEnum.KeyUp:
                    if (Focused && (this is Form || Owner.Focused))
                    {
                        if (KeyUp != null)
                            KeyUp(this, new ControlKeyEventArgs((Keys)msgTag));
                    }
                    break;
            }

            Messages.BroadcastMessage(Controls, message, msgTag);

            //if (Controls != null)
            //{
            //    foreach (Control control in Controls)
            //        Messages.SendMessage(control, message, msgTag);
            //}

            return StopSpreading;
        }

        public void Focus()
        {
            if (!Focused)
            {
                if (Owner != null && Owner.Controls != null && Owner.Focused)
                {
                    foreach (Control control in GetAllDescendingControls(Owner))
                    {
                        if (control != this)
                            Messages.SendMessage(control, MessageEnum.UnFocus);
                    }

                    Messages.SendMessage(this, MessageEnum.Focus);
                }
                else
                    Messages.SendMessage(this, MessageEnum.Focus);
            }
        }

        protected List<Control> GetAllUpperControlsTo(Control originControl, Control destinationControl)
        {
            List<Control> ctrls = new List<Control>();

            if (destinationControl != originControl)
            {
                if (originControl.Parent != null && originControl.Parent.Controls != null)
                {
                    ctrls.Add(originControl.Parent);

                    for (int i = 0; i < originControl.Parent.Controls.Count; i++)
                        ctrls.AddRange(GetAllUpperControlsTo(originControl.Parent, destinationControl));
                }
            }

            return ctrls;
        }

        protected List<Control> GetAllUpperControlsTo(Control destinationControl)
        {
            List<Control> ctrls = GetAllUpperControlsTo(this, destinationControl);

            return ctrls;
        }

        protected List<Control> GetAllDescendingControlsIncluding(Control originalControl)
        {
            List<Control> ctrls = new List<Control>();

            if (originalControl.Controls != null)
            {
                ctrls.Add(originalControl);

                for (int i = 0; i < originalControl.Controls.Count; i++)
                    ctrls.AddRange(GetAllDescendingControlsIncluding(originalControl.Controls[i]));
            }

            return ctrls;
        }

        protected List<Control> GetAllDescendingControls(Control originalControl)
        {
            List<Control> ctrls = new List<Control>();

            if (originalControl.Controls != null)
            {
                for (int i = 0; i < originalControl.Controls.Count; i++)
                    ctrls.AddRange(GetAllDescendingControlsIncluding(originalControl.Controls[i]));
            }

            return ctrls;
        }

        //protected bool MouseIsOnControl()
        //{
        //    if (WindowManager == null)
        //        return false;

        //    if (WindowManager.MouseX >= OwnerX + X && WindowManager.MouseY >= OwnerY + Y &&
        //        WindowManager.MouseX <= OwnerX + X + Width && WindowManager.MouseY <= OwnerY + Y + Height)
        //    {
        //        if (!(this is Form))
        //        {
        //            List<Control> allCtrlsOnOwner = GetAllDescendingControls(Owner);
        //            List<Control> allAscendingCtrlsOnThis = GetAllUpperControlsTo(Owner).FindAll(c => c != Owner);

        //            foreach (Control c in allCtrlsOnOwner)
        //            {
        //                List<Control> allAscendingCtrls = GetAllUpperControlsTo(c, c.Owner);

        //                if (c.Visible && !allAscendingCtrls.Exists(i => !i.Visible) && c != this)
        //                {
        //                    if (!allAscendingCtrlsOnThis.Contains(c))
        //                    {
        //                        Control UpperParent = this;

        //                        if (allAscendingCtrlsOnThis != null && allAscendingCtrlsOnThis.Count > 0)
        //                            UpperParent = allAscendingCtrlsOnThis.Find(up => up.Parent is Form);

        //                        if (WindowManager.MouseX >= c.OwnerX + c.X && WindowManager.MouseY >= c.OwnerY + c.Y &&
        //                            WindowManager.MouseX <= c.OwnerX + c.X + c.Width && WindowManager.MouseY <= c.OwnerY + c.Y + c.Height
        //                            && c.OwnerY + c.Y < UpperParent.OwnerY + UpperParent.Y)
        //                        {
        //                            return false;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        return true;
        //    }

        //    return false;
        //}

        protected bool MouseIsOnControl(bool DoZOverlapVerification = true)
        {
            if (WindowManager == null)
                return false;

            if (WindowManager.MouseX >= OwnerX + X && WindowManager.MouseY >= OwnerY + Y &&
                WindowManager.MouseX <= OwnerX + X + Width && WindowManager.MouseY <= OwnerY + Y + Height)
            {
                if (!(this is Form) && DoZOverlapVerification)
                {
                    List<Control> allCtrlsOnOwner = GetAllDescendingControls(Owner);

                    foreach (Control c in allCtrlsOnOwner)
                    {
                        if (!c.Visible)
                            continue;

                        if (c.Z < this.Z && WindowManager.MouseX >= c.OwnerX + c.X && WindowManager.MouseY >= c.OwnerY + c.Y &&
                            WindowManager.MouseX <= c.OwnerX + c.X + c.Width && WindowManager.MouseY <= c.OwnerY + c.Y + c.Height)
                        {
                            List<Control> allAscendingCtrls = GetAllUpperControlsTo(c, Owner);

                            if (!allAscendingCtrls.Exists(u => !u.Visible))
                                return false;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public void PerformClick()
        {
            if (Click != null)
                Click(this, new EventArgs());
        }
    }

    [Flags]
    public enum AnchorStyles
    {
        None = 0x0,
        Top = 0x1,
        Bottom = 0x2,
        Left = 0x4,
        Right = 0x8
    }
}

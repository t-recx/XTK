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
    public class Tab : Control
    {
        private int selectedIndex = 0;
        private int ?tabPageHeight = null;
        
        public List<Texture2D> MainFrame;
        public List<Texture2D> TabSelectedFrame;
        public List<Texture2D> TabFrame;

        private List<TabPage> tabPages = new List<TabPage>();
        private List<Button> TabButtons = new List<Button>();

        private Button btnPrevious;
        private Button btnNext;

        private int scrollX = 0;

        private int ScrollX
        {
            get
            {
                return scrollX;
            }
            set
            {
                if (scrollX != value)
                {
                    scrollX = value;

                    if (scrollX < 0)
                        scrollX = 0;

                    if (scrollX > TabButtons.Count - 1)
                        scrollX = TabButtons.Count - 1;
                }
            }
        }

        public List<TabPage> TabPages
        {
            get
            {
                return tabPages;
            }
            set
            {
                tabPages = value;

                ConfigureControl();
            }
        }

        public int TabPageHeight
        {
            get
            {
                if (!tabPageHeight.HasValue)
                    tabPageHeight = Drawing.GetFontHeight(Font) + 1;

                return tabPageHeight.Value;
            }
            set
            {
                tabPageHeight = value;

                ConfigureControl();
            }
        }

        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;

                    if (TabPages == null || TabPages.Count == 0)
                        selectedIndex = -1;
                    else if (selectedIndex < 0)
                        selectedIndex = 0;
                    else if (TabPages != null && selectedIndex > TabPages.Count - 1)
                        selectedIndex = TabPages.Count - 1;

                    if (SelectedIndexChanged != null)
                        SelectedIndexChanged(this, new EventArgs());
                }
            }
        }

        public TabPage SelectedPage
        {
            get
            {
                if (TabPages.Count == 0 || SelectedIndex < 0 || SelectedIndex > TabPages.Count - 1)
                    return null;

                return TabPages[SelectedIndex];
            }
        }

        public event EventHandler SelectedIndexChanged;

        public Tab(Form formowner)
            : base(formowner)
        {
            this.Init += new EventHandler(Tab_Init);
            this.WidthChanged += new EventHandler(Tab_SizeChanged);
            this.HeightChanged += new EventHandler(Tab_SizeChanged);
        }

        void Tab_SizeChanged(object sender, EventArgs e)
        {
            if (!Initialized)
                return;

            SetTabPagesSize();
            SetVisibilityTabButtons();
        }

        void Tab_Init(object sender, EventArgs e)
        {
            MainFrame = Theme.TabMainFrame;
            TabSelectedFrame = Theme.TabPageSelectedFrame;
            TabFrame = Theme.TabPageFrame;
            ConfigureControl();
        }

        public override void Draw()
        {
            for (int i = 0; i < TabButtons.Count; i++)
                TabButtons[i].Z = Z - 0.0025f;

            Drawing.DrawFrame(spriteBatch, MainFrame, OwnerX + X, OwnerY + Y + TabPageHeight, 
                Width, Height - TabPageHeight, Z - 0.001f);

            //base.Draw();
        }

        private void SetTabPageSize(int i)
        {
            TabPages[i].Width = Width;
            TabPages[i].Height = Height - TabPageHeight;
        }

        private void SetTabPagesSize()
        {
            for (int i = 0; i < TabPages.Count; i++)
                SetTabPageSize(i);
        }

        private void ConfigureNavigationButtons()
        {
            btnPrevious = new Button(Owner) { Parent = this, X = Width - TabPageHeight * 2, Height = TabPageHeight, Width = TabPageHeight, Image = Theme.ArrowLeft, Anchor = AnchorStyles.Right | AnchorStyles.Top };
            btnNext = new Button(Owner) { Parent = this, X = Width - TabPageHeight, Height = TabPageHeight, Width = TabPageHeight, Image = Theme.ArrowRight, Anchor = AnchorStyles.Right | AnchorStyles.Top };

            btnPrevious.InitControl();
            btnNext.InitControl();

            Controls.Add(btnPrevious);
            Controls.Add(btnNext);

            btnPrevious.Click += new EventHandler(btnNavigation_Click);
            btnNext.Click += new EventHandler(btnNavigation_Click);
        }

        private void SetVisibilityTabButtons()
        {
            int currentX = 0;
            int shownWidth = Width - (btnPrevious.Visible ? btnPrevious.Width : 0) - (btnNext.Visible ? btnNext.Width : 0);

            for (int i = ScrollX; i < TabButtons.Count; i++)
            {
                currentX += TabButtons[i].Width;

                TabButtons[i].Visible = currentX <= shownWidth;
            }

            SetVisibilityNavigationButtons();
        }

        private void SetVisibilityNavigationButtons()
        {
            if (TabButtons.Exists(tb => !tb.Visible))
                btnNext.Visible = btnPrevious.Visible = true;
            else
                btnNext.Visible = btnPrevious.Visible = false;
        }

        private void Navigate(TabNavigation tabNavigation)
        {
            ScrollX += (int)tabNavigation;

            for (int i = 0; i < TabButtons.Count; i++)
                TabButtons[i].Visible = i >= ScrollX;

            SetTabButtonsPosition();
            SetVisibilityTabButtons();
        }

        void btnNavigation_Click(object sender, EventArgs e)
        {
            Navigate(btnNext == sender ? TabNavigation.Right : TabNavigation.Left);
        }

        private void SetTabButtonsPosition()
        {
            int currentX = 0;

            for (int i = 0; i < TabButtons.Count; i++)
            {
                if (TabButtons[i].Visible)
                {
                    TabButtons[i].X = currentX;

                    currentX += TabButtons[i].Width;
                }
            }
        }

        private void ConfigureControl()
        {
            Controls.Clear();
            TabButtons.Clear();

            ConfigureNavigationButtons();

            for (int i = 0; i < TabPages.Count; i++)
            {
                TabButtons.Add(new Button(Owner) { Parent = this, Y = 0, Text = TabPages[i].Text,
                    Width = (int)Font.MeasureString(TabPages[i].Text).X + TabPages[i].TextPadding, Height = TabPageHeight + 1 });

                SetButtonFrame(TabButtons[i], SelectedIndex == i);

                TabButtons[i].Tag = i;

                TabButtons[i].DepressTextOnPress = false;
                TabButtons[i].Click += new EventHandler(Tab_Click);

                SetTabPageSize(i);
                TabPages[i].X = 0;
                TabPages[i].Y = TabPageHeight;
                TabPages[i].Parent = this;
                TabPages[i].Selected = SelectedIndex == i;


                Controls.AddRange(new Control[] { TabButtons[i], TabPages[i] });

                TabButtons[i].InitControl();
                TabPages[i].InitControl();
            }

            SetTabButtonsPosition();
            SetVisibilityTabButtons();
        }

        private void SetButtonFrame(Button b, bool selected)
        {
            b.Frame = b.FramePressed = selected ? TabSelectedFrame : TabFrame;
        }

        void Tab_Click(object sender, EventArgs e)
        {
            SelectedIndex = (int)(sender as Control).Tag;

            for (int i = 0; i < TabPages.Count; i++)
            {
                TabPages[i].Selected = SelectedIndex == i;

                SetButtonFrame(TabButtons[i], SelectedIndex == i);
            }
        }
    }

    public enum TabNavigation
    {
        Left = -1,
        Right = 1
    }
}

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
        private int tabPageHeight = 18;
        
        public List<Texture2D> MainFrame;
        public List<Texture2D> TabSelectedFrame;
        public List<Texture2D> TabFrame;

        private List<TabPage> tabPages = new List<TabPage>();
        public List<Button> TabButtons = new List<Button>();

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
                return tabPageHeight;
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
            SetTabPagesSize();
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

            Drawing.DrawFrame(spriteBatch, MainFrame, OwnerX + X, OwnerY + Y + tabPageHeight, 
                Width, Height - tabPageHeight, Z - 0.001f);

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

        private void ConfigureControl()
        {
            int currentX = 0;

            Controls.Clear();
            TabButtons.Clear();

            for (int i = 0; i < TabPages.Count; i++)
            {
                TabButtons.Add(new Button(Owner) { Parent = this, X = currentX, Y = 0, Text = TabPages[i].Text,
                    Width = (int)Font.MeasureString(TabPages[i].Text).X + TabPages[i].TextPadding, Height = tabPageHeight + 1 });

                SetButtonFrame(TabButtons[i], SelectedIndex == i);

                TabButtons[i].Tag = i;

                TabButtons[i].DepressTextOnPress = false;
                TabButtons[i].Click += new EventHandler(Tab_Click);

                SetTabPageSize(i);
                TabPages[i].X = 0;
                TabPages[i].Y = TabPageHeight;
                TabPages[i].Parent = this;
                TabPages[i].Selected = SelectedIndex == i;

                currentX = TabButtons[i].Width;

                Controls.AddRange(new Control[] { TabButtons[i], TabPages[i] });

                TabButtons[i].InitControl();
                TabPages[i].InitControl();
            }
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
}

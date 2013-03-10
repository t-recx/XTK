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
    public class frmMixer : Form
    {
        public frmMixer(WindowManager windowmanager)
            : base(windowmanager)
        {
            WindowManager = windowmanager;

            int grpWidth = 40;
            for (int i = 0; i < 4; i++)
            {
                GroupBox grpChannel = new GroupBox(this) { X = i * grpWidth + (i + 1) * 5, Y = 0, Width = grpWidth, Height = 100, Text = (i + 1).ToString() };

                TrackBar tckVolume = new TrackBar(this) { Parent = grpChannel, X = 10, Y = 15, Width = 20, Height = 80, TickStyle = XTK.TickStyle.Both,
                    Orientation = TrackBarOrientation.Vertical, BackColor = new Color(170, 170, 170) };

                grpChannel.Controls.Add(tckVolume);

                Controls.Add(grpChannel);

                GroupBox grpPan = new GroupBox(this) { X = i * grpWidth + (i + 1) * 5, Y = 100, Width = grpWidth, Height = 50, Text = "Pan" };

                TrackBar tckPan = new TrackBar(this) { Parent = grpPan, X = 5, Y = 15, Width = grpWidth - 10, Height = 30, TickStyle = TickStyle.Both,
                    Orientation = TrackBarOrientation.Horizontal, BackColor = new Color(170, 170, 170), Maximum = 2 };

                grpPan.Controls.Add(tckPan);

                Controls.Add(grpPan);
            }

            this.MaximizeWindowButtonVisible = this.MinimizeWindowButtonVisible = false;
        }
    }
}

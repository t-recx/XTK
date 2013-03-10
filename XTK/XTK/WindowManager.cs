using System;
using System.ComponentModel;
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
    public class WindowManager
    {
        public List<Form> Forms;

        public Theme Theme;

        public int MouseX, MouseY;

        public bool MouseLeftPressed = false;
        public bool MouseRightPressed = false;
        public bool PreviousMouseLeftPressed = false;
        public bool PreviousMouseRightPressed = false;

        public Texture2D MouseCursor;

        private Keys[] PreviousKeysPressed = null;

        public GraphicsDevice graphicsDevice;
        public SpriteBatch spriteBatch;

        public GameTime gameTime;
        public GameResolution.VirtualScreen virtualScreen;

        public int MaximizedWindowWidth, MaximizedWindowHeight;
        public int ScreenWidth, ScreenHeight;

        bool Initialized = false;

        public WindowManager(int maximizedwindowwidth, int maximizedwindowheight)
        {
            ScreenWidth = MaximizedWindowWidth = maximizedwindowwidth;
            ScreenHeight = MaximizedWindowHeight = maximizedwindowheight;

            Forms = new List<Form>();
        }

        void Forms_OnAdd(object sender, object item)
        {
            
        }

        public void Draw(GraphicsDevice gDevice, SpriteBatch sBatch)
        {
            graphicsDevice = gDevice;
            spriteBatch = sBatch;

            foreach (Form form in Forms)
                Messages.SendMessage(form, MessageEnum.Draw, null);

            if (MouseCursor != null)
                Drawing.Draw(spriteBatch, MouseCursor, new Vector2(MouseX, MouseY), Color.White, 0);
        }

        public void Update(GameTime gTime, GameResolution.VirtualScreen vScreen)
        {
            gameTime = gTime;
            virtualScreen = vScreen;
            MouseCursor = Theme.DefaultCursor;

            MouseState ms = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();

            PreviousMouseLeftPressed = MouseLeftPressed;
            PreviousMouseRightPressed = MouseRightPressed;

            MouseX = (ms.X * virtualScreen.VirtualWidth) / virtualScreen.graphicsDevice.Viewport.Width;
            MouseY = (ms.Y * virtualScreen.VirtualHeight) / virtualScreen.graphicsDevice.Viewport.Height;

            if (!MouseLeftPressed && ms.LeftButton == ButtonState.Pressed)
                MouseLeftPressed = true;
            if (MouseLeftPressed && ms.LeftButton != ButtonState.Pressed)
                MouseLeftPressed = false;

            if (!MouseRightPressed && ms.RightButton == ButtonState.Pressed)
                MouseRightPressed = true;
            if (MouseRightPressed && ms.RightButton != ButtonState.Pressed)
                MouseRightPressed = false;

            if (Forms != null && Forms.Count > 0)
            {
                if (!Initialized)
                {
                    foreach (Form form in Forms)
                        form.InitControl();

                    BringToFront(Forms[0]);

                    Messages.BroadcastMessage(Forms, MessageEnum.Init);

                    Initialized = true;
                }

                if (!PreviousMouseLeftPressed && MouseLeftPressed)
                    Messages.BroadcastMessage(Forms, MessageEnum.MouseLeftPressed);
                if (!PreviousMouseRightPressed && MouseRightPressed)
                    Messages.BroadcastMessage(Forms, MessageEnum.MouseRightPressed); 
                if (ms.LeftButton == ButtonState.Pressed)
                    Messages.BroadcastMessage(Forms, MessageEnum.MouseLeftDown);
                if (ms.RightButton == ButtonState.Pressed)
                    Messages.BroadcastMessage(Forms, MessageEnum.MouseRightDown);
                if (!MouseLeftPressed && PreviousMouseLeftPressed == true)
                    Messages.BroadcastMessage(Forms, MessageEnum.MouseLeftClick);
                if (!MouseRightPressed && PreviousMouseRightPressed == true)
                    Messages.BroadcastMessage(Forms, MessageEnum.MouseRightClick);
                if (PreviousMouseLeftPressed && !MouseLeftPressed)
                    Messages.BroadcastMessage(Forms, MessageEnum.MouseLeftUp);
                if (PreviousMouseRightPressed && !MouseRightPressed)
                    Messages.BroadcastMessage(Forms, MessageEnum.MouseRightUp);

                Keys[] KeysPressed = ks.GetPressedKeys();

                foreach (Keys keyPressed in KeysPressed)
                    Messages.BroadcastMessage(Forms, MessageEnum.KeyDown, keyPressed);

                if (PreviousKeysPressed != null)
                {
                    foreach (Keys keyPressed in PreviousKeysPressed)
                        if (!KeysPressed.Contains(keyPressed))
                            Messages.BroadcastMessage(Forms, MessageEnum.KeyUp, keyPressed);
                }

                PreviousKeysPressed = KeysPressed;

                foreach (Form form in Forms)
                    Messages.SendMessage(form, MessageEnum.Logic, null);
            }
        }

        public void BringToFront(Form form)
        {
            float interval = 1 / (float)Forms.Count;

            form.Z = interval;
            form.Focused = true;

            List<Form> FormsFiltered = Forms.FindAll(f => f != form);

            if (FormsFiltered != null)
            {
                float newZ = interval;

                FormsFiltered = FormsFiltered.OrderBy(f => f.Z).ToList();

                foreach (Form f in FormsFiltered)
                {
                    newZ += interval;
                    f.Focused = false;
                    f.Z = newZ;
                }
            }
            
            Forms = Forms.OrderBy(f => f.Z).ToList();
        }
    }
}

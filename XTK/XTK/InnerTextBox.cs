using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XTK
{
    public class InnerTextBox : Control
    {
        private int cursorX = 0, cursorY = 0;
        private int scrollX, scrollY;

        private int fontHeight;
        private int nrItemsVisible = 0;

        private bool multiLine = false;

        private int TextXPadding = 4;

        public List<TextBoxLine> internalText = new List<TextBoxLine>();
        private List<TextBoxKeyPress> internalKeyPress = new List<TextBoxKeyPress>();

        private int MSToNextKeyPress = 500;
        private int MSToNextCursorBlink = 500;
        private double LastUpdateCursorBlink = 0;
        private bool cursorVisible = false;

        private bool ShiftPressed = false;

        public bool CursorVisible
        {
            get
            {
                if (!Focused)
                    return false;

                return cursorVisible;
            }
            set
            {
                cursorVisible = value;
            }
        }

        public int NrLines
        {
            get
            {
                return internalText.Count;
            }
        }

        public int NrItemsVisible
        {
            get
            {
                if (nrItemsVisible <= 0)
                    UpdateNrItemsVisible();

                return nrItemsVisible;
            }
            set
            {
                nrItemsVisible = value;
            }
        }

        public int CursorX
        {
            get
            {
                return cursorX;
            }
            set
            {
                if (cursorX != value)
                {
                    cursorX = value;

                    AdjustCursorX();

                    UpdateScrollHorizontally();
                }
            }
        }

        public int CursorY
        {
            get
            {
                return cursorY;
            }
            set
            {
                if (cursorY != value)
                {
                    cursorY = value;

                    if (cursorY < 0)
                        cursorY = 0;
                    else if (cursorY > internalText.Count - 1)
                        cursorY = internalText.Count - 1;

                    if (CursorX > internalText[cursorY].Text.Length)
                        CursorX = internalText[cursorY].Text.Length;

                    ScrollX = 0;

                    UpdateScrollVertically();
                }
            }
        }

        public int ScrollX
        {
            get
            {
                return scrollX;
            }
            set
            {
                if (value >= 0 && value != scrollX)
                {
                    scrollX = value;

                    UpdateNrCharsVisible();

                    if (ScrollChanged != null)
                        ScrollChanged(this, new EventArgs());
                }
            }
        }

        public int ScrollY
        {
            get
            {
                return scrollY;
            }
            set
            {
                if (value >= 0 && value != scrollY)
                {
                    scrollY = value;

                    if (ScrollChanged != null)
                        ScrollChanged(this, new EventArgs());
                }
            }
        }

        public bool MultiLine
        {
            get
            {
                return multiLine;
            }
            set
            {
                multiLine = value;
            }
        }

        public override string Text
        {
            get
            {
                return GetTextInString();
            }
            set
            {
                SetTextInternalRepresentation(value);

                if (TextChanged != null)
                    TextChanged(this, new EventArgs());

                UpdateNrCharsVisible();
                UpdateNrItemsVisible();

                AdjustCursorX();
            }
        }

        private int FontHeight
        {
            get
            {
                if (fontHeight <= 0)
                    fontHeight = Drawing.GetFontHeight(Theme.Font);

                return fontHeight;
            }
        }

        public new event EventHandler TextChanged;
        public event EventHandler ScrollChanged;
        public event TextChangingEventHandler TextChanging;

        public InnerTextBox(Form formowner)
            : base(formowner)
        {
            this.WidthChanged += new EventHandler(InnerTextBox_WidthChanged);
            this.HeightChanged += new EventHandler(InnerTextBox_HeightChanged);
            this.KeyDown += new ControlKeyEventHandler(InnerTextBox_KeyDown);
            this.KeyUp += new ControlKeyEventHandler(InnerTextBox_KeyUp);
            this.Init += new EventHandler(InnerTextBox_Init);
            this.MouseLeftPressed += new EventHandler(InnerTextBox_MouseLeftPressed);
        }

        void InnerTextBox_MouseLeftPressed(object sender, EventArgs e)
        {
            int i;
            int pSX = ScrollX;
            CursorY = ScrollY + (WindowManager.MouseY - (OwnerY + Y) + WindowManager.MouseCursor.Height / 2) / FontHeight;

            for (i = 0; i < internalText[CursorY].Text.Length - pSX + 1; i++)
            {
                if (Font.MeasureString(internalText[CursorY].Text.Substring(pSX, i)).X < (WindowManager.MouseX - (OwnerX + X)))
                    CursorX = pSX + i;
                else
                    break;
            }

            ScrollX = pSX;

            //if (i == internalText[CursorY].Text.Length - pSX)
            //    CursorX++;

            if (CursorX < ScrollX)
                ScrollX = CursorX;
        }

        void InnerTextBox_KeyUp(object sender, ControlKeyEventArgs e)
        {
            if (e.Key == Keys.LeftShift || e.Key == Keys.RightShift)
                ShiftPressed = false;

            internalKeyPress.RemoveAll(x => x.KeyPressed == e.Key);
        }

        void InnerTextBox_Init(object sender, EventArgs e)
        {
            SetTextInternalRepresentation(Text);
            UpdateNrCharsVisible();

            if (Cursor == null)
                Cursor = Theme.WritingCursor;
        }

        private void AdjustCursorX()
        {
            if (cursorX < 0)
            {
                if (CursorY > 0)
                {
                    CursorY--;
                    cursorX = internalText[CursorY].Text.Length;
                }
                else
                    cursorX = 0;
            }
            else if (cursorX > internalText[CursorY].Text.Length)
            {
                if (CursorY < internalText.Count - 1)
                {
                    CursorY++;
                    cursorX = 0;
                }
                else
                {
                    if (internalText[CursorY].Text.Length > 0)
                        cursorX = internalText[CursorY].Text.Length;
                    else
                        cursorX = 0;

                }
            }
        }

        private bool CanRegisterKey(Keys keypressed)
        {
            TextBoxKeyPress tbkp = internalKeyPress.Find(x => x.KeyPressed == keypressed);

            if (tbkp != null)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds - tbkp.LastUpdate <= MSToNextKeyPress / (tbkp.nrKeyPressesRegistered > 1 ? 16 : 1))
                    return false;
            }

            return true;
        }

        private int GetEndOfLineCursorX()
        {
            return internalText[CursorY].Text.Length - 1;
        }

        private void KeyChangeCursorY(bool up)
        {
            int i;
            int pSX = ScrollX;
            int currentWidthToCursorX;

            currentWidthToCursorX = (int)Font.MeasureString(internalText[CursorY].Text.Substring(ScrollX, CursorX - ScrollX)).X;

            CursorY += up ? -1 : 1;

            for (i = 0; i < internalText[CursorY].Text.Length - pSX; i++)
            {
                if (Font.MeasureString(internalText[CursorY].Text.Substring(pSX, i)).X > currentWidthToCursorX)
                    CursorX = pSX + i;
                else
                    break;
            }
        }

        void InnerTextBox_KeyDown(object sender, ControlKeyEventArgs e)
        {
            string charKey = e.Key.ToString();

            if (internalKeyPress.FindAll(x => x.KeyPressed == e.Key).Count == 0)
                internalKeyPress.Add(new TextBoxKeyPress(e.Key, gameTime.TotalGameTime.TotalMilliseconds));
            else
            {
                if (!CanRegisterKey(e.Key))
                    return;
                else
                {
                    TextBoxKeyPress tbkp = internalKeyPress.Find(x => x.KeyPressed == e.Key);

                    tbkp.LastUpdate = gameTime.TotalGameTime.TotalMilliseconds;
                    tbkp.nrKeyPressesRegistered++;
                }
            }

            if (charKey.Length == 1)
            {
                string c;

                if (ShiftPressed)
                    c = charKey.ToString();
                else
                    c = charKey.ToString().ToLower();

                AddText(c);
            }
            else if (charKey.Length == 2 && charKey[0] == 'D' && Char.IsNumber(charKey[1]))
            {
                AddText(charKey[1].ToString());
            }
            else if (charKey.Length == 7 && charKey.Substring(0, 6) == "NumPad" && Char.IsNumber(charKey[6]))
            {
                AddText(charKey[6].ToString());
            }
            else
            {
                switch (e.Key)
                {
                    case Keys.Home:
                        CursorX = 0;
                        break;
                    case Keys.End:
                        CursorX = GetEndOfLineCursorX();
                        break;
                    case Keys.Left:
                        CursorX--;
                        break;
                    case Keys.Right:
                        CursorX++;
                        break;
                    case Keys.Up:
                        KeyChangeCursorY(true);
                        break;
                    case Keys.Down:
                        KeyChangeCursorY(false);
                        break;
                    case Keys.Back:
                        DeleteChar();
                        break;
                    case Keys.Enter:
                        AddNewLine();
                        break;
                    case Keys.Space:
                        AddText(" ");
                        break;
                    case Keys.LeftShift:
                    case Keys.RightShift:
                        ShiftPressed = true;
                        break;
                }
            }

            SetCursorVisibility(true);
            UpdateNrCharsVisible();
        }

        private void AddText(string t)
        {
            if (TextChanging != null)
            {
                TextChangingEventArgs tcEA = new TextChangingEventArgs(t);

                TextChanging(this, tcEA);

                if (tcEA.Cancel)
                    return;
            }

            if (internalText[CursorY].Text.Length >= CursorX)
                t = t + internalText[CursorY].Text.Substring(CursorX, internalText[CursorY].Text.Length - (CursorX));

            internalText[CursorY].Text = internalText[CursorY].Text.Substring(0, CursorX) + t;
            CursorX++;

            if (TextChanged != null)
                TextChanged(this, new EventArgs());
        }

        private void AddNewLine()
        {
            if (!MultiLine)
                return;

            if (TextChanging != null)
            {
                TextChangingEventArgs tcEA = new TextChangingEventArgs(Environment.NewLine);

                TextChanging(this, tcEA);

                if (tcEA.Cancel)
                    return;
            }

            string t = "";

            if (internalText[CursorY].Text.Length > CursorX)
                t = internalText[CursorY].Text.Substring(CursorX, internalText[CursorY].Text.Length - CursorX);

            internalText.Insert(CursorY + 1, new TextBoxLine(t, false));

            internalText[CursorY].Text = internalText[CursorY].Text.Substring(0, CursorX);
            CursorY++;
            CursorX = 0;

            if (TextChanged != null)
                TextChanged(this, new EventArgs());
        }

        private void DeleteChar()
        {
            if (CursorX > 0)
            {
                string t = "";

                if (internalText[CursorY].Text.Length > CursorX)
                    t = t + internalText[CursorY].Text.Substring(CursorX, internalText[CursorY].Text.Length - (CursorX));

                internalText[CursorY].Text = internalText[CursorY].Text.Substring(0, CursorX - 1) + t;

                CursorX--;
            }
            else
            {
                if (CursorY > 0 && CursorY < internalText.Count)
                {
                    int nextCursorX = internalText[CursorY - 1].Text.Length;

                    internalText[CursorY - 1].Text += internalText[CursorY].Text;
                    internalText.Remove(internalText[CursorY]);
                    CursorY--;
                    CursorX = nextCursorX;
                }
            }

            if (TextChanged != null)
                TextChanged(this, new EventArgs());
        }

        void InnerTextBox_WidthChanged(object sender, EventArgs e)
        {
            UpdateNrCharsVisible();

            UpdateNrItemsVisible();
        }

        void InnerTextBox_HeightChanged(object sender, EventArgs e)
        {
            UpdateNrItemsVisible();
        }

        private void SetTextInternalRepresentation(string textString)
        {
            internalText.Clear();

            if (!MultiLine)
            {
                internalText.Add(new TextBoxLine(textString, false));
                return;
            }
            
            string[] splitSTNewLines = textString.Split('\n'.ToString().ToCharArray());

            foreach (string line in splitSTNewLines)
                internalText.Add(new TextBoxLine(line, false));
        }

        private string GetTextInString()
        {
            string t = "";

            for (int i = 0; i < internalText.Count; i++)
            {
                t += internalText[i].Text;

                if (
                    (!internalText[i].Wrapped || 
                    (internalText[i].Wrapped && i + 1 < internalText.Count && !internalText[i + 1].Wrapped)) 
                    && i != internalText.Count
                    )
                    t += '\n'.ToString();
            }

            return t;
        }

        private void UpdateScrollHorizontally()
        {
            if (CursorX < 0)
                return;

            int stringSize;

            if (ScrollX < CursorX)
            {
                stringSize = (int)Theme.Font.MeasureString(internalText[CursorY].Text.Substring(ScrollX, CursorX - ScrollX)).X + 1;

                while (stringSize + Theme.TextCursor.Width + TextXPadding >= Width)
                {
                    ScrollX++;
                    stringSize = (int)Theme.Font.MeasureString(internalText[CursorY].Text.Substring(ScrollX, CursorX - ScrollX)).X + 1;
                }
            }

            while (ScrollX > CursorX)
                ScrollX--;
        }

        private void UpdateScrollVertically()
        {
            while (CursorY >= ScrollY + NrItemsVisible)
                ScrollY++;
            while (ScrollY > CursorY)
                ScrollY--;
        }

        public override void Draw()
        {
            Drawing.DrawRectangle(spriteBatch, Theme.Dot, Color.White, OwnerX + X, OwnerY + Y, Width, Height, Z - 0.0014f);

            for (int i = ScrollY; i < ScrollY + NrItemsVisible && i < internalText.Count; i++)
            {
                string itemText = internalText[i].Text;

                if (itemText == null)
                    continue;

                if (itemText.Length < ScrollX)
                    continue;

                itemText = itemText.Substring(ScrollX, internalText[i].NrCharsVisible);

                Drawing.DrawString(spriteBatch, Theme.Font, itemText, X + OwnerX + TextXPadding,
                    Y + OwnerY + (i - ScrollY) * FontHeight, ForeColor, Z - 0.0015f);

                if (Focused && Owner.Focused)
                {
                    if (CursorVisible && CursorY == i && CursorX >= ScrollX && CursorX <= internalText[i].Text.Length)
                    {
                        int stringSize = 0;

                        stringSize = (int)Theme.Font.MeasureString(internalText[i].Text.Substring(ScrollX, CursorX - ScrollX)).X + 1;

                        if (stringSize + Theme.TextCursor.Width + TextXPadding < Width)
                            Drawing.DrawRectangle(spriteBatch, Theme.TextCursor, Color.White, TextXPadding +
                                X + OwnerX + stringSize,
                                Y + OwnerY + (i - ScrollY) * FontHeight, Theme.TextCursor.Width, FontHeight - 1, Z - 0.0016f);
                    }
                }
            }
            
            //base.Draw();
        }

        private void SetCursorVisibility(bool cursorvisible)
        {
            CursorVisible = cursorvisible;
            LastUpdateCursorBlink = gameTime.TotalGameTime.TotalMilliseconds;
        }

        public override void Update()
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - LastUpdateCursorBlink > MSToNextCursorBlink)
                SetCursorVisibility(!CursorVisible);

            base.Update();
        }

        public void UpdateNrItemsVisible()
        {
            NrItemsVisible = (int)((double)Height / (double)(FontHeight));
        }

        private void UpdateNrCharsVisible()
        {
            if (!Initialized)
                return;

            for (int i = 0; i < internalText.Count; i++)
            {
                string itemText = internalText[i].Text;

                if (itemText == null)
                    continue;

                if (itemText.Length - ScrollX <= 0)
                {
                    internalText[i].NrCharsVisible = 0;
                    continue;
                }

                itemText = itemText.Substring(ScrollX, itemText.Length - ScrollX);

                internalText[i].NrCharsVisible = itemText.Length;

                while (Theme.Font.MeasureString(itemText.Substring(0, internalText[i].NrCharsVisible)).X + TextXPadding > Width &&
                    internalText[i].NrCharsVisible > 0)
                {
                    if (internalText[i].NrCharsVisible - 1 >= 0)
                    {
                        internalText[i].NrCharsVisible--;
                        itemText = itemText.Substring(0, internalText[i].NrCharsVisible);
                    }
                }
            }
        }
    }

    public class TextBoxLine
    {
        public string Text;
        public bool Wrapped;
        public int NrCharsVisible;

        public TextBoxLine(string text, bool wrapped)
        {
            Text = text;
            Wrapped = wrapped;
        }
    }

    public class TextBoxKeyPress
    {
        public Keys KeyPressed;
        public double LastUpdate;
        public int nrKeyPressesRegistered = 1;

        public TextBoxKeyPress(Keys keypressed, double lastupdate)
        {
            KeyPressed = keypressed;
            LastUpdate = lastupdate;
        }
    }
}

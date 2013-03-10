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
using System.Xml;
using System.Xml.Serialization;

namespace XTK
{
    public class Theme
    {
        public string Name;
        public string TSFilename;

        public Color ForeColor;
        public Color BackColor;

        [XmlIgnore]
        public SpriteFont Font;

        [XmlIgnore]
        public SpriteFont FontBold;

        [XmlIgnore]
        public TS.Tileset TileSet;

        [XmlIgnore]
        public Texture2D Dot;

        [XmlIgnore]
        public Texture2D TextCursor;

        [XmlIgnore]
        public Texture2D ResizeCursorDown;

        [XmlIgnore]
        public Texture2D ResizeCursorDownLeft;

        [XmlIgnore]
        public Texture2D ResizeCursorDownRight;

        [XmlIgnore]
        public Texture2D DefaultCursor;

        [XmlIgnore]
        public Texture2D WritingCursor;

        [XmlIgnore]
        public List<Texture2D> Frame;

        [XmlIgnore]
        public List<Texture2D> FramePressed;

        [XmlIgnore]
        public List<Texture2D> ListFrame;

        [XmlIgnore]
        public List<Texture2D> SliderBarFrame;

        [XmlIgnore]
        public List<Texture2D> SliderButtonFrame;

        [XmlIgnore]
        public List<Texture2D> ComboFrame;

        [XmlIgnore]
        public List<Texture2D> NumericUpDownFrame;

        [XmlIgnore]
        public List<Texture2D> GroupFrame;

        [XmlIgnore]
        public List<Texture2D> TitleBarFocusedFrame;

        [XmlIgnore]
        public List<Texture2D> TitleBarUnfocusedFrame;

        [XmlIgnore]
        public List<Texture2D> TextBoxFrame;

        [XmlIgnore]
        public List<Texture2D> ProgressBarFrame;

        [XmlIgnore]
        public List<Texture2D> WindowEdgeFrame;

        [XmlIgnore]
        public Texture2D SelectedListItemBackground;

        [XmlIgnore]
        public Texture2D RadioButtonChecked;

        [XmlIgnore]
        public Texture2D RadioButtonUnchecked;

        [XmlIgnore]
        public Texture2D CheckBoxChecked;

        [XmlIgnore]
        public Texture2D CheckBoxUnchecked;

        [XmlIgnore]
        public Texture2D VerticalHandle;

        [XmlIgnore]
        public Texture2D HorizontalHandle;

        [XmlIgnore]
        public Texture2D ArrowUp;

        [XmlIgnore]
        public Texture2D ArrowDown;

        [XmlIgnore]
        public Texture2D ArrowLeft;

        [XmlIgnore]
        public Texture2D ArrowRight;

        [XmlIgnore]
        public Texture2D ComboButton;

        [XmlIgnore]
        public Texture2D CloseIcon;

        [XmlIgnore]
        public Texture2D Maximize;

        [XmlIgnore]
        public Texture2D UnMaximize;

        [XmlIgnore]
        public Texture2D Minimize;

        [XmlIgnore]
        public Texture2D WindowOptions;

        [XmlIgnore]
        public Texture2D NumericUpDownUpButton;

        [XmlIgnore]
        public Texture2D NumericUpDownDownButton;

        [XmlIgnore]
        public Texture2D CheckedListBoxCheckBoxChecked;

        [XmlIgnore]
        public Texture2D CheckedListBoxCheckBoxUnchecked;
        
        [XmlIgnore]
        public Texture2D TreeListBoxExpand;
       
        [XmlIgnore]
        public Texture2D TreeListBoxCollapse;
    }

    public enum FramePart
    {
        UpLeft = 0,
        UpRight = 1,
        DownLeft = 2,
        DownRight = 3,
        Left = 4,
        Right = 5,
        Up = 6,
        Down = 7,
        Background = 8
    }
}

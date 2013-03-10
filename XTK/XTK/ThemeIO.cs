using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XTK
{
    public static class ThemeIO
    {
        public static Theme Load(string Filename, GraphicsDevice GraphDevice)
        {
            Theme Theme = new XTK.Theme();
            XmlSerializer serializer = new XmlSerializer(typeof(Theme));

            FileStream fs = new FileStream(Filename, FileMode.Open);
            XmlReader reader = new XmlTextReader(fs);

            Theme = (Theme)serializer.Deserialize(reader);

            Theme.TileSet = TS.IO.Load(Theme.TSFilename, GraphDevice);

            Theme.Dot = Theme.TileSet.GetPicture("Dot");
            Theme.DefaultCursor = Theme.TileSet.GetPicture("Cursor");
            Theme.ResizeCursorDown = Theme.TileSet.GetPicture("ResizeCursorDown");
            Theme.ResizeCursorDownLeft = Theme.TileSet.GetPicture("ResizeCursorDownLeft");
            Theme.ResizeCursorDownRight = Theme.TileSet.GetPicture("ResizeCursorDownRight");
            Theme.WritingCursor = Theme.TileSet.GetPicture("WritingCursor");
            Theme.TextCursor = Theme.TileSet.GetPicture("TextCursor");
            Theme.CloseIcon = Theme.TileSet.GetPicture("CloseIcon");
            Theme.Maximize = Theme.TileSet.GetPicture("Maximize");
            Theme.UnMaximize = Theme.TileSet.GetPicture("UnMaximize");
            Theme.Minimize = Theme.TileSet.GetPicture("Minimize");
            Theme.WindowOptions = Theme.TileSet.GetPicture("WindowOptions");

            Theme.Frame = GetFrame(Theme.TileSet, "Frame");
            Theme.FramePressed = GetFrame(Theme.TileSet, "FramePressed");
            Theme.ListFrame = GetFrame(Theme.TileSet, "List");
            Theme.ComboFrame = GetFrame(Theme.TileSet, "Combo");
            Theme.NumericUpDownFrame = GetFrame(Theme.TileSet, "NumericUpDown");
            Theme.TextBoxFrame = GetFrame(Theme.TileSet, "TextBox");
            Theme.GroupFrame = GetFrame(Theme.TileSet, "Group");
            Theme.SelectedListItemBackground = Theme.TileSet.GetPicture("List", "Part", "SelectedItemBackground", 0);
            Theme.ProgressBarFrame = GetFrame(Theme.TileSet, "ProgressBar");
            Theme.SliderBarFrame = GetFrame(Theme.TileSet, "SliderBar");
            Theme.SliderButtonFrame = GetFrame(Theme.TileSet, "Slider");
            Theme.TitleBarFocusedFrame = GetFrame(Theme.TileSet, "TitleBarFocused");
            Theme.TitleBarUnfocusedFrame = GetFrame(Theme.TileSet, "TitleBarUnfocused");
            Theme.WindowEdgeFrame = GetFrame(Theme.TileSet, "WindowEdge");

            Theme.RadioButtonChecked = Theme.TileSet.GetPicture("RadioButton", "Checked", "true");
            Theme.RadioButtonUnchecked = Theme.TileSet.GetPicture("RadioButton", "Checked", "false");
            Theme.CheckBoxChecked = Theme.TileSet.GetPicture("CheckBox", "Checked", "true");
            Theme.CheckBoxUnchecked = Theme.TileSet.GetPicture("CheckBox", "Checked", "false");

            Theme.HorizontalHandle = Theme.TileSet.GetPicture("HorizontalHandle");
            Theme.VerticalHandle = Theme.TileSet.GetPicture("VerticalHandle");

            Theme.ArrowUp = Theme.TileSet.GetPicture("Arrow", "Direction", "Up");
            Theme.ArrowDown = Theme.TileSet.GetPicture("Arrow", "Direction", "Down");
            Theme.ArrowLeft = Theme.TileSet.GetPicture("Arrow", "Direction", "Left");
            Theme.ArrowRight = Theme.TileSet.GetPicture("Arrow", "Direction", "Right");
            Theme.ComboButton = Theme.TileSet.GetPicture("ComboButton");
            Theme.NumericUpDownUpButton = Theme.TileSet.GetPicture("NumericUpDownUpButton");
            Theme.NumericUpDownDownButton = Theme.TileSet.GetPicture("NumericUpDownDownButton");

            Theme.CheckedListBoxCheckBoxChecked = Theme.TileSet.GetPicture("CheckedListBoxCheckBox", "Checked", "true");
            Theme.CheckedListBoxCheckBoxUnchecked = Theme.TileSet.GetPicture("CheckedListBoxCheckBox", "Checked", "false");

            Theme.TreeListBoxExpand = Theme.TileSet.GetPicture("TreeListBoxExpand");
            Theme.TreeListBoxCollapse = Theme.TileSet.GetPicture("TreeListBoxCollapse");

            fs.Close();

            return Theme;
        }

        public static int Save(string Filename, Theme Theme)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Theme));

            TextWriter tw = new StreamWriter(Filename);

            xs.Serialize(tw, Theme);

            tw.Close();

            return 0;
        }

        public static List<Texture2D> GetFrame(TS.Tileset tileset, string assetName)
        {
            List<Texture2D> lstItems = new List<Texture2D>();

            Array values = Enum.GetValues(typeof(FramePart));

            foreach(FramePart val in values)
                lstItems.Add(tileset.GetPicture(assetName, "Part", Enum.GetName(typeof(FramePart), val)));

            return lstItems;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTK
{
    public static class Messages
    {
        public static bool SendMessage(Control control, MessageEnum message, object msgTag = null)
        {
            if (control != null)
            {
                if (control.Visible || message == MessageEnum.Init)
                {
                    if (control.ReceiveMessage(message, msgTag))
                        return true;
                }
            }

            return false;
        }

        //private static bool HasFocus(Control c)
        //{
        //    if (c.Focused)
        //        return true;

        //    List<Control> ctrls = c.Controls;

        //    if (ctrls.FindAll(i => i.Focused).Count > 0)
        //        return true;

        //    foreach (Control c2 in ctrls)
        //    {
        //        if (HasFocus(c2))
        //            return true;
        //    }

        //    return false;
        //}

        public static bool BroadcastMessage(List<Control> Controls, MessageEnum message, object msgTag = null)
        {
            if (Controls != null)
            {
                foreach (Control control in Controls)
                {
                    if (control.Focused)
                        if (SendMessage(control, message, msgTag))
                            return true;
                }

                foreach (Control control in Controls)
                {
                    if (!control.Focused)
                        if (SendMessage(control, message, msgTag))
                            return true;
                }
            }

            return false;
        }

        public static bool BroadcastMessage(List<Form> Forms, MessageEnum message, object msgTag = null)
        {
            if (Forms != null)
            {
                foreach (Form form in Forms)
                {
                    if (SendMessage((Control)form, message, msgTag))
                        return true;
                }
            }

            return false;
        }
    }

    public enum MessageEnum
    {
        MouseLeftClick = 0,
        MouseRightClick = 1,
        MouseLeftDown = 2,
        MouseRightDown = 3,
        MouseLeftUp = 4,
        MouseRightUp = 5,
        MouseLeftPressed = 6,
        MouseRightPressed = 7,
        Focus = 8,
        UnFocus = 9,
        Init = 10,
        KeyDown = 11,
        KeyUp = 12,
        Draw = 13,
        Logic = 14
    }
}

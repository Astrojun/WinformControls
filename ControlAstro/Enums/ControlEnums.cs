using ControlAstro.Design;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace ControlAstro.Enums.ControlEnums
{
    //[Editor("System.Windows.Forms.Design.AnchorEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
    [Flags, Editor(typeof(BorderEditor), typeof(UITypeEditor))]
    public enum BorderState
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,
    }

    public enum LightState
    {
        On = 0,
        Bad = 1,
        Off = -1,
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ControlAstro.Structs
{
    [TypeConverter(typeof(FormColorConverter))]
    public struct FormColor
    {
        public Color HeaderBackColor { get; set; }
        public Color HeaderForeColor { get; set; }
        public Color ControlBoxBackColor { get; set; }
        public Color ControlBoxForeColor { get; set; }
        public FlatButtonAppearance MinButtonAppearance { get; set; }
        public FlatButtonAppearance MaxButtonAppearance { get; set; }
        public FlatButtonAppearance CloseButtonAppearance { get; set; }
        public Color BorderColor { get; set; }
        //public LinearGradientBrush BackBrush { get; set; }
        //public float BackAngle { get; set; }
    }

    public class FormColorConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return "";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (context != null && context.Instance is Label)
            {
                Attribute[] array = new Attribute[attributes.Length + 1];
                attributes.CopyTo(array, 0);
                array[attributes.Length] = new ApplicableToFormColorAttribute();
                attributes = array;
            }
            return TypeDescriptor.GetProperties(value, attributes);
        }
    }

    internal sealed class ApplicableToFormColorAttribute : Attribute
    {
    }

}

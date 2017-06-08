using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ControlAstro.Controls
{
    [ToolboxBitmap(typeof(Label))]
    [DefaultEvent("Click")]
    public class LabelButton : Label
    {
        private bool isHovered = false;
        private bool isPressed = false;
        private Color defaultOverColor = Color.Gainsboro;
        private Color defaultDownColor = Color.Silver;
        private Color defaultBackColor;

        private FlatLabelAppearance flatAppearance;
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("CatAppearance"), Description("ButtonFlatAppearance")]
        public FlatLabelAppearance FlatAppearance
        {
            get
            {
                if (this.flatAppearance == null)
                {
                    this.flatAppearance = new FlatLabelAppearance(this);
                }
                return this.flatAppearance;
            }
        }

        [Browsable(false)]
        public bool Pressed
        {
            get { return isPressed; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (defaultBackColor == null || defaultBackColor == Color.Empty) defaultBackColor = base.BackColor;
            base.OnPaint(e);
        }

        private void InvalidateState()
        {
            if (isHovered && !isPressed && Enabled)
            {
                BackColor = FlatAppearance.MouseOverBackColor == Color.Empty ? defaultOverColor : FlatAppearance.MouseOverBackColor;
            }
            else if (isHovered && isPressed && Enabled)
            {
                BackColor = FlatAppearance.MouseDownBackColor == Color.Empty ? defaultDownColor : FlatAppearance.MouseDownBackColor;
            }
            else
            {
                BackColor = defaultBackColor;
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            isHovered = false;
            isPressed = false;
            InvalidateState();

            base.OnLostFocus(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            isHovered = false;
            isPressed = false;
            InvalidateState();

            base.OnLeave(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                isHovered = true;
                isPressed = true;
                InvalidateState();
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            isHovered = false;
            isPressed = false;
            InvalidateState();

            base.OnKeyUp(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            InvalidateState();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPressed = true;
                InvalidateState();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isPressed = false;
            InvalidateState();

            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            InvalidateState();

            base.OnMouseLeave(e);
        }

    }

    [TypeConverter(typeof(FlatLabelAppearanceConverter))]
    public class FlatLabelAppearance
    {
        private Control owner;

        internal FlatLabelAppearance(Control owner)
        {
            this.owner = owner;
        }

        //private int borderSize = 1;
        //[EditorBrowsable(EditorBrowsableState.Always), DefaultValue(1), Browsable(true), ApplicableToLabel, NotifyParentProperty(true), Category("CatAppearance"), Description("ButtonBorderSizeDescr")]
        //public int BorderSize
        //{
        //    get
        //    {
        //        return this.borderSize;
        //    }
        //    set
        //    {
        //        if (value < 0)
        //        {
        //            throw new ArgumentOutOfRangeException("BorderSize", value, "InvalidLowBoundArgumentEx:\nBorderSize应大于等于0");
        //        }
        //        if (this.borderSize != value)
        //        {
        //            this.borderSize = value;
        //            this.owner.Invalidate();
        //        }
        //    }
        //}

        //private Color borderColor = Color.Empty;
        //[DefaultValue(typeof(Color), ""), Browsable(true), ApplicableToLabel, NotifyParentProperty(true), Category("CatAppearance"), Description("ButtonBorderColorDescr"), EditorBrowsable(EditorBrowsableState.Always)]
        //public Color BorderColor
        //{
        //    get
        //    {
        //        return this.borderColor;
        //    }
        //    set
        //    {
        //        if (value.Equals(Color.Transparent))
        //        {
        //            throw new NotSupportedException("ButtonFlatAppearanceInvalidBorderColor");
        //        }
        //        if (this.borderColor != value)
        //        {
        //            this.borderColor = value;
        //            this.owner.Invalidate();
        //        }
        //    }
        //}

        private Color mouseDownBackColor = Color.Empty;
        [NotifyParentProperty(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(typeof(Color), ""), Description("ButtonMouseDownBackColorDescr"), Browsable(true), ApplicableToLabel, Category("CatAppearance")]
        public Color MouseDownBackColor
        {
            get
            {
                return this.mouseDownBackColor;
            }
            set
            {
                if (this.mouseDownBackColor != value)
                {
                    this.mouseDownBackColor = value;
                    this.owner.Invalidate();
                }
            }
        }

        private Color mouseOverBackColor = Color.Empty;
        [NotifyParentProperty(true), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(typeof(Color), ""), Description("ButtonMouseOverBackColorDescr"), Browsable(true), Category("CatAppearance"), ApplicableToLabel]
        public Color MouseOverBackColor
        {
            get
            {
                return this.mouseOverBackColor;
            }
            set
            {
                if (this.mouseOverBackColor != value)
                {
                    this.mouseOverBackColor = value;
                    this.owner.Invalidate();
                }
            }
        }

    }

    internal class FlatLabelAppearanceConverter : ExpandableObjectConverter
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
                array[attributes.Length] = new ApplicableToLabelAttribute();
                attributes = array;
            }
            return TypeDescriptor.GetProperties(value, attributes);
        }
    }

    internal sealed class ApplicableToLabelAttribute : Attribute
    {
    }


}

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MixManagementPlatform.Controls
{
    public partial class SeparatorLine : Control
    {
        private HorizontalAlignment textAlign = HorizontalAlignment.Left;
        [DefaultValue(typeof(HorizontalAlignment), "Left")]
        public HorizontalAlignment TextAlign
        {
            get { return textAlign; }
            set
            {
                if (textAlign != value)
                {
                    textAlign = value;
                    Size textSize = TextRenderer.MeasureText(CreateGraphics(), Text, Font);
                    textSize.Width += 6;
                    textSize.Height += 4;
                    switch(value)
                    {
                        case HorizontalAlignment.Center:
                            TextPaddingLeft = (Width - textSize.Width) / 2;
                            break;
                        case HorizontalAlignment.Left:
                            TextPaddingLeft = 0;
                            break;
                        case HorizontalAlignment.Right:
                            TextPaddingLeft = Width - textSize.Width;
                            break;
                    }
                }
            }
        }

        private int textPaddingLeft = 0;
        [DefaultValue(0)]
        public int TextPaddingLeft
        {
            get { return textPaddingLeft; }
            set
            {
                if (textPaddingLeft != value)
                {
                    textPaddingLeft = value;
                    Invalidate();
                }
            }
        }

        private Color lineColor = Color.Black;
        [DefaultValue(typeof(Color), "Black")]
        public Color LineColor
        {
            get { return lineColor; }
            set
            {
                if (lineColor != value)
                {
                    lineColor = value;
                    Invalidate();
                }
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                Invalidate();
            }
        }


        public SeparatorLine()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, 
                true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Size textSize = TextRenderer.MeasureText(e.Graphics, Text, Font);
            textSize.Width += 6;
            textSize.Height += 4;
            Rectangle textRect = new Rectangle(new Point(TextPaddingLeft, (Height - textSize.Height) / 2), textSize);
            using (Pen pen = new Pen(LineColor, 1))
            {
                e.Graphics.DrawLine(pen, 0, Height / 2, textRect.Left - 4, Height / 2);
                e.Graphics.DrawLine(pen, textRect.Right, Height / 2, Width, Height / 2);
            }
            TextRenderer.DrawText(e.Graphics, Text, Font, textRect, ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }
    }
}

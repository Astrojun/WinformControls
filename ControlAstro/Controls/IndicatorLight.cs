using ControlAstro.Enums.ControlEnums;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ControlAstro.Controls
{
    public partial class IndicatorLight : Control
    {

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

        private bool showText = true;
        [Description("是否显示文本"), DefaultValue(true)]
        public bool ShowText
        {
            get { return showText; }
            set
            {
                showText = value;
                Invalidate();
            }
        }

        private LightState lightState = LightState.Off;
        [Description("是否为亮灯状态"), DefaultValue(LightState.Off)]
        public LightState LightState
        {
            get { return lightState; }
            set
            {
                lightState = value;
                Invalidate();
            }
        }

        

        public IndicatorLight()
        {
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Rectangle outRectangle = new Rectangle(2, 2, ClientRectangle.Height - 4, ClientRectangle.Height - 4);
            g.DrawEllipse(Pens.Black, outRectangle);
            Rectangle inRectangle = outRectangle;
            inRectangle.Inflate(-2, -2);
            if (LightState == LightState.On)
            {
                g.FillEllipse(Brushes.LimeGreen, inRectangle);
            }
            else if(LightState == LightState.Off)
            {
                g.FillEllipse(Brushes.Silver, inRectangle);
            }
            else
            {
                g.FillEllipse(Brushes.Red, inRectangle);
            }
            Point textPoint = new Point(outRectangle.Right + 3, 0);
            Rectangle textRectangle = new Rectangle(outRectangle.Right + 3, 0, ClientRectangle.Width - outRectangle.Right - 5, ClientRectangle.Height);
            if(ShowText)
            {
                TextRenderer.DrawText(g, Text, Font, textRectangle, ForeColor, TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);
            }
        }

    }
}

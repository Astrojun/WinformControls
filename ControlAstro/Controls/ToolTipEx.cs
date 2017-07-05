using System.Drawing;
using System.Windows.Forms;

namespace ControlAstro.Controls
{
    public class ToolTipEx : ToolTip
    {
        private Size size = new Size(200, 120);
        public Size Size
        {
            get { return size; }
            set { size = value; }
        }

        public ToolTipEx()
        {
            OwnerDraw = true;
            IsBalloon = false;
            Draw += ToolTipEx_Draw;
            Popup += ToolTipEx_Popup;
        }

        private void ToolTipEx_Popup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = Size;
        }

        private void ToolTipEx_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            e.DrawBorder();
            e.DrawText(TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);
        }

        

    }
}

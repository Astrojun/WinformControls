using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ControlAstro.Controls
{
    [ToolboxBitmap(typeof(Button))]
    [DefaultEvent("Click")]
    public partial class ButtonTile : Control
    {
        private bool isPressed = false;
        private bool isHovered = false;

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }

            set
            {
                base.BackColor = value;
            }
        }


        public ButtonTile()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.ResizeRedraw |
                         ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            base.BackColor = Color.Gray;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(SystemColors.Control);

            Rectangle rect = ClientRectangle;
            if (isPressed)
            {
                rect.Inflate(-3, -3);
            }
            using (SolidBrush backBrush = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(backBrush, rect);
            }
            if (isHovered)
            {
                ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                    Color.FromArgb(255, Color.Silver), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(255, Color.Silver), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(255, Color.Silver), 2, ButtonBorderStyle.Solid,
                    Color.FromArgb(255, Color.Silver), 2, ButtonBorderStyle.Solid);
            }


            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPressed = true;
                isHovered = false;
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isPressed = false;
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            Invalidate();
        }

    }
}

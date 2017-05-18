using ControlAstro.Design;
using ControlAstro.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ControlAstro.Controls
{
    [Designer(typeof(CircleButtonDesigner))]
    [ToolboxBitmap(typeof(Button))]
    [DefaultEvent("Click")]
    public partial class CircleButton : Button
    {
        private const int RADIUS = 2;
        private bool isHovered = false;
        private bool isPressed = false;
        private bool isFocused = false;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        private new FlatStyle FlatStyle { get; set; }

        public CircleButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.ResizeRedraw |
                         ControlStyles.UserPaint, true);
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Color backColor, borderColor, foreColor;
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, RADIUS * 2, RADIUS * 2, 180, 90);
            path.AddArc(Width - RADIUS * 2 - 1, 0, RADIUS * 2, RADIUS * 2, 270, 90);
            path.AddArc(Width - RADIUS * 2 - 1, Height - RADIUS * 2 - 1, RADIUS * 2, RADIUS * 2, 0, 90);
            path.AddArc(0, Height - RADIUS * 2 - 1, RADIUS * 2, RADIUS * 2, 90, 90);
            path.CloseFigure();

            if (isHovered && !isPressed && Enabled)
            {
                backColor = FlatAppearance.MouseOverBackColor;
                borderColor = FlatAppearance.BorderColor == null ? FlatAppearance.BorderColor : Color.Gray;
                foreColor = ForeColor;
            }
            else if (isHovered && isPressed && Enabled)
            {
                backColor = FlatAppearance.MouseDownBackColor;
                borderColor = FlatAppearance.BorderColor == null ? FlatAppearance.BorderColor : Color.Gray;
                foreColor = ForeColor;
            }
            else if (!Enabled)
            {
                backColor = BackColor;
                borderColor = FlatAppearance.BorderColor == null ? FlatAppearance.BorderColor : Color.Gray;
                foreColor = SystemColors.GrayText;
            }
            else
            {
                backColor = BackColor;
                borderColor = FlatAppearance.BorderColor == null ? FlatAppearance.BorderColor : Color.Gray;
                foreColor = ForeColor;
            }
            if (Parent == null)
            {
                g.Clear(SystemColors.Control);
            }
            else
            {
                g.Clear(Parent.BackColor);
            }
            using (SolidBrush brush = new SolidBrush(backColor))
            {
                g.FillPath(brush, path);
            }
            if (FlatAppearance.BorderSize != 0 || isHovered)
            {
                using (Pen pen = new Pen(borderColor, FlatAppearance.BorderSize))
                {
                    g.DrawPath(pen, path);
                }
            }

            TextRenderer.DrawText(g, Text, Font, ClientRectangle, foreColor, backColor, AlignmentTools.ContentAlignment2TextFormatFlags(TextAlign));
            //暂时无用
            if (false && isFocused)
                ControlPaint.DrawFocusRectangle(g, ClientRectangle);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            isFocused = true;
            Invalidate();

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            isFocused = false;
            isHovered = false;
            isPressed = false;
            Invalidate();

            base.OnLostFocus(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            isFocused = true;
            Invalidate();

            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            isFocused = false;
            isHovered = false;
            isPressed = false;
            Invalidate();

            base.OnLeave(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                isHovered = true;
                isPressed = true;
                Invalidate();
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            isHovered = false;
            isPressed = false;
            Invalidate();

            base.OnKeyUp(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isPressed = false;
            Invalidate();

            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            Invalidate();

            base.OnMouseLeave(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

    }
}

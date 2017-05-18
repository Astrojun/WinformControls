using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ControlAstro.Controls
{
    [System.ComponentModel.ToolboxItem(false)]
    public partial class ToolStripEx : ToolStrip
    {
        public ToolStripEx()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

    }


    public class ToolStripRendererEx : ToolStripRenderer
    {
        private Color _colorArrow = Color.Black;

        public ToolStripRendererEx()
            : base()
        {
        }

        //ToolStrip、MenuStrip、StatusStrip、ContextMenuStrip
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            Rectangle bounds = e.AffectedBounds;

            if (toolStrip is ToolStripDropDown)  //下拉选项区域
            {
                using (SolidBrush brush = new SolidBrush(e.BackColor))
                {
                    g.FillRectangle(brush, bounds);
                }
            }
            else  //非下拉，正常显示区域
            {
                using (SolidBrush brush = new SolidBrush(e.BackColor))
                {
                    g.FillRectangle(brush, bounds);
                }
            }
        }

        //ToolStrip、MenuStrip、StatusStrip、ContextMenuStrip
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            Rectangle bounds = e.AffectedBounds;

            if (toolStrip is ToolStripDropDown)  //下拉选项区域
            {
                using (Pen pen = new Pen(Color.Silver))
                {
                    g.DrawLines(pen, new Point[] {
                        new Point(bounds.X, bounds.Y),
                        new Point(bounds.X, bounds.Bottom - 1),
                        new Point(bounds.Right - 1, bounds.Bottom - 1),
                        new Point(bounds.Right - 1, bounds.Y),
                        new Point(e.ConnectedArea.Right, bounds.Y) });
                    //g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }

                if (toolStrip is ToolStripOverflow)  //控件大小随父容器大小改变时item溢出时显示的区域
                {
                    using (Pen pen = new Pen(Color.Silver))
                    {
                        g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                    }
                }
            }
            else
            {
                base.OnRenderToolStripBorder(e);
            }
        }

        //ToolStrip、MenuStrip、StatusStrip、ContextMenuStrip
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripItem item = e.Item;

            if (!item.Enabled)
            {
                return;
            }

            Graphics g = e.Graphics;
            Rectangle clientRectangle = new Rectangle(Point.Empty, item.Size);

            if (toolStrip is MenuStrip)  //MenuStrip
            {
                if (item.Pressed)  //item被按下
                {
                    using (SolidBrush brush = new SolidBrush(e.Item.BackColor))
                    {
                        g.FillRectangle(brush, clientRectangle);
                        using (Pen pen = new Pen(Color.Silver))
                        {
                            g.DrawLines(pen, new Point[] {
                                new Point(clientRectangle.X, clientRectangle.Bottom),
                                new Point(clientRectangle.X, clientRectangle.Y),
                                new Point(clientRectangle.Right - 1, clientRectangle.Y),
                                new Point(clientRectangle.Right - 1, clientRectangle.Bottom) });
                            //g.DrawRectangle(pen, clientRectangle.X, clientRectangle.Y, clientRectangle.Width - 1, clientRectangle.Height - 1);
                        }
                    }
                }
                else if (item.Selected)  //item被选中
                {
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(120, Color.Silver)))
                    {
                        g.FillRectangle(brush, clientRectangle);
                    }
                }
                else
                {
                    base.OnRenderMenuItemBackground(e);
                }
            }
            else if (toolStrip is ToolStripDropDown)
            {
                if (item.Selected)
                {
                    using (SolidBrush pen = new SolidBrush(Color.Silver))
                    {
                        g.FillRectangle(pen, clientRectangle);
                    }
                }
                else
                {
                    base.OnRenderMenuItemBackground(e);
                }
            }
            else
            {
                base.OnRenderMenuItemBackground(e);
            }
        }

        //ToolStrip、MenuStrip、StatusStrip、ContextMenuStrip，所有image
        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (toolStrip is ToolStripDropDown && e.Item is ToolStripMenuItem)
            {
                ToolStripMenuItem item = (ToolStripMenuItem)e.Item;
                if (item.Checked)
                {
                    return;
                }
                Rectangle rect = e.ImageRectangle;
                ToolStripItemImageRenderEventArgs ne = new ToolStripItemImageRenderEventArgs(g, e.Item, e.Image, rect);
                base.OnRenderItemImage(ne);
            }
            else
            {
                if (e.Item is ToolStripButton)
                {
                    ToolStripButton item = e.Item as ToolStripButton;
                    if (item.Pressed)
                    {
                        g.DrawImage(e.Image, e.ImageRectangle);
                    }
                    else
                    {
                        base.OnRenderItemImage(e);
                    }
                }
                else if (e.Item is ToolStripSplitButton)
                {
                    base.OnRenderItemImage(e);
                }
                else if (e.Item is ToolStripDropDownButton)
                {
                    base.OnRenderItemImage(e);
                }
                else
                {
                    base.OnRenderItemImage(e);
                }
            }
        }

        //ToolStrip、MenuStrip、StatusStrip、ContextMenuStrip
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            base.OnRenderImageMargin(e);
        }

        //ToolStrip、MenuStrip、StatusStrip、ContextMenuStrip，所有text
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            e.TextColor = Color.Black;

            if (toolStrip is ToolStripDropDown && e.Item is ToolStripMenuItem)
            {
                //int offsetMargin = e.Item.DisplayStyle == ToolStripItemDisplayStyle.Text ? -30 : 0;
                int offsetMargin = 0;
                Rectangle rect = e.TextRectangle;
                if (e.Item.RightToLeft == RightToLeft.Yes)
                {
                    rect.X -= offsetMargin;
                }
                else
                {
                    rect.X += offsetMargin;
                }
                e.TextRectangle = rect;
            }

            base.OnRenderItemText(e);
        }

        //ToolStrip
        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripItem item = e.Item;
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            bool rightToLeft = item.RightToLeft == RightToLeft.Yes;
            bool bHorizontal = toolStrip.Orientation == Orientation.Horizontal;
            Rectangle empty = Rectangle.Empty;
            Rectangle rect = new Rectangle(Point.Empty, item.Size);

            if (bHorizontal)
            {
                rect.Inflate(-3, 0);
                rect.Offset(rightToLeft ? -3 : 3, 0);
            }
            else
            {
                rect.Inflate(0, -3);
                rect.Offset(0, 3);
            }

            if (rightToLeft)
            {
                empty = new Rectangle(0, item.Height - 8, 10, 5);
            }
            else
            {
                empty = new Rectangle(item.Width - 12, item.Height - 8, 10, 5);
            }
            ArrowDirection direction = bHorizontal ? ArrowDirection.Down : ArrowDirection.Right;
            int x = (rightToLeft && bHorizontal) ? -1 : 1;
            empty.Offset(x, 1);

            Color arrowColor = toolStrip.Enabled ?
                _colorArrow : SystemColors.ControlDark;

            
            if (item.Pressed)
            {
                g.FillRectangle(Brushes.Green, rect);
            }
            else if (item.Selected)
            {
                g.FillRectangle(Brushes.Lime, rect);
            }

            using (Brush brush = new SolidBrush(arrowColor))
            {
                DrawRenderArrow(g, empty, direction, brush);
            }

            if (bHorizontal)
            {
                using (Pen pen = new Pen(arrowColor))
                {
                    g.DrawLine(
                        pen,
                        empty.Right - 8,
                        empty.Y - 2,
                        empty.Right - 2,
                        empty.Y - 2);
                    g.DrawLine(
                        pen,
                        empty.Right - 8,
                        empty.Y - 1,
                        empty.Right - 2,
                        empty.Y - 1);
                }
            }
            else
            {
                using (Pen pen = new Pen(arrowColor))
                {
                    g.DrawLine(
                        pen,
                        empty.X,
                        empty.Y,
                        empty.X,
                        empty.Bottom - 1);
                    g.DrawLine(
                        pen,
                        empty.X,
                        empty.Y + 1,
                        empty.X,
                        empty.Bottom);
                }
            }
        }

        //ToolStrip、ContextMenuStrip
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            base.OnRenderSeparator(e);
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripSeparator item = e.Item as ToolStripSeparator;

            if (item != null)
            {
                Graphics g = e.Graphics;
                Pen pen = new Pen(Color.Black, 1.5f);
                if (e.Vertical)
                {
                    g.DrawLine(pen,
                        item.Bounds.Width / 2, item.Bounds.Top + 6,
                        item.Bounds.Width / 2, item.Bounds.Bottom - 6);
                }
                else
                {
                    g.DrawLine(pen,
                       item.Bounds.Left + toolStrip.ImageScalingSize.Width + 15, item.Bounds.Height / 2,
                       item.Bounds.Right - 20, item.Bounds.Height / 2);
                }
            }
        }

        //ToolStrip、StatusStrip
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripButton item = e.Item as ToolStripButton;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            Rectangle bounds = new Rectangle(0, 0, item.Size.Width - 1, item.Size.Height - 1);

            if (item != null)
            {
                if (item.BackgroundImage != null)
                {
                    g.DrawImage(item.BackgroundImage, bounds);
                }

                if (item.CheckState == CheckState.Unchecked)
                {
                    if (item.Pressed)
                    {
                        g.DrawRectangle(Pens.Green, bounds);
                    }
                    else if (item.Selected)
                    {
                        g.DrawRectangle(Pens.Lime, bounds);
                    }
                }
                else if (item.CheckState == CheckState.Checked)
                {
                    if (item.Pressed)
                    {
                        g.DrawRectangle(Pens.Green, bounds);
                    }
                    else if (item.Selected)
                    {
                        g.DrawRectangle(Pens.Lime, bounds);
                    }
                    else
                    {
                        g.DrawRectangle(Pens.Lime, bounds);
                    }
                }
            }
        }

        //ToolStrip、StatusStrip
        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripSplitButton item = e.Item as ToolStripSplitButton;

            if (item != null)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.HighQuality;
                Rectangle bounds = new Rectangle(Point.Empty, item.Size);
                Color arrowColor = toolStrip.Enabled ? _colorArrow : SystemColors.ControlDark;
                arrowColor = item.Enabled ? _colorArrow : SystemColors.ControlDark;

                if (item.BackgroundImage != null)
                {
                    g.DrawImage(item.BackgroundImage, bounds);
                }

                if (item.Pressed && item.HasDropDownItems)
                {
                    
                }
                else if (item.Selected)
                {
                    
                }
                else if (toolStrip is ToolStripOverflow)
                {
                    
                }
                else
                {
                    base.OnRenderDropDownButtonBackground(e);
                }
            }

            base.OnRenderDropDownButtonBackground(e);
        }

        //ToolStrip、StatusStrip
        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripSplitButton item = e.Item as ToolStripSplitButton;

            if (item != null)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.HighQuality;
                Rectangle bounds = new Rectangle(Point.Empty, item.Size);
                Color arrowColor = toolStrip.Enabled ? _colorArrow : SystemColors.ControlDark;
                arrowColor = item.Enabled ? _colorArrow : SystemColors.ControlDark;

                if (item.BackgroundImage != null)
                {
                    g.DrawImage(item.BackgroundImage, bounds);
                }

                if (item.ButtonPressed)
                {
                    g.DrawRectangle(Pens.Green, item.ButtonBounds.X, item.ButtonBounds.Y,
                            item.ButtonBounds.Width - 1, item.ButtonBounds.Height - 1);
                }
                else if (item.Pressed || item.DropDownButtonPressed)
                {
                    g.DrawRectangle(Pens.Green, item.DropDownButtonBounds.X, item.DropDownButtonBounds.Y,
                            item.DropDownButtonBounds.Width - 1, item.DropDownButtonBounds.Height - 1);
                }
                else if (item.Selected || item.ButtonSelected||item.DropDownButtonSelected)
                {
                    g.DrawRectangle(Pens.Lime, 0, 0, item.Bounds.Width - 1, item.Bounds.Height - 1);
                }

                base.DrawArrow(
                   new ToolStripArrowRenderEventArgs(
                   g,
                   item,
                   item.DropDownButtonBounds,
                   arrowColor,
                   ArrowDirection.Down));
                return;
            }

            base.OnRenderSplitButtonBackground(e);
        }

        internal static void DrawRenderArrow(Graphics g, Rectangle dropDownRect, ArrowDirection direction, Brush brush)
        {
            Point point = new Point(
                dropDownRect.Left + (dropDownRect.Width / 2),
                dropDownRect.Top + (dropDownRect.Height / 2));
            Point[] points = null;
            switch (direction)
            {
                case ArrowDirection.Left:
                    points = new Point[] {
                        new Point(point.X + 2, point.Y - 3),
                        new Point(point.X + 2, point.Y + 3),
                        new Point(point.X - 1, point.Y) };
                    break;

                case ArrowDirection.Up:
                    points = new Point[] {
                        new Point(point.X - 3, point.Y + 2),
                        new Point(point.X + 3, point.Y + 2),
                        new Point(point.X, point.Y - 2) };
                    break;

                case ArrowDirection.Right:
                    points = new Point[] {
                        new Point(point.X - 2, point.Y - 3),
                        new Point(point.X - 2, point.Y + 3),
                        new Point(point.X + 1, point.Y) };
                    break;

                default:
                    points = new Point[] {
                        new Point(point.X - 3, point.Y - 1),
                        new Point(point.X + 3, point.Y - 1),
                        new Point(point.X, point.Y + 2) };
                    break;
            }
            g.FillPolygon(brush, points);
        }

    }

}

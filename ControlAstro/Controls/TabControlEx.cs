using ControlAstro.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ControlAstro.Controls
{
    [ToolboxBitmap(typeof(TabControl))]
    public class TabControlEx : TabControl
    {

        private Color backColor = SystemColors.Control;
        Image icon;
        //Image tabBackground;
        ImageAttributes ia;
        ColorMatrix cm;

        //图像灰度化的矩阵
        float[][] colorMatrix ={
            new float[]{0.299f, 0.299f, 0.299f, 0, 0},
            new float[]{0.587f, 0.587f, 0.587f, 0, 0},
            new float[]{0.114f, 0.114f, 0.114f, 0, 0},
            new float[]{0, 0, 0, 1, 0},
            new float[]{0, 0, 0, 1, 0}};

        public TabControlEx()
        {
            setStyles();

            ia = new ImageAttributes();
            cm = new ColorMatrix(colorMatrix);
            ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);  //未选中时图片的灰度化

            icon = new Bitmap(1, 1);
            //tabBackground = Properties.Resources.TabButtonBackground;
        }

        private void setStyles()
        {
            base.SetStyle(
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor,
                 true);
            base.UpdateStyles();
        }

        [Browsable(false)]
        public new TabDrawMode DrawMode
        {
            get
            {
                return base.DrawMode;
            }
            set
            {
                base.DrawMode = value;
            }
        }
        
        [Browsable(false)]
        public new TabAppearance Appearance
        {
            get
            {
                return base.Appearance;
            }
            set
            {
                base.Appearance = value;
            }
        }

        [Browsable(true)]
        public new Color BackColor
        {
            get
            {
                return this.backColor;
            }
            set
            {
                this.backColor = value;
                this.Invalidate();
            }
        }

        // 计算控件底线
        private void setLowBound(Control container)
        {
            int tmp = 0;
            Control c = null;
            for (int i = 0; i < container.Controls.Count; i++)
            {
                c = container.Controls[i];
                if (tmp < c.Bottom)
                    tmp = c.Bottom;
            }

            container.Tag = tmp;
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            setLowBound(e.Control);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.FillRectangle(new SolidBrush(this.BackColor), e.ClipRectangle);
            
            for (int i = 0; i < this.TabCount; i++)
            {
                bool flag1 = (Alignment == TabAlignment.Top || Alignment == TabAlignment.Bottom) ? true : false;
                bool flag2 = this.SelectedIndex == i ? true : false;
                Rectangle rectangle = this.GetTabRect(i);
                rectangle.Offset(-2, -2);
                TextFormatFlags format = AlignmentTools.TabAlignment2Flag(this.Alignment);
                SizeF textSize = TextRenderer.MeasureText(this.TabPages[i].Text, this.Font, rectangle.Size, format);

                if (this.ImageList != null && (this.TabPages[i].ImageIndex > -1 || !string.IsNullOrEmpty(this.TabPages[i].ImageKey)))
                {
                    if (this.TabPages[i].ImageIndex > -1)
                    {
                        icon = this.ImageList.Images[this.TabPages[i].ImageIndex];
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(this.TabPages[i].ImageKey))
                        {
                            icon = this.ImageList.Images[this.TabPages[i].ImageKey];
                        }
                    }

                    if (!flag2)
                    {
                        g.DrawImage(
                            icon,
                            new Rectangle(
                                this.GetTabRect(i).X + (this.GetTabRect(i).Width - icon.Width) / 2,
                                this.GetTabRect(i).Y + 4,
                                icon.Width,
                                icon.Height),
                            0,
                            0,
                            icon.Width,
                            icon.Height,
                            GraphicsUnit.Pixel,
                            ia);
                    }
                    else
                    {
                        //g.DrawImage(tabBackground, rectangle.X - 2, rectangle.Y - 2, rectangle.Width, rectangle.Height);
                        g.DrawImage(
                            icon,
                            this.GetTabRect(i).X + (this.GetTabRect(i).Width - icon.Width) / 2 - 2,
                            this.GetTabRect(i).Y + 4 - 2);
                    }
                }
                else
                {
                    if (flag2)
                    {
                        if (flag1)
                        {
                            g.FillRectangle(Brushes.Lime, Convert.ToInt32((rectangle.Width - textSize.Width) / 2) + rectangle.X, rectangle.Height - 2,
                                textSize.Width - 5, 2);
                        }
                        else
                        {
                            g.FillRectangle(Brushes.Lime, rectangle.Width - 2, Convert.ToInt32((rectangle.Height - textSize.Height) / 2) + rectangle.Y,
                                2, textSize.Height);
                        }
                    }
                }
                
                if (!this.TabPages[i].Text.Equals(string.Empty))
                {
                    TextRenderer.DrawText(g, this.TabPages[i].Text, this.Font, rectangle, SystemColors.ControlLightLight, format);
                    rectangle.Offset(-1, 0);
                    TextRenderer.DrawText(g, this.TabPages[i].Text, this.Font, rectangle, SystemColors.ControlText, format);
                }

                
            }
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                Rectangle rect = base.DisplayRectangle;
                return new Rectangle(rect.Left - 4, rect.Top - 4, rect.Width + 8, rect.Height + 8);
            }
        }

    }
}

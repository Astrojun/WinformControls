using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using ControlAstro.Properties;

namespace ControlAstro.Controls
{
    [ToolboxBitmap(typeof(TextBox))]
    public partial class TextBoxSearch : UserControl
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        const int WM_PAINT = 0x000F;
        const int WM_NCPAINT = 0x0085;
        const int EDIT_DELAY = 500;

        private Color borderColor = Color.Silver;
        private string hintText = string.Empty;
        private Size textSize;
        private System.Timers.Timer time;

        [DefaultValue(typeof(Color), "Gainsboro")]
        public Color BorderColor
        {
            get { return this.borderColor; }
            set 
            {
                this.borderColor = value;
                this.Invalidate();
            }
        }

        [Description("水印文本")]
        public string HintText
        {
            get { return this.textBox.HintText; }
            set
            {
                this.textBox.HintText = value;
                base.OnTextChanged(EventArgs.Empty);
            }
        }

        [Browsable(true)]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                base.OnFontChanged(EventArgs.Empty);
            }
        }

        [Browsable(true)]
        public new string Text
        {
            get
            {
                return this.textBox.Text;
            }
            set
            {
                this.textBox.Text = value;
            }
        }

        [Browsable(true)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                base.OnBackColorChanged(EventArgs.Empty);
            }
        }

        [Browsable(true)]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                base.OnForeColorChanged(EventArgs.Empty);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Size MaximumSize
        {
            get
            {
                return base.MaximumSize;
            }
            set
            {
                base.MaximumSize = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Size MinimumSize
        {
            get
            {
                return base.MinimumSize;
            }
            set
            {
                base.MinimumSize = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Padding Padding
        {
            get { return base.Padding; }
            set { base.Padding = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new BorderStyle BorderStyle
        {
            get
            {
                return base.BorderStyle;
            }
            set
            {
                base.BorderStyle = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AutoScroll
        {
            get
            {
                return base.AutoScroll;
            }
            set
            {
                base.AutoScroll = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Point AutoScrollOffset
        {
            get
            {
                return base.AutoScrollOffset;
            }
            set
            {
                base.AutoScrollOffset = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Size AutoScrollMargin
        {
            get;
            set;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Size AutoScrollMinSize
        {
            get;
            set;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new AutoSizeMode AutoSizeMode
        {
            get;
            set;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new AutoValidate AutoValidate
        {
            get;
            set;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        [Browsable(true), Description("当Text属性更改时发生")]
        public new event EventHandler TextChanged
        {
            add
            {
                base.TextChanged += value;
            }
            remove
            {
                base.TextChanged -= value;
            }
        }

        [Browsable(true), Description("当用户停止输入Text时发生(默认延迟事件为500毫秒)")]
        public event EventHandler EndEdit;

        public TextBoxSearch()
        {
            InitializeComponent();

            this.textBox.Font = this.Font;
            this.textBox.ForeColor = this.ForeColor;
            this.textBox.BackColor = this.BackColor = Color.FromArgb(239, 239, 239);

            textSize = TextRenderer.MeasureText("测试", this.Font, this.textBox.ClientSize);

            this.Width = 120;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MaximumSize = new Size(10000, textSize.Height + 8);
            this.MinimumSize = new Size(10, textSize.Height + 8);
            this.Height = textSize.Height + 8;

            this.button.Height = this.button.Width = this.Height;
            this.button.Location = new Point(Width - button.Width, 0);

            time = new System.Timers.Timer(EDIT_DELAY);
            time.Elapsed += new System.Timers.ElapsedEventHandler(time_Elapsed);
        }

        private void time_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine();
            if (this.EndEdit != null)
            {
                if (this.InvokeRequired)
                    this.Invoke(EndEdit, new object[] { this, EventArgs.Empty });
                else
                    EndEdit(this, EventArgs.Empty);
            }
            if (time.Enabled)
                time.Stop();
        }

        protected override void OnResize(EventArgs e)
        {
            this.button.Height = this.button.Width = this.Height - 2;
            this.button.Location = new Point(Width - button.Width, 0);
            this.textBox.Width = this.Width - this.button.Width - Padding.Left - 6;
        }

        protected sealed override void OnBackColorChanged(EventArgs e)
        {
            this.textBox.BackColor = this.BackColor;
        }

        protected sealed override void OnForeColorChanged(EventArgs e)
        {
            this.textBox.ForeColor = this.ForeColor;
        }

        protected sealed override void OnFontChanged(EventArgs e)
        {
            textSize = TextRenderer.MeasureText("测试", this.Font, this.textBox.ClientSize);
            this.textBox.Font = this.Font;
            this.textBox.HintFont = new Font(Font.FontFamily, Font.Size - 1);
            this.MaximumSize = new Size(10000, textSize.Height + 8);
            this.MinimumSize = new Size(10, textSize.Height + 8);
            this.Height = textSize.Height + 8;
        }

        protected sealed override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_PAINT || m.Msg == WM_NCPAINT)
            {
                IntPtr hDC = GetWindowDC(m.HWnd);
                if (hDC.ToInt32() == 0)
                {
                    return;
                }
                using (Graphics g = Graphics.FromHdc(hDC))
                {
                    using (Pen pen = new Pen(this.BorderColor, 1))
                    {
                        g.DrawRectangle(pen, 0, 0, Size.Width - 1, Size.Height - 1);
                    }
                }

                //返回结果 
                m.Result = IntPtr.Zero;
                //释放 
                ReleaseDC(m.HWnd, hDC);
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (!time.Enabled)
            {
                time.Start();
            }
            else
            {
                time.Stop();
                time.Start();
            }
            if (textBox.Text != null && textBox.Text != string.Empty)
            {
                this.button.BackgroundImage = Resources.btDelete;
                this.button.Cursor = Cursors.Hand;
            }
            else
            {
                this.button.BackgroundImage = Resources.btSearch;
                this.button.Cursor = Cursors.Default;
            }
            base.OnTextChanged(e);
        }

        private void button_MouseClick(object sender, MouseEventArgs e)
        {
            if (button.Cursor == Cursors.Hand)
            {
                this.textBox.Text = string.Empty;
            }
        }

    }
}

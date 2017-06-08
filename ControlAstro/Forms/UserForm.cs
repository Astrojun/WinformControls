using ControlAstro.Controls;
using ControlAstro.Drawing;
using ControlAstro.Enums.ControlEnums;
using ControlAstro.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ControlAstro.Forms
{
    public partial class UserForm : Form
    {

        private bool isInitialized = false;
        private int borderWidth = 1;
        private Dictionary<WindowButtons, Label> windowButtonList;
        private float fx = 1;
        private float fy = 1;
        private float fw = 1;
        private float fh = 1;

        [Description("背景色旋转角度"), Browsable(false)]
        public float BackAngle { get; set; }

        private LinearGradientBrush backBrush;
        [Description("渐变背景色"), Browsable(false)]
        public LinearGradientBrush BackBrush
        {
            get { return backBrush; }
            set
            {
                RectangleF rect = value.Rectangle;
                fx = rect.X / ClientRectangle.Width;
                fy = rect.Y / ClientRectangle.Height;
                fw = rect.Width / ClientRectangle.Width;
                fh = rect.Height / ClientRectangle.Height;
                backBrush = value;
            }
        }

        private bool isResizable = true;
        [Description("窗口大小是否可变")]
        public bool Resizable
        {
            get { return isResizable; }
            set
            {
                isResizable = value;
                if (!value) { MaximizeBox = value; }
            }
        }

        private bool isMovable = true;
        [Description("窗体是否可移动")]
        public bool Movable
        {
            get { return isMovable; }
            set { isMovable = value; }
        }

        private bool showHeader = true;
        [Description("是否显示窗体标题")]
        public bool ShowHeader
        {
            get { return showHeader; }
            set
            {
                showHeader = value;
                Padding = new Padding(0);
                Invalidate();
            }
        }

        private BorderState showBorder = BorderState.Left | BorderState.Bottom | BorderState.Right | BorderState.Top;
        [Description("是否显示边框"), DefaultValue(BorderState.Left | BorderState.Bottom | BorderState.Right | BorderState.Top),
            RefreshProperties(RefreshProperties.Repaint)]
        public BorderState ShowBorder
        {
            get { return showBorder; }
            set
            {
                showBorder = value;
                Invalidate();
            }
        }

        private Color borderColor = Color.Black;
        [Description("边框颜色")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        private Color controlBoxBackColor = Color.Transparent;
        [Description("窗体菜单框背景色")]
        public Color ControlBoxBackColor
        {
            get { return controlBoxBackColor; }
            set
            {
                controlBoxBackColor = value;
                Invalidate();
            }
        }

        private Color controlBoxForeColor = SystemColors.ControlText;
        [Description("窗体菜单框前景色")]
        public Color ControlBoxForeColor
        {
            get { return controlBoxForeColor; }
            set
            {
                controlBoxForeColor = value;
                Invalidate();
            }
        }

        public bool showHeaderBackground = false;
        [Description("是否显示标题头背景")]
        public bool ShowHeaderBackground
        {
            get { return showHeaderBackground; }
            set
            {
                showHeaderBackground = value;
                if(value)
                {
                    Padding = new Padding(1, 26, 1, 1);
                }
                else
                {
                    Padding = new Padding(1);
                }
                Invalidate();
            }
        }

        private Color headerBackground = SystemColors.Control;
        [Description("窗体菜单框前景色")]
        public Color HeaderBackground
        {
            get { return headerBackground; }
            set
            {
                headerBackground = value;
                Invalidate();
            }
        }

        private Color headerForeColor = SystemColors.ControlText;
        [Description("窗体菜单框前景色")]
        public Color HeaderForeColor
        {
            get { return headerForeColor; }
            set
            {
                headerForeColor = value;
                Invalidate();
            }
        }


        [Description("非客户区大小"), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new Padding Padding { get { return base.Padding; } set { base.Padding = value; } }

        [Browsable(false), DefaultValue(FormBorderStyle.None)]
        public new FormBorderStyle FormBorderStyle
        {
            get
            {
                return FormBorderStyle.None;
            }
            set
            {
                base.FormBorderStyle = FormBorderStyle.None;
            }
        }

        public new SizeGripStyle SizeGripStyle
        {
            get { return base.SizeGripStyle; }
            set
            {
                base.SizeGripStyle = value;
                Invalidate();
            }
        }

        private enum WindowButtons
        {
            Minimize,
            Maximize,
            Close,
        }


        public UserForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.ResizeRedraw |
                         ControlStyles.SupportsTransparentBackColor |
                         ControlStyles.UserPaint, true);
            MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            //Padding = new Padding(5, 40, 5, 5);
            Padding = new Padding(1);
            RemoveCloseButton();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //WinApi.SetClassLong(Handle, WinApi.GCL_STYLE, WinApi.GetClassLong(Handle, WinApi.GCL_STYLE) | WinApi.CS_DROPSHADOW);
        }

        private static void SetFormRoundRectRgn(Form form, int rgnRadius)
        {
            int hRgn = 0;
            hRgn = WinApi.CreateRoundRectRgn(0, 0, form.Width + 1, form.Height + 1, rgnRadius, rgnRadius);
            WinApi.SetWindowRgn(form.Handle, hRgn, true);
            WinApi.DeleteObject(hRgn);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= WinApi.CS_DROPSHADOW;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (BackgroundImage == null)
            {
                //渐变背景
                if (BackBrush != null && BackBrush != SystemBrushes.Control)
                {
                    RectangleF rect = new RectangleF(ClientRectangle.Width * fx, ClientRectangle.Height * fy, ClientRectangle.Width * fw, ClientRectangle.Height * fh);
                    e.Graphics.FillRectangle(new LinearGradientBrush(rect, BackBrush.LinearColors[0], BackBrush.LinearColors[1], BackAngle), ClientRectangle);
                }
            }

            if(ShowHeaderBackground)
            {
                e.Graphics.FillRectangle(new SolidBrush(HeaderBackground), new Rectangle(0, 0, Width, 26));
            }

            //标题头
            if (ShowHeader)
            {
                Point titlePoint = new Point(10, 3);
                titlePoint.X = ShowIcon ? 35 : 5;
                if (ShowIcon)
                {
                    ImageAttributes imgAtt = new ImageAttributes();
                    imgAtt.SetWrapMode(WrapMode.TileFlipXY);
                    using (var image = Icon.ToBitmap())
                    {
                        var rec = new Rectangle(borderWidth + 10, titlePoint.Y, 20, 20);
                        e.Graphics.DrawImage(image, rec, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imgAtt);
                    }
                }
                TextRenderer.DrawText(e.Graphics, Text, MetroFonts.Subtitle, titlePoint, HeaderForeColor);
            }

            //ControlPaint.DrawSizeGrip(e.Graphics, Color.Black, ClientRectangle.Width - 20, ClientRectangle.Height - 20, 19, 19);
            if (Resizable && SizeGripStyle == SizeGripStyle.Show)
            {
                using (SolidBrush b = new SolidBrush(Color.Black))
                {
                    Size resizeHandleSize = new Size(2, 2);
                    e.Graphics.FillRectangles(b, new Rectangle[] {
                        new Rectangle(new Point(ClientRectangle.Width-6, ClientRectangle.Height-6), resizeHandleSize),
                        new Rectangle(new Point(ClientRectangle.Width-10, ClientRectangle.Height-10), resizeHandleSize),
                        new Rectangle(new Point(ClientRectangle.Width-10, ClientRectangle.Height-6), resizeHandleSize),
                        new Rectangle(new Point(ClientRectangle.Width-6, ClientRectangle.Height-10), resizeHandleSize),
                        new Rectangle(new Point(ClientRectangle.Width-14, ClientRectangle.Height-6), resizeHandleSize),
                        new Rectangle(new Point(ClientRectangle.Width-6, ClientRectangle.Height-14), resizeHandleSize)
                    });
                }
            }

            //功能按钮
            if (DesignMode && ControlBox)
            {
                if (MinimizeBox)
                {
                    if (MaximizeBox)
                    {
                        TextRenderer.DrawText(e.Graphics, "0", new Font("Webdings", 9.25f), new Rectangle(ClientRectangle.Width - 91, 1, 30, 25),
                            ControlBoxForeColor, ControlBoxBackColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    }
                    else
                    {
                        TextRenderer.DrawText(e.Graphics, "0", new Font("Webdings", 9.25f), new Rectangle(ClientRectangle.Width - 61, 1, 30, 25),
                            ControlBoxForeColor, ControlBoxBackColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    }
                }
                if (MaximizeBox)
                {
                    TextRenderer.DrawText(e.Graphics, "1", new Font("Webdings", 9.25f), new Rectangle(ClientRectangle.Width - 61, 1, 30, 25),
                        ControlBoxForeColor, ControlBoxBackColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
                TextRenderer.DrawText(e.Graphics, "r", new Font("Webdings", 9.25f), new Rectangle(ClientRectangle.Width - 31, 1, 30, 25),
                    ControlBoxForeColor, ControlBoxBackColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
            //边框
            ButtonBorderStyle leftBorder = ShowBorder.HasFlag(BorderState.Left) ? ButtonBorderStyle.Solid : ButtonBorderStyle.None;
            ButtonBorderStyle topBorder = ShowBorder.HasFlag(BorderState.Top) ? ButtonBorderStyle.Solid : ButtonBorderStyle.None;
            ButtonBorderStyle rightBorder = ShowBorder.HasFlag(BorderState.Right) ? ButtonBorderStyle.Solid : ButtonBorderStyle.None;
            ButtonBorderStyle bottomBorder = ShowBorder.HasFlag(BorderState.Bottom) ? ButtonBorderStyle.Solid : ButtonBorderStyle.None;
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, BorderColor, 1, leftBorder, BorderColor, 1, topBorder,
                BorderColor, 1, rightBorder, BorderColor, 1, bottomBorder);

            e.Graphics.DrawArc(new Pen(BorderColor), 0, 0, 3, 3, 180, 90);
            e.Graphics.DrawArc(new Pen(BorderColor), ClientRectangle.Width - 3, 0, 3, 3, 270, 90);
            e.Graphics.DrawArc(new Pen(BorderColor), ClientRectangle.Width - 3, ClientRectangle.Height - 3, 3, 3, 0, 90);
            e.Graphics.DrawArc(new Pen(BorderColor), 0, ClientRectangle.Height - 3, 3, 3, 90, 90);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (!isInitialized)
            {
                if (ControlBox)
                {
                    AddControlBox(WindowButtons.Close);

                    if (MaximizeBox)
                        AddControlBox(WindowButtons.Maximize);

                    if (MinimizeBox)
                        AddControlBox(WindowButtons.Minimize);

                    UpdateWindowButtonPosition();
                }

                if (StartPosition == FormStartPosition.CenterScreen)
                {
                    Point initialLocation = new Point();
                    initialLocation.X = (Screen.PrimaryScreen.WorkingArea.Width - (ClientRectangle.Width + 5)) / 2;
                    initialLocation.Y = (Screen.PrimaryScreen.WorkingArea.Height - (ClientRectangle.Height + 5)) / 2;
                    Location = initialLocation;
                    base.OnActivated(e);
                }

                isInitialized = true;
            }

            //if (DesignMode) return;

            Refresh();
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            if (isInitialized)
            {
                Refresh();
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if(e.Button == MouseButtons.Left && e.Clicks == 2)
            {
                if (MaximizeBox)
                {
                    ControlBox_Click(windowButtonList[WindowButtons.Maximize], EventArgs.Empty);
                }
                else if (Resizable)
                {
                    WindowState = WindowState == FormWindowState.Normal ? WindowState = FormWindowState.Maximized : WindowState = FormWindowState.Normal;
                }
            }
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetFormRoundRectRgn(this, 2);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (!DesignMode)
            {
                if (m.Msg == (int)WinApi.Messages.WM_NCHITTEST && WindowState == FormWindowState.Normal)
                {
                    m.Result = HitTestNCA(m.HWnd, m.WParam, m.LParam);
                }
            }
        }

        /// <summary>
        /// 模拟窗体缩放
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        /// <returns></returns>
        private IntPtr HitTestNCA(IntPtr hwnd, IntPtr wparam, IntPtr lparam)
        {
            Point vPoint = PointToClient(new Point((int)lparam & 0xFFFF, (int)lparam >> 16 & 0xFFFF));
            int vPadding = 5;
            if (Resizable)
            {
                if (new Rectangle(ClientRectangle.Width - vPadding, ClientRectangle.Height - vPadding, vPadding, vPadding).Contains(vPoint))
                    return (IntPtr)WinApi.HitTest.HTBOTTOMRIGHT;
                if (new Rectangle(0, 0, vPadding, vPadding).Contains(vPoint))
                    return (IntPtr)WinApi.HitTest.HTTOPLEFT;
                if (new Rectangle(0, ClientRectangle.Height - vPadding, vPadding, vPadding).Contains(vPoint))
                    return (IntPtr)WinApi.HitTest.HTBOTTOMLEFT;
                if (new Rectangle(ClientRectangle.Width - vPadding, 0, vPadding, vPadding).Contains(vPoint))
                    return (IntPtr)WinApi.HitTest.HTTOPRIGHT;
                if (new Rectangle(0, vPadding, vPadding, ClientRectangle.Height - vPadding * 2).Contains(vPoint))
                    return (IntPtr)WinApi.HitTest.HTLEFT;
                if (new Rectangle(vPadding, 0, ClientRectangle.Width - vPadding * 2, vPadding).Contains(vPoint))
                    return (IntPtr)WinApi.HitTest.HTTOP;
                if (new Rectangle(ClientRectangle.Width - vPadding, vPadding, vPadding, ClientRectangle.Height - vPadding * 2).Contains(vPoint))
                    return (IntPtr)WinApi.HitTest.HTRIGHT;
                if (new Rectangle(vPadding, ClientRectangle.Height - vPadding, ClientRectangle.Width - vPadding * 2, vPadding).Contains(vPoint))
                    return (IntPtr)WinApi.HitTest.HTBOTTOM;
            }

            //if (new Rectangle(5, 5, ClientRectangle.Width - 10, 50).Contains(vPoint))
            //    return (IntPtr)WinApi.HitTest.HTCAPTION;

            return (IntPtr)WinApi.HitTest.HTCLIENT;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left && e.Clicks == 1 && Movable)
            {
                if (Width - borderWidth > e.Location.X && e.Location.X > borderWidth && e.Location.Y > borderWidth)
                {
                    MoveControl();
                    if (MaximizeBox && WindowState == FormWindowState.Normal)
                        windowButtonList[WindowButtons.Maximize].Text = "1";
                }
            }
        }

        private void MoveControl()
        {
            WinApi.ReleaseCapture();
            WinApi.SendMessage(Handle, (int)WinApi.Messages.WM_NCLBUTTONDOWN, (int)WinApi.HitTest.HTCAPTION, 0);
        }

        private void AddControlBox(WindowButtons button)
        {
            if (windowButtonList == null)
                windowButtonList = new Dictionary<WindowButtons, Label>();

            if (windowButtonList.ContainsKey(button))
                return;

            LabelButton newButton = new LabelButton();

            if (button == WindowButtons.Close)
            {
                newButton.Text = "r";
                newButton.Name = "btnClose";
                newButton.FlatAppearance.MouseOverBackColor = Color.Red;
                newButton.FlatAppearance.MouseDownBackColor = Color.OrangeRed;
            }
            else if (button == WindowButtons.Minimize)
            {
                newButton.Text = "0";
                newButton.Name = "btnMin";
            }
            else if (button == WindowButtons.Maximize)
            {
                newButton.Name = "btnMax";
                if (WindowState == FormWindowState.Normal)
                    newButton.Text = "1";
                else
                    newButton.Text = "2";
            }

            newButton.Tag = button;
            newButton.AutoSize = false;
            newButton.Size = new Size(30, 25);
            newButton.Font = new Font("Webdings", 9.25f);
            newButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            newButton.TextAlign = ContentAlignment.MiddleCenter;
            newButton.BackColor = ControlBoxBackColor;
            newButton.ForeColor = ControlBoxForeColor;
            newButton.TabStop = false;
            newButton.Click += new EventHandler(ControlBox_Click);
            Controls.Add(newButton);

            windowButtonList.Add(button, newButton);
        }

        private void ControlBox_Click(object sender, EventArgs e)
        {
            var btn = sender as Label;
            if (btn != null)
            {
                var btnFlag = (WindowButtons)btn.Tag;
                if (btnFlag == WindowButtons.Close)
                {
                    Close();
                }
                else if (btnFlag == WindowButtons.Minimize)
                {
                    WindowState = FormWindowState.Minimized;
                }
                else if (btnFlag == WindowButtons.Maximize)
                {
                    if (WindowState == FormWindowState.Normal)
                    {
                        WindowState = FormWindowState.Maximized;
                        btn.Text = "2";
                    }
                    else
                    {
                        WindowState = FormWindowState.Normal;
                        btn.Text = "1";
                    }
                }
            }
            Focus();
        }

        private void UpdateWindowButtonPosition()
        {
            if (!ControlBox) return;

            Dictionary<int, WindowButtons> priorityOrder = new Dictionary<int, WindowButtons>(3) { { 0, WindowButtons.Close }, { 1, WindowButtons.Maximize }, { 2, WindowButtons.Minimize } };

            Point firstButtonLocation = new Point(ClientRectangle.Width - 31, borderWidth);
            int lastDrawedButtonPosition = firstButtonLocation.X - 30;

            Control firstButton = null;

            if (windowButtonList.Count == 1)
            {
                foreach (KeyValuePair<WindowButtons, Label> button in windowButtonList)
                {
                    button.Value.Location = firstButtonLocation;
                }
            }
            else
            {
                foreach (KeyValuePair<int, WindowButtons> button in priorityOrder)
                {
                    bool buttonExists = windowButtonList.ContainsKey(button.Value);

                    if (firstButton == null && buttonExists)
                    {
                        firstButton = windowButtonList[button.Value];
                        firstButton.Location = firstButtonLocation;
                        continue;
                    }

                    if (firstButton == null || !buttonExists) continue;

                    windowButtonList[button.Value].Location = new Point(lastDrawedButtonPosition, borderWidth);
                    lastDrawedButtonPosition = lastDrawedButtonPosition - 30;
                }
            }

            Refresh();
        }

        private void RemoveCloseButton()
        {
            IntPtr hMenu = WinApi.GetSystemMenu(Handle, false);
            if (hMenu == IntPtr.Zero) return;

            int n = WinApi.GetMenuItemCount(hMenu);
            if (n <= 0) return;

            WinApi.RemoveMenu(hMenu, (uint)(n - 1), WinApi.MfByposition | WinApi.MfRemove);
            WinApi.RemoveMenu(hMenu, (uint)(n - 2), WinApi.MfByposition | WinApi.MfRemove);
            WinApi.DrawMenuBar(Handle);
        }


    }

}

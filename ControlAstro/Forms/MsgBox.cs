using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;
using FlowDirection = System.Windows.Forms.FlowDirection;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace ControlAstro.Forms
{
    [DebuggerStepThrough]
    public sealed class MsgBox : Form
    {

        #region Properties

        private static Font TitleFont = new System.Drawing.Font("Segoe UI", 18);
        private static Font MessageFont = new System.Drawing.Font("Segoe UI", 10);
        private static readonly object SyncLocker = new object();
        private static readonly Padding BoxPadding = new Padding(200, 0, 0, 200);

        private const int CS_DROPSHADOW = 0x00020000;
        private const int MinWidth = 350;
        private const int MinHeight = 250;
        private readonly Graphics _graphics;
        private const int cGrip = 16; // Grip size
        private const int cCaption = 32; // Caption bar height;

        private DialogResult _buttonResult;
        private Timer _timer;
        private Point _lastMousePos;

        private Panel _plHeader;
        private Panel _plFooter;
        private Panel _plIcon;
        private PictureBox _picIcon;
        private FlowLayoutPanel _flpButtons;
        private Label _lblTitle;
        private Label _lblMessage;
        private List<Button> _buttonCollection = new List<Button>();

        private Color BorderColor { get; set; }
        private ColorTem ColorTemelat { get; set; }

        private string BtnAbort = "终止";
        private string BtnCancel = "取消";
        private string BtnIgnore = "忽略";
        private string BtnNo = "否";
        private string BtnOK = "确定";
        private string BtnRetry = "重试";
        private string BtnYes = "是";

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool MessageBeep(uint type);

        #endregion

        #region Constructors

        [DebuggerStepThrough]
        private MsgBox()
        {
            InitializeComponent();
            ColorTemelat = new ColorTem();
            this.BackColor = ColorTemelat.BackColor;
            this.Width = 400;
            this._graphics = this.CreateGraphics();
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            var culture = CultureInfo.CurrentUICulture;

            _lblTitle = new Label
            {
                ForeColor = Color.White,
                Font = TitleFont,
                Dock = DockStyle.Top,
                Height = 50,
                RightToLeft = culture.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No
            };

            _lblMessage = new Label
            {
                ForeColor = Color.White,
                Font = MessageFont,
                Dock = DockStyle.Fill,
                RightToLeft = culture.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No
            };

            _flpButtons = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };

            _plHeader = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            _plHeader.Controls.Add(_lblMessage);
            _plHeader.Controls.Add(_lblTitle);

            _plFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Padding = new Padding(20),
                BackColor = ColorTemelat.FootBackColor,
                Height = 80
            };
            _plFooter.Controls.Add(_flpButtons);

            _picIcon = new PictureBox { Width = 32, Height = 32, Location = new Point(30, 50) };

            _plIcon = new Panel { Dock = DockStyle.Left, Padding = new Padding(20), Width = 70 };
            _plIcon.Controls.Add(_picIcon);

            var controlCollection = new List<Control>
            {
                this,
                _lblTitle,
                _lblMessage,
                _flpButtons,
                _plHeader,
                _plFooter,
                _plIcon,
                _picIcon
            };

            foreach (Control control in controlCollection)
            {
                control.MouseDown += MsgBox_MouseDown;
                control.MouseMove += MsgBox_MouseMove;
            }

            this.Controls.Add(_plHeader);
            this.Controls.Add(_plIcon);
            this.Controls.Add(_plFooter);
        }

        #endregion

        #region Methods

        [DebuggerStepThrough]
        private void MsgBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _lastMousePos = new Point(e.X, e.Y);
            }
        }

        [DebuggerStepThrough]
        private void MsgBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - _lastMousePos.X;
                this.Top += e.Y - _lastMousePos.Y;
            }
        }

        [DebuggerStepThrough]
        private void ButtonClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch (btn.Name)
            {
                case "Abort":
                    _buttonResult = DialogResult.Abort;
                    break;

                case "Retry":
                    _buttonResult = DialogResult.Retry;
                    break;

                case "Ignore":
                    _buttonResult = DialogResult.Ignore;
                    break;

                case "OK":
                    _buttonResult = DialogResult.OK;
                    break;

                case "Cancel":
                    _buttonResult = DialogResult.Cancel;
                    break;

                case "Yes":
                    _buttonResult = DialogResult.Yes;
                    break;

                case "No":
                    _buttonResult = DialogResult.No;
                    break;
            }

            this.Dispose();
        }

        [DebuggerStepThrough]
        private Size MessageSize(string message, string title)
        {
            var msgSize = GetTextSize(message, MessageFont);
            var titleSize = GetTextSize(title, TitleFont);

            var w = (int)(Math.Max(msgSize.Width, titleSize.Width) + BoxPadding.Left);
            var h = (int)(Math.Max(msgSize.Height, titleSize.Height) + BoxPadding.Bottom);

            var boxSize = new Size()
            {
                Width = MinWidth > w ? MinWidth : w,
                Height = MinHeight > h ? MinHeight : h
            };

            return boxSize;
        }

        private SizeF GetTextSize(string p, Font f)
        {
            // uncomment in vb.net:

            //var sb = new StringBuilder(p);
            //sb.Replace( vbCr, Environment.NewLine); 
            //sb.Replace(vbCrLf, Environment.NewLine); 
            //p = sb.ToString();
            //return _graphics.MeasureString(p, f);

            return _graphics.MeasureString(p, f);
        }

        [DebuggerStepThrough]
        private void timer_Tick(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;
            AnimateMsgBox animate = (AnimateMsgBox)timer.Tag;

            switch (animate.Style)
            {
                case AnimateStyle.SlideDown:
                    if (this.Height < animate.FormSize.Height)
                    {
                        this.Height += 17;
                        this.Invalidate();
                    }
                    else
                    {
                        _timer.Stop();
                        _timer.Dispose();
                    }
                    break;

                case AnimateStyle.FadeIn:
                    if (this.Opacity < 1)
                    {
                        this.Opacity += 0.1;
                        this.Invalidate();
                    }
                    else
                    {
                        _timer.Stop();
                        _timer.Dispose();
                    }
                    break;

                case AnimateStyle.ZoomIn:
                    if (this.Width > animate.FormSize.Width)
                    {
                        this.Width -= 17;
                        this.Invalidate();
                    }
                    if (this.Height > animate.FormSize.Height)
                    {
                        this.Height -= 17;
                        this.Invalidate();
                    }
                    if (this.Width <= animate.FormSize.Width && this.Height <= animate.FormSize.Height)
                    {
                        _timer.Stop();
                        _timer.Dispose();
                    }
                    break;
            }
        }

        [DebuggerStepThrough]
        private void InitButtons(Buttons buttons)
        {
            switch (buttons)
            {
                case Buttons.AbortRetryIgnore:
                    this.InitAbortRetryIgnoreButtons();
                    break;

                case Buttons.OK:
                    this.InitOkButton();
                    break;

                case Buttons.OKCancel:
                    this.InitOkCancelButtons();
                    break;

                case Buttons.RetryCancel:
                    this.InitRetryCancelButtons();
                    break;

                case Buttons.YesNo:
                    this.InitYesNoButtons();
                    break;

                case Buttons.YesNoCancel:
                    this.InitYesNoCancelButtons();
                    break;
            }

            foreach (Button btn in this._buttonCollection)
            {
                btn.ForeColor = Color.FromArgb(170, 170, 170);
                btn.Font = new System.Drawing.Font("Segoe UI", 8);
                btn.Padding = new Padding(3);
                btn.FlatStyle = FlatStyle.Flat;
                btn.Height = 30;
                btn.FlatAppearance.BorderColor = Color.FromArgb(99, 99, 98);

                this._flpButtons.Controls.Add(btn);
            }
        }

        [DebuggerStepThrough]
        private void InitIcon(Icons icon)
        {
            switch (icon)
            {
                case Icons.Application:
                    this._picIcon.Image = SystemIcons.Application.ToBitmap();
                    break;

                case Icons.Exclamation:
                    this._picIcon.Image = SystemIcons.Exclamation.ToBitmap();
                    break;

                case Icons.Error:
                    this._picIcon.Image = SystemIcons.Error.ToBitmap();
                    break;

                case Icons.Info:
                    this._picIcon.Image = SystemIcons.Information.ToBitmap();
                    break;

                case Icons.Question:
                    this._picIcon.Image = SystemIcons.Question.ToBitmap();
                    break;

                case Icons.Shield:
                    this._picIcon.Image = SystemIcons.Shield.ToBitmap();
                    break;

                case Icons.Warning:
                    this._picIcon.Image = SystemIcons.Warning.ToBitmap();
                    break;
            }
        }

        [DebuggerStepperBoundary]
        [DebuggerStepThrough]
        private static void ShowForm(MsgBox msgBox)
        {
            lock (SyncLocker)
            {
                if (msgBox.IsHandleCreated)
                {
                    msgBox.BeginInvoke(new Action(() => msgBox.ShowDialog()));
                }
            }
        }

        public Form GetParentForm()
        {
            Form parent = ActiveForm;

            if (parent != null) return parent;
            else if (Application.OpenForms.Count > 0)
            {
                parent = Application.OpenForms[0];
            }

            return parent;
        }

        /// <summary>
        /// Center Message Form To WinForm Parent
        /// </summary>
        public void CenterToParent(Size finalSize)
        {
            this.StartPosition = FormStartPosition.Manual;
            Point centerPos = new Point();

            var parent = GetParentForm();
            if (parent != null) // Center to Parent
            {
                var pW = parent.Size.Width;
                var pH = parent.Size.Height;
                var pX = parent.Location.X;
                var pY = parent.Location.Y;

                centerPos.X = pX + ((pW - finalSize.Width) / 2);
                centerPos.Y = pY + ((pH - finalSize.Height) / 2);
            }
            else // Center to Screen
            {
                this.StartPosition = FormStartPosition.CenterParent;
            }

            this.Location = centerPos;
        }

        private Button GetOkButton()
        {
            Button btnOk = new Button();
            btnOk.Name = "OK";
            btnOk.Text = BtnOK;
            btnOk.Click += ButtonClick;

            return btnOk;
        }

        private Button GetCancelButton()
        {
            Button btnCancel = new Button();
            btnCancel.Name = "Cancel";
            btnCancel.Text = BtnCancel;
            btnCancel.Click += ButtonClick;

            return btnCancel;
        }

        private Button GetRetryButton()
        {
            Button btnRetry = new Button();
            btnRetry.Name = "Retry";
            btnRetry.Text = BtnRetry;
            btnRetry.Click += ButtonClick;

            return btnRetry;
        }

        private Button GetAbortButton()
        {
            Button btnAbort = new Button();
            btnAbort.Name = "Abort";
            btnAbort.Text = BtnAbort;
            btnAbort.Click += ButtonClick;

            return btnAbort;
        }

        private Button GetIgnoreButton()
        {
            Button btnIgnore = new Button();
            btnIgnore.Name = "Ignore";
            btnIgnore.Text = BtnIgnore;
            btnIgnore.Click += ButtonClick;

            return btnIgnore;
        }

        private Button GetYesButton()
        {
            Button btnYes = new Button();
            btnYes.Name = "Yes";
            btnYes.Text = BtnYes;
            btnYes.Click += ButtonClick;

            return btnYes;
        }

        private Button GetNoButton()
        {
            Button btnNo = new Button();
            btnNo.Name = "No";
            btnNo.Text = BtnNo;
            btnNo.Click += ButtonClick;

            return btnNo;
        }


        private void InitOkButton()
        {
            _buttonCollection.Add(GetOkButton());
        }

        private void InitOkCancelButtons()
        {
            this._buttonCollection.Add(GetOkButton());
            this._buttonCollection.Add(GetCancelButton());
        }

        private void InitRetryCancelButtons()
        {
            this._buttonCollection.Add(GetRetryButton());
            this._buttonCollection.Add(GetCancelButton());
        }

        private void InitAbortRetryIgnoreButtons()
        {
            this._buttonCollection.Add(GetAbortButton());
            this._buttonCollection.Add(GetRetryButton());
            this._buttonCollection.Add(GetIgnoreButton());
        }

        private void InitYesNoButtons()
        {
            this._buttonCollection.Add(GetYesButton());
            this._buttonCollection.Add(GetNoButton());
        }

        private void InitYesNoCancelButtons()
        {
            this._buttonCollection.Add(GetYesButton());
            this._buttonCollection.Add(GetNoButton());
            this._buttonCollection.Add(GetCancelButton());
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, BorderColor, ButtonBorderStyle.Solid);

            // Draw Resize rectangle:
            //Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            //ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
        }

        protected override void WndProc(ref Message m)
        {
            //if (m.Msg == 0x84)
            //{  // Trap WM_NCHITTEST
            //    Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
            //    pos = this.PointToClient(pos);
            //    if (pos.Y < cCaption)
            //    {
            //        m.Result = (IntPtr)2;  // HTCAPTION
            //        return;
            //    }
            //    if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
            //    {
            //        m.Result = (IntPtr)17; // HTBOTTOMRIGHT
            //        return;
            //    }
            //}
            base.WndProc(ref m);
        }

        #endregion

        #region Normal MessageBox.Show Methods

        [DebuggerStepThrough]
        public static DialogResult Show(string message)
        {
            return ShowDoalog(message, string.Empty, Buttons.OK, Icons.None, AnimateStyle.None, null);
        }

        [DebuggerStepThrough]
        public static DialogResult Show(string message, string title)
        {
            return ShowDoalog(message, title, Buttons.OK, Icons.None, AnimateStyle.None, null);
        }

        [DebuggerStepThrough]
        public static DialogResult Show(string message, string title, Buttons buttons)
        {
            return ShowDoalog(message, title, buttons, Icons.None, AnimateStyle.None, null);
        }

        [DebuggerStepThrough]
        public static DialogResult Show(string message, string title, Buttons buttons, Icons icon)
        {
            return ShowDoalog(message, title, buttons, icon, AnimateStyle.None, null);
        }

        [DebuggerStepThrough]
        public static DialogResult Show(string message, string title, Buttons buttons, Icons icon, AnimateStyle style)
        {
            return ShowDoalog(message, title, buttons, icon, style, null);
        }

        [DebuggerStepThrough]
        public static DialogResult Show(string message, string title, Buttons buttons, Icons icon, AnimateStyle style, ColorTem colorTem)
        {
            return ShowDoalog(message, title, buttons, icon, style, colorTem);
        }

        [DebuggerStepThrough]
        private static DialogResult ShowDoalog(string message, string title, Buttons buttons, Icons icon, AnimateStyle style, ColorTem colorTem)
        {
            var msgBox = new MsgBox();
            msgBox.ColorTemelat = colorTem ?? msgBox.ColorTemelat;
            msgBox._lblMessage.Text = message;
            msgBox._lblTitle.Text = title;
            msgBox.BackColor = colorTem.BackColor;
            msgBox.ForeColor = colorTem.ForeColor;
            msgBox._lblTitle.ForeColor = colorTem.ForeColor;
            msgBox._lblMessage.ForeColor = colorTem.ForeColor;
            msgBox._plFooter.BackColor = colorTem.FootBackColor;
            msgBox.BorderColor = colorTem.BorderColor;
            Size formSize = msgBox.MessageSize(message, title);
            msgBox.Size = formSize;
            msgBox.InitButtons(buttons);
            foreach (Button btn in msgBox._buttonCollection)
            {
                btn.ForeColor = colorTem.ButtonForeColor;
                btn.FlatAppearance.BorderColor = colorTem.ButtonBorderColor;
                btn.BackColor = colorTem.ButtonBackColor;
            }

            if (icon == Icons.None)
            {
                msgBox._plIcon.Hide();
            }
            else
            {
                msgBox.InitIcon(icon);
            }

            if(style != AnimateStyle.None)
            {
                msgBox._timer = new Timer();
                switch (style)
                {
                    case AnimateStyle.SlideDown:
                        msgBox.Size = new Size(formSize.Width, 0);
                        msgBox._timer.Interval = 1;
                        msgBox._timer.Tag = new AnimateMsgBox(formSize, style);
                        break;

                    case AnimateStyle.FadeIn:
                        msgBox.Size = formSize;
                        msgBox.Opacity = 0;
                        msgBox._timer.Interval = 20;
                        msgBox._timer.Tag = new AnimateMsgBox(formSize, style);
                        break;

                    case AnimateStyle.ZoomIn:
                        msgBox.Size = new Size(formSize.Width + 100, formSize.Height + 100);
                        msgBox._timer.Tag = new AnimateMsgBox(formSize, style);
                        msgBox._timer.Interval = 1;
                        break;
                }
                msgBox.CenterToParent(formSize);
                msgBox._timer.Tick += msgBox.timer_Tick;
                msgBox._timer.Start();
            }

            ShowForm(msgBox);
            MessageBeep(0);
            return msgBox._buttonResult;
        }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MsgBox
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MsgBox";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);

        }


        class AnimateMsgBox
        {
            public Size FormSize;
            public AnimateStyle Style;

            public AnimateMsgBox(Size formSize, AnimateStyle style)
            {
                this.FormSize = formSize;
                this.Style = style;
            }
        }

        public enum AnimateStyle
        {
            SlideDown = 1,
            FadeIn = 2,
            ZoomIn = 3,
            None,
        }

        public enum Buttons
        {
            AbortRetryIgnore = 1,
            OK = 2,
            OKCancel = 3,
            RetryCancel = 4,
            YesNo = 5,
            YesNoCancel = 6
        }

        public enum Icons
        {
            Application = 1,
            Exclamation = 2,
            Error = 3,
            Warning = 4,
            Info = 5,
            Question = 6,
            Shield = 7,
            Search = 8,
            None = 0,
        }

        public class ColorTem
        {
            private Color _defaultBackColor = Color.White;
            private Color _defaultForeColor = Color.Black;
            private Color _defaultFootBackColor = Color.Silver;
            private Color _defaultButtonBackColor = Color.Transparent;
            private Color _defaultButtonForeColor = Color.Black;
            private Color _defaultButtonBorderColor = Color.FromArgb(0, 151, 251);
            private Color _defaultBorderColor = Color.FromArgb(0, 151, 251);


            private Color backColor = Color.Empty;
            public Color BackColor
            {
                get { return backColor == Color.Empty ? _defaultBackColor : backColor; }
                set { backColor = value; }
            }

            private Color foreColor = Color.Empty;
            public Color ForeColor
            {
                get { return foreColor == Color.Empty ? _defaultForeColor : foreColor; }
                set { foreColor = value; }
            }

            private Color footBackColor = Color.Empty;
            public Color FootBackColor
            {
                get { return footBackColor == Color.Empty ? _defaultFootBackColor : footBackColor; }
                set { footBackColor = value; }
            }

            private Color buttonForeColor = Color.Empty;
            public Color ButtonForeColor
            {
                get { return buttonForeColor == Color.Empty ? _defaultButtonForeColor : buttonForeColor; }
                set { buttonForeColor = value; }
            }

            private Color borderColor = Color.Empty;
            public Color BorderColor
            {
                get { return borderColor == Color.Empty ? _defaultBorderColor : borderColor; }
                set { borderColor = value; }
            }

            private Color buttonBorderColor = Color.Empty;
            public Color ButtonBorderColor
            {
                get { return buttonBorderColor == Color.Empty ? _defaultButtonBorderColor : buttonBorderColor; }
                set { buttonBorderColor = value; }
            }

            private Color buttonBackColor = Color.Empty;
            public Color ButtonBackColor
            {
                get { return buttonBackColor == Color.Empty ? _defaultButtonBackColor : buttonBackColor; }
                set { buttonBackColor = value; }
            }

        }

    }
}

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ControlAstro.Native
{
    public class WinApi
    {
        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct TT_HITTESTINFO
        {
            internal IntPtr hwnd;
            internal POINT pt;
            internal TOOLINFO ti;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOOLINFO
        {
            internal TOOLINFO(int flags)
            {
                this.cbSize = Marshal.SizeOf(typeof(TOOLINFO));
                this.uFlags = flags;
                this.hwnd = IntPtr.Zero;
                this.uId = IntPtr.Zero;
                this.rect = new RECT(0, 0, 0, 0);
                this.hinst = IntPtr.Zero;
                this.lpszText = IntPtr.Zero;
                this.lParam = IntPtr.Zero;
            }

            public int cbSize;
            public int uFlags;
            public IntPtr hwnd;
            public IntPtr uId;
            public RECT rect;
            public IntPtr hinst;
            public IntPtr lpszText;
            public IntPtr lParam;
        }

        //[StructLayout(LayoutKind.Sequential)]
        //public struct RECT
        //{
        //    public int Left;
        //    public int Top;
        //    public int Right;
        //    public int Bottom;
        //}

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(Rectangle rect)
            {
                Left = rect.Left;
                Top = rect.Top;
                Right = rect.Right;
                Bottom = rect.Bottom;
            }

            public Rectangle Rect
            {
                get
                {
                    return new Rectangle(Left, Top, Right - Left, Bottom - Top);
                }
            }

            public Size Size
            {
                get
                {
                    return new Size(Right - Left, Bottom - Top);
                }
            }

            public static RECT FromXYWH(int x, int y, int width, int height)
            {
                return new RECT(x, y, x + width, y + height);
            }

            public static RECT FromRectangle(Rectangle rect)
            {
                return new RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public int fErase;
            public RECT rcPaint;
            public int fRestore;
            public int fIncUpdate;
            public int Reserved1;
            public int Reserved2;
            public int Reserved3;
            public int Reserved4;
            public int Reserved5;
            public int Reserved6;
            public int Reserved7;
            public int Reserved8;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMTTDISPINFO
        {
            internal NMTTDISPINFO(int flags)
            {
                this.hdr = new NMHDR(0);
                this.lpszText = IntPtr.Zero;
                this.szText = IntPtr.Zero;
                this.hinst = IntPtr.Zero;
                this.uFlags = 0;
                this.lParam = IntPtr.Zero;
            }

            internal NMHDR hdr;
            internal IntPtr lpszText;
            internal IntPtr szText;
            internal IntPtr hinst;
            internal int uFlags;
            internal IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMTTCUSTOMDRAW
        {
            internal NMCUSTOMDRAW nmcd;
            internal uint uDrawFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMHDR
        {
            internal NMHDR(int flag)
            {
                hwndFrom = IntPtr.Zero;
                idFrom = 0;
                code = 0;
            }

            internal IntPtr hwndFrom;
            internal int idFrom;
            internal int code;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMCUSTOMDRAW
        {
            internal NMHDR hdr;
            internal uint dwDrawStage;
            internal IntPtr hdc;
            internal RECT rc;
            internal IntPtr dwItemSpec;
            internal uint uItemState;
            internal IntPtr lItemlParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INITCOMMONCONTROLSEX
        {
            internal INITCOMMONCONTROLSEX(int flags)
            {
                dwSize = Marshal.SizeOf(typeof(INITCOMMONCONTROLSEX));
                dwICC = flags;
            }

            internal int dwSize;
            internal int dwICC;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BLENDFUNCTION
        {
            internal byte BlendOp;
            internal byte BlendFlags;
            internal byte SourceConstantAlpha;
            internal byte AlphaFormat;

            internal BLENDFUNCTION(byte op, byte flags, byte alpha, byte format)
            {
                BlendOp = op;
                BlendFlags = flags;
                SourceConstantAlpha = alpha;
                AlphaFormat = format;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TRACKMOUSEEVENT
        {
            internal uint cbSize;
            internal TRACKMOUSEEVENT_FLAGS dwFlags;
            internal IntPtr hwndTrack;
            internal uint dwHoverTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TCHITTESTINFO
        {
            public Point pt;
            public uint flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NCCALCSIZE_PARAMS
        {
            public RECT rect0;
            public RECT rect1;
            public RECT rect2;
            public IntPtr lppos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public ABE uEdge;
            public RECT rc;
            public int lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowPos
        {
            public int hwnd;
            public int hWndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }

        #endregion

        #region Enums

        private enum DriverStringOptions
        {
            CmapLookup = 1,
            Vertical = 2,
            Advance = 4,
            LimitSubpixel = 8,
        }

        public enum nCmdShowWindow : int
        {
            //隐藏窗口并激活其他窗口。nCmdShow = 0。
            SW_HIDE = 0,
            //激活并显示一个窗口。如果窗口被最小化或最大化，系统将其恢复到原来的尺寸和大小。应用程序在第一次显示窗口的时候应该指定此标志。nCmdShow = 1。
            SW_SHOWNORMAL = 1,
            //激活窗口并将其最小化。nCmdShow = 2。
            SW_SHOWMINIMIZED = 2,
            //激活窗口并将其最大化。nCmdShow = 3。
            SW_SHOWMAXIMIZED = 3,
            //最大化指定的窗口。nCmdShow = 3。
            SW_MAXIMIZE = 3,
            //以窗口最近一次的大小和状态显示窗口。激活窗口仍然维持激活状态。nCmdShow = 4。
            SW_SHOWNOACTIVATE = 4,
            //在窗口原来的位置以原来的尺寸激活和显示窗口。nCmdShow = 5。
            SW_SHOW = 5,
            //最小化指定的窗口并且激活在Z序中的下一个顶层窗口。nCmdShow = 6。
            SW_MINIMIZE = 6,
            //窗口最小化，激活窗口仍然维持激活状态。nCmdShow = 7。
            SW_SHOWMINNOACTIVE = 7,
            //以窗口原来的状态显示窗口。激活窗口仍然维持激活状态。nCmdShow = 8。
            SW_SHOWNA = 8,
            //激活并显示窗口。如果窗口最小化或最大化，则系统将窗口恢复到原来的尺寸和位置。在恢复最小化窗口时，应用程序应该指定这个标志。nCmdShow = 9。
            SW_RESTORE = 9,
            //依据在STARTUPINFO结构中指定的SW_FLAG标志设定显示状态，STARTUPINFO 结构是由启动应用程序的程序传递给CreateProcess函数的。nCmdShow = 10。
            SW_SHOWDEFAULT = 10,
            //在WindowNT5.0中最小化窗口，即使拥有窗口的线程被挂起也会最小化。在从其他线程最小化窗口时才使用这个参数。nCmdShow = 11。
            SW_FORCEMINIMIZE = 11,
        }

        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            WM_NULL = 0,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8
        }

        public enum TRACKMOUSEEVENT_FLAGS : uint
        {
            TME_HOVER = 1,
            TME_LEAVE = 2,
            TME_QUERY = 0x40000000,
            TME_CANCEL = 0x80000000
        }

        public enum ABM : uint
        {
            New = 0x00000000,
            Remove = 0x00000001,
            QueryPos = 0x00000002,
            SetPos = 0x00000003,
            GetState = 0x00000004,
            GetTaskbarPos = 0x00000005,
            Activate = 0x00000006,
            GetAutoHideBar = 0x00000007,
            SetAutoHideBar = 0x00000008,
            WindowPosChanged = 0x00000009,
            SetState = 0x0000000A,
        }

        public enum ABE : uint
        {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3
        }

        public enum ScrollBar
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3,
        }

        public enum HitTest
        {
            HTNOWHERE = 0,
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTGROWBOX = 4,
            HTSIZE = HTGROWBOX,
            HTMINBUTTON = 8,
            HTMAXBUTTON = 9,
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17,
            HTREDUCE = HTMINBUTTON,
            HTZOOM = HTMAXBUTTON,
            HTSIZEFIRST = HTLEFT,
            HTSIZELAST = HTBOTTOMRIGHT,
            HTTRANSPARENT = -1
        }

        public enum TabControlHitTest
        {
            TCHT_NOWHERE = 1,
        }

        public enum Messages : uint
        {
            WM_NULL = 0x0,
            WM_CREATE = 0x1,
            WM_DESTROY = 0x2,
            WM_MOVE = 0x3,
            WM_SIZE = 0x5,
            WM_ACTIVATE = 0x6,
            WM_SETFOCUS = 0x7,
            WM_KILLFOCUS = 0x8,
            WM_ENABLE = 0xa,
            WM_SETREDRAW = 0xb,
            WM_SETTEXT = 0xc,
            WM_GETTEXT = 0xd,
            WM_GETTEXTLENGTH = 0xe,
            WM_PAINT = 0xf,
            WM_CLOSE = 0x10,
            WM_QUERYENDSESSION = 0x11,
            WM_QUERYOPEN = 0x13,
            WM_ENDSESSION = 0x16,
            WM_QUIT = 0x12,
            WM_ERASEBKGND = 0x14,
            WM_SYSCOLORCHANGE = 0x15,
            WM_SHOWWINDOW = 0x18,
            WM_WININICHANGE = 0x1a,
            WM_SETTINGCHANGE = WM_WININICHANGE,
            WM_DEVMODECHANGE = 0x1b,
            WM_ACTIVATEAPP = 0x1c,
            WM_FONTCHANGE = 0x1d,
            WM_TIMECHANGE = 0x1e,
            WM_CANCELMODE = 0x1f,
            WM_SETCURSOR = 0x20,
            WM_MOUSEACTIVATE = 0x21,
            WM_CHILDACTIVATE = 0x22,
            WM_QUEUESYNC = 0x23,
            WM_GETMINMAXINFO = 0x24,
            WM_PAINTICON = 0x26,
            WM_ICONERASEBKGND = 0x27,
            WM_NEXTDLGCTL = 0x28,
            WM_SPOOLERSTATUS = 0x2a,
            WM_DRAWITEM = 0x2b,
            WM_MEASUREITEM = 0x2c,
            WM_DELETEITEM = 0x2d,
            WM_VKEYTOITEM = 0x2e,
            WM_CHARTOITEM = 0x2f,
            WM_SETFONT = 0x30,
            WM_GETFONT = 0x31,
            WM_SETHOTKEY = 0x32,
            WM_GETHOTKEY = 0x33,
            WM_QUERYDRAGICON = 0x37,
            WM_COMPAREITEM = 0x39,
            WM_GETOBJECT = 0x3d,
            WM_COMPACTING = 0x41,
            WM_COMMNOTIFY = 0x44,
            WM_WINDOWPOSCHANGING = 0x46,
            WM_WINDOWPOSCHANGED = 0x47,
            WM_POWER = 0x48,
            WM_COPYDATA = 0x4a,
            WM_CANCELJOURNAL = 0x4b,
            WM_NOTIFY = 0x4e,
            WM_INPUTLANGCHANGEREQUEST = 0x50,
            WM_INPUTLANGCHANGE = 0x51,
            WM_TCARD = 0x52,
            WM_HELP = 0x53,
            WM_USERCHANGED = 0x54,
            WM_NOTIFYFORMAT = 0x55,
            WM_CONTEXTMENU = 0x7b,
            WM_STYLECHANGING = 0x7c,
            WM_STYLECHANGED = 0x7d,
            WM_DISPLAYCHANGE = 0x7e,
            WM_GETICON = 0x7f,
            WM_SETICON = 0x80,
            WM_NCCREATE = 0x81,
            WM_NCDESTROY = 0x82,
            WM_NCCALCSIZE = 0x83,
            WM_NCHITTEST = 0x84,
            WM_NCPAINT = 0x85,
            WM_NCACTIVATE = 0x86,
            WM_GETDLGCODE = 0x87,
            WM_SYNCPAINT = 0x88,
            WM_NCMOUSEMOVE = 0xa0,
            WM_NCLBUTTONDOWN = 0xa1,
            WM_NCLBUTTONUP = 0xa2,
            WM_NCLBUTTONDBLCLK = 0xa3,
            WM_NCRBUTTONDOWN = 0xa4,
            WM_NCRBUTTONUP = 0xa5,
            WM_NCRBUTTONDBLCLK = 0xa6,
            WM_NCMBUTTONDOWN = 0xa7,
            WM_NCMBUTTONUP = 0xa8,
            WM_NCMBUTTONDBLCLK = 0xa9,
            WM_NCXBUTTONDOWN = 0xab,
            WM_NCXBUTTONUP = 0xac,
            WM_NCXBUTTONDBLCLK = 0xad,
            WM_INPUT = 0xff,
            WM_KEYFIRST = 0x100,
            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101,
            WM_CHAR = 0x102,
            WM_DEADCHAR = 0x103,
            WM_SYSKEYDOWN = 0x104,
            WM_SYSKEYUP = 0x105,
            WM_SYSCHAR = 0x106,
            WM_SYSDEADCHAR = 0x107,
            WM_UNICHAR = 0x109,
            WM_KEYLAST = 0x108,
            WM_IME_STARTCOMPOSITION = 0x10d,
            WM_IME_ENDCOMPOSITION = 0x10e,
            WM_IME_COMPOSITION = 0x10f,
            WM_IME_KEYLAST = 0x10f,
            WM_INITDIALOG = 0x110,
            WM_COMMAND = 0x111,
            WM_SYSCOMMAND = 0x112,
            WM_TIMER = 0x113,
            WM_HSCROLL = 0x114,
            WM_VSCROLL = 0x115,
            WM_INITMENU = 0x116,
            WM_INITMENUPOPUP = 0x117,
            WM_MENUSELECT = 0x11f,
            WM_MENUCHAR = 0x120,
            WM_ENTERIDLE = 0x121,
            WM_MENURBUTTONUP = 0x122,
            WM_MENUDRAG = 0x123,
            WM_MENUGETOBJECT = 0x124,
            WM_UNINITMENUPOPUP = 0x125,
            WM_MENUCOMMAND = 0x126,
            WM_CHANGEUISTATE = 0x127,
            WM_UPDATEUISTATE = 0x128,
            WM_QUERYUISTATE = 0x129,
            WM_CTLCOLOR = 0x19,
            WM_CTLCOLORMSGBOX = 0x132,
            WM_CTLCOLOREDIT = 0x133,
            WM_CTLCOLORLISTBOX = 0x134,
            WM_CTLCOLORBTN = 0x135,
            WM_CTLCOLORDLG = 0x136,
            WM_CTLCOLORSCROLLBAR = 0x137,
            WM_CTLCOLORSTATIC = 0x138,
            WM_MOUSEFIRST = 0x200,
            WM_MOUSEMOVE = 0x200,
            WM_LBUTTONDOWN = 0x201,
            WM_LBUTTONUP = 0x202,
            WM_LBUTTONDBLCLK = 0x203,
            WM_RBUTTONDOWN = 0x204,
            WM_RBUTTONUP = 0x205,
            WM_RBUTTONDBLCLK = 0x206,
            WM_MBUTTONDOWN = 0x207,
            WM_MBUTTONUP = 0x208,
            WM_MBUTTONDBLCLK = 0x209,
            WM_MOUSEWHEEL = 0x20a,
            WM_XBUTTONDOWN = 0x20b,
            WM_XBUTTONUP = 0x20c,
            WM_XBUTTONDBLCLK = 0x20d,
            WM_MOUSELAST = 0x20d,
            WM_PARENTNOTIFY = 0x210,
            WM_ENTERMENULOOP = 0x211,
            WM_EXITMENULOOP = 0x212,
            WM_NEXTMENU = 0x213,
            WM_SIZING = 0x214,
            WM_CAPTURECHANGED = 0x215,
            WM_MOVING = 0x216,
            WM_POWERBROADCAST = 0x218,
            WM_DEVICECHANGE = 0x219,
            WM_MDICREATE = 0x220,
            WM_MDIDESTROY = 0x221,
            WM_MDIACTIVATE = 0x222,
            WM_MDIRESTORE = 0x223,
            WM_MDINEXT = 0x224,
            WM_MDIMAXIMIZE = 0x225,
            WM_MDITILE = 0x226,
            WM_MDICASCADE = 0x227,
            WM_MDIICONARRANGE = 0x228,
            WM_MDIGETACTIVE = 0x229,
            WM_MDISETMENU = 0x230,
            WM_ENTERSIZEMOVE = 0x231,
            WM_EXITSIZEMOVE = 0x232,
            WM_DROPFILES = 0x233,
            WM_MDIREFRESHMENU = 0x234,
            WM_IME_SETCONTEXT = 0x281,
            WM_IME_NOTIFY = 0x282,
            WM_IME_CONTROL = 0x283,
            WM_IME_COMPOSITIONFULL = 0x284,
            WM_IME_SELECT = 0x285,
            WM_IME_CHAR = 0x286,
            WM_IME_REQUEST = 0x288,
            WM_IME_KEYDOWN = 0x290,
            WM_IME_KEYUP = 0x291,
            WM_MOUSEHOVER = 0x2a1,
            WM_MOUSELEAVE = 0x2a3,
            WM_NCMOUSELEAVE = 0x2a2,
            WM_WTSSESSION_CHANGE = 0x2b1,
            WM_TABLET_FIRST = 0x2c0,
            WM_TABLET_LAST = 0x2df,
            WM_CUT = 0x300,
            WM_COPY = 0x301,
            WM_PASTE = 0x302,
            WM_CLEAR = 0x303,
            WM_UNDO = 0x304,
            WM_RENDERFORMAT = 0x305,
            WM_RENDERALLFORMATS = 0x306,
            WM_DESTROYCLIPBOARD = 0x307,
            WM_DRAWCLIPBOARD = 0x308,
            WM_PAINTCLIPBOARD = 0x309,
            WM_VSCROLLCLIPBOARD = 0x30a,
            WM_SIZECLIPBOARD = 0x30b,
            WM_ASKCBFORMATNAME = 0x30c,
            WM_CHANGECBCHAIN = 0x30d,
            WM_HSCROLLCLIPBOARD = 0x30e,
            WM_QUERYNEWPALETTE = 0x30f,
            WM_PALETTEISCHANGING = 0x310,
            WM_PALETTECHANGED = 0x311,
            WM_HOTKEY = 0x312,
            WM_PRINT = 0x317,
            WM_PRINTCLIENT = 0x318,
            WM_APPCOMMAND = 0x319,
            WM_THEMECHANGED = 0x31a,
            WM_HANDHELDFIRST = 0x358,
            WM_HANDHELDLAST = 0x35f,
            WM_AFXFIRST = 0x360,
            WM_AFXLAST = 0x37f,
            WM_PENWINFIRST = 0x380,
            WM_PENWINLAST = 0x38f,
            WM_USER = 0x400,
            WM_REFLECT = 0x2000,
            WM_APP = 0x8000
        }

        #endregion

        #region Fields

        public const int Autohide = 0x0000001;
        public const int AlwaysOnTop = 0x0000002;

        public const Int32 MfByposition = 0x400;
        public const Int32 MfRemove = 0x1000;

        public const int TCM_HITTEST = 0x1313;

        public const int CS_DROPSHADOW = 0x00020000;
        public const int GCL_STYLE = (-26);

        #endregion

        #region API Calls

        #region USER32.DLL

        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT ps);

        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT ps);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode, BestFitMapping = false)]
        public static extern IntPtr CreateWindowEx(
            int exstyle,
            string lpClassName,
            string lpWindowName,
            int dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hwndParent,
            IntPtr Menu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(
            IntPtr hInstance, int lpIconName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndAfter,
            int x,
            int y,
            int cx,
            int cy,
            uint flags);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(
            IntPtr hWnd, ref RECT r);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(
            IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(
            IntPtr hwnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(
            IntPtr hwnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hDc);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PtInRect(ref RECT lprc, Point pt);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr SetTimer(
            IntPtr hWnd,
            int nIDEvent,
            uint uElapse,
            IntPtr lpTimerFunc);

        [DllImport("user32.dll", ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool KillTimer(IntPtr hWnd, uint uIDEvent);

        [DllImport("user32.dll")]
        public static extern int SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr wnd, int msg, bool param, int lparam);

        [DllImport("user32.dll")]
        public extern static int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public extern static int SendMessage(IntPtr hWnd, int msg, int wParam, ref TOOLINFO lParam);

        [DllImport("user32.dll")]
        public extern static int SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public extern static int SendMessage(IntPtr hWnd, int msg, int wParam, ref RECT lParam);

        [DllImport("user32.dll")]
        public extern static int SendMessage(
            IntPtr hWnd,
            int msg,
            IntPtr wParam,
            [MarshalAs(UnmanagedType.LPTStr)]string lParam);

        [DllImport("user32.dll")]
        public extern static int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref NMHDR lParam);

        [DllImport("user32.dll")]
        public extern static int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        public static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll")]
        public static extern bool DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern IntPtr SHAppBarMessage(ABM dwMessage, [In] ref APPBARDATA pData);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDCEx(IntPtr hwnd, IntPtr hrgnclip, uint fdwOptions);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowScrollBar(IntPtr hWnd, int bar, int cmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowRgn(IntPtr hwnd, int hRgn, bool bRedraw);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsHungAppWindow(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
            IntPtr windowHandle,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags flags,
            uint timeout,
            out uint result);

        #endregion

        #region GDI32.DLL

        [DllImport("gdi32.dll", EntryPoint = "GdiAlphaBlend")]
        public static extern bool AlphaBlend(
            IntPtr hdcDest,
            int nXOriginDest,
            int nYOriginDest,
            int nWidthDest,
            int nHeightDest,
            IntPtr hdcSrc,
            int nXOriginSrc,
            int nYOriginSrc,
            int nWidthSrc,
            int nHeightSrc,
            BLENDFUNCTION blendFunction);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StretchBlt(
            IntPtr hDest,
            int X,
            int Y,
            int nWidth,
            int nHeight,
            IntPtr hdcSrc,
            int sX,
            int sY,
            int nWidthSrc,
            int nHeightSrc,
            int dwRop);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(
            IntPtr hdc,
            int nXDest,
            int nYDest,
            int nWidth,
            int nHeight,
            IntPtr hdcSrc,
            int nXSrc,
            int nYSrc,
            int dwRop);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDCA(
            [MarshalAs(UnmanagedType.LPStr)]string lpszDriver,
            [MarshalAs(UnmanagedType.LPStr)]string lpszDevice,
            [MarshalAs(UnmanagedType.LPStr)]string lpszOutput,
            int lpInitData);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDCW(
            [MarshalAs(UnmanagedType.LPWStr)]string lpszDriver,
            [MarshalAs(UnmanagedType.LPWStr)]string lpszDevice,
            [MarshalAs(UnmanagedType.LPWStr)]string lpszOutput,
            int lpInitData);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDC(
            string lpszDriver,
            string lpszDevice,
            string lpszOutput,
            int lpInitData);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(
            IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern int CreateRoundRectRgn(int x1, int y1, int x2, int y2, int x3, int y3);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject", CharSet = CharSet.Ansi)]
        public static extern int DeleteObject(int hObject);

        #endregion

        #region comctl32.dll

        [DllImport("comctl32.dll",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool InitCommonControlsEx(
            ref INITCOMMONCONTROLSEX iccex);

        #endregion

        #region kernel32.dll

        [DllImport("kernel32.dll")]
        public extern static int RtlMoveMemory(
            ref NMHDR destination, IntPtr source, int length);

        [DllImport("kernel32.dll")]
        public extern static int RtlMoveMemory(
            ref NMTTDISPINFO destination, IntPtr source, int length);

        [DllImport("kernel32.dll")]
        public extern static int RtlMoveMemory(
            IntPtr destination, ref NMTTDISPINFO Source, int length);

        [DllImport("kernel32.dll")]
        public extern static int RtlMoveMemory(
            ref POINT destination, ref RECT Source, int length);

        [DllImport("kernel32.dll")]
        public extern static int RtlMoveMemory(
            ref NMTTCUSTOMDRAW destination, IntPtr Source, int length);

        [DllImport("kernel32.dll")]
        public extern static int RtlMoveMemory(
            ref NMCUSTOMDRAW destination, IntPtr Source, int length);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr ResetEvent(IntPtr lpEvent);

        [DllImport("kernel32.dll")]
        public static extern IntPtr SetEvent(IntPtr lpEvent);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr intPtr);

        #endregion

        #region Gdiplus.dll

        /// <summary>
        /// 绘制字符串（通过控制每个字符的左下角坐标）
        /// </summary>
        /// <param name="graphics">graphics的native句柄</param>
        /// <param name="text">要绘制的文本</param>
        /// <param name="length">文本长度</param>
        /// <param name="font">font的native句柄</param>
        /// <param name="brush">brush的native句柄</param>
        /// <param name="positions">所有字符的坐标数组</param>
        /// <param name="flags">DriverStringOptions枚举</param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        [DllImport("Gdiplus.dll", CharSet = CharSet.Auto)]
        public static extern int GdipDrawDriverString(IntPtr graphics, string text, int length, IntPtr font, IntPtr brush,
            PointF[] positions, int flags, IntPtr matrix);
        //FieldInfo field = typeof(Graphics).GetField("nativeGraphics", BindingFlags.Instance | BindingFlags.NonPublic);
        //IntPtr hGraphics = (IntPtr)field.GetValue(g);
        //field = typeof(Font).GetField("nativeFont", BindingFlags.Instance | BindingFlags.NonPublic);
        //IntPtr hFont = (IntPtr)field.GetValue(font);
        //field = typeof(Brush).GetField("nativeBrush", BindingFlags.Instance | BindingFlags.NonPublic);
        //IntPtr hBrush = (IntPtr)field.GetValue(brush);

        #endregion

        #endregion

        #region Helper Methods

        public static int LoWord(int dwValue)
        {
            return dwValue & 0xffff;
        }

        public static int HiWord(int dwValue)
        {
            return (dwValue >> 16) & 0xffff;
        }

        #endregion

        #region 窗体Style、Exstyle

        //窗口风格(Window style)
        //WS_BORDER 有边框窗口
        //WS_CAPTION 必须和WS_BORDER风格配合，但不能与WS_DLGFRAME风格一起使用。指示窗口包含标题要部分。
        //WS_CHILD 说明窗口为子窗口，不能应用于弹出式窗口风格(WS_POPUP)。
        //WS_CHILDWINDOW 同WS_CHILD。
        //WS_CLIPCHILDREN 绘制父窗口时，不绘制子窗口的裁剪区域。使用在建立父窗口时。
        //WS_CLIPSIBLINGS 剪裁相关的子窗口，这意味着，当一个特定的子窗口接收到重绘消息时，WS_CLIPSIBLINGS风格将在子窗口要重画的区域中去掉与其它子窗口重叠的部分。（如果没有指定WS_CLIPSIBLINGS风格，并且子窗口有重叠，当你在一个子窗口的客户区绘图时，它可能会画在相邻的子窗口的客户区中。）只与WS_CHILD风格一起使用。
        //WS_DISABLED 创建一个初始状态为禁止的窗口。
        //WS_DLGFRAME 创建一个窗口，具有双重边界，但是没有标题条。
        //WS_GROUP 指定一组控件中的第一个，用户可以用箭头键在这组控件中移动。在第一个控件后面把WS_GROUP风格设置为FALSE的控件都属于这一组。下一个具有WS_GROUP风格的控件将开始下一组（这意味着一个组在下一组的开始处结束）。
        //WS_HSCROLL 创建一个具有水平滚动条的窗口。 
        //WS_ICONIC：创建一个初始状态为最小化状态的窗口。与WS_MINIMIZE风格相同。
        //WS_MAXIMIZE 创建一个最大化的窗口。
        //WS_MAXIMIZEBOX 创建一个具有最大化按钮的窗口。
        //WS_MINIMIZE 创建一个初始状态为最小化的窗口。仅与WS_OVERLAPPED风格一起使用。
        //WS_MINIMIZEBOX 创建一个具有最小化按钮的窗口。
        //WS_OVERLAPPED 创建一个重叠窗口。重叠窗口通常具有标题条和边界。
        //WS_OVERLAPPEDWINDOW 创建一个具有WS_OVERLAPPED, WS_CAPTION, WS_SYSMENU, WS_THICKFRAME, WS_MINIMIZEBOX和WS_MAXIMIZEBOX风格的重叠式窗口。
        //WS_POPUP 创建一个弹出式窗口，不能与WS_CHILD风格一起使用。
        //WS_POPUPWINDOW 创建一个具有WS_BORDER，WS_POPUP和WS_SYSMENU风格的弹出窗口。为了使控制菜单可见，必须与WS_POPUPWINDOW一起使用WS_CAPTION风格。
        //WS_SIZEBOX：创建一个可调边框的窗口，与WS_THICKFRAME风格相同。
        //WS_SYSMENU 创建一个在标题条上具有控制菜单的窗口。仅对带标题条的窗口使用。
        //WS_TABSTOP 指定了一些控件中的一个，用户可以通过TAB键来移过它。TAB键使用户移动到下一个用WS_TABSTOP风格定义的控件。
        //WS_THICKFRAME 创建一个具有厚边框的窗口，可以通过厚边框来改变窗口大小。
        //WS_TILED：产生一个层叠的窗口。一个层叠的窗口有一个标题和一个边框。与WS_OVERLAPPED风格相同。
        //WS_TILEDWINDOW:创建一个具有WS_OVERLAPPED,WS_CAPTION,WS_SYSMENU,MS_THICKFRAME风格的窗口。
        //WS_VISIBLE 创建一个最初可见的窗口。
        //WS_VSCROLL 创建一个具有垂直滚动条的窗口。
        //扩展窗口风格(Extended window style)
        //WS_EX_ACCEPTFILES 指明用这个风格创建的窗口能够接受拖放文件。
        //WS_EX_APPWINDOW - 当窗口可见时将一个顶层窗口放置在任务栏上。
        //WS_EX_CLIENTEDGE 指明窗口具有3D外观，这意味着，边框具有下沉的边界。
        //WS_EX_CONTEXTHELP 在窗口的标题条中包含问号。当用户单击问号时，鼠标光标的形状变为带指针的问号。如果用户随后单击一个子窗口，子窗口将接收到一个WM_HELP消息。
        //WS_EX_CONTROLPARENT 允许用户用TAB键遍历窗口的子窗口。
        //WS_EX_DLGMODALFRAME 指明一个具有双重边界的窗口，当你在dwStyle参数中指定了WS_CAPTION风格标志时，它可以具有标题条（可选）。
        //WS_EX_LEFT 指定窗口具有左对齐属性。这是缺省值。
        //WS_EX_LEFTSCROLLBAR 将垂直滚动条放在客户区的左边。
        //WS_EX_LTRREADING 按照从左到右的方式显示窗口文本。这是缺省方式。
        //WS_EX_MDICHILD 创建一个MDI子窗口。
        //WS_EX_NOPARENTNOTIFY 指定用这个风格创建的子窗口在被创建或销毁的时候将不向父窗口发送WM_PARENTNOTIFY消息。
        //WS_EX_OVERLAPPEDWINDOW 组合了WS_EX_CLIENTEDGE和WS_EX_WIND-OWEDGE风格。
        //WS_EX_PALETTEWINDOW 组合了WS_EX_WINDOWEDGE和WS_EX_TOPMOST风格。
        //WS_EX_RIGHT 赋予窗口右对齐属性。这与窗口类有关。
        //WS_EX_RIGHTSCROLLBAR 将垂直滚动条（如果有）放在客户区的右边。这是缺省方式。
        //WS_EX_RTLREADING 按照从右到左的顺序显示窗口文本。
        //WS_EX_STATICEDGE 创建一个具有三维边界的窗口，用于不接受用户输入的项。
        //WS_EX_TOOLWINDOW 创建一个工具窗口，目的是被用作浮动工具条。工具窗口具有标题条，比通常的标题条要短，窗口的标题是用小字体显示的。工具窗口不出现在任务条或用户按下ALT+TAB时出现的窗口中。
        //WS_EX_TOPMOST 指定用这个风格创建的窗口必须被放在所有非顶层窗口的上面，即使这个窗口已经不处于激活状态，它还是保留在最上面。应用程序可以用SetWindowsPos成员函数来加入或去掉这个属性。
        //WS_EX_TRANSPARENT 指定了用这个风格创建的窗口是透明的。这意味着，在这个窗口下面的任何窗口都不会被这个窗口挡住。用这个风格创建的窗口只有当它下面的窗口都更新过以后才接收WM_PAINT消息。
        //WS_EX_WINDOWEDGE 指定了具有凸起边框的窗口。


        //MFC 对话框不同风格
        //（BY Icer）
        //MFC对话框Border类型（None、Thin、Resizing、Dialog Frame）对应的部分属性
        //在运行中转换对话框类型需要用到的属性
        //Dialog Frame
        //WS_CAPTION | WS_POPUP | WS_SYSMENU | WS_CLIPSIBLINGS | DS_MODALFRAME
        //WS_EX_DLGMODALFRAME | WS_EX_WINDOWEDGE
        //Thin
        //比Dialog Frame缺少一个扩展属性WS_EX_DLGMODALFRAME
        //None
        //比Dialog Frame缺少WS_CAPTION | DS_MODALFRAME
        //比Dialog Frame缺少扩展属性WS_EX_DLGMODALFRAME | WS_EX_WINDOWEDGE
        //Resizing
        //比Dialog Frame多一个属性WS_THICKFRAME


        //控件
        //窗口样式
        //-------------------------
        //WS_POPUP - 弹出式窗口(不能与WS_CHILDWINDOW样式同时使用)
        //WS_CHILDWINDOW - 子窗口(不能与WS_POPUP合用)
        //WS_MINIMIZE - 最小化状态
        //WS_VISIBLE - 可见状态
        //WS_DISABLED - 不可用状态
        //WS_CLIPSIBLINGS - 使窗口排除子窗口之间的相对区域
        //WS_CLIPCHILDREN - 当在父窗口内绘图时,排除子窗口区域
        //WS_MAXIMIZE - 具有最大化按钮,须指定WS_SYSTEM样式
        //WS_CAPTION - 有标题框和边框(和WS_TILED样式相同)
        //WS_BORDER - 有单边框
        //WS_DLGFRAME - 带对话框边框样式,不带标题框
        //WS_VSCROLL - 有垂直滚动条
        //WS_HSCROLL - 有水平滚动条
        //WS_SYSMENU - 标题框上带有窗口菜单(须指定WS_CAPTION样式)
        //WS_THICKFRAME - 有可调边框(与WS_SIZEBOX样式相同)
        //WS_GROUP - 组样式,每个组的第一个控件具有WS_TABSTOP样式
        //WS_TABSTOP - 可接受TAB键焦点
        //WS_MINIMIZEBOX - 有最小化按钮
        //WS_MAXIMIZEBOX - 有最大化按钮
        //WS_OVERLAPPEDWINDOW - 具有层叠,标题框,系统菜单,可调边框,系统按钮
        //WS_POPUPWINDOW - 具有单边框,弹出式,系统菜单样式

        //扩展风格
        //-------------------------
        //WS_EX_DLGMODALFRAME - 带双层边框
        //WS_EX_NOPARENTNOTIFY - 创建/销毁时不通知父窗口
        //WS_EX_TOPMOST - 窗口置顶(停留在所有非最高层窗口的上面)
        //WS_EX_ACCEPTFILES - 可接受文件拖放
        //WS_EX_TRANSPARENT - 透明样式,在同属窗口已重画时该窗口才可重画
        //WS_EX_MDICHILD - MDI子窗口样式
        //WS_EX_TOOLWINDOW - 工具条窗口样式
        //WS_EX_WINDOWEDGE - 带凸起边缘的边框
        //WS_EX_CLIENTEDGE - 带阴影的边缘
        //WS_EX_CONTEXTHELP - 有上下文帮助样式,标题栏包含一个问号标志
        //WS_EX_RIGHT - 右对齐
        //WS_EX_RTLREADING - 窗口文本从右到左显示
        //WS_EX_LEFTSCROLLBAR - 垂直滚动条在窗口左边界
        //WS_EX_CONTROLPARENT - 允许用户使用TAB键在窗口的子窗口间搜索
        //WS_EX_STATICEDGE - 当窗口为不可用状态时创建一个三维边缘
        //WS_EX_APPWINDOW - 当窗口可见时将一个顶层窗口放置在任务栏上
        //WS_EX_OVERLAPPEDWINDOW - 带凸起边缘的边框,边缘有阴影
        //WS_EX_PALETTEWINDOW - 带立体边框,有工具条窗口样式,窗口在顶层
        //WS_EX_LAYERED - 分层或透明窗口,该样式可使用混合特效
        //WS_EX_NOINHERITLAYOUT - 子控件不继承窗体或控件的布局
        //WS_EX_LAYOUTRTL - 窗体或控件将具有从右向左的布局(因而会被镜像)
        //WS_EX_COMPOSITED - 用双缓冲从下到上绘制窗口的所有子孙(WinXP以上)
        //WS_EX_NOACTIVATE - 处于顶层但不激活


        //#32770 IDD_DLG
        //-------------------------
        //DS_ABSALIGN - 对话框的坐标为屏幕坐标(缺省为客户区坐标)
        //DS_SYSMODAL - 系统模式(仅支持16位程序),不能与DS_CONTROL同用
        //DS_LOCALEDIT - 在对话框内部为编辑框分配内存(仅支持16位程序)
        //DS_SETFONT - 可定制对话框字体
        //DS_MODALFRAME - 框架样式(不能与WS_CAPTION同用)
        //DS_NOIDLEMSG - 无空闲消息
        //DS_SETFOREGROUND - 使对话框在最前面显示
        //DS_3DLOOK - 四周有3维边框
        //DS_FIXEDSYS - 使用系统固定字体
        //DS_NOFAILCREATE - 忽略创建过程中的错误
        //DS_CONTROL - 控件模式,可作为其他对话框的子窗口
        //DS_CENTER - 在屏幕居中
        //DS_CENTERMOUSE - 在鼠标位置居中
        //DS_CONTEXTHELP - 有上下文帮助按钮
        //Edit
        //-------------------------
        //ES_LEFT - 左对齐
        //ES_CENTER - 居中对齐
        //ES_RIGHT - 右对齐
        //ES_MULTILINE - 多行
        //ES_UPPERCASE - 全部大写
        //ES_LOWERCASE - 全部小写
        //ES_PASSWORD - 密码框
        //ES_AUTOVSCROLL - 自动垂直滚动
        //ES_AUTOHSCROLL - 自动水平滚动
        //ES_NOHIDESEL - 始终显示选中部分
        //ES_OEMCONVERT - 允许ANSI和OEM互相转换
        //ES_READONLY - 只读
        //ES_WANTRETURN - 可接受回车换行
        //ES_NUMBER - 只接受数字输入
        //ToolbarWindow32
        //-------------------------
        //BTNS_BUTTON - 标准按钮
        //BTNS_SEP - 分隔条按钮
        //BTNS_CHECK - 按钮有选中和非选中两种状态,分别有不同的背景颜色
        //BTNS_GROUP - 指定一组按钮,与BTNS_CHECK样式一起使用,同组内的按钮互斥
        //BTNS_CHECKGROUP - 相当于BTNS_GROUP+BTNS_CHECK
        //BTNS_DROPDOWN - 下拉按钮
        //BTNS_AUTOSIZE - 自动改变大小
        //BTNS_NOPREFIX - 不处理前缀字符(&)
        //BTNS_SHOWTEXT - 显示按钮标题
        //BTNS_WHOLEDROPDOWN - 按钮有下拉箭头
        //TBSTYLE_TOOLTIPS - 允许出现提示窗口
        //TBSTYLE_WRAPABLE - 当工具栏太窄一行放不下时,工具栏按钮可换行
        //TBSTYLE_ALTDRAG - 允许按住ALT键的同时拖动按钮来更改按钮位置
        //TBSTYLE_FLAT - 平面工具栏,即工具栏和按钮都是透明的
        //TBSTYLE_LIST - 平面工具栏并且文字在位图右侧,须在显示前设置
        //TBSTYLE_CUSTOMERASE - 处理删除背景消息时生成NM_CUSTOMDRAW消息
        //TBSTYLE_REGISTERDROP - 鼠标经过按钮时生成通知消息以请求放置目标对象
        //TBSTYLE_TRANSPARENT - 透明工具栏,工具栏是透明的,但按钮不是
        //TBSTYLE_EX_DRAWDDARROWS - 有分隔条的下拉按钮
        //TBSTYLE_EX_MIXEDBUTTONS - 混合按钮
        //TBSTYLE_EX_HIDECLIPPEDBUTTONS - 隐藏部分省略的按钮
        //TBSTYLE_EX_DOUBLEBUFFER - 双缓冲机制(检测ToolBar是否改变)(WinXP以上)
        //CCS_TOP - 顶部对齐(工具栏控件缺省值)
        //CCS_NOMOVEY - 可以水平移动和改变大小(如指定了CCS_NORESIZE则无效)
        //CCS_BOTTOM - 底部对齐(状态栏控件缺少值)
        //CCS_NORESIZE - 禁止改变大小
        //CCS_NOPARENTALIGN - 禁止自动移动到父窗口的顶部或底部
        //CCS_ADJUSTABLE - 允许工具栏内置的定制按钮功能
        //CCS_NODIVIDER - 不在控件顶部画高亮线
        //CCS_VERT - 垂直显示
        //CCS_LEFT - 垂直并居左显示
        //CCS_RIGHT - 垂直并居右显示
        //CCS_NOMOVEX - 可以垂直移动和改变大小(如指定了CCS_NORESIZE则无效)
        //Static
        //-------------------------
        //SS_LEFT - 左对齐
        //SS_CENTER - 居中对齐
        //SS_RIGHT - 右对齐
        //SS_ICON - 显示为图标
        //SS_BLACKRECT - 黑色矩形
        //SS_GRAYRECT - 灰色矩形
        //SS_WHITERECT - 白色矩形
        //SS_BLACKFRAME - 黑色边框
        //SS_GRAYFRAME - 灰色边框
        //SS_WHITEFRAME - 白色边框
        //SS_USERITEM - 用户定义
        //SS_SIMPLE - 指定一个简单矩形并在矩形中显示一行文字
        //SS_LEFTNOWORDWRAP - 左对齐,文字不换行,超过部分省略
        //SS_OWNERDRAW - 允许自画,该控件可接受 WM_DRAWITEM 消息
        //SS_BITMAP - 显示为位图
        //SS_ENHMETAFILE - 显示为增强图元文件(WMF)
        //SS_ETCHEDHORZ - 用 EDGE_ETCHED 样式绘制上下边框
        //SS_ETCHEDVERT - 用 EDGE_ETCHED 样式绘制左右边框
        //SS_ETCHEDFRAME - 用 EDGE_ETCHED 样式绘制四周边框
        //SS_REALSIZECONTROL - 图像随控件缩放
        //SS_NOPREFIX - 不处理前缀字符(&)
        //SS_NOTIFY - 单击或双击时通知父窗口
        //SS_CENTERIMAGE - 图像居中
        //SS_RIGHTJUST - SS_BITMAP或SS_ICON样式改变大小时右边界不动
        //SS_REALSIZEIMAGE - 图像不缩放,超过部分省略
        //SS_SUNKEN - 凹边框
        //SS_ENDELLIPSIS - 结尾超出范围的字符串将省略,若超出限制后面不加圆点.
        //SS_PATHELLIPSIS - 尽可能保留'\'后的文字.并在'\'前插入圆点
        //SS_WORDELLIPSIS - 任何超出范围的字都要被省略并加三个圆点
        //Button IDC_GRP
        //-------------------------
        //BS_PUSHBUTTON - 按下弹起
        //BS_DEFPUSHBUTTON - 带默认的按钮
        //BS_CHECKBOX - 复选框
        //BS_AUTOCHECKBOX - 自动改变状态的复选框
        //BS_RADIOBUTTON - 单选框
        //BS_3STATE - 三态复选框(选中, 非选中, 灰色)
        //BS_AUTO3STATE - 自动改变状态的三态复选框
        //BS_GROUPBOX - 组框
        //BS_USERBUTTON - 此样式只为兼容16位Windows,已由BS_OWNERDRAW代替
        //BS_AUTORADIOBUTTON - 自动改变状态的单选按钮
        //BS_OWNERDRAW - 允许自画, 使其接受 WM_DRAWITEM 消息
        //BS_LEFTTEXT - 文字在左边
        //BS_TEXT - 文字标题
        //BS_ICON - 带图标
        //BS_BITMAP - 带位图
        //BS_LEFT - 左对齐
        //BS_RIGHT - 右对齐
        //BS_CENTER - 居中对齐
        //BS_TOP - 文字靠上
        //BS_BOTTOM - 文字靠下
        //BS_VCENTER - 文字垂直居中
        //BS_PUSHLIKE - 类似按下弹起
        //BS_MULTILINE - 多行文字
        //BS_NOTIFY - 允许将焦点消息通知父窗口
        //BS_FLAT - 平面
        //BS_RIGHTBUTTON - 按钮在右面,相当于BS_LEFTTEXT
        //ComboBox
        //-------------------------
        //CBS_SIMPLE - 不显示下拉按钮,总是显示列表框
        //CBS_DROPDOWN - 类似CBS_SIMPLE,只是在未点击下拉按钮以前不显示下拉列表
        //CBS_DROPDOWNLIST - 类似CBS_DROPDOWN,但不可以更改文本内容
        //CBS_OWNERDRAWFIXED - 允许自画,但行高一致
        //CBS_OWNERDRAWVARIABLE - 允许自画,行高可以不一致
        //CBS_AUTOHSCROLL - 自动滚动文本
        //CBS_OEMCONVERT - 允许OEM字符集转换
        //CBS_SORT - 自动按字母排序
        //CBS_HASSTRINGS - 指定自画样式的ComboBox包含的项目由字符串组成
        //CBS_NOINTEGRALHEIGHT - 组合框的大小可以不是项目大小的整数倍
        //CBS_DISABLENOSCROLL - 总是显示垂直滚动条
        //CBS_UPPERCASE - 全部大写
        //CBS_LOWERCASE - 全部小写
        //ScrollBar
        //-------------------------
        //SBS_HORZ - 水平滚动条
        //SBS_VERT - 垂直滚动条
        //SBS_TOPALIGN - 在指定区域内顶对齐
        //SBS_LEFTALIGN - 在指定区域内左对齐
        //SBS_BOTTOMALIGN - 在指定区域内底对齐
        //SBS_RIGHTALIGN - 在指定区域内右对齐
        //SBS_SIZEBOXTOPLEFTALIGN - 带有大小框,左上对齐
        //SBS_SIZEBOXBOTTOMRIGHTALIGN - 带有大小框,右下对齐
        //SBS_SIZEBOX - 带有大小框
        //SBS_SIZEGRIP - 类似SBS_SIZEBOX,但有凸起的边框

        //SysTabControl32
        //-------------------------
        //TCS_SCROLLOPPOSITE - 选择一个选项卡时不需要滚动对边
        //TCS_BOTTOM - 选项卡在底部
        //TCS_RIGHT - 选项卡在右边
        //TCS_MULTISELECT - 允许多选
        //TCS_FLATBUTTONS - 平面按钮样式, 四周没有边框
        //TCS_FORCEICONLEFT - 强制图标在左边
        //TCS_FORCELABELLEFT - 强制标签在左边
        //TCS_HOTTRACK - 允许热点选择
        //TCS_VERTICAL - 选项卡在左边并且文字垂直显示
        //TCS_TABS - 选项卡样式, 四周有边框, 缺省样式
        //TCS_BUTTONS - 按钮样式, 四周没有边框
        //TCS_SINGLELINE - 单行显示
        //TCS_MULTILINE - 多行显示
        //TCS_RIGHTJUSTIFY - 
        //TCS_FIXEDWIDTH - 选项卡宽度固定
        //TCS_RAGGEDRIGHT - 不允许拉伸选项卡以适应宽度
        //TCS_FOCUSONBUTTONDOWN - 点击时获取焦点
        //TCS_OWNERDRAWFIXED - 父窗口负责画出选项卡
        //TCS_TOOLTIPS - 允许出现提示信息窗口
        //TCS_FOCUSNEVER - 点击时不获取焦点
        //TCS_EX_FLATSEPARATORS - 平面分隔条
        //TCS_EX_REGISTERDROP - 注册拖放通知事件, 使用前须初始化OLE

        //ListBox
        //-------------------------
        //LBS_NOTIFY - 双击鼠标时通知父窗口
        //LBS_SORT - 自动按字母排序
        //LBS_NOREDRAW - 当改变时不重画
        //LBS_MULTIPLESEL - 允许多项选择
        //LBS_OWNERDRAWFIXED - 允许自画,但行高一致
        //LBS_OWNERDRAWVARIABLE - 允许自画,行高可不一致
        //LBS_HASSTRINGS - 指定自画样式的ListBox包含的项目由字符串组成
        //LBS_USETABSTOPS - 可接受TAB键焦点
        //LBS_NOINTEGRALHEIGHT - 列表框的大小可以不是项目大小的整数倍
        //LBS_MULTICOLUMN - 允许多列
        //LBS_WANTKEYBOARDINPUT - 拥有焦点且有按键时拥有者接收WM_VKEYTOITEM消息
        //LBS_EXTENDEDSEL - 允许使用Shift键多选
        //LBS_DISABLENOSCROLL - 总是显示垂直滚动条
        //LBS_NODATA - 无数据模式(必须指定LBS_OWNERDRAWFIXED)
        //LBS_NOSEL - 失去焦点时隐藏所选项
        //LBS_STANDARD - 标准样式:按字母排序,允许通知父窗口,有边框

        //msctls_progress32
        //-------------------------
        //PBS_SMOOTH - 平滑的滚动条(不是缺省的分段滚动条)
        //PBS_VERTICAL - 垂直方向

        //msctls_updown32
        //-------------------------
        //UDS_WRAP - 递增和递减如果超过边界则循环
        //UDS_SETBUDDYINT - 值改变时设置关联窗口的文本
        //UDS_ALIGNRIGHT - 位置在关联窗口的右边
        //UDS_ALIGNLEFT - 位置在关联窗口的左边
        //UDS_AUTOBUDDY - 自动选择上一个窗口为关联窗口
        //UDS_ARROWKEYS - 响应键盘的上下键
        //UDS_HORZ - 水平显示
        //UDS_NOTHOUSANDS - 不显示千位分隔符(',')
        //UDS_HOTTRACK - 允许热点选择

        //msctls_trackbar32
        //-------------------------
        //TBS_AUTOTICKS - 自动出现刻度
        //TBS_VERT - 垂直方向
        //TBS_HORZ - 水平方向
        //TBS_TOP - 刻度在上方
        //TBS_BOTTOM - 刻度在下方
        //TBS_LEFT - 刻度在左边
        //TBS_RIGHT - 刻度在右边
        //TBS_BOTH - 两边都有刻度
        //TBS_NOTICKS - 隐藏刻度
        //TBS_ENABLESELRANGE - 允许区域选择
        //TBS_FIXEDLENGTH - 滑动长度固定
        //TBS_NOTHUMB - 隐藏滑动块
        //TBS_TOOLTIPS - 允许提示窗口显示当前进度
        //TBS_REVERSED - 保留
        //TBS_DOWNISLEFT - 下=左,上=右(缺省是下= 右, 上= 左)
        //SysDateTimePick32
        //-------------------------
        //DTS_UPDOWN - 右边显示上下按钮(缺省显示为组合框),运行时不可更改
        //DTS_SHOWNONE - 在左侧显示一个复选框(允许没有选择)
        //DTS_SHORTDATEFORMAT - 短日期格式(年份为4位数)
        //DTS_LONGDATEFORMAT - 长日期格式(年份为4位数)
        //DTS_TIMEFORMAT - 显示时间(缺省为显示日期)
        //DTS_SHORTDATECENTURYFORMAT - 短日期格式
        //DTS_APPCANPARSE - 允许拥有者解析用户输入动作
        //DTS_RIGHTALIGN - 下拉框右对齐

        //SysTreeView32
        //-------------------------
        //TVS_HASBUTTONS - 在父项目前显示+号或-号
        //TVS_HASLINES - 显示项目间的连线
        //TVS_LINESATROOT - 根节点前显示+号或-号(忽略TVS_HASLINES样式)
        //TVS_EDITLABELS - 允许编辑标签(类似资源管理器的文件更名)
        //TVS_DISABLEDRAGDROP - 禁止拖放
        //TVS_SHOWSELALWAYS - 始终显示所选中项(包括失去焦点时)
        //TVS_RTLREADING - 从右到左阅读
        //TVS_NOTOOLTIPS - 不出现提示信息窗口
        //TVS_CHECKBOXES - 在各项目前面显示复选框
        //TVS_TRACKSELECT - 允许热点选择
        //TVS_SINGLEEXPAND - 按Ctrl键多选时鼠标点击仍然展开该项目
        //TVS_INFOTIP - 允许发送TVM_SETITEMHEIGHT消息获得信息
        //TVS_FULLROWSELECT - 允许选择整行
        //TVS_NOSCROLL - 隐藏滚动条
        //TVS_NONEVENHEIGHT - 设置项目高度为奇数

        //SysListView32
        //-------------------------
        //LVS_ICON - 大图标
        //LVS_REPORT - 详细资料
        //LVS_SMALLICON - 小图标
        //LVS_LIST - 列表
        //LVS_SINGLESEL - 只能单选
        //LVS_SHOWSELALWAYS - 始终显示所选中项(包括失去焦点时)
        //LVS_SORTASCENDING - 按升序排序
        //LVS_SORTDESCENDING - 按降序排序
        //LVS_SHAREIMAGELISTS - 共享图像列表(当控件销毁时图像列表仍可用)
        //LVS_NOLABELWRAP - 文字不换行
        //LVS_AUTOARRANGE - 自动排列
        //LVS_EDITLABELS - 允许编辑标签(类似资源管理器的文件更名)
        //LVS_OWNERDATA - 指定一个虚拟的ListView, 由用户自己管理Item数据
        //LVS_NOSCROLL - 禁止滚动条(对LVS_LIST和LVS_REPORT样式不适用)
        //LVS_ALIGNTOP - 顶端对齐
        //LVS_ALIGNLEFT - 左对齐
        //LVS_OWNERDRAWFIXED - 允许自画(仅详细资料模式)
        //LVS_NOCOLUMNHEADER - 详细资料模式时不显示列头
        //LVS_NOSORTHEADER - 列头显示为平面,不响应鼠标点击(详细资料模式)
        //LVS_EX_GRIDLINES - 显示表格线(仅详细资料模式)
        //LVS_EX_SUBITEMIMAGES - 允许子项目显示图像(仅详细资料模式)
        //LVS_EX_CHECKBOXES - 在各项目前面显示复选框
        //LVS_EX_TRACKSELECT - 允许热点选择
        //LVS_EX_HEADERDRAGDROP - 允许通过拖放列头来重新安排各列(详细资料模式)
        //LVS_EX_FULLROWSELECT - 允许选择整行(仅详细资料模式)
        //LVS_EX_ONECLICKACTIVATE - 单击项目激活
        //LVS_EX_TWOCLICKACTIVATE - 双击项目激活
        //LVS_EX_FLATSB - 平面滚动条
        //LVS_EX_REGIONAL - 项目范围只包括图标不透明区域和文字区域(大图标模式)
        //LVS_EX_INFOTIP - 将提示信息窗口的消息通知父窗口
        //LVS_EX_UNDERLINEHOT - 带热点的项目文字带下划线
        //LVS_EX_UNDERLINECOLD - 非热点的项目文字带下划线
        //LVS_EX_MULTIWORKAREAS - 多个工作区(需在没有添加项目前设置)
        //LVS_EX_LABELTIP - 如果没有提示信息文本就展开隐藏的标签
        //LVS_EX_BORDERSELECT - 用选择边框的样式代替高亮
        //LVS_EX_DOUBLEBUFFER - 双缓冲(WinXP以上)
        //LVS_EX_HIDELABELS - 隐藏标签(WinXP以上)
        //LVS_EX_SINGLEROW - 显示单行(WinXP以上)
        //LVS_EX_SNAPTOGRID - 图标自动对齐到表格(WinXP以上)
        //LVS_EX_SIMPLESELECT - (WinXP以上)

        //SysMonthCal32
        //-------------------------
        //MCS_DAYSTATE - 允许发送MCN_GETDAYSTATE通知获取日期状态
        //MCS_MULTISELECT - 允许多选
        //MCS_WEEKNUMBERS - 在左侧显示周数
        //MCS_NOTODAYCIRCLE - 不在今天的日期上显示一个圆圈
        //MCS_NOTODAY - 不显示今天

        //RichEdit20A
        //-------------------------
        //ES_LEFT - 左对齐
        //ES_CENTER - 居中对齐
        //ES_RIGHT - 右对齐
        //ES_MULTILINE - 多行
        //ES_UPPERCASE - 全部大写
        //ES_LOWERCASE - 全部小写
        //ES_PASSWORD - 密码框
        //ES_AUTOVSCROLL - 自动垂直滚动
        //ES_AUTOHSCROLL - 自动水平滚动
        //ES_NOHIDESEL - 始终显示选中部分
        //ES_OEMCONVERT - 允许ANSI和OEM互相转换
        //ES_READONLY - 只读
        //ES_WANTRETURN - 可接受回车换行
        //ES_NUMBER - 只接受数字输入
        //ES_SAVESEL - 
        //ES_SUNKEN - 
        //ES_DISABLENOSCROLL - 
        //ES_SELECTIONBAR - 
        //ES_NOOLEDRAGDROP - 
        //ENM_NONE - 
        //ENM_CHANGE - 
        //ENM_UPDATE - 
        //ENM_SCROLL - 
        //ENM_KEYEVENTS - 
        //ENM_MOUSEEVENTS - 
        //ENM_REQUESTRESIZE - 
        //ENM_SELCHANGE - 
        //ENM_DROPFILES - 
        //ENM_PROTECTED - 
        //ENM_CORRECTTEXT - 
        //ENM_SCROLLEVENTS - 
        //ENM_DRAGDROPDONE - 
        //ENM_IMECHANGE - 
        //ENM_LANGCHANGE - 
        //ENM_OBJECTPOSITIONS - 
        //ENM_LINK -

        //SysAnimate32
        //-------------------------
        //ACS_CENTER - 居中显示
        //ACS_TRANSPARENT - 透明
        //ACS_AUTOPLAY - 自动播放
        //ACS_TIMER - 使用内部定时器控制(不使用线程)

        //SysHeader32
        //-------------------------
        //HDS_HORZ - 水平显示
        //HDS_VERT - 垂直显示
        //HDS_BUTTONS - 按钮样式,可响应鼠标点击
        //HDS_HOTTRACK - 允许热点选择
        //HDS_HIDDEN - 使其高度为0,类似隐藏的效果,非真正隐藏
        //HDS_DRAGDROP - 允许通过拖放列头来重新安排各列
        //HDS_FULLDRAG - 拖动时显示列头内容
        //HDS_FILTERBAR - 包含一个过滤栏
        //HDS_FLAT - 平面样式,不响应鼠标点击(WinXP以上)
        #endregion
    }
}


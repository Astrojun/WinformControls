using ControlAstro.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ControlAstro.Controls
{
    /// <summary>
    /// Todo. Add RightToLeft Support for ReadOnlyTextbox
    /// </summary>
    [ToolboxBitmap(typeof(DataGridView))]
    public partial class DataGridViewSummary : DataGridView, ISupportInitialize
    {

        #region Browsable properties 属性

        /// <summary>
        /// If true a row header at the left side 
        /// of the summaryboxes is displayed.
        /// </summary>
        private bool displaySumRowHeader = false;
        [Browsable(true), Category("Summary"), Description("是否显示统计行头标题")]
        public bool DisplaySumRowHeader
        {
            get { return displaySumRowHeader; }
            set
            {
                displaySumRowHeader = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Text displayed in the row header
        /// of the summary row.
        /// </summary>
        private string sumRowHeaderText;
        [Browsable(true), Category("Summary"), Description("统计行头标题文本")]
        public string SumRowHeaderText
        {
            get { return sumRowHeaderText; }
            set { sumRowHeaderText = value; }
        }

        /// <summary>
        /// Text displayed in the row header
        /// of the summary row.
        /// </summary>
        private bool sumRowHeaderTextBold = false;
        [Browsable(true), Category("Summary"), Description("统计行头标题文本是否为粗体")]
        public bool SumRowHeaderTextBold
        {
            get { return sumRowHeaderTextBold; }
            set { sumRowHeaderTextBold = value; }
        }

        /// <summary>
        /// Add columns to sum up in text form
        /// </summary>
        private string[] summaryColumns;
        [Browsable(true), Category("Summary"), Description("需统计的列名")]
        public string[] SummaryColumns
        {
            get { return summaryColumns; }
            set { summaryColumns = value; }
        }

        /// <summary>
        /// Display the summary Row
        /// </summary>
        private bool summaryRowVisible;
        [Browsable(true), Category("Summary"), Description("是否显示统计行")]
        public bool SummaryRowVisible
        {
            get { return summaryRowVisible; }
            set
            {
                summaryRowVisible = value;
                if (summaryControl != null && spacePanel != null)
                {
                    summaryControl.Visible = value;
                    spacePanel.Visible = value;
                }
            }
        }

        private int summaryRowSpace = 0;
        [Browsable(true), Category("Summary"), Description("统计行与DataGridView工作区距离")]
        public int SummaryRowSpace
        {
            get { return summaryRowSpace; }
            set { summaryRowSpace = value; }
        }

        private string formatString = "F02";
        [Browsable(false), Category("Summary"), DefaultValue("F02"), Description("统计行文本默认格式化")]
        public string FormatString
        {
            get { return formatString; }
            set { formatString = value; }
        }

        [Browsable(true), Category("Summary"), Description("统计行背景色")]
        public Color SummaryRowBackColor
        {
            get { return this.summaryControl.SummaryRowBackColor; }
            set { summaryControl.SummaryRowBackColor = value; }
        }

        private Color barColor = Color.FromArgb(147, 147, 147);
        [Browsable(true), Category("外观"), DefaultValue(typeof(Color), "147, 147, 147"), Description("滚动条颜色")]
        public Color BarColor
        {
            get { return barColor; }
            set
            {
                barColor = value;
            }
        }

        /// <summary>
        /// advoid user from setting the scrollbars manually
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new ScrollBars ScrollBars
        {
            get { return base.ScrollBars; }
            set { base.ScrollBars = value; }
        }

        #endregion

        #region Declare variables 声明变量

        public event EventHandler CreateSummary;
        private VScrollBarEx vScrollBar;
        private HScrollBarEx hScrollBar;

        private SummaryControlContainer summaryControl;
        private Panel panel, spacePanel;
        private TextBox refBox;

        private int columnsWidth = 0;
        private int rowsHeight = 0;
        private int preferredWidth = 0;
        private int preferredHeight = 0;

        #endregion

        #region Constructor 构造函数

        public DataGridViewSummary()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.ScrollBars = System.Windows.Forms.ScrollBars.None;
            
            refBox = new TextBox();
            panel = new Panel();
            spacePanel = new Panel();
            hScrollBar = new HScrollBarEx();
            vScrollBar = new VScrollBarEx();

            summaryControl = new SummaryControlContainer(this);
            summaryControl.VisibilityChanged += new EventHandler(summaryControl_VisibilityChanged);

            Resize += new EventHandler(DataGridControlSum_Resize);
            ColumnAdded += new DataGridViewColumnEventHandler(DataGridControlSum_ColumnAdded);
            ColumnRemoved += new DataGridViewColumnEventHandler(DataGridControlSum_ColumnRemoved);
            RowsAdded += new DataGridViewRowsAddedEventHandler(DataGridViewSummary_RowsAdded);
            RowsRemoved += new DataGridViewRowsRemovedEventHandler(DataGridViewSummary_RowsRemoved);
            ColumnWidthChanged += new DataGridViewColumnEventHandler(DataGridViewSummary_ColumnWidthChanged);
            RowHeadersWidthChanged += new EventHandler(DataGridViewSummary_RowHeadersWidthChanged);
            RowHeightChanged += new DataGridViewRowEventHandler(DataGridViewSummary_RowHeightChanged);
            ColumnHeadersHeightChanged += new EventHandler(DataGridViewSummary_ColumnHeadersHeightChanged);
            DataBindingComplete += new DataGridViewBindingCompleteEventHandler(DataGridViewSummary_DataBindingComplete);

            hScrollBar.Scroll += new ScrollEventHandler(scrollBar_Scroll);
            hScrollBar.VisibleChanged += new EventHandler(scrollBar_VisibleChanged);
            hScrollBar.Value = 0;
            hScrollBar.Height = 10;
            hScrollBar.Visible = false;

            vScrollBar.Scroll += new ScrollEventHandler(scrollBar_Scroll);
            vScrollBar.VisibleChanged += new EventHandler(scrollBar_VisibleChanged);
            vScrollBar.Value = 0;
            vScrollBar.Width = 10;
            vScrollBar.Visible = false;
        }

        #endregion
        
        #region public functions 公共方法

        /// <summary>
        /// Refresh the summary 刷新统计行
        /// </summary>
        public void RefreshSummary()
        {
            if (this.summaryControl != null)
                this.summaryControl.RefreshSummary();
        }

        #endregion

        #region Calculate Columns, Rows and Scrollbars width 计算行高、列宽并处理行列变化事件



        private void DataGridControlSum_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            calculateColumnsWidth(false);
            summaryControl.Width = preferredWidth;
            if (hScrollBar.Visible || vScrollBar.Visible)
                adjustScrollbarToPanel();
        }

        private void DataGridControlSum_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            calculateColumnsWidth(false);
            summaryControl.Width = preferredWidth;
            if (hScrollBar.Visible || vScrollBar.Visible)
                adjustScrollbarToPanel();
        }

        private void DataGridViewSummary_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            calculateColumnsWidth(true);
            if (hScrollBar.Visible || vScrollBar.Visible)
                adjustScrollbarToPanel();
        }

        private void DataGridViewSummary_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            calculateColumnsWidth(true);
            if (hScrollBar.Visible || vScrollBar.Visible)
                adjustScrollbarToPanel();
        }

        private void DataGridViewSummary_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            adjustScrollBarToView(sender, e, false);
        }

        private void DataGridViewSummary_RowHeadersWidthChanged(object sender, EventArgs e)
        {
            adjustScrollBarToView(sender, e, false);
        }

        private void DataGridViewSummary_ColumnHeadersHeightChanged(object sender, EventArgs e)
        {
            adjustScrollBarToView(sender, e, true);
        }

        private void DataGridViewSummary_RowHeightChanged(object sender, DataGridViewRowEventArgs e)
        {
            adjustScrollBarToView(sender, e, true);
        }

        #endregion

        #region Other Events and delegates 其他事件和委托

        /// <summary>
        /// Moves viewable area of DataGridView according to the position of the scrollbar 滚动条滚动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                //按像素滚动
                HorizontalScrollingOffset = e.NewValue;
            }
            else
            {
                //按行滚动
                int vPosition = (int)Math.Ceiling(Convert.ToDouble(e.NewValue) * Convert.ToDouble(Rows.Count) / Convert.ToDouble(vScrollBar.Maximum));
                if (vPosition < Rows.Count)
                {
                    FirstDisplayedScrollingRowIndex = vPosition;
                }
            }
        }

        /// <summary>
        /// 创建统计行时发生
        /// </summary>
        public void CreateSummaryRow()
        {
            OnCreateSummary(this, EventArgs.Empty);
        }

        /// <summary>
        /// Calls the CreateSummary event 唤起创建统计行事件
        /// </summary>
        private void OnCreateSummary(object sender, EventArgs e)
        {
            if (CreateSummary != null)
                CreateSummary(sender, e);
        }

        /// <summary>
        /// 数据表调整大小时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridControlSum_Resize(object sender, EventArgs e)
        {
            if (!DesignMode && Parent != null)
            {
                resizeScrollBar();
                if(summaryControl.Visible)
                    adjustSumControlToGrid();
                if (hScrollBar.Visible || vScrollBar.Visible)
                    adjustScrollbarToPanel();
            }
        }

        /// <summary>
        /// 滚动条可见性变化时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scrollBar_VisibleChanged(object sender, EventArgs e)
        {
            if (!DesignMode && Parent != null)
            {
                if (Parent.Visible)
                {
                    this.Height = panel.Height - (summaryControl.Visible ? summaryControl.Height : 0) - 
                        (hScrollBar.Visible ? hScrollBar.Height : 0) - summaryRowSpace;
                    this.Width = panel.Width - (vScrollBar.Visible ? vScrollBar.Width : 0);
                    hScrollBar.IsGotParentMouseWheel = (hScrollBar.Visible && !vScrollBar.Visible) ? true : false;
                    vScrollBar.IsGotParentMouseWheel = vScrollBar.Visible ? true : false;
                }
            }
        }

        /// <summary>
        /// 统计行可见性变化时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void summaryControl_VisibilityChanged(object sender, EventArgs e)
        {
            this.Height = panel.Height - (summaryControl.Visible ? summaryControl.Height : 0) - summaryRowSpace;
        }

        #endregion

        #region Adjust summaryControl, scrollbar  调整统计行和滚动条属性

        /// <summary>
        /// Position the summaryControl under the DataGridView  调整统计行位置
        /// </summary>
        private void adjustSumControlToGrid()
        {
            if (summaryControl == null || Parent == null)
                return;
            summaryControl.Top = panel.Height - summaryControl.Height;
            summaryControl.Left = Left;
            summaryControl.Width = Width;
        }

        /// <summary>
        /// Position the hScrollbar under the summaryControl  调整横向滚动条适应统计行
        /// </summary>
        private void adjustScrollbarToPanel()
        {
            if (Parent != null)
            {
                if (hScrollBar.Visible)
                {
                    hScrollBar.Top = Bottom;
                    hScrollBar.Width = Width;
                    hScrollBar.Left = Left;
                }

                if (vScrollBar.Visible)
                {
                    vScrollBar.Left = Right;
                    vScrollBar.Height = Height;
                    vScrollBar.Top = Top;
                }
            }
        }

        /// <summary>
        /// 调整滚动条的属性值自适应
        /// </summary>
        private void resizeScrollBar()
        {
            if (columnsWidth > 0)
            {
                if (preferredWidth < Width || Width < preferredWidth - columnsWidth
                    || panel.Height < (ColumnHeadersVisible ? ColumnHeadersHeight : 0) + hScrollBar.Height + summaryRowSpace + (summaryControl.Visible ? summaryControl.Height : 0))
                {
                    if (hScrollBar.Visible)
                    {
                        hScrollBar.Visible = false;
                    }
                    hScrollBar.Value = hScrollBar.Minimum;
                }
                else
                {
                    if (!hScrollBar.Visible)
                    {
                        hScrollBar.Visible = true;
                    }
                }
                if (hScrollBar.Visible)
                {
                    hScrollBar.LargeChange = Width - (RowHeadersVisible ? RowHeadersWidth : 0) - Columns.GetColumnsWidth(DataGridViewElementStates.Frozen);
                    hScrollBar.SmallChange = Convert.ToInt32(hScrollBar.LargeChange / 6.0);
                    HorizontalScrollingOffset = hScrollBar.Value;
                }
            }
            else
            {
                hScrollBar.Visible = false;
            }

            if (rowsHeight > 0)
            {
                if (preferredHeight < Height || Height < preferredHeight - rowsHeight
                    || panel.Width < vScrollBar.Width + (RowHeadersVisible ? RowHeadersWidth : 0))
                {
                    if (vScrollBar.Visible)
                    {
                        vScrollBar.Visible = false;
                    }
                    vScrollBar.Value = vScrollBar.Minimum;
                }
                else
                {
                    if (!vScrollBar.Visible)
                    {
                        vScrollBar.Visible = true;
                    }
                }
                if (vScrollBar.Visible)
                {
                    vScrollBar.LargeChange = Height - (ColumnHeadersVisible ? ColumnHeadersHeight : 0) - Rows.GetRowsHeight(DataGridViewElementStates.Frozen);
                    vScrollBar.SmallChange = Convert.ToInt32(vScrollBar.LargeChange / 6.0);
                }
            }
            else
            {
                vScrollBar.Visible = false;
            }
        }

        /// <summary>
        /// scrollbar自适应行高列宽的变化
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件内容</param>
        /// <param name="isHScrollOrVScroll">true为VScrollBar变化，false为HScrollBar变化</param>
        private void adjustScrollBarToView(object sender, EventArgs e, bool isVScrollOrHScroll)
        {
            calculateColumnsWidth(isVScrollOrHScroll);
            summaryControl.Width = preferredWidth;
            resizeScrollBar();
            if (summaryControl.Visible)
                adjustSumControlToGrid();
            if (hScrollBar.Visible || vScrollBar.Visible)
                adjustScrollbarToPanel();
        }

        /// <summary>
        /// 计算行高、列宽
        /// </summary>
        /// <param name="isRowOrColumn">true为求RowsHeight，false为ColumnsWidth</param>
        private void calculateColumnsWidth(bool isRowOrColumn)
        {
            if (!isRowOrColumn)
            {
                columnsWidth = Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - Columns.GetColumnsWidth(DataGridViewElementStates.Frozen);
                hScrollBar.Maximum = columnsWidth;
                preferredWidth = Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + (RowHeadersVisible ? RowHeadersWidth : 0);
            }
            else
            {
                rowsHeight = Rows.GetRowsHeight(DataGridViewElementStates.Visible) - Rows.GetRowsHeight(DataGridViewElementStates.Frozen);
                vScrollBar.Maximum = rowsHeight;
                preferredHeight = Rows.GetRowsHeight(DataGridViewElementStates.Visible) + (ColumnHeadersVisible ? ColumnHeadersHeight : 0);
            }
        }

        private void DataGridViewSummary_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            calculateColumnsWidth(true);
            calculateColumnsWidth(false);
            resizeScrollBar();
            if (summaryControl.Visible)
                adjustSumControlToGrid();
            if (hScrollBar.Visible || vScrollBar.Visible)
                adjustScrollbarToPanel();
        }

        /// <summary>
        /// When the DataGridView is visible for the first time a panel is created.
        /// The DataGridView is then removed from the parent control and added as 
        /// child to the newly created panel   
        /// 初始化数据表和其他控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeParent()
        {
            if (!DesignMode && Parent != null)
            {
                summaryControl.InitialHeight = this.refBox.Height;
                summaryControl.Height = summaryControl.InitialHeight;
                summaryControl.BackColor = this.RowHeadersDefaultCellStyle.BackColor;
                summaryControl.ForeColor = Color.Transparent;
                summaryControl.RightToLeft = this.RightToLeft;
                summaryControl.Dock = DockStyle.Bottom;

                panel.Bounds = this.Bounds;
                panel.BackColor = this.BackgroundColor;
                panel.Dock = this.Dock;
                panel.Anchor = this.Anchor;
                panel.Padding = this.Padding;
                panel.Margin = this.Margin;
                panel.Top = this.Top;
                panel.Left = this.Left;
                panel.BorderStyle = this.BorderStyle;

                Margin = new Padding(0);
                Padding = new Padding(0);
                Top = 0;
                Left = 0;
                Dock = DockStyle.Fill;

                if (this.Parent is TableLayoutPanel)
                {
                    int rowSpan, colSpan;

                    TableLayoutPanel tlp = this.Parent as TableLayoutPanel;
                    TableLayoutPanelCellPosition cellPos = tlp.GetCellPosition(this);

                    rowSpan = tlp.GetRowSpan(this);
                    colSpan = tlp.GetColumnSpan(this);

                    tlp.Controls.Remove(this);
                    tlp.Controls.Add(panel, cellPos.Column, cellPos.Row);
                    tlp.SetRowSpan(panel, rowSpan);
                    tlp.SetColumnSpan(panel, colSpan);
                }
                else
                {
                    Control parent = this.Parent;
                    parent.Controls.Remove(this);
                    parent.Controls.Add(panel);
                }

                hScrollBar.Width = this.Width;
                hScrollBar.Left = this.Left;
                hScrollBar.BringToFront();
                hScrollBar.Dock = DockStyle.Bottom;
                hScrollBar.BarColor = barColor;

                vScrollBar.Height = this.Height;
                vScrollBar.Top = this.Top;
                vScrollBar.BringToFront();
                vScrollBar.Dock = DockStyle.Right;
                vScrollBar.BarColor = barColor;

                spacePanel = new Panel();
                spacePanel.BackColor = panel.BackColor;
                spacePanel.Height = summaryRowSpace;
                spacePanel.Dock = DockStyle.Bottom;

                panel.BringToFront();
                panel.Controls.Add(this);
                panel.Controls.Add(hScrollBar);
                panel.Controls.Add(vScrollBar);
                panel.Controls.Add(spacePanel);
                panel.Controls.Add(summaryControl);

            }
        }

        #endregion

        #region ISupportInitialzie 通知初始化事宜

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            changeParent();
        }

        #endregion


        [System.ComponentModel.ToolboxItem(false)]
        public class SummaryControlContainer : Control
        {

            #region Declare variables 声明变量

            private Hashtable sumBoxHash;
            private DataGridViewSummary dgv;
            private Label sumRowHeaderLabel;

            #endregion

            #region 属性

            private int initialHeight;
            public int InitialHeight
            {
                get { return initialHeight; }
                set { initialHeight = value; }
            }

            private bool lastVisibleState;
            public bool LastVisibleState
            {
                get { return lastVisibleState; }
                set { lastVisibleState = value; }
            }

            private Color summaryRowBackColor;
            public Color SummaryRowBackColor
            {
                get { return summaryRowBackColor; }
                set { summaryRowBackColor = value; }
            }

            #endregion

            #region 事件

            /// <summary>
            /// Event is raised when visibility changes and the
            /// lastVisibleState is not the new visible state
            /// </summary>
            public event EventHandler VisibilityChanged;

            #endregion

            #region Constructors 构造函数

            public SummaryControlContainer(DataGridViewSummary dgv)
            {
                if (dgv == null)
                    throw new Exception("DataGridView is null!");

                this.dgv = dgv;
                this.Height = dgv.RowTemplate.Height;

                sumBoxHash = new Hashtable();
                sumRowHeaderLabel = new Label();

                sumRowHeaderLabel.Anchor = AnchorStyles.Left;
                sumRowHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
                sumRowHeaderLabel.Height = dgv.RowTemplate.Height;
                sumRowHeaderLabel.Width = dgv.RowHeadersWidth;
                sumRowHeaderLabel.Top = 0;
                sumRowHeaderLabel.Padding = new Padding(0, 4, 0, 0);
                sumRowHeaderLabel.Visible = false;
                this.Controls.Add(sumRowHeaderLabel);

                this.dgv.CreateSummary += new EventHandler(dgv_CreateSummary);
                this.dgv.RowsAdded += new DataGridViewRowsAddedEventHandler(dgv_RowsAdded);
                this.dgv.RowsRemoved += new DataGridViewRowsRemovedEventHandler(dgv_RowsRemoved);
                this.dgv.CellValueChanged += new DataGridViewCellEventHandler(dgv_CellValueChanged);

                this.dgv.Scroll += new ScrollEventHandler(dgv_Scroll);
                this.dgv.ColumnWidthChanged += new DataGridViewColumnEventHandler(dgv_ColumnWidthChanged);
                this.dgv.RowHeadersWidthChanged += new EventHandler(dgv_RowHeadersWidthChanged);
                this.VisibleChanged += new EventHandler(SummaryControlContainer_VisibleChanged);

                this.dgv.ColumnAdded += new DataGridViewColumnEventHandler(dgv_ColumnAdded);
                this.dgv.ColumnRemoved += new DataGridViewColumnEventHandler(dgv_ColumnRemoved);
                this.dgv.ColumnStateChanged += new DataGridViewColumnStateChangedEventHandler(dgv_ColumnStateChanged);
                this.dgv.ColumnDisplayIndexChanged += new DataGridViewColumnEventHandler(dgv_ColumnDisplayIndexChanged);

                this.dgv.DataSourceChanged += new EventHandler(dgv_DataSourceChanged);
            }

            void dgv_DataSourceChanged(object sender, EventArgs e)
            {
                reCreateSumBoxes();
            }

            private void dgv_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
            {
                reCreateSumBoxes();
            }

            private void dgv_ColumnStateChanged(object sender, DataGridViewColumnStateChangedEventArgs e)
            {
                resizeSumBoxes();
            }

            private void dgv_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
            {
                reCreateSumBoxes();
            }

            private void dgv_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
            {
                reCreateSumBoxes();
            }

            private void dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
            {

                ReadOnlyTextBox roTextBox = (ReadOnlyTextBox)sumBoxHash[dgv.Columns[e.ColumnIndex]];
                if (roTextBox != null)
                {
                    if (roTextBox.IsSummary)
                        calcSummaries();
                }
            }

            private void dgv_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
            {
                calcSummaries();
            }

            private void dgv_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
            {
                calcSummaries();
            }

            private void SummaryControlContainer_VisibleChanged(object sender, EventArgs e)
            {
                if (lastVisibleState != this.Visible)
                {
                    OnVisiblityChanged(sender, e);
                }
            }

            protected void OnVisiblityChanged(object sender, EventArgs e)
            {
                if (VisibilityChanged != null)
                    VisibilityChanged(sender, e);

                lastVisibleState = this.Visible;
            }

            #endregion

            #region Events and delegates 事件和委托

            private void dgv_CreateSummary(object sender, EventArgs e)
            {
                reCreateSumBoxes();
                calcSummaries();
            }

            private void dgv_Scroll(object sender, ScrollEventArgs e)
            {
                resizeSumBoxes();
            }

            private void dgv_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
            {
                resizeSumBoxes();
            }

            private void dgv_RowHeadersWidthChanged(object sender, EventArgs e)
            {
                resizeSumBoxes();
            }

            private void dgv_Resize(object sender, EventArgs e)
            {
                resizeSumBoxes();
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                resizeSumBoxes();
            }

            #endregion

            #region Functions 其他方法

            /// <summary>
            /// Checks if passed object is of type of integer 输入是否为整数
            /// </summary>
            /// <param name="o">object</param>
            /// <returns>true/ false</returns>
            protected bool IsInteger(object o)
            {
                if (o is Int64)
                    return true;
                if (o is Int32)
                    return true;
                if (o is Int16)
                    return true;
                return false;
            }

            /// <summary>
            /// Checks if passed object is of type of decimal/ double 输入是否为浮点型数
            /// </summary>
            /// <param name="o">object</param>
            /// <returns>true/ false</returns>
            protected bool IsDecimal(object o)
            {
                if (o is Decimal)
                    return true;
                if (o is Single)
                    return true;
                if (o is Double)
                    return true;
                return false;
            }

            /// <summary>
            /// Enable manual refresh of the SummaryDataGridView 启用数据表的手动刷新
            /// </summary>
            internal void RefreshSummary()
            {
                calcSummaries();
            }

            /// <summary>
            /// Calculate the Sums of the summary columns 计算统计行数据总和
            /// </summary>
            private void calcSummaries()
            {
                foreach (ReadOnlyTextBox roTextBox in sumBoxHash.Values)
                {
                    if (roTextBox.IsSummary)
                    {
                        roTextBox.Tag = 0;
                        roTextBox.Text = "0";
                    }
                }

                if (dgv.SummaryColumns != null && dgv.SummaryColumns.Length > 0 && sumBoxHash.Count > 0)
                {
                    //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    //stopwatch.Start(); //  开始监视代码运行时间
                    ////......
                    //stopwatch.Stop(); //  停止监视
                    //TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间

                    foreach (DataGridViewColumn dgvColumn in sumBoxHash.Keys)
                    {
                        ReadOnlyTextBox sumBox = (ReadOnlyTextBox)sumBoxHash[dgvColumn];

                        for (int i = 0; i < dgv.Rows.Count; i++)
                        {
                            DataGridViewCell dgvCell = dgv.Rows[i].Cells[dgvColumn.Index];
                            if (sumBox != null && sumBox.IsSummary)
                            {
                                if (dgvCell.Value != null && !(dgvCell.Value is DBNull))
                                {
                                    if (IsInteger(dgvCell.Value))
                                    {
                                        sumBox.Tag = Convert.ToInt64(sumBox.Tag) + Convert.ToInt64(dgvCell.Value);
                                    }
                                    else if (IsDecimal(dgvCell.Value))
                                    {
                                        sumBox.Tag = Convert.ToDecimal(sumBox.Tag) + Convert.ToDecimal(dgvCell.Value);
                                    }

                                    sumBox.Text = string.Format("{0}", sumBox.Tag);
                                    sumBox.Invalidate();
                                }
                            }
                        }
                    }

                }
            }

            /// <summary>
            /// Create summary boxes for each Column of the DataGridView  为数据表各列创建统计文本
            /// </summary>
            private void reCreateSumBoxes()
            {
                ReadOnlyTextBox sumBox;

                foreach (Control control in sumBoxHash.Values)
                {
                    this.Controls.Remove(control);
                }
                sumBoxHash.Clear();

                int iCnt = 0;

                List<DataGridViewColumn> sortedColumns = SortedColumns;
                foreach (DataGridViewColumn dgvColumn in sortedColumns)
                {
                    sumBox = new ReadOnlyTextBox();
                    sumBoxHash.Add(dgvColumn, sumBox);

                    sumBox.Top = 0;
                    sumBox.Height = dgv.RowTemplate.Height;
                    sumBox.BorderColor = dgv.GridColor;
                    sumBox.ForeColor = Color.White;

                    if (summaryRowBackColor == null || summaryRowBackColor == Color.Transparent)
                        sumBox.BackColor = dgv.DefaultCellStyle.BackColor;
                    else
                        sumBox.BackColor = summaryRowBackColor;
                    sumBox.BringToFront();

                    if (dgv.ColumnCount - iCnt == 1)
                        sumBox.IsLastColumn = true;

                    if (dgv.SummaryColumns != null && dgv.SummaryColumns.Length > 0)
                    {
                        for (int iCntX = 0; iCntX < dgv.SummaryColumns.Length; iCntX++)
                        {
                            if (dgv.SummaryColumns[iCntX] == dgvColumn.DataPropertyName ||
                                dgv.SummaryColumns[iCntX] == dgvColumn.Name)
                            {
                                dgvColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                                sumBox.TextAlign = AlignmentTools.TranslateGridColumnAligment(dgvColumn.DefaultCellStyle.Alignment);
                                sumBox.IsSummary = true;

                                sumBox.FormatString = dgvColumn.CellTemplate.Style.Format;
                                if (dgvColumn.ValueType == typeof(System.Int32) || dgvColumn.ValueType == typeof(System.Int16) ||
                                    dgvColumn.ValueType == typeof(System.Int64) || dgvColumn.ValueType == typeof(System.Single) ||
                                    dgvColumn.ValueType == typeof(System.Double) || dgvColumn.ValueType == typeof(System.Single) ||
                                    dgvColumn.ValueType == typeof(System.Decimal))
                                    sumBox.Tag = System.Activator.CreateInstance(dgvColumn.ValueType);
                            }
                        }
                    }

                    sumBox.BringToFront();
                    this.Controls.Add(sumBox);

                    iCnt++;
                }

                if (dgv.DisplaySumRowHeader)
                {
                    sumRowHeaderLabel.Visible = true;
                    sumRowHeaderLabel.Text = dgv.SumRowHeaderText;
                    sumRowHeaderLabel.ForeColor = dgv.RowHeadersDefaultCellStyle.ForeColor;
                    sumRowHeaderLabel.Font = new Font(dgv.DefaultCellStyle.Font, dgv.SumRowHeaderTextBold ? FontStyle.Bold : FontStyle.Regular);
                    sumRowHeaderLabel.Height = dgv.RowTemplate.Height;
                    sumRowHeaderLabel.Width = dgv.RowHeadersWidth;
                }
                calcSummaries();
                resizeSumBoxes();
            }

            /// <summary>
            /// Order the columns in the way they are displayed  对可见列进行统计
            /// </summary>
            private List<DataGridViewColumn> SortedColumns
            {
                get
                {
                    List<DataGridViewColumn> result = new List<DataGridViewColumn>();
                    DataGridViewColumn column = dgv.Columns.GetFirstColumn(DataGridViewElementStates.None);
                    if (column == null)
                        return result;
                    result.Add(column);
                    while ((column = dgv.Columns.GetNextColumn(column, DataGridViewElementStates.None, DataGridViewElementStates.None)) != null)
                        result.Add(column);

                    return result;
                }
            }

            /// <summary>
            /// Resize the summary Boxes (depending on the width of the Columns of the DataGridView)    
            /// 调整统计文本框大小（取决于数据表的列宽）
            /// </summary>
            private void resizeSumBoxes()
            {
                this.SuspendLayout();
                if (sumBoxHash != null && sumBoxHash.Count > 0)
                    try
                    {
                        int rowHeaderWidth = dgv.RowHeadersVisible ? dgv.RowHeadersWidth - 1 : 0;
                        int sumLabelWidth = dgv.RowHeadersVisible ? dgv.RowHeadersWidth - 1 : 0;
                        int curPos = rowHeaderWidth;

                        if (dgv.DisplaySumRowHeader && sumLabelWidth > 0)
                        {
                            if (!sumRowHeaderLabel.Visible)
                                sumRowHeaderLabel.Visible = true;
                            sumRowHeaderLabel.Width = sumLabelWidth;

                            if (dgv.RightToLeft == RightToLeft.Yes)
                            {
                                if (sumRowHeaderLabel.Dock != DockStyle.Right)
                                    sumRowHeaderLabel.Dock = DockStyle.Right;
                            }
                            else
                            {
                                if (sumRowHeaderLabel.Dock != DockStyle.Left)
                                    sumRowHeaderLabel.Dock = DockStyle.Left;
                            }
                        }
                        else
                        {
                            if (sumRowHeaderLabel.Visible)
                                sumRowHeaderLabel.Visible = false;
                        }

                        int iCnt = 0;
                        Rectangle oldBounds;

                        foreach (DataGridViewColumn dgvColumn in SortedColumns) //dgv.Columns)
                        {
                            ReadOnlyTextBox sumBox = (ReadOnlyTextBox)sumBoxHash[dgvColumn];

                            if (sumBox != null)
                            {
                                oldBounds = sumBox.Bounds;
                                if (!dgvColumn.Visible)
                                {
                                    sumBox.Visible = false;
                                    continue;
                                }

                                int from = dgvColumn.Frozen ? curPos : curPos - dgv.HorizontalScrollingOffset;

                                int width = dgvColumn.Width + (iCnt == 0 ? 0 : 0);

                                if (from < rowHeaderWidth)
                                {
                                    width -= rowHeaderWidth - from;
                                    from = rowHeaderWidth;
                                }

                                if (from + width > this.Width)
                                    width = this.Width - from;

                                if (width < 4)
                                {
                                    if (sumBox.Visible)
                                        sumBox.Visible = false;
                                }
                                else
                                {
                                    if (this.RightToLeft == RightToLeft.Yes)
                                        from = this.Width - from - width;


                                    if (sumBox.Left != from || sumBox.Width != width)
                                        sumBox.SetBounds(from, 0, width, 0, BoundsSpecified.X | BoundsSpecified.Width);

                                    if (!sumBox.Visible)
                                        sumBox.Visible = true;
                                }

                                curPos += dgvColumn.Width + (iCnt == 0 ? 0 : 0);
                                if (oldBounds != sumBox.Bounds)
                                    sumBox.Invalidate();

                            }
                            iCnt++;
                        }
                    }
                    finally
                    {
                        this.ResumeLayout();
                    }
            }

            #endregion
        }

        [ToolboxItem(false)]
        public partial class ReadOnlyTextBox : Control
        {

            #region  声明字段
            private StringFormat format;

            #endregion

            #region  构造函数

            public ReadOnlyTextBox()
            {
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                format = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.FitBlackBox | StringFormatFlags.MeasureTrailingSpaces);
                format.LineAlignment = StringAlignment.Center;

                this.Height = 10;
                this.Width = 10;

                this.Padding = new Padding(2);
                this.TextChanged += new EventHandler(ReadOnlyTextBox_TextChanged);
            }

            #endregion

            #region  属性

            private bool isSummary;
            public bool IsSummary
            {
                get { return isSummary; }
                set { isSummary = value; }
            }

            private bool isLastColumn;
            public bool IsLastColumn
            {
                get { return isLastColumn; }
                set { isLastColumn = value; }
            }

            private string formatString;
            public string FormatString
            {
                get { return formatString; }
                set { formatString = value; }
            }

            private HorizontalAlignment textAlign = HorizontalAlignment.Left;
            [DefaultValue(HorizontalAlignment.Left)]
            public HorizontalAlignment TextAlign
            {
                get { return textAlign; }
                set
                {
                    textAlign = value;
                    setFormatFlags();
                }
            }

            private StringTrimming trimming = StringTrimming.None;
            [DefaultValue(StringTrimming.None)]
            public StringTrimming Trimming
            {
                get { return trimming; }
                set
                {
                    trimming = value;
                    setFormatFlags();
                }
            }

            private Color borderColor = Color.Black;
            public Color BorderColor
            {
                get { return borderColor; }
                set { borderColor = value; }
            }

            #endregion

            #region  其他方法

            private void setFormatFlags()
            {
                format.Alignment = AlignmentTools.TranslateAligment(TextAlign);
                format.Trimming = trimming;
            }

            #endregion

            #region  事件和委托

            private void ReadOnlyTextBox_TextChanged(object sender, EventArgs e)
            {
                if (!string.IsNullOrEmpty(formatString) && !string.IsNullOrEmpty(Text))
                {
                    Text = string.Format(formatString, Text);
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Rectangle textBounds;

                textBounds = new Rectangle(this.ClientRectangle.X + 2, this.ClientRectangle.Y + 2, this.ClientRectangle.Width - 2, this.ClientRectangle.Height - 2);
                using (Pen pen = new Pen(borderColor))
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);
                    e.Graphics.DrawString(Text, Font, Brushes.White, textBounds, format);
                }
            }

            #endregion

        }
    }
}
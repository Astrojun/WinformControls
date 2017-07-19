using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ControlAstro.Controls
{
    public class DataGridViewDragSort : DataGridView
    {
        private int indexOfDragRow = -1;
        private int indexOfInsertRow = -1;
        private int indexOfOverRow = -1;
        private Rectangle shieldBox = Rectangle.Empty;
        private TipPanel tipPanel;

        public bool CanDragCopy { get; set; }


        public DataGridViewDragSort()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            //
            // tipPanel
            //
            this.tipPanel = new TipPanel();
            this.tipPanel.Visible = false;
            this.tipPanel.BringToFront();
            //
            // this
            //
            this.AllowDrop = true;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToResizeRows = false;
            this.AutoGenerateColumns = false;
            this.MultiSelect = false;
            this.ReadOnly = true;
            this.RowHeadersVisible = false;
            this.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.Controls.Add(this.tipPanel);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
        }

        protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
        {
            base.OnRowPostPaint(e);
            if (AllowDrop && DataSource != null)
            {
                if (e.RowIndex == indexOfOverRow)
                {
                    e.Graphics.FillRectangle(Brushes.Lime, e.RowBounds.X, e.RowBounds.Y,
                            Columns.GetColumnsWidth(DataGridViewElementStates.Visible), 2);
                }
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            if (AllowDrop && DataSource != null)
            {
                Point p = PointToClient(new Point(drgevent.X, drgevent.Y));
                var tempModel = drgevent.Data.GetData(Rows[0].DataBoundItem.GetType());
                tipPanel.Visible = false;
                var hitTest = HitTest(p.X, p.Y);
                if (hitTest.Type != DataGridViewHitTestType.Cell || hitTest.RowIndex == indexOfDragRow + 1)
                    return;

                System.Collections.IList list = DataSource as System.Collections.IList;
                indexOfInsertRow = hitTest.RowIndex;
                if (drgevent.Effect.Equals(DragDropEffects.Move))
                {
                    list.RemoveAt(indexOfDragRow);
                }

                if (indexOfDragRow < indexOfInsertRow)
                    indexOfInsertRow--;
                list.Insert(indexOfInsertRow, tempModel);
                DataSource = null;
                DataSource = list;
                CurrentCell = Rows[indexOfInsertRow].Cells[0];
                Invalidate();
            }
        }
        
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
            if (AllowDrop && DataSource != null)
            {
                Point p = PointToClient(new Point(drgevent.X, drgevent.Y));
                var hitTest = HitTest(p.X, p.Y);
                if (hitTest.Type != DataGridViewHitTestType.Cell || hitTest.RowIndex == indexOfDragRow)
                {
                    drgevent.Effect = DragDropEffects.None;
                    tipPanel.Visible = false;
                    OnRowDragOver(-1);
                    return;
                }
                if (CanDragCopy)
                {
                    drgevent.Effect = drgevent.KeyState.Equals(9) ? DragDropEffects.Copy : DragDropEffects.Move;/* 9 means Ctrl Key */
                }
                else
                {
                    drgevent.Effect = DragDropEffects.Move;
                }

                if (SelectedRows[0].DataBoundItem != null)
                {
                    tipPanel.Visible = true;
                    tipPanel.Location = p;
                    tipPanel.ImgInfo = drgevent.Effect.Equals(DragDropEffects.Copy) ? Properties.Resources.ImgCopyMode : Properties.Resources.ImgMoveMode;
                    tipPanel.InfoText = "Row：" + (SelectedRows[0].Index + 1).ToString();
                }

                OnRowDragOver(hitTest.RowIndex);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (AllowDrop && DataSource != null)
            {
                var hitTest = HitTest(e.X, e.Y);
                if (hitTest.Type != DataGridViewHitTestType.Cell)
                {
                    indexOfDragRow = -1;
                    return;
                }

                tipPanel.Location = PointToClient(e.Location);
                tipPanel.Visible = true;
                indexOfDragRow = hitTest.RowIndex;
                if (indexOfDragRow > -1)
                {
                    Size dragSize = SystemInformation.DragSize;
                    shieldBox = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
                }
                else
                {
                    shieldBox = Rectangle.Empty;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (AllowDrop && DataSource != null)
            {
                if ((e.Button & MouseButtons.Left) != MouseButtons.Left)
                    return;
                if (shieldBox == Rectangle.Empty || shieldBox.Contains(e.X, e.Y))
                    return;
                if (indexOfDragRow < 0)
                    return;

                var row = Rows[indexOfDragRow];
                DragDropEffects dropEffect = DoDragDrop(row.DataBoundItem, DragDropEffects.All);

                OnRowDragOver(-1);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (AllowDrop && DataSource != null)
            {
                shieldBox = Rectangle.Empty;
                tipPanel.Visible = false;
            }
        }

        private void OnRowDragOver(int rowIndex)
        {
            if (indexOfOverRow == rowIndex)
                return;
            int oldIndex = indexOfOverRow;
            indexOfOverRow = rowIndex;
            if (oldIndex > -1)
                InvalidateRow(oldIndex);
            if (rowIndex > -1)
                InvalidateRow(rowIndex);
        }


        private void CopyModel<T>(T oriEntity, T newEntity)
        {
            if (oriEntity == null || newEntity == null)
                return;
            Type type = typeof(T);
            PropertyInfo[] proInfoArray = type.GetProperties();
            foreach (PropertyInfo proInfo in proInfoArray)
            {
                if (proInfo.CanWrite)
                {
                    object oriObj = type.GetProperty(proInfo.Name).GetValue(oriEntity, null);
                    type.GetProperty(proInfo.Name).SetValue(newEntity, oriObj, null);
                }
            }
        }
    }

}

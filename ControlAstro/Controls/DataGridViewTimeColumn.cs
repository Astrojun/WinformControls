using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ControlAstro.Controls
{
    public class DataGridViewTimeColumn : DataGridViewColumn
    {
        private bool showUpDown;

        public DataGridViewTimeColumn() : base(new DataGridViewDateTimeCell())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewDateTimeCell)))
                {
                    throw new InvalidCastException("不是DataGridViewDateTimeCell");
                }
                base.CellTemplate = value;
            }
        }

        //要實現自定義屬性設計時的保存必須同時重寫Clone方法
        public override object Clone()
        {
            DataGridViewTimeColumn col = (DataGridViewTimeColumn)base.Clone();
            col.ShowUpDown = this.showUpDown;
            return col;
        }

        //自定義的屬性
        public bool ShowUpDown
        {
            get
            {
                return showUpDown;
            }
            set
            {
                if (this.showUpDown != value)
                {
                    this.showUpDown = value;
                }

            }
        }
    }

    public class DataGridViewDateTimeCell : DataGridViewTextBoxCell
    {
        public DataGridViewDateTimeCell()
        {
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            //
            //設定DateTimePicker特性
            //
            DataGridViewColumn dgvColumn = this.OwningColumn;
            if (dgvColumn is DataGridViewTimeColumn)
            {
                DataGridViewDateTimeEditingControl ctl = DataGridView.EditingControl as DataGridViewDateTimeEditingControl;
                if (ctl == null) return;
                ctl.Value = DateTime.Parse((string)initialFormattedValue);
                DataGridViewTimeColumn dateTimeColumn = dgvColumn as DataGridViewTimeColumn;
                //
                //設定ShowUpDown
                //
                ctl.ShowUpDown = this.ShowUpDown;
                //
                //設定Format
                //
                switch (dateTimeColumn.DefaultCellStyle.Format)
                {
                    case "d":
                        ctl.Format = DateTimePickerFormat.Short;
                        break;
                    case "D":
                        ctl.Format = DateTimePickerFormat.Long;
                        break;
                    case "f":
                        ctl.Format = DateTimePickerFormat.Custom;
                        ctl.CustomFormat = "yyyy年M月d日 tt hh:mm";
                        break;
                    case "F":
                        ctl.Format = DateTimePickerFormat.Custom;
                        ctl.CustomFormat = "yyyy年M月d日 HH:mm:ss";
                        break;
                    case "g":
                        ctl.Format = DateTimePickerFormat.Custom;
                        ctl.CustomFormat = "yyyy/MM/dd tt hh:mm";
                        break;
                    case "G":
                        ctl.Format = DateTimePickerFormat.Custom;
                        ctl.CustomFormat = "yyyy/MM/dd HH:mm:ss";
                        break;
                    case "t":
                        ctl.Format = DateTimePickerFormat.Custom;
                        ctl.CustomFormat = "tt hh:mm";
                        break;
                    case "T":
                        ctl.Format = DateTimePickerFormat.Time;
                        ctl.CustomFormat = "HH:mm:ss";
                        break;
                    case "M":
                        ctl.Format = DateTimePickerFormat.Custom;
                        ctl.CustomFormat = "M月d日";
                        break;
                    default:
                        ctl.Format = DateTimePickerFormat.Custom;
                        ctl.CustomFormat = dateTimeColumn.DefaultCellStyle.Format;
                        break;

                }

            }
        }
        //一個自定義屬性
        public virtual bool ShowUpDown
        {
            get
            {
                return (OwningColumn as DataGridViewTimeColumn).ShowUpDown;
            }
        }
        public override Type EditType
        {
            get
            {
                return typeof(DataGridViewDateTimeEditingControl);
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(DateTime);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                return DateTime.Now;
            }
        }
    }

    public class DataGridViewDateTimeEditingControl : DateTimePicker, IDataGridViewEditingControl
    {
        protected int rowIndex;
        protected DataGridView dataGridView;
        protected bool valueChanged = false;

        public DataGridViewDateTimeEditingControl()
        {
        }

        //重寫基類
        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            NotifyDataGridViewOfValueChange();
        }
        //  當text值發生變化時，通知DataGridView
        private void NotifyDataGridViewOfValueChange()
        {
            valueChanged = true;
            dataGridView.NotifyCurrentCellDirty(true);
        }

        #region IDataGridViewEditingControl接口的實現
        /// <summary>
        /// 獲取或設置儲存格所在的DataGridView
        /// </summary>
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }

            set
            {
                dataGridView = value;
            }
        }


        /// <summary>
        /// 獲取或設置儲存格格式化後的值
        /// </summary>
        public object EditingControlFormattedValue
        {
            set
            {
                Text = value.ToString();
                NotifyDataGridViewOfValueChange();
            }
            get
            {
                return this.Text;
            }

        }

        /// <summary>
        /// 在Cell被編輯的時候光標顯示
        /// </summary>
        public Cursor EditingPanelCursor
        {
            get
            {
                return Cursors.IBeam;
            }
        }
        /// <summary>
        /// 取得或設定值，指出每當值變更時儲存格內容是否需要重新調整位置。
        /// </summary>
        public virtual bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 取得或設定儲存格所在行
        /// </summary>
        public int EditingControlRowIndex
        {
            get
            {
                return this.rowIndex;
            }

            set
            {
                this.rowIndex = value;
            }
        }
        /// <summary>
        /// 取得或設定值，指出編輯控制項的值是否與裝載儲存格的值不同。
        /// </summary>
        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                this.valueChanged = value;
            }
        }
        /// <summary>
        /// 擷取儲存格的格式化後的值。
        /// </summary>
        /// <param name="context">錯誤上下文</param>
        /// <returns></returns>
        public virtual object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return Text;
        }
        /// <summary>
        /// 判斷指定的按鍵是否為編輯控制項應該處理的標準輸入按鍵，或是 System.Windows.Forms.DataGridView 應該處理的特殊按鍵。
        /// </summary>
        /// <param name="keyData"></param>
        /// <param name="dataGridViewWantsInputKey"></param>
        /// <returns></returns>
        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            // Let the DateTimePicker handle the keys listed.
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 準備目前所選的儲存格來編輯。
        /// </summary>
        /// <param name="selectAll"></param>
        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        /// <summary>
        /// 變更控制項的使用者介面 (UI)，使其與指定的儲存格樣式一致。
        /// </summary>
        /// <param name="dataGridViewCellStyle"></param>
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
            this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }

        #endregion
    }
}

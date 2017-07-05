using ControlAstro.Enums.ControlEnums;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace ControlAstro.Controls
{
    [ToolboxBitmap(typeof(DataGridViewButtonColumn), "DataGridViewButtonColumn.bmp")]
    internal class DataGridViewStateColumn : DataGridViewColumn
    {

        public DataGridViewStateColumn()
            : base(new DataGridViewStateCell())
        { }
        
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                if (value != null && !(value is DataGridViewStateCell))
                {
                    throw new InvalidCastException("必须是一个状态显示单元格");
                }
                base.CellTemplate = value;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x40);
            builder.Append("DataGridViewStateColumn { Name=");
            builder.Append(base.Name);
            builder.Append(", Index=");
            builder.Append(base.Index.ToString(CultureInfo.CurrentCulture));
            builder.Append(" }");
            return builder.ToString();
        }

        public override object Clone()
        {
            Type type = base.GetType();
            DataGridViewStateColumn column;
            if (type == typeof(DataGridViewStateColumn))
            {
                column = base.Clone() as DataGridViewStateColumn;
            }
            else
            {
                column = (DataGridViewStateColumn)Activator.CreateInstance(type);
            }
            return column;
        }

    }

    public class DataGridViewStateCell : DataGridViewButtonCell
    {
        private bool isPressed = false;
        private bool isHovered = false;
        private bool isFocused = false;
        private Size preferredSize;
        private Rectangle contentBounds;

        private LightState powerState = LightState.Off;
        [DefaultValue(LightState.Off)]
        public LightState PowerState
        {
            get { return powerState; }
            set
            {
                if (powerState != value)
                {
                    powerState = value;
                }
                switch (value)
                {
                    case LightState.Bad:
                        Text = "设备异常";
                        break;
                    case LightState.Off:
                        Text = "点击运行";
                        break;
                    case LightState.On:
                        Text = "点击停止";
                        break;
                }
            }
        }

        public string Text { get; set; }

        public override Type EditType
        {
            get
            {
                return null;
            }
        }

        public override Type ValueType
        {
            get
            {
                Type valueType = base.ValueType;
                if (valueType != null)
                {
                    return valueType;
                }
                return typeof(object);
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                return typeof(string);
            }
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            isPressed = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            isPressed = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            isHovered = true;
            base.OnMouseEnter(rowIndex);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            isHovered = false;
            isPressed = false;
            base.OnMouseLeave(rowIndex);
        }

        protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
        {
            isHovered = GetContentBounds(e.RowIndex).Contains(e.X, e.Y);
            base.OnMouseMove(e);
        }

        protected override void OnEnter(int rowIndex, bool throughMouseClick)
        {
            isHovered = true;
            isFocused = true;
            base.OnEnter(rowIndex, throughMouseClick);
        }

        protected override void OnLeave(int rowIndex, bool throughMouseClick)
        {
            isHovered = false;
            isFocused = false;
            isPressed = false;
            base.OnLeave(rowIndex, throughMouseClick);
        }

        protected override void OnKeyDown(KeyEventArgs e, int rowIndex)
        {
            isPressed = true;
            base.OnKeyDown(e, rowIndex);
        }

        protected override void OnKeyUp(KeyEventArgs e, int rowIndex)
        {
            isPressed = false;
            base.OnKeyUp(e, rowIndex);
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException("cellStyle");
            }
            PaintCell(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }

        private void PaintCell(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            graphics.FillRectangle(new SolidBrush(cellStyle.BackColor), cellBounds);
            Point currentCellAddress = base.DataGridView.CurrentCellAddress;
            bool flag = (cellState & DataGridViewElementStates.Selected) != DataGridViewElementStates.None;
            bool flag2 = currentCellAddress.X == base.ColumnIndex && currentCellAddress.Y == rowIndex;
            LightState state = (LightState)value;
            PowerState = state;
            Rectangle rectangle = cellBounds;
            Rectangle rectangle2 = this.BorderWidths(advancedBorderStyle);
            rectangle.Offset(rectangle2.X, rectangle2.Y);
            rectangle.Width -= rectangle2.Right;
            rectangle.Height -= rectangle2.Bottom;
            Rectangle stateRect = new Rectangle(rectangle.X + 5, rectangle.Y + 5, rectangle.Height - 10, rectangle.Height - 10);
            if ((paintParts & DataGridViewPaintParts.Border) != DataGridViewPaintParts.None)
            {
                PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
            }

            if (rectangle.Height > 0 && rectangle.Width > 0)
            {
                Rectangle hover = cellBounds;
                hover.Width -= 1;
                hover.Height -= 1;
                if ((isHovered || isFocused) && !isPressed) graphics.DrawRectangle(new Pen(Color.Silver, 2), hover);
                hover.Inflate(-1, -1);
                if (isPressed) graphics.DrawRectangle(new Pen(Color.Gray, 2), hover);
                switch (state)
                {
                    case LightState.Bad:
                        GraphicsState gState = graphics.Save();
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.FillEllipse(Brushes.Red, stateRect);
                        graphics.Restore(gState);
                        break;
                    case LightState.Off:
                        graphics.FillPolygon(Brushes.Green, new Point[] {
                            new Point(rectangle.X + 5, rectangle.Y + 3),
                            new Point(rectangle.X + 5, rectangle.Y + rectangle.Height - 3),
                            new Point(rectangle.X + (rectangle.Height + 4) / 2, rectangle.Y + rectangle.Height / 2) });
                        break;
                    case LightState.On:
                        graphics.FillRectangle(Brushes.Silver, stateRect);
                        break;
                }

                Rectangle textRect = new Rectangle(stateRect.Right + 5, rectangle.Y, rectangle.Width - rectangle.Height, rectangle.Height);
                TextRenderer.DrawText(graphics, Text, cellStyle.Font, textRect, cellStyle.ForeColor, cellStyle.BackColor,
                     TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                Size textSize = TextRenderer.MeasureText(graphics, Text, cellStyle.Font, textRect.Size, TextFormatFlags.VerticalCenter);
                preferredSize = new Size(textSize.Width + stateRect.Width + 5, rectangle.Height);
                contentBounds = new Rectangle(cellBounds.Location, preferredSize);
            }
            else
            {
                preferredSize = Size.Empty;
                contentBounds = Rectangle.Empty;
            }
        }

        public override string ToString()
        {
            return string.Concat(new string[]
            {
                "DataGridViewStateCell { ColumnIndex=",
                base.ColumnIndex.ToString(CultureInfo.CurrentCulture),
                ", RowIndex=",
                base.RowIndex.ToString(CultureInfo.CurrentCulture),
                " }"
            });
        }

        public override object Clone()
        {
            Type type = base.GetType();
            DataGridViewStateCell dataGridViewStateCell;
            if (type == typeof(DataGridViewStateCell))
            {
                dataGridViewStateCell = base.Clone() as DataGridViewStateCell;
            }
            else
            {
                dataGridViewStateCell = (DataGridViewStateCell)Activator.CreateInstance(type);
            }
            dataGridViewStateCell.PowerState = this.PowerState;
            return dataGridViewStateCell;
        }

    }
}

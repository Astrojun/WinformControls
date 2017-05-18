using ControlAstro.Enums.ControlEnums;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ControlAstro.Design
{
    public sealed class BorderEditor : UITypeEditor
    {
        private BorderEditorControl editorControl;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {
                    if (editorControl == null)
                    {
                        editorControl = new BorderEditorControl();
                    }
                    editorControl.Start(editorService, value);
                    editorService.DropDownControl(editorControl);
                    value = editorControl.Value;
                    editorControl.End();
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return base.GetPaintValueSupported(context);
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            base.PaintValue(e);
        }


        private class BorderEditorControl : System.Windows.Forms.Control
        {
            private BorderState oldState;
            private EditControl top;
            private EditControl right;
            private EditControl bottom;
            private EditControl left;
            private IWindowsFormsEditorService edSvc;
            private object value;

            public BorderEditorControl()
            {
                Height = 90;
                Width = 120;

                top = new EditControl(this);
                top.Size = new Size(Width - 30, 15);
                top.Location = new Point(15, 0);
                top.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                left = new EditControl(this);
                left.Size = new Size(15, Height - 30);
                left.Location = new Point(0, 15);
                left.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;

                right = new EditControl(this);
                right.Size = new Size(15, Height - 30);
                right.Location = new Point(Width - 15, 15);
                right.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

                bottom = new EditControl(this);
                bottom.Size = new Size(Width - 30, 15);
                bottom.Location = new Point(15, Height - 15);
                bottom.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

                Controls.Clear();
                Controls.Add(top);
                Controls.Add(left);
                Controls.Add(right);
                Controls.Add(bottom);
            }

            public object Value
            {
                get
                {
                    return this.value;
                }
            }

            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                this.edSvc = edSvc;
                this.value = value;
                if (value is BorderState)
                {
                    this.left.Checked = ((BorderState)value & BorderState.Left) == BorderState.Left;
                    this.top.Checked = ((BorderState)value & BorderState.Top) == BorderState.Top;
                    this.bottom.Checked = ((BorderState)value & BorderState.Bottom) == BorderState.Bottom;
                    this.right.Checked = ((BorderState)value & BorderState.Right) == BorderState.Right;
                    this.oldState = (BorderState)value;
                    return;
                }
                this.oldState = (BorderState.Top | BorderState.Left | BorderState.Bottom | BorderState.Right);
            }

            public void End()
            {
                this.edSvc = null;
                this.value = null;
            }

            public void SetValue()
            {
                BorderState borderState = BorderState.None;
                if(left.Checked)
                {
                    borderState |= BorderState.Left;
                }
                if (top.Checked)
                {
                    borderState |= BorderState.Top;
                }
                if (right.Checked)
                {
                    borderState |= BorderState.Right;
                }
                if (bottom.Checked)
                {
                    borderState |= BorderState.Bottom;
                }
                this.value = borderState;
            }

            private void Teardown(bool saveState)
            {
                if (!saveState)
                {
                    this.value = this.oldState;
                }
                this.edSvc.CloseDropDown();
            }
            

            private class EditControl : System.Windows.Forms.Control
            {
                private BorderEditorControl picker;
                private bool isChecked = false;

                public bool Checked
                {
                    get { return isChecked; }
                    set
                    {
                        isChecked = value;
                        this.picker.SetValue();
                        base.Invalidate();
                    }
                }

                public EditControl(BorderEditorControl picker)
                {
                    if (picker == null)
                    {
                        throw new ArgumentException();
                    }
                    this.picker = picker;
                }

                protected override void OnMouseDown(MouseEventArgs e)
                {
                    this.Checked = !this.Checked;
                    base.Focus();
                }

                protected override void OnPaint(PaintEventArgs e)
                {
                    if (this.Checked)
                    {
                        e.Graphics.FillRectangle(SystemBrushes.ControlDark, ClientRectangle);
                        e.Graphics.DrawRectangle(SystemPens.WindowFrame, ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
                    }
                    else
                    {
                        ControlPaint.DrawFocusRectangle(e.Graphics, ClientRectangle);
                    }
                }

                protected override bool ProcessDialogChar(char charCode)
                {
                    if (charCode == ' ')
                    {
                        this.Checked = !Checked;
                        return true;
                    }
                    return base.ProcessDialogChar(charCode);
                }

                protected override bool ProcessDialogKey(Keys keyData)
                {
                    if ((keyData & Keys.KeyCode) == Keys.Return && (keyData & (Keys.Control | Keys.Alt)) == Keys.None)
                    {
                        this.picker.Teardown(true);
                        return true;
                    }
                    if ((keyData & Keys.KeyCode) == Keys.Escape && (keyData & (Keys.Control | Keys.Alt)) == Keys.None)
                    {
                        this.picker.Teardown(false);
                        return true;
                    }
                    return base.ProcessDialogKey(keyData);
                }
            }

        }

    }
}

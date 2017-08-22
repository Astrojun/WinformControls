using System;
using System.Windows.Forms;
using System.Drawing;

namespace ControlAstro.Controls
{
    [ToolboxBitmap(typeof(PrintPreviewDialog))]
    public partial class PrintPreviewDialogEx : PrintPreviewDialog
    {
        public PrintPreviewDialogEx()
        {
            InitializeComponent();
            foreach (Control ctrl in Controls)
            {
                if (ctrl is ToolStrip)
                {
                    ToolStrip tools = ctrl as ToolStrip;
                    tools.Items.Insert(0, CreatePrintsetButton());
                }
            }
        }

        private ToolStripButton CreatePrintsetButton()
        {
            ToolStripButton Stripbutton = new ToolStripButton();
            Stripbutton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Stripbutton.Image = Properties.Resources.SettingTransparent;
            Stripbutton.ImageTransparentColor = System.Drawing.Color.Magenta;
            Stripbutton.Name = "printsetStripButton";
            Stripbutton.Size = new System.Drawing.Size(23, 22);
            Stripbutton.Text = "打印设置";
            Stripbutton.Click += Stripbutton_Click; ;
            return Stripbutton;
        }

        private void Stripbutton_Click(object sender, EventArgs e)
        {
            using (PrintDialog diag = new PrintDialog())
            {
                diag.Document = Document;
                diag.ShowDialog();
            }
        }
    }
}

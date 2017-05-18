namespace ControlAstro.Controls
{
    partial class TextBoxSearch
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button = new System.Windows.Forms.Button();
            this.textBox = new ControlAstro.Controls.TextBoxWatermark();
            this.SuspendLayout();
            // 
            // button
            // 
            this.button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button.BackgroundImage = global::ControlAstro.Properties.Resources.btSearch;
            this.button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button.FlatAppearance.BorderSize = 0;
            this.button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button.Location = new System.Drawing.Point(211, 5);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(22, 23);
            this.button.TabIndex = 1;
            this.button.UseVisualStyleBackColor = true;
            this.button.MouseClick += new System.Windows.Forms.MouseEventHandler(this.button_MouseClick);
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox.BackColor = System.Drawing.Color.Lime;
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox.HintFont = new System.Drawing.Font("宋体", 8F);
            this.textBox.HintText = "";
            this.textBox.Location = new System.Drawing.Point(3, 3);
            this.textBox.Margin = new System.Windows.Forms.Padding(0);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(180, 14);
            this.textBox.TabIndex = 0;
            this.textBox.WordWrap = false;
            this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // TextBoxSearch
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.button);
            this.Controls.Add(this.textBox);
            this.DoubleBuffered = true;
            this.Name = "TextBoxSearch";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(249, 37);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBoxWatermark textBox;
        private System.Windows.Forms.Button button;

    }
}

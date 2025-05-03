namespace FPSAide
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            textBox_width = new TextBox();
            label1 = new Label();
            label2 = new Label();
            textBox_length = new TextBox();
            comboBox_color = new ComboBox();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            contextMenuStrip1 = new ContextMenuStrip(components);
            重置坐标ToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // textBox_width
            // 
            textBox_width.Location = new Point(72, 31);
            textBox_width.Name = "textBox_width";
            textBox_width.Size = new Size(100, 24);
            textBox_width.TabIndex = 1;
            textBox_width.TextChanged += textBox_width_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(31, 34);
            label1.Name = "label1";
            label1.Size = new Size(35, 19);
            label1.TabIndex = 2;
            label1.Text = "粗细";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(31, 64);
            label2.Name = "label2";
            label2.Size = new Size(35, 19);
            label2.TabIndex = 4;
            label2.Text = "长度";
            // 
            // textBox_length
            // 
            textBox_length.Location = new Point(72, 61);
            textBox_length.Name = "textBox_length";
            textBox_length.Size = new Size(100, 24);
            textBox_length.TabIndex = 3;
            textBox_length.TextChanged += textBox2_TextChanged;
            // 
            // comboBox_color
            // 
            comboBox_color.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_color.FormattingEnabled = true;
            comboBox_color.Location = new Point(72, 91);
            comboBox_color.Name = "comboBox_color";
            comboBox_color.Size = new Size(100, 27);
            comboBox_color.TabIndex = 5;
            comboBox_color.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(31, 94);
            label3.Name = "label3";
            label3.Size = new Size(35, 19);
            label3.TabIndex = 6;
            label3.Text = "颜色";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(86, 130);
            label4.Name = "label4";
            label4.Size = new Size(58, 19);
            label4.TabIndex = 7;
            label4.Text = "Shit+F9";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(59, 160);
            label5.Name = "label5";
            label5.Size = new Size(132, 19);
            label5.TabIndex = 9;
            label5.Text = "Ctrl+方向键调整准星";
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { 重置坐标ToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(131, 28);
            // 
            // 重置坐标ToolStripMenuItem
            // 
            重置坐标ToolStripMenuItem.Name = "重置坐标ToolStripMenuItem";
            重置坐标ToolStripMenuItem.Size = new Size(130, 24);
            重置坐标ToolStripMenuItem.Text = "重置坐标";
            重置坐标ToolStripMenuItem.Click += 重置坐标ToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(203, 188);
            ContextMenuStrip = contextMenuStrip1;
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(comboBox_color);
            Controls.Add(label2);
            Controls.Add(textBox_length);
            Controls.Add(label1);
            Controls.Add(textBox_width);
            Name = "Form1";
            Text = "FPS辅助";
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        public TextBox textBox_width;
        public Label label1;
        public Label label2;
        public TextBox textBox_length;
        public ComboBox comboBox_color;
        public Label label3;
        public Label label4;
        public Label label5;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem 重置坐标ToolStripMenuItem;
    }
}

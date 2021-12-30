namespace willem_winio32
{
    partial class S25XXChipLengthForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.labelLength = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.comboBoxLength = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "容量";
            // 
            // labelLength
            // 
            this.labelLength.AutoSize = true;
            this.labelLength.Location = new System.Drawing.Point(192, 9);
            this.labelLength.Name = "labelLength";
            this.labelLength.Size = new System.Drawing.Size(29, 12);
            this.labelLength.TabIndex = 1;
            this.labelLength.Text = "容量";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(59, 46);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "确定";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(185, 46);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // comboBoxLength
            // 
            this.comboBoxLength.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLength.FormattingEnabled = true;
            this.comboBoxLength.Items.AddRange(new object[] {
            "0x000100",
            "0x000200",
            "0x000400",
            "0x000800",
            "0x001000",
            "0x002000",
            "0x004000",
            "0x008000",
            "0x010000",
            "0x020000",
            "0x040000",
            "0x080000",
            "0x100000",
            "0x200000",
            "0x400000",
            "0x800000",
            "0x1000000",
            "0x2000000",
            "0x4000000",
            "0x8000000",
            "0x10000000",
            "0x20000000",
            "0x40000000",
            "0x80000000",
            "0x100000000"});
            this.comboBoxLength.Location = new System.Drawing.Point(45, 6);
            this.comboBoxLength.Name = "comboBoxLength";
            this.comboBoxLength.Size = new System.Drawing.Size(121, 20);
            this.comboBoxLength.TabIndex = 4;
            this.comboBoxLength.SelectedValueChanged += new System.EventHandler(this.comboBoxLength_SelectedValueChanged);
            // 
            // S25XXChipLengthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 80);
            this.Controls.Add(this.comboBoxLength);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.labelLength);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "S25XXChipLengthForm";
            this.Text = "设置S25XX芯片容量";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelLength;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboBoxLength;
    }
}
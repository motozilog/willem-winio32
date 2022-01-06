namespace willem_winio32
{
    partial class W27CEraseJumperForm
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
            this.pictureBoxJumper = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxJumper)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxJumper
            // 
            this.pictureBoxJumper.Image = global::willem_winio32.Properties.Resources.W27X010_Erase;
            this.pictureBoxJumper.InitialImage = null;
            this.pictureBoxJumper.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxJumper.Name = "pictureBoxJumper";
            this.pictureBoxJumper.Size = new System.Drawing.Size(259, 70);
            this.pictureBoxJumper.TabIndex = 85;
            this.pictureBoxJumper.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(101, 100);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 115;
            this.button1.Text = "&O.己跳线";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 12);
            this.label1.TabIndex = 114;
            this.label1.Text = "请将Willem跳线从黄色跳到蓝色";
            // 
            // W27CEraseJumperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 128);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxJumper);
            this.Name = "W27CEraseJumperForm";
            this.Text = "W27CEraseJumperForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxJumper)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxJumper;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
    }
}
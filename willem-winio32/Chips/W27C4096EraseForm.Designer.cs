namespace willem_winio32
{
    partial class W27C4096EraseForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBoxAdapter = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdapter)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 12);
            this.label1.TabIndex = 112;
            this.label1.Text = "请更换W27C4096擦除适配器";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(37, 169);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 113;
            this.button1.Text = "&O.己更换";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBoxAdapter
            // 
            this.pictureBoxAdapter.Image = global::willem_winio32.Properties.Resources.W27C4096_Erase_Adapter;
            this.pictureBoxAdapter.InitialImage = global::willem_winio32.Properties.Resources.W27C4096_Erase_Adapter;
            this.pictureBoxAdapter.Location = new System.Drawing.Point(1, 1);
            this.pictureBoxAdapter.Name = "pictureBoxAdapter";
            this.pictureBoxAdapter.Size = new System.Drawing.Size(150, 150);
            this.pictureBoxAdapter.TabIndex = 111;
            this.pictureBoxAdapter.TabStop = false;
            // 
            // W27C4096EraseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(153, 197);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxAdapter);
            this.Name = "W27C4096EraseForm";
            this.Text = "更换适配器";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdapter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxAdapter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}
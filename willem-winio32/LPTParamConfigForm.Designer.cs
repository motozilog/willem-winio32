﻿namespace willem_winio32
{
    partial class LPTParamConfigForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.labelParam = new System.Windows.Forms.Label();
            this.textBoxParam = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(33, 46);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(166, 46);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // labelParam
            // 
            this.labelParam.AutoSize = true;
            this.labelParam.Location = new System.Drawing.Point(12, 13);
            this.labelParam.Name = "labelParam";
            this.labelParam.Size = new System.Drawing.Size(29, 12);
            this.labelParam.TabIndex = 2;
            this.labelParam.Text = "参数";
            // 
            // textBoxParam
            // 
            this.textBoxParam.Location = new System.Drawing.Point(166, 10);
            this.textBoxParam.Name = "textBoxParam";
            this.textBoxParam.Size = new System.Drawing.Size(100, 21);
            this.textBoxParam.TabIndex = 3;
            // 
            // LPTParamConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 87);
            this.Controls.Add(this.textBoxParam);
            this.Controls.Add(this.labelParam);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "LPTParamConfigForm";
            this.Text = "参数设置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label labelParam;
        private System.Windows.Forms.TextBox textBoxParam;
    }
}
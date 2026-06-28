namespace SelfVSIXProject
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._btnClose = new System.Windows.Forms.Button();
            this._btnConvert = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this._txtFileName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _btnClose
            // 
            this._btnClose.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._btnClose.Location = new System.Drawing.Point(995, 244);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(190, 58);
            this._btnClose.TabIndex = 7;
            this._btnClose.Text = "关闭";
            this._btnClose.UseVisualStyleBackColor = true;
            this._btnClose.Click += new System.EventHandler(this._btnClose_Click);
            // 
            // _btnConvert
            // 
            this._btnConvert.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._btnConvert.Location = new System.Drawing.Point(858, 142);
            this._btnConvert.Name = "_btnConvert";
            this._btnConvert.Size = new System.Drawing.Size(327, 44);
            this._btnConvert.TabIndex = 6;
            this._btnConvert.Text = "转换为 RDLC2008 格式";
            this._btnConvert.UseVisualStyleBackColor = true;
            this._btnConvert.Click += new System.EventHandler(this._btnConvert_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(27, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 28);
            this.label1.TabIndex = 4;
            this.label1.Text = "文件名：";
            // 
            // _txtFileName
            // 
            this._txtFileName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtFileName.Font = new System.Drawing.Font("微软雅黑", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._txtFileName.Location = new System.Drawing.Point(155, 67);
            this._txtFileName.Name = "_txtFileName";
            this._txtFileName.ReadOnly = true;
            this._txtFileName.Size = new System.Drawing.Size(1030, 43);
            this._txtFileName.TabIndex = 8;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 35F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1208, 334);
            this.Controls.Add(this._txtFileName);
            this.Controls.Add(this._btnClose);
            this.Controls.Add(this._btnConvert);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "转换RDLC文件";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.Button _btnConvert;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _txtFileName;
    }
}
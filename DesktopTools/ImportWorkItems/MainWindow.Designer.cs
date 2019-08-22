namespace ImportWorkItems
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this._cboProject = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._txtWorkItemIDs = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._txtTFSWorkItemIDs = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._tbStatus = new System.Windows.Forms.TextBox();
            this._btnSave = new System.Windows.Forms.Button();
            this._btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _cboProject
            // 
            this._cboProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cboProject.FormattingEnabled = true;
            this._cboProject.Location = new System.Drawing.Point(169, 14);
            this._cboProject.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._cboProject.Name = "_cboProject";
            this._cboProject.Size = new System.Drawing.Size(400, 27);
            this._cboProject.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "团队项目";
            // 
            // _txtWorkItemIDs
            // 
            this._txtWorkItemIDs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtWorkItemIDs.Location = new System.Drawing.Point(169, 52);
            this._txtWorkItemIDs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._txtWorkItemIDs.Multiline = true;
            this._txtWorkItemIDs.Name = "_txtWorkItemIDs";
            this._txtWorkItemIDs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtWorkItemIDs.Size = new System.Drawing.Size(400, 97);
            this._txtWorkItemIDs.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 55);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "平台工作项ID串";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 162);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 19);
            this.label3.TabIndex = 5;
            this.label3.Text = "TFS生成工作项ID串";
            // 
            // _txtTFSWorkItemIDs
            // 
            this._txtTFSWorkItemIDs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtTFSWorkItemIDs.Location = new System.Drawing.Point(169, 158);
            this._txtTFSWorkItemIDs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._txtTFSWorkItemIDs.Multiline = true;
            this._txtTFSWorkItemIDs.Name = "_txtTFSWorkItemIDs";
            this._txtTFSWorkItemIDs.ReadOnly = true;
            this._txtTFSWorkItemIDs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtTFSWorkItemIDs.Size = new System.Drawing.Size(400, 97);
            this._txtTFSWorkItemIDs.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 268);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 19);
            this.label4.TabIndex = 7;
            this.label4.Text = "处理结果";
            // 
            // _tbStatus
            // 
            this._tbStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._tbStatus.Location = new System.Drawing.Point(171, 263);
            this._tbStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._tbStatus.Multiline = true;
            this._tbStatus.Name = "_tbStatus";
            this._tbStatus.ReadOnly = true;
            this._tbStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._tbStatus.Size = new System.Drawing.Size(400, 127);
            this._tbStatus.TabIndex = 6;
            // 
            // _btnSave
            // 
            this._btnSave.Location = new System.Drawing.Point(383, 399);
            this._btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._btnSave.Name = "_btnSave";
            this._btnSave.Size = new System.Drawing.Size(79, 35);
            this._btnSave.TabIndex = 8;
            this._btnSave.Text = "导入";
            this._btnSave.UseVisualStyleBackColor = true;
            this._btnSave.Click += new System.EventHandler(this._btnSave_Click);
            // 
            // _btnClose
            // 
            this._btnClose.Location = new System.Drawing.Point(489, 399);
            this._btnClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(79, 35);
            this._btnClose.TabIndex = 9;
            this._btnClose.Text = "关闭";
            this._btnClose.UseVisualStyleBackColor = true;
            this._btnClose.Click += new System.EventHandler(this._btnClose_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 446);
            this.Controls.Add(this._btnClose);
            this.Controls.Add(this._btnSave);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._tbStatus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._txtTFSWorkItemIDs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._txtWorkItemIDs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._cboProject);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "导入工作项";
            this.Activated += new System.EventHandler(this.MainWindow_Activated);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _cboProject;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _txtWorkItemIDs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _txtTFSWorkItemIDs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _tbStatus;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Button _btnClose;
    }
}
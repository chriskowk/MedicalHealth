namespace DesktopTools
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
            this._btnClose = new System.Windows.Forms.Button();
            this._btnSave = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this._tbStatus = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._txtTFSWorkItemIDs = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._txtWorkItemIDs = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._cboProjects = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this._cboTfsUris = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this._txtVersionID = new System.Windows.Forms.TextBox();
            this._btnRetrieve = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _btnClose
            // 
            this._btnClose.Location = new System.Drawing.Point(643, 470);
            this._btnClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(79, 35);
            this._btnClose.TabIndex = 19;
            this._btnClose.Text = "关闭";
            this._btnClose.UseVisualStyleBackColor = true;
            this._btnClose.Click += new System.EventHandler(this._btnClose_Click);
            // 
            // _btnSave
            // 
            this._btnSave.Location = new System.Drawing.Point(516, 470);
            this._btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._btnSave.Name = "_btnSave";
            this._btnSave.Size = new System.Drawing.Size(79, 35);
            this._btnSave.TabIndex = 18;
            this._btnSave.Text = "导入";
            this._btnSave.UseVisualStyleBackColor = true;
            this._btnSave.Click += new System.EventHandler(this._btnSave_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(151, 330);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 18);
            this.label4.TabIndex = 17;
            this.label4.Text = "处理结果";
            // 
            // _tbStatus
            // 
            this._tbStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._tbStatus.Location = new System.Drawing.Point(253, 327);
            this._tbStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._tbStatus.Multiline = true;
            this._tbStatus.Name = "_tbStatus";
            this._tbStatus.ReadOnly = true;
            this._tbStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._tbStatus.Size = new System.Drawing.Size(469, 127);
            this._tbStatus.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(70, 221);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 18);
            this.label3.TabIndex = 15;
            this.label3.Text = "TFS生成工作项ID串";
            // 
            // _txtTFSWorkItemIDs
            // 
            this._txtTFSWorkItemIDs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtTFSWorkItemIDs.Location = new System.Drawing.Point(253, 220);
            this._txtTFSWorkItemIDs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._txtTFSWorkItemIDs.Multiline = true;
            this._txtTFSWorkItemIDs.Name = "_txtTFSWorkItemIDs";
            this._txtTFSWorkItemIDs.ReadOnly = true;
            this._txtTFSWorkItemIDs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtTFSWorkItemIDs.Size = new System.Drawing.Size(469, 97);
            this._txtTFSWorkItemIDs.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(97, 87);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 18);
            this.label2.TabIndex = 13;
            this.label2.Text = "平台工作项ID串";
            // 
            // _txtWorkItemIDs
            // 
            this._txtWorkItemIDs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtWorkItemIDs.Location = new System.Drawing.Point(253, 89);
            this._txtWorkItemIDs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._txtWorkItemIDs.Multiline = true;
            this._txtWorkItemIDs.Name = "_txtWorkItemIDs";
            this._txtWorkItemIDs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtWorkItemIDs.Size = new System.Drawing.Size(469, 121);
            this._txtWorkItemIDs.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(151, 51);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 18);
            this.label1.TabIndex = 11;
            this.label1.Text = "团队项目";
            // 
            // _cboProjects
            // 
            this._cboProjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cboProjects.FormattingEnabled = true;
            this._cboProjects.Location = new System.Drawing.Point(253, 51);
            this._cboProjects.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._cboProjects.Name = "_cboProjects";
            this._cboProjects.Size = new System.Drawing.Size(469, 26);
            this._cboProjects.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(124, 15);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 18);
            this.label5.TabIndex = 21;
            this.label5.Text = "客户版本URL";
            // 
            // _cboTfsUris
            // 
            this._cboTfsUris.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cboTfsUris.FormattingEnabled = true;
            this._cboTfsUris.Location = new System.Drawing.Point(253, 15);
            this._cboTfsUris.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._cboTfsUris.Name = "_cboTfsUris";
            this._cboTfsUris.Size = new System.Drawing.Size(471, 26);
            this._cboTfsUris.TabIndex = 20;
            this._cboTfsUris.SelectedIndexChanged += new System.EventHandler(this._cboTfsUris_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._btnRetrieve);
            this.groupBox1.Controls.Add(this._txtVersionID);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(20, 112);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(211, 97);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 18);
            this.label6.TabIndex = 0;
            this.label6.Text = "提交版本ID";
            // 
            // _txtVersionID
            // 
            this._txtVersionID.Location = new System.Drawing.Point(13, 56);
            this._txtVersionID.Name = "_txtVersionID";
            this._txtVersionID.Size = new System.Drawing.Size(103, 28);
            this._txtVersionID.TabIndex = 1;
            // 
            // _btnRetrieve
            // 
            this._btnRetrieve.Location = new System.Drawing.Point(131, 54);
            this._btnRetrieve.Name = "_btnRetrieve";
            this._btnRetrieve.Size = new System.Drawing.Size(74, 30);
            this._btnRetrieve.TabIndex = 2;
            this._btnRetrieve.Text = "提取";
            this._btnRetrieve.UseVisualStyleBackColor = true;
            this._btnRetrieve.Click += new System.EventHandler(this._btnRetrieve_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 517);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._cboTfsUris);
            this.Controls.Add(this._btnClose);
            this.Controls.Add(this._btnSave);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._tbStatus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._txtTFSWorkItemIDs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._txtWorkItemIDs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._cboProjects);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "导入平台工作项";
            this.Activated += new System.EventHandler(this.MainWindow_Activated);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _tbStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _txtTFSWorkItemIDs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _txtWorkItemIDs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox _cboProjects;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox _cboTfsUris;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button _btnRetrieve;
        private System.Windows.Forms.TextBox _txtVersionID;
        private System.Windows.Forms.Label label6;
    }
}
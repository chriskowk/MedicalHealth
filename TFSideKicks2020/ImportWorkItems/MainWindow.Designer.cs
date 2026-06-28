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
            this._cboProjects = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._txtWorkItemIDs = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._txtTFSWorkItemIDs = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._tbStatus = new System.Windows.Forms.TextBox();
            this._btnSave = new System.Windows.Forms.Button();
            this._btnClose = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this._cboTfsUris = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRetrieve = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this._txtVersionID = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _cboProjects
            // 
            this._cboProjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cboProjects.FormattingEnabled = true;
            this._cboProjects.Location = new System.Drawing.Point(197, 57);
            this._cboProjects.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._cboProjects.Name = "_cboProjects";
            this._cboProjects.Size = new System.Drawing.Size(407, 27);
            this._cboProjects.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(125, 60);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "团队项目";
            // 
            // _txtWorkItemIDs
            // 
            this._txtWorkItemIDs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtWorkItemIDs.Location = new System.Drawing.Point(197, 95);
            this._txtWorkItemIDs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._txtWorkItemIDs.Multiline = true;
            this._txtWorkItemIDs.Name = "_txtWorkItemIDs";
            this._txtWorkItemIDs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtWorkItemIDs.Size = new System.Drawing.Size(407, 97);
            this._txtWorkItemIDs.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(85, 96);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "平台工作项ID串";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(63, 203);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 19);
            this.label3.TabIndex = 5;
            this.label3.Text = "TFS生成工作项ID串";
            // 
            // _txtTFSWorkItemIDs
            // 
            this._txtTFSWorkItemIDs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtTFSWorkItemIDs.Location = new System.Drawing.Point(197, 201);
            this._txtTFSWorkItemIDs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._txtTFSWorkItemIDs.Multiline = true;
            this._txtTFSWorkItemIDs.Name = "_txtTFSWorkItemIDs";
            this._txtTFSWorkItemIDs.ReadOnly = true;
            this._txtTFSWorkItemIDs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtTFSWorkItemIDs.Size = new System.Drawing.Size(407, 97);
            this._txtTFSWorkItemIDs.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(125, 309);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 19);
            this.label4.TabIndex = 7;
            this.label4.Text = "处理结果";
            // 
            // _tbStatus
            // 
            this._tbStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._tbStatus.Location = new System.Drawing.Point(197, 306);
            this._tbStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._tbStatus.Multiline = true;
            this._tbStatus.Name = "_tbStatus";
            this._tbStatus.ReadOnly = true;
            this._tbStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._tbStatus.Size = new System.Drawing.Size(409, 127);
            this._tbStatus.TabIndex = 6;
            // 
            // _btnSave
            // 
            this._btnSave.Location = new System.Drawing.Point(418, 442);
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
            this._btnClose.Location = new System.Drawing.Point(524, 442);
            this._btnClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(79, 35);
            this._btnClose.TabIndex = 9;
            this._btnClose.Text = "关闭";
            this._btnClose.UseVisualStyleBackColor = true;
            this._btnClose.Click += new System.EventHandler(this._btnClose_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(62, 20);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 19);
            this.label5.TabIndex = 11;
            this.label5.Text = "客户版本 (TFS URL)";
            // 
            // _cboTfsUris
            // 
            this._cboTfsUris.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cboTfsUris.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._cboTfsUris.FormattingEnabled = true;
            this._cboTfsUris.Location = new System.Drawing.Point(197, 17);
            this._cboTfsUris.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._cboTfsUris.Name = "_cboTfsUris";
            this._cboTfsUris.Size = new System.Drawing.Size(406, 27);
            this._cboTfsUris.TabIndex = 10;
            this._cboTfsUris.SelectedIndexChanged += new System.EventHandler(this._cboTfsUris_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRetrieve);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this._txtVersionID);
            this.groupBox1.Location = new System.Drawing.Point(23, 112);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(168, 76);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            // 
            // btnRetrieve
            // 
            this.btnRetrieve.Location = new System.Drawing.Point(81, 43);
            this.btnRetrieve.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRetrieve.Name = "btnRetrieve";
            this.btnRetrieve.Size = new System.Drawing.Size(84, 28);
            this.btnRetrieve.TabIndex = 17;
            this.btnRetrieve.Text = "提取";
            this.btnRetrieve.UseVisualStyleBackColor = true;
            this.btnRetrieve.Click += new System.EventHandler(this.btnRetrieve_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 17);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 19);
            this.label6.TabIndex = 16;
            this.label6.Text = "提交版本ID";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _txtVersionID
            // 
            this._txtVersionID.Location = new System.Drawing.Point(81, 14);
            this._txtVersionID.Name = "_txtVersionID";
            this._txtVersionID.Size = new System.Drawing.Size(84, 25);
            this._txtVersionID.TabIndex = 1;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 497);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _cboProjects;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _txtWorkItemIDs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _txtTFSWorkItemIDs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _tbStatus;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox _cboTfsUris;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRetrieve;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox _txtVersionID;
    }
}
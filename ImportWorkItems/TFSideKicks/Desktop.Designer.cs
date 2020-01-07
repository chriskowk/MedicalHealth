namespace TFSideKicks
{
    partial class Desktop
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Desktop));
            this.btnExcel = new System.Windows.Forms.Button();
            this.btnLocalService = new System.Windows.Forms.Button();
            this.btnFolder = new System.Windows.Forms.Button();
            this.lblStatus2 = new System.Windows.Forms.Label();
            this.chkIsWcfServer = new System.Windows.Forms.CheckBox();
            this.txtVersionID = new System.Windows.Forms.TextBox();
            this.chkVersion = new System.Windows.Forms.CheckBox();
            this.txtExecutePath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCheckVersion = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabComponent = new System.Windows.Forms.TabPage();
            this.lvwComponent = new System.Windows.Forms.ListView();
            this.ComponentName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LocalFileDateTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ServerFileDateTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LocalFileSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ServerFileSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabResource = new System.Windows.Forms.TabPage();
            this.lvwResource = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabDBScript = new System.Windows.Forms.TabPage();
            this.lvwDBScript = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnRar = new System.Windows.Forms.Button();
            this.txtSummary = new System.Windows.Forms.TextBox();
            this.tabMain.SuspendLayout();
            this.tabComponent.SuspendLayout();
            this.tabResource.SuspendLayout();
            this.tabDBScript.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExcel
            // 
            this.btnExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExcel.Image")));
            this.btnExcel.Location = new System.Drawing.Point(363, 41);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(23, 23);
            this.btnExcel.TabIndex = 27;
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // btnLocalService
            // 
            this.btnLocalService.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLocalService.Image = ((System.Drawing.Image)(resources.GetObject("btnLocalService.Image")));
            this.btnLocalService.Location = new System.Drawing.Point(869, 13);
            this.btnLocalService.Name = "btnLocalService";
            this.btnLocalService.Size = new System.Drawing.Size(23, 23);
            this.btnLocalService.TabIndex = 26;
            this.btnLocalService.UseVisualStyleBackColor = true;
            this.btnLocalService.Click += new System.EventHandler(this.btnLocalService_Click);
            // 
            // btnFolder
            // 
            this.btnFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFolder.Image = ((System.Drawing.Image)(resources.GetObject("btnFolder.Image")));
            this.btnFolder.Location = new System.Drawing.Point(841, 13);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(23, 23);
            this.btnFolder.TabIndex = 25;
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // lblStatus2
            // 
            this.lblStatus2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblStatus2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStatus2.Location = new System.Drawing.Point(486, 68);
            this.lblStatus2.Name = "lblStatus2";
            this.lblStatus2.Size = new System.Drawing.Size(352, 23);
            this.lblStatus2.TabIndex = 24;
            this.lblStatus2.Text = "本次实施版本资源内容均相同";
            this.lblStatus2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkIsWcfServer
            // 
            this.chkIsWcfServer.AutoSize = true;
            this.chkIsWcfServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkIsWcfServer.Location = new System.Drawing.Point(205, 17);
            this.chkIsWcfServer.Name = "chkIsWcfServer";
            this.chkIsWcfServer.Size = new System.Drawing.Size(93, 16);
            this.chkIsWcfServer.TabIndex = 23;
            this.chkIsWcfServer.Text = "中间层服务器";
            this.chkIsWcfServer.UseVisualStyleBackColor = true;
            // 
            // txtVersionID
            // 
            this.txtVersionID.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtVersionID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtVersionID.Enabled = false;
            this.txtVersionID.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtVersionID.Location = new System.Drawing.Point(272, 41);
            this.txtVersionID.Name = "txtVersionID";
            this.txtVersionID.Size = new System.Drawing.Size(90, 23);
            this.txtVersionID.TabIndex = 22;
            // 
            // chkVersion
            // 
            this.chkVersion.AutoSize = true;
            this.chkVersion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkVersion.Location = new System.Drawing.Point(205, 44);
            this.chkVersion.Name = "chkVersion";
            this.chkVersion.Size = new System.Drawing.Size(69, 16);
            this.chkVersion.TabIndex = 21;
            this.chkVersion.Text = "提交版本";
            this.chkVersion.UseVisualStyleBackColor = true;
            this.chkVersion.CheckedChanged += new System.EventHandler(this.chkVersion_CheckedChanged);
            // 
            // txtExecutePath
            // 
            this.txtExecutePath.BackColor = System.Drawing.SystemColors.Info;
            this.txtExecutePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtExecutePath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtExecutePath.Location = new System.Drawing.Point(486, 12);
            this.txtExecutePath.Name = "txtExecutePath";
            this.txtExecutePath.ReadOnly = true;
            this.txtExecutePath.Size = new System.Drawing.Size(352, 23);
            this.txtExecutePath.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(429, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 20;
            this.label2.Text = "比较结果";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStatus.Location = new System.Drawing.Point(486, 40);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(352, 23);
            this.lblStatus.TabIndex = 19;
            this.lblStatus.Text = "本次实施版本组件内容均相同";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(429, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "运行路径";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCheckVersion
            // 
            this.btnCheckVersion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckVersion.Image = ((System.Drawing.Image)(resources.GetObject("btnCheckVersion.Image")));
            this.btnCheckVersion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCheckVersion.Location = new System.Drawing.Point(12, 19);
            this.btnCheckVersion.Name = "btnCheckVersion";
            this.btnCheckVersion.Size = new System.Drawing.Size(82, 27);
            this.btnCheckVersion.TabIndex = 16;
            this.btnCheckVersion.Text = "执行比较";
            this.btnCheckVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCheckVersion.UseVisualStyleBackColor = true;
            this.btnCheckVersion.Click += new System.EventHandler(this.btnCheckVersion_Click);
            // 
            // btnExit
            // 
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.Location = new System.Drawing.Point(841, 64);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(74, 27);
            this.btnExit.TabIndex = 15;
            this.btnExit.Text = "退出";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabComponent);
            this.tabMain.Controls.Add(this.tabResource);
            this.tabMain.Controls.Add(this.tabDBScript);
            this.tabMain.Location = new System.Drawing.Point(12, 77);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(905, 498);
            this.tabMain.TabIndex = 14;
            // 
            // tabComponent
            // 
            this.tabComponent.Controls.Add(this.lvwComponent);
            this.tabComponent.Location = new System.Drawing.Point(4, 22);
            this.tabComponent.Name = "tabComponent";
            this.tabComponent.Padding = new System.Windows.Forms.Padding(3);
            this.tabComponent.Size = new System.Drawing.Size(897, 472);
            this.tabComponent.TabIndex = 0;
            this.tabComponent.Text = "组件";
            this.tabComponent.UseVisualStyleBackColor = true;
            // 
            // lvwComponent
            // 
            this.lvwComponent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwComponent.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ComponentName,
            this.LocalFileDateTime,
            this.ServerFileDateTime,
            this.LocalFileSize,
            this.ServerFileSize});
            this.lvwComponent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwComponent.FullRowSelect = true;
            this.lvwComponent.GridLines = true;
            this.lvwComponent.HideSelection = false;
            this.lvwComponent.Location = new System.Drawing.Point(3, 3);
            this.lvwComponent.MultiSelect = false;
            this.lvwComponent.Name = "lvwComponent";
            this.lvwComponent.Size = new System.Drawing.Size(891, 466);
            this.lvwComponent.TabIndex = 29;
            this.lvwComponent.UseCompatibleStateImageBehavior = false;
            this.lvwComponent.View = System.Windows.Forms.View.Details;
            // 
            // ComponentName
            // 
            this.ComponentName.Text = "组件名称";
            this.ComponentName.Width = 360;
            // 
            // LocalFileDateTime
            // 
            this.LocalFileDateTime.Text = "本地文件时间";
            this.LocalFileDateTime.Width = 150;
            // 
            // ServerFileDateTime
            // 
            this.ServerFileDateTime.Text = "服务器文件时间";
            this.ServerFileDateTime.Width = 150;
            // 
            // LocalFileSize
            // 
            this.LocalFileSize.Text = "本地文件大小";
            this.LocalFileSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.LocalFileSize.Width = 100;
            // 
            // ServerFileSize
            // 
            this.ServerFileSize.Text = "服务器文件大小";
            this.ServerFileSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ServerFileSize.Width = 100;
            // 
            // tabResource
            // 
            this.tabResource.Controls.Add(this.lvwResource);
            this.tabResource.Location = new System.Drawing.Point(4, 22);
            this.tabResource.Name = "tabResource";
            this.tabResource.Padding = new System.Windows.Forms.Padding(3);
            this.tabResource.Size = new System.Drawing.Size(897, 472);
            this.tabResource.TabIndex = 1;
            this.tabResource.Text = "资源";
            this.tabResource.UseVisualStyleBackColor = true;
            // 
            // lvwResource
            // 
            this.lvwResource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwResource.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lvwResource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwResource.FullRowSelect = true;
            this.lvwResource.GridLines = true;
            this.lvwResource.HideSelection = false;
            this.lvwResource.Location = new System.Drawing.Point(3, 3);
            this.lvwResource.MultiSelect = false;
            this.lvwResource.Name = "lvwResource";
            this.lvwResource.Size = new System.Drawing.Size(891, 466);
            this.lvwResource.TabIndex = 1;
            this.lvwResource.UseCompatibleStateImageBehavior = false;
            this.lvwResource.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "资源名称";
            this.columnHeader1.Width = 360;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "本地文件时间";
            this.columnHeader2.Width = 150;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "服务器文件时间";
            this.columnHeader3.Width = 150;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "本地文件大小";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "服务器文件大小";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader5.Width = 100;
            // 
            // tabDBScript
            // 
            this.tabDBScript.Controls.Add(this.lvwDBScript);
            this.tabDBScript.Location = new System.Drawing.Point(4, 22);
            this.tabDBScript.Name = "tabDBScript";
            this.tabDBScript.Padding = new System.Windows.Forms.Padding(3);
            this.tabDBScript.Size = new System.Drawing.Size(897, 472);
            this.tabDBScript.TabIndex = 2;
            this.tabDBScript.Text = "数据库脚本";
            this.tabDBScript.UseVisualStyleBackColor = true;
            // 
            // lvwDBScript
            // 
            this.lvwDBScript.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwDBScript.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11});
            this.lvwDBScript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwDBScript.FullRowSelect = true;
            this.lvwDBScript.GridLines = true;
            this.lvwDBScript.HideSelection = false;
            this.lvwDBScript.Location = new System.Drawing.Point(3, 3);
            this.lvwDBScript.MultiSelect = false;
            this.lvwDBScript.Name = "lvwDBScript";
            this.lvwDBScript.Size = new System.Drawing.Size(891, 466);
            this.lvwDBScript.TabIndex = 2;
            this.lvwDBScript.UseCompatibleStateImageBehavior = false;
            this.lvwDBScript.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "脚本ID";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "创建人";
            this.columnHeader7.Width = 80;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "摘要";
            this.columnHeader8.Width = 150;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "备注";
            this.columnHeader9.Width = 300;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "分支版本";
            this.columnHeader10.Width = 120;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "最后修改时间";
            this.columnHeader11.Width = 150;
            // 
            // btnRar
            // 
            this.btnRar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRar.Image = ((System.Drawing.Image)(resources.GetObject("btnRar.Image")));
            this.btnRar.Location = new System.Drawing.Point(390, 41);
            this.btnRar.Name = "btnRar";
            this.btnRar.Size = new System.Drawing.Size(23, 23);
            this.btnRar.TabIndex = 29;
            this.btnRar.UseVisualStyleBackColor = true;
            this.btnRar.Click += new System.EventHandler(this.btnRar_Click);
            // 
            // txtSummary
            // 
            this.txtSummary.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSummary.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtSummary.Location = new System.Drawing.Point(205, 68);
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.ReadOnly = true;
            this.txtSummary.Size = new System.Drawing.Size(253, 23);
            this.txtSummary.TabIndex = 30;
            // 
            // Desktop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(927, 581);
            this.Controls.Add(this.txtSummary);
            this.Controls.Add(this.btnRar);
            this.Controls.Add(this.btnExcel);
            this.Controls.Add(this.btnLocalService);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.lblStatus2);
            this.Controls.Add(this.chkIsWcfServer);
            this.Controls.Add(this.txtVersionID);
            this.Controls.Add(this.chkVersion);
            this.Controls.Add(this.txtExecutePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCheckVersion);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.tabMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Desktop";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "版本比较工具";
            this.Load += new System.EventHandler(this.Desktop_Load);
            this.tabMain.ResumeLayout(false);
            this.tabComponent.ResumeLayout(false);
            this.tabResource.ResumeLayout(false);
            this.tabDBScript.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Button btnLocalService;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.Label lblStatus2;
        private System.Windows.Forms.CheckBox chkIsWcfServer;
        private System.Windows.Forms.TextBox txtVersionID;
        private System.Windows.Forms.CheckBox chkVersion;
        private System.Windows.Forms.TextBox txtExecutePath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCheckVersion;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabComponent;
        private System.Windows.Forms.TabPage tabResource;
        private System.Windows.Forms.ListView lvwResource;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.TabPage tabDBScript;
        private System.Windows.Forms.ListView lvwComponent;
        private System.Windows.Forms.ColumnHeader ComponentName;
        private System.Windows.Forms.ColumnHeader LocalFileDateTime;
        private System.Windows.Forms.ColumnHeader ServerFileDateTime;
        private System.Windows.Forms.ColumnHeader LocalFileSize;
        private System.Windows.Forms.ColumnHeader ServerFileSize;
        private System.Windows.Forms.ListView lvwDBScript;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.Button btnRar;
        private System.Windows.Forms.TextBox txtSummary;
    }
}
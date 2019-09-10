namespace JobController
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lblCurState = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tscbStart = new System.Windows.Forms.ToolStripButton();
            this.tscbStop = new System.Windows.Forms.ToolStripButton();
            this.tscbExit = new System.Windows.Forms.ToolStripButton();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblDuration = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._txtWorkItemIDs = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._btnBuild = new System.Windows.Forms.Button();
            this._txtStatus = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._cboProjects = new System.Windows.Forms.ComboBox();
            this._dtpRestart = new System.Windows.Forms.DateTimePicker();
            this._btnResetJobConfig = new System.Windows.Forms.Button();
            this._rbZSYK = new System.Windows.Forms.RadioButton();
            this._rbSY = new System.Windows.Forms.RadioButton();
            this._btnRegYK = new System.Windows.Forms.Button();
            this._btnRegSY = new System.Windows.Forms.Button();
            this._btnStartServices = new System.Windows.Forms.Button();
            this._rbS12 = new System.Windows.Forms.RadioButton();
            this._btnRegS12 = new System.Windows.Forms.Button();
            this._btnRegGH = new System.Windows.Forms.Button();
            this._rbGH = new System.Windows.Forms.RadioButton();
            this._btnRegS1 = new System.Windows.Forms.Button();
            this._rbS1 = new System.Windows.Forms.RadioButton();
            this._btnRegSGS1 = new System.Windows.Forms.Button();
            this._rbSGS1 = new System.Windows.Forms.RadioButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCurState
            // 
            this.lblCurState.AutoSize = true;
            this.lblCurState.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lblCurState.Location = new System.Drawing.Point(12, 181);
            this.lblCurState.Name = "lblCurState";
            this.lblCurState.Size = new System.Drawing.Size(65, 12);
            this.lblCurState.TabIndex = 3;
            this.lblCurState.Text = "作业已启动";
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tscbStart,
            this.tscbStop,
            this.tscbExit});
            this.toolStrip1.Location = new System.Drawing.Point(472, 193);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(159, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tscbStart
            // 
            this.tscbStart.Enabled = false;
            this.tscbStart.Image = ((System.Drawing.Image)(resources.GetObject("tscbStart.Image")));
            this.tscbStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tscbStart.Name = "tscbStart";
            this.tscbStart.Size = new System.Drawing.Size(52, 22);
            this.tscbStart.Text = "启动";
            this.tscbStart.ToolTipText = "启动定时注册";
            this.tscbStart.Click += new System.EventHandler(this.tscbStart_Click);
            // 
            // tscbStop
            // 
            this.tscbStop.Image = ((System.Drawing.Image)(resources.GetObject("tscbStop.Image")));
            this.tscbStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tscbStop.Name = "tscbStop";
            this.tscbStop.Size = new System.Drawing.Size(52, 22);
            this.tscbStop.Text = "暂停";
            this.tscbStop.ToolTipText = "停止定时注册";
            this.tscbStop.Click += new System.EventHandler(this.tscbStop_Click);
            // 
            // tscbExit
            // 
            this.tscbExit.Image = ((System.Drawing.Image)(resources.GetObject("tscbExit.Image")));
            this.tscbExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tscbExit.Name = "tscbExit";
            this.tscbExit.Size = new System.Drawing.Size(52, 22);
            this.tscbExit.Text = "退出";
            this.tscbExit.ToolTipText = "退出程序";
            this.tscbExit.Click += new System.EventHandler(this.tscbExit_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "作业调度控制器";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // lblDuration
            // 
            this.lblDuration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDuration.Font = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblDuration.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblDuration.Location = new System.Drawing.Point(279, 194);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(97, 26);
            this.lblDuration.TabIndex = 8;
            this.lblDuration.Text = "00:00:00";
            this.lblDuration.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label1.Location = new System.Drawing.Point(194, 203);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "Time elapsed:";
            // 
            // _txtWorkItemIDs
            // 
            this._txtWorkItemIDs.Location = new System.Drawing.Point(12, 76);
            this._txtWorkItemIDs.Multiline = true;
            this._txtWorkItemIDs.Name = "_txtWorkItemIDs";
            this._txtWorkItemIDs.Size = new System.Drawing.Size(614, 91);
            this._txtWorkItemIDs.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label2.Location = new System.Drawing.Point(12, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "工作项ID串";
            // 
            // _btnBuild
            // 
            this._btnBuild.Location = new System.Drawing.Point(477, 42);
            this._btnBuild.Name = "_btnBuild";
            this._btnBuild.Size = new System.Drawing.Size(149, 29);
            this._btnBuild.TabIndex = 12;
            this._btnBuild.Text = "Sysnc Build WorkItems";
            this._btnBuild.UseVisualStyleBackColor = true;
            this._btnBuild.Click += new System.EventHandler(this._btnBuild_Click);
            // 
            // _txtStatus
            // 
            this._txtStatus.Location = new System.Drawing.Point(12, 292);
            this._txtStatus.Multiline = true;
            this._txtStatus.Name = "_txtStatus";
            this._txtStatus.ReadOnly = true;
            this._txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtStatus.Size = new System.Drawing.Size(614, 63);
            this._txtStatus.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label3.Location = new System.Drawing.Point(12, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "团队项目";
            // 
            // _cboProjects
            // 
            this._cboProjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cboProjects.FormattingEnabled = true;
            this._cboProjects.Location = new System.Drawing.Point(77, 16);
            this._cboProjects.Name = "_cboProjects";
            this._cboProjects.Size = new System.Drawing.Size(549, 20);
            this._cboProjects.TabIndex = 15;
            // 
            // _dtpRestart
            // 
            this._dtpRestart.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this._dtpRestart.Location = new System.Drawing.Point(12, 199);
            this._dtpRestart.Name = "_dtpRestart";
            this._dtpRestart.Size = new System.Drawing.Size(165, 21);
            this._dtpRestart.TabIndex = 16;
            // 
            // _btnResetJobConfig
            // 
            this._btnResetJobConfig.Location = new System.Drawing.Point(552, 230);
            this._btnResetJobConfig.Name = "_btnResetJobConfig";
            this._btnResetJobConfig.Size = new System.Drawing.Size(74, 24);
            this._btnResetJobConfig.TabIndex = 17;
            this._btnResetJobConfig.Text = "Reset";
            this._btnResetJobConfig.UseVisualStyleBackColor = true;
            this._btnResetJobConfig.Click += new System.EventHandler(this._btnResetJobConfig_Click);
            // 
            // _rbZSYK
            // 
            this._rbZSYK.AutoSize = true;
            this._rbZSYK.Location = new System.Drawing.Point(14, 238);
            this._rbZSYK.Name = "_rbZSYK";
            this._rbZSYK.Size = new System.Drawing.Size(47, 16);
            this._rbZSYK.TabIndex = 18;
            this._rbZSYK.TabStop = true;
            this._rbZSYK.Tag = "MyJob.TaskJob";
            this._rbZSYK.Text = "眼科";
            this._rbZSYK.UseVisualStyleBackColor = true;
            this._rbZSYK.CheckedChanged += new System.EventHandler(this._optCustomer_CheckedChanged);
            // 
            // _rbSY
            // 
            this._rbSY.AutoSize = true;
            this._rbSY.Location = new System.Drawing.Point(101, 238);
            this._rbSY.Name = "_rbSY";
            this._rbSY.Size = new System.Drawing.Size(47, 16);
            this._rbSY.TabIndex = 19;
            this._rbSY.TabStop = true;
            this._rbSY.Tag = "MyJob.TaskJobA";
            this._rbSY.Text = "省医";
            this._rbSY.UseVisualStyleBackColor = true;
            this._rbSY.CheckedChanged += new System.EventHandler(this._optCustomer_CheckedChanged);
            // 
            // _btnRegYK
            // 
            this._btnRegYK.Location = new System.Drawing.Point(14, 261);
            this._btnRegYK.Name = "_btnRegYK";
            this._btnRegYK.Size = new System.Drawing.Size(74, 24);
            this._btnRegYK.TabIndex = 20;
            this._btnRegYK.Text = "眼科配置";
            this._btnRegYK.UseVisualStyleBackColor = true;
            this._btnRegYK.Click += new System.EventHandler(this._btnRegYK_Click);
            // 
            // _btnRegSY
            // 
            this._btnRegSY.Location = new System.Drawing.Point(101, 261);
            this._btnRegSY.Name = "_btnRegSY";
            this._btnRegSY.Size = new System.Drawing.Size(74, 24);
            this._btnRegSY.TabIndex = 21;
            this._btnRegSY.Text = "省医配置";
            this._btnRegSY.UseVisualStyleBackColor = true;
            this._btnRegSY.Click += new System.EventHandler(this._tbnRegSY_Click);
            // 
            // _btnStartServices
            // 
            this._btnStartServices.Location = new System.Drawing.Point(552, 260);
            this._btnStartServices.Name = "_btnStartServices";
            this._btnStartServices.Size = new System.Drawing.Size(74, 24);
            this._btnStartServices.TabIndex = 22;
            this._btnStartServices.Text = "重启服务";
            this._btnStartServices.UseVisualStyleBackColor = true;
            this._btnStartServices.Click += new System.EventHandler(this._btnStartServices_Click);
            // 
            // _rbS12
            // 
            this._rbS12.AutoSize = true;
            this._rbS12.Location = new System.Drawing.Point(188, 239);
            this._rbS12.Name = "_rbS12";
            this._rbS12.Size = new System.Drawing.Size(59, 16);
            this._rbS12.TabIndex = 23;
            this._rbS12.TabStop = true;
            this._rbS12.Tag = "MyJob.TaskJobB";
            this._rbS12.Text = "市十二";
            this._rbS12.UseVisualStyleBackColor = true;
            this._rbS12.CheckedChanged += new System.EventHandler(this._optCustomer_CheckedChanged);
            // 
            // _btnRegS12
            // 
            this._btnRegS12.Location = new System.Drawing.Point(188, 261);
            this._btnRegS12.Name = "_btnRegS12";
            this._btnRegS12.Size = new System.Drawing.Size(74, 24);
            this._btnRegS12.TabIndex = 24;
            this._btnRegS12.Text = "市十二配置";
            this._btnRegS12.UseVisualStyleBackColor = true;
            this._btnRegS12.Click += new System.EventHandler(this._btnRegS12_Click);
            // 
            // _btnRegGH
            // 
            this._btnRegGH.Location = new System.Drawing.Point(275, 261);
            this._btnRegGH.Name = "_btnRegGH";
            this._btnRegGH.Size = new System.Drawing.Size(74, 24);
            this._btnRegGH.TabIndex = 26;
            this._btnRegGH.Text = "光华配置";
            this._btnRegGH.UseVisualStyleBackColor = true;
            this._btnRegGH.Click += new System.EventHandler(this._btnRegGH_Click);
            // 
            // _rbGH
            // 
            this._rbGH.AutoSize = true;
            this._rbGH.Location = new System.Drawing.Point(275, 238);
            this._rbGH.Name = "_rbGH";
            this._rbGH.Size = new System.Drawing.Size(71, 16);
            this._rbGH.TabIndex = 25;
            this._rbGH.TabStop = true;
            this._rbGH.Tag = "MyJob.TaskJobC";
            this._rbGH.Text = "光华口腔";
            this._rbGH.UseVisualStyleBackColor = true;
            this._rbGH.CheckedChanged += new System.EventHandler(this._optCustomer_CheckedChanged);
            // 
            // _btnRegS1
            // 
            this._btnRegS1.Location = new System.Drawing.Point(362, 260);
            this._btnRegS1.Name = "_btnRegS1";
            this._btnRegS1.Size = new System.Drawing.Size(74, 24);
            this._btnRegS1.TabIndex = 28;
            this._btnRegS1.Text = "市一配置";
            this._btnRegS1.UseVisualStyleBackColor = true;
            this._btnRegS1.Click += new System.EventHandler(this._btnRegS1_Click);
            // 
            // _rbS1
            // 
            this._rbS1.AutoSize = true;
            this._rbS1.Location = new System.Drawing.Point(362, 239);
            this._rbS1.Name = "_rbS1";
            this._rbS1.Size = new System.Drawing.Size(47, 16);
            this._rbS1.TabIndex = 27;
            this._rbS1.TabStop = true;
            this._rbS1.Tag = "MyJob.TaskJobD";
            this._rbS1.Text = "市一";
            this._rbS1.UseVisualStyleBackColor = true;
            this._rbS1.CheckedChanged += new System.EventHandler(this._optCustomer_CheckedChanged);
            // 
            // _btnRegSGS1
            // 
            this._btnRegSGS1.Location = new System.Drawing.Point(451, 259);
            this._btnRegSGS1.Name = "_btnRegSGS1";
            this._btnRegSGS1.Size = new System.Drawing.Size(74, 24);
            this._btnRegSGS1.TabIndex = 30;
            this._btnRegSGS1.Text = "韶关配置";
            this._btnRegSGS1.UseVisualStyleBackColor = true;
            this._btnRegSGS1.Click += new System.EventHandler(this._btnRegSGS1_Click);
            // 
            // _rbSGS1
            // 
            this._rbSGS1.AutoSize = true;
            this._rbSGS1.Location = new System.Drawing.Point(451, 238);
            this._rbSGS1.Name = "_rbSGS1";
            this._rbSGS1.Size = new System.Drawing.Size(71, 16);
            this._rbSGS1.TabIndex = 29;
            this._rbSGS1.TabStop = true;
            this._rbSGS1.Tag = "MyJob.TaskJobE";
            this._rbSGS1.Text = "韶关市一";
            this._rbSGS1.UseVisualStyleBackColor = true;
            this._rbSGS1.CheckedChanged += new System.EventHandler(this._optCustomer_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 366);
            this.Controls.Add(this._btnRegSGS1);
            this.Controls.Add(this._rbSGS1);
            this.Controls.Add(this._btnRegS1);
            this.Controls.Add(this._rbS1);
            this.Controls.Add(this._btnRegGH);
            this.Controls.Add(this._rbGH);
            this.Controls.Add(this._btnRegS12);
            this.Controls.Add(this._rbS12);
            this.Controls.Add(this._btnStartServices);
            this.Controls.Add(this._btnRegSY);
            this.Controls.Add(this._btnRegYK);
            this.Controls.Add(this._rbSY);
            this.Controls.Add(this._rbZSYK);
            this.Controls.Add(this._btnResetJobConfig);
            this.Controls.Add(this._dtpRestart);
            this.Controls.Add(this._cboProjects);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._txtStatus);
            this.Controls.Add(this._btnBuild);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._txtWorkItemIDs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.lblCurState);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "作业调度控制器";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCurState;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tscbStart;
        private System.Windows.Forms.ToolStripButton tscbStop;
        private System.Windows.Forms.ToolStripButton tscbExit;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _txtWorkItemIDs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button _btnBuild;
        private System.Windows.Forms.TextBox _txtStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox _cboProjects;
        private System.Windows.Forms.DateTimePicker _dtpRestart;
        private System.Windows.Forms.Button _btnResetJobConfig;
        private System.Windows.Forms.RadioButton _rbZSYK;
        private System.Windows.Forms.RadioButton _rbSY;
        private System.Windows.Forms.Button _btnRegYK;
        private System.Windows.Forms.Button _btnRegSY;
        private System.Windows.Forms.Button _btnStartServices;
        private System.Windows.Forms.RadioButton _rbS12;
        private System.Windows.Forms.Button _btnRegS12;
        private System.Windows.Forms.Button _btnRegGH;
        private System.Windows.Forms.RadioButton _rbGH;
        private System.Windows.Forms.Button _btnRegS1;
        private System.Windows.Forms.RadioButton _rbS1;
        private System.Windows.Forms.Button _btnRegSGS1;
        private System.Windows.Forms.RadioButton _rbSGS1;
    }
}


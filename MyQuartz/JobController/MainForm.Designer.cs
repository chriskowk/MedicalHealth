namespace JobController
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lblCurState = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tscbStart = new System.Windows.Forms.ToolStripButton();
            this.tscbStop = new System.Windows.Forms.ToolStripButton();
            this.tscbExit = new System.Windows.Forms.ToolStripButton();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblDuration = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._txtWorkItemIDs = new System.Windows.Forms.TextBox();
            this.lblRef = new System.Windows.Forms.Label();
            this._btnBuild = new System.Windows.Forms.Button();
            this._txtStatus = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._cboProjects = new System.Windows.Forms.ComboBox();
            this._dtpRestart = new System.Windows.Forms.DateTimePicker();
            this._btnResetJobConfig = new System.Windows.Forms.Button();
            this._btnStartServices = new System.Windows.Forms.Button();
            this._btnRegConfig = new System.Windows.Forms.Button();
            this.panCustomers = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this._txtVersionID = new System.Windows.Forms.TextBox();
            this.btnRetrieve = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCurState
            // 
            this.lblCurState.AutoSize = true;
            this.lblCurState.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lblCurState.Location = new System.Drawing.Point(10, 174);
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
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tscbStart,
            this.tscbStop,
            this.tscbExit});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(404, 187);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(184, 31);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tscbStart
            // 
            this.tscbStart.Enabled = false;
            this.tscbStart.Image = ((System.Drawing.Image)(resources.GetObject("tscbStart.Image")));
            this.tscbStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tscbStart.Name = "tscbStart";
            this.tscbStart.Size = new System.Drawing.Size(60, 28);
            this.tscbStart.Text = "启动";
            this.tscbStart.ToolTipText = "启动定时任务";
            this.tscbStart.Click += new System.EventHandler(this.tscbStart_Click);
            // 
            // tscbStop
            // 
            this.tscbStop.Image = ((System.Drawing.Image)(resources.GetObject("tscbStop.Image")));
            this.tscbStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tscbStop.Name = "tscbStop";
            this.tscbStop.Size = new System.Drawing.Size(60, 28);
            this.tscbStop.Text = "暂停";
            this.tscbStop.ToolTipText = "停止定时任务";
            this.tscbStop.Click += new System.EventHandler(this.tscbStop_Click);
            // 
            // tscbExit
            // 
            this.tscbExit.Image = ((System.Drawing.Image)(resources.GetObject("tscbExit.Image")));
            this.tscbExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tscbExit.Name = "tscbExit";
            this.tscbExit.Size = new System.Drawing.Size(60, 28);
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
            this.lblDuration.Location = new System.Drawing.Point(278, 187);
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
            this.label1.Location = new System.Drawing.Point(193, 195);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "Time elapsed:";
            // 
            // _txtWorkItemIDs
            // 
            this._txtWorkItemIDs.Location = new System.Drawing.Point(10, 72);
            this._txtWorkItemIDs.Multiline = true;
            this._txtWorkItemIDs.Name = "_txtWorkItemIDs";
            this._txtWorkItemIDs.Size = new System.Drawing.Size(578, 90);
            this._txtWorkItemIDs.TabIndex = 10;
            // 
            // lblRef
            // 
            this.lblRef.AutoSize = true;
            this.lblRef.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lblRef.Location = new System.Drawing.Point(10, 53);
            this.lblRef.Name = "lblRef";
            this.lblRef.Size = new System.Drawing.Size(65, 12);
            this.lblRef.TabIndex = 11;
            this.lblRef.Text = "工作项ID串";
            // 
            // _btnBuild
            // 
            this._btnBuild.Location = new System.Drawing.Point(434, 41);
            this._btnBuild.Name = "_btnBuild";
            this._btnBuild.Size = new System.Drawing.Size(155, 28);
            this._btnBuild.TabIndex = 12;
            this._btnBuild.Text = "Sysnc Build WorkItems";
            this._btnBuild.UseVisualStyleBackColor = true;
            this._btnBuild.Click += new System.EventHandler(this._btnBuild_Click);
            // 
            // _txtStatus
            // 
            this._txtStatus.Location = new System.Drawing.Point(10, 309);
            this._txtStatus.Multiline = true;
            this._txtStatus.Name = "_txtStatus";
            this._txtStatus.ReadOnly = true;
            this._txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtStatus.Size = new System.Drawing.Size(578, 63);
            this._txtStatus.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label3.Location = new System.Drawing.Point(10, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "团队项目";
            // 
            // _cboProjects
            // 
            this._cboProjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cboProjects.FormattingEnabled = true;
            this._cboProjects.Location = new System.Drawing.Point(77, 11);
            this._cboProjects.Name = "_cboProjects";
            this._cboProjects.Size = new System.Drawing.Size(511, 20);
            this._cboProjects.TabIndex = 15;
            // 
            // _dtpRestart
            // 
            this._dtpRestart.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this._dtpRestart.Location = new System.Drawing.Point(10, 191);
            this._dtpRestart.Name = "_dtpRestart";
            this._dtpRestart.Size = new System.Drawing.Size(165, 21);
            this._dtpRestart.TabIndex = 16;
            // 
            // _btnResetJobConfig
            // 
            this._btnResetJobConfig.Location = new System.Drawing.Point(415, 230);
            this._btnResetJobConfig.Name = "_btnResetJobConfig";
            this._btnResetJobConfig.Size = new System.Drawing.Size(84, 28);
            this._btnResetJobConfig.TabIndex = 17;
            this._btnResetJobConfig.Text = "Reset";
            this._btnResetJobConfig.UseVisualStyleBackColor = true;
            this._btnResetJobConfig.Click += new System.EventHandler(this._btnResetJobConfig_Click);
            // 
            // _btnStartServices
            // 
            this._btnStartServices.Location = new System.Drawing.Point(505, 230);
            this._btnStartServices.Name = "_btnStartServices";
            this._btnStartServices.Size = new System.Drawing.Size(84, 28);
            this._btnStartServices.TabIndex = 22;
            this._btnStartServices.Text = "重启服务";
            this._btnStartServices.UseVisualStyleBackColor = true;
            this._btnStartServices.Click += new System.EventHandler(this._btnStartServices_Click);
            // 
            // _btnRegConfig
            // 
            this._btnRegConfig.Location = new System.Drawing.Point(506, 265);
            this._btnRegConfig.Name = "_btnRegConfig";
            this._btnRegConfig.Size = new System.Drawing.Size(84, 28);
            this._btnRegConfig.TabIndex = 30;
            this._btnRegConfig.Text = "重设配置";
            this._btnRegConfig.UseVisualStyleBackColor = true;
            this._btnRegConfig.Click += new System.EventHandler(this._btnRegConfig_Click);
            // 
            // panCustomers
            // 
            this.panCustomers.AutoScroll = true;
            this.panCustomers.Location = new System.Drawing.Point(10, 216);
            this.panCustomers.Margin = new System.Windows.Forms.Padding(2);
            this.panCustomers.Name = "panCustomers";
            this.panCustomers.Size = new System.Drawing.Size(400, 90);
            this.panCustomers.TabIndex = 31;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(149, 53);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 33;
            this.label6.Text = "提交版本ID";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _txtVersionID
            // 
            this._txtVersionID.Location = new System.Drawing.Point(217, 48);
            this._txtVersionID.Name = "_txtVersionID";
            this._txtVersionID.Size = new System.Drawing.Size(84, 21);
            this._txtVersionID.TabIndex = 32;
            // 
            // btnRetrieve
            // 
            this.btnRetrieve.Location = new System.Drawing.Point(308, 41);
            this.btnRetrieve.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRetrieve.Name = "btnRetrieve";
            this.btnRetrieve.Size = new System.Drawing.Size(84, 28);
            this.btnRetrieve.TabIndex = 34;
            this.btnRetrieve.Text = "提取";
            this.btnRetrieve.UseVisualStyleBackColor = true;
            this.btnRetrieve.Click += new System.EventHandler(this.btnRetrieve_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 376);
            this.Controls.Add(this.btnRetrieve);
            this.Controls.Add(this.label6);
            this.Controls.Add(this._txtVersionID);
            this.Controls.Add(this.panCustomers);
            this.Controls.Add(this._btnRegConfig);
            this.Controls.Add(this._btnStartServices);
            this.Controls.Add(this._btnResetJobConfig);
            this.Controls.Add(this._dtpRestart);
            this.Controls.Add(this._cboProjects);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._txtStatus);
            this.Controls.Add(this._btnBuild);
            this.Controls.Add(this.lblRef);
            this.Controls.Add(this._txtWorkItemIDs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.lblCurState);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "作业调度控制器";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
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
        private System.Windows.Forms.Label lblRef;
        private System.Windows.Forms.Button _btnBuild;
        private System.Windows.Forms.TextBox _txtStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox _cboProjects;
        private System.Windows.Forms.DateTimePicker _dtpRestart;
        private System.Windows.Forms.Button _btnResetJobConfig;
        private System.Windows.Forms.Button _btnStartServices;
        private System.Windows.Forms.Button _btnRegConfig;
        private System.Windows.Forms.Panel panCustomers;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox _txtVersionID;
        private System.Windows.Forms.Button btnRetrieve;
    }
}


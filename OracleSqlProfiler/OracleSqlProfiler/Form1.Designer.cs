namespace OracleSqlProfiler
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tb_log = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_password = new System.Windows.Forms.TextBox();
            this.tb_user = new System.Windows.Forms.TextBox();
            this.tb_source = new System.Windows.Forms.TextBox();
            this.tb_oraname = new System.Windows.Forms.TextBox();
            this.cb_save = new System.Windows.Forms.CheckBox();
            this.cb_curruser = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tb_Status = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tb_log
            // 
            this.tb_log.Location = new System.Drawing.Point(475, 7);
            this.tb_log.Margin = new System.Windows.Forms.Padding(4);
            this.tb_log.Multiline = true;
            this.tb_log.Name = "tb_log";
            this.tb_log.ReadOnly = true;
            this.tb_log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_log.Size = new System.Drawing.Size(419, 177);
            this.tb_log.TabIndex = 8;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(8, 216);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(890, 382);
            this.dataGridView1.TabIndex = 9;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 199);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(574, 14);
            this.label5.TabIndex = 14;
            this.label5.Text = "shows only 100 lines order by elapsed_time desc,  see more to save data to table." +
    "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tb_password);
            this.groupBox1.Controls.Add(this.tb_user);
            this.groupBox1.Controls.Add(this.tb_source);
            this.groupBox1.Controls.Add(this.tb_oraname);
            this.groupBox1.Controls.Add(this.cb_save);
            this.groupBox1.Controls.Add(this.cb_curruser);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(8, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(460, 184);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(61, 51);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(189, 14);
            this.label4.TabIndex = 24;
            this.label4.Text = "like xx.xx.xx.xx:1521/orcl";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(248, 86);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 14);
            this.label3.TabIndex = 23;
            this.label3.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 86);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 14);
            this.label2.TabIndex = 22;
            this.label2.Text = "User";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 14);
            this.label1.TabIndex = 21;
            this.label1.Text = "Source";
            // 
            // tb_password
            // 
            this.tb_password.Location = new System.Drawing.Point(317, 83);
            this.tb_password.Margin = new System.Windows.Forms.Padding(4);
            this.tb_password.Name = "tb_password";
            this.tb_password.Size = new System.Drawing.Size(134, 22);
            this.tb_password.TabIndex = 2;
            // 
            // tb_user
            // 
            this.tb_user.Location = new System.Drawing.Point(63, 83);
            this.tb_user.Margin = new System.Windows.Forms.Padding(4);
            this.tb_user.Name = "tb_user";
            this.tb_user.Size = new System.Drawing.Size(134, 22);
            this.tb_user.TabIndex = 1;
            // 
            // tb_source
            // 
            this.tb_source.Location = new System.Drawing.Point(63, 23);
            this.tb_source.Margin = new System.Windows.Forms.Padding(4);
            this.tb_source.Name = "tb_source";
            this.tb_source.Size = new System.Drawing.Size(388, 22);
            this.tb_source.TabIndex = 0;
            // 
            // tb_oraname
            // 
            this.tb_oraname.Location = new System.Drawing.Point(128, 154);
            this.tb_oraname.Margin = new System.Windows.Forms.Padding(4);
            this.tb_oraname.Name = "tb_oraname";
            this.tb_oraname.Size = new System.Drawing.Size(151, 22);
            this.tb_oraname.TabIndex = 5;
            // 
            // cb_save
            // 
            this.cb_save.AutoSize = true;
            this.cb_save.Location = new System.Drawing.Point(13, 156);
            this.cb_save.Margin = new System.Windows.Forms.Padding(4);
            this.cb_save.Name = "cb_save";
            this.cb_save.Size = new System.Drawing.Size(117, 18);
            this.cb_save.TabIndex = 4;
            this.cb_save.Text = "Save to table";
            this.cb_save.UseVisualStyleBackColor = true;
            this.cb_save.CheckedChanged += new System.EventHandler(this.cb_save_CheckedChanged);
            // 
            // cb_curruser
            // 
            this.cb_curruser.AutoSize = true;
            this.cb_curruser.Location = new System.Drawing.Point(13, 125);
            this.cb_curruser.Margin = new System.Windows.Forms.Padding(4);
            this.cb_curruser.Name = "cb_curruser";
            this.cb_curruser.Size = new System.Drawing.Size(187, 18);
            this.cb_curruser.TabIndex = 3;
            this.cb_curruser.Text = "Trace only current user";
            this.cb_curruser.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(370, 148);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(77, 34);
            this.button2.TabIndex = 7;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(286, 148);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(77, 34);
            this.button1.TabIndex = 6;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tb_Status
            // 
            this.tb_Status.Location = new System.Drawing.Point(8, 605);
            this.tb_Status.Margin = new System.Windows.Forms.Padding(4);
            this.tb_Status.Multiline = true;
            this.tb_Status.Name = "tb_Status";
            this.tb_Status.ReadOnly = true;
            this.tb_Status.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_Status.Size = new System.Drawing.Size(889, 74);
            this.tb_Status.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(910, 685);
            this.Controls.Add(this.tb_Status);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.tb_log);
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "OracleSqlProfiler";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tb_log;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_password;
        private System.Windows.Forms.TextBox tb_user;
        private System.Windows.Forms.TextBox tb_source;
        private System.Windows.Forms.TextBox tb_oraname;
        private System.Windows.Forms.CheckBox cb_save;
        private System.Windows.Forms.CheckBox cb_curruser;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tb_Status;
    }
}


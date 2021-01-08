namespace WinCCReportControl
{
    partial class HourReport
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

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
			this.DataGridView1 = new System.Windows.Forms.DataGridView();
			this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
			this.ToolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.BackgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
			this.btnExport = new System.Windows.Forms.Button();
			this.btnReport = new System.Windows.Forms.Button();
			this.endTime = new System.Windows.Forms.DateTimePicker();
			this.Label2 = new System.Windows.Forms.Label();
			this.startTime = new System.Windows.Forms.DateTimePicker();
			this.Label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.DataGridView1)).BeginInit();
			this.StatusStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).BeginInit();
			this.SplitContainer1.Panel1.SuspendLayout();
			this.SplitContainer1.Panel2.SuspendLayout();
			this.SplitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// DataGridView1
			// 
			this.DataGridView1.AllowUserToAddRows = false;
			this.DataGridView1.AllowUserToDeleteRows = false;
			this.DataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.DataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DataGridView1.Location = new System.Drawing.Point(0, 0);
			this.DataGridView1.Name = "DataGridView1";
			this.DataGridView1.ReadOnly = true;
			this.DataGridView1.RowTemplate.Height = 23;
			this.DataGridView1.Size = new System.Drawing.Size(310, 492);
			this.DataGridView1.TabIndex = 0;
			// 
			// StatusStrip1
			// 
			this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripStatusLabel1,
            this.ProgressBar});
			this.StatusStrip1.Location = new System.Drawing.Point(0, 566);
			this.StatusStrip1.Name = "StatusStrip1";
			this.StatusStrip1.Size = new System.Drawing.Size(310, 22);
			this.StatusStrip1.SizingGrip = false;
			this.StatusStrip1.Stretch = false;
			this.StatusStrip1.TabIndex = 7;
			this.StatusStrip1.Text = "StatusStrip1";
			// 
			// ToolStripStatusLabel1
			// 
			this.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1";
			this.ToolStripStatusLabel1.Size = new System.Drawing.Size(35, 17);
			this.ToolStripStatusLabel1.Text = "进度:";
			// 
			// ProgressBar
			// 
			this.ProgressBar.Name = "ProgressBar";
			this.ProgressBar.Size = new System.Drawing.Size(200, 16);
			// 
			// BackgroundWorker
			// 
			this.BackgroundWorker.WorkerReportsProgress = true;
			this.BackgroundWorker.WorkerSupportsCancellation = true;
			this.BackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker_DoWork);
			// 
			// SplitContainer1
			// 
			this.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.SplitContainer1.Location = new System.Drawing.Point(0, 0);
			this.SplitContainer1.Name = "SplitContainer1";
			this.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// SplitContainer1.Panel1
			// 
			this.SplitContainer1.Panel1.Controls.Add(this.btnExport);
			this.SplitContainer1.Panel1.Controls.Add(this.btnReport);
			this.SplitContainer1.Panel1.Controls.Add(this.endTime);
			this.SplitContainer1.Panel1.Controls.Add(this.Label2);
			this.SplitContainer1.Panel1.Controls.Add(this.startTime);
			this.SplitContainer1.Panel1.Controls.Add(this.Label1);
			// 
			// SplitContainer1.Panel2
			// 
			this.SplitContainer1.Panel2.Controls.Add(this.DataGridView1);
			this.SplitContainer1.Size = new System.Drawing.Size(310, 588);
			this.SplitContainer1.SplitterDistance = 92;
			this.SplitContainer1.TabIndex = 8;
			// 
			// btnExport
			// 
			this.btnExport.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnExport.Location = new System.Drawing.Point(187, 61);
			this.btnExport.Name = "btnExport";
			this.btnExport.Size = new System.Drawing.Size(75, 23);
			this.btnExport.TabIndex = 5;
			this.btnExport.Text = "导出";
			this.btnExport.UseVisualStyleBackColor = true;
			this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
			// 
			// btnReport
			// 
			this.btnReport.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnReport.Location = new System.Drawing.Point(82, 61);
			this.btnReport.Name = "btnReport";
			this.btnReport.Size = new System.Drawing.Size(75, 23);
			this.btnReport.TabIndex = 4;
			this.btnReport.Text = "查询";
			this.btnReport.UseVisualStyleBackColor = true;
			this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
			// 
			// endTime
			// 
			this.endTime.CustomFormat = "yyyy-MM-dd 08:00:00";
			this.endTime.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.endTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.endTime.Location = new System.Drawing.Point(72, 34);
			this.endTime.Name = "endTime";
			this.endTime.Size = new System.Drawing.Size(200, 23);
			this.endTime.TabIndex = 3;
			// 
			// Label2
			// 
			this.Label2.AutoSize = true;
			this.Label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Label2.Location = new System.Drawing.Point(7, 40);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(59, 17);
			this.Label2.TabIndex = 2;
			this.Label2.Text = "结束时间:";
			// 
			// startTime
			// 
			this.startTime.CustomFormat = "yyyy-MM-dd 08:00:00";
			this.startTime.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.startTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.startTime.Location = new System.Drawing.Point(72, 7);
			this.startTime.Name = "startTime";
			this.startTime.Size = new System.Drawing.Size(200, 23);
			this.startTime.TabIndex = 1;
			// 
			// Label1
			// 
			this.Label1.AutoSize = true;
			this.Label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Label1.Location = new System.Drawing.Point(7, 13);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(59, 17);
			this.Label1.TabIndex = 0;
			this.Label1.Text = "开始时间:";
			// 
			// HourReport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.StatusStrip1);
			this.Controls.Add(this.SplitContainer1);
			this.Name = "HourReport";
			this.Size = new System.Drawing.Size(310, 588);
			((System.ComponentModel.ISupportInitialize)(this.DataGridView1)).EndInit();
			this.StatusStrip1.ResumeLayout(false);
			this.StatusStrip1.PerformLayout();
			this.SplitContainer1.Panel1.ResumeLayout(false);
			this.SplitContainer1.Panel1.PerformLayout();
			this.SplitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).EndInit();
			this.SplitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion

		private System.Windows.Forms.DataGridView DataGridView1;
		internal System.Windows.Forms.StatusStrip StatusStrip1;
		internal System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel1;
		internal System.Windows.Forms.ToolStripProgressBar ProgressBar;
		public System.ComponentModel.BackgroundWorker BackgroundWorker;
		internal System.Windows.Forms.SplitContainer SplitContainer1;
		internal System.Windows.Forms.Button btnExport;
		internal System.Windows.Forms.Button btnReport;
		internal System.Windows.Forms.DateTimePicker endTime;
		internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.DateTimePicker startTime;
		internal System.Windows.Forms.Label Label1;
	}
}

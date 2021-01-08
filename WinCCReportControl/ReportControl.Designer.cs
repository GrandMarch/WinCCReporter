namespace WinCCReportControl
{
    partial class ReportControl
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
			this.components = new System.ComponentModel.Container();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.lbInfo = new System.Windows.Forms.Label();
			this.labelVer = new System.Windows.Forms.Label();
			this.btnExport = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.platformReportList = new System.Windows.Forms.ComboBox();
			this.btnQuery = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.endTime = new System.Windows.Forms.DateTimePicker();
			this.label3 = new System.Windows.Forms.Label();
			this.txtInterVal = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.startTime = new System.Windows.Forms.DateTimePicker();
			this.dataGrid = new unvell.ReoGrid.ReoGridControl();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.lbInfo);
			this.splitContainer1.Panel1.Controls.Add(this.labelVer);
			this.splitContainer1.Panel1.Controls.Add(this.btnExport);
			this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
			this.splitContainer1.Panel1.Controls.Add(this.btnQuery);
			this.splitContainer1.Panel1.Controls.Add(this.label4);
			this.splitContainer1.Panel1.Controls.Add(this.endTime);
			this.splitContainer1.Panel1.Controls.Add(this.label3);
			this.splitContainer1.Panel1.Controls.Add(this.txtInterVal);
			this.splitContainer1.Panel1.Controls.Add(this.label2);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.startTime);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.dataGrid);
			this.splitContainer1.Size = new System.Drawing.Size(1600, 800);
			this.splitContainer1.SplitterDistance = 66;
			this.splitContainer1.TabIndex = 0;
			// 
			// lbInfo
			// 
			this.lbInfo.AutoSize = true;
			this.lbInfo.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lbInfo.Location = new System.Drawing.Point(1427, 26);
			this.lbInfo.Name = "lbInfo";
			this.lbInfo.Size = new System.Drawing.Size(41, 12);
			this.lbInfo.TabIndex = 1;
			this.lbInfo.Text = "label5";
			// 
			// labelVer
			// 
			this.labelVer.AutoSize = true;
			this.labelVer.Location = new System.Drawing.Point(1429, 5);
			this.labelVer.Name = "labelVer";
			this.labelVer.Size = new System.Drawing.Size(41, 12);
			this.labelVer.TabIndex = 14;
			this.labelVer.Text = "label5";
			// 
			// btnExport
			// 
			this.btnExport.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnExport.Location = new System.Drawing.Point(1023, 20);
			this.btnExport.Name = "btnExport";
			this.btnExport.Size = new System.Drawing.Size(115, 32);
			this.btnExport.TabIndex = 13;
			this.btnExport.Text = "导出为Excel...";
			this.toolTip1.SetToolTip(this.btnExport, "导出报表为Excel文件");
			this.btnExport.UseVisualStyleBackColor = true;
			this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.platformReportList);
			this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.groupBox2.Location = new System.Drawing.Point(656, 5);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(218, 50);
			this.groupBox2.TabIndex = 12;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "报表选择";
			// 
			// platformReportList
			// 
			this.platformReportList.FormattingEnabled = true;
			this.platformReportList.Location = new System.Drawing.Point(6, 18);
			this.platformReportList.Name = "platformReportList";
			this.platformReportList.Size = new System.Drawing.Size(206, 28);
			this.platformReportList.TabIndex = 10;
			this.toolTip1.SetToolTip(this.platformReportList, "报表选择");
			this.platformReportList.SelectedIndexChanged += new System.EventHandler(this.platformReportList_SelectedIndexChanged);
			// 
			// btnQuery
			// 
			this.btnQuery.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnQuery.Location = new System.Drawing.Point(893, 21);
			this.btnQuery.Name = "btnQuery";
			this.btnQuery.Size = new System.Drawing.Size(92, 32);
			this.btnQuery.TabIndex = 10;
			this.btnQuery.Text = "查询";
			this.toolTip1.SetToolTip(this.btnQuery, "查询报表");
			this.btnQuery.UseVisualStyleBackColor = true;
			this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label4.Location = new System.Drawing.Point(231, 31);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(68, 20);
			this.label4.TabIndex = 6;
			this.label4.Text = "结束时间:";
			// 
			// endTime
			// 
			this.endTime.CustomFormat = "yyyy-MM-dd HH:00:00";
			this.endTime.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.endTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.endTime.Location = new System.Drawing.Point(299, 28);
			this.endTime.Name = "endTime";
			this.endTime.Size = new System.Drawing.Size(154, 26);
			this.endTime.TabIndex = 5;
			this.toolTip1.SetToolTip(this.endTime, "报表结束时间(包含)");
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label3.Location = new System.Drawing.Point(601, 33);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(37, 20);
			this.label3.TabIndex = 4;
			this.label3.Text = "小时";
			// 
			// txtInterVal
			// 
			this.txtInterVal.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.txtInterVal.Location = new System.Drawing.Point(533, 29);
			this.txtInterVal.Name = "txtInterVal";
			this.txtInterVal.Size = new System.Drawing.Size(62, 26);
			this.txtInterVal.TabIndex = 3;
			this.txtInterVal.Text = "2";
			this.toolTip1.SetToolTip(this.txtInterVal, "报表时间间隔");
			this.txtInterVal.TextChanged += new System.EventHandler(this.txtInterVal_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label2.Location = new System.Drawing.Point(467, 31);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 20);
			this.label2.TabIndex = 2;
			this.label2.Text = "时间间隔:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label1.Location = new System.Drawing.Point(7, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "开始时间:";
			// 
			// startTime
			// 
			this.startTime.CustomFormat = "yyyy-MM-dd HH:00:00";
			this.startTime.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.startTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.startTime.Location = new System.Drawing.Point(75, 28);
			this.startTime.Name = "startTime";
			this.startTime.Size = new System.Drawing.Size(154, 26);
			this.startTime.TabIndex = 0;
			this.toolTip1.SetToolTip(this.startTime, "报表开始时间(不包含)");
			// 
			// dataGrid
			// 
			this.dataGrid.BackColor = System.Drawing.Color.White;
			this.dataGrid.ColumnHeaderContextMenuStrip = null;
			this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGrid.LeadHeaderContextMenuStrip = null;
			this.dataGrid.Location = new System.Drawing.Point(0, 0);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.RowHeaderContextMenuStrip = null;
			this.dataGrid.Script = null;
			this.dataGrid.SheetTabContextMenuStrip = null;
			this.dataGrid.SheetTabNewButtonVisible = true;
			this.dataGrid.SheetTabVisible = true;
			this.dataGrid.SheetTabWidth = 60;
			this.dataGrid.ShowScrollEndSpacing = true;
			this.dataGrid.Size = new System.Drawing.Size(1600, 730);
			this.dataGrid.TabIndex = 0;
			this.dataGrid.Text = "dataGrid";
			// 
			// ReportControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "ReportControl";
			this.Size = new System.Drawing.Size(1600, 800);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

        }
		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.DateTimePicker startTime;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtInterVal;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private unvell.ReoGrid.ReoGridControl dataGrid;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.DateTimePicker endTime;
		private System.Windows.Forms.Button btnQuery;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox platformReportList;
		private System.Windows.Forms.Button btnExport;
		private System.Windows.Forms.Label labelVer;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label lbInfo;
	}
}

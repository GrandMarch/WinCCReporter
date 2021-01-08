using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DotNetFormula;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace WinCCReportControl
{
	public partial class ReportControl: UserControl
    {
		private string baseDirectory="";

		private string reportFileName = "";
		//private object DataSheet;
        public ReportControl()
        {
            InitializeComponent();
			this.Load += ReportControl_Load;
        }



		private void txtInterVal_TextChanged(object sender, EventArgs e)
		{
			string reg = "^[1-9]([0-9]{0,1})$";
			System.Text.RegularExpressions.Regex regx = new System.Text.RegularExpressions.Regex(reg);
			if (txtInterVal.Text != "")
			{
				if (!regx.IsMatch(txtInterVal.Text))
				{
					MessageBox.Show("时间间隔必须满足如下格式:\n必须为数字\n必须小于两位数字\n首数字不能为0","Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
				}
			}
		}

		/// <summary>
		/// 初始化事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ReportControl_Load(object sender, System.EventArgs e)
		{
			lbInfo.Visible = false;
			//获取dll自己所在路径
			string who_am_i = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString();
			baseDirectory = who_am_i.Substring(0, who_am_i.LastIndexOf("\\"));
			//不显示新建sheet按钮
			this.dataGrid.SheetTabNewButtonVisible = false;
			//sheet tab 宽度
			this.dataGrid.SheetTabWidth = 200;
			//默认时间间隔
			this.txtInterVal.Text = "2";
			//默认结束时间
			endTime.Value = DateTime.Now.Date.AddHours(8);
			//默认开始时间
			startTime.Value = endTime.Value.AddHours(-24);
			//显示平台报表列表
			groupBox2.Visible = true;
			//增加信息到报表下拉菜单
			string[] files = Directory.GetFiles(baseDirectory + "\\db", "*.xml", SearchOption.TopDirectoryOnly);
			//添加到下拉菜单
			foreach (string file in files)
			{
				string a = file.Substring(file.LastIndexOf("\\") + 1, file.Length - file.LastIndexOf("\\") - 5);
				if (!platformReportList.Items.Contains(a)) platformReportList.Items.Add(a);
			}
			//版本号
			Assembly assembly= System.Reflection.Assembly.GetExecutingAssembly();
			Version version = assembly.GetName().Version;
			labelVer.Text = "v" + version.ToString();
		}
		
		private void btnQuery_Click(object sender, EventArgs e)
		{
			btnQuery.Enabled = false;
			if (this.platformReportList.SelectedIndex < 0)
			{
				MessageBox.Show("没有选择报表!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				btnQuery.Enabled = true;
				return;
			}
			if (this.startTime.Value >= this.endTime.Value)
			{
				MessageBox.Show("开始时间不能大于或等于结束时间!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				btnQuery.Enabled = true;
				return;
			}
			//检查行数
			double rowCount = (endTime.Value - startTime.Value).TotalHours / double.Parse(txtInterVal.Text)+1;
			//限制表体行数200
			if (rowCount >= 200)
			{
				MessageBox.Show("起止时间除以时间间隔不能大于200!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				btnQuery.Enabled = true;
				return;
			}
			lbInfo.Visible = false;
			Stopwatch sw = new Stopwatch();
			sw.Start();
			//
			//var sheet = dataGrid.CurrentWorksheet;
			//sheet.Name = "data";
			//dataGrid.CreateWorksheet();
			//dataGrid.
			dataGrid.Reset();
			dataGrid.CurrentWorksheet.Rows = 1000;//设置1000行
			//创建报表类
			Report mReport = new Report(reportFileName,startTime.Value.ToString("yyyy-MM-dd HH:00:00"), endTime.Value.ToString("yyyy-MM-dd HH:00:00"),txtInterVal.Text);
			//初始化报表
			mReport.InitReport();
			//画出报表并填充文本
			//unvell.ReoGrid.ReoGridControl nGrid = new unvell.ReoGrid.ReoGridControl();
			string[] DenseTags = mReport.DenseTags;
			string[] SloppyTags = mReport.SloppyTags;
			string st = startTime.Value.ToString("yyyy-MM-dd HH:00:00");
			string et = endTime.Value.ToString("yyyy-MM-dd HH:00:00");
			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += (o, ea) =>
			{
				SelfFunctionUtil.HistoryDataTable = HisData.getData(st, et, 60, DenseTags);

				if (SloppyTags.Length > 0)
				//SelfFunctionUtil.SloppyHistoryDataTable = HisData.getData(DateTime.Now.AddYears(-1).Year.ToString()+"-12-31 08:00:00", DateTime.Parse(endTime).Date.ToString("yyyy-MM-dd 08:00:00"), "1440", SloppyTags);
				{
					List<string> times = new List<string> { };
					times.Add(DateTime.Parse(et).ToString("yyyy-01-01 08:00:00"));
					times.Add(DateTime.Parse(et).ToString("yyyy-MM-01 08:00:00"));
					times.Add(DateTime.Parse(et).Date.ToString("yyyy-MM-dd 08:00:00"));
					SelfFunctionUtil.SloppyHistoryDataTable = HisData.GetDataByTime(times, SloppyTags);
				}
			};
			worker.RunWorkerCompleted += (o, ea) => 
			{
				mReport.PaintReport(ref dataGrid);
				//dataGrid.CurrentWorksheet = nGrid.CurrentWorksheet;
				//grid.Dispose();
				mReport.Dispose();
				lbInfo.Visible = true;
				lbInfo.Text = sw.Elapsed.TotalSeconds.ToString() + "s";
				btnQuery.Enabled = true;
			};
			//fuck
			worker.RunWorkerAsync();
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			SaveFileDialog Sfd = new SaveFileDialog();
			Sfd = new SaveFileDialog();       //保存文件对话框
			Sfd.DefaultExt = "xlsx";               //设置默认文件扩展名
			Sfd.Filter = "Excel 2007 文件|*.xlsx";        //文件类型
			if (Sfd.ShowDialog() == DialogResult.OK)
			{
				dataGrid.Save(Sfd.FileName, unvell.ReoGrid.IO.FileFormat.Excel2007);
				//dataGrid.Save(Sfd.FileName);
				MessageBox.Show("保存完毕！","OK",MessageBoxButtons.OK,MessageBoxIcon.Information);
			}
		}

		private void platformReportList_SelectedIndexChanged(object sender, EventArgs e)
		{
			reportFileName = baseDirectory + "\\db\\" + platformReportList.SelectedItem.ToString() + ".xml";
		}
		#region backgroudworker
		private void worker_DoWork(string[] DenseTags,string[] SloppyTags)
		{

		}
		private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			throw new NotImplementedException();
		}
		#endregion


	}
}

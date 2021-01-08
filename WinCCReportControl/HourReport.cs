using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Data.SqlClient;

namespace WinCCReportControl
{
    public partial class HourReport: UserControl
    {
		//const string SERVER_IP = "10.132.94.5";
		//const string DB = "report";
		//const string DBUSER = "dbuser";
		//const string dbPassword = "scada204";
		string tagName = "";
		string start_Time;
		string end_Time;
		DataTable dt2display = new DataTable();
		public  int  ReportType { get; set; }
		public string ReportTag { get; set; }
		public HourReport()
        {
            InitializeComponent();
			this.Load += TRQHourReport_Load;
			this.BackgroundWorker.DoWork += BackgroundWorker_DoWork;
			this.BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
			this.BackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
        }

		private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			ProgressBar.Value = e.ProgressPercentage;
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			DataGridView1.DataSource = dt2display;//显示
			this.ProgressBar.Value = 0;
			DataGridView1.Columns[0].Width = 145;
			DataGridView1.Columns[1].Width = 120;
			//定位到最后一行
			DataGridView1.FirstDisplayedScrollingRowIndex = DataGridView1.RowCount - 1;
			this.DataGridView1.DefaultCellStyle.Font = new Font("微软雅黑", 10);
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			//string conStr = "server=" + SERVER_IP + "\\WINCC;database=" + DB + ";User Id=" + DBUSER + ";Password=" + dbPassword;//数据库连接字符串			string ssql1 = "SELECT  distinct *  FROM [report].[dbo].[TagLog] where [DateTime] between cast('" + startTime + "' as datetime) and cast('" + endTime + "' as datetime)";//开始时间和结束时间
			//string ssql1 = "SELECT  distinct *  FROM [report].[dbo].[TagLog] where [DateTime] between cast('" + start_Time + "' as datetime) and cast('" + end_Time + "' as datetime)";//开始时间和结束时间
			//string ssql2 = " and datediff(MINUTE, cast('" + start_Time + "' as datetime),[DateTime])%60=0";//时间间隔
			//string ssql3 = " and  [TagName]='" + tagName + "'" + "order by [DateTime] asc ";
			DataTable dt = new DataTable();
			try
			{
				/*using (SqlConnection conn = new SqlConnection(conStr))
				{
					conn.Open();//打开数据库连接
					this.BackgroundWorker.ReportProgress(15);
					string sSQL = ssql1 + ssql2 + ssql3;
					using (SqlCommand cmd = new SqlCommand(ssql1 + ssql2 + ssql3, conn))
					{
						SqlDataAdapter sda = new SqlDataAdapter(cmd);
						sda.Fill(dt);//数据填充到表格
						this.BackgroundWorker.ReportProgress(25);
						sda.Dispose();
					}
				}*/
				MySqlParallelQuery.DateTimeAndTagNamesQuery q = new MySqlParallelQuery.DateTimeAndTagNamesQuery(DateTime.Parse(start_Time), DateTime.Parse(end_Time), 60, new string[] { tagName });
				q.ParalleExcute();
				dt = q.QueryResult;
				//处理数据
				dt2display = new DataTable();//重新new一个表格
				dt2display.Columns.Add("日期时间");
				switch (ReportType)
				{
					case 0://天然气
						dt2display.Columns.Add("小时产量(m³)");
						break;
					case 1://气田水
						dt2display.Columns.Add("小时产量(m³)");
						break;
					case 2://出站
						dt2display.Columns.Add("小时产量(1E4m³)");
						break;
				}
				int i = 0;
				float sum = 0.0f;
				for (i = 0; i < dt.Rows.Count; i++)
				{
					dt2display.Rows.Add();//添加行
					dt2display.Rows[i][0] = DateTime.Parse(dt.Rows[i][0].ToString()).ToString("yyy-MM-dd HH:mm:ss");
					switch (ReportType)
					{
						case 0:
							dt2display.Rows[i][1] = float.Parse(dt.Rows[i][2].ToString()).ToString("0.0");
							break;
						case 1:
							dt2display.Rows[i][1] = (float.Parse(dt.Rows[i][2].ToString())/1000).ToString("0.00");
							break;
						case 2:
							dt2display.Rows[i][1] = float.Parse(dt.Rows[i][2].ToString()).ToString("0.0000");
							break;
					}
					sum += float.Parse(dt2display.Rows[i][1].ToString());
					//this.BackgroundWorker.ReportProgress(25 + i * 70 / dt.Rows.Count);
				}
				//MessageBox.Show(dt2display.Rows.Count.ToString());
				dt2display.Rows.Add();//添加行
									  //MessageBox.Show(dt2display.Rows.Count.ToString());
				dt2display.Rows[i][0] = "总计：";
				
				switch (ReportType)
				{
					case 0:
						dt2display.Rows[i][1] = sum.ToString("0");
						break;
					case 1:
						dt2display.Rows[i][1] = sum.ToString("0.00");
						break;
					case 2:
						dt2display.Rows[i][1] = sum.ToString("0.0000");
						break;
				}
				//MessageBox.Show(dt2display.Rows[i][0].ToString());
				//MessageBox.Show(dt2display.Rows[i][1].ToString());
				//this.BackgroundWorker.ReportProgress(100);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "查询错误！", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			this.BackgroundWorker.ReportProgress(100);
		}

		private void TRQHourReport_Load(object sender, EventArgs e)
		{
			if (DateTime.Now.Hour < 9)
			{
				startTime.Text = DateTime.Now.AddDays(-1).ToString("yyy-MM-dd 08:00:00");
				endTime.Text = DateTime.Now.ToString("yyy-MM-dd 08:00:00");
			}
			else
			{
				startTime.Text = DateTime.Now.ToString("yyy-MM-dd 08:00:00");
				endTime.Text = DateTime.Now.AddDays(1).ToString("yyy-MM-dd 08:00:00");
			}
		}
		private void btnReport_Click(object sender, EventArgs e)
		{
			if (ReportTag == null)
			{
				MessageBox.Show("点名不能为空!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			tagName = ReportTag.ToString() ;
			start_Time = DateTime.Parse(startTime.Text).AddHours(1).ToString();//查询的时候是包含八点这个点的。加一个小时从九点开始
			end_Time = endTime.Text;
			BackgroundWorker.RunWorkerAsync();
		}

		/// <summary>
		/// 导出datagridview数据到excel文件
		/// </summary>
		/// <param name="dataGridView">dataGridView控件</param>
		/// <returns></returns>
		public int export2Excel(DataGridView dataGridView)
		{
			XSSFWorkbook workbook = new XSSFWorkbook();   //工作簿
			XSSFSheet sheet = new XSSFSheet();      //工作表
			SaveFileDialog saveDialog;//保存文件的对话框
			FileStream fs;//文件流
			string filename;//保存时的文件名称
			sheet = (XSSFSheet)workbook.CreateSheet();            //在工作簿中创建表
			try
			{
				sheet.CreateRow(0);                  //创建第一行
				for (int i = 0; i < dataGridView.Columns.Count; i++)//创建表头
				{
					sheet.GetRow(0).CreateCell(i).SetCellValue(dataGridView.Columns[i].HeaderText);
				}
				//创建单元格样式
				ICellStyle cellstyle;
				cellstyle = workbook.CreateCellStyle();
				IDataFormat Format = workbook.CreateDataFormat();
				cellstyle.DataFormat = Format.GetFormat("0.0000");
				//添加其他行和列
				for (int i = 0; i < dataGridView.Rows.Count; i++)
				{
					sheet.CreateRow(i + 1);//每遍历一行，则在sheet中创建一行
					for (int j = 0; j < dataGridView.Columns.Count; j++)
					{
						if (j == 0)
						{
							sheet.GetRow(i + 1).CreateCell(j).SetCellValue(dataGridView.Rows[i].Cells[j].Value.ToString());
						}
						else
						{
							sheet.GetRow(i + 1).CreateCell(j).SetCellValue(double.Parse(dataGridView.Rows[i].Cells[j].Value.ToString()));
						}
						sheet.GetRow(i).GetCell(j).CellStyle = cellstyle;
					}
				}
				saveDialog = new SaveFileDialog();       //保存文件对话框
				saveDialog.DefaultExt = "xlsx";               //设置默认文件扩展名
				saveDialog.Filter = "Excel 2007 文件|*.xlsx";        //文件类型
				if (saveDialog.ShowDialog() == DialogResult.OK)
				{
					filename = saveDialog.FileName;
					fs = new FileStream(filename, FileMode.Create);
					workbook.Write(fs);
					fs.Close();
					MessageBox.Show("导出完成！", "导出结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				cellstyle = null;
				Format = null;
				workbook = null;
				return 0;
			}
			catch
			{
				return -1;
			}
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			export2Excel(DataGridView1);
		}
	}
}

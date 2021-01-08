using DotNetFormula;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.ReoGrid;
using unvell.ReoGrid.DataFormat;

namespace WinCCReportControl
{
	class Report:IDisposable
	{
		/// <summary>
		/// 报表的头
		/// </summary>
		public Range[] Header;

		/// <summary>
		/// 报表体
		/// </summary>
		public Range[] Body;

		/// <summary>
		/// 报表foot
		/// </summary>
		public Range[] Foot;

		/// <summary>
		/// 报表密集查询变量
		/// </summary>
		public string[] DenseTags;

		/// <summary>
		/// 报表稀松查询变量
		/// </summary>
		public string[] SloppyTags;

		/// <summary>
		/// 报表类型
		/// </summary>
		public REPORTTYPE ReportType;

		/// <summary>
		/// 报表定义文件名
		/// </summary>
		private string reportFilePath;

		/// <summary>
		/// 报表参数
		/// </summary>
		private string startTime, endTime, interVal;

		/// <summary>
		/// 构造函数
		/// </summary>
		public Report( string fileName,string starttime,string endtime,string step)
		{
			reportFilePath = fileName;
			startTime = starttime;
			endTime = endtime;
			interVal = step;
		}

		/// <summary>
		/// 初始化报表
		/// </summary>
		public void  InitReport()
		{
			ReportAnalasis RA = new ReportAnalasis(reportFilePath, startTime,endTime,interVal);
			//读取报表类型
			ReportType = RA.ReportType;
			//读取报表变量
			 RA.GetReportTags(out DenseTags,out SloppyTags);
			//读取表头
			Header = RA.GetReportHead();
			//读取表体
			Body = RA.GetReportBody();
			//读取表尾
			Foot = RA.GetReportFoot();
		}

		/// <summary>
		/// 窗体中画出表格
		/// </summary>
		/// <param name="grid">窗体表格</param>
		public void PaintReport(ref ReoGridControl grid)
		{

			//准备演算类
			RPN rpn = new RPN();
			//准备演算数据
			//SelfFunctionUtil.HistoryDataTable = HisData.getData(startTime, endTime, "60", DenseTags);

			//if (SloppyTags.Length > 0)
			//
			//	List<string> times = new List<string> { };
			//	times.Add(DateTime.Now.AddYears(-1).Year.ToString() + "-01-01 08:00:00");
			//	times.Add(DateTime.Parse(endTime).Date.ToString("yyyy-MM-dd 08:00:00"));
			//	SelfFunctionUtil.SloppyHistoryDataTable = HisData.GetDataByTime(times, SloppyTags);
			//} 
			//画表头
			foreach (Range range in Header)
			{
				PaintRange(range, grid, ref rpn);
			}
			//画表体
			foreach (Range range in Body)
			{
				PaintRange(range, grid, ref rpn);
			}
			//画表尾
			foreach (Range range in Foot)
			{
				PaintRange(range, grid, ref rpn);
			}
			//自动列宽
			var sheet = grid.CurrentWorksheet;
			for (int i = 0; i < sheet.ColumnCount; i++)
			{
				sheet.AutoFitColumnWidth(i);
			}
		}

		/// <summary>
		/// 画range
		/// </summary>
		/// <param name="range"></param>
		private void PaintRange(Range range , ReoGridControl grid, ref RPN rpn)
		{
			var sheet = grid.CurrentWorksheet;
			//合并单元格
			string _posString = range.Postion.TopLeft + ":" + range.Postion.BottomRight;
			var _Gridrange = sheet.Ranges[_posString];
			_Gridrange.Merge();
			//设置range边框
			sheet.SetRangeBorders(_Gridrange, BorderPositions.Outside, new RangeBorderStyle
																																	{
																																		Color = unvell.ReoGrid.Graphics.SolidColor.Black,
																																		Style = BorderLineStyle.Solid,
																																	});
			//填充数据
			switch (range.RangeType)
			{
				case RANGETYPE.Text: _Gridrange.Data = range.FormulaOrText;
					//_Gridrange.Style.HorizontalAlign = ReoGridHorAlign.Center;
					//_Gridrange.Style.VerticalAlign = ReoGridVerAlign.Middle;
					break;
				case RANGETYPE.Calc:
					object value = rpn.Evaluate(range.FormulaOrText);
					_Gridrange.Data = value;
					sheet.SetRangeDataFormat(_Gridrange, CellDataFormatFlag.Number, range.NumberFormatArgs);
					break;
				default:
					_Gridrange.Data = range.FormulaOrText;
					break;
			}
			_Gridrange.Style.HorizontalAlign = range.HAlign;
			_Gridrange.Style.VerticalAlign = range.VAlign;
			_Gridrange.Style.FontName = range.Font;
			_Gridrange.Style.FontSize = range.FontSize;
		}

		#region IDisposable Support
		private bool disposedValue = false; // 要检测冗余调用

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					Header = null;
					Body = null;
					Foot = null;
				}

				// TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
				// TODO: 将大型字段设置为 null。

				disposedValue = true;
			}
		}

		// TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
		// ~Report() {
		//   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
		//   Dispose(false);
		// }

		// 添加此代码以正确实现可处置模式。
		public void Dispose()
		{
			// 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
			Dispose(true);
			// TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
			// GC.SuppressFinalize(this);
		}
		#endregion

	}
}

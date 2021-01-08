using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WinCCReportControl
{
	class HisData
	{
		/// <summary>
		/// 读取历史数据
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <param name="step"></param>
		/// <param name="tags"></param>
		/// <returns></returns>
		public static DataTable getData(string startTime, 
														string endTime, 
														uint step, 
														string[] tags)
		{
			//数据查询
			DataTable dt = new DataTable();
			MySqlParallelQuery.DateTimeAndTagNamesQuery q = new MySqlParallelQuery.DateTimeAndTagNamesQuery(DateTime.Parse(startTime), DateTime.Parse(endTime), step, tags);
			q.ParalleExcute();
			dt = q.QueryResult.Copy();
			q.Dispose();
			return dt;//返回表格
		}
		public static DataTable GetDataByTime(List<string>times,
												string[] tags)
		{
			List<DateTime> _times = new List<DateTime> { };
			foreach (string time in times)
			{
				_times.Add(DateTime.Parse(time));
			}
			//数据查询
			DataTable dt = new DataTable();
			MySqlParallelQuery.TimePointQuery q = new MySqlParallelQuery.TimePointQuery(_times.ToArray(), tags);
			q.ParalleExcute();
			dt = q.QueryResult.Copy();
			q.Dispose();
			return dt;//返回表格
		}
	}
}

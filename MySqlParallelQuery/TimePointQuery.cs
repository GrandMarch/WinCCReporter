using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlParallelQuery
{
	public class TimePointQuery:IDisposable
	{
        //private readonly string connectString = "server=10.132.94.5;user id=outuser;password=cq,icq2;database=scada_archive";
        private readonly string connectString = "server=11.13.130.4;user id=outuser;password=cq,icq2;database=scada_archive";
        //private readonly string connectString = "server=192.168.0.25;user id=outuser;password=cq,icq2;database=scada_archive";
        private List<BaseQuery> Querys = new List<BaseQuery> { };
		public string[] QueryTags { get; private set; }
		public DateTime[] QueryTimes { get; private set; }
		private List<TimePointMonth> _months = new List<TimePointMonth> { };
		private List<string> _tables = new List<string> { };
		public int QueryResultRowCount { get; private set; }
		public DataTable QueryResult { get; private set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="times"></param>
		/// <param name="tags"></param>
		public TimePointQuery(DateTime[] times,string[] tags )
		{
			QueryTags = tags;
			QueryTimes = times;
			SubQuery();
		}

		/// <summary>
		/// 分割查询
		/// </summary>
		private void SubQuery()
		{
			//1.组织时间点和月份以及表名对应关系
			List<string> mMonth = new List<string> { };
			foreach (DateTime dt in QueryTimes)
			{
				mMonth.Add(dt.ToString("yyyy-MM-01 08:00:00"));
			}
			mMonth = mMonth.Distinct().ToList();
			foreach (string month in mMonth)
			{
				TimePointMonth tm = new TimePointMonth();
				tm.TableName ="taglog"+ DateTime.Parse(month).ToString("yyyyMM");
				foreach (DateTime dt in QueryTimes)
				{
					if ((DateTime.Parse(month).Year == dt.Year) && (DateTime.Parse(month).Month == dt.Month))
					{
						tm.TimePoint.Add(dt);
					}
				}
				_months.Add(tm);
			}

			//2.组织tags
			string inStatement = "";
			foreach (string tag in QueryTags)
			{
				inStatement = inStatement + "'" + tag + "',";
			}
			inStatement = inStatement.Substring(0, inStatement.Length - 1);
			//3.组织select语句
			List<string> selects = new List<string> { };
			for (int i = 0; i < _months.Count; i++)
			{
				string times = " WHERE time in (  ";
				foreach (DateTime dt in _months[i].TimePoint)
				{
					times = times+"'"+ dt.ToString("yyyy-MM-dd HH:mm:ss") + "',";
				}
				times = times.Substring(0, times.Length -1)+")";
				string select = " SELECT   `Time` ,`TagName`,`Value` From " + _months[i].TableName +	times
				+" and tagname in (" + inStatement + "); ";
				selects.Add(select);
			}
			//4.创建query
			foreach (string select in selects)
			{
				BaseQuery bq = new BaseQuery(connectString, select);
				Querys.Add(bq);
			}
		}

		/// <summary>
		/// 合并结果
		/// </summary>
		private void Combine()
		{
			QueryResult = Querys[0].dataTable.Clone();//复制表结构
			object[] obj = new object[QueryResult.Columns.Count];
			foreach (BaseQuery q in Querys)
			{
				QueryResultRowCount += q.dataTable.Rows.Count;
				for (int i = 0; i < q.dataTable.Rows.Count; i++)
				{
					q.dataTable.Rows[i].ItemArray.CopyTo(obj, 0);
					QueryResult.Rows.Add(obj);
				}
				q.Dispose();//释放资源
			}
			DataView dv = QueryResult.DefaultView;
			dv.Sort = "time asc";
			QueryResult = dv.ToTable();
		}

		/// <summary>
		/// 并行执行查询
		/// </summary>
		public void ParalleExcute()
		{
			//并行查询
			Parallel.ForEach(Querys, item => { item.Excute(); });
			//合并查询结果
			Combine();
		}

		#region IDisposable Support
		public bool isDispoed = false; // 要检测冗余调用

		protected virtual void Dispose(bool disposing)
		{
			if (!isDispoed)
			{
				if (disposing)
				{
					//释放掉子查询
					foreach (BaseQuery bq in Querys)
					{
						if (!bq.isDisposed) bq.Dispose();
					}
					//释放掉内存表
					QueryResult = null;
				}
				isDispoed = true;
			}
		}
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MySqlParallelQuery
{
	public class DateTimeAndTagNamesQuery:IDisposable
	{

		private readonly string connectString= "server=11.13.130.4;user id=outuser;password=cq,icq2;database=scada_archive;UseCompression=True;";//启用压缩协议
		private List<BaseQuery> Querys=new List<BaseQuery> { };
		public DateTime QueryStartTime { get; private set; }
		public DateTime QueryEndTime { get; private set; }
		public uint QueryStep { get; private set; }
		public string[] QueryTags { get; private set; }
		private List<string> _times = new List<string> { };
		private List<string> _tables = new List<string> { };
		public int QueryResultRowCount { get; private set; }
		public DataTable QueryResult { get; private set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <param name="step"></param>
		/// <param name="tags"></param>
		public DateTimeAndTagNamesQuery(DateTime  startTime,DateTime  endTime,uint step,string[] tags)
		{
			QueryStartTime = startTime;
			QueryEndTime = endTime;
			QueryStep = step;
			QueryTags = tags;
			//分割查询
			SubQuery();
		}

		/// <summary>
		/// 并行执行查询
		/// </summary>
		public void ParalleExcute()
		{
			//并行查询
			Parallel.ForEach(Querys, item => { item.Excute(); }	);
			//合并查询结果
			Combine();
		}
		/// <summary>
		/// 并行执行查询
		/// </summary>
		public void ParalleExcuteB()
		{
			//清空
			System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\data\\");
			foreach (FileInfo file in di.GetFiles())
			{
				file.Delete();
			}
			//并行查询
			Parallel.ForEach(Querys, item => { item.ExcuteB(); });
			//合并查询结果
			//CombineB();
		}
		/// <summary>
		/// 拆分大查询
		/// </summary>
		private void  SubQuery()
		{
			//
			DateTime midtime,UTCStartTime,UTCEndTime;
			midtime = QueryStartTime.ToUniversalTime();
			UTCStartTime = QueryStartTime.ToUniversalTime();
			UTCEndTime = QueryEndTime.ToUniversalTime();
			//1.得到所有跨越的月
			_times.Add(QueryStartTime.ToString("yyyy-MM-dd HH:mm:ss"));
			_tables.Add("taglog" + midtime.ToString("yyyyMM"));
			do
			{
				midtime = DateTime.Parse(midtime.AddMonths(1).ToString("yyyy-MM-01 00:00:00"));
				if (midtime <= UTCEndTime)
				{
					_times.Add(midtime.ToLocalTime().ToString("yyyy-MM-dd 07:59:00"));
					_tables.Add("taglog" + midtime.ToString("yyyyMM"));
				}

			}
			while (midtime <= UTCEndTime);
			_times.Add(QueryEndTime.ToString("yyyy-MM-dd HH:mm:ss"));
			//2.组织tags
			string inStatement = "";
			foreach (string tag in QueryTags)
			{
				inStatement = inStatement + "'" + tag + "',";
			}
			inStatement = inStatement.Substring(0, inStatement.Length - 1);
			//3.组织select语句
			List<string> selects = new List<string> { };
			for (int i = 0; i < _times.Count-1; i++)
			{
				if (i == 0)
				{
					string select = " SELECT   `Time` ,`TagName`,`Value` From " + _tables[i] + " FORCE INDEX (name_time) " +//强制使用索引
					" WHERE  time between '" + _times[i] + "'  and '" + _times[i + 1]
					+ "' and timestampdiff(minute,'" + _times[0] + "',time) %" + QueryStep + "=0 and tagname in (" + inStatement + "); ";
					selects.Add(select);
				}
				else
				{
					string select = " SELECT   `Time` ,`TagName`,`Value` From " + _tables[i] + " FORCE INDEX (name_time) "+//强制使用索引
					" WHERE  time between '" + DateTime.Parse(_times[i]).AddMinutes(1).ToString("yyy-MM-dd HH:mm:ss") + "'  and '" + _times[i + 1]
					+ "' and timestampdiff(minute,'" + _times[0] + "',time) %" + QueryStep + "=0 and tagname in (" + inStatement + "); ";
					selects.Add(select);
				}
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
			Debug.WriteLine("开始合并表格数据");
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
			Debug.WriteLine("合并表格数据完成");
		}
		/// <summary>
		/// 合并结果
		/// </summary>
		private void CombineB()
		{
			Debug.WriteLine("开始合并表格数据");
			System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\data\\");
			FileStream fsw = new FileStream(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\data\\Result", FileMode.CreateNew);
			StreamWriter sw = new StreamWriter(fsw);
			foreach (FileInfo file in di.GetFiles())
			{
				if (file.Name == "Result") continue;
				FileStream fsr = new FileStream(file.FullName, FileMode.Open);
				StreamReader sr = new StreamReader(fsr, Encoding.Default);
				string line = sr.ReadLine();			
				while( null != line)
				{
					sw.WriteLine(line);
					line = sr.ReadLine();
					sw.Flush();
				}
				sr.Close();
				fsr.Close();
			}
			sw.Close();
			fsw.Close();
			Debug.WriteLine("数据写入到最终文件完成");
		}

		#region IDisposable Support
		public  bool isDisposed = false; // 要检测冗余调用

		protected virtual void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				if (disposing)
				{
					foreach (BaseQuery bq in Querys)
					{
						if (!bq.isDisposed) bq.Dispose();
					}
					if(QueryResult!=null)
					QueryResult.Dispose();
				}
				isDisposed = true;
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

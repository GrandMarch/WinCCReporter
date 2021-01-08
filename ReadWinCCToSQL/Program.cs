using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ReadWinCCToSQL
{
	/*
	 * 在很小的几率下读取回的数据为null,应该为此做异常处理(通常发生在第一次读取时,可能是HMITags没有准备好).
	 * 在控制台应用和winform中HMITags读取速度相差巨大,不明白为什么.可能控制台应用与com接口兼容更好.
	 * 如何防止不小心从任务栏关闭程序?
	 */
	struct TRQ
	{
		public List<string> TRQTAGS;
		public List<string> TRQ_YEAR;
		public List<string> TRQ_MONTH_DAY;
		public List<string> TRQ_WEEK_HOUR;
		public List<string> TRQ_LAST_TIME;
		public List<string> TRQ_THIS_TIME;
		public List<double> TRQ_THIS_VALUE;
		public List<double> TRQ_LAST_VALUE;
		public List<int> TRQ_FAIL_COUNT;

	}

	class Program
	{
		static readonly string SERVERIP = "11.13.130.4";
		static readonly string USERNAME = "dbuser";//拥有本地网络的读写权限
		static readonly string PASSWORD = "cq,icq2";
		static readonly string DATABASE = "scada_archive";
		static CCHMITAGS.HMITags mHMITags = new CCHMITAGS.HMITags();
		static int WatchDogState = 0;//看门狗
		static TRQ _TRQ_ = new TRQ();
		static bool willExit = false;//指示系统将退出
		static Hashtable threadstatus = new Hashtable();
		static bool stop = false;
		static void Main(string[] args)
		{
			#region    初始化
			// 禁用关闭按钮,防止误关闭
			DisableCloseButton(Console.Title);
			threadstatus.Clear();
			string tittle=System.Reflection.Assembly.GetExecutingAssembly().GetName().ToString();
			Console.Title = tittle.Substring(0, tittle.LastIndexOf(','));
			#endregion

			#region 添加轮询点
			FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\tags.tl", FileMode.Open);
			StreamReader sr = new StreamReader(fs, Encoding.Default);
			List<string> TAGS = new List<string> { };
			string line = sr.ReadLine();
			while (line != null)
			{
				TAGS.Add(line);
				line = sr.ReadLine();
			}
			sr.Close();
			fs.Close();
			#endregion

			#region     启动轮询查询线程
			ParameterizedThreadStart s = new ParameterizedThreadStart(SamppleInterVal);
			Thread t = new Thread(s);
			t.IsBackground = true;
			t.Start(TAGS);
			Console.WriteLine("[" + DateTime.Now.ToString() + "]"+"轮询线程已经启动");
			lock (threadstatus.SyncRoot)
			{
				if (!threadstatus.ContainsKey(t.GetHashCode()))
				{
					threadstatus.Add(t.GetHashCode(), true);
				}
			}
			//threadRunning.Add(true);
			#endregion

			#region 增加流量点

			fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\TRQList.tl", FileMode.Open);
			sr = new StreamReader(fs);
			List<string> TRQTAGS = new List<string> { };//天然气流量点
			List<string> TRQ_YEAR = new List<string> { };
			List<string> TRQ_MONTH_DAY = new List<string> { };//天然气年和月点
			List<string> TRQ_WEEK_HOUR = new List<string> { };//天然气星期和小时点
			List<string> TRQ_LAST_TIME = new List<string> { };//天然气上次报表时间
			List<string> TRQ_THIS_TIME = new List<string> { };//天然气本次报表时间
			List<double> TRQ_THIS_VALUE = new List<double> { };//天然气本次报表值
			List<double> TRQ_LAST_VALUE = new List<double> { };//天然气上次报表值
			List<int> TRQ_FAIL_COUNT = new List<int> { };
			line = sr.ReadLine();
			while (line != null)
			{
				TRQTAGS.Add(line+ "_HourReport");
				TRQ_YEAR.Add(line + "_ReportYear");
				TRQ_MONTH_DAY.Add(line + "_ReportMonthDay");
				TRQ_WEEK_HOUR.Add(line + "_ReportWeekHour");
				line = sr.ReadLine();
			}
			sr.Close();
			fs.Close();
			//设置初始值
			try
			{
				fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\log.tl", FileMode.Open);
				sr = new StreamReader(fs);
				string SW_Times = sr.ReadLine();
				string SW_Values = sr.ReadLine();
				TRQ_LAST_TIME.AddRange(SW_Times.Split(';'));
				List<string> strValues = new List<string> { };
				strValues.AddRange(SW_Values.Split(';'));
				foreach (string strValue in strValues)
				{
					float a = float.Parse(strValue);
					TRQ_LAST_VALUE.Add(a);
				}
				for (int i = 0; i < TRQTAGS.Count; i++)
				{
					TRQ_FAIL_COUNT.Add(0);
					TRQ_THIS_TIME.Add("1970-01-01 00:00:00");
					TRQ_THIS_VALUE.Add(0);
				}
				for (int k = TRQ_LAST_TIME.Count; k < TRQTAGS.Count; k++)
				{
					TRQ_LAST_TIME.Add("1970-01-01 00:00:00");
					TRQ_LAST_VALUE.Add(-1);
				}
			}
			catch
			{
				for (int i = 0; i < TRQTAGS.Count; i++)
				{
					TRQ_LAST_TIME.Add("1970-01-01 00:00:00");
					TRQ_LAST_VALUE.Add(-1);
					TRQ_FAIL_COUNT.Add(0);
					TRQ_THIS_TIME.Add("1970-01-01 00:00:00");
					TRQ_THIS_VALUE.Add(0);
				}
			}
			finally
			{
				sr.Close();
				fs.Close();
			}
			#endregion

			#region 启动流量查询线程

			_TRQ_.TRQTAGS = TRQTAGS;
			_TRQ_.TRQ_LAST_TIME = TRQ_LAST_TIME;
			_TRQ_.TRQ_LAST_VALUE = TRQ_LAST_VALUE;
			_TRQ_.TRQ_MONTH_DAY = TRQ_MONTH_DAY;
			_TRQ_.TRQ_THIS_TIME = TRQ_THIS_TIME;
			_TRQ_.TRQ_THIS_VALUE = TRQ_THIS_VALUE;
			_TRQ_.TRQ_WEEK_HOUR = TRQ_WEEK_HOUR;
			_TRQ_.TRQ_YEAR = TRQ_YEAR;
			_TRQ_.TRQ_FAIL_COUNT = TRQ_FAIL_COUNT;
			//启动线程
			ParameterizedThreadStart s1 = new ParameterizedThreadStart(SampleTRQ);
			Thread t1 = new Thread(s1);
			t1.IsBackground = true;
			t1.Start(_TRQ_);
			Console.WriteLine("[" + DateTime.Now.ToString() + "]"+"天然气查询线程已经启动");
			lock(threadstatus.SyncRoot)
			{
				if (!threadstatus.ContainsKey(t1.GetHashCode()))
				{
					threadstatus.Add(t1.GetHashCode(), true);
				}
			}

			#endregion

			#region 启动看门狗进程(看门狗进程为了防止进程出现意外卡死)
			/*Thread.Sleep(10000);
			Thread t2 = new Thread(WatchDog);
			t2.IsBackground = true;
			t2.Start();*/
			#endregion

			#region 等待用户的输入信息
			//输入exit后程序即刻退出
			while (true)
			{
				string cmd = Console.ReadLine();
				if (cmd == "exit")
				{
					ProcessExit();
					break;
				}
				if (cmd == "stop")
				{
					stop = !stop;
				}
				//Thread.Sleep(100);//?
			}
			mHMITags = null;
			Console.WriteLine("[" + DateTime.Now.ToString() + "]"+"程序已经停止.按任意键退出...");
			Console.ReadKey();
			#endregion
		}
		#region 轮询点相关函数
		/// <summary>
		/// 轮询采集
		/// </summary>
		/// <param name="tags">需要采集的点</param>
		static void SamppleInterVal(object tags)
		{
			List<string> _tags = (List<string>)tags;
			Stopwatch st = new Stopwatch();
			bool locker = false;
			while (!willExit)
			{
				if ((DateTime.Now.Second == 0) && (!locker))
				{
					Interlocked.Exchange(ref WatchDogState, 0);//将看门狗状态复位
					st.Start();
					locker = true;
					string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					List<row> rows = new List<row> { };
					foreach (string tag in _tags)//循环采集所有点的数据
					{
						try
						{
							row r = new row();
							r.Time = time;
							r.TagName = tag;
							r.Value = (float)mHMITags[tag].Read();//有时候会出错,返回null
							//if (tag == "JQZ/CZ_LJLL.Value") Console.WriteLine(r.Value);
							rows.Add(r);
						}
						catch
						{
							//出错后不执行任何操作
							Console.WriteLine("[" + DateTime.Now.ToString() + "]"+"[轮询点]读取数据返回null");							
						}
					}
					while (stop) ;
					st.Stop();
					Console.WriteLine("[" + DateTime.Now.ToString() + "]"+ "轮询采集用时{0}ms", st.Elapsed.TotalMilliseconds);
					st.Reset();
					try
					{
						st.Start();
						WriteSQL(rows);//网络阻塞时导致的线程卡死
						st.Stop();
						Console.WriteLine("[" + DateTime.Now.ToString() + "]"+"轮询结果写入数据库用时{0}ms", st.Elapsed.TotalMilliseconds);
						st.Reset();
					}
					catch
					{
						Console.WriteLine("[" + DateTime.Now.ToString() + "]"+"写入数据库出错");
					}
				}
				if (DateTime.Now.Second >= 55)
				{
					locker = false;
				}
				//每次循环结束时将状态设置为true
				//lock (threadstatus)
				//{

				//}
				lock (threadstatus.SyncRoot)
				{
					threadstatus[Thread.CurrentThread.GetHashCode()] = true;
				}
				
				Thread.Sleep(200);
			}
			//程序结束时将状态设置为false
			lock (threadstatus.SyncRoot)
			{
				threadstatus[Thread.CurrentThread.GetHashCode()] = false;
			}
			Console.WriteLine("[" + DateTime.Now.ToString() + "]"+"轮询采集线程收到退出信号并已经退出!");
		}

		#endregion

		#region 天然气相关函数
		/// <summary>
		/// 采集天然气数据
		/// </summary>
		/// <param name="trq"></param>
		static void SampleTRQ(object trq)
		{
			TRQ _TRQ = (TRQ)trq;
			ushort _year, _monthday, _weekhour;
			string datetime = "";
			bool locker = false;
			while (!willExit)
			{
				Stopwatch st = new Stopwatch();
				if ((DateTime.Now.Second % 9 == 0) && (!locker))//定时检查
				{
					Interlocked.Exchange(ref WatchDogState, 0);//将看门狗状态复位
					locker = true;
					st.Start();
					//bug处理
					//由于小时产量不确定在什么时间到达,如果在一日的八点之前到达那么就会导致数据存储的上一个月的表中.
					//解决办法就是在一日的7点半到8点之间不再扫描天然气点,八点以后再扫描
					bool bIgnore = false;
					if ((DateTime.Now.Day == 1) && (DateTime.Now.Hour == 7) && (DateTime.Now.Minute > 30))
					{
						bIgnore = true;
					}
					else
					{
						bIgnore = false;
					}
					#region 数据读取
					if (!bIgnore)
					{
						for (int i = 0; i < _TRQ.TRQTAGS.Count; i++)
						{
							try
							{
								//处理时间
								_year = (ushort)mHMITags[_TRQ.TRQ_YEAR[i]].Read();
								_monthday = (ushort)mHMITags[_TRQ.TRQ_MONTH_DAY[i]].Read();
								_weekhour = (ushort)mHMITags[_TRQ.TRQ_WEEK_HOUR[i]].Read();
								if (BCD2DateTime(_year, _monthday, _weekhour, ref datetime))
								{
									_TRQ.TRQ_THIS_TIME[i] = datetime;
								}
								//else//如果时间处理出错那么将时间设置为1970-01-01 00:00:00
								//{
								//	_TRQ.TRQ_THIS_TIME[i] = "1970-01-01 00:00:00";
								//}
								//处理数值
								_TRQ.TRQ_THIS_VALUE[i] = (double)mHMITags[_TRQ.TRQTAGS[i]].Read();
							}
							catch
							{
								//出错以后并不执行操作,数据还是保持以前的旧的数据
								Console.WriteLine("[" + DateTime.Now.ToString() + "]" + "天然气数据读取返回null!");
							}
						}
					}
					else
					{
						Console.WriteLine("[" + DateTime.Now.ToString() + "]" + "天然气产量每月一日7点半到八点之间不采集.");
					}

					#endregion

					st.Stop();
					Console.WriteLine("[" + DateTime.Now.ToString() + "]" + "天然气读扫描用时{0}ms", st.Elapsed.TotalMilliseconds);
					st.Reset();

					#region 数据处理
					List<row> rows = new List<row> { };
					for (int j = 0; j < _TRQ.TRQTAGS.Count; j++)
					{
						if (_TRQ.TRQ_LAST_TIME[j] != _TRQ.TRQ_THIS_TIME[j])
						{
							if (_TRQ.TRQ_LAST_VALUE[j] != _TRQ.TRQ_THIS_VALUE[j])
							{
								row r = new row();
								r.TagName = _TRQ.TRQTAGS[j];
								r.Time = _TRQ.TRQ_THIS_TIME[j];
								r.Value = _TRQ.TRQ_THIS_VALUE[j];
								rows.Add(r);
								_TRQ.TRQ_LAST_TIME[j] = _TRQ.TRQ_THIS_TIME[j];
								_TRQ.TRQ_LAST_VALUE[j] = _TRQ.TRQ_THIS_VALUE[j];
							}
							else
							{
								_TRQ.TRQ_FAIL_COUNT[j]++;
								if (_TRQ.TRQ_FAIL_COUNT[j] >= 10)//等待20秒
								{
									row r = new row();
									r.TagName = _TRQ.TRQTAGS[j];
									r.Time = _TRQ.TRQ_THIS_TIME[j];
									r.Value = _TRQ.TRQ_THIS_VALUE[j];
									rows.Add(r);
									_TRQ.TRQ_LAST_TIME[j] = _TRQ.TRQ_THIS_TIME[j];
									_TRQ.TRQ_LAST_VALUE[j] = _TRQ.TRQ_THIS_VALUE[j];
								}
							}
						}
					}
					#endregion

					#region 数据写入
					if (rows.Count > 0)
					{
						st.Start();
						WriteSQL(rows);
						st.Stop();
						Console.WriteLine("[" + DateTime.Now.ToString() + "]" + "天然气写入数据库用时{0}ms", st.Elapsed.TotalMilliseconds);
						st.Reset();
					}
					#endregion

				}
				if (DateTime.Now.Second % 9 == 1)
				{
					locker = false;
				}
				//每次循环结束时将状态设置为true
				lock (threadstatus.SyncRoot)
				{
					threadstatus[Thread.CurrentThread.GetHashCode()] = true;
				}

				Thread.Sleep(200);
			}
			//程序退出是将状态设置为false
			lock (threadstatus.SyncRoot)
			{
				threadstatus[Thread.CurrentThread.GetHashCode()] = false;
			}
			
			Console.WriteLine("[" + DateTime.Now.ToString() + "]"+"流量计采集线程收到退出信号并已经退出!");
		}

		/// <summary>
		/// BCD码时间转换为正常时间
		/// </summary>
		/// <param name="year"></param>
		/// <param name="monthDay"></param>
		/// <param name="weekHour"></param>
		/// <param name="sDT"></param>
		/// <returns></returns>
		static bool BCD2DateTime(ushort year, ushort monthDay, ushort weekHour, ref string sDT)
		{
			//sDT = "1970-01-01 00:00:00";//初始值
			string sYear, sMonth, sDay, sHour;
			sYear = ((year & 0xf000) >> 12).ToString() + ((year & 0x0f00) >> 8).ToString() + ((year & 0x00f0) >> 4).ToString() + (year & 0x000f).ToString();
			sMonth = ((monthDay & 0xf000) >> 12).ToString() + ((monthDay & 0x0f00) >> 8).ToString();
			sDay = ((monthDay & 0x00f0) >> 4).ToString() + (monthDay & 0x000f).ToString();
			sHour = ((weekHour & 0x00f0) >> 4).ToString() + (weekHour & 0x000f).ToString();
			sDT = sYear + "-" + sMonth + "-" + sDay + " " + sHour + ":00:00";
			try
			{
				DateTime.Parse(sDT);
				return true;
			}
			catch
			{
				return false;
			}
		}
		#endregion

		/// <summary>
		/// 看门狗程序
		/// </summary>
		static void WatchDog()
		{
			
			int counter = 0;
			while (true)
			{
				bool check = true;
				//检查每一个线程的状态
				lock (threadstatus.SyncRoot)//读取是不是需要锁定?
				{
					foreach (int key in threadstatus.Keys)
					{
						check = check && (bool)threadstatus[key];
					}
				}
				if (!check)
				{
					counter++;
				}
				else
				{
					counter = 0;
				}
				if (counter >= 40)//超过200秒线程没有激活
				{
					///TODO:程序假死或者阻塞需要重启程序
					//启动新进程
					Process p = new Process();
					ProcessStartInfo startInfo = new ProcessStartInfo(AppDomain.CurrentDomain.BaseDirectory+ "ReadWinCCToSQL.exe");
					p.StartInfo = startInfo;
					p.StartInfo.UseShellExecute = false;
					p.Start();
					Console.WriteLine("["+DateTime.Now.ToString()+"]"+"软件自动重启....");
					//结束当前进程
					ProcessExit();
				}
				if(counter>=1)	Console.WriteLine("[" + DateTime.Now.ToString() + "]"+"重启计数器工作中:" + counter);
				//将每一个线程的状态设置false
				lock (threadstatus.SyncRoot)
				{
					int[] keys = new int[threadstatus.Count];
					threadstatus.Keys.CopyTo(keys, 0);
					for (int i = 0; i < keys.Length; i++)
					{
						threadstatus[keys[i]] = false;
					}
				}
				Thread.Sleep(5000);//休眠5秒
			}
		}
		/// <summary>
		/// 禁用关闭按钮
		/// </summary>
		/// <param name="title"></param>
		static void DisableCloseButton(string title)
		{
			//线程睡眠，确保closebtn中能够正常FindWindow，否则有时会Find失败。。
			Thread.Sleep(100);
			IntPtr windowHandle = NativeMethods.FindWindow(null, title);
			IntPtr closeMenu = NativeMethods.GetSystemMenu(windowHandle, IntPtr.Zero);
			uint SC_CLOSE = 0xF060;
			NativeMethods.RemoveMenu(closeMenu, SC_CLOSE, 0x0);
		}

		/// <summary>
		/// 数据转换为SQL语句
		/// </summary>
		/// <param name="rows">行list</param>
		/// <param name="tableName">表名称</param>
		/// <returns></returns>
		private static string rows2sqlinsertstring(List<row> rows, string tableName)
		{
			string sqlstring = "insert into " + tableName + "(Time,TagName,Value) values";
			#if DEBUG
			Console.WriteLine("[" + DateTime.Now.ToString() + "]"+"调试中........!");
			sqlstring = "insert into data(DateTime,TagName,Value) values";
			#endif
			foreach (row _row in rows)
			{
				string _sql = "";
				_sql += "('" + _row.Time + "','" + _row.TagName + "'," + _row.Value + "),";// + ",'" + _row.InsertTime 
				sqlstring += _sql;
			}
			sqlstring = sqlstring.Substring(0, sqlstring.Length - 1);
			sqlstring += ";";
			return sqlstring;
		}

		/// <summary>
		/// 写入数据库函数
		/// </summary>
		/// <param name="rows"></param>
		static void WriteSQL(List<row> rows)
		{
			string conStr = "server=" + SERVERIP + ";user id=" + USERNAME + ";password=" + PASSWORD + ";database=" + DATABASE;
			if (rows.Count > 0)
			{
				using (MySqlConnection conn = new MySqlConnection(conStr))//insert date to database
				{
					try
					{
						//设置各种超时参数?
						//Console.WriteLine(conn.ConnectionTimeout);
						//
						conn.Open();
						string tableName = "";
						if ((DateTime.Now.Day == 1) && (DateTime.Now.Hour < 8))
						{
							tableName = "taglog" + DateTime.Now.AddMonths(-1).ToString("yyyyMM");
						}
						else
						{
							tableName = "taglog" + DateTime.Now.ToString("yyyyMM");
						}
						string sqlstring = rows2sqlinsertstring(rows, tableName);
						using (MySqlCommand command = new MySqlCommand(sqlstring, conn))
						{
							int affectLines = command.ExecuteNonQuery();
						}
					}
					catch
					{
						Console.WriteLine("数据写入数据库出现错误!");
					}
				}
			}
		}

		/// <summary>
		/// 程序结束
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void ProcessExit()
		{
			//发送退出信号通知后台线程
			willExit = true;
			//在此事件中把必要的数据保存下来
			FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\log.tl", FileMode.Create);
			StreamWriter sw = new StreamWriter(fs);
			List<string> times = _TRQ_.TRQ_LAST_TIME;
			List<double> values = _TRQ_.TRQ_LAST_VALUE;
			string sw_time = "";
			foreach (string time in times)
			{
				sw_time = sw_time + time + ";";
			}
			sw_time = sw_time.Substring(0, sw_time.Length - 1);
			string sw_vaule = "";
			foreach (float value in values)
			{
				sw_vaule = sw_vaule + value.ToString() + ";";
			}
			sw_vaule = sw_vaule.Substring(0, sw_vaule.Length - 1);
			sw.WriteLine(sw_time);
			sw.WriteLine(sw_vaule);
			sw.Flush();
			sw.Close();
			fs.Close();
			//检测所有的后台线程是不是已经正常退出
			//bool check = false;
			//do
			//{
			//	check = false;
			//	lock (threadstatus)
			//	{
			//		foreach (int key in threadstatus.Keys)
			//		{
			//			check = check || (bool)threadstatus[key];
			//			//Console.WriteLine(key + "\t" + (bool)threadstatus[key]);
			//		}
			//	}
			//	Console.WriteLine("等待所有的线程退出........");
			//	Thread.Sleep(200);
			//}
			//while (check);
			lock(threadstatus.SyncRoot)
			{
				threadstatus.Clear();
			}
			mHMITags = null;
            Marshal.ReleaseComObject(mHMITags);//减少引用计数
            Console.WriteLine("[" + DateTime.Now.ToString() + "]" + "程序已经退出.");
			//Environment.Exit(0);
		}
	}
}

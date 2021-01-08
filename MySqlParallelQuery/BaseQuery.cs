using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MySqlParallelQuery
{
	internal class BaseQuery :IDisposable
	{
		private  string ConnectionString;
		private string QueryString;
		//外部可读取的表格结果
		public DataTable dataTable  { get; private set; }

		public BaseQuery(string connectionstring,string querystring)
		{
			ConnectionString = connectionstring;
			QueryString = querystring;
			//connection = new MySqlConnection(connectionstring);
			//dataAdampter = new MySqlDataAdapter(querystring, connection);
		}

		public BaseQuery()
		{ }

		/// <summary>
		/// 执行查询
		/// </summary>
		public void Excute()
		{
			using (MySqlConnection Connection = new MySqlConnection(ConnectionString))
			{
				try
				{
					Connection.Open();
					Debug.WriteLine("["+DateTime.Now +"]"+ Connection.GetHashCode()+":连接数据库成功");
					//using (MySqlDataAdapter dataAdampter = new MySqlDataAdapter(QueryString, Connection))
					using (MySqlCommand cmd = new MySqlCommand(QueryString, Connection))
					{
						
						cmd.CommandTimeout = 65536;
						MySqlDataAdapter dataAdampter = new MySqlDataAdapter(cmd);
						dataTable = new DataTable();
						dataAdampter.Fill(dataTable);
						dataAdampter.Dispose();
						/*
						dataTable = new DataTable();
						MySqlDataReader dr = cmd.ExecuteReader();
						dataTable.Load(dr);
						dr.Close();	
						*/
					}
					Connection.Close();
					Debug.WriteLine("[" + DateTime.Now + "]" + Connection.GetHashCode() + ":查询结束\r\n."+ QueryString);
				}
				catch(Exception ex)
				{
					Debug.WriteLine("[" + DateTime.Now + "]" + Connection.GetHashCode()+ "连接数据库失败");
					Debug.WriteLine(ex.Message);
				}
			}
		}

		/// <summary>
		/// 执行查询(B计划)为大数据设计
		/// </summary>
		public void ExcuteB()
		{
			ConnectionString += ";UseCompression=true";//添加压缩选项
			using (MySqlConnection Connection = new MySqlConnection(ConnectionString))
			{
				try
				{
					//Connection.UseCompression = true;//
					Connection.Open();
					Debug.WriteLine("[" + DateTime.Now + "]" + Connection.GetHashCode() + ":连接数据库成功");
					//using (MySqlDataAdapter dataAdampter = new MySqlDataAdapter(QueryString, Connection))
					using (MySqlCommand cmd = new MySqlCommand(QueryString, Connection))
					{
						string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
						FileStream fs = new FileStream(path + "\\data\\" + GetHashCode(), FileMode.CreateNew);
						StreamWriter sw = new StreamWriter(fs);
						MySqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read())
						{
							sw.WriteLine(dr[0] + "," + dr[1] + "," + dr[2]);
						}
						sw.Flush();
						sw.Close();
						fs.Close();
						dr.Close();						
					}
					Connection.Close();
					Debug.WriteLine("[" + DateTime.Now + "]" + Connection.GetHashCode() + ":查询结束\r\n." + QueryString);
				}
				catch (Exception ex)
				{
					Debug.WriteLine("[" + DateTime.Now + "]" + Connection.GetHashCode() + "连接数据库失败");
					Debug.WriteLine(ex.Message);
				}
			}
		}

		#region IDisposable Support
		public  bool isDisposed = false; // 要检测冗余调用

		protected virtual void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				if (disposing)
				{
					//让GC去回收
					if(dataTable!=null)
					dataTable.Dispose();
				}
				isDisposed = true;
			}
		}
		public void Dispose()
		{
			Dispose(true);
		}
		#endregion
	}
}

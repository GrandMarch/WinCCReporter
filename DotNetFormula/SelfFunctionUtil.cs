using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using unvell.ReoGrid;

namespace DotNetFormula
{
  /// <summary>
    /// 存储所有的自定义方法，定义新的方法需要在此编写。这是一个工具类
    /// </summary>
  public  static  class SelfFunctionUtil
    {
        #region 需要外部设置数据的属性

        /// <summary>
        /// 历史数据表，查询历史数据函数使用（HistoryDataTable）
        /// </summary>
        public static DataTable HistoryDataTable { get; set; }

		/// <summary>
		/// 表格数据
		/// </summary>
		public static Worksheet sheet { get; set; }

		/// <summary>
		/// 稀松查询结果
		/// </summary>
		public static DataTable SloppyHistoryDataTable { get; set; }
		#endregion

		#region 自定义计算方法
		/// <summary>
		/// 读取历史数据
		/// </summary>
		/// <param name="tagName"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static decimal GetHistoryData(string tagName,string dateTime)
        {
            try
            {
                string filtString = "time=" + "'" + dateTime + "'" + " and tagName=" + "'" + tagName + "'";
                DataRow dr = HistoryDataTable.Select(filtString).First();
                return decimal.Parse(dr[2].ToString());//返回查找到的数值
            }
            catch(Exception ex)
            {
                return 0;
                throw ex;//抛出异常
            }
        }
		/// <summary>
		/// 读取稀松数据
		/// </summary>
		/// <param name="tagName"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static decimal GetHistorySloppyData(string tagName, string dateTime)
		{
			try
			{
				string filtString = "time=" + "'" + dateTime + "'" + " and tagName=" + "'" + tagName + "'";
				DataRow dr = SloppyHistoryDataTable.Select(filtString).First();
				return decimal.Parse(dr[2].ToString());//返回查找到的数值
			}
			catch (Exception ex)
			{
				return 0;
				throw ex;//抛出异常
			}
		}
		/// <summary>
		/// 历史数据求和
		/// </summary>
		/// <param name="tagName"></param>
		/// <returns></returns>
		public static decimal GetHistoryDataSum(string tagName,int exceptFirst)
		{
			//exceptFirst 是否舍去8点的数据,这里并不具有通用型,后续需要钙胶囊
			try
			{
				string filtString ="tagName=" + "'" + tagName + "'";
				DataRow[] drs = HistoryDataTable.Select(filtString);
				int firstHour = DateTime.Parse(drs[0][0].ToString()).Hour;
				decimal sum = 0;			
				if (exceptFirst == 0)
				{
					for (int i = 0; i < drs.Length; i++)
					{
						sum += decimal.Parse(drs[i][2].ToString());
					}
				}
				else
				{
					if (firstHour == 8)
					{
						for (int i = 1; i < drs.Length; i++)
						{
							sum += decimal.Parse(drs[i][2].ToString());
						}
					}
					else
					{
						for (int i = 0; i < drs.Length; i++)
						{
							sum += decimal.Parse(drs[i][2].ToString());
						}
					}
				}
				return sum;
			}
			catch (Exception ex)
			{
				return 0;
				throw ex;//抛出异常
			}
		}
		/// <summary>
		/// 读取单元格数据
		/// </summary>
		/// <param name="cellPositon"></param>
		/// <returns></returns>
		public static object readCellValue(string cellPositon)
		{
			object value;
			try
			{
				cellPositon = cellPositon.ToUpper();
				value = sheet[cellPositon];
				if (null == value) value = 0;
			}
			catch (Exception ex)
			{
				return 0;
				throw ex;//抛出异常
			}
			return value;
		}
		/// <summary>
		/// if语句
		/// </summary>
		/// <param name="compareValue"></param>
		/// <param name="gtString"></param>
		/// <param name="ltString"></param>
		/// <returns></returns>
		public static string ifTest(int  compareValue,string gtString,string ltString,string euqString)
		{
			if (compareValue == 1)
			{
				return gtString;
			}
			else if (compareValue == -1)
			{
				return ltString;
			}
			else
			{
				return euqString;
			}
			
		}
		/// <summary>
		/// 查找历史数据最大值
		/// </summary>
		/// <param name="tagName"></param>
		/// <returns></returns>
		public static double MaxOfHistory(string tagName)
		{
			try
			{
				string filtString = "tagName=" + "'" + tagName + "'";
				DataRow[] drs = HistoryDataTable.Select(filtString);
				//return decimal.Parse(dr[2].ToString());//返回查找到的数值
				double max = 0;
				foreach (DataRow dr in drs)
				{
					double d = double.Parse(dr[2].ToString());
					if (d > max) max = d;
				}
				return max;
			}
			catch (Exception ex)
			{
				return 0;
				throw ex;//抛出异常
			}
		}
		/// <summary>
		/// 查找历史数据最小值
		/// </summary>
		/// <param name="tagName"></param>
		/// <returns></returns>
		public static double MinOfHistory(string tagName)
		{
			try
			{
				string filtString = "tagName=" + "'" + tagName + "'";
				DataRow[] drs = HistoryDataTable.Select(filtString);
				//return decimal.Parse(dr[2].ToString());//返回查找到的数值
				double min = double.MaxValue;
				foreach (DataRow dr in drs)
				{
					double d = double.Parse(dr[2].ToString());
					if (d < min) min = d;
				}
				return min;
			}
			catch (Exception ex)
			{
				return 0;
				throw ex;//抛出异常
			}
		}
		/// <summary>
		/// 比较大小
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int Compare(double a, double b)
		{
			a=double.Parse(a.ToString("0.00"));
			b = double.Parse(b.ToString("0.00"));
			if (a > b)
			{
				return 1;
			}
			else if (a < b)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}
		/// <summary>
		/// 计算历史数据平均值
		/// </summary>
		/// <param name="tagName"></param>
		/// <returns></returns>
		public static double avgOfHistory(string tagName)
		{
			try
			{
				string filtString = "tagName=" + "'" + tagName + "'";
				DataRow[] drs = HistoryDataTable.Select(filtString);
				//return decimal.Parse(dr[2].ToString());//返回查找到的数值
				double avg = 0.0;
				double sum = 0.0;
				foreach (DataRow dr in drs)
				{
					double d = double.Parse(dr[2].ToString());
					sum += d;
				}
				avg = sum / drs.Length;
				return avg;
			}
			catch (Exception ex)
			{
				return 0;
				throw ex;//抛出异常
			}
		}
		/// <summary>
		/// 历史数据最后一个值
		/// </summary>
		/// <param name="tagName"></param>
		/// <returns></returns>
		public static double lastOfHistory(string tagName)
		{
			double value = 0.0;
			try
			{
				string filtString = "tagName=" + "'" + tagName + "'";
				DataRow[] drs = HistoryDataTable.Select(filtString);
				value  = double.Parse(drs.Last()[2].ToString());
				return value;
			}
			catch (Exception ex)
			{
				return 0;
				throw ex;//抛出异常
			}

		}
		/// <summary>
	/// 历史数据第一个值
	/// </summary>
	/// <param name="tagName"></param>
	/// <returns></returns>
		public static double firstOfHistory(string tagName)
		{
			double value = 0.0;
			try
			{
				string filtString = "tagName=" + "'" + tagName + "'";
				DataRow[] drs = HistoryDataTable.Select(filtString);
				value = double.Parse(drs.First()[2].ToString());
				return value;
			}
			catch (Exception ex)
			{
				return 0;
				throw ex;//抛出异常
			}

		}
		#endregion
	}
}

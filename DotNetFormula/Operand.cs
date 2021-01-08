using System;

namespace DotNetFormula
{
	/// <summary>  
	/// 操作数类型  
	/// </summary>  
	public enum OperandType
	{
		/// <summary>  
		/// 函数  
		/// </summary>  
		FUNC = 1,
		/// <summary>  
		/// 日期  
		/// </summary>  
		DATE = 2,
		/// <summary>  
		/// 数字  
		/// </summary>  
		NUMBER = 3,
		/// <summary>  
		/// 布尔  
		/// </summary>  
		BOOLEAN = 4,
		/// <summary>  
		/// 字符串  
		/// </summary>  
		STRING = 5
	}

	/// <summary>
	/// 操作数类
	/// </summary>
	public class Operand
	{
		#region 构造函数 
		public Operand(OperandType type, object value)
		{
			this.Type = type;
			this.Value = value;
		}
		public Operand(string opd, object value)
		{
			this.Type = ConvertOperand(opd);
			this.Value = value;
		}
		#endregion

		#region 属性 
		/// <summary>
		/// 操作数类型
		/// </summary>
		public OperandType Type { get; set; }
		/// <summary>  
		/// 关键字  
		/// </summary>  
		public string Key { get; set; }
		/// <summary>  
		/// 操作数值  
		/// </summary>  
		public object Value { get; set; }
		#endregion

		#region 公开方法  
		/// <summary>  
		/// 转换操作数到指定的类型  
		/// </summary>  
		/// <param name="opd">操作数</param>  	
		/// <returns>返回对应的操作数类型</returns>  
		public static OperandType ConvertOperand(string opd)
		{
			if (opd.IndexOf("(") > -1)
			{
				return OperandType.FUNC;
			}
			else if (IsNumber(opd))
			{
				return OperandType.NUMBER;
			}
			else if (IsDate(opd))
			{
				return OperandType.DATE;
			}
			else
			{
				return OperandType.STRING;
			}
		}
		/// <summary>
		/// 判断对象是否为数字
		/// </summary>
		/// <param name="value">要判断的对象</param>
		/// <returns>true/false</returns>
		public static bool IsNumber(object value)
		{
			double val;
			return double.TryParse(value.ToString(), out val);
		}
		/// <summary>
		/// 判断对象是否为日期时间
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsDate(object value)
		{
			DateTime dt;
			return DateTime.TryParse(value.ToString(), out dt);
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetFormula
{
	/// <summary>
	/// 运算符类型(从上到下优先级依次递减)，数值越大，优先级越低
	/// </summary>
	public enum OperatorType
	{
		/// <summary>  
		/// 左括号:(,left bracket  
		/// </summary>  
		LB = 10,

		/// <summary>  
		/// 右括号),right bracket  
		/// </summary>  
		RB = 11,

		/// <summary>  
		/// 逻辑非,!,NOT  
		/// </summary>  
		NOT = 15,

		/// <summary>  
		/// 正号,+,positive sign  
		/// </summary>  
		PS = 16,

		/// <summary>  
		/// 负号,-,negative sign  
		/// </summary>  
		NS = 17,

		/// <summary>  
		/// 正切，tan  
		/// </summary>  
		TAN = 101,

		/// <summary>  
		/// 反正切，atan  
		/// </summary>  
		ATAN = 102,

		/// <summary>
		/// 最大值(函数)
		/// </summary>
		MAX=103,

		/// <summary>
		/// 最小值(函数)
		/// </summary>
		MIN = 104,

		/// <summary>
		/// 平均值(函数)
		/// </summary>
		AVG = 105,

		/// <summary>
		/// 历史值(函数)
		/// </summary>
		HIS = 106,

		/// <summary>
		/// 历史求和函数
		/// </summary>
		HOS=107,

		/// <summary>
		/// 条件判断
		/// </summary>
		IF = 108,

		/// <summary>
		/// 历史数据平均值
		/// </summary>
		HISAVG=109,

		/// <summary>
		/// 历史最大值
		/// </summary>
		HISMAX = 110,

		/// <summary>
		/// 历史最小值
		/// </summary>
		HISMIN = 111,

		/// <summary>
		/// 历史数据第一个值
		/// </summary>
		HISLAST=112,

		/// <summary>
		/// 历史数据最后一个值
		/// </summary>
		HISFIRST=113,

		/// <summary>
		/// 稀松查询
		/// </summary>
		HISSLOPPY=114,

		/// <summary>
		/// 读取cell值
		/// </summary>
		CELL = 115,

		/// <summary>  
		/// 乘,*,multiplication  
		/// </summary>  
		MUL = 201,

		/// <summary>  
		/// 除,/,division  
		/// </summary>  
		DIV = 202,

		/// <summary>  
		/// 余,%,modulus  
		/// </summary>  
		MOD = 203,

		/// <summary>  
		/// 加,+,Addition  
		/// </summary>  
		ADD = 204,

		/// <summary>  
		/// 减,-,subtraction  
		/// </summary>  
		SUB = 205,

		/// <summary>  
		/// 小于,less than  
		/// </summary>  
		LT = 206,

		/// <summary>  
		/// 小于或等于,less than or equal to  
		/// </summary>  
		LE = 207,

		/// <summary>  
		/// 大于,>,greater than  
		/// </summary>  
		GT = 208,

		/// <summary>  
		/// 大于或等于,>=,greater than or equal to  
		/// </summary>  
		GE = 209,

		/// <summary>  
		/// 等于,=,equal to  
		/// </summary>  
		ET = 301,

		/// <summary>  
		/// 不等于,unequal to  
		/// </summary>  
		UT = 302,

		/// <summary>  
		/// 逻辑与,&,AND  
		/// </summary>  
		AND = 401,

		/// <summary>  
		/// 逻辑或,|,OR  
		/// </summary>  
		OR = 402,

		/// <summary>  
		/// 逗号,comma  
		/// </summary>  
		CA = 501,

		/// <summary>  
		/// 结束符号 @  
		/// </summary>  
		END = 502,

		/// <summary>  
		/// 错误符号  
		/// </summary>  
		ERR = 503

	}
	/// <summary>
	/// 操作符类
	/// </summary>
	public class Operator
	{
		/// <summary>
		/// 支持的操作符号（函数属于操作符号）
		/// </summary>
		private static readonly List<string> _OperatorsSupport = new List<string>(new string[]
		{ "(", "tan", ")", "atan", "*", "/", "%", "+", "-", "<", ">", "=", ",", "@", "max", "min", "avg", "his","hos","cell","if","hismax","hismin","hisavg","hislast","hisfirst","hissloppy"});//为了避免错误，全部保持小写
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		public Operator(OperatorType type, string value)
		{
			this.Type = type;
			this.Value = value;
		}

		/// <summary>  
		/// 运算符类型  
		/// </summary>  
		public OperatorType Type { get; set; }

		/// <summary>  
		/// 运算符值  
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// 返回支持的操作符号
		/// </summary>
		/// <returns></returns>
		public static List<string> GetSupportOperators()
		{
			//为了防止出现大小写不匹配
			for (int i = 0; i < _OperatorsSupport.Count; i++)
			{
				_OperatorsSupport[i] = _OperatorsSupport[i].ToLower();
			}
			//对字符串排序：从长到短，按照字母顺序排序
			List<string> m__OperatorsSupport = _OperatorsSupport;
			m__OperatorsSupport.Sort(Comparer);
			return m__OperatorsSupport;
		}
		/// <summary>
		/// 自定义排序方法：按照字符串长度，字符串的值排序
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static int Comparer(string a, string b)
		{
			if (a.Length > b.Length)
			{
				return -1;
			}
			else if (a.Length < b.Length)
			{
				return 1;
			}
			else
			{
				return string.Compare(a, b);
			}

		}

		/// <summary>  
		/// 转换运算符到指定的类型  
		/// </summary>  
		/// <param name="opt">运算符</param>  
		/// <returns>返回指定的运算符类型</returns>  
		public static OperatorType ConvertOperator(string opt)
		{
			switch (opt.ToLower())
			{
				case "!": return OperatorType.NOT;
				case "+": return OperatorType.ADD;
				case "-": return OperatorType.SUB;
				case "*": return OperatorType.MUL;
				case "/": return OperatorType.DIV;
				case "%": return OperatorType.MOD;
				case "<": return OperatorType.LT;
				case ">": return OperatorType.GT;
				case "<=": return OperatorType.LE;
				case ">=": return OperatorType.GE;
				case "<>": return OperatorType.UT;
				case "=": return OperatorType.ET;
				case "&": return OperatorType.AND;
				case "|": return OperatorType.OR;
				case ",": return OperatorType.CA;
				case "@": return OperatorType.END;
				case "tan": return OperatorType.TAN;
				case "atan": return OperatorType.ATAN;
				case "max":return OperatorType.MAX;
				case "min": return OperatorType.MIN;
				case "avg": return OperatorType.AVG;
				case "his": return OperatorType.HIS;
				case "hos":return OperatorType.HOS;
				case "cell":return OperatorType.CELL;
				case "if":return OperatorType.IF;
				case "hismax":return OperatorType.HISMAX;
				case "hismin":return OperatorType.HISMIN;
				case "hisavg": return OperatorType.HISAVG;
				case "hislast":return OperatorType.HISLAST;
				case "hisfirst":return OperatorType.HISFIRST;
				case "hissloppy":return OperatorType.HISSLOPPY;
				default: return OperatorType.ERR;
			}
		}

		/// <summary>  
		/// 运算符优先级比较  
		/// </summary>  
		/// <param name="optA">运算符类型A</param>  
		/// <param name="optB">运算符类型B</param>  
		/// <returns>A与B相比，-1，低；0,相等；1，高</returns>  
		public static int ComparePriority(OperatorType optA, OperatorType optB)
		{
			if (optA == optB)
			{
				//A、B优先级相等  
				return 0;
			}

			//乘,除,余(*,/,%)  
			if ((optA >= OperatorType.MUL && optA <= OperatorType.MOD) &&
				(optB >= OperatorType.MUL && optB <= OperatorType.MOD))
			{
				return 0;
			}
			//加,减(+,-)  
			if ((optA >= OperatorType.ADD && optA <= OperatorType.SUB) &&
				(optB >= OperatorType.ADD && optB <= OperatorType.SUB))
			{
				return 0;
			}
			//小于,小于或等于,大于,大于或等于(<,<=,>,>=)  
			if ((optA >= OperatorType.LT && optA <= OperatorType.GE) &&
				(optB >= OperatorType.LT && optB <= OperatorType.GE))
			{
				return 0;
			}
			//等于,不等于(=,<>)  
			if ((optA >= OperatorType.ET && optA <= OperatorType.UT) &&
				(optB >= OperatorType.ET && optB <= OperatorType.UT))
			{
				return 0;
			}
			//三角函数  
			if ((optA >= OperatorType.TAN && optA <= OperatorType.ATAN) &&
					(optB >= OperatorType.TAN && optB <= OperatorType.ATAN))
			{
				return 0;
			}
			//自定义函数 
			if ((optA >= OperatorType.MAX && optA <= OperatorType.CELL) &&
					(optB >= OperatorType.MAX && optB <= OperatorType.CELL))
			{
				return 0;
			}
			if (optA < optB)
			{
				//A优先级高于B  
				return 1;
			}

			//A优先级低于B  
			return -1;

		}
		
	}
}
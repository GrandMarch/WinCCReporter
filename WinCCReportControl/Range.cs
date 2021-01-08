using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using unvell.ReoGrid;
using unvell.ReoGrid.DataFormat;
using static unvell.ReoGrid.DataFormat.NumberDataFormatter;

namespace WinCCReportControl
{
	enum RANGETYPE
	{
		Calc,
		Text
	}
	
	class Range
	{
		#region 属性
		/// <summary>
		/// cell 位置
		/// </summary>
		public RangePosition Postion
		{
			get { return _position; }
		}

		/// <summary>
		/// cell 公式或者字符串
		/// </summary>
		public string FormulaOrText
		{
			get { return _formula; }
		}

		/// <summary>
		/// cell类型
		/// </summary>
		public RANGETYPE RangeType
		{
			get { return _type; }
		}

		/// <summary>
		/// 数据小数位数
		/// </summary>
		public NumberFormatArgs NumberFormatArgs
		{
			get { return _numberFormatArgs; }
		}

		/// <summary>
		/// 字体设置
		/// </summary>
		public string  Font
		{
			get;
			private set;
		}

		/// <summary>
		/// 字号
		/// </summary>
		public float FontSize
		{
			get;
			private set;
		}

		/// <summary>
		/// range 水平对齐方式
		/// </summary>
		public ReoGridHorAlign HAlign
		{
			get;
			private set;
		}
		
		/// <summary>
		/// 垂直对齐方式
		/// </summary>
		public ReoGridVerAlign VAlign
		{
			get;
			private set;
		}
		#endregion

		#region 变量
		private RangePosition _position;
		private string _formula = "";
		private NumberFormatArgs _numberFormatArgs;
		private RANGETYPE _type;
		#endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="position"></param>
		/// <param name="dataFormat"></param>
		/// <param name="formula"></param>
		public Range(RangePosition position, string dataFormat,string formula,RANGETYPE type,string font,float fontsize, ReoGridVerAlign valign, ReoGridHorAlign halign)
		{
			_position = position;
			_formula = formula;
			_numberFormatArgs = getDataFormat(dataFormat);
			_type = type;
			Font = font;
			FontSize = fontsize;
			HAlign = halign;
			VAlign = valign;
		}

		/// <summary>
		/// 根据格式返回 NumberFormatArgs
		/// </summary>
		/// <param name="dataFormat"></param>
		/// <returns></returns>
		private NumberFormatArgs getDataFormat(string dataFormat)
		{
			short lenth = 0;
			if (dataFormat.Length <= 1)
			{
				lenth = 2;
			}
			else
			{
				lenth = (short)dataFormat.Split('.')[1].Length;
			}
			
			NumberFormatArgs arg = new NumberDataFormatter.NumberFormatArgs()
			{
				// 保留小数位数
				DecimalPlaces = lenth,
				// 默认显示负数
				NegativeStyle = NumberDataFormatter.NumberNegativeStyle.Default,
				// 不使用逗号分隔
				UseSeparator = false,
			};
			return arg;
		}
	}
}

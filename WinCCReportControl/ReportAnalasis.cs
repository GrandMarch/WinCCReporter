using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using unvell.ReoGrid;

namespace WinCCReportControl
{
	enum REPORTTYPE
	{
		Fixed,
		RowAdd,
		NONE
	}
	class ReportAnalasis
	{
		private string xmlFileName;
		private XmlDocument xmlDocument = new XmlDocument();
		private XmlNode root;
		public REPORTTYPE ReportType = REPORTTYPE.NONE;
		private int RowCount = 0;
		private int HourStep=0;
		private int FootRowStart = 0;
		private string startTime, endTime, step;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="fileName"></param>
		public ReportAnalasis(string fileName,string starttime,string endtime,string step)
		{
			xmlFileName = fileName;
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.IgnoreComments = true;   //忽略注释
			XmlReader reader = XmlReader.Create(@fileName, settings);
			xmlDocument.Load(reader);
			root = xmlDocument.SelectSingleNode("Report");
			//读取报表类型
			ReportType = GetReportType();
			//报表行数
			RowCount = (int)(DateTime.Parse(endtime) - DateTime.Parse(starttime)).TotalHours / int.Parse(step);
			//小时间隔
			HourStep = int.Parse(step);
			startTime = starttime;
			endTime = endtime;
			this.step = step;
		}

		public ReportAnalasis()
		{ }

		/// <summary>
		/// 得到报表类型
		/// </summary>
		/// <returns></returns>
		private REPORTTYPE GetReportType()
		{
			XmlElement xe = (XmlElement)root;
			string typeString = xe.GetAttribute("type");
			switch (typeString)
			{
				case "Fixed":return REPORTTYPE.Fixed;
				case "RowAdd":return REPORTTYPE.RowAdd;
				default:return REPORTTYPE.NONE;
			}
		}

		/// <summary>
		/// 读取指定node的range
		/// </summary>
		/// <param name="nodeName"></param>
		/// <returns></returns>
		private Range[] GetRangeInNode(string nodeName)
		{
			List<Range> cellList = new List<Range> { };
			XmlNode cells = root.SelectSingleNode(nodeName);
			if (null == cells) throw new NullReferenceException("无法读取xml文件中<"+ nodeName +"></"+nodeName + ">标签");
			foreach (XmlNode xn in cells.ChildNodes)
			{
				XmlElement xe = (XmlElement)xn;
				string _TopLeft = xe.GetAttribute("TopLeft");
				string _BottomRight = xe.GetAttribute("BottomRight");
				string _format = xe.GetAttribute("Format");
				string _type= xe.GetAttribute("Type");
				string _formula = @xn.InnerText;
				string _font= xe.GetAttribute("Font");
				string _fontSize = xe.GetAttribute("FontSize");
				string _HAlign= xe.GetAttribute("HAlign");
				string _VAlign= xe.GetAttribute("VAlign");
				_formula = _formula.Replace("[REPORTDATE]", "'"+DateTime.Parse(endTime).ToString());
				//range位置
				RangePosition _pos = new RangePosition();
				if (_BottomRight == "")
				{
					_pos = new RangePosition(_TopLeft, _TopLeft);
				}
				else
				{
					_pos = new RangePosition(_TopLeft, _BottomRight);
				}
				//range 类型
				RANGETYPE rangeType;
				switch (_type)
				{
					case "text":rangeType = RANGETYPE.Text;
						break;
					case "calc":rangeType = RANGETYPE.Calc;
						break;
					default:
						rangeType = RANGETYPE.Text;
						break;
				}
				ReoGridHorAlign HAlign;
				switch (_HAlign)
				{
					case "Center":
						HAlign = ReoGridHorAlign.Center;
						break;
					case "DistributedIndent":
						HAlign = ReoGridHorAlign.DistributedIndent;
						break;
					case "General":
						HAlign = ReoGridHorAlign.General;
						break;
					case "Left":
						HAlign = ReoGridHorAlign.Left;
						break;
					case "Right":
						HAlign = ReoGridHorAlign.Right;
						break;
					default:
						HAlign = ReoGridHorAlign.Left;
						break;						
				}
				ReoGridVerAlign VAlign ;
				switch (_HAlign)
				{
					case "Bottom":
						VAlign = ReoGridVerAlign.Bottom;
						break;
					case "General":
						VAlign = ReoGridVerAlign.General;
						break;
					case "Middle":
						VAlign = ReoGridVerAlign.Middle;
						break;
					case "Top":
						VAlign = ReoGridVerAlign.Top;
						break;
					default:
						VAlign = ReoGridVerAlign.Middle;
						break;
				}
				//创建并添加range到list
				if (_font == "")
				{
					_font = "微软雅黑";
				}
				if (_fontSize == "")
				{
					_fontSize = "12";
				}
				_formula=_formula.Replace("[NOW]", "'"+DateTime.Parse(endTime).ToString("yyyy-MM-dd 08:00:00")+"'");
				_formula=_formula.Replace("[FIRSTDAYOFMONTH]", "'"+DateTime.Parse(endTime).ToString("yyyy-MM-01 08:00:00")+"'");
				_formula=_formula.Replace("[FIRSDAYOFYEAR]", "'"+DateTime.Parse(endTime).ToString("yyyy-01-01 08:00:00")+"'");
				Range range = new Range(_pos, _format, _formula, rangeType,_font,float.Parse(_fontSize),VAlign,HAlign);
				cellList.Add(range);
			}
			return cellList.ToArray(); 
		}

		/// <summary>
		/// 读取报表变量
		/// </summary>
		/// <returns></returns>
		public void GetReportTags(out string[] denseTag, out string[] sloppyTag)
		{
			List<string> denseList = new List<string> { };
			List<string> sloppyList = new List<string> { };
			XmlNode tags = root.SelectSingleNode("tags");
			if (null == tags) throw new NullReferenceException("无法读取xml文件中<tags></tags>标签");
			foreach (XmlNode xn in tags.ChildNodes)
			{
				XmlElement xe = (XmlElement)xn;
				string sloppy = xe.GetAttribute("sloppy");
				if (sloppy == "1")
				{
					sloppyList.Add(xn.InnerText);
				}
				else
				{
					denseList.Add(xn.InnerText);
				
				}
				
			}
			denseTag = denseList.ToArray();
			sloppyTag = sloppyList.ToArray();
			//return tagList.ToArray();
		}

		/// <summary>
		/// 读取报表头
		/// </summary>
		/// <returns></returns>
		public Range[] GetReportHead()
		{
			return GetRangeInNode("head");
		}

		/// <summary>
		/// 读取报表体
		/// </summary>
		/// <returns></returns>
		public Range[] GetReportBody()
		{
			if (ReportType == REPORTTYPE.Fixed)
			{
				return GetRangeInNode("body");
			}
			else
			{
				return GetRowAddBody();
			}
			
		}

		/// <summary>
		/// 读取报表脚
		/// </summary>
		/// <returns></returns>
		public Range[] GetReportFoot()
		{
			if (ReportType == REPORTTYPE.RowAdd)
			{
				return GetRowAddFoot();
			}
			else
			{
				return GetRangeInNode("foot");
			}
		}

		/// <summary>
		/// 行不确定报表的主体读取
		/// </summary>
		/// <returns></returns>
		private Range[] GetRowAddBody()
		{
			List<Range> cellList = new List<Range> { };
			XmlNode cells = root.SelectSingleNode("body");
			if (null == cells) throw new NullReferenceException("无法读取xml文件中<body></body>标签");
			XmlElement xe1 = (XmlElement)cells;
			string start = xe1.GetAttribute("Start");
			int initRow = int.Parse(Regex.Matches(start, @"\d+")[0].Value);
			FootRowStart = initRow + RowCount-1;
			string col = Regex.Matches(start, @"\D+")[0].Value;
			int initCol = FromNumberSystem26(col);		
			foreach (XmlNode xn in cells.ChildNodes)
			{
				int Row = initRow;
				for (int i = 0; i < RowCount; i++)
				{
					XmlElement xe = (XmlElement)xn;
					string _format = xe.GetAttribute("Format");
					string _type = xe.GetAttribute("Type");
					string _formula = @xn.InnerText;
					string date = (DateTime.Parse(startTime).AddHours(HourStep*(i+1))).ToString();				
					RangePosition _pos = new RangePosition(col + Row.ToString(), col + Row.ToString());
					string _font = xe.GetAttribute("Font");
					string _fontSize = xe.GetAttribute("FontSize");
					string _HAlign = xe.GetAttribute("HAlign");
					string _VAlign = xe.GetAttribute("VAlign");
					RANGETYPE rangeType;
					switch (_type)
					{
						case "text":
							rangeType = RANGETYPE.Text;
							break;
						case "calc":
							rangeType = RANGETYPE.Calc;
							break;
						default:
							rangeType = RANGETYPE.Text;
							break;
					}
					if (rangeType == RANGETYPE.Text)
					{
						_formula = _formula.Replace("[INCREASETIME]", "'" + date);
					}
					else
					{
						_formula = _formula.Replace("[INCREASETIME]", "'"+date+"'");
					}
					ReoGridHorAlign HAlign;
					switch (_HAlign)
					{
						case "Center":
							HAlign = ReoGridHorAlign.Center;
							break;
						case "DistributedIndent":
							HAlign = ReoGridHorAlign.DistributedIndent;
							break;
						case "General":
							HAlign = ReoGridHorAlign.General;
							break;
						case "Left":
							HAlign = ReoGridHorAlign.Left;
							break;
						case "Right":
							HAlign = ReoGridHorAlign.Right;
							break;
						default:
							HAlign = ReoGridHorAlign.Left;
							break;
					}
					ReoGridVerAlign VAlign;
					switch (_HAlign)
					{
						case "Bottom":
							VAlign = ReoGridVerAlign.Bottom;
							break;
						case "General":
							VAlign = ReoGridVerAlign.General;
							break;
						case "Middle":
							VAlign = ReoGridVerAlign.Middle;
							break;
						case "Top":
							VAlign = ReoGridVerAlign.Top;
							break;
						default:
							VAlign = ReoGridVerAlign.Middle;
							break;
					}
					if (_font == "")
					{
						_font = "微软雅黑";
					}
					if (_fontSize == "")
					{
						_fontSize = "12";
					}
					Range range = new Range(_pos, _format, _formula, rangeType,_font,float.Parse(_fontSize),VAlign,HAlign);
					cellList.Add(range);
					Row++;
				}
				initCol++;
				col=ToNumberSystem26(initCol);
			}
			return cellList.ToArray();
		}

		/// <summary>
		/// 行自增报表的表尾
		/// </summary>
		/// <returns></returns>
		private Range[] GetRowAddFoot()
		{
			List<Range> cellList = new List<Range> { };
			XmlNode cells = root.SelectSingleNode("foot");
			foreach (XmlNode xn in cells)
			{
				XmlElement xe = (XmlElement)xn;
				string _format = xe.GetAttribute("Format");
				string _type = xe.GetAttribute("Type");
				string _formula = @xn.InnerText;
				string _font = xe.GetAttribute("Font");
				string _fontSize = xe.GetAttribute("FontSize");
				string _HAlign = xe.GetAttribute("HAlign");
				string _VAlign = xe.GetAttribute("VAlign");

				RANGETYPE rangeType;
				switch (_type)
				{
					case "text":
						rangeType = RANGETYPE.Text;
						break;
					case "calc":
						rangeType = RANGETYPE.Calc;
						break;
					default:
						rangeType = RANGETYPE.Text;
						break;
				}
				ReoGridHorAlign HAlign;
				switch (_HAlign)
				{
					case "Center":
						HAlign = ReoGridHorAlign.Center;
						break;
					case "DistributedIndent":
						HAlign = ReoGridHorAlign.DistributedIndent;
						break;
					case "General":
						HAlign = ReoGridHorAlign.General;
						break;
					case "Left":
						HAlign = ReoGridHorAlign.Left;
						break;
					case "Right":
						HAlign = ReoGridHorAlign.Right;
						break;
					default:
						HAlign = ReoGridHorAlign.Left;
						break;
				}
				ReoGridVerAlign VAlign;
				switch (_HAlign)
				{
					case "Bottom":
						VAlign = ReoGridVerAlign.Bottom;
						break;
					case "General":
						VAlign = ReoGridVerAlign.General;
						break;
					case "Middle":
						VAlign = ReoGridVerAlign.Middle;
						break;
					case "Top":
						VAlign = ReoGridVerAlign.Top;
						break;
					default:
						VAlign = ReoGridVerAlign.Middle;
						break;
				}
				if (_font == "")
				{
					_font = "微软雅黑";
				}
				if (_fontSize == "")
				{
					_fontSize = "12";
				}
				string _TopLeft = xe.GetAttribute("TopLeft");
				string _BottomRight = xe.GetAttribute("BottomRight");
				if (_TopLeft == "")
				{
					_TopLeft = "A1";
				}
				else
				{
					if (_BottomRight == "")
					{
						_BottomRight = _TopLeft;
					}
				}
				int toprow = int.Parse(Regex.Matches(_TopLeft, @"\d+")[0].Value)+FootRowStart;
				string topcol=  Regex.Matches(_TopLeft, @"\D+")[0].Value;
				int bottomrow = int.Parse(Regex.Matches(_BottomRight, @"\d+")[0].Value)+ FootRowStart;
				string bottomcol = Regex.Matches(_BottomRight, @"\D+")[0].Value;
				RangePosition _pos = new RangePosition(topcol + toprow.ToString(), bottomcol + bottomrow.ToString());
				_formula=_formula.Replace("[NOW]", "'" + DateTime.Parse(endTime).ToString("yyyy-MM-dd 08:00:00") + "'");
				_formula=_formula.Replace("[FIRSTDAYOFMONTH]", "'" + DateTime.Parse(endTime).ToString("yyyy-MM-01 08:00:00") + "'");
				_formula=_formula.Replace("[FIRSTDAYOFYEAR]", "'" + DateTime.Parse(endTime).ToString("yyyy-01-01 08:00:00")+ "'");
				Range range = new Range(_pos, _format, _formula, rangeType, _font, float.Parse(_fontSize), VAlign, HAlign);
				cellList.Add(range);
			}
			return cellList.ToArray();
		}

		/// <summary>
		/// 将指定的自然数转换为26进制表示。映射关系：[1-26] ->[A-Z]。
		/// </summary>
		/// <param name="n">自然数（如果无效，则返回空字符串）。</param>
		/// <returns>26进制表示。</returns>
		private  string ToNumberSystem26(int n)
		{
			string s = string.Empty;
			while (n > 0)
			{
				int m = n % 26;
				if (m == 0) m = 26;
				s = (char)(m + 64) + s;
				n = (n - m) / 26;
			}
			return s;
		}

		/// <summary>
		/// 将指定的26进制表示转换为自然数。映射关系：[A-Z] ->[1-26]。
		/// </summary>
		/// <param name="s">26进制表示（如果无效，则返回0）。</param>
		/// <returns>自然数。</returns>
		private  int FromNumberSystem26(string s)
		{
			if (string.IsNullOrEmpty(s)) return 0;
			int n = 0;
			for (int i = s.Length - 1, j = 1; i >= 0; i--, j *= 26)
			{
				char c = Char.ToUpper(s[i]);
				if (c < 'A' || c > 'Z') return 0;
				n += ((int)c - 64) * j;
			}
			return n;
		}

		private void ReapleaceDefine(string Formula)
		{

		} 
	}
}

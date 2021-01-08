using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNetFormula
{
	/// Reverse Polish Notation  
	/// 逆波兰式  
	/// </summary>  
	public class RPN
	{
		Stack<object> m_tokens = new Stack<object>();  //最终逆波兰式堆栈  
		/// <summary>  
																 /// 最终逆波兰式堆栈  
																 /// </summary>  
		public Stack<object> Tokens
		{
			get { return m_tokens; }
		}

		private string _RPNExpression;
		/// <summary>  
		/// 生成的逆波兰式字符串  
		/// </summary>  
		public string RPNExpression
		{
			get
			{
				if (_RPNExpression == null)
				{
					foreach (var item in Tokens)
					{
						if (item is Operand)
						{
							_RPNExpression += ((Operand)item).Value + ",";
						}
						if (item is Operator)
						{
							_RPNExpression += ((Operator)item).Value + ",";
						}
					}
				}
				return _RPNExpression;
			}
		}

		List<string> m_Operators = Operator.GetSupportOperators();
		//new List<string>(new string[]{
			//"(","tan",")","atan","!","*","/","%","+","-","<",">","=","&","|",",","@"});    //允许使用的运算符  
		/// <summary>
		/// 检查运算符匹配
		/// </summary>
		/// <param name="exp"></param>
		/// <returns></returns>
		private bool IsMatching(string exp)
		{
			string opt = "";    //临时存储 " ' # (  

			for (int i = 0; i < exp.Length; i++)
			{
				string chr = exp.Substring(i, 1);   //读取每个字符  
				if ("\"'#".Contains(chr))   //当前字符是双引号、单引号、井号的一种  
				{
					if (opt.Contains(chr))  //之前已经读到过该字符  
					{
						opt = opt.Remove(opt.IndexOf(chr), 1);  //移除之前读到的该字符，即匹配的字符  
					}
					else
					{
						opt += chr;     //第一次读到该字符时，存储  
					}
				}
				else if ("()".Contains(chr))    //左右括号  
				{
					if (chr == "(")
					{
						opt += chr;
					}
					else if (chr == ")")
					{
						if (opt.Contains("("))
						{
							opt = opt.Remove(opt.IndexOf("("), 1);
						}
						else
						{
							return false;
						}
					}
				}
			}
			return (opt == "");
		}
		
		/// <summary>
		/// 自定义计算函数需要的数据
		/// </summary>
		//public object funData { get; set; }
		/*
		/// <summary>  
		/// 从表达式中查找运算符位置  
		/// </summary>  
		/// <param name="exp">表达式</param>  
		/// <param name="findOpt">要查找的运算符</param>  
		/// <returns>返回运算符位置</returns>  
		private int FindOperator(string exp, string findOpt)
		{
			string opt = "";
			for (int i = 0; i < exp.Length; i++)
			{
				string chr = exp.Substring(i, 1);
				if ("\"'#".Contains(chr))//忽略双引号、单引号、井号中的运算符  
				{
					if (opt.Contains(chr))
					{
						opt = opt.Remove(opt.IndexOf(chr), 1);
					}
					else
					{
						opt += chr;
					}
				}
				if (opt == "")
				{
					if (findOpt != "")
					{
						if (findOpt == chr)
						{
							return i;
						}
					}
					else
					{
						if (m_Operators.Exists(x => x.ToLower().Contains(chr.ToLower())))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}
		*/
		/// <summary>
		/// 从表达式查找运算符位置
		/// </summary>
		/// <param name="exp"></param>
		/// <param name="seps"></param>
		/// <returns></returns>
		private int[] FindOperator(string exp,string[] seps)
		{					 
			int[] arr = new int[2];
			string[] strMid;
			exp = exp.ToLower();
			char[] expChars = exp.ToCharArray();
			string escap = "\'";
			if ("\'".Contains(expChars[0]))//判断字符串是不是以'单引号开头
			{
				for (int i = 1; ; i++)
				{
					escap += expChars[i];
					if ("\'".Contains(expChars[i])) break;//找到下一个单引号就退出
				}
				string newExp = exp.Substring(escap.Length);
				strMid= newExp.Split(seps, 2, StringSplitOptions.None);//分割字符串
				arr[0] = escap.Length+ strMid[0].Length;//操作数的长度
				arr[1] = exp.Length - arr[0] - strMid[1].Length;//操作符长度
			}
			else
			{
				strMid = exp.Split(seps,2, StringSplitOptions.None);
				arr[0] = strMid[0].Length;//操作数的长度
				arr[1] = exp.Length - arr[0] - strMid[1].Length;//操作符长度
				//arr[2] = exp.Length- strMid[1].Length;//光标位置
			}
			return arr;
		}
		/// <summary>
		/// 算法
		/// </summary>
		/// <param name="exp"></param>
		/// <returns></returns>
		private bool Parse(string exp)
		{
			m_tokens.Clear();//清空语法单元堆栈  
			if (exp.Trim() == "")//表达式不能为空  
			{
				return false;
			}
			else if (!this.IsMatching(exp))//括号、引号、单引号等必须配对  
			{
				return false;
			}

			Stack<object> operands = new Stack<object>();             //操作数堆栈  
			Stack<Operator> operators = new Stack<Operator>();      //运算符堆栈  
			OperatorType optType = OperatorType.ERR;                //运算符类型  
			string curOpd = "";                                 //当前操作数  
			string curOpt = "";                                 //当前运算符  
			int curPos = 0;                                     //当前位置  
			//int funcCount = 0;                                        //函数数量  
			//curPos = FindOperator(exp, "");//找到第一个操作符
			exp += "@"; //结束操作符  
			#region
			while (true)
			{
				//curPos = FindOperator(exp, "");//找到第一个操作符
				int[] pos= FindOperator(exp, m_Operators.ToArray());
				curPos = pos[0]+ pos[1];
				curOpd = exp.Substring(0, pos[0]).Trim(new char[] { '\'', ' ' });//找到操作符前面的操作数
				curOpt = exp.Substring(pos[0], pos[1]);//读取运算符
				//存储当前操作数到操作数堆栈  
				if (curOpd != "")
				{
					operands.Push(new Operand(curOpd, curOpd));
				}
				//若当前运算符为结束运算符，则停止循环  
				if (curOpt == "@")
				{
					break;
				}
				//若当前运算符为左括号,则直接存入堆栈。  
				if (curOpt == "(")
				{
					operators.Push(new Operator(OperatorType.LB, "("));
					exp = exp.Substring(curPos ).Trim();
					continue;
				}
				//若当前运算符为右括号,则依次弹出运算符堆栈中的运算符并存入到操作数堆栈,直到遇到左括号为止,此时抛弃该左括号.  
				if (curOpt == ")")
				{
					while (operators.Count > 0)
					{
						if (operators.Peek().Type != OperatorType.LB)
						{
							operands.Push(operators.Pop());
						}
						else
						{
							operators.Pop();
							break;
						}
					}
					exp = exp.Substring(curPos ).Trim();
					continue;
				}
				optType = Operator.ConvertOperator(curOpt);
				//若运算符堆栈为空,或者若运算符堆栈栈顶为左括号,则将当前运算符直接存入运算符堆栈.  
				if (operators.Count == 0 || operators.Peek().Type == OperatorType.LB)
				{
					operators.Push(new Operator(optType, curOpt));
					exp = exp.Substring(curPos).Trim();
					continue;
				}
				//若当前运算符优先级大于运算符栈顶的运算符,则将当前运算符直接存入运算符堆栈.  
				if (Operator.ComparePriority(optType, operators.Peek().Type) > 0)
				{
					operators.Push(new Operator(optType, curOpt));
				}
				else
				{
					//若当前运算符若比运算符堆栈栈顶的运算符优先级低或相等，则输出栈顶运算符到操作数堆栈，直至运算符栈栈顶运算符低于（不包括等于）该运算符优先级，  
					//或运算符栈栈顶运算符为左括号  
					//并将当前运算符压入运算符堆栈。  
					while (operators.Count > 0)
					{
						if (Operator.ComparePriority(optType, operators.Peek().Type) <= 0 && operators.Peek().Type != OperatorType.LB)
						{
							operands.Push(operators.Pop());

							if (operators.Count == 0)
							{
								operators.Push(new Operator(optType, curOpt));
								break;
							}
						}
						else
						{
							operators.Push(new Operator(optType, curOpt));
							break;
						}
					}
				}
				exp = exp.Substring(curPos).Trim();//把处理过的字符串截取掉
			}
			#endregion
			//转换完成,若运算符堆栈中尚有运算符时,  
			//则依序取出运算符到操作数堆栈,直到运算符堆栈为空  
			while (operators.Count > 0)
			{
				operands.Push(operators.Pop());
			}
			//调整操作数栈中对象的顺序并输出到最终栈  
			while (operands.Count > 0)
			{
				m_tokens.Push(operands.Pop());
			}
			return true;
		}
		/// <summary>
		/// 计算
		/// </summary>
		/// <returns>计算结果或者null</returns>
		public  object Evaluate(string tmpExp)
		{
            /* 
              逆波兰表达式求值算法： 
              1、循环扫描语法单元的项目。 
              2、如果扫描的项目是操作数，则将其压入操作数堆栈，并扫描下一个项目。 
              3、如果扫描的项目是一个二元运算符，则对栈的顶上两个操作数执行该运算。 
              4、如果扫描的项目是一个一元运算符，则对栈的最顶上操作数执行该运算。 
              5、将运算结果重新压入堆栈。 
              6、重复步骤2-5，堆栈中即为结果值。 
            */
            if (!Parse(tmpExp)) return null;
            if (m_tokens.Count == 0) return null;
			object value = null;
			Stack<Operand> opds = new Stack<Operand>();
			Stack<object> pars = new Stack<object>();
			Operand opdA, opdB,opdC,opdD;
			foreach (object item in m_tokens)
			{
				if (item is Operand)
				{
					//TODO 解析公式，替换参数  
					//如果为操作数则压入操作数堆栈  
					opds.Push((Operand)item);
				}
				else
				{
					switch (((Operator)item).Type)
					{
						#region 乘,*,multiplication  
						case OperatorType.MUL:
							opdA = opds.Pop();
							opdB = opds.Pop();
							if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
							{
								opds.Push(new Operand(OperandType.NUMBER, double.Parse(opdB.Value.ToString()) * double.Parse(opdA.Value.ToString())));
							}
							else
							{
								throw new Exception("乘运算的两个操作数必须均为数字");
							}
							break;
						#endregion

						#region 除,/,division  
						case OperatorType.DIV:
							opdA = opds.Pop();
							opdB = opds.Pop();
							if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
							{
								opds.Push(new Operand(OperandType.NUMBER, double.Parse(opdB.Value.ToString()) / double.Parse(opdA.Value.ToString())));
							}
							else
							{
								throw new Exception("除运算的两个操作数必须均为数字");
							}
							break;
						#endregion

						#region 余,%,modulus  
						case OperatorType.MOD:
							opdA = opds.Pop();
							opdB = opds.Pop();
							if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
							{
								opds.Push(new Operand(OperandType.NUMBER, double.Parse(opdB.Value.ToString()) % double.Parse(opdA.Value.ToString())));
							}
							else
							{
								throw new Exception("余运算的两个操作数必须均为数字");
							}
							break;
						#endregion

						#region 加,+,Addition  
						case OperatorType.ADD:
							opdA = opds.Pop();
							opdB = opds.Pop();
							if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
							{
								opds.Push(new Operand(OperandType.NUMBER, double.Parse(opdB.Value.ToString()) + double.Parse(opdA.Value.ToString())));
							}
							else
							{
								throw new Exception("加运算的两个操作数必须均为数字");
							}
							break;
						#endregion

						#region 减,-,subtraction  
						case OperatorType.SUB:
							opdA = opds.Pop();
							opdB = opds.Pop();
							if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
							{
								opds.Push(new Operand(OperandType.NUMBER, double.Parse(opdB.Value.ToString()) - double.Parse(opdA.Value.ToString())));
							}
							else
							{
								throw new Exception("减运算的两个操作数必须均为数字");
							}
							break;
						#endregion

						#region 正切,tan,subtraction  
						case OperatorType.TAN:
							opdA = opds.Pop();
							if (Operand.IsNumber(opdA.Value))
							{
								opds.Push(new Operand(OperandType.NUMBER, Math.Tan(double.Parse(opdA.Value.ToString()) * Math.PI / 180)));
							}
							else
							{
								throw new Exception("正切运算的1个操作数必须均为角度数字");
							}
							break;
						#endregion

						#region 反正切,atan,subtraction  
						case OperatorType.ATAN:
							opdA = opds.Pop();
							if (Operand.IsNumber(opdA.Value))
							{
								opds.Push(new Operand(OperandType.NUMBER, Math.Atan(double.Parse(opdA.Value.ToString()))));
							}
							else
							{
								throw new Exception("反正切运算的1个操作数必须均为数字");
							}
							break;
						#endregion

						#region max 最大值
						case OperatorType.MAX:
							opdA = opds.Pop();
							opdB = opds.Pop();
							if (Operand.IsNumber(opdA.Value)&& Operand.IsNumber(opdB.Value))
							{
								opds.Push(new Operand(OperandType.NUMBER, Math.Max(decimal.Parse(opdA.Value.ToString()), decimal.Parse(opdB.Value.ToString()))));
							}
							else
							{
								throw new Exception("最大值比较两个参数必须为数字！");
							}
							break;
						#endregion

						#region min 最小值
						case OperatorType.MIN:
							opdA = opds.Pop();
							opdB = opds.Pop();
							if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
							{
								opds.Push(new Operand(OperandType.NUMBER, Math.Min(decimal.Parse(opdA.Value.ToString()), decimal.Parse(opdB.Value.ToString()))));
							}
							else
							{
								throw new Exception("最小值比较两个参数必须为数字！");
							}
							break;
						#endregion

						#region avg 平均值
						case OperatorType.AVG:
							opdA = opds.Pop();
							opdB = opds.Pop();
							if (Operand.IsNumber(opdA.Value) && Operand.IsNumber(opdB.Value))
							{
								//decimal sum = ;
								opds.Push(new Operand(OperandType.NUMBER, (decimal.Parse(opdA.Value.ToString()) + decimal.Parse(opdB.Value.ToString())) / 2));
							}
							else
							{
								throw new Exception("平均值计算的两个参数必须为数字！");
							}
							break;
						#endregion

						#region his 自定义
						case OperatorType.HIS:
							opdA = opds.Pop();
							opdB = opds.Pop();
							try
							{
                                opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.GetHistoryData(opdB.Value.ToString().Trim(new char[] { '\'',' '}), opdA.Value.ToString().Trim(new char[] { '\'', ' ' }))));
							}
							catch//(Exception ex)
							{
								opds.Push(new Operand(OperandType.NUMBER, 0.0));
								//throw ex;
							}
							break;
						#endregion

						#region hissum 自定义
						case OperatorType.HOS:
							opdA = opds.Pop();
							opdB = opds.Pop();
							//opdC = opds.Pop();
							try
							{
								opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.GetHistoryDataSum(opdB.Value.ToString(),int.Parse(opdA.Value.ToString()))));
								//opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.GetHistoryData(opdB.Value.ToString().Trim(new char[] { '\'', ' ' }), opdA.Value.ToString().Trim(new char[] { '\'', ' ' }))));
							}
							catch//(Exception ex)
							{
								opds.Push(new Operand(OperandType.NUMBER, 0.0));
								//throw ex;
							}
							break;
						#endregion

						#region cell值 自定义
						case OperatorType.CELL:
							opdA = opds.Pop();
							//opdB = opds.Pop();
							//opdC = opds.Pop();
							try
							{
								opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.readCellValue(opdA.Value.ToString())));
								//opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.GetHistoryData(opdB.Value.ToString().Trim(new char[] { '\'', ' ' }), opdA.Value.ToString().Trim(new char[] { '\'', ' ' }))));
							}
							catch//(Exception ex)
							{
								opds.Push(new Operand(OperandType.NUMBER, 0.0));
								//throw ex;
							}
							break;
						#endregion

						#region 大于 自定义
						case OperatorType.GT:
							opdA = opds.Pop();
							opdB = opds.Pop();
							try
							{
								//opds.Push(new Operand(OperandType.BOOLEAN, double.Parse(opdA.Value.ToString()) >= double.Parse(opdB.Value.ToString()) ? true : false));
								opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.Compare( double.Parse(opdA.Value.ToString()), double.Parse(opdB.Value.ToString()))));
								//opds.Push(new Operand(OperandType.NUMBER, ()=> { }));
								//opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.GetHistoryData(opdB.Value.ToString().Trim(new char[] { '\'', ' ' }), opdA.Value.ToString().Trim(new char[] { '\'', ' ' }))));
							}
							catch(Exception ex)
							{
								opds.Push(new Operand(OperandType.NUMBER, -1));
								throw ex;
							}
							break;
						#endregion

						#region if语句 自定义
						case OperatorType.IF:
							opdA = opds.Pop();
							opdB = opds.Pop();
							opdC = opds.Pop();
							opdD= opds.Pop();
							try
							{
								opds.Push(new Operand(OperandType.STRING, SelfFunctionUtil.ifTest((int)opdD.Value, opdC.Value.ToString(),opdB.Value.ToString(),opdA.Value.ToString())));
								//opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.GetHistoryData(opdB.Value.ToString().Trim(new char[] { '\'', ' ' }), opdA.Value.ToString().Trim(new char[] { '\'', ' ' }))));
							}
							catch//(Exception ex)
							{
								opds.Push(new Operand(OperandType.STRING,"0"));
								//throw ex;
							}
							break;
						#endregion

						#region hismax 自定义
						case OperatorType.HISMAX:
							opdA = opds.Pop();
							try
							{
								opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.MaxOfHistory(opdA.Value.ToString())));
								//opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.GetHistoryData(opdB.Value.ToString().Trim(new char[] { '\'', ' ' }), opdA.Value.ToString().Trim(new char[] { '\'', ' ' }))));
							}
							catch//(Exception ex)
							{
								opds.Push(new Operand(OperandType.NUMBER, "0"));
								//throw ex;
							}
							break;
						#endregion

						#region hismin 自定义
						case OperatorType.HISMIN:
							opdA = opds.Pop();
							try
							{
								opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.MinOfHistory(opdA.Value.ToString())));
								//opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.GetHistoryData(opdB.Value.ToString().Trim(new char[] { '\'', ' ' }), opdA.Value.ToString().Trim(new char[] { '\'', ' ' }))));
							}
							catch//(Exception ex)
							{
								opds.Push(new Operand(OperandType.NUMBER, "0"));
								//throw ex;
							}
							break;
						#endregion

						#region hisavg 自定义
						case OperatorType.HISAVG:
							opdA = opds.Pop();
							try
							{
								opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.avgOfHistory(opdA.Value.ToString())));
								//opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.GetHistoryData(opdB.Value.ToString().Trim(new char[] { '\'', ' ' }), opdA.Value.ToString().Trim(new char[] { '\'', ' ' }))));
							}
							catch//(Exception ex)
							{
								opds.Push(new Operand(OperandType.NUMBER, "0"));
								//throw ex;
							}
							break;
						#endregion

						#region hislast 自定义
						case OperatorType.HISLAST:
							opdA = opds.Pop();
							try
							{
								opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.lastOfHistory(opdA.Value.ToString())));
								//opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.GetHistoryData(opdB.Value.ToString().Trim(new char[] { '\'', ' ' }), opdA.Value.ToString().Trim(new char[] { '\'', ' ' }))));
							}
							catch//(Exception ex)
							{
								opds.Push(new Operand(OperandType.NUMBER, "0"));
								//throw ex;
							}
							break;
						#endregion

						#region hisfirst 自定义
						case OperatorType.HISFIRST:
							opdA = opds.Pop();
							try
							{
								opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.firstOfHistory(opdA.Value.ToString())));
								//opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.GetHistoryData(opdB.Value.ToString().Trim(new char[] { '\'', ' ' }), opdA.Value.ToString().Trim(new char[] { '\'', ' ' }))));
							}
							catch//(Exception ex)
							{
								opds.Push(new Operand(OperandType.NUMBER, "0"));
								//throw ex;
							}
							break;
						#endregion

						#region his 自定义
						case OperatorType.HISSLOPPY:
							opdA = opds.Pop();
							opdB = opds.Pop();
							try
							{
								opds.Push(new Operand(OperandType.NUMBER, SelfFunctionUtil.GetHistorySloppyData(opdB.Value.ToString().Trim(new char[] { '\'', ' ' }), opdA.Value.ToString().Trim(new char[] { '\'', ' ' }))));
							}
							catch//(Exception ex)
							{
								opds.Push(new Operand(OperandType.NUMBER, 0.0));
								//throw ex;
							}
							break;
							#endregion

					}
				}
			}
			if (opds.Count == 1)
			{
				value = opds.Pop().Value;
			}
			return value;//如果公式有错误会返回null
		}
	}
}

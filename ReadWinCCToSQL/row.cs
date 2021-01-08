using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadWinCCToSQL
{
	internal class row
	{
		public int ID { get; set; }
		public string Time { get; set; }
		public string TagName { get; set; }
		public double Value { get; set; }
		public string InsertTime { get; set; }
	}
}

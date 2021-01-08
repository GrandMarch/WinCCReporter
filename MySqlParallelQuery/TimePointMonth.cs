using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlParallelQuery
{
	internal class TimePointMonth
	{
		public string TableName { get; set; }
		public List<DateTime> TimePoint { get; set; }
		public TimePointMonth()
		{
			TimePoint = new List<DateTime> { };
		}
	}
}

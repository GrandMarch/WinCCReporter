using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCCReportControl
{
	internal class RangePosition
	{
		public string TopLeft { get; set; }
		public string BottomRight { get; set; }
		public RangePosition()
		{
			TopLeft = "A1";
			BottomRight = "A1";
		}
		public RangePosition(string topLeft,string bottomRight)
		{
			TopLeft = topLeft;
			BottomRight = bottomRight;
		}
	}
}

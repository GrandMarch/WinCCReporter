using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MySqlParallelQuery
{
   public  interface  IMySqlQuery
    {
		#region 属性
		string ConnectionString { get; set; }
		string QueryString { get; set; }
		DataTable dataTable { get;  }
		#endregion
		#region 方法
		void Excute();
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Layer01_Common.Common
{
    public static class Global_Variables
    {
		public static Global_Database gDatabase;
        public static string gConnection_Server, gConnection_Database, gConnection_Username, gConnection_Password;
        public static string gConnection_SqlServerConnectionString;
        public static string gConnection_SharePoint_Server, gConnection_SharePoint_UserName, gConnection_SharePoint_Password;
        
    }

	public class Global_Database
	{
		public string pConnectionString { get; set; }		
	}
		
}

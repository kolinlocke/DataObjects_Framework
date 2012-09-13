using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using DataObjects_Framework;
using DataObjects_Framework.Common;

namespace DataObjects_Framework.Connection
{
    public class ClsConnection_Wcf : Interface_Connection
    {
        String mConnectionString = "";

        public Boolean Connect()
        {
            this.mConnectionString = Do_Globals.gSettings.pConnectionString;
			return true;
        }

        public Boolean Connect(String ConnectionString)
        {
            this.mConnectionString = ConnectionString;
			return true;
        }

		public DbConnection pConnection
		{
			get { throw new NotImplementedException(); }
		}

		public DbTransaction pTransaction
        {
            get { throw new NotImplementedException(); }
        }

		public DbCommand CreateCommand()
		{
			throw new NotImplementedException();
		}

        public DbParameter CreateParameter()
        {
            throw new NotImplementedException();
        }

        public string pConnectionString 
        {
            get { return this.mConnectionString; }
        }

        public void Dispose() { }
    }
}

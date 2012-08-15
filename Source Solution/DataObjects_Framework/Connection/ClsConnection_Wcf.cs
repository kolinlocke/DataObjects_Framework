using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjects_Framework;
using DataObjects_Framework.Common;

namespace DataObjects_Framework.Connection
{
    public class ClsConnection_Wcf : Interface_Connection
    {
        public void Connect()
        {
            this.pConnectionString = Do_Globals.gSettings.pConnectionString;
        }

        public void Connect(String ConnectionString)
        {
            this.pConnectionString = ConnectionString;
        }

        public object pConnection
        {
            get { throw new NotImplementedException(); }
        }

        public System.Data.IDbTransaction pTransaction
        {
            get { throw new NotImplementedException(); }
        }

        public string pConnectionString { get; set; }
    }
}

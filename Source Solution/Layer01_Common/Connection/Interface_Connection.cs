using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Layer01_Common.Connection
{
    public interface Interface_Connection 
    {
        Object pConnection { get; }

        System.Data.IDbTransaction pTransaction { get; }
        
    }
}

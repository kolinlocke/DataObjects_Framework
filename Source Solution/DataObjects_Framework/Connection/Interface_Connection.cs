using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataObjects_Framework.Connection
{
    /// <summary>
    /// Interface for the connection object used
    /// </summary>
    public interface Interface_Connection : IDisposable
    {
        /// <summary>
        /// The connection object
        /// </summary>
        DbConnection pConnection { get; }

        /// <summary>
        /// The transaction object used by this connection
        /// </summary>
        DbTransaction pTransaction { get; }

        String pConnectionString { get; }

		Boolean Connect();

		Boolean Connect(String ConnectionString);

		DbCommand CreateCommand();

        DbParameter CreateParameter();
    }
}

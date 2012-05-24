using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DataObjects_Framework.Connection
{
    /// <summary>
    /// Interface for the connection object used
    /// </summary>
    public interface Interface_Connection 
    {
        /// <summary>
        /// The connection object
        /// </summary>
        Object pConnection { get; }

        /// <summary>
        /// The transaction object used by this connection
        /// </summary>
        System.Data.IDbTransaction pTransaction { get; }
        
    }
}

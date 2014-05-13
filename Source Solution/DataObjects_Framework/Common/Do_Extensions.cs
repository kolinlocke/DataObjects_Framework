using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjects_Framework.Objects;

namespace DataObjects_Framework.Common
{
    /// <summary>
    /// Extension methods are declared here
    /// </summary>
    public static class Do_Extensions
    {
        /// <summary>
        /// Extends List[QueryParameter], added GetParameter for quick access
        /// </summary>
        /// <param name="List">
        /// The object of the type it extends
        /// </param>
        /// <param name="Name">
        /// The Name Property of QueryParameter
        /// </param>
        /// <returns>
        /// Returns the QueryParameter found. Returns null if not found.
        /// </returns>
        public static QueryParameter GetParameter(this List<QueryParameter> List, String Name)
        {
            return List.FirstOrDefault(O => O.Name == Name);
        }
    }
}

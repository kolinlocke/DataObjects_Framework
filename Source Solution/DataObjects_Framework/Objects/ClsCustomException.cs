using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DataObjects_Framework.Objects
{
    /// <summary>
    /// A custom exception object
    /// </summary>
    public class ClsCustomException : System.Exception
    {
        #region _Constructor

        /// <summary>
        /// Constructor for ClsCustomException
        /// </summary>
        public ClsCustomException() : base() { }

        /// <summary>
        /// Constructor for ClsCustomException
        /// </summary>
        /// <param name="Message">
        /// The error message that explains the reason for the exception.
        /// </param>
        public ClsCustomException(string Message) : base(Message) { }

        /// <summary>
        /// Constructor for ClsCustomException
        /// </summary>
        /// <param name="Message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="InnerException">
        /// The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified. - from System.Exception        
        /// </param>
        public ClsCustomException(string Message, Exception InnerException) : base(Message, InnerException) { }

        #endregion
    }

    
}

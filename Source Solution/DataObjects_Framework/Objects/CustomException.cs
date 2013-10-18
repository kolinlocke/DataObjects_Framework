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
    public class CustomException : System.Exception
    {
        #region _Constructor

        /// <summary>
        /// Constructor for CustomException
        /// </summary>
        public CustomException() : base() { }

        /// <summary>
        /// Constructor for CustomException
        /// </summary>
        /// <param name="Message">
        /// The error message that explains the reason for the exception.
        /// </param>
        public CustomException(string Message) : base(Message) { }

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
        public CustomException(string Message, Exception InnerException) : base(Message, InnerException) { }

        #endregion
    }

    
}

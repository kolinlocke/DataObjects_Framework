using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DataObjects_Framework.Objects
{
    public class ClsCustomException : System.Exception
    {
        /*
        private String cMessage = "";

        public ClsException() {
            new ClsException("SNAP! An error occurred. Contact your system administrator about this.");
        }

        public ClsException(String message) {
            this.cMessage = message;
        }

        public override string Message
        {
            get
            {
                return this.cMessage;
            }
        }
        */

        #region _Constructor

        public ClsCustomException() : base() { }

        public ClsCustomException(string Message) : base(Message) { }

        public ClsCustomException(string Message, Exception InnerException) : base(Message, InnerException) { }

        #endregion
    }

    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Configuration;

namespace DataObjects_Wcf
{
    public class ErrorHandlerBehavior : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(WcfService); }
        }

        protected override object CreateBehavior()
        {
            return new WcfService();
        }
    }
}
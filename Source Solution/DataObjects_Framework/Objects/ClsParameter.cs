using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjects_Framework.Common;

namespace DataObjects_Framework.Objects
{
    public class ClsParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public Int32 Size { get; set; }
        public Byte Scale { get; set; }
        public Byte Precision { get; set; }
        public Do_Constants.eParameterType Type { get; set; }

        public ClsParameter() { }

        public ClsParameter(String Name, Object Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }
}

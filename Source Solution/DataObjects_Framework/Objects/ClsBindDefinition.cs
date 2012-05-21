using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects_Framework.Objects
{
    [Serializable()]
    public class ClsBindDefinition
    {
        public List<ClsBindGridColumn> List_Gc;
        public string DataSourceName;
        public string KeyName;
        public bool AllowSort;
        public bool AllowPaging;
        public bool IsPersistent;
    }
}

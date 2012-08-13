using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataObjects_Framework;
using DataObjects_Framework.Common;
using DataObjects_Framework.Objects;

namespace DataObjects_Wcf
{
    public class Constants
    {
        [Serializable()]
        public struct Str_Request_List
        {
            public string ObjectName;
            public string Fields;
            public ClsQueryCondition Condition;
            public string Sort;
            public Int32 Top;
            public Int32 Page;
        }

        [Serializable()]
        public struct Str_Request_Load
        {
            public string ObjectName;
            public List<string> ObjectKeys;
            public ClsKeys Key;
            public string Condition;
            public List<Do_Constants.Str_ForeignKeyRelation> ForeignKeys;
        }

    }
}
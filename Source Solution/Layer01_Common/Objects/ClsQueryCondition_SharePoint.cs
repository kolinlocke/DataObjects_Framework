﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Layer01_Common;
using Layer01_Common.Common;
using Layer01_Common.Connection;
using Layer01_Common.Objects;

namespace Layer01_Common.Objects
{
    public class ClsQueryCondition_SharePoint : ClsQueryCondition
    {
        #region _Methods

        public override string GetQueryCondition()
        {
            StringBuilder Sb_QueryCondition = new StringBuilder();
            string QueryCondition_And = "";
            bool IsStart = false;
            foreach (Str_QueryCondition Obj in this.mQc)
            {
                Object Value = null;
                if (Obj.DataType.ToUpper() == typeof(DateTime).ToString().ToUpper()
                    || Obj.DataType.ToUpper() == typeof(DateTime).Name.ToUpper()
                    || Obj.DataType.ToUpper() == typeof(string).ToString().ToUpper()
                    || Obj.DataType.ToUpper() == typeof(string).Name.ToUpper())
                { Value = @"'" + Obj.Value.ToString() + @"'"; }
                else
                { Value = Obj.Value; }

                Sb_QueryCondition.Append(QueryCondition_And + @" [" + Obj.FieldName + "] " + Obj.Operator + " " + Value);

                if (!IsStart)
                {
                    QueryCondition_And = " And ";
                    IsStart = true;
                }                
            }
            return Sb_QueryCondition.ToString();
        }

        #endregion
    }
}

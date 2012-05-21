using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Layer01_Common;
using Layer01_Common.Common;
using Layer01_Common.Connection;
using Layer01_Common.Objects;
using Layer02_Objects;
using Layer02_Objects._System;
using Layer02_Objects.Modules_Base;
using Layer02_Objects.Modules_Base.Abstract;

namespace Layer02_Objects.Modules_Objects
{
    public class ClsRights : ClsBase
    {
        #region _Constructor

        public ClsRights(ClsSysCurrentUser pCurrentUser = null)
        { 
            this.Setup(null, "RecruitmentTestRights");
            this.Add_TableDetail("RecruitmentTestRights_Details", "", "1 = 0");
        }

        #endregion

        #region _Methods

        public override DataTable List(string Condition = "", string Sort = "")
        { return this.List((ClsQueryCondition)null, Sort); }

        public override DataTable List(ClsQueryCondition Condition, string Sort = "", int Top = 0, int Page = 0)
        {
            if (Condition == null)
            { Condition = new Layer01_Common.Objects.ClsQueryCondition(); }

            Condition.Add("IsDeleted", "=", typeof(bool).ToString(), "0");
            
            return base.List(Condition, Sort, Top, Page);
        }

        public override long List_Count(ClsQueryCondition Condition = null)
        {
            if (Condition == null)
            { Condition = new ClsQueryCondition(); }
            Condition.Add("IsDeleted", "=", typeof(bool).ToString(), "0");
            return base.List_Count(Condition);
        }

        public override void Load(ClsKeys Keys = null)
        {
            base.Load(Keys);

            Int64 ID = 0;
            if (Keys != null)
            {
                try { ID = Keys["RecruitmentTestRightsID"]; }
                catch { }
            }

            List<Layer01_Constants.Str_Parameters> List_Sp = new List<Layer01_Constants.Str_Parameters>();
            List_Sp.Add(new Layer01_Constants.Str_Parameters("@ID", ID));
            DataTable Dt;
            Dt = (this.mDa.Connection as ClsConnection_SqlServer).ExecuteQuery("usp_RecruitmentTestRights_Details_Load", List_Sp).Tables[0];

            this.AddRequired(Dt);
            this.pTableDetail_Set("RecruitmentTestRights_Details", Dt);
        }

        #endregion

        #region _Properties

        public DataTable pDt_Details
        {
            get { return this.pTableDetail_Get("RecruitmentTestRights_Details"); }
        }

        #endregion
    }
}

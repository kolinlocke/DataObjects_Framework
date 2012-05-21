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
    public class ClsUser: ClsBase
    {
        #region _Constructor

        public ClsUser(ClsSysCurrentUser pCurrentUser = null)
        { 
            this.Setup(pCurrentUser, "RecruitmentTestUser", "uvw_RecruitmentTestUser");
            this.Add_TableDetail("RecruitmentTestUser_Rights", "", "1 = 0");

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
                try { ID = Keys["RecruitmentTestUserID"]; }
                catch { }
            }

            List<Layer01_Constants.Str_Parameters> List_Sp = new List<Layer01_Constants.Str_Parameters>();
            List_Sp.Add(new Layer01_Constants.Str_Parameters("@ID", ID));
            DataTable Dt;
			Dt = (this.mDa.Connection as ClsConnection_SqlServer).ExecuteQuery("usp_RecruitmentTestUser_Rights_Load", List_Sp).Tables[0];

            this.AddRequired(Dt);
            this.pTableDetail_Set("RecruitmentTestUser_Rights", Dt);
        }

        public string GeneratePassword(Int32 Length)
        {
            StringBuilder Sb = new StringBuilder();
            Random R_Type = new Random();
            Random R_Ch = new Random();
            Random R_Nm = new Random();
            for (Int32 Ct = 0; Ct < Length; Ct++)
            {
                if (R_Type.Next(2) == 0)
                { Sb.Append(Convert.ToChar(R_Ch.Next(26) + 65)); }
                else
                { Sb.Append(R_Nm.Next(10)); }
            }
            return Sb.ToString(); ;
        }

        #endregion

        #region _Properties

        public DataTable pDt_Rights
        {
            get { return this.pTableDetail_Get("RecruitmentTestUser_Rights"); }
        }

        #endregion
    }
}
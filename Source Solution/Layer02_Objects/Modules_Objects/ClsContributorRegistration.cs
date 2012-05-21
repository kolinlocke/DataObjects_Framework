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
    public class ClsContributorRegistration : ClsBase
    {
        #region _Variables

        DataTable mDt_List;

        #endregion
        
        #region _Constructor

        public ClsContributorRegistration(ClsSysCurrentUser pCurrentUser = null)
        { this.Setup(pCurrentUser, "RecruitmentTestContributorRegistrationRequest"); }

        #endregion

        #region _Methods

        public override DataTable List(ClsQueryCondition Condition, string Sort = "", int Top = 0, int Page = 0)
        {
            if (Condition == null)
            { Condition = this.mDa.CreateQueryCondition(); }
            Condition.Add("IsApproved", "0", typeof(bool).ToString(), "0");

            this.mDt_List = base.List(Condition, Sort, Top, Page );
            this.mDt_List.Columns.Add("IsSelected", typeof(bool));
            this.AddRequired(this.mDt_List);
            return this.mDt_List;
        }

        public override DataTable List(string Condition = "", string Sort = "")
        { return this.List((ClsQueryCondition)null, Sort); }

        public override long List_Count(ClsQueryCondition Condition = null)
        {
            if (Condition == null)
            { Condition = this.mDa.CreateQueryCondition(); }
            Condition.Add("IsApproved", "0", typeof(bool).ToString(), "0");
            
            return base.List_Count(Condition);
        }

        public override bool  Save(DataAccess.Interface_DataAccess Da = null)
        {
            DateTime ServerDate = DateTime.Now;

            if (this.pID == 0)
            { this.mHeader_Dr["DateRequested"] = ServerDate; }

            return base.Save(Da);
        }

        public ClsBaseObjs Approve()
        {
            ClsBaseObjs Rv = new ClsBaseObjs();
            
            DateTime ServerDate = DateTime.Now;
            this.mDa.Connect();
            this.mDa.BeginTransaction();
            
            try
            {
                string[] ArrRightsID = Layer01_Methods.Convert_String(this.mDa.GetSystemParameter(this.mDa.Connection, Layer02_Constants.CnsExam_DefaultContributor_RightsIDs)).Split(',');

                DataRow[] ArrDr = this.mDt_List.Select("IsSelected = 1", "", DataViewRowState.CurrentRows);
                foreach (DataRow Dr in ArrDr)
                {
                    ClsUser Obj_User = new ClsUser();
                    Obj_User.Load();
                    Obj_User.pDr["Name"] = Dr["Name"];
                    Obj_User.pDr["Password"] = Obj_User.GeneratePassword(6);
                    Obj_User.pDr["Email"] = Dr["Email"];

                    foreach (string RightsID in ArrRightsID)
                    {
                        DataRow Dr_New = Obj_User.pDt_Rights.NewRow();
                        Dr_New["RecruitmentTestRightsID"] = Layer01_Methods.Convert_Int64(RightsID);
                        Dr_New["IsActive"] = true;
                        Obj_User.pDt_Rights.Rows.Add(Dr_New);
                    }

                    Obj_User.Save(this.mDa);
                    Dr["RecruitmentTestUserID"] = Obj_User.pID;
                    Rv.Add(Obj_User.pID.ToString(), Obj_User);

                    Dr["IsApproved"] = true;
                    Dr["DateApproved"] = ServerDate;
                    Dr["RecruitmentTestUserID_ApprovedBy"] = this.mCurrentUser.pDrUser["RecruitmentTestUserID"];
                    
                    this.mDa.SaveDataRow(Dr, this.mHeader_TableName);
                }
                this.mDa.CommitTransaction();
            }
            catch (Exception Ex)
            {
                this.mDa.RollbackTransaction();
                throw Ex;
            }
            finally
            { this.mDa.Close(); }

            return Rv;
        }

        #endregion

        #region _Properties

        public DataTable pDt_List
        {
            get { return this.mDt_List; }
        }

        #endregion
    }
}

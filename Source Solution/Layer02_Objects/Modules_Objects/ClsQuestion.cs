using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Layer01_Common;
using Layer01_Common.Common;
using Layer01_Common.Objects;
using Layer02_Objects;
using Layer02_Objects._System;
using Layer02_Objects.Modules_Base;
using Layer02_Objects.Modules_Base.Abstract;
using Layer02_Objects.Modules_Objects;

namespace Layer02_Objects.Modules_Objects
{
    public class ClsQuestion : ClsBase
    {
        #region _Variables

        ClsBaseObjs mBO_Answer = new ClsBaseObjs();
        ClsQuestionAnswer mBL_QuestionAnswer = new ClsQuestionAnswer();

        #endregion

        #region _Constructor

        public ClsQuestion(ClsSysCurrentUser pCurrentUser = null)
        {
            //this.Setup(null, "RecruitmentTestQuestions"); 
            this.Setup(pCurrentUser, "RecruitmentTestQuestions", "uvw_RecruitmentTestQuestions");
        }

        #endregion

        #region _Methods

        // Gets the whole table data.
        public override DataTable List(string Condition = "", string Sort = "")
        {
            return this.List((ClsQueryCondition)null);
        }

        // Gets the table data that meet certain conditions.
        //
        // @param Condition The condition object where filter parameters for the questions are set.
        // @param Sort The sort expression, or the column to which the table is sorted.
        // @param Top For paging, define the number of results returned for.
        // @param Page The selected index (page) if pagination is enforced.
        public override DataTable List(ClsQueryCondition Condition, string Sort = "", int Top = 0, int Page = 0)
        {
            if (Condition == null)
            { Condition = this.mDa.CreateQueryCondition(); }

            Condition.Add("IsDeleted", "= 0", typeof(bool).ToString(), "0");

            if (!this.mCurrentUser.CheckAccess(Layer02_Constants.eSystem_Modules.Question, Layer02_Constants.eAccessLib.eAccessLib_View))
            { Condition.Add("RecruitmentTestUserID_CreatedBy", "= " + this.mCurrentUser.pUserID, typeof(Int64).ToString(), "0"); }

            return base.List(Condition, Sort, Top, Page);
        }

        public override long List_Count(ClsQueryCondition Condition = null)
        {
            if (Condition == null)
            { Condition = this.mDa.CreateQueryCondition(); }

            Condition.Add("IsDeleted", "= 0", typeof(bool).ToString(), "0");

            if (!this.mCurrentUser.CheckAccess(Layer02_Constants.eSystem_Modules.Question, Layer02_Constants.eAccessLib.eAccessLib_View))
            { Condition.Add("RecruitmentTestUserID_CreatedBy", "= " + this.mCurrentUser.pUserID, typeof(Int64).ToString(), "0"); }

            return base.List_Count(Condition);
        }

        public override void Load(ClsKeys Keys = null)
        {
            base.Load(Keys);
            this.CheckIfDeleted();

            //[-]

            Int64 QuestionID = Convert.ToInt64(Layer01_Methods.IsNull(this.pDr["RecruitmentTestQuestionsID"], 0));
            if (QuestionID != 0)
            {
                Keys = new ClsKeys();
                Keys.Add("Lkp_RecruitmentTestQuestionsID", QuestionID);
            }
            else
            { Keys = null; }

            this.mBL_QuestionAnswer.Load(Keys);

            foreach (DataRow Dr in this.mBL_QuestionAnswer.pDt_List.Rows)
            {
                ClsAnswer Inner_Obj = new ClsAnswer();
                ClsKeys Inner_Keys = null;
                Int64 Inner_ID = Convert.ToInt64(Layer01_Methods.IsNull(Dr["Lkp_RecruitmentTestAnswersID"], 0));

                if (Inner_ID != 0)
                {
                    Inner_Keys = new ClsKeys();
                    Inner_Keys.Add("RecruitmentTestAnswersID", Inner_ID);
                }
                
                Inner_Obj.Load(Inner_Keys);
                this.mBO_Answer.Add(Convert.ToInt64(Layer01_Methods.IsNull(Dr["TmpKey"], 0)).ToString(), Inner_Obj);
            }

            DataRow[] ArrDr = this.mBL_QuestionAnswer.pDt_List.Select("", "OrderIndex");
            int Ct = 0;
            foreach (DataRow Dr in ArrDr)
            {
                Ct++;
                Dr["OrderIndex"] = Ct;
            }

            this.FixOrderIndex(true);
            this.mBL_QuestionAnswer.pDt_List.DefaultView.Sort = "OrderIndex";
        }

        public override bool Save(DataAccess.Interface_DataAccess Da = null)
        { return this.Save(false); }

        // Commit question changes.
        //
        // @param IsApprove By default the approval will be in the application's control. Otherwise, approval must be explicitly done.
        public bool Save(bool IsApprove = false, DataAccess.Interface_DataAccess Da = null)
        {
            DateTime ServerDate = DateTime.Now;

            if (IsApprove)
            {
                this.mHeader_Dr["IsApproved"] = true;
                this.mHeader_Dr["DateApproved"] = ServerDate;
                this.mHeader_Dr["RecruitmentTestUserID_ApprovedBy"] = this.mCurrentUser.pDrUser["RecruitmentTestUserID"];
            }
            else
            {
                this.mHeader_Dr["IsApproved"] = DBNull.Value;
                this.mHeader_Dr["DateApproved"] = DBNull.Value;
                this.mHeader_Dr["RecruitmentTestUserID_ApprovedBy"] = DBNull.Value;
            }

            if (this.pID == 0)
            {
                this.mHeader_Dr["DateCreated"] = ServerDate;
                this.mHeader_Dr["RecruitmentTestUserID_CreatedBy"] = this.mCurrentUser.pDrUser["RecruitmentTestUserID"];
            }

            this.mHeader_Dr["DateUpdated"] = ServerDate;
            this.mHeader_Dr["RecruitmentTestUserID_UpdatedBy"] = this.mCurrentUser.pDrUser["RecruitmentTestUserID"];

            bool IsSave = false;
            IsSave = base.Save(Da);

            DataRow[] ArrDr = this.mBL_QuestionAnswer.pDt_List.Select("", "", DataViewRowState.CurrentRows);
            foreach (DataRow Inner_Dr in ArrDr)
            {
                ClsBase Obj = this.mBO_Answer[Layer01_Methods.Convert_Int64(Inner_Dr["TmpKey"]).ToString()];
                if (Obj != null)
                {
                    Obj.Save();
                    Inner_Dr["Lkp_RecruitmentTestAnswersID"] = Obj.pDr["RecruitmentTestAnswersID"];
                    Inner_Dr["Lkp_RecruitmentTestQuestionsID"] = this.pDr["RecruitmentTestQuestionsID"];
                }
            }

            ArrDr = this.mBL_QuestionAnswer.pDt_List.Select("", "", DataViewRowState.Deleted);
            foreach (DataRow Inner_Dr in ArrDr)
            {
                ClsBase Obj = this.mBO_Answer[Layer01_Methods.Convert_Int64(Inner_Dr["TmpKey", DataRowVersion.Original]).ToString()];
                if (Obj != null)
                {
                    if (!Microsoft.VisualBasic.Information.IsDBNull(Obj.pDr[Obj.pHeader_TableKey]))
                    { Obj.Delete(); }
                }
            }

            IsSave = this.mBL_QuestionAnswer.Save();

            return IsSave;
        }

        public void FixOrderIndex(bool IsSetup = false)
        {
            DataRow[] ArrDr = this.mBL_QuestionAnswer.pDt_List.Select("", "OrderIndex", DataViewRowState.CurrentRows);
            int Ct = 0;
            foreach (DataRow Dr in ArrDr)
            {
                Ct++;
                Dr["OrderIndex"] = Ct;
            }
            if (IsSetup)
            { this.mBL_QuestionAnswer.pDt_List.AcceptChanges(); }
        }

        #endregion

        #region _Properties

        public DataTable pDt_QuestionAnswer
        {
            get
            { 
                return this.mBL_QuestionAnswer.pDt_List; 
            }
        }

        public ClsBaseObjs pBO_Answer
        {
            get
            { 
                return this.mBO_Answer; 
            }
        }

        #endregion
    }
}
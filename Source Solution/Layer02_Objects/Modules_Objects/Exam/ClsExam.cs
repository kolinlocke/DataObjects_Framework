using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Layer01_Common;
using Layer01_Common.Common;
using Layer01_Common.Connection;
using Layer02_Objects;
using Layer02_Objects._System;
using Layer02_Objects.DataAccess;
using Layer02_Objects.Modules_Base;
using Layer02_Objects.Modules_Base.Abstract;

namespace Layer02_Objects.Modules_Objects.Exam
{
    [Serializable()]
    public class ClsExam
    {
        #region _Variables

        ClsApplicant mObj_Applicant;
        DataSet mDs;
        DataTable mDt_Question;
        DataTable mDt_Question_Answer;
        Int64 mItems;
        Int64 mPages;
        Int64 mItemsLimit;
        DateTime mDateStart;
        Int64 mCategoryID;

        DataRow mDr_Exam;

        Interface_ExamMethods mEm = new ClsExamMethods_SqlServer();

        #endregion

        #region _Constructor

        public ClsExam() { }

        #endregion

        #region _Methods
        
        public void GenerateExam(Int64 QuestionLimit, Int64 ItemsLimit, ClsApplicant Obj_Applicant, Int64 CategoryID)
        {
            this.mObj_Applicant = Obj_Applicant;

            DataSet Ds = this.mEm.GenerateExam(QuestionLimit, CategoryID);
            this.mDs = Ds;
            this.mDt_Question = Ds.Tables[0];
            this.mDt_Question_Answer = Ds.Tables[1];

            this.mDt_Question.Columns.Add("Ct", typeof(Int64));
            this.mDt_Question.Columns.Add("IsCorrect", typeof(bool));
            Int64 Ct = 0;
            foreach (DataRow Dr in this.mDt_Question.Rows)
            {
                Ct++;
                Dr["Ct"] = Ct;
            }

            this.mDt_Question_Answer.Columns.Add("Ct", typeof(Int64));
            this.mDt_Question_Answer.Columns.Add("IsAnswered", typeof(bool));
            Ct = 0;
            foreach (DataRow Dr in this.mDt_Question_Answer.Rows)
            {
                Ct++;
                Dr["Ct"] = Ct;
            }

            //[-]

            this.mItemsLimit = ItemsLimit;
            this.mItems = this.mDt_Question.Rows.Count;

            this.mPages = this.mItems / this.mItemsLimit;
            if (this.mItems % this.mItemsLimit > 0)
            { this.mPages++; }

            this.mCategoryID = CategoryID;

            //[-]

            this.mDateStart = DateTime.Now;
        }

        public void LoadExam(Int64 ExamID, Int64 ItemsLimit)
        {
            ClsBase Base = new ClsBase();
            Interface_DataAccess Da = Base.pDa;

            DataTable Dt_Exam = Da.GetQuery("RecruitmentTestExams", "", "RecruitmentTestExamsID = " + ExamID);
            Int64 ApplicantID = 0;
            if (Dt_Exam.Rows.Count > 0)
            { 
                this.mDr_Exam = Dt_Exam.Rows[0];
                ApplicantID = Convert.ToInt64(Layer01_Methods.IsNull(this.mDr_Exam["RecruitmentTestApplicantID"], 0));
            }
            else
            { throw new Exception("Exam Data not found."); }

            ClsKeys Key = new ClsKeys();
            Key.Add("RecruitmentTestApplicantID", ApplicantID);

            this.mObj_Applicant = new ClsApplicant();
            this.mObj_Applicant.Load(Key);

            DataSet Ds = this.mEm.LoadExam(ExamID);
            this.mDs = Ds;
            this.mDt_Question = Ds.Tables[0];
            this.mDt_Question_Answer = Ds.Tables[1];

            this.mDt_Question.Columns.Add("Ct", typeof(Int64));
            Int64 Ct = 0;
            foreach (DataRow Dr in this.mDt_Question.Rows)
            {
                Ct++;
                Dr["Ct"] = Ct;
            }

            this.mDt_Question_Answer.Columns.Add("Ct", typeof(Int64));
            Ct = 0;
            foreach (DataRow Dr in this.mDt_Question_Answer.Rows)
            {
                Ct++;
                Dr["Ct"] = Ct;
            }

            this.mItemsLimit = ItemsLimit;
            this.mItems = this.mDt_Question.Rows.Count;

            this.mPages = this.mItems / this.mItemsLimit;
            if (this.mItems % this.mItemsLimit > 0)
            { this.mPages++; }
        }

        public List<ClsExam_Questions> Get_Questions(Int64 Page)
        {
            if (Page == 0)
            { Page = 1; }

            Int64 RowStart = ((Page - 1) * this.mItemsLimit) + 1;
            Int64 RowEnd = RowStart + (this.mItemsLimit - 1);

            List<ClsExam_Questions> List_Questions = new List<ClsExam_Questions>();

            DataRow[] Arr_Dr = this.mDt_Question.Select(@"Ct >= " +  RowStart + @" And Ct <= " + RowEnd);
            foreach (DataRow Dr in Arr_Dr)
            {
                DataRow[] Inner_Arr_Dr = this.mDt_Question_Answer.Select(@"Lkp_RecruitmentTestQuestionsID = " + (Convert.ToInt64(Dr["RecruitmentTestQuestionsID"])).ToString());
                List_Questions.Add(new ClsExam_Questions(Dr, Inner_Arr_Dr));
            }

            return List_Questions;
        }

        public void Post()
        {
            ClsBase Obj_Base = new ClsBase();

            try
            {
                this.mObj_Applicant.Save();

                Obj_Base.pDa.Connect();
                Obj_Base.pDa.BeginTransaction();

                DataTable Dt_Exam = Obj_Base.pDa.List_Empty(Obj_Base.pDa.Connection, "RecruitmentTestExams");
                DataRow Dr_Exam = Dt_Exam.NewRow();
                this.mDr_Exam = Dr_Exam;
                Dr_Exam["RecruitmentTestApplicantID"] = this.mObj_Applicant.pDr["RecruitmentTestApplicantID"];
                Dr_Exam["LookupCategoryID"] = this.mCategoryID;
                Dr_Exam["DateTaken"] = DateTime.Now;
                Dr_Exam["DateStart"] = this.mDateStart;
                Dr_Exam["DateEnd"] = DateTime.Now;
                Dr_Exam["Score"] = this.mEm.ComputeScore(this.mDs);
                Dr_Exam["TotalItems"] = this.mDt_Question.Rows.Count;
                Obj_Base.pDa.SaveDataRow(Dr_Exam, "RecruitmentTestExams");

                DataTable Dt_Exams_Questions = Obj_Base.pDa.List_Empty(Obj_Base.pDa.Connection, "RecruitmentTestExams_Questions");
                foreach (DataRow Inner_Dr in this.mDt_Question.Rows)
                {
                    DataRow Dr_Exam_Question = Dt_Exams_Questions.NewRow();
                    Dr_Exam_Question["RecruitmentTestExamsID"] = Dr_Exam["RecruitmentTestExamsID"];
                    Dr_Exam_Question["Lkp_RecruitmentTestQuestionsID"] = Inner_Dr["RecruitmentTestQuestionsID"];
                    Obj_Base.pDa.SaveDataRow(Dr_Exam_Question, "RecruitmentTestExams_Questions");
                }

                DataTable Dt_Exams_Answers = Obj_Base.pDa.List_Empty(Obj_Base.pDa.Connection, "RecruitmentTestExams_Answers");
                DataRow[] Arr_Dr_Answer = this.mDt_Question_Answer.Select("ISNULL(IsAnswered,0) = 1");
                foreach (DataRow Inner_Dr in Arr_Dr_Answer)
                {
                    DataRow Dr_Exam_Answer = Dt_Exams_Answers.NewRow();
                    Dr_Exam_Answer["RecruitmentTestExamsID"] = Dr_Exam["RecruitmentTestExamsID"];
                    Dr_Exam_Answer["Lkp_RecruitmentTestQuestionsID"] = Inner_Dr["Lkp_RecruitmentTestQuestionsID"];
                    Dr_Exam_Answer["Lkp_RecruitmentTestAnswersID"] = Inner_Dr["Lkp_RecruitmentTestAnswersID"];
                    Dr_Exam_Answer["IsAnswer"] = Inner_Dr["IsAnswered"];
                    Obj_Base.pDa.SaveDataRow(Dr_Exam_Answer, "RecruitmentTestExams_Answers");
                }

                Obj_Base.pDa.CommitTransaction();
            }
            catch
            { Obj_Base.pDa.RollbackTransaction(); }
            finally
            { Obj_Base.pDa.Close(); }
        }
        
        #endregion

        #region _Properties

        public DataTable pDt_Question 
        {
            get { return this.mDt_Question; }
        }

        public DataTable pDt_Question_Answer
        {
            get { return this.pDt_Question_Answer; }
        }

        public Int64 pPages
        {
            get { return this.mPages; }
        }

        public Int64 pItems
        {
            get { return this.mItems; }
        }

        public Int64 pItemsLimit
        {
            get { return this.mItemsLimit; }
        }

        public ClsApplicant pObj_Applicant
        {
            get { return this.mObj_Applicant; }
        }

        public DataRow pDr_Exam
        {
            get { return this.mDr_Exam; }
        }

        #endregion
    }
}

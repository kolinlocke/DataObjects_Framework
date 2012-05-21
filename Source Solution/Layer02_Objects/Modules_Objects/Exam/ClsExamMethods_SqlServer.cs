using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Layer01_Common;
using Layer01_Common.Common;
using Layer01_Common.Connection;
using Layer01_Common.Objects;

namespace Layer02_Objects.Modules_Objects.Exam
{
    public class ClsExamMethods_SqlServer: Interface_ExamMethods
    {
        #region _ImplementedMethods

        public DataSet GenerateExam(Int64 QuestionLimit, Int64 CategoryID)
        {
            DataSet Rv = null;
            ClsConnection_SqlServer Cn = new ClsConnection_SqlServer();
            try
            { 
                Cn.Connect();
                List<Layer01_Constants.Str_Parameters> List_Sp = new List<Layer01_Constants.Str_Parameters>();
                List_Sp.Add(new Layer01_Constants.Str_Parameters("Question_Limit", QuestionLimit));
                List_Sp.Add(new Layer01_Constants.Str_Parameters("CategoryID", CategoryID));
                Rv = Cn.ExecuteQuery("usp_GenerateExam", List_Sp);
            }
            catch { }
            finally
            { Cn.Close(); }

            return Rv;
        }

        public DataSet LoadExam(long ExamID)
        {
            DataSet Rv = null;
            ClsConnection_SqlServer Cn = new ClsConnection_SqlServer();
            try
            {
                Cn.Connect();
                List<Layer01_Constants.Str_Parameters> List_Sp = new List<Layer01_Constants.Str_Parameters>();
                List_Sp.Add(new Layer01_Constants.Str_Parameters("ExamID", ExamID));
                Rv = Cn.ExecuteQuery("usp_LoadExam", List_Sp);
            }
            catch { }
            finally
            { Cn.Close(); }

            return Rv;
        }

        public long ComputeScore(DataSet Ds_Exam)
        {
            DataTable Dt_Question = Ds_Exam.Tables[0];
            DataTable Dt_Question_Answer = Ds_Exam.Tables[1];

            StringBuilder Sb_Query = new StringBuilder();
            Sb_Query.Append(@" Select * From RecruitmentTestQuestionAnswers ");
            Sb_Query.Append(@" Where ");
            Sb_Query.Append(@" Lkp_RecruitmentTestQuestionsID = @QuestionID ");
            Sb_Query.Append(@" And IsNull(IsAnswer,0) = 1 And IsNull(IsDeleted,0) = 0");

            ClsPreparedQuery Pq = new ClsPreparedQuery();
            Pq.pQuery = Sb_Query.ToString();
            Pq.Add_Parameter("QuestionID", SqlDbType.BigInt);
            Pq.Prepare();

            Int64 Score = 0;

            foreach (DataRow Dr in Dt_Question.Rows)
            {
                Pq.pParameters["QuestionID"].Value = Layer01_Methods.IsNull(Dr["RecruitmentTestQuestionsID"], 0);
                DataTable Inner_Dt = Pq.ExecuteQuery().Tables[0];
                Int64 Ct_Answer = Inner_Dt.Rows.Count;
                Int64 Ct_ExamAnswer = 0;
                DataRow[] Arr_Dr_Answers = Dt_Question_Answer.Select(@"Lkp_RecruitmentTestQuestionsID = " + ((Int64)Dr["RecruitmentTestQuestionsID"]).ToString());
                foreach (DataRow Inner_Dr in Arr_Dr_Answers)
                {
                    bool IsExamAnswer = (bool)Layer01_Methods.IsNull(Inner_Dr["IsAnswered"], false);
                    bool IsAnswer = false;

                    DataRow[] Inner_Arr_Dr = Inner_Dt.Select(@"Lkp_RecruitmentTestAnswersID = " + ((Int64)Inner_Dr["Lkp_RecruitmentTestAnswersID"]));
                    if (Inner_Arr_Dr.Length > 0)
                    { IsAnswer = true; }

                    if (IsAnswer && (IsExamAnswer == IsAnswer))
                    { Ct_ExamAnswer++; }
                }

                if (Ct_Answer == Ct_ExamAnswer)
                { Score++; }
            }

            return Score;
        }

        /*
        public void PostExam(DataSet Ds_Exam, DateTime DateStart)
        {
            ClsConnection_SqlServer Cn = new ClsConnection_SqlServer();
            try
            {
                Cn.Connect();
                Cn.BeginTransaction();

                DataTable Dt_Exam = Methods_Query.GetQuery(Cn, "RecruitmentTestExams", "", "1 = 0");
                DataRow Dr_Exam = Dt_Exam.NewRow();
                Dr_Exam["DateTaken"] = DateTime.Now;
                Dr_Exam["DateStart"] = DateStart;
                Dr_Exam["DateEnd"] = DateTime.Now;
                Dr_Exam["Score"] = this.ComputeScore(Ds_Exam);
                Dr_Exam["TotalItems"] = Ds_Exam.Tables[0].Rows.Count;

                Cn.SaveDataRow(Dr_Exam, "RecruitmentTestExams");

                //[-]

                DataTable Dt_Exams_Answers = Methods_Query.GetQuery(Cn, "RecruitmentTestExams_Answers", "", "1 = 0");
                DataRow[] Arr_Dr_Answer = Ds_Exam.Tables[1].Select("ISNULL(IsAnswered,0) = 1");
                foreach (DataRow Inner_Dr in Arr_Dr_Answer)
                {
                    DataRow Dr_Exam_Answer = Dt_Exams_Answers.NewRow();
                    Dr_Exam_Answer["Lkp_RecruitmentTestQuestionsID"] = Inner_Dr["Lkp_RecruitmentTestQuestionsID"];
                    Dr_Exam_Answer["Lkp_RecruitmentTestAnswersID"] = Inner_Dr["Lkp_RecruitmentTestAnswersID"];
                    Dr_Exam_Answer["IsAnswer"] = Inner_Dr["IsAnswered"];

                    Cn.SaveDataRow(Dr_Exam_Answer, "RecruitmentTestExams_Answers");
                }

                //[-]

                Cn.CommitTransaction();
            }
            catch(Exception Ex)
            {
                Cn.RollbackTransaction();
                throw Ex;
            }
            finally
            { Cn.Close(); }
        }
        */

        #endregion        
    }
}

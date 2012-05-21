using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using Layer01_Common;
using Layer01_Common.Common;
using Layer01_Common.Connection;
using Layer01_Common.Objects;
using Layer02_Objects;
using Layer02_Objects.DataAccess;

namespace Layer02_Objects.Modules_Objects.Exam
{
    public class ClsExamMethods_SharePoint: Interface_ExamMethods
    {
        #region _Variables

        ClsConnection_SharePoint mConnection;

        #endregion

        #region _ImplementedMethods

        public DataSet GenerateExam(Int64 QuestionLimit, Int64 CategoryID)
        {
            DataSet Rv = new DataSet();

            DataTable Dt_Question = null;
            DataTable Dt_Question_Answer = null;

            ClsConnection_SharePoint Cn = new ClsConnection_SharePoint();
            try
            {
                Cn.Connect();

                Dt_Question = Cn.GetData_Empty("RecruitmentTestQuestions").Clone();
                Dt_Question_Answer = Cn.GetData_Empty("RecruitmentTestQuestionAnswers").Clone();

                DataTable Dt_Source_Question = Cn.GetData("RecruitmentTestQuestions", null);
                DataTable Dt_Source_Question_Answer = Cn.GetData("RecruitmentTestQuestionAnswers", null); ;

                Int32 QuestionCount = Dt_Source_Question.Rows.Count;
                if (QuestionLimit < QuestionCount)
                {
                    Dt_Source_Question.Columns.Add("Ct", typeof(Int64));
                    Int64 Ct = 0;
                    foreach (DataRow Dr in Dt_Source_Question.Rows)
                    {
                        Ct++;
                        Dr["Ct"] = Ct;
                    }

                    Random Rnd = new Random();
                    Ct = 0;
                    while (Ct < QuestionLimit)
                    {
                        Int64 QuestionID = 0;
                        Int64 Selected_Question_Ct;
                        bool IsValid = false;
                        while (!IsValid)
                        {
                            Selected_Question_Ct = Rnd.Next(1, QuestionCount);
                            DataRow[] Inner_Arr_Dr = Dt_Source_Question.Select(@"Ct = " + Selected_Question_Ct);
                            if (Inner_Arr_Dr.Length > 0)
                            { QuestionID = Convert.ToInt64(Layer01_Methods.IsNull(Inner_Arr_Dr[0]["RecruitmentTestQuestions"], 0)); }

                            Inner_Arr_Dr = Dt_Question.Select(@"RecruitmentTestQuestionsID = " + QuestionID);
                            if (Inner_Arr_Dr.Length == 0)
                            { IsValid = true; }
                        }

                        DataRow[] Arr_Dr_Question = Dt_Source_Question.Select(@"RecruitmentTestQuestionsID = " + QuestionID);
                        DataRow Dr_Question = null;
                        if (Arr_Dr_Question.Length > 0)
                        { Dr_Question = Arr_Dr_Question[0]; }

                        List<Layer01_Constants.Str_Parameters> List_Sp = new List<Layer01_Constants.Str_Parameters>();
                        List_Sp.Add(new Layer01_Constants.Str_Parameters("RecruitmentTestQuestionsID", QuestionID));
                        List_Sp.Add(new Layer01_Constants.Str_Parameters("Question", Dr_Question["Question"]));
                        Layer01_Methods.AddDataRow(ref Dt_Question, List_Sp);

                        DataRow[] Arr_Dr_Question_Answer = Dt_Source_Question_Answer.Select("RecruitmentTestQuestionsID = " + QuestionID);
                        foreach (DataRow Inner_Dr in Arr_Dr_Question_Answer)
                        { Dt_Question_Answer.Rows.Add(Inner_Dr.ItemArray); }

                        Ct++;
                    }
                }
                else
                {
                    Dt_Question = Dt_Source_Question.Copy();
                    Dt_Question_Answer = Dt_Source_Question_Answer.Copy();
                }
            }
            catch { }
            finally
            { Cn.Close(); }

            Rv.Tables.Add(Dt_Question);
            Rv.Tables.Add(Dt_Question_Answer);

            return Rv;
        }

        public DataSet LoadExam(long ExamID)
        {
            throw new NotImplementedException();
        }

        public long ComputeScore(System.Data.DataSet Ds_Exam)
        {
            throw new NotImplementedException();
        }
        
        public DataRow CreateExamDataRow()
        {
            throw new NotImplementedException();
        }
        
        public void Connect()
        {
            this.mConnection = new ClsConnection_SharePoint();
            this.mConnection.Connect();
        }

        public void Close()
        { this.mConnection.Close(); }

        public void BeginTransaction()
        { }

        public void CommitTransaction()
        { }

        public void RollbackTransaction()
        { }

        public bool SaveDataRow(DataRow ObjDataRow, string TableName, string SchemaName = "", bool IsDelete = false)
        {
            this.mConnection.SaveData(TableName, ObjDataRow);
            return true;
        }

        public Interface_Connection Connection
        {
            get { return this.mConnection; }
        }

        #endregion
    }
}

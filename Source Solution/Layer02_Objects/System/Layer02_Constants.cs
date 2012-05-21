using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects_Framework._System
{
    public class Layer02_Constants
    {

        public const string CnsExam_NoItemsTotal = "Exam_NoItemsTotal";
        public const string CnsExam_NoItemsPerPage = "Exam_NoItemsPerPage";
        public const string CnsExam_NoRequiredAnswers = "Exam_NoRequiredAnswers";
        public const string CnsExam_DefaultContributor_RightsIDs = "Exam_DefaultContributor_RightsIDs";

        public enum eSystem_Modules : long
        { 
            None = 0,
            Sys_Login = 1,
            User = 2,
            Rights = 3,
            Question = 4,
            ContributorApproval = 5,
            ExamReport = 6,
            Configuration = 7,
            Exam = 8
        }

        public enum eAccessLib : int
        {
            eAccessLib_Access = 1,
            eAccessLib_New = 2,
            eAccessLib_Edit = 3,
            eAccessLib_Delete = 4,
            eAccessLib_View = 5,
            eAccessLib_Approve = 6,
            eAccessLib_Post = 7,
            eAccessLib_Cancel = 8
        }

        public enum eLookup : int
        {
            None = 0            
        }

        public enum eLookupUserType : int
        {
            None = 0,
            Administrator = 1,
            Contributor = 2,
            HR = 3
        }

        public enum eLookupQuestionType : int
        {
            Single_Answer = 1,
            Multiple_Answer = 2
        }

    }

}

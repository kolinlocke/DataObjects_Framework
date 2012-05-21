using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Layer01_Common;
using Layer01_Common.Common;
using Layer01_Common.Connection;
using Layer01_Common.Objects;
using Layer02_Objects;
using Layer02_Objects.DataAccess;
using Layer02_Objects.Modules_Base;
using Layer02_Objects.Modules_Base.Abstract;
using Layer02_Objects.Modules_Base.Objects;
using Layer02_Objects._System;
using Layer02_Objects._System.CurrentUser;

namespace Layer02_Objects._System
{
    public class ClsSysCurrentUser
    {
        #region _Variables

        bool mIsLoggedIn = false;
        bool mIsSystemAdmin = false;
        Int64 mUserID = 0;
        DataRow mDrUser;

        public enum eLoginResult
        {
            LoggedIn,
            WrongUser,
            WrongPassword,
            Administrator
        }

        //Session Variables
        Int64 mPageObjectIDs = 0;

        Interface_CurrentUserMethods mCum = new ClsCurrentUserMethods_SqlServer();

        #endregion

        #region _Methods

        public eLoginResult Login(string UserName, string Password)
        {
            //Administrator Login

            if (UserName.ToUpper() == "Administrator".ToUpper())
            {
                string Password_Check = this.mCum.GetAdministratorPassword();
                if (Password_Check == Password)
                {
                    this.AdministratorLogin();
                    return eLoginResult.Administrator;
                }
                else
                { return eLoginResult.WrongPassword; }
            }

            //User Login

            ClsBase Base = new ClsBase();
            Interface_DataAccess Da = Base.pDa;

            ClsQueryCondition Qc = Da.CreateQueryCondition();
            Qc.Add("Name", "=", (object)UserName, typeof(string).ToString(), "");
            Qc.Add("IsDeleted", "=", false, typeof(bool).ToString(), "0");

            DataTable Dt = Da.GetQuery("RecruitmentTestUser", "", Qc);
            if (Dt.Rows.Count > 0)
            {
                string Password_Check = (string)Layer01_Methods.IsNull(Dt.Rows[0]["Password"], "");

                if (Password_Check == Password)
                {
                    this.mDrUser = Dt.Rows[0];
                    this.mUserID = Convert.ToInt64(Dt.Rows[0]["RecruitmentTestUserID"]);
                    this.mIsLoggedIn = true;
                    return eLoginResult.LoggedIn;
                }
                else
                { return eLoginResult.WrongPassword; }
            }

            return eLoginResult.WrongUser;
        }

        public void AdministratorLogin()
        {
			DataTable Dt = new ClsBase().pDa.GetQuery("RecruitmentTestUser", "*", "1 = 0");
			this.mDrUser = Dt.NewRow();
			this.mDrUser["RecruitmentTestUserID"] = 0;
			this.mDrUser["Name"] = "Administrator";

			this.mIsLoggedIn = true;
			this.mIsSystemAdmin = true;

        }

        public void SystemLogin()
        { this.mIsLoggedIn = true; }

        public void Logoff()
        {
            this.mIsLoggedIn = false;
            this.mDrUser = null;
        }

        public bool CheckAccess(Layer02_Constants.eSystem_Modules System_ModulesID, Layer02_Constants.eAccessLib AccessType)
        {
            if (!this.mIsLoggedIn)
            { return false; }

            if (this.mIsSystemAdmin)
            { return true; }

            return this.mCum.CheckAccess(System_ModulesID, AccessType, this.pUserID);
        }

        public string GetNewPageObjectID()
        {
            this.mPageObjectIDs++;
            return "PageObject_" + this.mPageObjectIDs.ToString();
        }

        #endregion

        #region _Properties

        public bool pIsLoggedIn
        {
            get { return this.mIsLoggedIn; }
        }

        public DataRow pDrUser
        {
            get { return this.mDrUser; }
        }

        public bool pIsSystemAdmin
        {
            get { return this.mIsSystemAdmin; }
        }

        public Int64 pUserID 
        {
            get { return this.mUserID;}
        }

        #endregion
    }
}

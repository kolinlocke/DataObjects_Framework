using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using DataObjects_Framework;
using DataObjects_Framework.Common;

namespace DataObjects_Framework.DataAccess
{
    public class Client_WcfService : Interface_WcfService
    {
        Interface_WcfService mChannel = null;

        public Client_WcfService()
        {
            BasicHttpBinding Binding = new BasicHttpBinding();
            EndpointAddress Address = new EndpointAddress(Do_Globals.gSettings.pWcfAddress);
            this.mChannel = new ChannelFactory<Interface_WcfService>(Binding, Address).CreateChannel();
        }

        public static Client_WcfService CreateObject()
        { return new Client_WcfService(); }

		public string NewSession()
		{ return this.mChannel.NewSession(); }

		public string List(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_List Request_List)
		{ return this.mChannel.List(Request_Session, Request_List); }

		public long List_Count(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_List Request_List)
		{ return this.mChannel.List_Count(Request_Session, Request_List); }

		public string List_Empty(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_List Request_List)
		{ return this.mChannel.List_Empty(Request_Session, Request_List); }

		public string Load(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Load Request_Load)
		{ return this.mChannel.Load(Request_Session, Request_Load); }

		public string Load_TableDetails(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Load Request_Load)
		{ return this.mChannel.Load_TableDetails(Request_Session, Request_Load); }

		public string Load_RowDetails(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Load Request_Load)
		{ return this.mChannel.Load_RowDetails(Request_Session, Request_Load); }

		public bool SaveDataRow(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Save Request_Save)
		{ return this.mChannel.SaveDataRow(Request_Session, Request_Save); }

		public string GetQuery(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_GetQuery Request_GetQuery)
		{ return this.mChannel.GetQuery(Request_Session, Request_GetQuery); }

        public void ExecuteNonQuery(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Execute Request_Execute)
        { this.mChannel.ExecuteNonQuery(Request_Session, Request_Execute); }

        public string ExecuteQuery(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Execute Request_Execute)
        {
            return this.mChannel.ExecuteQuery(Request_Session, Request_Execute);
        }

        public string PreparedQuery_Prepare(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_PreparedQuery_Prepare Request_Command)
        { return this.mChannel.PreparedQuery_Prepare(Request_Session, Request_Command); }

        public void PreparedQuery_ExecuteNonQuery(Do_Constants.Str_Request_Session Request_Session, string PreparedQuerySessionID, Do_Constants.Str_Request_PreparedQuery_Parameters Request_Parameters)
        { this.mChannel.PreparedQuery_ExecuteNonQuery(Request_Session, PreparedQuerySessionID, Request_Parameters); }

        public string PreparedQuery_ExecuteQuery(Do_Constants.Str_Request_Session Request_Session, string PreparedQuerySessionID, Do_Constants.Str_Request_PreparedQuery_Parameters Request_Parameters)
        { return this.mChannel.PreparedQuery_ExecuteQuery(Request_Session, PreparedQuerySessionID, Request_Parameters); }

		public string GetTableDef(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_List Request_List)
		{ return this.mChannel.GetTableDef(Request_Session, Request_List); }

		public string GetSystemParameter(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_SystemParameter Request_SystemParameter)
		{ return this.mChannel.GetSystemParameter(Request_Session, Request_SystemParameter); }

		public void SetSystemParameter(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_SystemParameter Request_SystemParameter)
		{ this.mChannel.SetSystemParameter(Request_Session, Request_SystemParameter); }        
    }
}

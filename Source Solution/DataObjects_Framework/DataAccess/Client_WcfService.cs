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

        public string GetData(int value)
        { return this.mChannel.GetData(value); }

        public string List(Do_Constants.Str_Request_List Request_List)
        { return this.mChannel.List(Request_List); }

        public long List_Count(Do_Constants.Str_Request_List Request_List)
        { return this.mChannel.List_Count(Request_List); }

        public string List_Empty(Do_Constants.Str_Request_List Request_List)
        { return this.mChannel.List_Empty(Request_List); }

        public string Load(Do_Constants.Str_Request_Load Request_Load)
        { return this.mChannel.Load(Request_Load); }

        public string Load_TableDetails(Do_Constants.Str_Request_Load Request_Load)
        { return this.mChannel.Load_TableDetails(Request_Load); }

        public string Load_RowDetails(Do_Constants.Str_Request_Load Request_Load)
        { return this.mChannel.Load_RowDetails(Request_Load); }

        public bool SaveDataRow(Do_Constants.Str_Request_Save Request_Save)
        { return this.mChannel.SaveDataRow(Request_Save); }

        public string GetQuery(Do_Constants.Str_Request_GetQuery Request_GetQuery)
        { return this.mChannel.GetQuery(Request_GetQuery); }

        public string GetTableDef(Do_Constants.Str_Request_List Request_List)
        { return this.mChannel.GetTableDef(Request_List); }

        public string GetSystemParameter(Do_Constants.Str_Request_SystemParameter Request_SystemParameter)
        { return this.mChannel.GetSystemParameter(Request_SystemParameter); }

        public void SetSystemParameter(Do_Constants.Str_Request_SystemParameter Request_SystemParameter)
        { this.mChannel.SetSystemParameter(Request_SystemParameter); }
    }
}

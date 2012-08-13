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
        { 
            return this.mChannel.GetData(value); 
        }

        public string List(Common.Do_Constants.Str_Request_List Request_List)
        { return this.mChannel.List(Request_List); }

        public long List_Count(Common.Do_Constants.Str_Request_List Request_List)
        { return this.mChannel.List_Count(Request_List); }

        public string List_Empty(Common.Do_Constants.Str_Request_List Request_List)
        { return this.mChannel.List_Empty(Request_List); }

        public string Load(Common.Do_Constants.Str_Request_Load Request_Load)
        { return this.mChannel.Load(Request_Load); }

        public string Load_TableDetails(Common.Do_Constants.Str_Request_Load Request_Load)
        { return this.mChannel.Load_TableDetails(Request_Load); }

        public string Load_RowDetails(Common.Do_Constants.Str_Request_Load Request_Load)
        { return this.mChannel.Load_RowDetails(Request_Load); }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using DataObjects_Framework;
using DataObjects_Framework.Common;
using DataObjects_Framework.Objects;

namespace DataObjects_Framework.DataAccess
{
    [ServiceContract(Name = "ServiceContract", Namespace = "http://DataObjects_Wcf/")]
    public interface Interface_WcfService
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]        
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "List")]
        String List(Do_Constants.Str_Request_List Request_List);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "List_Count")]
        Int64 List_Count(Do_Constants.Str_Request_List Request_List);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "List_Empty")]
        String List_Empty(Do_Constants.Str_Request_List Request_List);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "Load")]
        String Load(Do_Constants.Str_Request_Load Request_Load);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "Load_TableDetails")]
        String Load_TableDetails(Do_Constants.Str_Request_Load Request_Load);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "Load_RowDetails")]
        String Load_RowDetails(Do_Constants.Str_Request_Load Request_Load);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "SaveDataRow")]
        Boolean SaveDataRow(Do_Constants.Str_Request_Save Request_Save);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "GetQuery")]
        String GetQuery(Do_Constants.Str_Request_GetQuery Request_GetQuery);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "GetTableDef")]
        String GetTableDef(Do_Constants.Str_Request_List Request_List);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "GetSystemParameter")]
        String GetSystemParameter(Do_Constants.Str_Request_SystemParameter Request_SystemParameter);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "SetSystemParameter")]
        void SetSystemParameter(Do_Constants.Str_Request_SystemParameter Request_SystemParameter);
    }
}

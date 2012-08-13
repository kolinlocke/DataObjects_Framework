using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using DataObjects_Framework;
using DataObjects_Framework.Objects;
using DataObjects_Wcf;

namespace DataObjects_Wcf
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
        String List(Constants.Str_Request_List Request_List);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "List_Count")]
        Int64 List_Count(Constants.Str_Request_List Request_List);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "List_Empty")]
        String List_Empty(Constants.Str_Request_List Request_List);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "Load")]
        String Load(Constants.Str_Request_Load Request_Load);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "Load_TableDetails")]
        String Load_TableDetails(Constants.Str_Request_Load Request_Load);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            , UriTemplate = "Load_RowDetails")]
        String Load_RowDetails(Constants.Str_Request_Load Request_Load);        
    }
}

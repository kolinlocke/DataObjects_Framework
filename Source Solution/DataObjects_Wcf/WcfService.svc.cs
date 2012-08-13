using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using DataObjects_Wcf;
using DataObjects_Framework;
using DataObjects_Framework.DataAccess;
using DataObjects_Framework.Common;
using DataObjects_Framework.Objects;

namespace DataObjects_Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : Interface_WcfService
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public String List(Do_Constants.Str_Request_List Request_List)
        {
            ClsSimpleDataTable Rv = null;
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_List.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_List.ConnectionString); }

                DataTable Dt = null;

                if (Do_Methods.Convert_String(Request_List.Condition_String) == "")
                {
                    Dt = Da.List(
                    Request_List.ObjectName
                    , Request_List.Condition_String
                    , Request_List.Sort);
                    Rv = new ClsSimpleDataTable(Dt);
                }
                else
                {
                    Dt = Da.List(
                    Request_List.ObjectName
                    , Request_List.Condition
                    , Request_List.Sort
                    , Request_List.Top
                    , Request_List.Page);
                    Rv = new ClsSimpleDataTable(Dt);
                }
            }
            catch (Exception Ex)
            { throw Ex; }
            finally { Da.Close(); }

            return Rv.Serialize();
        }

        public long List_Count(Do_Constants.Str_Request_List Request_List)
        {
            Int64 Rv = 0;
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_List.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_List.ConnectionString); }

                Rv = Da.List_Count(Request_List.ObjectName, Request_List.Condition);                
            }
            catch (Exception Ex)
            { throw Ex; }
            finally { Da.Close(); }

            return Rv;
        }

        public String List_Empty(Do_Constants.Str_Request_List Request_List)
        {
            ClsSimpleDataTable Rv = null;
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_List.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_List.ConnectionString); }

                DataTable Dt = Da.List_Empty(
                Request_List.ObjectName);
                Rv = new ClsSimpleDataTable(Dt);
            }
            catch (Exception Ex)
            { throw Ex; }
            finally { Da.Close(); }

            return Rv.Serialize();
        }

        public String Load(Do_Constants.Str_Request_Load Request_Load)
        {
            ClsSimpleDataRow Rv = null;
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_Load.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Load.ConnectionString); }

                DataRow Dr = Da.Load(Request_Load.ObjectName, Request_Load.ObjectKeys, Request_Load.Key);
                ClsSimpleDataTable Sds = new ClsSimpleDataTable(Dr.Table.Clone());
                Rv = Sds.NewRow(Dr);
            }
            catch (Exception Ex)
            { throw Ex; }
            finally { Da.Close(); }

            return Rv.Serialize();
        }

        public String Load_TableDetails(Do_Constants.Str_Request_Load Request_Load)
        {
            ClsSimpleDataTable Rv =  null;
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_Load.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Load.ConnectionString); }

                Rv = new ClsSimpleDataTable(Da.Load_TableDetails(Request_Load.ObjectName, Request_Load.Key, Request_Load.Condition, Request_Load.ForeignKeys));
            }
            catch (Exception Ex)
            { throw Ex; }
            finally
            { Da.Close(); }

            return Rv.Serialize();   
        }

        public String Load_RowDetails(Do_Constants.Str_Request_Load Request_Load)
        {
            ClsSimpleDataRow Rv = null;
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_Load.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Load.ConnectionString); }

                DataRow Dr = Da.Load_RowDetails( Request_Load.ObjectName, Request_Load.Key, Request_Load.Condition, Request_Load.ForeignKeys);
                ClsSimpleDataTable Sdt = new ClsSimpleDataTable(Dr.Table.Clone());
                Rv = Sdt.NewRow(Dr);
            }
            catch (Exception Ex) 
            { throw Ex; }
            finally
            { Da.Close(); }

            return Rv.Serialize();
        }
    }
}

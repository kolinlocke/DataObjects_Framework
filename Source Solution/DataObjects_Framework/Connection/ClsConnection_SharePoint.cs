using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Layer01_Common.Common;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;

namespace Layer01_Common.Connection
{
    public class ClsConnection_SharePoint : Interface_Connection
    {
        #region _Variables

        ClientContext mConnection = null;

        #endregion

        #region _Methods

        public void Connect()
        { 
            this.mConnection = new ClientContext(Global_Variables.gConnection_SharePoint_Server);

            //this.mConnection.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
            //this.mConnection.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo(Global_Variables.gConnection_SharePoint_UserName, Global_Variables.gConnection_SharePoint_Password);
            
            System.Net.NetworkCredential Nc = new System.Net.NetworkCredential(Global_Variables.gConnection_SharePoint_UserName, Global_Variables.gConnection_SharePoint_Password);
            this.mConnection.Credentials = Nc;
        }

        public void Close()
        { this.mConnection.Dispose(); }

        public DataTable GetData(string TableName, CamlQuery Query = null)
        {
            if (Query == null)
            { Query = new CamlQuery(); }

            ClientContext Cc = this.mConnection;
            Web SP_Web = this.mConnection.Web;
            List SP_List = SP_Web.Lists.GetByTitle(TableName);
            FieldCollection SP_FieldCollection = SP_List.Fields;
            ListItemCollection SP_ListItem = SP_List.GetItems(Query);
            
            Cc.Load(SP_FieldCollection);
            Cc.Load(SP_ListItem);
            Cc.ExecuteQuery();
            
            DataTable Dt = this.CreateTable(TableName, SP_List.Fields);
            this.PopulateTable(SP_ListItem, SP_FieldCollection, Dt, TableName);

            return Dt;
        }

        public DataTable GetData_Empty(string TableName)
        {
            ClientContext Cc = this.mConnection;
            Web SP_Web = this.mConnection.Web;
            List SP_List = SP_Web.Lists.GetByTitle(TableName);
            
            Cc.Load(SP_List.Fields);
            Cc.ExecuteQuery();

            DataTable Dt = this.CreateTable(TableName, SP_List.Fields);
            return Dt;
        }

        public void SaveData(string TableName, DataRow Dr)
        {
            ClientContext Cc = this.mConnection;
            Web SP_Web = this.mConnection.Web;
            List SP_List = SP_Web.Lists.GetByTitle(TableName);
            string TableKey = TableName + "ID";
            
            Cc.Load(SP_List.Fields);
            Cc.ExecuteQuery();

            ListItem Li = null;

            bool IsInsert = false;

            if ((Int32)Layer01_Methods.IsNull(Dr[TableKey], 0) == 0)
            { IsInsert = true; }

            if (IsInsert)
            {
                ListItemCreationInformation Lici = new ListItemCreationInformation();
                Li = SP_List.AddItem(Lici);
            }
            else
            {
                try
                { Li = SP_List.GetItemById((Int32)Dr[TableKey]); }
                catch
                {
                    ListItemCreationInformation Lici = new ListItemCreationInformation();                    
                    Li = SP_List.AddItem(Lici);
                }
            }
            
            FieldCollection Fc = SP_List.Fields;
            foreach (Field F in Fc)
            {
                foreach (DataColumn Dc in Dr.Table.Columns)
                {
                    if (Dc.ColumnName == F.StaticName
                        && Dc.ColumnName != TableKey
                        )
                    {
                        if (!F.ReadOnlyField)
                        {
                            switch (F.FieldTypeKind)
                            {
                                case FieldType.Lookup:
                                    {
                                        try
                                        { Li[F.StaticName] = new FieldLookupValue() { LookupId = (Int32)Dr[F.StaticName] }; }
                                        catch { }
                                        break;
                                    }
                                default:
                                    Li[F.StaticName] = Dr[F.StaticName]; 
                                    break;
                            }
                        }
                        break;
                    }
                }
            }
            Li.Update();
            Cc.ExecuteQuery();

            if (IsInsert)
            { Dr[TableKey] = Li.Id; }
        }

        DataTable CreateTable(string TableName, FieldCollection Fc)
        {
            DataTable Dt = new DataTable();
            foreach (Field F in Fc)
            {
                Type Inner_Type = typeof(string);
                string FieldName = F.StaticName;
                bool IsFound = true;

                switch (F.FieldTypeKind)
                {
                    case FieldType.Counter:
                        Inner_Type = typeof(Int32);
                        FieldName = TableName + "ID";
                        break;
                    case FieldType.Boolean:
                        Inner_Type = typeof(bool);
                        break;
                    case FieldType.DateTime:
                        Inner_Type = typeof(DateTime);
                        break;
                    case FieldType.Integer:
                        Inner_Type = typeof(Int64);
                        break;
                    case FieldType.Number:
                        Inner_Type = typeof(double);
                        break;
                    case FieldType.Text:
                    case FieldType.Note:
                        Inner_Type = typeof(string);
                        break;
                    case FieldType.Lookup:
                        Inner_Type = typeof(Int32);
                        Dt.Columns.Add(F.StaticName, typeof(Int32));
                        Dt.Columns.Add(F.StaticName + @"_Desc", typeof(string));
                        IsFound = false;
                        break;
                    default:
                        IsFound = false;
                        break;
                }
                if (IsFound)
                { Dt.Columns.Add(FieldName, Inner_Type); }
            }
            return Dt;
        }

        void PopulateTable(ListItemCollection Lic, FieldCollection Fc, DataTable Dt, string TableName = "")
        {
            foreach (ListItem Li in Lic)
            {
                DataRow Nr = Dt.NewRow();
                
                foreach (DataColumn Dc in Dt.Columns)
                {
                    try
                    { Nr[Dc] = Li.FieldValues[Dc.ColumnName]; }
                    catch { }
                }
                
                bool IsFound = true;
                foreach (Field F in Fc)
                {
                    switch (F.FieldTypeKind)
                    {
                        case FieldType.Counter:
                            try
                            { Nr[TableName + @"ID"] = Li.FieldValues[F.StaticName]; }
                            catch { }
                            break;
                        case FieldType.Lookup:
                            try
                            { 
                                FieldLookupValue Flv = (FieldLookupValue)Li.FieldValues[F.StaticName];
                                Nr[F.StaticName] = Flv.LookupId;
                                Nr[F.StaticName + @"_Desc"] = Flv.LookupValue;
                            }
                            catch { }
                            break;
                        default:
                            IsFound = false;
                            break;
                    }
                    if (IsFound) break;
                }
                Dt.Rows.Add(Nr);
            }
        }

        public DataTable GetTableDef(string TableName)
        {
            DataTable Dt = new DataTable();
            Dt.Columns.Add("ColumnName", typeof(string));
            Dt.Columns.Add("IsPk", typeof(bool));

            ClientContext Cc = this.mConnection;
            Web SP_Web = this.mConnection.Web;
            List SP_List = SP_Web.Lists.GetByTitle(TableName);

            Cc.Load(SP_List.Fields);
            Cc.ExecuteQuery();

            FieldCollection Fc = SP_List.Fields;
            foreach (Field F in Fc)
            {
                Type Inner_Type = typeof(string);
                string FieldName = F.StaticName;
                bool IsPk = false;
                bool IsFound = true;

                switch (F.FieldTypeKind)
                {
                    case FieldType.Counter:
                        Inner_Type = typeof(Int32);
                        FieldName = TableName + "ID";
                        IsPk = true;
                        break;
                    case FieldType.Boolean:
                        Inner_Type = typeof(bool);
                        break;
                    case FieldType.DateTime:
                        Inner_Type = typeof(DateTime);
                        break;
                    case FieldType.Integer:
                        Inner_Type = typeof(Int64);
                        break;
                    case FieldType.Number:
                        Inner_Type = typeof(double);
                        break;
                    case FieldType.Text:
                    case FieldType.Note:
                        Inner_Type = typeof(string);
                        break;
                    default:
                        IsFound = false;
                        break;
                }
                if (IsFound)
                {
                    DataRow Nr = Dt.NewRow();
                    Nr["ColumnName"] = FieldName;
                    Nr["IsPk"] = IsPk;
                    Dt.Rows.Add(Nr);
                }
            }
            
            return Dt;
        }

        public void Ex()
        {
            /*
            ClientContext Cc = this.mConnection;
            Web SP_Web = this.mConnection.Web;
            ListCreationInformation Lci = new ListCreationInformation();
            Lci.Title = "Janer Bago";

            List L = SP_Web.Lists.Add(Lci);
            //L.Title = "Janer Bago";
            L.Update();
            Cc.ExecuteQuery();
            */
        }
        
        #endregion

        #region _Properties

        public Object pConnection
        {
            get { return this.mConnection; }
        }

        public IDbTransaction pTransaction
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}

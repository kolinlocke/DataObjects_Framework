using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Layer01_Common.Common
{
    public class Layer01_Constants
    {
        public enum eSystem_Lookup : int
        {
            None = 0,
            FieldType_Static = 1,
            FieldType_Text = 2,
            FieldType_Checkbox = 3,
            FieldType_DateTime = 4,
            FieldType_Button = 5,
            FieldType_Delete = 6,
            FieldType_HyperLink = 7
        }

        public enum eSystem_Lookup_FieldType : int
        {
            None = eSystem_Lookup.None,
            FieldType_Static = eSystem_Lookup.FieldType_Static,
            FieldType_Text = eSystem_Lookup.FieldType_Text,
            FieldType_Checkbox = eSystem_Lookup.FieldType_Checkbox,
            FieldType_DateTime = eSystem_Lookup.FieldType_DateTime,
            FieldType_Button = eSystem_Lookup.FieldType_Button,
            FieldType_Delete = eSystem_Lookup.FieldType_Delete,
            FieldType_HyperLink = eSystem_Lookup.FieldType_HyperLink
        }

        public struct Str_AddSelectedFields
        {
            public string Field_Target;
            public string Field_Selected;

            public Str_AddSelectedFields(string pField_Target, string pField_Selected)
            {
                Field_Target = pField_Target;
                Field_Selected = pField_Selected;
            }
        }

        public struct Str_AddSelectedFieldsDefault
        {
            public string Field_Target;
            public object Value;

            public Str_AddSelectedFieldsDefault(string pField_Target, object pValue)
            {
                Field_Target = pField_Target;
                Value = pValue;
            }
        }

        public struct Str_Parameters
        {
            public string Name;
            public object Value;

            public Str_Parameters(string pName, object pValue)
            {
                Name = pName;
                Value = pValue;
            }
        }

        public struct Str_Sort
        {
            public string Name;
            public bool IsDesc;

            public Str_Sort(string pName, bool pIsDesc)
            {
                this.Name = pName;
                this.IsDesc = pIsDesc;
            }
        }

    }
}

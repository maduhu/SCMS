using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Utils
{
    public static class Constants
    {
        public const string NUMBER_FORMAT_TWO_DECIMAL = "#,##0.00";
        public const string NUMBER_FORMAT_NO_DECIMAL = "#,##0";
        public const string DATE_FORMAT = "dd/MM/yyyy";
        public const string DATETIME_FORMAT = "dd/MM/yyyy h:mm tt";
        public const string MVC_DATE_FORMAT = "{0:dd/MM/yyyy}";
        public const string MVC_NUMBER_FORMAT_TWO_DECIMAL = "{0:n2}";
        public static readonly string LOCAL = Resource.Resources.Global_String_Local;
        public static readonly string INTERNATIONAL = Resource.Resources.Global_String_International;

        public static List<KeyValuePair> YesNoList
        {
            get
            {
                return new KeyValuePair[] 
                    { 
                        new KeyValuePair { Key = "Yes", Value = "true" }, 
                        new KeyValuePair { Key = "No", Value = "false" } 
                    }.ToList();
            }
        }
    }

    public class KeyValuePair
    {
        public string Key { get; set; }

        public object Value { get; set; }
    }
}

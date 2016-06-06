using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic;

namespace System
{
    public static class TypeExtensions
    {
        public static TypeConverter GetCustomTypeConverter(this Type type)
        {
            if(type.TypeHandle.Equals(typeof(Boolean).TypeHandle))
            {
                var r = TypeDescriptor.GetConverter(type);
            }
            if (type == typeof(List<int>))
                return new GenericListTypeConverter<int>();
            if (type == typeof(List<decimal>))
                return new GenericListTypeConverter<decimal>();
            if (type == typeof(List<string>))
                return new GenericListTypeConverter<string>();

            return TypeDescriptor.GetConverter(type);
        }
    }
}

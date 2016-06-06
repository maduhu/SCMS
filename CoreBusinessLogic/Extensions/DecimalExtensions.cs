using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class DecimalExtensions
    {
        public static string FormatMoney(this decimal value)
        {
            return value.ToString("###,###,###.00");
        }

        public static string FormatMoney(this decimal? value)
        {
            return value.HasValue ? FormatMoney(value.Value) : "";
        }

        public static Decimal RoundedUp(this decimal value)
        {
            return Math.Truncate(value) == value ? value : ((Math.Truncate(value * 100) / 100.0m) + 0.01m);
        }

        public static Decimal? RoundedUp(this decimal? value)
        {
            return value == null? null : (Decimal?)value.Value.RoundedUp();
        }
    }
}

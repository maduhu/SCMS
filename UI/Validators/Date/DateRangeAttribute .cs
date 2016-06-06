using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SCMS.UI.Validators.Date
{
    public class DateRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime date = DateTime.Parse(value.ToString()); // assuming it's in a parsable string format

            if (date >= DateTime.Now.Subtract(new TimeSpan(14, 0, 0, 0)))
                return true;
            return false;
        }
    }
}
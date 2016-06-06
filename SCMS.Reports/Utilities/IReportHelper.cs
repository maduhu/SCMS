using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Reports.Utilities
{
    public interface IReportHelper
    {
        string GetLogoUrl();

        Guid GetCountryProgram();
        
        string GetCountryProgramAddress();

        string GetDefaultstr(object value);

        string GetDefaultstrings(string[] value);

        string GetDefaultstr(object paramValue, object value);
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using SCMS.Model;
using SCMS.CoreBusinessLogic.Users;
using SCMS.Resource;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.Reports.Utilities
{
    public class ReportHelper //: IReportHelper
    {
        //private IPermissionService permissionService;
        //private IUserContext userContext;

        //protected readonly CountryProgramme countryProg;
        //protected readonly Currency mbCurrency;
        //protected readonly SystemUser currentUser;
        //protected readonly Staff currentStaff;

        //public ReportHelper(IPermissionService permissionService, IUserContext userContext)
        //{
        //    this.permissionService = permissionService;
        //    this.userContext = userContext;
        //    if (userContext.CurrentUser != null)
        //    {
        //        countryProg = userContext.CurrentUser.Staff.CountrySubOffice.CountryProgramme;
        //        currentUser = userContext.CurrentUser;
        //        mbCurrency = countryProg.Currency;
        //        currentStaff = currentUser.Staff;
        //    }
        //}

        #region Previous
        
        public static byte[] GetLogoUrl()
        {
            //var uri = new UriBuilder(HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port);
            //return uri.ToString() + "Content/reports/logos-90.png";
            return SessionData.CurrentSession.CountryProg.Logo;
        }

        public static Guid GetCountryProgram()
        {
            return SessionData.CurrentSession.CountryProgrammeId;
        }

        public static string GetCountryProgramAddress()
        {
            //If it ever complains about the master budget currency use the countryPorg to get the currency
            return SessionData.CurrentSession.CountryProg.Address;
        }

        public static string GetDefaultstr(object value)
        {
            return value == null ? Resources.Global_String_ALL : value.ToString();
        }

        public static string GetDefaultstrings(string[] value)
        {
            string mm = "";
            if (value != null) foreach (string item in value) mm += item + ",";
            return value == null ? Resources.Global_String_ALL : mm;
        }

        public static string GetDefaultstr(object paramValue, object value)
        {

            return paramValue == null ? Resources.Global_String_ALL : value.ToString();
        }

        public static string GetDefaultstr1(object value)
        {
            return value == null ? Resources.Global_String_ALL : String.Format("{0:d/M/yyyy}", value);
        }
        #endregion

        #region Implementation

        //public string GetLogoUrl()
        //{
        //    var uri = new UriBuilder(HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port);
        //    return uri.ToString() + "Content/reports/logos-90.png";
        //}

        //public Guid GetCountryProgram()
        //{
        //    return countryProg.Id;
        //    //return new Guid("75A13FE7-E763-4CA8-9A94-FF3B3E3C0453");
        //}

        //public string GetCountryProgramAddress()
        //{
        //    return countryProg.Address;
        //    //return "COUNTRY OFFICE, UGANDA\n4688 Kalungi Road\nMuyenga, Kampala";
        //}

        //public string GetDefaultstr(object value)
        //{
        //    return value == null ? "All" : value.ToString();
        //}

        //public string GetDefaultstrings(string[] value)
        //{
        //    string mm = "";
        //    if (value != null) foreach (string item in value) mm += item + ",";
        //    return value == null ? "All" : mm;
        //}

        //public string GetDefaultstr(object paramValue, object value)
        //{
        //    return paramValue == null ? "All" : value.ToString();
        //}

        #endregion
    }

}

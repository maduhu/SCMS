using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class SystemUserExtensions
    {
        public static IQueryable<SystemUserRole> IncludeRole(this IQueryable<SystemUserRole> query)
        {
            return ((DbQuery<SystemUserRole>)query).Include("Role");
        }

        public static IQueryable<SystemUser> IncludeRoles(this IQueryable<SystemUser> query)
        {
            return ((DbQuery<SystemUser>)query).Include("SystemUserRoles.Role");
        }

        public static IQueryable<SystemUser> IncludePerson(this IQueryable<SystemUser> query)
        {
            return ((DbQuery<SystemUser>)query).Include("Staff.Person");
        }

        public static IQueryable<SystemUser> IncludeFinanceLimit(this IQueryable<SystemUser> query)
        {
            return ((DbQuery<SystemUser>)query).Include("Staff.FinanceLimit");
        }

        public static IQueryable<SystemUser> IncludeDesignation(this IQueryable<SystemUser> query)
        {
            return ((DbQuery<SystemUser>)query).Include("Staff.Designation");
        }

        public static IQueryable<SystemUser> IncludeCountry(this IQueryable<SystemUser> query)
        {
            return ((DbQuery<SystemUser>)query).Include("Staff.CountrySubOffice.CountryProgramme.Country");
        }

        public static IQueryable<SystemUser> IncludeCurrency(this IQueryable<SystemUser> query)
        {
            return ((DbQuery<SystemUser>)query).Include("Staff.CountrySubOffice.CountryProgramme.Currency");
        }
    }
}

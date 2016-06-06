using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class StaffExtensions
    {
        public static IQueryable<Staff> IncludeStaffDetails(this IQueryable<Staff> queryable)
        {
            return ((DbQuery<Staff>)queryable).Include("Person").Include("CountrySubOffice")
                .Include("Designation").Include("SystemUsers.Approvers").Include("SystemUsers.Approvers1");
        }
    }
}

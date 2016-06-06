using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class RoleExtensions
    {
        public static IQueryable<Role> IncludePermissionRecords(this IQueryable<Role> query)
        {
            return ((DbQuery<Role>)query).Include("RolePermissionRecords.PermissionRecord");
        }
    }
}

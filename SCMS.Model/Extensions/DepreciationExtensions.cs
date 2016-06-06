using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class DepreciationExtensions
    {
        public static IQueryable<Depreciation> IncludeAssetDetails(this IQueryable<Depreciation> queryable)
        {
            return ((DbQuery<Depreciation>)queryable).Include("Asset.Currency");
        }
    }
}

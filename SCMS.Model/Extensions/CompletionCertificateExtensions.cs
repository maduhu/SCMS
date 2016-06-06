using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public static class CompletionCertificateExtensions
    {
        public static IQueryable<CompletionCertificate> IncludePurchaseOrder(this IQueryable<CompletionCertificate> queryable)
        {
            return ((DbQuery<CompletionCertificate>)queryable).Include("PurchaseOrder.Supplier");
        }

        public static IQueryable<CompletionCertificate> IncludeStaff(this IQueryable<CompletionCertificate> queryable)
        {
            return ((DbQuery<CompletionCertificate>)queryable).Include("Staff.Person").Include("Staff.Designation")
                                                              .Include("Staff1.Person").Include("Staff1.Designation")
                                                              .Include("Staff2.Person").Include("Staff2.Designation");
        }

        public static IQueryable<CompletionCertificate> IncludeCountrySubOffice(this IQueryable<CompletionCertificate> queryable)
        {
            return ((DbQuery<CompletionCertificate>)queryable).Include("CountrySubOffice.CountryProgramme.Country").Include("CountrySubOffice1.CountryProgramme.Country");
        }
    }
}

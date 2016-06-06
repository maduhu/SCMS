using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.Model;
using SCMS.CoreBusinessLogic.CompletionCtificate;
using System.ComponentModel.DataAnnotations;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class CompletionCtificate
    {
        public Model.CompletionCertificate EntityCC { get; set; }

        public SelectList POItems { get; set; }

        public SelectList Offices { get; set; }

        public SelectList Staffs { get; set; }
    }

    public class ViewCC
    {
        public Model.CompletionCertificate EntityCC { get; set; }

        public VStaffDetail ComfirmedBy { get; set; }

        public VStaffDetail PreparedBy { get; set; }

        public VStaffDetail AprovedBy { get; set; }
    }

    public class CompletionCExtention
    {
        public static ViewCC PrepareCC(Guid CCid, ICompletionCertificateService ccService)
        {
            using (var db = new SCMSEntities())
            {
                var item = ccService.GetCCById(CCid);
                var model = new ViewCC()
                {
                    EntityCC = item,
                    PreparedBy = db.VStaffDetails.FirstOrDefault(p => p.StaffID == item.PreparedBy),
                    ComfirmedBy = db.VStaffDetails.FirstOrDefault(p => p.StaffID == item.ConfirmedBy),
                    AprovedBy = db.VStaffDetails.FirstOrDefault(p => p.StaffID == item.ApprovedBy)
                };
                return model;
            }
        }
    }

    
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using System.Data.Objects;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using System.Data.Entity.Infrastructure;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class PaymentTermController : PortalBaseController
    {

        public PaymentTermController(IPermissionService permissionService, IUserContext userContext)
            : base(userContext, permissionService)
        { }
        //
        // GET: /PaymentTerm/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListView()
        {
            using (var db = new SCMSEntities())
            {
                return View("ListView", db.PaymentTerms.Where(p => p.CountryProgrammeId == countryProg.Id).ToList());
            }
        }

        //
        // GET: /PaymentTerm/Create

        public ActionResult Create()
        {
            return View("CreateEdit");
        }

        //
        // POST: /PaymentTerm/Create

        [HttpPost]
        public ActionResult CreateEdit(PaymentTerm entity)
        {
            // TODO: Add insert logic here
            using (var context = new SCMSEntities())
            {                
                entity.CountryProgrammeId = countryProg.Id;
                if (entity.Id.Equals(Guid.Empty))
                {
                    entity.Id = Guid.NewGuid();
                    context.PaymentTerms.Add(entity);
                }
                else
                {
                    context.PaymentTerms.Attach(entity);
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                }
                context.SaveChanges();
            }
            return ListView();
        }

        //
        // GET: /PaymentTerm/Edit/5

        public ActionResult Edit(Guid id)
        {
            using (var db = new SCMSEntities())
            {
                return View("CreateEdit", db.PaymentTerms.FirstOrDefault(p => p.Id == id));
            }
        }

        //
        // GET: /PaymentTerm/Delete/5

        public ActionResult Delete(Guid id)
        {
            using (var db = new SCMSEntities())
            {
                db.PaymentTerms.Remove(db.PaymentTerms.FirstOrDefault(p => p.Id == id));
                db.SaveChanges();
            }
            return ListView();

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using System.Data.Entity.Infrastructure;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class PaymentTypeController : PortalBaseController
    {
        //
        // GET: /PayementType/

        public PaymentTypeController(IPermissionService permissionService, IUserContext userContext)
            : base(userContext, permissionService)
        {

        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListView()
        {
            using (var db = new SCMSEntities())
            {
                return View("ListView", db.PaymentTypes.Where(p => p.CountryProgrammeId == countryProg.Id).ToList());
            }
        }

        //
        // GET: /PayementType/Create

        public ActionResult Create()
        {
            return View("CreateEdit");
        }

        //
        // POST: /PayementType/Create

        [HttpPost]
        public ActionResult CreateEdit(PaymentType entity)
        {

            // TODO: Add insert logic here
            using (var context = new SCMSEntities())
            {
                entity.CountryProgrammeId = countryProg.Id;
                if (entity.Id.Equals(Guid.Empty))
                {
                    entity.Id = Guid.NewGuid();
                    context.PaymentTypes.Add(entity);
                }
                else
                {
                    context.PaymentTypes.Attach(entity);
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                }

                context.SaveChanges();
            }
            return ListView();

        }

        //
        // GET: /PayementType/Edit/5

        public ActionResult Edit(Guid id)
        {
            using (var db = new SCMSEntities())
            {
                return View("CreateEdit", db.PaymentTypes.SingleOrDefault(p => p.Id == id));
            }
        }

        //
        // GET: /PayementType/Delete/5

        public ActionResult Delete(Guid id)
        {

            using (var db = new SCMSEntities())
            {
                db.PaymentTypes.Remove(db.PaymentTypes.SingleOrDefault(p => p.Id == id));
                db.SaveChanges();
            }
            return ListView();

        }
    }
}

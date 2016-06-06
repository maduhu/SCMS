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
    public class ShippingTermController : PortalBaseController
    {

        public ShippingTermController(IPermissionService permissionService, IUserContext userContext)
            : base(userContext, permissionService)
        { }
        //
        // GET: /ShippingTerm/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListView()
        {
            using (var db = new SCMSEntities())
            {
                return View("ListView", GetShippingTerms());
            }
        }

        private List<ShippingTerm> GetShippingTerms()
        {
            using (var db = new SCMSEntities())
            {
                return db.ShippingTerms.Where(s => s.CountryProgrammeId == countryProg.Id).OrderBy(s => s.Name).ToList();
            }
        }

        //
        // GET: /ShippingTerm/Create

        public ActionResult CreateEdit()
        {
            return View("CreateEdit");
        }

        //
        // POST: /ShippingTerm/Create

        [HttpPost]
        public ActionResult CreateEdit(ShippingTerm entity)
        {

            // TODO: Add insert logic here
            using (var context = new SCMSEntities())
            {

                entity.CountryProgrammeId = countryProg.Id;
                if (entity.Id.Equals(Guid.Empty))
                {
                    entity.Id = Guid.NewGuid();
                    context.ShippingTerms.Add(entity);
                }
                else
                {
                    entity.CountryProgrammeId = countryProg.Id;
                    context.ShippingTerms.Attach(entity);
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                }
                context.SaveChanges();
            }
            return ListView();
        }

        //
        // GET: /ShippingTerm/Edit/5

        public ActionResult Edit(Guid id)
        {
            using (var db = new SCMSEntities())
            {
                return View("CreateEdit", db.ShippingTerms.SingleOrDefault(p => p.Id == id));
            }
        }

        //
        // GET: /ShippingTerm/Delete/5

        public ActionResult Delete(Guid id)
        {
            using (var db = new SCMSEntities())
            {
                db.ShippingTerms.Remove(db.ShippingTerms.SingleOrDefault(p => p.Id == id));
                db.SaveChanges();
            }
            return ListView();
        }
    }
}

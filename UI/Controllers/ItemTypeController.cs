using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.Model;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.ActionFilters;
using System.Data.Entity.Infrastructure;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class ItemTypeController : PortalBaseController
    {
        public ItemTypeController(IPermissionService permissionService, IUserContext userContext) : base(userContext, permissionService) { }
        //
        // GET: /ItemType/

        public ActionResult Index()
        {

            return View();

        }

        public ActionResult ListView()
        {
            using (var db = new SCMSEntities())
            {
                return View(db.ItemTypes.ToList());
            }
        }

        public ActionResult Create()
        {
            return View(new ItemType());
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(ItemType entity)
        {
            using (var db = new SCMSEntities())
            {
                entity.Id = Guid.NewGuid();
                //entity.CountryProgramId = countryProg.Id;
                db.ItemTypes.Add(entity);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid id)
        {
            using (var db = new SCMSEntities())
            {
                return View(db.ItemTypes.FirstOrDefault(p => p.Id == id));
            }
        }

        //
        // POST: /PayrollItem/Edit/5

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(Guid id, ItemType entity)
        {

            // TODO: Add update logic here
            using (var context = new SCMSEntities())
            {
                context.ItemTypes.Attach(entity);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                context.SaveChanges();
            }
            return RedirectToAction("Index");

        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Delete(Guid id)
        {
            using (var db = new SCMSEntities())
            {
                db.ItemTypes.Remove(db.ItemTypes.FirstOrDefault(p => p.Id == id));
                db.SaveChanges();
            }
            //return View("ListView");
            return RedirectToAction("ListView");


        }
    }
}

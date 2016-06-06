using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.Model;
using SCMS.UI.Models;
using System.Data.Entity.Infrastructure;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class ServiceCheckListController : PortalBaseController
    {
        public ServiceCheckListController(IPermissionService permissionService, IUserContext userContext)
            : base(userContext, permissionService)
        { }
        //
        // GET: /ServiceCheckList/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ListView()
        {
            return View("ListView", SessionData.CurrentSession.CheckListCategoryList.ToList());
        }
        public ActionResult CreateCheckListCat()
        {
            return View();
        }

        //
        // GET: /ServiceCheckLists/Create

        public ActionResult Create(Guid? sclId = null)
        {
            var model = new SCheckList()
            {
                EntitySCL = sclId == null ? new ServiceCheckList() : SessionData.CurrentSession.ServiceCheckListList.FirstOrDefault(p => p.Id == sclId),
                CheckListCatories = new SelectList(SessionData.CurrentSession.CheckListCategoryList.ToList(), "Id", "Name")
            };
            return View("CreateEdit", model);
        }

        //
        // POST: /ServiceCheckLists/Create

        [HttpPost]
        public ActionResult CreateEdit(SCheckList model)
        {
            // TODO: Add insert logic here
            using (var db = new SCMSEntities())
            {
                if (model.EntitySCL.Id.Equals(Guid.Empty))
                {
                    model.EntitySCL.Id = Guid.NewGuid();
                    db.ServiceCheckLists.Add(model.EntitySCL);
                }
                else
                {
                    db.ServiceCheckLists.Attach(model.EntitySCL);
                    ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(model.EntitySCL, System.Data.EntityState.Modified);
                }
                db.SaveChanges();
            }
            SessionData.CurrentSession.ServiceCheckListList = null;
            SessionData.CurrentSession.CheckListCategoryList = null;
            return ListView();
        }

        [HttpPost]
        public ActionResult CreateEditCatory(CheckListCategory entity)
        {
            using (var context = new SCMSEntities())
            {
                if (entity.Id.Equals(Guid.Empty))
                {
                    entity.Id = Guid.NewGuid();
                    entity.IssueDate = DateTime.Now;
                    entity.CountryprogrammeId = countryProg.Id;
                    context.CheckListCategories.Add(entity);
                }
                else
                {
                    context.CheckListCategories.Attach(entity);
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.EntityState.Modified);
                }
                context.SaveChanges();
            }
            SessionData.CurrentSession.CheckListCategoryList = null;
            SessionData.CurrentSession.ServiceCheckListList = null;
            return ListView();
        }

        public ActionResult EditCategory(Guid id)
        {
            return View("CreateCheckListCat", SessionData.CurrentSession.CheckListCategoryList.FirstOrDefault(p => p.Id == id));
        }
        //
        // GET: /ServiceCheckLists/Edit/5

        public ActionResult Edit(Guid id)
        {
            return View("CreateEdit", SessionData.CurrentSession.ServiceCheckListList.FirstOrDefault(p => p.Id == id));
        }

        //
        // GET: /ServiceCheckLists/Delete/5

        public ActionResult Delete(Guid sclId)
        {
            using (var db = new SCMSEntities())
            {
                var sc = new ServiceCheckList() { Id = sclId };
                db.ServiceCheckLists.Attach(sc);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(sc, System.Data.EntityState.Deleted);
                db.SaveChanges();
            }
            SessionData.CurrentSession.CheckListCategoryList = null;
            SessionData.CurrentSession.ServiceCheckListList = null;
            return ListView();
        }

        public ActionResult DeleteCategory(Guid CatId)
        {
            using (var db = new SCMSEntities())
            {
                var clc = new CheckListCategory() { Id = CatId };
                db.CheckListCategories.Attach(clc);
                ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(clc, System.Data.EntityState.Deleted);
                db.SaveChanges();
            }
            SessionData.CurrentSession.CheckListCategoryList = null;
            SessionData.CurrentSession.ServiceCheckListList = null;
            return ListView();
        }
    }
}

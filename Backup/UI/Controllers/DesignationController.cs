using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic._Designation;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.Resource;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class DesignationController : PortalBaseController
    {
        private IDesignationService designationService;

        public DesignationController(IPermissionService permissionService, IUserContext userContext, IDesignationService _designationService)
            : base(userContext, permissionService)
        {
            this.designationService = _designationService;
        }

        //
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: CreateEdit
        public ActionResult CreateEdit(string did)
        {
            UDesignation designation = new UDesignation();

            Guid designationId;
            if (Guid.TryParse(did, out designationId))
            {
                ViewBag.Action = Resources.Global_String_Update;
                designation._Designation = designationService.GetDesignation(designationId);
            }
            return View(designation);
        }

        //
        // POST: CreateEdit
        [HttpPost]
        public ActionResult CreateEdit(UDesignation designation, string Action)
        {
            if (ModelState.IsValid)
            {
                if (!designation.Id.Equals(Guid.Empty))
                {
                    designation._Designation.CountryProgrammeId = countryProg.Id;
                    if (designationService.EditDesignation(designation._Designation))
                    {
                        designation = new UDesignation();
                        ModelState.Clear();
                    }
                }
                else
                {
                    designation._Designation.CountryProgrammeId = countryProg.Id;
                    if (designationService.AddDesignation(designation._Designation))
                    {
                        designation = new UDesignation();
                        ModelState.Clear();
                    }
                }
            }

            return ListView();
        }

        //
        // GET: ListView
        public ActionResult ListView(string search = "")
        {
            List<Designation> designations = designationService.GetDesignations(countryProg.Id, search);
            return View("ListView", designations);
        }

        //
        // GET: DeleteItem
        public ActionResult DeleteItem(string did)
        {
            Guid designationId;
            if (Guid.TryParse(did, out designationId))
            {
                designationService.DeleteDesignation(designationId);
            }

            List<Designation> designations = designationService.GetDesignations(countryProg.Id);
            return View("ListView", designations);
        }

        public ActionResult GetDesignations(String text)
        {
            return Json(
                new SelectList((designationService.GetDesignations(countryProg.Id, text) ?? new List<Designation>())
                                   .Select(c => new {c.Id, c.Name}).ToList(),
                               "Id",
                               "Name"), JsonRequestBehavior.AllowGet);
        }
    }
}

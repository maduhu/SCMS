using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic._ItemClassification;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.Resource;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class ItemClassificationController : PortalBaseController
    {
        private IItemClassificationService itemClassificationService;

        public ItemClassificationController(IPermissionService permissionService, IUserContext userContext, IItemClassificationService _itemClassificationService)
            : base(userContext, permissionService)
        {
            this.itemClassificationService = _itemClassificationService;
        }

        //
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: CreateEdit
        public ActionResult CreateEdit(string iid)
        {
            UItemClassification itemClassification = new UItemClassification();

            Guid itemClassId;
            if (Guid.TryParse(iid, out itemClassId))
            {
                ViewBag.Action = Resources.Global_String_Edit;
                itemClassification._itemClassification = itemClassificationService.GetItemClassification(itemClassId);
            }
            return View(itemClassification);
        }

        //
        // POST: CreateEdit
        [HttpPost]
        public ActionResult CreateEdit(UItemClassification itemClassification, string Action)
        {
            if (ModelState.IsValid)
            {
                if (Action.Equals("Edit"))
                {
                    itemClassification._itemClassification.CountryProgrammeId = countryProg.Id;
                    if (itemClassificationService.EditItemClassification(itemClassification._itemClassification))
                    {
                        itemClassification = new UItemClassification();
                        ModelState.Clear();
                    }
                }
                else
                {
                    itemClassification._itemClassification.CountryProgrammeId = countryProg.Id;
                    if (itemClassificationService.AddItemClassification(itemClassification._itemClassification))
                    {
                        itemClassification = new UItemClassification();
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
            List<ItemClassification> classificationService = itemClassificationService.GetItemClassifications(countryProg.Id, search);
            return View("ListView", classificationService);
        }

        //
        // GET: DeleteItem
        public ActionResult DeleteItem(string iid)
        {
            Guid itemClassId;
            if (Guid.TryParse(iid, out itemClassId))
            {
                itemClassificationService.DeleteItemClassification(itemClassId);
            }

            List<ItemClassification> classificationService = itemClassificationService.GetItemClassifications(countryProg.Id);
            return View("ListView", classificationService);
        }
    }
}

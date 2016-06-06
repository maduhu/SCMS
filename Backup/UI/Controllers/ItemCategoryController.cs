using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic._ItemCategory;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.Resource;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class ItemCategoryController : PortalBaseController
    {
        private IItemCategoryService itemCategoryService;

        public ItemCategoryController(IPermissionService permissionService, IUserContext userContext, IItemCategoryService _itemCategoryService)
            : base(userContext, permissionService)
        {
            this.itemCategoryService = _itemCategoryService;
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
            UItemCategory itemCategory = new UItemCategory();

            Guid itemCatId;
            if (Guid.TryParse(iid, out itemCatId))
            {
                ViewBag.Action = Resources.Global_String_Edit;
                itemCategory._ItemCategory = itemCategoryService.GetItemCategory(itemCatId);
            }
            return View(itemCategory);
        }

        //
        // POST: CreateEdit
        [HttpPost]
        public ActionResult CreateEdit(UItemCategory itemCategory, string Action)
        {
            if (ModelState.IsValid)
            {
                if (Action.Equals("Edit"))
                {
                    if (itemCategoryService.EditItemCategory(itemCategory._ItemCategory))
                    {
                        itemCategory = new UItemCategory();
                        ModelState.Clear();
                    }
                }
                else
                {
                    if (itemCategoryService.AddItemCategory(itemCategory._ItemCategory))
                    {
                        itemCategory = new UItemCategory();
                        ModelState.Clear();
                    }
                }
            }
            return View(itemCategory);
        }

        //
        // GET: ListView
        public ActionResult ListView(string search = "")
        {
            List<ItemCategory> itemCategories = itemCategoryService.GetItemCategories(search);
            return View(itemCategories);
        }

        //
        // GET: DeleteItem
        public ActionResult DeleteItem(string iid)
        {
            Guid itemCatId;
            if (Guid.TryParse(iid, out itemCatId))
            {
                itemCategoryService.DeleteItemCategory(itemCatId);
            }

            List<ItemCategory> itemCategories = itemCategoryService.GetItemCategories();
            return View("ListView", itemCategories);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic._Supplier;
using SCMS.UI.Models;
using System.IO;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.UI.GeneralHelper;
using SCMS.Resource;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class SupplierController : PortalBaseController
    {
        private ISupplierService supplierService;

        public SupplierController(IPermissionService permissionService, IUserContext userContext, ISupplierService _supplierService)
            : base(userContext, permissionService)
        {
            this.supplierService = _supplierService;
        }

        //
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: CreateEdit
        public ActionResult CreateEdit(string sid)
        {

            USupplier supplier = new USupplier();
            supplier.CountrySelect = new SelectList(supplierService.CountryService.GetCountries(), "Id", "Name");
            supplier.CountryId = countryProg.CountryId.ToString();
            Guid supplierId;
            if (Guid.TryParse(sid, out supplierId))
            {
                ViewBag.Action = Resources.Global_String_Update;
                supplier._Supplier = supplierService.GetSupplier(supplierId);
            }
            return View(supplier);
        }

        //
        // POST: CreateEdit
        [HttpPost]
        public ActionResult CreateEdit(USupplier supplier, string Action = "")
        {
            if (ModelState.IsValid)
            {
                // save log file
                if (supplier.Logo != null && supplier.Logo.ContentLength > 0)
                {
                    string path;
                    var fileName = Path.GetFileName(supplier.Logo.FileName);
                    if (Action.Equals("Edit") && !string.IsNullOrWhiteSpace(supplier.LogoLocation))
                    {
                        path = Path.Combine(Server.MapPath("~/App_Data/Logos"), supplier.LogoLocation);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    } 

                    path = Path.Combine(Server.MapPath("~/App_Data/Logos"), fileName);
                    supplier.Logo.SaveAs(path);
                    supplier._Supplier.LogoLocation = fileName;
                }

                supplier._Supplier.CountryProgrammeId = countryProg.Id;
                supplier._Supplier.IsApproved = true;

                if (Action.Equals("Edit"))
                {
                    if (supplierService.EditSupplier(supplier._Supplier))
                    {
                        ModelState.Clear();
                        supplier = new USupplier();
                    }
                }
                else
                {
                    if (supplierService.AddSupplier(supplier._Supplier))
                    {
                        ModelState.Clear();
                        supplier = new USupplier();
                    }
                }
            }
            supplier.CountrySelect = new SelectList(supplierService.CountryService.GetCountries(), "Id", "Name");
            return ListView();
        }

        //
        // GET: ListView
        public ActionResult ListView(string search = "")
        {
            List<SupplierService.SupplierServiceView> suppliers = supplierService.GetSuppliers1(countryProg.Id, search);
            return View("ListView", suppliers);
        }

        //
        // GET: DeleteItem
        public ActionResult DeleteItem(string sid)
        {
            Guid supplierId;
            if (Guid.TryParse(sid, out supplierId))
            {
                Supplier supplier = supplierService.GetSupplier(supplierId);
                if (supplier != null)
                {
                    if (!string.IsNullOrWhiteSpace(supplier.LogoLocation))
                    {
                        var path = Path.Combine(Server.MapPath("~/App_Data/Logos"), supplier.LogoLocation);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }

                    if (supplierService.DeleteSupplier(supplierId))
                    {
                        ModelState.Clear();
                    }
                }
            }

            List<SupplierService.SupplierServiceView> suppliers = supplierService.GetSuppliers1(countryProg.Id);
            return View("ListView", suppliers);
        }

        public ActionResult SearchSuppliers()
        {
            string searchTerm = Request.QueryString["q"];
            if (UserSession.CurrentSession.SupplierList == null)
                UserSession.CurrentSession.SupplierList = supplierService.GetSuppliers(countryProg.Id);
            string searchResults = "";
            foreach (Supplier supplier in UserSession.CurrentSession.SupplierList)
            {
                if (supplier.Name.StartsWith(searchTerm, true, System.Globalization.CultureInfo.CurrentCulture))
                    searchResults += supplier.Name + "\n";
            }
            searchResults = searchResults != "" ? searchResults : "\n";
            return Content(searchResults, "text/html");
        }
        
    }
}

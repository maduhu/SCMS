using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.ProcurementPlan;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using SCMS.Model;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.OrderRequest;
using SCMS.CoreBusinessLogic._CountrySubOffice;
using System.Text;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.CoreBusinessLogic._Item;
using SCMS.Resource;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;
using System.Collections;

namespace SCMS.UI.Controllers
{
    public class ProcurementPlanController : PortalBaseController
    {
        private IProcurementPlanService ppService;
        private IOrderRequest orService;
        private ICountrySubOfficeService csoService;
        private INotificationService notificationService;
        private IItemService itemService;
        private static int TotalPP;

        public ProcurementPlanController(IProcurementPlanService ppService, IOrderRequest orService, ICountrySubOfficeService csoService, IUserContext uc,
            IPermissionService permService, INotificationService notificationService, IItemService itemService)
            : base(uc, permService)
        {
            this.ppService = ppService;
            this.orService = orService;
            this.csoService = csoService;
            this.notificationService = notificationService;
            this.itemService = itemService;
        }
        //
        // GET: /ProcurementPlan/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewPPList()
        {
            ViewData["total"] = TotalPP;
            //List<ProcurementPlan> model = ppService.GetProcurementPlans(countryProg.Id);
            return View("ViewPPList");
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _GenPPCustomBinding(GridCommand command)
        {
            var gridModel = (ppService.GetProcurementPlans(countryProg.Id)
                .Select(n => new Model.PPModelView()
                {
                    Id = n.Id,
                    RefNo = n.RefNumber,
                    ProjectName = n.ProjectDonor.Project.Name,
                    ProjectNo = n.ProjectDonor.ProjectNumber,
                    DonorShortName = n.ProjectDonor.Donor.ShortName,
                    CountrySubOffice = n.CountrySubOffice.Name,
                    PreparedOn = n.PreparedOn.Date

                }).AsQueryable())
                  .ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            TotalPP = ppService.GetProcurementPlans(countryProg.Id).Count();

            IEnumerable data = gridModel.Data.AsQueryable().ToIList();
            return View(new GridModel { Data = data, Total = TotalPP });
        }

        public ActionResult ViewPP(Guid id)
        {
            ProcurementPlan model = ppService.GetProcurementPlanById(id);

            model.CanApprove = notificationService.CanApprove(currentUser, NotificationHelper.ppCode, NotificationHelper.approvalCode, model.Id);
            model.CanApproveII = notificationService.CanApprove(currentUser, NotificationHelper.ppCode, NotificationHelper.approvalIICode, model.Id);
            model.CanReview = notificationService.CanApprove(currentUser, NotificationHelper.ppCode, NotificationHelper.reviewCode, model.Id);
            model.CanAuthorize = notificationService.CanApprove(currentUser, NotificationHelper.ppCode, NotificationHelper.authorizationCode, model.Id);

            model.PPItemList = model.ProcurementPlanItems.Where(p => p.IsAuthorized).OrderBy(p => p.Item.Name).ToList();
            return View("ViewPP", model);
        }

        public ActionResult ViewPPItems(Guid id, string type)
        {
            List<PPItemModel> model = new List<PPItemModel>();
            List<Model.ProcurementPlanItem> ppItems = ppService.GetProcurementPlanItems(id);
            if (type == "all")
            {
                foreach (ProcurementPlanItem ppItem in ppItems)
                {
                    if (ppItem.IsApproved)
                        model.Add(new PPItemModel { EntityPPItem = ppItem });
                }
                ViewBag.AllStyle = "font-weight: bold;";
                ViewBag.NoResultsMsg = Resources.ProcurementPlanController_String_NoItemsSet;
            }
            else if (type == "active")
            {
                foreach (ProcurementPlanItem ppItem in ppItems)
                {
                    if (ppItem.AddedToOR || !ppItem.IsApproved)
                        continue;
                    model.Add(new PPItemModel { EntityPPItem = ppItem });
                }
                ViewBag.ActiveStyle = "font-weight: bold;";
                ViewBag.NoResultsMsg = Resources.ProcurementPlanController_String_NoActiveItems;
            }
            else if (type == "inactive")
            {
                foreach (ProcurementPlanItem ppItem in ppItems)
                {
                    if (!ppItem.AddedToOR || !ppItem.IsApproved)
                        continue;
                    model.Add(new PPItemModel { EntityPPItem = ppItem });
                }
                ViewBag.InActiveStyle = "font-weight: bold;";
                ViewBag.NoResultsMsg = Resources.ProcurementPlanController_String_NoInactiveItems;
            }
            else if (type == "removed")
            {
                foreach (ProcurementPlanItem ppItem in ppItems)
                {
                    if (!ppItem.IsRemoved || !ppItem.IsApproved)
                        continue;
                    model.Add(new PPItemModel { EntityPPItem = ppItem });
                }
                ViewBag.RemovedStyle = "font-weight: bold;";
                ViewBag.NoResultsMsg = Resources.ProcurementPlanController_String_NoRemovedItems;
            }
            ViewBag.PPId = id;
            return View("PPItemsList", model);
        }

        public ActionResult CreatePP()
        {
            ProcurementPlan model = new ProcurementPlan();
            model.RefNumber = "--NEW PP--";
            model.Projects = new SelectList(orService.GetProjectsWithoutPP(), "Id", "Name");
            model.ProjectDonors = new SelectList(orService.GetProjectNosWithoutPP(Guid.Empty), "Id", "ProjectNumber");
            model.SubOffices = new SelectList(csoService.GetCountrySubOffices(countryProg.Id), "Id", "Name");
            model.PreparedOn = DateTime.Today;
            ViewBag.FormHeader = Resources.ProcurementPlanController_String_NewProcurementPlan;
            return View("CreatePP", model);
        }

        public ActionResult EditPP(Guid Id)
        {
            ProcurementPlan model = ppService.GetProcurementPlanById(Id);
            model.Projects = new SelectList(orService.GetProject(), "Id", "Name", model.ProjectId);
            model.ProjectDonors = new SelectList(orService.GetProjectNos(model.ProjectDonor.ProjectId), "Id", "ProjectNumber", model.ProjectDonorId);
            model.SubOffices = new SelectList(csoService.GetCountrySubOffices(countryProg.Id), "Id", "Name", model.PreparingOfficeId);
            ViewBag.FormHeader = Resources.ProcurementPlanController_String_EditProcurementPlan + " :: " + model.RefNumber;
            ViewBag.Donor = model.ProjectDonor.Donor.Name;
            return View("CreatePP", model);
        }

        [HttpPost]
        public ActionResult SaveProcPlan(ProcurementPlan model)
        {
            model.PreparedBy = currentStaff.Id;
            ppService.SaveProcurementPlan(model);
            return PPDetails(model.Id);
        }

        public ActionResult SubmitPP(Guid id)
        {
            ProcurementPlan pp = ppService.GetProcurementPlanById(id);
            pp.IsSubmitted = true;
            pp.IsRejected = pp.IsApproved1 = pp.IsApproved2 = pp.IsAuthorized = pp.IsReviewed = false;
            pp.PreparedBy = currentStaff.Id;
            pp.PreparedOn = DateTime.Now;
            ppService.SaveProcurementPlan(pp);
            notificationService.SendToAppropriateApprover(NotificationHelper.ppCode, NotificationHelper.approvalCode, pp.Id);
            return ViewPPList();
        }

        public ActionResult PPDetails(Guid Id)
        {
            ProcurementPlan model = ppService.GetProcurementPlanById(Id);
            model.PPItemList = model.ProcurementPlanItems.Where(p => !p.IsApproved).OrderBy(p => p.Item.Name).ToList();
            return View("CreatePPStep2", model);
        }

        public ActionResult GetProjectNumbers(Guid projectId)
        {
            StringBuilder pdOption = new StringBuilder();
            pdOption.Append("<select id=\"dplProNo\" name=\"ProjectDonorId\" onchange=\"javascript:GetDonor(this)\">");
            pdOption.Append("<option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            List<Model.ProjectDonor> pdList = orService.GetProjectNosWithoutPP(projectId);
            foreach (ProjectDonor item in pdList)
                pdOption.Append("<option value=\"" + item.Id + "\">" + item.ProjectNumber + "</option>");
            pdOption.Append("</select>");
            pdOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"ProjectDonorId\" data-valmsg-replace=\"false\">" + Resources.Global_String_Required + "</span>");
            return Content(pdOption.ToString(), "text/html");
        }

        public ActionResult CreatePPItem(Guid ppId)
        {
            ProcurementPlanItem model = new ProcurementPlanItem();
            ProcurementPlan pp = ppService.GetProcurementPlanById(ppId);
            model.ProcurementPlanId = ppId;
            model.Items = new SelectList(orService.GetItems(), "Id", "Name");
            model.BudgetLines = new SelectList(orService.GetProjectBugdetLines(pp.ProjectDonorId), "Id", "Description");
            model.CurrencyId = countryProg.Country.CurrencyId.Value;
            model.Currencies = new SelectList(orService.GetCurrencies(), "Id", "ShortName", countryProg.Country.CurrencyId);
            ViewBag.Action = Resources.Global_String_AddItem;
            return View("CreatePPItem", model);
        }

        public ActionResult SaveProcPlanItem(ProcurementPlanItem model)
        {
            ppService.SaveProcurementPlanItem(model);
            return PPDetails(model.ProcurementPlanId);
        }

        public ActionResult EditPPItem(Guid id)
        {
            ProcurementPlanItem model = ppService.GetProcurementPlanItemById(id);
            model.Items = new SelectList(orService.GetItems(), "Id", "Name", model.ItemId);
            model.BudgetLines = new SelectList(orService.GetProjectBugdetLines(model.ProcurementPlan.ProjectDonorId), "Id", "Description", model.BudgetLineId);
            model.UnitCost = Math.Round(model.UnitCost, 2);
            model.TotalCost = Math.Round(model.TotalCost, 2);
            model.Currencies = new SelectList(orService.GetCurrencies(), "Id", "ShortName", model.CurrencyId);
            ViewBag.Action = Resources.Global_String_Update;
            model.EditMode = true;
            return View("CreatePPItem", model);
        }

        public ActionResult RemovePPItem(Guid id, Guid ppId)
        {
            //ProcurementPlanItem ppItem = ppService.GetProcurementPlanItemById(id);
            ppService.DeleteProcurementPlanItem(id);
            return PPDetails(ppId);
        }

        public ActionResult DeletePP(Guid id)
        {
            ppService.DeleteProcumentPlan(id);
            return ViewPPList();
        }

        [HttpGet]
        public ActionResult AddNewItem(Guid ppId, bool editMode = false)
        {
            UItem model = new UItem
            {
                ItemCatSelect = new SelectList(itemService.ItemCatObj.GetItemCategories(), "Id", "CategoryName"),
                ItemClassSelect = new SelectList(itemService.ItemClassObj.GetItemClassifications(countryProg.Id), "Id", "Name"),
                ItemUnitsSelect = new SelectList(itemService.ItemClassObj.GetUnitOfMessures(countryProg.Id), "Id", "Code"),
                EditMode = editMode
            };
            ViewBag.PPId = ppId;
            return View("AddNewItem4PP", model);
        }

        [HttpPost]
        public ActionResult AddNewItem(UItem model, Guid ppId, bool editMode)
        {
            Item item = itemService.GetItemByName(model.Name, countryProg.Id);
            if (item == null)
            {
                item = new Item();
                item.Name = model.Name;
                item.ItemCategoryId = model.ItemCategoryId;
                item.ItemClassificationId = model.ItemClassificationId;
                item.UnitOfMessureId = model.UnitId;
                item.CountryProgrammeId = countryProg.Id;
                item.IsApproved = false;
                itemService.AddItem(item);
            }
            if (editMode)
                return EditPPItem(ppId);
            return CreatePPItem(ppId);
        }

    }
}

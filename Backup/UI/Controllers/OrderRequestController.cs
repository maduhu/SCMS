using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.OrderRequest;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.UI.Models;
using System.Data.Objects.DataClasses;
using SCMS.UI.GeneralHelper;
using System.Text;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.CoreBusinessLogic._Location;
using SCMS.CoreBusinessLogic._Item;
using SCMS.CoreBusinessLogic.ProcurementPlan;
using SCMS.CoreBusinessLogic._ExchangeRate;
using SCMS.CoreBusinessLogic._Currency;
using SCMS.Utils.DTOs;
using SCMS.Utils.DataTables;
using SCMS.Resource;
using SCMS.CoreBusinessLogic.Budgeting;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;
using System.Collections;
using System.IO;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class OrderRequestController : PortalBaseController
    {
        private IOrderRequest orderRequestService;
        private INotificationService notificationService;
        private ILocationService locationService;
        private IItemService itemService;
        private IProcurementPlanService ppService;
        private IExchangeRateService exchangeRateService;
        private ICurrencyService currencyService;
        private IBudgetService budgetService;
        private static int TotalOR;

        public OrderRequestController(IPermissionService permissionService, IOrderRequest orderRequestService, IUserContext userContext,
            INotificationService notificationService, ILocationService locationService, IItemService itemService, IProcurementPlanService ppService,
            IExchangeRateService exchangeRateService, ICurrencyService currencyService, IBudgetService budgetService)
            : base(userContext, permissionService)
        {
            this.orderRequestService = orderRequestService;
            this.notificationService = notificationService;
            this.locationService = locationService;
            this.itemService = itemService;
            this.ppService = ppService;
            this.exchangeRateService = exchangeRateService;
            this.currencyService = currencyService;
            this.budgetService = budgetService;
        }

        //
        // GET: /OrderRequest/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadOrderRequest(bool fromPP = false)
        {
            List<Model.Project> projects = fromPP ? orderRequestService.GetProjectsWithPP() : orderRequestService.GetProject();
            Model.OrderRequest model = new Model.OrderRequest
            {
                OrderDate = DateTime.Today,
                Currencies = new SelectList(orderRequestService.GetCurrencies(), "Id", "ShortName"),
                Projects = new SelectList(projects, "Id", "Name"),
                ProjectDonors = new SelectList(orderRequestService.GetProjectNos(Guid.Empty), "Id", "ProjectNumber"),
                Locations = new SelectList(orderRequestService.GetLocations(), "Id", "Name"),
                RefNumber = string.Format("--{0}--", Resources.Global_String_NewOR),
                CurrencyId = countryProg.Country.CurrencyId.HasValue ? countryProg.Country.CurrencyId.Value : mbCurrency.Id,
                FromPP = fromPP
            };
            return View("LoadOrderRequest", model);
        }

        public ActionResult EditOrderRequestFromPP()
        {
            List<Model.Project> projects = orderRequestService.GetProjectsWithPP();
            UserSession.CurrentSession.NewOR.OrderDate = DateTime.Today;
            UserSession.CurrentSession.NewOR.Currencies = new SelectList(orderRequestService.GetCurrencies(), "Id", "ShortName", UserSession.CurrentSession.NewOR.CurrencyId);
            UserSession.CurrentSession.NewOR.Projects = new SelectList(projects, "Id", "Name", UserSession.CurrentSession.NewOR.ProjectId);
            UserSession.CurrentSession.NewOR.ProjectDonors = new SelectList(orderRequestService.GetProjectNos(UserSession.CurrentSession.NewOR.ProjectId), "Id", "ProjectNumber", UserSession.CurrentSession.NewOR.ProjectDonorId);
            UserSession.CurrentSession.NewOR.Locations = new SelectList(orderRequestService.GetLocations(), "Id", "Name");
            UserSession.CurrentSession.NewOR.RefNumber = string.Format("--{0}--", Resources.Global_String_NewOR);
            UserSession.CurrentSession.NewOR.CurrencyId = countryProg.Country.CurrencyId.HasValue ? countryProg.Country.CurrencyId.Value : mbCurrency.Id;
            UserSession.CurrentSession.NewOR.FromPP = true;
            ViewBag.Donor = orderRequestService.GetProjectDonorById(UserSession.CurrentSession.NewOR.ProjectDonorId.Value).Donor.ShortName;
            return View("LoadOrderRequest", UserSession.CurrentSession.NewOR);
        }

        public ActionResult EditOrderRequest(Guid id)
        {
            Model.OrderRequest model = orderRequestService.GetOrderRequestById(id);
            if (model == null)
            {
                if (UserSession.CurrentSession.NewOR.Id.Equals(id))
                {
                    model = UserSession.CurrentSession.NewOR;
                    model.ProjectDonor = orderRequestService.GetProjectDonorById(model.ProjectDonorId.Value);
                }
            }
            model.Currencies = new SelectList(orderRequestService.GetCurrencies(), "Id", "ShortName");
            model.Projects = new SelectList(orderRequestService.GetProject(), "Id", "Name");
            model.ProjectDonors = new SelectList(orderRequestService.GetProjectNos(model.ProjectId), "Id", "ProjectNumber");
            model.Locations = new SelectList(orderRequestService.GetLocations(), "Id", "Name");
            return View("EditOrderRequest", model);
        }

        public ActionResult GetProjectNos(Guid id)
        {
            StringBuilder blOption = new StringBuilder();
            blOption.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"dplProNo\" name=\"ProjectDonorId\" onchange=\"javascript:GetDonor(this)\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            List<Model.ProjectDonor> BLines = orderRequestService.GetProjectNos(id);
            foreach (ProjectDonor item in BLines)
                blOption.Append("<option value=\"" + item.Id + "\">" + item.ProjectNumber + " (" + item.Donor.ShortName + ") " + "</option>");
            blOption.Append("</select>");
            blOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"ProjectDonorId\" data-valmsg-replace=\"true\"></span>");
            ViewBag.Html = blOption.ToString();
            return View("SelectIndexChangeResponse");
        }

        public ActionResult GetProjectNos4RegAsset(Guid id)
        {
            StringBuilder blineOption = new StringBuilder();
            blineOption.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"dplProNo\" name=\"EntityAsset.CurrentProjectDonorId\" onchange=\"javascript:GetAssetNo(this)\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            List<Model.ProjectDonor> BLines = orderRequestService.GetProjectNos(id);
            foreach (ProjectDonor item in BLines)
                blineOption.Append("<option value=\"" + item.Id + "\">" + item.ProjectNumber + " (" + item.Donor.ShortName + ") " + "</option>");
            blineOption.Append("</select>");
            blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"EntityAsset.CurrentProjectDonorId\" data-valmsg-replace=\"true\"></span>");
            ViewBag.Html = blineOption.ToString();
            return View("SelectIndexChangeResponse");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveOrderRequest(Model.OrderRequest entity)
        {
            entity.Id = Guid.NewGuid();
            entity.CountryProgrammeId = countryProg.Id;
            entity.PreparedBy = currentStaff.Id;
            entity.TotalAmount = 0;
            entity.IsReviewed = false;
            entity.IsAuthorized = false;
            entity.PreparedOn = DateTime.Now;

            UserSession.CurrentSession.NewOR = entity;
            if (entity.FromPP)
                return AddPPItems2OR(entity.ProjectDonorId.Value, entity);
            return PopulateCreateItem(entity.Id);
        }

        public ActionResult AddPPItems2OR(Guid pdId, Model.OrderRequest or)
        {
            ProcurementPlan model = ppService.GetProcurementPlanByProjectId(pdId);
            if (model == null)
            {
                return Content("<i style=\"color: Red\">" + Resources.OrderRequestController_String_NoPPItems + "</i>", "text/html");
            }
            model.PPItemList = model.ProcurementPlanItems.Where(p => (p.Quantity - p.ProcuredAmount) > 0).OrderBy(p => p.Item.Name).ToList();
            model.OrderRequestId = or.Id;
            Currency orCurr = currencyService.GetCurrency(or.CurrencyId);
            foreach (ProcurementPlanItem ppItem in model.PPItemList)
            {
                ppItem.UnitCost = Math.Round(exchangeRateService.GetForeignCurrencyValue(or.CurrencyId, ppItem.CurrencyId, ppItem.UnitCost, countryProg.Id), 2);
                ppItem.QuantityToOrder = ppItem.Quantity - ppItem.ProcuredAmount;
                ppItem.TotalCost = Math.Round((ppItem.UnitCost * ppItem.QuantityToOrder), 2);
            }
            //model.PPItemList = model.ProcurementPlanItems.Where(p => !p.AddedToOR && p.IsApproved && !p.IsRemoved).OrderBy(p => p.Item.Name).ToList();
            ViewBag.Currency = orCurr.ShortName;
            return View("AddPPItems2OR", model);
        }

        [HttpPost]
        public ActionResult AddPPItems2OR(ProcurementPlan model)
        {
            Model.OrderRequest or = UserSession.CurrentSession.NewOR != null ? UserSession.CurrentSession.NewOR : orderRequestService.GetOrderRequestById(model.OrderRequestId);
            Model.OrderRequestItem orItem;
            ProcurementPlanItem ppItem;
            foreach (ProcurementPlanItem ppItemModel in model.PPItemList)
            {
                if (!ppItemModel.AddedToOR)
                    continue;
                ppItem = ppService.GetProcurementPlanItemById(ppItemModel.Id);
                orItem = new OrderRequestItem();
                orItem.BudgetLineId = ppItem.BudgetLineId;
                orItem.EstimatedUnitPrice = exchangeRateService.GetForeignCurrencyValue(or.CurrencyId, ppItem.CurrencyId, ppItemModel.UnitCost, countryProg.Id);
                orItem.EstimatedPrice = orItem.EstimatedUnitPrice * ppItemModel.QuantityToOrder;
                orItem.Quantity = ppItemModel.QuantityToOrder;
                orItem.ItemDescription = ppItem.ItemDescription;
                orItem.ItemId = ppItem.ItemId;
                orItem.OrderRequestId = model.OrderRequestId;
                orItem.ProcurementPlanItemId = ppItem.Id;
                orderRequestService.AddOrderRequstItem(orItem, UserSession.CurrentSession.NewOR);
                if (UserSession.CurrentSession.NewOR != null)
                    UserSession.CurrentSession.NewOR = null;
            }

            return ViewOrderRequestItems(model.OrderRequestId);
        }

        public ActionResult UpdateOrderRequest(Model.OrderRequest entity)
        {
            bool projectChange = false;
            Model.OrderRequest or = orderRequestService.GetOrderRequestById(entity.Id);
            or.ProjectId = entity.ProjectId;
            or.CurrencyId = entity.CurrencyId;
            //check if project changed
            if (!or.ProjectDonorId.Value.Equals(entity.ProjectDonorId.Value))
                projectChange = true;
            or.ProjectDonorId = entity.ProjectDonorId;
            or.RequestedDestinationId = entity.RequestedDestinationId;
            or.FinalDestinationId = entity.FinalDestinationId;
            or.OrderDate = entity.OrderDate;
            or.DeliveryDate = entity.DeliveryDate;
            orderRequestService.UpdateORWithPossibleProjectChange(or, projectChange);
            return ViewOrderRequestItems(or.Id);
        }

        public ActionResult DeleteOrderRequest(Guid id)
        {
            orderRequestService.DeleteOrderRequst(id);
            return ViewOrderRequests();
        }

        public ActionResult LoadRequestItem(Guid id)
        {
            return PopulateCreateItem(id);
        }

        private ActionResult PopulateCreateItem(Guid id, string errormessage = "", Guid? selectedItem = null)
        {
            Model.OrderRequest or = UserSession.CurrentSession.NewOR == null ? orderRequestService.GetOrderRequestById(id) : UserSession.CurrentSession.NewOR;
            List<BudgetLineView> blList = orderRequestService.GetProjectBugdetLines((Guid)or.ProjectDonorId);
            OrderRequestItem model = new OrderRequestItem
            {
                BudgetLines = new SelectList(blList, "Id", "Description"),
                Items = new SelectList(orderRequestService.GetItems(), "Id", "Name"),
                OrderRequestId = id
            };

            if (selectedItem.HasValue)
            {
                model.ItemId = selectedItem.Value;
            }
            return View("LoadRequestItem", model);
        }

        public ActionResult AddItemInEditMode(Guid id)
        {
            Model.OrderRequest or = orderRequestService.GetOrderRequestById(id);
            List<BudgetLineView> blList = orderRequestService.GetProjectBugdetLines((Guid)or.ProjectDonorId);
            OrderRequestItem model = new OrderRequestItem
            {
                Id = Guid.Empty,
                BudgetLines = new SelectList(blList, "Id", "Description"),
                Items = new SelectList(orderRequestService.GetItems(), "Id", "Name"),
                OrderRequestId = id
            };

            ViewBag.Action = Resources.Global_String_Add;
            return View("AddItemInEditMode", model);
        }

        public ActionResult EditItem(Guid id)
        {
            Model.OrderRequestItem model = orderRequestService.GetOrderRequestItemById(id);
            List<BudgetLineView> blList = orderRequestService.GetProjectBugdetLines((Guid)model.ProjectBudget.BudgetCategory.ProjectDonorId);
            model.BudgetLines = new SelectList(blList, "Id", "Description");
            model.Items = new SelectList(orderRequestService.GetItems(), "Id", "Name");

            ViewBag.Action = Resources.Global_String_Update;
            return View("LoadRequestItem", model);
        }


        public ActionResult EditItemInEditMode(Guid id)
        {
            Model.OrderRequestItem model = orderRequestService.GetOrderRequestItemById(id);
            List<BudgetLineView> blList = orderRequestService.GetProjectBugdetLines((Guid)model.ProjectBudget.BudgetCategory.ProjectDonorId);

            model.BudgetLines = new SelectList(blList, "Id", "Description", model.ProjectBudget.Id);
            model.Items = new SelectList(orderRequestService.GetItems(), "Id", "Name");
            ViewBag.Action = Resources.Global_String_Update;
            return View("AddItemInEditMode", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LoadRequestItem(Guid id, OrderRequestItem entity)
        {
            if (!orderRequestService.ItemAlreadyAddedToOR(entity.ItemId, id))
            {
                if (entity.OrderRequestId.Equals(Guid.Empty))
                    entity.OrderRequestId = id;
                else
                    id = entity.OrderRequestId;
                if (orderRequestService.AddOrderRequstItem(entity, UserSession.CurrentSession.NewOR))
                {
                    ViewBag.OrderRequestID = entity.OrderRequestId;
                    ViewBag.Response = 1;
                    ViewBag.msg = Resources.OrderRequestController_String_ORItemSaved;
                    UserSession.CurrentSession.NewOR = null;
                }
                ModelState.Clear();
            }
            else { ViewBag.Response = 0; ViewBag.msg = Resources.OrderRequestController_String_ItemAlreadyAdded; }

            return ViewOrderRequestItems(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveRequestItem(Guid orItemId, Model.OrderRequestItem entity)
        {
            if (!orderRequestService.ItemAlreadyAddedToOR(entity.ItemId, entity.OrderRequestId) || !orItemId.Equals(Guid.Empty))
            {
                entity.Id = orItemId;
                if (orderRequestService.AddOrderRequstItem(entity, UserSession.CurrentSession.NewOR))
                {
                    ViewBag.Response = 1;
                    ViewBag.msg = Resources.OrderRequestController_String_ORItemSaved;
                }
                ModelState.Clear();
            }
            else { ViewBag.Response = 0; ViewBag.msg = Resources.OrderRequestController_String_ItemAlreadyAdded; }
            return ViewOrderRequestItems(entity.OrderRequestId);
        }

        public ActionResult RemoveItem(Guid id)
        {
            orderRequestService.DeleteOrderRequestItem(id);
            ViewBag.Response = 1;
            ViewBag.msg = Resources.OrderRequestController_String_ORItemDeleted;
            List<OrderRequestItem> model = orderRequestService.GetOrderRequestById(id).OrderRequestItems.ToList();
            ViewBag.OrderRequestID = model.Count > 0 ? model[0].OrderRequestId : Guid.Empty;

            return View("OrderRequestItemsList", model);
        }

        public ActionResult RemoveItemInEditMode(Guid id, Guid orId)
        {
            orderRequestService.DeleteOrderRequestItem(id);
            ViewBag.Response = 1;
            ViewBag.msg = Resources.OrderRequestController_String_ORItemDeleted;
            ModelState.Clear();
            return ViewOrderRequestItems(orId);
        }

        public ActionResult GetDonorName(Guid id)
        {
            StringBuilder blineOption = new StringBuilder();
            using (var dbcontenxt = new SCMSEntities())
            {
                Model.ProjectDonor pd = dbcontenxt.ProjectDonors.SingleOrDefault(p => p.Id == id);
                blineOption.Append("<label for=\"EntityOrderRequest_ProjectDonor_Donor_Name\">" + pd.Donor.Name + "</label>");
            }
            ViewBag.Html = blineOption.ToString();
            return View("SelectIndexChangeResponse");
        }

        public ActionResult SubmitOR(Guid Id)
        {
            Model.OrderRequest or = orderRequestService.GetOrderRequestById(Id);
            //Run another check on funds availability
            List<BudgetCheckResult> bcrList = orderRequestService.RunFundsAvailableCheck(Id);
            if (bcrList.Count > 0)
                return ViewOrderRequestItems(Id, bcrList);//Render OR with message saying funds not sufficient
            or.IsRejected = false;
            or.IsAuthorized = false;
            or.IsReviewed = false;
            or.IsSubmitted = true;
            or.PreparedBy = currentStaff.Id;
            or.PreparedOn = or.OrderDate = DateTime.Now;
            or.RefNumber = orderRequestService.GenerateUniquNumber(countryProg);
            orderRequestService.UpdateOrderRequest(or);
            //Send notification
            notificationService.SendToAppropriateApprover(NotificationHelper.orCode, NotificationHelper.approvalCode, or.Id);
            return ViewOrderRequests();
        }

        [HttpPost]
        public ActionResult ViewOrderRequestsWithPaging(DataTablesPageRequest gridParams)
        {
            int page = gridParams.DisplayStart / (gridParams.DisplayLength == 0 ? 1 : gridParams.DisplayLength);

            //countryProg.Id

            Dictionary<String, String> options = new Dictionary<string, string>();
            options.Add("CountryProgrammeId", countryProg.Id.ToString().ToUpper());

            OrderRequestPagePacket packet = orderRequestService.getPagedOrderRequests(page, gridParams.DisplayLength, options);

            return Json(new
            {
                iTotalRecords = packet.TotalOrders,
                iTotalDisplayRecords = packet.TotalOrders,
                aaData = packet.Orders,
                sEcho = gridParams.Echo
            },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult List()
        {
            Dictionary<String, String> options = new Dictionary<string, string>();
            options.Add("CountryProgrammeId", countryProg.Id.ToString().ToUpper());

            OrderRequestPagePacket packet = orderRequestService.getPagedOrderRequests(1, 10, options);

            return View("ViewPagedOrderRequests");
        }

        public ActionResult ViewOrderRequests()
        {
            ViewData["total"] = TotalOR;
            return View("ViewOrderRequests");
        }

        private string ORStatus(Model.OrderRequest or, out DateTime statusDate)
        {
            string orStatus;
            if (or.IsAuthorized == true)
                orStatus = Resources.Global_String_StatusAU;
            else if (or.IsRejected == true)
                orStatus = Resources.Global_String_StatusRJ;
            else if (or.IsReviewed == true)
                orStatus = Resources.Global_String_StatusRV;
            else if (or.IsApproved)
                orStatus = Resources.Global_String_StatusAP;
            else if (or.IsSubmitted == true)
                orStatus = Resources.Global_String_StatusCR;
            else
                orStatus = Resources.Global_String_StatusNEW;

            //Get OR Status date
            if (or.IsAuthorized == true)
                statusDate = or.AuthorizedOn.Value;
            else if (or.IsRejected == true)
            {
                if (or.AuthorizedOn.HasValue)
                    statusDate = or.AuthorizedOn.Value;
                else if (or.ReviewedOn.HasValue)
                    statusDate = or.ReviewedOn.Value;
                else
                    statusDate = or.ApprovedOn.Value;
            }
            else if (or.IsReviewed == true)
                statusDate = or.ReviewedOn.Value;
            else if (or.IsApproved)
                statusDate = or.ApprovedOn.Value;
            else if (or.IsSubmitted == true)
                statusDate = or.PreparedOn.Value;
            else
                statusDate = or.PreparedOn.Value;

            return orStatus;
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _GenORCustomBinding(GridCommand command)
        {
            DateTime statusDate;
            var gridModel = (orderRequestService.GetOrderRequests()
                .Select(n => new Model.ORViewModel()
                {
                    Id = n.Id,
                    RefNumber = n.RefNumber,
                    ProjectNumber = n.ProjectDonor.ProjectNumber + " (" + n.ProjectDonor.Donor.ShortName + ")",
                    FirstItem = n.OrderRequestItems.Count > 0 ? n.OrderRequestItems.ToList()[0].Item.Name : string.Empty,
                    ORValue = (float)(n.TotalAmount),
                    Requestor = n.Staff2.StaffName,
                    Status = ORStatus(n, out statusDate),
                    StatusDate = statusDate.Date

                }).AsQueryable())
                  .ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            TotalOR = orderRequestService.GetOrderRequests().Count();

            IEnumerable data = gridModel.Data.AsQueryable().ToIList();
            return View(new GridModel { Data = data, Total = TotalOR });
        }

        public ActionResult ExportCsv(int page, int pageSize, string orderBy, string filter, string groupBy)
        {
             DateTime statusDate;
            var gridModel = orderRequestService.GetOrderRequests()
                .Select(n => new Model.ORViewModel()
                {
                    Id = n.Id,
                    RefNumber = n.RefNumber,
                    ProjectNumber = n.ProjectDonor.ProjectNumber + " (" + n.ProjectDonor.Donor.ShortName + ")",
                    FirstItem = n.OrderRequestItems.Count > 0 ? n.OrderRequestItems.ToList()[0].Item.Name : string.Empty,
                    ORValue = (float)(n.TotalAmount),
                    Requestor = n.Staff2.StaffName,
                    Status = ORStatus(n, out statusDate),
                    StatusDate = statusDate.Date

                });

            IEnumerable ors = gridModel.AsQueryable().ToGridModel(page, pageSize, orderBy, string.Empty, filter).Data;
            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            writer.Write(Resources.Global_String_RefNo + ",");
            writer.Write(Resources.Budget_CategoryList_ProjectNo + ",");
            writer.Write(Resources.OrderRequest_ViewOrderRequests_FirstItem + ",");
            writer.Write(Resources.OrderRequest_String_ORValue + ",");
            writer.Write(Resources.Global_String_Requester + ",");
            writer.Write(Resources.Global_String_Status + ",");
            writer.Write(Resources.Global_String_StatusDate);
            writer.WriteLine();
            foreach (ORViewModel order in ors)
            {
                writer.Write(order.RefNumber);
                writer.Write(",");
                writer.Write("\"");
                writer.Write(order.ProjectNumber);
                writer.Write("\"");
                writer.Write(",");
                writer.Write("\"");
                writer.Write(order.FirstItem);
                writer.Write("\"");
                writer.Write(",");
                writer.Write("\"");
                writer.Write(order.ORValue.ToString(SCMS.Utils.Constants.NUMBER_FORMAT_TWO_DECIMAL));
                writer.Write("\"");
                writer.Write(",");
                writer.Write("\"");
                writer.Write(order.Requestor);
                writer.Write("\"");
                writer.Write(",");
                writer.Write("\"");
                writer.Write(order.Status);
                writer.Write("\"");
                writer.Write(",");
                writer.Write(order.StatusDate.ToShortDateString());
                writer.WriteLine();
            }
            writer.Flush();
            output.Position = 0;
            return File(output, "text/comma-separated-values", "OrderRequests.csv");

            //return File(output.ToArray(), "application/vnd.ms-excel", "GridExcelExport.xls");
        }

        public ActionResult ViewOrderRequestItems(Guid id, List<BudgetCheckResult> brcList = null)
        {
            Model.OrderRequest model = orderRequestService.GetOrderRequestById(id);
            model.BudgetCheckResults = brcList;
            //Manage Edit/Delete Link
            model.CanEdit = (!model.IsSubmitted && model.IsRejected != true && model.Staff2.Id == currentStaff.Id) ||
                    (model.IsSubmitted && model.IsRejected == true && model.IsReviewed != true && model.Staff2.Id == currentStaff.Id);
            //Manage approval link
            string actionType = null;
            if (model.IsReviewed == true && model.IsAuthorized == false)
                actionType = NotificationHelper.authorizationCode;
            else if (model.IsApproved && model.IsReviewed == false)
                actionType = NotificationHelper.reviewCode;
            else if (model.IsSubmitted && !model.IsApproved)
                actionType = NotificationHelper.approvalCode;
            if (actionType != null)
                model.CanApprove = notificationService.CanApprove(currentUser, NotificationHelper.orCode, actionType, model.Id);
            else
                model.CanApprove = false;
            model.CanPreparePO = orderRequestService.CanPreparePO(id) && userContext.HasPermission(StandardPermissionProvider.PurchaseOrderManage);

            return View("ViewOrderRequestItems", model);
        }

        public ActionResult GetBudgetLines(Guid id)
        {
            StringBuilder blineOption = new StringBuilder();
            blineOption.Append("<select class=\"dpl\" data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"BudgetLineID\" name=\"BudgetLineID\" onchange = \"javascript:checkBalance()\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            List<Model.BudgetLineView> BLines = orderRequestService.GetProjectBugdetLines(id);
            foreach (BudgetLineView item in BLines)
                blineOption.Append("<option value=\"" + item.Id + "\">" + item.Description + "</option>");
            blineOption.Append("</select>");
            blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"BudgetLineID\" data-valmsg-replace=\"true\"></span>");
            ViewBag.Html = blineOption.ToString();
            return View("SelectIndexChangeResponse");
        }

        /// <summary>
        /// Same as the above but for OR Items in a loop
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetBudgetLineForORItems(Guid id)
        {
            //# is a placeholder that will be replaced on the other side
            StringBuilder blinOption = new StringBuilder();
            blinOption.Append("<select class=\"smallControl\" id=\"ORItems_#__BudgetLineID\" name=\"ORItems[#].BudgetLineID\" onchange = \"javascript:checkBalance4Review(this)\">");
            List<Model.BudgetLineView> BLines = orderRequestService.GetProjectBugdetLines(id);
            foreach (BudgetLineView item in BLines)
                blinOption.Append("<option value=\"" + item.Id + "\">" + item.Description + "</option>");
            blinOption.Append("</select>");
            //blinOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"EntityOrderRequestItem.ProjectDonorId\" data-valmsg-replace=\"true\"></span>");
            ViewBag.Html = blinOption.ToString();
            return View("SelectIndexChangeResponse");
        }

        /// <summary>
        /// Same as the above but for PO Items in a loop
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetBudgetLineForPOItems(Guid id)
        {
            //# is a placeholder that will be replaced on the other side
            StringBuilder blinOption = new StringBuilder();
            blinOption.Append("<select class=\"smallControl\" id=\"POItems_#__BudgetLineID\" name=\"POItems[#].BudgetLineID\" onchange = \"javascript:checkBalance4Review(this)\">");
            List<Model.BudgetLineView> BLines = orderRequestService.GetProjectBugdetLines(id);
            foreach (BudgetLineView item in BLines)
                blinOption.Append("<option value=\"" + item.Id + "\">" + item.Description + "</option>");
            blinOption.Append("</select>");
            ViewBag.Html = blinOption.ToString();
            return View("SelectIndexChangeResponse");
        }

        /// <summary>
        /// Same as the above but for RFP Items in a loop
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetBudgetLineForRFPItems(Guid id)
        {
            //# is a placeholder that will be replaced on the other side
            StringBuilder blinOption = new StringBuilder();
            blinOption.Append("<select id=\"paymentDetails_#__BudgetLineId\" name=\"paymentDetails[#].BudgetLineId\" onchange = \"javascript:checkBalance4Review(this)\">");
            List<Model.BudgetLineView> BLines = orderRequestService.GetProjectBugdetLines(id);
            foreach (BudgetLineView item in BLines)
                blinOption.Append("<option value=\"" + item.Id + "\">" + item.Description + "</option>");
            blinOption.Append("</select>");
            ViewBag.Html = blinOption.ToString();
            return View("SelectIndexChangeResponse");
        }

        /// <summary>
        /// Same as the above but for RFA Items in a loop
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetBudgetLineForRFAItems(Guid id)
        {
            //# is a placeholder that will be replaced on the other side
            StringBuilder blinOption = new StringBuilder();
            blinOption.Append("<select id=\"PaymentDetails_#__BudgetLineID\" name=\"PaymentDetails[#].BudgetLineID\" onchange = \"javascript:checkBalance4Review(this)\">");
            List<Model.BudgetLineView> BLines = orderRequestService.GetProjectBugdetLines(id);
            foreach (BudgetLineView item in BLines)
                blinOption.Append("<option value=\"" + item.Id + "\">" + item.Description + "</option>");
            blinOption.Append("</select>");
            ViewBag.Html = blinOption.ToString();
            return View("SelectIndexChangeResponse");
        }

        /// <summary>
        /// Same as the above but for ECF Items in a loop
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetBudgetLineForECFItems(Guid id)
        {
            //# is a placeholder that will be replaced on the other side
            StringBuilder blineOption = new StringBuilder();
            blineOption.Append("<select id=\"SalaryPaymentItems_#__BudgetLineId\" name=\"SalaryPaymentItems[#].BudgetLineId\" onchange = \"javascript:checkBalance4Review(this)\">");
            List<Model.BudgetLineView> BLines = orderRequestService.GetProjectBugdetLines(id);
            foreach (BudgetLineView item in BLines)
                blineOption.Append("<option value=\"" + item.Id + "\">" + item.Description + "</option>");
            blineOption.Append("</select>");
            blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"SalaryPaymentItems[#].BudgetLineId\" data-valmsg-replace=\"true\"></span>");
            ViewBag.Html = blineOption.ToString();
            return View("SelectIndexChangeResponse");
        }

        /// <summary>
        /// Same as the above but for SPM Items in a loop
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetBudgetLineForSPMItems(Guid id)
        {
            //# is a placeholder that will be replaced on the other side
            StringBuilder blineOption = new StringBuilder();
            blineOption.Append("<select id=\"ExpenseClaimItems_#__BudgetLineId\" name=\"ExpenseClaimItems[#].BudgetLineId\" onchange = \"javascript:checkBalance4Review(this)\">");
            List<Model.BudgetLineView> BLines = orderRequestService.GetProjectBugdetLines(id);
            foreach (BudgetLineView item in BLines)
                blineOption.Append("<option value=\"" + item.Id + "\">" + item.Description + "</option>");
            blineOption.Append("</select>");
            blineOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"ExpenseClaimItems[#].BudgetLineId\" data-valmsg-replace=\"true\"></span>");
            ViewBag.Html = blineOption.ToString();
            return View("SelectIndexChangeResponse");
        }

        /// <summary>
        /// Same as the above but for PO Items in a loop in create purchase order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetBudgetLineForCreatePOItems(Guid id)
        {
            //# is a placeholder that will be replaced on the other side
            StringBuilder blinOption = new StringBuilder();
            blinOption.Append("<select class=\"poTextBox\" id=\"ORItems_#__EntityORItem_BudgetLineId\" name=\"ORItems[#].BudgetLineId\">");
            List<Model.BudgetLineView> BLines = orderRequestService.GetProjectBugdetLines(id);
            foreach (BudgetLineView item in BLines)
                blinOption.Append("<option value=\"" + item.Id + "\">" + item.Description + "</option>");
            blinOption.Append("</select>");
            ViewBag.Html = blinOption.ToString();
            return View("SelectIndexChangeResponse");
        }

        /// <summary>
        /// Same as the above but for create PO for TA
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetBudgetLineForCreatePOTAItems(Guid id)
        {
            //# is a placeholder that will be replaced on the other side
            StringBuilder blinOption = new StringBuilder();
            blinOption.Append("<select class=\"poTextBox\" id=\"ORTenderItems_#__TBQuoteEntity_OrderRequestItem_BudgetLineId\" name=\"ORTenderItems[#].BudgetLineId\">");
            List<Model.BudgetLineView> BLines = orderRequestService.GetProjectBugdetLines(id);
            foreach (BudgetLineView item in BLines)
                blinOption.Append("<option value=\"" + item.Id + "\">" + item.Description + "</option>");
            blinOption.Append("</select>");
            ViewBag.Html = blinOption.ToString();
            return View("SelectIndexChangeResponse");
        }

        [HttpGet]
        public ActionResult AddNewLocation4OR(bool final, Guid? otherDDSelectedId)
        {
            ULocation model = new ULocation();
            model.CountrySelect = new SelectList(locationService.CountryObj.GetCountries(), "Id", "Name");
            model.CountryId = countryProg.CountryId;
            model.IsFinal = final;
            ViewBag.OtherDDSelectedId = otherDDSelectedId;
            return View("AddNewLocation4OR", model);
        }

        [HttpPost]
        public ActionResult AddNewLocation4OR(ULocation model, bool final, Guid? otherDDSelectedId)
        {
            Location location = locationService.GetLocationByName(model.Name, countryProg.Id);
            if (location == null)
            {
                location = new Location();
                location.Name = model.Name;
                location.Description = model.Description;
                location.CountryId = model.CountryId;
                location.CountryProgrammeId = countryProg.Id;
                locationService.AddLocation(location);
            }
            return RepopulateLocationLists(location.Id, final, otherDDSelectedId);
        }

        private ActionResult RepopulateLocationLists(Guid selectedLocationId, bool final, Guid? otherDDSelectedId)
        {
            StringBuilder locationDropDown = new StringBuilder();
            StringBuilder finalLocationDD = new StringBuilder();
            string selected = "";
            locationDropDown.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"RequestDelivLocationID\" name=\"RequestDelivLocationID\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            List<Model.Location> locations = orderRequestService.GetLocations();
            foreach (Location location in locations)
            {
                if (!final && location.Id.Equals(selectedLocationId))
                    selected = "selected";
                else if (final && otherDDSelectedId.HasValue && location.Id.Equals(otherDDSelectedId.Value))
                    selected = "selected";
                else
                    selected = "";
                locationDropDown.Append("<option value=\"" + location.Id + "\" " + selected + ">" + location.Name + "</option>");
            }
            locationDropDown.Append("</select>");
            locationDropDown.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"RequestDelivLocationID\" data-valmsg-replace=\"true\"></span>");

            //final location DD
            selected = "";
            finalLocationDD.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"FinalDelivLocationID\" name=\"FinalDelivLocationID\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");

            foreach (Location location in locations)
            {
                if (final && location.Id.Equals(selectedLocationId))
                    selected = "selected";
                else if (!final && otherDDSelectedId.HasValue && location.Id.Equals(otherDDSelectedId.Value))
                    selected = "selected";
                else
                    selected = "";
                finalLocationDD.Append("<option value=\"" + location.Id + "\" " + selected + ">" + location.Name + "</option>");
            }
            finalLocationDD.Append("</select>");
            finalLocationDD.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"FinalDelivLocationID\" data-valmsg-replace=\"true\"></span>");
            return Content(locationDropDown.ToString() + "###" + finalLocationDD.ToString());
        }

        [HttpGet]
        public ActionResult AddNewItem(Guid id, bool editMode = false)
        {
            //maintain session OR variable if not null
            if (UserSession.CurrentSession.NewOR != null)
            {
                var or = UserSession.CurrentSession.NewOR;
            }
            UItem model = new UItem
            {
                ItemCatSelect = new SelectList(itemService.ItemCatObj.GetItemCategories(), "Id", "CategoryName"),
                ItemClassSelect = new SelectList(itemService.ItemClassObj.GetItemClassifications(countryProg.Id), "Id", "Name"),
                ItemUnitsSelect = new SelectList(itemService.ItemClassObj.GetUnitOfMessures(countryProg.Id), "Id", "Code"),
                EditMode = editMode
            };
            ViewBag.OrId = id;
            return View("AddNewItem4OR", model);
        }

        [HttpPost]
        public ActionResult AddNewItem(UItem model, Guid orId, bool editMode)
        {
            //maintain session OR variable if not null
            if (UserSession.CurrentSession.NewOR != null)
            {
                var or = UserSession.CurrentSession.NewOR;
            }
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
                return AddItemInEditMode(orId);
            return PopulateCreateItem(orId, "", item.Id);
        }

        public ActionResult BackDateOR(Guid id)
        {
            Model.OrderRequest model = orderRequestService.GetOrderRequestById(id);
            return View("BackDateOR", model);
        }

        [HttpPost]
        public ActionResult BackDateOR(Model.OrderRequest model)
        {
            if (model.OrderDate.Value > DateTime.Today)
                return ViewOrderRequestItems(model.Id);
            model.BackDatedBy = currentStaff.Id;
            orderRequestService.BackDateOR(model);
            return ViewOrderRequestItems(model.Id);
        }

        public ActionResult GetRecurringPx(Guid itemId)
        {
            return Content(orderRequestService.GetLastPOItemPrice(itemId).ToString(), "text/html");
        }

        public ActionResult PreparePO(Guid id)
        {
            var or = orderRequestService.GetOrderRequestById(id);
            UserSession.CurrentSession.NewOR = or;
            return RedirectToAction("Index", "PurchaseOrder", new { orId = id });
        }
    }
}

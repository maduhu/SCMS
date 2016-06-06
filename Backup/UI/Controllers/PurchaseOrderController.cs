using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.PurchaseOrder;
using SCMS.CoreBusinessLogic.Web;
using SCMS.UI.Models;
using SCMS.Model;
using System.Data.Objects.DataClasses;
using SCMS.CoreBusinessLogic.OrderRequest;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.NotificationsManager;
using SCMS.CoreBusinessLogic._ExchangeRate;
using SCMS.CoreBusinessLogic._Supplier;
using System.Text;
using SCMS.CoreBusinessLogic._Location;
using SCMS.Resource;
using SCMS.UI.GeneralHelper;
using SCMS.Utils;
using SCMS.CoreBusinessLogic.ProcurementPlan;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;
using System.Collections;

namespace SCMS.UI.Controllers
{
    [MyException]
    //[SessionExpireFilter]
    public class PurchaseOrderController : PortalBaseController
    {
        private IPurchaseOrderService poService;
        private IOrderRequest orService;
        private INotificationService notificationService;
        private IExchangeRateService exchangeRateService;
        private ISupplierService supplierService;
        private ILocationService locationService;
        private IProcurementPlanService ppService;
        private static int TotalPO;

        public PurchaseOrderController(IPermissionService permissionService, IUserContext userContext, IPurchaseOrderService POService, IOrderRequest ORService,
            IExchangeRateService exchangeRateService, INotificationService notificationService, ISupplierService supplierService, ILocationService locationService,
            IProcurementPlanService ppService)
            : base(userContext, permissionService)
        {
            this.poService = POService;
            this.orService = ORService;
            this.ppService = ppService;
            this.notificationService = notificationService;
            this.exchangeRateService = exchangeRateService;
            this.supplierService = supplierService;
            this.locationService = locationService;
        }
        //
        // GET: /PurchaseOrder/
        //[OutputCache(Duration = 3600, VaryByParam = "none")]
        public ActionResult Index(Guid? orId = null)
        {
            return View("Index", new PurchaseOrder { OrderRequestId = orId });
        }

        public ActionResult LoadPO()
        {
            var model = new Model.PurchaseOrder()
            {
                PODate = DateTime.Today,
                LatestDeliveryDate = DateTime.Today,
                Currencies = new SelectList(orService.GetCurrencies(), "Id", "ShortName",
                    UserSession.CurrentSession.NewOR != null ? UserSession.CurrentSession.NewOR.CurrencyId : Guid.Empty),
                Projects = new SelectList(orService.GetProject(), "Id", "Name",
                    UserSession.CurrentSession.NewOR != null ? UserSession.CurrentSession.NewOR.ProjectId : Guid.Empty),
                ProjectDonors = new SelectList(orService.GetProjectNos(UserSession.CurrentSession.NewOR != null ? UserSession.CurrentSession.NewOR.ProjectId : Guid.Empty), "Id", "ProjectNumber",
                    UserSession.CurrentSession.NewOR != null ? UserSession.CurrentSession.NewOR.ProjectDonorId : Guid.Empty),
                Suppliers = new SelectList(poService.GetSuppliers(), "Id", "Name"),
                ShippingTerms = new SelectList(poService.GetShippingTerms(), "Id", "Description"),
                Locations = new SelectList(orService.GetLocations(), "Id", "Name"),
                PaymentTerms = new SelectList(poService.GetPaymentTerms(), "Id", "Description"),
                RefNumber = string.Format("--{0}--", Resources.Global_String_NewPO)
            };
            if (UserSession.CurrentSession.NewOR != null)
            {
                model.ProjectDonorId = UserSession.CurrentSession.NewOR.ProjectDonorId;
                model.CurrencyId = UserSession.CurrentSession.NewOR.CurrencyId;
                model.ProjectId = UserSession.CurrentSession.NewOR.ProjectId;
            }
            return View("LoadPO", model);
        }

        public ActionResult EditPO(Guid Id)
        {
            Model.PurchaseOrder model = poService.GetPurchaseOrderById(Id);
            model.Currencies = new SelectList(orService.GetCurrencies(), "Id", "ShortName", model.CurrencyId);
            model.Suppliers = new SelectList(poService.GetSuppliers(), "Id", "Name", model.SupplierId);
            model.ShippingTerms = new SelectList(poService.GetShippingTerms(), "Id", "Description", model.ShippingTermId);
            model.Locations = new SelectList(orService.GetLocations(), "Id", "Name", model.DeliveryAddress);
            model.PaymentTerms = new SelectList(poService.GetPaymentTerms(), "Id", "Description", model.PaymentTermId);
            return View("LoadPO", model);
        }

        public ActionResult SavePO(Model.PurchaseOrder model)
        {
            if (model.Id.Equals(Guid.Empty))
            {
                model.TotalAmount = 0;
                model.CountryProgrammeId = countryProg.Id;
                model.PreparedBy = currentStaff.Id;
                model.PreparedOn = DateTime.Now;
            }
            poService.SavePuchaseOrder(model);
            //Check if PO prepare was initiated from OR
            if (UserSession.CurrentSession.NewOR != null)
            {
                //Add OR items to PO
                poService.AddPOItemsFromOR(UserSession.CurrentSession.NewOR, model.Id);
                //Clear OR Session
                UserSession.CurrentSession.NewOR = null;
            }
            return LoadPOItems(model.Id);
        }

        [HttpGet]
        public ActionResult ValidateQuotationRef(string QuotationRef, Guid Id)
        {
            if (poService.QuotationRefExists(QuotationRef, Id))
                return Content("FAILURE");
            return Content("SUCCESS");
        }

        [HttpGet]
        public ActionResult ValidateTenderNumber(string TenderNumber, Guid PurchaseOrderId)
        {
            if (poService.TenderNumberExists(TenderNumber, PurchaseOrderId))
                return Content("FAILURE");
            return Content("SUCCESS");
        }

        public ActionResult LoadPOItems(Guid Id, List<BudgetCheckResult> brcList = null)
        {
            Model.PurchaseOrder model = poService.GetPurchaseOrderById(Id);
            model.ORList = poService.GetPOrderORs(Id);
            model.PPList = poService.GetPOrderPPs(Id);
            model.BudgetCheckResults = brcList;

            foreach (var or in model.ORList)
            {
                or.POItems = poService.GetPOItemsByOrId(model.Id, or.Id);
            }

            foreach (var pp in model.PPList)
            {
                pp.POItems = poService.GetPOItemsByPPId(model.Id, pp.Id);
            }

            ViewBag.PORefNumber = model.RefNumber;
            return View("LoadPOItems", model);
        }

        public ActionResult LoadAddItems(Guid Id)
        {
            Model.PurchaseOrder model = poService.GetPurchaseOrderById(Id);
            //G Order Requests with unprocessed items
            List<Model.OrderRequest> orList = poService.GetOrderRequestsForPO(countryProg.Id);
            UserSession.CurrentSession.OrderRequestList = orList;

            var projectList = orList.OrderBy(o => o.ProjectDonor.Project.Name).Select(o => o.ProjectDonor.Project).Distinct().ToList();
            var pdList = orList.Where(o => projectList != null && projectList.Count > 0 && o.ProjectDonor.ProjectId == projectList[0].Id).Select(o => o.ProjectDonor).Distinct().ToList();

            if (orList.Count > 0)
            {
                model.OrderRequest = orList[0];
                model.OrderRequestId = orList[0].Id;
                model.OrderRequests = new SelectList(orList.Where(o => o.ProjectDonorId == pdList[0].Id).ToList(), "Id", "RefNumber", orList[0].Id);
                model.Projects = new SelectList(projectList, "Id", "Name");
                model.ProjectDonors = new SelectList(pdList, "Id", "ProjectNumber");
                model.POItems = model.PurchaseOrderItems.ToList();
                //model.ORItems = poService.GetUnprocessedORItems(orList[0].Id);
                model.ORItems = orList[0].OrderRequestItems.Where(i => i.Quantity > i.PurchaseOrderItems.Sum(p => p.Quantity)).ToList();
                foreach (var orItem in model.ORItems)
                {
                    ConvertORItemNumbersToPOCurrency(model, orItem);
                    orItem.AddToPO = true;
                    orItem.Quantity = (int)orItem.Quantity - orItem.PurchaseOrderItems.Sum(p => p.Quantity);
                    orItem.EstimatedPrice = orItem.EstimatedUnitPrice * orItem.Quantity;
                    orItem.BudgetLines = new SelectList(orService.GetProjectBugdetLines((Guid)orList[0].ProjectDonorId), "Id", "Description", orItem.BudgetLineId);
                }
                model.SameCurrency = model.Currency.Id.Equals(orList[0].Currency.Id);
            }
            return View("AddPOItems", model);
        }

        private void ConvertORItemNumbersToPOCurrency(Model.PurchaseOrder po, Model.OrderRequestItem orItem)
        {
            orItem.EstimatedUnitPrice = Math.Round((decimal)exchangeRateService.GetForeignCurrencyValue(po.Currency, orItem.OrderRequest.Currency, orItem.EstimatedUnitPrice, countryProg.Id), 2);
            orItem.EstimatedPrice = orItem.EstimatedUnitPrice * orItem.Quantity;
        }

        public ActionResult SelectOR(Guid PoId, Guid OrId, Guid PdId)
        {
            Model.PurchaseOrder model = poService.GetPurchaseOrderById(PoId);
            //G Order Requests with unprocessed items
            List<Model.OrderRequest> orList = poService.GetOrderRequestsForPO(countryProg.Id).Where(o => o.ProjectDonorId == PdId).ToList();
            
            Model.OrderRequest selectedOR = null;
            foreach (var or in orList)
                if (or.Id == OrId)
                {
                    selectedOR = or;
                    break;
                }
            if (orList.Count > 0)
            {
                if (selectedOR == null)
                    selectedOR = orList[0];
                var projectList = UserSession.CurrentSession.OrderRequestList.OrderBy(o => o.ProjectDonor.Project.Name).Select(o => o.ProjectDonor.Project).Distinct().ToList();
                var pdList = orList.Where(o => o.ProjectDonor.ProjectId == selectedOR.ProjectDonor.ProjectId).Select(o => o.ProjectDonor).Distinct().ToList();
                model.ProjectId = selectedOR.ProjectDonor.ProjectId;
                model.ProjectDonorId = PdId;

                model.Projects = new SelectList(projectList, "Id", "Name", model.ProjectId);
                model.ProjectDonors = new SelectList(pdList, "Id", "ProjectNumber", PdId);
                model.OrderRequest = selectedOR;
                model.OrderRequestId = selectedOR.Id;
                model.OrderRequests = new SelectList(orList, "Id", "RefNumber", selectedOR.Id);
                model.POItems = model.PurchaseOrderItems.ToList();
                //model.ORItems = poService.GetUnprocessedORItems(selectedOR.Id);
                model.ORItems = selectedOR.OrderRequestItems.Where(i => i.Quantity > i.PurchaseOrderItems.Sum(p => p.Quantity)).ToList();
                
                foreach (var orItem in model.ORItems)
                {
                    ConvertORItemNumbersToPOCurrency(model, orItem);
                    orItem.AddToPO = true;
                    orItem.Quantity = (int)orItem.Quantity - orItem.PurchaseOrderItems.Sum(p => p.Quantity);
                    orItem.EstimatedPrice = orItem.EstimatedUnitPrice * orItem.Quantity;
                    orItem.BudgetLines = new SelectList(orService.GetProjectBugdetLines((Guid)selectedOR.ProjectDonorId), "Id", "Description", orItem.BudgetLineId);
                }
                model.SameCurrency = model.Currency.Id.Equals(selectedOR.Currency.Id);
            }
            return View("AddPOItems", model);
        }

        public ActionResult LoadAddPPItemsToPO(Guid poId, Guid? pdId = null)
        {
            PurchaseOrder po = poService.GetPurchaseOrderById(poId);
            ProjectDonor pd = pdId.HasValue ? orService.GetProjectDonorById(pdId.Value) : orService.GetProjectNosWithPP().FirstOrDefault();
            ProcurementPlan model = pd != null ? ppService.GetProcurementPlanByProjectId(pd.Id) : null;
            if (model == null || !model.IsAuthorized)
            {
                model = new ProcurementPlan();
                model.PPItemList = new List<ProcurementPlanItem>();
            }
            else
            {
                model.PPItemList = model.ProcurementPlanItems.Where(p => (p.Quantity - p.ProcuredAmount) > 0).OrderBy(p => p.Item.Name).ToList();
                foreach (ProcurementPlanItem ppItem in model.PPItemList)
                {
                    ppItem.UnitCost = Math.Round(exchangeRateService.GetForeignCurrencyValue(po.CurrencyId, ppItem.CurrencyId, ppItem.UnitCost, countryProg.Id), 2);
                    ppItem.QuantityToOrder = ppItem.Quantity - ppItem.ProcuredAmount;
                    ppItem.TotalCost = Math.Round((ppItem.UnitCost * ppItem.QuantityToOrder), 2);
                }
            }
            model.PurchaseOrderId = poId;
            model.ProjectId = pd != null ? pd.ProjectId : Guid.Empty;
            model.Projects = new SelectList(orService.GetProjectsWithPP(), "Id", "Name", pd != null ? pd.ProjectId : Guid.Empty);
            model.ProjectDonors = new SelectList(orService.GetProjectNosWithPP(pd != null ? pd.ProjectId : Guid.Empty), "Id", "ProjectNumber", model.ProjectDonorId);
            return View("AddPPItemsToPO", model);
        }

        public ActionResult AddItemsToPO(Model.PurchaseOrder model)
        {
            Model.PurchaseOrder po = poService.GetPurchaseOrderById(model.Id);
            Model.PurchaseOrderItem poItem;
            if (po.TotalAmount == null)
                po.TotalAmount = 0;
            foreach (var orItem in model.ORItems)
            {
                if (orItem.AddToPO)
                {
                    poItem = new PurchaseOrderItem
                    {
                        BudgetLineId = orItem.BudgetLineId,
                        OrderRequestItemId = orItem.Id,
                        PurchaseOrderId = po.Id,
                        Quantity = (int)orItem.Quantity,
                        Remarks = orItem.Remarks,
                        TotalPrice = orItem.EstimatedPrice,
                        UnitPrice = (double)orItem.EstimatedUnitPrice
                    };
                    po.TotalAmount += orItem.EstimatedPrice;
                    poService.SavePOItem(poItem);
                }
            }
            if (po.OrderRequest == null)
                po.OrderRequestId = model.OrderRequestId;
            poService.SaveReviewedPO(po);
            return LoadPOItems(po.Id);
        }

        public ActionResult AddPPItemsToPO(Model.ProcurementPlan pp)
        {
            Model.PurchaseOrder po = poService.GetPurchaseOrderById(pp.PurchaseOrderId);
            Model.PurchaseOrderItem poItem;
            Model.ProcurementPlanItem item;
            if (po.TotalAmount == null)
                po.TotalAmount = 0;
            foreach (var ppItem in pp.PPItemList)
            {
                if (ppItem.AddedToOR)
                {
                    item = ppService.GetProcurementPlanItemById(ppItem.Id);
                    poItem = new PurchaseOrderItem
                    {
                        BudgetLineId = item.BudgetLineId,
                        ProcurementPlanItemId = item.Id,
                        PurchaseOrderId = po.Id,
                        Quantity = ppItem.QuantityToOrder,
                        TotalPrice = ppItem.TotalCost,
                        UnitPrice = (double)ppItem.UnitCost
                    };
                    po.TotalAmount += ppItem.TotalCost;
                    poService.SavePOItem(poItem);
                }
            }
            poService.SaveReviewedPO(po);
            return LoadPOItems(po.Id);
        }

        public ActionResult EditPOItem(Guid Id)
        {
            Model.PurchaseOrderItem model = poService.GetPurchaseOrderItemById(Id);
            model.BudgetLines = new SelectList(orService.GetProjectBugdetLines((Guid)model.ProjectBudget.BudgetCategory.ProjectDonorId), "Id", "Description", model.BudgetLineId);
            model.UnitPrice = Math.Round(model.UnitPrice, 2);
            model.TotalPrice = Math.Round(model.TotalPrice, 2);
            return View("EditPOItem", model);
        }

        public ActionResult UpdatePOItem(Model.PurchaseOrderItem model)
        {
            poService.SavePOItem(model);
            return LoadPOItems(model.PurchaseOrderId);
        }

        public ActionResult DeletePOItem(Guid poItemId, Guid poId)
        {
            poService.DeletePOItem(poItemId);
            return LoadPOItems(poId);
        }

        public ActionResult SetTenderingType(Guid id)
        {
            PurchaseOrder po = poService.GetPurchaseOrderById(id);
            TenderingType tt = poService.DetermineTenderingType(po);
            PurchaseOrderTenderingType model = new PurchaseOrderTenderingType
            {
                POValue = po.TotalAmount.Value,
                PurchaseOrderId = po.Id,
                TenderingTypeId = tt != null ? tt.Id : Guid.Empty,
                TenderingTypes = new SelectList(poService.GetTenderingTypes(), "Id", "Name"),
                TenderNumber = po.TenderNumber
            };
            if (po.TenderingTypeId.HasValue)
                model.TenderingTypeId = po.TenderingTypeId.Value;
            ViewBag.DefaultTenderingType = tt != null ? tt.Name : string.Empty;

            return View("SelectTenderingType", model);
        }

        [HttpPost]
        public ActionResult SaveTenderingType(PurchaseOrderTenderingType model)
        {
            PurchaseOrder po = poService.GetPurchaseOrderById(model.PurchaseOrderId);
            TenderingType tt = poService.GetTenderingTypeById(model.TenderingTypeId);
            po.TenderingTypeId = model.TenderingTypeId;
            po.TenderNumber = model.TenderNumber;
            po.IsInternational = tt.IsInternational;
            po.WaiverAcquired = null;
            //Save ProjectDonor
            po.ProjectDonorId = poService.DeterminePOProjectDonorId(po.Id);
            if (model.WaiverAcquired.HasValue)
                po.WaiverAcquired = model.WaiverAcquired;
            poService.SaveReviewedPO(po);
            if ((model.WaiverAcquired.HasValue && !model.WaiverAcquired.Value) || po.WaiverAcquired == false)
                return ViewPurchaseOrders();
            return LoadOtherPODetails(po.Id);
        }

        public ActionResult ChangeTenderingType(Guid typeId, Guid poId)
        {
            PurchaseOrder po = poService.GetPurchaseOrderById(poId);
            TenderingType poTT = poService.DetermineTenderingType(po);
            TenderingType tt = poService.GetTenderingTypeById(typeId);
            if (poTT != null && tt != null)
            {
                if (tt.MinValue < poTT.MinValue)
                {
                    return Content("CheckWaiver");
                }
            }
            return Content("None");
        }

        public ActionResult LoadOtherPODetails(Guid id)
        {
            Model.PurchaseOrder po = poService.GetPurchaseOrderById(id);
            return LoadLastCreateStep(po);
        }

        private ActionResult LoadLastCreateStep(Model.PurchaseOrder model)
        {
            return View("LoadOtherPODetails", model);
        }

        public ActionResult Load2ndLastStep(Guid poId)
        {
            return LoadLastCreateStep(poService.GetPurchaseOrderById(poId));
        }

        public ActionResult LoadAttacheDocs(Model.PurchaseOrder model)
        {
            //Run another check on funds availability
            List<BudgetCheckResult> bcrList = poService.RunFundsAvailableCheck(model.Id);
            if (bcrList.Count > 0)
                return LoadPOItems(model.Id, bcrList);//Render PO with message saying funds not sufficient

            Model.PurchaseOrder po = poService.GetPurchaseOrderById(model.Id);
            po.ProformaRequired = model.ProformaRequired;
            po.CommercialInvoiceRequired = model.CommercialInvoiceRequired;
            po.WayBillRequired = model.WayBillRequired;
            po.PackingListRequired = model.PackingListRequired;
            po.DeliveryNoteRequired = model.DeliveryNoteRequired;
            po.ManualsRequired = model.ManualsRequired;
            po.CertificatesReqired = model.CertificatesReqired;
            po.OtherRequired = model.OtherRequired;
            if (po.OtherRequired == true)
                po.OtherSpecify = model.OtherSpecify;
            po.ShippingMarks = model.ShippingMarks;
            po.ConsigneeAddress = model.ConsigneeAddress;
            po.ConsigneeEmail1 = model.ConsigneeEmail1;
            po.ConsigneeEmail2 = model.ConsigneeEmail2;
            po.PrefinancingGuaranteeRequired = model.PrefinancingGuaranteeRequired;
            if (po.PrefinancingGuaranteeRequired == true)
                po.PFGPercentage = model.PFGPercentage;
            poService.SaveReviewedPO(po);
            return LoadAttachDocs(po.Id);
        }

        public ActionResult LoadAttachDocs(Guid id)
        {
            Model.PurchaseOrder po = poService.GetPurchaseOrderById(id);
            return View("LoadAttacheDocs", new AttachedDocument() { POEntity = poService.GetPurchaseOrderById(po.Id), DocList = poService.GetList(po.Id, countryProg.Id) });
        }

        public ActionResult AttachNewDoc(Guid docId, Guid? attachedDocId = null)
        {
            return View("AttachNewDoc", attachedDocId == null ? new AttachedDocument() { DocumentId = docId, Action = "SaveNewDoc" } : poService.GetDocById((Guid)attachedDocId));
        }

        public ActionResult SaveNewDoc(Model.AttachedDocument model)
        {
            if (UserSession.CurrentSession.UploadedDocDetails == null)
                return AttachNewDoc((Guid)model.DocumentId);
            model.CountryProgrammeId = countryProg.Id;
            model.AttachedBy = currentStaff.Id;
            model.AttachedOn = DateTime.Now;
            model.Id = Guid.NewGuid();
            model.DocumentType = "PO";
            model.FileContent = UserSession.CurrentSession.UploadedDocDetails.FileContent;
            model.FileSize = UserSession.CurrentSession.UploadedDocDetails.ContentLength;
            model.ContentType = UserSession.CurrentSession.UploadedDocDetails.ContentType;
            model.FileName = UserSession.CurrentSession.UploadedDocDetails.FileName;
            poService.Save(model);
            UserSession.CurrentSession.UploadedDocDetails = null;
            return View("LoadAttacheDocs", new AttachedDocument()
            {
                POEntity = poService.GetPurchaseOrderById((Guid)model.DocumentId),
                DocList = poService.GetList((Guid)model.DocumentId, countryProg.Id)
            });
        }

        public ActionResult UpdateAttachedDoc(Model.AttachedDocument model)
        {
            if (UserSession.CurrentSession.UploadedDocDetails != null)
            {
                model.FileContent = UserSession.CurrentSession.UploadedDocDetails.FileContent;
                model.FileSize = UserSession.CurrentSession.UploadedDocDetails.ContentLength;
                model.ContentType = UserSession.CurrentSession.UploadedDocDetails.ContentType;
                model.FileName = UserSession.CurrentSession.UploadedDocDetails.FileName;
            }
            poService.Update(model);
            UserSession.CurrentSession.UploadedDocDetails = null;
            return View("LoadAttacheDocs", new AttachedDocument()
            {
                POEntity = poService.GetPurchaseOrderById((Guid)model.DocumentId),
                DocList = poService.GetList((Guid)model.DocumentId, countryProg.Id)
            });
        }

        public ActionResult DeleteAttachedDoc(Guid attachedDocId, Guid docId)
        {
            poService.DeleteAttachedDoc(attachedDocId);
            return View("LoadAttacheDocs", new AttachedDocument()
            {
                POEntity = poService.GetPurchaseOrderById(docId),
                DocList = poService.GetList(docId, countryProg.Id)
            });
        }

        public ActionResult SubmitPO(Model.AttachedDocument model)
        {
            Model.PurchaseOrder po = poService.GetPurchaseOrderById(model.POEntity.Id);
            po.PreparedOn = DateTime.Now;
            po.PODate = DateTime.Now;
            po.PreparedBy = currentStaff.Id;
            po.IsSubmitted = true;
            po.IsApproved = po.IsRejected = false;
            po.RefNumber = poService.GenerateUniquNumber(countryProg);
            poService.SaveReviewedPO(po);
            //Send notification
            notificationService.SendToAppropriateApprover(NotificationHelper.poCode, NotificationHelper.approvalCode, po.Id);
            return ViewPurchaseOrders();
        }

        public ActionResult DeletePO(Guid Id)
        {
            poService.DeletePO(Id);
            return ViewPurchaseOrders();
        }

        public ActionResult ViewPurchaseOrders()
        {
            ViewData["total"] = TotalPO;
            return View("ViewPurchaseOrders", poService.GetPurchaseOrders());
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _GenPOCustomBinding(GridCommand command)
        {
            DateTime statusDate;
            var gridModel = (poService.GetPurchaseOrders()
                .Select(n => new Model.POViewModel()
                {
                    Id = n.Id,
                    PONo = n.RefNumber,
                    ORNo = n.OrderRequest != null ? n.OrderRequest.RefNumber : n.PPNumber,
                    Supplier = n.Supplier != null ? n.Supplier.Name : string.Empty,
                    DeliveryDate = n.LatestDeliveryDate,
                    DeliveryAddress = n.Location.Name,
                    POValue = (float)n.TotalAmount,
                    Status = POStatus(n, out statusDate),
                    StatusDate = statusDate.Date

                }).AsQueryable())
                  .ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
            TotalPO = poService.GetPurchaseOrders().Count();

            IEnumerable data = gridModel.Data.AsQueryable().ToIList();
            return View(new GridModel { Data = data, Total = TotalPO });
        }

        private string POStatus(Model.PurchaseOrder po, out DateTime statusDate)
        {
            string poStatus;
            if (po.IsApproved)
                poStatus = Resources.Global_String_StatusAP;
            else if (po.IsRejected)
                poStatus = Resources.Global_String_StatusRJ;
            else if (po.IsSubmitted == true)
                poStatus = Resources.Global_String_StatusCR;
            else
                poStatus = Resources.Global_String_StatusNEW;

            //Get OR Status date
            if (po.IsApproved == true)
                statusDate = po.ApprovedOn.Value;
            else if (po.IsRejected == true && po.ApprovedOn.HasValue)
            {
                statusDate = po.ApprovedOn.Value;
            }
            else if (po.IsSubmitted == true)
                statusDate = po.PreparedOn.Value;
            else
                statusDate = po.PreparedOn.Value;

            return poStatus;
        }

        public ActionResult ViewPurchaseOrdersDetails(Guid id)
        {
            Model.PurchaseOrder model = poService.GetPurchaseOrderById(id);
            model.AttachedDocuments = poService.GetPOAttachedDocuments(model.Id);

            model.POItems = model.PurchaseOrderItems.ToList();

            model.CanEdit = (!model.IsSubmitted && !model.IsRejected && model.Staff1.Id == currentStaff.Id) ||
                (model.IsSubmitted && model.IsRejected && model.Staff1.Id == currentStaff.Id);
            //Check number of ORs/PPs
            var ors = poService.GetPOrderORs(id).Count;
            var pps = poService.GetPOrderPPs(id).Count;
            model.HasMoreThanOneOR = (ors + pps) > 1;

            //Manage approval link
            string actionType = null;
            if (!model.IsApproved)
                actionType = NotificationHelper.approvalCode;
            if (actionType != null)
                model.CanApprove = notificationService.CanApprove(currentUser, NotificationHelper.poCode, actionType, model.Id);
            else
                model.CanApprove = false;

            return View("ViewPurchaseOrdersDetails", model);
        }

        [HttpGet]
        public ActionResult AddNewSupplier4PO()
        {
            return View("AddNewSupplier4PO", new USupplier());
        }

        [HttpPost]
        public ActionResult AddNewSupplier4PO(USupplier model)
        {
            Supplier supplier = null;
            supplier = supplierService.GetSupplierByName(model.Name, countryProg.Id);
            if (supplier == null)
            {
                supplier = new Supplier();
                supplier.Name = model.Name;
                supplier.Address = model.Address != null && model.Address.Trim().Length > 0 ? model.Address : Resources.SystemUser_ViewProfile_NotSet.ToUpper();
                supplier.PrimaryPhone = model.PrimaryPhone != null && model.PrimaryPhone.Trim().Length > 0 ? model.PrimaryPhone : Resources.SystemUser_ViewProfile_NotSet.ToUpper();
                supplier.CountryId = countryProg.CountryId;
                supplier.CountryProgrammeId = countryProg.Id;
                supplier.IsApproved = false;
                supplierService.AddSupplier(supplier);
            }
            return RepopulateSupplierList(supplier.Id);
        }

        private ActionResult RepopulateSupplierList(Guid selectedSupplierId)
        {
            StringBuilder supplierDropDown = new StringBuilder();
            string selected = "";
            supplierDropDown.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"SupplierId\" name=\"SupplierId\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            List<Model.Supplier> suppliers = supplierService.GetSuppliers(countryProg.Id);
            foreach (Supplier supplier in suppliers)
            {
                if (supplier.Id.Equals(selectedSupplierId))
                    selected = "selected";
                else
                    selected = "";
                supplierDropDown.Append("<option value=\"" + supplier.Id + "\" " + selected + ">" + supplier.Name + "</option>");
            }
            supplierDropDown.Append("</select>");
            supplierDropDown.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"SupplierId\" data-valmsg-replace=\"true\"></span>");
            return Content(supplierDropDown.ToString(), "text/html");
        }

        [HttpGet]
        public ActionResult AddNewLocation4PO()
        {
            ULocation model = new ULocation();
            model.CountrySelect = new SelectList(locationService.CountryObj.GetCountries(), "Id", "Name");
            model.CountryId = countryProg.CountryId;
            return View("AddNewLocation4PO", model);
        }

        [HttpPost]
        public ActionResult AddNewLocation4PO(ULocation model)
        {
            Location location = null;
            location = locationService.GetLocationByName(model.Name, countryProg.Id);
            if (location == null)
            {
                location = new Location();
                location.Name = model.Name;
                location.Description = model.Description;
                location.CountryId = model.CountryId;
                location.CountryProgrammeId = countryProg.Id;
                //location.IsApproved = false;
                locationService.AddLocation(location);
            }
            return RepopulateLocationList(location.Id);
        }

        private ActionResult RepopulateLocationList(Guid selectedLocationId)
        {
            StringBuilder locationDropDown = new StringBuilder();
            string selected = "";
            locationDropDown.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"deliveryAddressID\" name=\"deliveryAddressID\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            List<Model.Location> locations = orService.GetLocations();
            foreach (Location location in locations)
            {
                if (location.Id.Equals(selectedLocationId))
                    selected = "selected";
                else
                    selected = "";
                locationDropDown.Append("<option value=\"" + location.Id + "\" " + selected + ">" + location.Name + "</option>");
            }
            locationDropDown.Append("</select>");
            locationDropDown.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"deliveryAddressID\" data-valmsg-replace=\"true\"></span>");
            return Content(locationDropDown.ToString(), "text/html");
        }

        public ActionResult BackDatePO(Guid id)
        {
            Model.PurchaseOrder model = poService.GetPurchaseOrderById(id);
            return View("BackDatePO", model);
        }

        [HttpPost]
        public ActionResult BackDatePO(Model.PurchaseOrder model)
        {
            if (model.PODate > DateTime.Today)
                return ViewPurchaseOrdersDetails(model.Id);
            model.BackDatedBy = currentStaff.Id;
            poService.BackDatePO(model);
            return ViewPurchaseOrdersDetails(model.Id);
        }

        public ActionResult GetAttachment(Guid docId)
        {
            Model.AttachedDocument doc = poService.GetAttachedDocumentById(docId);
            return File(doc.FileContent, doc.ContentType, doc.FileName);
        }

        public ActionResult GetProjectDonors(Guid id)
        {
            StringBuilder blOption = new StringBuilder();
            blOption.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"ProjectDonorId\" name=\"ProjectDonorId\" onchange=\"javascript:loadPdProcurementPlan(this)\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            List<ProjectDonor> pdList = orService.GetProjectNosWithPP(id);
            foreach (ProjectDonor item in pdList)
                blOption.Append("<option value=\"" + item.Id + "\">" + item.ProjectNumber + "</option>");
            blOption.Append("</select>");
            blOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"ProjectDonorId\" data-valmsg-replace=\"true\"></span>");
            return Content(blOption.ToString(), "text/html");
        }

        public ActionResult GetProjectDonorsWithOR(Guid id)
        {
            StringBuilder blOption = new StringBuilder();
            var orList = UserSession.CurrentSession.OrderRequestList;
            var pdList = orList.Where(o => o.ProjectDonor.ProjectId == id).Select(o => o.ProjectDonor).Distinct().ToList();

            blOption.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"pdId\" name=\"ProjectDonorId\" onchange=\"javascript:loadOrItems()\"><option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            foreach (ProjectDonor item in pdList)
                blOption.Append("<option value=\"" + item.Id + "\">" + item.ProjectNumber + "</option>");
            blOption.Append("</select>");
            blOption.Append("<span class=\"field-validation-valid\" data-valmsg-for=\"ProjectDonorId\" data-valmsg-replace=\"true\"></span>");
            return Content(blOption.ToString(), "text/html");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.GeneralHelper
{
    public partial class SessionData
    {

        #region .private variables.

        private IEnumerable<Model.OrderRequest> _orList;
        private IEnumerable<Model.ProcurementPlan> _ppList;
        private IEnumerable<Model.PurchaseOrder> _poList;
        private IEnumerable<Model.ProcurementPlanItem> _ppItemList;
        private IEnumerable<Model.AttachedDocument> _attachedDocList;
        private IEnumerable<Model.CompletionCertificate> _ccList;
        private IEnumerable<Model.TenderingType> _ttList;

        #endregion

        public IEnumerable<Model.OrderRequest> OrderRequestList 
        {
            get
            {
                if (_orList == null || _orList.ToList().Count == 0)
                {
                    using (var db = new SCMSEntities())
                    {
                        _orList = db.OrderRequests
                            .IncludeCurrency()
                            .IncludeProjectDonor()
                            .IncludeStaff()
                            .IncludeLocation()
                            .IncludeOrderRequestItemsWithProject()
                            .IncludeOrderRequestItemsWithPO()
                            .IncludePurchaseOrders()
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId)
                            .OrderByDescending(g => g.PreparedOn).ToList();                        
                    }
                }
                return _orList;
            }
            set
            {
                _orList = value;
            }
        }
        
        public IEnumerable<Model.ProcurementPlan> ProcurementPlanList
        {
            get
            {
                if (_ppList == null || _ppList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _ppList = dbContext.ProcurementPlans
                            .IncludeCountrySubOffice()
                            .IncludeProcurementPlanItems()
                            .IncludeProjectDonor()
                            .IncludeStaff()
                            .IncludeNotifications()
                            .Where(p => p.ProjectDonor.Project.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _ppList;
            }
            set
            {
                _ppList = value;
            }
        }

        public IEnumerable<Model.ProcurementPlanItem> ProcurementPlanItemList
        {
            get
            {
                if (_ppItemList == null || _ppItemList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _ppItemList = dbContext.ProcurementPlanItems
                            .IncludeItem()
                            .IncludeOrderRequest()
                            .IncludeProjectBudget()
                            .IncludeProcurementPlan()
                            .IncludeProjectBudget()
                            .Where(p => p.ProcurementPlan.ProjectDonor.Project.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _ppItemList;
            }
            set
            {
                _ppItemList = value;
            }
        }

        public IEnumerable<Model.PurchaseOrder> PurchaseOrderList 
        {
            get
            {
                if (_poList == null || _poList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _poList = dbContext.PurchaseOrders
                            .IncludeOrderRequest()
                            .IncludeSupplier()
                            .IncludePurchaseOrderItems()
                            .IncludePOItemsWithGRNItems()
                            .IncludeOrderRequest()
                            .IncludeCurrency()
                            .IncludeLocation()
                            .IncludePaymentTerm()
                            .IncludeProjectDonor()
                            .IncludeShippingTerm()
                            .IncludeStaff()
                            .IncludeNotifications()
                            .IncludeTenderingType()
                            .Where(p => p.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _poList;
            }
            set
            {
                _poList = value;
            }
        }

        public IEnumerable<Model.AttachedDocument> AttachedDocumentList
        {
            get
            {
                if (_attachedDocList == null || _attachedDocList.ToList().Count == 0)
                {
                    using (var context = new SCMSEntities())
                    {
                        _attachedDocList = context.AttachedDocuments.Where(d => d.CountryProgrammeId == CountryProgrammeId).OrderBy(d => d.Name).ToList();
                    }
                }
                return _attachedDocList;
            }
            set
            {
                _attachedDocList = value;
            }
        }

        public IEnumerable<Model.CompletionCertificate> CompletionCertificateList
        {
            get
            {
                if (_ccList == null || _ccList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _ccList = dbContext.CompletionCertificates
                            .IncludeCountrySubOffice()
                            .IncludePurchaseOrder()
                            .IncludeStaff()
                            .Where(c => c.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _ccList;
            }
            set
            {
                _ccList = value;
            }
        }

        public IEnumerable<Model.TenderingType> TenderingTypeList
        {
            get
            {
                if (_ttList == null || _ttList.ToList().Count == 0)
                {
                    using (var dbContext = new SCMSEntities())
                    {
                        _ttList = dbContext.TenderingTypes.Include("Currency").Where(t => t.CountryProgrammeId == CountryProgrammeId).ToList();
                    }
                }
                return _ttList;
            }
            set
            {
                _ttList = value;
            }
        }

    }
}

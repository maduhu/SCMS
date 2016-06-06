using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using SCMS.CoreBusinessLogic._ExchangeRate;
using System.Transactions;
using System.Data.Entity.Infrastructure;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.CoreBusinessLogic.Budgeting
{
    public class BudgetService : IBudgetService
    {
        private IExchangeRateService exchangeRateService;
        
        public BudgetService(IExchangeRateService exchangeRateService)
        {
            this.exchangeRateService = exchangeRateService;
        }

        private static void ClearProjectBudgetList()
        {
            SessionData.CurrentSession.ProjectBudgetList = null;
        }

        private static void ClearProjectDonorList()
        {
            SessionData.CurrentSession.ProjectDonorList = null;
        }

        private static void ClearProjectList()
        {
            SessionData.CurrentSession.ProjectList = null;
        }

        #region IBudgetService Members

        #region .BudgetCategory CRUD.

        public void CreateBudgetCategory(BudgetCategory budgetCategory, ProjectDonor pd)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var bc = new BudgetCategory();
                    bc.Id = Guid.NewGuid();
                    bc.Name = budgetCategory.Name;
                    bc.Number = budgetCategory.Number;
                    bc.Description = budgetCategory.Description;
                    bc.ProjectDonorId = pd.Id;
                    context.BudgetCategories.Add(bc);
                    context.SaveChanges();
                    ClearProjectDonorList();
                }
            }
            catch (Exception ex)
            { 
                
            }
        }
        public void UpdateBudgetCategory(BudgetCategory budgetCategory)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var bc = context.BudgetCategories.First(b => b.Id == budgetCategory.Id);
                    bc.Name = budgetCategory.Name;
                    bc.Number = budgetCategory.Number;
                    bc.Description = budgetCategory.Description;
                    context.SaveChanges();
                    ClearProjectDonorList();
                }
            }
            catch (Exception ex)
            { 
                
            }
        }

        public void DeleteBudgetCategory(BudgetCategory budgetCategory)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var bc = context.BudgetCategories.First(b => b.Id == budgetCategory.Id);
                    context.BudgetCategories.Remove(bc);
                    context.SaveChanges();
                    ClearProjectDonorList();
                }
            }
            catch (Exception ex)
            { 
            
            }
        }

        public void DeleteBudgetCategory(string id)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var bc = context.BudgetCategories.First(b => b.Id == new Guid(id));
                    context.BudgetCategories.Remove(bc);
                    context.SaveChanges();
                    ClearProjectDonorList();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public BudgetCategory GetBudgetCategoryById(string id)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var bc = context.BudgetCategories.IncludeProjectDonor().FirstOrDefault(b => b.Id == new Guid(id));
                    return bc;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public BudgetCategory GetBudgetCategoryByNumber(string number, Guid projectDonorId)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var bc = context.BudgetCategories.Where(b => b.Number == number && b.ProjectDonorId == projectDonorId).ToList();
                    if (bc.Count > 0)
                        return bc[0];
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<BudgetCategory> GetBudgetCategories(ProjectDonor pd)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var list = context.BudgetCategories.Where(b => b.ProjectDonorId == pd.Id).OrderBy(b => b.Number).ToList<BudgetCategory>();
                    list = SCMS.Utils.BudgetLineSorter.SortCategory(list);
                    return list;
                }
            }
            catch (Exception ex)
            {
                return new List<BudgetCategory>();
            }
        }

        #endregion 

        #region .BudgetLine CRUD.

        public List<ProjectBudget> GetBudgetLines(BudgetCategory bc)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var list = context.ProjectBudgets.Where(b => b.BudgetCategoryId == bc.Id).OrderBy(b => b.LineNumber).ToList<ProjectBudget>();
                    list = SCMS.Utils.BudgetLineSorter.SortBudgetLine(list);
                    return list;
                }
            }
            catch (Exception ex)
            {
                return new List<ProjectBudget>();
            }
        }

        public void CreateBudgetLine(ProjectBudget bsl)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var newBsl = new ProjectBudget();
                    newBsl.Id = Guid.NewGuid();
                    newBsl.LineNumber = bsl.LineNumber;
                    newBsl.Description = bsl.Description;
                    newBsl.BudgetCategoryId = bsl.BudgetCategoryId;
                    newBsl.TotalBudget = bsl.TotalBudget;
                    newBsl.TotalCommitted = newBsl.TotalPosted = 0;
                    //Save Overrun Adjustment Percentage
                    var bc = context.BudgetCategories.IncludeProjectDonor().FirstOrDefault(b => b.Id == bsl.BudgetCategoryId.Value);
                    if (!bsl.OverrunAdjustment.HasValue)
                        newBsl.OverrunAdjustment = bc.ProjectDonor.OverrunAdjustment;
                    context.ProjectBudgets.Add(newBsl);
                    context.SaveChanges();
                    ClearProjectBudgetList();
                }
            }
            catch (Exception ex)
            { 
            
            }
        }
        public void UpdateBudgetLine(ProjectBudget bsl)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var subLine = context.ProjectBudgets.First(s => s.Id == bsl.Id);
                    subLine.LineNumber = bsl.LineNumber;
                    subLine.Description = bsl.Description;
                    subLine.TotalBudget = bsl.TotalBudget;
                    subLine.OverrunAdjustment = bsl.OverrunAdjustment;
                    context.SaveChanges();
                    ClearProjectBudgetList();
                }
            }
            catch (Exception ex)
            { 
            
            }
        }

        public void DeleteBudgetLine(ProjectBudget bsl)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var subLine = context.ProjectBudgets.First(s => s.Id == bsl.Id);
                    context.ProjectBudgets.Remove(subLine);
                    context.SaveChanges();
                    ClearProjectBudgetList();
                }
            }
            catch (Exception ex)
            {

            }
        }
        
        public void DeleteBudgetLine(Guid subLineId)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var subLine = context.ProjectBudgets.First(s => s.Id == subLineId);
                    context.ProjectBudgets.Remove(subLine);
                    context.SaveChanges();
                    ClearProjectBudgetList();
                }
            }
            catch (Exception ex)
            { 
                
            }
        }
        public ProjectBudget GetBudgetLineById(Guid id)
        {
            return SessionData.CurrentSession.ProjectBudgetList.FirstOrDefault(s => s.Id == id);
        }

        public ProjectBudget GetBudgetLineByNumber(string number, string bcId)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var subLine = context.ProjectBudgets.First(s => s.LineNumber == number && s.BudgetCategoryId == new Guid(bcId));
                    //dummy fetch underlying BudgetCategory
                    var bc = subLine.BudgetCategory;
                    return subLine;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion 

        #region .Budget.
        /// <summary>
        /// Adds new items to the budget. To make sure that items can't be added twice, the UI only presents to the user what's not already in the budget
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="budget"></param>
        public void AddBudgetLines(ProjectDonor pd, List<ProjectBudget> budget)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    foreach (ProjectBudget pb in budget)
                    {
                        var newBgt = new ProjectBudget();
                        newBgt.Id = pb.Id;
                        newBgt.TotalBudget = pb.TotalBudget;
                        newBgt.TotalCommitted = newBgt.TotalPosted = 0;
                        context.ProjectBudgets.Add(newBgt);
                    }
                    context.SaveChanges();
                    ClearProjectBudgetList();
                }
            }
            catch (Exception ex)
            { 
            
            }
        }

        /// <summary>
        /// Saves budget. It only affects the TotalBudget amount
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="budget"></param>
        public void SaveBudgetLines(ProjectDonor pd, List<ProjectBudget> budgetLines)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    foreach (var bl in budgetLines)
                    {
                        var budgetLine = context.ProjectBudgets.FirstOrDefault(b => b.Id == bl.Id);
                        if (budgetLine == null)
                            continue;
                        budgetLine.TotalBudget = bl.TotalBudget;
                    }
                    context.SaveChanges();
                    ClearProjectBudgetList();
                }
            }
            catch (Exception ex)
            { 
            
            }
        }

        public List<ProjectBudget> GetProjectBugdetLines(ProjectDonor pd)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var pbList = context.ProjectBudgets.Where(pb => pb.BudgetCategory.ProjectDonorId == pd.Id).ToList<ProjectBudget>();
                    pbList = SCMS.Utils.BudgetLineSorter.SortBudgetLine(pbList);
                    return pbList;
                }
            }
            catch (Exception ex)
            {
                return new List<ProjectBudget>();
            }
        }

        public List<GeneralLedger> GetGeneralLedgers(Guid countryProgId)
        {
            using (var context = new SCMSEntities())
            {
                return context.GeneralLedgers.Where(g => g.MasterBudgetCategory.CountryProgrammeId == countryProgId).OrderBy(g => g.Code).ToList();
            }
        }

        public List<ProjectBudget> GetProjectBLByCategory(ProjectDonor pd, BudgetCategory bc)
        {
            try
            {
                List<ProjectBudget> projectBL = new List<ProjectBudget>();
                using (var context = new SCMSEntities())
                {
                    var budgetLines = context.ProjectBudgets.Where(b => b.BudgetCategory.ProjectDonorId == pd.Id).ToList<ProjectBudget>();
                    var subLines = context.ProjectBudgets.Where(s => s.BudgetCategoryId == bc.Id).ToList<ProjectBudget>();
                    foreach (var budgetLine in budgetLines)
                    {
                        var curr = budgetLine.BudgetCategory.ProjectDonor.Currency;
                        if (BudgetLineExistsInCategory(budgetLine, subLines))
                            projectBL.Add(budgetLine);
                    }
                }
                projectBL = SCMS.Utils.BudgetLineSorter.SortBudgetLine(projectBL);
                return projectBL;
            }
            catch (Exception ex)
            {
                return new List<ProjectBudget>();
            }
        }

        public List<ProjectBudget> GetBudgetLinesNotInBudget(ProjectDonor pd, BudgetCategory bc)
        {
            try
            {
                List<ProjectBudget> budgetLineList = new List<ProjectBudget>();
                using (var context = new SCMSEntities())
                {
                    var budgetLines = context.ProjectBudgets.Where(b => b.BudgetCategory.ProjectDonorId == pd.Id).ToList<ProjectBudget>();
                    var subLines = context.ProjectBudgets.Where(s => s.BudgetCategoryId == bc.Id).ToList<ProjectBudget>();
                    foreach (var subLine in subLines)
                    {
                        if (!SubLineExistsInBudgetLines(subLine, budgetLines))
                            budgetLineList.Add(subLine);
                    }
                }
                //sort bl
                budgetLineList = SCMS.Utils.BudgetLineSorter.SortBudgetLine(budgetLineList);
                return budgetLineList;
            }
            catch (Exception ex)
            {
                return new List<ProjectBudget>();
            }
        }

        public List<ProjectBudget> GetBudgetLinesNotInMB(BudgetCategory bc)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var pbList = context.ProjectBudgets.Where(b => b.BudgetCategoryId == bc.Id && b.GeneralLedgerId == null).ToList();
                    pbList = SCMS.Utils.BudgetLineSorter.SortBudgetLine(pbList);
                    return pbList;
                }
            }
            catch (Exception ex)
            {
                return new List<ProjectBudget>();
            }
        }

        public List<ProjectBudget> GetBudgetLinesByMBCategory(MasterBudgetCategory mbc, ProjectDonor pd)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var pbList = context.ProjectBudgets.IncludeGeneralLedger().Where(b => b.GeneralLedger.MasterBudgetCategoryId == mbc.Id && b.BudgetCategory.ProjectDonorId == pd.Id).ToList();
                    pbList = SCMS.Utils.BudgetLineSorter.SortBudgetLine(pbList);
                    return pbList;
                }
            }
            catch (Exception ex)
            {
                return new List<ProjectBudget>();
            }
        }

        public List<ProjectBudget> GetBudgetLinesByGLCode(GeneralLedger gl, ProjectDonor pd)
        {
            using (var context = new SCMSEntities())
            {
                var pbList = context.ProjectBudgets.Where(b => b.GeneralLedger.Id == gl.Id && b.BudgetCategory.ProjectDonorId == pd.Id).ToList();
                pbList = SCMS.Utils.BudgetLineSorter.SortBudgetLine(pbList);
                return pbList;
            }
        }

        /// <summary>
        /// Link BudgetLine to MasterBudgetCategory
        /// </summary>
        /// <param name="blId"></param>
        /// <param name="mbcId"></param>
        public void LinkBudgetLineToMasterBudget(Guid blId, Guid? glId)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var pb = context.ProjectBudgets.FirstOrDefault(p => p.Id == blId);
                    if (pb != null)
                    {
                        pb.GeneralLedgerId = glId;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            { 
                
            }
        }

        public void UnLinkBudgetLineToMasterBudget(Guid blId)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var pb = context.ProjectBudgets.FirstOrDefault(p => p.Id == blId);
                    if (pb != null)
                    {
                        pb.GeneralLedgerId = null;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public ProjectBudget GetProjectBudgetById(Guid id)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var pb = context.ProjectBudgets.FirstOrDefault(p => p.Id == id);
                    if (pb != null)
                    {
                        var pd = pb.BudgetCategory.ProjectDonor;
                        return pb;
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }

        /// <summary>
        /// Checks if the budgetLine exists in the categorySubLines
        /// </summary>
        /// <param name="budgetLine"></param>
        /// <param name="categorySubLines"></param>
        /// <returns></returns>
        private bool BudgetLineExistsInCategory(ProjectBudget budgetLine, List<ProjectBudget> categorySubLines)
        {
            foreach (ProjectBudget subLine in categorySubLines)
            {
                if (budgetLine.Id == subLine.Id)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Checks if Category subLine exists in budgetLine
        /// </summary>
        /// <param name="subLine"></param>
        /// <param name="budgetLines"></param>
        /// <returns></returns>
        private bool SubLineExistsInBudgetLines(ProjectBudget subLine, List<ProjectBudget> budgetLines)
        {
            foreach (ProjectBudget budgetLine in budgetLines)
            {
                if (budgetLine.Id == subLine.Id)
                    return true;
            }
            return false;
        }
        
        public List<ProjectDonor> GetProjectDonors(CountryProgramme prog)
        {
            try
            {
                using (var context = SCMSEntities.Define())
                {
                    return context.ProjectDonors.Where(p => p.Project.CountryProgrammeId == prog.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<ProjectDonor>();
            }
        }

        public void SaveBudgetLine(ProjectBudget budgetLine)
        {
            throw new NotImplementedException();
        }

        public void DeleteBudgetLine(ProjectBudget budgetLine, ref ProjectDonor pd)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var pb = context.ProjectBudgets.First(b => b.Id == budgetLine.Id);
                    var dummyCurrency = pd.Currency;
                    var dummyDonor = pd.Donor;
                    context.ProjectBudgets.Remove(pb);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            { 
                
            }
        }

        public bool SufficientFundsAvailable(decimal amount, string budgetLineId, string currencyId, Guid poId, Guid rfpId, Guid orItemId)
        {
            decimal? budgetCurrencyAmount;
            using (var context = new SCMSEntities())
            {
                var orCurrency = context.Currencies.FirstOrDefault(c => c.Id == new Guid(currencyId));
                var pb = context.ProjectBudgets.FirstOrDefault(p => p.Id == new Guid(budgetLineId));
                budgetCurrencyAmount = exchangeRateService.GetForeignCurrencyValue(pb.BudgetCategory.ProjectDonor.Currency, orCurrency, amount, pb.BudgetCategory.ProjectDonor.Project.CountryProgrammeId);
                //Get percentage allowed overrun for bl
                double overrun = 1;
                if (pb.OverrunAdjustment.HasValue)
                {
                    overrun = pb.OverrunAdjustment.Value / 100;
                    overrun += 1;
                }

                //Compute available balance while considering overrun %age cussion
                var blAvailableFunds = (pb.TotalBudget * (decimal)overrun) - pb.TotalCommitted - pb.TotalPosted;

                //Add back commitments from related dox
                if (poId != Guid.Empty)
                {
                    var poItemList = context.PurchaseOrderItems.Where(p => p.PurchaseOrderId == poId).ToList();
                    var orItemIdList = new List<Guid>();
                    foreach (var poItem in poItemList)
                    {
                        if (poItem.OrderRequestItemId.HasValue)
                            orItemIdList.Add(poItem.OrderRequestItemId.Value);
                    }
                    var commits = context.BudgetCommitments.Where(bc => bc.BudgetLineId == new Guid(budgetLineId) && bc.OrderRequestItemId.HasValue == true && orItemIdList.Contains(bc.OrderRequestItemId.Value));
                    foreach (var commit in commits)
                    {
                        blAvailableFunds += commit.AmountCommitted;
                    }
                }
                else if (rfpId != Guid.Empty)
                {
                    var rfpItemList = context.PaymentRequestBudgetLines.Where(p => p.PaymentRequestId == rfpId).ToList();
                    var poItemIdList = new List<Guid>();
                    foreach (var rfpItem in rfpItemList)
                    {
                        poItemIdList.Add(rfpItem.PurchaseOrderItemId.Value);
                    }
                    var commits = context.BudgetCommitments.Where(bc => bc.BudgetLineId == new Guid(budgetLineId) && bc.PurchaseOrderItemId.HasValue == true && poItemIdList.Contains(bc.PurchaseOrderItemId.Value));
                    foreach (var commit in commits)
                    {
                        blAvailableFunds += commit.AmountCommitted;
                    }
                }
                else if (orItemId != Guid.Empty)
                {
                    var commit = context.BudgetCommitments.FirstOrDefault(bc => bc.BudgetLineId == new Guid(budgetLineId) && bc.OrderRequestItemId == orItemId);
                    if (commit != null)
                        blAvailableFunds += commit.AmountCommitted;
                }

                if (budgetCurrencyAmount <= blAvailableFunds)
                    return true;
            }
            return false;
        }

        public List<BudgetCommitment> GetBudgetCommitmentsByLineId(Guid BudgetLineId)
        {
            using (var context = new SCMSEntities())
            {
                return context.BudgetCommitments
                                .IncludeOrderRequest()
                                .IncludePaymentRequest()
                                .IncludePurchaseOrder()
                                .Where(c => c.BudgetLineId == BudgetLineId).OrderByDescending(c => c.DateCommitted).ToList();
            }
        }

        public List<BudgetPosting> GetBudgetPostingsByLineId(Guid BudgetLineId)
        {
            using (var context = new SCMSEntities())
            {
                return context.BudgetPostings
                                            .IncludeDesignation()
                                            .IncludeItemViaRFP()
                                            .IncludeOrderRequestViaRFP()
                                            .IncludePerson()
                                            .IncludeProjectBudgetViaRFP()
                                            .IncludePurchaseOrder()
                                            .IncludeRequestForPayment()
                                            .Where(c => (c.RFPBudgetLineId.HasValue == true && c.PaymentRequestBudgetLine.BudgetLineId == BudgetLineId)).OrderByDescending(c => c.DatePosted).ToList();                
            }
        }

        public List<Rebooking> GetBudgetPostingPartRebookings(Guid BudgetLineId)
        {
            using (var context = new SCMSEntities())
            {
                return context.Rebookings
                    .IncludeProjectDonor()
                    .IncludeDesignation()
                    .IncludePerson()
                    .IncludeItemViaRFP()
                    .IncludeOrderRequestViaRFP()
                    .IncludePurchaseOrder()
                    .IncludeRequestForPayment()
                    .Where(r => r.IsFullRebook == false && r.ToBudgetLineId == BudgetLineId).ToList();
            }
        }

        public BudgetPosting GetBudgetPostingById(Guid Id)
        {
            using (var context = new SCMSEntities())
            {
                var posting = context.BudgetPostings.FirstOrDefault(b => b.Id == Id);
                //deal with the dummies
                var person = posting.Staff.Person;
                var desg = posting.Staff.Designation;

                if (posting.RFPBudgetLineId.HasValue)
                {
                    var rfp = posting.PaymentRequestBudgetLine.PaymentRequest;
                    var po = posting.PaymentRequestBudgetLine.PurchaseOrderItem.PurchaseOrder;
                    var or = posting.PaymentRequestBudgetLine.PurchaseOrderItem.OrderRequestItem.OrderRequest;
                    var item = posting.PaymentRequestBudgetLine.PurchaseOrderItem.OrderRequestItem.Item;
                    var pd = posting.PaymentRequestBudgetLine.ProjectBudget.BudgetCategory.ProjectDonor;
                }
                return posting;
            }
        }

        public void FullRebookPostedFunds(Guid postedId, Guid newBudgetLineId, Staff rebooker)
        {
            using (var context = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        var posting = context.BudgetPostings.IncludeProjectBudgetViaRFP().FirstOrDefault(p => p.Id == postedId);
                        var fromBudgetLineId = posting.PaymentRequestBudgetLine.ProjectBudget.Id;
                        var amount = posting.AmountPosted;
                        //Deduct amount from current budget line
                        posting.PaymentRequestBudgetLine.ProjectBudget.TotalPosted -= amount;
                        var newBL = context.ProjectBudgets.FirstOrDefault(b => b.Id == newBudgetLineId);
                        //Get amount in new budget line currency
                        var newAmount = (decimal)exchangeRateService.GetForeignCurrencyValue(newBL.BudgetCategory.ProjectDonor.Currency, posting.PaymentRequestBudgetLine.ProjectBudget.BudgetCategory.ProjectDonor.Currency, amount, newBL.BudgetCategory.ProjectDonor.Project.CountryProgrammeId);
                        //Add amount to new budget line posted amount
                        newBL.TotalPosted += newAmount;
                        //Set new budget line in BudgetPosting
                        posting.PaymentRequestBudgetLine.BudgetLineId = newBudgetLineId;
                        posting.AmountPosted = newAmount;
                        context.SaveChanges();

                        var rebooking = new Rebooking();
                        rebooking.Id = Guid.NewGuid();
                        rebooking.BudgetPostingId = postedId;
                        rebooking.FromAmount = amount;
                        rebooking.FromBudgetLineId = fromBudgetLineId;
                        rebooking.IsFullRebook = true;
                        rebooking.RebookedBy = rebooker.Id;
                        rebooking.RebookedOn = DateTime.Now;
                        rebooking.ToAmount = newAmount;
                        rebooking.ToBudgetLineId = newBudgetLineId;
                        context.Rebookings.Add(rebooking);
                        context.SaveChanges();
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Amount is in From Budget currency
        /// </summary>
        /// <param name="postedId"></param>
        /// <param name="newBudgetLineId"></param>
        /// <param name="Amount"></param>
        /// <param name="rebooker"></param>
        public void PartRebookPostedFunds(Guid postedId, Guid newBudgetLineId, decimal amount, Staff rebooker)
        {
            using (var context = new SCMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        var posting = context.BudgetPostings.IncludeProjectBudgetViaRFP().FirstOrDefault(p => p.Id == postedId);
                        var fromBudgetLineId = posting.PaymentRequestBudgetLine.ProjectBudget.Id;
                        //Deduct amount from current budget line
                        posting.PaymentRequestBudgetLine.ProjectBudget.TotalPosted -= amount;
                        var newBL = context.ProjectBudgets.FirstOrDefault(b => b.Id == newBudgetLineId);
                        //Get amount in new budget line currency
                        var newAmount = (decimal)exchangeRateService.GetForeignCurrencyValue(newBL.BudgetCategory.ProjectDonor.Currency, posting.PaymentRequestBudgetLine.ProjectBudget.BudgetCategory.ProjectDonor.Currency, amount, newBL.BudgetCategory.ProjectDonor.Project.CountryProgrammeId);
                        //Add amount to new budget line posted amount
                        newBL.TotalPosted += newAmount;
                        //Set new budget line in 
                        posting.AmountPosted -= amount;
                        context.SaveChanges();

                        var rebooking = new Rebooking();
                        rebooking.Id = Guid.NewGuid();
                        rebooking.BudgetPostingId = postedId;
                        rebooking.FromAmount = amount;
                        rebooking.FromBudgetLineId = fromBudgetLineId;
                        rebooking.IsFullRebook = false;
                        rebooking.RebookedBy = rebooker.Id;
                        rebooking.RebookedOn = DateTime.Now;
                        rebooking.ToAmount = newAmount;
                        rebooking.ToBudgetLineId = newBudgetLineId;
                        context.Rebookings.Add(rebooking);
                        context.SaveChanges();
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                    }
                }
            }
        }

        #endregion

        #region .Finance Limits.

        public List<FinanceLimit> GetFinanceLimits(Guid countryProgId)
        {
            using (var context = SCMSEntities.Define())
            {
                return context.FinanceLimits.Where(f => f.CountryProgrammeId == countryProgId).OrderBy(f => f.Limit).ToList();
            }
        }

        public void SaveFinanceLimit(FinanceLimit fl)
        {
            using (var context = new SCMSEntities())
            {
                if (fl.Id.Equals(Guid.Empty))
                {
                    fl.Id = Guid.NewGuid();
                    context.FinanceLimits.Add(fl);
                }
                else
                {
                    context.FinanceLimits.Attach(fl);
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(fl, System.Data.EntityState.Modified);
                }
                context.SaveChanges();
            }
        }

        public FinanceLimit GetFinanceLimitById(Guid Id)
        {
            using (var context = new SCMSEntities())
            {
                return context.FinanceLimits.FirstOrDefault(f => f.Id == Id);
            }
        }
        public void DeleteFinanceLimit(Guid flId)
        {
            using (var context = new SCMSEntities())
            {
                var fl = context.FinanceLimits.FirstOrDefault(f => f.Id == flId);
                context.FinanceLimits.Remove(fl);
                context.SaveChanges();
            }
        }
    
        #endregion
        
        #endregion
    }
}

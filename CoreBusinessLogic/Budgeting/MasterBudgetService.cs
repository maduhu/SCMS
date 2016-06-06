using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.Projects;
using SCMS.Model;
using SCMS.CoreBusinessLogic._ExchangeRate;
using System.Data.Entity.Infrastructure;

namespace SCMS.CoreBusinessLogic.Budgeting
{
    public class MasterBudgetService: IMasterBudgetService
    {
        #region IMasterBudgetService Members

        private IProjectService projectService;
        private IBudgetService budgetService;
        private IExchangeRateService exchangeRateService;

        public MasterBudgetService(IProjectService projectService, IBudgetService budgetService, IExchangeRateService exchangeRateService)
        {
            this.budgetService = budgetService;
            this.projectService = projectService;
            this.exchangeRateService = exchangeRateService;
        }

        public List<MBCategory> GenerateMasterBudget(Currency currency, CountryProgramme cp)
        {
            List<MBCategory> mbCategories = new List<MBCategory>();
            MBCategory mbc;
            List<MasterBudgetProject> mbpList;
            try
            {  
                List<ProjectDonor> projectDonors = projectService.GetCurrentProjectDonors(cp);
                List<MasterBudgetCategory> budgetCategories = GetMasterBudgetCategories(cp);
                foreach (MasterBudgetCategory budgetCategory in budgetCategories)
                {                    
                    mbpList = InitMasterBudgetProjects(budgetCategory, projectDonors, currency);
                    if (mbpList.Count > 0)
                    {
                        mbc = new MBCategory();
                        mbc.EntityBudgetCategory = budgetCategory;
                        mbc.TotalBudget = mbpList[0].TotalBudget;
                        mbc.TotalCommitted = mbpList[0].TotalCommitted;
                        mbc.TotalPosted = mbpList[0].TotalPosted;
                        for (int i = 1; i < mbpList.Count; i++)
                        {
                            mbc.TotalBudget += mbpList[i].TotalBudget;
                            mbc.TotalCommitted += mbpList[i].TotalCommitted;
                            mbc.TotalPosted += mbpList[i].TotalPosted;
                        }
                        mbc.RemainingBalance = mbc.TotalBudget - mbc.TotalCommitted - mbc.TotalPosted;
                        mbc.PercentageSpent = (float?)(mbc.TotalPosted / mbc.TotalBudget) * 100;
                        mbc.CostProjection = GetMBCategoryProjection(mbc.EntityBudgetCategory.Id, cp.Id, currency);
                        mbc.Surplus = mbc.RemainingBalance - mbc.CostProjection;
                        mbc.Projects = mbpList;
                        mbCategories.Add(mbc);
                    }
                }
            }
            catch (Exception ex)
            { 
                
            }
            return mbCategories;
        }

        public List<MBCategory> GenerateGLMasterBudget(Currency currency, CountryProgramme cp)
        {
            List<MBCategory> mbCategories = new List<MBCategory>();
            MBCategory mbc;
            List<MasterBudgetProject> mbpList;
            try
            {
                List<ProjectDonor> projectDonors = projectService.GetCurrentProjectDonors(cp);
                List<GeneralLedger> budgetCategories = GetMasterBudgetGLs(cp);
                foreach (GeneralLedger generalLedger in budgetCategories)
                {
                    mbpList = InitGLMasterBudgetProjects(generalLedger, projectDonors, currency);
                    if (mbpList.Count > 0)
                    {
                        mbc = new MBCategory();
                        mbc.EntityGeneralLedger = generalLedger;
                        mbc.TotalBudget = mbpList[0].TotalBudget;
                        mbc.TotalCommitted = mbpList[0].TotalCommitted;
                        mbc.TotalPosted = mbpList[0].TotalPosted;
                        for (int i = 1; i < mbpList.Count; i++)
                        {
                            mbc.TotalBudget += mbpList[i].TotalBudget;
                            mbc.TotalCommitted += mbpList[i].TotalCommitted;
                            mbc.TotalPosted += mbpList[i].TotalPosted;
                        }
                        mbc.RemainingBalance = mbc.TotalBudget - mbc.TotalCommitted - mbc.TotalPosted;
                        mbc.PercentageSpent = (float?)(mbc.TotalPosted / mbc.TotalBudget) * 100;
                        mbc.Projects = mbpList;
                        mbCategories.Add(mbc);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return mbCategories;
        }

        public MBCategory GetMasterBudgetTotals(CountryProgramme cp)
        {
            try
            {
                List<MBCategory> mbcList = GenerateMasterBudget(cp.Currency, cp);
                MBCategory mbc = new MBCategory();
                mbc.TotalBudget = 0;
                mbc.TotalCommitted = mbc.TotalPosted = mbc.RemainingBalance = mbc.CostProjection = 0;
                foreach (MBCategory mbCat in mbcList)
                {
                    mbc.TotalBudget += mbCat.TotalBudget;
                    mbc.TotalCommitted += mbCat.TotalCommitted;
                    mbc.TotalPosted += mbCat.TotalPosted;
                    mbc.RemainingBalance += mbCat.RemainingBalance;
                    mbc.CostProjection += mbCat.CostProjection;
                }
                mbc.PercentageSpent = (float?)(mbc.TotalPosted / mbc.TotalBudget) * 100;
                mbc.Surplus = mbc.RemainingBalance - mbc.CostProjection;
                return mbc;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mbId">MasterBudgetCategoryId</param>
        /// <param name="cpId">CountryProgrammeId</param>
        /// <returns></returns>
        private decimal? GetMBCategoryProjection(Guid mbId, Guid cpId, Currency mbCurr)
        {
            using (var context = new SCMSEntities())
            {
                var mbp = context.AnnualCostProjections.FirstOrDefault(a => a.MasterBudgetCategoryId == mbId && a.CountryProgrammeId == cpId);
                if (mbp == null)
                    return 0;
                return exchangeRateService.GetForeignCurrencyValue(mbCurr, mbp.Currency, mbp.AmountProjected, cpId);
            }
        }

        private List<MasterBudgetProject> InitMasterBudgetProjects(MasterBudgetCategory mbc, List<ProjectDonor> pdList, Currency currency)
        {
            List<MasterBudgetProject> mbpList = new List<MasterBudgetProject>();
            MasterBudgetProject mbp;
            try
            {                
                using (var context = new SCMSEntities())
                {
                    foreach (ProjectDonor pd in pdList)
                    {
                        var projectBudgets = context.ProjectBudgets.Where(pb => pb.GeneralLedger.MasterBudgetCategoryId == mbc.Id && pb.BudgetCategory.ProjectDonorId == pd.Id).ToList<ProjectBudget>();
                        if (projectBudgets != null && projectBudgets.Count > 0)
                        {
                            mbp = new MasterBudgetProject();
                            mbp.EntityProjectDonor = pd; 
                            mbp.TotalBudget = (decimal)exchangeRateService.GetForeignCurrencyValue(currency, pd.Currency, projectBudgets[0].TotalBudget, pd.Project.CountryProgrammeId);
                            mbp.TotalCommitted = exchangeRateService.GetForeignCurrencyValue(currency, pd.Currency, projectBudgets[0].TotalCommitted, pd.Project.CountryProgrammeId);
                            mbp.TotalPosted = exchangeRateService.GetForeignCurrencyValue(currency, pd.Currency, projectBudgets[0].TotalPosted, pd.Project.CountryProgrammeId);
                            for (int i = 1; i < projectBudgets.Count; i++)
                            {
                                mbp.TotalBudget += (decimal)exchangeRateService.GetForeignCurrencyValue(currency, pd.Currency, projectBudgets[i].TotalBudget, pd.Project.CountryProgrammeId);
                                mbp.TotalCommitted += exchangeRateService.GetForeignCurrencyValue(currency, pd.Currency, projectBudgets[i].TotalCommitted, pd.Project.CountryProgrammeId);
                                mbp.TotalPosted += exchangeRateService.GetForeignCurrencyValue(currency, pd.Currency, projectBudgets[i].TotalPosted, pd.Project.CountryProgrammeId);
                            }
                            mbp.RemainingBalance = mbp.TotalBudget - mbp.TotalCommitted - mbp.TotalPosted;
                            mbp.PercentageSpent = (float?)(mbp.TotalPosted / mbp.TotalBudget) * 100;
                            mbpList.Add(mbp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return mbpList;
        }

        private List<MasterBudgetProject> InitGLMasterBudgetProjects(GeneralLedger gl, List<ProjectDonor> pdList, Currency currency)
        {
            List<MasterBudgetProject> mbpList = new List<MasterBudgetProject>();
            MasterBudgetProject mbp;
            try
            {
                using (var context = new SCMSEntities())
                {
                    foreach (ProjectDonor pd in pdList)
                    {
                        var projectBudgets = context.ProjectBudgets.Where(pb => pb.GeneralLedgerId == gl.Id && pb.BudgetCategory.ProjectDonorId == pd.Id).ToList<ProjectBudget>();
                        if (projectBudgets != null && projectBudgets.Count > 0)
                        {
                            mbp = new MasterBudgetProject();
                            mbp.EntityProjectDonor = pd;
                            mbp.TotalBudget = (decimal)exchangeRateService.GetForeignCurrencyValue(currency, pd.Currency, projectBudgets[0].TotalBudget, pd.Project.CountryProgrammeId);
                            mbp.TotalCommitted = exchangeRateService.GetForeignCurrencyValue(currency, pd.Currency, projectBudgets[0].TotalCommitted, pd.Project.CountryProgrammeId);
                            mbp.TotalPosted = exchangeRateService.GetForeignCurrencyValue(currency, pd.Currency, projectBudgets[0].TotalPosted, pd.Project.CountryProgrammeId);
                            for (int i = 1; i < projectBudgets.Count; i++)
                            {
                                mbp.TotalBudget += (decimal)exchangeRateService.GetForeignCurrencyValue(currency, pd.Currency, projectBudgets[i].TotalBudget, pd.Project.CountryProgrammeId);
                                mbp.TotalCommitted += exchangeRateService.GetForeignCurrencyValue(currency, pd.Currency, projectBudgets[i].TotalCommitted, pd.Project.CountryProgrammeId);
                                mbp.TotalPosted += exchangeRateService.GetForeignCurrencyValue(currency, pd.Currency, projectBudgets[i].TotalPosted, pd.Project.CountryProgrammeId);
                            }
                            mbp.RemainingBalance = mbp.TotalBudget - mbp.TotalCommitted - mbp.TotalPosted;
                            mbp.PercentageSpent = (float?)(mbp.TotalPosted / mbp.TotalBudget) * 100;
                            mbpList.Add(mbp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return mbpList;
        }

        public List<MasterBudgetCategory> GetMasterBudgetCategories(CountryProgramme cp)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var mbcList = context.MasterBudgetCategories.Include("GeneralLedgers").Where(mb => mb.CountryProgrammeId == cp.Id).ToList();
                    mbcList = SCMS.Utils.BudgetLineSorter.SortMBCategory(mbcList);
                    return mbcList;
                }
            }
            catch (Exception ex)
            {
                return new List<MasterBudgetCategory>();
            }
        }

        public List<GeneralLedger> GetMasterBudgetGLs(CountryProgramme cp)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var glList = context.GeneralLedgers.Include("MasterBudgetCategory").Where(mb => mb.MasterBudgetCategory.CountryProgrammeId == cp.Id).ToList();
                    glList = SCMS.Utils.BudgetLineSorter.SortGLMBCategory(glList);
                    return glList;
                }
            }
            catch (Exception ex)
            {
                return new List<GeneralLedger>();
            }
        }

        #region .Master Budget Cateogries CRUD.

        public List<MasterBudgetCategory> GetMBCListNotProjected(CountryProgramme cp)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var mbcList = context.MasterBudgetCategories.Where(m => m.CountryProgrammeId == cp.Id && m.AnnualCostProjections.Count == 0).ToList();
                    mbcList = SCMS.Utils.BudgetLineSorter.SortMBCategory(mbcList);
                    return mbcList;
                }
            }
            catch (Exception ex)
            {
                return new List<MasterBudgetCategory>();
            }
        }

        public MasterBudgetCategory GetMasterBudgetCategoryById(string id)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    return context.MasterBudgetCategories.FirstOrDefault(m => m.Id == new Guid(id));
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public MasterBudgetCategory GetMasterBudgetCategoryByNumber(string number, CountryProgramme cp)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    return context.MasterBudgetCategories.FirstOrDefault(m => m.Number == number && m.CountryProgrammeId == cp.Id);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void CreateMBCategory(MasterBudgetCategory mbc)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var newMbc = new MasterBudgetCategory();
                    newMbc.Id = Guid.NewGuid();
                    newMbc.Number = mbc.Number;
                    newMbc.Description = mbc.Description;
                    newMbc.CountryProgrammeId = mbc.CountryProgrammeId;
                    context.MasterBudgetCategories.Add(newMbc);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            { 
            
            }
        }

        public void UpdateMBCategory(MasterBudgetCategory mbc)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var mbcat = context.MasterBudgetCategories.FirstOrDefault(m => m.Id == mbc.Id);
                    if (mbcat != null)
                    {
                        mbcat.Number = mbc.Number;
                        mbcat.Description = mbc.Description;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            { 
            
            }
        }

        public void DeleteMBCategory(string id)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var mbc = context.MasterBudgetCategories.FirstOrDefault(m => m.Id == new Guid(id));
                    if (mbc != null)
                    {
                        context.MasterBudgetCategories.Remove(mbc);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            { 
            
            }
        }

        #endregion

        #region. Master Budget Projections.

        public List<AnnualCostProjection> GetMBProjections(CountryProgramme cp)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var acpList = context.AnnualCostProjections.Where(a => a.CountryProgrammeId == cp.Id).ToList();
                    foreach (var acp in acpList)
                    {
                        var curr = acp.Currency;
                        var mb = acp.MasterBudgetCategory;
                    }
                    return acpList;
                }
            }
            catch (Exception ex)
            {
                return new List<AnnualCostProjection>();
            }
        }

        public AnnualCostProjection GetMasterBudgetProjectionById(string id)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    return context.AnnualCostProjections.FirstOrDefault(a => a.Id == new Guid(id));
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void CreateMBProjection(AnnualCostProjection acp)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var newAcp = new AnnualCostProjection();
                    newAcp.Id = Guid.NewGuid();
                    newAcp.MasterBudgetCategoryId = acp.MasterBudgetCategoryId;
                    newAcp.CountryProgrammeId = acp.CountryProgrammeId;
                    newAcp.AmountProjected = acp.AmountProjected;
                    newAcp.CurrencyId = acp.CurrencyId;
                    context.AnnualCostProjections.Add(newAcp);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            { 
            
            }
        }

        public void UpdateMBProjection(AnnualCostProjection acp)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var selectedAcp = context.AnnualCostProjections.FirstOrDefault(a => a.Id == acp.Id);
                    if (selectedAcp != null)
                    {
                        selectedAcp.CountryProgrammeId = acp.CountryProgrammeId;
                        selectedAcp.AmountProjected = acp.AmountProjected;
                        selectedAcp.CurrencyId = acp.CurrencyId;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void DeleteMBProjection(string id)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var acp = context.AnnualCostProjections.FirstOrDefault(a => a.Id == new Guid(id));
                    if (acp != null)
                    {
                        context.AnnualCostProjections.Remove(acp);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            { 
                
            }
        }

        #endregion

        #region .General Ledger.

        public GeneralLedger GetGeneralLedgerById(Guid id)
        {
            using (var context = new SCMSEntities())
            {
                return context.GeneralLedgers.Include("MasterBudgetCategory").FirstOrDefault(g => g.Id == id);
            }
        }

        public void SaveGeneralLedger(GeneralLedger gl)
        {
            using (var context = new SCMSEntities())
            {
                if(GLCodeExists(gl, context))
                    return;
                if (gl.Id.Equals(Guid.Empty))
                {
                    gl.Id = Guid.NewGuid();
                    context.GeneralLedgers.Add(gl);
                }
                else
                {
                    context.GeneralLedgers.Attach(gl);
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(gl, System.Data.EntityState.Modified);
                }
                context.SaveChanges();
            }
        }

        private static bool GLCodeExists(GeneralLedger gl, SCMSEntities context)
        {
            var mbc = context.MasterBudgetCategories.FirstOrDefault(c => c.Id == gl.MasterBudgetCategoryId);
            //check if gl code already exists
            var existingGL = context.GeneralLedgers.FirstOrDefault(g => g.Code.Trim() == gl.Code.Trim() && g.MasterBudgetCategory.CountryProgramme.Id == mbc.CountryProgramme.Id && g.Id != gl.Id);
            if (existingGL != null)
                return true;
            return false;
        }

        public void DeleteGeneralLedgerById(Guid id)
        {
            using (var context = new SCMSEntities())
            {
                var gl = context.GeneralLedgers.FirstOrDefault(g => g.Id == id);
                context.GeneralLedgers.Remove(gl);
                context.SaveChanges();
            }
        }

        #endregion

        #endregion
    }
}

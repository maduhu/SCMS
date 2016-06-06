using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.Budgeting
{
    public interface IBudgetService
    {
        void CreateBudgetCategory(BudgetCategory budgetCategory, ProjectDonor pd);
        void UpdateBudgetCategory(BudgetCategory budgetCategory);
        void DeleteBudgetCategory(BudgetCategory budgetCategory);
        void DeleteBudgetCategory(string id);
        void CreateBudgetLine(ProjectBudget bsl);
        void UpdateBudgetLine(ProjectBudget bsl);
        void DeleteBudgetLine(ProjectBudget bsl);
        void DeleteBudgetLine(Guid id);
        List<BudgetCategory> GetBudgetCategories(ProjectDonor pd);
        List<ProjectBudget> GetBudgetLines(BudgetCategory bc);
        BudgetCategory GetBudgetCategoryById(string id);
        BudgetCategory GetBudgetCategoryByNumber(string number, Guid projectDonorId);
        ProjectBudget GetBudgetLineById(Guid id);
        ProjectBudget GetBudgetLineByNumber(string number, string bcId);
        void AddBudgetLines(ProjectDonor pd, List<ProjectBudget> budget);
        void SaveBudgetLines(ProjectDonor pd, List<ProjectBudget> budgetLines);
        List<ProjectBudget> GetProjectBugdetLines(ProjectDonor pd);
        List<ProjectDonor> GetProjectDonors(CountryProgramme prog);
        void SaveBudgetLine(ProjectBudget budgetLine);
        void DeleteBudgetLine(ProjectBudget budgetLine, ref ProjectDonor pd);
        List<ProjectBudget> GetProjectBLByCategory(ProjectDonor pd, BudgetCategory bc);
        List<ProjectBudget> GetBudgetLinesNotInBudget(ProjectDonor pd, BudgetCategory bc);
        List<ProjectBudget> GetBudgetLinesNotInMB(BudgetCategory bc);
        List<ProjectBudget> GetBudgetLinesByMBCategory(MasterBudgetCategory mbc, ProjectDonor pd);
        void LinkBudgetLineToMasterBudget(Guid blId, Guid? glId);
        void UnLinkBudgetLineToMasterBudget(Guid blId);
        ProjectBudget GetProjectBudgetById(Guid id);
        bool SufficientFundsAvailable(decimal amount, string budgetLineId, string currencyId, Guid poId, Guid rfpId, Guid orItemId);
        List<FinanceLimit> GetFinanceLimits(Guid countryProgId);
        void SaveFinanceLimit(FinanceLimit fl);
        FinanceLimit GetFinanceLimitById(Guid Id);
        void DeleteFinanceLimit(Guid flId);
        List<GeneralLedger> GetGeneralLedgers(Guid countryProgId);
        List<ProjectBudget> GetBudgetLinesByGLCode(GeneralLedger gl, ProjectDonor pd);

        List<BudgetCommitment> GetBudgetCommitmentsByLineId(Guid BudgetLineId);
        List<BudgetPosting> GetBudgetPostingsByLineId(Guid BudgetLineId);
        /// <summary>
        /// Get Part Rebookings to this budget line
        /// </summary>
        /// <param name="BudgetLineId"></param>
        /// <returns></returns>
        List<Rebooking> GetBudgetPostingPartRebookings(Guid BudgetLineId);
        BudgetPosting GetBudgetPostingById(Guid Id);
        /// <summary>
        /// Rebook Posted Funds from one budget line to another
        /// </summary>
        /// <param name="postedId"></param>
        /// <param name="newBudgetLineId"></param>
        /// <param name="Amount"></param>
        void FullRebookPostedFunds(Guid postedId, Guid newBudgetLineId, Staff rebooker);
        /// <summary>
        /// Rebook part of the amount to another budget line
        /// </summary>
        /// <param name="postedId"></param>
        /// <param name="newBudgetLineId"></param>
        /// <param name="Amount"></param>
        void PartRebookPostedFunds(Guid postedId, Guid newBudgetLineId, decimal Amount, Staff rebooker);
    }
}

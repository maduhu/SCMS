using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.Budgeting
{
    public interface IMasterBudgetService
    {
        List<MBCategory> GenerateMasterBudget(Currency currency, CountryProgramme cp);
        List<MBCategory> GenerateGLMasterBudget(Currency currency, CountryProgramme cp);
        List<MasterBudgetCategory> GetMasterBudgetCategories(CountryProgramme cp);
        List<AnnualCostProjection> GetMBProjections(CountryProgramme cp);
        List<MasterBudgetCategory> GetMBCListNotProjected(CountryProgramme cp);
        MasterBudgetCategory GetMasterBudgetCategoryById(string id);
        MasterBudgetCategory GetMasterBudgetCategoryByNumber(string number, CountryProgramme cp);
        AnnualCostProjection GetMasterBudgetProjectionById(string id);
        MBCategory GetMasterBudgetTotals(CountryProgramme cp);
        void CreateMBCategory(MasterBudgetCategory mbc);
        void UpdateMBCategory(MasterBudgetCategory mbc);
        void DeleteMBCategory(string id);
        void CreateMBProjection(AnnualCostProjection acp);
        void UpdateMBProjection(AnnualCostProjection acp);
        void DeleteMBProjection(string id);

        GeneralLedger GetGeneralLedgerById(Guid id);
        void SaveGeneralLedger(GeneralLedger gl);
        void DeleteGeneralLedgerById(Guid id);
    }
}

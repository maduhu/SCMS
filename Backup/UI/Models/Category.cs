using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SCMS.Model;
using SCMS.CoreBusinessLogic.Budgeting;
using SCMS.CoreBusinessLogic._ExchangeRate;
using SCMS.CoreBusinessLogic._Currency;

namespace SCMS.UI.Models
{

    public class Category
    {
        public SCMS.Model.BudgetCategory EntityBudgetCategory { get; set; }

        public string Id { get; set; }

        public string Name
        {
            get { return EntityBudgetCategory.Name; }
            set { EntityBudgetCategory.Name = value; }
        }

        public string Number
        {
            get { return EntityBudgetCategory.Number; }
            set { EntityBudgetCategory.Number = value; }
        }

        public string Description
        {
            get { return EntityBudgetCategory.Description; }
            set { EntityBudgetCategory.Description = value; }
        }

        public List<BudgetLine> BudgetLines { get; set; }

        [DataType(DataType.Currency, ErrorMessage = "")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal TotalBudget { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? TotalCommitted { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? TotalPosted { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00;(#,##0.00);0}")]
        public decimal? RemainingBalance { get; set; }

        public SelectList Currencies { get; set; }

        public Guid CurrencyId { get; set; }

        public Guid ProjectDonorId { get; set; }
    }

    #region .Budget Extension.
    
    public static class BudgetExtension
    {
        public static List<Models.Category> PrepareBudgetModel(IBudgetService budgetService, IExchangeRateService exchangeRateService,
            ICurrencyService currencyService, CountryProgramme countryProg, ProjectDonor pd, Currency displayCurrency)
        {
            List<Model.BudgetCategory> bcList = budgetService.GetBudgetCategories(pd);
            List<Model.ProjectBudget> budgetLines;
            List<Models.Category> categories = new List<Models.Category>();
            Models.Category category;
            int i = 0;
            foreach (Model.BudgetCategory bc in bcList)
            {
                budgetLines = budgetService.GetProjectBLByCategory(pd, bc);
                if (budgetLines.Count > 0)
                {
                    category = new Models.Category();
                    category.EntityBudgetCategory = bc;
                    category.Id = bc.Id.ToString();
                    if (i == 0)
                    {
                        category.ProjectDonorId = (Guid)bc.ProjectDonorId;
                        category.Currencies = new SelectList(currencyService.GetCurrencies(countryProg.Id), "Id", "ShortName", displayCurrency.Id);
                        category.CurrencyId = displayCurrency.Id;
                    }
                    AddCategoryLinesWithNumbers(exchangeRateService, category, budgetLines, displayCurrency, countryProg);
                    categories.Add(category);
                }
            }
            if (categories.Count > 0)
                ComputeTotals(categories);
            return categories;
        }

        /// <summary>
        /// This is used mainly for ViewBudget. It calculates the numbers (totals) for each category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="budgetLines"></param>
        private static void AddCategoryLinesWithNumbers(IExchangeRateService exchangeRateService, Models.Category category, 
            List<ProjectBudget> budgetLines, Currency displayCurrency, CountryProgramme countryProg)
        {
            List<Models.BudgetLine> bsl = new List<Models.BudgetLine>();
            Models.BudgetLine subLine;
            decimal budgetAmount;
            decimal? committed, posted;
            budgetAmount = 0;
            committed = posted = 0;
            foreach (ProjectBudget item in budgetLines)
            {
                subLine = new Models.BudgetLine();
                subLine.EntityBudgetLine = item;
                subLine.SubLineId = item.Id;
                subLine.BudgetCategoryId = category.EntityBudgetCategory.Id.ToString();
                subLine.BudgetCategoryNumber = category.EntityBudgetCategory.Number;
                subLine.TotalBudget = (decimal)exchangeRateService.GetForeignCurrencyValue(displayCurrency, item.BudgetCategory.ProjectDonor.Currency, item.TotalBudget, countryProg.Id);
                subLine.TotalCommitted = (decimal)exchangeRateService.GetForeignCurrencyValue(displayCurrency, item.BudgetCategory.ProjectDonor.Currency, item.TotalCommitted, countryProg.Id);
                subLine.TotalPosted = (decimal)exchangeRateService.GetForeignCurrencyValue(displayCurrency, item.BudgetCategory.ProjectDonor.Currency, item.TotalPosted, countryProg.Id);
                subLine.RemainingBalance = subLine.TotalBudget - (subLine.TotalCommitted + subLine.TotalPosted);
                //Sum up for category
                budgetAmount += subLine.TotalBudget;
                committed += subLine.TotalCommitted;
                posted += subLine.TotalPosted;
                bsl.Add(subLine);
            }
            category.BudgetLines = bsl;
            category.TotalBudget = budgetAmount;
            category.TotalCommitted = committed;
            category.TotalPosted = posted;
            category.RemainingBalance = budgetAmount - (committed + posted);
        }

        private static void ComputeTotals(List<Models.Category> categories)
        {
            Models.Category category = new Models.Category();
            category.TotalBudget = 0;
            category.TotalCommitted = category.TotalPosted = category.RemainingBalance = 0;

            foreach (Models.Category cat in categories)
            {
                category.TotalBudget += cat.TotalBudget;
                category.TotalCommitted += cat.TotalCommitted;
                category.TotalPosted += cat.TotalPosted;
                category.RemainingBalance += cat.RemainingBalance;
            }
            categories.Add(category);
        }
    } 
    #endregion
}
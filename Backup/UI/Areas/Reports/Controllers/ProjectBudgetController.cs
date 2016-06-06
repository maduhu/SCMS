using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using OfficeOpenXml;
using SCMS.CoreBusinessLogic.Projects;
using SCMS.Model;
using System.Drawing;
using SCMS.CoreBusinessLogic.Budgeting;
using SCMS.Reports.Budgets;
using OfficeOpenXml.Style;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Reports;
using SCMS.CoreBusinessLogic._Currency;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic._ExchangeRate;

namespace SCMS.UI.Areas.Reports.Controllers
{
    public class ProjectBudgetController : AdminBaseController
    {
        //
        // GET: /Reports/ProjectBudget/
        private IBudgetService budSvc;
        private IProjectService projectService;
        private ICurrencyService currencyService;
        private IExchangeRateService exchangeRateService;

        public ProjectBudgetController(IBudgetService budSvc, IProjectService _projectService, ICurrencyService currencyService,
            IExchangeRateService exchangeRateService, IUserContext userContext)
            : base(userContext)
        {
            this.budSvc = budSvc;
            this.projectService = _projectService;
            this.currencyService = currencyService;
            this.exchangeRateService = exchangeRateService;
        }

        public ActionResult Download(ProjectDonor pdModel)
        {
            ReportService rpSvc = new ReportService();

            ProjectService pSvc = new ProjectService();

            ProjectDonor pDonor = pSvc.GetProjectDonorById(pdModel.Id);
            Currency displayCurrency = currencyService.GetCurrency((Guid)pdModel.CurrencyId);
            if (displayCurrency == null)
                displayCurrency = pDonor.Currency;

            if (pDonor == null)
            {
                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage())
                    {
                        var ws = xlPackage.Workbook.Worksheets.Add("Sheet1");
                        ws.SetValue(1, 1, "Unknown Donor.");
                        xlPackage.SaveAs(stream);
                    }

                    return File(stream.ToArray(), "application/excel", "unknown-donor" + " - " + DateTime.Now.Ticks.ToString() + ".xlsx");
                }
            }


            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage())
                {
                    var ws = xlPackage.Workbook.Worksheets.Add("Project Budget");
                    ws.SetValue(1, 1, "Project No.");
                    ws.SetValue(1, 2, pDonor.Project.ProjectNumber);

                    //format
                    using (ExcelRange r = ws.Cells["A1:B1"])
                    {
                        //r.Merge = true;
                        r.Style.Font.SetFromFont(new Font("Arial", 10, FontStyle.Regular));
                        r.Style.Font.Color.SetColor(Color.Red);
                        //r.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(23, 55, 93));
                    }

                    ws.SetValue(2, 1, "Currency");
                    ws.SetValue(2, 2, displayCurrency.ShortName);

                    //headings
                    ws.SetValue(4, 1, "BUDGETLINE");
                    ws.SetValue(4, 2, "DESCRIPTION");
                    ws.SetValue(4, 3, "TOTAL BUDGET");
                    ws.SetValue(4, 4, "COMMITTED");
                    ws.SetValue(4, 5, "ACTUAL POSTING");
                    ws.SetValue(4, 6, "REMAINING FUND");
                    ws.SetValue(4, 7, "% SPEND");
                    //
                    using (ExcelRange r = ws.Cells["A4:G4"])
                    {
                        r.Style.Font.SetFromFont(new Font("Arial", 10, FontStyle.Regular));
                        //r.Style.Font.UnderLine = true;
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    //List<BudgetCategory> bCats = budSvc.GetBudgetCategories(pDonor);
                    //List<Models.Category> categories = new List<Models.Category>();
                    //foreach (BudgetCategory b in bCats)
                    //{
                    //    List<SCMS.Model.ProjectBudget> pBudgets = budSvc.GetProjectBLByCategory(pDonor, b);

                    //    if (pBudgets.Count > 0)
                    //    {
                    //        Models.Category category = new Models.Category();
                    //        category.EntityBudgetCategory = b;
                    //        category.Id = b.Id.ToString();
                    //        AddCategoryLinesWithNumbers(category, pBudgets, displayCurrency);
                    //        categories.Add(category);
                    //    }
                    //}

                    List<Models.Category> categories = Models.BudgetExtension.PrepareBudgetModel(budSvc, exchangeRateService, currencyService, countryProg, pDonor, displayCurrency);

                    int c = 6;
                    foreach (var cat in categories)
                    {
                        if (cat.BudgetLines == null)
                            continue;
                        foreach (Models.BudgetLine line in cat.BudgetLines)
                        {
                            ws.SetValue(c, 1, line.LineNumber);
                            ws.SetValue(c, 2, line.Description);
                            ws.SetValue(c, 3, line.TotalBudget);
                            ws.SetValue(c, 4, line.TotalCommitted);
                            ws.SetValue(c, 5, line.TotalPosted);
                            ws.SetValue(c, 6, line.RemainingBalance);
                            ws.SetValue(c, 7, 100 * (line.TotalBudget > 0 ? (line.TotalPosted / line.TotalBudget) : 0));
                            c++;
                        }

                        ws.SetValue(c, 1, cat.Number);
                        ws.SetValue(c, 2, cat.Name);
                        ws.SetValue(c, 3, cat.TotalBudget);
                        ws.SetValue(c, 4, cat.TotalCommitted);
                        ws.SetValue(c, 5, cat.TotalPosted);
                        ws.SetValue(c, 6, cat.RemainingBalance);
                        ws.SetValue(c, 7, 100 * (cat.TotalBudget > 0 ? (cat.TotalPosted / cat.TotalBudget) : 0));

                        using (ExcelRange r = ws.Cells["A" + c.ToString() + ":G" + c.ToString()])
                        {
                            r.Style.Font.Bold = true;
                        }
                        c++;
                    }

                    ws.Cells["C3:G" + c.ToString()].Style.Numberformat.Format = "#,##0.00";

                    ws.Cells["A1:G" + c.ToString()].AutoFitColumns();

                    xlPackage.SaveAs(stream);
                }

                return File(stream.ToArray(), "application/excel", pDonor.Project.ProjectNumber + " - " + DateTime.Now.Ticks.ToString() + ".xlsx");
            }


        }


        public ActionResult Detail(ProjectDonor pdModel)
        {
            ProjectDonor pDonor = this.projectService.GetProjectDonorById(pdModel.Id);
            Currency displayCurrency = currencyService.GetCurrency((Guid)pdModel.CurrencyId);
            if (displayCurrency == null)
                displayCurrency = pDonor.Currency;

            List<Models.Category> categories = Models.BudgetExtension.PrepareBudgetModel(budSvc, exchangeRateService, currencyService, countryProg, pDonor, displayCurrency);
            Dictionary<String, String> data = new Dictionary<string, string>();

            data.Add("{START-DATE}", pDonor.StartDate.ToString("dd/MM/yyyy"));
            data.Add("{END-DATE}", pDonor.EndDate.ToString("dd/MM/yyyy"));

            data.Add("{PROJECT}", pDonor.Project.Name);
            data.Add("{DONOR}", pDonor.Donor.Name);
            data.Add("{CURRENCY}", displayCurrency.ShortName);
            data.Add("{BUDGET_NUM}", pDonor.ProjectNumber);

            data.Add("{TOTAL_BUDGET}", categories[categories.Count - 1].TotalBudget.ToString("#,##0.00"));
            data.Add("{TOTAL_COMMITTED}", ND2S(categories[categories.Count - 1].TotalCommitted));
            data.Add("{ACTUAL_POSTED}", ND2S(categories[categories.Count - 1].TotalPosted));
            data.Add("{REMAINING_FUNDS}", ND2S(categories[categories.Count - 1].RemainingBalance));

            data.Add("{GENERATED}", DateTime.Now.ToString("f"));

            String tbody = "";
            for (int k = 0; k < categories.Count - 1; k++)
            {
                tbody += "<tr style=\"background-color: #EAF1DD; font-weight:bold;\"><td class='central'><b>" + categories[k].Number + "</b></td>";
                tbody += "<td><b>" + categories[k].Name + "</b></td>";
                tbody += "<td class='east'><b>" + categories[k].TotalBudget.ToString("#,##0.00") + "</b></td>";
                tbody += "<td class='east'><b>" + ND2S(categories[k].TotalCommitted) + "</b></td>";
                tbody += "<td class='east'><b>" + ND2S(categories[k].TotalPosted) + "</b></td>";
                tbody += "<td class='east'><b>" + ND2S(categories[k].RemainingBalance) + "</b></td></tr>";

                foreach (SCMS.UI.Models.BudgetLine b in categories[k].BudgetLines)
                {
                    tbody += "<tr><td class='central'>" + b.LineNumber + "</td>";
                    tbody += "<td>" + b.Description + "</td>";
                    tbody += "<td class='east'>" + b.TotalBudget.ToString("#,##0.00") + "</td>";
                    tbody += "<td class='east'>" + ND2S(b.TotalCommitted) + "</td>";
                    tbody += "<td class='east'>" + ND2S(b.TotalPosted) + "</td>";
                    tbody += "<td class='east'>" + ND2S(b.RemainingBalance) + "</td></tr>";
                }
            }

            data.Add("{DETAIL}", tbody);

            Byte[] output = WkHtml2Pdf.CreateReport(data, "project-budget.html");//= new Byte[1];;//

            return File(output, "application/pdf", "ProjectB_" + DateTime.Now.FormatDDMMMYYYYHHmm());
        }

        private string ND2S(decimal? num)
        {
            if (num.HasValue)
            {
                return ((decimal)num).ToString("#,##0.00");
            }

            return "";
        }
    }

}

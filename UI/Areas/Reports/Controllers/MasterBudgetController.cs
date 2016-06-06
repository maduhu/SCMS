using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using OfficeOpenXml;
using SCMS.CoreBusinessLogic.Budgeting;
using SCMS.CoreBusinessLogic.Projects;
using SCMS.CoreBusinessLogic._ExchangeRate;
using SCMS.Model;
using SCMS.CoreBusinessLogic._Currency;
using System.Drawing;
using OfficeOpenXml.Style;
using SCMS.Reports;
using SCMS.CoreBusinessLogic._CountryProgramme;
using SCMS.CoreBusinessLogic.Web;
using SCMS.CoreBusinessLogic.Security;
using SCMS.UI.Models;

namespace SCMS.UI.Areas.Reports.Controllers
{
    public class MasterBudgetController : PortalBaseController
    {
        private IMasterBudgetService mbService;
        private ICurrencyService currencyService;

        private ICountryProgrammeService countryProgrammeSvc;

        public MasterBudgetController(IPermissionService permissionService, IMasterBudgetService mbService, ICurrencyService _currencyService,
            ICountryProgrammeService _countryProgmSvc, IUserContext userContext)
            : base(userContext, permissionService)
        {
            this.mbService = mbService;
            this.currencyService = _currencyService;
            this.countryProgrammeSvc = _countryProgmSvc;
        }

        public ActionResult PrintMasterB()
        {
            if (!permissionService.Authorize(StandardPermissionProvider.MasterBudgetView))
            {
                return AccessDeniedView();
            }

            Dictionary<String, String> data = new Dictionary<string, string>();
            string mb_currency = ""; int pjCount = 0;
            Models.MasterBudgetHeader mb = MBHeader(ref mb_currency);

            data.Add("{CURRENCY}", mb_currency);

            data.Add("{TOTAL_BUDGET}", ND2S(mb.MBCList[mb.MBCList.Count - 1].TotalBudget));
            data.Add("{TOTAL_COMMITTED}", ND2S(mb.MBCList[mb.MBCList.Count - 1].TotalCommitted));
            data.Add("{TOTAL_POSTED}", ND2S(mb.MBCList[mb.MBCList.Count - 1].TotalPosted));
            data.Add("{REMAINING_FUNDS}", ND2S(mb.MBCList[mb.MBCList.Count - 1].RemainingBalance));
            data.Add("{%_ACTUAL_SPENT}", ND2S((decimal)mb.MBCList[mb.MBCList.Count - 1].PercentageSpent));
            data.Add("{FUNDS_AVAILABLE}", ND2S(mb.MBCList[mb.MBCList.Count - 1].RemainingBalance));
            data.Add("{EXPENDITURE_PROJECTION}", ND2S(mb.MBCList[mb.MBCList.Count - 1].CostProjection));
            if (mb.MBCList[mb.MBCList.Count - 1].Surplus < 0)
                data.Add("{SHORTFALL_SURPLUS}", "<font color=\"red\"><b>" + ND2S(mb.MBCList[mb.MBCList.Count - 1].Surplus) + "</b></font>");
            else
                data.Add("{SHORTFALL_SURPLUS}", ND2S(mb.MBCList[mb.MBCList.Count - 1].Surplus));

            string DetailsHTML = ""; int x = 0;
            for (int j = 0; j < mb.MBCList.Count - 1; j++)
            {
                if (x >= 0) { x = x + 1; pjCount = mb.MBCList[j].Projects.Count; }

                DetailsHTML += "<tr><td rowspan=\"" + pjCount + "\" align=\"center\" valign=\"middle\" style=\"background-color: #FCD5B4;\"><b>";
                DetailsHTML += mb.MBCList[j].EntityBudgetCategory.Description + "</b>";
                DetailsHTML += "</td><td  style=\"background-color: #FCD5B4;\">";
                DetailsHTML += mb.MBCList[j].Projects[0].EntityProjectDonor.ProjectNumber;
                DetailsHTML += "</td><td  style=\"background-color: #FCD5B4;\">";
                DetailsHTML += mb.MBCList[j].Projects[0].EntityProjectDonor.Donor.ShortName;
                DetailsHTML += "</td><td align=\"right\" style=\"background-color: #DCE6F1;\">";
                DetailsHTML += ND2S(mb.MBCList[j].Projects[0].TotalBudget);
                DetailsHTML += "</td><td align=\"right\" style=\"background-color: #DCE6F1;\">";
                DetailsHTML += ND2S(mb.MBCList[j].Projects[0].TotalCommitted);
                DetailsHTML += "</td><td align=\"right\" style=\"background-color: #DCE6F1;\">";
                DetailsHTML += ND2S(mb.MBCList[j].Projects[0].TotalPosted);
                DetailsHTML += "</td><td align=\"right\" style=\"background-color: #DCE6F1;\">";
                DetailsHTML += ND2S(mb.MBCList[j].Projects[0].RemainingBalance);
                DetailsHTML += "</td><td align=\"right\" style=\"background-color: #DCE6F1;\">";
                DetailsHTML += ND2S((decimal)mb.MBCList[j].Projects[0].PercentageSpent);
                DetailsHTML += "</td><td rowspan=\"" + pjCount + "\" align=\"center\" valign=\"middle\" style=\"background-color: #DCE6F1; border-bottom-style: none;\"><b>";
                DetailsHTML += ND2S(mb.MBCList[j].RemainingBalance) + "</b>";
                DetailsHTML += "</td><td rowspan=\"" + pjCount + "\" align=\"center\" valign=\"middle\" style=\"background-color: #C4D79B; border-bottom-style: none;\"><b>";
                if (mb.MBCList[j].CostProjection > 0)
                    DetailsHTML += ND2S(mb.MBCList[j].CostProjection) + "</b>";
                DetailsHTML += "</td><td rowspan=\"" + pjCount + "\" align=\"center\" valign=\"middle\" style=\"background-color: #C4D79B; border-bottom-style: none;\">";
                if (mb.MBCList[j].CostProjection > 0)
                {
                    if (mb.MBCList[j].Surplus < 0)
                        DetailsHTML += "<font color=\"red\"><b>" + ND2S(mb.MBCList[j].Surplus) + "</b></font>";
                    else
                        DetailsHTML += "<b>" + ND2S(mb.MBCList[j].Surplus) + "</b>";
                }
                DetailsHTML += "</td></tr>";

                for (int i = 1; i < mb.MBCList[j].Projects.Count; i++)
                {
                    DetailsHTML += "<tr><td style=\"background-color: #FCD5B4;\">";
                    DetailsHTML += mb.MBCList[j].Projects[i].EntityProjectDonor.ProjectNumber;
                    DetailsHTML += "</td><td style=\"background-color: #FCD5B4;\">";
                    DetailsHTML += mb.MBCList[j].Projects[i].EntityProjectDonor.Donor.Name;
                    DetailsHTML += "</td><td align=\"right\" style=\"background-color: #DCE6F1;\">";
                    DetailsHTML += ND2S(mb.MBCList[j].Projects[i].TotalBudget);
                    DetailsHTML += "</td><td align=\"right\" style=\"background-color: #DCE6F1;\">";
                    DetailsHTML += ND2S(mb.MBCList[j].Projects[i].TotalCommitted);
                    DetailsHTML += "</td><td align=\"right\" style=\"background-color: #DCE6F1;\">";
                    DetailsHTML += ND2S(mb.MBCList[j].Projects[i].TotalPosted);
                    DetailsHTML += "</td><td align=\"right\" style=\"background-color: #DCE6F1;\">";
                    DetailsHTML += ND2S(mb.MBCList[j].Projects[i].RemainingBalance);
                    DetailsHTML += "</td><td align=\"right\" style=\"background-color: #DCE6F1;\">";
                    DetailsHTML += ND2S((decimal)mb.MBCList[j].Projects[i].PercentageSpent);
                    DetailsHTML += "</td></tr>";
                }

                DetailsHTML += "<tr><td colspan=\"11\">&nbsp;</td></tr>";
            }

            data.Add("DETAIL", DetailsHTML);

            List<String> options = new List<string>();
            options.Add(" --orientation Landscape ");
            //options.Add(" --copies 3 ");
            Byte[] output = WkHtml2Pdf.CreateReport(data, "master-budget.html", options);
            return File(output, "application/pdf", "Project_Budget_" + DateTime.Now.FormatDDMMMYYYYHHmm());
        }

        public ActionResult Summary(Guid currencyId, Guid countryProgrammeId)
        {
            Dictionary<String, String> data = new Dictionary<string, string>();

            Currency currency = currencyService.GetCurrency(currencyId);
            data.Add("{CURRENCY}", currency.Name);

            CountryProgramme cp = countryProgrammeSvc.GetCountryProgrammeById(countryProgrammeId);

            List<MBCategory> mbcList = mbService.GenerateMasterBudget(currency, cp);
            this.ComputeTotals(mbcList);

            decimal totalCommitted = 0;
            if (mbcList[mbcList.Count - 1].TotalCommitted.HasValue)
            {
                totalCommitted = (decimal)mbcList[mbcList.Count - 1].TotalCommitted;
            }

            decimal totalPosted = 0;
            if (mbcList[mbcList.Count - 1].TotalPosted.HasValue)
            {
                totalPosted = (decimal)mbcList[mbcList.Count - 1].TotalPosted;
            }

            decimal remBal = 0;
            if (mbcList[mbcList.Count - 1].RemainingBalance.HasValue)
            {
                remBal = (decimal)mbcList[mbcList.Count - 1].RemainingBalance;
            }

            float pctSpent = 0;
            if (mbcList[mbcList.Count - 1].PercentageSpent.HasValue)
            {
                pctSpent = (float)mbcList[mbcList.Count - 1].PercentageSpent;
            }

            decimal costP = 0;

            if (mbcList[mbcList.Count - 1].CostProjection.HasValue)
            {
                costP = (decimal)mbcList[mbcList.Count - 1].CostProjection;
            }


            decimal surplus = 0;
            string style = " style='color:red;' ";

            if (mbcList[mbcList.Count - 1].Surplus.HasValue)
            {
                surplus = (decimal)mbcList[mbcList.Count - 1].Surplus;
            }

            if (surplus > 0)
            {
                style = "";
            }

            data.Add("{TOTAL_BUDGET}", mbcList[mbcList.Count - 1].TotalBudget.ToString("###,###.00"));
            data.Add("{TOTAL_COMMITTED}", totalCommitted.ToString("###,###.00"));
            data.Add("{TOTAL_POSTED}", totalPosted.ToString("###,###.00"));
            data.Add("{REMAINING_FUNDS}", remBal.ToString("###,###.00"));
            data.Add("{%_ACTUAL_SPENT}", pctSpent.ToString("0.00"));
            data.Add("{FUNDS_AVAILABLE}", remBal.ToString("###,###.00"));
            data.Add("{EXPENDITURE_PROJECTION}", costP.ToString("###,###.00"));
            data.Add("{SHORTFALL_SURPLUS}", "<span" + style + ">" + surplus.ToString("###,###.00") + "</span>");

            string tbody = "";

            for (int i = 0; i < mbcList.Count - 1; i++)
            {
                tbody += "<tr><td>";
                tbody += mbcList[i].EntityBudgetCategory.Description + "</td><td>";
                tbody += mbcList[i].Projects[0].EntityProjectDonor.ProjectNumber + "</td><td>";
                tbody += mbcList[i].Projects[0].EntityProjectDonor.Donor.ShortName + "</td><td>";
                tbody += mbcList[i].Projects[0].TotalBudget.ToString("###,###.00") + "</td><td>";
                tbody += ND2S(mbcList[i].Projects[0].TotalCommitted) + "</td><td>";
                tbody += ND2S(mbcList[i].Projects[0].TotalPosted) + "</td><td>";
                tbody += ND2S(mbcList[i].Projects[0].RemainingBalance) + "</td><td>";
                tbody += FD2S(mbcList[i].Projects[0].PercentageSpent) + "</td><td>";
                tbody += ND2S(mbcList[i].CostProjection) + "</td><td>";
                tbody += "</tr>";
            }

            data.Add("{DETAIL}", tbody);

            List<String> options = new List<string>();
            options.Add(" --orientation Landscape ");

            Byte[] output = WkHtml2Pdf.CreateReport(data, "master-budget.html", options);

            return File(output, "application/pdf", "Project_Bu=dget_" + DateTime.Now.FormatDDMMMYYYYHHmm());
        }


        public MasterBudgetHeader MBHeader(ref string mb_currency)
        {
            Models.MasterBudgetHeader mbh = new Models.MasterBudgetHeader();
            List<MBCategory> mbcList = mbService.GenerateMasterBudget(mbCurrency, countryProg);
            ComputeTotals(mbcList);
            mb_currency = mbCurrency.ShortName;
            ViewBag.CurrencyId = mbCurrency.Id;
            ViewBag.CountryProgrammeId = countryProg.Id;
            ViewBag.CPName = countryProg.ProgrammeName;
            mbh.MBCList = mbcList;
            return mbh;

        }

        public ActionResult Detail(Guid currencyId, Guid countryProgrammeId)
        {
            Dictionary<String, String> data = new Dictionary<string, string>();

            Currency currency = currencyService.GetCurrency(currencyId);
            data.Add("{CURRENCY}", currency.Name);

            CountryProgramme cp = countryProgrammeSvc.GetCountryProgrammeById(countryProgrammeId);

            List<MBCategory> mbcList = mbService.GenerateMasterBudget(currency, cp);
            this.ComputeTotals(mbcList);

            decimal totalCommitted = 0;
            if (mbcList[mbcList.Count - 1].TotalCommitted.HasValue)
            {
                totalCommitted = (decimal)mbcList[mbcList.Count - 1].TotalCommitted;
            }

            decimal totalPosted = 0;
            if (mbcList[mbcList.Count - 1].TotalPosted.HasValue)
            {
                totalPosted = (decimal)mbcList[mbcList.Count - 1].TotalPosted;
            }

            decimal remBal = 0;
            if (mbcList[mbcList.Count - 1].RemainingBalance.HasValue)
            {
                remBal = (decimal)mbcList[mbcList.Count - 1].RemainingBalance;
            }

            float pctSpent = 0;
            if (mbcList[mbcList.Count - 1].PercentageSpent.HasValue)
            {
                pctSpent = (float)mbcList[mbcList.Count - 1].PercentageSpent;
            }

            decimal costP = 0;

            if (mbcList[mbcList.Count - 1].CostProjection.HasValue)
            {
                costP = (decimal)mbcList[mbcList.Count - 1].CostProjection;
            }


            decimal surplus = 0;
            string style = " style='color:red;' ";

            if (mbcList[mbcList.Count - 1].Surplus.HasValue)
            {
                surplus = (decimal)mbcList[mbcList.Count - 1].Surplus;
            }

            if (surplus > 0)
            {
                style = "";
            }

            data.Add("{TOTAL_BUDGET}", mbcList[mbcList.Count - 1].TotalBudget.ToString("###,0.00"));
            data.Add("{TOTAL_COMMITTED}", totalCommitted.ToString("###,0.00"));
            data.Add("{TOTAL_POSTED}", totalPosted.ToString("###,0.00"));
            data.Add("{REMAINING_FUNDS}", remBal.ToString("###,0.00"));
            data.Add("{%_ACTUAL_SPENT}", pctSpent.ToString("0.00"));
            data.Add("{FUNDS_AVAILABLE}", remBal.ToString("###,0.00"));
            data.Add("{EXPENDITURE_PROJECTION}", costP.ToString("###,0.00"));
            data.Add("{SHORTFALL_SURPLUS}", "<span" + style + ">" + surplus.ToString("###,0.00") + "</span>");

            string tbody = "";

            for (int i = 0; i < mbcList.Count - 1; i++)
            {
                int pCount = mbcList[i].Projects.Count;
                int rowSpan = 1 + pCount;

                tbody += "<tr><td  style='vertical-align:middle;' rowspan='" + rowSpan + "'><h4>";
                tbody += mbcList[i].EntityBudgetCategory.Description + "</h4></td></tr>";

                for (int k = 0; k < pCount; k++)
                {
                    tbody += "<tr><td class='central'>";
                    tbody += mbcList[i].Projects[k].EntityProjectDonor.ProjectNumber + "</td><td>";
                    tbody += mbcList[i].Projects[k].EntityProjectDonor.Donor.ShortName + "</td><td class='east'>";
                    tbody += mbcList[i].Projects[k].TotalBudget.ToString("###,0.00") + "</td><td class='east'>";
                    tbody += ND2S(mbcList[i].Projects[k].TotalCommitted) + "</td><td class='east'>";
                    tbody += ND2S(mbcList[i].Projects[k].TotalPosted) + "</td><td class='east'>";
                    tbody += ND2S(mbcList[i].Projects[k].RemainingBalance) + "</td><td class='east'>";
                    tbody += FD2S(mbcList[i].Projects[k].PercentageSpent);

                    if (k == 0)
                    {
                        tbody += "</td><td class='east'  style='vertical-align:middle;'  rowspan='" + rowSpan + "'><b>";
                        tbody += WrapMinus(ND2S(mbcList[i].RemainingBalance)) + "</b></td>";
                        tbody += "<td class='east'  style='vertical-align:middle;'  rowspan='" + rowSpan + "'><b>";
                        tbody += WrapMinus(ND2S(mbcList[i].CostProjection)) + "</b></td>";
                        tbody += "<td class='east'  style='vertical-align:middle;'  rowspan='" + rowSpan + "'><b>";
                        tbody += WrapMinus(ND2S(mbcList[i].Surplus)) + "</b></td></tr>";
                    }
                    else
                    {
                        tbody += "</td></tr>";
                    }
                }

                tbody += "<tr><td colspan='3'><h4 class='central'>";
                tbody += mbcList[i].EntityBudgetCategory.Description + "</h4></td><td class='east'><b>";
                tbody += mbcList[i].TotalBudget.ToString("###,0.00") + "</b></td><td class='east'><b>";
                tbody += ND2S(mbcList[i].TotalCommitted) + "</b></td><td class='east'><b>";
                tbody += ND2S(mbcList[i].TotalPosted) + "</b></td><td class='east'><b>";
                tbody += WrapMinus(ND2S(mbcList[i].RemainingBalance)) + "</b></td><td class='east'><b>";
                tbody += FD2S(mbcList[i].PercentageSpent) + "</b></td>";
                tbody += "</tr>";

                tbody += "<tr><td colspan='11'>&nbsp;&nbsp;</td></tr>";

            }

            data.Add("{DETAIL}", tbody);
            data.Add("{GENERATED}", DateTime.Now.ToString("f"));

            List<String> options = new List<string>();
            options.Add(" --orientation Landscape ");

            Byte[] output = WkHtml2Pdf.CreateReport(data, "master-budget.html", options);

            return File(output, "application/pdf");
        }

        private void ComputeTotals(List<MBCategory> mbcList)
        {
            MBCategory mbc = new MBCategory();
            mbc.TotalBudget = 0;
            mbc.TotalCommitted = mbc.TotalPosted = mbc.RemainingBalance = mbc.CostProjection = mbc.Surplus = 0;
            foreach (MBCategory mbCat in mbcList)
            {
                mbc.TotalBudget += mbCat.TotalBudget;
                mbc.TotalCommitted += mbCat.TotalCommitted;
                mbc.TotalPosted += mbCat.TotalPosted;
                mbc.RemainingBalance += mbCat.RemainingBalance;
                mbc.CostProjection += mbCat.CostProjection;
            }
            mbc.PercentageSpent = mbc.TotalBudget > 0 ? (float?)(mbc.TotalPosted / mbc.TotalBudget) * 100 : 0;
            mbc.Surplus = mbc.RemainingBalance - mbc.CostProjection;
            mbcList.Add(mbc);
        }

        private string ND2S(decimal? num)
        {
            if (num.HasValue)
            {
                return ((decimal)num).ToString("###,0.00");
            }

            return "";
        }

        private string FD2S(float? num)
        {
            if (num.HasValue)
            {
                return ((float)num).ToString("###,0.00");
            }

            return "";
        }

        private string WrapMinus(string val)
        {
            if (val.Contains("-"))
            {
                return "<span style='color:red'>" + val + "</span>";
            }
            return val;
        }

        public ActionResult Download(String id)
        {
            List<MBCategory> mbcList = this.mbService.GenerateMasterBudget(mbCurrency, countryProg);

            if (mbcList != null && mbcList.Count > 0)
            {
                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage())
                    {
                        var ws = xlPackage.Workbook.Worksheets.Add("MASTER BUDGET EX");
                        ws.SetValue(1, 1, "MASTERBUDGET");

                        ws.SetValue(3, 1, "CATEGORY");
                        ws.SetValue(3, 2, "PROJECT NO");
                        ws.SetValue(3, 3, "DONOR");
                        ws.SetValue(3, 4, "DESCRIPTION");
                        ws.SetValue(3, 5, "TOTAL BUDGET");
                        ws.SetValue(3, 6, "TOTAL COMMITTED");
                        ws.SetValue(3, 7, "ACTUAL POSTING");
                        ws.SetValue(3, 8, "REMAINING FUNDS/PROJECT");
                        ws.SetValue(3, 9, "% ACTUAL SPEND");
                        ws.SetValue(3, 10, "COUNTRY PROGRAMME FUNDS AVAILABLE");
                        ws.SetValue(3, 11, "EXPENDITURE PROJECTION");
                        ws.SetValue(3, 12, "SHORTFALL/SURPLUS");

                        using (ExcelRange r = ws.Cells["A3:D3"])
                        {
                            //worksheet.Cells["A4:G4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0XFC, 0XD5, 0XB4));
                        }
                        using (ExcelRange r = ws.Cells["E3:J3"])
                        {
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0XDC, 0XE6, 0XF1));
                        }
                        using (ExcelRange r = ws.Cells["K3:L3"])
                        {
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0XC2, 0XD6, 0X9A));
                        }

                        int nextRow = 5;
                        foreach (MBCategory cat in mbcList)
                        {


                            nextRow++;
                            int startRow = nextRow;

                            for (int i = 0; i < cat.Projects.Count; i++)
                            {
                                ws.SetValue(nextRow, 2, cat.Projects[i].EntityProjectDonor.ProjectNumber);
                                ws.SetValue(nextRow, 3, cat.Projects[i].EntityProjectDonor.Donor.Name);
                                ws.SetValue(nextRow, 4, cat.EntityBudgetCategory.Description);

                                ws.SetValue(nextRow, 5, cat.Projects[i].TotalBudget);
                                ws.SetValue(nextRow, 6, cat.Projects[i].TotalCommitted);
                                ws.SetValue(nextRow, 7, cat.Projects[i].TotalPosted);
                                ws.SetValue(nextRow, 8, cat.Projects[i].RemainingBalance);
                                ws.SetValue(nextRow, 9, cat.Projects[i].PercentageSpent);
                                nextRow++;
                            }

                            ws.SetValue(startRow, 1, cat.EntityBudgetCategory.Description);

                            ws.SetValue(nextRow, 4, cat.EntityBudgetCategory.Description);
                            ws.SetValue(nextRow, 5, cat.TotalBudget);
                            ws.SetValue(nextRow, 7, cat.TotalPosted);
                            ws.SetValue(nextRow, 6, cat.TotalCommitted);
                            ws.SetValue(nextRow, 8, cat.RemainingBalance);
                            ws.SetValue(nextRow, 9, cat.PercentageSpent);

                            ws.SetValue(startRow, 10, cat.RemainingBalance);
                            ws.SetValue(startRow, 11, "");
                            ws.SetValue(startRow, 12, "");


                            using (ExcelRange r = ws.Cells[startRow, 5, nextRow, 10])
                            {
                                r.Style.Numberformat.Format = "0,000.00";
                                r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                r.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0XDC, 0XE6, 0XF1));
                            }

                            using (ExcelRange r = ws.Cells[startRow, 1, nextRow - 1, 4])
                            {
                                r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                r.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0XFC, 0XD5, 0XB4));
                            }

                            ws.Cells["D" + nextRow.ToString() + ":I" + nextRow.ToString()].Style.Font.Bold = true;

                            ws.Cells["D" + nextRow.ToString() + ":D" + nextRow.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["D" + nextRow.ToString() + ":D" + nextRow.ToString()].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0XFC, 0XD5, 0XB4));

                            ws.Cells[startRow, 4, nextRow - 1, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells[startRow, 4, nextRow - 1, 4].Merge = true;



                            nextRow++;
                        }
                        ws.Cells["A1:L" + nextRow.ToString()].AutoFitColumns();

                        xlPackage.SaveAs(stream);
                    }

                    return File(stream.ToArray(), "application/excel", "master-budget" + " - " + DateTime.Now.Ticks.ToString() + ".xlsx");
                }
            }

            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage())
                {
                    var ws = xlPackage.Workbook.Worksheets.Add("Sheet1");
                    ws.SetValue(1, 1, "Unknown Masterbudget.");
                    xlPackage.SaveAs(stream);
                }

                return File(stream.ToArray(), "application/excel", "unknown-master-budget" + " - " + DateTime.Now.Ticks.ToString() + ".xlsx");
            }



        }

    }
}

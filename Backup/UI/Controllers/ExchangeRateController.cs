using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic._ExchangeRate;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class ExchangeRateController : PortalBaseController
    {
        private IExchangeRateService exchangeRateService;

        public ExchangeRateController(IPermissionService permissionService, IUserContext userContext, IExchangeRateService _exchangeRateService)
            : base(userContext, permissionService)
        {
            this.exchangeRateService = _exchangeRateService;
        }

        //
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: CreateEdit
        public ActionResult CreateEdit(string xid)
        {

            UExchangeRate exRate = new UExchangeRate();
            exRate.CurrencySelect = new SelectList(exchangeRateService.CurrencyObj.GetCurrencies(countryProg.Id), "Id", "Name");
            exRate.MainCurrencyId = mbCurrency.Id.ToString();
            exRate.MonthSelect = new SelectList(ExchangeRateExtension.GetMonths(), "Month", "MonthName");
            exRate.Month = DateTime.Today.Month;
            exRate.Year = DateTime.Today.Year;
            Guid excRateId;
            if (Guid.TryParse(xid, out excRateId))
            {
                ViewBag.Action = "Edit";
                exRate._ExchangeRate = exchangeRateService.GetExchangeRate(excRateId);
            }
            return View(exRate);
        }

        //
        // POST: CreateEdit
        [HttpPost]
        public ActionResult CreateEdit(UExchangeRate exRate, string Action = "")
        {
            if (ModelState.IsValid)
            {
                if (!exRate.Id.Equals(Guid.Empty))
                {
                    exRate._ExchangeRate.CountryProgrammeId = countryProg.Id;
                    if (exchangeRateService.EditExchangeRate(exRate._ExchangeRate))
                    {
                        ModelState.Clear();
                        exRate = new UExchangeRate();
                    }
                }
                else
                {
                    exRate._ExchangeRate.CountryProgrammeId = countryProg.Id;
                    if (exchangeRateService.AddExchangeRate(exRate._ExchangeRate))
                    {
                        ModelState.Clear();
                        exRate = new UExchangeRate();
                    }
                }
            }

            exRate.CurrencySelect = new SelectList(exchangeRateService.CurrencyObj.GetCurrencies(countryProg.Id), "Id", "Name");
            return ListView();
        }

        //
        // GET: ListView
        public ActionResult ListView(string search = "")
        {
            List<ExchangeRateService.ExchangeRateView> exRates = exchangeRateService.GetExchangeRates1(countryProg.Id);
            return View("ListView", exRates);
        }

        //
        // GET: DeleteItem
        public ActionResult DeleteItem(string xid)
        {
            Guid exRateId;
            if (Guid.TryParse(xid, out exRateId))
            {
                if (exchangeRateService.DeleteExchangeRate(exRateId))
                {
                    ModelState.Clear();
                }
            }

            List<ExchangeRateService.ExchangeRateView> exRates = exchangeRateService.GetExchangeRates1(countryProg.Id);
            return View("ListView", exRates);
        }

    }
}

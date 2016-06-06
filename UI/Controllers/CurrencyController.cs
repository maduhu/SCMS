using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.CoreBusinessLogic;
using SCMS.CoreBusinessLogic._Currency;
using SCMS.UI.Models;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.Resource;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class CurrencyController : PortalBaseController
    {
        private ICurrencyService currencyService;

        public CurrencyController(IPermissionService permissionService, IUserContext userContext, ICurrencyService _currencyService)
            : base(userContext, permissionService)
        {
            this.currencyService = _currencyService;
        }

        //
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: CreateEdit
        public ActionResult CreateEdit(string cid)
        {
            UCurrency currency = new UCurrency();

            Guid currencyId;
            if (Guid.TryParse(cid, out currencyId))
            {
                ViewBag.Action = Resources.Global_String_Update;
                currency._Currency = currencyService.GetCurrency(currencyId);
            }
            return View(currency);
        }

        //
        // POST: CreateEdit
        [HttpPost]
        public ActionResult CreateEdit(UCurrency currency, string Action)
        {
            if (ModelState.IsValid)
            {
                if (!currency._Currency.Id.Equals(Guid.Empty))
                {
                    currency._Currency.CountryProgrammeId = countryProg.Id;
                    if (currencyService.EditCurrency(currency._Currency))
                    {
                        currency = new UCurrency();
                        ModelState.Clear();
                    }
                }
                else
                {
                    currency._Currency.CountryProgrammeId = countryProg.Id;
                    if (currencyService.AddCurrency(currency._Currency))
                    {
                        currency = new UCurrency();
                        ModelState.Clear();
                    }
                }
            }
            return ListView();
        }

        //
        // GET: ListView
        public ActionResult ListView(string search = "")
        {
            List<Currency> currencies = currencyService.GetCurrencies(countryProg.Id, search);
            return View("ListView", currencies);
        }

        //
        // GET: DeleteItem
        public ActionResult DeleteItem(string cid)
        {
            Guid currencyId;
            if (Guid.TryParse(cid, out currencyId))
            {
                currencyService.DeleteCurrency(currencyId);
            }

            List<Currency> currencies = currencyService.GetCurrencies(countryProg.Id);
            return View("ListView", currencies);
        }
    }
}

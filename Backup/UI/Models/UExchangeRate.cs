using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SCMS.Model;
using SCMS.Resource;

namespace SCMS.UI.Models
{
    public class UExchangeRate
    {
        public UExchangeRate()
        {
            this._ExchangeRate = new SCMS.Model.ExchangeRate();
        }

        public SelectList CurrencySelect { get; set; }

        public ExchangeRate _ExchangeRate { get; set; }

        public Guid Id
        {
            get
            {
                return _ExchangeRate.Id;
            }
            set
            {
                _ExchangeRate.Id = value;
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public double? Rate
        {
            get { return _ExchangeRate.Rate; }
            set { _ExchangeRate.Rate = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public int Month
        {
            get { return _ExchangeRate.Month; }
            set { _ExchangeRate.Month = value; }
        }

        public SelectList MonthSelect { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public int Year
        {
            get { return _ExchangeRate.Year; }
            set { _ExchangeRate.Year = value; }
        }

        public SelectList YearSelect 
        {
            get
            {
                List<int> num = new List<int>();
                for (int i = DateTime.Now.Year; i >= DateTime.Now.Year - 10; i--)
                {
                    num.Add(i);
                }
                return new SelectList(num);
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string MainCurrencyId
        {
            get
            {
                return _ExchangeRate.MainCurrencyId.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _ExchangeRate.MainCurrencyId = new Guid(value);
                }
            }
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string LocalCurrencyId
        {
            get
            {
                return _ExchangeRate.LocalCurrencyId.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _ExchangeRate.LocalCurrencyId = new Guid(value);
                }
            }
        }
    }

    public class FxMonth
    {
        public int Month { get; set; }

        public string MonthName { get; set; }
    }

    public static class ExchangeRateExtension
    {
        private static string[] months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        public static List<FxMonth> GetMonths()
        {
            List<FxMonth> monthList = new List<FxMonth>();
            int i = 1;
            foreach (string month in months)
                monthList.Add(new FxMonth { Month = i++, MonthName = month });
            return monthList;
        }
    }
}
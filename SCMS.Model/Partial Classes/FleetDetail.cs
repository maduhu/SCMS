using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace SCMS.Model
{

    [MetadataType(typeof(FleetDetailMetaData))]
    public partial class FleetDetail
    {
        public SelectList FDList { get; set; }
        public SelectList Brands { get; set; }
        public SelectList Modelz { get; set; }
        public SelectList Assets { get; set; }
        public SelectList InsuranceTypes { get; set; }
        public SelectList DutyTypes { get; set; }
        public SelectList Staffs { get; set; }
        public string Action { get; set; }
        public string AssetOrPlateNo { get { return PlateNo == null ? Asset.AssetNameAndNo : Asset.Name + " (" + PlateNo + ")"; } }
        public string AvrgFuelConsumption { get; set; }
    }

    public class FleetDetailMetaData
    {
        [Required]
        public object AssetId { get; set; }
        //----------this has to be validated------
        //[Required]
        //public object ModelId { get; set; }
        //[Required]
        //public object ModelYear { get; set; }
        //[Required]
        //public object InsuranceTypeId { get; set; }
        //[Required]
        //public object InsuranceExpiryDate { get; set; }
        //[Required]
        //public object DutyTypeId { get; set; }
        //[Required]
        //public object FuelType { get; set; }
        //[Required]
        //public object FleetType { get; set; }
        //[Required]
       //// [Remote("IsPlateNoExist", "FleetManager", AdditionalFields = "Id")]
        //public object PlateNo { get; set; }

        //--------------Dont remember this-------
        //[Required]
        //public object AverageDailyFuelConsumption { get; set; }
        //[Required]
        //public object AverageDailyFuelCost { get; set; }

    }

    [MetadataType(typeof(FleetDailyManagementMetaData))]
    public partial class FleetDailyManagement
    {
        public SelectList Incidences { get; set; }
        public SelectList Staffs { get; set; }
        public double? LastMilleage { get; set; }
        public double? CurrentStartMilleage { get; set; }
        public string FleetTypLable { get; set; }
        public string Action { get; set; }
        public string SavedId { get; set; }

        public string IncidenceName { get { return FleetMajorIncidence == null ? FleetMajorIncidence.Name : string.Empty; } }
    }

    public class FleetDailyManagementMetaData
    {
        [Required]
        //[Range(typeof(DateTime), "2012-10-29", "2012-11-2", ErrorMessage = "Value for {0} must be between {1} and {2}")]
        //[CurrentDate(ErrorMessage = "")]
        public object Date { get; set; }
        [Required]
        public object StartMilleage { get; set; }
        [Required]
        public object EndMilleage { get; set; }
        [Required]
        public object DistancedCovered { get; set; }
        [Required]
        public object TotalFuelConsumed { get; set; }
        [Required]
        public object IncidenceId { get; set; }
        [Required]
        public object FuelCost { get; set; }
    }

    public class CurrentDateAttribute : ValidationAttribute
    {
        public CurrentDateAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            var dt = (DateTime)value;
            if (dt >= DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }

    public class DateRangeAttribute : ValidationAttribute
    {
        private const string DateFormat = "yyyy/MM/dd";
        private const string DefaultErrorMessage = "'{0}' must be a date between {1:d} and {2:d}.";

        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }

        public DateRangeAttribute(string minDate, string maxDate)
            : base(DefaultErrorMessage)
        {
            MinDate = ParseDate(minDate);
            MaxDate = ParseDate(maxDate);
        }

        public override bool IsValid(object value)
        {
            if (value == null || !(value is DateTime))
            {
                return true;
            }
            DateTime dateValue = (DateTime)value;
            return MinDate <= dateValue && dateValue <= MaxDate;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MinDate, MaxDate);
        }

        private static DateTime ParseDate(string dateValue)
        {
            return DateTime.ParseExact(dateValue, DateFormat, CultureInfo.InvariantCulture);
        }
    }


    [MetadataType(typeof(GarageInfoMetaData))]
    public partial class GarageInfo
    {
        public SelectList Currencies { get; set; }
        public SelectList Suppliers { get; set; }
        public SelectList MileageCategory { get; set; }
        public string MaintenaceType { get; set; }
        public Guid? ChecKListCatId { get; set; }
        public string SupplierName { get { return Supplier != null ? Supplier.Name : string.Empty; } }
        public string SavedId { get; set; }
    }

    public class GarageInfoMetaData
    {
        [Required]
        public object SupplierId { get; set; }
        [Required]
        public object Dateken { get; set; }
        [Required]
        public object ReasonType { get; set; }
        [Required]
        public object CurrencyId { get; set; }
        [Required]
        public object Cost { get; set; }
        [Required]
        public object DateOutOfGarage { get; set; }
        [Required]
        public object IfServiceCurrentMilleage { get; set; }
        //[Required]
        //public object IfServiceNextMilleage { get; set; }
        [Required]
        public object MilleageOut { get; set; }
        [DataType(DataType.MultilineText)]
        public object OtherProblemsDescription { get; set; }

    }

    [MetadataType(typeof(Equipment2FleetMetaData))]
    public partial class Equipment2Fleet
    {
        public SelectList Accesories { get; set; }
        public int rowCount { get; set; }
        public string EquipmentName { get { return FleetEquipment != null ? FleetEquipment.EquipmentName : string.Empty; } }
    }

    public class Equipment2FleetMetaData
    {
        [Required]
        public object EquipmentId { get; set; }
        [Required]
        public object Quantity { get; set; }

    }
}

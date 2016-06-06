using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentValidation.Attributes;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.UI.Areas.Admin.Validators.SystemUser;
using System.ComponentModel.DataAnnotations;
using SCMS.Resource;

namespace SCMS.UI.Areas.Admin.Models.SystemUser
{
    //[Validator(typeof(SystemUserModelValidator))]
    public class SystemUserModel : BaseModel
    {
        public SystemUserModel()
        {
            AvailableRoles=new List<SelectListItem>();
            AvailableCountrySubOffices=new List<SelectListItem>();
            AvailableDesignation=new List<SelectListItem>();
        }

        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string OtherNames { get; set; }
        [Required]
        public string Email { get; set; }

        public string LastIpAddress { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public bool Active { get; set; }

        public bool Locked { get; set; }

        public int UserLoginCount { get; set; }

        [Required]
        public string Password { get; set; }
        [Compare("Password", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_PasswordMismatch")]
        public string ConfirmPassword { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public List<SelectListItem> AvailableRoles { get; set; }
        public Guid[] SelectedRoleIds { get; set; }

        public string OfficialPhone { get; set; }
        [Required]
        public string IdNumber { get; set; }

        public List<SelectListItem> AvailableDesignation { get; set; }
        [Required]
        public Guid SelectedDesignationId { get; set; }

        public List<SelectListItem> AvailableCountrySubOffices { get; set; }
        [Required]
        public Guid? SelectedCountrySubOfficeId { get; set; }

        public SelectList FinanceLimits { get; set; }
        [Required]
        public Guid? FinanceLimitId { get; set; }

        public bool Available { get; set; }

        public string PhotoLocation { get; set; }

        public HttpPostedFileBase PhotoLocationUpload { get; set; }

        public string SignatureImage { get; set; }

        public HttpPostedFileBase SignatureImageUpload { get; set; }

        public Guid? PersonSignatureImageId { get; set; }

    }

    public static class SystemUserModelExtensions
    {
        public static Staff ToEntity(this SystemUserModel model, Model.Staff staff)
        {
            staff = staff ?? new Staff
                                 {
                                     Id = Guid.NewGuid()
                                 };
            staff.DesignationId = model.SelectedDesignationId;
            staff.CountrySubOfficeId = model.SelectedCountrySubOfficeId;
            if (model.FinanceLimitId.HasValue)
                staff.FinanceLimitId = (Guid)model.FinanceLimitId;
            return staff;
        }

        public static Model.Person ToEntity(this SystemUserModel model, Model.Person person)
        {
            person = person ?? new Person
                                   {
                                       Id = Guid.NewGuid()
                                   };
            if (model.Email != null)
                person.OfficialEmail = model.Email;
            person.FirstName = model.FirstName;
            person.OtherNames = model.OtherNames;
            person.IDNo = model.IdNumber;
            person.OfficialPhone = model.OfficialPhone;
            return person;
        }

        public static Model.SystemUser ToEntity(this SystemUserModel model, Model.SystemUser systemUser=null)
        {
            var user = systemUser ?? new Model.SystemUser()
            {
                Id = Guid.NewGuid(),
                Created = DateTime.Now,
                Modified = DateTime.Now,
            };
            user.Active = model.Active;
            user.Locked = model.Locked;
            
            return user;
        }

        public static SystemUserModel ToModel(this Model.SystemUser systemUser)
        {
            return new SystemUserModel
                       {
                           Id = systemUser.Id,
                           Active = systemUser.Active,
                           Created = systemUser.Created,
                           Email = systemUser.Staff != null ? systemUser.Staff.Person.OfficialEmail : "",
                           FirstName = systemUser.Staff != null ? systemUser.Staff.Person.FirstName: "",
                           LastIpAddress = systemUser.LastIpAddress,
                           LastLoginDate = systemUser.LastLoginDate,
                           OtherNames = systemUser.Staff != null ? systemUser.Staff.Person.OtherNames: "",
                           Locked = systemUser.Locked,
                           Modified = systemUser.Modified,
                           UserLoginCount = systemUser.UserLoginCount,
                           SelectedCountrySubOfficeId = systemUser.Staff != null ? systemUser.Staff.CountrySubOfficeId: null,
                           SelectedDesignationId = systemUser.Staff != null ? systemUser.Staff.DesignationId : Guid.Empty,
                           OfficialPhone = systemUser.Staff != null ? systemUser.Staff.Person.OfficialPhone :"",
                           IdNumber = systemUser.Staff != null ? systemUser.Staff.Person.IDNo : "",
                           PhotoLocation = systemUser.Staff != null ? systemUser.Staff.Person.PhotoLocation : "",
                           PersonSignatureImageId = systemUser.Staff != null && systemUser.Staff.Person.SignatureImage != null ? (Guid?)systemUser.Staff.Person.Id : null,
                           FinanceLimitId = systemUser.Staff.FinanceLimitId,
                           Available = systemUser.IsAvailable                          
                       };
        }
    }

    public class ChangePassword
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string OldPassword { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string NewPassword { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Global_String_Required")]
        public string ConfirmNewPassword { get; set; }
    }
}
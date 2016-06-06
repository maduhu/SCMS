using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentValidation.Attributes;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.UI.Validators.Person;
using SCMS.Resource;

namespace SCMS.UI.Models.Person
{
    [Validator(typeof(SystemUserModelValidator))]
    public class SystemUserModel : BaseModel
    {

        public string FirstName { get; set; }

        public string OtherNames { get; set; }

        public string Email { get; set; }

        public string OldPassword { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string OfficialPhone { get; set; }

        public string IdNumber { get; set; }
                
        public string PhotoLocation { get; set; }
        public HttpPostedFileBase PhotoLocationUpload { get; set; }

        public string SignatureImage { get; set; }
        public HttpPostedFileBase SignatureImageUpload { get; set; }

        public Guid? PersonSignatureImageId { get; set; }

    }

    public static class SystemUserModelExtensions
    {
        public static Model.Person ToEntity(this SystemUserModel model, Model.Person person)
        {
            person = person ?? new Model.Person
                                   {
                                       Id = Guid.NewGuid()
                                   };
            person.OfficialEmail = model.Email;
            person.FirstName = model.FirstName;
            person.OtherNames = model.OtherNames;
            person.IDNo = model.IdNumber;
            person.OfficialPhone = model.OfficialPhone;
            return person;
        }

        public static SystemUserModel ToModel(this Model.SystemUser systemUser)
        {
            return new SystemUserModel
                       {
                           Email = systemUser.Staff != null ? systemUser.Staff.Person.OfficialEmail : "",
                           FirstName = systemUser.Staff != null ? systemUser.Staff.Person.FirstName: "",
                           OtherNames = systemUser.Staff != null ? systemUser.Staff.Person.OtherNames: "",
                           OfficialPhone = systemUser.Staff != null ? systemUser.Staff.Person.OfficialPhone :"",
                           IdNumber = systemUser.Staff != null ? systemUser.Staff.Person.IDNo : "",
                           PhotoLocation = systemUser.Staff != null ? systemUser.Staff.Person.PhotoLocation : "",
                           PersonSignatureImageId = systemUser.Staff != null && systemUser.Staff.Person.SignatureImage != null ? (Guid?)systemUser.Staff.Person.Id : null,
                       };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.People;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Settings;
using SCMS.CoreBusinessLogic.Users;
using SCMS.CoreBusinessLogic.Web;
using SCMS.UI.Models.Person;
using SCMS.CoreBusinessLogic.ActionFilters;
using SCMS.Resource;

namespace SCMS.UI.Controllers
{
    [MyException]
    public class PersonController : PortalBaseController
    {
        private readonly IPersonService m_PersonService;
        private readonly ImageService m_ImageService;
        private readonly IUserContext m_UserContext;
        private readonly ISystemUserService m_SystemUserService;

        public PersonController(IPermissionService permissionService, IPersonService personService, ImageService imageService, IUserContext userContext, ISystemUserService systemUserService)
            : base(userContext, permissionService)
        {
            m_PersonService = personService;
            m_ImageService = imageService;
            m_UserContext = userContext;
            m_SystemUserService = systemUserService;
        }

        [HttpGet]
        public ActionResult Photo(Guid id)
        {
            var person = m_PersonService.GetPersonById(id);
            if(person == null || person.SignatureImage == null)
            {
                return Content("");
            }
            
            return new FileContentResult(person.SignatureImage, "image/jpeg");
        }

        [HttpGet]
        public ActionResult Profile()
        {
            var user = m_SystemUserService.GetUserById(m_UserContext.CurrentUser.Id, false);
            if (user == null)
                return RedirectToAction("Index","Home");

            var model = user.ToModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Profile(SystemUserModel model)
        {
            ValidateImageUploaded(model, false);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = m_SystemUserService.GetUserById(m_UserContext.CurrentUser.Id, false);
            if (user == null)
                return RedirectToAction("Index","Home");

            if (user.Staff != null && user.Staff.Person != null && user.Staff.Person.OfficialEmail != model.Email)
            {
                var emailUser = m_SystemUserService.FindUsers(new UserFilter { Email = model.Email }, 0, 1).FirstOrDefault();
                if (emailUser != null)
                {                   
                    ModelState.AddModelError("", Resources.PersonController_String_EmailAlreadyInUse.F(model.Email));
                    return View(model);
                }
            }

            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    var person = m_PersonService.GetPersonById(user.Staff.Person.Id);
                    person = model.ToEntity(person);
                    SaveImages(person, model);
                    m_PersonService.UpdatePerson(person);

                    if (model.Password.IsNotNullOrWhiteSpace())
                    {
                        m_SystemUserService.ChangePassword(new ChangePasswordRequest(model.Email, true, model.Password, model.OldPassword));
                    }

                    transactionScope.Complete();

                    return RedirectToAction("Index","Home");
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("", exception.Message);
            }

            return View(model);
        }

        private void SaveImages(Model.Person person, SystemUserModel model)
        {

            if (model.PhotoLocationUpload != null && model.PhotoLocationUpload.ContentLength > 0)
            {
                var destinationImagePath = SettingsHelper<CommonSettings>.Settings.PersonPhotoUploadPath;
                var physicalPath = Path.GetFullPath(Server.MapPath(destinationImagePath));
                if (!Directory.Exists(physicalPath))
                    Directory.CreateDirectory(physicalPath);
                var fileExtension = Path.GetExtension(model.PhotoLocationUpload.FileName);
                var newPath = Path.Combine(destinationImagePath, Guid.NewGuid() + fileExtension);
                model.PhotoLocationUpload.SaveAs(Path.GetFullPath(Server.MapPath(newPath)));
                person.PhotoLocation = newPath.Replace('\\', '/');
            }

            if (model.SignatureImageUpload != null && model.SignatureImageUpload.ContentLength > 0)
            {
                var destination = new Byte[model.SignatureImageUpload.ContentLength];
                model.SignatureImageUpload.InputStream.Seek(0, SeekOrigin.Begin);
                model.SignatureImageUpload.InputStream.Read(destination, 0, model.SignatureImageUpload.ContentLength);
                person.SignatureImage = destination;
            }
        }

        private bool ValidateImageUploaded(SystemUserModel model, bool createMode = true)
        {
            model.PhotoLocationUpload = (model.PhotoLocationUpload ?? Request.Files["PhotoLocationUpload"]);
            if (model.PhotoLocationUpload != null && model.PhotoLocationUpload.ContentLength > 0)
            {
                var fileExtension = Path.GetExtension(model.PhotoLocationUpload.FileName).TrimStart(new[] { '.' });
                if (!SettingsHelper<CommonSettings>.Settings.AllowedImageExtensions.Any(p => p.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase)))
                {
                    ModelState.AddModelError("PhotoLocationUpload", Resources.GoodsReceivedNoteController_String_FileExtensionNotSupported);
                }
                else
                {
                    if (model.PhotoLocationUpload.ContentLength >
                        SettingsHelper<CommonSettings>.Settings.PersonPhotoMaxSize * 1024)
                    {
                        ModelState.AddModelError("PhotoLocationUpload", 
                                                 Resources.GoodsReceivedNoteController_String_ImageSizeMsg.F(
                                                     SettingsHelper<CommonSettings>.Settings.PersonPhotoMaxSize * 1024,
                                                     model.PhotoLocationUpload.ContentLength));
                    }
                    else if (!m_ImageService.IsImage(model.PhotoLocationUpload.InputStream))
                    {
                        ModelState.AddModelError("PhotoLocationUpload", Resources.GoodsReceivedNoteController_String_FileUploadedNotValid);
                    }
                }

                model.PhotoLocationUpload.InputStream.Seek(0, SeekOrigin.Begin);
            }

            model.SignatureImageUpload = (model.SignatureImageUpload ?? Request.Files["SignatureImageUpload"]);
            if (model.SignatureImageUpload != null && model.SignatureImageUpload.ContentLength > 0)
            {
                var fileExtension = Path.GetExtension(model.SignatureImageUpload.FileName).TrimStart(new[] { '.' });
                if (!SettingsHelper<CommonSettings>.Settings.AllowedImageExtensions.Any(p => p.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase)))
                {
                    ModelState.AddModelError("SignatureImageUpload", Resources.GoodsReceivedNoteController_String_FileExtensionNotSupported);
                }
                else
                {
                    if (model.SignatureImageUpload.ContentLength >
                        SettingsHelper<CommonSettings>.Settings.SignaturePhotoMaxSize * 1024)
                    {
                        ModelState.AddModelError("SignatureImageUpload",
                                                 Resources.GoodsReceivedNoteController_String_ImageSizeMsg.F(
                                                     SettingsHelper<CommonSettings>.Settings.SignaturePhotoMaxSize * 1024,
                                                     model.SignatureImageUpload.ContentLength));
                    }
                    else if (!m_ImageService.IsImage(model.SignatureImageUpload.InputStream))
                    {
                        ModelState.AddModelError("SignatureImageUpload", Resources.GoodsReceivedNoteController_String_FileExtensionNotSupported);
                    }
                }

                model.SignatureImageUpload.InputStream.Seek(0, SeekOrigin.Begin);
            }

            return ModelState.IsValid;
        }

    }
}

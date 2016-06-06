using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic._CountrySubOffice;
using SCMS.CoreBusinessLogic._Designation;
using SCMS.CoreBusinessLogic.People;
using SCMS.CoreBusinessLogic.Security;
using SCMS.CoreBusinessLogic.Settings;
using SCMS.CoreBusinessLogic.StaffServices;
using SCMS.CoreBusinessLogic.Users;
using SCMS.CoreBusinessLogic.Web;
using SCMS.Model;
using SCMS.UI.Areas.Admin.Models.SystemUser;
using SCMS.CoreBusinessLogic.Budgeting;
using SCMS.CoreBusinessLogic.NotificationsManager;
using System.Text;
using SCMS.Resource;
using SCMS.CoreBusinessLogic.Caching;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;

namespace SCMS.UI.Areas.Admin.Controllers
{
    public class SystemUserController : AdminBaseController
    {
        private readonly ISystemUserService m_SystemUserService;
        private readonly IImageService m_ImageService;
        private readonly IPermissionService m_PermissionService;
        private readonly IPersonService m_PersonService;
        private readonly IStaffService m_StaffService;
        private readonly ICountrySubOfficeService m_CountrySubOfficeService;
        private readonly IDesignationService m_DesignationService;
        private readonly IBudgetService m_budgetService;
        private readonly INotificationService m_notificationService;
        private readonly ICacheService m_cacheService;

        public SystemUserController(ISystemUserService systemUserService, IImageService imageService, IPermissionService permissionService,
            IPersonService personService, IStaffService staffService, ICountrySubOfficeService countrySubOfficeService, IBudgetService budgetService,
            IDesignationService designationService, INotificationService notificationService, IUserContext usercontext, ICacheService cacheService)
            : base(usercontext)
        {
            m_SystemUserService = systemUserService;
            m_ImageService = imageService;
            m_PermissionService = permissionService;
            m_PersonService = personService;
            m_StaffService = staffService;
            m_CountrySubOfficeService = countrySubOfficeService;
            m_DesignationService = designationService;
            m_budgetService = budgetService;
            m_notificationService = notificationService;
            m_cacheService = cacheService;
        }

        private const int UserListPageSize = 1000;//30;

        //
        // GET: /Admin/SystemUser/

        public ActionResult Index()
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.SystemUsersManage))
            {
                return AccessDeniedView();
            }

            return View();
        }

        public ActionResult List(SystemUserListModel model, int p = 1)
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.SystemUsersManage))
            {
                return AccessDeniedView();
            }
            var filter = new UserFilter
                             {
                                 Active = model.Active,
                                 Email = model.Email,
                                 Locked = model.Locked,
                                 Name = model.Name,
                                 RoleIds = model.SelectedRoleIds,
                                 CountryProgrammeId = countryProg.Id
                             };
            if (Request.HttpMethod.Equals("get", StringComparison.InvariantCultureIgnoreCase))
            {
                filter.Locked = filter.Active = null;
            }
            p = Math.Max(p, 1) - 1;

            var users = m_SystemUserService.FindUsers(filter, p, UserListPageSize);
            model.Users = users;
            model.PagingUrl = WebHelper.ModifyQueryString(Request.RawUrl, "p=_pagenum_", null);
            model.PagingUrlPageKey = "_pagenum_";

            var roles = m_SystemUserService.GetAllRoles(false);
            if (roles.IsNotNullOrEmpty())
            {
                roles.ForEach(r => model.AvailableRoles.Add(new SelectListItem { Text = r.Name, Value = r.Id.ToString(), Selected = model.SelectedRoleIds.IsNotNullOrEmpty() && model.SelectedRoleIds.Contains(r.Id) }));
            }

            return View("List", model);
        }

        private void PrepareSystemUserModel(SystemUserModel model)
        {
            var roles = m_SystemUserService.GetAllRoles(false);

            if (model.Id.IsNotEmpty())
            {
                var userRoles = m_SystemUserService.GetUserRoles(model.Id, false);
                if (userRoles.IsNotNullOrEmpty())
                {
                    model.SelectedRoleIds = userRoles.Select(p => p.RoleId).ToArray();
                }
            }

            if (roles.IsNotNullOrEmpty())
            {
                roles.ForEach(p => model.AvailableRoles.Add(new SelectListItem { Text = p.Name, Value = p.Id.ToString(), Selected = model.SelectedRoleIds.IsNotNullOrEmpty() && model.SelectedRoleIds.Contains(p.Id) }));
            }

            var availableDesignations = m_DesignationService.GetDesignations(countryProg.Id);
            if (availableDesignations.IsNotNullOrEmpty())
            {
                availableDesignations.ForEach(p => model.AvailableDesignation.Add(new SelectListItem { Text = p.Name, Value = p.Id.ToString(), Selected = model.SelectedDesignationId == p.Id }));
            }

            var availableCountrySuboffices = m_CountrySubOfficeService.GetCountrySubOffices1(countryProg.Id);
            if (availableCountrySuboffices.IsNotNullOrEmpty())
            {
                availableCountrySuboffices.OrderBy(p => p.countryProgramme.ProgrammeName)
                    .ForEach(p => model.AvailableCountrySubOffices.Add(new SelectListItem
                                                                          {
                                                                              Text = p.countrySubOffice.Name,
                                                                              Value = p.countrySubOffice.Id.ToString(),
                                                                              Selected = p.countrySubOffice.Id == model.SelectedCountrySubOfficeId
                                                                          }));
            }

            var financeLimits = m_budgetService.GetFinanceLimits(countryProg.Id);
            model.Password = "";
            model.FinanceLimits = new SelectList(financeLimits, "Id", "Name");
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.SystemUsersManage))
            {
                return AccessDeniedView();
            }
            var model = new SystemUserModel();
            PrepareSystemUserModel(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SystemUserModel model)
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.SystemUsersManage))
            {
                return AccessDeniedView();
            }

            ValidateImageUploaded(model, true);

            //if (!ModelState.IsValid)
            //{
            //    PrepareSystemUserModel(model);
            //    return View(model);
            //}

            var user = m_SystemUserService.FindUsers(new UserFilter { Email = model.Email }, 0, 1).FirstOrDefault();
            if (user != null)
            {
                ModelState.AddModelError("", Resources.PersonController_String_EmailAlreadyInUse.F(model.Email));
                PrepareSystemUserModel(model);
                return View(model);
            }

            try
            {

                using (var transactionScope = new TransactionScope())
                {
                    Person person = null;
                    person = model.ToEntity(person);
                    person.CountryProgrammeId = countryProg.Id;
                    SaveImages(person, model);
                    m_PersonService.InsertPerson(person);

                    Staff staff = null;
                    staff = model.ToEntity(staff);
                    staff.PersonId = person.Id;
                    m_StaffService.InsertStaff(staff);

                    var entity = model.ToEntity(user);
                    entity.Password = entity.PasswordSalt = entity.PIN = string.Empty;
                    entity.StaffId = staff.Id;
                    entity.Modified = DateTime.Now;
                    m_SystemUserService.InsertUser(entity);

                    m_SystemUserService.ChangePassword(new ChangePasswordRequest(model.Email, false, model.Password));
                    if (m_PermissionService.Authorize(StandardPermissionProvider.RolesManage) && model.SelectedRoleIds.IsNotNullOrEmpty())
                        m_SystemUserService.AssignRoles(entity, model.SelectedRoleIds);

                    transactionScope.Complete();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("", exception.Message);
            }
            PrepareSystemUserModel(model);

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.SystemUsersManage))
            {
                return AccessDeniedView();
            }

            var user = m_SystemUserService.GetUserById(id, false);
            if (user == null)
                return RedirectToAction("Index");

            var model = user.ToModel();
            PrepareSystemUserModel(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SystemUserModel model)
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.SystemUsersManage))
            {
                return AccessDeniedView();
            }

            if (model.Id.IsEmpty())
                return RedirectToAction("Index");

            //ValidateImageUploaded(model, false);

            //if (!ModelState.IsValid)
            //{
            //    PrepareSystemUserModel(model);
            //    return View(model);
            //}

            var user = m_SystemUserService.GetUserById(model.Id, false);
            if (user == null)
                return RedirectToAction("Index");

            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    var person = user.Staff != null && user.Staff.Person != null
                        ? m_PersonService.GetPersonById(user.Staff.Person.Id) : null;

                    if (person == null)
                    {
                        person = model.ToEntity(person);
                        SaveImages(person, model);
                        m_PersonService.InsertPerson(person);
                    }
                    else
                    {
                        person = model.ToEntity(person);
                        //SaveImages(person, model);
                        m_PersonService.UpdatePerson(person);
                    }

                    var staff = user.Staff != null ? m_StaffService.GetStaffById(user.Staff.Id) : null;
                    if (staff == null)
                    {
                        staff = model.ToEntity(staff);
                        staff.PersonId = person.Id;
                        m_StaffService.InsertStaff(staff);
                    }
                    else
                    {
                        staff = model.ToEntity(staff);
                        staff.PersonId = person.Id;
                        m_StaffService.UpdateStaff(staff);
                    }

                    var entity = model.ToEntity(user);
                    entity.StaffId = staff.Id;
                    entity.Modified = DateTime.Now;
                    m_SystemUserService.UpdateUser(entity);
                    
                    if (m_PermissionService.Authorize(StandardPermissionProvider.RolesManage))
                        m_SystemUserService.AssignRoles(entity, model.SelectedRoleIds);

                    transactionScope.Complete();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("", exception.Message);
            }

            PrepareSystemUserModel(model);
            return View(model);
        }

        [HttpGet]
        public ActionResult ViewUser(Guid id)
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.SystemUsersManage))
            {
                return AccessDeniedView();
            }

            var user = m_SystemUserService.GetUserById(id, false);
            if (user == null)
                return null;

            var model = user.ToModel();
            PrepareSystemUserModel(model);
            return View(model);
        }

        [HttpGet]
        public ActionResult SendWelcomeMsg()
        {
            m_SystemUserService.SendWelcomeMessages(m_notificationService, countryProg.Id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ViewProfile()
        {
            var user = m_SystemUserService.GetUserById(currentUser.Id, false);
            if (user == null)
                return null;

            var model = user.ToModel();
            PrepareSystemUserModel(model);
            return View("ViewProfile", model);
        }

        [HttpGet]
        public ActionResult EditProfile()
        {
            var user = m_SystemUserService.GetUserById(currentUser.Id, false);
            if (user == null)
                return RedirectToAction("ViewProfile");

            var model = user.ToModel();
            PrepareSystemUserModel(model);
            //set the confirm password value to the password value by default
            model.ConfirmPassword = model.Password;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditProfile(SystemUserModel model)
        {
            if (model.Id.IsEmpty())
                return RedirectToAction("ViewProfile");

            ValidateImageUploaded(model, false);

            var user = m_SystemUserService.GetUserById(model.Id, false);
            if (user == null)
                return RedirectToAction("ViewProfile");

            if (user.Staff != null && user.Staff.Person != null && user.Staff.Person.OfficialEmail != model.Email)
            {
                var emailUser = m_SystemUserService.FindUsers(new UserFilter { Email = model.Email }, 0, 1).FirstOrDefault();
                if (emailUser != null)
                {
                    ModelState.AddModelError("", Resources.PersonController_String_EmailAlreadyInUse.F(model.Email));
                    PrepareSystemUserModel(model);
                    return View(model);
                }
            }

            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    var person = user.Staff != null && user.Staff.Person != null
                        ? m_PersonService.GetPersonById(user.Staff.Person.Id) : null;

                    if (person == null)
                    {
                        return RedirectToAction("ViewProfile");
                    }
                    else
                    {
                        person = model.ToEntity(person);
                        SaveImages(person, model);
                        m_PersonService.UpdatePerson(person);
                    }

                    var staff = user.Staff != null ? m_StaffService.GetStaffById(user.Staff.Id) : null;
                    if (staff == null)
                    {
                        return RedirectToAction("ViewProfile");
                    }

                    //Update user
                    user.IsAvailable = model.Available;
                    user.Modified = DateTime.Now;
                    m_SystemUserService.UpdateUser(user);

                    //if (model.Password.IsNotNullOrWhiteSpace())
                    //{
                    //    m_SystemUserService.ChangePassword(new ChangePasswordRequest(model.Email, false, model.Password));
                    //}

                    transactionScope.Complete();

                    return RedirectToAction("ViewProfile");
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("", exception.Message);
            }

            PrepareSystemUserModel(model);
            return View(model);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassword model)
        {
            PasswordChangeResult result = m_SystemUserService.ChangePassword(new ChangePasswordRequest(currentStaff.Person.OfficialEmail, true, model.NewPassword, model.OldPassword));
            if (result.Errors.Count > 0)
            {
                ViewBag.StatusMsg = result.Errors[0];
                return View();
            }

            return _ViewProfile();
        }

        public ActionResult _ViewProfile()
        {
            var user = m_SystemUserService.GetUserById(currentUser.Id, false);
            if (user == null)
                return null;

            var mdl = user.ToModel();
            PrepareSystemUserModel(mdl);
            return View("_ViewProfile", mdl);
        }

        public ActionResult SwitchCountryProgramme()
        {
            if (!userContext.HasPermission(StandardPermissionProvider.MultipleCountryProgrammeAccess))
                return Content("<b><font color=Red>" + Resources.SystemUserController_String_ActionNotPermitted + "</font></b>", "text/html");
            SwitchCPModel model = new SwitchCPModel();
            model.CountryProgrammes = new SelectList(m_CountrySubOfficeService.CountryProgObj.GetCountryProgrammes(), "Id", "ProgrammeName", countryProg.Id);
            model.CountryProgrammeId = countryProg.Id;
            model.SubOffices = new SelectList(m_CountrySubOfficeService.GetCountrySubOffices(countryProg.Id), "Id", "Name", currentStaff.CountrySubOfficeId);
            model.SubOfficeId = currentStaff.CountrySubOfficeId.HasValue ? currentStaff.CountrySubOfficeId.Value : Guid.Empty;
            return View("SwitchCountryProgramme", model);
        }

        [HttpPost]
        public ActionResult SwitchCountryProgramme(SwitchCPModel model)
        {
            if (userContext.HasPermission(StandardPermissionProvider.MultipleCountryProgrammeAccess))
            {
                var staff = m_StaffService.GetStaffById(currentStaff.Id);
                staff.CountrySubOfficeId = model.SubOfficeId;
                m_StaffService.UpdateStaff(staff);
                //clear cache then update Current User in HttpContext           
                m_cacheService.Clear();
                userContext.CurrentUser = m_SystemUserService.GetUserByEmail(currentStaff.Person.OfficialEmail, false);
            }
            //redirect to Dashboard
            return Redirect("/Dashboard/");
        }

        public ActionResult GetCPSubOffices(Guid id)
        {
            StringBuilder selectHTML = new StringBuilder();
            selectHTML.Append("<select data-val=\"true\" data-val-required=\"" + Resources.Global_String_Required + "\" id=\"SubOfficeId\" name=\"SubOfficeId\">");
            selectHTML.Append("<option value=\"\">" + Resources.Global_String_PleaseSelect + "</option>");
            List<CountrySubOffice> subOffices = m_CountrySubOfficeService.GetCountrySubOffices(id);
            foreach (CountrySubOffice subOffice in subOffices)
                selectHTML.Append("<option value=\"" + subOffice.Id + "\">" + subOffice.Name + "</option>");
            selectHTML.Append("</select><br /><span class=\"field-validation-valid\" data-valmsg-for=\"SubOfficeId\" data-valmsg-replace=\"false\">" + Resources.Global_String_Required + "</span>");
            return Content(selectHTML.ToString(), "text/html");
        }

        private void SaveImages(Person person, SystemUserModel model)
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
                        ModelState.AddModelError("SignatureImageUpload", Resources.GoodsReceivedNoteController_String_FileUploadedNotValid);
                    }
                }

                model.SignatureImageUpload.InputStream.Seek(0, SeekOrigin.Begin);
            }

            return ModelState.IsValid;
        }

        private IQueryable<GridSystemUserModel> GetSystemUsersQuery(SCMSEntities context)
        {
            return from systemUser in context.SystemUsers
                        join staff in context.Staffs on systemUser.StaffId equals staff.Id into systemUserStafLeftJoin
                        from staff in systemUserStafLeftJoin.DefaultIfEmpty()
                        join person in context.People on staff.PersonId equals person.Id into staffPersonLeftJoin
                        from person in staffPersonLeftJoin.DefaultIfEmpty()
                        join financialLimit in context.FinanceLimits on staff.FinanceLimitId equals financialLimit.Id into staffFinancialLimitLeftJoin
                        from financialLimit in staffFinancialLimitLeftJoin.DefaultIfEmpty()
                        where systemUser.Staff.CountrySubOffice.CountryProgrammeId == countryProg.Id
                        select new GridSystemUserModel()
                                   {
                                       Id = systemUser.Id,
                                       Active = systemUser.Active,
                                       Locked = systemUser.Locked,
                                       FinancialLimitName = financialLimit.Name,
                                       FirstName = person.FirstName,
                                       OfficialEmail = person.OfficialEmail,
                                       OfficialPhone = person.OfficialPhone,
                                       OtherNames = person.OtherNames
                                   };

        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult SystemUserList(GridCommand command)
        {
            if (!m_PermissionService.Authorize(StandardPermissionProvider.SystemUsersManage))
                return AccessDeniedView();
            using (var context = SCMSEntities.Define())
            {

                var gridModel = GetSystemUsersQuery(context).ToGridModel(command.Page, command.PageSize, command.SortDescriptors, command.FilterDescriptors, command.GroupDescriptors);
                var data = gridModel.Data.Cast<GridSystemUserModel>().ToArray();
                data.ForEach(p=>
                                 {
                                     p.FinancialLimitName = p.FinancialLimitName ?? Resources.SystemUser_ViewProfile_NotSet;
                                     p.OfficialEmail = p.OfficialEmail ?? Resources.SystemUser_ViewProfile_NotSet;
                                     p.FirstName = p.FirstName ?? Resources.SystemUser_ViewProfile_NotSet;
                                     p.OtherNames = p.OtherNames ?? Resources.SystemUser_ViewProfile_NotSet;
                                 });
                gridModel.Data = data;
                
                return new JsonResult
                {
                    Data = gridModel,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

    }
}

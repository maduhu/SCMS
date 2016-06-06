using SCMS.Model;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using SCMS.CoreBusinessLogic.GeneralHelper;

namespace SCMS.CoreBusinessLogic.StaffServices
{
    public class StaffService : IStaffService
    {
        public void InsertStaff(Model.Staff staff)
        {
            using (var context = SCMSEntities.Define())
            {
                context.Staffs.Add(staff);
                context.SaveChanges();
            }
        }

        public void UpdateStaff(Model.Staff staff)
        {
            using (var context = SCMSEntities.Define())
            {
                context.Staffs.Attach(staff);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(staff, System.Data.EntityState.Modified);
                context.SaveChanges();
            }
        }

        public Model.Staff GetStaffById(Guid id)
        {
            using (var context = SCMSEntities.Define())
            {
                var staff = context.Staffs.Where(p => p.Id == id).FirstOrDefault();
                var person = staff.Person;
                var desg = staff.Designation;
                return staff;
            }
        }

        /// <summary>
        /// Returns StaffView object with SystemUser.Id as the Id field
        /// </summary>
        /// <param name="CountryProgId"></param>
        /// <returns></returns>
        public List<StaffView> GetStaffByCountryProgramme(Guid CountryProgId)
        {
            try
            {
                using (var context = SCMSEntities.Define())
                {
                    return (from sysUser in context.SystemUsers
                            where sysUser.Staff.CountrySubOffice.CountryProgrammeId == CountryProgId
                            orderby sysUser.Staff.Person.FirstName, sysUser.Staff.Person.OtherNames
                            select new StaffView
                            {
                                Id = sysUser.Id,
                                StaffId = (Guid)sysUser.StaffId,
                                StaffName = sysUser.Staff.Person.FirstName + " " + sysUser.Staff.Person.OtherNames,
                                StaffDesignation = sysUser.Staff.Designation.Name
                            }).ToList<StaffView>();
                }
            }
            catch (Exception ex)
            {
                return new List<StaffView>();
            }
        }

        public List<Model.Approver> GetCPGlobalApprovers(Guid countryProgId)
        {
            try
            {
                using (var context = SCMSEntities.Define())
                {
                    var approvers = context.Approvers.Where(a => a.CountryProgrammeId == countryProgId && (a.ProjectDonorId == null || a.ProjectDonorId == Guid.Empty)).OrderBy(a => a.ActivityCode).ToList();
                    foreach (var approver in approvers)
                    {
                        //drag more details into memory before passing list back
                        var desg = approver.SystemUser.Staff.Designation;
                        var person = approver.SystemUser.Staff.Person;
                        var financeLimit = approver.FinanceLimit;
                        desg = approver.SystemUser1.Staff.Designation;
                        person = approver.SystemUser1.Staff.Person;
                    }
                    return approvers;
                }
            }
            catch (Exception ex)
            {
                return new List<Approver>();
            }
        }

        public List<Model.Approver> GetProjectApprovers(Guid projectDonorId)
        {
            try
            {
                using (var context = SCMSEntities.Define())
                {
                    var approvers = context.Approvers.Where(a => a.ProjectDonorId == projectDonorId).OrderBy(a => a.ActivityCode).ToList();
                    foreach (var approver in approvers)
                    {
                        //drag more details into memory before passing list back
                        var desg = approver.SystemUser.Staff.Designation;
                        var person = approver.SystemUser.Staff.Person;
                        var financeLimit = approver.FinanceLimit;
                        desg = approver.SystemUser1.Staff.Designation;
                        person = approver.SystemUser1.Staff.Person;
                    }
                    return approvers;
                }
            }
            catch (Exception ex)
            {
                return new List<Approver>();
            }
        }

        public Approver GetApproverById(Guid id)
        {
            try
            {
                using (var context = SCMSEntities.Define())
                {
                    return context.Approvers.FirstOrDefault(a => a.Id == id);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void InsertApprover(Approver approver)
        {
            try
            {
                using (var context = SCMSEntities.Define())
                {
                    context.Approvers.Attach(approver);
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(approver, System.Data.EntityState.Added);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateApprover(Approver approver)
        {
            try
            {
                using (var context = SCMSEntities.Define())
                {
                    context.Approvers.Attach(approver);
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(approver, System.Data.EntityState.Modified);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DeleteApprover(Approver approver)
        {
            try
            {
                using (var context = SCMSEntities.Define())
                {
                    context.Approvers.Attach(approver);
                    ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(approver, System.Data.EntityState.Deleted);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Returns StaffView object with Staff.Id as the Id value
        /// </summary>
        /// <param name="activityCode"></param>
        /// <param name="actionType"></param>
        /// <param name="countryProgId"></param>
        /// <returns></returns>
        public List<StaffView> GetApproversByDocumentType(string activityCode, string actionType, Guid countryProgId)
        {
            try
            {
                using (var context = SCMSEntities.Define())
                {
                    return (from approver in context.Approvers
                            where approver.SystemUser.Staff.CountrySubOffice.CountryProgrammeId == countryProgId
                            && approver.ActivityCode == activityCode && approver.ActionType == actionType
                            select new StaffView
                            {
                                Id = approver.SystemUser.Staff.Id,
                                StaffName = approver.SystemUser.Staff.Person.FirstName + " " + approver.SystemUser.Staff.Person.OtherNames,
                                StaffDesignation = approver.SystemUser.Staff.Designation.Name
                            }).ToList<StaffView>();
                }
            }
            catch (Exception ex)
            {
                return new List<StaffView>();
            }
        }

        public List<StaffView> GetStaffByFinanceLimit(Guid FLId, Guid countryProgId)
        {
            try
            {
                using (var context = SCMSEntities.Define())
                {
                    var finlimit = context.FinanceLimits.FirstOrDefault(p => p.Id == FLId);
                    if (finlimit.Limit > 0)
                    {
                        var staffList = (from sysUser in context.SystemUsers
                                         where sysUser.Staff.FinanceLimit.Limit >= finlimit.Limit &&
                                         sysUser.Staff.CountrySubOffice.CountryProgrammeId == countryProgId
                                         orderby sysUser.Staff.FinanceLimit.Limit, sysUser.Staff.Person.FirstName, sysUser.Staff.Person.OtherNames
                                         select new StaffView
                                         {
                                             Id = sysUser.Id,
                                             StaffId = (Guid)sysUser.StaffId,
                                             StaffName = sysUser.Staff.Person.FirstName + " " + sysUser.Staff.Person.OtherNames,
                                             StaffDesignation = sysUser.Staff.Designation.Name
                                         }).ToList<StaffView>();
                        var unlimitedStaff = (from sysUser in context.SystemUsers
                                              where sysUser.Staff.FinanceLimit.Limit == 0 &&
                                              sysUser.Staff.CountrySubOffice.CountryProgrammeId == countryProgId
                                              orderby sysUser.Staff.FinanceLimit.Limit
                                              select new StaffView
                                              {
                                                  Id = sysUser.Id,
                                                  StaffId = (Guid)sysUser.StaffId,
                                                  StaffName = sysUser.Staff.Person.FirstName + " " + sysUser.Staff.Person.OtherNames,
                                                  StaffDesignation = sysUser.Staff.Designation.Name
                                              }).ToList<StaffView>();
                        foreach (var staff in unlimitedStaff)
                        {
                            staffList.Add(staff);
                        }
                        return staffList;
                    }
                    else
                    {
                        return (from sysUser in context.SystemUsers
                                where sysUser.Staff.FinanceLimit.Limit == finlimit.Limit &&
                                sysUser.Staff.CountrySubOffice.CountryProgrammeId == countryProgId
                                orderby sysUser.Staff.FinanceLimit.Limit, sysUser.Staff.Person.FirstName, sysUser.Staff.Person.OtherNames
                                select new StaffView
                                {
                                    Id = sysUser.Id,
                                    StaffId = (Guid)sysUser.StaffId,
                                    StaffName = sysUser.Staff.Person.FirstName + " " + sysUser.Staff.Person.OtherNames,
                                    StaffDesignation = sysUser.Staff.Designation.Name
                                }).ToList<StaffView>();
                    }
                }
            }
            catch (Exception ex)
            {
                return new List<StaffView>();
            }
        }

        /// <summary>
        /// Returns StaffView list with Staff.Id as Id
        /// </summary>
        /// <param name="documentCode"></param>
        /// <param name="countryProgId"></param>
        /// <returns></returns>
        public List<Staff> GetStaffByApprovalDoc(string documentCode, Guid countryProgId)
        {
            using (var context = SCMSEntities.Define())
            {
                var staffList = context.Staffs.Include("Person").Where(s => (s.SystemUsers.FirstOrDefault(u => u.StaffId == s.Id && u.IsAvailable == true).Approvers.Where(a => a.ActivityCode == documentCode).Count() > 0
                    || s.SystemUsers.FirstOrDefault(u => u.StaffId == s.Id && u.IsAvailable == true).Approvers1.Where(a => a.ActivityCode == documentCode).Count() > 0)
                    && s.CountrySubOffice.CountryProgrammeId == countryProgId).ToList();
                return staffList;
            }
        }

        public List<DocumentPreparer> GetProjectDocPrepares(Guid projectDonorId)
        {
            using (var context = SCMSEntities.Define())
            {
                var prepList = context.DocumentPreparers.Where(d => d.ProjectDonorId == projectDonorId).ToList();
                foreach (var prep in prepList)
                {
                    var person = prep.Staff.Person;
                    var desg = prep.Staff.Designation;
                }
                return prepList;
            }
        }

        public void InsertDocPreparer(DocumentPreparer docPreparer)
        {
            using (var context = SCMSEntities.Define())
            {
                var dp = context.DocumentPreparers.FirstOrDefault(d => d.DocumentCode == docPreparer.DocumentCode && d.PreparerId == docPreparer.PreparerId && d.ProjectDonorId == docPreparer.ProjectDonorId);
                if (dp == null)
                {
                    docPreparer.Id = Guid.NewGuid();
                    context.DocumentPreparers.Add(docPreparer);
                    context.SaveChanges();
                }
            }
        }

        public void DeleteDocPreparerById(Guid id)
        {
            using (var context = SCMSEntities.Define())
            {
                var dp = context.DocumentPreparers.FirstOrDefault(d => d.Id == id);
                context.DocumentPreparers.Remove(dp);
                context.SaveChanges();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.Security
{
    public interface IPermissionService
    {
        void DeletePermissionRecord(PermissionRecord permission);
        void DeleteRolePermissionRecord(RolePermissionRecord rolePermission);
        PermissionRecord GetPermissionRecordById(Guid permissionId);
        PermissionRecord GetPermissionRecordBySystemName(string systemName, bool userCache=false);
        IList<PermissionRecord> GetAllPermissionRecords();
        void InsertRolePermissionRecord(RolePermissionRecord rolePermissionRecord);
        void InsertPermissionRecord(PermissionRecord permission);
        void UpdatePermissionRecord(PermissionRecord permission);
        void InstallPermissions(IPermissionProvider permissionProvider);
        void UninstallPermissions(IPermissionProvider permissionProvider);
        bool Authorize(string permissionRecordSystemName);
        bool Authorize(PermissionRecord permission);
    }
}

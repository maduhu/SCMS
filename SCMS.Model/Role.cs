//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SCMS.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Role
    {
        public Role()
        {
            this.RolePermissionRecords = new HashSet<RolePermissionRecord>();
            this.SystemUserRoles = new HashSet<SystemUserRole>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool IsSystemRole { get; set; }
        public string SystemName { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Modified { get; set; }
        public string Description { get; set; }
        public System.Guid CountryProgrammeId { get; set; }
    
        public virtual ICollection<RolePermissionRecord> RolePermissionRecords { get; set; }
        public virtual ICollection<SystemUserRole> SystemUserRoles { get; set; }
    }
}

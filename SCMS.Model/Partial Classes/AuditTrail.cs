using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.Data.Entity.Infrastructure;

namespace SCMS.Model
{
    class AuditTrail { }

    public partial class SCMSEntities
    {
        public string UserName { get; set; }
        //List<DocumentAudit> auditTrailList = new List<DocumentAudit>();
        //List<DBAudit> auditTrailList = new List<DBAudit>();

        public enum AuditActions
        {
            I,
            U,
            D
        }

        //partial void OnContextCreated()
        //{
        //    ((IObjectContextAdapter)this).ObjectContext.SavingChanges += new EventHandler(SCMSEntities_SavingChanges);
        //}

        void SCMSEntities_SavingChanges(object sender, EventArgs e)
        {
            IEnumerable<ObjectStateEntry> changes =((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified);
            foreach (ObjectStateEntry stateEntryEntity in changes)
            {
                if (!this.EntityIsAuditable((EntityObject)stateEntryEntity.Entity))
                    continue;
                if (stateEntryEntity.State == EntityState.Modified | stateEntryEntity.State == EntityState.Deleted)
                {
                    Guid auditId = new Guid(GetEntryValueInString(stateEntryEntity, stateEntryEntity.State == EntityState.Modified ? false : true, "Id"));
                    if (this.DocumentAudits.FirstOrDefault(p => p.DocumentId == auditId) == null)
                        continue;
                }
                if (!stateEntryEntity.IsRelationship &&
                        stateEntryEntity.Entity != null &&
                            !(stateEntryEntity.Entity is DocumentAudit))
                {//is a normal entry, not a relationship
                    DocumentAudit audit = this.AuditTrailFactory(stateEntryEntity, UserName);
                    //DBAudit audit = this.AuditTrailFactory(stateEntryEntity, UserName);
                    switch (stateEntryEntity.State)
                    {
                        case EntityState.Added:
                            //auditTrailList.Add(audit);
                            this.DocumentAudits.Add(audit);
                            break;
                        case EntityState.Deleted:
                            this.DocumentAudits.Remove(audit);
                            break;
                        case EntityState.Detached:
                            break;
                        case EntityState.Modified:
                            ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.ChangeObjectState(audit, System.Data.EntityState.Modified);
                            break;
                        case EntityState.Unchanged:
                            break;
                        default:
                            break;
                    }
                }
            }

            //if (auditTrailList.Count > 0)
            //{
            //    foreach (var audit in auditTrailList)
            //    {//add all audits 
            //        this.AddToDocumentAudits(audit);
            //        //this.AddToDBAudits(audit);
            //    }
            //}
        }


        private DocumentAudit AuditTrailFactory(ObjectStateEntry entry, string UserName)
        {
            DocumentAudit auditEntity;
            string countprog, authorizedBy, authorizedOn, IsApproved, IsVerified,
                AprovedOn, IsAuthorized, IsRejected, IsReviewed, IsSubmited,
                PreparedBy, PreparedOn, ReviewedBy, ReviewedOn, VerifiedBy, ApprovedBy, ApprovedOn;

            //DBAudit audit = new DBAudit();
            if (entry.State == EntityState.Added)
            {//entry is Added 
                GetProperyValue(entry, out countprog, out authorizedBy, out authorizedOn, out IsApproved,
                    out IsVerified, out AprovedOn, out IsAuthorized, out IsRejected, out IsReviewed, out IsSubmited,
                    out PreparedBy, out PreparedOn, out ReviewedBy, out ReviewedOn, out ApprovedBy, out ApprovedOn, out VerifiedBy);

                auditEntity = new DocumentAudit();
                auditEntity.CountryProgrammeId = countprog.Equals(string.Empty) ? Guid.Empty : new Guid(countprog);
                auditEntity.Id = Guid.NewGuid();
                auditEntity.IssueDate = DateTime.Now;
                auditEntity.DocumentId = new Guid(GetEntryValueInString(entry, false, "Id"));
                auditEntity.PreparedBy = PreparedBy == string.Empty ? (Guid?)null : new Guid(PreparedBy);
                auditEntity.ApprovedBy = ApprovedBy == string.Empty ? (Guid?)null : new Guid(ApprovedBy);
                auditEntity.PreparedOn = PreparedOn == string.Empty ? (DateTime?)null : DateTime.Parse(PreparedOn);
                auditEntity.DocumentType = entry.EntitySet.Name;
                auditEntity.AuthorizedBy = authorizedBy == string.Empty ? (Guid?)null : new Guid(authorizedBy);
                auditEntity.AuthorizedOn = authorizedOn == string.Empty ? (DateTime?)null : DateTime.Parse(authorizedOn);
                auditEntity.IsApproved = IsApproved == string.Empty ? (bool?)null : bool.Parse(IsApproved);
                auditEntity.IsAprovedOn = AprovedOn == string.Empty ? (DateTime?)null : DateTime.Parse(AprovedOn);
                auditEntity.IsAuthorized = IsAuthorized == string.Empty ? (bool?)null : bool.Parse(IsAuthorized);
                auditEntity.IsRejected = IsRejected == string.Empty ? (bool?)null : bool.Parse(IsRejected);
                auditEntity.IsReviewed = IsReviewed == string.Empty ? (bool?)null : bool.Parse(IsReviewed);
                auditEntity.IsSubmitted = IsSubmited == string.Empty ? (bool?)null : bool.Parse(IsSubmited);
                auditEntity.ReviewedBy = ReviewedBy == string.Empty ? (Guid?)null : new Guid(ReviewedBy);
                auditEntity.ReviewedOn = ReviewedOn == string.Empty ? (DateTime?)null : DateTime.Parse(ReviewedOn);
                auditEntity.IsVerified = IsVerified == string.Empty ? (bool?)null : bool.Parse(IsVerified);
                auditEntity.VerifiedBy = VerifiedBy == string.Empty ? (Guid?)null : new Guid(VerifiedBy);

            }
            else if (entry.State == EntityState.Deleted)
            {//entry in deleted
                auditEntity = new DocumentAudit();
                Guid auditId = new Guid(GetEntryValueInString(entry, true, "Id"));
                auditEntity = this.DocumentAudits.FirstOrDefault(p => p.DocumentId == auditId);
            }
            else
            {//entry is modified
                auditEntity = new DocumentAudit();
                Guid auditId = new Guid(GetEntryValueInString(entry, false, "Id"));
                auditEntity = this.DocumentAudits.FirstOrDefault(p => p.DocumentId == auditId);
                GetEntryValueInString(entry, false, ref auditEntity);
            }

            return auditEntity;// audit;
        }

        private void GetProperyValue(ObjectStateEntry entry, out string countprog, out string authorizedBy, out string authorizedOn,
            out string IsApproved, out string IsVerified, out string AprovedOn, out string IsAuthorized, out string IsRejected,
            out string IsReviewed, out string IsSubmited, out string PreparedBy, out string PreparedOn, out string ReviewedBy,
            out string ReviewedOn, out string ApprovedBy, out string ApprovedOn, out string VerifiedBy)
        {
            countprog = GetEntryValueInString(entry, false, "CountryProgrammeId");
            authorizedBy = GetEntryValueInString(entry, false, "AuthorizedBy");
            authorizedOn = GetEntryValueInString(entry, false, "AuthorizedOn");
            IsApproved = GetEntryValueInString(entry, false, "IsApproved");
            AprovedOn = GetEntryValueInString(entry, false, "ApprovedOn");
            IsAuthorized = GetEntryValueInString(entry, false, "IsAuthorized");
            IsRejected = GetEntryValueInString(entry, false, "IsRejected");
            IsReviewed = GetEntryValueInString(entry, false, "IsReviewed");
            IsSubmited = GetEntryValueInString(entry, false, "IsSubmitted");
            IsVerified = GetEntryValueInString(entry, false, "Verified");
            PreparedBy = GetEntryValueInString(entry, false, "PreparedBy");
            PreparedOn = GetEntryValueInString(entry, false, "PreparedOn");
            ReviewedBy = GetEntryValueInString(entry, false, "ReviewedBy");
            ReviewedOn = GetEntryValueInString(entry, false, "ReviewedOn");
            VerifiedBy = GetEntryValueInString(entry, false, "ReceptionApprovedBy");
            ApprovedBy = GetEntryValueInString(entry, false, "ApprovedBy");
            ApprovedOn = GetEntryValueInString(entry, false, "ApprovedOn");
        }

        private void GetEntryValueInString(ObjectStateEntry entry, bool isOrginal, ref DocumentAudit auditEntity)
        {
            PropertyInfo[] entityProperties = auditEntity.GetType().GetProperties().Where(p => p.PropertyType != typeof(EntityCollection<>)
                && p.PropertyType != typeof(EntityState)
                && p.PropertyType != typeof(EntityKey)
                && p.PropertyType != typeof(EntityObject)
                && p.PropertyType != typeof(EntityReference)
                && p.PropertyType.FullName != "System.Object").ToArray();

            foreach (string propName in entry.GetModifiedProperties())
            {
                if (propName == "Id") continue;

                // object setterValue = null;
                var prop = entityProperties.FirstOrDefault(p => p.Name.ToLower() == propName.ToLower());
                if (prop != null)
                {
                    var currentValue = entry.CurrentValues[propName];
                    var orignalValue = auditEntity.GetType().GetProperty(propName).GetValue(auditEntity, null);//.OriginalValues[propName];
                    if (!object.ReferenceEquals(currentValue, orignalValue))//currentValue.Equals(orignalValue))
                    {
                        if (currentValue == DBNull.Value) currentValue = null;
                        PropertyInfo propInfo = auditEntity.GetType().GetProperty(propName);
                        propInfo.SetValue(auditEntity, currentValue, null);
                    }
                }
            }//end foreach
        }

        private string GetEntryValueInString(ObjectStateEntry entry, bool isOrginal, string PropName)
        {
            PropertyInfo[] entityProperties = entry.Entity.GetType().GetProperties().Where(p => p.PropertyType != typeof(EntityCollection<>)
                && p.PropertyType != typeof(EntityState)
                && p.PropertyType != typeof(EntityKey)
                && p.PropertyType != typeof(EntityObject)
                && p.PropertyType != typeof(EntityReference)
                && p.PropertyType.FullName != "System.Object").ToArray();

            var prop = entityProperties.FirstOrDefault(p => p.Name.ToLower() == PropName.ToLower());
            if (isOrginal)
                return prop != null ? entry.OriginalValues[PropName].ToString() : string.Empty;
            else return prop != null ? entry.CurrentValues[PropName].ToString() : string.Empty;
        }

        private string GetEntryValueInString(ObjectStateEntry entry, bool isOrginal)
        {
            if (entry.Entity is EntityObject)
            {
                object target = CloneEntity((EntityObject)entry.Entity);

                foreach (string propName in entry.GetModifiedProperties())
                {
                    object setterValue = null;
                    if (isOrginal)
                    {
                        //Get orginal value 
                        setterValue = entry.OriginalValues[propName];
                    }
                    else
                    {
                        //Get orginal value 
                        setterValue = entry.CurrentValues[propName];
                    }
                    //Find property to update 
                    PropertyInfo propInfo = target.GetType().GetProperty(propName);

                    //update property with orgibal value 
                    if (setterValue == DBNull.Value)
                    {//
                        setterValue = null;
                    }
                    propInfo.SetValue(target, setterValue, null);
                }//end foreach

                XmlSerializer formatter = new XmlSerializer(target.GetType());
                XDocument document = new XDocument();

                using (XmlWriter xmlWriter = document.CreateWriter())
                {
                    formatter.Serialize(xmlWriter, target);
                }
                return document.Root.ToString();
            }
            return null;
        }

        public EntityObject CloneEntity(EntityObject obj)
        {
            DataContractSerializer dcSer = new DataContractSerializer(obj.GetType());
            MemoryStream memoryStream = new MemoryStream();

            dcSer.WriteObject(memoryStream, obj);
            memoryStream.Position = 0;

            EntityObject newObject = (EntityObject)dcSer.ReadObject(memoryStream);
            return newObject;
        }

        /// <summary>
        /// Determine if an entity is in the list of auditable ones.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private Boolean EntityIsAuditable(EntityObject entity)
        {
            if (
                (entity is OrderRequest)
                || (entity is PurchaseOrder)
                || (entity is CompletionCertificate)
                || (entity is GoodsReceivedNote)
                || (entity is WayBill)
                || (entity is WarehouseRelease)
                || (entity is ProcurementPlan)
                || (entity is PaymentRequest)
                || (entity is PaymentVoucher)
               )
            {
                return true;
            }

            return false;
        }


        //http://www.codeproject.com/Articles/34491/Implementing-Audit-Trail-using-Entity-Framework-Pa
        //http://www.codeproject.com/Articles/34627/Implementing-Audit-Trail-using-Entity-Framework-Pa
    }
}


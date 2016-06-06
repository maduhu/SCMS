using System;
using SCMS.Model;
using System.Linq;
using System.Data.Entity.Infrastructure;

namespace SCMS.CoreBusinessLogic.People
{
    public class PersonService : IPersonService
    {
        #region Implementation of IPersonService

        public void InsertPerson(Model.Person person)
        {
            using (var context = SCMSEntities.Define())
            {
                context.People.Add(person);
                context.SaveChanges();
            }
        }

        public void UpdatePerson(Model.Person person)
        {
            using (var context = SCMSEntities.Define())
            {
                context.People.Attach(person);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(person, System.Data.EntityState.Modified);
                context.SaveChanges();
            }
        }

        public Model.Person GetPersonById(Guid id)
        {
            using (var context = SCMSEntities.Define())
            {
                return context.People.Where(p => p.Id == id).FirstOrDefault();
            }
        }
    

        #endregion
    }
}

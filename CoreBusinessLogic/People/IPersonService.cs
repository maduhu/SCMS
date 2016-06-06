using System;

namespace SCMS.CoreBusinessLogic.People
{
    public interface IPersonService
    {
        void InsertPerson(Model.Person person);
        void UpdatePerson(Model.Person person);
        Model.Person GetPersonById(Guid id);
    }
}

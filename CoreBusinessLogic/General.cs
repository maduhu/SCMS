using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic
{
    public class GeneralService : IGeneralService
    {
        public class StaffView
        {
            /// <summary>
            /// Staff entity
            /// </summary>
            public Staff staff;

            /// <summary>
            /// Person entity
            /// </summary>
            public Person person;

            /// <summary>
            /// Designation entity
            /// </summary>
            public Designation designation;

            /// <summary>
            /// CountrySubOffice entity
            /// </summary>
            public CountrySubOffice countrySubOffice;

            public string Name
            {
                get { return staff.Person.IDNo + ": " + staff.Person.FirstName + " " + staff.Person.OtherNames; }
            }

            public Guid Id
            {
                get
                {
                    return staff.Id;
                }

                set
                {
                    staff.Id = value;
                }
            }
        }

        public class OrderRequestItemView
        {
            public OrderRequestItem orderRequestItem;
            public Item item;
            public UnitOfMeasure unitOfMeasure;
            public Model.OrderRequest orderRequest;
        }

        public List<Model.PurchaseOrder> GetPurchaseOrders(Guid cpId, string search = null)
        {
            using (var dbContext = new SCMSEntities())
            {
                var purschaseOList = from pOrder in dbContext.PurchaseOrders
                                     where pOrder.CountryProgrammeId == cpId
                                     select pOrder;
                return purschaseOList.ToList<Model.PurchaseOrder>();
            }
        }

        public List<Project> GetProjects(Guid cpId, string search = null)
        {
            using (var dbContext = new SCMSEntities())
            {
                var purschaseOList = from proj in dbContext.Projects
                                     where proj.CountryProgrammeId == cpId
                                     select proj;
                return purschaseOList.ToList<Project>();
            }
        }


        public List<StaffView> GetStaff(Guid cpId, string search = null)
        {
            using (var dbContext = new SCMSEntities())
            {
                var staffList = from staff in dbContext.Staffs
                                where staff.CountrySubOffice.CountryProgrammeId==cpId
                                select staff;
                return (from staff in staffList
                        select new StaffView
                        {
                            staff = staff,
                            person = staff.Person,
                            designation = staff.Designation,
                            countrySubOffice = staff.CountrySubOffice
                        }).ToList<StaffView>();
            }
        }

        public List<Model.OrderRequest> GetOrderRequests(Guid cpId, string search = null)
        {
            using (var dbContext = new SCMSEntities())
            {
                var orderRequestList = from orderR in dbContext.OrderRequests
                                       where orderR.CountryProgrammeId == cpId
                                       select orderR;
                return orderRequestList.ToList<Model.OrderRequest>();
            }
        }

        public List<Person> GetPersons(Guid cpId, string search = null)
        {
            using (var dbContext = new SCMSEntities())
            {
                var personList = from proj in dbContext.People
                                 where proj.CountryProgrammeId == cpId
                                 select proj;
                return personList.ToList<Person>();
            }
        }

        public List<OrderRequestItemView> GetOrderRequestItems(Guid orderRequestId)
        {
            using (var dbContext = new SCMSEntities())
            {
                var oRList = (from orderRqti in dbContext.OrderRequestItems
                              where orderRqti.OrderRequestId == orderRequestId
                              select new OrderRequestItemView
                              {
                                  orderRequestItem = orderRqti,
                                  orderRequest = orderRqti.OrderRequest,
                                  item = orderRqti.Item,
                                  unitOfMeasure = orderRqti.Item.UnitOfMeasure
                              }).ToList();
                return oRList;
            }
        }

        public enum CodeType : int
        {
            GoodsReceivedNote = 1
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GenerateUniquNumber(CodeType type)
        {
            long count = 1;
            string code = string.Empty; // shd be generate not hard coded as in the switch loop
            using (var dbContext = new SCMSEntities())
            {
                switch (type)
                {
                    case CodeType.GoodsReceivedNote:
                    default:
                        code = "GRN/DRC/UG/";
                        GoodsReceivedNote grn = dbContext.GoodsReceivedNotes.OrderByDescending(x => x.Index).FirstOrDefault();
                        if (grn != null) { count = grn.Index + 1; }
                        break;
                }

                if (count < 10000)
                {
                    if (count < 10)
                        return code + "0000" + count;
                    if (count < 100 & count > 10)
                        return code + "000" + count;
                    if (count < 1000 & count > 100)
                        return code + "00" + count;
                    if (count < 10000 & count > 1000)
                        return code + "0" + count;
                }
                return code + count;
            }
        }
    }
}

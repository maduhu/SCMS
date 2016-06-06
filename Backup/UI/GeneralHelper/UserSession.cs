using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SCMS.Model;

namespace SCMS.UI.GeneralHelper
{
    public class UserSession
    {
        /// <summary>
        /// Holds global property that can be accessed in the whole application
        /// </summary>
        private UserSession() { }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        public static UserSession CurrentSession
        {
            get
            {
                UserSession session = (UserSession)HttpContext.Current.Session["__MySession__"];
                if (session == null)
                {
                    session = new UserSession();
                    HttpContext.Current.Session["__MySession__"] = session;
                }
                return session;
            }
        }

        /// <summary>
        /// Propert that gets and sets Tender Analysis Selected items
        /// </summary>
        public Model.OrderRequest NewOR { get; set; }

        public Model.WarehouseRelease NewWRN { get; set; }

        public Models.Request4Payment R4Pmodel { get; set; }

        public List<Model.Project> ProjectList { get; set; }

        public List<Model.Supplier> SupplierList { get; set; }

        public List<Model.OrderRequest> OrderRequestList { get; set; }

        public List<Model.Asset> AssetList { get; set; }

        public List<Model.Location> LocationList { get; set; }

        public List<Model.Item> ItemList { get; set; }

        public List<Approver> ApproverList { get; set; }

        public Guid ProjectDonorId { get; set; }

        public Model.SystemUser SystemUser { get; set; }

        public byte[] UploadedFile { get; set; }

        public UploadedDoc UploadedDocDetails { get; set; }

        public IEnumerable<Guid> ServiceItems { get; set; }

        public Model.Bin NewBin { get; set; }

    }

    public class UploadedDoc
    {
        public byte[] FileContent { get; set; }
        public int ContentLength { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
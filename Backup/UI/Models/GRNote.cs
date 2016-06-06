using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Data.Objects.DataClasses;
using SCMS.CoreBusinessLogic._GoodsReceivedNote;
using System.ComponentModel.DataAnnotations;

namespace SCMS.UI.Models
{
    
    ////public class GRNoteItem
    ////{
    ////    public Model.GoodsReceivedNoteItem EntityGRNItem { get; set; }

    ////    public SelectList POItems { get; set; }
    ////}

    public class GRNDetailsParams
    {
        public bool Verify { get; set; }
        public Guid GRNId { get; set; }
    }

    //public class RegisteredAsset
    //{
    //    public List<Model.GoodsReceivedNoteItem> EntityAsset { get; set; }
    //}
}
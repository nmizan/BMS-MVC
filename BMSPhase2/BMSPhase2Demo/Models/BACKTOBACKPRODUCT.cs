//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BMSPhase2Demo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class BACKTOBACKPRODUCT
    {
        public BACKTOBACKPRODUCT()
        {
            this.RAWMATERIALs = new List<RAWMATERIAL>();
        }
    
        public short ID { get; set; }

        [Display(Name = "Product")]
        public string NAME { get; set; }

        [Display(Name = "Size & Details")]
        public string SIZEANDDETAIL { get; set; }

        [Display(Name = "Quantity")]
        public Nullable<decimal> QUANTITY { get; set; }

        [Display(Name = "Unit")]
        public string QUANTITYUNIT { get; set; }
        public short BACKTOBACKLCID { get; set; }
        public string CREATEDBY { get; set; }
        public Nullable<System.DateTime> CREATEDDATE { get; set; }
        public string MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFIEDDATE { get; set; }
    
        public virtual BACKTOBACKLC BACKTOBACKLC { get; set; }
        public virtual IList<RAWMATERIAL> RAWMATERIALs { get; set; }
    }
}

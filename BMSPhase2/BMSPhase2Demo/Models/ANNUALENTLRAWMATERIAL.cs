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
    
    public partial class ANNUALENTLRAWMATERIAL
    {
        public short AESLNO { get; set; }
        public int BONDERSLNO { get; set; }
        public short MSLNO { get; set; }
        public Nullable<short> MUSLNO { get; set; }
        public string INPUTBY { get; set; }
        public Nullable<System.DateTime> INPUTDATE { get; set; }
        public string MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFYDATE { get; set; }
        [Display(Name = "Quantity")]
        public Nullable<decimal> AEQTY { get; set; }
        public string AIL_HSCODE { get; set; }
        public string SPEC_GRADE { get; set; }
    
        public virtual MATERIAL MATERIAL { get; set; }
        public virtual MEASUREMENTUNIT MEASUREMENTUNIT { get; set; }
        [Display(Name = "Date From")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> ENTITLEFROM { get; set; }
        [Display(Name = "Date To")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> ENTITLETO { get; set; }
                                          
    }
}
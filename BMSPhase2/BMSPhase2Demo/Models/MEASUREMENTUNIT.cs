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
    
    public partial class MEASUREMENTUNIT
    {
        public MEASUREMENTUNIT()
        {
            this.ANNUALENTLRAWMATERIALs = new HashSet<ANNUALENTLRAWMATERIAL>();
            this.BONDERANNUALENTITLEMENTs = new HashSet<BONDERANNUALENTITLEMENT>();
            this.BONDERANNUALENTITLEMENTs1 = new HashSet<BONDERANNUALENTITLEMENT>();
            this.BONDERANNUALENTITLEMENTs2 = new HashSet<BONDERANNUALENTITLEMENT>();
            this.BONDERRAWMATERIALs = new HashSet<BONDERRAWMATERIAL>();
            this.MATERIALS = new HashSet<MATERIAL>();
            this.MEASUREMENTUNITCONVERSIONs = new HashSet<MEASUREMENTUNITCONVERSION>();
            this.PRODCAPACITYMACHINEWISEMs = new HashSet<PRODCAPACITYMACHINEWISEM>();
            this.MEASUREMENTUNITCONVERSIONs1 = new HashSet<MEASUREMENTUNITCONVERSION>();
        }
    
        public short MUSLNO { get; set; }
        [Display(Name = "Unit")]
        [Required]
        public string MUNAME { get; set; }
        public string MUNAMEBG { get; set; }
        public string INPUTBY { get; set; }
        public Nullable<System.DateTime> INPUTDATE { get; set; }
        public string MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFYDATE { get; set; }
    
        public virtual ICollection<ANNUALENTLRAWMATERIAL> ANNUALENTLRAWMATERIALs { get; set; }
        public virtual ICollection<BONDERANNUALENTITLEMENT> BONDERANNUALENTITLEMENTs { get; set; }
        public virtual ICollection<BONDERANNUALENTITLEMENT> BONDERANNUALENTITLEMENTs1 { get; set; }
        public virtual ICollection<BONDERANNUALENTITLEMENT> BONDERANNUALENTITLEMENTs2 { get; set; }
        public virtual ICollection<BONDERRAWMATERIAL> BONDERRAWMATERIALs { get; set; }
        public virtual ICollection<MATERIAL> MATERIALS { get; set; }
        public virtual ICollection<MEASUREMENTUNITCONVERSION> MEASUREMENTUNITCONVERSIONs { get; set; }
        public virtual ICollection<PRODCAPACITYMACHINEWISEM> PRODCAPACITYMACHINEWISEMs { get; set; }
        public virtual ICollection<MEASUREMENTUNITCONVERSION> MEASUREMENTUNITCONVERSIONs1 { get; set; }
    }
}
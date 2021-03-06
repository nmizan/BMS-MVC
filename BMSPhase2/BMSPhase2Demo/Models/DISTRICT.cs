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
    
    public partial class DISTRICT
    {
        public DISTRICT()
        {
            this.BONDERs = new HashSet<BONDER>();
            this.BONDERs1 = new HashSet<BONDER>();
            this.UPAZILAs = new HashSet<UPAZILA>();
            this.OFFICEs = new HashSet<OFFICE>();
            this.OWNERINFOes = new HashSet<OWNERINFO>();
        }
    
        public short DISTRICTSLNO { get; set; }
        public Nullable<short> COUNTRYSLNO { get; set; }
        [Required]
        public string DISTRICTNAME { get; set; }
        public string DISTRICTNAMEBG { get; set; }
        public string INPUTBY { get; set; }
        public Nullable<System.DateTime> INPUTDATE { get; set; }
        public string MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFYDATE { get; set; }
    
        public virtual ICollection<BONDER> BONDERs { get; set; }
        public virtual ICollection<BONDER> BONDERs1 { get; set; }
        public virtual COUNTRY COUNTRY { get; set; }
        public virtual ICollection<UPAZILA> UPAZILAs { get; set; }
        public virtual ICollection<OFFICE> OFFICEs { get; set; }
        public virtual ICollection<OWNERINFO> OWNERINFOes { get; set; }
    }
}

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
    
    public partial class MATERIAL
    {
        public MATERIAL()
        {
            this.ANNUALENTLRAWMATERIALs = new HashSet<ANNUALENTLRAWMATERIAL>();
            this.BONDERANNUALENTITLEMENTCOEDETs = new HashSet<BONDERANNUALENTITLEMENTCOEDET>();
            this.BONDERANNUALENTITLEMENTCOEDETs1 = new HashSet<BONDERANNUALENTITLEMENTCOEDET>();
            this.BONDERANNUALENTITLEMENTCOMs = new HashSet<BONDERANNUALENTITLEMENTCOM>();
            this.BONDERPRODUCTs = new HashSet<BONDERPRODUCT>();
            this.INBONDRAWMATERIALs = new HashSet<INBONDRAWMATERIAL>();
            this.RAWMATERIALs = new HashSet<RAWMATERIAL>();
        }
    
        public short MSLNO { get; set; }
        public Nullable<short> MGSLNO { get; set; }
         [Display(Name = "HS Code")]
        public string MHSCODE { get; set; }
        public string MHSCODEBG { get; set; }
        [Display(Name = "Description")]
        public string MDESCRIPTION { get; set; }
        public string MDESCRIPTIONBG { get; set; }
         [Display(Name = "Sp Grade")]
        public string SPGRADE { get; set; }
        public string SPGRADEBG { get; set; }
        public Nullable<short> MUSLNO { get; set; }
        public string MTYPE { get; set; }
        public string REMARKS { get; set; }
        public string INPUTBY { get; set; }
        public Nullable<System.DateTime> INPUTDATE { get; set; }
        public string MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFYDATE { get; set; }
         [Display(Name = "Product Name")]
         [Required]
        public string MATERIALNAME { get; set; }
    
        public virtual ICollection<ANNUALENTLRAWMATERIAL> ANNUALENTLRAWMATERIALs { get; set; }
        public virtual ICollection<BONDERANNUALENTITLEMENTCOEDET> BONDERANNUALENTITLEMENTCOEDETs { get; set; }
        public virtual ICollection<BONDERANNUALENTITLEMENTCOEDET> BONDERANNUALENTITLEMENTCOEDETs1 { get; set; }
        public virtual ICollection<BONDERANNUALENTITLEMENTCOM> BONDERANNUALENTITLEMENTCOMs { get; set; }
        public virtual ICollection<BONDERPRODUCT> BONDERPRODUCTs { get; set; }
        public virtual ICollection<INBONDRAWMATERIAL> INBONDRAWMATERIALs { get; set; }
        public virtual ICollection<RAWMATERIAL> RAWMATERIALs { get; set; }
        public virtual MATERIALSGROUP MATERIALSGROUP { get; set; }
        public virtual MEASUREMENTUNIT MEASUREMENTUNIT { get; set; }
    }
}
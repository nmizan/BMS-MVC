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
    
    public partial class BONDDIVISION
    {
        public BONDDIVISION()
        {
            this.BONDCIRCLEs = new HashSet<BONDCIRCLE>();
            this .BONDAREAs = new HashSet <BONDAREA >();
        }
         [Key]
        public short BONDDIVISIONSLNO { get; set; }
        [Required]
        public string BONDDIVISIONNAME { get; set; }
        public string BONDDIVISIONNAMEBG { get; set; }
        public string INPUTBY { get; set; }
        public Nullable<System.DateTime> INPUTDATE { get; set; }
        public string MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFYDATE { get; set; }
    
        public virtual ICollection<BONDCIRCLE> BONDCIRCLEs { get; set; }
        public virtual ICollection<BONDAREA> BONDAREAs { get; set; }
    }
}

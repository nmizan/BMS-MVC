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
    
    public partial class BONDTYPE
    {
        public BONDTYPE()
        {
            this.BONDERs = new HashSet<BONDER>();
            this.BONDREGISTRATIONs = new HashSet<BONDREGISTRATION>();
        }
    
        public short BTYPESLNO { get; set; }
        public string BTYPECODE { get; set; }
        public string BTYPECODEBG { get; set; }
        [Required]
        public string BTYPENAME { get; set; }
        public string BTYPENAMEBG { get; set; }
        public string INPUTBY { get; set; }
        public Nullable<System.DateTime> INPUTDATE { get; set; }
        public string MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFYDATE { get; set; }
    
        public virtual ICollection<BONDER> BONDERs { get; set; }
        public virtual ICollection<BONDREGISTRATION> BONDREGISTRATIONs { get; set; }
    }
}

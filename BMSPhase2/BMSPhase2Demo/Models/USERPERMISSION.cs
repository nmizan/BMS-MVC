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
    
    public partial class USERPERMISSION
    {
        public short ID { get; set; }
        
        [Required(ErrorMessage = "Please Select User")]
        [Display(Name = "User ID")]
        public short USERID { get; set; }
        [Required(ErrorMessage = "Please Select Role")]
        [Display(Name = "Role")]
        public string ROLENAME { get; set; }
        [Display(Name = "Bonder")]
        public Nullable<int> BONDERID { get; set; }
        [Display(Name = "Employee")]
        public Nullable<short> EMPLOYID { get; set; }
    
        public virtual APPUSER APPUSER { get; set; }
        public virtual BONDER BONDER { get; set; }
        public virtual EMPLOYEE EMPLOYEE { get; set; }
    }
}

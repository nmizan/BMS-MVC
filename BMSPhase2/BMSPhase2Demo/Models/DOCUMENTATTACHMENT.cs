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
    
    public partial class DOCUMENTATTACHMENT
    {
        public short ATTCHSLNO { get; set; }
        public int BONDERSLNO { get; set; }
        public short BSNO { get; set; }
        public string APPLICANTREFNO { get; set; }
        public string DOCHEADINGNAME { get; set; }
        public string DOCHEADINGNAMEBG { get; set; }
        public string RGATTCHNAME { get; set; }
        public string RGATTCHNAMEBG { get; set; }
        public Nullable<System.DateTime> ISSUEDATE { get; set; }
        public Nullable<System.DateTime> EXPDATE { get; set; }
        public string ATTACHFILENM { get; set; }
        public string ATTACHFILEPATH { get; set; }
        public byte[] ATTACHFILEDATA { get; set; }
        public string INPUTBY { get; set; }
        public Nullable<System.DateTime> INPUTDATE { get; set; }
        public string MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFYDATE { get; set; }
    
        public virtual BONDER BONDER { get; set; }
    }
}

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
    
    public partial class BONDERLIENBANK
    {
        public short LBANKSLNO { get; set; }
        public int BONDERSLNO { get; set; }
        public short BBRANCHSLNO { get; set; }
        public string LIENBANKCODE { get; set; }
        public string LIENBANKCODEBG { get; set; }
        public string CERATTACHFILENM { get; set; }
        public string CERATTACHFILEPATH { get; set; }
        public byte[] CERATTACHFILEDATA { get; set; }
        public string INPUTBY { get; set; }
        public Nullable<System.DateTime> INPUTDATE { get; set; }
        public string MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFYDATE { get; set; }
        public Nullable<short> BANKSLNO { get; set; }
        public string ADDRESS { get; set; }
        public string PHONE { get; set; }
        public string FAX { get; set; }
    
        public virtual BANK BANK { get; set; }
        public virtual BANKBRANCH BANKBRANCH { get; set; }
        public virtual BONDER BONDER { get; set; }
    }
}
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
    
    public partial class ASSOCIATIONENROLL
    {
        public short ASSOENSLNO { get; set; }
        public int BONDERSLNO { get; set; }
        public short OFFICESLNO { get; set; }
        public short BSNO { get; set; }
        public string MEMBERSHIPREGNO { get; set; }
        public string MEMBERSHIPREGNOBG { get; set; }
        public Nullable<System.DateTime> ISSUEDATE { get; set; }
        public Nullable<System.DateTime> EXPDATE { get; set; }
        public string CERATTACHFILENM { get; set; }
        public string CERATTACHFILEPATH { get; set; }
        public byte[] CERATTACHFILEDATA { get; set; }
        public string RECATTACHFILENM { get; set; }
        public string RECATTACHFILEPATH { get; set; }
        public byte[] RECATTACHFILEDATA { get; set; }
        public string INPUTBY { get; set; }
        public Nullable<System.DateTime> INPUTDATE { get; set; }
        public string MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFYDATE { get; set; }
    
        public virtual BONDER BONDER { get; set; }
        public virtual OFFICE OFFICE { get; set; }
        public virtual BONDSTATU BONDSTATU { get; set; }
    }
}

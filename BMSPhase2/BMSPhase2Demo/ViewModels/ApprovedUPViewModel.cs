using BMSPhase2Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.ViewModels
{
    public class ApprovedUPViewModel
    {
        public ApprovedUPViewModel()
        {
            this.UPEXBONDLISTs = new HashSet<UPEXBONDLIST>();
            this.EXBONDs = new List<EXBOND>();
            this.ATTACHMENTs = new List<ATTACHMENT>();
        }
        public virtual ICollection<UPEXBONDLIST> UPEXBONDLISTs { get; set; }
        public virtual IList<EXBOND> EXBONDs { get; set; }
        public virtual IList<ATTACHMENT> ATTACHMENTs { get; set; }
        public virtual IList<BACKTOBACKLC> BACKTOBACKLCs { get; set; }
        public virtual IList<BACKTOBACKPRODUCT> BACKTOBACKPRODUCTs { get; set; }
        public virtual IList<RAWMATERIAL> RAWMATERIALs { get; set; }
        public virtual IList<UPREQUEST> UPREQUESTs { get; set; }
        public virtual IList<UPREQUESTLIST> UPREQUESTLISTs { get; set; }

        public UP UP { get; set; }
    }
}
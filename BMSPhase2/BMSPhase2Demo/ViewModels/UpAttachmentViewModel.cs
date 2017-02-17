using BMSPhase2Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.ViewModels
{
    public class UpAttachmentViewModel
    {
        public UpAttachmentViewModel()
        {
            this.UPEXBONDLISTs = new HashSet<UPEXBONDLIST>();
            this.EXBONDs = new List<EXBOND>();
            this.ATTACHMENTs = new List<ATTACHMENT>();
        }
        public virtual ICollection<UPEXBONDLIST> UPEXBONDLISTs { get; set; }
        public virtual IList<EXBOND> EXBONDs { get; set; }
        public virtual IList<ATTACHMENT> ATTACHMENTs { get; set; }

        public UPREQUEST uprequest { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.Models
{
    public class RenewalViewModel
    {
        public List<BONDER> Bonder { get; set; }
        public List<DOCUMENTATTACHMENT> DocumentAttachments { get; set; }
        public List<BONDSTATU> Bondstatus { get; set; }
    }
}
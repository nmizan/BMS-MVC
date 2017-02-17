using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.Models
{
    public class RegistrationViewModel
    {
        public List<DOCUMENTATTACHMENT> documentattachment { get; set; }
        public List<ASSOCIATIONENROLL> associationenroll { get; set; }
        public List<BONDERLIENBANK> bonderlienbank { get; set; }
        public List<INSTALLEDMACHINEINFO> installedmachineinfo { get; set; }
        public List<OWNERINFO> ownerinfo { get; set; }
        public List<BONDERPRODUCT> bonderproduct { get; set; }
        public List<PRODCAPACITYMACHINEWISEM> prodcapacitymachinewisem { get; set; }
        public List<PRODCAPACITYMACHINEWISEDET> prodcapacitymachinewisedet { get; set; }
        public List<OWNERINFO> assocOwnerInfo { get; set; }
        public List<OWNERASSOCIATEBUSINESS> ownerassociatebusiness { get; set; }
        public List<ANNUALENTLRAWMATERIAL> annualentlrawmaterial { get; set; }
        public List<BONDER> bonder { get; set; }
        public List<BONDREGISTRATION> bondregistration { get; set; }
    }
}
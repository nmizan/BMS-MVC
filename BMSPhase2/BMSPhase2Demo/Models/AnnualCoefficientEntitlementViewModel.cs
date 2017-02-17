using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.Models
{
    public class AnnualCoefficientEntitlementViewModel
    {
        public BONDERANNUALENTITLEMENT BonderAnnualEntitlement { set; get; }
        public ANNUALENTLRAWMATERIAL AnnualEntitlementRawMaterial { set; get; }
        public MATERIAL Materials { set; get; }
        public BONDER Bonder { get; set; }
        public BONDSTATU BondStatus { get; set; }
        public MACHINEINFORMATION Machine { get; set; }
        public BONDERANNUALENTITLEMENTCOM BonderAnnualEntitlementCoM { get; set; }
        public BONDERANNUALENTITLEMENTCOEDET BonderannualEntitlementCoEDet { get; set; }
    }
}
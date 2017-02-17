using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.Models
{
    public class AnnualCoefficientEntitlementViewModelList
    {
        public BONDERANNUALENTITLEMENT BonderAnnualEntitlement { set; get; }
        public IEnumerable<ANNUALENTLRAWMATERIAL> AnnualEntitlementRawMaterials { set; get; }
        public IEnumerable<MATERIAL> Materials { set; get; }
        public IEnumerable<BONDER> Bonders { get; set; }
        public IEnumerable<BONDSTATU> BondStatus { get; set; }
        public IEnumerable<MACHINEINFORMATION> Machine { get; set; }
        public IEnumerable<BONDERANNUALENTITLEMENTCOM> BonderAnnualEntitlementCoMs { get; set; }
        public IEnumerable<BONDERANNUALENTITLEMENTCOEDET> BonderannualEntitlementCoEDets { get; set; }
        
    }
}
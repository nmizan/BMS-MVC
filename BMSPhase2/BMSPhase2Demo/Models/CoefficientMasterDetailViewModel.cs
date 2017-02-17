using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.Models
{
    public class CoefficientMasterDetailViewModel
    {
        public BONDERANNUALENTITLEMENTCOM Bonderannualentitlementcoms { set; get; }
        public List<BONDERANNUALENTITLEMENTCOEDET> Bonderannualentitlementcoedets { set; get; }
    }
}
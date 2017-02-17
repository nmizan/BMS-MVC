using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.Models
{
    public class RawMaterialInfo
    {
        public RawMaterialInfo()
        {
            this.InBondInfoList = new List<InBondInfo>();
        }

        public short RawMaterialID { get; set; }

        public short BonderID { get; set; }

        public short ProductID { get; set; }

        public string RawMaterialName { get; set; }

        public string RawMaterialQuantity { get; set; }

        public string QuantityUnit { get; set; }
        public string QuantityWastage { get; set; }
        public string TotalUsage { get; set; }

        public string HSCode { get; set; }
        public decimal AnnualEntitlement { get; set; }

        public decimal TotalInBondOfThePeriod { get; set; }

        public decimal BalanceEntitlement { get; set; }

        public string AnnualEntitleUnit { get; set; }

        public string TotalInBondUnit { get; set; }

        public string BalanceEntitlementUnit { get; set; }

        public virtual IList<InBondInfo> InBondInfoList { get; set; }
    }

}
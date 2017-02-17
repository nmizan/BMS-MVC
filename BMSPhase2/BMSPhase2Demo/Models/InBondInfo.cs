using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.Models
{
    public class InBondInfo
    {
        public InBondInfo()
        {
            this.ExBondInfos = new List<ExBondInfo>();
        }

        public short InBondInfoID { get; set; }

        public short RawMaterialID { get; set; }

        public System.DateTime ImportDate { get; set; }

        public string LC { get; set; }

        public string BOE { get; set; }

        public System.DateTime InBondDate { get; set; }

        public decimal InBondQuantity { get; set; }

        public decimal InBondValue { get; set; }

        public string InBondQuantityUnit { get; set; }

        public string InBondValueUnit { get; set; }

        public List<ExBondInfo> ExBondInfos { get; set; }
    }
}
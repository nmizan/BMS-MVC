using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.Models
{
    public class ExBondInfo
    {
        public ExBondInfo()
        {

        }

        public Int32 ExBondInfoID { get; set; }
        public System.DateTime ExBondDate { get; set; }
        public decimal ExBondQuantity { get; set; }
        public string ExBondQuantityUnit { get; set; }
        public decimal ExBondValue { get; set; }
        public string ExBondValueUnit { get; set; }
        public decimal PreviousBalance { get; set; }
        public string PreviousBalanceUnit { get; set; }
        public decimal BalanceQuantity { get; set; }
        public string BalanceQuantityUnit { get; set; }
        public decimal BalanceValue { get; set; }
        public string BalanceValueUnit { get; set; }
        public Int32 InBondInfoID { get; set; }

    }
}
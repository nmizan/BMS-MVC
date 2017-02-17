using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.Models
{
    public class InBondExBondMaterial
    {
        public InBondExBondMaterial()
        {

        }

        [Display(Name = "Bonder Name")]
        public string BonderName { get; set; }
        [Display(Name = "Bonder LC No")]
        public string BonderLCNo { get; set; }
        public System.DateTime DateFrom { get; set; }
        public System.DateTime DateTo { get; set; }
        public string BonderAddress { get; set; }
        [Display(Name = "Raw Material Name")]
        public string RawMaterialName { get; set; }
        public string HSCode { get; set; }
        [Display(Name = "Annual Entitlement")]
        public decimal AnnualEntitlement { get; set; }
        [Display(Name = "Total In Bond Of The Period")]
        public decimal TotalInBondOfThePeriod { get; set; }
        [Display(Name = "Balance Entitlement")]
        public decimal BalanceEntitlement { get; set; }
        public string AnnualEntitleUnit { get; set; }
        public string TotalInBondUnit { get; set; }
        public string BalanceEntitlementUnit { get; set; }
        [Display(Name = "Import Date")]
        public System.DateTime ImportDate { get; set; }
        public string LC { get; set; }
        public string BOE { get; set; }
        [Display(Name = "In Bond Date")]
        public System.DateTime InBondDate { get; set; }
        [Display(Name = "In Bond Quantity")]
        public decimal InBondQuantity { get; set; }
        [Display(Name = "In Bond Value")]
        public decimal InBondValue { get; set; }
        public string InBondQuantityUnit { get; set; }
        public string InBondValueUnit { get; set; }
        [Display(Name = "Ex Bond Date")]
        public System.DateTime ExBondDate { get; set; }
        [Display(Name = "Ex Bond Quantity")]
        public decimal ExBondQuantity { get; set; }
        public string ExBondQuantityUnit { get; set; }
        [Display(Name = "Ex Bond Value")]
        public decimal ExBondValue { get; set; }
        public string ExBondValueUnit { get; set; }
        [Display(Name = "Previous Balance")]
        public decimal PreviousBalance { get; set; }
        public string PreviousBalanceUnit { get; set; }
        [Display(Name = "Balance Quantity")]
        public decimal BalanceQuantity { get; set; }
        public string BalanceQuantityUnit { get; set; }
        [Display(Name = "Balance Value")]
        public decimal BalanceValue { get; set; }
        public string BalanceValueUnit { get; set; }
        public Int32 RAWMATERIALCODE { get; set; }
        public Int32 BonderID { get; set; }

    }
}
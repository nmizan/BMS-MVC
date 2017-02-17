using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.Models
{
    public class BonderProductRawMaterial
    {
        public BonderProductRawMaterial()
        {

        }

        [Display(Name = "Bonder Name")]
        public string BonderName { get; set; }

        [Display(Name = "Bond License No")]
        public string BonderLCNo { get; set; }

        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime DateFrom { get; set; }

        [Display(Name = "To Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime DateTo { get; set; }

        [Display(Name = "UP No")]
        public string UPNo { get; set; }

        [Display(Name = "Address")]
        public string BonderAddress { get; set; }

        [Display(Name = "Buyer Name")]
        public string BuyerName { get; set; }

        [Display(Name = "UD No")]
        public string UDNo { get; set; }

        [Display(Name = "UD Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime UDDate { get; set; }

        [Display(Name = "Product in the UD")]
        public string UDProduct { get; set; }

        [Display(Name = "Qty of Product in the UD")]
        public decimal UDProductQuantity { get; set; }

        [Display(Name = "Unit of Product in the UD")]
        public string UDProductQuantityUnit { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Product Size")]
        public string ProductSize { get; set; }

        [Display(Name = "Product Description")]
        public string ProductDescription { get; set; }

        [Display(Name = "Product Quantity")]
        public Nullable<decimal> ProductQuantity { get; set; }

        [Display(Name = "Product Quantity Unit")]
        public string ProductQuantityUnit { get; set; }

        [Display(Name = "Raw Materials Used")]
        public string RawMaterialName { get; set; }

        [Display(Name = "Raw Materials Qnt Used")]
        public Nullable<decimal> RawMaterialQuantity { get; set; }

        [Display(Name = "Raw Materials Qnt Unit")]
        public string RawMaterialQuantityUnit { get; set; }

        [Display(Name = "Quantity Wastage")]
        public Nullable<decimal> QuantityWastage { get; set; }

        [Display(Name = "Total Usage")]
        public Nullable<decimal> TotalUsageQuantity { get; set; }

        [Display(Name = "Total Usage Unit")]
        public string TotalUsageQuantityUnit { get; set; }

    }
}
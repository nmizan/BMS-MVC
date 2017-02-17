using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.Models
{
    public class UDInfo
    {
        public UDInfo()
        {
            this.ProductInfoList = new List<ProductInfo>();
        }


        public short UDID { get; set; }

        public short BonderID { get; set; }

        public string BuyerName { get; set; }

        public string UDNo { get; set; }

        public System.DateTime UDDate { get; set; }

        public string UDProduct { get; set; }

        public string UDProductQuantity { get; set; }

        public string UDProductQuantityUnit { get; set; }

        public virtual IList<ProductInfo> ProductInfoList { get; set; }
    }



}
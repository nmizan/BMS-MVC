using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BMSPhase2Demo.Models
{
    public class ProductInfo
    {
        public ProductInfo()
        {
            this.RawMaterialInfo = new List<RawMaterialInfo>();
        }
        public short ProductID { get; set; }

        public short UDID { get; set; }

        public string ProductName { get; set; }

        public string ProductSize { get; set; }

        public string ProductDescription { get; set; }

        public string ProductQuantity { get; set; }

        public string ProductQuantityUnit { get; set; }

        public virtual IList<RawMaterialInfo> RawMaterialInfo { get; set; }
    }
}

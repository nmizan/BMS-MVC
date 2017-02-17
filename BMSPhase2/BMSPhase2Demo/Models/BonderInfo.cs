using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.Models
{
    public class BonderInfo
    {
        public BonderInfo()
        {
            this.RawMaterialInfoList = new List<RawMaterialInfo>();
            this.UDInfoList = new List<UDInfo>();
        }

        public short BonderID { get; set; }

        public string BonderName { get; set; }

        public string BonderLCNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime DateFrom { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime DateTo { get; set; }
        public string BonderAddress { get; set; }

        public string UPNo { get; set; }

        public virtual IList<RawMaterialInfo> RawMaterialInfoList { get; set; }

        public virtual IList<UDInfo> UDInfoList { get; set; }

    }
}
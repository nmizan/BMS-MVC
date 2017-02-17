using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSPhase2Demo.Models
{
    public class QuantityModel
    {
        public QuantityModel()
        {
            QUANTITYUNITLIST = new List<SelectListItem>{
                //new SelectListItem {Text = "Piece", Value = "PCS"},
                new SelectListItem {Text = "Kg", Value = "Kg"},
                new SelectListItem {Text = "Ton", Value = "Ton"},
                new SelectListItem {Text = "Pcs", Value = "Pcs"},
                new SelectListItem {Text = "Meter", Value = "Meter"},
                new SelectListItem {Text = "Dozen", Value = "Dozen"},
                new SelectListItem {Text = "Inch", Value = "Inch"},
                new SelectListItem {Text = "Foot", Value = "Foot"},
                new SelectListItem {Text = "Litre", Value = "Litre"}
            };
            COSTUNITLIST = new List<SelectListItem>
             {
                new SelectListItem {Text = "USD", Value = "USD"},
                new SelectListItem {Text = "BDT", Value = "BDT"},
                
            };
        }
        public List<SelectListItem> QUANTITYUNITLIST { get; set; }
        public List<SelectListItem> COSTUNITLIST { get; set; }
    }
}
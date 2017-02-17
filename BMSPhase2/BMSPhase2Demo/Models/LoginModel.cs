using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.Models
{
    public class LoginModel
    {
        [Display(Name = "Username")]
        public string USERNAME { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string PASSWARD { get; set; }
    }
}
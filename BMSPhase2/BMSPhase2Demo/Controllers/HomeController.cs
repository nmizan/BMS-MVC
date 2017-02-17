using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSPhase2Demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Bond Module System";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Application Description";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact US";

            return View();
        }
    }
}

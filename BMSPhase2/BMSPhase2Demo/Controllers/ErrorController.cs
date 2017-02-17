using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSPhase2Demo.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult AccessDenied()
        {
            return View("Unauthorized");
        }

    }
}

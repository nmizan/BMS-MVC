using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMSPhase2Demo.Models;

namespace BMSPhase2Demo.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/

        protected override void ExecuteCore()
        {
            int culture = 0;
            if (this.Session == null || this.Session["CurrentUICulture"] == null)
            {
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Culture"], out culture);
                this.Session["CurrentUICulture"] = culture;
            }
            else
            {
                culture = (int)this.Session["CurrentUICulture"];
            }
            //
            SiteSession.CurrentUICulture = culture;
            //
            // Invokes the action in the current controller context.
            //
            base.ExecuteCore();
        }
        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }
    }
}

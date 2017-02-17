using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace BMSPhase2Demo.Models
{
    public class SiteSession
    {
        public static int CurrentUICulture
        {
            get
            {
                if (Thread.CurrentThread.CurrentUICulture.Name == "bn-BD")
                    return 1;
                else
                    return 0;
            }
            set
            {
                if (value == 1)
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("bn-BD");
                else
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            }
        }
    }
}
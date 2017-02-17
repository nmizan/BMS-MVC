using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.CommonAppSet
{
    public class ConnInfo
    {
        public static string server = System.Configuration.ConfigurationManager.
   ConnectionStrings["BMSDbContext1"].ConnectionString.Substring(System.Configuration.ConfigurationManager.
   ConnectionStrings["BMSDbContext1"].ConnectionString.IndexOf('=') + 1, System.Configuration.ConfigurationManager.
   ConnectionStrings["BMSDbContext1"].ConnectionString.IndexOf(';') - System.Configuration.ConfigurationManager.
   ConnectionStrings["BMSDbContext1"].ConnectionString.IndexOf('=') - 1);
        static string a = System.Configuration.ConfigurationManager.
      ConnectionStrings["BMSDbContext1"].ConnectionString.Remove(0, System.Configuration.ConfigurationManager.
      ConnectionStrings["BMSDbContext1"].ConnectionString.IndexOf(';') + 1);
        public static string userId = a.Substring(a.IndexOf('=') + 1, a.IndexOf(';') - a.IndexOf('=') - 1);
        static string b = a.Remove(0, a.IndexOf(';') + 1);
        public static string pass = b.Substring(b.IndexOf('=') + 1, b.IndexOf(';') - b.IndexOf('=') - 1);
    }
}
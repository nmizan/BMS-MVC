using BMSPhase2Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace BMSPhase2Demo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        //let us take out the username now                
                        string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        
                        String userRole = "";
                        using (OracleEntitiesConnStr db = new OracleEntitiesConnStr())
                        {
                            APPUSER appuser = db.APPUSERs.SingleOrDefault(u => u.USERNAME.Equals(username, StringComparison.OrdinalIgnoreCase));
                            USERPERMISSION userpermission = db.USERPERMISSIONs.SingleOrDefault(u => u.USERID == appuser.ID);
                            string role = userpermission.ROLENAME;
                            userRole = role;
                            System.Web.HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                            new System.Security.Principal.GenericIdentity(username, "Forms"), new[] { userRole });
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }
}
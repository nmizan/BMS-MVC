using BMSPhase2Demo.Models;
using BMSPhase2Demo.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BMSPhase2Demo.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();
        private EncryptionDecryptionUtil encryptionDecryptionUtil = new EncryptionDecryptionUtil();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(LoginModel loginModel, string returnUrl) {
            
            if (loginModel.USERNAME == null || loginModel.PASSWARD == null || loginModel.USERNAME.Trim().Equals("") || loginModel.PASSWARD.Trim().Equals("")) {
                ModelState.AddModelError("", "Wrong Username or Password");
            }
            List<APPUSER> appusers = db.APPUSERs.ToList();
            foreach (var appuser in appusers)
            {
                if (appuser.USERNAME.Equals(loginModel.USERNAME) && encryptionDecryptionUtil.VerifyPassword(appuser.PASSWARD,loginModel.PASSWARD,appuser.SALT)) {
                    
                    FormsAuthentication.SetAuthCookie(loginModel.USERNAME, false);
                    
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else {

                        USERPERMISSION userpermission = db.USERPERMISSIONs.SingleOrDefault(u => u.USERID == appuser.ID);
                        if (userpermission == null)
                        {
                            FormsAuthentication.SignOut();
                            return RedirectToAction("AccessDenied", "Error", null);
                        }
                        appuser.LASTLOGIN = DateTime.Now;
                        db.Entry(appuser).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            if (ModelState.IsValid)
            {
                ModelState.AddModelError("", "Wrong Username or Password");
            }
            return View(loginModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}

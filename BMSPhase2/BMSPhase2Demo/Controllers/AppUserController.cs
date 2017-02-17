using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMSPhase2Demo.Models;
using BMSPhase2Demo.Util;
using System.Web.Security;
using System.Data.Entity.Infrastructure;
using BMSPhase2Demo.Utils;
using PagedList;
namespace BMSPhase2Demo.Controllers
{
    [CustomAuthorize(Roles = "Operation Admin,Bonder")]
    public class AppUserController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();
        private EncryptionDecryptionUtil encryptionDecryptionUtil = new EncryptionDecryptionUtil();
        private int saltLength = 5;
        //
        // GET: /AppUser/
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Index(string user, string firstname, string lastname, string currentFilterUser, String currentFilterFirstname, String currentFilterLastname, int? page)
        {
            var requestType = this.HttpContext.Request.RequestType;
            int pageSize = GlobalConstants.recordNumbers;
            int pageNumber = (page ?? 1);
            if (System.Web.HttpContext.Current.User.IsInRole("Bonder"))
            {
                string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                APPUSER appuser = db.APPUSERs.SingleOrDefault(u => u.USERNAME.Equals(username, StringComparison.OrdinalIgnoreCase));
                List<APPUSER> appusers = new List<APPUSER>();
                appusers.Add(appuser);
                return View(appusers.ToPagedList(pageNumber, pageSize));
            }

            if (user != null || firstname != null || lastname != null)
            {
                page = 1;
            }
            else
            {
                if (user == null)
                {
                    user = currentFilterUser;
                }
                if (firstname == null)
                {
                    firstname = currentFilterFirstname;
                }
                if (lastname == null)
                {
                    lastname = currentFilterLastname;
                }
            }
            ViewBag.CurrentFilterUser = user;
            ViewBag.CurrentFilterFirstname = firstname;
            ViewBag.CurrentFilterLastname = lastname;
            List<APPUSER> searchAppUsers = new List<APPUSER>();
            searchAppUsers = db.APPUSERs.ToList();
            if ("GET" == requestType)
            {
                if (user != null || firstname != null || lastname != null)
                {
                    searchAppUsers = GetAppusersBySearchCriteria(user, firstname, lastname);
                }
                return View(searchAppUsers.ToPagedList(pageNumber, pageSize));

            }
            else if ("POST" == requestType)
            {
                searchAppUsers = GetAppusersBySearchCriteria(user, firstname, lastname);
            }
            pageSize = GlobalConstants.recordNumbers;
            pageNumber = (page ?? 1);
            return View(searchAppUsers.ToPagedList(pageNumber, pageSize));
        }
        public List<APPUSER> GetAppusersBySearchCriteria(string user, string firstname, string lastname)
        {
            if (user == null)
            {
                user = "";
            }
            if (firstname == null)
            {
                firstname = "";
            }
            if (lastname == null)
            {
                lastname = "";
            }
            if (user.Equals("") && firstname.Equals("") && lastname.Equals(""))
            {
                return db.APPUSERs.ToList();
            }
            return db.APPUSERs.Where(u => u.FIRSTNAME.Trim().ToUpper().Contains(firstname.Trim().ToUpper())
            || u.LASTNAME.Trim().ToUpper().Contains(lastname.Trim().ToUpper()) || u.USERNAME.Trim().ToUpper().Contains(user.Trim().ToUpper())).OrderByDescending(u=>u.ID).ToList();
        }
        //
        // GET: /AppUser/Details/5
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Details(short id = 0)
        {
            APPUSER appuser = db.APPUSERs.Find(id);
            if (appuser == null)
            {
                return HttpNotFound();
            }
            return View(appuser);
        }

        //
        // GET: /AppUser/Create
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Create()
        {
            APPUSER appuser = new APPUSER();
            appuser.SALT = encryptionDecryptionUtil.GenerateSalt(saltLength);
            return View(appuser);
        }

        //
        // POST: /AppUser/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Operation Admin")]
        public ActionResult Create(APPUSER appuser)
        {
            if (ModelState.IsValid)
            {
                appuser.PASSWARD = encryptionDecryptionUtil.CreatePasswordHash(appuser.PASSWARD, appuser.SALT);
                appuser.ISACTIVE = 1;
                APPUSER duplicateUser = db.APPUSERs.SingleOrDefault(u => u.USERNAME.Equals(appuser.USERNAME, StringComparison.OrdinalIgnoreCase));
                if (duplicateUser != null)
                {
                    ModelState.AddModelError("", "User Already Exists");
                    appuser.PASSWARD = "";
                    return View(appuser);
                }
                db.APPUSERs.Add(appuser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(appuser);
        }

        //
        // GET: /AppUser/Edit/5
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Edit(short id = 0, bool isPasswordChange = false)
        {
            APPUSER appuser = db.APPUSERs.Find(id);
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            if (System.Web.HttpContext.Current.User.IsInRole("Bonder"))
            {

                if (!appuser.USERNAME.Equals(username))
                {
                    return RedirectToAction("AccessDenied", "Error");
                }
            }
            if (appuser == null)
            {
                return HttpNotFound();
            }
            if (appuser.USERNAME.Equals(username) && isPasswordChange)
            {
                appuser.PASSWARD = "";
                return View("EditForBonder", appuser);
            }
            else if (appuser.USERNAME.Equals(username))
            {
                return View("EditOwnInfo", appuser);
            }
            return View(appuser);
        }

        //
        // POST: /AppUser/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Edit(APPUSER appuser, string newPassword, string confirmPassword, bool? isPasswordChange)
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            if (ModelState.IsValid)
            {
                if (username.Equals(appuser.USERNAME) && isPasswordChange == true)
                {

                    APPUSER appuserFound = db.APPUSERs.SingleOrDefault(u => u.USERNAME.Equals(username, StringComparison.OrdinalIgnoreCase));
                    if (appuserFound != null)
                    {
                        if (encryptionDecryptionUtil.VerifyPassword(appuserFound.PASSWARD, appuser.PASSWARD, appuser.SALT))
                        {
                            if (newPassword.Trim().Equals("") && confirmPassword.Trim().Equals(""))
                            {
                                ModelState.AddModelError("", "Password Cannot Be Empty");
                                appuser.PASSWARD = "";
                                return View("EditForBonder", appuser);
                            }
                            if (newPassword.Equals(confirmPassword))
                            {
                                appuser.PASSWARD = encryptionDecryptionUtil.CreatePasswordHash(newPassword, appuser.SALT);
                                ((IObjectContextAdapter)db).ObjectContext.Detach(appuserFound);
                                db.Entry(appuser).State = EntityState.Modified;
                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                ModelState.AddModelError("", "New Password Does Not Match");
                                appuser.PASSWARD = "";
                                return View("EditForBonder", appuser);
                            }
                        }
                        ModelState.AddModelError("", "Current Password is Wrong");
                        appuser.PASSWARD = "";
                        return View("EditForBonder", appuser);
                    }
                }
                appuser.PASSWARD = encryptionDecryptionUtil.CreatePasswordHash(appuser.PASSWARD, appuser.SALT);
                db.Entry(appuser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else if (appuser.USERNAME.Equals(username))
            {
                return View("EditOwnInfo", appuser);
            }
            return View(appuser);
        }

        //
        // GET: /AppUser/Delete/5
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Delete(short id = 0)
        {
            APPUSER appuser = db.APPUSERs.Find(id);
            if (appuser == null)
            {
                return HttpNotFound();
            }
            return View(appuser);
        }

        //
        // POST: /AppUser/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult DeleteConfirmed(short id)
        {
            APPUSER appuser = db.APPUSERs.Find(id);
            OracleEntitiesConnStr userDb = new OracleEntitiesConnStr();
            List<USERPERMISSION> userPermissions = userDb.USERPERMISSIONs.Where(u => u.USERID == appuser.ID).ToList();
            foreach (USERPERMISSION userpermission in userPermissions)
            {
                userDb.USERPERMISSIONs.Remove(userpermission);
                userDb.SaveChanges();
            }

            db.APPUSERs.Remove(appuser);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
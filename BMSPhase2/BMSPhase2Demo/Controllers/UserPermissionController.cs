using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMSPhase2Demo.Models;
using System.Data.Entity.Infrastructure;
using BMSPhase2Demo.Utils;

namespace BMSPhase2Demo.Controllers
{
    public class UserPermissionController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /UserPermission/
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Index()
        {
            var userpermissions = db.USERPERMISSIONs.Include(u => u.APPUSER).Include(u => u.BONDER).Include(u => u.EMPLOYEE);
            return View(userpermissions.ToList());
        }

        //
        // GET: /UserPermission/Details/5
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Details(short id = 0)
        {
            USERPERMISSION userpermission = db.USERPERMISSIONs.Find(id);
            if (userpermission == null)
            {
                return HttpNotFound();
            }
            return View(userpermission);
        }

        //
        // GET: /UserPermission/Create
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Create(APPUSER appuser)
        {
            USERPERMISSION userpermission = db.USERPERMISSIONs.SingleOrDefault(u => u.USERID == appuser.ID);

            if (userpermission == null)
            {
                List<APPUSER> appusers = new List<APPUSER>();
                List<SelectListItem> listItem = new List<SelectListItem>();
                listItem.Add(new SelectListItem
                {
                    Text = "Bonder",
                    Value = "Bonder",

                });
                listItem.Add(new SelectListItem
                {
                    Text = "Operation Admin",
                    Value = "Operation Admin",

                });
                appusers.Add(appuser);
                ViewBag.USERID = new SelectList(appusers, "ID", "USERNAME");
                ViewBag.ROLENAME = listItem;
                ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
                ViewBag.EMPLOYID = new SelectList(db.EMPLOYEEs, "EMPLOYEESLNO", "EMPLOYEENAME");
            }
            else
            {
                List<APPUSER> appusers = new List<APPUSER>();
                appusers.Add(appuser);
                List<SelectListItem> listItem = new List<SelectListItem>();
                if (userpermission.ROLENAME.Equals("Bonder"))
                {
                    listItem.Add(new SelectListItem
                    {
                        Text = "Bonder",
                        Value = "Bonder",
                        Selected = true,
                    });
                    listItem.Add(new SelectListItem
                    {
                        Text = "Operation Admin",
                        Value = "Operation Admin"

                    });
                }
                else if (userpermission.ROLENAME.Equals("Operation Admin"))
                {
                    listItem.Add(new SelectListItem
                    {
                        Text = "Bonder",
                        Value = "Bonder",
                    });
                    listItem.Add(new SelectListItem
                    {
                        Text = "Operation Admin",
                        Value = "Operation Admin",
                        Selected = true
                    });
                }
                ViewBag.USERID = new SelectList(appusers, "ID", "USERNAME");
                if (userpermission.BONDER != null)
                {
                    ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", userpermission.BONDER.BONDERNAME);
                }
                else
                {
                    ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
                }
                if (userpermission.EMPLOYEE != null)
                {
                    ViewBag.EMPLOYID = new SelectList(db.EMPLOYEEs, "EMPLOYEESLNO", "EMPLOYEENAME", userpermission.EMPLOYEE.EMPLOYEENAME);
                }
                else
                {
                    ViewBag.EMPLOYID = new SelectList(db.EMPLOYEEs, "EMPLOYEESLNO", "EMPLOYEENAME");
                }
                ViewBag.ROLENAME = listItem;
                ViewBag.EMPLOYID = new SelectList(db.EMPLOYEEs, "EMPLOYEESLNO", "EMPLOYEENAME");
            }
            return View();
        }

        //
        // POST: /UserPermission/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Create(USERPERMISSION userpermission,string type="")
        {
            if (ModelState.IsValid)
            {
                USERPERMISSION userpermissionFound = db.USERPERMISSIONs.SingleOrDefault(u => u.USERID == userpermission.USERID);

                if (userpermissionFound != null)
                {
                    if (userpermission.BONDERID != null)
                    {
                        if (userpermission.ROLENAME == "Bonder")
                        {
                            BONDER bonder = db.BONDERs.SingleOrDefault(u => u.BONDERSLNO == userpermission.BONDERID);
                            userpermission.BONDER = bonder;
                        }
                        else
                        {
                            userpermission.BONDERID = null;
                        }

                    }
                    else if (userpermission.EMPLOYID != null)
                    {
                        EMPLOYEE employee = db.EMPLOYEEs.SingleOrDefault(u => u.EMPLOYEESLNO == userpermission.EMPLOYID);
                        userpermission.EMPLOYEE = employee;

                    }
                    if (userpermissionFound.EMPLOYID == null && userpermission.EMPLOYID != null)
                    {
                        userpermission.BONDER = null;
                        userpermission.BONDERID = null;
                    }
                    if (userpermissionFound.BONDERID == null && userpermission.BONDERID != null)
                    {
                        userpermission.EMPLOYEE = null;
                        userpermission.EMPLOYID = null;
                    }
                    if (userpermission.USERID > 0)
                    {
                        APPUSER appuser = db.APPUSERs.SingleOrDefault(u => u.ID == userpermission.USERID);
                        userpermission.APPUSER = appuser;
                        userpermissionFound.APPUSER = appuser;
                    }


                    ((IObjectContextAdapter)db).ObjectContext.Detach(userpermissionFound);
                    USERPERMISSION userpermissionFnd = db.USERPERMISSIONs.Find(userpermissionFound.ID);
                    db.USERPERMISSIONs.Remove(userpermissionFnd);
                    db.SaveChanges();
                    //db.Entry(userpermission).State = EntityState.Modified;
                    if (type.Equals("Operation Admin")) {
                        userpermission.BONDER = null;
                        userpermission.BONDERID = null;
                    }
                    else if (type.Equals("Bonder"))
                    {
                        userpermission.EMPLOYEE = null;
                        userpermission.EMPLOYID = null;
                    }
                    db.USERPERMISSIONs.Add(userpermission);
                    db.SaveChanges();

                }
                else
                {
                    if (type.Equals("Operation Admin"))
                    {
                        userpermission.BONDER = null;
                        userpermission.BONDERID = null;
                    }
                    else if (type.Equals("Bonder"))
                    {
                        userpermission.EMPLOYEE = null;
                        userpermission.EMPLOYID = null;
                    }
                    db.USERPERMISSIONs.Add(userpermission);
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "AppUser", null);
            }

            ViewBag.USERID = new SelectList(db.APPUSERs, "ID", "USERNAME", userpermission.USERID);
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", userpermission.BONDERID);
            ViewBag.EMPLOYID = new SelectList(db.EMPLOYEEs, "EMPLOYEESLNO", "EMPLOYEENAME", userpermission.EMPLOYID);
            return View(userpermission);
        }

        //
        // GET: /UserPermission/Edit/5
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Edit(short id = 0)
        {
            USERPERMISSION userpermission = db.USERPERMISSIONs.Find(id);
            if (userpermission == null)
            {
                return HttpNotFound();
            }
            ViewBag.USERID = new SelectList(db.APPUSERs, "ID", "USERNAME", userpermission.USERID);
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", userpermission.BONDERID);
            ViewBag.EMPLOYID = new SelectList(db.EMPLOYEEs, "EMPLOYEESLNO", "EMPLOYEENAME", userpermission.EMPLOYID);
            return View(userpermission);
        }

        //
        // POST: /UserPermission/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Edit(USERPERMISSION userpermission)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userpermission).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.USERID = new SelectList(db.APPUSERs, "ID", "USERNAME", userpermission.USERID);
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", userpermission.BONDERID);
            ViewBag.EMPLOYID = new SelectList(db.EMPLOYEEs, "EMPLOYEESLNO", "EMPLOYEENAME", userpermission.EMPLOYID);
            return View(userpermission);
        }

        //
        // GET: /UserPermission/Delete/5
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Delete(short id = 0)
        {
            USERPERMISSION userpermission = db.USERPERMISSIONs.Find(id);
            if (userpermission == null)
            {
                return HttpNotFound();
            }
            return View(userpermission);
        }

        //
        // POST: /UserPermission/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult DeleteConfirmed(short id)
        {
            USERPERMISSION userpermission = db.USERPERMISSIONs.Find(id);
            db.USERPERMISSIONs.Remove(userpermission);
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
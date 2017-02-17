using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMSPhase2Demo.Models;

namespace BMSPhase2Demo.Controllers
{
    public class DesignationController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /Designation/

        public ActionResult Index()
        {
            return View(db.DESIGNATIONs.ToList());
        }

        //
        // GET: /Designation/Details/5

        public ActionResult Details(short id = 0)
        {
            DESIGNATION designation = db.DESIGNATIONs.Find(id);
            if (designation == null)
            {
                return HttpNotFound();
            }
            return View(designation);
        }

        //
        // GET: /Designation/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Designation/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DESIGNATION designation)
        {
            if (ModelState.IsValid)
            {
                db.DESIGNATIONs.Add(designation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(designation);
        }

        //
        // GET: /Designation/Edit/5

        public ActionResult Edit(short id = 0)
        {
            DESIGNATION designation = db.DESIGNATIONs.Find(id);
            if (designation == null)
            {
                return HttpNotFound();
            }
            return View(designation);
        }

        //
        // POST: /Designation/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DESIGNATION designation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(designation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(designation);
        }

        //
        // GET: /Designation/Delete/5

        public ActionResult Delete(short id = 0)
        {
            DESIGNATION designation = db.DESIGNATIONs.Find(id);
            if (designation == null)
            {
                return HttpNotFound();
            }
            return View(designation);
        }

        //
        // POST: /Designation/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            DESIGNATION designation = db.DESIGNATIONs.Find(id);
            db.DESIGNATIONs.Remove(designation);
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
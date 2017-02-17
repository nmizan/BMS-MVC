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
    public class UpazilaController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /Upazila/

        public ActionResult Index()
        {
            var upazilas = db.UPAZILAs.Include(u => u.DISTRICT);
            return View(upazilas.ToList());
        }

        //
        // GET: /Upazila/Details/5

        public ActionResult Details(short id = 0)
        {
            UPAZILA upazila = db.UPAZILAs.Find(id);
            if (upazila == null)
            {
                return HttpNotFound();
            }
            return View(upazila);
        }

        //
        // GET: /Upazila/Create

        public ActionResult Create()
        {
            ViewBag.DISTRICTSLNO = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");
            return View();
        }

        //
        // POST: /Upazila/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UPAZILA upazila)
        {
            if (ModelState.IsValid)
            {
                db.UPAZILAs.Add(upazila);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DISTRICTSLNO = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME", upazila.DISTRICTSLNO);
            return View(upazila);
        }

        //
        // GET: /Upazila/Edit/5

        public ActionResult Edit(short id = 0)
        {
            UPAZILA upazila = db.UPAZILAs.Find(id);
            if (upazila == null)
            {
                return HttpNotFound();
            }
            ViewBag.DISTRICTSLNO = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME", upazila.DISTRICTSLNO);
            return View(upazila);
        }

        //
        // POST: /Upazila/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UPAZILA upazila)
        {
            if (ModelState.IsValid)
            {
                db.Entry(upazila).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DISTRICTSLNO = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME", upazila.DISTRICTSLNO);
            return View(upazila);
        }

        //
        // GET: /Upazila/Delete/5

        public ActionResult Delete(short id = 0)
        {
            UPAZILA upazila = db.UPAZILAs.Find(id);
            if (upazila == null)
            {
                return HttpNotFound();
            }
            return View(upazila);
        }

        //
        // POST: /Upazila/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            UPAZILA upazila = db.UPAZILAs.Find(id);
            db.UPAZILAs.Remove(upazila);
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
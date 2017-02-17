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
    public class MeasurementUnitController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /MeasurementUnit/

        public ActionResult Index()
        {
            return View(db.MEASUREMENTUNITs.ToList());
        }

        //
        // GET: /MeasurementUnit/Details/5

        public ActionResult Details(short id = 0)
        {
            MEASUREMENTUNIT measurementunit = db.MEASUREMENTUNITs.Find(id);
            if (measurementunit == null)
            {
                return HttpNotFound();
            }
            return View(measurementunit);
        }

        //
        // GET: /MeasurementUnit/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /MeasurementUnit/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MEASUREMENTUNIT measurementunit)
        {
            if (ModelState.IsValid)
            {
                db.MEASUREMENTUNITs.Add(measurementunit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(measurementunit);
        }

        //
        // GET: /MeasurementUnit/Edit/5

        public ActionResult Edit(short id = 0)
        {
            MEASUREMENTUNIT measurementunit = db.MEASUREMENTUNITs.Find(id);
            if (measurementunit == null)
            {
                return HttpNotFound();
            }
            return View(measurementunit);
        }

        //
        // POST: /MeasurementUnit/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MEASUREMENTUNIT measurementunit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(measurementunit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(measurementunit);
        }

        //
        // GET: /MeasurementUnit/Delete/5

        public ActionResult Delete(short id = 0)
        {
            MEASUREMENTUNIT measurementunit = db.MEASUREMENTUNITs.Find(id);
            if (measurementunit == null)
            {
                return HttpNotFound();
            }
            return View(measurementunit);
        }

        //
        // POST: /MeasurementUnit/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            MEASUREMENTUNIT measurementunit = db.MEASUREMENTUNITs.Find(id);
            db.MEASUREMENTUNITs.Remove(measurementunit);
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
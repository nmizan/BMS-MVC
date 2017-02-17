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
    public class DistrictController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /District/

        public ActionResult Index()
        {
            var districts = db.DISTRICTs.Include(d => d.COUNTRY);
            return View(districts.ToList());
        }

        //
        // GET: /District/Details/5

        public ActionResult Details(short id = 0)
        {
            DISTRICT district = db.DISTRICTs.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            return View(district);
        }

        //
        // GET: /District/Create

        public ActionResult Create()
        {
            ViewBag.COUNTRYSLNO = new SelectList(db.COUNTRies, "COUNTRYSLNO", "COUNTRYCODE");
            return View();
        }

        //
        // POST: /District/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DISTRICT district)
        {
            if (ModelState.IsValid)
            {
                db.DISTRICTs.Add(district);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.COUNTRYSLNO = new SelectList(db.COUNTRies, "COUNTRYSLNO", "COUNTRYCODE", district.COUNTRYSLNO);
            return View(district);
        }

        //
        // GET: /District/Edit/5

        public ActionResult Edit(short id = 0)
        {
            DISTRICT district = db.DISTRICTs.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            ViewBag.COUNTRYSLNO = new SelectList(db.COUNTRies, "COUNTRYSLNO", "COUNTRYCODE", district.COUNTRYSLNO);
            return View(district);
        }

        //
        // POST: /District/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DISTRICT district)
        {
            if (ModelState.IsValid)
            {
                db.Entry(district).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.COUNTRYSLNO = new SelectList(db.COUNTRies, "COUNTRYSLNO", "COUNTRYCODE", district.COUNTRYSLNO);
            return View(district);
        }

        //
        // GET: /District/Delete/5

        public ActionResult Delete(short id = 0)
        {
            DISTRICT district = db.DISTRICTs.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            return View(district);
        }

        //
        // POST: /District/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            DISTRICT district = db.DISTRICTs.Find(id);
            db.DISTRICTs.Remove(district);
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
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
    public class OfficeController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /Office/

        public ActionResult Index()
        {
            var offices = db.OFFICEs.Include(o => o.COUNTRY).Include(o => o.DESIGNATION).Include(o => o.DISTRICT);
            return View(offices.ToList());
        }

        //
        // GET: /Office/Details/5

        public ActionResult Details(short id = 0)
        {
            OFFICE office = db.OFFICEs.Find(id);
            if (office == null)
            {
                return HttpNotFound();
            }
            return View(office);
        }

        //
        // GET: /Office/Create

        public ActionResult Create()
        {
            ViewBag.COUNTRYSLNO = new SelectList(db.COUNTRies, "COUNTRYSLNO", "COUNTRYCODE");
            ViewBag.DESSLNO = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESCODE");
            ViewBag.DISTRICTSLNO = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");
            return View();
        }

        //
        // POST: /Office/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OFFICE office)
        {
            if (ModelState.IsValid)
            {
                db.OFFICEs.Add(office);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.COUNTRYSLNO = new SelectList(db.COUNTRies, "COUNTRYSLNO", "COUNTRYCODE", office.COUNTRYSLNO);
            ViewBag.DESSLNO = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESCODE", office.DESSLNO);
            ViewBag.DISTRICTSLNO = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME", office.DISTRICTSLNO);
            return View(office);
        }

        //
        // GET: /Office/Edit/5

        public ActionResult Edit(short id = 0)
        {
            OFFICE office = db.OFFICEs.Find(id);
            if (office == null)
            {
                return HttpNotFound();
            }
            ViewBag.COUNTRYSLNO = new SelectList(db.COUNTRies, "COUNTRYSLNO", "COUNTRYCODE", office.COUNTRYSLNO);
            ViewBag.DESSLNO = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESCODE", office.DESSLNO);
            ViewBag.DISTRICTSLNO = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME", office.DISTRICTSLNO);
            return View(office);
        }

        //
        // POST: /Office/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OFFICE office)
        {
            if (ModelState.IsValid)
            {
                db.Entry(office).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.COUNTRYSLNO = new SelectList(db.COUNTRies, "COUNTRYSLNO", "COUNTRYCODE", office.COUNTRYSLNO);
            ViewBag.DESSLNO = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESCODE", office.DESSLNO);
            ViewBag.DISTRICTSLNO = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME", office.DISTRICTSLNO);
            return View(office);
        }

        //
        // GET: /Office/Delete/5

        public ActionResult Delete(short id = 0)
        {
            OFFICE office = db.OFFICEs.Find(id);
            if (office == null)
            {
                return HttpNotFound();
            }
            return View(office);
        }

        //
        // POST: /Office/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            OFFICE office = db.OFFICEs.Find(id);
            db.OFFICEs.Remove(office);
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
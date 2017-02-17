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
    public class BondCircleController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /BondCircle/

        public ActionResult Index()
        {
            var bondcircles = db.BONDCIRCLEs.Include(b => b.BONDDIVISION);
            return View(bondcircles.ToList());
        }

        //
        // GET: /BondCircle/Details/5

        public ActionResult Details(short id = 0)
        {
            BONDCIRCLE bondcircle = db.BONDCIRCLEs.Find(id);
            if (bondcircle == null)
            {
                return HttpNotFound();
            }
            return View(bondcircle);
        }

        //
        // GET: /BondCircle/Create

        public ActionResult Create()
        {
            ViewBag.BONDDIVISIONSLNO = new SelectList(db.BONDDIVISIONs, "BONDDIVISIONSLNO", "BONDDIVISIONNAME");
            return View();
        }

        //
        // POST: /BondCircle/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BONDCIRCLE bondcircle)
        {
            if (ModelState.IsValid)
            {
                db.BONDCIRCLEs.Add(bondcircle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BONDDIVISIONSLNO = new SelectList(db.BONDDIVISIONs, "BONDDIVISIONSLNO", "BONDDIVISIONNAME", bondcircle.BONDDIVISIONSLNO);
            return View(bondcircle);
        }

        //
        // GET: /BondCircle/Edit/5

        public ActionResult Edit(short id = 0)
        {
            BONDCIRCLE bondcircle = db.BONDCIRCLEs.Find(id);
            if (bondcircle == null)
            {
                return HttpNotFound();
            }
            ViewBag.BONDDIVISIONSLNO = new SelectList(db.BONDDIVISIONs, "BONDDIVISIONSLNO", "BONDDIVISIONNAME", bondcircle.BONDDIVISIONSLNO);
            return View(bondcircle);
        }

        //
        // POST: /BondCircle/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BONDCIRCLE bondcircle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bondcircle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BONDDIVISIONSLNO = new SelectList(db.BONDDIVISIONs, "BONDDIVISIONSLNO", "BONDDIVISIONNAME", bondcircle.BONDDIVISIONSLNO);
            return View(bondcircle);
        }

        //
        // GET: /BondCircle/Delete/5

        public ActionResult Delete(short id = 0)
        {
            BONDCIRCLE bondcircle = db.BONDCIRCLEs.Find(id);
            if (bondcircle == null)
            {
                return HttpNotFound();
            }
            return View(bondcircle);
        }

        //
        // POST: /BondCircle/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            BONDCIRCLE bondcircle = db.BONDCIRCLEs.Find(id);
            db.BONDCIRCLEs.Remove(bondcircle);
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
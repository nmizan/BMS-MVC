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
    public class BondDivisionController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /BondDivision/

        public ActionResult Index()
        {
            return View(db.BONDDIVISIONs.ToList());
        }

        //
        // GET: /BondDivision/Details/5

        public ActionResult Details(short id = 0)
        {
            BONDDIVISION bonddivision = db.BONDDIVISIONs.Find(id);
            if (bonddivision == null)
            {
                return HttpNotFound();
            }
            return View(bonddivision);
        }

        //
        // GET: /BondDivision/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /BondDivision/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BONDDIVISION bonddivision)
        {
            if (ModelState.IsValid)
            {
                db.BONDDIVISIONs.Add(bonddivision);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bonddivision);
        }

        //
        // GET: /BondDivision/Edit/5

        public ActionResult Edit(short id = 0)
        {
            BONDDIVISION bonddivision = db.BONDDIVISIONs.Find(id);
            if (bonddivision == null)
            {
                return HttpNotFound();
            }
            return View(bonddivision);
        }

        //
        // POST: /BondDivision/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BONDDIVISION bonddivision)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bonddivision).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bonddivision);
        }

        //
        // GET: /BondDivision/Delete/5

        public ActionResult Delete(short id = 0)
        {
            BONDDIVISION bonddivision = db.BONDDIVISIONs.Find(id);
            if (bonddivision == null)
            {
                return HttpNotFound();
            }
            return View(bonddivision);
        }

        //
        // POST: /BondDivision/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            BONDDIVISION bonddivision = db.BONDDIVISIONs.Find(id);
            db.BONDDIVISIONs.Remove(bonddivision);
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
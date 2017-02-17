using BMSPhase2Demo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSPhase2Demo.Controllers
{
    public class BonderLienBankController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /BonderLienBank/

        public ActionResult Index()
        {
            var bonderlienbanks = db.BONDERLIENBANKs.Include(b => b.BANK).Include(b => b.BANKBRANCH).Include(b => b.BONDER);
            return View(bonderlienbanks.ToList());
        }

        //
        // GET: /BonderLienBank/Details/5

        public ActionResult Details(short id = 0)
        {
            BONDERLIENBANK bonderlienbank = db.BONDERLIENBANKs.Find(id);
            if (bonderlienbank == null)
            {
                return HttpNotFound();
            }
            return View(bonderlienbank);
        }

        //
        // GET: /BonderLienBank/Create

        public ActionResult Create()
        {
            ViewBag.BANKSLNO = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME");
            ViewBag.BBRANCHSLNO = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME");
            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO");
            return View();
        }

        //
        // POST: /BonderLienBank/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BONDERLIENBANK bonderlienbank)
        {
            if (ModelState.IsValid)
            {
                db.BONDERLIENBANKs.Add(bonderlienbank);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BANKSLNO = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME", bonderlienbank.BANKSLNO);
            ViewBag.BBRANCHSLNO = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME", bonderlienbank.BBRANCHSLNO);
            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO", bonderlienbank.BONDERSLNO);
            return View(bonderlienbank);
        }

        //
        // GET: /BonderLienBank/Edit/5

        public ActionResult Edit(short id = 0)
        {
            BONDERLIENBANK bonderlienbank = db.BONDERLIENBANKs.Find(id);
            if (bonderlienbank == null)
            {
                return HttpNotFound();
            }
            ViewBag.BANKSLNO = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME", bonderlienbank.BANKSLNO);
            ViewBag.BBRANCHSLNO = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME", bonderlienbank.BBRANCHSLNO);
            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO", bonderlienbank.BONDERSLNO);
            return View(bonderlienbank);
        }

        //
        // POST: /BonderLienBank/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BONDERLIENBANK bonderlienbank)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bonderlienbank).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BANKSLNO = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME", bonderlienbank.BANKSLNO);
            ViewBag.BBRANCHSLNO = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME", bonderlienbank.BBRANCHSLNO);
            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO", bonderlienbank.BONDERSLNO);
            return View(bonderlienbank);
        }

        //
        // GET: /BonderLienBank/Delete/5

        public ActionResult Delete(short id = 0)
        {
            BONDERLIENBANK bonderlienbank = db.BONDERLIENBANKs.Find(id);
            if (bonderlienbank == null)
            {
                return HttpNotFound();
            }
            return View(bonderlienbank);
        }

        //
        // POST: /BonderLienBank/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            BONDERLIENBANK bonderlienbank = db.BONDERLIENBANKs.Find(id);
            db.BONDERLIENBANKs.Remove(bonderlienbank);
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
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
    public class BondStatusController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /BondStatus/

        public ActionResult Index()
        {
            var bondstatus = db.BONDSTATUS.Include(b => b.BONDER);
            return View(bondstatus.ToList());
        }

        //
        // GET: /BondStatus/Details/5

        public ActionResult Details(short id = 0)
        {
           // BONDSTATU bondstatu = db.BONDSTATUS.Find(id);
            BONDSTATU bondstatu = db.BONDSTATUS.SingleOrDefault(b => b.BSNO == id);
            if (bondstatu == null)
            {
                return HttpNotFound();
            }
            return View(bondstatu);
        }

        //
        // GET: /BondStatus/Create

        public ActionResult Create()
        {
            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO");
            return View();
        }

        //
        // POST: /BondStatus/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BONDSTATU bondstatu)
        {
            if (ModelState.IsValid)
            {
                db.BONDSTATUS.Add(bondstatu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO", bondstatu.BONDERSLNO);
            return View(bondstatu);
        }

        //
        // GET: /BondStatus/Edit/5

        public ActionResult Edit(short id = 0)
        {
            //BONDSTATU bondstatu = db.BONDSTATUS.Find(id);
            BONDSTATU bondstatu = db.BONDSTATUS.SingleOrDefault(b => b.BSNO == id);
            if (bondstatu == null)
            {
                return HttpNotFound();
            }
            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO", bondstatu.BONDERSLNO);
            return View(bondstatu);
        }

        //
        // POST: /BondStatus/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BONDSTATU bondstatu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bondstatu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO", bondstatu.BONDERSLNO);
            return View(bondstatu);
        }

        //
        // GET: /BondStatus/Delete/5

        public ActionResult Delete(short id = 0)
        {
            //BONDSTATU bondstatu = db.BONDSTATUS.Find(id);
            BONDSTATU bondstatu = db.BONDSTATUS.SingleOrDefault(b => b.BSNO == id);
            if (bondstatu == null)
            {
                return HttpNotFound();
            }
            return View(bondstatu);
        }

        //
        // POST: /BondStatus/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
           // BONDSTATU bondstatu = db.BONDSTATUS.Find(id);
            BONDSTATU bondstatu = db.BONDSTATUS.SingleOrDefault(b => b.BSNO == id);
            db.BONDSTATUS.Remove(bondstatu);
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
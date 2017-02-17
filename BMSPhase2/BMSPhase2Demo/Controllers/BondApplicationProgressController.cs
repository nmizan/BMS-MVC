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
    public class BondApplicationProgressController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /BondApplicationProgress/

        public ActionResult Index()
        {
            var bondapplicationprogresses = db.BONDAPPLICATIONPROGRESSes.Include(b => b.BONDER).Include(b => b.BONDSTATU);
            return View(bondapplicationprogresses.ToList());
        }

        //
        // GET: /BondApplicationProgress/Details/5

        public ActionResult Details(short id = 0)
        {
            BONDAPPLICATIONPROGRESS bondapplicationprogress = db.BONDAPPLICATIONPROGRESSes.Find(id);
            if (bondapplicationprogress == null)
            {
                return HttpNotFound();
            }
            return View(bondapplicationprogress);
        }

        //
        // GET: /BondApplicationProgress/Create

        public ActionResult Create()
        {
            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO");
            ViewBag.BSNO = new SelectList(db.BONDSTATUS, "BSNO", "REFNO");
            return View();
        }

        //
        // POST: /BondApplicationProgress/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BONDAPPLICATIONPROGRESS bondapplicationprogress)
        {
            if (ModelState.IsValid)
            {
                db.BONDAPPLICATIONPROGRESSes.Add(bondapplicationprogress);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO", bondapplicationprogress.BONDERSLNO);
            ViewBag.BSNO = new SelectList(db.BONDSTATUS, "BSNO", "REFNO", bondapplicationprogress.BSNO);
            return View(bondapplicationprogress);
        }

        //
        // GET: /BondApplicationProgress/Edit/5

        public ActionResult Edit(short id = 0)
        {
            BONDAPPLICATIONPROGRESS bondapplicationprogress = db.BONDAPPLICATIONPROGRESSes.Find(id);
            if (bondapplicationprogress == null)
            {
                return HttpNotFound();
            }
            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO", bondapplicationprogress.BONDERSLNO);
            ViewBag.BSNO = new SelectList(db.BONDSTATUS, "BSNO", "REFNO", bondapplicationprogress.BSNO);
            return View(bondapplicationprogress);
        }

        //
        // POST: /BondApplicationProgress/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BONDAPPLICATIONPROGRESS bondapplicationprogress)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bondapplicationprogress).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO", bondapplicationprogress.BONDERSLNO);
            ViewBag.BSNO = new SelectList(db.BONDSTATUS, "BSNO", "REFNO", bondapplicationprogress.BSNO);
            return View(bondapplicationprogress);
        }

        //
        // GET: /BondApplicationProgress/Delete/5

        public ActionResult Delete(short id = 0)
        {
            BONDAPPLICATIONPROGRESS bondapplicationprogress = db.BONDAPPLICATIONPROGRESSes.Find(id);
            if (bondapplicationprogress == null)
            {
                return HttpNotFound();
            }
            return View(bondapplicationprogress);
        }

        //
        // POST: /BondApplicationProgress/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            BONDAPPLICATIONPROGRESS bondapplicationprogress = db.BONDAPPLICATIONPROGRESSes.Find(id);
            db.BONDAPPLICATIONPROGRESSes.Remove(bondapplicationprogress);
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
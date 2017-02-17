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
    public class AssociationEnrollController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /AssociationEnroll/

        public ActionResult Index()
        {
            var associationenrolls = db.ASSOCIATIONENROLLs.Include(a => a.BONDER).Include(a => a.OFFICE);
            return View(associationenrolls.ToList());
        }

        //
        // GET: /AssociationEnroll/Details/5

        public ActionResult Details(short id = 0)
        {
            //ASSOCIATIONENROLL associationenroll = db.ASSOCIATIONENROLLs.Find(id);
            ASSOCIATIONENROLL associationenroll = db.ASSOCIATIONENROLLs.SingleOrDefault(x => x.ASSOENSLNO == id);
         
            if (associationenroll == null)
            {
                return HttpNotFound();
            }
            return View(associationenroll);
        }

        //
        // GET: /AssociationEnroll/Create

        public ActionResult Create()
        {
            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO");
            ViewBag.OFFICESLNO = new SelectList(db.OFFICEs, "OFFICESLNO", "OFFICENAME");
            return View();
        }

        //
        // POST: /AssociationEnroll/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ASSOCIATIONENROLL associationenroll)
        {
            if (ModelState.IsValid)
            {
                db.ASSOCIATIONENROLLs.Add(associationenroll);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO", associationenroll.BONDERSLNO);
            ViewBag.OFFICESLNO = new SelectList(db.OFFICEs, "OFFICESLNO", "OFFICENAME", associationenroll.OFFICESLNO);
            return View(associationenroll);
        }

        //
        // GET: /AssociationEnroll/Edit/5

        public ActionResult Edit(short id = 0)
        {
            //ASSOCIATIONENROLL associationenroll = db.ASSOCIATIONENROLLs.Find(id);
            ASSOCIATIONENROLL associationenroll = db.ASSOCIATIONENROLLs.SingleOrDefault(x => x.ASSOENSLNO == id);
            if (associationenroll == null)
            {
                return HttpNotFound();
            }
            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO", associationenroll.BONDERSLNO);
            ViewBag.OFFICESLNO = new SelectList(db.OFFICEs, "OFFICESLNO", "OFFICENAME", associationenroll.OFFICESLNO);
            return View(associationenroll);
        }

        //
        // POST: /AssociationEnroll/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ASSOCIATIONENROLL associationenroll)
        {
            if (ModelState.IsValid)
            {
                db.Entry(associationenroll).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BONDERSLNO = new SelectList(db.BONDERs, "BONDERSLNO", "BONDLICENSENO", associationenroll.BONDERSLNO);
            ViewBag.OFFICESLNO = new SelectList(db.OFFICEs, "OFFICESLNO", "OFFICENAME", associationenroll.OFFICESLNO);
            return View(associationenroll);
        }

        //
        // GET: /AssociationEnroll/Delete/5

        public ActionResult Delete(short id = 0)
        {
            //ASSOCIATIONENROLL associationenroll = db.ASSOCIATIONENROLLs.Find(id);
            ASSOCIATIONENROLL associationenroll = db.ASSOCIATIONENROLLs.SingleOrDefault(x => x.ASSOENSLNO == id);
            if (associationenroll == null)
            {
                return HttpNotFound();
            }
            return View(associationenroll);
        }

        //
        // POST: /AssociationEnroll/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            //ASSOCIATIONENROLL associationenroll = db.ASSOCIATIONENROLLs.Find(id);
            ASSOCIATIONENROLL associationenroll = db.ASSOCIATIONENROLLs.SingleOrDefault(x => x.ASSOENSLNO == id);
            db.ASSOCIATIONENROLLs.Remove(associationenroll);
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
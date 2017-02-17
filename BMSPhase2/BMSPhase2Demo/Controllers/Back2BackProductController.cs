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
    public class Back2BackProductController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /Back2BackProduct/

        public ActionResult Index()
        {
            var backtobackproducts = db.BACKTOBACKPRODUCTs.Include(b => b.BACKTOBACKLC);
            return View(backtobackproducts.ToList());
        }

        //
        // GET: /Back2BackProduct/Details/5

        public ActionResult Details(short id = 0)
        {
            BACKTOBACKPRODUCT backtobackproduct = db.BACKTOBACKPRODUCTs.Find(id);
            if (backtobackproduct == null)
            {
                return HttpNotFound();
            }
            return View(backtobackproduct);
        }

        //
        // GET: /Back2BackProduct/Create

        public ActionResult Create()
        {
            ViewBag.BACKTOBACKLCID = new SelectList(db.BACKTOBACKLCs, "ID", "BUYERSNAME");
            return View();
        }

        //
        // POST: /Back2BackProduct/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BACKTOBACKPRODUCT backtobackproduct)
        {
            if (ModelState.IsValid)
            {
                db.BACKTOBACKPRODUCTs.Add(backtobackproduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BACKTOBACKLCID = new SelectList(db.BACKTOBACKLCs, "ID", "BUYERSNAME", backtobackproduct.BACKTOBACKLCID);
            return View(backtobackproduct);
        }

        //
        // GET: /Back2BackProduct/Edit/5

        public ActionResult Edit(short id = 0)
        {
            BACKTOBACKPRODUCT backtobackproduct = db.BACKTOBACKPRODUCTs.Find(id);
            if (backtobackproduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.BACKTOBACKLCID = new SelectList(db.BACKTOBACKLCs, "ID", "BUYERSNAME", backtobackproduct.BACKTOBACKLCID);
            return View(backtobackproduct);
        }

        //
        // POST: /Back2BackProduct/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BACKTOBACKPRODUCT backtobackproduct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(backtobackproduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BACKTOBACKLCID = new SelectList(db.BACKTOBACKLCs, "ID", "BUYERSNAME", backtobackproduct.BACKTOBACKLCID);
            return View(backtobackproduct);
        }

        //
        // GET: /Back2BackProduct/Delete/5

        public ActionResult Delete(short id = 0)
        {
            BACKTOBACKPRODUCT backtobackproduct = db.BACKTOBACKPRODUCTs.Find(id);
            if (backtobackproduct == null)
            {
                return HttpNotFound();
            }
            return View(backtobackproduct);
        }

        //
        // POST: /Back2BackProduct/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            BACKTOBACKPRODUCT backtobackproduct = db.BACKTOBACKPRODUCTs.Find(id);
            db.BACKTOBACKPRODUCTs.Remove(backtobackproduct);
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
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
    public class BonderProductController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /BonderProduct/

        public ActionResult Index()
        {
            var bonderproducts = db.BONDERPRODUCTs.Include(b => b.MATERIAL);
            return View(bonderproducts.ToList());
        }

        //
        // GET: /BonderProduct/Details/5

        public ActionResult Details(int id = 0)
        {
            BONDERPRODUCT bonderproduct = db.BONDERPRODUCTs.Find(id);
            if (bonderproduct == null)
            {
                return HttpNotFound();
            }
            return View(bonderproduct);
        }

        //
        // GET: /BonderProduct/Create

        public ActionResult Create()
        {
            ViewBag.FPSLNO = new SelectList(db.MATERIALS, "MSLNO", "MHSCODE");
            return View();
        }

        //
        // POST: /BonderProduct/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BONDERPRODUCT bonderproduct)
        {
            if (ModelState.IsValid)
            {
                db.BONDERPRODUCTs.Add(bonderproduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FPSLNO = new SelectList(db.MATERIALS, "MSLNO", "MHSCODE", bonderproduct.FPSLNO);
            return View(bonderproduct);
        }

        //
        // GET: /BonderProduct/Edit/5

        public ActionResult Edit(int id = 0)
        {
            BONDERPRODUCT bonderproduct = db.BONDERPRODUCTs.Find(id);
            if (bonderproduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.FPSLNO = new SelectList(db.MATERIALS, "MSLNO", "MHSCODE", bonderproduct.FPSLNO);
            return View(bonderproduct);
        }

        //
        // POST: /BonderProduct/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BONDERPRODUCT bonderproduct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bonderproduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FPSLNO = new SelectList(db.MATERIALS, "MSLNO", "MHSCODE", bonderproduct.FPSLNO);
            return View(bonderproduct);
        }

        //
        // GET: /BonderProduct/Delete/5

        public ActionResult Delete(int id = 0)
        {
            BONDERPRODUCT bonderproduct = db.BONDERPRODUCTs.Find(id);
            if (bonderproduct == null)
            {
                return HttpNotFound();
            }
            return View(bonderproduct);
        }

        //
        // POST: /BonderProduct/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BONDERPRODUCT bonderproduct = db.BONDERPRODUCTs.Find(id);
            db.BONDERPRODUCTs.Remove(bonderproduct);
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
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
    public class BondCategoryController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /BondCategory/

        public ActionResult Index()
        {
            return View(db.BONDCATEGORies.ToList());
        }

        //
        // GET: /BondCategory/Details/5

        public ActionResult Details(short id = 0)
        {
            BONDCATEGORY bondcategory = db.BONDCATEGORies.Find(id);
            if (bondcategory == null)
            {
                return HttpNotFound();
            }
            return View(bondcategory);
        }

        //
        // GET: /BondCategory/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /BondCategory/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BONDCATEGORY bondcategory)
        {
            if (ModelState.IsValid)
            {
                db.BONDCATEGORies.Add(bondcategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bondcategory);
        }

        //
        // GET: /BondCategory/Edit/5

        public ActionResult Edit(short id = 0)
        {
            BONDCATEGORY bondcategory = db.BONDCATEGORies.Find(id);
            if (bondcategory == null)
            {
                return HttpNotFound();
            }
            return View(bondcategory);
        }

        //
        // POST: /BondCategory/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BONDCATEGORY bondcategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bondcategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bondcategory);
        }

        //
        // GET: /BondCategory/Delete/5

        public ActionResult Delete(short id = 0)
        {
            BONDCATEGORY bondcategory = db.BONDCATEGORies.Find(id);
            if (bondcategory == null)
            {
                return HttpNotFound();
            }
            return View(bondcategory);
        }

        //
        // POST: /BondCategory/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            BONDCATEGORY bondcategory = db.BONDCATEGORies.Find(id);
            db.BONDCATEGORies.Remove(bondcategory);
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
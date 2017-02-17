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
    public class BondSubCategoryController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /BondSubCategory/

        public ActionResult Index()
        {
            return View(db.BONDSUBCATEGORies.ToList());
        }

        //
        // GET: /BondSubCategory/Details/5

        public ActionResult Details(short id = 0)
        {
            BONDSUBCATEGORY bondsubcategory = db.BONDSUBCATEGORies.Find(id);
            if (bondsubcategory == null)
            {
                return HttpNotFound();
            }
            return View(bondsubcategory);
        }

        //
        // GET: /BondSubCategory/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /BondSubCategory/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BONDSUBCATEGORY bondsubcategory)
        {
            if (ModelState.IsValid)
            {
                db.BONDSUBCATEGORies.Add(bondsubcategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bondsubcategory);
        }

        //
        // GET: /BondSubCategory/Edit/5

        public ActionResult Edit(short id = 0)
        {
            BONDSUBCATEGORY bondsubcategory = db.BONDSUBCATEGORies.Find(id);
            if (bondsubcategory == null)
            {
                return HttpNotFound();
            }
            return View(bondsubcategory);
        }

        //
        // POST: /BondSubCategory/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BONDSUBCATEGORY bondsubcategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bondsubcategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bondsubcategory);
        }

        //
        // GET: /BondSubCategory/Delete/5

        public ActionResult Delete(short id = 0)
        {
            BONDSUBCATEGORY bondsubcategory = db.BONDSUBCATEGORies.Find(id);
            if (bondsubcategory == null)
            {
                return HttpNotFound();
            }
            return View(bondsubcategory);
        }

        //
        // POST: /BondSubCategory/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            BONDSUBCATEGORY bondsubcategory = db.BONDSUBCATEGORies.Find(id);
            db.BONDSUBCATEGORies.Remove(bondsubcategory);
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
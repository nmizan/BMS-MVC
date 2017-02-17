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
    public class Back2BackRawMaterialController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /Back2BackRawMaterial/

        public ActionResult Index()
        {
            var rawmaterials = db.RAWMATERIALs.Include(r => r.BACKTOBACKPRODUCT);
            return View(rawmaterials.ToList());
        }

        //
        // GET: /Back2BackRawMaterial/Details/5

        public ActionResult Details(short id = 0)
        {
            RAWMATERIAL rawmaterial = db.RAWMATERIALs.Find(id);
            if (rawmaterial == null)
            {
                return HttpNotFound();
            }
            return View(rawmaterial);
        }

        //
        // GET: /Back2BackRawMaterial/Create

        public ActionResult Create()
        {
            ViewBag.PRODUCTID = new SelectList(db.BACKTOBACKPRODUCTs, "ID", "NAME");
            return View();
        }

        //
        // POST: /Back2BackRawMaterial/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RAWMATERIAL rawmaterial)
        {
            if (ModelState.IsValid)
            {
                db.RAWMATERIALs.Add(rawmaterial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PRODUCTID = new SelectList(db.BACKTOBACKPRODUCTs, "ID", "NAME", rawmaterial.PRODUCTID);
            return View(rawmaterial);
        }

        //
        // GET: /Back2BackRawMaterial/Edit/5

        public ActionResult Edit(short id = 0)
        {
            RAWMATERIAL rawmaterial = db.RAWMATERIALs.Find(id);
            if (rawmaterial == null)
            {
                return HttpNotFound();
            }
            ViewBag.PRODUCTID = new SelectList(db.BACKTOBACKPRODUCTs, "ID", "NAME", rawmaterial.PRODUCTID);
            return View(rawmaterial);
        }

        //
        // POST: /Back2BackRawMaterial/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RAWMATERIAL rawmaterial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rawmaterial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PRODUCTID = new SelectList(db.BACKTOBACKPRODUCTs, "ID", "NAME", rawmaterial.PRODUCTID);
            return View(rawmaterial);
        }

        //
        // GET: /Back2BackRawMaterial/Delete/5

        public ActionResult Delete(short id = 0)
        {
            RAWMATERIAL rawmaterial = db.RAWMATERIALs.Find(id);
            if (rawmaterial == null)
            {
                return HttpNotFound();
            }
            return View(rawmaterial);
        }

        //
        // POST: /Back2BackRawMaterial/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            RAWMATERIAL rawmaterial = db.RAWMATERIALs.Find(id);
            db.RAWMATERIALs.Remove(rawmaterial);
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
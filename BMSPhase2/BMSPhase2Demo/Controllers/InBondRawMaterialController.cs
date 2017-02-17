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
    public class InBondRawMaterialController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /InBondRawMaterial/

        public ActionResult Index()
        {
            var inbondrawmaterials = db.INBONDRAWMATERIALs.Include(i => i.INBOND);
            return View(inbondrawmaterials.ToList());
        }

        //
        // GET: /InBondRawMaterial/Details/5

        public ActionResult Details(short id = 0)
        {
            INBONDRAWMATERIAL inbondrawmaterial = db.INBONDRAWMATERIALs.Find(id);
            if (inbondrawmaterial == null)
            {
                return HttpNotFound();
            }
            return View(inbondrawmaterial);
        }

        //
        // GET: /InBondRawMaterial/Create

        public ActionResult Create()
        {
            ViewBag.INBONDID = new SelectList(db.INBONDs, "ID", "ID");

            var initialData = new[] {
            new INBONDRAWMATERIAL { ID =1, RAWMATERIALCODE = 1 }
           
             };
            return View(initialData);
            //return View();
        }

        //
        // POST: /InBondRawMaterial/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(INBONDRAWMATERIAL inbondrawmaterial)
        {
            if (ModelState.IsValid)
            {
                db.INBONDRAWMATERIALs.Add(inbondrawmaterial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.INBONDID = new SelectList(db.INBONDs, "ID", "ID", inbondrawmaterial.INBONDID);
            return View(inbondrawmaterial);
        }

        //
        // GET: /InBondRawMaterial/Edit/5

        public ActionResult Edit(short id = 0)
        {
            INBONDRAWMATERIAL inbondrawmaterial = db.INBONDRAWMATERIALs.Find(id);
            if (inbondrawmaterial == null)
            {
                return HttpNotFound();
            }
            ViewBag.INBONDID = new SelectList(db.INBONDs, "ID", "ID", inbondrawmaterial.INBONDID);
            return View(inbondrawmaterial);
        }

        //
        // POST: /InBondRawMaterial/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(INBONDRAWMATERIAL inbondrawmaterial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inbondrawmaterial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.INBONDID = new SelectList(db.INBONDs, "ID", "ID", inbondrawmaterial.INBONDID);
            return View(inbondrawmaterial);
        }

        //
        // GET: /InBondRawMaterial/Delete/5

        public ActionResult Delete(short id = 0)
        {
            INBONDRAWMATERIAL inbondrawmaterial = db.INBONDRAWMATERIALs.Find(id);
            if (inbondrawmaterial == null)
            {
                return HttpNotFound();
            }
            return View(inbondrawmaterial);
        }

        //
        // POST: /InBondRawMaterial/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            INBONDRAWMATERIAL inbondrawmaterial = db.INBONDRAWMATERIALs.Find(id);
            db.INBONDRAWMATERIALs.Remove(inbondrawmaterial);
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
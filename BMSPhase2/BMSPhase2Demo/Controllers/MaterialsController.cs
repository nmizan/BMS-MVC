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
    public class MaterialsController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /Materials/

        public ActionResult Index()
        {
            var materials = db.MATERIALS.Include(m => m.MATERIALSGROUP).Include(m => m.MEASUREMENTUNIT);
            return View(materials.ToList());
        }

        //
        // GET: /Materials/Details/5

        public ActionResult Details(short id = 0)
        {
            MATERIAL material = db.MATERIALS.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        //
        // GET: /Materials/Create

        public ActionResult Create()
        {
            ViewBag.MGSLNO = new SelectList(db.MATERIALSGROUPs, "MGSLNO", "MGROUPCODE");
            ViewBag.MUSLNO = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
            return View();
        }

        //
        // POST: /Materials/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MATERIAL material)
        {
            if (ModelState.IsValid)
            {
                db.MATERIALS.Add(material);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MGSLNO = new SelectList(db.MATERIALSGROUPs, "MGSLNO", "MGROUPCODE", material.MGSLNO);
            ViewBag.MUSLNO = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", material.MUSLNO);
            return View(material);
        }

        //
        // GET: /Materials/Edit/5

        public ActionResult Edit(short id = 0)
        {
            MATERIAL material = db.MATERIALS.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            ViewBag.MGSLNO = new SelectList(db.MATERIALSGROUPs, "MGSLNO", "MGROUPCODE", material.MGSLNO);
            ViewBag.MUSLNO = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", material.MUSLNO);
            return View(material);
        }

        //
        // POST: /Materials/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MATERIAL material)
        {
            if (ModelState.IsValid)
            {
                db.Entry(material).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MGSLNO = new SelectList(db.MATERIALSGROUPs, "MGSLNO", "MGROUPCODE", material.MGSLNO);
            ViewBag.MUSLNO = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", material.MUSLNO);
            return View(material);
        }

        //
        // GET: /Materials/Delete/5

        public ActionResult Delete(short id = 0)
        {
            MATERIAL material = db.MATERIALS.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        //
        // POST: /Materials/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            MATERIAL material = db.MATERIALS.Find(id);
            db.MATERIALS.Remove(material);
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
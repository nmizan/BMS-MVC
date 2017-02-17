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
    public class MaterialGroupController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /MaterialGroup/

        public ActionResult Index()
        {
            return View(db.MATERIALSGROUPs.ToList());
        }

        //
        // GET: /MaterialGroup/Details/5

        public ActionResult Details(short id = 0)
        {
            MATERIALSGROUP materialsgroup = db.MATERIALSGROUPs.Find(id);
            if (materialsgroup == null)
            {
                return HttpNotFound();
            }
            return View(materialsgroup);
        }

        //
        // GET: /MaterialGroup/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /MaterialGroup/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MATERIALSGROUP materialsgroup)
        {
            if (ModelState.IsValid)
            {
                db.MATERIALSGROUPs.Add(materialsgroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(materialsgroup);
        }

        //
        // GET: /MaterialGroup/Edit/5

        public ActionResult Edit(short id = 0)
        {
            MATERIALSGROUP materialsgroup = db.MATERIALSGROUPs.Find(id);
            if (materialsgroup == null)
            {
                return HttpNotFound();
            }
            return View(materialsgroup);
        }

        //
        // POST: /MaterialGroup/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MATERIALSGROUP materialsgroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(materialsgroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(materialsgroup);
        }

        //
        // GET: /MaterialGroup/Delete/5

        public ActionResult Delete(short id = 0)
        {
            MATERIALSGROUP materialsgroup = db.MATERIALSGROUPs.Find(id);
            if (materialsgroup == null)
            {
                return HttpNotFound();
            }
            return View(materialsgroup);
        }

        //
        // POST: /MaterialGroup/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            MATERIALSGROUP materialsgroup = db.MATERIALSGROUPs.Find(id);
            db.MATERIALSGROUPs.Remove(materialsgroup);
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
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
    public class FinishedProductController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /FinishedProduct/

        public ActionResult Index()
        {
            return View(db.FINISHEDPRODUCTS.ToList());
        }

        //
        // GET: /FinishedProduct/Details/5

        public ActionResult Details(short id = 0)
        {
            FINISHEDPRODUCT finishedproduct = db.FINISHEDPRODUCTS.Find(id);
            if (finishedproduct == null)
            {
                return HttpNotFound();
            }
            return View(finishedproduct);
        }

        //
        // GET: /FinishedProduct/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /FinishedProduct/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FINISHEDPRODUCT finishedproduct)
        {
            if (ModelState.IsValid)
            {
                db.FINISHEDPRODUCTS.Add(finishedproduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(finishedproduct);
        }

        //
        // GET: /FinishedProduct/Edit/5

        public ActionResult Edit(short id = 0)
        {
            FINISHEDPRODUCT finishedproduct = db.FINISHEDPRODUCTS.Find(id);
            if (finishedproduct == null)
            {
                return HttpNotFound();
            }
            return View(finishedproduct);
        }

        //
        // POST: /FinishedProduct/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FINISHEDPRODUCT finishedproduct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(finishedproduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(finishedproduct);
        }

        //
        // GET: /FinishedProduct/Delete/5

        public ActionResult Delete(short id = 0)
        {
            FINISHEDPRODUCT finishedproduct = db.FINISHEDPRODUCTS.Find(id);
            if (finishedproduct == null)
            {
                return HttpNotFound();
            }
            return View(finishedproduct);
        }

        //
        // POST: /FinishedProduct/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            FINISHEDPRODUCT finishedproduct = db.FINISHEDPRODUCTS.Find(id);
            db.FINISHEDPRODUCTS.Remove(finishedproduct);
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
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
    public class CurrencyController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /Currency/

        public ActionResult Index()
        {
            return View(db.CURRENCies.ToList());
        }

        //
        // GET: /Currency/Details/5

        public ActionResult Details(short id = 0)
        {
            CURRENCY currency = db.CURRENCies.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        //
        // GET: /Currency/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Currency/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CURRENCY currency)
        {
            if (ModelState.IsValid)
            {
                db.CURRENCies.Add(currency);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(currency);
        }

        //
        // GET: /Currency/Edit/5

        public ActionResult Edit(short id = 0)
        {
            CURRENCY currency = db.CURRENCies.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        //
        // POST: /Currency/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CURRENCY currency)
        {
            if (ModelState.IsValid)
            {
                db.Entry(currency).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(currency);
        }

        //
        // GET: /Currency/Delete/5

        public ActionResult Delete(short id = 0)
        {
            CURRENCY currency = db.CURRENCies.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        //
        // POST: /Currency/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            CURRENCY currency = db.CURRENCies.Find(id);
            db.CURRENCies.Remove(currency);
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
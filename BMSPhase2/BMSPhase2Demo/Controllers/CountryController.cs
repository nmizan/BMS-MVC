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
    public class CountryController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /Country/

        public ActionResult Index()
        {
            return View(db.COUNTRies.ToList());
        }

        //
        // GET: /Country/Details/5

        public ActionResult Details(short id = 0)
        {
            COUNTRY country = db.COUNTRies.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        //
        // GET: /Country/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Country/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(COUNTRY country)
        {
            if (ModelState.IsValid)
            {
                db.COUNTRies.Add(country);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(country);
        }

        //
        // GET: /Country/Edit/5

        public ActionResult Edit(short id = 0)
        {
            COUNTRY country = db.COUNTRies.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        //
        // POST: /Country/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(COUNTRY country)
        {
            if (ModelState.IsValid)
            {
                db.Entry(country).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(country);
        }

        //
        // GET: /Country/Delete/5

        public ActionResult Delete(short id = 0)
        {
            COUNTRY country = db.COUNTRies.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        //
        // POST: /Country/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            COUNTRY country = db.COUNTRies.Find(id);
            db.COUNTRies.Remove(country);
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
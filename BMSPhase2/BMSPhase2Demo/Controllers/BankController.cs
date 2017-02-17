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
    public class BankController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /Bank/

        public ActionResult Index()
        {
            return View(db.BANKs.ToList());
        }

        //
        // GET: /Bank/Details/5

        public ActionResult Details(short id = 0)
        {
            BANK bank = db.BANKs.Find(id);
            if (bank == null)
            {
                return HttpNotFound();
            }
            return View(bank);
        }

        //
        // GET: /Bank/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Bank/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BANK bank)
        {
            if (ModelState.IsValid)
            {
                db.BANKs.Add(bank);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bank);
        }

        //
        // GET: /Bank/Edit/5

        public ActionResult Edit(short id = 0)
        {
            BANK bank = db.BANKs.Find(id);
            if (bank == null)
            {
                return HttpNotFound();
            }
            return View(bank);
        }

        //
        // POST: /Bank/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BANK bank)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bank).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bank);
        }

        //
        // GET: /Bank/Delete/5

        public ActionResult Delete(short id = 0)
        {
            BANK bank = db.BANKs.Find(id);
            if (bank == null)
            {
                return HttpNotFound();
            }
            return View(bank);
        }

        //
        // POST: /Bank/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            BANK bank = db.BANKs.Find(id);
            db.BANKs.Remove(bank);
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
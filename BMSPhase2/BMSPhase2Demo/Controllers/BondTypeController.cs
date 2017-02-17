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
    public class BondTypeController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /BondType/

        public ActionResult Index()
        {
            return View(db.BONDTYPEs.ToList());
        }

        //
        // GET: /BondType/Details/5

        public ActionResult Details(short id = 0)
        {
            BONDTYPE bondtype = db.BONDTYPEs.Find(id);
            if (bondtype == null)
            {
                return HttpNotFound();
            }
            return View(bondtype);
        }

        //
        // GET: /BondType/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /BondType/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BONDTYPE bondtype)
        {
            if (ModelState.IsValid)
            {
                db.BONDTYPEs.Add(bondtype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bondtype);
        }

        //
        // GET: /BondType/Edit/5

        public ActionResult Edit(short id = 0)
        {
            BONDTYPE bondtype = db.BONDTYPEs.Find(id);
            if (bondtype == null)
            {
                return HttpNotFound();
            }
            return View(bondtype);
        }

        //
        // POST: /BondType/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BONDTYPE bondtype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bondtype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bondtype);
        }

        //
        // GET: /BondType/Delete/5

        public ActionResult Delete(short id = 0)
        {
            BONDTYPE bondtype = db.BONDTYPEs.Find(id);
            if (bondtype == null)
            {
                return HttpNotFound();
            }
            return View(bondtype);
        }

        //
        // POST: /BondType/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            BONDTYPE bondtype = db.BONDTYPEs.Find(id);
            db.BONDTYPEs.Remove(bondtype);
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
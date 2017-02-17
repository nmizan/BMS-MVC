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
    public class BankBranchController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /BankBranch/

        public ActionResult Index()
        {
            var bankbranches = db.BANKBRANCHes.Include(b => b.BANK);
            return View(bankbranches.ToList());
        }

        //
        // GET: /BankBranch/Details/5

        public ActionResult Details(short id = 0)
        {
            BANKBRANCH bankbranch = db.BANKBRANCHes.Find(id);
            if (bankbranch == null)
            {
                return HttpNotFound();
            }
            return View(bankbranch);
        }

        //
        // GET: /BankBranch/Create

        public ActionResult Create()
        {
            ViewBag.BANKSLNO = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME");
            return View();
        }

        //
        // POST: /BankBranch/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BANKBRANCH bankbranch)
        {
            if (ModelState.IsValid)
            {
                db.BANKBRANCHes.Add(bankbranch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BANKSLNO = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME", bankbranch.BANKSLNO);
            return View(bankbranch);
        }

        //
        // GET: /BankBranch/Edit/5

        public ActionResult Edit(short id = 0)
        {
            BANKBRANCH bankbranch = db.BANKBRANCHes.Find(id);
            if (bankbranch == null)
            {
                return HttpNotFound();
            }
            ViewBag.BANKSLNO = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME", bankbranch.BANKSLNO);
            return View(bankbranch);
        }

        //
        // POST: /BankBranch/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BANKBRANCH bankbranch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankbranch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BANKSLNO = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME", bankbranch.BANKSLNO);
            return View(bankbranch);
        }

        //
        // GET: /BankBranch/Delete/5

        public ActionResult Delete(short id = 0)
        {
            BANKBRANCH bankbranch = db.BANKBRANCHes.Find(id);
            if (bankbranch == null)
            {
                return HttpNotFound();
            }
            return View(bankbranch);
        }

        //
        // POST: /BankBranch/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            BANKBRANCH bankbranch = db.BANKBRANCHes.Find(id);
            db.BANKBRANCHes.Remove(bankbranch);
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
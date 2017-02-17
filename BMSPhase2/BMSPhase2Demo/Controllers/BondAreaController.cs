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
    public class BondAreaController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /BondArea/

        public ActionResult Index()
        {
            return View(db.BONDAREAs.ToList());
            //var bondareas = db.BONDAREAs.Include(b=>b.BONDDIVISION);
           

            //var bondareas = from s in db.BONDAREAs 
            //                join a in db.BONDDIVISIONs 
            //                on s.BONDDIVISIONSLNO equals (a.BONDDIVISIONSLNO ) 
            //                select new 
            //                            {
            //                                AreaSlNo = s.BONDAREASLNO ,
            //                                Division = s.BONDDIVISION.BONDDIVISIONNAME ,
            //                                AreaName = s.BONDAREAENAME,
                                           
            //                            };
            //return View(bondareas.ToList());
        }

        //
        // GET: /BondArea/Details/5

        public ActionResult Details(short id = 0)
        {
            BONDAREA bondarea = db.BONDAREAs.Find(id);
            if (bondarea == null)
            {
                return HttpNotFound();
            }
            return View(bondarea);
        }

        //
        // GET: /BondArea/Create

        public ActionResult Create()
        {
            ViewBag.BONDDIVISIONSLNO = new SelectList(db.BONDDIVISIONs, "BONDDIVISIONSLNO", "BONDDIVISIONNAME");
            return View();
        }

        //
        // POST: /BondArea/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BONDAREA bondarea)
        {
            if (ModelState.IsValid)
            {
                db.BONDAREAs.Add(bondarea);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BONDDIVISIONSLNO = new SelectList(db.BONDDIVISIONs, "BONDDIVISIONSLNO", "BONDDIVISIONNAME", bondarea.BONDDIVISIONSLNO);
            return View(bondarea);
        }

        //
        // GET: /BondArea/Edit/5

        public ActionResult Edit(short id = 0)
        {
            BONDAREA bondarea = db.BONDAREAs.Find(id);
            if (bondarea == null)
            {
                return HttpNotFound();
            }
            ViewBag.BONDDIVISIONSLNO = new SelectList(db.BONDDIVISIONs, "BONDDIVISIONSLNO", "BONDDIVISIONNAME", bondarea.BONDDIVISIONSLNO);
            return View(bondarea);
        }

        //
        // POST: /BondArea/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BONDAREA bondarea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bondarea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BONDDIVISIONSLNO = new SelectList(db.BONDDIVISIONs, "BONDDIVISIONSLNO", "BONDDIVISIONNAME", bondarea.BONDDIVISIONSLNO);
            return View(bondarea);
        }

        //
        // GET: /BondArea/Delete/5

        public ActionResult Delete(short id = 0)
        {
            BONDAREA bondarea = db.BONDAREAs.Find(id);
            if (bondarea == null)
            {
                return HttpNotFound();
            }
            return View(bondarea);
        }

        //
        // POST: /BondArea/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            BONDAREA bondarea = db.BONDAREAs.Find(id);
            db.BONDAREAs.Remove(bondarea);
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
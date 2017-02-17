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
    public class AttachmentController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //
        // GET: /Attachment/

        public ActionResult Index()
        {
            var attachments = db.ATTACHMENTs.Include(a => a.UPREQUEST);
            return View(attachments.ToList());
        }

        //
        // GET: /Attachment/Details/5

        public ActionResult Details(short id = 0)
        {
            ATTACHMENT attachment = db.ATTACHMENTs.Find(id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            return View(attachment);
        }

        //
        // GET: /Attachment/Create

        public ActionResult Create()
        {
            ViewBag.UPREQUESTID = new SelectList(db.UPREQUESTs, "ID", "CREATEDBY");
            return View();
        }

        //
        // POST: /Attachment/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ATTACHMENT attachment)
        {
            if (ModelState.IsValid)
            {
                db.ATTACHMENTs.Add(attachment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UPREQUESTID = new SelectList(db.UPREQUESTs, "ID", "CREATEDBY", attachment.UPREQUESTID);
            return View(attachment);
        }

        //
        // GET: /Attachment/Edit/5

        public ActionResult Edit(short id = 0)
        {
            ATTACHMENT attachment = db.ATTACHMENTs.Find(id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            ViewBag.UPREQUESTID = new SelectList(db.UPREQUESTs, "ID", "CREATEDBY", attachment.UPREQUESTID);
            return View(attachment);
        }

        //
        // POST: /Attachment/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ATTACHMENT attachment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attachment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UPREQUESTID = new SelectList(db.UPREQUESTs, "ID", "CREATEDBY", attachment.UPREQUESTID);
            return View(attachment);
        }

        //
        // GET: /Attachment/Delete/5

        public ActionResult Delete(short id = 0)
        {
            ATTACHMENT attachment = db.ATTACHMENTs.Find(id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            return View(attachment);
        }

        //
        // POST: /Attachment/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            ATTACHMENT attachment = db.ATTACHMENTs.Find(id);
            db.ATTACHMENTs.Remove(attachment);
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
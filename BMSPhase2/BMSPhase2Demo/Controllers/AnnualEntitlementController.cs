using BMSPhase2Demo.Models;
using BMSPhase2Demo.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using BMSPhase2Demo.Utils;
namespace BMSPhase2Demo.Controllers
{
    public class AnnualEntitlementController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();
        private int recordNumbers = GlobalConstants.recordNumbers;
        //
        // GET: /AnnualEntitlement/
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Index(int? page, int filterBonderId = 0, int BONDERID = 0)
        {
            int pageSize = recordNumbers;
            int pageNumber = (page ?? 1);
            var annualentlrawmaterials = db.ANNUALENTLRAWMATERIALs.Include(a => a.MATERIAL).Include(a => a.MEASUREMENTUNIT);
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            if (BONDERID == 0 && filterBonderId == 0 && !User.IsInRole("Bonder"))
            {
                List <ANNUALENTLRAWMATERIAL> aerawmaterials = new List<ANNUALENTLRAWMATERIAL>();
                ViewBag.resultofbonderID = BONDERID;
                return View(aerawmaterials.ToList().ToPagedList(pageNumber, pageSize));
            }
           
            USERPERMISSION loggedinUser = new SessionAttributeRetreival().getStoredUserPermission();
            if (BONDERID != 0)
            {
                page = 1;
            }
            else
            {
                if (BONDERID == 0)
                {
                    BONDERID = filterBonderId;
                }
            }
            ViewBag.filterBonderId = BONDERID;
            if (loggedinUser!= null && loggedinUser.BONDERID > 0)
            {
                ViewBag.resultofbonderID = loggedinUser.BONDERID;
                annualentlrawmaterials = db.ANNUALENTLRAWMATERIALs.Include(a => a.MATERIAL).Include(a => a.MEASUREMENTUNIT).Where(e => e.BONDERSLNO == loggedinUser.BONDERID);
                
            }
            else if (BONDERID > 0)
            {
                ViewBag.resultofbonderID = BONDERID;
                annualentlrawmaterials = db.ANNUALENTLRAWMATERIALs.Include(a => a.MATERIAL).Include(a => a.MEASUREMENTUNIT).Where(e => e.BONDERSLNO == BONDERID);
            }
           
            if (BONDERID != 0)
            {
                if (loggedinUser.BONDERID > 0)
                {
                    ViewBag.resultofbonderID = loggedinUser.BONDERID;
                    annualentlrawmaterials = db.ANNUALENTLRAWMATERIALs.Include(a => a.MATERIAL).Include(a => a.MEASUREMENTUNIT).Where(e => e.BONDERSLNO == loggedinUser.BONDERID);
                }
                else if (BONDERID > 0)
                {
                    ViewBag.resultofbonderID = BONDERID;
                    annualentlrawmaterials = db.ANNUALENTLRAWMATERIALs.Include(a => a.MATERIAL).Include(a => a.MEASUREMENTUNIT).Where(e => e.BONDERSLNO == BONDERID);
                }
            }
            if (loggedinUser != null && loggedinUser.BONDERID != null)
            {
                ViewBag.resultofbonderID = loggedinUser.BONDERID;
                annualentlrawmaterials = annualentlrawmaterials.Where(i => i.BONDERSLNO == loggedinUser.BONDERID);
            }
           foreach(var item in annualentlrawmaterials.ToList()){
               var bonderAE = db.BONDERANNUALENTITLEMENTs.Where(bae => bae.AESLNO == item.AESLNO && bae.BONDERSLNO == item.BONDERSLNO).Select(bae => new
               {
                   From = bae.ENTITLEFROM,
                   To = bae.ENTITLETO
               }).Single();

               item.ENTITLEFROM = bonderAE.From;
               item.ENTITLETO = bonderAE.To;
           }
           ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", ViewBag.resultofbonderID);
            return View(annualentlrawmaterials.ToList().ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult Index(int? page, string FromDate, string ToDate, int BONDERID=0)
        {
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            List<ANNUALENTLRAWMATERIAL> aerawmaterials = new List<ANNUALENTLRAWMATERIAL>();
            int pageSize = recordNumbers;
            int pageNumber = (page ?? 1);
            USERPERMISSION loggedinUser = new SessionAttributeRetreival().getStoredUserPermission();
            
            if (BONDERID == 0  && !User.IsInRole("Bonder"))
            {
                
                ViewBag.resultofbonderID = BONDERID;
                return View(aerawmaterials.ToList().ToPagedList(pageNumber, pageSize));
            }
            if (loggedinUser != null && loggedinUser.BONDERID != null)
            {
                BONDERID = (int)loggedinUser.BONDERID;
            }
            var annualentlrawmaterials = db.ANNUALENTLRAWMATERIALs.Include(a => a.MATERIAL).Include(a => a.MEASUREMENTUNIT);
            if ((FromDate == String.Empty || ToDate == String.Empty) && BONDERID != 0) {
                BONDREGISTRATION registrationInfo = db.BONDREGISTRATIONs.Where(br=>br.BONDERSLNO==BONDERID).SingleOrDefault();
                FromDate = registrationInfo.ISSUEDATE.ToString();
                ToDate = registrationInfo.EXPIRYDATE.ToString();
            }
            if ((FromDate != null && FromDate != String.Empty) && (ToDate != null && ToDate != String.Empty) && BONDERID != 0)
            {
                DateTime fromAEDate = Convert.ToDateTime(FromDate);
                DateTime toAEDate = Convert.ToDateTime(ToDate);
                annualentlrawmaterials = annualentlrawmaterials.Where(ae => ae.BONDERSLNO == BONDERID);
                if (annualentlrawmaterials != null)
                {
                    foreach (var item in annualentlrawmaterials.ToList())
                    {
                        var bonderAE = db.BONDERANNUALENTITLEMENTs.Where(bae => bae.AESLNO == item.AESLNO && bae.BONDERSLNO == item.BONDERSLNO && bae.ENTITLEFROM >= fromAEDate && bae.ENTITLETO <= toAEDate).Select(bae => new
                        {
                            From = bae.ENTITLEFROM,
                            To = bae.ENTITLETO
                        }).SingleOrDefault();
                        if (bonderAE!=null && bonderAE.From != null && bonderAE.To != null)
                        {
                            item.ENTITLEFROM = bonderAE.From;
                            item.ENTITLETO = bonderAE.To;
                            aerawmaterials.Add(item);
                        }

                    }
                }
                ViewBag.resultofbonderID = BONDERID;
                ViewBag.CurrentFilterFrDate = FromDate;
                ViewBag.CurrentFilterToDate = ToDate;
            }
            else if ((FromDate == String.Empty || ToDate == String.Empty) && BONDERID != 0)
            {
                annualentlrawmaterials = annualentlrawmaterials.Where(ae => ae.BONDERSLNO == BONDERID);
                foreach (var item in annualentlrawmaterials.ToList())
                {
                    var bonderAE = db.BONDERANNUALENTITLEMENTs.Where(bae => bae.AESLNO == item.AESLNO && bae.BONDERSLNO == item.BONDERSLNO ).Select(bae => new
                    {
                        From = bae.ENTITLEFROM,
                        To = bae.ENTITLETO
                    }).FirstOrDefault();
                    if ( bonderAE != null && bonderAE.From != null && bonderAE.To != null)
                    {
                        item.ENTITLEFROM = bonderAE.From;
                        item.ENTITLETO = bonderAE.To;
                        aerawmaterials.Add(item);
                    }
                     
                }
                ViewBag.resultofbonderID = BONDERID;
                ViewBag.CurrentFilterFrDate = FromDate;
                ViewBag.CurrentFilterToDate = ToDate;

            }
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", ViewBag.resultofbonderID);
            return View(aerawmaterials.ToPagedList(pageNumber, pageSize));
        }
        //
        // GET: /AnnualEntitlement/Details/5
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Details(short id = 0)
        {
            ANNUALENTLRAWMATERIAL annualentlrawmaterial = db.ANNUALENTLRAWMATERIALs.Find(id);
            if (annualentlrawmaterial == null)
            {
                return HttpNotFound();
            }
            return View(annualentlrawmaterial);
        }

        //
        // GET: /AnnualEntitlement/Create

        public ActionResult Create()
        {
            ViewBag.MSLNO = new SelectList(db.MATERIALS, "MSLNO", "MHSCODE");
            ViewBag.MUSLNO = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
            return View();
        }

        //
        // POST: /AnnualEntitlement/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ANNUALENTLRAWMATERIAL annualentlrawmaterial)
        {
            if (ModelState.IsValid)
            {
                db.ANNUALENTLRAWMATERIALs.Add(annualentlrawmaterial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MSLNO = new SelectList(db.MATERIALS, "MSLNO", "MHSCODE", annualentlrawmaterial.MSLNO);
            ViewBag.MUSLNO = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", annualentlrawmaterial.MUSLNO);
            return View(annualentlrawmaterial);
        }

        //
        // GET: /AnnualEntitlement/Edit/5

        public ActionResult Edit(short id = 0)
        {
            ANNUALENTLRAWMATERIAL annualentlrawmaterial = db.ANNUALENTLRAWMATERIALs.Find(id);
            if (annualentlrawmaterial == null)
            {
                return HttpNotFound();
            }
            ViewBag.MSLNO = new SelectList(db.MATERIALS, "MSLNO", "MHSCODE", annualentlrawmaterial.MSLNO);
            ViewBag.MUSLNO = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", annualentlrawmaterial.MUSLNO);
            return View(annualentlrawmaterial);
        }

        //
        // POST: /AnnualEntitlement/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ANNUALENTLRAWMATERIAL annualentlrawmaterial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(annualentlrawmaterial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MSLNO = new SelectList(db.MATERIALS, "MSLNO", "MHSCODE", annualentlrawmaterial.MSLNO);
            ViewBag.MUSLNO = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", annualentlrawmaterial.MUSLNO);
            return View(annualentlrawmaterial);
        }

        //
        // GET: /AnnualEntitlement/Delete/5

        public ActionResult Delete(short id = 0)
        {
            ANNUALENTLRAWMATERIAL annualentlrawmaterial = db.ANNUALENTLRAWMATERIALs.Find(id);
            if (annualentlrawmaterial == null)
            {
                return HttpNotFound();
            }
            return View(annualentlrawmaterial);
        }

        //
        // POST: /AnnualEntitlement/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            ANNUALENTLRAWMATERIAL annualentlrawmaterial = db.ANNUALENTLRAWMATERIALs.Find(id);
            db.ANNUALENTLRAWMATERIALs.Remove(annualentlrawmaterial);
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
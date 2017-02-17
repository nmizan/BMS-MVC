using BMSPhase2Demo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using BMSPhase2Demo.Utils;
using BMSPhase2Demo.Util;
namespace BMSPhase2Demo.Controllers
{
    public class ExBondController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();
        private int recordNumbers = GlobalConstants.recordNumbers;
        SessionAttributeRetreival sessionAttributeRetreival = new SessionAttributeRetreival();
       
        //
        // GET: /ExBond/
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult Index(int? page,int filterBonderId=0,int BONDERID = 0)
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
                TempData.Remove("Message");
            } 

            int pageSize = recordNumbers;
            int pageNumber = (page ?? 1);
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
           USERPERMISSION loggedinUser = sessionAttributeRetreival.getStoredUserPermission();
           var exbonds = db.EXBONDs.Include(e => e.BONDER).OrderBy(e => e.BONDERID).ThenByDescending(e => e.ID);
           if (BONDERID == 0 && filterBonderId ==0 && !User.IsInRole("Bonder"))
           {
               List<EXBOND> exbond = new List<EXBOND>();
               ViewBag.resultofbonderID = BONDERID;
               return View(exbond.ToList().ToPagedList(pageNumber, pageSize));
           }
           if (BONDERID!= 0)
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
           if (loggedinUser.BONDERID > 0)
           {
               ViewBag.resultofbonderID = loggedinUser.BONDERID;
               exbonds = db.EXBONDs.Include(e => e.BONDER).Where(e => e.BONDERID == loggedinUser.BONDERID).OrderBy(e => e.BONDERID).ThenByDescending(e => e.ID);
           }
           else if (BONDERID > 0 )
           {
               ViewBag.resultofbonderID = BONDERID;
               exbonds = db.EXBONDs.Include(e => e.BONDER).Where(e => e.BONDERID == BONDERID).OrderBy(e => e.BONDERID).ThenByDescending(e => e.ID);
           }
            ViewBag.lcNo = db.BACKTOBACKLCs.Include(e => e.ID);
            ViewBag.userBonderId = loggedinUser.BONDERID;
          
            if (BONDERID!=0)
            {
                if (loggedinUser.BONDERID > 0)
                {
                    ViewBag.resultofbonderID = loggedinUser.BONDERID;
                    exbonds = db.EXBONDs.Include(e => e.BONDER).Where(e => e.BONDERID == loggedinUser.BONDERID).OrderBy(e => e.BONDERID).ThenByDescending(e => e.ID);
                }
                else if (BONDERID > 0)
                {
                    ViewBag.resultofbonderID = BONDERID;
                    exbonds = db.EXBONDs.Include(e => e.BONDER).Where(e => e.BONDERID == BONDERID).OrderBy(e => e.BONDERID).ThenByDescending(e => e.ID);
                }
            }
           
            return View(exbonds.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /ExBond/Details/5
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult Details(short id = 0) 
        {
            EXBOND exbond = db.EXBONDs.Find(id);
            if (exbond == null)
            {
                return HttpNotFound();
            }
            IList<BACKTOBACKLC> b2bList = new List<BACKTOBACKLC>();
            var exbondb2b = db.EXBONDBACKTOBACKs.Where(eb => eb.EXBONDID == id).Select(u => new
            {
                ID = u.BACKTOBACKID
            }).ToList();

            for (int i = 0; i < exbondb2b.Count(); i++)
            {
                BACKTOBACKLC b2blc = db.BACKTOBACKLCs.Find(exbondb2b[i].ID);
                b2bList.Add(b2blc);
            }
            ViewBag.ExbondId = id;
            return View(b2bList);
        }

        //
        // GET: /ExBond/Create
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult Create(int? page, int BONDERID = 0, int filterBonderId = 0)
        {
            USERPERMISSION loggedinUser = sessionAttributeRetreival.getStoredUserPermission();
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            ViewBag.error = Request.QueryString["error"];
            ViewBag.ExbondID = (short)0;
            ViewBag.userBonderId = loggedinUser.BONDERID;
            var requestType = this.HttpContext.Request.RequestType;
            var backtobacklcs = db.BACKTOBACKLCs.OrderByDescending(b => b.ID).Include(b => b.BANKBRANCH).Include(b => b.BANK).Include(b => b.BONDER).Where(b => b.EXBONDBACKTOBACKs.Count()<1);
            int pageSize = recordNumbers;
            int pageNumber = (page ?? 1);
            if (loggedinUser.BONDERID > 0)
            {
                backtobacklcs =  db.BACKTOBACKLCs.OrderByDescending(b => b.ID).Include(b => b.BANKBRANCH).Include(b => b.BANK).Include(b => b.BONDER).Where(b => b.EXBONDBACKTOBACKs.Count() < 1 && b.BONDERID == loggedinUser.BONDERID);
            }
            if (BONDERID == 0 && filterBonderId == 0 && !User.IsInRole("Bonder"))
            {
                List<BACKTOBACKLC> lcs = new List<BACKTOBACKLC>();
                ViewBag.resultofbonderID = BONDERID;
                return View("Lcselector", lcs.ToPagedList(pageNumber, pageSize));
            }
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
      
            if (requestType == "GET")
            {
                if (BONDERID > 0)
                {
                    backtobacklcs = db.BACKTOBACKLCs.OrderByDescending(b => b.ID).Include(b => b.BANKBRANCH).Include(b => b.BANK).Include(b => b.BONDER).Where(b => b.EXBONDBACKTOBACKs.Count() < 1 && b.BONDERID == BONDERID);
                    ViewBag.resultofbonderID = BONDERID;
                }
                if (loggedinUser != null && loggedinUser.BONDERID != null)
                {
                    ViewBag.resultofbonderID = loggedinUser.BONDERID;
                    backtobacklcs = db.BACKTOBACKLCs.OrderByDescending(b => b.ID).Include(b => b.BANKBRANCH).Include(b => b.BANK).Include(b => b.BONDER).Where(b => b.EXBONDBACKTOBACKs.Count() < 1 && b.BONDERID == loggedinUser.BONDERID);
                }
                return View("Lcselector", backtobacklcs.ToPagedList(pageNumber, pageSize));
            }
            else if (requestType == "POST")
            {
                if (BONDERID > 0)
                {
                    ViewBag.resultofbonderID = BONDERID;
                    backtobacklcs = db.BACKTOBACKLCs.OrderByDescending(b => b.ID).Include(b => b.BANKBRANCH).Include(b => b.BANK).Include(b => b.BONDER).Where(b => b.EXBONDBACKTOBACKs.Count() < 1 && b.BONDERID == BONDERID);

                }
            }
            if (loggedinUser != null && loggedinUser.BONDERID != null)
            {
                ViewBag.resultofbonderID = loggedinUser.BONDERID;
                backtobacklcs = db.BACKTOBACKLCs.OrderByDescending(b => b.ID).Include(b => b.BANKBRANCH).Include(b => b.BANK).Include(b => b.BONDER).Where(b => b.EXBONDBACKTOBACKs.Count() < 1 && b.BONDERID == loggedinUser.BONDERID);
            }
            return View("Lcselector", backtobacklcs.ToPagedList(pageNumber, pageSize));
        }

        //
        // POST: /ExBond/Create
/*
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult Create(EXBOND exbond)
        {
            if (ModelState.IsValid)
            {
                db.EXBONDs.Add(exbond);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERSLNO", exbond.BONDERID);
            return View(exbond);
        }
*/
        //
        // GET: /ExBond/Edit/5
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult Edit(short id = 0)
        {
            EXBOND exbond = db.EXBONDs.Find(id);
            if (exbond == null)
            {
                return HttpNotFound();
            }
            IList<BACKTOBACKLC> b2bList = new List<BACKTOBACKLC>();
            var exbondb2b = db.EXBONDBACKTOBACKs.Where(eb => eb.EXBONDID == id).Select(u => new
            {   ID = u.BACKTOBACKID
            }).ToList();
           
            for (int i = 0; i < exbondb2b.Count(); i++)
            {
                BACKTOBACKLC b2blc = db.BACKTOBACKLCs.Find(exbondb2b[i].ID);
                b2bList.Add(b2blc);
            }
            ViewBag.ExbondID = id;
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERSLNO", exbond.BONDERID);
            return View(b2bList);
        }

        //
        // POST: /ExBond/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult Edit(EXBOND exbond)
        {
            if (ModelState.IsValid)
            {
                db.Entry(exbond).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERSLNO", exbond.BONDERID);
            return View(exbond);
        }

        //
        // GET: /ExBond/Delete/5
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult Delete(short id = 0)
        {
            EXBOND exbond = db.EXBONDs.Find(id);
            if (exbond == null)
            {
                return HttpNotFound();
            }
            return View(exbond);
        }

        //
        // POST: /ExBond/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult DeleteConfirmed(short id)
        {
           
            var childData = db.EXBONDBACKTOBACKs.Where(eb =>eb.EXBONDID == id).ToList();
            foreach (var data in childData)
            {

                db.EXBONDBACKTOBACKs.Remove(data);
            }
            EXBOND exbond = db.EXBONDs.Find(id);
            TempData["Message"] = string.Format("ExBond deleted successfully, Which Bonder Name was {0}", exbond.BONDER.BONDERNAME);
            db.EXBONDs.Remove(exbond);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        public ActionResult addMoreB2BLC(short id, int? page) 
        {
            USERPERMISSION loggedinUser = sessionAttributeRetreival.getStoredUserPermission();
            List<BACKTOBACKLC> b2bList = new List<BACKTOBACKLC>();
            EXBOND exbond = db.EXBONDs.Find(id);
            
           int id1 = exbond.BONDER.BONDERSLNO;
           bool flag = false;
            var b2b = db.BACKTOBACKLCs.Where(eb => eb.BONDERID == id1 && (eb.BACKTOBACKPRODUCTs.Count() >0) ).ToList();
            for (int i = 0; i < b2b.Count(); i++)
            {
                BACKTOBACKLC b2blc = db.BACKTOBACKLCs.Find(b2b[i].ID);
                if (b2blc != null && b2blc.EXBONDBACKTOBACKs.Count() < 1)
                {
                    flag = true;
                    b2bList.Add(b2blc);
                  
                }
               
            }
            if (!flag)
            {
                return RedirectToAction("Edit/" + id, new { error = "Sorry! There is no more Back to Back LC exist for this bonder" });
               
            }
            ViewBag.ExbondID = id;
            ViewBag.resultofbonderID = exbond.BONDERID;
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", exbond.BONDER.BONDERNAME);
            int pageSize = recordNumbers;
            int pageNumber = (page ?? 1);
            return View("Lcselector", b2bList.ToPagedList(pageNumber, pageSize));
           // return View("Lcselector", b2bList);
            
           
         }
        public ActionResult CreateorEditExbond(IList<BACKTOBACKLC> backtobacklcs, short ID = 0)
        {

                USERPERMISSION loggedinUser = sessionAttributeRetreival.getStoredUserPermission();
                IList<BACKTOBACKLC> b2bList = new List<BACKTOBACKLC>();
                EXBOND exbond = new EXBOND();
                if (backtobacklcs == null) {
                    return RedirectToAction("Create", new { error = "Please select at least one back to back lc" });
                }
                bool flag = true;
                bool isanyselected = false;
                
                for (int i = 0; i < backtobacklcs.Count(); i++)
                {
                    int id = backtobacklcs[i].ID;
                    if (backtobacklcs[i].IsSelected)
                    {
                        isanyselected = true;
                        BACKTOBACKLC back2backlc = db.BACKTOBACKLCs.Find(id);
                        int productCount = back2backlc.BACKTOBACKPRODUCTs.Count();
                        if (productCount < 1)
                        {
                            flag = false;
                        }
                        else
                        {
                            exbond.BONDERID = backtobacklcs[i].BONDER.BONDERSLNO;
                            b2bList.Add(back2backlc);

                        }
                    }
                }
                if (!isanyselected && ID <= 0)
                {

                    return RedirectToAction("Create", new { error = "Please select at least one back to back lc" });

                }
                if (!flag && ID <= 0)
                {
                    return RedirectToAction("Create", new { error = "Sorry! please check all selected LC has at least one product" });
                }
                else
                {
                    short lastInserted = 0;
                    //if (ModelState.IsValid)
                    //{
                        if (ID <= 0)//add new exbond
                        {
                            if (loggedinUser!=null)
                            {
                                exbond.CREATEDBY = loggedinUser.APPUSER.USERNAME;
                            }
                            //else
                            //{
                            //    exbond.CREATEDBY = "" + loggedinUser.USERID;
                            //}
                            exbond.CREATEDDATE = DateTime.Today;
                            db.EXBONDs.Add(exbond);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception e) {
                                Console.WriteLine(e.ToString());
                            }
                            lastInserted = db.EXBONDs.Max(item => item.ID);
                        }
                        else//edit new exbond
                        {
                            using (var db2 = new OracleEntitiesConnStr())
                            {
                                exbond = db.EXBONDs.Find(ID);
                                EXBOND newExbond = new EXBOND();
                                newExbond.ID = ID;
                                newExbond.BONDERID = exbond.BONDERID;
                                newExbond.DESCRIPTION = exbond.DESCRIPTION;
                                if (loggedinUser!=null)
                                {
                                    newExbond.MODIFIEDBY = loggedinUser.APPUSER.USERNAME;
                                }

                                newExbond.MODIFIEDDATE = DateTime.Today;
                                newExbond.CREATEDDATE = exbond.CREATEDDATE;
                                newExbond.CREATEDBY = exbond.CREATEDBY;
                                db2.Entry(newExbond).State = EntityState.Modified;
                                db2.SaveChanges();
                            }
                            lastInserted = ID;
                        }
                        for (int x = 0; x < b2bList.Count(); x++)
                        {
                            using (var db1 = new OracleEntitiesConnStr())
                            {
                                EXBONDBACKTOBACK exbondb2b = new EXBONDBACKTOBACK();
                                exbondb2b.BACKTOBACKID = b2bList[x].ID;
                                exbondb2b.EXBONDID = lastInserted;
                                db1.EXBONDBACKTOBACKs.Add(exbondb2b);
                                db1.SaveChanges();
                            }

                        }
                   // }

                    if (ID <= 0)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Edit/" + ID);
                    }

               
            }
              
        }

        public ActionResult DeleteBack2backLc(short id)
        {

            var childData = db.EXBONDBACKTOBACKs.Where(eb => eb.BACKTOBACKID == id).ToList();
            foreach (var data in childData)
            {

                db.EXBONDBACKTOBACKs.Remove(data);
            }
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        [HttpPost]

        public ActionResult Save(short id)
        {
            EXBOND exbond = db.EXBONDs.Find(id);
            if (exbond.EXBONDBACKTOBACKs.Count() < 1)
            {
                db.EXBONDs.Remove(exbond);
                db.SaveChanges();

            }
            return RedirectToAction("Index");
        }
    }
}
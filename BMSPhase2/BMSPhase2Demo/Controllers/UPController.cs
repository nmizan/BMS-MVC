using BMSPhase2Demo.Models;
using BMSPhase2Demo.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using BMSPhase2Demo.Util;
using BMSPhase2Demo.Utils;
namespace BMSPhase2Demo.Controllers
{
    public class UPController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();
        private int recordNumbers = GlobalConstants.recordNumbers;
        SessionAttributeRetreival sessionAttributeRetreival = new SessionAttributeRetreival();
        //
        // GET: /UP/
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Index(int? page, string UPNO, string currentFilterUP, int filterBonderId = 0, int BONDERID = 0)
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
                TempData.Remove("Message");
            }

            var ups = db.UPs.Include(u => u.BONDER);
            int pageSize = recordNumbers;
            int pageNumber = (page ?? 1);
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            USERPERMISSION loggedinUser = new SessionAttributeRetreival().getStoredUserPermission();
            if (BONDERID == 0 && filterBonderId == 0 && !User.IsInRole("Bonder"))
            {
                List<UP> initialUps = new List<UP>();
                ViewBag.resultofbonderID = BONDERID;
                return View(initialUps.ToList().ToPagedList(pageNumber, pageSize));
            }
            if (UPNO!=null||BONDERID != 0)
            {
                page = 1;
            }
            else
            {
                if (UPNO == null)
                {
                    UPNO = currentFilterUP;
                }
                if (BONDERID == 0)
                {
                    BONDERID = filterBonderId;
                }
            }
            ViewBag.filterBonderId = BONDERID;
            ViewBag.CurrentFilterUP = UPNO;
            if (loggedinUser.BONDERID!=null)
            {
                ViewBag.resultofbonderID = loggedinUser.BONDERID;
                ups = db.UPs.Include(u => u.BONDER).Where(e => e.BONDERID == loggedinUser.BONDERID);
            }
            else if (BONDERID > 0)
            {
                ViewBag.resultofbonderID = BONDERID;
                ups = db.UPs.Include(u => u.BONDER).Where(e => e.BONDERID == BONDERID);
            }
            if (UPNO != null) {
                ups = ups.Include(u => u.BONDER).Where(e => e.UPNO.Contains(UPNO));
            }
            ViewBag.userBonderId = loggedinUser.BONDERID;

            if (BONDERID != 0)
            {
                if (loggedinUser.BONDERID!=null)
                {
                    ViewBag.resultofbonderID = loggedinUser.BONDERID;
                    ups = db.UPs.Include(u => u.BONDER).Where(e => e.BONDERID == loggedinUser.BONDERID);
                }
                else if (BONDERID > 0)
                {
                    ViewBag.resultofbonderID = BONDERID;
                    ups = db.UPs.Include(u => u.BONDER).Where(e => e.BONDERID == BONDERID);
                }
                if (UPNO != null)
                {
                    ups = ups.Include(u => u.BONDER).Where(e => e.UPNO.Contains(UPNO));
                }
            }
            return View(ups.ToList().ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /UP/Details/5
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Details(short id = 0)
        {
            ApprovedUPViewModel viewModel = new ApprovedUPViewModel();
            UP up = db.UPs.Find(id);
            if (up == null)
            {
                return HttpNotFound();
            }

            List<UPREQUEST> uprequestList = new List<UPREQUEST>();
            List<EXBOND> exbondList = new List<EXBOND>();
            List<BACKTOBACKLC> b2bList = new List<BACKTOBACKLC>();
            List<BACKTOBACKPRODUCT> productList = new List<BACKTOBACKPRODUCT>();
            List<RAWMATERIAL> rawmaterialList = new List<RAWMATERIAL>();
            List<ATTACHMENT> attachments = new List<ATTACHMENT>();

            var uprequests = db.UPREQUESTLISTs.Where(eb => eb.UPID == id).ToList();

            for (int i = 0; i < uprequests.Count(); i++)
            {
                UPREQUEST uprequest = db.UPREQUESTs.Find(uprequests[i].UPREQUESTID);

                if (uprequest != null)
                {
                    for (int j = 0; j < uprequest.UPEXBONDLISTs.Count(); j++)
                    {
                        EXBOND exbond = db.EXBONDs.Find(uprequest.UPEXBONDLISTs[j].EXBONDID);
                        if (exbond != null)
                        {
                            for (int k = 0; k < exbond.EXBONDBACKTOBACKs.Count(); k++)
                            {
                                BACKTOBACKLC backtoback = db.BACKTOBACKLCs.Find(exbond.EXBONDBACKTOBACKs.ElementAt(k).BACKTOBACKID);
                                if (backtoback != null)
                                {
                                    for (int l = 0; l < backtoback.BACKTOBACKPRODUCTs.Count(); l++)
                                    {
                                        BACKTOBACKPRODUCT product = db.BACKTOBACKPRODUCTs.Find(backtoback.BACKTOBACKPRODUCTs[l].ID);
                                        if (product != null)
                                        {
                                            if (product.RAWMATERIALs.Count() > 0)
                                            {
                                                rawmaterialList = product.RAWMATERIALs.ToList();
                                            }
                                            productList.Add(product);
                                        }
                                    }
                                    b2bList.Add(backtoback);
                                }
                            }
                            exbondList.Add(exbond);
                        }
                    }
                    if (uprequest.ATTACHMENTs.Count() > 0)
                    {
                        attachments = uprequest.ATTACHMENTs.ToList();

                    }
                    uprequestList.Add(uprequest);
                }
            }

            viewModel.UP = up;
            viewModel.UPREQUESTs = uprequestList;
            viewModel.BACKTOBACKLCs = b2bList;
            viewModel.BACKTOBACKPRODUCTs = productList;
            viewModel.EXBONDs = exbondList;
            viewModel.RAWMATERIALs = rawmaterialList;
            viewModel.ATTACHMENTs = attachments;
            return View(viewModel);
        }

        //
        // GET: /UP/Create
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Create(int? page, int BONDERID = 0, int filterBonderId = 0)
        {
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            ViewBag.requestID = (short)0;
            ViewBag.error = Request.QueryString["error"];
            int pageSize = recordNumbers;
            int pageNumber = (page ?? 1);
            var requests = db.UPREQUESTs.OrderByDescending(r => r.ID).Where(r => r.UPREQUESTLISTs.Count() < 1 && r.STATUS == 2);
            var requestType = this.HttpContext.Request.RequestType;
            if (BONDERID == 0 && filterBonderId == 0 && ViewBag.error == null)
            {
                List<UPREQUEST> exbond = new List<UPREQUEST>();
                ViewBag.resultofbonderID = BONDERID;
                return View("RequestSelector", exbond.ToList().ToPagedList(pageNumber, pageSize));
            }
            if (requests.Count() > 0)
            {
                ViewBag.error = Request.QueryString["error"];
            }
            else
            {
                ViewBag.error = "There is no request Pending for Approval";
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
                    ViewBag.resultofbonderID = BONDERID;
                    requests = db.UPREQUESTs.Where(u => u.BONDERID == BONDERID && u.UPREQUESTLISTs.Count() < 1 && u.STATUS == 2).OrderByDescending(r => r.ID);

                }
                return View("RequestSelector", requests.ToPagedList(pageNumber, pageSize));
            }
            else if (requestType == "POST")
            {
                if (BONDERID > 0)
                {
                    ViewBag.resultofbonderID = BONDERID;
                    requests = db.UPREQUESTs.Where(u => u.BONDERID == BONDERID && u.UPREQUESTLISTs.Count() < 1 && u.STATUS == 2).OrderByDescending(r => r.ID);

                }
            }
            return View("RequestSelector", requests.ToPagedList(pageNumber, pageSize));
        }

        //
        // POST: /UP/Create
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Create(UP up)
        {
            if (ModelState.IsValid)
            {
                db.UPs.Add(up);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERSLNO", up.BONDERID);
            return View(up);
        }
        */
        //
        // GET: /UP/Edit/5
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Edit(short id = 0)
        {
            UP up = db.UPs.Find(id);
            if (up == null)
            {
                return HttpNotFound();
            }
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERSLNO", up.BONDERID);
            return View(up);
        }

        //
        // POST: /UP/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Edit(UP up)
        {
            if (ModelState.IsValid)
            {
                db.Entry(up).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERSLNO", up.BONDERID);
            return View(up);
        }

        //
        // GET: /UP/Delete/5
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Delete(short id = 0)
        {
            UP up = db.UPs.Find(id);
            if (up == null)
            {
                return HttpNotFound();
            }
            return View(up);
        }

        //
        // POST: /UP/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult DeleteConfirmed(short id)
        {

            var childData = db.UPREQUESTLISTs.Where(r => r.UPID == id).ToList();
            foreach (var data in childData)
            {
                db.UPREQUESTLISTs.Remove(data);
            }
            UP up = db.UPs.Find(id);
            TempData["Message"] = string.Format("UP deleted successfully, Which UP No was {0}", up.UPNO);
            db.UPs.Remove(up);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = "Operation Admin")]
        [HttpPost]
        public ActionResult ProvideUPNO(IList<UPREQUEST> requests)
        {
            ViewBag.requestID = (short)0;
            List<UPREQUEST> selectedRequests = new List<UPREQUEST>();
            bool flag=true, isSelected=false;
            foreach (UPREQUEST uprequest in requests) {
                if (uprequest.IsSelected) {
                    
                    UPREQUEST request = db.UPREQUESTs.Find(uprequest.ID);
                    request.IsSelected = true;
                    ModelState.Remove("ID");
                    request.ID = uprequest.ID;
                    selectedRequests.Add(request);
                    int productCount = request.UPEXBONDLISTs.Count();
                    if (productCount < 1)
                    {
                        flag = false;
                    }
                    isSelected = true;
                }
            }
            if (!isSelected)
            {
                return RedirectToAction("Create", new { error = "Please select at least one exbond" });

            }
            if (!flag)
            {
                return RedirectToAction("Create", new { error = "Sorry! please check all selected Requests has at least one exbond" });
            }
            
            return View("UPNoEditor",selectedRequests);
        }

        [CustomAuthorize(Roles = "Operation Admin")]
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult CreateOREditUP(IList<UPREQUEST> requests, string UPNO, short ID = 0)
        {
            USERPERMISSION loggedinUser = sessionAttributeRetreival.getStoredUserPermission();
            IList<UPREQUEST> requestList = new List<UPREQUEST>();
            UP up = new UP();
            bool flag = true;
            bool isanyselected = false;
            if (UPNO == null||UPNO.Equals(""))
            {
                return RedirectToAction("Create", new { error = "Please Provide UP NO" });
            }
            else{
                UP duplicateUp = db.UPs.SingleOrDefault(u=>u.UPNO.Equals(UPNO));
                if (duplicateUp != null) {
                    return RedirectToAction("Create",new { error = "UP No Already Exists" });
                }
            }
            for (int i = 0; i < requests.Count(); i++)
            {
                int id = requests[i].ID;
                if (requests[i].IsSelected)
                {
                    isanyselected = true;
                    UPREQUEST request = db.UPREQUESTs.Find(id);
                    int productCount = request.UPEXBONDLISTs.Count();
                    if (productCount < 1)
                    {
                        flag = false;
                    }
                    else
                    {
                        requestList.Add(request);
                        up.BONDERID = request.BONDERID;
                    }
                }
            }
            if (!isanyselected && ID <= 0)
            {
                return RedirectToAction("Create", new { error = "Please select at least one exbond" });

            }
            if (!flag && ID <= 0)
            {
                return RedirectToAction("Create", new { error = "Sorry! please check all selected Requests has at least one exbond" });
            }
            else
            {
                short lastInserted = 0;

                if (ID <= 0)//add new UP
                {
                    DateTime thisDay = DateTime.Today;
                    up.CREATEDDATE = thisDay;
                    if (loggedinUser != null)
                    {
                        up.CREATEDBY = loggedinUser.APPUSER.USERNAME;
                    }
                    up.UPNO = UPNO;
                    db.UPs.Add(up);
                    db.SaveChanges();
                    lastInserted = db.UPs.Max(item => item.ID);
                }
                else//Edit existing UP
                {
                    if (loggedinUser!=null)
                    {
                        up.MODIFIEDBY = loggedinUser.APPUSER.USERNAME;
                    }
                    up = db.UPs.Find(ID);
                    UP newUp = new UP();
                    newUp.ID = ID;
                    newUp.BONDERID = up.BONDERID;
                    newUp.UPNO = up.UPNO;
                    newUp.MODIFIEDDATE = DateTime.Today;
                    newUp.CREATEDDATE = up.CREATEDDATE;
                    newUp.CREATEDBY = up.CREATEDBY;
                    using (var db2 = new OracleEntitiesConnStr())
                    {
                        db2.Entry(newUp).State = EntityState.Modified;
                        db2.SaveChanges();
                    }
                    lastInserted = ID;
                }
                for (int x = 0; x < requestList.Count(); x++)
                {
                    using (var db1 = new OracleEntitiesConnStr())//save value to relational table
                    {
                        UPREQUESTLIST uprequestList = new UPREQUESTLIST();
                        uprequestList.UPREQUESTID = requestList[x].ID;
                        uprequestList.UPID = lastInserted;
                        db1.UPREQUESTLISTs.Add(uprequestList);
                        db1.SaveChanges();
                    }
                    using (var db2 = new OracleEntitiesConnStr())//change status of up request as approved
                    {
                        UPREQUEST request = new UPREQUEST();// db.UPREQUESTs.Find(requestList[x].ID);
                        request.ID = requestList[x].ID;
                        request.BONDERID = requestList[x].BONDERID;
                        request.STATUS = 20; //approved
                        db2.Entry(request).State = EntityState.Modified;
                        db2.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }
        }
        public ActionResult DeleteRequest(string rejectMessage,int id = 0)
        {
            UPREQUEST request = db.UPREQUESTs.Find(id);
            UPREQUEST newRequest = new UPREQUEST();
            newRequest.ID = request.ID;
            newRequest.STATUS = 10;
            newRequest.BONDERID = request.BONDERID;
            newRequest.CREATEDBY = request.CREATEDBY;
            newRequest.CREATEDDATE = request.CREATEDDATE;
            newRequest.MODIFIEDBY = request.MODIFIEDBY;
            newRequest.MODIFIEDDATE = request.MODIFIEDDATE;
            newRequest.REJECTCOMMENT = rejectMessage;
          
                if (newRequest != null)
                {
                    using (var db2 = new OracleEntitiesConnStr())
                    {
                        db2.Entry(newRequest).State = EntityState.Modified;
                        db2.SaveChanges();
                    }
                   
                }
           
           
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
    }
}
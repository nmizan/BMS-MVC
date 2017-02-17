using BMSPhase2Demo.Models;
using BMSPhase2Demo.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
using BMSPhase2Demo.Util;
using BMSPhase2Demo.Utils;
using System.Drawing;
namespace BMSPhase2Demo.Controllers
{
    public class UPRequestController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();
        private int recordNumbers = GlobalConstants.recordNumbers;
        //
        // GET: /UPRequest/
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Index(int? page, int filterBonderId = 0, int BONDERID = 0, int STATUSID = 0)
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
                TempData.Remove("Message");
            }

            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            ViewBag.STATUSID = new List<SelectListItem>{
                new SelectListItem {Text = "Approved", Value = "20"},
                new SelectListItem {Text = "Pending For Approval", Value = "2"},                
                new SelectListItem {Text = "Rejected", Value = "10"}};

            int pageSize = recordNumbers;
            int pageNumber = (page ?? 1);
            
            USERPERMISSION loggedinUser = new SessionAttributeRetreival().getStoredUserPermission();
            var uprequests = db.UPREQUESTs.OrderByDescending(r => r.ID);
            if (User.IsInRole("Bonder"))
            {
                BONDERID = (int)loggedinUser.BONDERID;
            }

            if (BONDERID == 0 && filterBonderId == 0)
            {
                List<UPREQUEST> request = new List<UPREQUEST>();
                ViewBag.resultofbonderID = BONDERID;
                return View(request.ToList().ToPagedList(pageNumber, pageSize));
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
            if (loggedinUser.BONDERID > 0)
            {
                ViewBag.resultofbonderID = loggedinUser.BONDERID;
                uprequests = db.UPREQUESTs.Include(e => e.BONDER).Where(e => e.BONDERID == loggedinUser.BONDERID && e.STATUS == STATUSID).OrderBy(e => e.BONDERID).ThenByDescending(e => e.ID);
            }
            else if (BONDERID > 0)
            {
                ViewBag.resultofbonderID = BONDERID;
                uprequests = db.UPREQUESTs.Include(e => e.BONDER).Where(e => e.BONDERID == BONDERID && e.STATUS == STATUSID).OrderBy(e => e.BONDERID).ThenByDescending(e => e.ID);
            }

            if (BONDERID != 0)
            {
                if (loggedinUser.BONDERID > 0)
                {
                    ViewBag.resultofbonderID = loggedinUser.BONDERID;
                    uprequests = db.UPREQUESTs.Include(e => e.BONDER).Where(e => e.BONDERID == loggedinUser.BONDERID && e.STATUS == STATUSID).OrderBy(e => e.BONDERID).ThenByDescending(e => e.ID);
                }
                else if (BONDERID > 0)
                {
                    ViewBag.resultofbonderID = BONDERID;
                    uprequests = db.UPREQUESTs.Include(e => e.BONDER).Where(e => e.BONDERID == BONDERID && e.STATUS == STATUSID).OrderBy(e => e.BONDERID).ThenByDescending(e => e.ID);
                }
            }
            if (loggedinUser != null && loggedinUser.BONDERID != null)
            {
                ViewBag.resultofbonderID = loggedinUser.BONDERID;
                uprequests = uprequests.Where(i => i.BONDERID == loggedinUser.BONDERID && i.STATUS == STATUSID).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);
            }
           
            return View(uprequests.ToPagedList(pageNumber, pageSize));
        }
        public static string GetStatusinString(short value)
        {
            string status = "saved";

            if (value == 0) status = "Saved";
            if (value == 2) status = "pending for Approval";
            if (value == 20) status = "Approved";


            return status;
        } 
        //
        // GET: /UPRequest/Details/5
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Details(short id = 0)
        {
           
            UPREQUEST uprequest = db.UPREQUESTs.Find(id);

            if (uprequest == null)
            {
                return HttpNotFound();
            }
            IList<EXBOND> exbondList = new List<EXBOND>();
            IList<ATTACHMENT> attachmentList = new List<ATTACHMENT>();
            var upUxbond = db.UPEXBONDLISTs.Where(eb => eb.UPREQUESTID == id).ToList();

            for (int i = 0; i < upUxbond.Count(); i++)
            {
                EXBOND exbond = db.EXBONDs.Find(upUxbond[i].EXBONDID);
                exbondList.Add(exbond);
            }
            var attachments = db.ATTACHMENTs.Where(eb => eb.UPREQUESTID == id).ToList();

            ViewBag.requestID = id;
            ViewBag.error = Request.QueryString["error"];
            UpAttachmentViewModel viewModel = new UpAttachmentViewModel();
            viewModel.ATTACHMENTs = attachments;
            viewModel.EXBONDs = exbondList;
            viewModel.uprequest = uprequest;
            return View(viewModel);
           
        }

        //
        // GET: /UPRequest/Create
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Create(int? page, int BONDERID = 0, int filterBonderId = 0)
        {
            TempData["isAnyAttachment"] = false;
            TempData.Keep();
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            int pageSize = recordNumbers;
            int pageNumber = (page ?? 1);
            var exbonds = db.EXBONDs.OrderByDescending(b => b.ID).Where(b => b.UPEXBONDLISTs.Count()<1);
            var requestType = this.HttpContext.Request.RequestType;
            USERPERMISSION loggedinUser = new SessionAttributeRetreival().getStoredUserPermission();
            ViewBag.error = Request.QueryString["error"];
            ViewBag.requestID = (short)0;
            if (BONDERID == 0 && filterBonderId == 0 &&!User.IsInRole("Bonder"))
            {
                List<EXBOND> exbond = new List<EXBOND>();
                ViewBag.resultofbonderID = BONDERID;
                return View("Exbondselector", exbond.ToList().ToPagedList(pageNumber, pageSize));
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
                    exbonds = db.EXBONDs.Where(u => u.BONDERID == BONDERID && u.UPEXBONDLISTs.Count() < 1).OrderByDescending(r => r.ID);
                    ViewBag.resultofbonderID = BONDERID;
                }
                if (loggedinUser != null && loggedinUser.BONDERID != null)
                {
                    ViewBag.resultofbonderID = loggedinUser.BONDERID;
                    exbonds = exbonds.Where(i => i.BONDERID == loggedinUser.BONDERID && i.UPEXBONDLISTs.Count() < 1).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);
                }
                return View("Exbondselector", exbonds.ToPagedList(pageNumber, pageSize));
            }
            else if (requestType == "POST")
            {
                if (BONDERID > 0)
                {
                    ViewBag.resultofbonderID = BONDERID;
                    exbonds = db.EXBONDs.Where(u => u.BONDERID == BONDERID && u.UPEXBONDLISTs.Count() < 1).OrderByDescending(r => r.ID);

                }
            }
            if (loggedinUser != null && loggedinUser.BONDERID != null)
            {
                ViewBag.resultofbonderID = loggedinUser.BONDERID;
                exbonds = exbonds.Where(i => i.BONDERID == loggedinUser.BONDERID && i.UPEXBONDLISTs.Count() < 1).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);
            }
            return View("Exbondselector", exbonds.ToPagedList(pageNumber, pageSize));
           
        }

        //
        // POST: /UPRequest/Create
/*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UPREQUEST uprequest)
        {
            if (ModelState.IsValid)
            {
                db.UPREQUESTs.Add(uprequest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(uprequest);
        }
*/
        //
        // GET: /UPRequest/Edit/5
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Edit(short id = 0)
        {
            TempData["isAnyAttachment"] = true;
            TempData.Keep();
            UPREQUEST uprequest = db.UPREQUESTs.Find(id);
           
            if (uprequest == null)
            {
                return HttpNotFound();
            }
            IList<EXBOND> exbondList = new List<EXBOND>();
            IList<ATTACHMENT> attachmentList = new List<ATTACHMENT>();
            var upUxbond = db.UPEXBONDLISTs.Where(eb => eb.UPREQUESTID == id).ToList();

            for (int i = 0; i < upUxbond.Count(); i++)
            {
                EXBOND exbond = db.EXBONDs.Find(upUxbond[i].EXBONDID);
                exbondList.Add(exbond);
            }
            var attachments = db.ATTACHMENTs.Where(eb => eb.UPREQUESTID == id).ToList();
            
            ViewBag.requestID = id;
            ViewBag.error = Request.QueryString["error"];
            UpAttachmentViewModel viewModel = new UpAttachmentViewModel();
            viewModel.ATTACHMENTs = attachments;
            viewModel.EXBONDs = exbondList;
            return View(viewModel);
        }

        //
        // POST: /UPRequest/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Edit(UPREQUEST uprequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uprequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(uprequest);
        }

        //
        // GET: /UPRequest/Delete/5
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Delete(short id = 0)
        {
            UPREQUEST uprequest = db.UPREQUESTs.Find(id);
            if (uprequest == null)
            {
                return HttpNotFound();
            }
            return View(uprequest);
        }

        //
        // POST: /UPRequest/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult DeleteConfirmed(short id)
        {
            var childData = db.UPEXBONDLISTs.Where(eb => eb.UPREQUESTID == id).ToList();
            foreach (var data in childData)
            {

                db.UPEXBONDLISTs.Remove(data);
            }
            var attachments = db.ATTACHMENTs.Where(eb => eb.UPREQUESTID == id).ToList();
            foreach (var data in attachments)
            {
                string filepath = data.CONTENT;
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
                db.ATTACHMENTs.Remove(data);
            }
            UPREQUEST uprequest = db.UPREQUESTs.Find(id);
            TempData["Message"] = string.Format("UP Request deleted successfully, Which Bonder Name was {0}", uprequest.BONDER.BONDERNAME);
            db.UPREQUESTs.Remove(uprequest);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult createOrEditRequest(IList<EXBOND> exbonds, IList<ATTACHMENT> attachments, short ID = 0)
        {
            USERPERMISSION loggedinUser = new SessionAttributeRetreival().getStoredUserPermission();

            if (TempData["isAnyAttachment"] != null) { 
                bool isAnyAttachment =(bool) TempData["isAnyAttachment"];
                if (!isAnyAttachment)
                {
                    attachments = null;
                }
            }
                var files = Request.Files;
                //for (int i = 0; i < Request.Files.Count; i++)
                //{
                //    HttpPostedFileBase file = files[i];
                //    var fileName = Path.GetFileName(file.FileName);
                //}
                if (attachments != null)
                {
                    foreach (ATTACHMENT attachment in attachments)
                    {
                        if (attachment != null && attachment.CONTENT == null && attachment.ID == 0 && attachment.NAME== null)
                        {
                            if (ID <= 0)
                            {
                                return RedirectToAction("Create", new { error = "Please provide a valid file" });
                            }
                            else
                            {

                                return RedirectToAction("AddMoreAttachment", new { ID, errorMessage = "Please provide a valid file" });
                            } 
                        }
                        
                       
                    }
                }
                bool attachmentValid = false;
                foreach (string extensions in GlobalConstants.extensions)
                {
                    try
                    {
                        
                        if (attachments.Any(item => item.CONTENT.EndsWith(extensions)))
                        {
                            attachmentValid = true;
                            break;
                        }
                    }
                    catch (Exception e) {
                        attachmentValid = true;
                    }
                }
                if (!attachmentValid)
                {

                    if (ID <= 0)
                    {
                        return RedirectToAction("Create", new { error = "Please provide a valid file" });
                    }
                    else
                    {

                        return RedirectToAction("Edit", new { ID, error = "Please provide a valid file" });
                    } 
                }
                List<ATTACHMENT> newAttachmentList = new List<ATTACHMENT>();
                if (attachments != null){
                for(int i=0;i< attachments.Count();i++){
                    if(attachments[i].CONTENT != null){
                        
                        newAttachmentList.Add(attachments[i]);
                    }
                }
                }
                UPREQUEST uprequest = new UPREQUEST();
                IList<EXBOND> exbondList = new List<EXBOND>();
                bool flag = true;
                bool isanyselected = false;
                if (exbonds != null)
                {
                    for (int i = 0; i < exbonds.Count(); i++)
                    {

                        int id = exbonds[i].ID;
                        if (exbonds[i].IsSelected)
                        {
                            isanyselected = true;
                            EXBOND exbond = db.EXBONDs.Find(id);
                            int b2bCount = exbond.EXBONDBACKTOBACKs.Count();
                            if (exbond != null && b2bCount < 1)
                            {
                                flag = false;
                            }
                            else
                            {
                                uprequest.BONDERID = exbond.BONDERID;
                                exbondList.Add(exbond);

                            }
                        }
                    }
                }
                if (!isanyselected && ID <=0)
                {
                   
                        return RedirectToAction("Create", new { error = "Please select at least one exbond" });
                  
                }
                if (!flag && ID <= 0)
                {
                    return RedirectToAction("Create", new { error = "Sorry! please check all selected Exbond has at least one backtoback" });
                }
                else
                {
                    short lastInserted = 0;
                    //if (ModelState.IsValid)
                    //{
                        if (ID <= 0)
                        {
                            DateTime thisDay = DateTime.Today;
                            uprequest.CREATEDDATE = thisDay;
                            if (loggedinUser != null) {
                                uprequest.CREATEDBY = loggedinUser.APPUSER.USERNAME;
                            }
                            uprequest.STATUS = 2;
                            db.UPREQUESTs.Add(uprequest);
                            db.SaveChanges();           
                            lastInserted = db.UPREQUESTs.Max(item => item.ID);
                        }
                        else
                        {
                            uprequest = db.UPREQUESTs.Find(ID);
                            UPREQUEST newRequest = new UPREQUEST();
                            newRequest.ID = ID;
                            newRequest.BONDERID = uprequest.BONDERID;
                            newRequest.STATUS = uprequest.STATUS;
                            newRequest.MODIFIEDDATE = DateTime.Today;
                            uprequest.CREATEDDATE = uprequest.CREATEDDATE;
                            uprequest.CREATEDBY = uprequest.CREATEDBY;
                            if (loggedinUser != null) {
                                uprequest.MODIFIEDBY = loggedinUser.APPUSER.USERNAME;
                            }
                            using (var newdb2 = new OracleEntitiesConnStr())
                            {
                                newdb2.Entry(newRequest).State = EntityState.Modified;
                                newdb2.SaveChanges();
                            }
                            lastInserted = ID;
                        }
                        for (int x = 0; x < exbondList.Count(); x++)
                        {
                            using (var db1 = new OracleEntitiesConnStr())
                            {
                                UPEXBONDLIST upexbond = new UPEXBONDLIST();
                                upexbond.EXBONDID = exbondList[x].ID;
                                upexbond.UPREQUESTID = lastInserted;
                                db1.UPEXBONDLISTs.Add(upexbond);
                                db1.SaveChanges();
                            }

                        }
                        for (int k = 0; k < newAttachmentList.Count(); k++)
                        {
                            string path = UploadedPath(files[k], newAttachmentList[k].CONTENT);
                            attachments[k].CONTENT = path;
                            using (var db2 = new OracleEntitiesConnStr())
                            {
                                attachments[k].UPREQUESTID = lastInserted;
                                db2.ATTACHMENTs.Add(attachments[k]);
                                db2.SaveChanges();
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

        private string UploadedPath(HttpPostedFileBase file, string p)
        {
           var fileName = Path.GetFileName(file.FileName);
            string ext = Path.GetExtension(file.FileName);
            var _ext = Path.GetExtension(file.FileName);
           
            var _imgname = Guid.NewGuid().ToString();
            String path = System.Web.Configuration.WebConfigurationManager.AppSettings["fileUploadPath"];
            var _comPath = path+"/attachment_" + _imgname + _ext;
            file.SaveAs(_comPath);
            return _comPath;
        }

      
        public ActionResult DeleteExbond(short id)
        {

            var childData = db.UPEXBONDLISTs.Where(eb => eb.EXBONDID == id).ToList();
            foreach (var data in childData)
            {

                db.UPEXBONDLISTs.Remove(data);
            }
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        public ActionResult DeleteAttachment(short id)
        {

            ATTACHMENT attachment = db.ATTACHMENTs.Find(id);
            String filepath = attachment.CONTENT;
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
            db.ATTACHMENTs.Remove(attachment);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        public ActionResult addMoreExbond(short id,int? page)
        {
            List<EXBOND> exbondList = new List<EXBOND>();
            UPREQUEST uprequest = db.UPREQUESTs.Find(id);
            bool flag = false;
            int id1 = uprequest.BONDER.BONDERSLNO;
            ViewBag.resultofbonderID = uprequest.BONDER.BONDERSLNO;
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", uprequest.BONDER.BONDERNAME);
            var b2b = db.EXBONDs.Where(eb => eb.BONDERID == id1 && (eb.UPEXBONDLISTs.Count() < 1)).ToList();
            
            for (int i = 0; i < b2b.Count(); i++)
            {
                EXBOND b2blc = db.EXBONDs.Find(b2b[i].ID);
                if (b2blc != null && b2blc.EXBONDBACKTOBACKs.Count() >=  1)
                {
                    flag = true;
                    exbondList.Add(b2blc);

                }

            }
            if (!flag )
            {
                return RedirectToAction("Edit/" + id, new { error = "Sorry! There is no more Exbond exist to add for this bonder" });
            }
            ViewBag.requestID = id;
            
            int pageSize = recordNumbers;
            int pageNumber = (page ?? 1);
            return View("Exbondselector", exbondList.ToPagedList(pageNumber, pageSize));


        }
        public ActionResult addMoreAttachment(short id,int ?page,string errorMessage)
        {
            List<EXBOND> exbondList = new List<EXBOND>();
            
            UPREQUEST uprequest = db.UPREQUESTs.Find(id);
            int id1 = uprequest.BONDER.BONDERSLNO;

            var b2b = db.EXBONDs.Where(eb => eb.BONDERID == id1 && (eb.UPEXBONDLISTs.Count() < 1)).ToList();

            for (int i = 0; i < b2b.Count(); i++)
            {
                EXBOND b2blc = db.EXBONDs.Find(b2b[i].ID);
                if (b2blc != null && b2blc.EXBONDBACKTOBACKs.Count() >= 1)
                {
                    exbondList.Add(b2blc);
                }
            }
            ViewBag.requestID = id;
            ViewBag.resultofbonderID = id1;
            int pageSize = recordNumbers;
            int pageNumber = (page ?? 1);
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERSLNO");
            if (errorMessage!=null&&!errorMessage.Trim().Equals(""))
            {
                ModelState.AddModelError("fileError", errorMessage);
                return View("Exbondselector", exbondList.ToPagedList(pageNumber, pageSize));
            }
            return View("Exbondselector", exbondList.ToPagedList(pageNumber, pageSize));

        }
        [HttpPost]
       
        public ActionResult Save(short id)
        {
            UPREQUEST uprequest = db.UPREQUESTs.Find(id);
            if (uprequest.UPEXBONDLISTs.Count() < 1 && uprequest.ATTACHMENTs.Count() < 1)
            {
                db.UPREQUESTs.Remove(uprequest);
                db.SaveChanges();             
                
            }
            return RedirectToAction("Index");
        }
        public ActionResult AttachmentEntryRow()
        {
            TempData["isAnyAttachment"] = true;
            TempData.Keep();
            return PartialView("AttachmentEntryEditor");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string getFileName()
        {
            string _imgname = string.Empty;
            var fileName = "";
            HttpPostedFile pic = null;
           
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
               
               
                 pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                 
                 if (pic.ContentLength > 0)
               {
                  fileName = Path.GetFileName(pic.FileName);
                  
               }
            }
            return fileName;
        }
        
    }
}
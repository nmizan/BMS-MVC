using BMSPhase2Demo.CommonAppSet;
using BMSPhase2Demo.Models;
using BMSPhase2Demo.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OracleClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSPhase2Demo.Controllers
{
    public class BondRenewalController : Controller
    {
        OracleEntitiesConnStr db = new OracleEntitiesConnStr();
        SessionAttributeRetreival session = new SessionAttributeRetreival();
        public ActionResult Index()
        {
            return View();
        }
        public List<RenewalViewModel> getRenewalInfo(int bonderSlNo)
        {
            RenewalViewModel renewalviewmodel = new RenewalViewModel();
            renewalviewmodel.Bondstatus = db.BONDSTATUS.Where(y => y.BONDERSLNO == bonderSlNo).ToList();
            renewalviewmodel.Bonder = db.BONDERs.Where(x => x.BONDERSLNO == bonderSlNo).ToList();
            renewalviewmodel.DocumentAttachments = db.DOCUMENTATTACHMENTs.Where(x => x.BONDERSLNO == bonderSlNo).ToList();
            List<RenewalViewModel> viewModelList = new List<RenewalViewModel>();
            viewModelList.Add(renewalviewmodel);
            return viewModelList;
        }
        //public ActionResult Search()
        //{
        //    return View("Index");
        //}
        //[HttpPost]
        public ActionResult Search(string BondLicenseNo)
        {


            if (!(System.Web.HttpContext.Current.User.IsInRole("Bonder")))
            {
                var bonderslno = (from b in db.BONDERs where b.BONDLICENSENO == BondLicenseNo select b.BONDERSLNO).SingleOrDefault();
                System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();

                System.Data.OracleClient.OracleConnection conn = ConnectBMS.Connection();

                cmd.Connection = conn;

                System.Data.OracleClient.OracleTransaction bmsTransaction = conn.BeginTransaction();
                cmd.Transaction = bmsTransaction;
                cmd.CommandText = "select STATUS,BSDATE,SUBMITTEDBYNM,REMARKS from BONDSTATUS where BONDERSLNO=:BONDERSLNO ";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("BONDERSLNO", bonderslno);
                System.Data.OracleClient.OracleDataReader drBondStatus = cmd.ExecuteReader();
                if (drBondStatus.HasRows)
                {
                    drBondStatus.Read();
                    ViewBag.STATUS = drBondStatus.GetValue(0);
                    ViewBag.BSDATE = drBondStatus.GetValue(1);
                    ViewBag.SUBMITTEDBYNM = drBondStatus.GetValue(2);
                    ViewBag.REMARKS = drBondStatus.GetValue(3);
                }

                try
                {
                    return View(getRenewalInfo(bonderslno));
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    ViewBag.Message = "Insertion Failed";
                    return View("RenewalForm");
                }
                return RedirectToAction("Index");
            }
            else
            {
                USERPERMISSION permission = session.getStoredUserPermission();
                var bonderName = permission.BONDER.BONDERNAME;
                var bonderslno = (from b in db.BONDERs where b.BONDLICENSENO == BondLicenseNo && b.BONDERNAME == bonderName select b.BONDERSLNO).SingleOrDefault();
                System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();

                System.Data.OracleClient.OracleConnection conn = ConnectBMS.Connection();

                cmd.Connection = conn;

                System.Data.OracleClient.OracleTransaction bmsTransaction = conn.BeginTransaction();
                cmd.Transaction = bmsTransaction;
                cmd.CommandText = "select STATUS,BSDATE,SUBMITTEDBYNM,REMARKS from BONDSTATUS where BONDERSLNO=:BONDERSLNO";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("BONDERSLNO", bonderslno);
                System.Data.OracleClient.OracleDataReader drBondStatus = cmd.ExecuteReader();
                if (drBondStatus.HasRows)
                {
                    drBondStatus.Read();
                    ViewBag.STATUS = drBondStatus.GetValue(0);
                    ViewBag.BSDATE = drBondStatus.GetValue(1);
                    ViewBag.SUBMITTEDBYNM = drBondStatus.GetValue(2);
                    ViewBag.REMARKS = drBondStatus.GetValue(3);
                }

                try
                {
                    return View(getRenewalInfo(bonderslno));
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    ViewBag.Message = "Insertion Failed";
                    return View("RenewalForm");
                }
                return RedirectToAction("Index");
            }


        }

        [HttpPost]
        public ActionResult Renewinfo(RenewalViewModel rvm, string[] doc, string[] rgno, string[] issuedate, string[] expdate, HttpPostedFileBase[] files)
        {
            System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();

            System.Data.OracleClient.OracleConnection conn = ConnectBMS.Connection();

            cmd.Connection = conn;

            System.Data.OracleClient.OracleTransaction bmsTransaction = conn.BeginTransaction();
            cmd.Transaction = bmsTransaction;
            cmd.CommandText = "update BONDSTATUS set STATUS=:STATUS,BSDATE=:BSDATE,SUBMITTEDBYNM=:SUBMITTEDBYNM,REMARKS=:REMARKS"
               + " where BONDERSLNO=:BONDERSLNO";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("STATUS", Request["STATUS"]);

            if (!String.IsNullOrEmpty(Request["ApplicationSubmissionDate"]))
                cmd.Parameters.Add(new OracleParameter(":BSDATE", OracleType.DateTime)).Value = Request["ApplicationSubmissionDate"];
            else
                cmd.Parameters.Add(new OracleParameter(":BSDATE", OracleType.DateTime)).Value = DBNull.Value;

            cmd.Parameters.AddWithValue("SUBMITTEDBYNM", Request["SubmittedBy"]);
            cmd.Parameters.AddWithValue("REMARKS", Request["Remarks"]);
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToRenew);
            try
            {
                try
                {
                    bmsTransaction = conn.BeginTransaction();
                }
                catch { }
                cmd.Transaction = bmsTransaction;
                cmd.ExecuteNonQuery();
                bmsTransaction.Commit();
            }
            catch
            {
                bmsTransaction.Rollback();
            }
            if (Request["STATUS"] == "Cm")
            {
                cmd.CommandText = "update BONDAPPLICATIONPROGRESS set READYFORAPP=:READYFORAPP where BONDERSLNO=:BONDERSLNO and BSNO=:BSNO";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToRenew);
                cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToRenew);
                cmd.Parameters.AddWithValue("READYFORAPP", "Y");
                try
                {
                    try
                    {
                        bmsTransaction = conn.BeginTransaction();
                    }
                    catch { }
                    cmd.Transaction = bmsTransaction;
                    cmd.ExecuteNonQuery();
                    bmsTransaction.Commit();
                }
                catch
                {
                    bmsTransaction.Rollback();
                }
            }
            int p = 1;
            if (doc != null)
            {
                try
                {
                    for (int i = 0; i < doc.Length; i++)
                    {
                        if (doc[i] != null && doc[i] != "")
                        {
                            DOCUMENTATTACHMENT D = new DOCUMENTATTACHMENT();
                            var path = "";

                            D.ATTCHSLNO = (Int16)p;
                            //foreach (var outitem in rvm)
                            //{
                            //    foreach (var item in outitem.Bonder)
                            //    {
                            D.BONDERSLNO = BondInfo.bondSlNoToRenew;
                            //}
                            //foreach (var item in outitem.Bondstatus)
                            //{
                            D.BSNO = BondInfo.BSNoToRenew;
                            //    }
                            //}
                            if (files[i] != null)
                            {
                                var filename = Path.GetFileName(files[i].FileName);
                                path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), filename);
                                files[i].SaveAs(path);
                                D.ATTACHFILENM = path;
                            }
                            D.DOCHEADINGNAME = doc[i];
                            D.RGATTCHNAME = rgno[i];
                            //D.ISSUEDATE = Convert.ToDateTime(issuedate[i], CultureInfo.CurrentCulture);
                            //D.EXPDATE = Convert.ToDateTime(expdate[i], CultureInfo.CurrentCulture);
                            if (!String.IsNullOrEmpty(issuedate[i]))
                                D.ISSUEDATE = DateTime.ParseExact(issuedate[i], "dd/MM/yyyy", null);
                            if (!String.IsNullOrEmpty(expdate[i]))
                                D.EXPDATE = DateTime.ParseExact(expdate[i], "dd/MM/yyyy", null);
                            db.DOCUMENTATTACHMENTs.Add(D);
                            p++;
                            db.SaveChanges();
                        }
                    }

                    ViewBag.Message = "Successfully Inserted";


                    //return View(renewalviewmodel);

                    //return Search(BIMS.CommonAppSet.BondInfo.bondLicenseNoToRenew);
                    //return View("Search",rvm);


                    //return View("RenewalForm");
                }

                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }

                    ViewBag.Message = "Insertion Failed";

                    return View("RenewalForm");
                }
            }

            cmd.CommandText = "select STATUS,BSDATE,SUBMITTEDBYNM,REMARKS from BONDSTATUS where BONDERSLNO=:BONDERSLNO";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToRenew);
            System.Data.OracleClient.OracleDataReader drBondStatus = cmd.ExecuteReader();
            if (drBondStatus.HasRows)
            {
                drBondStatus.Read();
                ViewBag.STATUS = drBondStatus.GetValue(0);
                ViewBag.BSDATE = drBondStatus.GetValue(1);
                ViewBag.SUBMITTEDBYNM = drBondStatus.GetValue(2);
                ViewBag.REMARKS = drBondStatus.GetValue(3);
            }
            return View("Search", getRenewalInfo(BondInfo.bondSlNoToRenew));

            //return View("RenewalForm");
        }

        public ActionResult ApplicantsList()
        {

            if (!(System.Web.HttpContext.Current.User.IsInRole("Bonder")))     // added By Mizan (18 Aug 2016)--
            {
                var docatt = db.DOCUMENTATTACHMENTs.Where(x => x.BONDER.BONDSTATUS == "R").
              GroupBy(x => new { x.BONDERSLNO, x.BONDER.BONDLICENSENO, x.BONDER.BONDERNAME }).
                                              Select(x => new { x.Key.BONDERSLNO, x.Key.BONDLICENSENO, x.Key.BONDERNAME }).ToList();

                List<BONDER> bonder = new List<BONDER>();

                foreach (var item in docatt)
                {
                    bonder.Add(
                                  new BONDER

                                  {

                                      BONDERSLNO = item.BONDERSLNO,

                                      BONDLICENSENO = item.BONDLICENSENO,

                                      BONDERNAME = item.BONDERNAME
                                  }
                        );
                }
                return View(bonder);
            }
            else                                                                         // added By Mizan (18 Aug 2016)--
            {
                USERPERMISSION permission = session.getStoredUserPermission();
                var bonderName = permission.BONDER.BONDERNAME;
                var docatt = db.DOCUMENTATTACHMENTs.Where(x => x.BONDER.BONDSTATUS == "R" && x.BONDER.BONDERNAME == bonderName).
                GroupBy(x => new { x.BONDERSLNO, x.BONDER.BONDLICENSENO, x.BONDER.BONDERNAME }).
                                                Select(x => new { x.Key.BONDERSLNO, x.Key.BONDLICENSENO, x.Key.BONDERNAME }).ToList();

                List<BONDER> bonder = new List<BONDER>();

                foreach (var item in docatt)
                {
                    bonder.Add(
                                  new BONDER

                                  {

                                      BONDERSLNO = item.BONDERSLNO,

                                      BONDLICENSENO = item.BONDLICENSENO,

                                      BONDERNAME = item.BONDERNAME
                                  }
                        );
                }
                return View(bonder);
            }

            //List<DOCUMENTATTACHMENT> documentattachment = new List<DOCUMENTATTACHMENT>();

            //foreach (var item in docatt)
            //{
            //    documentattachment.Add(
            //                  new DOCUMENTATTACHMENT

            //                    {

            //                        BONDERSLNO = item.BONDERSLNO,

            //                        DOCHEADINGNAMEBG = item.BONDLICENSENO,

            //                        RGATTCHNAMEBG = item.BONDERNAME
            //                    }
            //        );
            //}


        }

        public ActionResult ApplicantDetails(int? id)
        {

            List<DOCUMENTATTACHMENT> documentattachment = new List<DOCUMENTATTACHMENT>();

            if (id != 0)
            {


                ViewBag.BonderName = (from b in db.BONDERs
                                      where b.BONDERSLNO == id
                                      select b.BONDERNAME).SingleOrDefault();


                ViewBag.BonderLicenseNo = (from b in db.BONDERs
                                           where b.BONDERSLNO == id
                                           select b.BONDLICENSENO).SingleOrDefault();


                documentattachment = db.DOCUMENTATTACHMENTs.Where(x => x.BONDERSLNO == id).ToList();

                ViewBag.BonderSlno = id;
                ViewBag.BonderStatus = (from b in db.BONDSTATUS
                                        where b.BONDERSLNO == id
                                        select b.BSNO).SingleOrDefault();

                return View(documentattachment);
            }

            return View("ApplicantsList");

        }






        public ActionResult AddNewDocument(int? id, int? id2)
        {

            ViewBag.BonderSlno = id;
            ViewBag.BonderStatus = id2;

            return View();

        }

        [HttpPost]
        public ActionResult AddNewDocument(DOCUMENTATTACHMENT documentattachment_new, HttpPostedFileBase file, int? bonderslno, int? bsno)
        {
            DOCUMENTATTACHMENT documentattachment = new DOCUMENTATTACHMENT();

            string path = "";

            documentattachment.BONDERSLNO = (Int16)bonderslno;
            documentattachment.BSNO = (Int16)bsno;

            documentattachment.DOCHEADINGNAME = documentattachment_new.DOCHEADINGNAME;
            documentattachment.RGATTCHNAME = documentattachment_new.RGATTCHNAME;
            documentattachment.ISSUEDATE = documentattachment_new.ISSUEDATE;
            documentattachment.EXPDATE = documentattachment_new.EXPDATE;


            if (file != null && file.ContentLength > 0)
            {

                var filename = Path.GetFileName(file.FileName);
                path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), filename);
                file.SaveAs(path);
                documentattachment.ATTACHFILENM = path;


            }


            db.DOCUMENTATTACHMENTs.Add(documentattachment);
            db.SaveChanges();

            return RedirectToAction("ApplicantDetails", new { id = bonderslno });

        }



        public FileStreamResult DownloadFile(int? id)
        {

            DOCUMENTATTACHMENT documentattachment = new DOCUMENTATTACHMENT();

            documentattachment = db.DOCUMENTATTACHMENTs.Where(x => x.ATTCHSLNO == id).SingleOrDefault();

            string path = documentattachment.ATTACHFILENM.ToString();


            FileInfo info = new FileInfo(path);
            string value = info.Extension;
            string filename = info.Name;


            return File(new FileStream(path, FileMode.Open), value, filename);



        }


        public ActionResult Delete(int? id)
        {

            var Bonder = db.DOCUMENTATTACHMENTs.Where(x => x.BONDERSLNO == id);

            foreach (var b in Bonder)
            {

                db.DOCUMENTATTACHMENTs.Remove(b);


            }


            db.SaveChanges();

            ViewBag.Message = " Successfully Deleted";
            ViewBag.Action = "ApplicantsList";
            ViewBag.Controller = "BondReneal";



            return View("RenewalForm");

        }


        public ActionResult DeleteSingleDoc(int? id, bool fromCreate = false)
        {

            var doc = db.DOCUMENTATTACHMENTs.Where(x => x.ATTCHSLNO == id).SingleOrDefault();

            db.DOCUMENTATTACHMENTs.Remove(doc);

            db.SaveChanges();

            ViewBag.Message = " Successfully Deleted";
            ViewBag.Action = "ApplicantsList";
            ViewBag.Controller = "BondReneal";

            if (fromCreate == true)
            {
                return View("Search", getRenewalInfo(BondInfo.bondSlNoToRenew));
            }
            else
            {
                return RedirectToAction("ApplicantDetails", new { id = doc.BONDERSLNO });
            }

        }


        public ActionResult Edit(short id = 0)
        {
            DOCUMENTATTACHMENT documentattachment = db.DOCUMENTATTACHMENTs.Where(x => x.ATTCHSLNO == id).SingleOrDefault();
            if (documentattachment == null)
            {
                return HttpNotFound();
            }
            return View(documentattachment);
        }

        //
        // POST: /Bank/Edit/5


        [HttpPost]
        public ActionResult Edit(DOCUMENTATTACHMENT documentattachmentEdited, HttpPostedFileBase file)
        {

            DOCUMENTATTACHMENT documentattachment = new DOCUMENTATTACHMENT();


            //var bonderslno = (from b in db.DOCUMENTATTACHMENTs
            //                  where b.ATTCHSLNO == documentattachmentEdited.ATTCHSLNO
            //                  select b.BONDERSLNO).SingleOrDefault();

            //var bsno = (from b in db.DOCUMENTATTACHMENTs
            //            where b.ATTCHSLNO == documentattachmentEdited.ATTCHSLNO
            //            select b.BSNO).SingleOrDefault();

            if (ModelState.IsValid)
            {


                var path = "";

                documentattachment.ATTCHSLNO = documentattachmentEdited.ATTCHSLNO;
                documentattachment.BONDERSLNO = documentattachmentEdited.BONDERSLNO;
                documentattachment.BSNO = documentattachmentEdited.BSNO;





                if (file != null && file.ContentLength > 0)
                {

                    var filename = Path.GetFileName(file.FileName);
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), filename);
                    file.SaveAs(path);
                    documentattachment.ATTACHFILENM = path;


                }



                else
                {


                    documentattachment.ATTACHFILENM = documentattachmentEdited.ATTACHFILENM;



                }



                documentattachment.DOCHEADINGNAME = documentattachmentEdited.DOCHEADINGNAME;
                documentattachment.RGATTCHNAME = documentattachmentEdited.RGATTCHNAME;
                documentattachment.ISSUEDATE = documentattachmentEdited.ISSUEDATE;
                documentattachment.EXPDATE = documentattachmentEdited.EXPDATE;







                db.Entry(documentattachment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ApplicantDetails", new { id = documentattachmentEdited.BONDERSLNO });
            }
            return View(documentattachment);
        }



        // END BOND RENEWAL .................................................................................................................


    }
}

using BMSPhase2Demo.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity.Validation;
using System.Diagnostics;
using BMSPhase2Demo.Utils;
using System.Data.Entity.Infrastructure;
using BMSPhase2Demo.Util;

namespace BMSPhase2Demo.Controllers
{
    public class Back2BackLCController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();
        private int recordNumbers = GlobalConstants.recordNumbers;
        QuantityModel model = new QuantityModel();
        SessionAttributeRetreival sessionAttributeRetreival = new SessionAttributeRetreival();
        //
        // GET: /Back2BackLC/
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Index(string LCNo, string FromDate, string ToDate, string currentFilterLC, String currentFilterFrDate, String currentFilterToDate, int? page, int filterBonderId = 0, int BONDERID = 0)
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
                TempData.Remove("Message");
            }

            var requestType = this.HttpContext.Request.RequestType;
            System.Diagnostics.Debug.WriteLine("requestType = " + requestType);
            USERPERMISSION loggedinUser = new SessionAttributeRetreival().getStoredUserPermission();
            var backtobacklcs = db.BACKTOBACKLCs.OrderByDescending(b => b.ID).Include(b => b.BANKBRANCH).Include(b => b.BANK).Include(b => b.BONDER);
            int pageSize = recordNumbers;
            int pageNumber = (page ?? 1);
            if (LCNo != null || FromDate != null || ToDate != null||BONDERID!=0)
            {
                page = 1;
            }
            else
            {
                if (LCNo == null)
                {
                    LCNo = currentFilterLC;
                }
                if (FromDate == null)
                {
                    FromDate = currentFilterFrDate;
                }
                if (ToDate == null)
                {
                    ToDate = currentFilterToDate;
                }
                if (BONDERID == 0)
                {
                    BONDERID = filterBonderId;
                }
            }
            ViewBag.CurrentFilterLC = LCNo;
            ViewBag.CurrentFilterFrDate = FromDate;
            ViewBag.CurrentFilterToDate = ToDate;
            ViewBag.filterBonderId = BONDERID;
            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            if ("GET" == requestType)
            {
                if (LCNo != null || FromDate != null || ToDate != null || BONDERID!=0)
                {
                    ViewBag.resultofbonderID = BONDERID;
                    backtobacklcs = GetBackToBackRolesBySearchCriterial(LCNo, FromDate, ToDate,loggedinUser,BONDERID);
                }
                if (loggedinUser != null && loggedinUser.BONDERID != null)
                {
                    ViewBag.resultofbonderID = loggedinUser.BONDERID;
                    backtobacklcs = backtobacklcs.Where(i => i.BONDERID == loggedinUser.BONDERID).OrderByDescending(b => b.ID).Include(b => b.BANKBRANCH).Include(b => b.BANK).Include(b => b.BONDER);
                }
                else if (BONDERID == 0 && filterBonderId == 0 && !User.IsInRole("Bonder"))
                {
                    ViewBag.resultofbonderID = BONDERID;
                    List<BACKTOBACKLC> backtobacklc = new List<BACKTOBACKLC>();
                    return View(backtobacklc.ToPagedList(pageNumber, pageSize));
                }
                
                return View(backtobacklcs.ToList().ToPagedList(pageNumber, pageSize));
            }
            else if ("POST" == requestType)
            {
                if (BONDERID == 0 && filterBonderId == 0 && !User.IsInRole("Bonder"))
                {
                    ViewBag.resultofbonderID = BONDERID;
                    List<BACKTOBACKLC> backtobacklc = new List<BACKTOBACKLC>();
                    return View(backtobacklc.ToPagedList(pageNumber, pageSize));
                }
                if (!string.IsNullOrEmpty(LCNo))
                {
                    LCNo = LCNo.Trim();
                }
                System.Diagnostics.Debug.WriteLine("LCNo = " + LCNo + ", FromDate = " + FromDate + ", ToDate = " + ToDate);

                if (LCNo.Equals(String.Empty) && FromDate.Equals(String.Empty) && ToDate.Equals(String.Empty))
                {
                    System.Diagnostics.Debug.WriteLine("if");

                }
                else if ((LCNo != null && !LCNo.Equals(String.Empty)) && (FromDate != null && !FromDate.Equals(String.Empty)) && (ToDate != null && !ToDate.Equals(String.Empty)))
                {
                    System.Diagnostics.Debug.WriteLine("else if 1");
                    DateTime fromLCDate = Convert.ToDateTime(FromDate);
                    DateTime toLCDate = Convert.ToDateTime(ToDate);
                    backtobacklcs = backtobacklcs.Where(b => b.LCNUMBER.Contains(LCNo) && (b.LCDATE >= fromLCDate && b.LCDATE <= toLCDate)).Include(b => b.BANKBRANCH).OrderByDescending(b => b.ID);

                }
                else if ((FromDate != null && !FromDate.Equals(String.Empty)) && (ToDate != null && !ToDate.Equals(String.Empty)))
                {
                    System.Diagnostics.Debug.WriteLine("else if 2");
                    DateTime fromLCDate = Convert.ToDateTime(FromDate);
                    DateTime toLCDate = Convert.ToDateTime(ToDate);
                    backtobacklcs = backtobacklcs.Where(b => (b.LCDATE >= fromLCDate && b.LCDATE <= toLCDate)).Include(b => b.BANKBRANCH).OrderByDescending(b => b.ID);
                }
                else if (LCNo != null && !LCNo.Equals(String.Empty))
                {
                    System.Diagnostics.Debug.WriteLine("else if 3");
                    backtobacklcs = backtobacklcs.Where(b => b.LCNUMBER.Contains(LCNo)).Include(b => b.BANKBRANCH).OrderByDescending(b => b.ID);
                }
                else if (FromDate != null && !FromDate.Equals(String.Empty))
                {
                    System.Diagnostics.Debug.WriteLine("else if 4");
                    DateTime fromLCDate = Convert.ToDateTime(FromDate);
                    backtobacklcs = backtobacklcs.Where(b => b.LCDATE >= fromLCDate).OrderByDescending(b => b.ID);
                }
                else if (ToDate != null && !ToDate.Equals(String.Empty))
                {
                    System.Diagnostics.Debug.WriteLine("else if 5");
                    DateTime toLCDate = Convert.ToDateTime(ToDate);
                    backtobacklcs = backtobacklcs.Where(b => b.LCDATE <= toLCDate).OrderByDescending(b => b.ID);
                }
                if (loggedinUser.BONDERID > 0)
                {
                    ViewBag.resultofbonderID = loggedinUser.BONDERID;
                    backtobacklcs = backtobacklcs.Where(e => e.BONDERID == loggedinUser.BONDERID).OrderByDescending(b => b.ID);
                }
                else if (BONDERID > 0)
                {
                    ViewBag.resultofbonderID = BONDERID;
                    backtobacklcs = backtobacklcs.Where(e => e.BONDERID == BONDERID).OrderByDescending(b => b.ID);
                }
                if (loggedinUser != null && loggedinUser.BONDERID != null)
                {
                    ViewBag.resultofbonderID = loggedinUser.BONDERID;
                    backtobacklcs = backtobacklcs.Where(i => i.BONDERID == loggedinUser.BONDERID).OrderByDescending(b => b.ID).Include(b => b.BANKBRANCH).Include(b => b.BANK).Include(b => b.BONDER);
                }
                
            }
            else
            {
                // Error handel
            }
            pageSize = recordNumbers;
            pageNumber = (page ?? 1);
            
            return View(backtobacklcs.ToPagedList(pageNumber, pageSize));

        }


        private IQueryable<BACKTOBACKLC> GetBackToBackRolesBySearchCriterial(String LCNo, String FromDate, String ToDate, USERPERMISSION loggedinUser, int BONDERID)
        {
            var backtobacklcs = db.BACKTOBACKLCs.OrderByDescending(b => b.ID).Include(b => b.BANKBRANCH).Include(b => b.BANK).Include(b => b.BONDER);
            if (!string.IsNullOrEmpty(LCNo))
            {
                LCNo = LCNo.Trim();
            }
            System.Diagnostics.Debug.WriteLine("LCNo = " + LCNo + ", FromDate = " + FromDate + ", ToDate = " + ToDate);


            if ((LCNo != null && !LCNo.Equals(String.Empty)) && (FromDate != null && !FromDate.Equals(String.Empty)) && (ToDate != null && !ToDate.Equals(String.Empty)))
            {
                System.Diagnostics.Debug.WriteLine("else if 1");
                DateTime fromLCDate = Convert.ToDateTime(FromDate);
                DateTime toLCDate = Convert.ToDateTime(ToDate);
                backtobacklcs = backtobacklcs.Where(b => b.LCNUMBER.Contains(LCNo) && (b.LCDATE >= fromLCDate && b.LCDATE <= toLCDate)).Include(b => b.BANKBRANCH).OrderByDescending(b => b.ID);

            }
            else if ((FromDate != null && !FromDate.Equals(String.Empty)) && (ToDate != null && !ToDate.Equals(String.Empty)))
            {
                System.Diagnostics.Debug.WriteLine("else if 2");
                DateTime fromLCDate = Convert.ToDateTime(FromDate);
                DateTime toLCDate = Convert.ToDateTime(ToDate);
                backtobacklcs = backtobacklcs.Where(b => (b.LCDATE >= fromLCDate && b.LCDATE <= toLCDate)).Include(b => b.BANKBRANCH).OrderByDescending(b => b.ID);
            }
            else if (LCNo != null && !LCNo.Equals(String.Empty))
            {
                System.Diagnostics.Debug.WriteLine("else if 3");
                backtobacklcs = backtobacklcs.Where(b => b.LCNUMBER.Contains(LCNo)).Include(b => b.BANKBRANCH).OrderByDescending(b => b.ID);
            }
            else if (FromDate != null && !FromDate.Equals(String.Empty))
            {
                System.Diagnostics.Debug.WriteLine("else if 4");
                DateTime fromLCDate = Convert.ToDateTime(FromDate);
                backtobacklcs = backtobacklcs.Where(b => b.LCDATE >= fromLCDate).OrderByDescending(b => b.ID);
            }
            else if (ToDate != null && !ToDate.Equals(String.Empty))
            {
                System.Diagnostics.Debug.WriteLine("else if 5");
                DateTime toLCDate = Convert.ToDateTime(ToDate);
                backtobacklcs = backtobacklcs.Where(b => b.LCDATE <= toLCDate).OrderByDescending(b => b.ID);
            }
            if (loggedinUser.BONDERID > 0)
            {
                backtobacklcs = backtobacklcs.Where(e => e.BONDERID == loggedinUser.BONDERID).OrderByDescending(b => b.ID);
            }
            else if (BONDERID > 0)
            {
                backtobacklcs = backtobacklcs.Where(e => e.BONDERID == BONDERID).OrderByDescending(b => b.ID);
            }
            return backtobacklcs;
            ;
        }

        //
        // GET: /Back2BackLC/Details/5
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Details(short id = 0)
        {
            BACKTOBACKLC backtobacklc = db.BACKTOBACKLCs.Find(id);
            if (backtobacklc == null)
            {
                return HttpNotFound();
            }
            return View(backtobacklc);
        }

        //
        // GET: /Back2BackLC/Create
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Create()
        {
            ViewBag.BUYERSBANKBRANCHID = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME");
            ViewBag.BUYERSBANKID = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME");
            USERPERMISSION loggedinUser = sessionAttributeRetreival.getStoredUserPermission();
            if (loggedinUser != null && loggedinUser.BONDERID != null)
            {
                ViewBag.BONDERID = new SelectList(db.BONDERs.Where(b => b.BONDERSLNO == loggedinUser.BONDERID), "BONDERSLNO", "BONDERNAME", loggedinUser.BONDERID);
            }
            else
            {
                ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            }

            ViewData["QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text");

            ViewData["PRICEUNIT"] = new SelectList(model.COSTUNITLIST, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult AddRawMaterial(BACKTOBACKPRODUCT backtobackProduct)
        {
            USERPERMISSION userpermission = sessionAttributeRetreival.getStoredUserPermission();
            foreach (var item in backtobackProduct.RAWMATERIALs)
            {
                item.PRODUCTID = backtobackProduct.ID;
            }

            DateTime thisDay = DateTime.Now;
            var user = "User";

            System.Diagnostics.Debug.WriteLine("Time: " + thisDay + " ModelState.IsValid: " + ModelState.IsValid);

            // if (ModelState.IsValid)
            if (true)
            {
                try
                {
                    foreach (var rawMaterial in backtobackProduct.RAWMATERIALs)
                    {
                        var productId = rawMaterial.PRODUCTID;
                        System.Diagnostics.Debug.WriteLine("Time: " + thisDay + " productId: " + productId);
                        System.Diagnostics.Debug.WriteLine("Time: " + thisDay + " backtobackProduct.BACKTOBACKLCID: " + backtobackProduct.BACKTOBACKLCID);
                        using (var dbRawMaterial = new OracleEntitiesConnStr())
                        {
                            RAWMATERIAL newRawMaterial = new RAWMATERIAL();
                            newRawMaterial.RAWMATERIALCODE = rawMaterial.RAWMATERIALCODE;
                            newRawMaterial.QUANTITY = rawMaterial.QUANTITY;
                            newRawMaterial.UNIT = rawMaterial.UNIT;
                            newRawMaterial.PERMITTEDWASTE = rawMaterial.PERMITTEDWASTE;
                            newRawMaterial.PRODUCTID = productId;
                            if (userpermission != null)
                            {
                                newRawMaterial.CREATEDBY = userpermission.APPUSER.USERNAME;
                            }
                            newRawMaterial.CREATEDDATE = thisDay;

                            dbRawMaterial.RAWMATERIALs.Add(newRawMaterial);
                            dbRawMaterial.SaveChanges();
                        }
                    }

                    return RedirectToAction("Details/" + backtobackProduct.BACKTOBACKLCID);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Create(BACKTOBACKPRODUCTs): Time-" + thisDay + " Exception = " + e.StackTrace);
                }
            }

            return View();
        }

        //
        // POST: /Back2BackLC/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Create(BACKTOBACKLC backToBackLC)
        {
            USERPERMISSION loggedinUser = sessionAttributeRetreival.getStoredUserPermission();
            if (loggedinUser != null) {
                backToBackLC.CREATEDBY = loggedinUser.APPUSER.USERNAME;
            }
            if (loggedinUser != null && loggedinUser.BONDERID != null)
            {
                ViewBag.BONDERID = new SelectList(db.BONDERs.Where(b => b.BONDERSLNO == loggedinUser.BONDERID), "BONDERSLNO", "BONDERNAME", loggedinUser.BONDERID);
                
            }
            else
            {
                ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            }
            foreach (var item in backToBackLC.BACKTOBACKPRODUCTs)
            {
                item.BACKTOBACKLCID = backToBackLC.ID;
            }

            DateTime thisDay = DateTime.Now;


            System.Diagnostics.Debug.WriteLine("Time: " + thisDay + " ModelState.IsValid: " + ModelState.IsValid);

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            System.Diagnostics.Debug.WriteLine("Time: " + thisDay + " errors: " + errors);
            //if (ModelState.IsValid)
            if (true)
            {
                try
                {
                    if (backToBackLC.BACKTOBACKPRODUCTs != null && backToBackLC.BACKTOBACKPRODUCTs.Count > 0)
                    {
                        if (backToBackLC.BONDERID != null)
                        {
                            USERPERMISSION userpermission = sessionAttributeRetreival.getStoredUserPermission();
                            int bonderId = backToBackLC.BONDERID;
                            if (userpermission != null)
                            {

                                List<INBONDRAWMATERIAL> inbondRawMaterials = db.INBONDRAWMATERIALs.Where(inb => inb.INBOND.BONDERID == bonderId).ToList();
                                if (inbondRawMaterials != null)
                                {
                                    decimal bonderUsedQuantity = inbondRawMaterials.Sum(item => item.PRODUCTQUANTITY);
                                    decimal? bonderProvidedQuantity = backToBackLC.BACKTOBACKPRODUCTs.Sum(item => item.QUANTITY);
                                    if (bonderUsedQuantity < bonderProvidedQuantity)
                                    {
                                        ModelState.AddModelError("", "Your Remaining Inbond Raw Material Quantity " + bonderUsedQuantity);
                                        ViewBag.BUYERSBANKBRANCHID = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME", backToBackLC.BUYERSBANKBRANCHID);
                                        ViewBag.BUYERSBANKID = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME", backToBackLC.BUYERSBANKID);

                                        if (userpermission != null && userpermission.BONDERID != null)
                                        {
                                            ViewBag.BONDERID = new SelectList(db.BONDERs.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", userpermission.BONDERID);
                                        }
                                        else
                                        {
                                            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", backToBackLC.BONDERID);
                                        }
                                        ViewData["QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text", backToBackLC.QUANTITYUNIT);

                                        ViewData["PRICEUNIT"] = new SelectList(model.COSTUNITLIST, "Value", "Text", backToBackLC.PRICEUNIT);
                                        for (int i = 0; i < backToBackLC.BACKTOBACKPRODUCTs.Count(); i++)
                                        {
                                            ViewData["BACKTOBACKPRODUCTs[" + i + "].QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text", backToBackLC.BACKTOBACKPRODUCTs[i].QUANTITYUNIT);

                                        }
                                        return View(backToBackLC);
                                    }
                                }
                            }
                        }
                        BACKTOBACKLC newBacktobacklc = new BACKTOBACKLC();
                        newBacktobacklc.BUYERSNAME = backToBackLC.BUYERSNAME;
                        newBacktobacklc.BUYERSADDRESS = backToBackLC.BUYERSADDRESS;
                        newBacktobacklc.LCNUMBER = backToBackLC.LCNUMBER;
                        newBacktobacklc.LCDATE = backToBackLC.LCDATE;
                        newBacktobacklc.BUYERSBANKID = backToBackLC.BUYERSBANKID;
                        newBacktobacklc.BUYERSBANKBRANCHID = backToBackLC.BUYERSBANKBRANCHID;
                        newBacktobacklc.SHIPPINGDATE = backToBackLC.SHIPPINGDATE;
                        newBacktobacklc.CORRECTIONDATE = backToBackLC.CORRECTIONDATE;
                        newBacktobacklc.PRODUCTQUANTITY = backToBackLC.PRODUCTQUANTITY;
                        newBacktobacklc.QUANTITYUNIT = backToBackLC.QUANTITYUNIT;
                        newBacktobacklc.LCUSEDPRICE = backToBackLC.LCUSEDPRICE;
                        newBacktobacklc.PRICEUNIT = backToBackLC.PRICEUNIT;
                        newBacktobacklc.MASTERLCNUMBER = backToBackLC.MASTERLCNUMBER;
                        newBacktobacklc.MASTERLCCORRECTIONDATE = backToBackLC.MASTERLCCORRECTIONDATE;
                        newBacktobacklc.UDNUMBER = backToBackLC.UDNUMBER;
                        newBacktobacklc.UDDATE = backToBackLC.UDDATE;
                        newBacktobacklc.UDCORRECTIONDATE = backToBackLC.UDCORRECTIONDATE;
                        newBacktobacklc.UDPRODUCTDETAIL = backToBackLC.UDPRODUCTDETAIL;
                        newBacktobacklc.BONDERID = backToBackLC.BONDERID;

                        if (loggedinUser != null)
                        {
                            newBacktobacklc.CREATEDBY = loggedinUser.APPUSER.USERNAME;
                        }
                        newBacktobacklc.CREATEDDATE = thisDay;

                        db.BACKTOBACKLCs.Add(newBacktobacklc);
                        db.SaveChanges();

                        short lastBackToBackId = db.BACKTOBACKLCs.Max(item => item.ID);

                        foreach (var product in backToBackLC.BACKTOBACKPRODUCTs)
                        {
                            using (var dbBackToBackProduct = new OracleEntitiesConnStr())
                            {
                                BACKTOBACKPRODUCT newProduct = new BACKTOBACKPRODUCT();
                                newProduct.NAME = product.NAME;
                                newProduct.SIZEANDDETAIL = product.SIZEANDDETAIL;
                                newProduct.QUANTITY = product.QUANTITY;
                                newProduct.QUANTITYUNIT = product.QUANTITYUNIT;
                                newProduct.BACKTOBACKLCID = lastBackToBackId;
                                newProduct.CREATEDBY = backToBackLC.CREATEDBY;
                                newProduct.CREATEDDATE = thisDay;

                                System.Diagnostics.Debug.WriteLine("Time: " + thisDay + " BACKTOBACKPRODUCTs " + backToBackLC.BACKTOBACKPRODUCTs.Count());

                                dbBackToBackProduct.BACKTOBACKPRODUCTs.Add(newProduct);
                                dbBackToBackProduct.SaveChanges();

                                short lastProductId = db.BACKTOBACKPRODUCTs.Max(itemBack2Back => itemBack2Back.ID);

                                //System.Diagnostics.Debug.WriteLine("Time: " + thisDay + " lastProductId =  " + lastProductId);

                                foreach (var rawMaterial in product.RAWMATERIALs)
                                {
                                    using (var dbRawMaterial = new OracleEntitiesConnStr())
                                    {
                                        RAWMATERIAL newRawMaterial = new RAWMATERIAL();
                                        newRawMaterial.RAWMATERIALCODE = rawMaterial.RAWMATERIALCODE;
                                        newRawMaterial.QUANTITY = rawMaterial.QUANTITY;
                                        newRawMaterial.UNIT = rawMaterial.UNIT;
                                        newRawMaterial.PERMITTEDWASTE = rawMaterial.PERMITTEDWASTE;
                                        newRawMaterial.PRODUCTID = lastProductId;
                                        newRawMaterial.CREATEDDATE = thisDay;
                                        dbRawMaterial.RAWMATERIALs.Add(newRawMaterial);
                                        dbRawMaterial.SaveChanges();
                                    }
                                }
                            }
                        }

                    }
                    else
                    {

                        backToBackLC.CREATEDDATE = thisDay;

                        db.BACKTOBACKLCs.Add(backToBackLC);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                    {
                        // Get entry

                        DbEntityEntry entry = item.Entry;
                        string entityTypeName = entry.Entity.GetType().Name;

                        // Display or log error messages

                        foreach (DbValidationError subItem in item.ValidationErrors)
                        {
                            string message = string.Format("Error '{0}' occurred in {1} at {2}", subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                            System.Diagnostics.Debug.WriteLine("Create(BACKTOBACKPRODUCTs): Time-" + thisDay + " Exception = " + message);
                            ViewBag.error = message;
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Create(BACKTOBACKPRODUCTs): Time-" + thisDay + " Exception = " + e.StackTrace);
                }
            }

            ViewBag.BUYERSBANKBRANCHID = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME", backToBackLC.BUYERSBANKBRANCHID);
            ViewBag.BUYERSBANKID = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME", backToBackLC.BUYERSBANKID);


            ViewData["QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text", backToBackLC.QUANTITYUNIT);

            ViewData["PRICEUNIT"] = new SelectList(model.COSTUNITLIST, "Value", "Text", backToBackLC.PRICEUNIT);
            for (int i = 0; i < backToBackLC.BACKTOBACKPRODUCTs.Count(); i++)
            {
                ViewData["BACKTOBACKPRODUCTs[" + i + "].QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text", backToBackLC.BACKTOBACKPRODUCTs[i].QUANTITYUNIT);

            }
            return View(backToBackLC);
        }

        //
        // GET: /Back2BackLC/Edit/5
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Edit(short id = 0)
        {
            BACKTOBACKLC backtobacklc = db.BACKTOBACKLCs.Find(id);
            if (backtobacklc == null)
            {
                return HttpNotFound();
            }
            ViewBag.BUYERSBANKBRANCHID = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME", backtobacklc.BUYERSBANKBRANCHID);
            ViewBag.BUYERSBANKID = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME", backtobacklc.BUYERSBANKID);
            USERPERMISSION loggedinUser = sessionAttributeRetreival.getStoredUserPermission();
            if (loggedinUser != null && loggedinUser.BONDERID != null)
            {
                ViewBag.BONDERID = new SelectList(db.BONDERs.Where(b => b.BONDERSLNO == loggedinUser.BONDERID), "BONDERSLNO", "BONDERNAME", loggedinUser.BONDERID);
            }
            else
            {
                ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", backtobacklc.BONDERID);
            }

            //ViewBag.QUANTITYUNIT = new SelectList("Piece", "KG");
            ViewData["QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text", backtobacklc.QUANTITYUNIT);
            ViewData["PRICEUNIT"] = new SelectList(model.COSTUNITLIST, "Value", "Text", backtobacklc.PRICEUNIT);
            for (int i = 0; i < backtobacklc.BACKTOBACKPRODUCTs.Count(); i++)
            {
                ViewData["BACKTOBACKPRODUCTs[" + i + "].QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text", backtobacklc.BACKTOBACKPRODUCTs[i].QUANTITYUNIT);

            }
            return View(backtobacklc);
        }

        //
        // POST: /Back2BackLC/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Edit(BACKTOBACKLC backtobacklc)
        {
          
            USERPERMISSION loggedinUser = sessionAttributeRetreival.getStoredUserPermission();
            if (loggedinUser != null) {
                backtobacklc.MODIFIEDBY = loggedinUser.APPUSER.USERNAME;
            }
            if (loggedinUser != null && loggedinUser.BONDERID != null)
            {
                ViewBag.BONDERID = new SelectList(db.BONDERs.Where(b => b.BONDERSLNO == loggedinUser.BONDERID), "BONDERSLNO", "BONDERNAME", backtobacklc.BONDERID);
                
            }
            else
            {
                ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", backtobacklc.BONDERID);
            }
            foreach (var item in backtobacklc.BACKTOBACKPRODUCTs)
            {
                item.BACKTOBACKLCID = backtobacklc.ID;
            }

            DateTime thisDay = DateTime.Now;


            bool isAddMore = false;

            if (true)
            {
                try
                {
                    if (backtobacklc.BACKTOBACKPRODUCTs != null && backtobacklc.BACKTOBACKPRODUCTs.Count > 0)
                    {
                        if (backtobacklc.BONDERID != null)
                        {
                            USERPERMISSION userpermission = sessionAttributeRetreival.getStoredUserPermission();
                            int bonderId = backtobacklc.BONDERID;
                            if (userpermission != null)
                            {

                                List<INBONDRAWMATERIAL> inbondRawMaterials = db.INBONDRAWMATERIALs.Where(inb => inb.INBOND.BONDERID == bonderId).ToList();
                                if (inbondRawMaterials != null)
                                {
                                    decimal bonderUsedQuantity = inbondRawMaterials.Sum(item => item.PRODUCTQUANTITY);
                                    decimal? bonderProvidedQuantity = backtobacklc.BACKTOBACKPRODUCTs.Sum(item => item.QUANTITY);
                                    if (bonderUsedQuantity < bonderProvidedQuantity)
                                    {
                                        ModelState.AddModelError("", "Your Remaining Inbond Raw Material Quantity " + bonderUsedQuantity);
                                        ViewBag.BUYERSBANKBRANCHID = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME", backtobacklc.BUYERSBANKBRANCHID);
                                        ViewBag.BUYERSBANKID = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME", backtobacklc.BUYERSBANKID);

                                        if (userpermission != null && userpermission.BONDERID != null)
                                        {
                                            ViewBag.BONDERID = new SelectList(db.BONDERs.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", userpermission.BONDERID);
                                        }
                                        else
                                        {
                                            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", backtobacklc.BONDERID);
                                        }
                                        ViewData["QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text", backtobacklc.QUANTITYUNIT);

                                        ViewData["PRICEUNIT"] = new SelectList(model.COSTUNITLIST, "Value", "Text", backtobacklc.PRICEUNIT);
                                        for (int i = 0; i < backtobacklc.BACKTOBACKPRODUCTs.Count(); i++)
                                        {
                                            ViewData["BACKTOBACKPRODUCTs[" + i + "].QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text", backtobacklc.BACKTOBACKPRODUCTs[i].QUANTITYUNIT);

                                        }
                                        for (int i = 0; i < backtobacklc.BACKTOBACKPRODUCTs.Count(); i++)
                                        {
                                            return RedirectToAction("Addmore", new { errorMessage = "Your Remaining Inbond Raw Material Quantity " + bonderUsedQuantity, id = backtobacklc.ID });

                                        }
                                        return View(backtobacklc);
                                    }
                                }
                            }
                        }

                        System.Diagnostics.Debug.WriteLine("Edit(BACKTOBACKPRODUCTs): Time-" + thisDay + " BACKTOBACKPRODUCTs count = " + backtobacklc.BACKTOBACKPRODUCTs.Count);

                        foreach (var item in backtobacklc.BACKTOBACKPRODUCTs)
                        {
                            if (item.NAME != null)
                            {
                                System.Diagnostics.Debug.WriteLine("Edit(BACKTOBACKPRODUCTs): Time-" + thisDay + " item.ID = " + item.ID);
                                if (item.ID == 0)
                                {
                                    using (var db1 = new OracleEntitiesConnStr())
                                    {


                                        BACKTOBACKPRODUCT newItem = new BACKTOBACKPRODUCT();
                                        newItem.ID = item.ID;
                                        newItem.BACKTOBACKLCID = item.BACKTOBACKLCID;
                                        newItem.NAME = item.NAME;
                                        newItem.QUANTITY = item.QUANTITY;
                                        newItem.QUANTITYUNIT = item.QUANTITYUNIT;
                                        newItem.CREATEDDATE = thisDay;
                                        newItem.SIZEANDDETAIL = item.SIZEANDDETAIL;
                                        newItem.ID = item.ID;
                                        db1.BACKTOBACKPRODUCTs.Add(newItem);
                                        db1.SaveChanges();
                                    }
                                    short lastProductId = db.BACKTOBACKPRODUCTs.Max(itemBack2Back => itemBack2Back.ID);
                                    foreach (var rawMaterial in item.RAWMATERIALs)
                                    {
                                        using (var dbRawMaterial = new OracleEntitiesConnStr())
                                        {
                                            RAWMATERIAL newRawMaterial = new RAWMATERIAL();
                                            newRawMaterial.RAWMATERIALCODE = rawMaterial.RAWMATERIALCODE;
                                            newRawMaterial.QUANTITY = rawMaterial.QUANTITY;
                                            newRawMaterial.UNIT = rawMaterial.UNIT;
                                            newRawMaterial.PERMITTEDWASTE = rawMaterial.PERMITTEDWASTE;
                                            newRawMaterial.PRODUCTID = lastProductId;
                                            newRawMaterial.CREATEDDATE = thisDay;
                                            dbRawMaterial.RAWMATERIALs.Add(newRawMaterial);
                                            dbRawMaterial.SaveChanges();
                                        }
                                    }
                                }
                                if (item.ID > 0)
                                {
                                    using (var db1 = new OracleEntitiesConnStr())
                                    {
                                        BACKTOBACKPRODUCT newItem = new BACKTOBACKPRODUCT();
                                        newItem.ID = item.ID;
                                        newItem.BACKTOBACKLCID = item.BACKTOBACKLCID;
                                        newItem.NAME = item.NAME;
                                        newItem.QUANTITY = item.QUANTITY;
                                        newItem.QUANTITYUNIT = item.QUANTITYUNIT;
                                        newItem.CREATEDDATE = thisDay;
                                        newItem.SIZEANDDETAIL = item.SIZEANDDETAIL;
                                        newItem.ID = item.ID;
                                        db1.Entry(newItem).State = EntityState.Modified;
                                        db1.SaveChanges();
                                    }
                                    foreach (var rawMaterial in item.RAWMATERIALs)
                                    {
                                        using (var dbRawMaterial = new OracleEntitiesConnStr())
                                        {
                                            RAWMATERIAL newRawMaterial = new RAWMATERIAL();
                                            newRawMaterial.ID = rawMaterial.ID;
                                            newRawMaterial.RAWMATERIALCODE = rawMaterial.RAWMATERIALCODE;
                                            newRawMaterial.QUANTITY = rawMaterial.QUANTITY;
                                            newRawMaterial.UNIT = rawMaterial.UNIT;
                                            newRawMaterial.PERMITTEDWASTE = rawMaterial.PERMITTEDWASTE;
                                            newRawMaterial.PRODUCTID = item.ID;
                                            newRawMaterial.CREATEDDATE = thisDay;
                                            dbRawMaterial.Entry(newRawMaterial).State = EntityState.Modified;
                                            dbRawMaterial.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }


                        BACKTOBACKLC newBacktobacklc = new BACKTOBACKLC();
                        newBacktobacklc.BUYERSNAME = backtobacklc.BUYERSNAME;
                        newBacktobacklc.BUYERSADDRESS = backtobacklc.BUYERSADDRESS;
                        newBacktobacklc.LCNUMBER = backtobacklc.LCNUMBER;
                        newBacktobacklc.LCDATE = backtobacklc.LCDATE;
                        newBacktobacklc.BUYERSBANKID = backtobacklc.BUYERSBANKID;
                        newBacktobacklc.BUYERSBANKBRANCHID = backtobacklc.BUYERSBANKBRANCHID;
                        newBacktobacklc.SHIPPINGDATE = backtobacklc.SHIPPINGDATE;
                        newBacktobacklc.CORRECTIONDATE = backtobacklc.CORRECTIONDATE;
                        newBacktobacklc.PRODUCTQUANTITY = backtobacklc.PRODUCTQUANTITY;
                        newBacktobacklc.QUANTITYUNIT = backtobacklc.QUANTITYUNIT;
                        newBacktobacklc.LCUSEDPRICE = backtobacklc.LCUSEDPRICE;
                        newBacktobacklc.PRICEUNIT = backtobacklc.PRICEUNIT;
                        newBacktobacklc.MASTERLCNUMBER = backtobacklc.MASTERLCNUMBER;
                        newBacktobacklc.MASTERLCCORRECTIONDATE = backtobacklc.MASTERLCCORRECTIONDATE;
                        newBacktobacklc.UDNUMBER = backtobacklc.UDNUMBER;
                        newBacktobacklc.UDDATE = backtobacklc.UDDATE;
                        newBacktobacklc.UDCORRECTIONDATE = backtobacklc.UDCORRECTIONDATE;
                        newBacktobacklc.UDPRODUCTDETAIL = backtobacklc.UDPRODUCTDETAIL;
                        newBacktobacklc.BONDERID = backtobacklc.BONDERID;

                        newBacktobacklc.CREATEDBY = backtobacklc.CREATEDBY;
                        newBacktobacklc.CREATEDDATE = backtobacklc.CREATEDDATE;
                        if (loggedinUser != null)
                        {
                            newBacktobacklc.MODIFIEDBY = loggedinUser.APPUSER.USERNAME;
                        }
                        newBacktobacklc.MODIFIEDDATE = thisDay;
                        newBacktobacklc.ID = backtobacklc.ID;
                        db.Entry(newBacktobacklc).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");


                    }
                }
                catch (Exception e)
                {


                    ViewBag.BUYERSBANKBRANCHID = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME", backtobacklc.BUYERSBANKBRANCHID);
                    ViewBag.BUYERSBANKID = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME", backtobacklc.BUYERSBANKID);

                    ViewData["QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text", backtobacklc.QUANTITYUNIT);

                    ViewData["PRICEUNIT"] = new SelectList(model.COSTUNITLIST, "Value", "Text", backtobacklc.PRICEUNIT);
                    for (int i = 0; i < backtobacklc.BACKTOBACKPRODUCTs.Count(); i++)
                    {
                        if (backtobacklc.BACKTOBACKPRODUCTs[i].ID <= 0)
                        {
                            isAddMore = true;
                        }
                        ViewData["BACKTOBACKPRODUCTs[" + i + "].QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text", backtobacklc.BACKTOBACKPRODUCTs[i].QUANTITYUNIT);

                    }
                    var message = string.Join(" | ", ModelState.Values
                                               .SelectMany(v => v.Errors)
                                               .Select(c => c.ErrorMessage));
                    ViewBag.error = "all field values are required for product!";

                    if (isAddMore) {
                            return RedirectToAction("Addmore", new { errorMessage = message, id = backtobacklc.ID });
                    }

                    return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());

                }
            }
            return View(backtobacklc);
        }
    

        //
        // GET: /Back2BackLC/Delete/5
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult Delete(short id = 0)
        {
            BACKTOBACKLC backtobacklc = db.BACKTOBACKLCs.Find(id);
            if (backtobacklc == null)
            {
                return HttpNotFound();
            }
            return View(backtobacklc);
        }

        //
        // POST: /Back2BackLC/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Operation Admin")]
        public ActionResult DeleteConfirmed(short id)
        {
            //BACKTOBACKLC backtobacklc = db.BACKTOBACKLCs.Find(id);
            var backtobackLC = db.BACKTOBACKLCs.Include("BACKTOBACKPRODUCTs").Where(p => p.ID == id).FirstOrDefault();
            TempData["Message"] = string.Format("LC deleted successfully, Which Local LC No was {0}", backtobackLC.LCNUMBER);
            var product = backtobackLC.BACKTOBACKPRODUCTs.ToList();

            foreach (var data in product)
            {

                var rawmaterials = data.RAWMATERIALs.ToList();
                foreach (var rawmaterial in rawmaterials)
                {
                    db.RAWMATERIALs.Remove(rawmaterial);
                }

                db.BACKTOBACKPRODUCTs.Remove(data);
            }

            //TODO add delete logic
            db.BACKTOBACKLCs.Remove(backtobackLC);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult ProductEntryEditorRow()
        {
            ViewData["QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text");
            return PartialView("ProductEntryEditor");
        }

        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult RawMaterialEntry(short productId = 0)
        {
            System.Diagnostics.Debug.WriteLine("RawMaterialEntry(BACKTOBACKPRODUCTs): productId = " + productId);
            BACKTOBACKPRODUCT backtobackProduct = db.BACKTOBACKPRODUCTs.Find(productId);
            if (backtobackProduct == null)
            {
                return HttpNotFound();
            }
            return View(backtobackProduct);
        }
        /*public ActionResult navigateToExbondDetails(int? id) {
            int? exbond = db.EXBONDBACKTOBACKs.SingleOrDefault(e => e.BACKTOBACKID == id).EXBONDID;
            if (exbond != null) {
                return RedirectToAction("Details", "ExBond", new { });
            }
            return RedirectToAction("Details", "ExBond", new{ });
        }*/
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult RawMaterialEntryRow(string containerPrefix)
        {
            //System.Diagnostics.Debug.WriteLine("RawMaterialEntryRow(productId): productId = " + productId);

            ViewData["RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME");

            ViewData["UNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text");
            ViewData["ContainerPrefix"] = containerPrefix;

            return PartialView("RawMaterialEntryEditor", new RAWMATERIAL());
        }
        [HttpDelete]
        public ActionResult DeleteProducts(int id)
        {
        
            BACKTOBACKPRODUCT product = db.BACKTOBACKPRODUCTs.Find(id);
            try
            {
                if (product != null)
                {
                    if (product.RAWMATERIALs.Count() > 0)
                    {
                        var rawmaterials = product.RAWMATERIALs.ToList();
                        foreach (var rawmaterial in rawmaterials)
                        {
                            db.RAWMATERIALs.Remove(rawmaterial);
                        }
                    }
                    db.BACKTOBACKPRODUCTs.Remove(product);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("dilete(BACKTOBACKPRODUCTs):  Exception = " + e.StackTrace);
            }
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
          [HttpDelete]
        public ActionResult DeleteRawMaterial(int id)
        {
            RAWMATERIAL rawmaterial = db.RAWMATERIALs.Find(id);
            try
            {
                if (rawmaterial != null)
                {
                    db.RAWMATERIALs.Remove(rawmaterial);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {

            }
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
       
        public ActionResult Addmore(string errorMessage,short id = 0)
        {


            BACKTOBACKLC backtobacklc = db.BACKTOBACKLCs.Find(id);
            if (backtobacklc == null)
            {
                return HttpNotFound();
            }
            ViewBag.BUYERSBANKBRANCHID = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME", backtobacklc.BUYERSBANKBRANCHID);
            ViewBag.BUYERSBANKID = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME", backtobacklc.BUYERSBANKID);
            USERPERMISSION loggedinUser = sessionAttributeRetreival.getStoredUserPermission();
            if (loggedinUser != null && loggedinUser.BONDERID != null)
            {
                ViewBag.BONDERID = new SelectList(db.BONDERs.Where(b => b.BONDERSLNO == loggedinUser.BONDERID), "BONDERSLNO", "BONDERNAME", loggedinUser.BONDERID);
            }
            else
            {
                ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", backtobacklc.BONDERID);
            }

            //ViewBag.QUANTITYUNIT = new SelectList("Piece", "KG");
            ViewData["QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text", backtobacklc.QUANTITYUNIT);
            ViewData["PRICEUNIT"] = new SelectList(model.COSTUNITLIST, "Value", "Text", backtobacklc.PRICEUNIT);
            for (int i = 0; i < backtobacklc.BACKTOBACKPRODUCTs.Count(); i++)
            {
                ViewData["BACKTOBACKPRODUCTs[" + i + "].QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text", backtobacklc.BACKTOBACKPRODUCTs[i].QUANTITYUNIT);

            }
            if (errorMessage != null && !errorMessage.Trim().Equals(""))
            {
                ModelState.AddModelError("editerror", errorMessage);
                return View(backtobacklc);
            }
            return View(backtobacklc);

        }
    }
}
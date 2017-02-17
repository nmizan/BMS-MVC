using BMSPhase2Demo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using BMSPhase2Demo.Util;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using BMSPhase2Demo.Utils;
namespace BMSPhase2Demo.Controllers
{
    public class InBondController : Controller
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();
        private int recordNumbers = GlobalConstants.recordNumbers;
        SessionAttributeRetreival sessionAttributeRetreival = new SessionAttributeRetreival();
        QuantityModel unitModel = new QuantityModel();
        //
        // GET: /InBond/
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult Index(string LCNo, string FromDate, string ToDate, string currentFilterLC, String currentFilterFrDate, String currentFilterToDate, int? page, int filterBonderId = 0, int BONDERID = 0)
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
                TempData.Remove("Message");
            }            

            USERPERMISSION loggedinUser = sessionAttributeRetreival.getStoredUserPermission();
            var requestType = this.HttpContext.Request.RequestType;
            System.Diagnostics.Debug.WriteLine("requestType = " + requestType);
            var inbonds = db.INBONDs.Include(i => i.BONDER).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);

            List<INBOND> inbondList = inbonds.ToList();
            int pageSize = recordNumbers;
            int pageNumber = (page ?? 1);
            if (LCNo != null || FromDate != null || ToDate != null || BONDERID != 0)
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

                if (LCNo != null || FromDate != null || ToDate != null || BONDERID != 0)
                {
                    ViewBag.resultofbonderID = BONDERID;
                    inbonds = GetInbondsBySearchCriterial(LCNo, FromDate, ToDate, loggedinUser, BONDERID);
                }
                if (loggedinUser != null && loggedinUser.BONDERID != null)
                {
                    ViewBag.resultofbonderID = loggedinUser.BONDERID;
                    inbonds = inbonds.Where(i => i.BONDERID == loggedinUser.BONDERID).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);
                }
                else if (BONDERID == 0 && (page <= 1 || page == null))
                {
                    ViewBag.resultofbonderID = BONDERID;
                    List<INBOND> inbonds1 = new List<INBOND>();
                    return View(inbonds1.ToPagedList(pageNumber, pageSize));
                }
                return View(inbonds.ToPagedList(pageNumber, pageSize));

            }
            else if ("POST" == requestType)
            {
                if (BONDERID == 0 && filterBonderId == 0 && !User.IsInRole("Bonder"))
                {
                    ViewBag.resultofbonderID = BONDERID;
                    List<INBOND> inbond = new List<INBOND>();
                    return View(inbond.ToPagedList(pageNumber, pageSize));
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
                    inbonds = inbonds.Where(b => b.LCNUMBER.Contains(LCNo) && (b.LCDATE >= fromLCDate && b.LCDATE <= toLCDate)).OrderBy(b => b.BONDER.BONDERNAME).ThenByDescending(b => b.ID);
                    //if (loggedinUser.BONDERID > 0)
                    //{
                    //    inbonds = db.INBONDs.Where(b => b.LCNUMBER.Contains(LCNo) && (b.LCDATE >= fromLCDate && b.LCDATE <= toLCDate) && (b.BONDERID = loggedinUser.BONDERID)).OrderBy(b => b.BONDER.BONDERNAME).ThenByDescending(b => b.ID);
                    //}
                }
                else if ((FromDate != null && !FromDate.Equals(String.Empty)) && (ToDate != null && !ToDate.Equals(String.Empty)))
                {
                    System.Diagnostics.Debug.WriteLine("else if 2");
                    DateTime fromLCDate = Convert.ToDateTime(FromDate);
                    DateTime toLCDate = Convert.ToDateTime(ToDate);
                    inbonds = inbonds.Where(b => (b.LCDATE >= fromLCDate && b.LCDATE <= toLCDate)).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);
                }
                else if (LCNo != null && !LCNo.Equals(String.Empty))
                {
                    System.Diagnostics.Debug.WriteLine("else if 3");
                    inbonds = inbonds.Where(b => b.LCNUMBER.Contains(LCNo)).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);
                }
                else if (FromDate != null && !FromDate.Equals(String.Empty))
                {
                    System.Diagnostics.Debug.WriteLine("else if 4");
                    DateTime fromLCDate = Convert.ToDateTime(FromDate);
                    inbonds = inbonds.Where(b => b.LCDATE >= fromLCDate).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);
                }
                else if (ToDate != null && !ToDate.Equals(String.Empty))
                {
                    System.Diagnostics.Debug.WriteLine("else if 5");
                    DateTime toLCDate = Convert.ToDateTime(ToDate);
                    inbonds = inbonds.Where(b => b.LCDATE <= toLCDate).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);
                }

                //System.Diagnostics.Debug.WriteLine("backtobacklcs " + backtobacklcs)
                if (loggedinUser.BONDERID > 0)
                {
                    ViewBag.resultofbonderID = loggedinUser.BONDERID;
                    inbonds = inbonds.Where(e => e.BONDERID == loggedinUser.BONDERID).OrderByDescending(b => b.ID);
                }
                else if (BONDERID > 0)
                {
                    ViewBag.resultofbonderID = BONDERID;
                    inbonds = inbonds.Where(e => e.BONDERID == BONDERID).OrderByDescending(b => b.ID);
                }
                if (loggedinUser != null && loggedinUser.BONDERID != null)
                {
                    ViewBag.resultofbonderID = loggedinUser.BONDERID;
                    inbonds = inbonds.Where(i => i.BONDERID == loggedinUser.BONDERID).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);
                }
            }
            else
            {
                // Error handel
            }
            pageSize = recordNumbers;
            pageNumber = (page ?? 1);
            return View(inbonds.ToPagedList(pageNumber, pageSize));
        }
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        private IOrderedQueryable<INBOND> GetInbondsBySearchCriterial(String LCNo, String FromDate, String ToDate, USERPERMISSION loggedinUser, int BONDERID)
        {
            var inbonds = db.INBONDs.Include(i => i.BONDER).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);
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
                inbonds = inbonds.Where(b => b.LCNUMBER.Contains(LCNo) && (b.LCDATE >= fromLCDate && b.LCDATE <= toLCDate)).OrderBy(b => b.BONDER.BONDERNAME).ThenByDescending(b => b.ID);

            }
            else if ((FromDate != null && !FromDate.Equals(String.Empty)) && (ToDate != null && !ToDate.Equals(String.Empty)))
            {
                System.Diagnostics.Debug.WriteLine("else if 2");
                DateTime fromLCDate = Convert.ToDateTime(FromDate);
                DateTime toLCDate = Convert.ToDateTime(ToDate);
                inbonds = inbonds.Where(b => (b.LCDATE >= fromLCDate && b.LCDATE <= toLCDate)).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);
            }
            else if (LCNo != null && !LCNo.Equals(String.Empty))
            {
                System.Diagnostics.Debug.WriteLine("else if 3");
                inbonds = inbonds.Where(b => b.LCNUMBER.Contains(LCNo)).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);
            }
            else if (FromDate != null && !FromDate.Equals(String.Empty))
            {
                System.Diagnostics.Debug.WriteLine("else if 4");
                DateTime fromLCDate = Convert.ToDateTime(FromDate);
                inbonds = inbonds.Where(b => b.LCDATE >= fromLCDate).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);
            }
            else if (ToDate != null && !ToDate.Equals(String.Empty))
            {
                System.Diagnostics.Debug.WriteLine("else if 5");
                DateTime toLCDate = Convert.ToDateTime(ToDate);
                inbonds = inbonds.Where(b => b.LCDATE <= toLCDate).OrderBy(i => i.BONDER.BONDERNAME).ThenByDescending(i => i.ID);
            }
            if (loggedinUser.BONDERID > 0)
            {
                inbonds = inbonds.Where(e => e.BONDERID == loggedinUser.BONDERID).OrderByDescending(b => b.ID);
            }
            else if (BONDERID > 0)
            {
                inbonds = inbonds.Where(e => e.BONDERID == BONDERID).OrderByDescending(b => b.ID);
            }
            return inbonds;
        }
        //
        // GET: /InBond/Details/5
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult Details(short id = 0)
        {
            INBOND inbond = db.INBONDs.Find(id);
            if (inbond == null)
            {
                return HttpNotFound();
            }
            return View(inbond);
        }

        //
        // GET: /InBond/Create
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult Create()
        {
            var bonders = db.BONDERs;
            USERPERMISSION userpermission = sessionAttributeRetreival.getStoredUserPermission();
            if (User.IsInRole("Bonder") && userpermission != null)
            {
                ViewBag.BONDERID = new SelectList(bonders.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", userpermission.BONDERID);
            }
            else if (User.IsInRole("Operation Admin") && userpermission != null)
            {
                ViewBag.BONDERID = new SelectList(bonders, "BONDERSLNO", "BONDERNAME");
            }

            //INBONDRAWMATERIAL rawMaterials = new INBONDRAWMATERIAL();
            //IList<INBONDRAWMATERIAL> INBONDRAWMATERIALs = new List<INBONDRAWMATERIAL>();
            //INBONDRAWMATERIALs.Add(rawMaterials);
            //INBOND inbond = new INBOND();
            //inbond.INBONDRAWMATERIALs = INBONDRAWMATERIALs;
            return View();

        }

        //
        // POST: /InBond/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult Create(INBOND inbond)
        {
            var bonders = db.BONDERs;
            USERPERMISSION userpermission = sessionAttributeRetreival.getStoredUserPermission();
            if (userpermission != null) {
                inbond.CREATEDBY = userpermission.APPUSER.USERNAME;   
            }
            if (User.IsInRole("Bonder") && userpermission != null)
            {
                ViewBag.BONDERID = new SelectList(bonders.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
            }
            else if (User.IsInRole("Operation Admin") && userpermission != null)
            {
                ViewBag.BONDERID = new SelectList(bonders, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
            }
            if (inbond.INBONDRAWMATERIALs != null)
            {
                foreach (var item in inbond.INBONDRAWMATERIALs)
                {
                    item.INBONDID = inbond.ID;
                }
                if (inbond.BONDERID != null)
                {
                    userpermission = sessionAttributeRetreival.getStoredUserPermission();
                    int bonderId = inbond.BONDERID;
                    if (userpermission != null)
                    {
                        List<ANNUALENTLRAWMATERIAL> annualEntitlementRawMaterials = db.ANNUALENTLRAWMATERIALs.Where(eb => eb.BONDERSLNO == bonderId).ToList();
                        if (annualEntitlementRawMaterials != null && annualEntitlementRawMaterials.Count() > 0)
                        {
                            foreach (ANNUALENTLRAWMATERIAL material in annualEntitlementRawMaterials) { 
                            ANNUALENTLRAWMATERIAL annualEntitlementRawMaterial = material;
                            BONDERANNUALENTITLEMENT bonderAnnualEntitlement = db.BONDERANNUALENTITLEMENTs.SingleOrDefault(be => be.AESLNO == annualEntitlementRawMaterial.AESLNO && be.BONDERSLNO == bonderId);
                            if (bonderAnnualEntitlement != null)
                            {
                                DateTime today = DateTime.Today;
                                List<BONDERANNUALENTITLEMENT> annualEntitleMentForDateRanges = db.BONDERANNUALENTITLEMENTs.Where(dt => dt.ENTITLEFROM <= today && dt.ENTITLETO >= today).ToList();
                                if (annualEntitleMentForDateRanges != null && annualEntitleMentForDateRanges.Count() == 0)
                                {
                                    ModelState.AddModelError("", "No Annual Entitilement Exists for This Current Date");
                                    if (User.IsInRole("Bonder") && userpermission != null)
                                    {
                                        ViewBag.BONDERID = new SelectList(bonders.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                                    }
                                    else if (User.IsInRole("Operation Admin") && userpermission != null)
                                    {
                                        ViewBag.BONDERID = new SelectList(bonders, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                                    }
                                    for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
                                    {

                                        int bonderid=inbond.BONDERID;
                                        /*if (bonderid != 0)
                                        {

                                            if (userpermission != null)
                                            {
                                                List<ANNUALENTLRAWMATERIAL> materials = db.ANNUALENTLRAWMATERIALs.Where(eb => eb.BONDERSLNO == bonderId).ToList();
                                                if (annualEntitlementRawMaterials != null && annualEntitlementRawMaterials.Count() > 0)
                                                {
                                                    ANNUALENTLRAWMATERIAL mtrl = annualEntitlementRawMaterials.First();
                                                    List<MATERIAL> mtrls = new List<MATERIAL>();
                                                    foreach (ANNUALENTLRAWMATERIAL mtr in annualEntitlementRawMaterials)
                                                    {
                                                        mtrls.Add(db.MATERIALS.Where(b => b.MTYPE.Equals("r") && b.MSLNO == mtr.MSLNO).First());
                                                    }
                                                    ViewData["INBONDRAWMATERIALs[" + i + "].RAWMATERIALCODE"] = new SelectList(mtrls, "MSLNO", "MATERIALNAME");

                                                }
                                            }
                                        }*/
                                        ViewData["INBONDRAWMATERIALs[" + i + "].RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME", inbond.INBONDRAWMATERIALs[i].RAWMATERIALCODE);
                                        ViewData["INBONDRAWMATERIALs[" + i + "].QUANTITYUNIT"] = new SelectList(unitModel.QUANTITYUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].QUANTITYUNIT);
                                        ViewData["INBONDRAWMATERIALs[" + i + "].COSTUNIT"] = new SelectList(unitModel.COSTUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].COSTUNIT);
                                    }
                                    return View(inbond);
                                }
                                decimal? totalQuantity = annualEntitlementRawMaterial.AEQTY;
                                MATERIAL matrl = db.MATERIALS.SingleOrDefault(me => me.MSLNO == material.MSLNO);
                                List<INBONDRAWMATERIAL> inbondRawMaterials = db.INBONDRAWMATERIALs.Where(inb => inb.INBOND.BONDERID == bonderId&&inb.RAWMATERIALCODE==matrl.MSLNO).ToList();
                                if (inbondRawMaterials != null)
                                {
                                    decimal sum = addRawMaterialQuantity(inbondRawMaterials);
                                    //bonderUsedQuantity = inbondRawMaterials.Sum(item => item.PRODUCTQUANTITY);
                                    decimal bonderUsedQuantity = sum;
                                    sum = addRawMaterialQuantity(inbond.INBONDRAWMATERIALs.Where(ib=>ib.RAWMATERIALCODE==matrl.MSLNO).ToList());
                                    //decimal bonderProvidedQuantity = inbond.INBONDRAWMATERIALs.Sum(item => item.PRODUCTQUANTITY);
                                    decimal bonderProvidedQuantity = sum;
                                    //decimal bonderUsedQuantity = inbondRawMaterials.Sum(item => item.PRODUCTQUANTITY);
                                    //decimal bonderProvidedQuantity = inbond.INBONDRAWMATERIALs.Sum(item => item.PRODUCTQUANTITY);
                                    if ((totalQuantity - bonderUsedQuantity) < bonderProvidedQuantity)
                                    {
                                        ModelState.AddModelError("", "You can in bond maximum " + (totalQuantity - bonderUsedQuantity) + " TONS of "+ db.MATERIALS.SingleOrDefault(m=>m.MSLNO==material.MSLNO).MATERIALNAME+" for this period");
                                        if (User.IsInRole("Bonder") && userpermission != null)
                                        {
                                            ViewBag.BONDERID = new SelectList(bonders.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                                        }
                                        else if (User.IsInRole("Operation Admin") && userpermission != null)
                                        {
                                            ViewBag.BONDERID = new SelectList(bonders, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                                        }
                                        for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
                                        {
                                            ViewData["INBONDRAWMATERIALs[" + i + "].RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME", inbond.INBONDRAWMATERIALs[i].RAWMATERIALCODE);
                                            ViewData["INBONDRAWMATERIALs[" + i + "].QUANTITYUNIT"] = new SelectList(unitModel.QUANTITYUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].QUANTITYUNIT);
                                            ViewData["INBONDRAWMATERIALs[" + i + "].COSTUNIT"] = new SelectList(unitModel.COSTUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].COSTUNIT);
                                        }
                                        return View(inbond);
                                    }
                                }

                            }
                            else
                            {
                                ModelState.AddModelError("", "No Bonder Annual Entitlement For This User");
                                if (User.IsInRole("Bonder") && userpermission != null)
                                {
                                    ViewBag.BONDERID = new SelectList(bonders.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                                }
                                else if (User.IsInRole("Operation Admin") && userpermission != null)
                                {
                                    ViewBag.BONDERID = new SelectList(bonders, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                                }
                                for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
                                {

                                    ViewData["INBONDRAWMATERIALs[" + i + "].RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME", inbond.INBONDRAWMATERIALs[i].RAWMATERIALCODE);
                                    ViewData["INBONDRAWMATERIALs[" + i + "].QUANTITYUNIT"] = new SelectList(unitModel.QUANTITYUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].QUANTITYUNIT);
                                    ViewData["INBONDRAWMATERIALs[" + i + "].COSTUNIT"] = new SelectList(unitModel.COSTUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].COSTUNIT);
                                }
                                return View(inbond);
                            }
                        }

                        }
                        else
                        {
                            ModelState.AddModelError("", "No Annual Entitlement For This User");
                            if (User.IsInRole("Bonder") && userpermission != null)
                            {
                                ViewBag.BONDERID = new SelectList(bonders.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                            }
                            else if (User.IsInRole("Operation Admin") && userpermission != null)
                            {
                                ViewBag.BONDERID = new SelectList(bonders, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                            }
                            for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
                            {

                                ViewData["INBONDRAWMATERIALs[" + i + "].RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME", inbond.INBONDRAWMATERIALs[i].RAWMATERIALCODE);
                                ViewData["INBONDRAWMATERIALs[" + i + "].QUANTITYUNIT"] = new SelectList(unitModel.QUANTITYUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].QUANTITYUNIT);
                                ViewData["INBONDRAWMATERIALs[" + i + "].COSTUNIT"] = new SelectList(unitModel.COSTUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].COSTUNIT);
                            }
                            return View(inbond);
                        }

                    }
                }
            }
            DateTime thisDay = DateTime.Today;
            try
            {
                if (ModelState.IsValid)
                {


                    if (inbond.INBONDRAWMATERIALs != null && inbond.INBONDRAWMATERIALs.Count > 0)
                    {
                        INBOND newInbond = new INBOND();
                        newInbond.LCNUMBER = inbond.LCNUMBER;
                        newInbond.LCDATE = inbond.LCDATE;
                        newInbond.BOENUMBER = inbond.BOENUMBER;
                        newInbond.BOEDATE = inbond.BOEDATE;
                        newInbond.MODIFIEDBY = inbond.MODIFIEDBY;
                        newInbond.MODIFIEDDATE = inbond.MODIFIEDDATE;
                        newInbond.BONDERID = inbond.BONDERID;
                        newInbond.CREATEDDATE = thisDay;
                        newInbond.CREATEDBY = inbond.CREATEDBY;
                        db.INBONDs.Add(newInbond);
                        db.SaveChanges();
                        short lastProductId = db.INBONDs.Max(item => item.ID);
                        foreach (var item in inbond.INBONDRAWMATERIALs)
                        {
                            using (var db1 = new OracleEntitiesConnStr())
                            {
                                INBONDRAWMATERIAL newRawmaterial = new INBONDRAWMATERIAL();
                                newRawmaterial.RAWMATERIALCODE = item.RAWMATERIALCODE;
                                newRawmaterial.PRODUCTCOST = item.PRODUCTCOST;
                                newRawmaterial.PRODUCTQUANTITY = item.PRODUCTQUANTITY;
                                newRawmaterial.QUANTITYUNIT = item.QUANTITYUNIT;
                                newRawmaterial.COSTUNIT = item.COSTUNIT;
                                newRawmaterial.INBONDID = lastProductId;
                                newRawmaterial.CREATEDDATE = thisDay;
                                db1.INBONDRAWMATERIALs.Add(newRawmaterial);
                                db1.SaveChanges();
                            }

                        }

                    }
                    else
                    {
                        inbond.CREATEDDATE = thisDay;
                        db.INBONDs.Add(inbond);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    var message = string.Join(" | ", ModelState.Values
                                               .SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage));

                    ModelState.AddModelError(string.Empty, message.ToString());
                    if (User.IsInRole("Bonder") && userpermission != null)
                    {
                        ViewBag.BONDERID = new SelectList(bonders.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                    }
                    else if (User.IsInRole("Operation Admin") && userpermission != null)
                    {
                        ViewBag.BONDERID = new SelectList(bonders, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                    }
                    //return View(inbond);
                }
            }
            catch (Exception ex)
            {

                string message = string.Format("Error : '{0}' ", ex.ToString());
                if (User.IsInRole("Bonder") && userpermission != null)
                {
                    ViewBag.BONDERID = new SelectList(bonders.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                }
                else if (User.IsInRole("Operation Admin") && userpermission != null)
                {
                    ViewBag.BONDERID = new SelectList(bonders, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                }
                ModelState.AddModelError(string.Empty, message);

            }
            for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
            {

                ViewData["INBONDRAWMATERIALs[" + i + "].RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME", inbond.INBONDRAWMATERIALs[i].RAWMATERIALCODE);
                ViewData["INBONDRAWMATERIALs[" + i + "].QUANTITYUNIT"] = new SelectList(unitModel.QUANTITYUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].QUANTITYUNIT);
                ViewData["INBONDRAWMATERIALs[" + i + "].COSTUNIT"] = new SelectList(unitModel.COSTUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].COSTUNIT);
            }
            return View(inbond);
        }

        //
        // GET: /InBond/Edit/5
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult Edit(short id = 0)
        {

            INBOND inbond = db.INBONDs.Find(id);
            ViewBag.error = Request.QueryString["error"];
            if (inbond == null)
            {
                return HttpNotFound();
            }
            for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
            {

                ViewData["INBONDRAWMATERIALs[" + i + "].RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME", inbond.INBONDRAWMATERIALs[i].RAWMATERIALCODE);
                ViewData["INBONDRAWMATERIALs[" + i + "].QUANTITYUNIT"] = new SelectList(unitModel.QUANTITYUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].QUANTITYUNIT);
                ViewData["INBONDRAWMATERIALs[" + i + "].COSTUNIT"] = new SelectList(unitModel.COSTUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].COSTUNIT);
            }

            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
            return View(inbond);
        }

        //
        // POST: /InBond/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult Edit(INBOND inbond)
        {
            var bonders = db.BONDERs;
            USERPERMISSION userpermission = sessionAttributeRetreival.getStoredUserPermission();
            if (userpermission != null) { 
                inbond.MODIFIEDBY = userpermission.APPUSER.USERNAME; 
            }
            
            if (User.IsInRole("Bonder") && userpermission != null)
            {
                ViewBag.BONDERID = new SelectList(bonders.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                
            }
            else if (User.IsInRole("Operation Admin") && userpermission != null)
            {
                ViewBag.BONDERID = new SelectList(bonders, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
            }
            if (inbond.INBONDRAWMATERIALs != null)
            {
                foreach (var item in inbond.INBONDRAWMATERIALs)
                {
                    item.INBONDID = inbond.ID;
                }
                if (inbond.BONDERID != null)
                {
                    userpermission = sessionAttributeRetreival.getStoredUserPermission();
                    int bonderId = inbond.BONDERID;
                    if (userpermission != null)
                    {
                        List<ANNUALENTLRAWMATERIAL> annualEntitlementRawMaterials = db.ANNUALENTLRAWMATERIALs.Where(eb => eb.BONDERSLNO == bonderId).ToList();
                        if (annualEntitlementRawMaterials != null && annualEntitlementRawMaterials.Count() > 0)
                        {
                            foreach (ANNUALENTLRAWMATERIAL matr in annualEntitlementRawMaterials)
                            {
                                ANNUALENTLRAWMATERIAL annualEntitlementRawMaterial = matr;
                                BONDERANNUALENTITLEMENT bonderAnnualEntitlement = db.BONDERANNUALENTITLEMENTs.SingleOrDefault(be => be.AESLNO == annualEntitlementRawMaterial.AESLNO && be.BONDERSLNO == bonderId);
                                if (bonderAnnualEntitlement != null)
                                {
                                    DateTime today = DateTime.Today;
                                    List<BONDERANNUALENTITLEMENT> annualEntitleMentForDateRanges = db.BONDERANNUALENTITLEMENTs.Where(dt => dt.ENTITLEFROM <= today && dt.ENTITLETO >= today).ToList();
                                    if (annualEntitleMentForDateRanges != null && annualEntitleMentForDateRanges.Count() == 0)
                                    {
                                        ModelState.AddModelError("", "No Annual Entitilement Exists for This Current Date");
                                        if (User.IsInRole("Bonder") && userpermission != null)
                                        {
                                            ViewBag.BONDERID = new SelectList(bonders.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                                        }
                                        else if (User.IsInRole("Operation Admin") && userpermission != null)
                                        {
                                            ViewBag.BONDERID = new SelectList(bonders, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                                        }
                                        for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
                                        {

                                            ViewData["INBONDRAWMATERIALs[" + i + "].RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME", inbond.INBONDRAWMATERIALs[i].RAWMATERIALCODE);
                                            ViewData["INBONDRAWMATERIALs[" + i + "].QUANTITYUNIT"] = new SelectList(unitModel.QUANTITYUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].QUANTITYUNIT);
                                            ViewData["INBONDRAWMATERIALs[" + i + "].COSTUNIT"] = new SelectList(unitModel.COSTUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].COSTUNIT);
                                        }
                                        for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
                                        {
                                            if (inbond.INBONDRAWMATERIALs[i].ID == 0)
                                            {
                                                return RedirectToAction("Addmore", new { errorMessage = "No Annual Entitilement Exists for This Current Date", id = inbond.ID });
                                            }
                                        }

                                        return View(inbond);
                                    }
                                    decimal? totalQuantity = annualEntitlementRawMaterial.AEQTY;
                                    MATERIAL matrl = db.MATERIALS.SingleOrDefault(me => me.MSLNO == matr.MSLNO);
                                    List<INBONDRAWMATERIAL> inbondRawMaterials = db.INBONDRAWMATERIALs.Where(inb => inb.INBOND.BONDERID == bonderId && inb.RAWMATERIALCODE == matrl.MSLNO).ToList();
                                    if (inbondRawMaterials != null)
                                    {
                                        List<INBONDRAWMATERIAL> editedRawMaterials = inbond.INBONDRAWMATERIALs.Where(ib => ib.ID > 0).ToList();
                                        decimal bonderUsedQuantity = inbondRawMaterials.Sum(item => item.PRODUCTQUANTITY);
                                        foreach (INBONDRAWMATERIAL material in editedRawMaterials)
                                        {
                                            INBONDRAWMATERIAL raw = inbondRawMaterials.SingleOrDefault(ib => ib.ID == material.ID);
                                            inbondRawMaterials.Remove(inbondRawMaterials.SingleOrDefault(ib => ib.ID == material.ID));
                                        }
                                        decimal sum = addRawMaterialQuantity(inbondRawMaterials);
                                        //bonderUsedQuantity = inbondRawMaterials.Sum(item => item.PRODUCTQUANTITY);
                                        bonderUsedQuantity = sum;
                                        sum = addRawMaterialQuantity(inbond.INBONDRAWMATERIALs.Where(ib => ib.RAWMATERIALCODE == matrl.MSLNO).ToList());
                                        //decimal bonderProvidedQuantity = inbond.INBONDRAWMATERIALs.Sum(item => item.PRODUCTQUANTITY);
                                        decimal bonderProvidedQuantity = sum;
                                        if ((totalQuantity - bonderUsedQuantity) < bonderProvidedQuantity)
                                        {
                                            ModelState.AddModelError("", "You can in bond maximum " + (totalQuantity - bonderUsedQuantity) + " TONS of " + db.MATERIALS.SingleOrDefault(m => m.MSLNO == matr.MSLNO).MATERIALNAME + " for this period");
                                            if (User.IsInRole("Bonder") && userpermission != null)
                                            {
                                                ViewBag.BONDERID = new SelectList(bonders.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                                            }
                                            else if (User.IsInRole("Operation Admin") && userpermission != null)
                                            {
                                                ViewBag.BONDERID = new SelectList(bonders, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                                            }
                                            for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
                                            {

                                                ViewData["INBONDRAWMATERIALs[" + i + "].RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME", inbond.INBONDRAWMATERIALs[i].RAWMATERIALCODE);
                                                ViewData["INBONDRAWMATERIALs[" + i + "].QUANTITYUNIT"] = new SelectList(unitModel.QUANTITYUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].QUANTITYUNIT);
                                                ViewData["INBONDRAWMATERIALs[" + i + "].COSTUNIT"] = new SelectList(unitModel.COSTUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].COSTUNIT);
                                            }
                                            for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
                                            {
                                                if (inbond.INBONDRAWMATERIALs[i].ID == 0)
                                                {
                                                    return RedirectToAction("Addmore", new { errorMessage ="You can in bond maximum " + (totalQuantity - bonderUsedQuantity) + " TONS of " + db.MATERIALS.SingleOrDefault(m => m.MSLNO == matr.MSLNO).MATERIALNAME + " for this period", id = inbond.ID });
                                                }
                                            }
                                            return View(inbond);
                                        }
                                    }


                                }

                                else
                                {
                                    ModelState.AddModelError("", "No Bonder Annual Entitlement For This User");
                                    if (User.IsInRole("Bonder") && userpermission != null)
                                    {
                                        ViewBag.BONDERID = new SelectList(bonders.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                                    }
                                    else if (User.IsInRole("Operation Admin") && userpermission != null)
                                    {
                                        ViewBag.BONDERID = new SelectList(bonders, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                                    }
                                    for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
                                    {

                                        ViewData["INBONDRAWMATERIALs[" + i + "].RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME", inbond.INBONDRAWMATERIALs[i].RAWMATERIALCODE);
                                        ViewData["INBONDRAWMATERIALs[" + i + "].QUANTITYUNIT"] = new SelectList(unitModel.QUANTITYUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].QUANTITYUNIT);
                                        ViewData["INBONDRAWMATERIALs[" + i + "].COSTUNIT"] = new SelectList(unitModel.COSTUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].COSTUNIT);
                                    }
                                    for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
                                    {
                                        if (inbond.INBONDRAWMATERIALs[i].ID == 0)
                                        {
                                            return RedirectToAction("Addmore", new { errorMessage = "No Bonder Annual Entitlement For This User", id = inbond.ID });
                                        }
                                    }
                                    return View(inbond);
                                }

                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "No Annual Entitlement For This User");
                            if (User.IsInRole("Bonder") && userpermission != null)
                            {
                                ViewBag.BONDERID = new SelectList(bonders.Where(b => b.BONDERSLNO == userpermission.BONDERID), "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                            }
                            else if (User.IsInRole("Operation Admin") && userpermission != null)
                            {
                                ViewBag.BONDERID = new SelectList(bonders, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
                            }
                            for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
                            {

                                ViewData["INBONDRAWMATERIALs[" + i + "].RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME", inbond.INBONDRAWMATERIALs[i].RAWMATERIALCODE);
                                ViewData["INBONDRAWMATERIALs[" + i + "].QUANTITYUNIT"] = new SelectList(unitModel.QUANTITYUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].QUANTITYUNIT);
                                ViewData["INBONDRAWMATERIALs[" + i + "].COSTUNIT"] = new SelectList(unitModel.COSTUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].COSTUNIT);
                            }
                            for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
                            {
                                if (inbond.INBONDRAWMATERIALs[i].ID == 0)
                                {
                                    return RedirectToAction("Addmore", new { errorMessage = "No Annual Entitlement For This User", id = inbond.ID });
                                }
                            }
                            return View(inbond);
                        }

                    }
                }
            }
            bool isAddMore = false;
            DateTime thisDay = DateTime.Today;
            if (ModelState.IsValid)
            {

                if (inbond.INBONDRAWMATERIALs != null && inbond.INBONDRAWMATERIALs.Count > 0)
                {


                    foreach (var item in inbond.INBONDRAWMATERIALs)
                    {
                        using (var db1 = new OracleEntitiesConnStr())
                        {
                            if (item.ID == 0)
                            {
                                item.INBONDID = inbond.ID;
                                item.CREATEDDATE = thisDay;
                                db1.INBONDRAWMATERIALs.Add(item);
                                db1.SaveChanges();
                            }
                            if (item.ID > 0)
                            {
                                isAddMore = true;
                                item.MODIFIEDDATE = thisDay;
                                db1.Entry(item).State = EntityState.Modified;
                                db1.SaveChanges();

                            }

                        }
                    }
                }
                using (var db2 = new OracleEntitiesConnStr())
                {
                    INBOND newInbond = new INBOND();
                    newInbond.ID = inbond.ID;
                    newInbond.LCNUMBER = inbond.LCNUMBER;
                    newInbond.LCDATE = inbond.LCDATE;
                    newInbond.BOENUMBER = inbond.BOENUMBER;
                    newInbond.BOEDATE = inbond.BOEDATE;
                    newInbond.CREATEDBY = inbond.CREATEDBY;
                    newInbond.CREATEDDATE = inbond.CREATEDDATE;
                    newInbond.BONDERID = inbond.BONDERID;
                    newInbond.MODIFIEDDATE = DateTime.Today;
                    newInbond.MODIFIEDBY = inbond.MODIFIEDBY;
                    db2.Entry(newInbond).State = EntityState.Modified;
                    db2.SaveChanges();
                    /*if (isAddMore)
                    {
                        return RedirectToAction("Addmore/" + inbond.ID);
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }*/
                    return RedirectToAction("Index");
                }
            }
            else // when model state invalid
            {




                QuantityModel unitmodel = new QuantityModel();
                for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
                {
                    if (inbond.INBONDRAWMATERIALs[i].ID <= 0)
                    {
                        isAddMore = true;
                    }
                    ViewData["INBONDRAWMATERIALs[" + i + "].RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME", inbond.INBONDRAWMATERIALs[i].RAWMATERIALCODE);
                    ViewData["INBONDRAWMATERIALs[" + i + "].QUANTITYUNIT"] = new SelectList(unitmodel.QUANTITYUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].QUANTITYUNIT);
                    ViewData["INBONDRAWMATERIALs[" + i + "].COSTUNIT"] = new SelectList(unitmodel.COSTUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].COSTUNIT);

                }
                var message = string.Join(" | ", ModelState.Values
                                           .SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage));
                ViewBag.error = message.ToString();

                if (isAddMore) {
                    return RedirectToAction("Addmore", new { errorMessage = message, id = inbond.ID });
                }
                //if (isAddMore)
                //{
                //    TempData["Inbond"] = inbond;
                //    TempData.Keep();
                //    return RedirectToAction("Addmore/" + inbond.ID);
                //}
                //else
                //{
                //    return View(inbond);
                //}
                return View(inbond);
            }

        }

        private static decimal addRawMaterialQuantity(IList<INBONDRAWMATERIAL> inbondRawMaterials)
        {
            decimal sum=0;
            foreach (INBONDRAWMATERIAL material in inbondRawMaterials)
            {
                if (material != null)
                {
                    if (material.QUANTITYUNIT.Equals("KG"))
                    {
                        sum += material.PRODUCTQUANTITY * (decimal)0.00110231;
                    }
                    else
                    {
                        sum += material.PRODUCTQUANTITY;
                    }
                }
            }
            return sum;
        }

        private INBOND mergeMaterials(INBOND inbond)
        {
            INBOND oldInbond = db.INBONDs.Find(inbond.ID);
            List<INBONDRAWMATERIAL> materialList = new List<INBONDRAWMATERIAL>();
            foreach (var item1 in oldInbond.INBONDRAWMATERIALs)
            {

                materialList.Add(item1);
                using (var db3 = new OracleEntitiesConnStr())
                {
                    INBONDRAWMATERIAL rawmaterials = db3.INBONDRAWMATERIALs.Find(item1.ID); ;
                    db3.INBONDRAWMATERIALs.Remove(rawmaterials);
                    db3.SaveChanges();
                }
            }
            foreach (var item1 in inbond.INBONDRAWMATERIALs)
            {

                materialList.Add(item1);
            }
            inbond.INBONDRAWMATERIALs = materialList;
            return inbond;
        }

        //
        // GET: /InBond/Delete/5
        [CustomAuthorize(Roles = "Operation Admin,Bonder")]
        public ActionResult Delete(short id = 0)
        {
            INBOND inbond = db.INBONDs.Find(id);
            if (inbond == null)
            {
                return HttpNotFound();
            }
            return View(inbond);
        }

        //
        // POST: /InBond/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Bonder,Operation Admin")]
        public ActionResult DeleteConfirmed(short id)
        {
            var masterData = db.INBONDs.Include("INBONDRAWMATERIALs").Where(p => p.ID == id).FirstOrDefault();
            TempData["Message"] = string.Format("InBond deleted successfully, Which LC No was {0}", masterData.LCNUMBER);
            var childData = masterData.INBONDRAWMATERIALs.ToList();
            foreach (var data in childData)
            {

                db.INBONDRAWMATERIALs.Remove(data);
            }

            db.INBONDs.Remove(masterData);
            db.SaveChanges();
            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult RawMaterialEntryRow(int bonderid=0)
        {
            QuantityModel model = new QuantityModel();
            USERPERMISSION userpermission;
            userpermission = sessionAttributeRetreival.getStoredUserPermission();
            if (bonderid!=0)
            {
                
                int bonderId = bonderid;
                if (userpermission != null)
                {
                    List<ANNUALENTLRAWMATERIAL> annualEntitlementRawMaterials = db.ANNUALENTLRAWMATERIALs.Where(eb => eb.BONDERSLNO == bonderId).ToList();
                    BONDREGISTRATION registrationInfo = db.BONDREGISTRATIONs.Where(br => br.BONDERSLNO == bonderid).SingleOrDefault();
                    DateTime? fromAEDate = registrationInfo.ISSUEDATE;
                    DateTime? toAEDate = registrationInfo.EXPIRYDATE;
                    List<ANNUALENTLRAWMATERIAL> aerawmaterials = new List<ANNUALENTLRAWMATERIAL>();
                    if (annualEntitlementRawMaterials != null)
                    {
                        foreach (var item in annualEntitlementRawMaterials.ToList())
                        {
                            var bonderAE = db.BONDERANNUALENTITLEMENTs.Where(bae => bae.AESLNO == item.AESLNO && bae.BONDERSLNO == item.BONDERSLNO && bae.ENTITLEFROM >= fromAEDate && bae.ENTITLETO <= toAEDate).Select(bae => new
                            {
                                From = bae.ENTITLEFROM,
                                To = bae.ENTITLETO
                            }).SingleOrDefault();
                            if (bonderAE != null && bonderAE.From != null && bonderAE.To != null)
                            {
                                item.ENTITLEFROM = bonderAE.From;
                                item.ENTITLETO = bonderAE.To;
                                aerawmaterials.Add(item);
                            }

                        }
                    }
                    if (aerawmaterials != null && aerawmaterials.Count() > 0)
                    {
                        ANNUALENTLRAWMATERIAL annualEntitlementRawMaterial = aerawmaterials.FirstOrDefault();
                        List<MATERIAL> materials = new List<MATERIAL>();
                        foreach (ANNUALENTLRAWMATERIAL material in aerawmaterials)
                        {
                            var mat = db.MATERIALS.Where(b => b.MTYPE.Equals("r") && b.MSLNO == material.MSLNO).FirstOrDefault();
                            if(null != mat)
                                materials.Add(mat);
                        }
                        ViewData["RAWMATERIALCODE"] = new SelectList(materials, "MSLNO", "MATERIALNAME");

                    }
                    else {
                        ModelState.AddModelError("", "No Raw Material exists for the current year"); ViewData["RAWMATERIALCODE"] = new SelectList(new List<MATERIAL>(), "MSLNO", "MATERIALNAME");
                    }

                }
            }
            
            //ViewData["RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME");
            ViewData["QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text");
            ViewData["COSTUNIT"] = new SelectList(model.COSTUNITLIST, "Value", "Text");
            return PartialView("RawMaterialEntryEditor");

        }
        [HttpGet]
        public ActionResult Addmore(string errorMessage,short id = 0)
        {
            INBOND inbond = (INBOND)TempData["inbondData"];
            if (inbond == null)
            {
                inbond = db.INBONDs.Find(id);
            }
            ViewBag.error = Request.QueryString["error"];
            if (inbond == null)
            {
                return HttpNotFound();
            }
            for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
            {

                ViewData["INBONDRAWMATERIALs[" + i + "].RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME", inbond.INBONDRAWMATERIALs[i].RAWMATERIALCODE);
                ViewData["INBONDRAWMATERIALs[" + i + "].QUANTITYUNIT"] = new SelectList(unitModel.QUANTITYUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].QUANTITYUNIT);
                ViewData["INBONDRAWMATERIALs[" + i + "].COSTUNIT"] = new SelectList(unitModel.COSTUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].COSTUNIT);
            }

            ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
            QuantityModel model = new QuantityModel();

            ViewData["RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME");
            ViewData["QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text");
            ViewData["COSTUNIT"] = new SelectList(model.COSTUNITLIST, "Value", "Text");
            if (errorMessage != null && !errorMessage.Trim().Equals(""))
            {
                ModelState.AddModelError("fileError", errorMessage);
                return View(inbond);
            }
            return View(inbond);

        }

        //public ActionResult Addmore(INBOND inbond)
        //{
        //    if (inbond == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    for (int i = 0; i < inbond.INBONDRAWMATERIALs.Count(); i++)
        //    {

        //        ViewData["INBONDRAWMATERIALs[" + i + "].RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME", inbond.INBONDRAWMATERIALs[i].RAWMATERIALCODE);
        //        ViewData["INBONDRAWMATERIALs[" + i + "].QUANTITYUNIT"] = new SelectList(unitModel.QUANTITYUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].QUANTITYUNIT);
        //        ViewData["INBONDRAWMATERIALs[" + i + "].COSTUNIT"] = new SelectList(unitModel.COSTUNITLIST, "Value", "Text", inbond.INBONDRAWMATERIALs[i].COSTUNIT);
        //    }

        //    ViewBag.BONDERID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME", inbond.BONDERID);
        //    QuantityModel model = new QuantityModel();
        //    ViewData["RAWMATERIALCODE"] = new SelectList(db.MATERIALS.Where(b => b.MTYPE.Equals("r")), "MSLNO", "MATERIALNAME");
        //    ViewData["QUANTITYUNIT"] = new SelectList(model.QUANTITYUNITLIST, "Value", "Text");
        //    ViewData["COSTUNIT"] = new SelectList(model.COSTUNITLIST, "Value", "Text");
        //    return View(inbond);
        //}
        public ActionResult RawMaterialEntryRowH()
        {
            return PartialView("RawMaterialEntryEditorH");
        }

        [HttpDelete]
        public ActionResult DeleteRawMaterials(int id)
        {
            INBONDRAWMATERIAL rawmaterial = db.INBONDRAWMATERIALs.Find(id);
            try
            {
                if (rawmaterial != null)
                {
                    db.INBONDRAWMATERIALs.Remove(rawmaterial);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {

            }
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
    }
}
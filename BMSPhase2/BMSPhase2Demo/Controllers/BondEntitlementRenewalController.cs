using BMSPhase2Demo.Models;
using BMSPhase2Demo.Util;
using Domain.Concrete;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSPhase2Demo.Controllers
{
    public class BondEntitlementRenewalController : Controller
    {
        SessionAttributeRetreival session = new SessionAttributeRetreival();
        OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        //public ActionResult Index()
        //{
        //    return View();

        //}


        public ActionResult Search()
        {

            return View("Search");

        }

        //---------------------------------Mizan Work (24 Oct 16)-----------------------------------------------
        public ActionResult CreateDetails(string BondLicenseNo, int? id, int bonderSlNo)
        {
            var bonderslno = (from b in db.BONDERs
                              where b.BONDLICENSENO == BondLicenseNo
                              select b.BONDERSLNO).FirstOrDefault();

           
            EntitlementViewModel entitlementviewmodel = new EntitlementViewModel();

            entitlementviewmodel.BonderAnnualEntitlement = db.BONDERANNUALENTITLEMENTs.Where(x => x.AESLNO == id).SingleOrDefault();
            entitlementviewmodel.AnnualEntitlementRawMaterials = db.ANNUALENTLRAWMATERIALs.Where(x => x.BONDERSLNO == bonderSlNo).ToList();
            entitlementviewmodel.BonderAnnualEntitlementCoMs = db.BONDERANNUALENTITLEMENTCOMs.Where(x => x.BONDERSLNO == bonderSlNo).ToList();
            
            var List = (from r in db.BONDERANNUALENTITLEMENTCOEDETs
                        where r.AESLNO == id
                        select new
                         {
                            rawslno = r.RMSLNO,
                             mslno = r.MSLNO
                         }).ToList();

            List<BONDERANNUALENTITLEMENTCOEDET> rawdetlist = new List<BONDERANNUALENTITLEMENTCOEDET>();

            foreach (var item in List)
            {
                rawdetlist.Add(

                new BONDERANNUALENTITLEMENTCOEDET

                     {
                         MSLNO = item.mslno,
                         INPUTBY = (from t2 in db.MATERIALS where t2.MSLNO == item.rawslno select t2.MDESCRIPTION).FirstOrDefault().ToString(),

                     }

                    );

            }
            entitlementviewmodel.BonderannualEntitlementCoEDets = rawdetlist;
           
            ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MHSCODE");
            ViewBag.Products = new SelectList(db.MATERIALS.Where(p => p.MTYPE == "F"), "MSLNO", "MHSCODE");
            ViewBag.Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
            if (entitlementviewmodel.BonderAnnualEntitlement!=null )
            {
                ViewBag.UnitTS = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", entitlementviewmodel.BonderAnnualEntitlement.TSMUSLNO);
                ViewBag.UnitOT = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", entitlementviewmodel.BonderAnnualEntitlement.OTMUSLNO);
                ViewBag.UnitET = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", entitlementviewmodel.BonderAnnualEntitlement.ETMUSLNO);
            }
            
            ViewBag.MachineList = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM");

          if (bonderslno != 0)
            {

                entitlementviewmodel.BondStatus  = db.BONDSTATUS.Where(y => y.BONDERSLNO == bonderslno).FirstOrDefault();

                entitlementviewmodel.Bonder = db.BONDERs.Where(x => x.BONDERSLNO == bonderslno).SingleOrDefault();

                ViewBag.HSCodesR = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MHSCODE");
                ViewBag.HSCodesF = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "F"), "MSLNO", "MHSCODE");
                ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MATERIALNAME");
                ViewBag.Products = new SelectList(db.MATERIALS.Where(p => p.MTYPE == "F"), "MSLNO", "MATERIALNAME");
                ViewBag.Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
                ViewBag.MachineList = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM");
                return View(entitlementviewmodel);

            }

          return View(entitlementviewmodel);

           


        }

        //---------------------------------Mizan Work (24 Oct 16)-----------------------------------------------
        public ActionResult Create(string BondLicenseNo)
        {

            var bonderslno = (from b in db.BONDERs
                              where b.BONDLICENSENO == BondLicenseNo
                              select b.BONDERSLNO).FirstOrDefault();

            AnnualCoefficientEntitlementViewModel annualcoefficiententitlementviewmodel = new AnnualCoefficientEntitlementViewModel();


            if (bonderslno != 0)
            {

                annualcoefficiententitlementviewmodel.BondStatus = db.BONDSTATUS.Where(y => y.BONDERSLNO == bonderslno).FirstOrDefault();

                annualcoefficiententitlementviewmodel.Bonder = db.BONDERs.Where(x => x.BONDERSLNO == bonderslno).SingleOrDefault();

                ViewBag.HSCodesR = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MHSCODE");
                ViewBag.HSCodesF = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "F"), "MSLNO", "MHSCODE");
                ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MATERIALNAME");
                ViewBag.Products = new SelectList(db.MATERIALS.Where(p => p.MTYPE == "F"), "MSLNO", "MATERIALNAME");
                ViewBag.Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
                ViewBag.MachineList = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM");
                return View(annualcoefficiententitlementviewmodel);

            }

            return View("Search");

        }


        [HttpPost]
        public ActionResult Create(
            string entitlementtype, int? bondersl, int[] hscodes, decimal[] quantitys, int[] units, decimal? totalentitlement, int? totalentitlementunit, string totalentitlementinword,
        decimal? onetimestoragequantity, int? onetimestoragequantitytunit, string onetimestoragequantityinwords,
        decimal? entitlementpercentaccordingtoproduction, int? entitlementpercentaccordingtoproductionunit, string entitlementpercentaccordingtoproductioninword,
        string entitlementdatefrom, string entitlementdateto, int[] ProductCoE, string[] ProductWeightCoE, string[] ProductDescriptionCoE,
        string[] ProductSizeCoE, string[] ProductMeasurementCoE, string[] ProductRawmaterialsCoE, int[] ProductMachineCoE, int[] hscodesCoE
        , int[] measuementoCoE, int[] grossCoEs, int[] wastageCoEs, int[] shrinkageCoEs, int[] netCoEs, int[] RawmaterialProductID)
        
        {
            BondEntitlementRepository repo = new BondEntitlementRepository();
            int id = repo.AddBonderAnnualEntitlement(entitlementtype, bondersl, totalentitlement, totalentitlementunit, totalentitlementinword, onetimestoragequantity, onetimestoragequantitytunit, onetimestoragequantityinwords,
                                                     entitlementpercentaccordingtoproduction, entitlementpercentaccordingtoproductionunit, entitlementpercentaccordingtoproductioninword,
                                                     entitlementdatefrom, entitlementdateto);
            if (id > 0)
            {
                repo.AddAnnualEntitlementRawMaterilas(bondersl, hscodes, quantitys, units, id);
                repo.AddBonderAnnualEntitlementCoE(bondersl, id, ProductCoE, ProductWeightCoE, ProductDescriptionCoE, ProductSizeCoE, ProductMeasurementCoE, ProductRawmaterialsCoE, ProductMachineCoE,
                                                    hscodesCoE, RawmaterialProductID, measuementoCoE, grossCoEs, wastageCoEs, shrinkageCoEs, netCoEs);
            }
            return RedirectToAction("BondEntitlementList");
        }


        public ActionResult BondEntitlementList()
        {
            if (!(System.Web.HttpContext.Current.User.IsInRole("Bonder")))   // added By Mizan (18 Aug 2016)--
            {
                var EntitlementList = from e in db.BONDERANNUALENTITLEMENTs
                                      group e by e.BONDERSLNO
                                          into g
                                          select new
                                          {
                                              bonderslno = g.Key,
                                              aeslno = (from t2 in g select t2.AESLNO).Max()
                                          };
                List<BONDERANNUALENTITLEMENT> entlmntlst = new List<BONDERANNUALENTITLEMENT>();
                foreach (var item in EntitlementList)
                {
                    entlmntlst.Add(
                                  new BONDERANNUALENTITLEMENT
                                  {


                                      AESLNO = item.aeslno,
                                      BONDERSLNO = item.bonderslno,
                                      INPUTBY = (from t2 in db.BONDERs where t2.BONDERSLNO == item.bonderslno select t2.BONDERNAME).FirstOrDefault(),
                                      MODIFIEDBY = (from t2 in db.BONDERs where t2.BONDERSLNO == item.bonderslno select t2.BONDLICENSENO).FirstOrDefault()
                                  });

                }
                return View(entlmntlst);
            }
            else                                                                        // added By Mizan (18 Aug 2016)--
            {
                USERPERMISSION permission = session.getStoredUserPermission();
                var bonderName = permission.BONDER.BONDERNAME;
                var EntitlementList = from e in db.BONDERANNUALENTITLEMENTs.Where(e => e.BONDER.BONDERNAME == bonderName)
                                      group e by e.BONDERSLNO
                                          into g
                                          select new
                                          {
                                              bonderslno = g.Key,
                                              aeslno = (from t2 in g select t2.AESLNO).Max()
                                          };
                List<BONDERANNUALENTITLEMENT> entlmntlst = new List<BONDERANNUALENTITLEMENT>();
                foreach (var item in EntitlementList)
                {
                    entlmntlst.Add(
                                  new BONDERANNUALENTITLEMENT
                                  {


                                      AESLNO = item.aeslno,
                                      BONDERSLNO = item.bonderslno,
                                      INPUTBY = (from t2 in db.BONDERs where t2.BONDERSLNO == item.bonderslno select t2.BONDERNAME).FirstOrDefault(),
                                      MODIFIEDBY = (from t2 in db.BONDERs where t2.BONDERSLNO == item.bonderslno select t2.BONDLICENSENO).FirstOrDefault()
                                  });

                }
                return View(entlmntlst);
            }

        }


        //--------------------------------------Mizan Work (25 Oct 16)-------------------------------------------
        public ActionResult EntitlementDetails(int? id, int bonderSlNo)
        {

            AnnualCoefficientEntitlementViewModelList annualcoefficiententitlementviewmodellst = new AnnualCoefficientEntitlementViewModelList();
         
            annualcoefficiententitlementviewmodellst.BonderAnnualEntitlement = db.BONDERANNUALENTITLEMENTs.Where(x => x.AESLNO == id).SingleOrDefault();
          //annualcoefficiententitlementviewmodellst.AnnualEntitlementRawMaterials = db.ANNUALENTLRAWMATERIALs.Where(x => x.AESLNO == id).ToList();
            annualcoefficiententitlementviewmodellst.AnnualEntitlementRawMaterials = db.ANNUALENTLRAWMATERIALs.Where(x => x.BONDERSLNO == bonderSlNo).ToList();
            annualcoefficiententitlementviewmodellst.BonderAnnualEntitlementCoMs = db.BONDERANNUALENTITLEMENTCOMs.Where(x => x.BONDERSLNO == bonderSlNo).ToList();
            //annualcoefficiententitlementviewmodellst.BonderannualEntitlementCoEDets = db.BONDERANNUALENTITLEMENTCOEDETs.Where(x => x.AESLNO == id).ToList();

            var List = (from r in db.BONDERANNUALENTITLEMENTCOEDETs
                        where r.AESLNO == id
                        select new
                        {
                            rawslno = r.RMSLNO,


                            mslno = r.MSLNO

                        }).ToList();

            List<BONDERANNUALENTITLEMENTCOEDET> rawdetlist = new List<BONDERANNUALENTITLEMENTCOEDET>();

            foreach (var item in List)
            {
                rawdetlist.Add(

                     new BONDERANNUALENTITLEMENTCOEDET

                     {
                         MSLNO = item.mslno,
                         INPUTBY = (from t2 in db.MATERIALS where t2.MSLNO == item.rawslno select t2.MDESCRIPTION).FirstOrDefault().ToString(),

                     }

                    );

            }

            annualcoefficiententitlementviewmodellst.BonderannualEntitlementCoEDets = rawdetlist;
         
            ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MHSCODE");
            ViewBag.Products = new SelectList(db.MATERIALS.Where(p => p.MTYPE == "F"), "MSLNO", "MHSCODE");
            ViewBag.Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
            ViewBag.UnitTS = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", annualcoefficiententitlementviewmodellst.BonderAnnualEntitlement.TSMUSLNO);
            ViewBag.UnitOT = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", annualcoefficiententitlementviewmodellst.BonderAnnualEntitlement.OTMUSLNO);
            ViewBag.UnitET = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", annualcoefficiententitlementviewmodellst.BonderAnnualEntitlement.ETMUSLNO);
            ViewBag.MachineList = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM");

            return View(annualcoefficiententitlementviewmodellst);
        }

        //--------------------------------------Mizan Work (25 Oct 16)------------------------------------------------------------------------


        //public ActionResult EntitlementDetails(int? id)
        //{

        //    AnnualCoefficientEntitlementViewModelList annualcoefficiententitlementviewmodellst = new AnnualCoefficientEntitlementViewModelList();

        //    annualcoefficiententitlementviewmodellst.BonderAnnualEntitlement = db.BONDERANNUALENTITLEMENTs.Where(x => x.AESLNO == id).SingleOrDefault();
        //    annualcoefficiententitlementviewmodellst.AnnualEntitlementRawMaterials = db.ANNUALENTLRAWMATERIALs.Where(x => x.AESLNO == id).ToList();
        //    annualcoefficiententitlementviewmodellst.BonderannualEntitlementCoEDets = db.BONDERANNUALENTITLEMENTCOEDETs.Where(x => x.AESLNO == id).ToList();

        //    var List = (from r in db.BONDERANNUALENTITLEMENTCOEDETs
        //                where r.AESLNO == id
        //                select new
        //                {
        //                    rawslno = r.RMSLNO,


        //                    mslno = r.MSLNO

        //                }).ToList();

        //    List<BONDERANNUALENTITLEMENTCOEDET> rawdetlist = new List<BONDERANNUALENTITLEMENTCOEDET>();

        //    foreach (var item in List)
        //    {
        //        rawdetlist.Add(

        //          new BONDERANNUALENTITLEMENTCOEDET

        //             {
        //                 MSLNO = item.mslno,
        //                 INPUTBY = (from t2 in db.MATERIALS where t2.MSLNO == item.rawslno select t2.MDESCRIPTION).FirstOrDefault().ToString(),

        //             }

        //            );

        //    }

        //    annualcoefficiententitlementviewmodellst.BonderannualEntitlementCoEDets = rawdetlist;
       
        //    ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MHSCODE");
        //    ViewBag.Products = new SelectList(db.MATERIALS.Where(p => p.MTYPE == "F"), "MSLNO", "MHSCODE");
        //    ViewBag.Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
        //    ViewBag.UnitTS = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", annualcoefficiententitlementviewmodellst.BonderAnnualEntitlement.TSMUSLNO);
        //    ViewBag.UnitOT = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", annualcoefficiententitlementviewmodellst.BonderAnnualEntitlement.OTMUSLNO);
        //    ViewBag.UnitET = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", annualcoefficiententitlementviewmodellst.BonderAnnualEntitlement.ETMUSLNO);
        //    ViewBag.MachineList = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM");

        //    return View(annualcoefficiententitlementviewmodellst);
        //}

        public ActionResult EditEntitlement(int? id, int? id2, int? id3)
        {

            ANNUALENTLRAWMATERIAL annualentlrawmaterial = db.ANNUALENTLRAWMATERIALs.Where(x => x.AESLNO == id && x.BONDERSLNO == id2 && x.MSLNO == id3).SingleOrDefault();
            if (annualentlrawmaterial == null)
            {
                return HttpNotFound();
            }

            ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MHSCODE", annualentlrawmaterial.MSLNO);
            ViewBag.Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", annualentlrawmaterial.MUSLNO);
            return View(annualentlrawmaterial);

        }



        [HttpPost]
        public ActionResult EditEntitlement(ANNUALENTLRAWMATERIAL annualentlrawmaterialedited, int? rawmaterilas, int? unit)
        {


            ANNUALENTLRAWMATERIAL annualentlrawmaterial = new ANNUALENTLRAWMATERIAL();

            if (ModelState.IsValid)
            {

                try
                {

                    annualentlrawmaterial.AESLNO = annualentlrawmaterialedited.AESLNO;
                    annualentlrawmaterial.BONDERSLNO = annualentlrawmaterialedited.BONDERSLNO;
                    annualentlrawmaterial.MSLNO = annualentlrawmaterialedited.MSLNO;
                    //annualentlrawmaterial.MATERIAL.MDESCRIPTION = annualentlrawmaterialedited.MATERIAL.MDESCRIPTION;
                    //annualentlrawmaterial.MATERIAL.SPGRADE = annualentlrawmaterialedited.MATERIAL.SPGRADE;
                    annualentlrawmaterial.AEQTY = annualentlrawmaterialedited.AEQTY;
                    annualentlrawmaterial.MUSLNO = (Int16)unit;


                    db.Entry(annualentlrawmaterial).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("EntitlementDetails", new { id = annualentlrawmaterialedited.AESLNO });

                }
                catch (Exception e)
                {


                }




            }

            ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MHSCODE", annualentlrawmaterial.MSLNO);
            ViewBag.Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", annualentlrawmaterial.MUSLNO);
            return View(annualentlrawmaterial);


        }




        public ActionResult DeleteAnnualEntlRawmaterial(int? id, int? id2, int? id3)
        {
            ANNUALENTLRAWMATERIAL annualentlrawmaterial = db.ANNUALENTLRAWMATERIALs.Find(id, id2, id3);
            db.ANNUALENTLRAWMATERIALs.Remove(annualentlrawmaterial);
            db.SaveChanges();

            return RedirectToAction("EntitlementDetails", new { ID = id });

        }

        //------------Mizan Work ----------------------

        public ActionResult EntitlementReport()
        {
            if (!(System.Web.HttpContext.Current.User.IsInRole("Bonder")))   // added By Mizan (18 Aug 2016)--
            {
                ViewBag.AESLNO = new SelectList(db.BONDERANNUALENTITLEMENTs, "AESLNO", "AESLNO");
            }
            else                                                             // added By Mizan (18 Aug 2016)--
            {
                USERPERMISSION permission = session.getStoredUserPermission();
                var bonderName = permission.BONDER.BONDERNAME;
                ViewBag.AESLNO = new SelectList(db.BONDERANNUALENTITLEMENTs.Where(x => x.BONDER.BONDERNAME == bonderName), "AESLNO", "AESLNO");
            }

            return View("EntitlementDetailsCrReport");

        }

        //------------Mizan Work ----------------------

        public ActionResult AddEntitlementRawMaterial(int? aeslno, int? bonderslno)
        {
            ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MATERIALNAME");
            ViewBag.AeSlno = aeslno;
            ViewBag.BonderSlno = bonderslno;

            return View();

        }


        [HttpPost]
        public ActionResult AddEntitlementRawMaterial(int? aeslno, int? bonderslno, int? rawmaterilas,
                                        string hscode, string description, string specificationorgrade, int? quantity,
                                           int? unitslno)
        {


            try
            {
                ANNUALENTLRAWMATERIAL annualentlrawmaterial = new ANNUALENTLRAWMATERIAL();


                // annualentlrawmaterial.AESLNO = (Int16)aeslno;
                annualentlrawmaterial.BONDERSLNO = (Int16)bonderslno;
                annualentlrawmaterial.MSLNO = (Int16)rawmaterilas;
                annualentlrawmaterial.MUSLNO = (Int16)unitslno;
                annualentlrawmaterial.AEQTY = quantity;


                db.ANNUALENTLRAWMATERIALs.Add(annualentlrawmaterial);
                db.SaveChanges();

                return RedirectToAction("EntitlementDetails", new { ID = aeslno });
            }


            catch (Exception e)
            {
                ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MHSCODE");
                ViewBag.Error = "Same Product Exist !!";
                return View();


            }
        }


        public ActionResult DeleteCoefficientMas(int? id, int? id2, int? id3)
        {

            var CoefficientsMas = db.BONDERANNUALENTITLEMENTCOMs.Where(x => x.AESLNO == id && x.BONDERSLNO == id2 && x.MSLNO == id3);
            foreach (var cm in CoefficientsMas)
            {
                var CoefficientsDet = db.BONDERANNUALENTITLEMENTCOEDETs.Where(x => x.AESLNO == id && x.BONDERSLNO == id2 && x.MSLNO == id3);
                foreach (var cd in CoefficientsDet)
                {
                    db.BONDERANNUALENTITLEMENTCOEDETs.Remove(cd);
                }
                db.BONDERANNUALENTITLEMENTCOMs.Remove(cm);
            }
            db.SaveChanges();
            return RedirectToAction("EntitlementDetails", new { ID = id });
        }


        public ActionResult DeleteCoefficientDet(int? id, int? id2, int? id3, int? id4)
        {

            var CoefficientsDet = db.BONDERANNUALENTITLEMENTCOEDETs.Where(x => x.AESLNO == id && x.BONDERSLNO == id2 && x.MSLNO == id3 && x.RMSLNO == id4);

            foreach (var cd in CoefficientsDet)
            {

                db.BONDERANNUALENTITLEMENTCOEDETs.Remove(cd);

            }

            db.SaveChanges();

            return RedirectToAction("EntitlementDetails", new { ID = id });
        }


        public ActionResult AddEntitlementCoefficient(int? aeslno, int? bonderslno)
        {

            ViewBag.Products = new SelectList(db.MATERIALS.Where(p => p.MTYPE == "F"), "MSLNO", "MATERIALNAME");
            ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MATERIALNAME");
            ViewBag.Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
            ViewBag.MachineList = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM");

            ViewBag.AeSlno = aeslno;
            ViewBag.BonderSlno = bonderslno;

            return View();


        }

        [HttpPost]
        public ActionResult AddEntitlementCoefficient(int? aeslno, int? bonderslno, int[] ProductCoE, string[] ProductWeightCoE, string[] ProductDescriptionCoE,
                                              string[] ProductSizeCoE, string[] ProductMeasurementCoE, string[] ProductRawmaterialsCoE, int[] ProductMachineCoE, int[] hscodesCoE
                                              , int[] measuementoCoE, int[] grossCoEs, int[] wastageCoEs, int[] shrinkageCoEs, int[] netCoEs, int[] RawmaterialProductID)
        {

            try
            {


                if (ProductCoE != null)
                {


                    int lenth = ProductCoE.Length;

                    for (int i = 0; i < lenth; i++)
                    {
                        BONDERANNUALENTITLEMENTCOM bonderannualentitlementcom = new BONDERANNUALENTITLEMENTCOM();

                        bonderannualentitlementcom.AESLNO = (Int16)aeslno;
                        bonderannualentitlementcom.BONDERSLNO = (Int16)bonderslno;
                        bonderannualentitlementcom.MSLNO = (Int16)ProductCoE[i];
                        bonderannualentitlementcom.WEIGHT = ProductWeightCoE[i];
                        bonderannualentitlementcom.MSIZE = ProductSizeCoE[i];
                        bonderannualentitlementcom.MEASUREMENT = ProductMeasurementCoE[i];
                        //bonderannualentitlementcom.MDESC = ProductDescriptionCoE[i];
                        bonderannualentitlementcom.MACHINESLNO = (Int16)ProductMachineCoE[i];

                        for (int j = 0; j < hscodesCoE.Length; j++)
                        {
                            BONDERANNUALENTITLEMENTCOEDET bonderannualentitlementcoedet = new BONDERANNUALENTITLEMENTCOEDET();

                            OracleEntitiesConnStr db2 = new OracleEntitiesConnStr();

                            if (ProductCoE[i] == RawmaterialProductID[j])
                            {


                                bonderannualentitlementcoedet.AESLNO = (Int16)aeslno;
                                bonderannualentitlementcoedet.BONDERSLNO = (Int16)bonderslno;
                                bonderannualentitlementcoedet.MSLNO = (Int16)ProductCoE[i];
                                bonderannualentitlementcoedet.RMSLNO = (Int16)hscodesCoE[j];
                                bonderannualentitlementcoedet.GROSSQT = (Int16)grossCoEs[j];
                                bonderannualentitlementcoedet.WASTAGEQT = (Int16)wastageCoEs[j];
                                bonderannualentitlementcoedet.SHRINKAGEQT = (Int16)shrinkageCoEs[j];
                                bonderannualentitlementcoedet.NETQT = (Int16)netCoEs[j];

                                db2.BONDERANNUALENTITLEMENTCOEDETs.Add(bonderannualentitlementcoedet);


                            }

                            db2.SaveChanges();


                        }


                        db.BONDERANNUALENTITLEMENTCOMs.Add(bonderannualentitlementcom);

                    }

                    db.SaveChanges();
                }


                return RedirectToAction("EntitlementDetails", new { ID = aeslno });

            }


            catch
            {
                ViewBag.MachineList = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM");
                ViewBag.Products = new SelectList(db.MATERIALS.Where(p => p.MTYPE == "F"), "MSLNO", "MHSCODE");
                ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MHSCODE");
                ViewBag.AeSlno = aeslno;
                ViewBag.BonderSlno = bonderslno;

                return View();

            }



        }


        public ActionResult EditCoefficientDet(int? id, int? id2, int? id3, int? id4)
        {

            BONDERANNUALENTITLEMENTCOEDET bonderannualentitlementcoedet = db.BONDERANNUALENTITLEMENTCOEDETs.Where(x => x.AESLNO == id && x.BONDERSLNO == id2 && x.MSLNO == id3 && x.RMSLNO == id4).FirstOrDefault();
            if (bonderannualentitlementcoedet == null)
            {
                return HttpNotFound();
            }

            //ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "R"), "MSLNO", "MHSCODE", annualentlrawmaterial.MSLNO);
            //ViewBag.Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", bonderannualentitlementcoedet.MUSLNO);

            return View(bonderannualentitlementcoedet);

        }



        [HttpPost]
        public ActionResult EditCoefficientDet(BONDERANNUALENTITLEMENTCOEDET bonderannualentitlementcoedetedited)
        {


            BONDERANNUALENTITLEMENTCOEDET bonderannualentitlementcoedet = new BONDERANNUALENTITLEMENTCOEDET();

            if (ModelState.IsValid)
            {

                try
                {

                    bonderannualentitlementcoedet.AESLNO = bonderannualentitlementcoedetedited.AESLNO;
                    bonderannualentitlementcoedet.BONDERSLNO = bonderannualentitlementcoedetedited.BONDERSLNO;
                    bonderannualentitlementcoedet.MSLNO = bonderannualentitlementcoedetedited.MSLNO;
                    bonderannualentitlementcoedet.RMSLNO = bonderannualentitlementcoedetedited.RMSLNO;

                    bonderannualentitlementcoedet.GROSSQT = bonderannualentitlementcoedetedited.GROSSQT;
                    bonderannualentitlementcoedet.WASTAGEQT = bonderannualentitlementcoedetedited.WASTAGEQT;
                    bonderannualentitlementcoedet.SHRINKAGEQT = bonderannualentitlementcoedetedited.SHRINKAGEQT;
                    bonderannualentitlementcoedet.NETQT = bonderannualentitlementcoedetedited.NETQT;


                    db.Entry(bonderannualentitlementcoedet).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("EditCoefficientMas", new { id = bonderannualentitlementcoedetedited.AESLNO, id2 = bonderannualentitlementcoedetedited.BONDERSLNO, id3 = bonderannualentitlementcoedetedited.MSLNO });

                }
                catch (Exception e)
                {


                }




            }


            return View(bonderannualentitlementcoedet);


        }

        public ActionResult EditCoefficientMas(int? id, int? id2, int? id3)
        {

            CoefficientMasterDetailViewModel coefficientmasterdetailviewmodel = new CoefficientMasterDetailViewModel();

            coefficientmasterdetailviewmodel.Bonderannualentitlementcoms = db.BONDERANNUALENTITLEMENTCOMs.Where(x => x.AESLNO == id && x.BONDERSLNO == id2 && x.MSLNO == id3).FirstOrDefault();
            //coefficientmasterdetailviewmodel.Bonderannualentitlementcoedets = db.BONDERANNUALENTITLEMENTCOEDETs.Where(x => x.AESLNO == id && x.BONDERSLNO == id2 && x.MSLNO == id3).ToList();


            var List = (from r in db.BONDERANNUALENTITLEMENTCOEDETs
                        where r.AESLNO == id && r.BONDERSLNO == id2 && r.MSLNO == id3
                        select new


                        {

                            aeslno = r.AESLNO,
                            bonderslno = r.BONDERSLNO,
                            mslno = r.MSLNO,
                            rawslno = r.RMSLNO,
                            gross = r.GROSSQT,
                            wastage = r.WASTAGEQT,
                            shrinkage = r.SHRINKAGEQT,
                            net = r.NETQT



                        }).ToList();

            List<BONDERANNUALENTITLEMENTCOEDET> rawdetlist = new List<BONDERANNUALENTITLEMENTCOEDET>();

            foreach (var item in List)
            {
                rawdetlist.Add(




                     new BONDERANNUALENTITLEMENTCOEDET

                     {
                         AESLNO = item.aeslno,
                         BONDERSLNO = item.bonderslno,
                         MSLNO = item.mslno,
                         RMSLNO = item.rawslno,
                         INPUTBY = (from t2 in db.MATERIALS where t2.MSLNO == item.rawslno select t2.MATERIALNAME).FirstOrDefault().ToString(),
                         MODIFIEDBY = (from t2 in db.MATERIALS where t2.MSLNO == item.rawslno select t2.MHSCODE).FirstOrDefault().ToString(),
                         GROSSQT = item.gross,
                         WASTAGEQT = item.wastage,
                         SHRINKAGEQT = item.shrinkage,
                         NETQT = item.net



                     }



                    );



            }




            coefficientmasterdetailviewmodel.Bonderannualentitlementcoedets = rawdetlist;





            if (coefficientmasterdetailviewmodel == null)
            {
                return HttpNotFound();
            }

            ViewBag.Products = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "F"), "MSLNO", "MATERIALNAME", coefficientmasterdetailviewmodel.Bonderannualentitlementcoms.MSLNO);
            ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MATERIALNAME");
            //ViewBag.Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", bonderannualentitlementcoedet.MUSLNO);
            ViewBag.MachineList = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM", coefficientmasterdetailviewmodel.Bonderannualentitlementcoms.MACHINESLNO);
            return View(coefficientmasterdetailviewmodel);

        }



        [HttpPost]
        public ActionResult EditCoefficientMas(CoefficientMasterDetailViewModel coefficientmasterdetailviewmodeledt, int? MachineList)
        {


            BONDERANNUALENTITLEMENTCOM bonderannualentitlementcom = new BONDERANNUALENTITLEMENTCOM();

            //CoefficientMasterDetailViewModel coefficientmasterdetailviewmodel = new CoefficientMasterDetailViewModel();

            if (ModelState.IsValid)
            {

                try
                {

                    bonderannualentitlementcom.AESLNO = coefficientmasterdetailviewmodeledt.Bonderannualentitlementcoms.AESLNO;
                    bonderannualentitlementcom.BONDERSLNO = coefficientmasterdetailviewmodeledt.Bonderannualentitlementcoms.BONDERSLNO;
                    bonderannualentitlementcom.MSLNO = coefficientmasterdetailviewmodeledt.Bonderannualentitlementcoms.MSLNO;


                    bonderannualentitlementcom.WEIGHT = coefficientmasterdetailviewmodeledt.Bonderannualentitlementcoms.WEIGHT;
                    bonderannualentitlementcom.MSIZE = coefficientmasterdetailviewmodeledt.Bonderannualentitlementcoms.MSIZE;
                    bonderannualentitlementcom.MEASUREMENT = coefficientmasterdetailviewmodeledt.Bonderannualentitlementcoms.MEASUREMENT;
                    bonderannualentitlementcom.MACHINESLNO = (Int16)MachineList;


                    db.Entry(bonderannualentitlementcom).State = EntityState.Modified;
                    db.SaveChanges();

                    ViewBag.Products = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "F"), "MSLNO", "MATERIALNAME", bonderannualentitlementcom.MSLNO);
                    ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MATERIALNAME");
                    return RedirectToAction("EntitlementDetails", new { id = bonderannualentitlementcom.AESLNO });

                }
                catch (Exception e)
                {

                }

            }


            ViewBag.MachineList = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM", coefficientmasterdetailviewmodeledt.Bonderannualentitlementcoms.MACHINESLNO);
            return View(bonderannualentitlementcom);


        }





        public ActionResult CalculateStorageInfo(AnnualCoefficientEntitlementViewModelList bonderannualentitlementedt, int? UnitTS, int? UnitOT, int? UnitET)
        {


            try
            {
                List<ANNUALENTLRAWMATERIAL> annualentlrawmaterial = db.ANNUALENTLRAWMATERIALs.Where(x => x.AESLNO == bonderannualentitlementedt.BonderAnnualEntitlement.AESLNO && x.BONDERSLNO == bonderannualentitlementedt.BonderAnnualEntitlement.BONDERSLNO).ToList();

                decimal totalentitlement = 0;
                decimal entitlementepercentage = 0;
                decimal percentage = 100;
                if (bonderannualentitlementedt.BonderAnnualEntitlement.PERCENTVAL != null)
                {
                    percentage = (Int64)bonderannualentitlementedt.BonderAnnualEntitlement.PERCENTVAL;
                }




                foreach (var ae in annualentlrawmaterial)
                {

                    totalentitlement = totalentitlement + (Int64)ae.AEQTY;


                }

                entitlementepercentage = percentage / 100 * totalentitlement;


                BONDERANNUALENTITLEMENT bonderannualentitlement = new BONDERANNUALENTITLEMENT();

                bonderannualentitlement.AESLNO = bonderannualentitlementedt.BonderAnnualEntitlement.AESLNO;
                bonderannualentitlement.BONDERSLNO = bonderannualentitlementedt.BonderAnnualEntitlement.BONDERSLNO;


                bonderannualentitlement.TOTALSTORAGEQTY = totalentitlement;
                bonderannualentitlement.TSMUSLNO = (Int16)UnitTS;
                bonderannualentitlement.TOTALSTORAGEQTYEN = bonderannualentitlementedt.BonderAnnualEntitlement.TOTALSTORAGEQTYEN;

                bonderannualentitlement.ONETIMESTORAGEQTY = bonderannualentitlementedt.BonderAnnualEntitlement.ONETIMESTORAGEQTY;
                bonderannualentitlement.OTMUSLNO = (Int16)UnitOT;
                bonderannualentitlement.ONETIMESTORAGEQTYEN = bonderannualentitlementedt.BonderAnnualEntitlement.ONETIMESTORAGEQTYEN;

                bonderannualentitlement.ENTITLEPERCENTQTY = (Int32)entitlementepercentage;
                bonderannualentitlement.ETMUSLNO = (Int16)UnitET;
                bonderannualentitlement.ENTITLEPERCENTEN = bonderannualentitlementedt.BonderAnnualEntitlement.ENTITLEPERCENTEN;

                bonderannualentitlement.PERCENTVAL = bonderannualentitlementedt.BonderAnnualEntitlement.PERCENTVAL;

                bonderannualentitlement.ENTITLEFROM = bonderannualentitlementedt.BonderAnnualEntitlement.ENTITLEFROM;
                bonderannualentitlement.ENTITLETO = bonderannualentitlementedt.BonderAnnualEntitlement.ENTITLETO;

                db.Entry(bonderannualentitlement).State = EntityState.Modified;
                db.SaveChanges();


                return RedirectToAction("EntitlementDetails", new { id = bonderannualentitlementedt.BonderAnnualEntitlement.AESLNO });

            }


            catch (Exception e)
            {

                return RedirectToAction("EntitlementDetails", new { id = bonderannualentitlementedt.BonderAnnualEntitlement.AESLNO });

            }
        }



        public ActionResult EditStorageInfo(int? aeslno, int? bonderslno)
        {
            
            BONDERANNUALENTITLEMENT bonderannualentitlement = db.BONDERANNUALENTITLEMENTs.Find(aeslno, bonderslno);

            ViewBag.UnitTS = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", bonderannualentitlement.TSMUSLNO);
            ViewBag.UnitOT = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", bonderannualentitlement.OTMUSLNO);
            ViewBag.UnitET = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME", bonderannualentitlement.ETMUSLNO);

            return View(bonderannualentitlement);
        }


        [HttpPost]
        public ActionResult EditStorageInfo(BONDERANNUALENTITLEMENT bonderannualentitlementedited, int? aeslno, int? blsno, int? UnitTS, int? UnitOT, int? UnitET)
        {
            
            BONDERANNUALENTITLEMENT bonderannualentitlement = new BONDERANNUALENTITLEMENT();
            
            if (ModelState.IsValid)
            {

                try
                {

                    bonderannualentitlement.AESLNO = bonderannualentitlementedited.AESLNO;
                    bonderannualentitlement.BONDERSLNO = bonderannualentitlementedited.BONDERSLNO;

                    bonderannualentitlement.TOTALSTORAGEQTY = bonderannualentitlementedited.TOTALSTORAGEQTY;
                    bonderannualentitlement.TSMUSLNO = (Int16)UnitTS;
                    bonderannualentitlement.TOTALSTORAGEQTYEN = bonderannualentitlementedited.TOTALSTORAGEQTYEN;

                    bonderannualentitlement.ONETIMESTORAGEQTY = bonderannualentitlementedited.ONETIMESTORAGEQTY;
                    bonderannualentitlement.TSMUSLNO = (Int16)UnitOT;
                    bonderannualentitlement.ONETIMESTORAGEQTY = bonderannualentitlementedited.ONETIMESTORAGEQTY;

                    bonderannualentitlement.ENTITLEPERCENTQTY = bonderannualentitlementedited.ENTITLEPERCENTQTY;
                    bonderannualentitlement.TSMUSLNO = (Int16)UnitET;
                    bonderannualentitlement.ENTITLEPERCENTEN = bonderannualentitlementedited.ENTITLEPERCENTEN;

                    bonderannualentitlement.PERCENTVAL = bonderannualentitlementedited.PERCENTVAL;

                    bonderannualentitlement.ENTITLEFROM = bonderannualentitlementedited.ENTITLEFROM;
                    bonderannualentitlement.ENTITLETO = bonderannualentitlementedited.ENTITLETO;


                    db.Entry(bonderannualentitlement).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("EntitlementDetails", new { id = bonderannualentitlement.AESLNO });
                    return RedirectToAction("EntitlementDetails", new { id = bonderannualentitlement.AESLNO, bonderSlNo = bonderannualentitlement.BONDERSLNO });

                }
                catch (Exception e)
                {


                }

        }

            //ViewBag.MachineList = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM", bonderannualentitlementedited.MACHINESLNO);
            return View();


        }


        public ActionResult AddCoefficientRawMaterials(int? aeslno, int? bonderslno, int? mslno)
        {


            ViewBag.aeslno = aeslno;
            ViewBag.bonderslno = bonderslno;
            ViewBag.materialname = (from m in db.MATERIALS
                                    where m.MSLNO == mslno
                                    select m.MATERIALNAME).SingleOrDefault(); ;

            ViewBag.mslno = mslno;
            ViewBag.Rawmaterilas = new SelectList(db.MATERIALS.Where(r => r.MTYPE == "r"), "MSLNO", "MATERIALNAME");

            return View();
        }

        [HttpPost]
        public ActionResult AddCoefficientRawMaterials(BONDERANNUALENTITLEMENTCOEDET bonderannualentitlementcoedetedited, int? aeslno, int? bonderslno, int? mslno, int? Rawmaterilas)
        {



            BONDERANNUALENTITLEMENTCOEDET bonderannualentitlementcoedet = new BONDERANNUALENTITLEMENTCOEDET();

            bonderannualentitlementcoedet.AESLNO = (Int16)aeslno;
            bonderannualentitlementcoedet.BONDERSLNO = (Int16)bonderslno;
            bonderannualentitlementcoedet.MSLNO = (Int16)mslno;
            bonderannualentitlementcoedet.RMSLNO = (Int16)Rawmaterilas;
            bonderannualentitlementcoedet.GROSSQT = (Int32)bonderannualentitlementcoedetedited.GROSSQT;
            bonderannualentitlementcoedet.WASTAGEQT = (Int32)bonderannualentitlementcoedetedited.WASTAGEQT;
            bonderannualentitlementcoedet.SHRINKAGEQT = (Int32)bonderannualentitlementcoedetedited.SHRINKAGEQT;
            bonderannualentitlementcoedet.NETQT = (Int32)bonderannualentitlementcoedetedited.NETQT;

            db.BONDERANNUALENTITLEMENTCOEDETs.Add(bonderannualentitlementcoedet);
            db.SaveChanges();



            return RedirectToAction("EditCoefficientMas", new { id = aeslno, id2 = bonderslno, id3 = mslno });



        }


        public ActionResult GetRawMaterial(int? id)
        {
            String[] data = new String[5];
            int? unit = 0;
            data[0] = (from a in db.MATERIALS where a.MSLNO == id select a.MHSCODE).SingleOrDefault();
            data[1] = (from a in db.MATERIALS where a.MSLNO == id select a.MDESCRIPTION).SingleOrDefault();
            data[2] = (from a in db.MATERIALS where a.MSLNO == id select a.SPGRADE).SingleOrDefault();
            unit = (Int16)(from a in db.MATERIALS where a.MSLNO == id select a.MUSLNO).SingleOrDefault();
            data[3] = (from a in db.MEASUREMENTUNITs where a.MUSLNO == unit select a.MUNAME).SingleOrDefault();
            data[4] = unit.ToString();


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProduct(int? id)
        {
            String[] data = new String[3];
            int? unit = 0;
            data[0] = (from a in db.MATERIALS where a.MSLNO == id select a.MHSCODE).SingleOrDefault();
            data[1] = (from a in db.MATERIALS where a.MSLNO == id select a.MDESCRIPTION).SingleOrDefault();
            unit = (Int16)(from a in db.MATERIALS where a.MSLNO == id select a.MUSLNO).SingleOrDefault();
            data[2] = (from a in db.MEASUREMENTUNITs where a.MUSLNO == unit select a.MUNAME).SingleOrDefault();


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCoRawMaterial(int? id)
        {
            String[] data = new String[2];
            int? unit = 0;
            unit = (Int16)(from a in db.MATERIALS where a.MSLNO == id select a.MUSLNO).SingleOrDefault();
            data[0] = (from a in db.MEASUREMENTUNITs where a.MUSLNO == unit select a.MUNAME).SingleOrDefault();
            data[1] = (from a in db.MATERIALS where a.MSLNO == id select a.MHSCODE).SingleOrDefault();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CalculateStorageInfo1(string[] ary, int? prtg)
        {
            //String[] data = new String[2];
            // int? unit = 0;
            //unit = (Int16)(from a in db.MATERIALS where a.MSLNO == id select a.MUSLNO).SingleOrDefault();
            //data[0] = (from a in db.MEASUREMENTUNITs where a.MUSLNO == unit select a.MUNAME).SingleOrDefault();
            //data[1] = (from a in db.MATERIALS where a.MSLNO == id select a.MHSCODE).SingleOrDefault();

            return Json(ary, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ApproveEntitlement()
        {



            return View();
        }


    }
}

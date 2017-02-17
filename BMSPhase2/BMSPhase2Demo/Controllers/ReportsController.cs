using BMSPhase2Demo.Models;
using BMSPhase2Demo.Report;
using BMSPhase2Demo.Utils;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CrystalDecisions.Shared;
using BMSPhase2Demo.CustomDataSet;
using System.Reflection;
using BMSPhase2Demo.Util;
using System.Globalization;


namespace BMSPhase2Demo.Controllers
{
    public class ReportsController : Controller
    {
        class UPJSON
        {
            public short ID { get; set; }
            public string UPNO { get; set; }
        }

        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();
        SessionAttributeRetreival sessionAttributeRetreival = new SessionAttributeRetreival();
        
        private readonly string INBOND_EXBOND_RAWMATERIAL_REPORT_DATA = "InbondExBondRawMaterialReportData";
        
        //
        // GET: /Reports/


        [CustomAuthorize(Roles = "Operation Admin, Bonder")]
        public ActionResult Index()
        {
            BonderInfo bonderInfo = new BonderInfo();
            //ViewModels.BonderWithReport1 bonderWithReport = new ViewModels.BonderWithReport1();
            return View();
        }

        [CustomAuthorize(Roles = "Operation Admin, Bonder")]
        public ActionResult InbondExbondRawMaterialStatus(int? BonderID, Nullable<System.DateTime> FromDate, Nullable<System.DateTime> ToDate)
        {

            ReportViewerViewModel rvvm = new ReportViewerViewModel();
            string contentPath = Url.Content("~/Report/CrystalViewer/InBondExBondRptVw.aspx");
            rvvm.reportPath = contentPath;

            if (null != BonderID && null != FromDate && null != ToDate)
            {
                ViewBag.SearchBonderID = BonderID;
                ViewBag.SearchFromDate = FromDate;
                ViewBag.SearchToDate = ToDate;

                Session[AppConstants.SearchBonderID] = BonderID;
                Session[AppConstants.SearchFromDate] = FromDate;
                Session[AppConstants.SearchToDate] = ToDate;

                return View("ReportViewerPage", rvvm);
            }

            ViewBag.BonderID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            return View();
            
            
            //Session[INBOND_EXBOND_RAWMATERIAL_REPORT_DATA] = null;
            //List<InBondExBondMaterial> RawMaterialList = null;
            //if (null != BonderID && null != FromDate && null != ToDate)
            //{
            //    ViewBag.SearchBonderID = BonderID;
            //    ViewBag.SearchFromDate = FromDate;
            //    ViewBag.SearchToDate = ToDate;

            //    RawMaterialList = getInBondExBondQueryData(BonderID, FromDate, ToDate);
            //}

            //ViewBag.BonderID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            //Session[INBOND_EXBOND_RAWMATERIAL_REPORT_DATA] = RawMaterialList;
            //return View(RawMaterialList);
        }

        //--------------Mizan Work (23-Aug-16 - 31-Aug-16)------------------------------------

        public ActionResult InBondImportReport()
        {
           
            ViewBag.BonderID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            ViewBag.RawMaterial = new SelectList(db.MATERIALS, "MATERIALNAME", "MATERIALNAME");

            return View("InBondImport");
        }
        [HttpPost]
        public ActionResult InBondImportReport(int BonderID, Nullable<System.DateTime> FromDate, Nullable<System.DateTime> ToDate, string RawMaterial)
        {

            return File(getReportStream("~/Report/Crystal/InBond_ImportReport.rpt", new string[] { "ParamBonderSlNo", "ParamRawMaterial", "fromDate", "toDate" }, new object[] { BonderID, RawMaterial, FromDate, ToDate }), "application/pdf");
        }
        public ActionResult ExBondUsageReport()
        {
            
            ViewBag.BonderID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            ViewBag.RawMaterial = new SelectList(db.MATERIALS, "MATERIALNAME", "MATERIALNAME");
            return View("ExBondUsage");
        }
        [HttpPost]
        public ActionResult ExBondUsageReport(int BonderID, Nullable<System.DateTime> FromDate, Nullable<System.DateTime> ToDate, string RawMaterial)
        {

            return File(getReportStream("~/Report/Crystal/ExBond_UsageReport.rpt", new string[] { "paramBonderSlNo", "rawMaterial", "fromDate", "toDate" }, new object[] { BonderID, RawMaterial, FromDate, ToDate }), "application/pdf");
        }
        public ActionResult BonderExportReport()
        {

            ViewBag.BonderID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            return View("BonderExportReport");
        }
        [HttpPost]
        public ActionResult BonderExportReport(int BonderID, Nullable<System.DateTime> FromDate, Nullable<System.DateTime> ToDate)
        {

            return File(getReportStream("~/Report/Crystal/BonderExportReport.rpt", new string[] { "paramBonderSlNo", "fromDate", "toDate" }, new object[] { BonderID, FromDate, ToDate }), "application/pdf");
        }
        public ActionResult TotalExportReport()
        {

            return View("TotalExportReport");
        }
        [HttpPost]
        public ActionResult TotalExportReport(Nullable<System.DateTime> FromDate, Nullable<System.DateTime> ToDate)
        {

            return File(getReportStream("~/Report/Crystal/TotalExportReport.rpt", new string[] { "fromDate", "toDate" }, new object[] { FromDate, ToDate }), "application/pdf");
        }
        public Stream getReportStream(string reportPath, string[] paramName = null, object[] paramVal = null)
        {
            ReportDocument rpt = new ReportDocument();
            rpt.Load(Server.MapPath(reportPath));

            ConnectionInfo connectInfo = new ConnectionInfo()
            {
                ServerName = CommonAppSet.ConnInfo.server,
                DatabaseName ="",
                UserID =CommonAppSet .ConnInfo.userId ,
                Password =CommonAppSet.ConnInfo.pass 
            };
            

            rpt.SetDatabaseLogon(connectInfo.UserID, connectInfo.Password, connectInfo.ServerName, "");

            foreach (Table tbl in rpt.Database.Tables)
            {
                tbl.LogOnInfo.ConnectionInfo = connectInfo;
                tbl.ApplyLogOnInfo(tbl.LogOnInfo);
            }
            for (int i = 0; i < rpt.Subreports.Count; i++)
            {
                rpt.Subreports[i].SetDatabaseLogon(connectInfo.UserID, connectInfo.Password, connectInfo.ServerName, "");
                foreach (Table tbl in rpt.Subreports[i].Database.Tables)
                {
                    tbl.LogOnInfo.ConnectionInfo = connectInfo;
                    tbl.ApplyLogOnInfo(tbl.LogOnInfo);
                }
            }
            if (paramName != null && paramVal != null)
            {
                for (int i = 0; i < paramName.Length; i++)
                {
                    rpt.SetParameterValue(paramName[i], paramVal[i]);
                }
            }
            return rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            
        }

        [HttpGet]
        public ActionResult FillMaterial(int bondersl)
        {
      
           
            List<Int16> rawMatcode = new List<Int16>();
            List<String> material = new List<String>();
            int rawMaterial = 0 ;
           
            rawMatcode = (from x in db.INBONDRAWMATERIALs where x.INBOND.BONDERID == bondersl select x.RAWMATERIALCODE).ToList();
                foreach (var rawMaterialcode in rawMatcode)
                {
                   rawMaterial = rawMaterialcode;
                   var rawMatName = (from a in db.MATERIALS where a.MSLNO == rawMaterial select a.MATERIALNAME).FirstOrDefault  ();
                   material.Add(rawMatName);
                }
            return Json(material, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult FillExBondMaterial(int bonderslNo)
        {
            List<Int16> rawMatcode = new List<Int16>();
            List<String> material = new List<String>();
            int rawMaterial = 0;

             rawMatcode = (from y in db.RAWMATERIALs where y.BACKTOBACKPRODUCT.BACKTOBACKLC.BONDERID == bonderslNo select y.RAWMATERIALCODE).ToList();
             foreach (var rawMaterialcode in rawMatcode)
             {
                 rawMaterial = rawMaterialcode;
                 var rawMatName = (from a in db.MATERIALS where a.MSLNO == rawMaterial select a.MATERIALNAME).FirstOrDefault();
                 material.Add(rawMatName);
             }

             return Json(material, JsonRequestBehavior.AllowGet);
        }

        //--------------Mizan Work (23-Aug-16 - 31-Aug-16)-----------------------------------------------------------------

        [CustomAuthorize(Roles = "Operation Admin, Bonder")]
        public ActionResult ProductAndRawMaterialStatus(int? BonderID, int? UPNO)
        {
            var requestType = this.HttpContext.Request.RequestType;
            System.Diagnostics.Debug.WriteLine("requestType = " + requestType);

            List<BonderProductRawMaterial> bonderProductRawMaterialList = null;

            if ("GET" == requestType)
            {

            }
            else if ("POST" == requestType)
            {
                if (null != BonderID && null != UPNO)
                {
                    ViewBag.SearchBonderID = BonderID;
                    ViewBag.SearchUPNO = UPNO;

                    //Do the query based on parameters
                    bonderProductRawMaterialList = getBonderProductRawMaterialQueryData(UPNO);
                    
                    if (null != bonderProductRawMaterialList && bonderProductRawMaterialList.Count == 0)
                    {
                        bonderProductRawMaterialList = null;
                        ViewBag.ErrorMsg = "No data found!!!! ";
                    }

                }
                else
                {
                    ViewBag.ErrorMsg = "Please select Bonder and UP No!!!! ";
                }
            }
            else
            {
                // Error handel
            }

            

            USERPERMISSION loggedinUser = sessionAttributeRetreival.getStoredUserPermission();
            if (loggedinUser != null && loggedinUser.BONDERID != null)
            {
                ViewBag.BonderID = new SelectList(db.BONDERs.Where(b => b.BONDERSLNO == loggedinUser.BONDERID), "BONDERSLNO", "BONDERNAME", loggedinUser.BONDERID);
            }
            else
            {
                ViewBag.BonderID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            }

            if (null != UPNO)
            {
                ViewBag.UPNO = new SelectList(db.UPs.Where(b => b.BONDERID == BonderID), "ID", "UPNO", UPNO);
            }
            else
            {
                ViewBag.UPNO = new SelectList(db.UPs.Where(b => b.BONDERID == 0), "ID", "UPNO");
            }
            

            return View(bonderProductRawMaterialList);
        }

        [CustomAuthorize(Roles = "Operation Admin, Bonder")]
        public JsonResult UPFromBonderID(int? BonderID)
        {

            List<UPJSON> upList = new List<UPJSON>();
            if (null != BonderID)
            {
                //upList = db.UPs.Where(b => b.BONDERID == BonderID).ToList();
                //ViewBag.UPNO = new SelectList(db.UPs.Where(b => b.BONDERID == BonderID), "ID", "UPNO");
                String queryStr = "Select ID, UPNO from  UP where BONDERID = "+ BonderID;
                upList = db.Database.SqlQuery<UPJSON>(queryStr).ToList();
            }

            JsonResult result = Json(new { upList }, JsonRequestBehavior.AllowGet);
            return result;
        }

        [CustomAuthorize(Roles = "Operation Admin, Bonder")]
        public ActionResult ExportInBondExBondRawMaterialReport(int? SearchBonderID, Nullable<System.DateTime> SearchFromDate, Nullable<System.DateTime> SearchToDate)
        {
            if (null != SearchBonderID && null != SearchFromDate && null != SearchToDate)
            {
                List<INBOND> inbondList = getInbondList(SearchBonderID, SearchFromDate, SearchToDate);

                List<BonderInfo> bonderInfoList = new List<BonderInfo>();
                BonderInfo bonderInfo = null;

                foreach (var inbondInfo in inbondList)
                {
                    bonderInfo = new BonderInfo();
                    bonderInfo.BonderName = inbondInfo.BONDER.BONDERNAME;
                    bonderInfo.BonderLCNo = inbondInfo.LCNUMBER;

                    for (int i = 0; i < inbondInfo.INBONDRAWMATERIALs.Count; i++)
                    {
                        RawMaterialInfo rw = new RawMaterialInfo();
                        rw.BonderID = (short)inbondInfo.BONDERID;
                        rw.RawMaterialName = inbondInfo.INBONDRAWMATERIALs[i].MATERIAL.MHSCODE;
                        rw.HSCode = inbondInfo.INBONDRAWMATERIALs[i].MATERIAL.MHSCODE;

                        bonderInfo.RawMaterialInfoList.Add(rw);
                    }

                    bonderInfoList.Add(bonderInfo);

                }


                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Report/Crystal"), "InBondExBondRawMaterialStatus_1.rpt"));
                
                rd.SetDataSource(DataSourceUtil.ToDataSet(bonderInfoList));

                rd.SetDatabaseLogon("", "","","");
                

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                

                //rd.SetDatabaseLogon("", "", "192.168.40.55:1521/orcl", "");
                try
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "InBondRawMaterial.pdf");
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("ExportInBondExBondRawMaterialReport: Time-" + DateTime.Now + " Exception = " + e.ToString());
                    ViewBag.ErrorMsg = "Export InBondExBond Rawmaterial Report Development is On going";
                    throw e;
                }
            }
            //return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            ViewBag.BonderID = new SelectList(db.BONDERs, "BONDERSLNO", "BONDERNAME");
            return View("InbondExbondRawMaterialStatus");
        }

        [CustomAuthorize(Roles = "Operation Admin, Bonder")]
        public ActionResult ExportProductRawMaterialReport(int? SearchBonderID, int? SearchUPNO)
        {

            List<BonderProductRawMaterial> bonderProductRawMaterialList = null;
            if (null != SearchBonderID && null != SearchUPNO)
            {
                bonderProductRawMaterialList = getBonderProductRawMaterialQueryData(SearchUPNO);
            }

            if (null == bonderProductRawMaterialList) return RedirectToAction("ProductAndRawMaterialStatus");

            return exportReport("ProductRawMaterialUsegesReport", "BonderProductRawMaterial", bonderProductRawMaterialList, "ProductRawMaterialUsage");           
            
        }


        private List<INBONDRAWMATERIAL> getRawMaterialList()
        {
            List<INBONDRAWMATERIAL> inbondRawMaterials = new List<INBONDRAWMATERIAL>();
            IQueryable<INBONDRAWMATERIAL> irm = db.INBONDRAWMATERIALs;
            inbondRawMaterials = irm.ToList();

            return inbondRawMaterials;
        }

        private List<BACKTOBACKPRODUCT> getBack2BackProductList()
        {
            List<BACKTOBACKPRODUCT> back2backProductList = new List<BACKTOBACKPRODUCT>();
            IQueryable<BACKTOBACKPRODUCT> b2bp = db.BACKTOBACKPRODUCTs;
            back2backProductList = b2bp.ToList();

            return back2backProductList;
        }

        private List<INBOND> getInbondList(int? BonderID, Nullable<System.DateTime> FromDate, Nullable<System.DateTime> ToDate)
        {
            List<INBOND> inbondList = new List<INBOND>();

            inbondList = db.INBONDs.Where(m => m.BONDERID == BonderID && m.LCDATE >= FromDate && m.LCDATE <= ToDate).ToList();

            return inbondList;
        }

        //Get report 1 data
        public List<InBondExBondMaterial> getInBondExBondQueryData(int? SearchBonderID, Nullable<System.DateTime> SearchFromDate, Nullable<System.DateTime> SearchToDate)
        {
            List<InBondExBondMaterial> reportData = new List<InBondExBondMaterial>();
            
            string queryStr1 = "SELECT"
                               + " BONDER.BONDERSLNO AS BonderID,"
                               + " BONDER.BONDERNAME AS BonderName,"
                               + " BONDER.BONDLICENSENO AS BonderLCNo,"
                               + " BONDER.ADDRESS AS BonderAddress,"
                               + " INBOND.BOENUMBER AS BOE,"
                               + " INBOND.LCDATE AS ImportDate,"
                               + " INBOND.LCNUMBER AS LC,"
                               + " INBOND.CREATEDDATE AS InBondDate,"
                               + " INBONDRAWMATERIAL.PRODUCTQUANTITY AS InBondQuantity,"
                               + " INBONDRAWMATERIAL.QUANTITYUNIT AS InBondQuantityUnit,"
                               + " INBONDRAWMATERIAL.PRODUCTCOST AS InBondValue,"
                               + " INBONDRAWMATERIAL.COSTUNIT AS InBondValueUnit,"
                               + " MATERIALS.MHSCODE AS HSCode,"
                               + " MATERIALS.MATERIALNAME AS RawMaterialName,"
                               + " MATERIALS.MSLNO AS RAWMATERIALCODE"
                               + " FROM INBONDRAWMATERIAL"
                               + " INNER JOIN INBOND ON INBOND.ID = INBONDRAWMATERIAL.INBONDID"
                               + " INNER JOIN MATERIALS ON MATERIALS.MSLNO = INBONDRAWMATERIAL.RAWMATERIALCODE"
                               + " INNER JOIN BONDER ON BONDER.BONDERSLNO = INBOND.BONDERID"

                               + " WHERE INBOND.BONDERID = 383 and INBOND.CREATEDDATE BETWEEN TO_DATE('" + SearchFromDate + "', '" + AppConstants.DATE_FORMATE + "')"
                               + "     AND TO_DATE('" + SearchToDate + "', '" + AppConstants.DATE_FORMATE + "')"
                               + "     ORDER BY MATERIALS.MATERIALNAME, INBOND.CREATEDDATE";

            reportData = db.Database.SqlQuery<InBondExBondMaterial>(queryStr1).ToList();

            List<InBondExBondMaterial> finalData = new List<InBondExBondMaterial>();
            //Update data based on response
            for (int i = 0; i < reportData.Count(); i++)
            {
                InBondExBondMaterial ibebm = reportData.ElementAt(i);
                //Adding fromdate and todate value to show in report
                
                ibebm.DateFrom = SearchFromDate ?? DateTime.Now;
                ibebm.DateTo = SearchToDate ?? DateTime.Now;
                finalData.Add(ibebm);
                List<InBondExBondMaterial> exbondList = new List<InBondExBondMaterial>();
                if (i == reportData.Count - 1)
                {
                    //Last element. add all exbond from this to SearchToDate
                    exbondList = exbondListByRawMaterialInDate(ibebm, ibebm.DateTo, false);
                }
                else
                {   //Get the next element and check if it is same material
                    InBondExBondMaterial ibNext = reportData.ElementAt(i + 1);
                    if (ibNext.RAWMATERIALCODE.Equals(ibebm.RAWMATERIALCODE))
                    {
                        exbondList = exbondListByRawMaterialInDate(ibebm, ibNext.InBondDate, true);
                    }
                    else
                    {
                        //Last element of this kind. add all exbond from this to SearchToDate
                        exbondList = exbondListByRawMaterialInDate(ibebm, ibebm.DateTo, false);
                    }
                }
                finalData.AddRange(exbondList);
            }

            return finalData;
            

            ////Old implementation

            //String strquery = "SELECT BONDER.BONDERNAME AS BonderName, BONDER.ADDRESS AS BonderAddress,"
            //    + " BONDER.BONDLICENSENO AS BonderLCNo, MATERIALS.MATERIALNAME AS RawMaterialName,"
            //    + " MATERIALS.MHSCODE AS HSCode, INBOND.LCNUMBER AS LC, INBOND.LCDATE AS ImportDate, "
            //    + " INBOND.BOENUMBER AS BOE, INBONDRAWMATERIAL.PRODUCTQUANTITY AS InBondQuantity,"
            //    + " INBONDRAWMATERIAL.QUANTITYUNIT AS InBondQuantityUnit, INBONDRAWMATERIAL.PRODUCTCOST AS InBondValue,"
            //    + " INBONDRAWMATERIAL.COSTUNIT AS InBondValueUnit, INBOND.CREATEDDATE AS InBondDate,"
            //    + " (CASE WHEN RAWMATERIAL.QUANTITY IS NULL THEN 0.0 ELSE RAWMATERIAL.QUANTITY END) AS ExBondQuantity,"
            //    + " RAWMATERIAL.UNIT ExBondQuantityUnit,"
            //    + " (CASE WHEN EXBOND.CREATEDDATE IS NULL THEN SYSDATE ELSE EXBOND.CREATEDDATE END) AS ExBondDate "
            //    + " FROM INBONDRAWMATERIAL"
            //    + " INNER JOIN INBOND ON INBOND.ID = INBONDRAWMATERIAL.INBONDID"
            //    + " INNER JOIN MATERIALS ON MATERIALS.MSLNO = INBONDRAWMATERIAL.RAWMATERIALCODE"
            //    + " INNER JOIN BONDER ON BONDER.BONDERSLNO = INBOND.BONDERID"
            //    + " LEFT JOIN RAWMATERIAL ON RAWMATERIAL.RAWMATERIALCODE =INBONDRAWMATERIAL.RAWMATERIALCODE"
            //    + " LEFT JOIN BACKTOBACKPRODUCT ON BACKTOBACKPRODUCT.ID = RAWMATERIAL.PRODUCTID"
            //    + " LEFT JOIN BACKTOBACKLC ON BACKTOBACKLC.ID = BACKTOBACKPRODUCT.BACKTOBACKLCID"
            //    + " LEFT JOIN EXBONDBACKTOBACK ON EXBONDBACKTOBACK.BACKTOBACKID = BACKTOBACKLC.ID"
            //    + " LEFT JOIN EXBOND ON EXBOND.ID = EXBONDBACKTOBACK.EXBONDID"
            //    + " WHERE BONDER.BONDERSLNO = " + SearchBonderID + " AND INBOND.CREATEDDATE BETWEEN TO_DATE('" + SearchFromDate + "', 'DD-MON-yy HH:MI:SS PM')"
            //    + " AND TO_DATE('" + SearchToDate + "', 'DD-MON-yy HH:MI:SS PM')";

            //using (StreamWriter _testData = new StreamWriter(Server.MapPath("~/data.txt"), true))
            //{
            //    _testData.WriteLine(SearchFromDate);
            //    _testData.WriteLine(SearchToDate);
            //    _testData.WriteLine(strquery); // Write the file.
            //}

            //reportData = db.Database.SqlQuery<InBondExBondMaterial>(strquery).ToList();
            
            //return reportData;
        }

        public ActionResult InBondExBondRawMaterialReport(int? SearchBonderID, Nullable<System.DateTime> SearchFromDate, Nullable<System.DateTime> SearchToDate)
        {
            List<InBondExBondMaterial> RawMaterialList = (List<InBondExBondMaterial>)Session[INBOND_EXBOND_RAWMATERIAL_REPORT_DATA];
            if (RawMaterialList == null)
            {
                if (null != SearchBonderID && null != SearchFromDate && null != SearchToDate)
                {
                    RawMaterialList = getInBondExBondQueryData(SearchBonderID, SearchFromDate, SearchToDate);
                }

                if (null == RawMaterialList) return View(RawMaterialList);
            }

            return exportReport("InBondExBondRawMaterialStatus_2", "InBondExBondRawMaterial", RawMaterialList, "InBondExBondRawMaterial");
        }

        //Get report 1 data
        public List<BonderProductRawMaterial> getBonderProductRawMaterialQueryData(int? SearchBonderID)
        {
            List<BonderProductRawMaterial> reportData = new List<BonderProductRawMaterial>();
            
            String queryStr = " SELECT  BONDER.BONDERNAME AS BonderName, BONDER.ADDRESS AS BonderAddress, BONDER.BONDLICENSENO AS BonderLCNo, UP.UPNO AS UPNo, UP.CREATEDDATE AS DateFrom, UP.CREATEDDATE AS DateTo, "
                            + "          BACKTOBACKLC.BUYERSNAME AS BuyerName, BACKTOBACKLC.UDNUMBER AS UDNo, BACKTOBACKLC.UDDATE AS UDDate, BACKTOBACKLC.UDPRODUCTDETAIL AS UDProduct, 700 AS UDProductQuantity, 'Pis' AS UDProductQuantityUnit, "
                            + "          BACKTOBACKPRODUCT.NAME AS ProductName, BACKTOBACKPRODUCT.SIZEANDDETAIL AS ProductSize, BACKTOBACKPRODUCT.SIZEANDDETAIL AS ProductDescription, BACKTOBACKPRODUCT.QUANTITY AS ProductQuantity, BACKTOBACKPRODUCT.QUANTITYUNIT AS ProductQuantityUnit, "
                            + "          MATERIALS.MATERIALNAME AS RawMaterialName, RAWMATERIAL.QUANTITY AS RawMaterialQuantity, RAWMATERIAL.UNIT AS RawMaterialQuantityUnit, RAWMATERIAL.PERMITTEDWASTE AS QuantityWastage, (NVL(RAWMATERIAL.QUANTITY, 0) + (NVL(RAWMATERIAL.QUANTITY, 0) * NVL(RAWMATERIAL.PERMITTEDWASTE, 0)) / 100) AS TotalUsageQuantity, RAWMATERIAL.UNIT AS TotalUsageQuantityUnit  "

                            + "  FROM UP "
                            + "      INNER JOIN BONDER ON BONDER.BONDERSLNO = UP.BONDERID "

                            + "      INNER JOIN UPREQUESTLIST ON UPREQUESTLIST.UPID = UP.ID "
                            + "      INNER JOIN UPREQUEST ON UPREQUEST.ID = UPREQUESTLIST.UPREQUESTID "
                            + "      INNER JOIN UPEXBONDLIST ON UPEXBONDLIST.UPREQUESTID = UPREQUEST.ID "
                            + "      INNER JOIN EXBOND ON EXBOND.ID = UPEXBONDLIST.EXBONDID "
                            + "      INNER JOIN EXBONDBACKTOBACK ON EXBONDBACKTOBACK.EXBONDID = EXBOND.ID "

                            + "      INNER JOIN BACKTOBACKLC ON BACKTOBACKLC.ID = EXBONDBACKTOBACK.BACKTOBACKID "
                            + "      INNER JOIN BACKTOBACKPRODUCT ON BACKTOBACKPRODUCT.BACKTOBACKLCID = BACKTOBACKLC.ID "
                            + "      INNER JOIN RAWMATERIAL ON RAWMATERIAL.PRODUCTID = BACKTOBACKPRODUCT.ID "
                            + "      INNER JOIN MATERIALS ON MATERIALS.MSLNO = RAWMATERIAL.RAWMATERIALCODE "
                            + " WHERE UP.ID = " + SearchBonderID ;

            //using (StreamWriter _testData = new StreamWriter(Server.MapPath("~/data.txt"), true))
            //{
            //    _testData.WriteLine(queryStr); // Write the file.
            //}
            try
            {
                reportData = db.Database.SqlQuery<BonderProductRawMaterial>(queryStr).ToList();
                
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("getBonderProductRawMaterialQueryData: Time-" + DateTime.Now + " Exception = " + e.ToString());
                
                throw e;
            }
            

            return reportData;

        }
        

        private ActionResult exportReport<T>(string reportFileName, string modelName, IList<T> dataList, string outputFileName)
        {
            //Prepare report
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report/Crystal"), reportFileName + ".rpt"));

            //Set datasource
            InBondExBondRawMaterial_01 ibrd = new InBondExBondRawMaterial_01();
            DataTable inbondTb = ibrd.Tables[ibrd.Tables.IndexOf(modelName)];

            DataRow dr;
            foreach (object ib in dataList)
            {
                dr = inbondTb.NewRow();
                DataColumnCollection dcc = inbondTb.Columns;
                foreach (DataColumn dc in dcc)
                {
                    Type tp = ib.GetType();
                    PropertyInfo pif = tp.GetProperty(dc.ColumnName);
                    var value = pif.GetValue(ib);
                    dr[dc.ColumnName] = ib.GetType().GetProperty(dc.ColumnName).GetValue(ib, null);
                }
                inbondTb.Rows.Add(dr);
            }

            rd.SetDataSource(inbondTb);
            

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", outputFileName + ".pdf");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("BMSReport: Time-" + DateTime.Now + " Exception = " + e.ToString());
                ViewBag.ErrorMsg = "Report Development is On going";
                throw e;
            }
        }

        private List<InBondExBondMaterial> exbondListByRawMaterialInDate(InBondExBondMaterial currentMaterial, System.DateTime lastDate, bool isBefore)
        {
            List<InBondExBondMaterial> exbondList = new List<InBondExBondMaterial>();
            String checkStr = "<=";
            if (isBefore) checkStr = "<";
            string exbondQuery = "SELECT"
                                  + " EXBOND.CREATEDDATE AS ExBondDate,"
                                  + " RAWMATERIAL.QUANTITY AS ExBondQuantity,"
                                  + " RAWMATERIAL.UNIT AS ExBondQuantityUnit"
  
                                  + " FROM EXBOND"
                                  + " INNER JOIN EXBONDBACKTOBACK ON EXBONDBACKTOBACK.EXBONDID = EXBOND.ID"
                                  + " INNER JOIN BACKTOBACKLC ON BACKTOBACKLC.ID = EXBONDBACKTOBACK.BACKTOBACKID"
                                  + " INNER JOIN BACKTOBACKPRODUCT ON BACKTOBACKPRODUCT.BACKTOBACKLCID = BACKTOBACKLC.ID"
                                  + " INNER JOIN RAWMATERIAL ON RAWMATERIAL.PRODUCTID = BACKTOBACKPRODUCT.ID"
                                  + " INNER JOIN MATERIALS ON MATERIALS.MSLNO = RAWMATERIAL.RAWMATERIALCODE"
                                  + " WHERE MATERIALS.MSLNO = " + currentMaterial.RAWMATERIALCODE + " AND EXBOND.BONDERID = " + currentMaterial.BonderID + " AND EXBOND.CREATEDDATE >= TO_DATE('" + currentMaterial.InBondDate + "', '" + AppConstants.DATE_FORMATE + "') "
                                  + "   AND EXBOND.CREATEDDATE " + checkStr + " TO_DATE('" + lastDate +"', '" + AppConstants.DATE_FORMATE + "')";
            
            exbondList = db.Database.SqlQuery<InBondExBondMaterial>(exbondQuery).ToList();
            foreach (InBondExBondMaterial item in exbondList)
            {
                //set inbond related value from param
                item.BonderName = currentMaterial.BonderName;
                item.BonderLCNo = currentMaterial.BonderLCNo;
                item.DateFrom = currentMaterial.DateFrom;
                item.DateTo = currentMaterial.DateTo;
                item.BonderAddress = currentMaterial.BonderAddress;
                item.RawMaterialName = currentMaterial.RawMaterialName;
                item.HSCode = currentMaterial.HSCode;
                item.AnnualEntitlement = currentMaterial.AnnualEntitlement;
                item.TotalInBondOfThePeriod = currentMaterial.TotalInBondOfThePeriod;
                item.BalanceEntitlement = currentMaterial.BalanceEntitlement;
                item.AnnualEntitleUnit = currentMaterial.AnnualEntitleUnit;
                item.TotalInBondUnit = currentMaterial.TotalInBondUnit;
                item.BalanceEntitlementUnit = currentMaterial.BalanceEntitlementUnit;
                item.ImportDate = currentMaterial.ImportDate;
                item.LC = currentMaterial.LC;
                item.BOE = currentMaterial.BOE;
                item.InBondDate = currentMaterial.InBondDate;
                item.InBondQuantity = currentMaterial.InBondQuantity;
                item.InBondValue = currentMaterial.InBondValue;
                item.InBondQuantityUnit = currentMaterial.InBondQuantityUnit;
                item.InBondValueUnit = currentMaterial.InBondValueUnit;
                item.RAWMATERIALCODE = currentMaterial.RAWMATERIALCODE;
                item.BonderID = currentMaterial.BonderID;

            }
            return exbondList;
        }

    }
}

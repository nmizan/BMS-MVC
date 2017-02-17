using BMSPhase2Demo.CommonAppSet;
using BMSPhase2Demo.Models;
using BMSPhase2Demo.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.OracleClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace BMSPhase2Demo.Controllers
{
    public class BondRegistrationController : Controller
    {
        //
        // GET: /BondRegistration/

        SessionAttributeRetreival session = new SessionAttributeRetreival();
        OracleEntitiesConnStr db = new OracleEntitiesConnStr();
        String user = "Yasir";
        String saveMode = "update";
        int prevBondSlNo = 0;
        String path = "";
        byte[] blob = null;
        int pathCount = 1;

        //
        // GET: /BondRegistration/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /BondRegistration/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /BondRegistration/Create    

        public void setViewBagDataForCreate()
        {

            ViewBag.Upazila = new SelectList(db.UPAZILAs, "UPAZILASLNO", "UPAZILANAME");
            ViewBag.District = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");
            ViewBag.HoUpazila = new SelectList(db.UPAZILAs, "UPAZILASLNO", "UPAZILANAME");
            ViewBag.HoDistrict = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");
            ViewBag.BondType = new SelectList(db.BONDTYPEs, "BTYPESLNO", "BTYPENAME");
            ViewBag.BondCategory = new SelectList(db.BONDCATEGORies, "BCATSLNO", "BCATNAME");
            ViewBag.BondSubCategory = new SelectList(db.BONDSUBCATEGORies, "BSCATSLNO", "BSCATNAME");
            ViewBag.Office = new SelectList(db.OFFICEs, "OFFICESLNO", "OFFICENAME");
            ViewBag.Bank = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME");
            ViewBag.Branch = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME");
            ViewBag.OwnerPositionDesignation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME");
            ViewBag.OwnerPassportDistrict = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");
            ViewBag.fpHsCode = new SelectList(db.MATERIALS, "MSLNO", "MHSCODE");
            //ViewBag.fpDesc = new SelectList(db.MATERIALS, "MDESCRIPTION", "MDESCRIPTION");
            ViewBag.PWM_HS_Code = new SelectList(db.MATERIALS, "MHSCODE", "MHSCODE");
            ViewBag.IMI_Country_of_Origin = new SelectList(db.COUNTRies, "COUNTRYSLNO", "COUNTRYNAME");
            ViewBag.IMI_Brand_Name = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "BRAND_NAME");
            ViewBag.IMI_Model_No = new SelectList(db.MODELs, "MODELSLNO", "MODELNM");
            ViewBag.PWM_Model_No = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM");
            ViewBag.PWM_Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
            ViewBag.ABI_Designation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME");
            ViewBag.MPO_Bank = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME");
            ViewBag.MPO_Branch = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME");
            ViewBag.AIL_HS_Code = new SelectList(db.MATERIALS, "MHSCODE", "MHSCODE");
            ViewBag.AIL_Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
            ViewBag.Declare_Designation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME");
            ViewBag.BondCircle = new SelectList(db.BONDCIRCLEs, "BONDCIRCLESLNO", "BONDCIRCLENAME");
        }

        public void setViewBagDataForEdit()
        {

        }

        public FileStreamResult DownloadFileByPath(string filePath)
        {
            FileInfo info = new FileInfo(filePath);
            string value = info.Extension;
            string filename = info.Name;
            return File(new FileStream(filePath, FileMode.Open), value, filename);
        }

        public ActionResult getData(string table, string dataColumn, string idColumn, string id)
        {
            System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();

            System.Data.OracleClient.OracleConnection conn = ConnectBMS.Connection();
            cmd.Connection = conn;
            System.Data.OracleClient.OracleDataReader dr;

            cmd.CommandText = "select " + dataColumn + " from " + table + " where " + idColumn + "=:id";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id", id);
            dr = cmd.ExecuteReader();
            var data = "";
            //data = (from a in db.MATERIALS where a.MHSCODE == id select a.MDESCRIPTION).SingleOrDefault();
            dr.Read();
            if (dr.HasRows)
                data = dr.GetValue(0).ToString();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBranches(int intBankId)
        {
            var products = db.BANKBRANCHes
                .Where(p => p.BANKSLNO == intBankId)
                .Select(p => new { p.BBRANCHSLNO, p.BRANCHNAME })
                .OrderBy(p => p.BRANCHNAME);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Start()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Start(FormCollection colleciton)
        {

            String viewToReturn = "";
            //if (BondInfo.bondNameToEdit == "")
            //{
            if (!string.IsNullOrEmpty(Request["ManufacturingUnitName"]))
            {
                try
                {
                    BondInfo.bondNameToEdit = Request["ManufacturingUnitName"].Trim();
                }
                catch
                {
                    return View();
                }
            }
            //}
            try
            {
                if (!String.IsNullOrEmpty(BondInfo.bondNameToEdit))
                {
                    System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();
                    System.Data.OracleClient.OracleConnection conn = ConnectBMS.Connection();
                    System.Data.OracleClient.OracleTransaction bmsTransaction;

                    try
                    {
                        cmd.Connection = conn;
                        //cmd.Connection = ConnectBMS.Connection();
                        bmsTransaction = conn.BeginTransaction();
                        cmd.Transaction = bmsTransaction;


                        System.Data.OracleClient.OracleDataReader drBondEditInfo;
                        System.Data.OracleClient.OracleDataReader drNewBondInfo;

                        cmd.CommandText = "select BONDERSLNO,HOLDINGNO,ROADNO,AREAVILLAGE,MOWJA," +
                                    "UNIONNAME,BUPAZILASLNO,BDISTRICTSLNO,PSTATION,WARD,CITYPOURO,PHONE,FAX,MOBILE,EMAIL," +
            "HOHOLDINGNO,HOROADNO,HOAREAVILLAGE,HOMOWJA,HOUNION,HOUPAZILASLNO,HODISTRICTSLNO,HOPSTATION,HOWARD," +
            "HOCITYPOURO,HOPHONE,HOFAX,HOMOBILE,HOEMAIL, PREMESISSTATUS,HIREDATE,HIREEXPDATE,OWNERSHIPDATE,OWNERSHIPDOLILNO," +
            "DolilFileNM,DEEDFILENM,RENTVATCHALLANNO," +
            "VCHALNFILENM,CSDAGNO,RSDAGNO,BONDFACPREV,BONDLICENSENO,BTYPESLNO,BCATSLNO,BSCATSLNO,INPUTBY,INPUTDATE,TOTFACLENG," +
            "TOTFACWIDTH,TOTRMWHLENG,TOTRMWHWIDTH,TOTFGWHLENG,TOTFGWHWIDTH,AUTHORIZEDSIGNAME,INITIALAUTHSIGFILENM,PHOTOFILENM,"
                        + "DECLARNMBLOCK,DECLARSIGFILENM,DECLARSEALFILENM,DECLARDESSLNO,DECLARDATE,BONDSTATUS,ADDRESS,COADDRESS"
                        + " from BONDER where BONDERNAME=:BONDERNAME";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("BONDERNAME", BondInfo.bondNameToEdit);
                        drBondEditInfo = cmd.ExecuteReader();

                        //drBondEditInfo.Read();                    
                        ViewBag.ManufacturingUnitName = BondInfo.bondNameToEdit;


                        if (!drBondEditInfo.HasRows)
                        {
                            cmd.CommandText = "insert into BONDER(BONDSTATUS,BONDERNAME) values(:BONDSTATUS,:BONDERNAME)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("BONDSTATUS", "A");
                            cmd.Parameters.AddWithValue("BONDERNAME", BondInfo.bondNameToEdit);

                            try
                            {
                                cmd.ExecuteNonQuery();
                                bmsTransaction.Commit();
                            }
                            catch
                            {
                                bmsTransaction.Rollback();
                                return View();
                            }

                            cmd.CommandText = "select max(BONDERSLNO) from Bonder";
                            cmd.Parameters.Clear();
                            drNewBondInfo = cmd.ExecuteReader();
                            if (drNewBondInfo.Read())
                            {
                                BondInfo.newBondSlNo = drNewBondInfo.GetInt32(0);
                                BondInfo.bondSlNoToEdit = drNewBondInfo.GetInt32(0);
                            }
                            cmd.CommandText = "insert into BONDSTATUS(BONDERSLNO,STATUS) values(:BONDERSLNO,:STATUS)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.newBondSlNo);
                            cmd.Parameters.AddWithValue("STATUS", "N");
                            bmsTransaction = conn.BeginTransaction();
                            cmd.Transaction = bmsTransaction;
                            try
                            {
                                cmd.ExecuteNonQuery();
                                bmsTransaction.Commit();
                            }
                            catch
                            {
                                bmsTransaction.Rollback();
                                return View();
                            }

                            cmd.CommandText = "select max(BSNO) from BONDSTATUS";
                            cmd.Parameters.Clear();
                            System.Data.OracleClient.OracleDataReader drNewBSNo = cmd.ExecuteReader();
                            if (drNewBSNo.HasRows)
                            {
                                drNewBSNo.Read();
                                BondInfo.newBSNO = drNewBSNo.GetInt32(0);
                            }

                            cmd.CommandText = "insert into BONDAPPLICATIONPROGRESS(BONDERSLNO,BSNO,READYFORAPP) values(:BONDERSLNO,:BSNO,:READYFORAPP)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.newBondSlNo);
                            cmd.Parameters.AddWithValue("BSNO", BondInfo.newBSNO);
                            cmd.Parameters.AddWithValue("READYFORAPP", "N");
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

                            ViewBag.Upazila = new SelectList(db.UPAZILAs, "UPAZILASLNO", "UPAZILANAME");
                            ViewBag.District = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");
                            ViewBag.HoUpazila = new SelectList(db.UPAZILAs, "UPAZILASLNO", "UPAZILANAME");
                            ViewBag.HoDistrict = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");
                            ViewBag.BondType = new SelectList(db.BONDTYPEs, "BTYPESLNO", "BTYPENAME");
                            ViewBag.BondCategory = new SelectList(db.BONDCATEGORies, "BCATSLNO", "BCATNAME");
                            ViewBag.BondSubCategory = new SelectList(db.BONDSUBCATEGORies, "BSCATSLNO", "BSCATNAME");
                            ViewBag.Office = new SelectList(db.OFFICEs, "OFFICESLNO", "OFFICENAME");
                            ViewBag.Bank = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME");
                            ViewBag.Branch = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME");
                            ViewBag.OwnerPositionDesignation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME");
                            ViewBag.OwnerPassportDistrict = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");
                            ViewBag.fpHsCode = new SelectList(db.MATERIALS, "MSLNO", "MHSCODE");
                            ViewBag.PWM_HS_Code = new SelectList(db.MATERIALS, "MHSCODE", "MHSCODE");
                            ViewBag.fpDesc = new SelectList(db.MATERIALS, "MDESCRIPTION", "MDESCRIPTION");
                            ViewBag.IMI_Country_of_Origin = new SelectList(db.COUNTRies, "COUNTRYSLNO", "COUNTRYNAME");
                            ViewBag.IMI_Brand_Name = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "BRAND_NAME");
                            ViewBag.IMI_Model_No = new SelectList(db.MODELs, "MODELSLNO", "MODELNM");
                            ViewBag.PWM_Model_No = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM");
                            ViewBag.PWM_Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
                            ViewBag.ABI_Designation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME");
                            ViewBag.MPO_Bank = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME");
                            ViewBag.MPO_Branch = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME");
                            ViewBag.AIL_HS_Code = new SelectList(db.MATERIALS, "MHSCODE", "MHSCODE");
                            ViewBag.AIL_Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
                            ViewBag.Declare_Designation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME");
                            ViewBag.BondCircle = new SelectList(db.BONDCIRCLEs, "BONDCIRCLESLNO", "BONDCIRCLENAME");
                            viewToReturn = "Create";
                        }
                        else
                        {
                            //ViewBag.BONDERSLNO = drBondEditInfo.GetValue(0).ToString();
                            while (drBondEditInfo.Read())
                            {
                                BondInfo.bondSlNoToEdit = drBondEditInfo.GetInt32(0);

                                if (drBondEditInfo["BONDSTATUS"] != DBNull.Value)
                                    BondInfo.bondStatus = drBondEditInfo["BONDSTATUS"].ToString();
                                if (drBondEditInfo.GetValue(6) != DBNull.Value)
                                    BondInfo.bondUpazilaSlNo = drBondEditInfo.GetInt32(6);
                                if (drBondEditInfo.GetValue(7) != DBNull.Value)
                                    BondInfo.bondDistrictSlNo = drBondEditInfo.GetInt32(7);
                                if (drBondEditInfo.GetValue(20) != DBNull.Value)
                                    BondInfo.bondHoUpazilaSlNo = drBondEditInfo.GetInt32(20);
                                if (drBondEditInfo.GetValue(21) != DBNull.Value)
                                    BondInfo.bondHoDistrictSlNo = drBondEditInfo.GetInt32(21);
                                if (drBondEditInfo.GetValue(42) != DBNull.Value)
                                    BondInfo.bondTypeSlNo = drBondEditInfo.GetInt32(42);
                                if (drBondEditInfo.GetValue(43) != DBNull.Value)
                                    BondInfo.bondCategorySlNo = drBondEditInfo.GetInt32(43);
                                if (drBondEditInfo.GetValue(44) != DBNull.Value)
                                    BondInfo.bondSubCategorySlNo = drBondEditInfo.GetInt32(44);
                                if (drBondEditInfo.GetValue(59) != DBNull.Value)
                                    BondInfo.declareDesSlNo = drBondEditInfo.GetInt32(59);
                                string fieldName;
                                for (int i = 0; i < drBondEditInfo.FieldCount; i++)
                                {
                                    fieldName = drBondEditInfo.GetName(i);
                                    ViewData.Add(fieldName, drBondEditInfo.GetValue(i).ToString());
                                }
                                cmd.CommandText = "select BSNO from BONDSTATUS where BONDERSLNO=:BONDERSLNO";
                                //cmd.CommandText = "select BSNO from BONDSTATUS where BONDERSLNO=:BONDERSLNO and STATUS=:STATUS";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                                //cmd.Parameters.AddWithValue("STATUS", "A");
                                System.Data.OracleClient.OracleDataReader drBSNoToEdit = cmd.ExecuteReader();
                                if (drBSNoToEdit.HasRows)
                                {
                                    drBSNoToEdit.Read();
                                    BondInfo.BSNoToEdit = drBSNoToEdit.GetInt32(0);
                                }

                                cmd.CommandText = "select STATUS,BSDATE,SUBMITTEDBYNM,REMARKS from BONDSTATUS where BONDERSLNO=:BONDERSLNO";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                                System.Data.OracleClient.OracleDataReader drBondStatus = cmd.ExecuteReader();
                                if (drBondStatus.HasRows)
                                {
                                    drBondStatus.Read();
                                    ViewBag.STATUS = drBondStatus.GetValue(0);
                                    ViewBag.BSDATE = drBondStatus.GetValue(1);
                                    ViewBag.SUBMITTEDBYNM = drBondStatus.GetValue(2);
                                    ViewBag.REMARKS = drBondStatus.GetValue(3);
                                }

                                //         cmd.CommandText = "select DOCHEADINGNAME,RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM from REGISTRATIONATTACHMENT" +
                                //" where BONDERSLNO=:BONDERSLNO";
                                cmd.CommandText = "select DOCHEADINGNAME,RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM from DOCUMENTATTACHMENT" +
                      " where BONDERSLNO=:BONDERSLNO and BSNO=:BSNO";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                                cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                                System.Data.OracleClient.OracleDataReader drBondDocInfo = cmd.ExecuteReader();
                                if (drBondDocInfo.HasRows)
                                {
                                    string docHeadName;
                                    while (drBondDocInfo.Read())
                                    {
                                        docHeadName = drBondDocInfo.GetValue(0).ToString();
                                        for (int i = 0; i < drBondDocInfo.FieldCount; i++)
                                        {
                                            fieldName = docHeadName + drBondDocInfo.GetName(i);
                                            if (!ViewData.Keys.Contains(fieldName))
                                                ViewData.Add(fieldName, drBondDocInfo.GetValue(i).ToString());
                                        }
                                    }
                                }

                                #region old_select_Enroll_info
                                //cmd.CommandText = "select OFFICESLNO,MEMBERSHIPREGNO,ISSUEDATE,EXPDATE,CERATTACHFILENM,CERATTACHFILEPATH," +
                                //    "RECATTACHFILENM,RECATTACHFILEPATH from ASSOCIATIONENROLL where " +
                                //   "BONDERSLNO=:BONDERSLNO";
                                //cmd.Parameters.Clear();
                                //cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                                //System.Data.OracleClient.OracleDataReader drBondAssocEnroll = cmd.ExecuteReader();
                                //while (drBondAssocEnroll.Read())
                                //{
                                //    if (drBondAssocEnroll.HasRows)
                                //    {
                                //        if (drBondAssocEnroll.GetValue(0) != DBNull.Value)
                                //            BondInfo.enrollmentOfficeSlNo = drBondAssocEnroll.GetInt32(0);
                                //        for (int i = 0; i < drBondAssocEnroll.FieldCount; i++)
                                //        {
                                //            fieldName = "";
                                //            fieldName = drBondAssocEnroll.GetName(i);
                                //            ViewData.Add("ASSOCIATIONENROLL_" + fieldName, drBondAssocEnroll.GetValue(i).ToString());
                                //        }
                                //    }
                                //}
                                #endregion

                                #region old_select_Lien_Bank_Info
                                //cmd.CommandText = "select LIENBANKCODE,BANKSLNO,BBRANCHSLNO,CERATTACHFILENM,Address,Phone,Fax from BONDERLIENBANK" +
                                //    " where " +
                                //    "BONDERSLNO=:BONDERSLNO";
                                //cmd.Parameters.Clear();
                                //cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                                //System.Data.OracleClient.OracleDataReader drBondLienBank = cmd.ExecuteReader();
                                //while (drBondLienBank.Read())
                                //{
                                //    if (drBondLienBank.HasRows)
                                //    {
                                //        ViewBag.BONDERLIENBANK_LIENBANKCODE = drBondLienBank.GetValue(0).ToString();
                                //        if (drBondLienBank.GetValue(1) != DBNull.Value)
                                //            BondInfo.lienBankSlNo = drBondLienBank.GetInt32(1);
                                //        if (drBondLienBank.GetValue(2) != DBNull.Value)
                                //            BondInfo.lienBranchSlNo = drBondLienBank.GetInt32(2);
                                //        for (int i = 1; i < drBondLienBank.FieldCount; i++)
                                //        {
                                //            fieldName = "";
                                //            fieldName = drBondLienBank.GetName(i);
                                //            ViewData.Add("BONDERLIENBANK_" + fieldName, drBondLienBank.GetValue(i).ToString());
                                //        }
                                //    }
                                //}
                                #endregion

                                cmd.CommandText = "select OWNERCATEGORY from BONDER where " +
                                    "BONDERSLNO=:BONDERSLNO";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                                System.Data.OracleClient.OracleDataReader drOWNERCATEGORY = cmd.ExecuteReader();
                                while (drOWNERCATEGORY.Read())
                                {
                                    if (drOWNERCATEGORY.HasRows)
                                    {
                                        ViewData.Add("OWNERCATEGORY", drOWNERCATEGORY.GetValue(0).ToString());
                                    }
                                }

                                #region select_ownerInfo
                                //cmd.CommandText = "select DESSLNO,OWNERNAME,OWNERFHNAME,OWNERPERADDRESS,OWNERPRSADDRESS,OWNERCONTACTNO,OWNERRESPHONE" +
                                //",OWNERMOBPHONE,TINNO,TINISSDT,PASSPORTNO,PASSDISTRICTSLNO,PASSISSUEDT,PASSEXPDT,NATIONALITY,NATIONALZIDNO," +
                                //"SIGNATUREFILENM,PHOTOFILENM from OWNERINFO where BONDERSLNO=:BONDERSLNO";
                                //cmd.Parameters.Clear();
                                //cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                                //System.Data.OracleClient.OracleDataReader drOWNERINFO = cmd.ExecuteReader();
                                //while (drOWNERINFO.Read())
                                //{
                                //    if (drOWNERINFO.HasRows)
                                //    {
                                //        if (drOWNERINFO.GetValue(11) != DBNull.Value)
                                //            BondInfo.ownerPassportDistrictSlNo = drOWNERINFO.GetInt32(11);
                                //        if (drOWNERINFO.GetValue(0) != DBNull.Value)
                                //            BondInfo.ownerPositionDesignationSlNo = drOWNERINFO.GetInt32(0);
                                //        for (int i = 1; i < drOWNERINFO.FieldCount; i++)
                                //        {
                                //            fieldName = "";
                                //            fieldName = drOWNERINFO.GetName(i);
                                //            ViewData.Add("OWNERINFO_" + fieldName, drOWNERINFO.GetValue(i).ToString());
                                //        }
                                //    }
                                //}
                                #endregion
                            }
                            //DISTRICT district = db.DISTRICTs.Find(Convert.ToInt32(drBondEditInfo.GetValue(7)));
                            //if(BondInfo.bondDistrictSlNo!=null)
                            ViewBag.Upazila = new SelectList(db.UPAZILAs, "UPAZILASLNO", "UPAZILANAME", BondInfo.bondUpazilaSlNo);
                            ViewBag.District = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME", BondInfo.bondDistrictSlNo);
                            ViewBag.HoUpazila = new SelectList(db.UPAZILAs, "UPAZILASLNO", "UPAZILANAME", BondInfo.bondHoUpazilaSlNo);
                            ViewBag.HoDistrict = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME", BondInfo.bondHoDistrictSlNo);
                            ViewBag.BondType = new SelectList(db.BONDTYPEs, "BTYPESLNO", "BTYPENAME", BondInfo.bondTypeSlNo);
                            ViewBag.BondCategory = new SelectList(db.BONDCATEGORies, "BCATSLNO", "BCATNAME", BondInfo.bondCategorySlNo);
                            ViewBag.BondSubCategory = new SelectList(db.BONDSUBCATEGORies, "BSCATSLNO", "BSCATNAME", BondInfo.bondSubCategorySlNo);
                            ViewBag.Office = new SelectList(db.OFFICEs, "OFFICESLNO", "OFFICENAME", BondInfo.enrollmentOfficeSlNo);
                            ViewBag.Bank = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME", BondInfo.lienBankSlNo);
                            ViewBag.Branch = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME", BondInfo.lienBranchSlNo);
                            ViewBag.OwnerPositionDesignation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME", BondInfo.ownerPositionDesignationSlNo);
                            ViewBag.OwnerPassportDistrict = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME", BondInfo.ownerPassportDistrictSlNo);
                            ViewBag.fpHsCode = new SelectList(db.MATERIALS, "MSLNO", "MHSCODE");
                            ViewBag.PWM_HS_Code = new SelectList(db.MATERIALS, "MHSCODE", "MHSCODE");
                            ViewBag.fpDesc = new SelectList(db.MATERIALS, "MSLNO", "MDESCRIPTION");
                            //else
                            //    ViewBag.District = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");
                            ViewBag.IMI_Country_of_Origin = new SelectList(db.COUNTRies, "COUNTRYSLNO", "COUNTRYNAME");
                            ViewBag.IMI_Brand_Name = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "BRAND_NAME");
                            ViewBag.IMI_Model_No = new SelectList(db.MODELs, "MODELSLNO", "MODELNM");
                            ViewBag.PWM_Model_No = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM");
                            ViewBag.PWM_Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
                            ViewBag.ABI_Designation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME");
                            ViewBag.MPO_Bank = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME");
                            ViewBag.MPO_Branch = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME");
                            ViewBag.AIL_HS_Code = new SelectList(db.MATERIALS, "MHSCODE", "MHSCODE");
                            ViewBag.AIL_Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
                            ViewBag.Declare_Designation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME", BondInfo.declareDesSlNo);
                            ViewBag.BondCircle = new SelectList(db.BONDCIRCLEs, "BONDCIRCLESLNO", "BONDCIRCLENAME");
                            viewToReturn = "Edit";
                        }

                        conn.Close();
                        conn.Dispose();

                        RegistrationViewModel registrationviewmodel = new RegistrationViewModel();
                        try
                        {
                            registrationviewmodel.associationenroll = db.ASSOCIATIONENROLLs.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            registrationviewmodel.bonderlienbank = db.BONDERLIENBANKs.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            registrationviewmodel.installedmachineinfo = db.INSTALLEDMACHINEINFOes.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            registrationviewmodel.documentattachment = db.DOCUMENTATTACHMENTs.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            registrationviewmodel.ownerinfo = db.OWNERINFOes.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            registrationviewmodel.bonderproduct = db.BONDERPRODUCTs.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            registrationviewmodel.prodcapacitymachinewisem = db.PRODCAPACITYMACHINEWISEMs.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            registrationviewmodel.annualentlrawmaterial = db.ANNUALENTLRAWMATERIALs.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            //registrationviewmodel.assocOwnerInfo = from assocOwner in db.OWNERINFOes join assoc in db.OWNERASSOCIATEBUSINESSes on assocOwner.OWNERSLNO equals assoc.OWNERSLNO;
                            //registrationviewmodel.assocOwnerInfo = db.OWNERINFOes.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit && y.OWNERSLNO in (from assoc in db.OWNERASSOCIATEBUSINESSes select new {assoc.OWNERSLNO}).ToList();
                            //var assoc = (from pd in db.OWNERINFOes
                            //                                        join od in db.OWNERASSOCIATEBUSINESSes on pd.OWNERSLNO equals od.OWNERSLNO into t
                            //                                        from rt in t.DefaultIfEmpty()
                            //                                        select new
                            //                                        {
                            //                                            pd.OWNERSLNO,
                            //                                            pd.BONDERSLNO,
                            //                                            pd.DESSLNO,
                            //                                            pd.OWNERNAME,
                            //                                            pd.OWNERFHNAME,
                            //                                        }).ToList();
                            //registrationviewmodel.assocOwnerInfo = assoc.ToList<string>();
                            registrationviewmodel.ownerassociatebusiness = db.OWNERASSOCIATEBUSINESSes.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                        }
                        catch { }
                        List<RegistrationViewModel> viewModelList = new List<RegistrationViewModel>();

                        viewModelList.Add(registrationviewmodel);

                        return View("Edit", viewModelList);

                        //return View(viewToReturn);

                    }
                    catch
                    {
                        return View();
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
                else
                {
                    return View();
                    //Response.Write("name required");                
                }
            }
            catch
            {
                return View();
            }
        }



        public ActionResult ApplicantsList()
        {
            RegistrationViewModel registrationviewmodel = new RegistrationViewModel();

            if (!(System.Web.HttpContext.Current.User.IsInRole("Bonder")))
            {
                registrationviewmodel.bonder = db.BONDERs.Where(y => y.BONDSTATUS == "A").ToList();
                List<RegistrationViewModel> viewModelList = new List<RegistrationViewModel>();
                viewModelList.Add(registrationviewmodel);
                return View(viewModelList);
            }
            else
            {
                USERPERMISSION permission = session.getStoredUserPermission();
                var bonderName = permission.BONDER.BONDERNAME;
                registrationviewmodel.bonder = db.BONDERs.Where(y => y.BONDSTATUS == "A" && y.BONDERNAME == bonderName).ToList();
                List<RegistrationViewModel> viewModelList = new List<RegistrationViewModel>();
                viewModelList.Add(registrationviewmodel);
                return View(viewModelList);
            }

            //return View();
        }

        public ActionResult BondersList()
        {
            using (OracleEntitiesConnStr db = new OracleEntitiesConnStr())
            {

                RegistrationViewModel registrationviewmodel = new RegistrationViewModel();

                if (!(System.Web.HttpContext.Current.User.IsInRole("Bonder")))   // added By Mizan (17 Aug 2016)--
                {
                    registrationviewmodel.bonder = db.BONDERs.Where(y => y.BONDSTATUS == "R" || y.BONDSTATUS == "RR").ToList();
                    List<RegistrationViewModel> viewModelList = new List<RegistrationViewModel>();

                    viewModelList.Add(registrationviewmodel);
                    return View(viewModelList);
                }
                else                                                        // added By Mizan (17 Aug 2016)--
                {
                    USERPERMISSION permission = session.getStoredUserPermission();
                    var bonderName = permission.BONDER.BONDERNAME;
                    registrationviewmodel.bonder = db.BONDERs.Where(y => (y.BONDSTATUS == "R" || y.BONDSTATUS == "RR") && y.BONDERNAME == bonderName).ToList();
                    List<RegistrationViewModel> viewModelList = new List<RegistrationViewModel>();

                    viewModelList.Add(registrationviewmodel);
                    return View(viewModelList);
                }

                //List<BONDER> viewModelList = db.BONDERs.Where(y => y.BONDSTATUS == "R" || y.BONDSTATUS == "RR").ToList();

                ViewBag.District = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");


                //return View();
            }
        }
        
        // -------------------------added By Mizan (19 Oct 2016)---------------------------
        public ActionResult RenewalList()
        {
            using (OracleEntitiesConnStr db =new OracleEntitiesConnStr ())
            {
                RegistrationViewModel registrationviewmodel = new RegistrationViewModel();
                DateTime dtCurrent = DateTime.Now ;

                if (!(System.Web.HttpContext.Current.User.IsInRole("Bonder")))   
                {
                   
                    registrationviewmodel.bondregistration  = db.BONDREGISTRATIONs  .Where(b => EntityFunctions.TruncateTime(b.EXPIRYDATE) < EntityFunctions.TruncateTime(dtCurrent)).ToList ();
                    List<RegistrationViewModel> viewModelList = new List<RegistrationViewModel>();

                    viewModelList.Add(registrationviewmodel);
                    return View(viewModelList);
                }
                else                                                        
                {
                    USERPERMISSION permission = session.getStoredUserPermission();
                    var bonderName = permission.BONDER.BONDERNAME;
                    registrationviewmodel.bondregistration = db.BONDREGISTRATIONs.Where(b => (EntityFunctions.TruncateTime(b.EXPIRYDATE) < EntityFunctions.TruncateTime(dtCurrent)) && b.BONDER.BONDERNAME == bonderName).ToList();
                    List<RegistrationViewModel> viewModelList = new List<RegistrationViewModel>();

                    viewModelList.Add(registrationviewmodel);
                    return View(viewModelList);
                }
               
                ViewBag.District = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");
            }
            
        }

        // -------------------------added By Mizan (19 Oct 2016)---------------------------
        public static List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();

                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(dr[prop.Name], DBNull.Value))
                    {
                        try
                        {
                            prop.SetValue(obj, dr[prop.Name]);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "Object of type 'System.Decimal' cannot be converted to type 'System.Int16'.")
                            {
                                prop.SetValue(obj, Convert.ToInt16(dr[prop.Name]));
                            }
                        }
                    }
                }
                list.Add(obj);
            }
            return list;
        }
        public JsonResult ApplicantsListToFilter()
        {
            using (OracleEntitiesConnStr db = new OracleEntitiesConnStr())
            {

                if (!(System.Web.HttpContext.Current.User.IsInRole("Bonder")))
                {
                    var result = db.BONDERs.Where(y => y.BONDSTATUS == "A")
                    .Select(a => new
                    {
                        Name = a.BONDERNAME,
                        Address = a.ADDRESS,
                        Type = a.BONDTYPE.BTYPENAME,
                        bonderSlNo = a.BONDERSLNO
                    }).ToList();
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else                                                                // added By Mizan (17 Aug 2016)--
                {
                    USERPERMISSION permission = session.getStoredUserPermission();  // added By Mizan (17 Aug 2016)--
                    var bonderName = permission.BONDER.BONDERNAME;                  // added By Mizan (17 Aug 2016)--

                    var result = db.BONDERs.Where(y => y.BONDSTATUS == "A" && y.BONDERNAME == bonderName)
                        .Select(a => new
                        {
                            Name = a.BONDERNAME,
                            Address = a.ADDRESS,
                            Type = a.BONDTYPE.BTYPENAME,
                            bonderSlNo = a.BONDERSLNO
                        }).ToList();
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
        }

        public JsonResult BondersListToFilter()
        {
            #region oldBondersListToFilter
            //    var result = db.BONDERs.Where(y => (y.BONDSTATUS == "R" || y.BONDSTATUS == "RR")
            //|| (y.ASSOCIATIONENROLLs.Any(e => e.BONDERSLNO == y.BONDERSLNO) && (y.BONDSTATUS == "R" || y.BONDSTATUS == "RR")))
            //        .Select(n => new
            //        {
            //            Address = n.ADDRESS,
            //            Name = n.BONDERNAME,
            //            Type = n.BONDTYPE.BTYPENAME,
            //            BondLicenseNo = n.BONDLICENSENO,
            //            bonderSlNo = n.BONDERSLNO
            //        }).ToList();    

            //var result = from b in db.BONDERs
            //             join c in db.ASSOCIATIONENROLLs on b.BONDERSLNO equals c.BONDERSLNO into bc
            //             from c in bc.DefaultIfEmpty()
            //             where b.BONDSTATUS == "R" || b.BONDSTATUS == "RR"
            //             select new
            //             {
            //                 Name = b.BONDERNAME,
            //                 Address = b.ADDRESS,
            //                 Type = b.BONDTYPE.BTYPENAME,
            //                 BondLicenseNo = b.BONDLICENSENO,
            //                 bonderSlNo = b.BONDERSLNO,
            //                 BGAPMEALicenseNo = c.MEMBERSHIPREGNO,
            //                 District = b.BDISTRICTSLNO
            //             };
            #endregion

            using (OracleEntitiesConnStr db = new OracleEntitiesConnStr())
            {

                if (!(System.Web.HttpContext.Current.User.IsInRole("Bonder")))
                {
                    var result = (from b in db.BONDERs
                                  join c in db.ASSOCIATIONENROLLs on b.BONDERSLNO equals c.BONDERSLNO into bc
                                  from c in bc.DefaultIfEmpty()
                                  join d in db.BONDERANNUALENTITLEMENTs on b.BONDERSLNO equals d.BONDERSLNO into cd
                                  from d in cd.DefaultIfEmpty()
                                  where b.BONDSTATUS == "R" || b.BONDSTATUS == "RR"
                                  select new
                                  {
                                      Name = b.BONDERNAME,
                                      Address = b.ADDRESS,
                                      Type = b.BONDTYPE.BTYPENAME,
                                      BondLicenseNo = b.BONDLICENSENO,
                                      bonderSlNo = b.BONDERSLNO,
                                      BGAPMEALicenseNo = c.MEMBERSHIPREGNO,
                                      District = b.BDISTRICTSLNO,
                                      EntitlementSlNo = (from e in cd select (int?)e.AESLNO).Max()
                                  }).Distinct().ToList();

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else                                                                // added By Mizan (17 Aug 2016)--
                {

                    USERPERMISSION permission = session.getStoredUserPermission();  // added By Mizan (17 Aug 2016)--
                    var bonderName = permission.BONDER.BONDERNAME;                  // added By Mizan (17 Aug 2016)--

                    var result = (from b in db.BONDERs
                                  join c in db.ASSOCIATIONENROLLs on b.BONDERSLNO equals c.BONDERSLNO into bc
                                  from c in bc.DefaultIfEmpty()
                                  join d in db.BONDERANNUALENTITLEMENTs on b.BONDERSLNO equals d.BONDERSLNO into cd
                                  from d in cd.DefaultIfEmpty()
                                  where (b.BONDSTATUS == "R" || b.BONDSTATUS == "RR") && b.BONDERNAME == bonderName
                                  select new
                                  {
                                      Name = b.BONDERNAME,
                                      Address = b.ADDRESS,
                                      Type = b.BONDTYPE.BTYPENAME,
                                      BondLicenseNo = b.BONDLICENSENO,
                                      bonderSlNo = b.BONDERSLNO,
                                      BGAPMEALicenseNo = c.MEMBERSHIPREGNO,
                                      District = b.BDISTRICTSLNO,
                                      EntitlementSlNo = (from e in cd select (int?)e.AESLNO).Max()
                                  }).Distinct().ToList();

                    return Json(result, JsonRequestBehavior.AllowGet);
                }


                //ViewBag.District = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");

            }
        }


        // -------------------------added By Mizan (19 Oct 2016)---------------------------
        public JsonResult RenewalListToFilter()
        {
            using (OracleEntitiesConnStr db = new OracleEntitiesConnStr())
            {

                if (!(System.Web.HttpContext.Current.User.IsInRole("Bonder")))
                {
                    var result = (from b in db.BONDERs
                                  join f in db.BONDREGISTRATIONs on b.BONDERSLNO equals f.BONDERSLNO 
                                  join c in db.ASSOCIATIONENROLLs on b.BONDERSLNO equals c.BONDERSLNO into bc
                                  from c in bc.DefaultIfEmpty()
                                  join d in db.BONDERANNUALENTITLEMENTs on b.BONDERSLNO equals d.BONDERSLNO into cd
                                  from d in cd.DefaultIfEmpty()
                                  where f.EXPIRYDATE  < DateTime .Now 
                                  select new
                                  {
                                      Name = b.BONDERNAME,
                                      Address = b.ADDRESS,
                                      Type = b.BONDTYPE.BTYPENAME,
                                      BondLicenseNo = b.BONDLICENSENO,
                                      bonderSlNo = b.BONDERSLNO,
                                      BGAPMEALicenseNo = c.MEMBERSHIPREGNO,
                                      District = b.BDISTRICTSLNO,
                                      EntitlementSlNo = (from e in cd select (int?)e.AESLNO).Max()
                                  }).Distinct().ToList();

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else                                                                
                {

                    USERPERMISSION permission = session.getStoredUserPermission();  
                    var bonderName = permission.BONDER.BONDERNAME;                  

                    var result = (from b in db.BONDERs
                                  join f in db.BONDREGISTRATIONs on b.BONDERSLNO equals f.BONDERSLNO 
                                  join c in db.ASSOCIATIONENROLLs on b.BONDERSLNO equals c.BONDERSLNO into bc
                                  from c in bc.DefaultIfEmpty()
                                  join d in db.BONDERANNUALENTITLEMENTs on b.BONDERSLNO equals d.BONDERSLNO into cd
                                  from d in cd.DefaultIfEmpty()
                                  where f.EXPIRYDATE < DateTime.Now && b.BONDERNAME == bonderName
                                  select new
                                  {
                                      Name = b.BONDERNAME,
                                      Address = b.ADDRESS,
                                      Type = b.BONDTYPE.BTYPENAME,
                                      BondLicenseNo = b.BONDLICENSENO,
                                      bonderSlNo = b.BONDERSLNO,
                                      BGAPMEALicenseNo = c.MEMBERSHIPREGNO,
                                      District = b.BDISTRICTSLNO,
                                      EntitlementSlNo = (from e in cd select (int?)e.AESLNO).Max()
                                  }).Distinct().ToList();

                    return Json(result, JsonRequestBehavior.AllowGet);
                }


                //ViewBag.District = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");

            }
        }

        // -------------------------added By Mizan (19 Oct 2016)---------------------------
        public JsonResult getDistricts()
        {
            var result = db.DISTRICTs.Select(d => new
            {
                id = d.DISTRICTSLNO,
                name = d.DISTRICTNAME
            }
            ).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApproveBond(int bonderSlNo, bool renewApproval = false)
        {
            if (renewApproval == true)
                CommonAppSet.BondInfo.renewApproval = true;
            else
                CommonAppSet.BondInfo.renewApproval = false;
            RegistrationViewModel registrationviewmodel = new RegistrationViewModel();
            registrationviewmodel.bonder = db.BONDERs.Where(y => y.BONDERSLNO == bonderSlNo).ToList();
            registrationviewmodel.bondregistration = db.BONDREGISTRATIONs.Where(y => y.BONDERSLNO == bonderSlNo).ToList();
            List<RegistrationViewModel> viewModelList = new List<RegistrationViewModel>();

            viewModelList.Add(registrationviewmodel);
            if (registrationviewmodel.bondregistration.Count > 0)
            {
                if (!string.IsNullOrEmpty(registrationviewmodel.bondregistration.FirstOrDefault().BONDCIRCLESLNO.ToString()))
                    ViewBag.BondCircle = new SelectList(db.BONDCIRCLEs, "BONDCIRCLESLNO", "BONDCIRCLENAME", registrationviewmodel.bondregistration.FirstOrDefault().BONDCIRCLESLNO);
                else
                    ViewBag.BondCircle = new SelectList(db.BONDCIRCLEs, "BONDCIRCLESLNO", "BONDCIRCLENAME");
            }
            else
                ViewBag.BondCircle = new SelectList(db.BONDCIRCLEs, "BONDCIRCLESLNO", "BONDCIRCLENAME");
            return View(viewModelList);
        }

        public ActionResult EditBond(string bondName)
        {
            BondInfo.bondNameToEdit = bondName;
            FormCollection colleciton = new FormCollection();
            return Start(colleciton);
        }
       
        //---------------------------Added By Mizan (23-Oct-16)---------------------------------------------------------------//
        public ActionResult ApproveBondForBonder(int bonderSlNo, bool renewApproval = false)
        {
            if (renewApproval == true)
                CommonAppSet.BondInfo.renewApproval = true;
            else
                CommonAppSet.BondInfo.renewApproval = false;
            RegistrationViewModel registrationviewmodel = new RegistrationViewModel();
            registrationviewmodel.bonder = db.BONDERs.Where(y => y.BONDERSLNO == bonderSlNo).ToList();
            registrationviewmodel.bondregistration = db.BONDREGISTRATIONs.Where(y => y.BONDERSLNO == bonderSlNo).ToList();
            List<RegistrationViewModel> viewModelList = new List<RegistrationViewModel>();

            viewModelList.Add(registrationviewmodel);
            if (registrationviewmodel.bondregistration.Count > 0)
            {
                if (!string.IsNullOrEmpty(registrationviewmodel.bondregistration.FirstOrDefault().BONDCIRCLESLNO.ToString()))
                    ViewBag.BondCircle = new SelectList(db.BONDCIRCLEs, "BONDCIRCLESLNO", "BONDCIRCLENAME", registrationviewmodel.bondregistration.FirstOrDefault().BONDCIRCLESLNO);
                else
                    ViewBag.BondCircle = new SelectList(db.BONDCIRCLEs, "BONDCIRCLESLNO", "BONDCIRCLENAME");
            }
            else
                ViewBag.BondCircle = new SelectList(db.BONDCIRCLEs, "BONDCIRCLESLNO", "BONDCIRCLENAME");
            return View(viewModelList);
        }

        public ActionResult ViewEditBond(string bondName)
        {
            BondInfo.bondNameToEdit = bondName;
            FormCollection colleciton = new FormCollection();
            return ViewStart(colleciton);
        }

        public ActionResult ViewStart(FormCollection colleciton)
        {

            String viewToReturn = "";
            //if (BondInfo.bondNameToEdit == "")
            //{
            if (!string.IsNullOrEmpty(Request["ManufacturingUnitName"]))
            {
                try
                {
                    BondInfo.bondNameToEdit = Request["ManufacturingUnitName"].Trim();
                }
                catch
                {
                    return View();
                }
            }
            //}
            try
            {
                if (!String.IsNullOrEmpty(BondInfo.bondNameToEdit))
                {
                    System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();
                    System.Data.OracleClient.OracleConnection conn = ConnectBMS.Connection();
                    System.Data.OracleClient.OracleTransaction bmsTransaction;

                    try
                    {
                        cmd.Connection = conn;
                        //cmd.Connection = ConnectBMS.Connection();
                        bmsTransaction = conn.BeginTransaction();
                        cmd.Transaction = bmsTransaction;


                        System.Data.OracleClient.OracleDataReader drBondEditInfo;
                        System.Data.OracleClient.OracleDataReader drNewBondInfo;

                        cmd.CommandText = "select BONDERSLNO,HOLDINGNO,ROADNO,AREAVILLAGE,MOWJA," +
                                    "UNIONNAME,BUPAZILASLNO,BDISTRICTSLNO,PSTATION,WARD,CITYPOURO,PHONE,FAX,MOBILE,EMAIL," +
            "HOHOLDINGNO,HOROADNO,HOAREAVILLAGE,HOMOWJA,HOUNION,HOUPAZILASLNO,HODISTRICTSLNO,HOPSTATION,HOWARD," +
            "HOCITYPOURO,HOPHONE,HOFAX,HOMOBILE,HOEMAIL, PREMESISSTATUS,HIREDATE,HIREEXPDATE,OWNERSHIPDATE,OWNERSHIPDOLILNO," +
            "DolilFileNM,DEEDFILENM,RENTVATCHALLANNO," +
            "VCHALNFILENM,CSDAGNO,RSDAGNO,BONDFACPREV,BONDLICENSENO,BTYPESLNO,BCATSLNO,BSCATSLNO,INPUTBY,INPUTDATE,TOTFACLENG," +
            "TOTFACWIDTH,TOTRMWHLENG,TOTRMWHWIDTH,TOTFGWHLENG,TOTFGWHWIDTH,AUTHORIZEDSIGNAME,INITIALAUTHSIGFILENM,PHOTOFILENM,"
                        + "DECLARNMBLOCK,DECLARSIGFILENM,DECLARSEALFILENM,DECLARDESSLNO,DECLARDATE,BONDSTATUS,ADDRESS,COADDRESS"
                        + " from BONDER where BONDERNAME=:BONDERNAME";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("BONDERNAME", BondInfo.bondNameToEdit);
                        drBondEditInfo = cmd.ExecuteReader();

                        //drBondEditInfo.Read();                    
                        ViewBag.ManufacturingUnitName = BondInfo.bondNameToEdit;


                        if (!drBondEditInfo.HasRows)
                        {
                            cmd.CommandText = "insert into BONDER(BONDSTATUS,BONDERNAME) values(:BONDSTATUS,:BONDERNAME)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("BONDSTATUS", "A");
                            cmd.Parameters.AddWithValue("BONDERNAME", BondInfo.bondNameToEdit);

                            try
                            {
                                cmd.ExecuteNonQuery();
                                bmsTransaction.Commit();
                            }
                            catch
                            {
                                bmsTransaction.Rollback();
                                return View();
                            }

                            cmd.CommandText = "select max(BONDERSLNO) from Bonder";
                            cmd.Parameters.Clear();
                            drNewBondInfo = cmd.ExecuteReader();
                            if (drNewBondInfo.Read())
                            {
                                BondInfo.newBondSlNo = drNewBondInfo.GetInt32(0);
                                BondInfo.bondSlNoToEdit = drNewBondInfo.GetInt32(0);
                            }
                            cmd.CommandText = "insert into BONDSTATUS(BONDERSLNO,STATUS) values(:BONDERSLNO,:STATUS)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.newBondSlNo);
                            cmd.Parameters.AddWithValue("STATUS", "N");
                            bmsTransaction = conn.BeginTransaction();
                            cmd.Transaction = bmsTransaction;
                            try
                            {
                                cmd.ExecuteNonQuery();
                                bmsTransaction.Commit();
                            }
                            catch
                            {
                                bmsTransaction.Rollback();
                                return View();
                            }

                            cmd.CommandText = "select max(BSNO) from BONDSTATUS";
                            cmd.Parameters.Clear();
                            System.Data.OracleClient.OracleDataReader drNewBSNo = cmd.ExecuteReader();
                            if (drNewBSNo.HasRows)
                            {
                                drNewBSNo.Read();
                                BondInfo.newBSNO = drNewBSNo.GetInt32(0);
                            }

                            cmd.CommandText = "insert into BONDAPPLICATIONPROGRESS(BONDERSLNO,BSNO,READYFORAPP) values(:BONDERSLNO,:BSNO,:READYFORAPP)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.newBondSlNo);
                            cmd.Parameters.AddWithValue("BSNO", BondInfo.newBSNO);
                            cmd.Parameters.AddWithValue("READYFORAPP", "N");
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

                            ViewBag.Upazila = new SelectList(db.UPAZILAs, "UPAZILASLNO", "UPAZILANAME");
                            ViewBag.District = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");
                            ViewBag.HoUpazila = new SelectList(db.UPAZILAs, "UPAZILASLNO", "UPAZILANAME");
                            ViewBag.HoDistrict = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");
                            ViewBag.BondType = new SelectList(db.BONDTYPEs, "BTYPESLNO", "BTYPENAME");
                            ViewBag.BondCategory = new SelectList(db.BONDCATEGORies, "BCATSLNO", "BCATNAME");
                            ViewBag.BondSubCategory = new SelectList(db.BONDSUBCATEGORies, "BSCATSLNO", "BSCATNAME");
                            ViewBag.Office = new SelectList(db.OFFICEs, "OFFICESLNO", "OFFICENAME");
                            ViewBag.Bank = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME");
                            ViewBag.Branch = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME");
                            ViewBag.OwnerPositionDesignation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME");
                            ViewBag.OwnerPassportDistrict = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");
                            ViewBag.fpHsCode = new SelectList(db.MATERIALS, "MSLNO", "MHSCODE");
                            ViewBag.PWM_HS_Code = new SelectList(db.MATERIALS, "MHSCODE", "MHSCODE");
                            ViewBag.fpDesc = new SelectList(db.MATERIALS, "MDESCRIPTION", "MDESCRIPTION");
                            ViewBag.IMI_Country_of_Origin = new SelectList(db.COUNTRies, "COUNTRYSLNO", "COUNTRYNAME");
                            ViewBag.IMI_Brand_Name = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "BRAND_NAME");
                            ViewBag.IMI_Model_No = new SelectList(db.MODELs, "MODELSLNO", "MODELNM");
                            ViewBag.PWM_Model_No = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM");
                            ViewBag.PWM_Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
                            ViewBag.ABI_Designation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME");
                            ViewBag.MPO_Bank = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME");
                            ViewBag.MPO_Branch = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME");
                            ViewBag.AIL_HS_Code = new SelectList(db.MATERIALS, "MHSCODE", "MHSCODE");
                            ViewBag.AIL_Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
                            ViewBag.Declare_Designation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME");
                            ViewBag.BondCircle = new SelectList(db.BONDCIRCLEs, "BONDCIRCLESLNO", "BONDCIRCLENAME");
                            viewToReturn = "Create";
                        }
                        else
                        {
                            //ViewBag.BONDERSLNO = drBondEditInfo.GetValue(0).ToString();
                            while (drBondEditInfo.Read())
                            {
                                BondInfo.bondSlNoToEdit = drBondEditInfo.GetInt32(0);

                                if (drBondEditInfo["BONDSTATUS"] != DBNull.Value)
                                    BondInfo.bondStatus = drBondEditInfo["BONDSTATUS"].ToString();
                                if (drBondEditInfo.GetValue(6) != DBNull.Value)
                                    BondInfo.bondUpazilaSlNo = drBondEditInfo.GetInt32(6);
                                if (drBondEditInfo.GetValue(7) != DBNull.Value)
                                    BondInfo.bondDistrictSlNo = drBondEditInfo.GetInt32(7);
                                if (drBondEditInfo.GetValue(20) != DBNull.Value)
                                    BondInfo.bondHoUpazilaSlNo = drBondEditInfo.GetInt32(20);
                                if (drBondEditInfo.GetValue(21) != DBNull.Value)
                                    BondInfo.bondHoDistrictSlNo = drBondEditInfo.GetInt32(21);
                                if (drBondEditInfo.GetValue(42) != DBNull.Value)
                                    BondInfo.bondTypeSlNo = drBondEditInfo.GetInt32(42);
                                if (drBondEditInfo.GetValue(43) != DBNull.Value)
                                    BondInfo.bondCategorySlNo = drBondEditInfo.GetInt32(43);
                                if (drBondEditInfo.GetValue(44) != DBNull.Value)
                                    BondInfo.bondSubCategorySlNo = drBondEditInfo.GetInt32(44);
                                if (drBondEditInfo.GetValue(59) != DBNull.Value)
                                    BondInfo.declareDesSlNo = drBondEditInfo.GetInt32(59);
                                string fieldName;
                                for (int i = 0; i < drBondEditInfo.FieldCount; i++)
                                {
                                    fieldName = drBondEditInfo.GetName(i);
                                    ViewData.Add(fieldName, drBondEditInfo.GetValue(i).ToString());
                                }
                                cmd.CommandText = "select BSNO from BONDSTATUS where BONDERSLNO=:BONDERSLNO";
                                //cmd.CommandText = "select BSNO from BONDSTATUS where BONDERSLNO=:BONDERSLNO and STATUS=:STATUS";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                                //cmd.Parameters.AddWithValue("STATUS", "A");
                                System.Data.OracleClient.OracleDataReader drBSNoToEdit = cmd.ExecuteReader();
                                if (drBSNoToEdit.HasRows)
                                {
                                    drBSNoToEdit.Read();
                                    BondInfo.BSNoToEdit = drBSNoToEdit.GetInt32(0);
                                }

                                cmd.CommandText = "select STATUS,BSDATE,SUBMITTEDBYNM,REMARKS from BONDSTATUS where BONDERSLNO=:BONDERSLNO";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                                System.Data.OracleClient.OracleDataReader drBondStatus = cmd.ExecuteReader();
                                if (drBondStatus.HasRows)
                                {
                                    drBondStatus.Read();
                                    ViewBag.STATUS = drBondStatus.GetValue(0);
                                    ViewBag.BSDATE = drBondStatus.GetValue(1);
                                    ViewBag.SUBMITTEDBYNM = drBondStatus.GetValue(2);
                                    ViewBag.REMARKS = drBondStatus.GetValue(3);
                                }

                                cmd.CommandText = "select DOCHEADINGNAME,RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM from DOCUMENTATTACHMENT" +
                      " where BONDERSLNO=:BONDERSLNO and BSNO=:BSNO";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                                cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                                System.Data.OracleClient.OracleDataReader drBondDocInfo = cmd.ExecuteReader();
                                if (drBondDocInfo.HasRows)
                                {
                                    string docHeadName;
                                    while (drBondDocInfo.Read())
                                    {
                                        docHeadName = drBondDocInfo.GetValue(0).ToString();
                                        for (int i = 0; i < drBondDocInfo.FieldCount; i++)
                                        {
                                            fieldName = docHeadName + drBondDocInfo.GetName(i);
                                            if (!ViewData.Keys.Contains(fieldName))
                                                ViewData.Add(fieldName, drBondDocInfo.GetValue(i).ToString());
                                        }
                                    }
                                }

                              


                                cmd.CommandText = "select OWNERCATEGORY from BONDER where " +
                                    "BONDERSLNO=:BONDERSLNO";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                                System.Data.OracleClient.OracleDataReader drOWNERCATEGORY = cmd.ExecuteReader();
                                while (drOWNERCATEGORY.Read())
                                {
                                    if (drOWNERCATEGORY.HasRows)
                                    {
                                        ViewData.Add("OWNERCATEGORY", drOWNERCATEGORY.GetValue(0).ToString());
                                    }
                                }

               
                            }
                         
                            ViewBag.Upazila = new SelectList(db.UPAZILAs, "UPAZILASLNO", "UPAZILANAME", BondInfo.bondUpazilaSlNo);
                            ViewBag.District = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME", BondInfo.bondDistrictSlNo);
                            ViewBag.HoUpazila = new SelectList(db.UPAZILAs, "UPAZILASLNO", "UPAZILANAME", BondInfo.bondHoUpazilaSlNo);
                            ViewBag.HoDistrict = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME", BondInfo.bondHoDistrictSlNo);
                            ViewBag.BondType = new SelectList(db.BONDTYPEs, "BTYPESLNO", "BTYPENAME", BondInfo.bondTypeSlNo);
                            ViewBag.BondCategory = new SelectList(db.BONDCATEGORies, "BCATSLNO", "BCATNAME", BondInfo.bondCategorySlNo);
                            ViewBag.BondSubCategory = new SelectList(db.BONDSUBCATEGORies, "BSCATSLNO", "BSCATNAME", BondInfo.bondSubCategorySlNo);
                            ViewBag.Office = new SelectList(db.OFFICEs, "OFFICESLNO", "OFFICENAME", BondInfo.enrollmentOfficeSlNo);
                            ViewBag.Bank = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME", BondInfo.lienBankSlNo);
                            ViewBag.Branch = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME", BondInfo.lienBranchSlNo);
                            ViewBag.OwnerPositionDesignation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME", BondInfo.ownerPositionDesignationSlNo);
                            ViewBag.OwnerPassportDistrict = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME", BondInfo.ownerPassportDistrictSlNo);
                            ViewBag.fpHsCode = new SelectList(db.MATERIALS, "MSLNO", "MHSCODE");
                            ViewBag.PWM_HS_Code = new SelectList(db.MATERIALS, "MHSCODE", "MHSCODE");
                            ViewBag.fpDesc = new SelectList(db.MATERIALS, "MSLNO", "MDESCRIPTION");
                            
                            ViewBag.IMI_Country_of_Origin = new SelectList(db.COUNTRies, "COUNTRYSLNO", "COUNTRYNAME");
                            ViewBag.IMI_Brand_Name = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "BRAND_NAME");
                            ViewBag.IMI_Model_No = new SelectList(db.MODELs, "MODELSLNO", "MODELNM");
                            ViewBag.PWM_Model_No = new SelectList(db.MACHINEINFORMATIONs, "MACHINESLNO", "MODELNM");
                            ViewBag.PWM_Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
                            ViewBag.ABI_Designation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME");
                            ViewBag.MPO_Bank = new SelectList(db.BANKs, "BANKSLNO", "BANKNAME");
                            ViewBag.MPO_Branch = new SelectList(db.BANKBRANCHes, "BBRANCHSLNO", "BRANCHNAME");
                            ViewBag.AIL_HS_Code = new SelectList(db.MATERIALS, "MHSCODE", "MHSCODE");
                            ViewBag.AIL_Unit = new SelectList(db.MEASUREMENTUNITs, "MUSLNO", "MUNAME");
                            ViewBag.Declare_Designation = new SelectList(db.DESIGNATIONs, "DESSLNO", "DESNAME", BondInfo.declareDesSlNo);
                            ViewBag.BondCircle = new SelectList(db.BONDCIRCLEs, "BONDCIRCLESLNO", "BONDCIRCLENAME");
                            viewToReturn = "ViewEdit";
                        }

                        conn.Close();
                        conn.Dispose();

                        RegistrationViewModel registrationviewmodel = new RegistrationViewModel();
                        try
                        {
                            registrationviewmodel.associationenroll = db.ASSOCIATIONENROLLs.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            registrationviewmodel.bonderlienbank = db.BONDERLIENBANKs.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            registrationviewmodel.installedmachineinfo = db.INSTALLEDMACHINEINFOes.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            registrationviewmodel.documentattachment = db.DOCUMENTATTACHMENTs.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            registrationviewmodel.ownerinfo = db.OWNERINFOes.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            registrationviewmodel.bonderproduct = db.BONDERPRODUCTs.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            registrationviewmodel.prodcapacitymachinewisem = db.PRODCAPACITYMACHINEWISEMs.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                            registrationviewmodel.annualentlrawmaterial = db.ANNUALENTLRAWMATERIALs.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                           
                            registrationviewmodel.ownerassociatebusiness = db.OWNERASSOCIATEBUSINESSes.Where(y => y.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
                        }
                        catch { }
                        List<RegistrationViewModel> viewModelList = new List<RegistrationViewModel>();

                        viewModelList.Add(registrationviewmodel);

                        return View("ViewEdit", viewModelList);

                    }
                    catch
                    {
                        return View();
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
                else
                {
                    return View();
                                 
                }
            }
            catch
            {
                return View();
            }
        }

        //---------------------------Added By Mizan (23-Oct-16)---------------------------------------------------------------//

        [HttpPost]
        public ActionResult approveBondLicense(FormCollection collection)
        {
            try
            {
                int bonderSlNo = Convert.ToInt32(Request["bonderSlNo"]);
                System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();
                System.Data.OracleClient.OracleConnection conn = ConnectBMS.Connection();
                System.Data.OracleClient.OracleTransaction bmsTransaction;
                bmsTransaction = conn.BeginTransaction();
                cmd.Connection = conn;
                cmd.Transaction = bmsTransaction;
                cmd.CommandText = "update BONDSTATUS set STATUS=:STATUS where BonderSlNo=:BonderSlNo";
                cmd.Parameters.Clear();
                if (CommonAppSet.BondInfo.renewApproval == true)
                    cmd.Parameters.AddWithValue("STATUS", "RR");
                else
                    cmd.Parameters.AddWithValue("STATUS", "R");
                cmd.Parameters.AddWithValue("BonderSlNo", bonderSlNo);
                try
                {
                    try
                    {
                        bmsTransaction = conn.BeginTransaction();
                    }
                    catch { }
                    cmd.ExecuteNonQuery();
                    bmsTransaction.Commit();
                }
                catch (Exception ex)
                {
                    bmsTransaction.Rollback();
                }

                cmd.CommandText = "update Bonder set BONDSTATUS=:BONDSTATUS,BONDLICENSENO=:BONDLICENSENO where BonderSlNo=:BonderSlNo";
                cmd.Parameters.Clear();
                if (CommonAppSet.BondInfo.renewApproval == true)
                    cmd.Parameters.AddWithValue("BONDSTATUS", "RR");
                else
                    cmd.Parameters.AddWithValue("BONDSTATUS", "R");
                cmd.Parameters.AddWithValue("BONDLICENSENO", Request["bondLicenseNo"]);
                cmd.Parameters.AddWithValue("BonderSlNo", bonderSlNo);
                try
                {
                    try
                    {
                        bmsTransaction = conn.BeginTransaction();
                    }
                    catch { }
                    cmd.ExecuteNonQuery();
                    bmsTransaction.Commit();
                }
                catch (Exception ex)
                {
                    bmsTransaction.Rollback();
                }
                if (BMSPhase2Demo.CommonAppSet.BondInfo.updateApproval == false)
                    cmd.CommandText = "insert into BONDREGISTRATION(BONDERSLNO,BSNO,BONDLICENSENO,BTYPESLNO,BONDCIRCLESLNO,FEEAMT,"
                    + "ISSUEDATE,EXPIRYDATE,RENEWALDATE,autoRenewal) values(:BONDERSLNO,(select max(BSNO) from BONDSTATUS where BonderSlNo=:BONDERSLNO),:BONDLICENSENO,:BTYPESLNO,:BONDCIRCLESLNO,:FEEAMT,"
                    + ":ISSUEDATE,:EXPIRYDATE,:RENEWALDATE,:autoRenewal)";
                else if (BMSPhase2Demo.CommonAppSet.BondInfo.updateApproval == true)
                    cmd.CommandText = "update BONDREGISTRATION set BSNO=(select max(BSNO) from BONDSTATUS where BonderSlNo=:BONDERSLNO),BONDLICENSENO=:BONDLICENSENO,BTYPESLNO=:BTYPESLNO,BONDCIRCLESLNO=:BONDCIRCLESLNO,FEEAMT=:FEEAMT,"
                + "ISSUEDATE=:ISSUEDATE,EXPIRYDATE=:EXPIRYDATE,RENEWALDATE=:RENEWALDATE,autoRenewal=:autoRenewal"
                + " where BONDERSLNO=:BONDERSLNO";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("BONDERSLNO", bonderSlNo);
                cmd.Parameters.AddWithValue("BONDLICENSENO", Request["bondLicenseNo"]);
                cmd.Parameters.AddWithValue("BTYPESLNO", Request["BTYPESLNO"]);
                cmd.Parameters.AddWithValue("BONDCIRCLESLNO", Request["BondCircle"]);
                cmd.Parameters.AddWithValue("FEEAMT", Request["regFee"]);
                if (!String.IsNullOrEmpty(Request["issueDate"]))
                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = Request["issueDate"];
                else
                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;
                if (!String.IsNullOrEmpty(Request["expDate"]))
                    cmd.Parameters.Add(new OracleParameter(":EXPIRYDATE", OracleType.DateTime)).Value = Request["expDate"];
                else
                    cmd.Parameters.Add(new OracleParameter(":EXPIRYDATE", OracleType.DateTime)).Value = DBNull.Value;
                if (!String.IsNullOrEmpty(Request["autoRenewalDate"]))
                    cmd.Parameters.Add(new OracleParameter(":RENEWALDATE", OracleType.DateTime)).Value = Request["autoRenewalDate"];
                else
                    cmd.Parameters.Add(new OracleParameter(":RENEWALDATE", OracleType.DateTime)).Value = DBNull.Value;
                cmd.Parameters.AddWithValue("autoRenewal", Request["autoRenewal"]);
                try
                {
                    try
                    {
                        bmsTransaction = conn.BeginTransaction();
                    }
                    catch { }
                    cmd.ExecuteNonQuery();
                    bmsTransaction.Commit();
                }
                catch (Exception ex)
                {
                    bmsTransaction.Rollback();
                }
            }
            catch
            { }
            RegistrationViewModel registrationviewmodel = new RegistrationViewModel();
            if (BMSPhase2Demo.CommonAppSet.BondInfo.updateApproval == false)
                registrationviewmodel.bonder = db.BONDERs.Where(y => y.BONDSTATUS == "A").ToList();
            else if (BMSPhase2Demo.CommonAppSet.BondInfo.updateApproval == true)
                registrationviewmodel.bonder = db.BONDERs.Where(y => y.BONDSTATUS == "R").ToList();
            List<RegistrationViewModel> viewModelList = new List<RegistrationViewModel>();

            viewModelList.Add(registrationviewmodel);
            if (BMSPhase2Demo.CommonAppSet.BondInfo.updateApproval == false)
                return View("ApplicantsList", viewModelList);
            else
                return View("BondersList", viewModelList);
        }

        [HttpPost]
        public ActionResult Search(string BondLicenseNo = "", int distSlNo = 0, string BGAPMEALicenseNo = "")
        {
            RegistrationViewModel registrationviewmodel = new RegistrationViewModel();

            if (BondLicenseNo != "")
                registrationviewmodel.bonder = db.BONDERs.Where(y => (y.BONDSTATUS == "R" || y.BONDSTATUS == "RR") && (y.BONDLICENSENO == BondLicenseNo)).ToList();
            else if (BGAPMEALicenseNo != "")
                registrationviewmodel.bonder = db.BONDERs.Where(y => (y.BONDSTATUS == "R" || y.BONDSTATUS == "RR") && (y.ASSOCIATIONENROLLs.Any(e => e.BONDERSLNO == y.BONDERSLNO && e.MEMBERSHIPREGNO == BGAPMEALicenseNo))).ToList();
            else if (!String.IsNullOrEmpty(Request["District"]))
            {
                try
                {
                    distSlNo = Convert.ToInt32(Request["District"]);
                }
                catch
                { }
                registrationviewmodel.bonder = db.BONDERs.Where(y => (y.BONDSTATUS == "R" || y.BONDSTATUS == "RR") && (y.BDISTRICTSLNO == distSlNo)).ToList();
            }
            else
                registrationviewmodel.bonder = db.BONDERs.Where(y => y.BONDSTATUS == "R" || y.BONDSTATUS == "RR").ToList();
            List<RegistrationViewModel> viewModelList = new List<RegistrationViewModel>();

            viewModelList.Add(registrationviewmodel);
            ViewBag.District = new SelectList(db.DISTRICTs, "DISTRICTSLNO", "DISTRICTNAME");
            return View("BondersList", viewModelList);
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(string[] IMI_Description, string[] IMI_Brand_Name, string[] IMI_Model_No,
            string[] IMI_Manufacturing_Year, string[] IMI_Country_of_Origin, string[] IMI_Date_of_Installation,
            string[] IMI_BE_No_or_VAT_Challan_No, string[] IMI_VAT_Date, string[] IMI_BL_No,
            string[] IMI_BL_Date, string[] IMI_Invoice_No, string[] IMI_Invoice_Date,
            string[] IMI_LC_No, string[] IMI_LC_Date, string[] IMI_No_of_Machines,
            string[] IMI_Machine_Type, string[] IMI_Procurement_Type, string[] IMI_Indemnity_Bond,
            string[] PWM_HS_Code, string[] PWM_AnnualCapacity, string[] PWM_Unit, string[] PWM_Machine, string[] PWM_Qty,
            string[] PWM_Machines_Type, string[] PWM_Machines_Qty, string[] PWM_Model_No,
            string[] LBI_Code, string[] Branch,
            string[] Office, string[] EnrollmetnMEMBERSHIPREGNO, string[] EnrollmetnInputIssueDate,
            string[] EnrollmetnInputExpiryDate,
            string[] DocumentName, string[] DocumentNo, string[] DocumentIssueDate, string[] DocumentExpiryDate,
            string[] OwnerPositionDesignation, string[] OII_Name, string[] OII_FathersHusbandName, string[] OII_TIN_No,
            string[] OII_IssueDate, string[] OII_Present_Address, string[] OII_Permanent_Address, string[] OII_ContactNo,
            string[] OII_ResidencePhoneNo, string[] OII_MobileNo, string[] OII_PassportNo, string[] OwnerPassportDistrict,
            string[] OII_PassportIssueDate, string[] OII_PassportExpiryDate, string[] OII_Nationality, string[] OII_NationalID_No,
            string[] fpHsCode, string[] fpDesc, string[] regOrg,
            string[] ABI_Name, string[] ABI_Designation, string[] ABI_TIN_No, string[] ABI_TIN_Issue_Date, string[] ABI_NID_No,
            string[] ABI_NID_Issue_Date, string[] ABI_NameOfAssoc, string[] ABI_AddressOfAssoc, string[] ABI_BINOfAssoc,
            string[] ABI_TIN_OfAssoc, string[] ABI_NatureOfAssoc, string[] ABI_PositionOfAssoc, string[] ABI_ShareOfAssoc,
            string[] MII_Machine_Desc, string[] MII_BE_No, string[] MII_BE_Date, string[] MII_Indemn_Undertake_No,
            string[] MII_Indemn_Date, string[] MII_Cash_Challan_No, string[] MII_Cash_Challan_Date, string[] MII_Due_Date,
            string[] MII_Actual_Date,
            string[] MPO_Clear_Cer_No, string[] MPO_Clear_Date, string[] MPO_Perm, string[] MPO_Perm_Date,
            string[] MPO_Transfer_No, string[] MPO_Transfer_Date, string[] MPO_Bank_Code, string[] MPO_Bank_Name,
            string[] MPO_Branch, string[] MPO_Bank_Address,
            string[] AIL_HS_Code, string[] AIL_Desc, string[] AIL_Spec_Grade, string[] AIL_Quantity, string[] AIL_Unit,
            FormCollection collection, HttpPostedFileBase UploadDolil, HttpPostedFileBase UploadDeed,
            HttpPostedFileBase UploadRentalVATChallan, HttpPostedFileBase DI_TIN_Upload, HttpPostedFileBase DI_BOI_Upload,
            HttpPostedFileBase DI_BSCIC_Upload, HttpPostedFileBase DI_ERC_Upload, HttpPostedFileBase DI_IRC_Upload,
            HttpPostedFileBase DI_VAT_Upload, HttpPostedFileBase DI_Trade_License_Upload,
            HttpPostedFileBase DI_Fire_License_Upload, HttpPostedFileBase DI_Environment_Certificate_Upload,
            HttpPostedFileBase DI_Boiler_Certificate_Upload, HttpPostedFileBase EI_CertificateUpload,
            HttpPostedFileBase EI_RecommendationUpload, HttpPostedFileBase[] LBI_CertificateFileName,
            HttpPostedFileBase[] OII_SignatureUpload, HttpPostedFileBase[] OII_PhotoUpload,
            HttpPostedFileBase[] EI_CertificateFileName, HttpPostedFileBase[] EI_RecommendationFileName,
            HttpPostedFileBase[] DocumentFile, HttpPostedFileBase uploadAuthSign, HttpPostedFileBase uploadPhotoOfAuthSign,
            HttpPostedFileBase uploadDeclareSign, HttpPostedFileBase uploadDeclareSeal)
        {
            //string BMSdb = "Data Source=192.168.2.11:1521/XE; User ID=BMS;Password=BMS;";

            //System.Data.OracleClient.OracleConnection conn = new System.Data.OracleClient.OracleConnection(BMSdb);

            //cmd.Connection = ConnectBMS.Connection();

            System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();

            System.Data.OracleClient.OracleConnection conn = ConnectBMS.Connection();

            cmd.Connection = conn;

            System.Data.OracleClient.OracleTransaction bmsTransaction = conn.BeginTransaction();
            cmd.Transaction = bmsTransaction;
            int bondSlNo = 0;
            switch (Request.Form["Save"])
            {
                case "Save as Draft(Section A)":
                    try
                    {
                        saveSectionA(LBI_Code, Branch, Office, EnrollmetnMEMBERSHIPREGNO, EnrollmetnInputIssueDate,
                 EnrollmetnInputExpiryDate, DocumentName, DocumentNo, DocumentIssueDate, DocumentExpiryDate,
                collection, UploadDolil, UploadDeed, UploadRentalVATChallan, DI_TIN_Upload, DI_BOI_Upload,
                DI_BSCIC_Upload, DI_ERC_Upload, DI_IRC_Upload, DI_VAT_Upload, DI_Trade_License_Upload, DI_Fire_License_Upload,
                DI_Environment_Certificate_Upload, DI_Boiler_Certificate_Upload, EI_CertificateUpload,
                EI_RecommendationUpload, LBI_CertificateFileName, EI_CertificateFileName, EI_RecommendationFileName,
                DocumentFile, cmd, conn, bmsTransaction, bondSlNo);
                    }
                    catch
                    {
                        return Start(collection);
                    }
                    break;
                case "Save as Draft(Section B)":
                    saveSectionB(OwnerPositionDesignation, OII_Name, OII_FathersHusbandName, OII_TIN_No,
            OII_IssueDate, OII_Present_Address, OII_Permanent_Address, OII_ContactNo,
            OII_ResidencePhoneNo, OII_MobileNo, OII_PassportNo, OwnerPassportDistrict,
            OII_PassportIssueDate, OII_PassportExpiryDate, OII_Nationality, OII_NationalID_No,
            OII_SignatureUpload, OII_PhotoUpload, cmd, conn, bmsTransaction, bondSlNo);
                    //ViewBag.SectionB = 0;

                    #region old_OwnerInfo_SectionB
                    //try
                    //{
                    //    #region connection
                    //    //System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();

                    //    //System.Data.OracleClient.OracleConnection conn = ConnectBMS.Connection();
                    //    //try
                    //    //{
                    //    //    conn.Open();                            
                    //    //}
                    //    //catch
                    //    //{                            
                    //    //}
                    //    //finally
                    //    //{
                    //    //    cmd.Connection = conn;
                    //    //}
                    //    //System.Data.OracleClient.OracleTransaction bmsTransaction = conn.BeginTransaction();
                    //    //bmsTransaction = conn.BeginTransaction();
                    //    //cmd.Transaction = bmsTransaction;
                    //    //cmd.Connection = ConnectBMS.Connection();
                    //    #endregion

                    //    if (saveMode == "update")
                    //    {
                    //        cmd.CommandText = "update Bonder set OWNERCATEGORY=:OWNERCATEGORY,MODIFIEDBY=:MODIFIEDBY," +
                    //            "MODIFYDATE=:MODIFYDATE where BONDERSLNO=:BONDERSLNO";
                    //        cmd.Parameters.Clear();
                    //        cmd.Parameters.AddWithValue("OWNERCATEGORY", Request["OwnershipCategory"]);
                    //        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                    //        cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                    //        cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    //        try
                    //        {
                    //            cmd.ExecuteNonQuery();
                    //            bmsTransaction.Commit();
                    //        }
                    //        catch
                    //        {
                    //            bmsTransaction.Rollback();
                    //        }

                    //        try
                    //        {
                    //            try
                    //            {
                    //                bmsTransaction = conn.BeginTransaction();
                    //            }
                    //            catch { }
                    //            cmd.CommandText = "select OWNERSLNO from OWNERINFO where BONDERSLNO=:BONDERSLNO";
                    //            cmd.Parameters.Clear();
                    //            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);

                    //            System.Data.OracleClient.OracleDataReader drBONDEROwnerSlNo;
                    //            drBONDEROwnerSlNo = cmd.ExecuteReader();
                    //            drBONDEROwnerSlNo.Read();


                    //            if (!drBONDEROwnerSlNo.HasRows)
                    //            {
                    //                cmd.CommandText = "insert into OwnerInfo(BONDERSLNO,DESSLNO,OWNERNAME,OWNERFHNAME,TINNO,TINISSDT," +
                    //                    "OWNERPRSADDRESS,OWNERPERADDRESS,OWNERCONTACTNO,OWNERRESPHONE,OWNERMOBPHONE,PASSPORTNO," +
                    //                    "PASSDISTRICTSLNO,PASSISSUEDT,PASSEXPDT,NATIONALITY,NATIONALZIDNO,SIGNATUREFILENM,PHOTOFILENM," +
                    //                    "INPUTBY,INPUTDATE) values(:BONDERSLNO,:DESSLNO,:OWNERNAME,:OWNERFHNAME,:TINNO,:TINISSDT," +
                    //                ":OWNERPRSADDRESS,:OWNERPERADDRESS,:OWNERCONTACTNO,:OWNERRESPHONE,:OWNERMOBPHONE,:PASSPORTNO," +
                    //                ":PASSDISTRICTSLNO,:PASSISSUEDT,:PASSEXPDT,:NATIONALITY,:NATIONALZIDNO,:SIGNATUREFILENM," +
                    //                ":PHOTOFILENM,:INPUTBY,:INPUTDATE)";
                    //                cmd.Parameters.Clear();

                    //                cmd.Parameters.AddWithValue("INPUTBY", user);
                    //                cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();

                    //            }
                    //            else
                    //            {
                    //                cmd.CommandText = "update OwnerInfo set DESSLNO=:DESSLNO,OWNERNAME=:OWNERNAME,OWNERFHNAME=:OWNERFHNAME," +
                    //                    "TINNO=:TINNO,TINISSDT=:TINISSDT,OWNERPRSADDRESS=:OWNERPRSADDRESS,OWNERPERADDRESS=:OWNERPERADDRESS," +
                    //                    "OWNERCONTACTNO=:OWNERCONTACTNO,OWNERRESPHONE=:OWNERRESPHONE,OWNERMOBPHONE=:OWNERMOBPHONE," +
                    //                    "PASSPORTNO=:PASSPORTNO,PASSDISTRICTSLNO=:PASSDISTRICTSLNO,PASSISSUEDT=:PASSISSUEDT," +
                    //                    "PASSEXPDT=:PASSEXPDT,NATIONALITY=:NATIONALITY,NATIONALZIDNO=:NATIONALZIDNO," +
                    //                    "SIGNATUREFILENM=:SIGNATUREFILENM,PHOTOFILENM=:PHOTOFILENM,MODIFIEDBY=:MODIFIEDBY," +
                    //                    "MODIFYDATE=:MODIFYDATE where BONDERSLNO=:BONDERSLNO";
                    //                cmd.Parameters.Clear();
                    //                cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                    //                cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    //            }
                    //            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                    //            cmd.Parameters.AddWithValue("DESSLNO", Request["OwnerPositionDesignation"]);
                    //            cmd.Parameters.AddWithValue("OWNERNAME", Request["OII_Name"]);
                    //            cmd.Parameters.AddWithValue("OWNERFHNAME", Request["OII_FathersHusbandName"]);
                    //            cmd.Parameters.AddWithValue("TINNO", Request["OII_TIN_No"]);
                    //            if (!String.IsNullOrEmpty(Request["OII_IssueDate"]))

                    //                cmd.Parameters.Add(new OracleParameter(":TINISSDT", OracleType.DateTime)).Value = Request["OII_IssueDate"];
                    //            else
                    //                cmd.Parameters.Add(new OracleParameter(":TINISSDT", OracleType.DateTime)).Value = DBNull.Value;
                    //            cmd.Parameters.AddWithValue("OWNERPRSADDRESS", Request["OII_Present_Address"]);
                    //            cmd.Parameters.AddWithValue("OWNERPERADDRESS", Request["OII_Permanent_Address"]);
                    //            cmd.Parameters.AddWithValue("OWNERCONTACTNO", Request["OII_ContactNo"]);
                    //            cmd.Parameters.AddWithValue("OWNERRESPHONE", Request["OII_ResidencePhoneNo"]);
                    //            cmd.Parameters.AddWithValue("OWNERMOBPHONE", Request["OII_MobileNo"]);
                    //            cmd.Parameters.AddWithValue("PASSPORTNO", Request["OII_PassportNo"]);
                    //            cmd.Parameters.AddWithValue("PASSDISTRICTSLNO", Request["OwnerPassportDistrict"]);

                    //            if (!String.IsNullOrEmpty(Request["OII_PassportIssueDate"]))
                    //                cmd.Parameters.Add(new OracleParameter(":PASSISSUEDT", OracleType.DateTime)).Value = Request["OII_PassportIssueDate"];
                    //            else
                    //                cmd.Parameters.Add(new OracleParameter(":PASSISSUEDT", OracleType.DateTime)).Value = DBNull.Value;
                    //            if (!String.IsNullOrEmpty(Request["OII_PassportExpiryDate"]))
                    //                cmd.Parameters.Add(new OracleParameter(":PASSEXPDT", OracleType.DateTime)).Value = Request["OII_PassportExpiryDate"];
                    //            else
                    //                cmd.Parameters.Add(new OracleParameter(":PASSEXPDT", OracleType.DateTime)).Value = DBNull.Value;
                    //            cmd.Parameters.AddWithValue("NATIONALITY", Request["OII_Nationality"]);
                    //            cmd.Parameters.AddWithValue("NATIONALZIDNO", Request["OII_NationalID_No"]);
                    //            //cmd.Parameters.AddWithValue("SIGNATUREFILENM", Request["btnOII_SignatureUpload"]);
                    //            //cmd.Parameters.AddWithValue("PHOTOFILENM", Request["btnOII_PhotoUpload"]);
                    //            if (OII_SignatureUpload != null && OII_SignatureUpload.ContentLength > 0)
                    //            {
                    //                path = Path.Combine(Server.MapPath("~/Uploads"), Path.GetFileName(OII_SignatureUpload.FileName));
                    //                pathCount = 1;
                    //                while (System.IO.File.Exists(path))
                    //                {
                    //                    pathCount++;
                    //                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(OII_SignatureUpload.FileName) + pathCount + Path.GetExtension(OII_SignatureUpload.FileName));
                    //                }
                    //                OII_SignatureUpload.SaveAs(path);
                    //                cmd.Parameters.AddWithValue("SIGNATUREFILENM", path);
                    //            }
                    //            else
                    //            {
                    //                cmd.Parameters.AddWithValue("SIGNATUREFILENM", Request["OII_SignatureFileName"]);
                    //            }
                    //            if (OII_PhotoUpload != null && OII_PhotoUpload.ContentLength > 0)
                    //            {
                    //                path = Path.Combine(Server.MapPath("~/Uploads"), Path.GetFileName(OII_PhotoUpload.FileName));
                    //                pathCount = 1;
                    //                while (System.IO.File.Exists(path))
                    //                {
                    //                    pathCount++;
                    //                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(OII_PhotoUpload.FileName) + pathCount + Path.GetExtension(OII_PhotoUpload.FileName));
                    //                }
                    //                OII_PhotoUpload.SaveAs(path);
                    //                cmd.Parameters.AddWithValue("PHOTOFILENM", path);
                    //            }
                    //            else
                    //            {
                    //                cmd.Parameters.AddWithValue("PHOTOFILENM", Request["OII_PhotoFileName"]);
                    //            }
                    //            try
                    //            {
                    //                try
                    //                {
                    //                    bmsTransaction = conn.BeginTransaction();
                    //                }
                    //                catch
                    //                { }
                    //                cmd.Transaction = bmsTransaction;
                    //                cmd.ExecuteNonQuery();
                    //                bmsTransaction.Commit();
                    //            }
                    //            catch
                    //            {
                    //                bmsTransaction.Rollback();
                    //            }
                    //        }
                    //        catch
                    //        {
                    //        }

                    //    }
                    //}
                    //catch
                    //{
                    //    return Start(collection);
                    //}
                    #endregion
                    break;
                case "Save as Draft(Section C)":
                    saveSectionC(fpHsCode, fpDesc, regOrg, cmd, conn, bmsTransaction, bondSlNo);
                    break;
                case "Save Application":
                    try
                    {
                        saveSectionA(LBI_Code, Branch, Office, EnrollmetnMEMBERSHIPREGNO, EnrollmetnInputIssueDate,
                 EnrollmetnInputExpiryDate, DocumentName, DocumentNo, DocumentIssueDate, DocumentExpiryDate,
                collection, UploadDolil, UploadDeed, UploadRentalVATChallan, DI_TIN_Upload, DI_BOI_Upload,
                DI_BSCIC_Upload, DI_ERC_Upload, DI_IRC_Upload, DI_VAT_Upload, DI_Trade_License_Upload, DI_Fire_License_Upload,
                DI_Environment_Certificate_Upload, DI_Boiler_Certificate_Upload, EI_CertificateUpload,
                EI_RecommendationUpload, LBI_CertificateFileName, EI_CertificateFileName, EI_RecommendationFileName,
                DocumentFile, cmd, conn, bmsTransaction, bondSlNo);

                        saveSectionB(OwnerPositionDesignation, OII_Name, OII_FathersHusbandName, OII_TIN_No,
           OII_IssueDate, OII_Present_Address, OII_Permanent_Address, OII_ContactNo,
           OII_ResidencePhoneNo, OII_MobileNo, OII_PassportNo, OwnerPassportDistrict,
           OII_PassportIssueDate, OII_PassportExpiryDate, OII_Nationality, OII_NationalID_No,
           OII_SignatureUpload, OII_PhotoUpload, cmd, conn, bmsTransaction, bondSlNo);

                        saveSectionC(fpHsCode, fpDesc, regOrg, cmd, conn, bmsTransaction, bondSlNo);

                        saveSectionD(IMI_Description, IMI_Brand_Name, IMI_Model_No, IMI_Manufacturing_Year,
                            IMI_Country_of_Origin, IMI_Date_of_Installation, IMI_BE_No_or_VAT_Challan_No, IMI_VAT_Date,
                            IMI_BL_No, IMI_BL_Date, IMI_Invoice_No, IMI_Invoice_Date, IMI_LC_No, IMI_LC_Date, IMI_No_of_Machines,
             IMI_Machine_Type, IMI_Procurement_Type, IMI_Indemnity_Bond, PWM_HS_Code, PWM_AnnualCapacity, PWM_Unit, PWM_Machine, PWM_Qty,
             PWM_Machines_Type, PWM_Machines_Qty, PWM_Model_No, ABI_Name, ABI_Designation, ABI_TIN_No,
             ABI_TIN_Issue_Date, ABI_NID_No, ABI_NID_Issue_Date, ABI_NameOfAssoc, ABI_AddressOfAssoc, ABI_BINOfAssoc,
            ABI_TIN_OfAssoc, ABI_NatureOfAssoc, ABI_PositionOfAssoc, ABI_ShareOfAssoc, MII_Machine_Desc, MII_BE_No, MII_BE_Date,
            MII_Indemn_Undertake_No, MII_Indemn_Date, MII_Cash_Challan_No, MII_Cash_Challan_Date, MII_Due_Date, MII_Actual_Date,
            MPO_Clear_Cer_No, MPO_Clear_Date, MPO_Perm, MPO_Perm_Date, MPO_Transfer_No, MPO_Transfer_Date, MPO_Bank_Code,
            MPO_Bank_Name, MPO_Branch, MPO_Bank_Address, AIL_HS_Code, AIL_Desc, AIL_Spec_Grade, AIL_Quantity, AIL_Unit,
            uploadAuthSign, uploadPhotoOfAuthSign, uploadDeclareSign, uploadDeclareSeal,
            cmd, conn, bmsTransaction, bondSlNo);
                        # region comment
                        //cmdM.CommandText = "insert into MACHINEINFORMATION(DESCRIPTION,Brand_Name,MODELNM,MANUFACTUREYEAR,COUNTRYSLNO,"
                        //+ "INPUTBY,INPUTDATE) values(:DESCRIPTION,:Brand_Name,:MODELNM,:MANUFACTUREYEAR,:COUNTRYSLNO,:INPUTBY,:INPUTDATE)";

                        //cmdM.Parameters.Clear();

                        //cmdM.Parameters.AddWithValue("DESCRIPTION", Request["IMI_Description"]);
                        //cmdM.Parameters.AddWithValue("Brand_Name", Request["IMI_Brand_Name"]);
                        //cmdM.Parameters.AddWithValue("MODELNM", Request["IMI_Model_No"]);
                        //cmdM.Parameters.AddWithValue("MANUFACTUREYEAR", Request["IMI_Manufacturing_Year"]);
                        //cmdM.Parameters.AddWithValue("COUNTRYSLNO", Request["IMI_Country_of_Origin"]);
                        //cmdM.Parameters.AddWithValue("INPUTBY", user);
                        //cmdM.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                        //try
                        //{
                        //    //try
                        //    //{
                        //    //    bmsTransaction = conn.BeginTransaction();
                        //    //}
                        //    //catch { }
                        //    cmdM.Transaction = bmsTransaction;
                        //    cmdM.ExecuteNonQuery();
                        //    //bmsTransaction.Commit();
                        //    cmdM.CommandText = "select max(MACHINESLNO) from MACHINEINFORMATION";
                        //    cmdM.Parameters.Clear();

                        //    System.Data.OracleClient.OracleDataReader drMACHINESlNo;
                        //    drMACHINESlNo = cmdM.ExecuteReader();
                        //    drMACHINESlNo.Read();
                        //    BondInfo.MACHINESLNO = drMACHINESlNo.GetInt32(0);
                        //}
                        //catch
                        //{
                        //    //bmsTransaction.Rollback();
                        //}
                        #endregion
                        # region comment
                        //cmd.CommandText = "insert into INSTALLEDMACHINEINFO(BONDERSLNO,DATEOFINSTALL,BENO,BENODATE,VATCHNO,"
                        //    + "VATCHNODATE,BLNO,BLNODATE,INVOICENO,INVOICENODATE,LCNO,LCNODATE,MACHINEQTY,MACHINETYPE,"
                        //    + "PROCUREMENTTYPE,INDEMNITYBOND,INDEMNITYUNDERTKNO,INDEMNITYUNDERTKDT,CASHCHALNVOUCHNO,"
                        //    + "CASHCHALNVOUCHDT,DUERELEASDT,ACTRELEASDT,LOCALPURCLEARENCEFROM,LOCALPURCLEARENCENO,"
                        //    + "LOCALPURCLEARENCEDT,LOCALPURPERMISSIONFROMCBC,LOCALPURPERMISSIONFROMCBCDT,TRANSFERLIENBANK,"
                        //    + "TRANSFERLIENBANKNOCNO,TRANSFERLIENBANKDT,INPUTBY,INPUTDATE)"
                        //    + " values(:BONDERSLNO,:DATEOFINSTALL,:BENO,:BENODATE,:VATCHNO,"
                        //    + ":VATCHNODATE,:BLNO,:BLNODATE,:INVOICENO,:INVOICENODATE,:LCNO,:LCNODATE,:MACHINEQTY,:MACHINETYPE,"
                        //    + ":PROCUREMENTTYPE,:INDEMNITYBOND,:INDEMNITYUNDERTKNO,:INDEMNITYUNDERTKDT,:CASHCHALNVOUCHNO,"
                        //    + ":CASHCHALNVOUCHDT,:DUERELEASDT,:ACTRELEASDT,:LOCALPURCLEARENCEFROM,:LOCALPURCLEARENCENO,"
                        //    + ":LOCALPURCLEARENCEDT,:LOCALPURPERMISSIONFROMCBC,:LOCALPURPERMISSIONFROMCBCDT,:TRANSFERLIENBANK,"
                        //    + ":TRANSFERLIENBANKNOCNO,:TRANSFERLIENBANKDT,:INPUTBY,:INPUTDATE)";
                        #endregion

                        //}
                        #region old_else
                        //                        else
                        //                        {
                        //                            BondInfo.MACHINESLNO = drINSTALLEDMACHINEINFOSlNo.GetInt32(0);
                        //                            #region comment
                        //                            //cmdM.CommandText = "update MACHINEINFORMATION set DESCRIPTION=:DESCRIPTION,Brand_Name=:Brand_Name," +
                        //                            //    "MODELNM=:MODELNM,MANUFACTUREYEAR=:MANUFACTUREYEAR,COUNTRYSLNO=:COUNTRYSLNO," +
                        //                            //    "MODIFIEDBY=:MODIFIEDBY,MODIFYDATE=:MODIFYDATE where MACHINESLNO=:MACHINESLNO";
                        //                            //cmdM.Parameters.Clear();
                        //                            //cmdM.Parameters.AddWithValue("DESCRIPTION", Request["IMI_Description"]);
                        //                            //cmdM.Parameters.AddWithValue("Brand_Name", Request["IMI_Brand_Name"]);
                        //                            //cmdM.Parameters.AddWithValue("MODELNM", Request["IMI_Model_No"]);
                        //                            //cmdM.Parameters.AddWithValue("MANUFACTUREYEAR", Request["IMI_Manufacturing_Year"]);
                        //                            //cmdM.Parameters.AddWithValue("COUNTRYSLNO", Request["IMI_Country_of_Origin"]);
                        //                            //cmdM.Parameters.AddWithValue("MACHINESLNO", BondInfo.MACHINESLNO);
                        //                            //cmdM.Parameters.AddWithValue("MODIFIEDBY", user);
                        //                            //cmdM.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();

                        //                            //try
                        //                            //{
                        //                            //    //try
                        //                            //    //{
                        //                            //    //    bmsTransaction = conn.BeginTransaction();
                        //                            //    //}
                        //                            //    //catch { }
                        //                            //    cmdM.Transaction = bmsTransaction;
                        //                            //    cmdM.ExecuteNonQuery();
                        //                            //    //bmsTransaction.Commit();                                     
                        //                            //}
                        //                            //catch
                        //                            //{
                        //                            //    //bmsTransaction.Rollback();
                        //                            //}
                        //#endregion
                        //                            cmd.CommandText = "update INSTALLEDMACHINEINFO set DATEOFINSTALL=:DATEOFINSTALL,BENO=:BENO,BENODATE=:BENODATE,"
                        //                          + "VATCHNO=:VATCHNO,VATCHNODATE=:VATCHNODATE,BLNO=:BLNO,BLNODATE=:BLNODATE,INVOICENO=:INVOICENO,"
                        //                          + "INVOICENODATE=:INVOICENODATE,LCNO=:LCNO,LCNODATE=:LCNODATE,MACHINEQTY=:MACHINEQTY,MACHINETYPE=:MACHINETYPE,"
                        //                          + "PROCUREMENTTYPE=:PROCUREMENTTYPE,INDEMNITYBOND=:INDEMNITYBOND,MODIFIEDBY=:MODIFIEDBY,"
                        //                          + "MODIFYDATE=:MODIFYDATE,DESCRIPTION=:DESCRIPTION,MANUFACTUREYEAR=:MANUFACTUREYEAR,COUNTRYSLNO=:COUNTRYSLNO"
                        //                          + "where BONDERSLNO=:BONDERSLNO and MACHINESLNO=:MACHINESLNO";
                        //                            #region comment
                        //                            //cmd.CommandText = "update INSTALLEDMACHINEINFO set DATEOFINSTALL=:DATEOFINSTALL,BENO=:BENO,BENODATE=:BENODATE,"
                        //                            //+ "VATCHNO=:VATCHNO,VATCHNODATE=:VATCHNODATE,BLNO=:BLNO,BLNODATE=:BLNODATE,INVOICENO=:INVOICENO,"
                        //                            //+ "INVOICENODATE=:INVOICENODATE,LCNO=:LCNO,LCNODATE=:LCNODATE,MACHINEQTY=:MACHINEQTY,MACHINETYPE=:MACHINETYPE,"
                        //                            //+ "PROCUREMENTTYPE=:PROCUREMENTTYPE,INDEMNITYBOND=:INDEMNITYBOND,INDEMNITYUNDERTKNO=:INDEMNITYUNDERTKNO,"
                        //                            //+ "INDEMNITYUNDERTKDT=:INDEMNITYUNDERTKDT,CASHCHALNVOUCHNO=:CASHCHALNVOUCHNO,"
                        //                            //+ "CASHCHALNVOUCHDT=:CASHCHALNVOUCHDT,DUERELEASDT=:DUERELEASDT,ACTRELEASDT=:ACTRELEASDT,"
                        //                            //+ "LOCALPURCLEARENCEFROM=:LOCALPURCLEARENCEFROM,LOCALPURCLEARENCENO=:LOCALPURCLEARENCENO,"
                        //                            //+ "LOCALPURCLEARENCEDT=:LOCALPURCLEARENCEDT,LOCALPURPERMISSIONFROMCBC=:LOCALPURPERMISSIONFROMCBC,"
                        //                            //+ "LOCALPURPERMISSIONFROMCBCDT=:LOCALPURPERMISSIONFROMCBCDT,TRANSFERLIENBANK=:TRANSFERLIENBANK,"
                        //                            //+ "TRANSFERLIENBANKNOCNO=:TRANSFERLIENBANKNOCNO,TRANSFERLIENBANKDT=:TRANSFERLIENBANKDT,MODIFIEDBY=:MODIFIEDBY,"
                        //                            //+ "MODIFYDATE=:MODIFYDATE where BONDERSLNO=:BONDERSLNO";
                        //                            #endregion
                        //                            cmd.Parameters.Clear();
                        //                            cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                        //                            cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                        //                        }

                        //                        //cmd.Parameters.AddWithValue("MACHINESLNO", BondInfo.MACHINESLNO);
                        //                        cmd.Parameters.AddWithValue("MACHINESLNO", Request["IMI_Brand_Name"]);
                        //                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);

                        //                        if (!String.IsNullOrEmpty(Request["IMI_Date_of_Installation"]))
                        //                            cmd.Parameters.Add(new OracleParameter(":DATEOFINSTALL", OracleType.DateTime)).Value = Request["IMI_Date_of_Installation"];
                        //                        else
                        //                            cmd.Parameters.Add(new OracleParameter(":DATEOFINSTALL", OracleType.DateTime)).Value = DBNull.Value;

                        //                        cmd.Parameters.AddWithValue("BENO", Request["IMI_BE_No_or_VAT_Challan_No"]);

                        //                        if (!String.IsNullOrEmpty(Request["IMI_VAT_Date"]))
                        //                            cmd.Parameters.Add(new OracleParameter(":BENODATE", OracleType.DateTime)).Value = Request["IMI_VAT_Date"];
                        //                        else
                        //                            cmd.Parameters.Add(new OracleParameter(":BENODATE", OracleType.DateTime)).Value = DBNull.Value;

                        //                        cmd.Parameters.AddWithValue("VATCHNO", Request["IMI_BE_No_or_VAT_Challan_No"]);

                        //                        if (!String.IsNullOrEmpty(Request["IMI_VAT_Date"]))
                        //                            cmd.Parameters.Add(new OracleParameter(":VATCHNODATE", OracleType.DateTime)).Value = Request["IMI_VAT_Date"];
                        //                        else
                        //                            cmd.Parameters.Add(new OracleParameter(":VATCHNODATE", OracleType.DateTime)).Value = DBNull.Value;

                        //                        cmd.Parameters.AddWithValue("BLNO", Request["IMI_BL_No"]);

                        //                        if (!String.IsNullOrEmpty(Request["IMI_BL_Date"]))
                        //                            cmd.Parameters.Add(new OracleParameter(":BLNODATE", OracleType.DateTime)).Value = Request["IMI_BL_Date"];
                        //                        else
                        //                            cmd.Parameters.Add(new OracleParameter(":BLNODATE", OracleType.DateTime)).Value = DBNull.Value;

                        //                        cmd.Parameters.AddWithValue("INVOICENO", Request["IMI_Invoice_No"]);

                        //                        if (!String.IsNullOrEmpty(Request["IMI_Invoice_Date"]))
                        //                            cmd.Parameters.Add(new OracleParameter(":INVOICENODATE", OracleType.DateTime)).Value = Request["IMI_Invoice_Date"];
                        //                        else
                        //                            cmd.Parameters.Add(new OracleParameter(":INVOICENODATE", OracleType.DateTime)).Value = DBNull.Value;

                        //                        cmd.Parameters.AddWithValue("LCNO", Request["IMI_LC_No"]);

                        //                        if (!String.IsNullOrEmpty(Request["IMI_LC_Date"]))
                        //                            cmd.Parameters.Add(new OracleParameter(":LCNODATE", OracleType.DateTime)).Value = Request["IMI_LC_Date"];
                        //                        else
                        //                            cmd.Parameters.Add(new OracleParameter(":LCNODATE", OracleType.DateTime)).Value = DBNull.Value;

                        //                        cmd.Parameters.AddWithValue("MACHINEQTY", Request["IMI_No_of_Machines"]);
                        //                        cmd.Parameters.AddWithValue("MACHINETYPE", Request["IMI_Machine_Type"]);
                        //                        cmd.Parameters.AddWithValue("PROCUREMENTTYPE", Request["IMI_Procurement_Type"]);
                        //                        cmd.Parameters.AddWithValue("INDEMNITYBOND", Request["IMI_Indemnity_Bond"]);
                        //                        cmd.Parameters.AddWithValue("DESCRIPTION", Request["IMI_Description"]);
                        //                        cmd.Parameters.AddWithValue("MANUFACTUREYEAR", Request["IMI_Manufacturing_Year"]);
                        //                        cmd.Parameters.AddWithValue("COUNTRYSLNO", Request["IMI_Country"]);
                        //                        try
                        //                        {
                        //                            try
                        //                            {
                        //                                bmsTransaction = conn.BeginTransaction();
                        //                            }
                        //                            catch { }
                        //                            cmd.Transaction = bmsTransaction;
                        //                            cmd.ExecuteNonQuery();
                        //                            bmsTransaction.Commit();
                        //                        }
                        //                        catch
                        //                        {
                        //                            bmsTransaction.Rollback();
                        //                        }
                        #endregion
                    }
                    catch
                    {
                        return Start(collection);
                    }
                    break;
                default:
                    break;
            }
            //var n = Request.Form["Save"];
            //var name = Request.Form["ManufacturingUnitName"];            
            return Start(collection);
        }

        public void saveSectionA(string[] LBI_Code, string[] Branch,
            string[] Office, string[] EnrollmetnMEMBERSHIPREGNO, string[] EnrollmetnInputIssueDate,
            string[] EnrollmetnInputExpiryDate,
            string[] DocumentName, string[] DocumentNo, string[] DocumentIssueDate, string[] DocumentExpiryDate,
 FormCollection collection, HttpPostedFileBase UploadDolil, HttpPostedFileBase UploadDeed,
            HttpPostedFileBase UploadRentalVATChallan, HttpPostedFileBase DI_TIN_Upload, HttpPostedFileBase DI_BOI_Upload,
            HttpPostedFileBase DI_BSCIC_Upload, HttpPostedFileBase DI_ERC_Upload, HttpPostedFileBase DI_IRC_Upload,
            HttpPostedFileBase DI_VAT_Upload, HttpPostedFileBase DI_Trade_License_Upload,
            HttpPostedFileBase DI_Fire_License_Upload, HttpPostedFileBase DI_Environment_Certificate_Upload,
            HttpPostedFileBase DI_Boiler_Certificate_Upload, HttpPostedFileBase EI_CertificateUpload,
            HttpPostedFileBase EI_RecommendationUpload, HttpPostedFileBase[] LBI_CertificateFileName, HttpPostedFileBase[] EI_CertificateFileName, HttpPostedFileBase[] EI_RecommendationFileName,
            HttpPostedFileBase[] DocumentFile, System.Data.OracleClient.OracleCommand cmd,
            System.Data.OracleClient.OracleConnection conn,
            System.Data.OracleClient.OracleTransaction bmsTransaction, int bondSlNo)
        {
            //try
            //{
            //cmd.CommandText = "insert into BONDER(BONDERSLNO,BONDERNAME,HOLDINGNO,ROADNO,AREAVILLAGE,MOWJA,UNIONNAME,BUPAZILASLNO,BDISTRICTSLNO,PSTATION,WARD,CITYPOURO,PHONE,FAX,MOBILE,EMAIL) values((select NVL(max(BONDERSLNO),0)+1 from BONDER)," +
            //    "'" + Request.Form["ManufacturingUnitName"] + "','" + Request["HoldingNo"] + "','" + Request["RoadNo"] + "','" + Request["AreaVillage"] + "','" + Request["Mowja"] + "','" + Request["Union"] + "','" + Request["Upazila"] + "','" + Request["District"] + "','" + Request["PoliceStation"] + "','" + Request["Ward"] + "','" + Request["CtCorpPowrosova"] + "','" + Request["Phone"] + "','" + Request["Fax"] + "','" + Request["Mobile"] + "','"+Request["Email"]+"')";                        



            if (!String.IsNullOrEmpty(Request["ManufacturingUnitName"]))
            {
                System.Data.OracleClient.OracleDataReader drBondSlNo;

                cmd.CommandText = "select BONDERSLNO from BONDER where BONDERNAME=:BONDERNAME";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("BONDERNAME", Request["ManufacturingUnitName"]);
                drBondSlNo = cmd.ExecuteReader();
                drBondSlNo.Read();
                if (drBondSlNo.HasRows)
                {
                    saveMode = "update";
                    bondSlNo = drBondSlNo.GetInt32(0);
                    BondInfo.bondSlNoToEdit = bondSlNo;
                }
                else
                {
                    //saveMode = "insert";
                }
            }

            //----------------Mizan Work (06 Aug 2016)------------------

            System.Data.OracleClient.OracleDataReader drBSNo;
            cmd.CommandText = "select max(BSNO) from BONDSTATUS where BONDERSLNO=:BONDERSLNO";
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            drBSNo = cmd.ExecuteReader();
            drBSNo.Read();
            if (drBSNo.HasRows)
            {
                int bsno = 0;
                bsno = drBSNo.GetInt32(0);
                BondInfo.BSNoToEdit = bsno;
            }

            //----------------Mizan Work (06 Aug 2016)------------------

            cmd.CommandText = "select NVL(max(BONDERSLNO),0)+1 from BONDER";
            cmd.Parameters.Clear();
            System.Data.OracleClient.OracleDataReader drMaxBondSlNo = cmd.ExecuteReader();
            drMaxBondSlNo.Read();

            if (saveMode == "insert")
            {
                cmd.CommandText = "insert into BONDER(ADDRESS," +
                    "BDISTRICTSLNO,PHONE,FAX,MOBILE,EMAIL," +
"HODISTRICTSLNO," +
"HOPHONE,HOFAX,HOMOBILE,HOEMAIL, PREMESISSTATUS,HIREDATE,HIREEXPDATE,OWNERSHIPDATE,OWNERSHIPDOLILNO," +
"DolilFileNM,DEEDFILENM,RENTVATCHALLANNO," +
"VCHALNFILENM,CSDAGNO,RSDAGNO,BONDFACPREV," +/*BONDLICENSENO,*/"BTYPESLNO,BCATSLNO,BSCATSLNO,INPUTBY,INPUTDATE) values(" +
":ADDRESS,:BDISTRICTSLNO,:PHONE,:FAX,:MOBILE,:EMAIL," +
":HODISTRICTSLNO," +
":HOPHONE,:HOFAX,:HOMOBILE,:HOEMAIL, :PREMESISSTATUS,:HIREDATE,:HIREEXPDATE," +
":OWNERSHIPDATE,:OWNERSHIPDOLILNO,:DolilFileNM," +
":DEEDFILENM,:RENTVATCHALLANNO,:VCHALNFILENM,:CSDAGNO,:RSDAGNO,:BONDFACPREV,"/*:BONDLICENSENO,"*/ +
":BTYPESLNO,:BCATSLNO,:BSCATSLNO,:INPUTBY,:INPUTDATE)";

                cmd.Parameters.Clear();
                //cmd.Parameters.AddWithValue("BONDERSLNO", drMaxBondSlNo.GetInt32(0));
                cmd.Parameters.AddWithValue("INPUTBY", user);
                cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                //cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DBNull.Value;
            }
            else if (saveMode == "update")
            {
                cmd.CommandText = "update BONDER set BONDERNAME=:BONDERNAME," +
                    "ADDRESS=:ADDRESS," +
                    "BDISTRICTSLNO=:BDISTRICTSLNO," +
                    "PHONE=:PHONE,FAX=:FAX,MOBILE=:MOBILE,EMAIL=:EMAIL," +
                    "COADDRESS=:COADDRESS,HODISTRICTSLNO=:HODISTRICTSLNO," +
                    "HOPHONE=:HOPHONE,HOFAX=:HOFAX,HOMOBILE=:HOMOBILE,HOEMAIL=:HOEMAIL," +
                    "PREMESISSTATUS=:PREMESISSTATUS,HIREDATE=:HIREDATE,HIREEXPDATE=:HIREEXPDATE," +
                    "OWNERSHIPDATE=:OWNERSHIPDATE,OWNERSHIPDOLILNO=:OWNERSHIPDOLILNO," +
                    "DOLILFILENM=:DOLILFILENM,DEEDFILENM=:DEEDFILENM," +
                    "RENTVATCHALLANNO=:RENTVATCHALLANNO," +
                    "VCHALNFILENM=:VCHALNFILENM,CSDAGNO=:CSDAGNO,RSDAGNO=:RSDAGNO,BONDFACPREV=:BONDFACPREV," +
                    /*"BONDLICENSENO=:BONDLICENSENO,*/"BTYPESLNO=:BTYPESLNO,BCATSLNO=:BCATSLNO,BSCATSLNO=:BSCATSLNO," +
                    "MODIFIEDBY=:MODIFIEDBY,MODIFYDATE=:MODIFYDATE,BONDSTATUS=:BONDSTATUS where BONDERSLNO=:BONDERSLNO";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                cmd.Parameters.AddWithValue("BONDERNAME", Request["ManufacturingUnitName"]);
                cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
            }

            //cmd.Parameters.AddWithValue("HOLDINGNO", Request["HoldingNo"]);
            //cmd.Parameters.AddWithValue("ROADNO", Request["RoadNo"]);
            //cmd.Parameters.AddWithValue("AREAVILLAGE", Request["AreaVillage"]);
            //cmd.Parameters.AddWithValue("MOWJA", Request["Mowja"]);
            //cmd.Parameters.AddWithValue("UNIONNAME", Request["Union"]);
            //cmd.Parameters.AddWithValue("BUPAZILASLNO", Request["Upazila"]);
            cmd.Parameters.AddWithValue("ADDRESS", Request["Address"]);
            cmd.Parameters.AddWithValue("BDISTRICTSLNO", Request["District"]);
            //cmd.Parameters.AddWithValue("PSTATION", Request["PoliceStation"]);
            //cmd.Parameters.AddWithValue("WARD", Request["Ward"]);
            //cmd.Parameters.AddWithValue("CITYPOURO", Request["CtCorpPowrosova"]);
            cmd.Parameters.AddWithValue("PHONE", Request["Phone"]);
            cmd.Parameters.AddWithValue("FAX", Request["Fax"]);
            cmd.Parameters.AddWithValue("MOBILE", Request["Mobile"]);
            cmd.Parameters.AddWithValue("EMAIL", Request["Email"]);

            //cmd.Parameters.AddWithValue("HOHOLDINGNO", Request["HoldingNo2"]);
            //cmd.Parameters.AddWithValue("HOROADNO", Request["RoadNo2"]);
            //cmd.Parameters.AddWithValue("HOAREAVILLAGE", Request["AreaVillage2"]);
            //cmd.Parameters.AddWithValue("HOMOWJA", Request["Mowja2"]);
            //cmd.Parameters.AddWithValue("HOUNION", Request["Union2"]);
            //cmd.Parameters.AddWithValue("HOUPAZILASLNO", Request["HoUpazila"]);
            cmd.Parameters.AddWithValue("COADDRESS", Request["CoAddress"]);
            cmd.Parameters.AddWithValue("HODISTRICTSLNO", Request["HoDistrict"]);
            //cmd.Parameters.AddWithValue("HOPSTATION", Request["PoliceStation2"]);
            //cmd.Parameters.AddWithValue("HOWARD", Request["Ward2"]);
            //cmd.Parameters.AddWithValue("HOCITYPOURO", Request["CtCorpPowrosova2"]);
            cmd.Parameters.AddWithValue("HOPHONE", Request["Phone2"]);
            cmd.Parameters.AddWithValue("HOFAX", Request["Fax2"]);
            cmd.Parameters.AddWithValue("HOMOBILE", Request["Mobile2"]);
            cmd.Parameters.AddWithValue("HOEMAIL", Request["Email2"]);

            cmd.Parameters.AddWithValue("PREMESISSTATUS", Request["MUPSSelect"]);

            if (!String.IsNullOrEmpty(Request["MUPSDate"]))
                cmd.Parameters.Add(new OracleParameter(":HIREDATE", OracleType.DateTime)).Value = Request["MUPSDate"];
            else
                cmd.Parameters.Add(new OracleParameter(":HIREDATE", OracleType.DateTime)).Value = DBNull.Value;
            if (!String.IsNullOrEmpty(Request["MUPSExpDate"]))
                cmd.Parameters.Add(new OracleParameter(":HIREEXPDATE", OracleType.DateTime)).Value = Request["MUPSExpDate"];
            else
                cmd.Parameters.Add(new OracleParameter(":HIREEXPDATE", OracleType.DateTime)).Value = DBNull.Value;
            if (!String.IsNullOrEmpty(Request["IfOwnedDate"]))
                cmd.Parameters.Add(new OracleParameter(":OWNERSHIPDATE", OracleType.DateTime)).Value = Request["IfOwnedDate"];
            else
                cmd.Parameters.Add(new OracleParameter(":OWNERSHIPDATE", OracleType.DateTime)).Value = DBNull.Value;

            cmd.Parameters.AddWithValue("OWNERSHIPDOLILNO", Request["DolilNo"]);

            #region old_fileReq
            //foreach (string upload in Request.Files)
            //{
            //    //if (!Request.Files[upload].HasFile()) continue;

            //    string mimeType = Request.Files[upload].ContentType;
            //    Stream fileStream = Request.Files[upload].InputStream;
            //    string fileName = Path.GetFileName(Request.Files[upload].FileName);
            //    int fileLength = Request.Files[upload].ContentLength;
            //    byte[] fileData = new byte[fileLength];
            //    fileStream.Read(fileData, 0, fileLength);
            //}

            //if (files != null)
            //{
            //    foreach (var file in files)
            //    {
            //        if (file.ContentLength > 0)
            //        {
            //            var fileName = Path.GetFileName(file.FileName);
            //            path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
            //            file.SaveAs(path);
            //        }
            //    }
            //}
            #endregion

            if (UploadDolil != null && UploadDolil.ContentLength > 0)
            {
                // extract only the fielname
                var fileName = Path.GetFileName(UploadDolil.FileName);
                // store the file inside ~/App_Data/uploads folder
                path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), fileName);
                pathCount = 1;
                while (System.IO.File.Exists(path))
                {
                    pathCount++;
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(UploadDolil.FileName) + pathCount + Path.GetExtension(UploadDolil.FileName));
                }
                UploadDolil.SaveAs(path);
                cmd.Parameters.AddWithValue("DOLILFILENM", path);
            }
            else
            {
                cmd.Parameters.AddWithValue("DOLILFILENM", Request["DolilFileName"]);
            }
            if (UploadDeed != null && UploadDeed.ContentLength > 0)
            {
                // extract only the fielname
                var fileName = Path.GetFileName(UploadDeed.FileName);
                // store the file inside ~/App_Data/uploads folder
                path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                pathCount = 1;
                while (System.IO.File.Exists(path))
                {
                    pathCount++;
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(UploadDeed.FileName) + pathCount + Path.GetExtension(UploadDeed.FileName));
                }
                UploadDeed.SaveAs(path);
                cmd.Parameters.AddWithValue("DEEDFILENM", path);
            }
            else
            {
                cmd.Parameters.AddWithValue("DEEDFILENM", Request["DeedFileName"]);
            }

            cmd.Parameters.AddWithValue("RENTVATCHALLANNO", Request["RentalVATChallanNo"]);

            if (UploadRentalVATChallan != null && UploadRentalVATChallan.ContentLength > 0)
            {
                path = Path.Combine(Server.MapPath("~/Uploads"), Path.GetFileName(UploadRentalVATChallan.FileName));
                pathCount = 1;
                while (System.IO.File.Exists(path))
                {
                    pathCount++;
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(UploadRentalVATChallan.FileName) + pathCount + Path.GetExtension(UploadRentalVATChallan.FileName));
                }
                UploadRentalVATChallan.SaveAs(path);
                cmd.Parameters.AddWithValue("VCHALNFILENM", path);
            }
            else
            {
                cmd.Parameters.AddWithValue("VCHALNFILENM", Request["RentalVATChallanFileName"]);
            }

            #region old_up_file
            //HttpPostedFile file = Request.Files["UploadDolil"];

            //if (Request.Files.Count > 0)
            //{
            //    var file = Request.Files[0];

            //    if (file != null && file.ContentLength > 0)
            //    {
            //        var fileName = Path.GetFileName(file.FileName);
            //        var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
            //        file.SaveAs(path);
            //    }
            //}

            //if (!String.IsNullOrEmpty(Request["UploadDolil"]))
            //{

            //    try
            //    {
            //        string filePath = Path.GetFullPath(Request["UploadDolil"]);

            //        FileStream FS = new FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            //        blob = new byte[FS.Length];
            //        FS.Read(blob, 0, System.Convert.ToInt32(FS.Length));
            //        FS.Close();
            //        // close all instances
            //        FS.Close();
            //        //FS.Dispose();
            //        OracleParameter blobParameter = new OracleParameter();
            //        blobParameter.OracleType = OracleType.Blob;
            //        blobParameter.ParameterName = "DOLILFILEDATA";
            //        blobParameter.Value = blob;
            //        cmd.Parameters.Add(blobParameter);
            //    }
            //    catch (Exception ex)
            //    {
            //        //MessageBox.Show(ex.Message);
            //    }
            //}

            //if (!String.IsNullOrEmpty(Request["UploadDolil"]))
            //    cmd.Parameters.AddWithValue("DOLILFILENM", Request["UploadDolil"]);
            //else
            //    cmd.Parameters.AddWithValue("DOLILFILENM", "");
            //if (!String.IsNullOrEmpty(Request["UploadDeed"]))
            //    cmd.Parameters.AddWithValue("DEEDFILENM", Request["UploadDeed"]);
            //else
            //    cmd.Parameters.AddWithValue("DEEDFILENM", "");



            //if (!String.IsNullOrEmpty(Request["UploadRentalVATChallan"]))
            //    cmd.Parameters.AddWithValue("VCHALNFILENM", Request["UploadRentalVATChallan"]);
            //else
            //    cmd.Parameters.AddWithValue("VCHALNFILENM", "");
            #endregion

            cmd.Parameters.AddWithValue("CSDAGNO", Request["CSDagNo"]);
            cmd.Parameters.AddWithValue("RSDAGNO", Request["RSDagNo"]);
            cmd.Parameters.AddWithValue("BONDFACPREV", Request["OtherBondedFactorySelect"]);
            //cmd.Parameters.AddWithValue("BONDLICENSENO", Request["BondLicenseNo"]);
            cmd.Parameters.AddWithValue("BTYPESLNO", Request["BondType"]);
            cmd.Parameters.AddWithValue("BCATSLNO", Request["BondCategory"]);
            cmd.Parameters.AddWithValue("BSCATSLNO", Request["BondSubCategory"]);
            if (BondInfo.bondStatus != "")
                cmd.Parameters.AddWithValue("BONDSTATUS", BondInfo.bondStatus);
            else
                cmd.Parameters.AddWithValue("BONDSTATUS", "A");

            try
            {
                //string query = cmd.CommandText;

                //try
                //{
                //    foreach (OracleParameter p in cmd.Parameters)
                //    {
                //        query = query.Replace(":" + p.ParameterName, p.Value.ToString());
                //    }
                //}
                //catch
                //{
                //    string r = query;
                //}
                cmd.ExecuteNonQuery();
                bmsTransaction.Commit();
            }
            catch
            {
                bmsTransaction.Rollback();
            }
            //finally
            //{
            //    conn.Close();
            //    conn.Dispose();
            //}

            // Document Information

            //Tin

            cmd.CommandText = "select NVL(max(RGATTCHSLNO),0)+1 from REGISTRATIONATTACHMENT";
            cmd.Parameters.Clear();
            System.Data.OracleClient.OracleDataReader drMaxSlNo;
            //drMaxSlNo = cmd.ExecuteReader();
            //drMaxSlNo.Read();

            System.Data.OracleClient.OracleDataReader drTinSlNo;

            cmd.CommandText = "select ATTCHSLNO,ATTACHFILENM from DOCUMENTATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
                " and BONDERSLNO=:BONDERSLNO and BSNO=:BSNO";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DOCHEADINGNAME", "Tin");
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);

            drTinSlNo = cmd.ExecuteReader();
            drTinSlNo.Read();

            if (!String.IsNullOrEmpty(Request["DI_TIN_NO"]))
            {
                if (saveMode == "insert")
                {
                    cmd.CommandText = "insert into REGISTRATIONATTACHMENT(RGATTCHSLNO,BONDERSLNO,DOCHEADINGNAME," +
                    "RGATTCHNAME,ISSUEDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(:RGATTCHSLNO," +
                    ":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                    cmd.Parameters.Clear();
                    //cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("BONDERSLNO", drMaxBondSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("INPUTBY", user);
                    cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                }

                else if (saveMode == "update")
                {
                    //  cmd.CommandText = "select RGATTCHSLNO from REGISTRATIONATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
                    //" and BONDERSLNO=:BONDERSLNO";


                    if (!drTinSlNo.HasRows)
                    {
                        //        cmd.CommandText = "insert into REGISTRATIONATTACHMENT(BONDERSLNO,DOCHEADINGNAME," +
                        //"RGATTCHNAME,ISSUEDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
                        //":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.CommandText = "insert into DOCUMENTATTACHMENT(BONDERSLNO,BSNO,DOCHEADINGNAME," +
                "RGATTCHNAME,ISSUEDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
                ":BONDERSLNO,:BSNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                        cmd.Parameters.AddWithValue("INPUTBY", user);
                        cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                    else
                    {
                        //    cmd.CommandText = "update REGISTRATIONATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                        //"RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,ATTACHFILENM=:ATTACHFILENM,MODIFIEDBY=:MODIFIEDBY," +
                        //"MODIFYDATE=:MODIFYDATE where RGATTCHSLNO=:RGATTCHSLNO" +
                        //" and BONDERSLNO=:BONDERSLNO";

                        cmd.CommandText = "update DOCUMENTATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                    "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,ATTACHFILENM=:ATTACHFILENM,MODIFIEDBY=:MODIFIEDBY," +
                    "MODIFYDATE=:MODIFYDATE where ATTCHSLNO=:ATTCHSLNO" +
                    " and BONDERSLNO=:BONDERSLNO";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drTinSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("ATTCHSLNO", drTinSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                        cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                }


                cmd.Parameters.AddWithValue("DOCHEADINGNAME", "Tin");
                cmd.Parameters.AddWithValue("RGATTCHNAME", Request["DI_TIN_NO"]);
                if (!String.IsNullOrEmpty(Request["DI_TIN_Issue_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = Request["DI_TIN_Issue_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;

                if (DI_TIN_Upload != null)
                {
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(DI_TIN_Upload.FileName));
                    pathCount = 1;
                    while (System.IO.File.Exists(path))
                    {
                        pathCount++;
                        path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(DI_TIN_Upload.FileName) + pathCount + Path.GetExtension(DI_TIN_Upload.FileName));
                    }
                    DI_TIN_Upload.SaveAs(path);
                    cmd.Parameters.AddWithValue("ATTACHFILENM", path);
                }
                else if (drTinSlNo.HasRows)
                {
                    if (!string.IsNullOrEmpty(drTinSlNo.GetValue(1).ToString()))
                        cmd.Parameters.AddWithValue("ATTACHFILENM", drTinSlNo.GetValue(1).ToString());
                    else
                        cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                }
                else
                    cmd.Parameters.AddWithValue("ATTACHFILENM", "");

                //if (!String.IsNullOrEmpty(Request["DI_TIN_Upload"]))
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", Request["DI_TIN_Upload"]);
                //else
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", "");


                try
                {
                    bmsTransaction = conn.BeginTransaction();
                    cmd.Transaction = bmsTransaction;
                    cmd.ExecuteNonQuery();
                    bmsTransaction.Commit();
                }
                catch
                {
                    bmsTransaction.Rollback();
                }
            }

            //Boi


            cmd.CommandText = "select ATTCHSLNO,ATTACHFILENM from DOCUMENTATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
      " and BONDERSLNO=:BONDERSLNO and BSNO=:BSNO";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DOCHEADINGNAME", "Boi");
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);

            System.Data.OracleClient.OracleDataReader drBoiSlNo;
            drBoiSlNo = cmd.ExecuteReader();
            drBoiSlNo.Read();

            if (!String.IsNullOrEmpty(Request["DI_BOI_Reg_NO"]))
            {
                if (saveMode == "insert")
                {
                    cmd.CommandText = "select NVL(max(RGATTCHSLNO),0)+1 from REGISTRATIONATTACHMENT";
                    cmd.Parameters.Clear();
                    drMaxSlNo = cmd.ExecuteReader();
                    drMaxSlNo.Read();

                    cmd.CommandText = "insert into REGISTRATIONATTACHMENT(RGATTCHSLNO,BONDERSLNO,DOCHEADINGNAME," +
                    "RGATTCHNAME,ISSUEDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(:RGATTCHSLNO," +
                    ":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("BONDERSLNO", drMaxBondSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("INPUTBY", user);
                    cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                }

                else if (saveMode == "update")
                {
                    //  cmd.CommandText = "select RGATTCHSLNO from REGISTRATIONATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
                    //" and BONDERSLNO=:BONDERSLNO";

                    if (!drBoiSlNo.HasRows)
                    {
                        //       cmd.CommandText = "insert into REGISTRATIONATTACHMENT(BONDERSLNO,DOCHEADINGNAME," +
                        //"RGATTCHNAME,ISSUEDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
                        //":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.CommandText = "insert into DOCUMENTATTACHMENT(BONDERSLNO,BSNO,DOCHEADINGNAME," +
               "RGATTCHNAME,ISSUEDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
               ":BONDERSLNO,:BSNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                        cmd.Parameters.AddWithValue("INPUTBY", user);
                        cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                    else
                    {
                        //cmd.CommandText = "update REGISTRATIONATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                        //    "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,ATTACHFILENM=:ATTACHFILENM," +
                        //    "MODIFIEDBY=:MODIFIEDBY,MODIFYDATE=:MODIFYDATE where RGATTCHSLNO=:RGATTCHSLNO" +
                        //    " and BONDERSLNO=:BONDERSLNO";

                        cmd.CommandText = "update DOCUMENTATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,ATTACHFILENM=:ATTACHFILENM,MODIFIEDBY=:MODIFIEDBY," +
                "MODIFYDATE=:MODIFYDATE where ATTCHSLNO=:ATTCHSLNO" +
                " and BONDERSLNO=:BONDERSLNO";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drBoiSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("ATTCHSLNO", drBoiSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                        cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                }


                cmd.Parameters.AddWithValue("DOCHEADINGNAME", "Boi");
                cmd.Parameters.AddWithValue("RGATTCHNAME", Request["DI_BOI_Reg_NO"]);
                if (!String.IsNullOrEmpty(Request["DI_BOI_Issue_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = Request["DI_BOI_Issue_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;

                if (DI_BOI_Upload != null)
                {
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(DI_BOI_Upload.FileName));
                    pathCount = 1;
                    while (System.IO.File.Exists(path))
                    {
                        pathCount++;
                        path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(DI_BOI_Upload.FileName) + pathCount + Path.GetExtension(DI_BOI_Upload.FileName));
                    }
                    DI_BOI_Upload.SaveAs(path);
                    cmd.Parameters.AddWithValue("ATTACHFILENM", path);
                }
                else if (drBoiSlNo.HasRows)
                {
                    if (!string.IsNullOrEmpty(drBoiSlNo.GetValue(1).ToString()))
                        cmd.Parameters.AddWithValue("ATTACHFILENM", drBoiSlNo.GetValue(1).ToString());
                    else
                        cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                }
                else
                    cmd.Parameters.AddWithValue("ATTACHFILENM", "");

                //if (!String.IsNullOrEmpty(Request["DI_BOI_Upload"]))
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", Request["DI_BOI_Upload"]);
                //else
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", "");

                try
                {
                    bmsTransaction = conn.BeginTransaction();
                    cmd.Transaction = bmsTransaction;
                    cmd.ExecuteNonQuery();
                    bmsTransaction.Commit();
                }
                catch
                {
                    bmsTransaction.Rollback();
                }
            }

            //BSCIC

            cmd.CommandText = "select ATTCHSLNO,ATTACHFILENM from DOCUMENTATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
          " and BONDERSLNO=:BONDERSLNO and BSNO=:BSNO";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DOCHEADINGNAME", "BSCIC");
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);

            System.Data.OracleClient.OracleDataReader drBSCICSlNo;
            drBSCICSlNo = cmd.ExecuteReader();
            drBSCICSlNo.Read();

            if (!String.IsNullOrEmpty(Request["DI_BSCIC_Reg_No"]))
            {
                if (saveMode == "insert")
                {
                    cmd.CommandText = "select NVL(max(RGATTCHSLNO),0)+1 from REGISTRATIONATTACHMENT";
                    cmd.Parameters.Clear();
                    drMaxSlNo = cmd.ExecuteReader();
                    drMaxSlNo.Read();

                    cmd.CommandText = "insert into REGISTRATIONATTACHMENT(RGATTCHSLNO,BONDERSLNO,DOCHEADINGNAME," +
                    "RGATTCHNAME,ISSUEDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(:RGATTCHSLNO," +
                    ":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("BONDERSLNO", drMaxBondSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("INPUTBY", user);
                    cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                }
                else if (saveMode == "update")
                {
                    //   cmd.CommandText = "select RGATTCHSLNO from REGISTRATIONATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
                    //" and BONDERSLNO=:BONDERSLNO";


                    if (!drBSCICSlNo.HasRows)
                    {

                        //cmd.CommandText = "insert into REGISTRATIONATTACHMENT(BONDERSLNO,DOCHEADINGNAME," +
                        //"RGATTCHNAME,ISSUEDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
                        //":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.CommandText = "insert into DOCUMENTATTACHMENT(BONDERSLNO,BSNO,DOCHEADINGNAME," +
               "RGATTCHNAME,ISSUEDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
               ":BONDERSLNO,:BSNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                        cmd.Parameters.AddWithValue("INPUTBY", user);
                        cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                    else
                    {
                        //cmd.CommandText = "update REGISTRATIONATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                        //                                 "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,ATTACHFILENM=:ATTACHFILENM," +
                        //                                 "MODIFIEDBY=:MODIFIEDBY,MODIFYDATE=:MODIFYDATE where RGATTCHSLNO=:RGATTCHSLNO" +
                        //                                 " and BONDERSLNO=:BONDERSLNO";

                        cmd.CommandText = "update DOCUMENTATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,ATTACHFILENM=:ATTACHFILENM,MODIFIEDBY=:MODIFIEDBY," +
                "MODIFYDATE=:MODIFYDATE where ATTCHSLNO=:ATTCHSLNO" +
                " and BONDERSLNO=:BONDERSLNO";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drBSCICSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("ATTCHSLNO", drBSCICSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                        cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                }
                cmd.Parameters.AddWithValue("DOCHEADINGNAME", "BSCIC");
                cmd.Parameters.AddWithValue("RGATTCHNAME", Request["DI_BSCIC_Reg_No"]);
                if (!String.IsNullOrEmpty(Request["DI_BSCIC_Issue_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = Request["DI_BSCIC_Issue_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;


                if (DI_BSCIC_Upload != null)
                {
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(DI_BSCIC_Upload.FileName));
                    pathCount = 1;
                    while (System.IO.File.Exists(path))
                    {
                        pathCount++;
                        path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(DI_BSCIC_Upload.FileName) + pathCount + Path.GetExtension(DI_BSCIC_Upload.FileName));
                    }
                    DI_BSCIC_Upload.SaveAs(path);
                    cmd.Parameters.AddWithValue("ATTACHFILENM", path);
                }
                else if (drBSCICSlNo.HasRows)
                {
                    if (!string.IsNullOrEmpty(drBSCICSlNo.GetValue(1).ToString()))
                        cmd.Parameters.AddWithValue("ATTACHFILENM", drBSCICSlNo.GetValue(1).ToString());
                    else
                        cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                }
                else
                    cmd.Parameters.AddWithValue("ATTACHFILENM", "");


                //if (!String.IsNullOrEmpty(Request["DI_BSCIC_Upload"]))
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", Request["DI_BSCIC_Upload"]);
                //else
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", "");


                try
                {
                    bmsTransaction = conn.BeginTransaction();
                    cmd.Transaction = bmsTransaction;
                    cmd.ExecuteNonQuery();
                    bmsTransaction.Commit();
                }
                catch
                {
                    bmsTransaction.Rollback();
                }
            }
            //ERC

            cmd.CommandText = "select ATTCHSLNO,ATTACHFILENM from DOCUMENTATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
          " and BONDERSLNO=:BONDERSLNO and BSNO=:BSNO";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DOCHEADINGNAME", "ERC");
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);

            System.Data.OracleClient.OracleDataReader drERCSlNo;
            drERCSlNo = cmd.ExecuteReader();
            drERCSlNo.Read();

            if (!String.IsNullOrEmpty(Request["DI_ERC_No"]))
            {
                if (saveMode == "insert")
                {
                    cmd.CommandText = "select NVL(max(RGATTCHSLNO),0)+1 from REGISTRATIONATTACHMENT";
                    cmd.Parameters.Clear();
                    drMaxSlNo = cmd.ExecuteReader();
                    drMaxSlNo.Read();

                    cmd.CommandText = "insert into REGISTRATIONATTACHMENT(RGATTCHSLNO,BONDERSLNO,DOCHEADINGNAME," +
                    "RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(:RGATTCHSLNO," +
                    ":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("BONDERSLNO", drMaxBondSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("INPUTBY", user);
                    cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                }
                else if (saveMode == "update")
                {
                    //cmd.CommandText = "select RGATTCHSLNO from REGISTRATIONATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
                    //                       " and BONDERSLNO=:BONDERSLNO";                              

                    if (!drERCSlNo.HasRows)
                    {
                        //        cmd.CommandText = "insert into REGISTRATIONATTACHMENT(BONDERSLNO,DOCHEADINGNAME," +
                        //"RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
                        //":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.CommandText = "insert into DOCUMENTATTACHMENT(BONDERSLNO,BSNO,DOCHEADINGNAME," +
               "RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
               ":BONDERSLNO,:BSNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                        cmd.Parameters.AddWithValue("INPUTBY", user);
                        cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                    else
                    {
                        //    cmd.CommandText = "update REGISTRATIONATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                        //"RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,EXPDATE=:EXPDATE,ATTACHFILENM=:ATTACHFILENM," +
                        //"MODIFIEDBY=:MODIFIEDBY,MODIFYDATE=:MODIFYDATE where RGATTCHSLNO=:RGATTCHSLNO" +
                        //" and BONDERSLNO=:BONDERSLNO";

                        cmd.CommandText = "update DOCUMENTATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,EXPDATE=:EXPDATE,ATTACHFILENM=:ATTACHFILENM,MODIFIEDBY=:MODIFIEDBY," +
                "MODIFYDATE=:MODIFYDATE where ATTCHSLNO=:ATTCHSLNO" +
                " and BONDERSLNO=:BONDERSLNO";
                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drERCSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("ATTCHSLNO", drERCSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                        cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                }

                cmd.Parameters.AddWithValue("DOCHEADINGNAME", "ERC");
                cmd.Parameters.AddWithValue("RGATTCHNAME", Request["DI_ERC_No"]);
                if (!String.IsNullOrEmpty(Request["DI_ERC_Issue_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = Request["DI_ERC_Issue_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;

                if (!String.IsNullOrEmpty(Request["DI_ERC_Expiry_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = Request["DI_ERC_Expiry_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = DBNull.Value;

                if (DI_ERC_Upload != null)
                {
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(DI_ERC_Upload.FileName));
                    pathCount = 1;
                    while (System.IO.File.Exists(path))
                    {
                        pathCount++;
                        path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(DI_ERC_Upload.FileName) + pathCount + Path.GetExtension(DI_ERC_Upload.FileName));
                    }
                    DI_ERC_Upload.SaveAs(path);
                    cmd.Parameters.AddWithValue("ATTACHFILENM", path);
                }
                else if (drERCSlNo.HasRows)
                {
                    if (!string.IsNullOrEmpty(drERCSlNo.GetValue(1).ToString()))
                        cmd.Parameters.AddWithValue("ATTACHFILENM", drERCSlNo.GetValue(1).ToString());
                    else
                        cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                }
                else
                    cmd.Parameters.AddWithValue("ATTACHFILENM", "");


                //if (!String.IsNullOrEmpty(Request["DI_ERC_Upload"]))
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", Request["DI_ERC_Upload"]);
                //else
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", "");

                try
                {
                    bmsTransaction = conn.BeginTransaction();
                    cmd.Transaction = bmsTransaction;
                    cmd.ExecuteNonQuery();
                    bmsTransaction.Commit();
                }
                catch
                {
                    bmsTransaction.Rollback();
                }
            }
            //IRC

            cmd.CommandText = "select ATTCHSLNO,ATTACHFILENM from DOCUMENTATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
          " and BONDERSLNO=:BONDERSLNO and BSNO=:BSNO";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DOCHEADINGNAME", "IRC");
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);

            System.Data.OracleClient.OracleDataReader drIRCSlNo;
            drIRCSlNo = cmd.ExecuteReader();
            drIRCSlNo.Read();

            if (!String.IsNullOrEmpty(Request["DI_IRC_No"]))
            {
                if (saveMode == "insert")
                {
                    cmd.CommandText = "select NVL(max(RGATTCHSLNO),0)+1 from REGISTRATIONATTACHMENT";
                    cmd.Parameters.Clear();
                    drMaxSlNo = cmd.ExecuteReader();
                    drMaxSlNo.Read();

                    cmd.CommandText = "insert into REGISTRATIONATTACHMENT(RGATTCHSLNO,BONDERSLNO,DOCHEADINGNAME," +
                    "RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(:RGATTCHSLNO," +
                    ":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("BONDERSLNO", drMaxBondSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("INPUTBY", user);
                    cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                }
                else if (saveMode == "update")
                {
                    //cmd.CommandText = "select RGATTCHSLNO from REGISTRATIONATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
                    //                                                " and BONDERSLNO=:BONDERSLNO";


                    if (!drIRCSlNo.HasRows)
                    {
                        //        cmd.CommandText = "insert into REGISTRATIONATTACHMENT(BONDERSLNO,DOCHEADINGNAME," +
                        //"RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
                        //":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.CommandText = "insert into DOCUMENTATTACHMENT(BONDERSLNO,BSNO,DOCHEADINGNAME," +
               "RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
               ":BONDERSLNO,:BSNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";
                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                        cmd.Parameters.AddWithValue("INPUTBY", user);
                        cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                    else
                    {
                        //cmd.CommandText = "update REGISTRATIONATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                        //    "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,EXPDATE=:EXPDATE,ATTACHFILENM=:ATTACHFILENM," +
                        //    "MODIFIEDBY=:MODIFIEDBY,MODIFYDATE=:MODIFYDATE where RGATTCHSLNO=:RGATTCHSLNO" +
                        //" and BONDERSLNO=:BONDERSLNO";

                        cmd.CommandText = "update DOCUMENTATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,EXPDATE=:EXPDATE,ATTACHFILENM=:ATTACHFILENM,MODIFIEDBY=:MODIFIEDBY," +
                "MODIFYDATE=:MODIFYDATE where ATTCHSLNO=:ATTCHSLNO" +
                " and BONDERSLNO=:BONDERSLNO";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drIRCSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("ATTCHSLNO", drIRCSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                        cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                }
                cmd.Parameters.AddWithValue("DOCHEADINGNAME", "IRC");
                cmd.Parameters.AddWithValue("RGATTCHNAME", Request["DI_IRC_No"]);
                if (!String.IsNullOrEmpty(Request["DI_IRC_Issue_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = Request["DI_IRC_Issue_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;

                if (!String.IsNullOrEmpty(Request["DI_IRC_Expiry_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = Request["DI_IRC_Expiry_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = DBNull.Value;

                if (DI_IRC_Upload != null)
                {
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(DI_IRC_Upload.FileName));
                    pathCount = 1;
                    while (System.IO.File.Exists(path))
                    {
                        pathCount++;
                        path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(DI_IRC_Upload.FileName) + pathCount + Path.GetExtension(DI_IRC_Upload.FileName));
                    }
                    DI_IRC_Upload.SaveAs(path);
                    cmd.Parameters.AddWithValue("ATTACHFILENM", path);
                }
                else if (drIRCSlNo.HasRows)
                {
                    if (!string.IsNullOrEmpty(drIRCSlNo.GetValue(1).ToString()))
                        cmd.Parameters.AddWithValue("ATTACHFILENM", drIRCSlNo.GetValue(1).ToString());
                    else
                        cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                }
                else
                    cmd.Parameters.AddWithValue("ATTACHFILENM", "");


                //if (!String.IsNullOrEmpty(Request["DI_IRC_Upload"]))
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", Request["DI_IRC_Upload"]);
                //else
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", "");

                try
                {
                    bmsTransaction = conn.BeginTransaction();
                    cmd.Transaction = bmsTransaction;
                    cmd.ExecuteNonQuery();
                    bmsTransaction.Commit();
                }
                catch
                {
                    bmsTransaction.Rollback();
                }
            }
            //VAT

            cmd.CommandText = "select ATTCHSLNO,ATTACHFILENM from DOCUMENTATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
           " and BONDERSLNO=:BONDERSLNO and BSNO=:BSNO";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DOCHEADINGNAME", "VAT");
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);

            System.Data.OracleClient.OracleDataReader drVATSlNo;
            drVATSlNo = cmd.ExecuteReader();
            drVATSlNo.Read();

            if (!String.IsNullOrEmpty(Request["DI_VAT_No"]))
            {
                if (saveMode == "insert")
                {
                    cmd.CommandText = "select NVL(max(RGATTCHSLNO),0)+1 from REGISTRATIONATTACHMENT";
                    drMaxSlNo = cmd.ExecuteReader();
                    drMaxSlNo.Read();

                    cmd.Parameters.Clear();
                    cmd.CommandText = "insert into REGISTRATIONATTACHMENT(RGATTCHSLNO,BONDERSLNO,DOCHEADINGNAME," +
                    "RGATTCHNAME,ISSUEDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(:RGATTCHSLNO," +
                    ":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("BONDERSLNO", drMaxBondSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("INPUTBY", user);
                    cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                }
                else if (saveMode == "update")
                {
                    //cmd.CommandText = "select RGATTCHSLNO from REGISTRATIONATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
                    //                                                                          " and BONDERSLNO=:BONDERSLNO";

                    if (!drVATSlNo.HasRows)
                    {
                        //    cmd.CommandText = "insert into REGISTRATIONATTACHMENT(BONDERSLNO,DOCHEADINGNAME," +
                        //"RGATTCHNAME,ISSUEDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
                        //":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.CommandText = "insert into DOCUMENTATTACHMENT(BONDERSLNO,BSNO,DOCHEADINGNAME," +
               "RGATTCHNAME,ISSUEDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
               ":BONDERSLNO,:BSNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                        cmd.Parameters.AddWithValue("INPUTBY", user);
                        cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                    else
                    {
                        //cmd.CommandText = "update REGISTRATIONATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                        //    "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,ATTACHFILENM=:ATTACHFILENM," +
                        //    "MODIFIEDBY=:MODIFIEDBY,MODIFYDATE=:MODIFYDATE where RGATTCHSLNO=:RGATTCHSLNO" +
                        //    " and BONDERSLNO=:BONDERSLNO";

                        cmd.CommandText = "update DOCUMENTATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,ATTACHFILENM=:ATTACHFILENM,MODIFIEDBY=:MODIFIEDBY," +
                "MODIFYDATE=:MODIFYDATE where ATTCHSLNO=:ATTCHSLNO" +
                " and BONDERSLNO=:BONDERSLNO";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drVATSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("ATTCHSLNO", drVATSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                        cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                }
                cmd.Parameters.AddWithValue("DOCHEADINGNAME", "VAT");
                cmd.Parameters.AddWithValue("RGATTCHNAME", Request["DI_VAT_No"]);
                if (!String.IsNullOrEmpty(Request["DI_VAT_Issue_Date"]))
                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = Request["DI_VAT_Issue_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;

                if (DI_VAT_Upload != null)
                {
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(DI_VAT_Upload.FileName));
                    pathCount = 1;
                    while (System.IO.File.Exists(path))
                    {
                        pathCount++;
                        path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(DI_VAT_Upload.FileName) + pathCount + Path.GetExtension(DI_VAT_Upload.FileName));
                    }
                    DI_VAT_Upload.SaveAs(path);
                    cmd.Parameters.AddWithValue("ATTACHFILENM", path);
                }
                else if (drVATSlNo.HasRows)
                {
                    if (!string.IsNullOrEmpty(drVATSlNo.GetValue(1).ToString()))
                        cmd.Parameters.AddWithValue("ATTACHFILENM", drVATSlNo.GetValue(1).ToString());
                    else
                        cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                }
                else
                    cmd.Parameters.AddWithValue("ATTACHFILENM", "");

                //if (!String.IsNullOrEmpty(Request["DI_VAT_Upload"]))
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", Request["DI_VAT_Upload"]);
                //else
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", "");


                try
                {
                    bmsTransaction = conn.BeginTransaction();
                    cmd.Transaction = bmsTransaction;
                    cmd.ExecuteNonQuery();
                    bmsTransaction.Commit();
                }
                catch
                {
                    bmsTransaction.Rollback();
                }
            }
            //Trade License

            cmd.CommandText = "select ATTCHSLNO,ATTACHFILENM from DOCUMENTATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
           " and BONDERSLNO=:BONDERSLNO and BSNO=:BSNO";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DOCHEADINGNAME", "TradeLicense");
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);

            System.Data.OracleClient.OracleDataReader drTradeLicenseSlNo;
            drTradeLicenseSlNo = cmd.ExecuteReader();
            drTradeLicenseSlNo.Read();

            if (!String.IsNullOrEmpty(Request["DI_Trade_License_No"]))
            {
                if (saveMode == "insert")
                {
                    cmd.CommandText = "select NVL(max(RGATTCHSLNO),0)+1 from REGISTRATIONATTACHMENT";
                    cmd.Parameters.Clear();
                    drMaxSlNo = cmd.ExecuteReader();
                    drMaxSlNo.Read();

                    cmd.CommandText = "insert into REGISTRATIONATTACHMENT(RGATTCHSLNO,BONDERSLNO,DOCHEADINGNAME," +
                    "RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(:RGATTCHSLNO," +
                    ":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("BONDERSLNO", drMaxBondSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("INPUTBY", user);
                    cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                }
                else if (saveMode == "update")
                {
                    //cmd.CommandText = "select RGATTCHSLNO from REGISTRATIONATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
                    //                                                                                                     " and BONDERSLNO=:BONDERSLNO";


                    if (!drTradeLicenseSlNo.HasRows)
                    {
                        //      cmd.CommandText = "insert into REGISTRATIONATTACHMENT(BONDERSLNO,DOCHEADINGNAME," +
                        //"RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
                        //":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.CommandText = "insert into DOCUMENTATTACHMENT(BONDERSLNO,BSNO,DOCHEADINGNAME," +
               "RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
               ":BONDERSLNO,:BSNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                        cmd.Parameters.AddWithValue("INPUTBY", user);
                        cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                    else
                    {
                        //cmd.CommandText = "update REGISTRATIONATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                        //    "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,EXPDATE=:EXPDATE,ATTACHFILENM=:ATTACHFILENM," +
                        //    "MODIFIEDBY=:MODIFIEDBY,MODIFYDATE=:MODIFYDATE where RGATTCHSLNO=:RGATTCHSLNO" +
                        //    " and BONDERSLNO=:BONDERSLNO";

                        cmd.CommandText = "update DOCUMENTATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,EXPDATE=:EXPDATE,ATTACHFILENM=:ATTACHFILENM,MODIFIEDBY=:MODIFIEDBY," +
                "MODIFYDATE=:MODIFYDATE where ATTCHSLNO=:ATTCHSLNO" +
                " and BONDERSLNO=:BONDERSLNO";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drTradeLicenseSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("ATTCHSLNO", drTradeLicenseSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                        cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                }
                cmd.Parameters.AddWithValue("DOCHEADINGNAME", "TradeLicense");
                cmd.Parameters.AddWithValue("RGATTCHNAME", Request["DI_Trade_License_No"]);
                if (!String.IsNullOrEmpty(Request["DI_Trade_License_Issue_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = Request["DI_Trade_License_Issue_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;

                if (!String.IsNullOrEmpty(Request["DI_Trade_License_Expiry_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = Request["DI_Trade_License_Expiry_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = DBNull.Value;

                if (DI_Trade_License_Upload != null)
                {
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(DI_Trade_License_Upload.FileName));
                    pathCount = 1;
                    while (System.IO.File.Exists(path))
                    {
                        pathCount++;
                        path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(DI_Trade_License_Upload.FileName) + pathCount + Path.GetExtension(DI_Trade_License_Upload.FileName));
                    }
                    DI_Trade_License_Upload.SaveAs(path);
                    cmd.Parameters.AddWithValue("ATTACHFILENM", path);
                }
                else if (drTradeLicenseSlNo.HasRows)
                {
                    if (!string.IsNullOrEmpty(drTradeLicenseSlNo.GetValue(1).ToString()))
                        cmd.Parameters.AddWithValue("ATTACHFILENM", drTradeLicenseSlNo.GetValue(1).ToString());
                    else
                        cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                }
                else
                    cmd.Parameters.AddWithValue("ATTACHFILENM", "");

                //if (!String.IsNullOrEmpty(Request["DI_Trade_License_Upload"]))
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", Request["DI_Trade_License_Upload"]);
                //else
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", "");


                try
                {
                    bmsTransaction = conn.BeginTransaction();
                    cmd.Transaction = bmsTransaction;
                    cmd.ExecuteNonQuery();
                    bmsTransaction.Commit();
                }
                catch
                {
                    bmsTransaction.Rollback();
                }
            }
            //Fire License

            cmd.CommandText = "select ATTCHSLNO,ATTACHFILENM from DOCUMENTATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
         " and BONDERSLNO=:BONDERSLNO and BSNO=:BSNO";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DOCHEADINGNAME", "FireLicense");
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);

            System.Data.OracleClient.OracleDataReader drFireLicenseSlNo;
            drFireLicenseSlNo = cmd.ExecuteReader();
            drFireLicenseSlNo.Read();

            if (!String.IsNullOrEmpty(Request["DI_Fire_License_No"]))
            {
                if (saveMode == "insert")
                {
                    cmd.CommandText = "select NVL(max(RGATTCHSLNO),0)+1 from REGISTRATIONATTACHMENT";
                    cmd.Parameters.Clear();
                    drMaxSlNo = cmd.ExecuteReader();
                    drMaxSlNo.Read();

                    cmd.CommandText = "insert into REGISTRATIONATTACHMENT(RGATTCHSLNO,BONDERSLNO,DOCHEADINGNAME," +
                    "RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(:RGATTCHSLNO," +
                    ":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("BONDERSLNO", drMaxBondSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("INPUTBY", user);
                    cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                }
                else if (saveMode == "update")
                {
                    //cmd.CommandText = "select RGATTCHSLNO from REGISTRATIONATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
                    //    " and BONDERSLNO=:BONDERSLNO";


                    if (!drFireLicenseSlNo.HasRows)
                    {
                        //        cmd.CommandText = "insert into REGISTRATIONATTACHMENT(BONDERSLNO,DOCHEADINGNAME," +
                        //"RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
                        //":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.CommandText = "insert into DOCUMENTATTACHMENT(BONDERSLNO,BSNO,DOCHEADINGNAME," +
            "RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
            ":BONDERSLNO,:BSNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";
                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                        cmd.Parameters.AddWithValue("INPUTBY", user);
                        cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                    else
                    {
                        //cmd.CommandText = "update REGISTRATIONATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                        //    "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,EXPDATE=:EXPDATE,ATTACHFILENM=:ATTACHFILENM," +
                        //    "MODIFIEDBY=:MODIFIEDBY,MODIFYDATE=:MODIFYDATE where RGATTCHSLNO=:RGATTCHSLNO" +
                        //    " and BONDERSLNO=:BONDERSLNO";

                        cmd.CommandText = "update DOCUMENTATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,EXPDATE=:EXPDATE,ATTACHFILENM=:ATTACHFILENM,MODIFIEDBY=:MODIFIEDBY," +
                "MODIFYDATE=:MODIFYDATE where ATTCHSLNO=:ATTCHSLNO" +
                " and BONDERSLNO=:BONDERSLNO";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drFireLicenseSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("ATTCHSLNO", drFireLicenseSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                        cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                }
                cmd.Parameters.AddWithValue("DOCHEADINGNAME", "FireLicense");
                cmd.Parameters.AddWithValue("RGATTCHNAME", Request["DI_Fire_License_No"]);
                if (!String.IsNullOrEmpty(Request["DI_Fire_License_Issue_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = Request["DI_Fire_License_Issue_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;

                if (!String.IsNullOrEmpty(Request["DI_Fire_License_Expiry_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = Request["DI_Fire_License_Expiry_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = DBNull.Value;


                if (DI_Fire_License_Upload != null)
                {
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(DI_Fire_License_Upload.FileName));
                    pathCount = 1;
                    while (System.IO.File.Exists(path))
                    {
                        pathCount++;
                        path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(DI_Fire_License_Upload.FileName) + pathCount + Path.GetExtension(DI_Fire_License_Upload.FileName));
                    }
                    DI_Fire_License_Upload.SaveAs(path);
                    cmd.Parameters.AddWithValue("ATTACHFILENM", path);
                }
                else if (drFireLicenseSlNo.HasRows)
                {
                    if (!string.IsNullOrEmpty(drFireLicenseSlNo.GetValue(1).ToString()))
                        cmd.Parameters.AddWithValue("ATTACHFILENM", drFireLicenseSlNo.GetValue(1).ToString());
                    else
                        cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                }
                else
                    cmd.Parameters.AddWithValue("ATTACHFILENM", "");


                //if (!String.IsNullOrEmpty(Request["DI_Fire_License_Upload"]))
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", Request["DI_Fire_License_Upload"]);
                //else
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                try
                {
                    bmsTransaction = conn.BeginTransaction();
                    cmd.Transaction = bmsTransaction;
                    cmd.ExecuteNonQuery();
                    bmsTransaction.Commit();
                }
                catch
                {
                    bmsTransaction.Rollback();
                }
            }
            //Environment Certificate

            cmd.CommandText = "select ATTCHSLNO,ATTACHFILENM from DOCUMENTATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
            " and BONDERSLNO=:BONDERSLNO and BSNO=:BSNO";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DOCHEADINGNAME", "EnvironmentCertificate");
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);

            System.Data.OracleClient.OracleDataReader drEnvironmentCertificateSlNo;
            drEnvironmentCertificateSlNo = cmd.ExecuteReader();
            drEnvironmentCertificateSlNo.Read();

            if (!String.IsNullOrEmpty(Request["DI_Environment_Certificate_No"]))
            {
                if (saveMode == "insert")
                {
                    cmd.CommandText = "select NVL(max(RGATTCHSLNO),0)+1 from REGISTRATIONATTACHMENT";
                    cmd.Parameters.Clear();
                    drMaxSlNo = cmd.ExecuteReader();
                    drMaxSlNo.Read();

                    cmd.CommandText = "insert into REGISTRATIONATTACHMENT(RGATTCHSLNO,BONDERSLNO,DOCHEADINGNAME," +
                    "RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(:RGATTCHSLNO," +
                    ":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("BONDERSLNO", drMaxBondSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("INPUTBY", user);
                    cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                }
                else if (saveMode == "update")
                {
                    //cmd.CommandText = "select RGATTCHSLNO from REGISTRATIONATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
                    //                                " and BONDERSLNO=:BONDERSLNO";

                    if (!drEnvironmentCertificateSlNo.HasRows)
                    {
                        //           cmd.CommandText = "insert into REGISTRATIONATTACHMENT(BONDERSLNO,DOCHEADINGNAME," +
                        //"RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
                        //":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.CommandText = "insert into DOCUMENTATTACHMENT(BONDERSLNO,BSNO,DOCHEADINGNAME," +
            "RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
            ":BONDERSLNO,:BSNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                        cmd.Parameters.AddWithValue("INPUTBY", user);
                        cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                    else
                    {
                        //cmd.CommandText = "update REGISTRATIONATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                        //    "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,EXPDATE=:EXPDATE,ATTACHFILENM=:ATTACHFILENM," +
                        //    "MODIFIEDBY=:MODIFIEDBY,MODIFYDATE=:MODIFYDATE where RGATTCHSLNO=:RGATTCHSLNO" +
                        //    " and BONDERSLNO=:BONDERSLNO";

                        cmd.CommandText = "update DOCUMENTATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,EXPDATE=:EXPDATE,ATTACHFILENM=:ATTACHFILENM,MODIFIEDBY=:MODIFIEDBY," +
                "MODIFYDATE=:MODIFYDATE where ATTCHSLNO=:ATTCHSLNO" +
                " and BONDERSLNO=:BONDERSLNO";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drEnvironmentCertificateSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("ATTCHSLNO", drEnvironmentCertificateSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                        cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                }
                cmd.Parameters.AddWithValue("DOCHEADINGNAME", "EnvironmentCertificate");
                cmd.Parameters.AddWithValue("RGATTCHNAME", Request["DI_Environment_Certificate_No"]);
                if (!String.IsNullOrEmpty(Request["DI_Environment_Certificate_Issue_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime )).Value = Request["DI_Environment_Certificate_Issue_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;

                if (!String.IsNullOrEmpty(Request["DI_Environment_Certificate_Expiry_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = Request["DI_Environment_Certificate_Expiry_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = DBNull.Value;

                if (DI_Environment_Certificate_Upload != null)
                {
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(DI_Environment_Certificate_Upload.FileName));
                    pathCount = 1;
                    while (System.IO.File.Exists(path))
                    {
                        pathCount++;
                        path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(DI_Environment_Certificate_Upload.FileName) + pathCount + Path.GetExtension(DI_Environment_Certificate_Upload.FileName));
                    }
                    DI_Environment_Certificate_Upload.SaveAs(path);
                    cmd.Parameters.AddWithValue("ATTACHFILENM", path);
                }
                else if (drEnvironmentCertificateSlNo.HasRows)
                {
                    if (!string.IsNullOrEmpty(drEnvironmentCertificateSlNo.GetValue(1).ToString()))
                        cmd.Parameters.AddWithValue("ATTACHFILENM", drEnvironmentCertificateSlNo.GetValue(1).ToString());
                    else
                        cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                }
                else
                    cmd.Parameters.AddWithValue("ATTACHFILENM", "");


                //if (!String.IsNullOrEmpty(Request["DI_Environment_Certificate_Upload"]))
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", Request["DI_Environment_Certificate_Upload"]);
                //else
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", "");


                try
                {
                    bmsTransaction = conn.BeginTransaction();
                    cmd.Transaction = bmsTransaction;
                    cmd.ExecuteNonQuery();
                    bmsTransaction.Commit();
                }
                catch
                {
                    bmsTransaction.Rollback();
                }
            }
            //Boiler Certificate

            cmd.CommandText = "select ATTCHSLNO,ATTACHFILENM from DOCUMENTATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
        " and BONDERSLNO=:BONDERSLNO and BSNO=:BSNO";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DOCHEADINGNAME", "BoilerCertificate");
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);

            System.Data.OracleClient.OracleDataReader drBoilerCertificateSlNo;
            drBoilerCertificateSlNo = cmd.ExecuteReader();
            drBoilerCertificateSlNo.Read();

            if (!String.IsNullOrEmpty(Request["DI_Boiler_Certificate_No"]))
            {
                if (saveMode == "insert")
                {
                    cmd.CommandText = "select NVL(max(RGATTCHSLNO),0)+1 from REGISTRATIONATTACHMENT";
                    cmd.Parameters.Clear();
                    drMaxSlNo = cmd.ExecuteReader();
                    drMaxSlNo.Read();

                    cmd.CommandText = "insert into REGISTRATIONATTACHMENT(RGATTCHSLNO,BONDERSLNO,DOCHEADINGNAME," +
                    "RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(:RGATTCHSLNO," +
                    ":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("BONDERSLNO", drMaxBondSlNo.GetInt32(0));
                    cmd.Parameters.AddWithValue("INPUTBY", user);
                    cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                }
                else if (saveMode == "update")
                {
                    //cmd.CommandText = "select RGATTCHSLNO from REGISTRATIONATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME" +
                    //                                " and BONDERSLNO=:BONDERSLNO";

                    if (!drBoilerCertificateSlNo.HasRows)
                    {
                        //       cmd.CommandText = "insert into REGISTRATIONATTACHMENT(BONDERSLNO,DOCHEADINGNAME," +
                        //"RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
                        //":BONDERSLNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.CommandText = "insert into DOCUMENTATTACHMENT(BONDERSLNO,BSNO,DOCHEADINGNAME," +
            "RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(" +
            ":BONDERSLNO,:BSNO,:DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drMaxSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                        cmd.Parameters.AddWithValue("INPUTBY", user);
                        cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                    else
                    {
                        //cmd.CommandText = "update REGISTRATIONATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                        //    "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,EXPDATE=:EXPDATE,ATTACHFILENM=:ATTACHFILENM," +
                        //    "MODIFIEDBY=:MODIFIEDBY,MODIFYDATE=:MODIFYDATE where RGATTCHSLNO=:RGATTCHSLNO" +
                        //    " and BONDERSLNO=:BONDERSLNO";

                        cmd.CommandText = "update DOCUMENTATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME," +
                "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,EXPDATE=:EXPDATE,ATTACHFILENM=:ATTACHFILENM,MODIFIEDBY=:MODIFIEDBY," +
                "MODIFYDATE=:MODIFYDATE where ATTCHSLNO=:ATTCHSLNO" +
                " and BONDERSLNO=:BONDERSLNO";

                        cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("RGATTCHSLNO", drBoilerCertificateSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("ATTCHSLNO", drBoilerCertificateSlNo.GetInt32(0));
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                        cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                    }
                }
                cmd.Parameters.AddWithValue("DOCHEADINGNAME", "BoilerCertificate");
                cmd.Parameters.AddWithValue("RGATTCHNAME", Request["DI_Boiler_Certificate_No"]);
                if (!String.IsNullOrEmpty(Request["DI_Boiler_Certificate_Issue_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = Request["DI_Boiler_Certificate_Issue_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;

                if (!String.IsNullOrEmpty(Request["DI_Boiler_Certificate_Expiry_Date"]))

                    cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = Request["DI_Boiler_Certificate_Expiry_Date"];
                else
                    cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = DBNull.Value;

                if (DI_Boiler_Certificate_Upload != null)
                {
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(DI_Boiler_Certificate_Upload.FileName));
                    pathCount = 1;
                    while (System.IO.File.Exists(path))
                    {
                        pathCount++;
                        path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(DI_Boiler_Certificate_Upload.FileName) + pathCount + Path.GetExtension(DI_Boiler_Certificate_Upload.FileName));
                    }
                    DI_Boiler_Certificate_Upload.SaveAs(path);
                    cmd.Parameters.AddWithValue("ATTACHFILENM", path);
                }
                else if (drBoilerCertificateSlNo.HasRows)
                {
                    if (!string.IsNullOrEmpty(drBoilerCertificateSlNo.GetValue(1).ToString()))
                        cmd.Parameters.AddWithValue("ATTACHFILENM", drBoilerCertificateSlNo.GetValue(1).ToString());
                    else
                        cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                }
                else
                    cmd.Parameters.AddWithValue("ATTACHFILENM", "");

                //if (!String.IsNullOrEmpty(Request["DI_Boiler_Certificate_Upload"]))
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", Request["DI_Boiler_Certificate_Upload"]);
                //else
                //    cmd.Parameters.AddWithValue("ATTACHFILENM", "");


                try
                {
                    bmsTransaction = conn.BeginTransaction();
                    cmd.Transaction = bmsTransaction;
                    cmd.ExecuteNonQuery();
                    bmsTransaction.Commit();
                }
                catch
                {
                    bmsTransaction.Rollback();
                }
            }

            // Other Documents
            if (DocumentName != null)
            {
                for (int i = 0; i < DocumentName.Length; i++)
                {
                    cmd.CommandText = "select * from DOCUMENTATTACHMENT where DOCHEADINGNAME=:DOCHEADINGNAME"
                        + " and BONDERSLNO=:BONDERSLNO and BSNO=:BSNO";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("DOCHEADINGNAME", DocumentName[i]);
                    cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                    cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                    try
                    {
                        try
                        {
                            bmsTransaction = conn.BeginTransaction();
                        }
                        catch { }
                        cmd.Transaction = bmsTransaction;
                        OracleDataReader dr = cmd.ExecuteReader();
                        dr.Read();
                        if (dr.HasRows)
                        {
                            cmd.CommandText = "update DOCUMENTATTACHMENT set DOCHEADINGNAME=:DOCHEADINGNAME,"
                            + "RGATTCHNAME=:RGATTCHNAME,ISSUEDATE=:ISSUEDATE,EXPDATE=:EXPDATE,"
                                + "ATTACHFILENM=:ATTACHFILENM,MODIFIEDBY=:MODIFIEDBY,MODIFYDATE=:MODIFYDATE"
                            + " where ATTCHSLNO=:ATTCHSLNO";
                            cmd.Parameters.Clear();

                            cmd.Parameters.AddWithValue("ATTCHSLNO", dr.GetInt32(0));

                            cmd.Parameters.AddWithValue("DOCHEADINGNAME", DocumentName[i]);
                            cmd.Parameters.AddWithValue("RGATTCHNAME", DocumentNo[i]);

                            if (!String.IsNullOrEmpty(DocumentIssueDate[i]))
                                cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DocumentIssueDate[i];
                            else
                                cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;

                            if (!String.IsNullOrEmpty(DocumentExpiryDate[i]))
                                cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = DocumentExpiryDate[i];
                            else
                                cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = DBNull.Value;
                            if (DocumentFile != null)
                            {
                                if (DocumentFile[i] != null)
                                {
                                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(DocumentFile[i].FileName));
                                    pathCount = 1;
                                    while (System.IO.File.Exists(path))
                                    {
                                        pathCount++;
                                        path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(DocumentFile[i].FileName) + pathCount + Path.GetExtension(DocumentFile[i].FileName));
                                    }
                                    DocumentFile[i].SaveAs(path);
                                    cmd.Parameters.AddWithValue("ATTACHFILENM", path);
                                }
                                else if (dr.HasRows)
                                {
                                    if (!string.IsNullOrEmpty(dr.GetValue(10).ToString()))
                                        cmd.Parameters.AddWithValue("ATTACHFILENM", dr.GetValue(10).ToString());
                                    else
                                        cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                                }
                                else
                                    cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                            }
                            else
                                cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                            cmd.Parameters.AddWithValue("MODIFIEDBY", user);
                            cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();

                            cmd.ExecuteNonQuery();
                            bmsTransaction.Commit();
                        }
                        else
                        {
                            cmd.CommandText = "insert into DOCUMENTATTACHMENT(BONDERSLNO,BSNO,DOCHEADINGNAME,"
                            + "RGATTCHNAME,ISSUEDATE,EXPDATE,ATTACHFILENM,INPUTBY,INPUTDATE) values(:BONDERSLNO,:BSNO,"
                            + ":DOCHEADINGNAME,:RGATTCHNAME,:ISSUEDATE,:EXPDATE,:ATTACHFILENM,:INPUTBY,:INPUTDATE)";

                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                            cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                            cmd.Parameters.AddWithValue("DOCHEADINGNAME", DocumentName[i]);
                            cmd.Parameters.AddWithValue("RGATTCHNAME", DocumentNo[i]);

                            if (!String.IsNullOrEmpty(DocumentIssueDate[i]))
                                cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DocumentIssueDate[i];
                            else
                                cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;

                            if (!String.IsNullOrEmpty(DocumentExpiryDate[i]))
                                cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = DocumentExpiryDate[i];
                            else
                                cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = DBNull.Value;
                            try
                            {
                                if (DocumentFile != null)
                                {
                                    if (DocumentFile[i] != null)
                                    {
                                        path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(DocumentFile[i].FileName));
                                        pathCount = 1;
                                        while (System.IO.File.Exists(path))
                                        {
                                            pathCount++;
                                            path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(DocumentFile[i].FileName) + pathCount + Path.GetExtension(DocumentFile[i].FileName));
                                        }
                                        DocumentFile[i].SaveAs(path);
                                        cmd.Parameters.AddWithValue("ATTACHFILENM", path);
                                    }
                                    else
                                        cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                                }
                                else
                                    cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                            }
                            catch
                            {
                                cmd.Parameters.AddWithValue("ATTACHFILENM", "");
                            }
                            cmd.Parameters.AddWithValue("INPUTBY", user);
                            cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                            cmd.ExecuteNonQuery();
                            bmsTransaction.Commit();
                        }

                    }
                    catch
                    {
                        bmsTransaction.Rollback();
                    }

                }
            }

            // Enrollment
            if (Office != null)
            {
                for (int i = 1; i < Office.Length; i++)
                {
                    cmd.CommandText = "insert into ASSOCIATIONENROLL(BONDERSLNO,OFFICESLNO,BSNO,MEMBERSHIPREGNO,ISSUEDATE," +
                                "EXPDATE,CERATTACHFILENM,RECATTACHFILENM,INPUTBY,INPUTDATE) values(:BONDERSLNO," +
                                ":OFFICESLNO,:BSNO,:MEMBERSHIPREGNO,:ISSUEDATE,:EXPDATE,:CERATTACHFILENM,:RECATTACHFILENM,:INPUTBY,:INPUTDATE)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                    cmd.Parameters.AddWithValue("OFFICESLNO", Office[i]);
                    cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
                    cmd.Parameters.AddWithValue("MEMBERSHIPREGNO", EnrollmetnMEMBERSHIPREGNO[i]);

                    if (!String.IsNullOrEmpty(EnrollmetnInputIssueDate[i]))
                        cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = EnrollmetnInputIssueDate[i];
                    else
                        cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;

                    if (!String.IsNullOrEmpty(EnrollmetnInputExpiryDate[i]))
                        cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = EnrollmetnInputExpiryDate[i];
                    else
                        cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = DBNull.Value;
                    if (EI_CertificateFileName != null)
                    {
                        if (EI_CertificateFileName[i - 1] != null)
                        {
                            path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(EI_CertificateFileName[i - 1].FileName));
                            pathCount = 1;
                            while (System.IO.File.Exists(path))
                            {
                                pathCount++;
                                path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(EI_CertificateFileName[i - 1].FileName) + pathCount + Path.GetExtension(EI_CertificateFileName[i - 1].FileName));
                            }
                            EI_CertificateFileName[i - 1].SaveAs(path);
                            cmd.Parameters.AddWithValue("CERATTACHFILENM", path);
                        }
                        else
                            cmd.Parameters.AddWithValue("CERATTACHFILENM", "");
                    }
                    else
                        cmd.Parameters.AddWithValue("CERATTACHFILENM", "");
                    if (EI_RecommendationFileName != null)
                    {
                        if (EI_RecommendationFileName[i - 1] != null)
                        {
                            path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(EI_RecommendationFileName[i - 1].FileName));
                            pathCount = 1;
                            while (System.IO.File.Exists(path))
                            {
                                pathCount++;
                                path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(EI_RecommendationFileName[i - 1].FileName) + pathCount + Path.GetExtension(EI_RecommendationFileName[i - 1].FileName));
                            }
                            EI_RecommendationFileName[i - 1].SaveAs(path);
                            cmd.Parameters.AddWithValue("RECATTACHFILENM", path);
                        }
                        else
                            cmd.Parameters.AddWithValue("RECATTACHFILENM", "");
                    }
                    else
                        cmd.Parameters.AddWithValue("RECATTACHFILENM", "");
                    cmd.Parameters.AddWithValue("INPUTBY", user);
                    cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
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
            }
            #region Enrollment_old
            //if (!String.IsNullOrEmpty(Request["Office"]) && !String.IsNullOrEmpty(Request["EnrollmetnMEMBERSHIPREGNO"]))
            //{
            //    if (saveMode == "insert")
            //    {
            //        cmd.CommandText = "select NVL(max(ASSOENSLNO),0)+1 from ASSOCIATIONENROLL";
            //        cmd.Parameters.Clear();
            //        drMaxSlNo = cmd.ExecuteReader();
            //        drMaxSlNo.Read();

            //        cmd.CommandText = "insert into ASSOCIATIONENROLL(ASSOENSLNO,BONDERSLNO,OFFICESLNO,MEMBERSHIPREGNO,ISSUEDATE," +
            //        "EXPDATE,CERATTACHFILENM,RECATTACHFILENM,INPUTBY,INPUTDATE) values(:ASSOENSLNO,:BONDERSLNO," +
            //        ":OFFICESLNO,:MEMBERSHIPREGNO,:ISSUEDATE,:EXPDATE,:CERATTACHFILENM,:RECATTACHFILENM,:INPUTBY,:INPUTDATE)";

            //        cmd.Parameters.Clear();
            //        cmd.Parameters.AddWithValue("ASSOENSLNO", drMaxSlNo.GetInt32(0));
            //        cmd.Parameters.AddWithValue("BONDERSLNO", drMaxBondSlNo.GetInt32(0));
            //        cmd.Parameters.AddWithValue("INPUTBY", user);
            //        cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
            //    }
            //    else if (saveMode == "update")
            //    {
            //        cmd.CommandText = "select ASSOENSLNO from ASSOCIATIONENROLL where BONDERSLNO=:BONDERSLNO";
            //        cmd.Parameters.Clear();
            //        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);

            //        System.Data.OracleClient.OracleDataReader drASSOCIATIONENROLLSlNo;
            //        drASSOCIATIONENROLLSlNo = cmd.ExecuteReader();
            //        drASSOCIATIONENROLLSlNo.Read();
            //        if (!drASSOCIATIONENROLLSlNo.HasRows)
            //        {
            //            cmd.CommandText = "insert into ASSOCIATIONENROLL(BONDERSLNO,OFFICESLNO,BSNO,MEMBERSHIPREGNO,ISSUEDATE," +
            //            "EXPDATE,CERATTACHFILENM,RECATTACHFILENM,INPUTBY,INPUTDATE) values(:BONDERSLNO," +
            //            ":OFFICESLNO,:BSNO,:MEMBERSHIPREGNO,:ISSUEDATE,:EXPDATE,:CERATTACHFILENM,:RECATTACHFILENM,:INPUTBY,:INPUTDATE)";

            //            cmd.Parameters.Clear();
            //            //cmd.Parameters.AddWithValue("ASSOENSLNO", drMaxSlNo.GetInt32(0));
            //            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            //            cmd.Parameters.AddWithValue("INPUTBY", user);
            //            cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
            //        }
            //        else
            //        {
            //            cmd.CommandText = "update ASSOCIATIONENROLL set OFFICESLNO=:OFFICESLNO,MEMBERSHIPREGNO=:MEMBERSHIPREGNO," +
            //                "ISSUEDATE=:ISSUEDATE,EXPDATE=:EXPDATE,CERATTACHFILENM=:CERATTACHFILENM," +
            //                "RECATTACHFILENM=:RECATTACHFILENM,MODIFIEDBY=:MODIFIEDBY,MODIFYDATE=:MODIFYDATE" +
            //               " where ASSOENSLNO=:ASSOENSLNO and BONDERSLNO=:BONDERSLNO AND BSNO=:BSNO";
            //            cmd.Parameters.Clear();
            //            cmd.Parameters.AddWithValue("ASSOENSLNO", drASSOCIATIONENROLLSlNo.GetInt32(0));
            //            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            //            cmd.Parameters.AddWithValue("MODIFIEDBY", user);
            //            cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
            //        }
            //    }
            //    cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
            //    cmd.Parameters.AddWithValue("OFFICESLNO", Request["Office"]);
            //    cmd.Parameters.AddWithValue("MEMBERSHIPREGNO", Request["EnrollmetnMEMBERSHIPREGNO"]);
            //    if (!String.IsNullOrEmpty(Request["EnrollmetnInputIssueDate"]))

            //        cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = Request["EnrollmetnInputIssueDate"];
            //    else
            //        cmd.Parameters.Add(new OracleParameter(":ISSUEDATE", OracleType.DateTime)).Value = DBNull.Value;

            //    if (!String.IsNullOrEmpty(Request["EnrollmetnInputExpiryDate"]))

            //        cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = Request["EnrollmetnInputExpiryDate"];
            //    else
            //        cmd.Parameters.Add(new OracleParameter(":EXPDATE", OracleType.DateTime)).Value = DBNull.Value;

            //    //cmd.Parameters.AddWithValue("CERATTACHFILENM", Request["EI_CertificateUpload"]);
            //    //cmd.Parameters.AddWithValue("RECATTACHFILENM", Request["EI_RecommendationUpload"]);

            //    if (EI_CertificateUpload != null && EI_CertificateUpload.ContentLength > 0)
            //    {
            //        path = Path.Combine(Server.MapPath("~/Uploads"), Path.GetFileName(EI_CertificateUpload.FileName));
            //        pathCount = 1;
            //        while (System.IO.File.Exists(path))
            //        {
            //            pathCount++;
            //            path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(EI_CertificateUpload.FileName) + pathCount + Path.GetExtension(EI_CertificateUpload.FileName));
            //        }
            //        EI_CertificateUpload.SaveAs(path);
            //        cmd.Parameters.AddWithValue("CERATTACHFILENM", path);
            //    }
            //    else
            //    {
            //        cmd.Parameters.AddWithValue("CERATTACHFILENM", Request["EI_CertificateFileName"]);
            //    }


            //    if (EI_RecommendationUpload != null && EI_RecommendationUpload.ContentLength > 0)
            //    {
            //        path = Path.Combine(Server.MapPath("~/Uploads"), Path.GetFileName(EI_RecommendationUpload.FileName));
            //        pathCount = 1;
            //        while (System.IO.File.Exists(path))
            //        {
            //            pathCount++;
            //            path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(EI_RecommendationUpload.FileName) + pathCount + Path.GetExtension(EI_RecommendationUpload.FileName));
            //        }
            //        EI_RecommendationUpload.SaveAs(path);
            //        cmd.Parameters.AddWithValue("RECATTACHFILENM", path);
            //    }
            //    else
            //    {
            //        cmd.Parameters.AddWithValue("RECATTACHFILENM", Request["EI_RecommendationFileName"]);
            //    }


            //    try
            //    {
            //        bmsTransaction = conn.BeginTransaction();
            //        cmd.Transaction = bmsTransaction;
            //        cmd.ExecuteNonQuery();
            //        bmsTransaction.Commit();
            //    }
            //    catch
            //    {
            //        bmsTransaction.Rollback();
            //    }
            //}
            #endregion
            // Lien Bank
            if (Branch != null)
            {
                for (int i = 0; i < Branch.Length; i++)
                {
                    cmd.CommandText = "insert into BONDERLIENBANK(BONDERSLNO,BBRANCHSLNO,LIENBANKCODE,CERATTACHFILENM,INPUTBY,INPUTDATE)"
                        + " values(:BONDERSLNO,:BBRANCHSLNO,:LIENBANKCODE,:CERATTACHFILENM,:INPUTBY,:INPUTDATE)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                    cmd.Parameters.AddWithValue("BBRANCHSLNO", Branch[i]);
                    cmd.Parameters.AddWithValue("LIENBANKCODE", LBI_Code[i + 1]);
                    //if (LBI_CertificateUpload != null && LBI_CertificateUpload.ContentLength > 0)
                    //{
                    //    path = Path.Combine(Server.MapPath("~/Uploads"), Path.GetFileName(LBI_CertificateUpload.FileName));
                    //    LBI_CertificateUpload.SaveAs(path);
                    //    cmd.Parameters.AddWithValue("CERATTACHFILENM", path);
                    //}
                    if (LBI_CertificateFileName != null)
                    {
                        if (LBI_CertificateFileName[i] != null)
                        {
                            path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileName(LBI_CertificateFileName[i].FileName));
                            pathCount = 1;
                            while (System.IO.File.Exists(path))
                            {
                                pathCount++;
                                path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(LBI_CertificateFileName[i].FileName) + pathCount + Path.GetExtension(LBI_CertificateFileName[i].FileName));
                            }
                            LBI_CertificateFileName[i].SaveAs(path);
                            cmd.Parameters.AddWithValue("CERATTACHFILENM", path);
                        }
                        else
                            cmd.Parameters.AddWithValue("CERATTACHFILENM", "");
                    }
                    else
                        cmd.Parameters.AddWithValue("CERATTACHFILENM", "");
                    cmd.Parameters.AddWithValue("INPUTBY", user);
                    cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
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
            }
            #region old_Lien Bank
            //if (!String.IsNullOrEmpty(Request["Branch"]))
            //{
            //if (saveMode == "insert")
            //{
            //    cmd.CommandText = "select NVL(max(LBANKSLNO),0)+1 from BONDERLIENBANK";
            //    cmd.Parameters.Clear();
            //    drMaxSlNo = cmd.ExecuteReader();
            //    drMaxSlNo.Read();

            //    cmd.CommandText = "insert into BONDERLIENBANK(LBANKSLNO,BONDERSLNO,BANKSLNO,BBRANCHSLNO,LIENBANKCODE,CERATTACHFILENM," +
            //    "INPUTBY,INPUTDATE) values(:LBANKSLNO,:BONDERSLNO,:BANKSLNO,:BBRANCHSLNO,:LIENBANKCODE,:CERATTACHFILENM," +
            //    ":INPUTBY,:INPUTDATE)";

            //    cmd.Parameters.Clear();
            //    cmd.Parameters.AddWithValue("LBANKSLNO", drMaxSlNo.GetInt32(0));
            //    cmd.Parameters.AddWithValue("BONDERSLNO", drMaxBondSlNo.GetInt32(0));
            //    cmd.Parameters.AddWithValue("INPUTBY", user);
            //    cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
            //}
            //else if (saveMode == "update")
            //{
            //    cmd.CommandText = "select LBANKSLNO from BONDERLIENBANK where BONDERSLNO=:BONDERSLNO";
            //    cmd.Parameters.Clear();
            //    cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);

            //    System.Data.OracleClient.OracleDataReader drBONDERLIENBANKSlNo;
            //    drBONDERLIENBANKSlNo = cmd.ExecuteReader();
            //    drBONDERLIENBANKSlNo.Read();


            //    if (!drBONDERLIENBANKSlNo.HasRows)
            //    {
            //        cmd.CommandText = "insert into BONDERLIENBANK(BONDERSLNO,BANKSLNO,BBRANCHSLNO,LIENBANKCODE,CERATTACHFILENM," +
            // "INPUTBY,INPUTDATE,Address,Phone,Fax) values(:BONDERSLNO,:BANKSLNO,:BBRANCHSLNO,:LIENBANKCODE,:CERATTACHFILENM," +
            // ":INPUTBY,:INPUTDATE,:Address,:Phone,:Fax)";

            //        cmd.Parameters.Clear();
            //        //cmd.Parameters.AddWithValue("LBANKSLNO", drMaxSlNo.GetInt32(0));
            //        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            //        cmd.Parameters.AddWithValue("INPUTBY", user);
            //        cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
            //    }

            //    else
            //    {
            //        cmd.CommandText = "update BONDERLIENBANK set BANKSLNO=:BANKSLNO,BBRANCHSLNO=:BBRANCHSLNO," +
            //            "LIENBANKCODE=:LIENBANKCODE,CERATTACHFILENM=:CERATTACHFILENM,MODIFIEDBY=:MODIFIEDBY," +
            //        "MODIFYDATE=:MODIFYDATE,Address=:Address,Phone=:Phone,Fax=:Fax" +
            //        " where BONDERSLNO=:BONDERSLNO And LBANKSLNO=:LBANKSLNO";

            //        cmd.Parameters.Clear();
            //        cmd.Parameters.AddWithValue("LBANKSLNO", drBONDERLIENBANKSlNo.GetInt32(0));
            //        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            //        cmd.Parameters.AddWithValue("MODIFIEDBY", user);
            //        cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
            //    }
            //}
            //cmd.Parameters.AddWithValue("BANKSLNO", Request["Bank"]);
            //cmd.Parameters.AddWithValue("BBRANCHSLNO", Request["Branch"]);
            //cmd.Parameters.AddWithValue("LIENBANKCODE", Request["LBI_Code"]);
            ////if (!String.IsNullOrEmpty(Request["LBI_CertificateUpload"]))
            ////    cmd.Parameters.AddWithValue("CERATTACHFILENM", Request["LBI_CertificateUpload"]);
            ////else
            ////    cmd.Parameters.AddWithValue("CERATTACHFILENM", "");

            //if (LBI_CertificateUpload != null && LBI_CertificateUpload.ContentLength > 0)
            //{
            //    path = Path.Combine(Server.MapPath("~/Uploads"), Path.GetFileName(LBI_CertificateUpload.FileName));
            //    LBI_CertificateUpload.SaveAs(path);
            //    cmd.Parameters.AddWithValue("CERATTACHFILENM", path);
            //}
            //else
            //{
            //    cmd.Parameters.AddWithValue("CERATTACHFILENM", Request["LBI_CertificateFileName"]);
            //}

            //cmd.Parameters.AddWithValue("Address", Request["LBI_Address"]);
            //cmd.Parameters.AddWithValue("Phone", Request["LBI_Phone"]);
            //cmd.Parameters.AddWithValue("Fax", Request["LBI_Fax"]);

            //try
            //{
            //    bmsTransaction = conn.BeginTransaction();
            //    cmd.Transaction = bmsTransaction;
            //    cmd.ExecuteNonQuery();
            //    bmsTransaction.Commit();
            //}
            //catch
            //{
            //    bmsTransaction.Rollback();
            //}
            //}
            #endregion
            //}
            //catch (Exception)
            //{
            //    return Start(collection);
            //}
        }

        public void saveSectionB(string[] OwnerPositionDesignation, string[] OII_Name, string[] OII_FathersHusbandName, string[] OII_TIN_No,
            string[] OII_IssueDate, string[] OII_Present_Address, string[] OII_Permanent_Address, string[] OII_ContactNo,
            string[] OII_ResidencePhoneNo, string[] OII_MobileNo, string[] OII_PassportNo, string[] OwnerPassportDistrict,
            string[] OII_PassportIssueDate, string[] OII_PassportExpiryDate, string[] OII_Nationality, string[] OII_NationalID_No,
HttpPostedFileBase[] OII_SignatureUpload, HttpPostedFileBase[] OII_PhotoUpload,
            System.Data.OracleClient.OracleCommand cmd, System.Data.OracleClient.OracleConnection conn,
System.Data.OracleClient.OracleTransaction bmsTransaction, int bondSlNo)
        {
            cmd.CommandText = "update Bonder set OWNERCATEGORY=:OWNERCATEGORY,MODIFIEDBY=:MODIFIEDBY," +
                              "MODIFYDATE=:MODIFYDATE where BONDERSLNO=:BONDERSLNO";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("OWNERCATEGORY", Request["OwnershipCategory"]);
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            cmd.Parameters.AddWithValue("MODIFIEDBY", user);
            cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
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
            if (OwnerPositionDesignation != null)
            {
                for (int i = 1; i < OwnerPositionDesignation.Length; i++)
                {
                    cmd.CommandText = "insert into OWNERINFO(BONDERSLNO,DESSLNO,OWNERNAME,OWNERFHNAME,OWNERPERADDRESS,"
                    + "OWNERPRSADDRESS,OWNERCONTACTNO,OWNERRESPHONE,OWNERMOBPHONE,TINNO,TINISSDT,PASSPORTNO,"
                    + "PASSDISTRICTSLNO,PASSISSUEDT,PASSEXPDT,NATIONALITY,NATIONALZIDNO,SIGNATUREFILENM,PHOTOFILENM,"
                    + "INPUTBY,INPUTDATE)"
                        + " values(:BONDERSLNO,:DESSLNO,:OWNERNAME,:OWNERFHNAME,:OWNERPERADDRESS,"
                    + ":OWNERPRSADDRESS,:OWNERCONTACTNO,:OWNERRESPHONE,:OWNERMOBPHONE,:TINNO,:TINISSDT,:PASSPORTNO,"
                    + ":PASSDISTRICTSLNO,:PASSISSUEDT,:PASSEXPDT,:NATIONALITY,:NATIONALZIDNO,:SIGNATUREFILENM,:PHOTOFILENM,"
                    + ":INPUTBY,:INPUTDATE)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                    cmd.Parameters.AddWithValue("DESSLNO", OwnerPositionDesignation[i]);
                    cmd.Parameters.AddWithValue("OWNERNAME", OII_Name[i]);
                    cmd.Parameters.AddWithValue("OWNERFHNAME", OII_FathersHusbandName[i]);
                    cmd.Parameters.AddWithValue("OWNERPERADDRESS", OII_Permanent_Address[i]);
                    cmd.Parameters.AddWithValue("OWNERPRSADDRESS", OII_Present_Address[i]);
                    cmd.Parameters.AddWithValue("OWNERCONTACTNO", OII_ContactNo[i]);
                    cmd.Parameters.AddWithValue("OWNERRESPHONE", OII_ResidencePhoneNo[i]);
                    cmd.Parameters.AddWithValue("OWNERMOBPHONE", OII_MobileNo[i]);
                    cmd.Parameters.AddWithValue("TINNO", OII_TIN_No[i]);
                    //cmd.Parameters.AddWithValue("TINISSDT", OII_IssueDate[i]);
                     if (!String.IsNullOrEmpty(OII_IssueDate[i]))
                        cmd.Parameters.Add(new OracleParameter(":TINISSDT", OracleType.DateTime)).Value = OII_IssueDate[i];
                    else
                        cmd.Parameters.Add(new OracleParameter(":TINISSDT", OracleType.DateTime)).Value = DBNull.Value;
                    cmd.Parameters.AddWithValue("PASSPORTNO", OII_PassportNo[i]);
                    cmd.Parameters.AddWithValue("PASSDISTRICTSLNO", OwnerPassportDistrict[i]);
                    //cmd.Parameters.AddWithValue("PASSISSUEDT", OII_PassportIssueDate[i]);
                    if (!String.IsNullOrEmpty(OII_PassportIssueDate[i]))
                        cmd.Parameters.Add(new OracleParameter(":PASSISSUEDT", OracleType.DateTime)).Value = OII_PassportIssueDate[i];
                    else
                        cmd.Parameters.Add(new OracleParameter(":PASSISSUEDT", OracleType.DateTime)).Value = DBNull.Value;

                    //cmd.Parameters.AddWithValue("PASSEXPDT", OII_PassportExpiryDate[i]);
                    
                    if (!String.IsNullOrEmpty(OII_PassportExpiryDate[i]))
                        cmd.Parameters.Add(new OracleParameter(":PASSEXPDT", OracleType.DateTime)).Value = OII_PassportExpiryDate[i];
                    else
                        cmd.Parameters.Add(new OracleParameter(":PASSEXPDT", OracleType.DateTime)).Value = DBNull.Value;

                    cmd.Parameters.AddWithValue("NATIONALITY", OII_Nationality[i]);
                    cmd.Parameters.AddWithValue("NATIONALZIDNO", OII_NationalID_No[i]);
                    if (OII_SignatureUpload != null)
                    {
                        try
                        {
                            if (OII_SignatureUpload[i - 1] != null)
                            {
                                path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")),
                                    Path.GetFileName(OII_SignatureUpload[i - 1].FileName));
                                pathCount = 1;
                                while (System.IO.File.Exists(path))
                                {
                                    pathCount++;
                                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")),
                                        Path.GetFileNameWithoutExtension(OII_SignatureUpload[i - 1].FileName)
                                        + pathCount + Path.GetExtension(OII_SignatureUpload[i - 1].FileName));
                                }
                                OII_SignatureUpload[i - 1].SaveAs(path);
                                cmd.Parameters.AddWithValue("SIGNATUREFILENM", path);
                            }
                            else
                                cmd.Parameters.AddWithValue("SIGNATUREFILENM", "");
                        }
                        catch
                        {
                            cmd.Parameters.AddWithValue("SIGNATUREFILENM", "");
                        }
                    }
                    else
                        cmd.Parameters.AddWithValue("SIGNATUREFILENM", "");
                    if (OII_PhotoUpload != null)
                    {
                        try
                        {
                            if (OII_PhotoUpload[i - 1] != null)
                            {
                                path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")),
                                    Path.GetFileName(OII_PhotoUpload[i - 1].FileName));
                                pathCount = 1;
                                while (System.IO.File.Exists(path))
                                {
                                    pathCount++;
                                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")),
                                        Path.GetFileNameWithoutExtension(OII_PhotoUpload[i - 1].FileName)
                                        + pathCount + Path.GetExtension(OII_PhotoUpload[i - 1].FileName));
                                }
                                OII_PhotoUpload[i - 1].SaveAs(path);
                                cmd.Parameters.AddWithValue("PHOTOFILENM", path);
                            }
                            else
                                cmd.Parameters.AddWithValue("PHOTOFILENM", "");
                        }
                        catch
                        {
                            cmd.Parameters.AddWithValue("PHOTOFILENM", "");
                        }
                    }
                    else
                        cmd.Parameters.AddWithValue("PHOTOFILENM", "");
                    cmd.Parameters.AddWithValue("INPUTBY", user);
                    cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
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
            }
        }

        public void saveSectionC(string[] fpHsCode, string[] fpDesc, string[] regOrg,
            System.Data.OracleClient.OracleCommand cmd, System.Data.OracleClient.OracleConnection conn,
System.Data.OracleClient.OracleTransaction bmsTransaction, int bondSlNo)
        {
            for (int i = 1; i < fpHsCode.Length; i++)
            {
                cmd.CommandText = "insert into BonderProduct(BonderSlNo,FPSlNo,RegFrom,Inputby,InputDate) values"
                    + "(:BonderSlNo,:FPSlNo,:RegFrom,:Inputby,:InputDate)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                cmd.Parameters.AddWithValue("FPSlNo", fpHsCode[i]);
                cmd.Parameters.AddWithValue("RegFrom", regOrg[i]);
                cmd.Parameters.AddWithValue("INPUTBY", user);
                cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
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
            cmd.CommandText = "update Bonder set TOTFACLENG=:TOTFACLENG,TOTFACWIDTH=:TOTFACWIDTH,"
            + "TOTRMWHLENG=:TOTRMWHLENG,TOTRMWHWIDTH=:TOTRMWHWIDTH,TOTFGWHLENG=:TOTFGWHLENG,"
            + "TOTFGWHWIDTH=:TOTFGWHWIDTH,MODIFIEDBY=:MODIFIEDBY,MODIFYDATE=:MODIFYDATE where BONDERSLNO=:BONDERSLNO";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
            cmd.Parameters.AddWithValue("TOTFACLENG", Request["TofLen"]);
            cmd.Parameters.AddWithValue("TOTFACWIDTH", Request["TofWid"]);
            cmd.Parameters.AddWithValue("TOTRMWHLENG", Request["TorLen"]);
            cmd.Parameters.AddWithValue("TOTRMWHWIDTH", Request["TorWid"]);
            cmd.Parameters.AddWithValue("TOTFGWHLENG", Request["TogLen"]);
            cmd.Parameters.AddWithValue("TOTFGWHWIDTH", Request["TogWid"]);
            cmd.Parameters.AddWithValue("MODIFIEDBY", user);
            cmd.Parameters.Add(new OracleParameter(":MODIFYDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();

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

        public void saveSectionD(string[] IMI_Description, string[] IMI_Brand_Name, string[] IMI_Model_No,
            string[] IMI_Manufacturing_Year, string[] IMI_Country_of_Origin, string[] IMI_Date_of_Installation,
            string[] IMI_BE_No_or_VAT_Challan_No, string[] IMI_VAT_Date, string[] IMI_BL_No,
            string[] IMI_BL_Date, string[] IMI_Invoice_No, string[] IMI_Invoice_Date,
            string[] IMI_LC_No, string[] IMI_LC_Date, string[] IMI_No_of_Machines,
            string[] IMI_Machine_Type, string[] IMI_Procurement_Type, string[] IMI_Indemnity_Bond,
            string[] PWM_HS_Code, string[] PWM_AnnualCapacity, string[] PWM_Unit, string[] PWM_Machine, string[] PWM_Qty,
            string[] PWM_Machines_Type, string[] PWM_Machines_Qty, string[] PWM_Model_No,
            string[] ABI_Name, string[] ABI_Designation, string[] ABI_TIN_No, string[] ABI_TIN_Issue_Date, string[] ABI_NID_No,
            string[] ABI_NID_Issue_Date, string[] ABI_NameOfAssoc, string[] ABI_AddressOfAssoc, string[] ABI_BINOfAssoc,
            string[] ABI_TIN_OfAssoc, string[] ABI_NatureOfAssoc, string[] ABI_PositionOfAssoc, string[] ABI_ShareOfAssoc,
            string[] MII_Machine_Desc, string[] MII_BE_No, string[] MII_BE_Date, string[] MII_Indemn_Undertake_No,
            string[] MII_Indemn_Date, string[] MII_Cash_Challan_No, string[] MII_Cash_Challan_Date, string[] MII_Due_Date,
            string[] MII_Actual_Date,
            string[] MPO_Clear_Cer_No, string[] MPO_Clear_Date, string[] MPO_Perm, string[] MPO_Perm_Date,
            string[] MPO_Transfer_No, string[] MPO_Transfer_Date, string[] MPO_Bank_Code, string[] MPO_Bank_Name,
            string[] MPO_Branch, string[] MPO_Bank_Address,
            string[] AIL_HS_Code, string[] AIL_Desc, string[] AIL_Spec_Grade, string[] AIL_Quantity, string[] AIL_Unit,
            HttpPostedFileBase uploadAuthSign, HttpPostedFileBase uploadPhotoOfAuthSign,
            HttpPostedFileBase uploadDeclareSign, HttpPostedFileBase uploadDeclareSeal,
            System.Data.OracleClient.OracleCommand cmd, System.Data.OracleClient.OracleConnection conn,
System.Data.OracleClient.OracleTransaction bmsTransaction, int bondSlNo)
        {
            try
            {
                bmsTransaction = conn.BeginTransaction();
            }
            catch { }
            cmd.CommandText = "select MACHINESLNO from INSTALLEDMACHINEINFO where BONDERSLNO=:BONDERSLNO";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);

            System.Data.OracleClient.OracleDataReader drINSTALLEDMACHINEINFOSlNo;
            drINSTALLEDMACHINEINFOSlNo = cmd.ExecuteReader();
            drINSTALLEDMACHINEINFOSlNo.Read();

            //System.Data.OracleClient.OracleCommand cmdM = new System.Data.OracleClient.OracleCommand();
            //cmdM.Connection = conn;


            //if (!drINSTALLEDMACHINEINFOSlNo.HasRows)
            //{
            int indemnity = 0;
            if (MII_BE_No != null)
            {
                indemnity = MII_BE_No.Length;
            }
            int local = 0;
            if (MPO_Clear_Cer_No != null)
            {
                local = MPO_Clear_Cer_No.Length;
            }
            for (int i = 1; i < IMI_Brand_Name.Length; i++)
            {
                if (IMI_Indemnity_Bond[i] == "D")
                {
                    cmd.CommandText = "insert into INSTALLEDMACHINEINFO(MACHINESLNO,BONDERSLNO,DATEOFINSTALL,VATCHNO,"
                       + "VATCHNODATE,BLNO,BLNODATE,INVOICENO,INVOICENODATE,LCNO,LCNODATE,MACHINEQTY,MACHINETYPE,"
                       + "PROCUREMENTTYPE,INDEMNITYBOND,INPUTBY,INPUTDATE,DESCRIPTION,MANUFACTUREYEAR,COUNTRYSLNO,ModelSlNo)"
                       + " values(:MACHINESLNO,:BONDERSLNO,:DATEOFINSTALL,:VATCHNO,"
                       + ":VATCHNODATE,:BLNO,:BLNODATE,:INVOICENO,:INVOICENODATE,:LCNO,:LCNODATE,:MACHINEQTY,:MACHINETYPE,"
                       + ":PROCUREMENTTYPE,:INDEMNITYBOND,:INPUTBY,:INPUTDATE,:DESCRIPTION,:MANUFACTUREYEAR,:COUNTRYSLNO,:ModelSlNo)";
                    cmd.Parameters.Clear();
                }
                else if (IMI_Indemnity_Bond[i] == "I")
                {
                    cmd.CommandText = "insert into INSTALLEDMACHINEINFO(MACHINESLNO,BONDERSLNO,DATEOFINSTALL,VATCHNO,"
                                         + "VATCHNODATE,BLNO,BLNODATE,INVOICENO,INVOICENODATE,LCNO,LCNODATE,MACHINEQTY,MACHINETYPE,"
                                         + "PROCUREMENTTYPE,INDEMNITYBOND,INPUTBY,INPUTDATE,DESCRIPTION,MANUFACTUREYEAR,COUNTRYSLNO,ModelSlNo,"
                                         + "BENO,BENODATE,INDEMNITYUNDERTKNO,INDEMNITYUNDERTKDT,CASHCHALNVOUCHNO,CASHCHALNVOUCHDT,DUERELEASDT,ACTRELEASDT)"
                                         + " values(:MACHINESLNO,:BONDERSLNO,:DATEOFINSTALL,:VATCHNO,"
                                         + ":VATCHNODATE,:BLNO,:BLNODATE,:INVOICENO,:INVOICENODATE,:LCNO,:LCNODATE,:MACHINEQTY,:MACHINETYPE,"
                                         + ":PROCUREMENTTYPE,:INDEMNITYBOND,:INPUTBY,:INPUTDATE,:DESCRIPTION,:MANUFACTUREYEAR,:COUNTRYSLNO,:ModelSlNo,"
                                         + ":BENO,:BENODATE,:INDEMNITYUNDERTKNO,:INDEMNITYUNDERTKDT,:CASHCHALNVOUCHNO,:CASHCHALNVOUCHDT,:DUERELEASDT,:ACTRELEASDT)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("BENO", MII_BE_No[MII_BE_No.Length - indemnity]);
                    if (!String.IsNullOrEmpty(MII_BE_Date[MII_BE_No.Length - indemnity]))
                        cmd.Parameters.Add(new OracleParameter(":BENODATE", OracleType.DateTime)).Value = MII_BE_Date[MII_BE_No.Length - indemnity];
                    else
                        cmd.Parameters.Add(new OracleParameter(":BENODATE", OracleType.DateTime)).Value = DBNull.Value;
                    //cmd.Parameters.AddWithValue("BENODATE", MII_BE_Date[MII_BE_No.Length - indemnity]);
                    cmd.Parameters.AddWithValue("INDEMNITYUNDERTKNO", MII_Indemn_Undertake_No[MII_BE_No.Length - indemnity]);
                    if (!String.IsNullOrEmpty(MII_Indemn_Date[MII_BE_No.Length - indemnity]))
                        cmd.Parameters.Add(new OracleParameter(":INDEMNITYUNDERTKDT", OracleType.DateTime)).Value = MII_Indemn_Date[MII_BE_No.Length - indemnity];
                    else
                        cmd.Parameters.Add(new OracleParameter(":INDEMNITYUNDERTKDT", OracleType.DateTime)).Value = DBNull.Value;
                    //cmd.Parameters.AddWithValue("INDEMNITYUNDERTKDT", MII_Indemn_Date[MII_BE_No.Length - indemnity]);
                    cmd.Parameters.AddWithValue("CASHCHALNVOUCHNO", MII_Cash_Challan_No[MII_BE_No.Length - indemnity]);
                    if (!String.IsNullOrEmpty(MII_Cash_Challan_Date[MII_BE_No.Length - indemnity]))
                        cmd.Parameters.Add(new OracleParameter(":CASHCHALNVOUCHDT", OracleType.DateTime)).Value = MII_Cash_Challan_Date[MII_BE_No.Length - indemnity];
                    else
                        cmd.Parameters.Add(new OracleParameter(":CASHCHALNVOUCHDT", OracleType.DateTime)).Value = DBNull.Value;
                    //cmd.Parameters.AddWithValue("CASHCHALNVOUCHDT", MII_Cash_Challan_Date[MII_BE_No.Length - indemnity]);
                    if (!String.IsNullOrEmpty(MII_Due_Date[MII_BE_No.Length - indemnity]))
                        cmd.Parameters.Add(new OracleParameter(":DUERELEASDT", OracleType.DateTime)).Value = MII_Due_Date[MII_BE_No.Length - indemnity];
                    else
                        cmd.Parameters.Add(new OracleParameter(":DUERELEASDT", OracleType.DateTime)).Value = DBNull.Value;
                    //cmd.Parameters.AddWithValue("DUERELEASDT", MII_Due_Date[MII_BE_No.Length-indemnity]);
                    if (!String.IsNullOrEmpty(MII_Actual_Date[MII_BE_No.Length - indemnity]))
                        cmd.Parameters.Add(new OracleParameter(":ACTRELEASDT", OracleType.DateTime)).Value = MII_Actual_Date[MII_BE_No.Length - indemnity];
                    else
                        cmd.Parameters.Add(new OracleParameter(":ACTRELEASDT", OracleType.DateTime)).Value = DBNull.Value;
                    //cmd.Parameters.AddWithValue("ACTRELEASDT", MII_Actual_Date[MII_BE_No.Length-indemnity]);
                    indemnity--;
                }
                else if (IMI_Indemnity_Bond[i] == "L")
                {
                    cmd.CommandText = "insert into INSTALLEDMACHINEINFO(MACHINESLNO,BONDERSLNO,DATEOFINSTALL,VATCHNO,"
                                                            + "VATCHNODATE,BLNO,BLNODATE,INVOICENO,INVOICENODATE,LCNO,LCNODATE,MACHINEQTY,MACHINETYPE,"
                                                            + "PROCUREMENTTYPE,INDEMNITYBOND,INPUTBY,INPUTDATE,DESCRIPTION,MANUFACTUREYEAR,COUNTRYSLNO,ModelSlNo,"
                                                            + "LOCALPURCLEARENCENO,LOCALPURCLEARENCEDT,LOCALPURPERMISSIONFROMCBC,LOCALPURPERMISSIONFROMCBCDT,TRANSFERLIENBANKNOCNO,TRANSFERLIENBANKDT,TRANSFERLIENBANK)"
                                                            + " values(:MACHINESLNO,:BONDERSLNO,:DATEOFINSTALL,:VATCHNO,"
                                                            + ":VATCHNODATE,:BLNO,:BLNODATE,:INVOICENO,:INVOICENODATE,:LCNO,:LCNODATE,:MACHINEQTY,:MACHINETYPE,"
                                                            + ":PROCUREMENTTYPE,:INDEMNITYBOND,:INPUTBY,:INPUTDATE,:DESCRIPTION,:MANUFACTUREYEAR,:COUNTRYSLNO,:ModelSlNo,"
                                                            + ":LOCALPURCLEARENCENO,:LOCALPURCLEARENCEDT,:LOCALPURPERMISSIONFROMCBC,:LOCALPURPERMISSIONFROMCBCDT,:TRANSFERLIENBANKNOCNO,:TRANSFERLIENBANKDT,:TRANSFERLIENBANK)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("LOCALPURCLEARENCENO", MPO_Clear_Cer_No[MPO_Clear_Cer_No.Length - local]);
                    if (!String.IsNullOrEmpty(MPO_Clear_Date[MPO_Clear_Cer_No.Length - local]))
                        cmd.Parameters.Add(new OracleParameter(":LOCALPURCLEARENCEDT", OracleType.DateTime)).Value = MPO_Clear_Date[MPO_Clear_Cer_No.Length - local];
                    else
                        cmd.Parameters.Add(new OracleParameter(":LOCALPURCLEARENCEDT", OracleType.DateTime)).Value = DBNull.Value;
                    //cmd.Parameters.AddWithValue("LOCALPURCLEARENCEDT", MPO_Clear_Date[MPO_Clear_Cer_No.Length - local]);
                    cmd.Parameters.AddWithValue("LOCALPURPERMISSIONFROMCBC", MPO_Perm[MPO_Clear_Cer_No.Length - local]);
                    if (!String.IsNullOrEmpty(MPO_Perm_Date[MPO_Clear_Cer_No.Length - local]))
                        cmd.Parameters.Add(new OracleParameter(":LOCALPURPERMISSIONFROMCBCDT", OracleType.DateTime)).Value = MPO_Perm_Date[MPO_Clear_Cer_No.Length - local];
                    else
                        cmd.Parameters.Add(new OracleParameter(":LOCALPURPERMISSIONFROMCBCDT", OracleType.DateTime)).Value = DBNull.Value;
                    //cmd.Parameters.AddWithValue("LOCALPURPERMISSIONFROMCBCDT", MPO_Perm_Date[MPO_Clear_Cer_No.Length - local]);
                    cmd.Parameters.AddWithValue("TRANSFERLIENBANKNOCNO", MPO_Transfer_No[MPO_Clear_Cer_No.Length - local]);
                    if (!String.IsNullOrEmpty(MPO_Transfer_Date[MPO_Clear_Cer_No.Length - local]))
                        cmd.Parameters.Add(new OracleParameter(":TRANSFERLIENBANKDT", OracleType.DateTime)).Value = MPO_Transfer_Date[MPO_Clear_Cer_No.Length - local];
                    else
                        cmd.Parameters.Add(new OracleParameter(":TRANSFERLIENBANKDT", OracleType.DateTime)).Value = DBNull.Value;
                    //cmd.Parameters.AddWithValue("TRANSFERLIENBANKDT", MPO_Transfer_Date[MPO_Clear_Cer_No.Length - local]);
                    cmd.Parameters.AddWithValue("TRANSFERLIENBANK", MPO_Branch[MPO_Clear_Cer_No.Length - local]);
                    local--;
                }
                cmd.Parameters.AddWithValue("MACHINESLNO", IMI_Brand_Name[i]);
                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                //cmd.Parameters.AddWithValue("DATEOFINSTALL", IMI_Date_of_Installation[i]);
                if (!String.IsNullOrEmpty(IMI_Date_of_Installation[i]))
                    cmd.Parameters.Add(new OracleParameter(":DATEOFINSTALL", OracleType.DateTime)).Value = IMI_Date_of_Installation[i];
                else
                    cmd.Parameters.Add(new OracleParameter(":DATEOFINSTALL", OracleType.DateTime)).Value = DBNull.Value;
                //cmd.Parameters.AddWithValue("BENO", IMI_BE_No_or_VAT_Challan_No[i]);
                ////cmd.Parameters.AddWithValue("BENODATE", IMI_VAT_Date[i]);
                //if (!String.IsNullOrEmpty(IMI_VAT_Date[i]))
                //    cmd.Parameters.Add(new OracleParameter(":BENODATE", OracleType.DateTime)).Value = IMI_VAT_Date[i];
                //else
                //cmd.Parameters.Add(new OracleParameter(":BENODATE", OracleType.DateTime)).Value = DBNull.Value;
                cmd.Parameters.AddWithValue("VATCHNO", IMI_BE_No_or_VAT_Challan_No[i]);
                //cmd.Parameters.AddWithValue("VATCHNODATE", IMI_VAT_Date[i]);
                if (!String.IsNullOrEmpty(IMI_VAT_Date[i]))
                    cmd.Parameters.Add(new OracleParameter(":VATCHNODATE", OracleType.DateTime)).Value = IMI_VAT_Date[i];
                else
                    cmd.Parameters.Add(new OracleParameter(":VATCHNODATE", OracleType.DateTime)).Value = DBNull.Value;
                cmd.Parameters.AddWithValue("BLNO", IMI_BL_No[i]);
                //cmd.Parameters.AddWithValue("BLNODATE", IMI_BL_Date[i]);
                if (!String.IsNullOrEmpty(IMI_BL_Date[i]))
                    cmd.Parameters.Add(new OracleParameter(":BLNODATE", OracleType.DateTime)).Value = IMI_BL_Date[i];
                else
                    cmd.Parameters.Add(new OracleParameter(":BLNODATE", OracleType.DateTime)).Value = DBNull.Value;
                cmd.Parameters.AddWithValue("INVOICENO", IMI_Invoice_No[i]);
                //cmd.Parameters.AddWithValue("INVOICENODATE", IMI_Invoice_Date[i]);
                if (!String.IsNullOrEmpty(IMI_Invoice_Date[i]))
                    cmd.Parameters.Add(new OracleParameter(":INVOICENODATE", OracleType.DateTime)).Value = IMI_Invoice_Date[i];
                else
                    cmd.Parameters.Add(new OracleParameter(":INVOICENODATE", OracleType.DateTime)).Value = DBNull.Value;
                cmd.Parameters.AddWithValue("LCNO", IMI_LC_No[i]);
                //cmd.Parameters.AddWithValue("LCNODATE", IMI_LC_Date[i]);
                if (!String.IsNullOrEmpty(IMI_LC_Date[i]))
                    cmd.Parameters.Add(new OracleParameter(":LCNODATE", OracleType.DateTime)).Value = IMI_LC_Date[i];
                else
                    cmd.Parameters.Add(new OracleParameter(":LCNODATE", OracleType.DateTime)).Value = DBNull.Value;
                cmd.Parameters.AddWithValue("MACHINEQTY", IMI_No_of_Machines[i]);
                cmd.Parameters.AddWithValue("MACHINETYPE", IMI_Machine_Type[i]);
                cmd.Parameters.AddWithValue("PROCUREMENTTYPE", IMI_Procurement_Type[i]);
                cmd.Parameters.AddWithValue("INDEMNITYBOND", IMI_Indemnity_Bond[i]);
                cmd.Parameters.AddWithValue("INPUTBY", user);
                cmd.Parameters.Add(new OracleParameter(":INPUTDATE", OracleType.DateTime)).Value = DateTime.Today.ToShortDateString();
                cmd.Parameters.AddWithValue("DESCRIPTION", IMI_Description[i]);
                cmd.Parameters.AddWithValue("MANUFACTUREYEAR", IMI_Manufacturing_Year[i]);
                cmd.Parameters.AddWithValue("COUNTRYSLNO", IMI_Country_of_Origin[i]);
                cmd.Parameters.AddWithValue("ModelSlNo", IMI_Model_No[i]);
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

            for (int i = 1; i < PWM_HS_Code.Length; i++)
            {
                cmd.CommandText = "insert into PRODCAPACITYMACHINEWISEM(BONDERSLNO,MHSCODE,ANNUALPRODUCTION,MUSLNO,MACHINESLNO,MACHINEQTY) values"
                    + "(:BONDERSLNO,:MHSCODE,:ANNUALPRODUCTION,:MUSLNO,:MACHINESLNO,:MACHINEQTY)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                cmd.Parameters.AddWithValue("MHSCODE", PWM_HS_Code[i]);
                cmd.Parameters.AddWithValue("ANNUALPRODUCTION", PWM_AnnualCapacity[i]);
                cmd.Parameters.AddWithValue("MUSLNO", PWM_Unit[i]);
                cmd.Parameters.AddWithValue("MACHINESLNO", PWM_Machine[i - 1]);
                cmd.Parameters.AddWithValue("MACHINEQTY", PWM_Qty[i]);
                try
                {
                    try
                    {
                        bmsTransaction = conn.BeginTransaction();
                    }
                    catch { }
                    cmd.Transaction = bmsTransaction;
                    try
                    {
                        cmd.ExecuteNonQuery();
                        bmsTransaction.Commit();
                    }
                    catch
                    {
                        bmsTransaction.Rollback();
                    }
                    //cmd.CommandText = "insert into PRODCAPACITYMACHINEWISEDET(FPSLNO,BONDERSLNO,MACHINESLNO,MachineType,MACHINEQTY) values"
                    //    + "((select max(FPSLNO) from PRODCAPACITYMACHINEWISEM),:BONDERSLNO,:MACHINESLNO,:MachineType,:MACHINEQTY)";
                    //cmd.Parameters.Clear();
                    //cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                    //cmd.Parameters.AddWithValue("MACHINESLNO", PWM_Machine[i-1]);
                    //cmd.Parameters.AddWithValue("MachineType", 'M');
                    //cmd.Parameters.AddWithValue("MACHINEQTY", PWM_Qty[i]);
                    //try
                    //{
                    //    try
                    //    {
                    //        bmsTransaction = conn.BeginTransaction();
                    //    }
                    //    catch { }
                    //    cmd.Transaction = bmsTransaction;
                    //    cmd.ExecuteNonQuery();
                    //    bmsTransaction.Commit();
                    //}
                    //catch
                    //{
                    //    bmsTransaction.Rollback();
                    //}
                    for (int j = 0; j < PWM_Model_No.Length - 1; j++)
                    {
                        cmd.CommandText = "insert into PRODCAPACITYMACHINEWISEDET(FPSLNO,BONDERSLNO,MACHINESLNO,MachineType,MACHINEQTY) values"
        + "((select max(FPSLNO) from PRODCAPACITYMACHINEWISEM),:BONDERSLNO,:MACHINESLNO,:MachineType,:MACHINEQTY)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                        cmd.Parameters.AddWithValue("MACHINESLNO", PWM_Model_No[j + 1]);
                        cmd.Parameters.AddWithValue("MachineType", PWM_Machines_Type[j]);
                        cmd.Parameters.AddWithValue("MACHINEQTY", PWM_Machines_Qty[j]);
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
                }
                catch
                {
                    bmsTransaction.Rollback();
                }
            }

            for (int i = 1; i < AIL_HS_Code.Length; i++)
            {
                cmd.CommandText = "insert into ANNUALENTLRAWMATERIAL(BONDERSLNO,MSLNO,MUSLNO,AEQTY,AIL_HSCODE,SPEC_GRADE)"
                    + " values(:BONDERSLNO,(select MSLNO from MATERIALS where MHSCODE=:AIL_HSCODE),:MUSLNO,:AEQTY,:AIL_HSCODE,:SPEC_GRADE)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                cmd.Parameters.AddWithValue("MUSLNO", AIL_Unit[i]);
                cmd.Parameters.AddWithValue("AEQTY", AIL_Quantity[i - 1]);
                cmd.Parameters.AddWithValue("AIL_HSCODE", AIL_HS_Code[i]);
                cmd.Parameters.AddWithValue("SPEC_GRADE", AIL_Spec_Grade[i - 1]);
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

            for (int i = 1; i < ABI_Name.Length; i++)
            {
                cmd.CommandText = "insert into OWNERINFO(BONDERSLNO,DESSLNO,OWNERNAME,TINNO,TINISSDT,NATIONALZIDNO)"
                    + " values(:BONDERSLNO,:DESSLNO,:OWNERNAME,:TINNO,TO_DATE(:TINISSDT,'YYYY-MM-DD'),:NATIONALZIDNO)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                cmd.Parameters.AddWithValue("DESSLNO", ABI_Designation[i]);
                cmd.Parameters.AddWithValue("OWNERNAME", ABI_Name[i]);
                cmd.Parameters.AddWithValue("TINNO", ABI_TIN_No[i]);
                if (!String.IsNullOrEmpty(ABI_TIN_Issue_Date[i]))
                    cmd.Parameters.Add(new OracleParameter(":TINISSDT", OracleType.DateTime)).Value = ABI_TIN_Issue_Date[i];
                else
                    cmd.Parameters.Add(new OracleParameter(":TINISSDT", OracleType.DateTime)).Value = DBNull.Value;
                //cmd.Parameters.AddWithValue("TINISSDT", ABI_TIN_Issue_Date[i]);
                cmd.Parameters.AddWithValue("NATIONALZIDNO", ABI_NID_No[i]);
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
                    cmd.CommandText = "insert into OWNERASSOCIATEBUSINESS(OWNERSLNO,ownerDesSlNo,OWNERNAME,ownerTINNO,ownerTINISSDT,"
                    + "ownerNATIONALZIDNO,BONDERSLNO,ASSOBUSINESSNAME,"
                    + "ASSOBUSINESSADDRESS,TINASSOBUSINESS,BINASSOBUSINESS,BUSINESSNATURE,BUSINESSPOS,BUSINESSSHARE)"
                    + " values((select max(OWNERSLNO) from OWNERINFO),:ownerDesSlNo,:OWNERNAME,:ownerTINNO,:ownerTINISSDT,"
                    + ":ownerNATIONALZIDNO,:BONDERSLNO,:ASSOBUSINESSNAME,"
                    + ":ASSOBUSINESSADDRESS,:TINASSOBUSINESS,:BINASSOBUSINESS,:BUSINESSNATURE,:BUSINESSPOS,:BUSINESSSHARE)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("ownerDesSlNo", ABI_Designation[i]);
                    cmd.Parameters.AddWithValue("OWNERNAME", ABI_Name[i]);
                    cmd.Parameters.AddWithValue("ownerTINNO", ABI_TIN_No[i]);
                    if (!String.IsNullOrEmpty(ABI_TIN_Issue_Date[i]))
                        cmd.Parameters.Add(new OracleParameter(":ownerTINISSDT", OracleType.DateTime)).Value = ABI_TIN_Issue_Date[i];
                    else
                        cmd.Parameters.Add(new OracleParameter(":ownerTINISSDT", OracleType.DateTime)).Value = DBNull.Value;
                    //cmd.Parameters.AddWithValue("ownerTINISSDT", ABI_TIN_Issue_Date[i]);
                    cmd.Parameters.AddWithValue("ownerNATIONALZIDNO", ABI_NID_No[i]);
                    cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                    cmd.Parameters.AddWithValue("ASSOBUSINESSNAME", ABI_NameOfAssoc[i]);
                    cmd.Parameters.AddWithValue("ASSOBUSINESSADDRESS", ABI_AddressOfAssoc[i]);
                    cmd.Parameters.AddWithValue("TINASSOBUSINESS", ABI_TIN_OfAssoc[i]);
                    cmd.Parameters.AddWithValue("BINASSOBUSINESS", ABI_BINOfAssoc[i]);
                    cmd.Parameters.AddWithValue("BUSINESSNATURE", ABI_NatureOfAssoc[i]);
                    cmd.Parameters.AddWithValue("BUSINESSPOS", ABI_PositionOfAssoc[i]);
                    cmd.Parameters.AddWithValue("BUSINESSSHARE", ABI_ShareOfAssoc[i]);
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
                catch
                {
                    bmsTransaction.Rollback();
                }
            }

            cmd.CommandText = "update Bonder set AUTHORIZEDSIGNAME=:AUTHORIZEDSIGNAME,"
            + "INITIALAUTHSIGFILENM=:INITIALAUTHSIGFILENM,PHOTOFILENM=:PHOTOFILENM,DECLARNMBLOCK=:DECLARNMBLOCK,"
            + "DECLARSIGFILENM=:DECLARSIGFILENM,DECLARSEALFILENM=:DECLARSEALFILENM,DECLARDESSLNO=:DECLARDESSLNO,DECLARDATE=:DECLARDATE"
            + " where BONDERSLNO=:BONDERSLNO";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("AUTHORIZEDSIGNAME", Request["nameOfAuthSign"]);
            if (uploadAuthSign != null && uploadAuthSign.ContentLength > 0)
            {
                // extract only the fielname
                var fileName = Path.GetFileName(uploadAuthSign.FileName);
                // store the file inside ~/App_Data/uploads folder
                path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                pathCount = 1;
                while (System.IO.File.Exists(path))
                {
                    pathCount++;
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(uploadAuthSign.FileName) + pathCount + Path.GetExtension(uploadAuthSign.FileName));
                }
                uploadAuthSign.SaveAs(path);
                cmd.Parameters.AddWithValue("INITIALAUTHSIGFILENM", path);
            }
            else
            {
                cmd.Parameters.AddWithValue("INITIALAUTHSIGFILENM", Request["SignFileName"]);
            }
            if (uploadPhotoOfAuthSign != null && uploadPhotoOfAuthSign.ContentLength > 0)
            {
                // extract only the fielname
                var fileName = Path.GetFileName(uploadPhotoOfAuthSign.FileName);
                // store the file inside ~/App_Data/uploads folder
                path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                pathCount = 1;
                while (System.IO.File.Exists(path))
                {
                    pathCount++;
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(uploadPhotoOfAuthSign.FileName) + pathCount + Path.GetExtension(uploadPhotoOfAuthSign.FileName));
                }
                uploadPhotoOfAuthSign.SaveAs(path);
                cmd.Parameters.AddWithValue("PHOTOFILENM", path);
            }
            else
            {
                cmd.Parameters.AddWithValue("PHOTOFILENM", Request["PhotoFileName"]);
            }
            cmd.Parameters.AddWithValue("DECLARNMBLOCK", Request["DeclarationName"]);
            if (uploadDeclareSign != null && uploadDeclareSign.ContentLength > 0)
            {
                // extract only the fielname
                var fileName = Path.GetFileName(uploadDeclareSign.FileName);
                // store the file inside ~/App_Data/uploads folder
                path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                pathCount = 1;
                while (System.IO.File.Exists(path))
                {
                    pathCount++;
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(uploadDeclareSign.FileName) + pathCount + Path.GetExtension(uploadDeclareSign.FileName));
                }
                uploadDeclareSign.SaveAs(path);
                cmd.Parameters.AddWithValue("DECLARSIGFILENM", path);
            }
            else
            {
                cmd.Parameters.AddWithValue("DECLARSIGFILENM", Request["DeclareSignFileName"]);
            }
            if (uploadDeclareSeal != null && uploadDeclareSeal.ContentLength > 0)
            {
                // extract only the fielname
                var fileName = Path.GetFileName(uploadDeclareSeal.FileName);
                // store the file inside ~/App_Data/uploads folder
                path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                pathCount = 1;
                while (System.IO.File.Exists(path))
                {
                    pathCount++;
                    path = Path.Combine(Server.MapPath(Url.Content("~/Uploads/")), Path.GetFileNameWithoutExtension(uploadDeclareSeal.FileName) + pathCount + Path.GetExtension(uploadDeclareSeal.FileName));
                }
                uploadDeclareSeal.SaveAs(path);
                cmd.Parameters.AddWithValue("DECLARSEALFILENM", path);
            }
            else
            {
                cmd.Parameters.AddWithValue("DECLARSEALFILENM", Request["DeclareSealFileName"]);
            }
            cmd.Parameters.AddWithValue("DECLARDESSLNO", Request["Declare_Designation"]);
            if (!String.IsNullOrEmpty(Request["DeclarationDate"]))
                cmd.Parameters.Add(new OracleParameter(":DECLARDATE", OracleType.DateTime)).Value = Request["DeclarationDate"];
            else
                cmd.Parameters.Add(new OracleParameter(":DECLARDATE", OracleType.DateTime)).Value = DBNull.Value;
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
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
            cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
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
                cmd.Parameters.AddWithValue("BONDERSLNO", BondInfo.bondSlNoToEdit);
                cmd.Parameters.AddWithValue("BSNO", BondInfo.BSNoToEdit);
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
        }

        //
        // POST: /BondRegistration/Create


        #region create_old
        //public ActionResult Create(BondRegistration model)
        //{
        //    try
        //    {
        //        System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();
        //        string oradb = "Data Source=192.168.2.8:1522/XE; User ID=BMS;Password=BMS;";
        //        System.Data.OracleClient.OracleConnection conn = new System.Data.OracleClient.OracleConnection(oradb); // C#
        //        try
        //        {
        //            conn.Open();                   

        //        }
        //        catch (Exception ex)
        //        {                  
        //        }
        //        cmd.Connection = conn;
        //        System.Data.OracleClient.OracleTransaction oracleTransaction = conn.BeginTransaction();
        //        cmd.Transaction = oracleTransaction;
        //        cmd.CommandText = "insert into BondRegistration_(RgSlNo,BonderSlNo,RegType,BondLicenseNo,BondType," +
        //        "BondTypeBg,BondCircle,BondCircleBg,IssueDate,ExpiryDate,RenewalDate,InputBy," +
        //        "InputDate,ModifiedBy,ModifyDate) values('" +
        //            model.RgSlNO + "','" + model.BonderSlNo + "','" + model.RegType + "','" + model.BondLicenseNo + "','" + model.BondType
        //            + "','" + model.BondTypeBg + "','" + model.BondCircle + "','" + model.BondCircleBg + "',SYSDATE"
        //            + ",SYSDATE,SYSDATE,'" + model.Inputby + "',SYSDATE"
        //            + ",'" + model.ModifiedBy + "',SYSDATE)";
        //        cmd.CommandType = CommandType.Text;
        //        try
        //        {
        //            cmd.ExecuteNonQuery();
        //            oracleTransaction.Commit();
        //        }
        //        catch
        //        {
        //            oracleTransaction.Rollback();
        //        }
        //        finally
        //        {
        //            conn.Close();
        //            conn.Dispose();
        //        }
        //        var value1 = model.RgSlNO;
        //        var value2 = model.BonderSlNo;

        //        return RedirectToAction("Index","BondLicense","BondLicense");                
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //
        #endregion
        // GET: /BondRegistration/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /BondRegistration/Edit/5

        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            return View();
        }

        //
        // GET: /BondRegistration/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /BondRegistration/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //[AcceptVerbs(HttpVerbs.Delete)]        
        //[AcceptVerbs(HttpVerbs.Post)]
        //[HttpPost]

        public ActionResult DeleteFile(string tableName, string columnName, string filePath, FormCollection collection)
        {
            System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();

            System.Data.OracleClient.OracleConnection conn = ConnectBMS.Connection();

            System.Data.OracleClient.OracleTransaction bmsTransaction;
            bmsTransaction = conn.BeginTransaction();
            try
            {
                cmd.Connection = conn;
                cmd.Transaction = bmsTransaction;
                cmd.CommandText = "update " + tableName + " set " + columnName + "='' where " + columnName + "='" + filePath + "'";
                cmd.Parameters.Clear();
                cmd.ExecuteNonQuery();
                bmsTransaction.Commit();
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                //return "Deleted";
            }
            catch (Exception ex)
            {
                bmsTransaction.Rollback();
                //return "Cannot Delete" + ex.Message;
            }
            return Start(collection);
        }

        public ActionResult DeleteByID(string tableName, string columnName, string id, FormCollection collection)
        {
            //if (tableName == "PRODCAPACITYMACHINEWISEM")
            //    DeleteByID("PRODCAPACITYMACHINEWISEDET", columnName,id,collection);

            System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();
            System.Data.OracleClient.OracleConnection conn = ConnectBMS.Connection();
            System.Data.OracleClient.OracleTransaction bmsTransaction;
            bmsTransaction = conn.BeginTransaction();
            try
            {
                cmd.Connection = conn;
                cmd.Transaction = bmsTransaction;
                int n;
                cmd.CommandText = "delete from " + tableName + " where " + columnName + "=:id";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("id", id);
                cmd.ExecuteNonQuery();
                bmsTransaction.Commit();
                //return "Deleted";
            }
            catch (Exception ex)
            {
                bmsTransaction.Rollback();
                //return "Cannot Delete" + ex.Message;
            }
            return Start(collection);
        }

        public ActionResult CascadeDeleteByID(string tableName, string columnName, string id, string viewName, FormCollection collection)
        {
            //if (tableName == "PRODCAPACITYMACHINEWISEM")
            //    DeleteByID("PRODCAPACITYMACHINEWISEDET", columnName,id,collection);

            System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();
            System.Data.OracleClient.OracleConnection conn = ConnectBMS.Connection();
            System.Data.OracleClient.OracleTransaction bmsTransaction;
            bmsTransaction = conn.BeginTransaction();
            try
            {
                cmd.Connection = conn;
                cmd.Transaction = bmsTransaction;
                int n;
                //cmd.CommandText = "exec delete_cascade('BMS_N','" + tableName + "', 'where " + columnName + "=" + id + "')";
                cmd.CommandText = "delete_cascade";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("table_owner", "BMS_N");
                cmd.Parameters.AddWithValue("parent_table", tableName);
                cmd.Parameters.AddWithValue("where_clause", "where " + columnName + "=" + id);
                //cmd.Parameters.AddWithValue("id", id);
                cmd.ExecuteNonQuery();
                bmsTransaction.Commit();
                //return "Deleted";
            }
            catch (Exception ex)
            {
                bmsTransaction.Rollback();
                //return "Cannot Delete" + ex.Message;
            }
            if (viewName == "ApplicantsList")
            {
                RegistrationViewModel registrationviewmodel = new RegistrationViewModel();
                registrationviewmodel.bonder = db.BONDERs.Where(y => y.BONDSTATUS == "A").ToList();
                List<RegistrationViewModel> viewModelList = new List<RegistrationViewModel>();

                viewModelList.Add(registrationviewmodel);
                return View("ApplicantsList", viewModelList);
            }
            //    return View("ApplicantsList");
            else
                return View("BondersList");
        }

        public ActionResult DeleteBonder(int bonderSlNo)
        {
            System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();
            System.Data.OracleClient.OracleConnection conn = ConnectBMS.Connection();
            System.Data.OracleClient.OracleTransaction bmsTransaction;
            bmsTransaction = conn.BeginTransaction();
            try
            {
                cmd.Connection = conn;
                cmd.Transaction = bmsTransaction;

                cmd.CommandText = "delete from BONDSTATUS where BONDERSLNO=:BONDERSLNO";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("BONDERSLNO", bonderSlNo);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "delete from BONDER where BONDERSLNO=:BONDERSLNO";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("BONDERSLNO", bonderSlNo);
                cmd.ExecuteNonQuery();

                bmsTransaction.Commit();
                //return "Deleted";
            }
            catch (Exception ex)
            {
                bmsTransaction.Rollback();
                //return "Cannot Delete" + ex.Message;
            }
            return ApplicantsList();
        }


        //public ActionResult DeleteMachineByID(int id, FormCollection collection)
        //{
        //string result = DeleteByID("INSTALLEDMACHINEINFO","MACHINESLNO", id, collection);

        //if (result == "Deleted")
        //{
        //}
        #region old_delete
        //System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();

        //    System.Data.OracleClient.OracleConnection conn = ConnectBMS.Connection();

        //    System.Data.OracleClient.OracleTransaction bmsTransaction;
        //    bmsTransaction = conn.BeginTransaction();
        //    try
        //    {
        //        cmd.Connection = conn;
        //        //cmd.Connection = ConnectBMS.Connection();                   
        //        cmd.Transaction = bmsTransaction;
        //        cmd.CommandText = "delete from INSTALLEDMACHINEINFO where MACHINESLNO=:MACHINESLNO";
        //        cmd.Parameters.Clear();
        //        cmd.Parameters.AddWithValue("MACHINESLNO",id);
        //        cmd.ExecuteNonQuery();
        //        bmsTransaction.Commit();
        //    }
        //    catch
        //    {
        //        bmsTransaction.Rollback();
        //    }
        #endregion
        //return this.Content(" ");      
        //var inst = db.INSTALLEDMACHINEINFOes.Where(x => x.BONDERSLNO == BondInfo.bondSlNoToEdit).ToList();
        //return View("Edit", inst);         
        //return Start(collection);
        //}

        //public ActionResult DeleteByID(int id)
        //{

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult SubmitAction(BondRegistration model)
        //{
        //    try
        //    {
        //        System.Data.OracleClient.OracleCommand cmd = new System.Data.OracleClient.OracleCommand();
        //        string oradb = "Data Source=192.168.2.8:1522/XE; User ID=BMS;Password=BMS;";
        //        System.Data.OracleClient.OracleConnection conn = new System.Data.OracleClient.OracleConnection(oradb); // C#
        //        try
        //        {
        //            conn.Open();

        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //        cmd.Connection = conn;
        //        cmd.CommandText = "insert into BondRegistration_(RgSlNo,BonderSlNo,RegType,BondLicenseNo,BondType," +
        //        "BondTypeBg,BondCircle,BondCircleBg,IssueDate,ExpiryDate,RenewalDate,InputBy," +
        //        "InputDate,ModifiedBy,ModifyDate) values('" +
        //            model.RgSlNO + "','" + model.BonderSlNo + "','" + model.RegType + "','" + model.BondLicenseNo + "','" + model.BondType
        //            + "','" + model.BondTypeBg + "','" + model.BondCircle + "','" + model.BondCircleBg + "',SYSDATE"
        //            + ",SYSDATE,SYSDATE,'" + model.Inputby + "',SYSDATE"
        //            + ",'" + model.ModifiedBy + "',SYSDATE)";
        //        cmd.Parameters.Clear();
        //        cmd.CommandType = CommandType.Text;
        //        int result = cmd.ExecuteNonQuery();
        //        var value1 = model.RgSlNO;
        //        var value2 = model.BonderSlNo;
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write(ex.Message);
        //        //throw;
        //    }
        //    return View();
        //}

    }
}

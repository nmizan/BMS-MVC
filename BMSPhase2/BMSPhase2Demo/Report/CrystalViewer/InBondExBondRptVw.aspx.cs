using BMSPhase2Demo.CustomDataSet;
using BMSPhase2Demo.Models;
using BMSPhase2Demo.Utils;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BMSPhase2Demo.Report.CrystalViewer
{
    public partial class InBondExBondRptVw : System.Web.UI.Page
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {

            Int32 bonderId = (Int32)Session[AppConstants.SearchBonderID];
            System.DateTime searchDateFrom = (System.DateTime)Session[AppConstants.SearchFromDate];
            System.DateTime searchDateTo = (System.DateTime)Session[AppConstants.SearchToDate];

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

                               + " WHERE INBOND.BONDERID = " + bonderId + " and INBOND.CREATEDDATE BETWEEN TO_DATE('" + searchDateFrom + "', '" + AppConstants.DATE_FORMATE + "')"
                               + "     AND TO_DATE('" + searchDateTo + "', '" + AppConstants.DATE_FORMATE + "')"
                               + "     ORDER BY MATERIALS.MATERIALNAME, INBOND.CREATEDDATE";

            reportData = db.Database.SqlQuery<InBondExBondMaterial>(queryStr1).ToList();

            List<InBondExBondMaterial> finalData = new List<InBondExBondMaterial>();
            //Update data based on response
            for (int i = 0; i < reportData.Count(); i++)
            {
                InBondExBondMaterial ibebm = reportData.ElementAt(i);
                //Adding fromdate and todate value to show in report

                ibebm.DateFrom = searchDateFrom;
                ibebm.DateTo = searchDateTo;
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

            //return finalData;

            //Clear session data
            //Session[AppConstants.SearchBonderID] = null;
            //Session[AppConstants.SearchFromDate] = null;
            //Session[AppConstants.SearchToDate] = null;

            //Load data in report
            
            //Prepare report
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Report/Crystal"), "InBondExBondRawMaterialStatus_2.rpt"));

            //Set datasource
            InBondExBondRawMaterial_01 ibrd = new InBondExBondRawMaterial_01();
            DataTable inbondTb = ibrd.Tables[ibrd.Tables.IndexOf("InBondExBondRawMaterial")];

            DataRow dr;
            foreach (object ib in finalData)
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

            InBondExBondRptVwr.ReportSource = rd;
            InBondExBondRptVwr.DataBind();

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
                                  + "   AND EXBOND.CREATEDDATE " + checkStr + " TO_DATE('" + lastDate + "', '" + AppConstants.DATE_FORMATE + "')";

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
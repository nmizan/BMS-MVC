using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSPhase2Demo.Controllers
{
    public class ReportController : Controller
    {
        public object ParameterParse(ParameterValueKind type, string value)
        {
            if (String.IsNullOrEmpty(value))
                return null;

            switch (type)
            {
                case ParameterValueKind.NumberParameter:
                    return Convert.ToInt32(value);
                case ParameterValueKind.CurrencyParameter:
                    return Convert.ToDecimal(value);
                case ParameterValueKind.BooleanParameter:
                    return Convert.ToBoolean(value);
                case ParameterValueKind.DateParameter:
                case ParameterValueKind.DateTimeParameter:
                    return Convert.ToDateTime(value);
                case ParameterValueKind.StringParameter:
                    return Convert.ToString(value);
                case ParameterValueKind.TimeParameter:
                    TimeSpan time;
                    return TimeSpan.TryParse(value, out time) ? time : (object)null;
            }
            return null;
        }
        public Stream getReportStream(string reportPath, string[] paramName = null, object[] paramVal = null)
        {
            ReportDocument rpt = new ReportDocument();
            rpt.Load(Server.MapPath(reportPath));

            string server = CommonAppSet.ConnInfo.server;


            //if (paramName != null && paramVal != null)
            //    server = server.Split('/')[1];

            rpt.SetDatabaseLogon(CommonAppSet.ConnInfo.userId, CommonAppSet.ConnInfo.pass, server, "");



            ConnectionInfo connectInfo = new ConnectionInfo()
            {
                ServerName = server,
                DatabaseName = "",
                UserID = CommonAppSet.ConnInfo.userId,
                Password = CommonAppSet.ConnInfo.pass
            };


            foreach (Table tbl in rpt.Database.Tables)
            {
                tbl.LogOnInfo.ConnectionInfo = connectInfo;
                tbl.ApplyLogOnInfo(tbl.LogOnInfo);
            }
            for (int i = 0; i < rpt.Subreports.Count; i++)
            {
                rpt.Subreports[i].SetDatabaseLogon(CommonAppSet.ConnInfo.userId, CommonAppSet.ConnInfo.pass,
                    server, "");
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
            #region commentedGetReportStream
            //rpt.SetDatabaseLogon(CommonAppSet.ConnInfo.userId, CommonAppSet.ConnInfo.pass);
            //Sections crSections = rpt.ReportDefinition.Sections;
            //ReportObjects crReportObjects;
            //SubreportObject crSubreportObject;
            //ReportDocument subRepDoc;
            //Database crDatabase;
            //Tables crTables;
            //TableLogOnInfo crTableLogOnInfo;
            //foreach (Section crSection in crSections)
            //{
            //    crReportObjects = crSection.ReportObjects;
            //    foreach (ReportObject crReportObject in crReportObjects)
            //    {
            //        if (crReportObject.Kind == ReportObjectKind.SubreportObject)
            //        {
            //            crSubreportObject = (SubreportObject)Convert.ChangeType(crReportObject, typeof(SubreportObject));

            //            subRepDoc = crSubreportObject.OpenSubreport(crSubreportObject.SubreportName);
            //            crDatabase = subRepDoc.Database;
            //            crTables = crDatabase.Tables;
            //            foreach (Table crTable in crTables)
            //            {
            //                crTableLogOnInfo = crTable.LogOnInfo;
            //                crTableLogOnInfo.ConnectionInfo = connectInfo;
            //                crTable.ApplyLogOnInfo(crTableLogOnInfo);
            //                crTable.Location = crTable.Name;
            //            }
            //        }
            //    }
            //}


            //if (paramName != null && paramVal != null)
            //{
            //    ParameterDiscreteValue crParameterDiscreteValue;
            //    ParameterFieldDefinitions crParameterFieldDefinitions;
            //    ParameterFieldDefinition crParameterFieldLocation;
            //    ParameterValues crParameterValues;

            //    for (int i = 0; i < paramName.Length; i++)
            //    {
            //        crParameterFieldDefinitions = rpt.DataDefinition.ParameterFields;
            //        crParameterFieldLocation = (ParameterFieldDefinition)crParameterFieldDefinitions[i];
            //        crParameterValues = crParameterFieldLocation.CurrentValues;
            //        crParameterDiscreteValue = new CrystalDecisions.Shared.ParameterDiscreteValue();
            //        crParameterDiscreteValue.Description = paramName[i];
            //        crParameterDiscreteValue.Value = ParameterParse(rpt.ParameterFields[i].ParameterValueType, paramVal[i].ToString());
            //        crParameterValues.Add(crParameterDiscreteValue);

            //        crParameterFieldLocation.ApplyCurrentValues(crParameterValues);

            //        //rpt.SetParameterValue(paramName[i], ParameterParse(rpt.ParameterFields[paramName[i]].ParameterValueType,
            //        //    paramVal[i].ToString()));
            //    }
            //}
            //rpt.Refresh();
            //rpt.SetParameterValue(paramName[0],paramVal[0]);
            #endregion
        }
        public ActionResult BondApplicants()
        {
            return File(getReportStream("~/Report/BondApplicants.rpt"), "application/pdf");
        }

        public ActionResult Bonders()
        {
            return File(getReportStream("~/Report/Bonders.rpt"), "application/pdf");
        }

        public ActionResult BGAPMEAMembers()
        {
            return File(getReportStream("~/Report/BGAPMEABonders.rpt"), "application/pdf");
        }
        public ActionResult BondEntitlementList()
        {
            return File(getReportStream("~/Report/BondEntitlementList.rpt"), "application/pdf");
        }
        public ActionResult BonderEntitlementDetails(int id)
        {
            return File(getReportStream("~/Report/BonderEntitlementDetails.rpt", new string[] { "ParamAESLNO" }, new object[] { id }), "application/pdf");
        }

        public ActionResult BonderInfo(int id)
        {
            return File(getReportStream("~/Report/ApplicantsCopy.rpt", new string[] { "ParamBonderSlNo" }, new object[] { id }),
                "application/pdf");
            //return File(getReportStream("~/Reports/tstWithParam.rpt", new string[] { "ParamBonderSlNo" }, new object[] { id }),
            //   "application/pdf");
            #region commentedBonderInfo
            //Reports.ApplicantsCopy rpt = new Reports.ApplicantsCopy();
            //rpt.SetParameterValue("ParamBonderSlNo", id);
            //rpt.SetDatabaseLogon(CommonAppSet.ConnInfo.userId, CommonAppSet.ConnInfo.pass, CommonAppSet.ConnInfo.server.Split('/')[1], "");
            //ConnectionInfo connectInfo = new ConnectionInfo()
            //{
            //    ServerName = CommonAppSet.ConnInfo.server.Split('/')[1],                
            //    DatabaseName = "",
            //    UserID = CommonAppSet.ConnInfo.userId,
            //    Password = CommonAppSet.ConnInfo.pass
            //};
            //foreach (Table tbl in rpt.Database.Tables)
            //{
            //    tbl.LogOnInfo.ConnectionInfo = connectInfo;
            //    tbl.ApplyLogOnInfo(tbl.LogOnInfo);
            //}
            //for (int i = 0; i < rpt.Subreports.Count; i++)
            //{
            //    rpt.Subreports[i].SetDatabaseLogon(CommonAppSet.ConnInfo.userId, CommonAppSet.ConnInfo.pass,
            //        CommonAppSet.ConnInfo.server.Split('/')[1], "");                
            //    foreach (Table tbl in rpt.Subreports[i].Database.Tables)
            //    {
            //        tbl.LogOnInfo.ConnectionInfo = connectInfo;
            //        tbl.ApplyLogOnInfo(tbl.LogOnInfo);
            //    }
            //}            
            //return File(rpt.ExportToStream(ExportFormatType.PortableDocFormat), "application/pdf");

            //ReportDocument rpt = new ReportDocument();


            //rpt.Load(Server.MapPath("~/Reports/ApplicantsCopy.rpt"));

            //rpt.FileName = Server.MapPath("~/Reports/ApplicantsCopy.rpt");            

            //ParameterDiscreteValue crParameterDiscreteValue = new CrystalDecisions.Shared.ParameterDiscreteValue();
            //ParameterValues crParameterValues = new ParameterValues();
            //crParameterDiscreteValue.Value = id;
            //crParameterValues.Add(crParameterDiscreteValue);
            //rpt.DataDefinition.ParameterFields["ParamBonderSlNo"].ApplyCurrentValues(crParameterValues);



            //rpt.SetDatabaseLogon(CommonAppSet.ConnInfo.userId, CommonAppSet.ConnInfo.pass, CommonAppSet.ConnInfo.server, "");



            //ParameterFieldDefinitions crParameterFieldDefinitions;
            //ParameterFieldDefinition crParameterFieldLocation;


            //crParameterDiscreteValue = new CrystalDecisions.Shared.ParameterDiscreteValue();
            //crParameterDiscreteValue.Description = "ParamBonderSlNo";
            //crParameterDiscreteValue.Value = ParameterParse(rpt.ParameterFields[0].ParameterValueType, id.ToString());

            // crParameterFieldDefinitions = rpt.DataDefinition.ParameterFields;

            //crParameterFieldLocation = (ParameterFieldDefinition)crParameterFieldDefinitions["ParamBonderSlNo"];
            //crParameterValues = crParameterFieldLocation.CurrentValues;

            //crParameterValues.Clear();

            //crParameterFieldLocation.ApplyCurrentValues(crParameterValues);


            //rpt.SetParameterValue(paramName[i], ParameterParse(rpt.ParameterFields[paramName[i]].ParameterValueType,
            //    paramVal[i].ToString()));

            //rpt.Refresh();

            //rpt.VerifyDatabase();
            //rpt.ReadRecords();
            //rpt.SetParameterValue("ParamBonderSlNo", id);
            //ReportViewer crViewer = new ReportViewer();
            //Response.Redirect("~/ReportViewer.aspx");            
            //return File(rpt.ExportToStream(ExportFormatType.PortableDocFormat),"application/pdf");
            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            //rpt.SetParameterValue("ParamBonderSlNo", id);

            //stream.Seek(0, SeekOrigin.Begin);        
            #endregion
        }

        public ActionResult Index()
        {
            return View();
        }

    }
}

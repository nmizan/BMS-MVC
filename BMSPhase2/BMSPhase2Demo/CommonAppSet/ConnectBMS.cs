
using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.CommonAppSet
{
    public static class ConnectBMS
    {
        public static void Connect()
        {
            string oradb = "Data Source=192.168.2.8:1522/XE; User ID=BMS;Password=BMS;";

            System.Data.OracleClient.OracleConnection conn = new System.Data.OracleClient.OracleConnection(oradb); // C#
            try
            {
                conn.Open();
            }
            catch
            {

            }
        }

        public static OracleConnection Connection()
        {
            //string BMSdb = "Data Source=192.168.2.11:1521/XE; User ID=BMS;Password=BMS;";
            //string BMSdb = "Data Source=192.168.2.11:1521/XE; User ID=BMS_N;Password=ccl123;";
            //string BMSdb = "Data Source=192.168.2.6:1521/XE; User ID=BMS_N;Password=ccl123;";
            string BMSdb = System.Configuration.ConfigurationManager.ConnectionStrings["BMSDbContext1"].ConnectionString;
            System.Data.OracleClient.OracleConnection conn = new System.Data.OracleClient.OracleConnection(BMSdb);
            conn.Open();
            return conn;

        }
    }
}
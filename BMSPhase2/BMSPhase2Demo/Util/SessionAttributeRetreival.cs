using BMSPhase2Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace BMSPhase2Demo.Util
{
    public class SessionAttributeRetreival
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();
        public USERPERMISSION getStoredUserPermission() {
            string username = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            APPUSER appuser = db.APPUSERs.SingleOrDefault(u => u.USERNAME.Equals(username, StringComparison.OrdinalIgnoreCase));
            try
            {
                USERPERMISSION userpermission = db.USERPERMISSIONs.SingleOrDefault(u => u.USERID == appuser.ID);
                return userpermission;
            }
            catch (NullReferenceException exp) {
                return null;
            }
            
        }
    }
}
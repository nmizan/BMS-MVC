using BMSPhase2Demo.Controllers;
using BMSPhase2Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.Utils
{
    public class BonderInfoUtils
    {
        public static string getBonderName(short  id=0)
        {
            using(OracleEntitiesConnStr db = new OracleEntitiesConnStr()){
                BONDER bonder = db.BONDERs.Find(id);
                if (bonder == null) {
                    return String.Empty;
                }
                return bonder.BONDERNAME;
            }
        }

    }
}
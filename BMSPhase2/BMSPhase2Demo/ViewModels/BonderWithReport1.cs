using BMSPhase2Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMSPhase2Demo.ViewModels
{
    public class BonderWithReport1
    {
        private OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        public BonderWithReport1()
        {
            this.BonderInfo = null;
            var bonderList = db.BONDERs;
            Bonders = bonderList.ToList();
        }

        
        public List<Models.BONDER> Bonders { get; set; }
        public Models.BonderInfo BonderInfo { get; set; }
    }
}
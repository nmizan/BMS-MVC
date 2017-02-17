using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using BMSPhase2Demo.Models;

namespace Domain.Concrete
{
   class BondEntitlementRepository
    {

        OracleEntitiesConnStr db = new OracleEntitiesConnStr();

        public int AddBonderAnnualEntitlement(string entitlementtype, int? BonderSlno, decimal? totalentitlement, int? totalentitlementunit, string totalentitlementinword,
                                  decimal? onetimestoragequantity, int? onetimestoragequantitytunit, string onetimestoragequantityinwords,
                                  decimal? entitlementpercentaccordingtoproduction, int? entitlementpercentaccordingtoproductionunit,
                                  string entitlementpercentaccordingtoproductioninword,
                                 string entitlementdatefrom, string entitlementdateto)
        {

            BONDERANNUALENTITLEMENT BonderAnnualEntitlement = new BONDERANNUALENTITLEMENT();


            //BonderAnnualEntitlement.AESLNO = 200;


            BonderAnnualEntitlement.BONDERSLNO = (Int16)BonderSlno;
            BonderAnnualEntitlement.ENTLTYPE = entitlementtype;
            BonderAnnualEntitlement.TOTALSTORAGEQTY = totalentitlement;
            BonderAnnualEntitlement.TSMUSLNO = (Int16)totalentitlementunit;
            BonderAnnualEntitlement.TOTALSTORAGEQTYEN = totalentitlementinword;
            BonderAnnualEntitlement.TOTALSTORAGEQTYBG = "BG";
            BonderAnnualEntitlement.ONETIMESTORAGEQTY = onetimestoragequantity;
            BonderAnnualEntitlement.OTMUSLNO = (Int16)onetimestoragequantitytunit;
            BonderAnnualEntitlement.ONETIMESTORAGEQTYEN = onetimestoragequantityinwords;
            BonderAnnualEntitlement.ONETIMESTORAGEQTYBG = "BG";
            BonderAnnualEntitlement.ENTITLEPERCENTQTY = entitlementpercentaccordingtoproduction;
            BonderAnnualEntitlement.ETMUSLNO = (Int16)entitlementpercentaccordingtoproductionunit;
            BonderAnnualEntitlement.ENTITLEPERCENTEN = entitlementpercentaccordingtoproductioninword;
            //BonderAnnualEntitlement.ENTITLEFROM = DateTime.Now ;
            //BonderAnnualEntitlement.ENTITLETO = DateTime.Now ;
            BonderAnnualEntitlement.ENTITLEFROM = DateTime.Parse(entitlementdatefrom);
            BonderAnnualEntitlement.ENTITLETO = DateTime.Parse(entitlementdateto);

            db.BONDERANNUALENTITLEMENTs.Add(BonderAnnualEntitlement);


            db.SaveChanges();

            return db.BONDERANNUALENTITLEMENTs.Max(b => b.AESLNO);

        }



        public void AddAnnualEntitlementRawMaterilas(int? BonderSlno, int[] hscodes, decimal[] quantitys, int[] units, int id)
        {
            if (hscodes != null)
            {

                for (int i = 0; i < hscodes.Length; i++)
                {
                    ANNUALENTLRAWMATERIAL AnnuaRawMaterialEntitlement = new ANNUALENTLRAWMATERIAL();

                    //-----------Mizan Work (20 july 2016)----------------
                    var mslno = hscodes[i];
                    units[i] = (Int16)(from a in db.MATERIALS where a.MSLNO == mslno select a.MUSLNO).SingleOrDefault();
                    //-----------Mizan Work (20 july 2016)----------------

                    AnnuaRawMaterialEntitlement.AESLNO = (Int16)id;
                    AnnuaRawMaterialEntitlement.BONDERSLNO = (Int16)BonderSlno;
                    AnnuaRawMaterialEntitlement.MSLNO = (Int16)hscodes[i];
                    AnnuaRawMaterialEntitlement.AEQTY = (Decimal)quantitys[i];
                    AnnuaRawMaterialEntitlement.MUSLNO = (Int16)units[i];


                    db.ANNUALENTLRAWMATERIALs.Add(AnnuaRawMaterialEntitlement);
                    db.SaveChanges();


                }
            }


        }



        public void AddBonderAnnualEntitlementCoE(int? BonderSlno, int id, int[] ProductCoE, string[] ProductWeightCoE, string[] ProductDescriptionCoE, string[] ProductSizeCoE,
             string[] ProductMeasurementCoE, string[] ProductRawmaterialsCoE, int[] ProductMachineCoE, int[] hscodesCoE, int[] RawmaterialProductID,
            int[] measuementoCoE, int[] grossCoEs, int[] wastageCoEs, int[] shrinkageCoEs, int[] netCoEs)
        {

            if (ProductCoE != null)
            {


                int lenth = ProductCoE.Length;

                for (int i = 0; i < lenth; i++)
                {
                    BONDERANNUALENTITLEMENTCOM bonderannualentitlementcom = new BONDERANNUALENTITLEMENTCOM();

                    bonderannualentitlementcom.AESLNO = (Int16)id;
                    bonderannualentitlementcom.BONDERSLNO = (Int16)BonderSlno;
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


                            bonderannualentitlementcoedet.AESLNO = (Int16)id;
                            bonderannualentitlementcoedet.BONDERSLNO = (Int16)BonderSlno;
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


                //db.SaveChanges();

            }


        }


    }
}
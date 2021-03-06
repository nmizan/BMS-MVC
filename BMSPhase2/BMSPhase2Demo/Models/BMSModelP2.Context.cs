﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BMSPhase2Demo.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;

    public partial class OracleEntitiesConnStr : DbContext
    {
        public OracleEntitiesConnStr()
            : base("name=OracleEntitiesConnStr")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public DbSet<EXBONDBACKTOBACK> EXBONDBACKTOBACKs { get; set; }
        public DbSet<UPEXBONDLIST> UPEXBONDLISTs { get; set; }
        public DbSet<UP> UPs { get; set; }
        public DbSet<UPREQUEST> UPREQUESTs { get; set; }
        public DbSet<UPREQUESTLIST> UPREQUESTLISTs { get; set; }
        public DbSet<RAWMATERIAL> RAWMATERIALs { get; set; }
        public DbSet<BACKTOBACKLC> BACKTOBACKLCs { get; set; }
        public DbSet<BACKTOBACKPRODUCT> BACKTOBACKPRODUCTs { get; set; }
        public DbSet<EXBOND> EXBONDs { get; set; }
        public DbSet<INBOND> INBONDs { get; set; }
        public DbSet<INBONDRAWMATERIAL> INBONDRAWMATERIALs { get; set; }
        public DbSet<ANNUALENTLRAWMATERIAL> ANNUALENTLRAWMATERIALs { get; set; }
        public DbSet<APPUSER> APPUSERs { get; set; }
        public DbSet<ASSOCIATIONENROLL> ASSOCIATIONENROLLs { get; set; }
        public DbSet<ATTACHMENT> ATTACHMENTs { get; set; }
        public DbSet<BANK> BANKs { get; set; }
        public DbSet<BANKBRANCH> BANKBRANCHes { get; set; }
        public DbSet<BONDAPPLICATIONPROGRESS> BONDAPPLICATIONPROGRESSes { get; set; }
        public DbSet<BONDAREA> BONDAREAs { get; set; }
        public DbSet<BONDCATEGORY> BONDCATEGORies { get; set; }
        public DbSet<BONDCIRCLE> BONDCIRCLEs { get; set; }
        public DbSet<BONDDIVISION> BONDDIVISIONs { get; set; }
        public DbSet<BONDER> BONDERs { get; set; }
        public DbSet<BONDERANNUALENTITLEMENT> BONDERANNUALENTITLEMENTs { get; set; }
        public DbSet<BONDERANNUALENTITLEMENTCOEDET> BONDERANNUALENTITLEMENTCOEDETs { get; set; }
        public DbSet<BONDERANNUALENTITLEMENTCOM> BONDERANNUALENTITLEMENTCOMs { get; set; }
        public DbSet<BONDERANNUALENTITLEMENTDET> BONDERANNUALENTITLEMENTDETs { get; set; }
        public DbSet<BONDERLIENBANK> BONDERLIENBANKs { get; set; }
        public DbSet<BONDERPRODUCT> BONDERPRODUCTs { get; set; }
        public DbSet<BONDERRAWMATERIAL> BONDERRAWMATERIALs { get; set; }
        public DbSet<BONDREGISTRATION> BONDREGISTRATIONs { get; set; }
        public DbSet<BONDSTATU> BONDSTATUS { get; set; }
        public DbSet<BONDSUBCATEGORY> BONDSUBCATEGORies { get; set; }
        public DbSet<BONDTYPE> BONDTYPEs { get; set; }
        public DbSet<COUNTRY> COUNTRies { get; set; }
        public DbSet<CURRENCY> CURRENCies { get; set; }
        public DbSet<DESIGNATION> DESIGNATIONs { get; set; }
        public DbSet<DISTRICT> DISTRICTs { get; set; }
        public DbSet<DOCHEADINGINFO> DOCHEADINGINFOes { get; set; }
        public DbSet<DOCUMENTATTACHMENT> DOCUMENTATTACHMENTs { get; set; }
        public DbSet<EMPLOYEE> EMPLOYEEs { get; set; }
        public DbSet<FINISHEDPRODUCT> FINISHEDPRODUCTS { get; set; }
        public DbSet<INSTALLEDMACHINEINFO> INSTALLEDMACHINEINFOes { get; set; }
        public DbSet<MACHINEINFORMATION> MACHINEINFORMATIONs { get; set; }
        public DbSet<MATERIAL> MATERIALS { get; set; }
        public DbSet<MATERIALSGROUP> MATERIALSGROUPs { get; set; }
        public DbSet<MEASUREMENTUNIT> MEASUREMENTUNITs { get; set; }
        public DbSet<MEASUREMENTUNITCONVERSION> MEASUREMENTUNITCONVERSIONs { get; set; }
        public DbSet<MODEL> MODELs { get; set; }
        public DbSet<OFFICE> OFFICEs { get; set; }
        public DbSet<OWNERASSOCIATEBUSINESS> OWNERASSOCIATEBUSINESSes { get; set; }
        public DbSet<OWNERINFO> OWNERINFOes { get; set; }
        public DbSet<PRODCAPACITYMACHINEWISEDET> PRODCAPACITYMACHINEWISEDETs { get; set; }
        public DbSet<PRODCAPACITYMACHINEWISEM> PRODCAPACITYMACHINEWISEMs { get; set; }
        public DbSet<STATUS> STATUS { get; set; }
        public DbSet<UPAZILA> UPAZILAs { get; set; }
        public DbSet<USERPERMISSION> USERPERMISSIONs { get; set; }

        public virtual int DELETE_CASCADE(string tABLE_OWNER, string pARENT_TABLE, string wHERE_CLAUSE)
        {
            var tABLE_OWNERParameter = tABLE_OWNER != null ?
                new ObjectParameter("TABLE_OWNER", tABLE_OWNER) :
                new ObjectParameter("TABLE_OWNER", typeof(string));

            var pARENT_TABLEParameter = pARENT_TABLE != null ?
                new ObjectParameter("PARENT_TABLE", pARENT_TABLE) :
                new ObjectParameter("PARENT_TABLE", typeof(string));

            var wHERE_CLAUSEParameter = wHERE_CLAUSE != null ?
                new ObjectParameter("WHERE_CLAUSE", wHERE_CLAUSE) :
                new ObjectParameter("WHERE_CLAUSE", typeof(string));

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DELETE_CASCADE", tABLE_OWNERParameter, pARENT_TABLEParameter, wHERE_CLAUSEParameter);
        }
    }
}

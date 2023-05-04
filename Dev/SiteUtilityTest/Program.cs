using R_DW_100_CarePlanHtmlUpdate;
using R_JE_100_MovePractice;
using R_JE_109_AddSortColumn;
using R_1_10_CkccEngagement;
using R_1_11_IWH;
using R_JE_110_Init_UpdateProgramParticipation;
using R_JE_120_CkccKce;
using R_1_7_Referrall;
using Release_1_4;
using R_1_9_MedAlertHospitalizeAlerts;
using R_DW_110_MD_Timesheet;
using System.Collections.Generic;

namespace SiteUtilityTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Maintenance after CORE deployment...
             * program participation - CKCC/KCE Resources
             * program participation - Patient Status Updates
             * Data Exchange
             * Risk Adjustment
             * Quality
             * hospitalization alerts
             * medication alerts
             * 
             */

            //List<string> pracList = new List<string>();
            //pracList.Add("90653003729");
            //pracList.Add("99252413689");
            //pracList.Add("96341625189");
            //pracList.Add("94845273959");
            //pracList.Add("96472071029");
            //pracList.Add("96860303729");

            //foreach (var siteId in pracList)
            //{
            //    //string siteId = "96194284189";

            //    //MovePractice movePractice = new MovePractice();
            //    //movePractice.InitiateProg(siteId);

            //    //--------------------------------------------------------
            //    // Run Maintenance Code to Complete a new site Deployment
            //    //--------------------------------------------------------
            //    CompleteNewSiteDeployment(siteId); 
            //}

            // Deploy MD Timesheet for AIN
            //MD_TimesheetDeploy objMD_TimesheetDeploy = new MD_TimesheetDeploy();
            //objMD_TimesheetDeploy.InitiateProg("97438072639");

            ProgramNew_SS objSS = new ProgramNew_SS();
            objSS.InitiateProg();
        }
        static void CompleteNewSiteDeployment(string siteID)
        {


            //UpdateProgramParticipation updateProgramParticipation = new UpdateProgramParticipation();
            //updateProgramParticipation.InitProg(siteID);



            MedAlertHospitalizeAlerts medAlertHospitalizeAlerts = new MedAlertHospitalizeAlerts();
            medAlertHospitalizeAlerts.InitiateProg(siteID);

            //AddSortColumn addSortColumn = new AddSortColumn();
            //addSortColumn.InitiateProg(siteID);  // run again to sort Program Participation...

            //AddReferrall addReferrall = new AddReferrall();
            //addReferrall.InitiateProg(siteID);

            //AddCkccKce addCkccKce = new AddCkccKce();
            //addCkccKce.InitProg(siteID);

            //AddDialysisStart addDialysisStart = new AddDialysisStart();
            //addDialysisStart.InitProg(siteID);



            //AddIWH addIWH = new AddIWH();
            //addIWH.InitProg(siteID);
            //CkccEngagement ckccEngagement = new CkccEngagement();
            //ckccEngagement.InitiateProg(siteID);
            //CarePlanHtmlUpdate carePlanHtmlUpdate = new CarePlanHtmlUpdate();
            //carePlanHtmlUpdate.InitiateProg(siteID);

            //addSortColumn.InitiateProg(siteID);  // run again to sort Program Participation...
        }
    }
}

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
using System.Collections.Generic;

namespace SiteUtilityTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Maintenance after CORE deployment...
             * Upload Support files to SiteAssets and SiteAssets/Img
             * Update Permissions
             * Program Participation - Update URL refs
             * Program Manager - Correct the URL Global Nav
             * Program Participation - Update references (SiteAdminGroup, AdminGroup, Site Settings > Desc for Tooltip)
             * Program Participation - Update Img references
             * Program Participation - Resize Webpart
             * Program Participation - Update MultiTab JavaScript files
             * Program Participation - Add Sort_Order column
             * Deploy Feature - Hospitalization Alerts
             * Deploy Feature - Medication Alerts
             * 
             */

            List<string> pracList = new List<string>();
            pracList.Add("99441985029");

            //pracList.Add("98368501549");
            //pracList.Add("93594881659");
            //pracList.Add("98280667169");

            //pracList.Add("98822972489");
            //pracList.Add("91778940339");
            //pracList.Add("92641750339");
            //pracList.Add("93683360339");




            foreach (var siteId in pracList)
            {
                //--------------------------------------------------------
                // Update Practice(s) after CORE Deployment
                //--------------------------------------------------------
                //MovePractice movePractice = new MovePractice();
                //movePractice.InitiateProg(siteId);

                //--------------------------------------------------------
                // Run Maintenance Code to Complete a new site Deployment
                //--------------------------------------------------------
                CompleteNewSiteDeployment(siteId);

            }

            // Deploy MD Timesheet for AIN
            //MD_TimesheetDeploy objMD_TimesheetDeploy = new MD_TimesheetDeploy();
            //objMD_TimesheetDeploy.InitiateProg("97438072639");



        }
        static void CompleteNewSiteDeployment(string siteID)
        {
            CarePlanHtmlUpdate carePlanHtmlUpdate = new CarePlanHtmlUpdate();
            carePlanHtmlUpdate.InitiateProg(siteID);

            MedAlertHospitalizeAlerts medAlertHospitalizeAlerts = new MedAlertHospitalizeAlerts();
            medAlertHospitalizeAlerts.InitiateProg(siteID);

            AddSortColumn addSortColumn = new AddSortColumn();
            addSortColumn.InitiateProg(siteID);  // run again to sort Program Participation...

            AddIWH addIWH = new AddIWH();
            addIWH.InitProg(siteID);


            //AddReferrall addReferrall = new AddReferrall();
            //addReferrall.InitiateProg(siteID);

            //AddCkccKce addCkccKce = new AddCkccKce();
            //addCkccKce.InitProg(siteID);

            //AddDialysisStart addDialysisStart = new AddDialysisStart();
            //addDialysisStart.InitProg(siteID);

            //CkccEngagement ckccEngagement = new CkccEngagement();
            //ckccEngagement.InitiateProg(siteID);
            

            //addSortColumn.InitiateProg(siteID);  // run again to sort Program Participation...
        }
    }
}

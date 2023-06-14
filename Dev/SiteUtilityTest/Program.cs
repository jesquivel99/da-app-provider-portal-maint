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
using R_SS_130_TransitionManagement;
using R_1_9_PayorEnrollment;
using System.Collections.Generic;

namespace SiteUtilityTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> pracList = new List<string>();
            pracList.Add("94026153649");

            foreach (string siteId in pracList)
            {
                //--------------------------------------------------------
                // Update Practice(s) after CORE Deployment
                //--------------------------------------------------------
                //MovePractice movePractice = new MovePractice();
                //movePractice.InitiateProg(siteId);

                //--------------------------------------------------------
                // Run Maintenance Code to Complete a new site Deployment
                //--------------------------------------------------------
                //CompleteNewSiteDeployment(siteId);

                //ProgramNew_JE programNew_JE = new ProgramNew_JE();
                //programNew_JE.InitiateProg(siteId);
            }
            ProgramNew_JE programNew_JE = new ProgramNew_JE();
            programNew_JE.InitiateProg();
        }
        static void CompleteNewSiteDeployment(string siteID)
        {
            //CarePlanHtmlUpdate carePlanHtmlUpdate = new CarePlanHtmlUpdate();
            //carePlanHtmlUpdate.InitiateProg(siteID);

            //UpdateProgramParticipation updateProgramParticipation = new UpdateProgramParticipation();
            //updateProgramParticipation.InitProg(siteID);

            //MedAlertHospitalizeAlerts medAlertHospitalizeAlerts = new MedAlertHospitalizeAlerts();
            //medAlertHospitalizeAlerts.InitiateProg(siteID);

            //TransitionManagement transitionManagement = new TransitionManagement();
            //transitionManagement.InitiateProg(siteID);

            //AddIWH addIWH = new AddIWH();
            //addIWH.InitProg(siteID);

            //PayorEnrollment payorEnrollment = new PayorEnrollment();
            //payorEnrollment.Init_PayorEnrollment(siteID);

            //AddReferrall addReferrall = new AddReferrall();
            //addReferrall.InitiateProg(siteID);

            //AddCkccKce addCkccKce = new AddCkccKce();
            //addCkccKce.InitProg(siteID);

            //AddDialysisStart addDialysisStart = new AddDialysisStart();
            //addDialysisStart.InitProg(siteID);

            //CkccEngagement ckccEngagement = new CkccEngagement();
            //ckccEngagement.InitiateProg(siteID);

            //AddSortColumn addSortColumn = new AddSortColumn();
            //addSortColumn.InitiateProg(siteID);  // run again to sort Program Participation...

        }
    }
}

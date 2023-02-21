using System;
using System.Collections.Generic;
using SiteUtility;

namespace R_DW_100_CarePlanHtmlUpdate
{
    public class CarePlanHtmlUpdate
    {
        public void InitiateProg()
        {
            SiteInfoUtility siteInfo = new SiteInfoUtility();
            SiteLogUtility slu = new SiteLogUtility();

            //List<Practice> practices = siteInfo.GetPracticesByPM("10");
            List<Practice> practices = siteInfo.GetAllPractices();
            if (practices != null && practices.Count > 0)
            {
                try
                {
                    slu.LoggerInfo_Entry("================ Deployment Started =====================", true);
                    int intLoop = 0;

                    foreach (Practice practice in practices)
                    {
                        UpdateCarePlanHtmlFile(practice.NewSiteUrl);
                        slu.LoggerInfo_Entry(practice.Name + "  .. Html Updated.", true);
                        slu.LoggerInfo_Entry(practice.NewSiteUrl, true);
                        intLoop++;
                    }

                    slu.LoggerInfo_Entry("Total Practices: " + intLoop, true);
                    slu.LoggerInfo_Entry("================ Deployment Completed =====================", true);
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
                }
            }
        }
        public void InitiateProg(string siteID)
        {
            SiteInfoUtility siteInfo = new SiteInfoUtility();
            SiteLogUtility slu = new SiteLogUtility();

            Practice practice = siteInfo.GetPracticeBySiteID(siteID);
            if (practice != null)
            {
                try
                {
                    slu.LoggerInfo_Entry("================ Deployment Started =====================", true);
                    UpdateCarePlanHtmlFile(practice.NewSiteUrl);
                    slu.LoggerInfo_Entry(practice.Name + "  .. Html Updated.", true);
                    slu.LoggerInfo_Entry("================ Deployment Completed =====================", true);
                }
                catch (Exception ex)
                {
                    //SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", strPortalSiteURL);
                }
            }
        }
        public void UpdateCarePlanHtmlFile(string strURL)
        {
            try
            {
                SiteFilesUtility objFilesSite = new SiteFilesUtility();
                //objFilesSite.DocumentUpload(strURL, @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\cePrac_CarePlansDataTable.html", "SiteAssets");

                //HTML Update Files - Deploy 9/09...
                objFilesSite.DocumentUpload(strURL, @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\cePrac_CarePlansDataTable.html", "SiteAssets");
                objFilesSite.DocumentUpload(strURL, @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\cePrac_HospAlertDataTable.html", "SiteAssets");
                //objFilesSite.DocumentUpload(strURL, @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\cePrac_HospitalAlerts.html", "SiteAssets");
                objFilesSite.DocumentUpload(strURL, @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\cePrac_MedAlertDataTable.html", "SiteAssets");
                //objFilesSite.DocumentUpload(strURL, @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\cePrac_MedicationAlerts.html", "SiteAssets");
                objFilesSite.DocumentUpload(strURL, @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\cePrac_ProgramParTableData.html", "SiteAssets");
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
         
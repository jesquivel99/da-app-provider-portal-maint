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

            List<Practice> practices = siteInfo.GetPracticesByPM("10");
            if (practices != null && practices.Count > 0)
            {
                try
                {
                    Console.WriteLine("================ Deployment Started =====================");
                    int intLoop = 0;

                    foreach (Practice practice in practices)
                    {
                        UpdateCarePlanHtmlFile(practice.NewSiteUrl);
                        Console.WriteLine(++intLoop + " - " + practice.Name + "  .. Care Plan Html Updated.");
                    }
                    Console.WriteLine("=============== Deployment Completed ====================");
                }
                catch (Exception ex)
                {
                    //SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", strPortalSiteURL);
                }
            }
        }
        public void InitiateProg(string siteID)
        {
            SiteInfoUtility siteInfo = new SiteInfoUtility();

            Practice practice = siteInfo.GetPracticeBySiteID(siteID);
            if (practice != null)
            {
                try
                {
                    Console.WriteLine("================ Deployment Started =====================");
                    UpdateCarePlanHtmlFile(practice.NewSiteUrl);
                    Console.WriteLine(practice.Name + "  .. Care Plan Html Updated.");
                    Console.WriteLine("================ Deployment Completed =====================");
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
                objFilesSite.DocumentUpload(strURL, @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\cePrac_CarePlansDataTable.html", "SiteAssets");
            }
            catch (Exception ex)
            {

            }
        }
    }
}
         
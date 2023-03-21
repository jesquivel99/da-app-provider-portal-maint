using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SiteUtility;
using System.Configuration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using Microsoft.SharePoint.Client.WebParts;
using System.Net;
using System.IO;
using System.Xml;
using System.Reflection;

namespace SiteUtilityTest
{
    public class ProgramNew_AA
    {
        public void InitiateProg()
        {
            string releaseName = "StartUpTemplate";
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];

            string runPM = "PM01";
            string runPractice = "94910221369";

            SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            SiteLogUtility.Log_Entry("\n\n=============Release Starts=============", true);

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    SiteLogUtility.Log_Entry("\n\n=============[ Get all Portal Practice Data ]=============", true);
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext);

                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - Start]=============", true);
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        foreach (PracticeSite psite in pm.PracticeSiteCollection)
                        {
                            //if (psite.URL.Contains(runPM))
                            if (psite.URL.Contains(runPM) && (psite.URL.Contains(runPractice)))
                            {
                                SiteLogUtility.LogPracDetail(psite);
                                List<PMData> pmd = SiteInfoUtility.SP_GetAll_PMData(pm.URL, psite.SiteId);
                                if (pmd.Count > 0)
                                {
                                    if (pmd[0].IsCKCC == "true")
                                    {
                                        Init_Setup(psite);
                                        SiteLogUtility.Log_Entry("Site is CKCC - Setup is Complete");
                                    }
                                    else
                                    {
                                        SiteLogUtility.Log_Entry("Site is NOT CKCC - No changes made");
                                    }
                                }
                            }
                        }
                    }
                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - End]=============", true);
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
                }
                finally
                {
                    SiteLogUtility.Log_Entry("\n\n=============Release Ends=============", true);
                    SiteLogUtility.finalLog(releaseName);
                }
            }
        }

        private void Init_Setup(PracticeSite psite)
        {
            try
            {
                // Do something...
                SitePublishUtility spUtility = new SitePublishUtility();
                spUtility.InitializePage(psite.URL, "TestPage", "New Test Page");
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_Setup", ex.Message, "Error", "");
            }
        }
    }
}

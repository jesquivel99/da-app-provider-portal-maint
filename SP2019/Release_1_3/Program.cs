using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteUtility;
using System.Configuration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using System.Net;
using System.IO;

namespace Release_1_3
{
    public class Program
    {
        static public List<Practice> practicesIWH = new List<Practice>();
        static public List<Practice> practicesCKCC = new List<Practice>();
        static void Main(string[] args)
        {
            string releaseName = "OptimalStartNavigation";
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            string srcUrlIWH = ConfigurationManager.AppSettings["SP_IWHUrl"];
            string srcUrlCKCC = ConfigurationManager.AppSettings["SP_CKCCUrl"];
            string siteInfoFile = ConfigurationManager.AppSettings["Csv_File"];

            SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            SiteLogUtility.Log_Entry("\n\n=============Release Starts=============", true);

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    SiteLogUtility.Log_Entry("\n\n=============[ Get all Portal Practice Data ]=============", true);
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext, practicesIWH, practicesCKCC);

                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - Start]=============", true);
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        foreach (PracticeSite psite in pm.PracticeSiteCollection)
                        {
                            if (psite.URL.Contains("91882751659"))
                            {
                                SiteLogUtility.LogPracDetail(psite);
                                SiteLogUtility.Log_Entry("MENU BEFORE...");
                                SiteNavigateUtility.QuickLaunch_Print(psite.URL);
                                SiteNavigateUtility.NavigationPracticeMnt(psite.URL, pm.PMURL);
                                SiteLogUtility.Log_Entry("MENU AFTER...");
                                SiteNavigateUtility.QuickLaunch_Print(psite.URL);
                            }
                        }
                    }
                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - End]=============", true);
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", siteUrl);
                }
                finally
                {
                    SiteLogUtility.finalLog(releaseName);
                }
                SiteLogUtility.Log_Entry("=============Release Ends=============", true);
            }
        }

        private static List<Practice> GetAllPracticeExistingSites(ClientContext clientContext, List<Practice> practices, PracticeType practiceType)
        {
            clientContext.Load(clientContext.Web);
            clientContext.Load(clientContext.Web.Webs);
            clientContext.ExecuteQuery();

            foreach (Web web in clientContext.Web.Webs)
            {
                if (Char.IsDigit(web.Url.Last()))
                {
                    using (ClientContext clientContext0 = new ClientContext(web.Url))
                    {
                        clientContext0.Load(clientContext0.Web);
                        clientContext0.Load(clientContext0.Web.Webs);
                        clientContext0.ExecuteQuery();

                        if (clientContext0.Web.Url.Contains("/ICKCCGroup") || clientContext0.Web.Url.Contains("/iwn"))
                        {
                            string group = clientContext0.Web.Url.Substring(clientContext0.Web.Url.Length - 2);

                            if (group.CompareTo("12") < 0)
                            {
                                foreach (Web web0 in clientContext0.Web.Webs)
                                {
                                    Practice practice = new Practice();
                                    practice.ExistingSiteUrl = web0.Url;
                                    practice.Type = practiceType;
                                    practices.Add(practice);
                                }
                            }
                        }
                    }
                }
            }
            return practices;
        }

    }
}

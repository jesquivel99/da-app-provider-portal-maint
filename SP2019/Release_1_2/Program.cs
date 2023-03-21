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
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            string srcUrlIWH = ConfigurationManager.AppSettings["SP_IWHUrl"];
            string srcUrlCKCC = ConfigurationManager.AppSettings["SP_CKCCUrl"];
            string siteInfoFile = ConfigurationManager.AppSettings["Csv_File"];
            string releaseName = "HomePageRedirect";

            SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            SiteLogUtility.Log_Entry("\n\n=============Release Starts=============", true);


            // Get all existing IWN and iCKCC Practice Data...
            SiteLogUtility.Log_Entry("\n\n=============[ Get all Existing Practice Data (IWN-CKCC) ]=============", true);
            using (ClientContext clientContextIWH = new ClientContext(srcUrlIWH))
            {
                clientContextIWH.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                practicesIWH = GetAllPracticeExistingSites(clientContextIWH, practicesIWH, PracticeType.IWH);
            }
            using (ClientContext clientContextCKCC = new ClientContext(srcUrlCKCC))
            {
                clientContextCKCC.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                practicesCKCC = GetAllPracticeExistingSites(clientContextCKCC, practicesCKCC, PracticeType.iCKCC);
            }


            // Get Portal Data...
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    //  Get all Portal Practice Data...
                    SiteLogUtility.Log_Entry("\n\n=============[ Get all Portal Practice Data ]=============", true);
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext, practicesIWH, practicesCKCC);

                    //  Maintenance Tasks...
                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks ]=============", true);
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        foreach (PracticeSite psite in pm.PracticeSiteCollection)
                        {
                            //if (psite.URL.Contains("90395520569") && psite.ExistingSiteUrl.Length > 0)
                            if (psite.ExistingSiteUrl.Length > 0)
                            {
                                //SiteLogUtility.Log_Entry("\nHome Page Redirect - Test\n\n", true);

                                SiteFilesUtility objSiteFiles = new SiteFilesUtility();
                                SiteLogUtility.Log_Entry("--\n");
                                SiteLogUtility.Log_Entry($"--       Existing Name: {psite.PracticeName}");
                                SiteLogUtility.Log_Entry($"--       Existing Site: {psite.ExistingSiteUrl}");
                                SiteLogUtility.Log_Entry($"--Existing Pages Audit: {psite.ExistingSiteUrl}/Pages");
                                SiteLogUtility.Log_Entry($"--         Portal Site: {psite.URL}");
                                SiteLogUtility.Log_Entry($"--   Permissions Audit: {psite.URL}/_layouts/user.aspx");
                                SiteLogUtility.Log_Entry($"--         Pages Audit: {psite.URL}/Pages");

                                //Maintenance...
                                //SitePublishUtility.DownloadPage(psite, "Home");
                                //SitePublishUtility.CheckinHomePage(psite);

                                //Deployment...
                                //SitePublishUtility.DownloadBackupHomePage(psite);
                                //objSiteFiles.DocumentUpload(psite.ExistingSiteUrl, @"C:\Temp\Home_Backup.aspx", "Pages");
                                //SitePublishUtility.CheckinPage(psite, "Home_Backup");
                                //objSiteFiles.CreateRedirectPage(psite.URL);
                                //objSiteFiles.DocumentUpload(psite.ExistingSiteUrl, @"C:\Temp\Home.aspx", "Pages");
                                //SitePublishUtility.CheckinPage(psite, "Home");
                            }
                        }
                    }
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

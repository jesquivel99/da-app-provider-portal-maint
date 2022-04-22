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
    public class ProgramNew_JE
    {
        /// <summary>
        /// NOTES:
        /// Update LayoutsFolderMnt (if needed)
        /// Update runPM variable - variable will be used to determine PM site execution
        /// Update runPractice variable - variable will be used to determine Practice site execution
        /// Update urlAdminGroup - this url will point to the AdminGroup list for a given PM
        /// Change "If" statement loop with correct variable(s)
        /// Manually update AdminGroup with corrected Program Participation
        /// Update rootUrl and siteUrl in the App.config file
        /// Update Credentials in SiteCredentialUtility.cs
        /// Update Credentials in SiteLogUtility.cs
        /// </summary>
        static string LayoutsFolderMnt = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
        static public List<Practice> practicesIWH = new List<Practice>();
        static public List<Practice> practicesCKCC = new List<Practice>();
        static Guid _listGuid = Guid.Empty;
        static int cntRun = 0;
        static int cntRunAdminGroup = 0;
        static int cntIsCkcc = 0;
        static int cntIsIwh = 0;
        static int cntIsKc365 = 0;
        public void InitiateProg()
        {
            string releaseName = "SiteUtilityTest";
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];

            string runPM = "PM01";
            string runPractice = "99903210159";
            string urlAdminGroup = siteUrl + "/" + runPM;

            string connString = "Data Source=" + ConfigurationManager.AppSettings["SqlServer"]
                        + "; Initial Catalog=" + ConfigurationManager.AppSettings["Database"] + "; Integrated Security=SSPI";

            SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            SiteLogUtility.Log_Entry("\n\n=============Release Starts=============", true);

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    SiteLogUtility.Log_Entry("Processing AdminGroup: " + urlAdminGroup, true);
                    SiteLogUtility.Log_Entry(SiteLogUtility.textLine0);
                    List<PMData> pmData = SiteInfoUtility.initPMDataToList(urlAdminGroup);
                    cntRunAdminGroup = pmData.Count();

                    SiteLogUtility.Log_Entry("\n\n=============[ Get all Portal Practice Data ]=============", true);
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext, practicesIWH, practicesCKCC, pmData);

                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - Start]=============", true);
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        foreach (PracticeSite psite in pm.PracticeSiteCollection)
                        {
                            if (psite.URL.Contains(runPM))
                            //if (psite.URL.Contains(runPM) && psite.URL.Contains(runPractice))
                            {
                                cntRun++;
                                SiteLogUtility.Log_Entry("--");
                                SiteLogUtility.Log_Entry("\nRUN COUNT = " + cntRun.ToString() + " OF " + cntRunAdminGroup.ToString(), true);
                                SiteLogUtility.LogPracDetail(psite);

                                LoadParentWebTest(psite);
                            }
                        }
                    }
                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - End]=============", true);

                    SiteLogUtility.Log_Entry("\n\n--Program Participation Totals for " + runPM + "--", true);
                    PMData progPart = new PMData();
                    progPart.PrintProgramParticipationGroupTotal(pmData);

                    SiteLogUtility.Log_Entry(SiteLogUtility.textLine0, true);
                    progPart.PrintProgramParticipationGroupSubTotal(pmData, "KCE Participation");

                    SiteLogUtility.Log_Entry(SiteLogUtility.textLine0, true);
                    progPart.PrintProgramParticipationGroupSubTotal(pmData, "InterWell Health");
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
                }
                finally
                {
                    SiteLogUtility.Log_Entry(SiteLogUtility.textLine0, true);
                    SiteLogUtility.Log_Entry(" Total cntIsCkcc = " + cntIsCkcc.ToString(), true);
                    SiteLogUtility.Log_Entry("Total cntIsKc365 = " + cntIsKc365.ToString(), true);
                    SiteLogUtility.Log_Entry("  Total cntIsIwh = " + cntIsIwh.ToString(), true);
                    SiteLogUtility.finalLog(releaseName);
                }
                SiteLogUtility.Log_Entry("=============Release Ends=============", true);
            }
        }

        public static string urlRelativeReferral_Prod = @"/bi/fhppp/portal/referral/";
        public static string urlRelativeReferral_Dev = @"/bi/fhppp/interimckcc/referral/";
        
        public static void LoadParentWebTest(PracticeSite site)
        {
            using (ClientContext clientContext = new ClientContext(site.URL))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    clientContext.Load(clientContext.Web,
                                            web => web.ParentWeb.ServerRelativeUrl,
                                            web => web.ServerRelativeUrl,
                                            web => web.SiteGroups.Include(
                                                sg => sg.Description,
                                                sg => sg.Title));
                    clientContext.ExecuteQuery();

                    SiteInfoUtility siu = new SiteInfoUtility();
                    string rootUrl = siu.GetRootSite(site.URL);
                    string urlReferralSite = rootUrl;
                    if (rootUrl.Contains("sharepointdev"))
                    {
                        urlReferralSite = rootUrl + urlRelativeReferral_Dev;
                    }
                    else
                    {
                        urlReferralSite = rootUrl + urlRelativeReferral_Prod;
                    }
                    
                    SiteLogUtility.Log_Entry("RootWeb: " + rootUrl);
                    SiteLogUtility.Log_Entry("ParentWeb: " + clientContext.Web.ParentWeb.ServerRelativeUrl);
                    SiteLogUtility.Log_Entry("PracticeWeb: " + clientContext.Web.ServerRelativeUrl);
                    SiteLogUtility.Log_Entry("ReferralWeb: " + urlReferralSite);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error with LoadParentWebTest: " + ex.Message);
                }
            }
        }
        public void breakPageSecurityInheritance(string strURL, string strPageName, string strLibraryName)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(strURL))
                {
                    List targetList = clientContext.Web.Lists.GetByTitle(strLibraryName);
                    ListItem oItem = null;
                    CamlQuery oQuery = new CamlQuery();
                    oQuery.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='LinkFilename' /><Value Type='Text'>" + strPageName + "</Value></Eq></Where></Query></View>";

                    ListItemCollection oItems = targetList.GetItems(oQuery);
                    clientContext.Load(oItems);
                    clientContext.ExecuteQuery();

                    oItem = oItems.FirstOrDefault();
                    oItem.BreakRoleInheritance(true, true);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error with LoadParentWebTest: " + ex.Message);
            }
        }
    }
}

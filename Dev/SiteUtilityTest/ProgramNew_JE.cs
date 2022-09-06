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
using SP = Microsoft.SharePoint.Client;
using System.Net.Mail;
using Serilog;

namespace SiteUtilityTest
{
    public class ProgramNew_JE
    {
        const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
        static ILogger _logger = Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("Logs/maint" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + "_.log", rollingInterval: RollingInterval.Day, shared: false, outputTemplate: outputTemp1)
           .CreateLogger();
        static ILogger logger = _logger.ForContext<ProgramNew_JE>();
        static public List<Practice> practicesIWH = new List<Practice>();
        static public List<Practice> practicesCKCC = new List<Practice>();
        public void InitiateProg()
        {
            string releaseName = "SiteUtilityTest";
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];


            string runPM = "PM04";
            string runPractice = "97849590689";
            string urlAdminGroup = siteUrl + "/" + runPM;

            SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            logger.Information("========================================Release Starts========================================");

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    logger.Information("-------------[ Read Deployed DB:  " + urlAdminGroup + "  ]-------------");
                    //SitePMData objSitePMData = new SitePMData();
                    //DataTable dataTable = objSitePMData.readDBPortalDeployed(runPM);
                    //List<PMData> pmd = FilterPMData(dataTable);

                    logger.Information("-------------[ Processing AdminGroup:  " + urlAdminGroup + "  ]-------------");
                    List<PMData> pmData = SiteInfoUtility.initPMDataToList(urlAdminGroup);

                    logger.Information("-------------[ Get all Portal Practice Data         ]-------------");
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext, practicesIWH, practicesCKCC, pmData);

                    logger.Information("-------------[ Maintenance Tasks - Start            ]-------------");
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        foreach (PracticeSite psite in pm.PracticeSiteCollection)
                        {
                            //if (psite.URL.Contains(runPM))
                            if (psite.URL.Contains(runPM) && psite.URL.Contains(runPractice))
                            {
                                // get pm admingroup...
                                // create if does not exists in SiteAdminData...
                                // update if exists in SiteAdminData...
                                logger.Debug("--");
                                logger.Debug(psite.PracticeName);
                                logger.Debug(psite.Name + " - " + psite.URL);
                                logger.Debug(psite.ProgramParticipation);

                                // BEGIN - This code is used to update from Deployed table...
                                //PMData beforePmd = (PMData)pmData.Where(x => x.SiteId == psite.SiteId).FirstOrDefault();
                                //PMData afterPmd = (PMData)pmd.Where(x => x.SiteId == psite.SiteId).FirstOrDefault();

                                //if (afterPmd.IsTeleKC365 == "true")
                                //{
                                //    logger.Debug("--");
                                //    logger.Debug(psite.PracticeName);
                                //    logger.Debug(psite.Name + " - " + psite.URL);
                                //    //logger.Debug("BEFORE:" + beforePmd.ProgramParticipation);
                                //    logger.Debug(" AFTER:" + afterPmd.ProgramParticipation);

                                //    string adminUrl = LoadParentWeb(pm.URL);
                                //    UpdateProgramParticipation(pm.URL, psite, afterPmd.ProgramParticipation);
                                //    UpdateProgramParticipation(adminUrl, psite, afterPmd.ProgramParticipation, runPM);
                                //    SyncSiteDescription(psite.URL, psite.Name);
                                //}
                                // END - This code is used to update from Deployed table...

                            }
                        }
                    }
                    logger.Information("-------------[ Maintenance Tasks - End              ]-------------");
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
                    logger.Error("Error: " + ex.Message);
                }
                finally
                {
                    logger.Information(SiteLogUtility.textLine0);
                    SiteLogUtility.finalLog(releaseName);
                    SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@freseniusmedicalcare.com");
                }
                logger.Information("========================================Release Ends========================================");
            }

            Log.CloseAndFlush();
        }

        private List<PMData> FilterPMData(DataTable dataTable)
        {
            List<PMData> listPMData = new List<PMData>();
            SitePMData sitePMData = new SitePMData();
            string progPart = string.Empty;

            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    PMData pd = new PMData();

                    pd.GroupID = Convert.ToInt32(row["GroupID"]);
                    pd.SiteId = row["SiteID"].ToString();
                    switch (pd.SiteId)  // temporary for deployment 0729...
                    {
                        case "97849590689":
                            pd.GroupID = 4;
                            break;
                        case "98410990689":
                        case "94012182649":
                        case "96755254749":
                            pd.GroupID = 1;
                            break;
                        case "90667092639":
                        case "93943863639":
                        case "94069321269":
                            pd.GroupID = 9;
                            break;
                        default:
                            break;
                    }

                    pd.PracticeTIN = row["PracticeTIN"].ToString();
                    pd.PracticeName = row["PracticeName"].ToString();
                    pd.IsKC365 = row["KC365"].ToString().Equals("0") ? "false" : "true";
                    pd.IsCKCC = row["CKCCArea"].ToString().Equals("") ? "false" : "true";
                    pd.IsIWH = row["IWNRegion"].ToString().Equals("False") ? "false" : "true";
                    pd.IsTeleKC365 = row["IsTelephonic"].ToString().Equals("False") ? "false" : "true";
                    pd.ProgramParticipation = FormatProgramParticipation(pd);
                    
                    logger.Debug(pd.PracticeName + " - " + pd.SiteId + " - " + pd.PracticeTIN);
                    logger.Debug(pd.ProgramParticipation);
                    logger.Debug(" ");

                    listPMData.Add(pd);
                }
                return listPMData;
            }
            catch (Exception ex)
            {
                logger.Information("Error: " + ex.Message);
                return null;
            }
        }
        private List<PMData> LoadDeployedDataToList(DataTable dataTable)
        {
            List<PMData> listPMData = new List<PMData>();
            SitePMData sitePMData = new SitePMData();
            string progPart = string.Empty;

            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    PMData pd = new PMData();

                    pd.GroupID = Convert.ToInt32(row["GroupID"]);
                    pd.SiteId = row["SiteID"].ToString();
                    pd.PracticeTIN = row["PracticeTIN"].ToString();
                    pd.PracticeName = row["PracticeName"].ToString();
                    pd.IsKC365 = row["KC365"].ToString().Equals("0") ? "false" : "true";
                    pd.IsCKCC = row["CKCCArea"].ToString().Equals("") ? "false" : "true";
                    pd.IsIWH = row["IWNRegion"].ToString().Equals("False") ? "false" : "true";
                    pd.IsTeleKC365 = row["IsTelephonic"].ToString().Equals("False") ? "false" : "true";
                    pd.ProgramParticipation = FormatProgramParticipation(pd);

                    logger.Debug(pd.PracticeName + " - " + pd.SiteId + " - " + pd.PracticeTIN);
                    logger.Debug(pd.ProgramParticipation);
                    logger.Debug(" ");

                    listPMData.Add(pd);
                }
                return listPMData;
            }
            catch (Exception ex)
            {
                logger.Information("Error: " + ex.Message);
                return null;
            }
        }
        public void UpdateProgramParticipation(string adminUrl, PracticeSite site, string dbProgramParticipation, string strProgramManagerSite = "")
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(adminUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web w = clientContext.Web;
                    List adminList = null;

                    if (strProgramManagerSite == "")
                    {
                        adminList = clientContext.Web.Lists.GetByTitle("AdminGroup");
                    }
                    else
                    {
                        adminList = clientContext.Web.Lists.GetByTitle("AdminSiteData");
                    }
                    ListItem oItem = null;
                    clientContext.Load(w);

                    CamlQuery oQuery = new CamlQuery();
                    oQuery.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='PracticeTIN' /><Value Type='Text'>" + site.SiteId + "</Value></Eq></Where></Query></View>";

                    ListItemCollection oItems = adminList.GetItems(oQuery);
                    clientContext.Load(oItems);
                    clientContext.ExecuteQuery();
                    
                    oItem = oItems.FirstOrDefault();
                    logger.Debug("Item=" + oItem["URL"].ToString());
                    logger.Debug("Item=" + oItem["PracticeName"].ToString());
                    logger.Debug("Item=" + oItem["PracticeTIN"].ToString());
                    logger.Debug("Item=" + oItem["ProgramParticipation"].ToString());
                    logger.Debug("Item=" + oItem["KCEArea"].ToString());
                    oItem["ProgramParticipation"] = dbProgramParticipation;
                    //if (strProgramManagerSite != "")
                    //{
                    //    logger.Debug("Item=" + oItem["ProgramManagerSite"].ToString());
                    //}
                    oItem.Update();
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error: " + ex.Message);
            }
        }
        private string FormatProgramParticipation(PMData pd)
        {
            string programParticipation = string.Empty;
            SitePMData sitePmd = new SitePMData();

            try
            {
                if (pd.IsIWH == "true")
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePmd.programParticipationIWH : programParticipation + "; " + sitePmd.programParticipationIWH;
                }
                if (pd.IsCKCC == "true")
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePmd.programParticipationCKCC : programParticipation + "; " + sitePmd.programParticipationCKCC;
                }
                if (pd.IsKC365 == "true")
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePmd.programParticipationKC365 : programParticipation + "; " + sitePmd.programParticipationKC365;
                }
                if (pd.IsTeleKC365 == "true")
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePmd.programParticipationTelephonicKC365 : programParticipation + "; " + sitePmd.programParticipationTelephonicKC365;
                }

                return programParticipation;
            }
            
            catch (Exception ex)
            {
                logger.Error("Error: " + ex.Message);
                return null;
            }
        }
        public void SyncSiteDescription(string wUrl, string pracName)
        {
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.Load(web.ParentWeb);
                    clientContext.ExecuteQuery();

                    SiteInfoUtility siu = new SiteInfoUtility();
                    string strSiteDesc = GetSiteDescriptionData(siu.GetRootSite(wUrl) + web.ParentWeb.ServerRelativeUrl, pracName);

                    web.Description = strSiteDesc;
                    web.Update();
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    
                    logger.Information(strSiteDesc);
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("SyncSubSiteDescription", ex.Message, "Error", wUrl);
                }
            }
        }

        public string GetSiteDescriptionData(string wUrl, string SiteTitle)
        {
            string strDescription = "";
            string strParticipationValue = "";
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                try
                {
                    var web = clientContext.Web;
                    clientContext.Load(web.ParentWeb);
                    clientContext.ExecuteQuery();

                    List list = clientContext.Web.Lists.GetByTitle("AdminGroup");
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='PracticeName' /><Value Type='Text'>" + SiteTitle + "</Value></Eq></Where></Query></View>";
                    ListItemCollection items = list.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    if (items.Count > 0)
                    {
                        if (items[0].FieldValues["KCEArea"] != null)
                        {
                            strDescription = SiteTitle + " is a member of " + items[0].FieldValues["KCEArea"].ToString() + ". Program Participation: ";
                        }
                        else
                        {
                            strDescription = SiteTitle + ". Program Participation: ";
                        }
                        if (items[0].FieldValues["ProgramParticipation"] != null)
                        {
                            string[] strParticipationList = items[0].FieldValues["ProgramParticipation"].ToString().Split(';');
                            for (int intLoop = 0; intLoop < strParticipationList.Length; intLoop++)
                            {
                                strParticipationValue = strParticipationValue + " " + (intLoop + 1) + "." + strParticipationList[intLoop].ToString() + ";";
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("SyncSubSiteDescription", ex.Message, "Error", wUrl);
                }
            }
            strDescription = strDescription + " " + strParticipationValue;
            return strDescription;
        }
        public static List<PMData> GetPMData(string urlAdminGrp)
        {
            List<PMData> pmData = new List<PMData>();
            SitePMData sitePMData = new SitePMData();
            int cntRunAdminGroup = 0;

            using (ClientContext clientContext = new ClientContext(urlAdminGrp))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                List list = clientContext.Web.Lists.GetByTitle("AdminGroup");
                clientContext.Load(list);
                clientContext.ExecuteQuery();
                View view = list.Views.GetByTitle("All Links");

                clientContext.Load(view);
                clientContext.ExecuteQuery();
                CamlQuery query = new CamlQuery();
                query.ViewXml = view.ViewQuery;

                ListItemCollection items = list.GetItems(query);
                clientContext.Load(items);
                clientContext.ExecuteQuery();

                logger.Debug(SiteLogUtility.textLine0);
                logger.Debug("Total Count: " + items.Count);
                cntRunAdminGroup = items.Count;

                foreach (var item in items)
                {
                    PMData pmd = new PMData();
                    
                    logger.Information(item["PracticeTIN"] + " - " + item["PracticeName"] + " - " + item["ProgramParticipation"]);

                    pmd.PracticeName = item["PracticeName"].ToString();
                    pmd.PracticeTIN = item["PracticeTIN"].ToString();
                    pmd.SiteId = item["PracticeTIN"].ToString();
                    pmd.ProgramParticipation = item["ProgramParticipation"].ToString();

                    pmd.IsKC365 = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationKC365) ? "true" : "false";
                    pmd.IsTeleKC365 = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationTelephonicKC365) ? "true" : "false";
                    pmd.IsCKCC = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationCKCC) ? "true" : "false";
                    pmd.IsIWH = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationIWH) ? "true" : "false";

                    pmData.Add(pmd);
                }
            }

            return pmData;
        }
        
        public static string urlRelativeReferral_Prod = @"/bi/fhppp/portal/referral/";
        public static string urlRelativeReferral_Dev = @"/bi/fhppp/interimckcc/referral/";
        public static string LoadReferralParentWeb(PracticeSite site)
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

                    return urlReferralSite;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error with LoadParentWebTest: " + ex.Message);
                    return null;
                }
            }
        }
        public static string LoadParentWeb(string wUrl)
        {
            using (ClientContext clientContext = new ClientContext(wUrl))
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
                    string rootUrl = siu.GetRootSite(wUrl);

                    SiteLogUtility.Log_Entry("RootWeb: " + rootUrl, true);
                    SiteLogUtility.Log_Entry("ParentWeb: " + clientContext.Web.ParentWeb.ServerRelativeUrl, true);
                    SiteLogUtility.Log_Entry("PracticeWeb: " + clientContext.Web.ServerRelativeUrl, true);

                    return rootUrl + clientContext.Web.ParentWeb.ServerRelativeUrl;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error with LoadParentWeb: " + ex.Message);
                    return null;
                }
            }
        }

    }
}

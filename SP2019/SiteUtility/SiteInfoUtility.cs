﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using Microsoft.SharePoint.Client;
using Serilog;
using Microsoft.SharePoint.Client.WebParts;

namespace SiteUtility
{
    public enum PracticeType { IWH, iCKCC };
    public enum FolderType { IWH, iCKCC, BOTH };
    public enum SpServer { DEV, PROD };

    public class Practice
    {
        public string PMGroup;
        public string PMName;
        public string SiteID;
        public string Name;
        public string TIN;
        public string NPI;
        public string CKCCArea;
        public bool IsIWH;
        public bool IsCKCC;
        public bool IsKC365;
        public bool IsTelephonic;
        public string MedicalDirector;
        public string NewSiteUrl;
        public string ExistingSiteUrl;
        public string ProgramParticipation; 
        public PracticeType Type;
        public Practice()
        {
        }
    }
  
    public class SiteInfoUtility
    {
        public List<Practice> AllPractices;
        string strPortalSiteURL = ConfigurationManager.AppSettings["SP_SiteUrl"];

        public SiteInfoUtility()
        {
            AllPractices = new List<Practice>();

            // Read All Practice Info from [HealthCloud_NightlyProd].PORTAL.PracticeInfo_Deployed
            try
            {
                using (SqlConnection sqlConn = new SqlConnection())
                {
                    sqlConn.ConnectionString = "Data Source=" + ConfigurationManager.AppSettings["SqlServer"]
                        + "; Initial Catalog=" + ConfigurationManager.AppSettings["Database"] + "; Integrated Security=SSPI";
                    logger.Information("ConnectionString = " + sqlConn.ConnectionString);
                    Console.WriteLine("ConnectionString = " + sqlConn.ConnectionString);
                    string query = "SELECT * FROM [HealthCloud_NightlyProd].[PORTAL].[PracticeInfo_Deployed] WHERE IsActive = 1 ORDER BY GroupID";
                    sqlConn.Open();
                    SqlCommand getQuery = new SqlCommand(query, sqlConn);

                    using (SqlDataReader reader = getQuery.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Practice practice = new Practice();

                            if (reader["GroupID"].ToString().Length > 1)
                                practice.PMGroup = reader["GroupID"].ToString();
                            else 
                                practice.PMGroup = '0' + reader["GroupID"].ToString();

                            practice.PMName = reader["ProgramManager"].ToString();
                            practice.Name = reader["PracticeName"].ToString();
                            practice.SiteID = reader["SiteID"].ToString();
                            practice.TIN = reader["PracticeTIN"].ToString();
                            practice.NPI = reader["PracticeNPI"].ToString();
                            practice.NewSiteUrl = strPortalSiteURL + "/PM" + practice.PMGroup + "/" + practice.SiteID + "/";

                            practice.CKCCArea = reader["CKCCArea"].ToString();

                            if (practice.CKCCArea == "")
                                practice.IsCKCC = false;
                            else
                                practice.IsCKCC = true;

                            practice.IsIWH = (bool)reader["IWNRegion"];
                            practice.IsKC365 = (bool)reader["KC365"];
                            practice.IsTelephonic = (bool)reader["IsTelephonic"];

                            practice.MedicalDirector = reader["MedicalDirector"].ToString();

                            AllPractices.Add(practice);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public List<Practice> GetAllPractices()
        {
            return AllPractices;
        }
        public List<Practice> GetAllIWHPractices()
        {
            List<Practice> AllIWHSite = new List<Practice>();
            return AllPractices.Where(p => p.IsIWH).ToList();
        }
        public List<Practice> GetAllCKCCPractices()
        {
            List<Practice> AllIWHSite = new List<Practice>();
            return AllPractices.Where(p => p.IsCKCC).ToList();
        }
        public List<Practice> GetAllKC365Practices()
        {
            List<Practice> AllIWHSite = new List<Practice>();
            return AllPractices.Where(p => p.IsKC365).ToList();
        }
        public List<Practice> GetAllTelephonicPractices()
        {
            List<Practice> AllIWHSite = new List<Practice>();
            return AllPractices.Where(p => p.IsTelephonic).ToList();
        }
        public List<Practice> GetAllMedicalDirectorPractices()
        {
            List<Practice> AllIWHSite = new List<Practice>();
            return AllPractices.Where(p => p.MedicalDirector != "").ToList();
        }
        public List<Practice> GetPracticesByPM(string pmGroup)
        {
            List<Practice> AllIWHSite = new List<Practice>();
            return AllPractices.Where(p => p.PMGroup == pmGroup).ToList();
        }
        public Practice GetPracticeBySiteID(string siteID)
        {
            return AllPractices.Where(p => p.SiteID == siteID).FirstOrDefault();
        }
        public Practice GetPracticeByTIN(string tin)
        {
            return AllPractices.Where(p => p.TIN == tin).FirstOrDefault();
        }
        public Practice GetPracticeByNPI(string npi)
        {
            return AllPractices.Where(p => p.NPI == npi).FirstOrDefault();
        }



        static ILogger logger = Log.ForContext<SiteInfoUtility>();
        //public List<Practice> practicesIWH = new List<Practice>();
        //public List<Practice> practicesCKCC = new List<Practice>();
        public static List<ProgramManagerSite> getSubWebs(string path, string rootUrl)
        {
            List<ProgramManagerSite> pmSites = new List<ProgramManagerSite>();
            List<PracticeSite> practices = new List<PracticeSite>();
            try
            {
                using (ClientContext ctx = new ClientContext(path))
                {
                    Web web = ctx.Web;
                    ctx.Load(web, w => w.Webs,
                                       w => w.Title,
                                       w => w.Description,
                                       w => w.ServerRelativeUrl,
                                       w => w.Url,
                                       w => w.Navigation);
                    ctx.ExecuteQuery();

                    foreach (Web w in web.Webs)
                    {
                        string newpath = rootUrl + w.ServerRelativeUrl;
                        Console.WriteLine(newpath);

                        getSubWebs(newpath, rootUrl);

                        PracticeSite prac = new PracticeSite();
                        prac.Name = w.Title;
                        prac.URL = w.Url;
                        practices.Add(prac);
                    }
                    ProgramManagerSite pmsite = new ProgramManagerSite();
                    pmsite.URL = web.Url;
                    pmSites.Add(pmsite);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return pmSites;
        }

        public static void GetPMPracticeDetails(ClientContext clientContext)
        {
            clientContext.Load(clientContext.Web.Webs);
            clientContext.ExecuteQuery();

            foreach (Web web in clientContext.Web.Webs)
            {
                if (Char.IsDigit(web.Url.Last()))
                {
                    using (ClientContext clientContext0 = new ClientContext(web.Url))
                    {
                        clientContext0.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                        clientContext0.Load(clientContext0.Web.Webs);
                        clientContext0.ExecuteQuery();

                        foreach (Web web0 in clientContext0.Web.Webs)
                        {
                            //Practice practice = new Practice();
                            //practice.Name = web0.Title;
                            //practice.Url = web0.Url;
                            //practice.Type = practiceType;
                            //Practices.Add(practice);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// SP_GetAll_PMData will read the AdminGroup SharePoint List for a given Program Manager site and a single Practice site
        ///     and return a List of Type PMData
        /// </summary>
        /// <param name="urlAdminGrp"></param>
        /// <param name="strSiteID"></param>
        /// <returns>List of Type PMData</returns>
        public static List<PMData> SP_GetAll_PMData(string urlAdminGrp, string strSiteID)
        {
            List<PMData> pmData = new List<PMData>();
            SitePMData sitePMData = new SitePMData();

            using (ClientContext clientContext = new ClientContext(urlAdminGrp))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                List list = clientContext.Web.Lists.GetByTitle("AdminGroup");
                clientContext.Load(list);
                clientContext.ExecuteQuery();
                View view = list.Views.GetByTitle("All Links");

                clientContext.Load(view);
                clientContext.ExecuteQuery();
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='PracticeTIN' /><Value Type='Text'>" + strSiteID + "</Value></Eq></Where></Query></View>";

                ListItemCollection items = list.GetItems(camlQuery);
                clientContext.Load(items);
                clientContext.ExecuteQuery();
                //   SiteLogUtility.Log_Entry("Total Count: " + items.Count, true);

                foreach (var item in items)
                {
                    PMData pmd = new PMData();


                    //   SiteLogUtility.Log_Entry(item["PracticeTIN"] + " - " + item["PracticeName"] + " - " + item["ProgramParticipation"], true);

                    pmd.PracticeName = item["PracticeName"].ToString();
                    pmd.PracticeTIN = item["PracticeTIN"].ToString();
                    pmd.SiteId = item["PracticeTIN"].ToString();
                    pmd.ProgramParticipation = item["ProgramParticipation"].ToString();

                    pmd.IsKC365 = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationKC365) ? "true" : "false";
                    pmd.IsCKCC = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationCKCC) ? "true" : "false";
                    pmd.IsIWH = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationIWH) ? "true" : "false";

                    pmData.Add(pmd);
                }
            }

            return pmData;
        }

        /// <summary>
        /// SP_GetAll_PMData_All will read the AdminGroup SharePoint List for a given Program Manager site and the PM's Practice sites
        ///     and return a List of Type PMData
        /// </summary>
        /// <param name="urlAdminGrp"></param>
        /// <returns>List of Type PMData</returns>
        public static List<PMData> SP_GetAll_PMData_All(string urlAdminGrp)
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


                    SiteLogUtility.Log_Entry(item["PracticeTIN"] + " - " + item["PracticeName"] + " - " + item["ProgramParticipation"], true);
                    logger.Debug(item["PracticeTIN"] + " - " + item["PracticeName"] + " - " + item["ProgramParticipation"]);

                    pmd.PracticeName = item["PracticeName"].ToString();
                    pmd.PracticeTIN = item["PracticeTIN"].ToString();
                    pmd.SiteId = item["PracticeTIN"].ToString();
                    pmd.ProgramParticipation = item["ProgramParticipation"].ToString();

                    pmd.IsKC365 = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationKC365) ? "true" : "false";
                    pmd.IsCKCC = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationCKCC) ? "true" : "false";
                    pmd.IsIWH = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationIWH) ? "true" : "false";
                    pmd.IsTeleKC365 = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationTelephonicKC365) ? "true" : "false";

                    pmData.Add(pmd);
                }
            }

            return pmData;
        }
        public static List<PMData> initPMDataToList(string adminGroupUrl)
        {
            List<PMData> pmData = new List<PMData>();
            try
            {
                pmData = SP_GetPortalData_PMData(adminGroupUrl);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("initPMDataToList", ex.Message, "Error", "");
            }
            return pmData;
        }
        public static List<PMData> SP_GetPortalData_PMData(string adminGroupUrl)
        {
            List<PMData> All_PortalData = new List<PMData>();
            try
            {
                All_PortalData = SP_GetAll_PMData_All(adminGroupUrl);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("SP_GetPortalData_PMData", ex.Message, "Error", "");
            }

            return All_PortalData;
        }
        public static List<ProgramManagerSite> GetAllPracticeDetails(ClientContext clientContext, List<Practice> pracIWH=null, List<Practice> pracCKCC = null, List<PMData> pmData = null)
        {
            clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

            clientContext.Load(clientContext.Web.Webs);
            clientContext.ExecuteQuery();
            string strUrl = clientContext.Url;

            List<ProgramManagerSite> pmSites = new List<ProgramManagerSite>();

            try
            {
                foreach (Web web in clientContext.Web.Webs)
                {
                    if (web.Url.Contains("admingroup01") == false && Char.IsDigit(web.Url.Last()))
                    {
                        PmAssignment pmAssignments = GetPM(web.Url);
                        ProgramManagerSite pmSite = new ProgramManagerSite();
                        pmSite.ProgramManagerName = pmAssignments.PMName;
                        pmSite.PMURL = web.Url;
                        pmSite.URL = web.Url;
                        pmSite.ProgramManager = pmAssignments.PMRefId;
                        pmSite.IWNSiteMgrPermission = pmAssignments.PMGroup + "_SiteManager";
                        pmSite.IWNSiteMgrReadOnlyPermission = pmAssignments.PMGroup + "_ReadOnly";
                        pmSite.PracticeSiteCollection = new List<PracticeSite>();

                        //logger.Debug(SiteLogUtility.textLine);
                        //logger.Debug($"{pmSite.ProgramManagerName} - {pmSite.ProgramManager}");
                        //logger.Debug(pmSite.PMURL);

                        using (ClientContext clientContext0 = new ClientContext(web.Url))
                        {
                            clientContext0.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                            clientContext0.Load(clientContext0.Web.Webs);
                            clientContext0.ExecuteQuery();

                            foreach (Web web0 in clientContext0.Web.Webs)
                            {
                                string siteId = GetPracSiteName(web0.Url);
                                string siteId0 = siteId;
                                siteId = DecryptPTIN(siteId);
                                PracticeSite practiceSite = new PracticeSite();
                                practiceSite.Name = web0.Title;
                                practiceSite.URL = web0.Url;
                                practiceSite.PracticeTIN = siteId;
                                practiceSite.SiteId = siteId0;
                                practiceSite.PracUserPermission = $"Prac_{siteId}_User";
                                practiceSite.PracUserReadOnlyPermission = $"Prac_{siteId}_ReadOnly";
                                //practiceSite.ExistingSiteUrl = MapExistingSite(practiceSite.PracticeTIN, pracIWH, pracCKCC);
                                practiceSite.ProgramParticipation = MapProgramParticipation(siteId0, pmData);
                                
                                PMData pMData = MapPMData(siteId0, pmData);
                                practiceSite.ProgramParticipation = pMData == null ? "" : pMData.ProgramParticipation;
                                practiceSite.IsIWH = pMData == null ? "" : pMData.IsIWH;
                                practiceSite.IsCKCC = pMData == null ? "" : pMData.IsCKCC;
                                practiceSite.IsKC365 = pMData == null ? "" : pMData.IsKC365;
                                practiceSite.IsTeleKC365 = pMData == null ? "" : pMData.IsTeleKC365;

                                practiceSite.siteType = GetSiteType(practiceSite.IsIWH, practiceSite.IsCKCC, practiceSite.IsKC365, practiceSite.IsTeleKC365);

                                pmSite.PracticeSiteCollection.Add(practiceSite);

                                //logger.Debug(practiceSite.Name);
                                //logger.Debug(practiceSite.URL);
                                //logger.Debug(practiceSite.ProgramParticipation);
                            }
                        }

                        if (pmSite.PMURL.Contains("admingroup01") == false)
                        {
                            pmSites.Add(pmSite);
                            //logger.Debug($"Total Practices:  {pmSite.PracticeSiteCollection.Count}");
                        } 
                    }
                    
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetAllPracticeDetails", ex.Message, "Error", strUrl);
                throw;
            }
            
            SiteLogUtility.Log_Entry("1. GetAllPracticeDetails - Complete", true);
            return pmSites;
        }

        private static string GetSiteType(string isIWH, string isCKCC, string isKC365, string isTeleKC365)
        {
            string siteType = "";
            try
            {
                if (isIWH == "true")
                {
                    siteType = "iwh";
                }
                if (isCKCC == "true")
                {
                    siteType = siteType + "ckcc";
                }
                if (isKC365 == "true")
                {
                    siteType = siteType + "kc365";
                }
                if (isTeleKC365 == "true")
                {
                    siteType = siteType + "telekc365";
                }
                return siteType;
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetSiteType", ex.Message, "Error", "");
                return "";
            }
        }


        /// <summary>
        /// This method is called by GetPM(string sUrl).
        /// 
        /// Returns a List of PmAssignment class which will be used as a cross-reference table to find a match from the PM SiteName.
        /// EXAMPLE:
        ///   If the URL is https://sharepointdev.fmc-na-icg.com/bi/fhppp/portal/PracticeSite20_PM01
        ///   the last two characters of the URL (01 in this example) will be compared to the PMRefId and return the match.
        ///   
        ///   The PMGroup is hard-coded with 'IWNRegionXX' so it can be used with the existing SPGroups
        /// </summary>
        /// <returns> List<PmAssignment> </returns>
        private static List<PmAssignment> GetPmAssignments()
        {
            List<PmAssignment> pmAssignments = new List<PmAssignment>();

            PmAssignment pma1 = new PmAssignment() { PMRefId = "01", PMName = "Angela Korf", PMGroup = "IWNRegion01" };
            PmAssignment pma2 = new PmAssignment() { PMRefId = "02", PMName = "Annie Fambro", PMGroup = "IWNRegion02" };
            PmAssignment pma3 = new PmAssignment() { PMRefId = "03", PMName = "Francisco Calles", PMGroup = "IWNRegion03" };
            PmAssignment pma4 = new PmAssignment() { PMRefId = "04", PMName = "Glenda S Wooten", PMGroup = "IWNRegion04" };
            PmAssignment pma5 = new PmAssignment() { PMRefId = "05", PMName = "Karen Bauer", PMGroup = "IWNRegion05" };
            PmAssignment pma6 = new PmAssignment() { PMRefId = "06", PMName = "Katelyn Minnick", PMGroup = "IWNRegion06" };
            PmAssignment pma7 = new PmAssignment() { PMRefId = "07", PMName = "Linda S Biermann", PMGroup = "IWNRegion07" };
            PmAssignment pma8 = new PmAssignment() { PMRefId = "08", PMName = "Marion C Grantham", PMGroup = "IWNRegion08" };
            PmAssignment pma9 = new PmAssignment() { PMRefId = "09", PMName = "Thomas Locke", PMGroup = "IWNRegion09" };
            PmAssignment pma10 = new PmAssignment() { PMRefId = "10", PMName = "Kristina Dunigan", PMGroup = "IWNRegion10" };

            pmAssignments.Add(pma1);
            pmAssignments.Add(pma2);
            pmAssignments.Add(pma3);
            pmAssignments.Add(pma4);
            pmAssignments.Add(pma5);
            pmAssignments.Add(pma6);
            pmAssignments.Add(pma7);
            pmAssignments.Add(pma8);
            pmAssignments.Add(pma9);
            pmAssignments.Add(pma10);

            return pmAssignments;
        }

        /// <summary>
        /// This method gets the Program Manager cross-reference data
        /// and the SiteName, to return a PmAssignment class.
        /// 
        /// Needed a method to get the last two characters of the Program Manager URL and return
        /// the Program Manager Name and the name of the SPGroup to be used for permissions.
        /// </summary>
        /// <param name="sUrl"></param>
        /// <returns> PmAssignment class </returns>
        public static PmAssignment GetPM(string sUrl)
        {
            string siteName = string.Empty;
            List<PmAssignment> pmAssignments = new List<PmAssignment>();

            try
            {
                pmAssignments = GetPmAssignments();
                siteName = GetSiteName(sUrl);

                int sStart = siteName.Length - 2;
                string PMid = siteName.Substring(sStart, 2);
                PmAssignment result = pmAssignments.Find(x => x.PMRefId == PMid);

                return result;
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetPM", ex.Message, "Error", "");
                throw;
            }

            
        }

        public class PmAssignment
        {
            public PmAssignment()
            {

            }

            public string PMRefId { get; set; }
            public string PMName { get; set; }
            public string PMGroup { get; set; }
        }

        public static string GetSiteName(string sUrl)
        {
            Uri pracUrl = new Uri(sUrl);
            int segCnt = pracUrl.Segments.Count();
            string siteName = segCnt > 4 ? pracUrl.Segments.Last() : string.Empty;

            return siteName;
        }
        public static string GetPMUrl(string pracUrl)
        {
            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();
            try
            {
                string pmUrl = siteInfoUtility.GetRootSite(pracUrl) + siteInfoUtility.GetRelativeParentWeb(pracUrl);
                return pmUrl;
            }
            catch (Exception ex)
            {
                logger.Information(ex.Message);
                SiteLogUtility.CreateLogEntry("GetPMRef", ex.Message, "Error", "");
                return string.Empty;
            }
        }
        public static string GetPMRef(string sUrl)
        {
            try
            {
                Uri pracUrl = new Uri(sUrl);
                string pmRef = string.Empty;
                
                foreach (var item in pracUrl.Segments)
                {
                    if (item.StartsWith("PM"))
                    {
                        pmRef = item.Substring(0, 4);
                        continue;
                    }
                }
                return pmRef;
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetPMRef", ex.Message, "Error", "");
                return string.Empty;
            }
        }

        public static string GetPracSiteName(string sUrl)
        {
            Uri pracUrl = new Uri(sUrl);
            int segCnt = pracUrl.Segments.Count();
            string siteName = segCnt > 4 ? pracUrl.Segments.Last() : string.Empty;

            return siteName;
        }
        public string GetSiteDescriptionData(string wUrl, string sTitle)
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
                    query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='PracticeName' /><Value Type='Text'>" + sTitle + "</Value></Eq></Where></Query></View>";
                    ListItemCollection items = list.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    if (items.Count > 0)
                    {
                        if (items[0].FieldValues["KCEArea"] != null)
                        {
                            strDescription = sTitle + " is a member of " + items[0].FieldValues["KCEArea"].ToString() + ". Program Participation: ";
                        }
                        else
                        {
                            strDescription = sTitle + ". Program Participation: ";
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
                    SiteLogUtility.CreateLogEntry("GetSiteDescriptionData", ex.Message, "Error", "");
                }
            }
            strDescription = strDescription + " " + strParticipationValue;
            return strDescription;
        }

        public string FormatSiteDescriptionData(string wUrl, string sTitle, Practice practice)
        {
            string strParticipationNew = FormatProgramParticipation(practice);
            string strDescription = string.Empty;
            string strParticipationValue = string.Empty;

            try
            {
                if (practice.CKCCArea != null)
                {
                    strDescription = sTitle + " is a member of " + practice.CKCCArea.ToString() + ". Program Participation: ";
                }
                else
                {
                    strDescription = sTitle + ". Program Participation: ";
                }
                if (strParticipationNew != null)
                {
                    string[] strParticipationList = strParticipationNew.ToString().Split(';');
                    for (int intLoop = 0; intLoop < strParticipationList.Length; intLoop++)
                    {
                        strParticipationValue = strParticipationValue + " " + (intLoop + 1) + "." + strParticipationList[intLoop].ToString() + ";";
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetSiteDescriptionData", ex.Message, "Error", "");
            }

            strDescription = strDescription + " " + strParticipationValue;
            return strDescription;
        }

        public string FormatProgramParticipation(Practice practice)
        {
            string strDesc = string.Empty;
            string programParticipation = string.Empty;
            SitePMData sitePMData = new SitePMData();
            try
            {
                if (practice.IsIWH)
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePMData.programParticipationIWH : programParticipation + "; " + sitePMData.programParticipationIWH;
                }
                if (practice.IsCKCC)
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePMData.programParticipationCKCC : programParticipation + "; " + sitePMData.programParticipationCKCC;
                }
                if (practice.IsKC365)
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePMData.programParticipationKC365 : programParticipation + "; " + sitePMData.programParticipationKC365;
                }
                if (practice.IsTelephonic)
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePMData.programParticipationTelephonicKC365 : programParticipation + "; " + sitePMData.programParticipationTelephonicKC365;
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("FormatProgramParticipation", ex.Message, "Error", "");
            }
            return programParticipation;
        }

        public static string GetProgramParticipation(Practice practice)
        {
            string strDesc = string.Empty;
            string programParticipation = string.Empty;
            SitePMData sitePMData = new SitePMData();
            try
            {
                if (practice.IsIWH)
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePMData.programParticipationIWH : programParticipation + "; " + sitePMData.programParticipationIWH;
                }
                if (practice.IsCKCC)
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePMData.programParticipationCKCC : programParticipation + "; " + sitePMData.programParticipationCKCC;
                }
                if (practice.IsKC365)
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePMData.programParticipationKC365 : programParticipation + "; " + sitePMData.programParticipationKC365;
                }
                if (practice.IsTelephonic)
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePMData.programParticipationTelephonicKC365 : programParticipation + "; " + sitePMData.programParticipationTelephonicKC365;
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("FormatProgramParticipation", ex.Message, "Error", "");
            }
            return programParticipation;
        }

        public void SyncSubSiteDescription(string wUrl, string psiteTitle)
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

                    string strSiteDesc = GetSiteDescriptionData(GetRootSite(wUrl) + web.ParentWeb.ServerRelativeUrl, psiteTitle);

                    web.Description = strSiteDesc;
                    web.Update();
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("SyncSubSiteDescription", ex.Message, "Error", "");
                }
            }
        }

        public void Init_UpdateAllProgramParticipation(Practice practice)
        {
            SitePMData sitePMData = new SitePMData();
            SiteListUtility siteListUtility = new SiteListUtility();
            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();

            try
            {
                string wUrl = practice.NewSiteUrl;
                string pmUrl = siteInfoUtility.GetRootSite(wUrl) + siteInfoUtility.GetRelativeParentWeb(wUrl);

                string pracName = SitePMData.formateSiteName(practice.Name);
                pracName = SitePMData.formateSiteName(pracName);
                practice.Name = pracName;

                string pracCKCCArea = SitePMData.formateSiteName(practice.CKCCArea);
                pracCKCCArea = SitePMData.formateSiteName(pracCKCCArea);
                practice.CKCCArea = pracCKCCArea;

                if (practice.CKCCArea == "Nsipa")
                {
                    practice.CKCCArea = "NSIPA";
                }

                practice.ProgramParticipation = siteInfoUtility.FormatProgramParticipation(practice);
                
                //Test...
                //var lastIndex1 = pmUrl.LastIndexOf('/');
                //var lastIndex2 = pmUrl.Split('/')[pmUrl.Split('/').Length - 1];

                //PM Site...
                if (siteListUtility.CheckAdminList(wUrl, practice.SiteID) == false)
                {
                    siteListUtility.List_AddAdminListItem(pmUrl, practice);
                }
                else
                {
                    siteListUtility.List_UpdateAdminListItem(pmUrl, practice);
                }

                //Admin Site...
                string adminUrl = pmUrl.Substring(0, pmUrl.LastIndexOf('/'));
                if (siteListUtility.CheckAdminList(pmUrl, practice.SiteID, pmUrl) == false)
                {
                    siteListUtility.List_AddAdminListItem(adminUrl, practice, pmUrl);
                }
                else
                {
                    siteListUtility.List_UpdateAdminListItem(adminUrl.Substring(0, pmUrl.LastIndexOf('/')), practice, pmUrl);
                }

                //Practice Site...
                UpdateSiteSettingsDesc(wUrl, pracName, practice);

            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_UpdateAllProgramParticipation", ex.Message, "Error", "");
            }
        }
        public static int gridHeight(Practice site)
        {
            int intCount = -1;
            int[] intHeight = new int[5] { 156, 253, 350, 447, 544 };
            try
            {
                if (site.IsIWH)
                {
                    intCount++;  // Payor Program Education Resources...
                }
                if (site.IsCKCC)
                {
                    intCount++;  // CKCC/KCE Resources...
                    intCount++;  // Patient Status Updates...
                }
                if (site.IsKC365)
                {
                    intCount++;  // Payor Enrollment...
                }
                if (site.IsTelephonic)
                {
                    intCount++;  // CKCC/KCE Engagement...
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("gridHeight", ex.Message, "Error", "");
            }
            return intHeight[intCount];
        }
        public static bool modifyWebPartProgramParticipation(string webUrl, Practice practiceSite)
        {
            SiteLogUtility slu = new SiteLogUtility();
            bool outcome = false;
            string clink = string.Empty;
            int webPartHeight = gridHeight(practiceSite);

            using (ClientContext clientContext = new ClientContext(webUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                Web web = clientContext.Web;
                clientContext.Load(web, w => w.ServerRelativeUrl);
                clientContext.ExecuteQuery();
                var file = clientContext.Web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + "/Pages/ProgramParticipation.aspx");
                file.CheckOut();
                try
                {
                    clientContext.Load(file);
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    try
                    {
                        LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);
                        clientContext.Load(limitedWebPartManager.WebParts,
                            wps => wps.Include(
                                wp => wp.WebPart.Title,
                                wp => wp.WebPart.Properties));
                        //clientContext.Load(limitedWebPartManager.WebParts);
                        clientContext.ExecuteQuery();

                        if (limitedWebPartManager.WebParts.Count == 0)
                        {
                            throw new Exception("No Webparts on this page.");
                        }

                        foreach (WebPartDefinition webPartDefinition1 in limitedWebPartManager.WebParts)
                        {
                            clientContext.Load(webPartDefinition1.WebPart.Properties);
                            clientContext.ExecuteQuery();

                            if (webPartDefinition1.WebPart.Title.Equals("Data Exchange"))
                            {
                                //webPartDefinition1.WebPart.Properties["Title"] = "ProgramParticipation";
                                webPartDefinition1.WebPart.Properties["Height"] = webPartHeight.ToString();
                                //webPartDefinition1.WebPart.Properties["ChromeType"] = 2;
                                webPartDefinition1.SaveWebPartChanges();
                                
                                slu.LoggerInfo_Entry("Adjusted WebPart Height: " + webPartHeight.ToString());
                            }
                        }

                        //WebPartDefinition webPartDefinition = limitedWebPartManager.WebParts[0];
                        //WebPart webPart = webPartDefinition.WebPart;
                        //webPart.Title = "Program Participation";
                        //webPartDefinition.SaveWebPartChanges();
                        //clientContext.ExecuteQuery();

                        file.CheckIn("Updating webparts", CheckinType.MajorCheckIn);
                        file.Publish("Updating webparts");
                        clientContext.Load(file);
                        web.Update();
                        clientContext.ExecuteQuery();
                        outcome = true;
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("modifyWebPart", ex.Message, "Error", "");
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("modifyWebPart", ex.Message, "Error", "");
                    outcome = false;
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
            return outcome;
        }
        public string GetSiteSettingsDesc(string wUrl)
        {
            SiteLogUtility siteLogUtility = new SiteLogUtility();

            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.Load(web.ParentWeb);
                    clientContext.ExecuteQuery();

                    siteLogUtility.LoggerInfo_Entry("SiteTitle: " + web.Title);
                    siteLogUtility.LoggerInfo_Entry(" SiteDesc: " + web.Description);

                    return web.Title;
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("UpdatePmAdminGrp", ex.Message, "Error", "");
                    return "ERROR - Title Not Found";
                }
            }
        }
        public void UpdateSiteSettingsDesc(string wUrl, string psiteTitle, Practice practice)
        {
            SiteLogUtility siteLogUtility = new SiteLogUtility();

            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.Load(web.ParentWeb);
                    clientContext.ExecuteQuery();

                    siteLogUtility.LoggerInfo_Entry("SiteDesc BEFORE: " + web.Description);
                    string strSiteDesc = FormatSiteDescriptionData(GetRootSite(wUrl) + web.ParentWeb.ServerRelativeUrl, psiteTitle, practice);
                    web.Description = strSiteDesc;
                    siteLogUtility.LoggerInfo_Entry("SiteDesc  AFTER: " + web.Description);

                    web.Update();
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("UpdatePmAdminGrp", ex.Message, "Error", "");
                }
            }
        }

        public string GetRootSite(string url)
        {
            Uri uri = new Uri(url.TrimEnd(new[] { '/' }));
            return $"{uri.Scheme}://{ uri.DnsSafeHost}";
        }

        public string GetRelativeParentWeb(string strUrl)
        {
            using (ClientContext clientContext = new ClientContext(strUrl))
            {
                string strParentWeb = string.Empty;

                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                try
                {
                    clientContext.Load(clientContext.Web, web => web.ParentWeb.ServerRelativeUrl);
                    clientContext.ExecuteQuery();

                    strParentWeb = clientContext.Web.ParentWeb.ServerRelativeUrl;
                    clientContext.Web.ServerRelativeUrl = "";
                    clientContext.Web.Update();
                    
                    return strParentWeb;
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("GetParentWeb", ex.Message, "Error", "");
                    return string.Empty;
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
        public static string GetReferralUrl(string sUrl)
        {
            SiteInfoUtility siu = new SiteInfoUtility();
            try
            {
                string rootSite = siu.GetRootSite(sUrl);
                string urlReferralSiteAssets = string.Empty;

                // "https://sharepoint.fmc-na-icg.com/bi/fhppp/portal/referral";
                // "https://sharepointdev.fmc-na-icg.com/bi/fhppp/interimckcc/referral";

                if (rootSite.Contains("sharepointdev"))
                {
                    urlReferralSiteAssets = @"https://sharepointdev.fmc-na-icg.com/bi/fhppp/interimckcc/referral";
                }
                else
                {
                    urlReferralSiteAssets = @"https://sharepoint.fmc-na-icg.com/bi/fhppp/portal/referral";
                }

                return urlReferralSiteAssets;
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetReferralUrl", ex.Message, "Error", "");
                return string.Empty;
            }
        }

        public static string DecryptPTIN(string s)
        {
            try
            {
                int sLen = s.Length;
                string sFirst = s.Substring(0, 1);
                string sLast = s.Substring(sLen - 1, 1);

                if (sFirst.Equals("9") && sLast.Equals("9"))
                {
                    s = s.Substring(1, sLen - 1);
                    sLen = s.Length;
                    s = s.Substring(0, sLen - 1);
                }

                char[] charArray = s.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("DecryptPTIN", ex.Message, "Error", "");
                return s;
            }
        }

        public static List<Practice> GetAllPracticeExistingSites(ClientContext clientContext, List<Practice> practices, PracticeType practiceType)
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

        private static string MapExistingSite(string TIN, List<Practice> practicesIWH=null, List<Practice> practicesCKCC=null)
        {
            Practice practice = practicesIWH.Where(p => p.ExistingSiteUrl.Contains(TIN)).FirstOrDefault();
            if (practice == null)
                practice = practicesCKCC.Where(p => p.ExistingSiteUrl.Contains(TIN)).FirstOrDefault();

            if (practice == null)
            {
                //Console.WriteLine(TIN);
                SiteLogUtility.Log_Entry("Mapping Does Not Exist: " + TIN, true);
                return "";
            }
            else
                return practice.ExistingSiteUrl;
        }

        private static string MapProgramParticipation(string TIN, List<PMData> pmd=null)
        {
            try
            {
                PMData pmData = pmd.Where(p => p.SiteId.Contains(TIN)).FirstOrDefault();
                if (pmData == null)
                {
                    //SiteLogUtility.Log_Entry("Program Participation Does Not Exist: " + TIN, true);
                    return "";
                }
                else
                {
                    //SiteLogUtility.Log_Entry(SiteLogUtility.textLine0);
                    //SiteLogUtility.Log_Entry(TIN + " - " + pmData.ProgramParticipation, true);
                    return pmData.ProgramParticipation;
                }
            }
            catch (Exception ex)
            {
                //SiteLogUtility.CreateLogEntry("MapProgramParticipation", ex.Message, "Error", "");
                //SiteLogUtility.Log_Entry("Error - Program Participation Does Not Exist: " + TIN, true);
                return "";
            }
        }

        private static PMData MapPMData(string TIN, List<PMData> pmd = null)
        {
            try
            {
                PMData pmDataReturn = pmd
                    .Where(p => p != null)
                    .Where(p => p.SiteId.Contains(TIN)).FirstOrDefault();
                if (pmDataReturn == null)
                {
                    //SiteLogUtility.Log_Entry("Program Participation Does Not Exist: " + TIN, true);
                    return null;
                }
                else
                {
                    //SiteLogUtility.Log_Entry(TIN + " - " + pmDataReturn.ProgramParticipation, true);
                    return pmDataReturn;
                }
            }
            catch (Exception ex)
            {
                //SiteLogUtility.CreateLogEntry("MapProgramParticipation", ex.Message, "Error", "");
                //SiteLogUtility.Log_Entry("Error - Program Participation Does Not Exist: " + TIN, true);
                return null;
            }
        }

        /// <summary>
        /// Method will receive CSV file input
        /// Utilize existing Classes
        /// </summary>
        public static class GenericTextFileProcessor
        {
            public static List<T> LoadFromTextFile<T>(string filePath) where T : class, new()
            {
                var lines = System.IO.File.ReadAllLines(filePath).ToList();
                List<T> output = new List<T>();
                T entry = new T();
                var cols = entry.GetType().GetProperties();

                // Checks to be sure we have at least one header row and one data row...
                if (lines.Count < 2)
                {
                    throw new IndexOutOfRangeException("The file was either empty or missing.");
                }

                // Splits the header into one column header per entry...
                var headers = lines[0].Split(',');

                // Removes header row from the lines so we don't
                //  have to worry about skipping over that first row.
                lines.RemoveAt(0);

                foreach (var row in lines)
                {
                    entry = new T();
                    var vals = row.Split(',');

                    for (int i = 0; i < headers.Length; i++)
                    {
                        foreach (var col in cols)
                        {
                            if (col.Name == headers[i])
                            {
                                col.SetValue(entry, Convert.ChangeType(vals[i], col.PropertyType));
                            }
                        }
                    }

                    output.Add(entry);
                }

                return output;
            }
        }

        public static void loadFromTextFile()
        {
            // Load records to process into PracticeSite...
            //List<PracticeSite> newSiteInfo = SiteInfoUtility.GenericTextFileProcessor.LoadFromTextFile<PracticeSite>(siteInfoFile);
            //SiteLogUtility.Log_Entry("Will be processed: ");
            //foreach (var item in newSiteInfo)
            //{
            //    Console.WriteLine($"{item.URL}, {item.Name}");
            //    SiteLogUtility.Log_Entry($"{item.URL}, {item.Name}");
            //}
            //Console.ReadLine();
        }

        public bool PrintProgramParticipationGroupTotal(List<Practice> practices)
        {
            SiteLogUtility siteLogUtility = new SiteLogUtility();
            try
            {
                var groupPerProgram = practices
                                .GroupBy(u => u.ProgramParticipation)
                                .Select(grp => new
                                {
                                    Program = grp.Key,
                                    Count = grp.Count(),
                                    pmData = grp.ToList()
                                })
                                .OrderBy(pp => pp.Program)
                                .ToList();

                foreach (var item in groupPerProgram)
                {
                    siteLogUtility.LoggerInfo_Entry(item.Program + " = " + item.pmData.Count().ToString(), true);
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("PrintParticipationGroupTotal", ex.Message, "Error", "");
                return false;
            }

            return true;
        }
        public static string GetPortalUrl()
        {
            try
            {
                return ConfigurationManager.AppSettings["SP_SiteUrl"];
            }
            catch (Exception ex)
            {
                logger.Information(ex.Message);
                SiteLogUtility.CreateLogEntry("PrintParticipationGroupTotal", ex.Message, "Error", "");
                return null;
            }
        }
    }
}

using System;
using System.Data;
using System.Collections.Generic;
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

namespace Release_1_4
{
    class Program
    {
        static public List<Practice> practicesIWH = new List<Practice>();
        static public List<Practice> practicesCKCC = new List<Practice>();
        static void Main(string[] args)
        {
            string releaseName = "OptimalStartPage";
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            string srcUrlIWH = ConfigurationManager.AppSettings["SP_IWHUrl"];
            string srcUrlCKCC = ConfigurationManager.AppSettings["SP_CKCCUrl"];
            string siteInfoFile = ConfigurationManager.AppSettings["Csv_File"];

            string pageName = "OptimalStart";
            string urlSiteAssets = @"https://sharepointdev.fmc-na-icg.com/bi/fhppp/interimckcc/referral";
            string urlAdminGroup = @"https://sharepointdev.fmc-na-icg.com/bi/fhppp/portal/PM02";

            SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            SiteLogUtility.Log_Entry("\n\n=============Release Starts=============", true);

            List<PMData> pmData = initPMDataToList(urlAdminGroup);

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    SiteLogUtility.Log_Entry("\n\n=============[ Get all Portal Practice Data ]=============", true);
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext, practicesIWH, practicesCKCC, pmData);

                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - Start]=============", true);
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        foreach (PracticeSite psite in pm.PracticeSiteCollection)
                        {
                            if (psite.URL.Contains("98617280649") && psite.IsCKCC.Equals("true"))
                            //if (psite.URL.Contains("98617280649") && psite.ProgramParticipation.Contains("KCE Participation"))
                            //if (psite.ProgramParticipation.Contains("KCE Participation") && psite.URL.Contains("PM02"))
                            {
                                /*
                                 * 1. create page
                                 * 2. delete web parts
                                 * 3. upload content editor ?
                                 * 4. configure page with content editor
                                 * 5. add link to Program Participation
                                 */
                                
                                SiteFilesUtility sfUtility = new SiteFilesUtility();
                                SitePublishUtility spUtility = new SitePublishUtility();
                                SiteLogUtility.LogPracDetail(psite);

                                //spUtility.InitializePage(psite.URL, pageName, "Optimal Starts");
                                //spUtility.DeleteWebPart(psite.URL, pageName);
                                ////sfUtility.DocumentUpload(psite.URL, @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\OptimalStart_DataTable.html", "Site Assets");
                                //ConfigureOptimalStartPage(psite.URL, urlSiteAssets);
                                //sfUtility.uploadProgramPracticeSupportFiles(psite);

                                //UpdateWebPartSize(psite.URL);
                                modifyWebPart(psite.URL);
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

        public static bool ConfigureOptimalStartPage(string webUrl, string siteAssetUrl)
        {
            bool outcome = false;
            string clink = string.Empty;
            string scntPx = "666";

            using (ClientContext clientContext = new ClientContext(webUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                Web web = clientContext.Web;
                clientContext.Load(web, w => w.ServerRelativeUrl);
                clientContext.ExecuteQuery();
                var file = clientContext.Web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + "/Pages/OptimalStart.aspx");
                file.CheckOut();
                try
                {
                    clientContext.Load(file);
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    try
                    {
                        LimitedWebPartManager olimitedwebpartmanager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);

                        WebPartDefinition wpd1 = olimitedwebpartmanager.ImportWebPart(contentEditorXML("OptimalStartDT", scntPx, "1100px", siteAssetUrl + "/SiteAssets/OptimalStart_DataTable.html"));
                        wpd1.WebPart.Title = "OptimalStartDT";
                        olimitedwebpartmanager.AddWebPart(wpd1.WebPart, "CenterLeftColumn", 1);

                        file.CheckIn("Adding ConfigureHomePage webparts", CheckinType.MajorCheckIn);
                        file.Publish("Adding ConfigureHomePage webparts");
                        clientContext.Load(file);
                        web.Update();
                        clientContext.ExecuteQuery();
                        outcome = true;
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("DeleteWeConfigureHomePagebPart", ex.Message, "Error", webUrl);
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("ConfigureHomePage", ex.Message, "Error", webUrl);
                    outcome = false;
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
            return outcome;
        }

        public static string contentEditorXML(string webPartTitle, string webPartHeight, string webPartWidth, string webPartContentLink)
        {
            string strXML = "";
            strXML = String.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                       "<WebPart xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" +
                                       " xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"" +
                                       " xmlns=\"http://schemas.microsoft.com/WebPart/v2\">" +
                                       "<Title>{0}</Title><FrameType>Default</FrameType>" +
                                       "<Description>Allows authors to enter rich text content.</Description>" +
                                       "<IsIncluded>true</IsIncluded>" +
                                       "<ZoneID>Header</ZoneID>" +
                                       "<PartOrder>0</PartOrder>" +
                                       "<FrameState>Normal</FrameState>" +
                                       "<Height>{1}</Height>" +
                                       "<Width>{2}</Width>" +
                                       "<AllowRemove>true</AllowRemove>" +
                                       "<AllowZoneChange>true</AllowZoneChange>" +
                                       "<AllowMinimize>true</AllowMinimize>" +
                                       "<AllowConnect>true</AllowConnect>" +
                                       "<AllowEdit>true</AllowEdit>" +
                                       "<AllowHide>true</AllowHide>" +
                                       "<IsVisible>true</IsVisible>" +
                                       "<DetailLink />" +
                                       "<HelpLink />" +
                                       "<HelpMode>Modeless</HelpMode>" +
                                       "<Dir>Default</Dir>" +
                                       "<PartImageSmall />" +
                                       "<MissingAssembly>Cannot import this Web Part.</MissingAssembly>" +
                                       "<PartImageLarge>/_layouts/15/images/mscontl.gif</PartImageLarge>" +
                                       "<IsIncludedFilter />" +
                                       "<Assembly>Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>" +
                                       "<TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>" +
                                       "<ContentLink xmlns='http://schemas.microsoft.com/WebPart/v2/ContentEditor'>{3}</ContentLink>" +
                                       "<Content xmlns='http://schemas.microsoft.com/WebPart/v2/ContentEditor' />" +
                                       "<PartStorage xmlns=\"http://schemas.microsoft.com/WebPart/v2/ContentEditor\" /></WebPart>", webPartTitle, webPartHeight, webPartWidth, webPartContentLink);
            return strXML;
        }

        public static bool modifyWebPart(string webUrl)
        {
            bool outcome = false;
            string clink = string.Empty;
            string scntPx = "666";

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
                                wp => wp.WebPart.Tag));
                        clientContext.ExecuteQuery();

                        if(limitedWebPartManager.WebParts.Count == 0)
                        {
                            throw new Exception("No Webparts on this page.");
                        }

                        WebPartDefinition webPartDefinition = limitedWebPartManager.WebParts[0];
                        WebPart webPart = webPartDefinition.WebPart;
                        webPart.Title = "Program Participation";
                        webPart.Tag = webPart.Properties["Height"] + "; " + webPart.Properties["Width"];
                        webPartDefinition.SaveWebPartChanges();
                        clientContext.ExecuteQuery();

                        file.CheckIn("Updating webparts", CheckinType.MajorCheckIn);
                        file.Publish("Updating webparts");
                        clientContext.Load(file);
                        web.Update();
                        clientContext.ExecuteQuery();
                        outcome = true;
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("modifyWebPart", ex.Message, "Error", webUrl);
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("modifyWebPart", ex.Message, "Error", webUrl);
                    outcome = false;
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
            return outcome;
        }

        public static void UpdateWebPartSize(string webURL)
        {
            var pageRelativeUrl = "/Pages/ProgramParticipation.aspx";
            using (ClientContext clientContext = new ClientContext(webURL))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();

                    var file = clientContext.Web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + pageRelativeUrl);
                    file.CheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();

                    var wpManager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);
                    var webParts = wpManager.WebParts;
                    clientContext.Load(webParts);
                    clientContext.ExecuteQuery();

                    if (wpManager.WebParts.Count > 0)
                    {
                        foreach (var oWebPart in wpManager.WebParts)
                        {
                            //oWebPart.DeleteWebPart();
                            oWebPart.IsPropertyAvailable("Height");
                            oWebPart.IsPropertyAvailable("Width");
                            clientContext.ExecuteQuery();
                        }
                    }
                    file.CheckIn("Delete webpart", CheckinType.MajorCheckIn);
                    file.Publish("Delete webpart");
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    clientContext.Dispose();
                }
            }
        }
        public static List<PMData> initPMDataToList(string adminGroupUrl)
        {
            //SitePMData objSitePMData = new SitePMData();
            List<PMData> pmData = new List<PMData>();
            pmData = SP_GetPortalData_PMData(adminGroupUrl);
            return pmData;
        }
        public static DataTable readPMSiteData()
        {
            try
            {
                string connString = "Data Source=" + ConfigurationManager.AppSettings["SqlServer"]
                        + "; Initial Catalog=" + ConfigurationManager.AppSettings["Database"] + "; Integrated Security=SSPI";

                string query = @"SELECT * FROM [HealthCloud_NightlyProd].[PORTAL].[vwPracticeInfo] ORDER BY GroupID";

                DataTable dtTable = new DataTable();
                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtTable);
                conn.Close();
                da.Dispose();

                return dtTable;
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("readPMSiteData", ex.Message, "Error", "");
                return null;
            }
        }

        public static List<PMData> SP_GetPortalData_PMData(string adminGroupUrl)
        {
            List<PMData> All_PortalData = new List<PMData>();
            List<PMData> CKCC_PMData = new List<PMData>();
            try
            {
                All_PortalData = SP_GetAll_PMData(adminGroupUrl);
                CKCC_PMData = All_PortalData.Where
                    (x => x.ProgramParticipation.Contains("KCE Participation")).ToList();

                //ResultDescription += "[" + Answers_NewReferrals.Count + "] items found in SP => Visible and have answers." + textLine;
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("SP_GetPortalData_PMData", ex.Message, "Error", "");
            }

            return CKCC_PMData;
        }

        public static List<PMData> SP_GetAll_PMData(string urlAdminGrp)
        {
            List<PMData> pmData = new List<PMData>();
            SitePMData sitePMData = new SitePMData();

            using(ClientContext clientContext = new ClientContext(urlAdminGrp))
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
                SiteLogUtility.Log_Entry("Total Count: " + items.Count, true);

                foreach (var item in items)
                {
                    PMData pmd = new PMData();
                    

                    SiteLogUtility.Log_Entry(item["PracticeTIN"] + " - " + item["PracticeName"] + " - " + item["ProgramParticipation"], true);
                    //SiteLogUtility.Log_Entry("PracticeName: " + item["PracticeName"], true);
                    //SiteLogUtility.Log_Entry("PracticeTIN: " + item["PracticeTIN"], true);
                    //SiteLogUtility.Log_Entry("ProgramParticipation: " + item["ProgramParticipation"], true);

                    pmd.PracticeName = item["PracticeName"].ToString();
                    pmd.PracticeTIN = item["PracticeTIN"].ToString();
                    pmd.SiteId = item["PracticeTIN"].ToString();
                    pmd.ProgramParticipation = item["ProgramParticipation"].ToString();

                    pmd.IsKC365 = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationKC365) ? "true" : "false";
                    pmd.IsCKCC = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationCKCC) ? "true" : "false";
                    pmd.IsIWH = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationIWH) ? "true" : "false";
                    //sourceSite.SetAttributeValue("IsKC365", Convert.ToInt32(dr["KC365"]) == 0 ? "false" : "true");
                    //sourceSite.SetAttributeValue("kceArea", dr["CKCCArea"]);
                    //sourceSite.SetAttributeValue("IsCKCC", dr["CKCCArea"].ToString() == "" ? "false" : "true");
                    //sourceSite.SetAttributeValue("IsIWH", dr["IWNRegion"].ToString() == "" ? "false" : "true");

                    pmData.Add(pmd);
                }
            }

            return pmData;
        }

        public List<PMData> filterPMDataToList(DataTable pmDT)
        {
            List<PMData> pmData = new List<PMData>();
            return pmData;
        }

        public static void filterPMSiteData(DataTable allData)
        {
            try
            {
                DataTable dtDataNew = allData.Clone();
                DataView view = new DataView(allData);
                DataTable distinctValues = view.ToTable(true, "GroupID");
                
                for (int intLoop = 0; intLoop < distinctValues.Rows.Count; intLoop++)
                {
                    if (intLoop <= 9)
                    {
                        dtDataNew = allData.AsEnumerable().Where(row => row.Field<Int32>("GroupID") == Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"])).CopyToDataTable();
                        //updateXML(dtDataNew, ConfigurationManager.AppSettings["ConfigURL"] + "PracticeSiteTemplate_PM" + Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"]).ToString("00") + ".config", "PracticeSite20_PM" + Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"]).ToString("00"));
                    }
                    else
                    {
                        dtDataNew = allData.AsEnumerable().Where(row => row.Field<Int32>("GroupID") == Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"])).CopyToDataTable();
                        //updateXML(dtDataNew, ConfigurationManager.AppSettings["ConfigURL"] + "PracticeSiteTemplate_PM" + distinctValues.Rows[intLoop]["GroupID"].ToString() + ".config", "PracticeSite20_PM" + distinctValues.Rows[intLoop]["GroupID"].ToString());
                    }
                    dtDataNew.Rows.Clear();
                }
            }
            catch (Exception ex)
            {

            }
        }

        //public static void updateXML(DataTable dt, string xmlfilePath, string strRegionID)
        //{
        //    try
        //    {
        //        XDocument sourceFile = XDocument.Load(ConfigurationManager.AppSettings["ConfigURL"] + "PracticeSiteTemplate.config");
        //        XDocument xdoc = XDocument.Load(xmlfilePath);
        //        var sourceElementSbsite = sourceFile.Elements("Config").Elements("Sites").Elements("Site").Elements("SubSites").Elements("Site");
        //        var propertyValueSourceEle = sourceFile.Elements("Config").Elements("Sites").Elements("Site").Elements("SubSites").Elements("Site").Elements("SiteSettings").Elements("PropertyBag").Elements("Property");
        //        var sourceSite = sourceElementSbsite.FirstOrDefault();
        //        var propertySourceSite = propertyValueSourceEle.FirstOrDefault();
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            DataRow dr = dt.Rows[i];
        //            sourceSite.SetAttributeValue("SiteName", dr["SiteID"]);
        //            sourceSite.SetAttributeValue("SiteTitle", dr["PracticeName"]);
        //            sourceSite.SetAttributeValue("RegionID", strRegionID);
        //            sourceSite.SetAttributeValue("SiteDescription", dr["PracticeName"] + " is a member of " + strRegionID);
        //            sourceSite.SetAttributeValue("IsKC365", Convert.ToInt32(dr["KC365"]) == 0 ? "false" : "true");
        //            sourceSite.SetAttributeValue("kceArea", dr["CKCCArea"]);
        //            sourceSite.SetAttributeValue("IsCKCC", dr["CKCCArea"].ToString() == "" ? "false" : "true");
        //            sourceSite.SetAttributeValue("IsIWH", dr["IWNRegion"].ToString() == "" ? "false" : "true");
        //            sourceSite.SetAttributeValue("encryptedTIN", dr["EncryptedPracticeTIN"]);
        //            propertySourceSite.SetAttributeValue("PropertyValue", strRegionID);
        //            xdoc.Element("Config").Element("Sites").Element("Site").Element("SubSites").Add(sourceSite);
        //            //xdoc.Element("Config").Element("Sites").Element("Site").Element("SubSites").Element("Site").Element("SiteSettings").Element("PropertyBag").Element("Property").Add(propertySourceSite);
        //            xdoc.Save(xmlfilePath);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
    }
}

using System;
using System.Collections.Generic;
using SiteUtility;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System.Net;
using Serilog;
using System.Configuration;

namespace Release_1_4
{
    public class AddDialysisStart
    {
        static Guid _listGuid = Guid.Empty;
        static string dateHrMin = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
        const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
        static ILogger _logger = Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("Logs/maint" + dateHrMin + "_.log", rollingInterval: RollingInterval.Day, shared: true, outputTemplate: outputTemp1)
           .CreateLogger();
        static ILogger logger = _logger.ForContext<AddDialysisStart>();
        static string LayoutsFolderImg = ConfigurationManager.AppSettings["LayoutsFolderImg"];
        public void InitProg()
        {
            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();
            SiteLogUtility siteLogUtility = new SiteLogUtility();

            List<Practice> practices = siteInfoUtility.GetAllCKCCPractices();

            try
            {
                siteLogUtility.LoggerInfo_Entry("\n\n=============Release Starts=============", true);

                if (practices != null && practices.Count > 0)
                {
                    foreach (var practice in practices)
                    {
                        string urlSiteAssets = SiteInfoUtility.GetReferralUrl(practice.NewSiteUrl);
                        siteLogUtility.LoggerInfoBody(practice);

                        DialysisStartsSetup(practice, SitePublishUtility.pagePatientStatusUpdates, urlSiteAssets);
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
            }
            finally
            {
                siteLogUtility.LoggerInfo_Entry(SiteLogUtility.textLine0, true);
                siteLogUtility.LoggerInfo_Entry("=============Release Ends=============", true);
            }
        }
        public void InitProg(string siteId)
        {

            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();
            SiteLogUtility siteLogUtility = new SiteLogUtility();

            Practice practice = siteInfoUtility.GetPracticeBySiteID(siteId);
            string urlSiteAssets = SiteInfoUtility.GetReferralUrl(practice.NewSiteUrl);

            try
            {
                siteLogUtility.LoggerInfo_Entry("\n\n============= AddDialysisStart Release Starts=============", true);

                if (practice != null)
                {
                    if (practice.IsCKCC)
                    {
                        siteLogUtility.LoggerInfoBody(practice);
                        DialysisStartsSetup(practice, SitePublishUtility.pagePatientStatusUpdates, urlSiteAssets);
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
            }
            finally
            {
                siteLogUtility.LoggerInfo_Entry(SiteLogUtility.textLine0, true);
                siteLogUtility.LoggerInfo_Entry("============= AddDialysisStart Release Ends =============", true);
            }
        }
        public static void DialysisStartsSetup(Practice psite, string pageName, string urlSiteAssets)
        {
            try
            {
                SiteFilesUtility sfUtility = new SiteFilesUtility();
                SitePublishUtility spUtility = new SitePublishUtility();

                if (!SiteFilesUtility.FileExists(psite.NewSiteUrl, "Pages", pageName + ".aspx"))
                {
                    spUtility.InitializePage(psite.NewSiteUrl, pageName, "Patient Status Updates"); 
                }
                spUtility.DeleteWebPart(psite.NewSiteUrl, pageName);
                ConfigureDialysisStartsPage(psite.NewSiteUrl, urlSiteAssets, pageName);

                uploadProgramPracticeSupportFilesDialysisStarts(psite);
                modifyWebPartProgramParticipation(psite.NewSiteUrl, psite);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("DialysisStartsSetup", ex.Message, "Error", "");
            }
        }

        public static bool ConfigureDialysisStartsPage(string webUrl, string siteAssetUrl, string pgName)
        {
            bool outcome = false;
            string clink = string.Empty;
            string scntPx = "667";

            using (ClientContext clientContext = new ClientContext(webUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                Web web = clientContext.Web;
                clientContext.Load(web, w => w.ServerRelativeUrl);
                clientContext.ExecuteQuery();
                var file = clientContext.Web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + "/Pages/" + pgName + ".aspx");
                file.CheckOut();
                try
                {
                    clientContext.Load(file);
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    try
                    {
                        LimitedWebPartManager olimitedwebpartmanager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);

                        WebPartDefinition wpd1 = olimitedwebpartmanager.ImportWebPart(contentEditorXML("DialysisStartsDT", "", "1000px", siteAssetUrl + "/SiteAssets/DialysisStarts_DataTable.html"));
                        wpd1.WebPart.Title = "DialysisStartsDT";
                        olimitedwebpartmanager.AddWebPart(wpd1.WebPart, "CenterLeftColumn", 1);

                        file.CheckIn("Adding DialysisStartsDT webparts", CheckinType.MajorCheckIn);
                        file.Publish("Adding DialysisStartsDT webparts");
                        clientContext.Load(file);
                        web.Update();
                        clientContext.ExecuteQuery();
                        outcome = true;
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("Create ConfigureDialysisStartsPage", ex.Message, "Error", "");
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("ConfigureDialysisStartsPage", ex.Message, "Error", "");
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
                                       "<Title>{0}</Title><FrameType>None</FrameType>" +
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
        public static bool modifyWebPartProgramParticipation(string webUrl, Practice practiceSite)
        {
            SiteLogUtility.Log_Entry("   modifyWebPartProgramParticipation - In Progress...");
            bool outcome = false;
            string clink = string.Empty;
            int webPartHeight = SiteInfoUtility.gridHeight(practiceSite);

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
                        SiteLogUtility.CreateLogEntry("Update - modifyWebPartProgramParticipation", ex.Message, "Error", "");
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("modifyWebPartProgramParticipation", ex.Message, "Error", "");
                    outcome = false;
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
            return outcome;
        }
        public static int gridHeight(string webUrl, PracticeSite site)
        {
            int intCount = -1;
            int[] intHeight = new int[4] { 156, 253, 350, 447 };
            try
            {
                if (site.IsIWH == "true")
                {
                    intCount++;
                }
                if (site.IsCKCC == "true")
                {
                    intCount++;
                    intCount++;  // For Dialysis Starts...
                }
                if (site.IsKC365 == "true")
                {
                    intCount++;
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("gridHeight", ex.Message, "Error", "");
            }
            return intHeight[intCount];
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
                                wp => wp.WebPart.Properties));
                        //clientContext.Load(limitedWebPartManager.WebParts);
                        clientContext.ExecuteQuery();

                        if(limitedWebPartManager.WebParts.Count == 0)
                        {
                            throw new Exception("No Webparts on this page.");
                        }

                        foreach(WebPartDefinition webPartDefinition1 in limitedWebPartManager.WebParts)
                        {
                            clientContext.Load(webPartDefinition1.WebPart.Properties,
                                wp => wp.FieldValues);
                            clientContext.ExecuteQuery();

                            if(webPartDefinition1.WebPart.Title.Equals("Content Editor"))
                            {
                                webPartDefinition1.WebPart.Properties["Title"] = "ProgramParticipation";
                                webPartDefinition1.WebPart.Properties["Height"] = "600";
                                webPartDefinition1.SaveWebPartChanges();
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
        //public static List<PMData> initPMDataToList(string adminGroupUrl)
        //{
        //    List<PMData> pmData = new List<PMData>();
        //    try
        //    {
        //        pmData = SP_GetPortalData_PMData(adminGroupUrl);
        //    }
        //    catch (Exception ex)
        //    {
        //        SiteLogUtility.CreateLogEntry("initPMDataToList", ex.Message, "Error", "");
        //    }
        //    return pmData;
        //}
        //public static List<PMData> SP_GetPortalData_PMData(string adminGroupUrl)
        //{
        //    List<PMData> All_PortalData = new List<PMData>();
        //    //List<PMData> CKCC_PMData = new List<PMData>();
        //    try
        //    {
        //        All_PortalData = SP_GetAll_PMData(adminGroupUrl);
        //        //CKCC_PMData = All_PortalData.Where
        //        //    (x => x.ProgramParticipation.Contains("KCE Participation")).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        SiteLogUtility.CreateLogEntry("SP_GetPortalData_PMData", ex.Message, "Error", "");
        //    }

        //    //return CKCC_PMData;
        //    return All_PortalData;
        //}
        //public static List<PMData> SP_GetAll_PMData(string urlAdminGrp)
        //{
        //    List<PMData> pmData = new List<PMData>();
        //    SitePMData sitePMData = new SitePMData();

        //    using(ClientContext clientContext = new ClientContext(urlAdminGrp))
        //    {
        //        clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

        //        List list = clientContext.Web.Lists.GetByTitle("AdminGroup");
        //        clientContext.Load(list);
        //        clientContext.ExecuteQuery();
        //        View view = list.Views.GetByTitle("All Links");

        //        clientContext.Load(view);
        //        clientContext.ExecuteQuery();
        //        CamlQuery query = new CamlQuery();
        //        query.ViewXml = view.ViewQuery;

        //        ListItemCollection items = list.GetItems(query);
        //        clientContext.Load(items);
        //        clientContext.ExecuteQuery();
        //        SiteLogUtility.Log_Entry(SiteLogUtility.textLine0, true);
        //        SiteLogUtility.Log_Entry("Total Count: " + items.Count, true);
        //        cntRunAdminGroup = items.Count;

        //        foreach (var item in items)
        //        {
        //            PMData pmd = new PMData();
                    

        //            SiteLogUtility.Log_Entry(item["PracticeTIN"] + " - " + item["PracticeName"] + " - " + item["ProgramParticipation"], true);

        //            pmd.PracticeName = item["PracticeName"].ToString();
        //            pmd.PracticeTIN = item["PracticeTIN"].ToString();
        //            pmd.SiteId = item["PracticeTIN"].ToString();
        //            pmd.ProgramParticipation = item["ProgramParticipation"].ToString();

        //            pmd.IsKC365 = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationKC365) ? "true" : "false";
        //            pmd.IsCKCC = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationCKCC) ? "true" : "false";
        //            pmd.IsIWH = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationIWH) ? "true" : "false";

        //            pmData.Add(pmd);
        //        }
        //    }

        //    return pmData;
        //}
        public static void uploadProgramPracticeSupportFilesDialysisStarts(Practice practiceSite)
        {
            SiteLogUtility.Log_Entry("   uploadProgramPracticeSupportFilesDialysisStarts - In Progress...");
            using (ClientContext clientContext = new ClientContext(practiceSite.NewSiteUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    string rootWebUrl = GetRootSite(practiceSite.NewSiteUrl);

                    string LibraryName = "Program Participation";
                    string fileName3 = "optimalstarts.jpg";

                    byte[] f3 = System.IO.File.ReadAllBytes(LayoutsFolderImg + fileName3);

                    FileCreationInformation fc3 = new FileCreationInformation();
                    fc3.Url = fileName3;
                    fc3.Overwrite = true;
                    fc3.Content = f3;
                    List myLibrary = web.Lists.GetByTitle(LibraryName);

                    if (practiceSite.IsCKCC)
                    {
                        Microsoft.SharePoint.Client.File newFile3 = myLibrary.RootFolder.Files.Add(fc3);
                        clientContext.Load(newFile3);
                        clientContext.ExecuteQuery();

                        ListItem lItem3 = newFile3.ListItemAllFields;
                        lItem3.File.CheckOut();
                        clientContext.ExecuteQuery();
                        //lItem3["Title"] = "Optimal Starts Coming Soon!";
                        lItem3["Title"] = "Patient Status Updates";
                        lItem3["ProgramNameText"] = practiceSite.NewSiteUrl + "/Pages/" + SitePublishUtility.pagePatientStatusUpdates + ".aspx";
                        lItem3["Thumbnail"] = practiceSite.NewSiteUrl + "/Program%20Participation/" + fileName3;
                        lItem3.Update();
                        lItem3.File.CheckIn("Checkin - Create OptimalStart item", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();
                    }

                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("uploadProgramPracticeSupportFilesDialysisStarts", ex.Message, "Error", "");
                }
            }
        }
        public static string GetRootSite(string url)
        {
            Uri uri = new Uri(url.TrimEnd(new[] { '/' }));
            return $"{uri.Scheme}://{ uri.DnsSafeHost}";
        }
    }
    
}

using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using Serilog;
using SiteUtility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace R_1_9_PayorEnrollment
{
    public class PayorEnrollment
    {
        //const string outputTemp = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
        //   static ILogger _logger = Log.Logger = new LoggerConfiguration()
        //       .MinimumLevel.Debug()
        //       .Enrich.FromLogContext()
        //       .WriteTo.Console()
        //       .WriteTo.File("Logs/ex_.log", rollingInterval: RollingInterval.Day, shared: true, outputTemplate: outputTemp)
        //       .CreateLogger();

        static Guid _listGuid = Guid.Empty;
        static string dateHrMin = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
        const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
        static ILogger _logger = Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("Logs/maint" + dateHrMin + "_.log", rollingInterval: RollingInterval.Day, shared: true, outputTemplate: outputTemp1)
           .CreateLogger();
        static ILogger logger = _logger.ForContext<PayorEnrollment>();

        const string LayoutsFolder = @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\";
        string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
        static string strPortalSiteURL = ConfigurationManager.AppSettings["SP_SiteUrl"];
        //static void Main(string[] args)
        //{
        //    _logger.Information("InitiateProg() started...");

        //    using (ClientContext clientContext = new ClientContext(strPortalSiteURL))
        //    {
        //        clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

        //        try
        //        {
        //            List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext);
        //            foreach (ProgramManagerSite pm in practicePMSites)
        //            {
        //                if (pm.ProgramManager == "10")
        //                {
        //                    foreach (PracticeSite psite in pm.PracticeSiteCollection)
        //                    {
        //                        List<PMData> pmd = SiteInfoUtility.SP_GetAll_PMData(pm.URL, psite.SiteId);
        //                        if (pmd.Count > 0)
        //                        {
        //                            if (pmd[0].IsKC365 == "true")
        //                            {
        //                                if (PayorEnrollment_Setup(psite.URL + "/"))
        //                                {
        //                                    _logger.Information(psite.Name + " setup is completed");
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.Error(ex.Message + " " + ex.StackTrace);
        //        }


        //        _logger.Information("Maintenance Tasks Completed Successfully!");
        //    }
        //}

        public void Init_PayorEnrollment()
        {
            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();

            //List<Practice> practices = siteInfo.GetPracticesByPM("10");
            List<Practice> practices = siteInfoUtility.GetAllPractices();
            if (practices != null && practices.Count > 0)
            {
                try
                {
                    _logger.Information("================ Deployment Started =====================", true);
                    int intLoop = 0;

                    foreach (Practice practice in practices)
                    {
                        if (practice.IsKC365)
                        {
                            PayorEnrollment_Setup(practice.NewSiteUrl, practice.SiteID);
                            _logger.Information(practice.Name + "  .. Html Updated.", true);
                            _logger.Information(practice.NewSiteUrl, true);
                            intLoop++; 
                        }
                    }

                    _logger.Information("Total Practices: " + intLoop, true);
                    _logger.Information("================ Deployment Completed =====================", true);
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("Init_PayorEnrollment", ex.Message, "Error", "");
                }
            }
        }
        public void Init_PayorEnrollment(string siteId)
        {
            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();

            Practice practice = siteInfoUtility.GetPracticeBySiteID(siteId);
            if (practice != null && practice.IsKC365)
            {
                try
                {
                    _logger.Information("================ PayorEnrollment Started =====================", true);
                    PayorEnrollment_Setup(practice.NewSiteUrl, practice.SiteID);
                    _logger.Information(practice.Name + "  .. Html Updated.", true);
                    _logger.Information("================ PayorEnrollment Completed =====================", true);
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("Init_PayorEnrollment(siteId)", ex.Message, "Error", "");
                }
            }
        }
        public static bool PayorEnrollment_Setup(string siteUrl, string siteId)
        {
            try
            {
                SiteInfoUtility siteInfoUtility = new SiteInfoUtility();
                SiteFilesUtility siteFilesUtility = new SiteFilesUtility();
                Practice practice = siteInfoUtility.GetPracticeBySiteID(siteId);
                string urlSiteAssets = SiteInfoUtility.GetReferralUrl(practice.NewSiteUrl);

                //siteFilesUtility.DocumentUpload(siteUrl, @"C:\Users\nalkazaki\OneDrive - Fresenius Medical Care\Documents\VisualStudio\PayorEnrollment\PayorEnrollment.html", "SiteAssets");
                //siteFilesUtility.DocumentUpload(siteUrl, @"C:\Users\nalkazaki\OneDrive - Fresenius Medical Care\Documents\VisualStudio\PayorEnrollment\bootstrap-float-label.min.css", "SiteAssets");

                //siteFilesUtility.DocumentUpload(siteUrl, @layoutsFolder + "PayorEnrollment.html", "SiteAssets");
                //siteFilesUtility.DocumentUpload(siteUrl, @layoutsFolder + "bootstrap-float-label.min.css", "SiteAssets");

                if (!SiteFilesUtility.FileExists(siteUrl, "Pages", "PayorEnrollment.aspx"))
                {
                    createPayorEnrollmentPage(siteUrl, "PayorEnrollment", "Payor Enrollment", "1000px", urlSiteAssets + "/SiteAssets/PayorEnrollment.html");
                }

                updateProgramParticipation(siteUrl);

                //AddPermissionGroup_PayorEnrollment(siteId, "Referrals", "Contribute_NoDelete");             // Dev...
                //AddPermissionGroup_PayorEnrollment(siteId, "ReferralsPrevious", "Contribute_NoDelete");     // Dev...
                AddPermissionGroup_PayorEnrollment(siteId, "Referrals", "Contribute Edit Only");             // Prod...
                AddPermissionGroup_PayorEnrollment(siteId, "ReferralsPrevious", "Contribute Edit Only");     // Prod...

                logger.Information(" Payor Enrollment setup is completed");

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + " " + ex.StackTrace);
                return false;
            }


        }

        public static void AddPermissionGroup_PayorEnrollment(string siteId, string listName, string permType)
        {
            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();
            Practice practice = siteInfoUtility.GetPracticeBySiteID(siteId);
            try
            {
                string pracUserGroup = "Prac_" + practice.TIN + "_User";
                string strReferralURL = SiteInfoUtility.GetPayorEnrollmentUrl(practice.NewSiteUrl);  //NO SLASH AT THE END

                SitePermissionUtility.AddSecurityGroupToList(strReferralURL, pracUserGroup, listName, permType);
            }
            catch (Exception ex)
            {
                logger.Information(ex.Message);
            }
        }

        public static void createPayorEnrollmentPage(string siteUrl, string strPageName, string strTitle, string strWPWidth, string strContentWPLink)
        {
            try
            {
                SitePublishUtility spUtility = new SitePublishUtility();
                spUtility.InitializePage(siteUrl, strPageName, strTitle);
                spUtility.DeleteWebPart(siteUrl, strPageName);

                using (ClientContext clientContext = new ClientContext(siteUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.Load(web.ParentWeb);
                    clientContext.ExecuteQuery();

                    var file = web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + "/Pages/" + strPageName + ".aspx");
                    file.CheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    try
                    {
                        LimitedWebPartManager olimitedwebpartmanager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);

                        WebPartDefinition wpd1 = olimitedwebpartmanager.ImportWebPart(contentEditorXML(strTitle, strWPWidth, strContentWPLink));
                        wpd1.WebPart.Title = strTitle;
                        olimitedwebpartmanager.AddWebPart(wpd1.WebPart, "Header", 1);

                        file.CheckIn("CheckIn - Adding Webparts to " + strTitle, CheckinType.MajorCheckIn);
                        file.Publish("Publish - Adding Webparts to " + strTitle);
                        clientContext.Load(file);
                        web.Update();
                        clientContext.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                        _logger.Error(ex.Message + " " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + " " + ex.StackTrace);
            }
        }


        public static string contentEditorXML(string webPartTitle, string webPartWidth, string webPartContentLink)
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
                                       "<PartStorage xmlns=\"http://schemas.microsoft.com/WebPart/v2/ContentEditor\" /></WebPart>", webPartTitle, "", webPartWidth, webPartContentLink);
            return strXML;
        }

        private static void updateProgramParticipation(string siteUrl)
        {
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                Web web = clientContext.Web;
                clientContext.Load(web);
                clientContext.ExecuteQuery();

                try
                {
                    List list = web.Lists.GetByTitle("Program Participation");
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();

                    CamlQuery query = new CamlQuery();
                    query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>Payor Enrollment</Value></Eq></Where></Query></View>";

                    ListItemCollection items = list.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    if (items.Count > 0)
                    {
                        ListItem payorItem = items.FirstOrDefault();
                        payorItem["ProgramNameText"] = siteUrl + "Pages/PayorEnrollment.aspx";
                        payorItem.Update();
                        clientContext.ExecuteQuery();
                    }
                    else
                    {
                        string fileLocation = @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\";
                        string fileName = "PracticeReferrals.JPG";

                        byte[] f = System.IO.File.ReadAllBytes(fileLocation + fileName);

                        FileCreationInformation fc = new FileCreationInformation();
                        fc.Url = fileName;
                        fc.Overwrite = true;
                        fc.Content = f;

                        Microsoft.SharePoint.Client.File newFile = list.RootFolder.Files.Add(fc);
                        clientContext.Load(newFile);
                        clientContext.ExecuteQuery();

                        ListItem newItem = newFile.ListItemAllFields;
                        newItem.File.CheckOut();
                        clientContext.ExecuteQuery();
                        newItem["Title"] = "Payor Enrollment";

                        newItem["ProgramNameText"] = siteUrl + "Pages/PayorEnrollment.aspx";
                        newItem["Thumbnail"] = siteUrl + "Program%20Participation/" + fileName;
                        newItem.Update();
                        newItem.File.CheckIn("Checkin - Create Payor Item", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();

                        modifyWebPartProgramParticipation(siteUrl);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message + " " + ex.StackTrace);
                }
            }
        }


        public static void modifyWebPartProgramParticipation(string webUrl)
        {
            string clink = string.Empty;
            int webPartHeight = gridHeight(webUrl);

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
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message + " " + ex.StackTrace);
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message + " " + ex.StackTrace);
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
        }


        public static int gridHeight(string siteUrl)
        {
            int intCount = 0;
            int[] intHeight = new int[5] { 156, 253, 350, 447, 544 };
            try
            {
                using (ClientContext clientContext = new ClientContext(siteUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();

                    List list = web.Lists.GetByTitle("Program Participation");
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();

                    CamlQuery query = new CamlQuery();
                    query.ViewXml = @"<View><Query></Query></View>";

                    ListItemCollection items = list.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    intCount = items.Count;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + " " + ex.StackTrace);
            }
            return intHeight[intCount - 1];
        }

    }
}

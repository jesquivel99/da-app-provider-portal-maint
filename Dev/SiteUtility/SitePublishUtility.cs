using System;
using System.Net;
using System.IO;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using Microsoft.SharePoint.Client.Publishing;

namespace SiteUtility
{
    public class SitePublishUtility
    {
        public static string pagePayorEnrollment = "PayorEnrollment";
        public static string titlePayorEnrollment = "Payor Enrollment";
        public static string imgPayorEnrollment = "PracticeReferrals.JPG";

        public static string pageCkccKceResources = "CkccKceResources";
        public static string titleCkccKceResources = "CKCC/KCE Resources";
        public static string imgCkccKceResources = "KCEckcc.JPG";

        public static string pagePatientStatusUpdates = "PatientUpdates";
        public static string titlePatientStatusUpdates = "Patient Status Updates";
        public static string imgPatientStatusUpdates = "optimalstarts.jpg";

        public static string pagePayorProgramEdResources = "PayorEdResources";
        public static string titlePayorProgramEdResources = "Payor Program Education Resources";
        public static string imgPayorProgramEdResources = "EducationReviewPro.JPG";

        public static string pageCkccEngagement = "CkccKceEngagement";
        public static string titleCkccEngagement = "CKCC/KCE Engagement";
        public static string imgCkccEngagement = "CKCC_KCEEngagement.png";

        public static void DownloadBackupHomePage(PracticeSite practiceSite, bool keepHomePageCheckedOut=true)
        {
            var pageRelativeUrl = "/Pages/Home.aspx";

            try
            {
                using (ClientContext clientContext = new ClientContext(practiceSite.ExistingSiteUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle("Pages");
                    clientContext.Load(web);
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();

                    Microsoft.SharePoint.Client.File fileToDownload = web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + pageRelativeUrl);
                    fileToDownload.CheckOut();

                    clientContext.Load(fileToDownload);
                    clientContext.ExecuteQuery();

                    if (fileToDownload.Exists)
                    {
                        String fileRef = fileToDownload.ServerRelativeUrl;
                        FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, fileRef);

                        //String fileName = Path.Combine("C:\\Temp", (string)fileToDownload.Name);
                        String fileName = Path.Combine("C:\\Temp", "Home_Backup.aspx");

                        using (var fileStream = System.IO.File.Create(fileName))
                        {
                            fileInfo.Stream.CopyTo(fileStream);
                        }
                    }

                    if (keepHomePageCheckedOut == false)
                    {
                        fileToDownload.CheckIn("Home.aspx downloaded and saved as Home_backup.aspx", CheckinType.MajorCheckIn);
                        fileToDownload.Publish("Home.aspx downloaded and saved as Home_backup.aspx");
                        clientContext.Load(fileToDownload);
                        clientContext.ExecuteQuery(); 
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("DownloadBackupHomePage", ex.Message, "Error", "");
            }
        }

        public static void DownloadPage(PracticeSite practiceSite, string pageName)
        {
            var pageRelativeUrl = @"/Pages/" + pageName + ".aspx";

            try
            {
                using (ClientContext clientContext = new ClientContext(practiceSite.ExistingSiteUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle("Pages");
                    clientContext.Load(web);
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();

                    Microsoft.SharePoint.Client.File fileToDownload = web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + pageRelativeUrl);
                    fileToDownload.CheckOut();

                    clientContext.Load(fileToDownload);
                    clientContext.ExecuteQuery();

                    if (fileToDownload.Exists)
                    {
                        String fileRef = fileToDownload.ServerRelativeUrl;
                        FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, fileRef);

                        String fileName = Path.Combine("C:\\Temp", (string)fileToDownload.Name);

                        using (var fileStream = System.IO.File.Create(fileName))
                        {
                            fileInfo.Stream.CopyTo(fileStream);
                        }
                    }

                    fileToDownload.CheckIn(pageRelativeUrl + " downloaded", CheckinType.MajorCheckIn);
                    fileToDownload.Publish(pageRelativeUrl + " downloaded");
                    clientContext.Load(fileToDownload);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("DownloadPage", ex.Message, "Error", "");
            }
        }

        public static void CheckinHomePage(PracticeSite practiceSite)
        {
            var pageRelativeUrl = "/Pages/Home.aspx";
            //var pageRelativeUrl = "/Pages/Home_Backup.aspx";

            try
            {
                using (ClientContext clientContext = new ClientContext(practiceSite.ExistingSiteUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle("Pages");
                    clientContext.Load(web);
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();

                    Microsoft.SharePoint.Client.File fileToDownload = web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + pageRelativeUrl);
                    fileToDownload.CheckIn("Checkin Home Page", CheckinType.MajorCheckIn);
                    fileToDownload.Publish("Checkin Home Page");
                    clientContext.Load(fileToDownload);
                    clientContext.ExecuteQuery();

                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("CheckinHomePage", ex.Message, "Error", "");
            }
        }

        public static void CheckinPage(PracticeSite practiceSite, string pageName)
        {
            var pageRelativeUrl = @"/Pages/" + pageName + ".aspx";

            try
            {
                using (ClientContext clientContext = new ClientContext(practiceSite.ExistingSiteUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle("Pages");
                    clientContext.Load(web);
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();

                    Microsoft.SharePoint.Client.File fileToDownload = web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + pageRelativeUrl);
                    fileToDownload.CheckIn($"Checkin {pageRelativeUrl} Page", CheckinType.MajorCheckIn);
                    fileToDownload.Publish($"Publish {pageRelativeUrl} Page");
                    clientContext.Load(fileToDownload);
                    clientContext.ExecuteQuery();

                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("CheckinPage", ex.Message, "Error", "");
            }
        }

        public PublishingPage InitializePage(string webUrl, string pageName, string pageTitle)
        {
            SiteLogUtility.Log_Entry("   InitializePage - In Progress...");
            String filename = pageName + ".aspx";
            String title = pageTitle;
            String list = "Pages";
            using (ClientContext clientContext = new ClientContext(webUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;
                    clientContext.Load(clientContext.Site.RootWeb, w => w.ServerRelativeUrl);
                    clientContext.ExecuteQuery();

                    // Get Page Layout
                    Microsoft.SharePoint.Client.File pageFromDocLayout = clientContext.Site.RootWeb.GetFileByServerRelativeUrl(String.Format("{0}/_catalogs/masterpage/BlankWebPartPage.aspx", clientContext.Site.RootWeb.ServerRelativeUrl.TrimEnd('/')));
                    Microsoft.SharePoint.Client.ListItem pageLayoutItem = pageFromDocLayout.ListItemAllFields;
                    clientContext.Load(pageLayoutItem);
                    clientContext.ExecuteQuery();

                    // Create Publishing Page
                    PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(clientContext, web);
                    PublishingPage page = publishingWeb.AddPublishingPage(new PublishingPageInformation
                    {
                        Name = filename,
                        PageLayoutListItem = pageLayoutItem
                    });
                    clientContext.ExecuteQuery();

                    // Set Page Title and Publish Page
                    Microsoft.SharePoint.Client.ListItem pageItem = page.ListItem;
                    pageItem["Title"] = title;
                    pageItem.Update();
                    pageItem.File.CheckIn(String.Empty, CheckinType.MajorCheckIn);
                    clientContext.ExecuteQuery();
                    return page;
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("InitializeHomePage", ex.Message, "Error", "");
                    clientContext.Dispose();
                }
            }
            return null;
        }

        public void SetWelcomePage(string webUrl, string serverRelativeUrl)
        {
            using (ClientContext clientContext = new ClientContext(webUrl))
            {
                try
                {
                    clientContext.Web.RootFolder.WelcomePage = serverRelativeUrl;
                    clientContext.Web.RootFolder.Update();
                    clientContext.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("SetWelcomePage", ex.Message, "Error", webUrl);
                    clientContext.Dispose();
                }
            }
        }

        public void DeleteWebPart(string webURL, string pageName)
        {
            SiteLogUtility.Log_Entry("   DeleteWebPart - In Progress...");
            //var pageRelativeUrl = "/Pages/Home.aspx";
            var pageRelativeUrl = "/Pages/" + pageName + ".aspx";
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
                            oWebPart.DeleteWebPart();
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
                    SiteLogUtility.CreateLogEntry("DeleteWebPart", ex.Message, "Error", "");
                    clientContext.Dispose();
                }
            }
        }
        public static void CreateAspxPage(string siteUrl, string strPageName, string strTitle, string strWPWidth, string strContentWPLink)
        {
            try
            {
                SitePublishUtility spUtility = new SitePublishUtility();
                spUtility.InitializePage(siteUrl, strPageName, strTitle);
                // spUtility.DeleteWebPart(siteUrl, strPageName);

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
                    }
                }
            }
            catch (Exception ex)
            {
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
    }
}
 
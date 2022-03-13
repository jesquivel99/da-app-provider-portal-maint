using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;

namespace SiteUtility
{
    public class SitePublishUtility
    {
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
                    SiteLogUtility.CreateLogEntry("InitializeHomePage", ex.Message, "Error", webUrl);
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
                    SiteLogUtility.CreateLogEntry("DeleteWebPart", ex.Message, "Error", webURL);
                    clientContext.Dispose();
                }
            }
        }
        public static void PublishFunction1()
        {
            Console.WriteLine("PublishFunction 1");
        }
        public static void PublishFunction2()
        {
            Console.WriteLine("PublishFunction 2");
        }
    }
}

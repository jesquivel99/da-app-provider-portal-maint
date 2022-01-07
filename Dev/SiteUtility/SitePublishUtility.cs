using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Microsoft.SharePoint.Client;

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

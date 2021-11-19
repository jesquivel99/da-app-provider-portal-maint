using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SiteUtility
{
    public class SiteRootAdminList
    {
        string sURL = ConfigurationManager.AppSettings["SP_SiteUrl"];
        string sAdminListName = ConfigurationManager.AppSettings["AdminRootListName"];
        public void removeAdminRootSiteSetup()
        {
            try
            {
                //createAdminList(sURL);
                var pageRelativeUrl = "/Pages/AdminHome.aspx";
                using (ClientContext clientContext = new ClientContext(sURL))
                {
                    ListCollection listCol = clientContext.Web.Lists;
                    clientContext.Load(listCol, lists => lists.Include(list => list.Title).Where(list => list.Title == sAdminListName));
                    clientContext.ExecuteQuery();

                    if (listCol.Count > 0)
                    {
                        List olist = clientContext.Web.Lists.GetByTitle(sAdminListName);
                        Web web = clientContext.Web;
                        olist.DeleteObject();
                        clientContext.Load(web);
                        clientContext.ExecuteQuery();

                        SetWelcomePage(sURL, @"SitePages/Home.aspx");

                        File file = web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + pageRelativeUrl);
                        clientContext.Load(file);
                        file.DeleteObject();
                        //file.CheckIn("Delete webpart", CheckinType.MajorCheckIn);
                        //file.Publish("Delete webpart");
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("removeAdminRootSiteSetup", ex.Message, "Error", sURL);
            }
            
        }
        public void setupAdminRootSiteList()
        {
            SiteFilesUtility objSiteFiles = new SiteFilesUtility();
            createAdminList(sURL);
            InitializeHomePage(sURL, "AdminHome", "AdminHome");
            DeleteWebPart(sURL, "AdminHome");
            objSiteFiles.DocumentUpload(sURL, @"C:\Projects\Applications\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\cePrac_AdminRootDataTable.html", "Site Assets");
            ConfigureHomePage(sURL);
            SetWelcomePage(sURL, @"Pages/AdminHome.aspx");
        }
        public void createAdminList(string sURL)
        {
            SiteListUtility objListUtility = new SiteListUtility();
            objListUtility.CreateList(sAdminListName, sURL, (int)ListTemplateType.Links);
            objListUtility.CreateListColumn("<Field Type='Text' DisplayName='PracticeName' Name='PracticeName' />", sAdminListName, sURL);
            objListUtility.CreateListColumn("<Field Type='Text' DisplayName='PracticeTIN' Name='PracticeTIN' />", sAdminListName, sURL);
            objListUtility.CreateListColumn("<Field Type='Text' DisplayName='ProgramParticipation' Name='ProgramParticipation' />", sAdminListName, sURL);
            objListUtility.CreateListColumn("<Field Type='Text' DisplayName='KCEArea' Name='KCEArea' />", sAdminListName, sURL);
            objListUtility.CreateListColumn("<Field Type='Text' DisplayName='AdminGroupPractices' Name='AdminGroupPractices' />", sAdminListName, sURL);
            objListUtility.CreateListColumn("<Field Type='Text' DisplayName='PracticeManagerSite' Name='PracticeManagerSite' />", sAdminListName, sURL);
        }

        public PublishingPage InitializeHomePage(string webUrl, string pageName, string pageTitle)
        {
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

        public void SetWelcomePage(string webUrl,string serverRelativeUrl)
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

        public bool ConfigureHomePage(string webUrl)
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
                var file = clientContext.Web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + "/Pages/AdminHome.aspx");
                file.CheckOut();
                try
                {
                    clientContext.Load(file);
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    try
                    {
                        LimitedWebPartManager olimitedwebpartmanager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);

                        WebPartDefinition wpd1 = olimitedwebpartmanager.ImportWebPart(contentEditorXML("Admin", scntPx, "1100px", web.Url + "/SiteAssets/cePrac_AdminRootDataTable.html"));
                        wpd1.WebPart.Title = "Admin";
                        olimitedwebpartmanager.AddWebPart(wpd1.WebPart, "CenterLeftColumn", 1);

                        WebPartDefinition wpd6 = olimitedwebpartmanager.ImportWebPart(contentEditorXML("SupportStyles", "0px", "0px", web.Url + "/SiteAssets/smlcal.js"));
                        wpd6.WebPart.Title = "SupportStyles";
                        olimitedwebpartmanager.AddWebPart(wpd6.WebPart, "Footer", 1);

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

        public string contentEditorXML(string webPartTitle, string webPartHeight, string webPartWidth, string webPartContentLink)
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
    }
}

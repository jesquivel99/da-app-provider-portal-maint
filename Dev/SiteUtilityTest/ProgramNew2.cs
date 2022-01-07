using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteUtility;
using System.Configuration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using System.Net;
using System.IO;

namespace SiteUtilityTest
{
    public class ProgramNew2
    {
        // AuditMode = true    will NOT execute code to remove SharePoint Permission Groups
        // AuditMode = false   will execute code to remove SharePoint Permission Groups
        public static bool AuditMode = true;
        public List<Practice> practicesIWH = new List<Practice>();
        public List<Practice> practicesCKCC = new List<Practice>();
        public void InitiateProgNew2()
        {
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            string srcUrlIWH = ConfigurationManager.AppSettings["SP_IWHUrl"];
            string srcUrlCKCC = ConfigurationManager.AppSettings["SP_CKCCUrl"];
            string siteInfoFile = ConfigurationManager.AppSettings["Csv_File"];
            string releaseName = "SiteUtilityTest";

            SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            SiteLogUtility.Log_Entry("\n\n=============Release Starts=============", true);

            // Get all existing IWN and iCKCC Practice Data...
            SiteLogUtility.Log_Entry("\n\n=============[ Get all Existing Practice Data (IWN-CKCC) ]=============", true);
            using (ClientContext clientContextIWH = new ClientContext(srcUrlIWH))
            {
                clientContextIWH.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                practicesIWH = GetAllPracticeExistingSites(clientContextIWH, practicesIWH, PracticeType.IWH);
            }
            using (ClientContext clientContextCKCC = new ClientContext(srcUrlCKCC))
            {
                clientContextCKCC.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                practicesCKCC = GetAllPracticeExistingSites(clientContextCKCC, practicesCKCC, PracticeType.iCKCC);
            }

            // Get Portal Data...
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    //  Get all Portal Practice Data...
                    SiteLogUtility.Log_Entry("\n\n=============[ Get all Portal Practice Data ]=============", true);
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext, practicesIWH, practicesCKCC);

                    //  Maintenance Tasks...
                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks ]=============", true);
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        foreach (PracticeSite psite in pm.PracticeSiteCollection)
                        {
                            if (psite.URL.Contains("PM01"))
                            {
                                //SiteLogUtility.Log_Entry("\nPermissions - Test\n\n", true);
                                SiteFilesUtility objSiteFiles = new SiteFilesUtility();
                                //InitializeHomePage(psite.URL, "Home_Backup", "Home_Backup");
                                //getHomePage(psite); 
                                SiteLogUtility.Log_Entry("--\n");
                                SiteLogUtility.Log_Entry($"--Existing Site: {psite.ExistingSiteUrl}");
                                SiteLogUtility.Log_Entry($"--  Portal Site: {psite.URL}");
                                SiteLogUtility.Log_Entry($"--        Audit: {psite.URL}/_layouts/user.aspx");
                                objSiteFiles.DocumentUpload(psite.URL, @"C:\Temp\Home_New.aspx", "Pages");

                                // Add Prac permissions to Practice...
                                SitePermissionUtility.RoleAssignment_AddPracUser(psite);
                                SitePermissionUtility.RoleAssignment_AddPracReadOnly(psite);

                                // Audit Prac permissions...
                                SitePermissionUtility.GetWebGroups(psite);
                                //GetPermission(psite.PracUserReadOnlyPermission, psite.PracUserReadOnlyPermissionDesc, psite.URL);
                                //GetPermission(psite.PracUserPermission, psite.PracUserPermissionDesc, psite.URL);
                            }
                        }
                    }
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

        //------------------------[ Testing - Views ]------------------------------------------------------------------------------

        public void setView(PracticeSite practiceSite)
        {

            using (ClientContext clientContext = new ClientContext(practiceSite.URL))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                List docList = clientContext.Web.Lists.GetByTitle("Documentsckcc");
                View view = docList.DefaultView;
                view.ViewQuery = CreateOrder("Name", true);
                clientContext.Load(view, 
                                      v => v.Id
                                    , v => v.ViewQuery
                                    , v => v.Title
                                    , v => v.ViewFields
                                    , v => v.ViewType
                                    , v => v.DefaultView
                                    , v => v.PersonalView
                                    , v => v.ListViewXml
                                    , v => v.RowLimit);
                view.Update();
                clientContext.ExecuteQuery();
            }
        }

        public static string CreateOrder(string fieldName, bool ascending)
        {
            return string.Format("<OrderBy><FieldRef Name=\"{0}\" Ascending=\"{1}\" /></OrderBy>", fieldName, ascending ? "TRUE" : "FALSE");
        }
        public void ListViewIfExists(PracticeSite practiceSite)
        {
            using (ClientContext clientContext = new ClientContext(practiceSite.URL))
            {
                bool contentExists = false;
                string checkingMessage = "Checking in back";
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                Web w = clientContext.Web;
                List list = w.Lists.GetByTitle("Documentsckcc");
                clientContext.Load(list);
                clientContext.Load(list.Views);
                clientContext.Load(list.Fields);
                clientContext.Load(w);
                clientContext.ExecuteQuery();
                Microsoft.SharePoint.Client.File pvFile = w.GetFileByServerRelativeUrl("/Documentsiwh/Forms/PageViewer.aspx");
                try
                {
                    pvFile.CheckOut();
                    clientContext.Load(pvFile);
                    clientContext.ExecuteQuery();
                    if (pvFile.Exists)
                    {
                        string str1 = @"<SharePoint:RssLink runat=""server"" />";
                        string str2 = @"<link rel=""stylesheet"" type=""text/css"" href=""/_layouts/15/PageViewerCustom.css"" />";

                        FileInformation oFileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, pvFile.ServerRelativeUrl);

                        using (System.IO.StreamReader sr = new System.IO.StreamReader(oFileInfo.Stream))
                        {
                            string line = sr.ReadToEnd();
                            if (!line.Contains(str2) && line.Contains(str1))
                            {
                                contentExists = true;
                            }
                        }
                        if (contentExists)
                        {
                            using (var stream = new MemoryStream())
                            {
                                using (var writer = new StreamWriter(stream))
                                {
                                    writer.WriteLine(str1 + str2);
                                    writer.Flush();
                                    stream.Position = 0;
                                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, pvFile.ServerRelativeUrl, stream, true);
                                    checkingMessage = "Added PageViewerCustom css link";
                                }
                            }
                        }

                        pvFile.CheckIn(checkingMessage, CheckinType.MajorCheckIn);
                        pvFile.Publish(checkingMessage);
                        clientContext.Load(pvFile);
                        clientContext.ExecuteQuery();
                    }
                }
                catch (Exception ex)
                {
                    //SpLog.CreateLog("ReturnListViewIfExists", ex.Message, "Error", clientContext.Web.ServerRelativeUrl);
                    //pvFile.CheckIn(checkingMessage, CheckinType.MajorCheckIn);
                    //pvFile.Publish(checkingMessage);
                    //clientContext.Load(pvFile);
                    //clientContext.ExecuteQuery();
                    //clientContext.Dispose();
                    // ignored
                }
            }
            //Microsoft.SharePoint.Client.View v = list.Views[i];
            //v.Update();
        }


        //------------------------[ Testing - Publishing  ]------------------------------------------------------------------------

        public void getHomePage(PracticeSite practiceSite)
        {
            var pageRelativeUrl = "/Pages/Home.aspx";

            try
            {
                using (ClientContext clientContext = new ClientContext(practiceSite.URL))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    
                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle("Pages");
                    clientContext.Load(web);
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();

                    Microsoft.SharePoint.Client.File fileToDownload = web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + pageRelativeUrl);
                    fileToDownload.CheckOut();
                    // OR...
                    //fileToDownload.CheckIn("Test", CheckinType.MajorCheckIn);
                    //fileToDownload.Publish("Test");
                    //clientContext.Load(fileToDownload);
                    //clientContext.ExecuteQuery();



                    clientContext.Load(fileToDownload);
                    clientContext.ExecuteQuery();

                    if (fileToDownload.Exists)
                    {
                        String fileRef = fileToDownload.ServerRelativeUrl;
                        FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, fileRef);

                        //Test write to log file...
                        //using (System.IO.StreamReader sr = new System.IO.StreamReader(fileInfo.Stream))
                        //{
                        //    string line = sr.ReadToEnd();
                        //    SiteLogUtility.Log_Entry(line, true);
                        //}

                        //String fileName = Path.Combine("C:\\Temp", (string)fileToDownload.Name);
                        String fileName = Path.Combine("C:\\Temp", "Home_Backup.aspx");

                        using (var fileStream = System.IO.File.Create(fileName))
                        {
                            fileInfo.Stream.CopyTo(fileStream);
                        }
                    }

                    //fileToDownload.CheckIn("Home.aspx stream to Log and saved as Home_Backup.aspx", CheckinType.MajorCheckIn);
                    //fileToDownload.Publish("Home.aspx stream to Log and saved as Home_Backup.aspx");
                    //clientContext.Load(fileToDownload);
                    //clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("getHomePage", ex.Message, "Error", "");
            }
        }

        //public void PublishPage(PracticeSite practiceSite)
        //{
        //    using (ClientContext clientContext = new ClientContext(practiceSite.URL))
        //    {
        //        clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
        //        Web web = clientContext.Web;
        //        clientContext.Load(web);
        //        //clientContext.ExecuteQuery();

        //        // This value is NOT List internal name
        //        List targetList = clientContext.Web.Lists.GetByTitle("Pages");
        //        clientContext.Load(targetList);
        //        clientContext.ExecuteQuery();

        //        Folder folder = targetList.RootFolder;
        //        FileCollection files = folder.Files;
        //        clientContext.Load(files);
        //        clientContext.ExecuteQuery();

        //        SiteLogUtility.Log_Entry(targetList.EntityTypeName + " - " + practiceSite.URL, true);
        //        foreach (File f in files)
        //        {
        //            SiteLogUtility.Log_Entry(f.Name, true);
        //        }
        //    }
        //}

        //public void renamePage(PracticeSite practiceSite)
        //{
        //    try
        //    {
        //        String filename = pageName + ".aspx";
        //        String title = pageTitle;
        //        String list = "Pages";

        //        var pageRelativeUrl = "/Pages/HomeTest.aspx";
        //        using (ClientContext clientContext = new ClientContext(practiceSite.URL))
        //        {
        //            clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
        //            Web web = clientContext.Web;
        //            clientContext.Load(web);
        //            clientContext.ExecuteQuery();

        //            // Get Page Layout
        //            Microsoft.SharePoint.Client.File pageFromDocLayout = clientContext.Site.RootWeb.GetFileByServerRelativeUrl(String.Format("{0}/_catalogs/masterpage/BlankWebPartPage.aspx", clientContext.Site.RootWeb.ServerRelativeUrl.TrimEnd('/')));
        //            Microsoft.SharePoint.Client.ListItem pageLayoutItem = pageFromDocLayout.ListItemAllFields;
        //            clientContext.Load(pageLayoutItem);
        //            clientContext.ExecuteQuery();

        //            // Create Publishing Page
        //            PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(clientContext, web);
        //            PublishingPage page = publishingWeb.AddPublishingPage(new PublishingPageInformation
        //            {
        //                Name = filename,
        //                PageLayoutListItem = pageLayoutItem
        //            });
        //            clientContext.ExecuteQuery();

        //            // Set Page Title and Publish Page
        //            Microsoft.SharePoint.Client.ListItem pageItem = page.ListItem;
        //            pageItem["Title"] = title;
        //            pageItem.Update();
        //            pageItem.File.CheckIn(String.Empty, CheckinType.MajorCheckIn);
        //            clientContext.ExecuteQuery();
        //            return page;

        //            File file = web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + pageRelativeUrl);
        //            clientContext.Load(file);
        //            file.CheckOut();
        //            //file.CheckIn("Delete webpart", CheckinType.MajorCheckIn);
        //            //file.Publish("Delete webpart");
        //            clientContext.Load(file);
        //            clientContext.ExecuteQuery();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SiteLogUtility.CreateLogEntry("removeAdminRootSiteSetup", ex.Message, "Error", sURL);
        //    }

        //}

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

        //------------------------[ Testing - Permissions  ]------------------------------------------------------------------------

        public static bool RoleAssignment_AddPortalBusinessAdminUserReadOnly(PracticeSite pracInfo)
        {
            string pTin = pracInfo.PracticeTIN;

            //string path = siteUrl + pracInfo.SiteMgrRegionRef + "/" + pracInfo.PracticeTIN;
            string path = pracInfo.URL;

            try
            {
                using (ClientContext ctx = new ClientContext(path))
                {
                    ctx.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web w = ctx.Web;
                    ctx.Load(w);
                    ctx.ExecuteQuery();

                    //Get by name > RoleDefinition...
                    RoleDefinition roleReadOnly = w.RoleDefinitions.GetByName("Read");

                    //Get by name > Group...
                    Group oGroup = w.SiteGroups.GetByName("Portal_Business_Admin_User");

                    RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(ctx);
                    collRoleDefinitionBinding.Add(roleReadOnly);

                    // Add Group and RoleDefinitionBinding to RoleAssignments...
                    w.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

                    ctx.Load(oGroup, group => group.Title);
                    ctx.Load(roleReadOnly, role => role.Name);
                    ctx.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("RoleAssignment_AddPortalBusinessAdminUserReadOnly", ex.Message, "Error", "");
                return false;
            }

            return true;
        }

        public static bool RoleAssignment_AddRiskAdjustmentUserReadOnly(PracticeSite pracInfo)
        {
            string pTin = pracInfo.PracticeTIN;

            //string path = siteUrl + pracInfo.SiteMgrRegionRef + "/" + pracInfo.PracticeTIN;
            string path = pracInfo.URL;

            try
            {
                using (ClientContext ctx = new ClientContext(path))
                {
                    ctx.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web w = ctx.Web;
                    ctx.Load(w);
                    ctx.ExecuteQuery();

                    //Get by name > RoleDefinition...
                    RoleDefinition roleReadOnly = w.RoleDefinitions.GetByName("Read");

                    //Get by name > Group...
                    Group oGroup = w.SiteGroups.GetByName("Risk_Adjustment_User");

                    RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(ctx);
                    collRoleDefinitionBinding.Add(roleReadOnly);

                    // Add Group and RoleDefinitionBinding to RoleAssignments...
                    w.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

                    ctx.Load(oGroup, group => group.Title);
                    ctx.Load(roleReadOnly, role => role.Name);
                    ctx.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("RoleAssignment_AddRiskAdjustmentUserReadOnly", ex.Message, "Error", "");
                return false;
            }

            return true;
        }

        public static bool GetWebGroups(PracticeSite pracInfo)
        {
            var path = pracInfo.URL;

            try
            {
                using (ClientContext clientContext = new ClientContext(path))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;

                    //Parameters to receive response from the server    
                    //RoleAssignments property should be passed in Load method to get the collection of Groups assigned to the web    
                    clientContext.Load(web, w => w.Title);
                    RoleAssignmentCollection roleAssignmentColl = web.RoleAssignments;

                    //RoleAssignment.Member property returns the group associated to the web  
                    //RoleAssignement.RoleDefinitionBindings property returns the permissions associated to the group for the web  
                    clientContext.Load(roleAssignmentColl,
                        roleAssignement => roleAssignement.Include(
                            r => r.Member,
                            r => r.RoleDefinitionBindings));
                    clientContext.ExecuteQuery();


                    SiteLogUtility.LogText = $"Groups has permission to the Web:  {web.Title}";
                    SiteLogUtility.Log_Entry(SiteLogUtility.LogText, true);

                    SiteLogUtility.LogText = $"Groups Count:  {roleAssignmentColl.Count}";
                    SiteLogUtility.Log_Entry(SiteLogUtility.LogText, true);

                    SiteLogUtility.LogText = "Group with Permissions as follows:  ";
                    SiteLogUtility.Log_Entry(SiteLogUtility.LogText, true);

                    foreach (RoleAssignment grp in roleAssignmentColl)
                    {
                        string strGroup = "";
                        strGroup += $"    {grp.Member.Title} : ";

                        foreach (RoleDefinition rd in grp.RoleDefinitionBindings)
                        {
                            strGroup += $"{rd.Name} ";
                        }
                        SiteLogUtility.Log_Entry(strGroup, true);
                    }
                    //Console.Read();
                }
            }
            catch (Exception)
            {
                return false;
                //throw;
            }

            return true;
        }

        //------------------------[ Testing - Web ]--------------------------------------------------------------------------------
        //public bool CheckForSiteExistance(string sUrl)
        //{
        //    string webURL = SUrl;   // + @"/" + SiteName;
        //    using (ClientContext clientContext = new ClientContext(webURL))
        //    {
        //        try
        //        {
        //            clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
        //            var web = clientContext.Web;
        //            clientContext.Load(web, w => w.ServerRelativeUrl, w => w.Webs);
        //            clientContext.ExecuteQuery();

        //            var subWeb = web.Webs.Where(w => w.ServerRelativeUrl.Contains(SiteName)).SingleOrDefault();
        //            if (subWeb != null)
        //                return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            clientContext.Dispose();
        //            SpLog.CreateLog("CheckForSiteExistance", ex.Message, "Error", sUrl);
        //            // ignored
        //        }
        //    }
        //    return false;
        //}

        //public void RetrieveWeb()
        //{
        //    string webURL = SUrl + @"/" + SiteName;
        //    using (ClientContext clientContext = new ClientContext(webURL))
        //    {
        //        clientContext.Credentials = new NetworkCredential(SpCredential.UserName, SpCredential.Password, SpCredential.Domain);
        //        var web = clientContext.Web;
        //        clientContext.Load(web);
        //        clientContext.ExecuteQuery();
        //        WUrl = web.ServerRelativeUrl;
        //        Web = web;
        //    }
        //}

        //------------------------[ Testing all programs below or pulling code to use for above ]----------------------------------
        public static bool GetSpGroups(ProgramManagerSite pmInfo, PracticeSite pracInfo)
        {
            try
            {
                var path = pracInfo.URL;

                SiteLogUtility.LogText = $"Processing:  {path}";
                SiteLogUtility.Log_Entry(SiteLogUtility.LogText, true);

                // Set Permission Property Values...
                //SetPermissionValue(pmInfo, pracInfo);

                using (ClientContext clientContext = new ClientContext(path))
                {
                    bool removePracUserGroup = false;
                    bool removePracReadOnlyGroup = false;
                    bool removeSiteMgrGroup = false;
                    bool removeSiteMgrReadOnlyGroup = false;

                    try
                    {
                        removePracUserGroup = RemoveSpGroups(pracInfo.PracUserPermission, path);
                        removePracReadOnlyGroup = RemoveSpGroups(pracInfo.PracUserReadOnlyPermission, path);
                        removeSiteMgrGroup = RemoveSpGroups(pmInfo.IWNSiteMgrPermission, path);
                        removeSiteMgrReadOnlyGroup = RemoveSpGroups(pmInfo.IWNSiteMgrReadOnlyPermission, path);
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.Log_Entry("GetSpGroups Error: " + ex.ToString(), true);
                    }

                    //finally
                    //{
                    //    SiteLogUtility.Log_Entry("Remove Summary: " +
                    //        "PracUserGroup = " + removePracUserGroup.ToString() + " | " +
                    //        "PracReadOnlyGroup = " + removePracReadOnlyGroup.ToString() + " | " +
                    //        "SiteMgrGroup = " + removeSiteMgrGroup.ToString() + " | " +
                    //        "SiteMgrReadOnly = " + removeSiteMgrReadOnlyGroup.ToString());
                    //}

                }
            }

            catch (Exception ex)
            {
                SiteLogUtility.Log_Entry("GetSpGroups Error: " + ex.ToString(), true);
                return false;
            }

            return true;
        }

        public static bool RemoveAllSpGroups(ProgramManagerSite pmInfo, PracticeSite pracInfo)
        {
            try
            {
                var path = pracInfo.URL;

                SiteLogUtility.LogText = $"Processing:  {path}";
                Console.WriteLine(SiteLogUtility.LogText);
                SiteLogUtility.Log_Entry(SiteLogUtility.LogText);

                using (ClientContext clientContext = new ClientContext(path))
                {
                    bool removePracUserGroup = false;
                    bool removePracReadOnlyGroup = false;
                    bool removeSiteMgrGroup = false;
                    bool removeSiteMgrReadOnlyGroup = false;

                    try
                    {
                        removePracUserGroup = RemoveSpGroups(pracInfo.PracUserPermission, path);
                        removePracReadOnlyGroup = RemoveSpGroups(pracInfo.PracUserReadOnlyPermission, path);
                        removeSiteMgrGroup = RemoveSpGroups(pmInfo.IWNSiteMgrPermission, path);
                        removeSiteMgrReadOnlyGroup = RemoveSpGroups(pmInfo.IWNSiteMgrReadOnlyPermission, path);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("GetSpGroups Error: " + ex.ToString());
                        SiteLogUtility.Log_Entry("GetSpGroups Error: " + ex.ToString());
                    }

                    //finally
                    //{
                    //    SiteLogUtility.Log_Entry("Remove Summary: " +
                    //        "PracUserGroup = " + removePracUserGroup.ToString() + " | " +
                    //        "PracReadOnlyGroup = " + removePracReadOnlyGroup.ToString() + " | " +
                    //        "SiteMgrGroup = " + removeSiteMgrGroup.ToString() + " | " +
                    //        "SiteMgrReadOnly = " + removeSiteMgrReadOnlyGroup.ToString());
                    //}

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("GetSpGroups Error: " + ex.ToString());
                SiteLogUtility.Log_Entry("GetSpGroups Error: " + ex.ToString());
                return false;
            }

            return true;
        }

        public static bool RemoveSingleSpGroup(string spUserGroup, string permLevel, string sUrl)
        {
            try
            {
                SiteLogUtility.LogText = $"Processing:  {sUrl}";
                SiteLogUtility.Log_Entry(SiteLogUtility.LogText, true);

                using (ClientContext clientContext = new ClientContext(sUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                    bool removePracUserGroup = false;
                    string getPracUserPermission = string.Empty;

                    try
                    {
                        getPracUserPermission = GetPermission(spUserGroup, permLevel, sUrl);  //this actually deletes at the moment...
                        removePracUserGroup = RemoveSpGroups(spUserGroup, sUrl);
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.Log_Entry("GetSpGroups Error: " + ex.ToString(), true);
                    }

                }
            }

            catch (Exception ex)
            {
                SiteLogUtility.Log_Entry("GetSpGroups Error: " + ex.ToString(), true);
                return false;
            }

            return true;
        }

        private static string GetPermission(string spUserGroup, string permLevel, string sUrl)
        {
            using (ClientContext clientContext = new ClientContext(sUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    clientContext.Load(clientContext.Web,
                        web => web.SiteGroups.Include(
                            g => g.Title,
                            g => g.Id),
                        web => web.RoleAssignments.Include(
                            assignment => assignment.PrincipalId,
                            assignment => assignment.Member,
                            assignment => assignment.RoleDefinitionBindings.Include(
                                defBindings => defBindings.Name)),
                        web => web.RoleDefinitions.Include(
                            definition => definition.Id,
                            definition => definition.Name,
                            definition => definition.Description));
                    clientContext.ExecuteQuery();

                    RoleDefinition readDef = clientContext.Web.RoleDefinitions.FirstOrDefault(
                            definition => definition.Name == permLevel);
                    Group group = clientContext.Web.SiteGroups.FirstOrDefault(
                            g => g.Title == spUserGroup);
                    if (readDef == null || group == null) return "";

                    foreach (RoleAssignment roleAssignment in clientContext.Web.RoleAssignments)
                    {
                        if (roleAssignment.PrincipalId == group.Id)
                        {
                            SiteLogUtility.Log_Entry($"{roleAssignment.Member} - PrincipalId: {roleAssignment.PrincipalId}  - GroupId: {group.Id}", true);

                            // If we want to Remove selected Permission
                            //roleAssignment.RoleDefinitionBindings.Remove(readDef);
                        }
                        SiteLogUtility.Log_Entry($"{roleAssignment.Member} - PrincipalId: {roleAssignment.PrincipalId} - RoleDefBindings Cnt: {roleAssignment.RoleDefinitionBindings.Count}", true);
                        clientContext.ExecuteQuery();
                    }

                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("GetPermission", ex.Message, "Error", sUrl);
                }
                return "";
            }
        }

        private static bool RemoveSpGroups(string spUserGroup, string path)
        {
            using (ClientContext clientContext = new ClientContext(path))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    Web web = null;
                    web = clientContext.Web;
                    RoleAssignmentCollection assignColl;
                    RoleAssignment roleAssign;

                    string userOrGroup = spUserGroup; //we can give either title or login Name of the user/group.
                    string permissionLevel = "Practice Site User Permission Level"; //we can Give name of any permission level name.

                    clientContext.Load(web.RoleAssignments,
                        roles => roles.Include(
                            r => r.Member,
                            r => r.Member.LoginName,
                            r => r.Member.Title,
                            r => r.RoleDefinitionBindings
                    ));

                    clientContext.ExecuteQuery();

                    assignColl = web.RoleAssignments;

                    //for (int isitePermCount = 0; isitePermCount < assignColl.Count; isitePermCount++)
                    for (int isitePermCount = 0; isitePermCount < assignColl.Count; isitePermCount++)
                    {
                        try
                        {
                            roleAssign = assignColl[isitePermCount];
                            string userLoginName = string.Empty;
                            string userTitle = string.Empty;
                            userLoginName = roleAssign.Member.LoginName;
                            userTitle = roleAssign.Member.Title;

                            if (userTitle == userOrGroup || userLoginName == userOrGroup)

                            {
                                //Get the roledefinition from it’s name
                                RoleDefinition roleDef = web.RoleDefinitions.GetByName(permissionLevel);

                                // If we want to Remove selected Permission
                                //roleAssign.RoleDefinitionBindings.Remove(roleDef);
                                SiteLogUtility.LogText = $"Will be removed: {roleAssign.PrincipalId}";
                                SiteLogUtility.Log_Entry(SiteLogUtility.LogText);

                                // If we want to Add selected Permission
                                //roleAssign.RoleDefinitionBindings.Add(roleDef);
                                //roleAssign.Update();
                                clientContext.ExecuteQuery();

                                Console.WriteLine(SiteLogUtility.LogText);
                            }
                        }

                        catch
                        {
                            return false;
                        }
                    }
                    //Console.ReadLine();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }

        public static bool RoleAssignmentCollection_Add()
        {
            return true;
        }

        public static bool RoleAssignmentCollection_AddGroupReadOnly(PracticeSite pracInfo)
        {
            var path = pracInfo.URL;

            try
            {
                using (ClientContext clientContext = new ClientContext(path))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;

                    Group oGroup = web.SiteGroups.GetByName(pracInfo.PracUserReadOnlyPermission);
                    RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(clientContext);
                    RoleDefinition oRoleDefinition = web.RoleDefinitions.GetByType(RoleType.Reader);
                    collRoleDefinitionBinding.Add(oRoleDefinition);
                    web.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);
                    clientContext.Load(oGroup,
                        g => g.Id,
                        g => g.Title);
                    clientContext.Load(oRoleDefinition,
                        role => role.Id,
                        role => role.Name);
                    clientContext.ExecuteQuery();

                    SiteLogUtility.LogText = $"{oGroup.Title} created and assigned {oRoleDefinition.Name} role.";
                    SiteLogUtility.Log_Entry(SiteLogUtility.LogText, true);
                }
            }
            catch (Exception)
            {
                return false;
                //throw;
            }

            return true;
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

        private string MapExistingSite(string TIN)
        {
            Practice practice = practicesIWH.Where(p => p.ExistingSiteUrl.Contains(TIN)).FirstOrDefault();
            if (practice == null)
                practice = practicesCKCC.Where(p => p.ExistingSiteUrl.Contains(TIN)).FirstOrDefault();

            if (practice == null)
            {
                Console.WriteLine(TIN);
                return "";
            }
            else
                return practice.ExistingSiteUrl;
        }
        private string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        ///// <summary>
        ///// Set Group SiteManager - Practice Manager Site Permission Level ...
        ///// ICKCCGroup01_SiteManager
        ///// Practice Manager Site Permission Level
        ///// </summary>
        ///// <param name="pracInfo"></param>
        ///// <returns></returns>
        //private static bool RoleAssignment_AddSiteManager(SiteInfo pracInfo, List<PmAssignment> pmAssignment)
        //{
        //    int sStart = pracInfo.PMSiteName.Length - 2;
        //    string PMid = pracInfo.PMSiteName.Substring(sStart, 2);
        //    PmAssignment result = pmAssignment.Find(x => x.PMRefId == PMid);

        //    //string path = siteUrl + pracInfo.SiteMgrRegionRef + "/" + pracInfo.PracticeTIN;
        //    string path = pracInfo.SiteUrl;

        //    try
        //    {
        //        using (ClientContext ctx = new ClientContext(path))
        //        {
        //            Web w = ctx.Web;
        //            ctx.Load(w);
        //            ctx.ExecuteQuery();

        //            //Get by name > RoleDefinition...
        //            RoleDefinition oRoleDefinition = w.RoleDefinitions.GetByName("Practice Manager Site Permission Level");

        //            //Get by name > Group...
        //            //Group oGroup = w.SiteGroups.GetByName(pracInfo.SiteMgrRegionRef + "_SiteManager");
        //            Group oGroup = w.SiteGroups.GetByName(result.PMGroup + "_SiteManager");

        //            RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(ctx);
        //            collRoleDefinitionBinding.Add(oRoleDefinition);

        //            // Add Group and RoleDefinitionBinding to RoleAssignments...
        //            w.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

        //            ctx.Load(oGroup, group => group.Title);
        //            ctx.Load(oRoleDefinition, role => role.Name);
        //            ctx.ExecuteQuery();

        //            Console.WriteLine($"{oGroup.Title} created and assigned {oRoleDefinition.Name} role.");
        //            SiteLogUtility.Log_Entry($"{oGroup.Title} created and assigned {oRoleDefinition.Name} role.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SiteLogUtility.CreateLogEntry("RoleAssignmentCollection", ex.Message, "Error", "");
        //        return false;
        //    }

        //    return true;
        //}

        public static void SetPermissionValue(ProgramManagerSite pmsi, PracticeSite si)
        {
            si.PracUserPermission = "Prac_" + si.PracticeTIN + "_User";
            si.PracUserReadOnlyPermission = "Prac_" + si.PracticeTIN + "_ReadOnly";
            pmsi.IWNSiteMgrPermission = si.IWNRegion + "_SiteManager";
            pmsi.IWNSiteMgrReadOnlyPermission = si.IWNRegion + "_ReadOnly";

        }

        public static void SetPracPermissionValue(ProgramManagerSite pmsi, PracticeSite si)
        {
            si.PracUserPermission = "Prac_" + si.PracticeTIN + "_User";
            si.PracUserReadOnlyPermission = "Prac_" + si.PracticeTIN + "_ReadOnly";
            pmsi.IWNSiteMgrPermission = si.IWNRegion + "_SiteManager";
            pmsi.IWNSiteMgrReadOnlyPermission = si.IWNRegion + "_ReadOnly";

        }

        public static void SetPMPermissionValue(ProgramManagerSite pmsi, PracticeSite si)
        {
            pmsi.IWNSiteMgrPermission = si.IWNRegion + "_SiteManager";
            pmsi.IWNSiteMgrReadOnlyPermission = si.IWNRegion + "_ReadOnly";

        }

        public static void Permissions_BAK()
        {
            //----------------------------------------------------------------------
            //Web web = clientContext.Web;
            //clientContext.Load(web);
            //clientContext.ExecuteQuery();

            //var contributeRole = web.RoleDefinitions.GetByType(RoleType.Contributor);
            //var contributeRole2 = web.RoleDefinitions.GetByName("Read");

            //var group = web.SiteGroups.GetByName(spUserGroup);
            //var groupAssignment = web.RoleAssignments.GetByPrincipalId(group.Id);
            //var roles = groupAssignment.RoleDefinitionBindings;

            //clientContext.Load(group);
            //clientContext.Load(groupAssignment);
            //clientContext.ExecuteQuery();

            //if (roles.Contains(contributeRole2))
            //{
            //    //roles.Remove(contributeRole);
            //    //groupAssignment.Update();
            //    SiteLogUtility.Log_Entry($"{groupAssignment.PrincipalId} - {contributeRole2.Name}");
            //}
            //----------------------------------------------------------------------

            ////////clientContext.Load(
            ////////    clientContext.Web,
            ////////    web => web.SiteGroups.Include(
            ////////        g => g.Title,
            ////////        g => g.Id),
            ////////        web => web.RoleAssignments.Include(

            ////////            assignment => assignment.PrincipalId,
            ////////            assignment => assignment.RoleDefinitionBindings.Include(

            ////////                definitionb => definitionb.Name)),
            ////////        web => web.RoleDefinitions.Include(

            ////////                definition => definition.Name));

            ////////clientContext.ExecuteQuery();

            ////////RoleDefinition readDef = clientContext.Web.RoleDefinitions.FirstOrDefault(
            ////////        definition => definition.Name == "Read");
            ////////Group group = clientContext.Web.SiteGroups.FirstOrDefault(
            ////////        g => g.Title == spUserGroup);
            ////////if (readDef == null || group == null) return false;

            ////////foreach (RoleAssignment roleAssignment in clientContext.Web.RoleAssignments)
            ////////{
            ////////    if(roleAssignment.PrincipalId == group.Id)
            ////////    {
            ////////        SiteLogUtility.Log_Entry($"PrincipalId: {roleAssignment.PrincipalId}  - GroupId: {group.Id}");
            ////////    }
            ////////    SiteLogUtility.Log_Entry($"PrincipalId: {roleAssignment.PrincipalId} - RoleDefBindings Cnt: {roleAssignment.RoleDefinitionBindings.Count}");
            ////////    //clientContext.ExecuteQuery();
            ////////}

            //foreach (var rd in from roleAssignment in clientContext.Web.RoleAssignments
            //                   where roleAssignment.PrincipalId == @group.Id
            //                   from rd in roleAssignment.RoleDefinitionBindings.Where(
            //                       rd => rd.Name == readDef.Name)
            //                   select rd)
            //{
            //    //rd.DeleteObject();
            //    SiteLogUtility.Log_Entry($"Will be deleted: {rd.Name} - {rd.Description} - {rd.Id}");
            //}
            //clientContext.ExecuteQuery();
            //}
            //}
            //catch (Exception ex)
            //{
            //    SiteLogUtility.LogText = path;
            //    SiteLogUtility.Log_Entry(SiteLogUtility.LogText);
            //    SiteLogUtility.Log_Entry("RemoveSpGroups Error: " + ex.Message.ToString());
            //    return false;
            //}
            //return true;
            //}
        }
    }
}

using System;
using SiteUtility;
using System.Configuration;
using Microsoft.SharePoint.Client;
using System.Net;
using Serilog;
using System.Collections.Generic;
using Microsoft.SharePoint.Client.WebParts;
using System.Linq;

namespace SiteUtilityTest
{
    public class ProgramNew_JE
    {
        //const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
        //static ILogger _logger = Log.Logger = new LoggerConfiguration()
        //   .MinimumLevel.Debug()
        //   .Enrich.FromLogContext()
        //   .WriteTo.Console()
        //   .WriteTo.File("Logs/maint" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + "_.log", rollingInterval: RollingInterval.Day, shared: false, outputTemplate: outputTemp1)
        //   .CreateLogger();
        static ILogger logger = Log.ForContext<ProgramNew_JE>();
        private Guid _listGuid = Guid.Empty;
        readonly string EmailToMe = ConfigurationManager.AppSettings["EmailStatusToMe"];
        public void InitiateProg()
        {
            string releaseName = "ProgramNew_JE";
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();
            SiteLogUtility siteLogUtility = new SiteLogUtility();

            List<Practice> practices = siteInfoUtility.GetAllCKCCPractices();
            //List<Practice> practices = siteInfoUtility.GetPracticesByPM("01");

            //SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            siteLogUtility.LoggerInfo_Entry("This is the Release Name: " + releaseName);
            siteLogUtility.LoggerInfo_Entry("========================================Release Starts========================================");

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    siteLogUtility.LoggerInfo_Entry("-------------[ Maintenance Tasks - Start            ]-------------");

                    foreach (Practice practice in practices)
                    {
                        // Build xml configuration file...
                        //SiteUtility.SitePMData.InitialConnectDBPortalDeployed("PM06");
                        
                        siteLogUtility.LoggerInfoBody(practice);

                        //siteLogUtility.LoggerInfoBody(practice);
                        //siteInfoUtility.Init_UpdateAllProgramParticipation(practice);

                        //SiteNavigateUtility.ClearQuickNavigationRecent(practice.NewSiteUrl);
                        //SiteNavigateUtility.RenameQuickNavigationNode(practice.NewSiteUrl, "Hospitalization Alert", "Hospitalization Alerts");
                    }

                    siteLogUtility.LoggerInfo_Entry("-------------[ Maintenance Tasks - End              ]-------------");

                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
                    logger.Error("Error: " + ex.Message);
                }
                finally
                {
                    siteLogUtility.LoggerInfo_Entry(SiteLogUtility.textLine0);
                    //SiteLogUtility.finalLog("Final: " + releaseName);
                    SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", EmailToMe);
                }
                siteLogUtility.LoggerInfo_Entry("========================================Release Ends========================================");
            }

            Log.CloseAndFlush();
        }
        public void InitiateProg(string siteId)
        {
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();
            SiteLogUtility siteLogUtility = new SiteLogUtility();
            SiteListUtility siteListUtility = new SiteListUtility();
            

            //List<Practice> practices = siteInfoUtility.GetAllCKCCPractices();
            //List<Practice> practices = siteInfoUtility.GetPracticesByPM("01");
            List<Practice> practices = new List<Practice>();
            Practice practice = siteInfoUtility.GetPracticeBySiteID(siteId);
            practices.Add(practice);

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    siteLogUtility.LoggerInfo_Entry("======================================== ProgramNew_JE - Release Start ========================================", true);

                    siteLogUtility.LoggerInfoBody(practice);
                    //Init_BuildXmlConfig();
                    //Init_AddPayorEnrollment(practices);

                    //Testing...
                    //siteLogUtility.LoggerInfoBody(practice);
                    //siteInfoUtility.Init_UpdateAllProgramParticipation(practice);
                    //AddPermissionGroup_PayorEnrollment(siteId,"Referrals", "Contribute_NoDelete");
                    //AddPermissionGroup_PayorEnrollment(siteId,"ReferralsPrevious", "Contribute_NoDelete");
                    //SitePermissionUtility.BreakRoleInheritanceOnList(practice.NewSiteUrl, siteListUtility.listNameRiskAdjustmentIwh, "Risk_Adjustment_User", RoleType.Contributor);
                    //BreakRoleInheritanceForList(practice.NewSiteUrl, "RiskAdjustment_ckcc");
                    //BreakRoleInheritanceForList(practice.NewSiteUrl, "RiskAdjustment_iwh");
                    //SiteNavigateUtility.ClearQuickNavigationRecent(practice.NewSiteUrl);
                    //SiteNavigateUtility.RenameQuickNavigationNode(practice.NewSiteUrl, "Hospitalization Alerts Coming Soon", "Hospitalization Alerts");
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
                    logger.Error("Error: " + ex.Message);
                }
                finally
                {
                    siteLogUtility.LoggerInfo_Entry(SiteLogUtility.textLine0);
                    siteLogUtility.LoggerInfo_Entry("======================================== ProgramNew_JE - Release End ========================================", true);

                    //SiteLogUtility.finalLog("Final: " + releaseName);
                    //SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@interwellhealth.com");
                }
            }

            //Log.CloseAndFlush();
        }

        public void AddPermissionGroup_PayorEnrollment(string siteId, string listName, string permType)
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

        private void Init_AddPayorEnrollment(List<Practice> practices)
        {
            SiteLogUtility siteLogUtility = new SiteLogUtility();
            try
            {
                foreach (var practice in practices)
                {
                    siteLogUtility.LoggerInfoBody(practice);
                    if (practice.IsKC365 == true)
                    {
                        if (PayorEnrollment_Setup(practice, practice.NewSiteUrl))
                        {
                            UpdateUrlRef(practice, "Program Participation");
                            AddPermissionGroup_PayorEnrollment(practice.SiteID, "Referrals", "Contribute_NoDelete");
                            AddPermissionGroup_PayorEnrollment(practice.SiteID, "ReferralsPrevious", "Contribute_NoDelete");

                            logger.Information(practice.Name);
                            logger.Information(" Payor Enrollment setup is completed");
                        }
                    } 
                }
            }
            catch (Exception ex)
            {
                logger.Information("ERROR Init_AddPayorEnrollment: " + ex.Message);
            }
        }

        private void Init_BuildXmlConfig()
        {
            try
            {
                SiteUtility.SitePMData.InitialConnectDBPortalDeployed("PM06");
            }
            catch (Exception ex)
            {
                logger.Information("ERROR - Init_BuildXmlConfig: " + ex.Message);
            }
        }

        public void GetListGuid(string wUrl, string listName)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(wUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    
                    List list = clientContext.Web.Lists.GetByTitle(listName);
                    clientContext.Load(list, o => o.Id, o => o.ContentTypes);
                    clientContext.ExecuteQuery();
                    if (list.Id != Guid.Empty)
                    {
                        _listGuid = list.Id;
                    }
                }
                
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetListGuid", ex.Message, "Error", "");
            }
        }

        public void GetListContentTypes(string wUrl, string listName)
        {
            GetListGuid(wUrl, listName);

            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                ContentTypeCollection contentTypes = clientContext.Web.AvailableContentTypes;
                ListCollection lists = clientContext.Web.Lists;
                Web web = clientContext.Web;
                clientContext.Load(web);
                clientContext.Load(lists);
                clientContext.Load(contentTypes);
                clientContext.ExecuteQuery();

                List list1 = lists.GetById(_listGuid);
                ContentType contentType;
                clientContext.Load(list1);
                clientContext.ExecuteQuery();

                if (DoesContentType_Exist(list1.ContentTypes, "Text"))
                {
                    contentType = RetrieveExistingContentType(list1.ContentTypes, "Text");
                }
            }
        }
        public bool DoesContentType_Exist(ContentTypeCollection spc, string name)
        {
            foreach (ContentType c in spc)
            {
                if (c.Name.ToLower() == name.ToLower())
                {
                    return true;
                }
            }

            return false;
        }
        public ContentType RetrieveExistingContentType(ContentTypeCollection spc, string name)
        {
            foreach (ContentType c in spc)
            {
                if (c.Name.ToLower() == name.ToLower())
                {
                    return c;
                }
            }

            return null;
        }
        public RoleDefinition PracticeSiteManagerPermissionLevel(string siteUrl)
        {
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                {
                    Web web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();

                    RoleDefinition def = web.RoleDefinitions.GetByName("Practice Manager Site Permission Level");
                    clientContext.Load(def);
                    clientContext.ExecuteQuery();

                    return def;
                }
            }
        }
        public static void BreakRoleInheritanceForList(string wUrl, string listName)
        {
            SiteLogUtility siteLogUtility = new SiteLogUtility();
            try
            {
                using (ClientContext clientContext = new ClientContext(wUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;
                    clientContext.Load(clientContext.Web, a => a.Lists);
                    //clientContext.ExecuteQuery();

                    List list = clientContext.Web.Lists.GetByTitle(listName);
                    clientContext.Load(list, l => l.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();
                    if (list.HasUniqueRoleAssignments == false)
                    {
                        list.BreakRoleInheritance(true, false);
                        list.Update();
                        clientContext.ExecuteQuery();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("BreakRoleInheritanceForList", ex.Message, "Error", "");
            }
        }
        public static Boolean CheckListUniquePermissions(string wUrl, string listName)
        {
            SiteLogUtility siteLogUtility = new SiteLogUtility();
            Boolean permissionsUnique = false;
            try
            {
                using (ClientContext clientContext = new ClientContext(wUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;

                    List list = clientContext.Web.Lists.GetByTitle(listName);
                    clientContext.Load(list, l => l.HasUniqueRoleAssignments, l => l.Title);
                    clientContext.ExecuteQuery();


                    if (list.HasUniqueRoleAssignments)
                    {
                        siteLogUtility.LoggerInfo_Entry(listName + " has Unique Permissions - " + list.HasUniqueRoleAssignments.ToString());
                    }
                    else
                    {
                        siteLogUtility.LoggerInfo_Entry(listName + " INHERITS Permissions - " + list.HasUniqueRoleAssignments.ToString());
                    }

                    return permissionsUnique = list.HasUniqueRoleAssignments ? true : false;
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("BreakRoleInheritanceForList", ex.Message, "Error", "");
                return false;
            }
        }

        public bool PayorEnrollment_Setup(Practice practice, string siteUrl)
        {
            string urlSiteAssets = SiteInfoUtility.GetReferralUrl(practice.NewSiteUrl);
            try
            {
                //SiteFilesUtility objSiteFiles = new SiteFilesUtility();
                //objSiteFiles.DocumentUpload(siteUrl, @"C:\Users\nalkazaki\OneDrive - Fresenius Medical Care\Documents\VisualStudio\PayorEnrollment\PayorEnrollment.html", "SiteAssets");
                //objSiteFiles.DocumentUpload(siteUrl, @"C:\Users\nalkazaki\OneDrive - Fresenius Medical Care\Documents\VisualStudio\PayorEnrollment\bootstrap-float-label.min.css", "SiteAssets");

                if (!SiteFilesUtility.FileExists(siteUrl, "Pages", "PayorEnrollment.aspx"))
                {
                    CreatePayorEnrollmentPage(siteUrl, "PayorEnrollment", "Payor Enrollment", "1000px", urlSiteAssets + "/" + "SiteAssets/PayorEnrollment.html");
                }

                UpdateProgramParticipation(siteUrl);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " " + ex.StackTrace);
                return false;
            }


        }
        public void CreatePayorEnrollmentPage(string siteUrl, string strPageName, string strTitle, string strWPWidth, string strContentWPLink)
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

                        WebPartDefinition wpd1 = olimitedwebpartmanager.ImportWebPart(ContentEditorXML(strTitle, strWPWidth, strContentWPLink));
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
                        logger.Error(ex.Message + " " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " " + ex.StackTrace);
            }
        }
        public string ContentEditorXML(string webPartTitle, string webPartWidth, string webPartContentLink)
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
        private void UpdateProgramParticipation(string siteUrl)
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
                        string fileLocation = @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Images\";
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

                        ModifyWebPartProgramParticipation(siteUrl);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message + " " + ex.StackTrace);
                }
            }
        }
        public void ModifyWebPartProgramParticipation(string webUrl)
        {
            string clink = string.Empty;
            int webPartHeight = GridHeight(webUrl);

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
                        logger.Error(ex.Message + " " + ex.StackTrace);
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message + " " + ex.StackTrace);
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
        }
        public int GridHeight(string siteUrl)
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
                logger.Error(ex.Message + " " + ex.StackTrace);
            }
            return intHeight[intCount - 1];
        }

        private static void UpdateUrlRef(Practice psite, string listName)
        {
            SiteLogUtility siteLogUtility = new SiteLogUtility();
            try
            {
                using (ClientContext clientContext = new ClientContext(psite.NewSiteUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle(listName);
                    var items = list.GetItems(CamlQuery.CreateAllItemsQuery());

                    clientContext.Load(items);
                    clientContext.ExecuteQuery();


                    foreach (var item in items)
                    {
                        var fndTitle = item["Title"].ToString();
                        string thumbNail = GetProgramParticipationImg(fndTitle);
                        item["Thumbnail"] = psite.NewSiteUrl + "/Program%20Participation/" + thumbNail;
                        item.Update();
                        clientContext.ExecuteQuery();
                        logger.Information($">>> {item["Title"]} - Thumbnail = {item["Thumbnail"]}", true);
                        siteLogUtility.LoggerInfo_Entry($">>> {item["Title"]} - Thumbnail = {item["Thumbnail"]}", true);
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("UpdateSortCol", ex.Message, "Error", "");
            }
        }
        private static string GetProgramParticipationImg(string fndTitle)
        {
            string thumbNail = string.Empty;
            try
            {
                switch (fndTitle)
                {
                    case SiteListUtility.progpart_PayorEnrollment:
                        thumbNail = "PracticeReferrals.JPG";
                        break;
                    case SiteListUtility.progpart_CkccKceResources:
                        thumbNail = "KCEckcc.JPG";
                        break;
                    case SiteListUtility.progpart_PayorProgeducation:
                        thumbNail = "EducationReviewPro.JPG";
                        break;
                    case SiteListUtility.progpart_PatientStatusUpdates:
                        thumbNail = "optimalstarts.jpg";
                        break;
                    case SiteListUtility.progpart_CkccKceEngagement:
                        thumbNail = "CKCC_KCEEngagement.png";
                        break;


                    default:
                        thumbNail = "";
                        break;
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetProgramParticipationImg", ex.Message, "Error", "");
            }
            return thumbNail;
        }
    }
}

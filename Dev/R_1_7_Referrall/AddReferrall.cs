using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using SiteUtility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using Serilog;

namespace R_1_7_Referrall
{
    public class AddReferrall
    {
        string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
        string strPortalSiteURL = ConfigurationManager.AppSettings["SP_SiteUrl"];

        static string dateHrMin = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
        const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
        static ILogger _logger = Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("Logs/maint" + dateHrMin + "_.log", rollingInterval: RollingInterval.Day, shared: true, outputTemplate: outputTemp1)
           .CreateLogger();
        static ILogger logger = _logger.ForContext<AddReferrall>();
        const string LayoutsFolder = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";

        public void InitiateProg()
        {
            SiteInfoUtility siu = new SiteInfoUtility();
            SiteLogUtility slu = new SiteLogUtility();

            List<Practice> practices = siu.GetAllCKCCPractices();

            try
            {
                if (practices != null && practices.Count > 0)
                {
                    foreach (var practice in practices)
                    {
                        if (practice.IsCKCC == true)
                        {
                            ReferralSetup(practice.NewSiteUrl, LayoutsFolder, practice.SiteID);

                            slu.LoggerInfo_Entry(practice.NewSiteUrl, true);
                            slu.LoggerInfo_Entry(practice.Name + " setup is completed", true);
                            slu.LoggerInfo_Entry("=======================================", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                slu.LoggerInfo_Entry(ex.Message, true);
                SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
            }
            finally
            {
                slu.LoggerInfo_Entry("=======================================");
                slu.LoggerInfo_Entry("3. Maintenance Tasks Complete - Complete");
                slu.LoggerInfo_Entry("=============Release Ends=============");
                SiteLogUtility.CreateLogEntry("PracticeSite-Maint - SiteUtilityTest", "=============Release Ends=============", "Log", "");
            }
        }
        public void InitiateProg(string siteId)
        {
            SiteInfoUtility siu = new SiteInfoUtility();
            SiteLogUtility slu = new SiteLogUtility();
            
            Practice practice = siu.GetPracticeBySiteID(siteId);

            try
            {
                if (practice != null)
                {
                    if (practice.IsCKCC == true)
                    {
                        ReferralSetup(practice.NewSiteUrl, LayoutsFolder, practice.SiteID);

                        slu.LoggerInfo_Entry(practice.NewSiteUrl, true);
                        slu.LoggerInfo_Entry(practice.Name + " setup is completed", true);
                        slu.LoggerInfo_Entry("=======================================", true);
                    }
                }
            }
            catch (Exception ex)
            {
                slu.LoggerInfo_Entry(ex.Message, true);
                SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
            }
            finally
            {
                slu.LoggerInfo_Entry("=======================================");
                slu.LoggerInfo_Entry("3. Maintenance Tasks Complete - Complete");
                slu.LoggerInfo_Entry("=============Release Ends=============");
                SiteLogUtility.CreateLogEntry("PracticeSite-Maint - SiteUtilityTest", "=============Release Ends=============", "Log", "");
            }
        }

        public void ReferralSetup(string sitrUrl, string layoutsFolder, string siteId)
        {
            try
            {
                string strReferralURL = SiteInfoUtility.GetReferralUrl(sitrUrl);  //NO SLASH AT THE END
                //string strSiteID = getSiteID(sitrUrl);
                string strSiteID = siteId;


                SiteFilesUtility objSiteFiles = new SiteFilesUtility();
                objSiteFiles.DocumentUpload(sitrUrl, @layoutsFolder + "cePrac_CarePlans.html", "SiteAssets");
                objSiteFiles.DocumentUpload(sitrUrl, @layoutsFolder + "SW_RD_Referrals.jpg", "SiteAssets/Img");
                increaseCarePlansWPHeight(sitrUrl);

                ConfigureReferralPage(sitrUrl, "ReferralPage", "Referral Page", "900px", "900px", strReferralURL + "/SiteAssets/ReferralGrid.html");
                ConfigureReferralPage(sitrUrl, "Referrals", "Referrals", "1400px", "1100px", strReferralURL + "/SiteAssets/ReferralForm.html");

                breakPageSecurityInheritance(sitrUrl, "ReferralPage.aspx", "Pages");
                breakPageSecurityInheritance(sitrUrl, "Referrals.aspx", "Pages");
                breakPageSecurityInheritance(sitrUrl, "FHPIcon.JPG", "Site Assets");

                addSecurityGroupToList(strReferralURL, "Prac_" + strSiteID + "_User", "ReferralMembers", "Contribute");
                addSecurityGroupToList(strReferralURL, "Prac_" + strSiteID + "_User", "ReferralRequests", "Contribute");
                addSecurityGroupToList(strReferralURL, "Prac_" + strSiteID + "_User", "Site Assets", "Contribute");
                addSecurityGroupToASPXPage(sitrUrl, "CKD_Referral_Internal_User", "Pages", "Contribute", "Referrals");
                addSecurityGroupToASPXPage(sitrUrl, "CKD_Referral_Internal_User", "Pages", "Contribute", "Referral Page");
                addSecurityGroupToASPXPage(sitrUrl, "CKD_Referral_Internal_User", "Site Assets", "Contribute", "KC365_Logo_HEALTHprogram_RGB");


                addSWReferralNavigationNode(sitrUrl);


                //break inheritance for new pages Referrals.aspx and ReferralPage.aspx
                //add new security to Referrals.aspx and ReferralPage.aspx
                //break inheritance for referralMembers and referralRequest.. 
                //give prac_123123 to referralMembers and referralRequest list in referral subsite and give contribute permission
                //Add node on left navigation for Referral (give link for ReferralPage.aspx)
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("ReferralSetup", ex.Message, "Error", strPortalSiteURL);
            }
        }
        public void addSWReferralNavigationNode(string webUrl)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(webUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;
                    NavigationNodeCollection objNodeColl = clientContext.Web.Navigation.QuickLaunch;
                    clientContext.Load(web);
                    clientContext.Load(web.ParentWeb);
                    clientContext.ExecuteQuery();

                    clientContext.Load(objNodeColl);
                    clientContext.ExecuteQuery();

                    foreach (NavigationNode objNav in objNodeColl)
                    {
                        if (objNav.Title == "Care Coordination")
                        {
                            clientContext.Load(objNav.Children);
                            clientContext.ExecuteQuery();

                            NavigationNodeCreationInformation objNewNode = new NavigationNodeCreationInformation();
                            objNewNode.Title = "SW/RD Referrals";
                            objNewNode.Url = webUrl + "Pages/ReferralPage.aspx";
                            objNewNode.AsLastNode = true;

                            objNav.Children.Add(objNewNode);
                            clientContext.ExecuteQuery();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("addSWReferralNavigationNode", ex.Message, "Error", strPortalSiteURL);
            }
        }
        public void ConfigureReferralPage(string webUrl, string strPageName, string strTitle, string strWPHeight, string strWPWidth, string strContentWPLink)
        {
            try
            {
                SitePublishUtility spUtility = new SitePublishUtility();
                if (!SiteFilesUtility.FileExists(webUrl, "Pages", strPageName + ".aspx"))
                {
                    spUtility.InitializePage(webUrl, strPageName, strTitle); 
                }
                spUtility.DeleteWebPart(webUrl, strPageName);

                using (ClientContext clientContext = new ClientContext(webUrl))
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

                        WebPartDefinition wpd1 = olimitedwebpartmanager.ImportWebPart(contentEditorXML(strTitle, strWPHeight, strWPWidth, strContentWPLink));
                        wpd1.WebPart.Title = strTitle;
                        olimitedwebpartmanager.AddWebPart(wpd1.WebPart, "CenterLeftColumn", 1);

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
                SiteLogUtility.CreateLogEntry("ConfigureReferralPage", ex.Message, "Error", strPortalSiteURL);
            }
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
        public void increaseCarePlansWPHeight(string webURL)
        {
            var pageRelativeUrl = "/Pages/CareCoordination.aspx";
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

                    for (int intLoop = 0; intLoop < wpManager.WebParts.Count; intLoop++)
                    {
                        WebPartDefinition obj = wpManager.WebParts[intLoop];
                        clientContext.Load(obj.WebPart);
                        clientContext.ExecuteQuery();
                        if (obj.WebPart.Title == "Care Plans")
                        {
                            obj.WebPart.Properties["Height"] = "475px";
                            obj.SaveWebPartChanges();
                            clientContext.ExecuteQuery();
                        }
                    }

                    file.CheckIn("increaseCarePlansWPHeight webpart", CheckinType.MajorCheckIn);
                    file.Publish("increaseCarePlansWPHeight webpart");
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("increaseCarePlansWPHeight", ex.Message, "Error", strPortalSiteURL);
                }
            }
        }
        public void breakPageSecurityInheritance(string strURL, string strPageName, string strLibraryName)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(strURL))
                {
                    List targetList = clientContext.Web.Lists.GetByTitle(strLibraryName);
                    ListItem oItem = null;
                    CamlQuery oQuery = new CamlQuery();
                    oQuery.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='LinkFilename' /><Value Type='Text'>" + strPageName + "</Value></Eq></Where></Query></View>";

                    ListItemCollection oItems = targetList.GetItems(oQuery);
                    clientContext.Load(oItems);
                    clientContext.ExecuteQuery();

                    oItem = oItems.FirstOrDefault();
                    oItem.BreakRoleInheritance(true, true);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("breakPageSecurityInheritance", ex.Message, "Error", strPortalSiteURL);
            }
        }
        public void addSecurityGroupToList(string strURL, string strSecurityGroupName, string strListName, string strPermissionType)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(strURL))
                {
                    List targetList = clientContext.Web.Lists.GetByTitle(strListName);
                    clientContext.Load(targetList, target => target.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();

                    if (targetList.HasUniqueRoleAssignments)
                    {
                        // Write group name to be added in the list
                        Group group = clientContext.Web.SiteGroups.GetByName(strSecurityGroupName);
                        RoleDefinitionBindingCollection roleDefCollection = new RoleDefinitionBindingCollection(clientContext);

                        // Set the permission level of the group for this particular list
                        RoleDefinition readDef = clientContext.Web.RoleDefinitions.GetByName(strPermissionType);
                        roleDefCollection.Add(readDef);

                        Principal userGroup = group;
                        RoleAssignment roleAssign = targetList.RoleAssignments.Add(userGroup, roleDefCollection);

                        clientContext.Load(roleAssign);
                        roleAssign.Update();
                        clientContext.ExecuteQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("addSecurityGroupToList", ex.Message, "Error", strPortalSiteURL);
            }
        }
        public void addSecurityGroupToASPXPage(string strURL, string strSecurityGroupName, string strListName, string strPermissionType, string strPageName)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(strURL))
                {
                    List targetList = clientContext.Web.Lists.GetByTitle(strListName);
                    clientContext.Load(targetList);
                    clientContext.ExecuteQuery();

                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + strPageName + "</Value></Eq></Where></Query></View>";

                    ListItemCollection items = targetList.GetItems(camlQuery);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();
                    foreach (var item in items)
                    {
                        Group group = clientContext.Web.SiteGroups.GetByName(strSecurityGroupName);
                        RoleDefinitionBindingCollection roleDefCollection = new RoleDefinitionBindingCollection(clientContext);

                        // Set the permission level of the group for this particular list
                        RoleDefinition readDef = clientContext.Web.RoleDefinitions.GetByName(strPermissionType);
                        roleDefCollection.Add(readDef);

                        Principal userGroup = group;
                        RoleAssignment roleAssign = item.RoleAssignments.Add(userGroup, roleDefCollection);

                        clientContext.Load(roleAssign);
                        roleAssign.Update();
                        clientContext.ExecuteQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("addSecurityGroupToASPXPage", ex.Message, "Error", strPortalSiteURL);
            }
        }
        public string getSiteID(string strURL)
        {
            string strRealSiteID = string.Empty;
            try
            {
                string[] strResult = strURL.Split('/');
                string strSiteID = strResult[strResult.Length - 2];
                char[] charArray = strSiteID.Substring(1, strSiteID.Length - 2).ToArray();
                Array.Reverse(charArray);
                strRealSiteID = new string(charArray);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("getSiteID", ex.Message, "Error", strPortalSiteURL);
            }
            return strRealSiteID;
        }
    }
}

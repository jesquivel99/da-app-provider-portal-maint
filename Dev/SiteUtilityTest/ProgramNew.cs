using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using SiteUtility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SiteUtilityTest
{
    public class ProgramNew
    {
        public void InitiateProg()
        {
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            string sAdminListName = ConfigurationManager.AppSettings["AdminRootListName"];
            string releaseName = "SiteUtilityTest";
            SiteRootAdminList objRootSite = new SiteRootAdminList();
            SiteDeleteUtility objDeleteSite = new SiteDeleteUtility();
            SiteFilesUtility objFilesSite = new SiteFilesUtility();

            SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                Console.WriteLine("=============Release Starts=============");
                SiteLogUtility.CreateLogEntry("PracticeSite-Maint - SiteUtilityTest", "=============Release Starts=============", "Log", siteUrl);

                try
                {
                    //objRootSite.removeAdminRootSiteSetup();
                    //objRootSite.setupAdminRootSiteList();

                    Console.WriteLine("Admin List, Page setup completed successfully");
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - SiteUtilityTest", "Admin List, Page setup completed successfully", "Log", siteUrl);

                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext);

                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                       // if (pm.ProgramManager == "09")
                        //{
                            foreach (PracticeSite psite in pm.PracticeSiteCollection)
                            {
                          //      objFilesSite.DocumentUpload(psite.URL, @"C:\Projects\Applications\PracticeSite-Core\Prod\PracticeSiteTemplate\Config\cePrac_CarePlansDataTable.html", "Site Assets");
                          //      Console.WriteLine(psite.URL+ " carePlans fix deployed successfully");
                        }
                        //}
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", siteUrl);
                }

                Console.WriteLine("3. Maintenance Tasks Complete - Complete");
                Console.WriteLine("=============Release Ends=============");
                SiteLogUtility.CreateLogEntry("PracticeSite-Maint - SiteUtilityTest", "=============Release Ends=============", "Log", siteUrl);
            }
        }

        public void ReferralSetup()
        {
            string sitrUrl = "https://sharepointdev.fmc-na-icg.com/bi/fhppp/portal/PM02/99856032229";
            string strReferralURL = "https://sharepointdev.fmc-na-icg.com/bi/fhppp/interimckcc/referral";
            SiteFilesUtility objSiteFiles = new SiteFilesUtility();
            objSiteFiles.DocumentUpload(sitrUrl, @"C:\Users\ssaleh\Documents\VisualStudio\cePrac_CarePlans.html", "SiteAssets");
            objSiteFiles.DocumentUpload(sitrUrl, @"C:\Users\ssaleh\Documents\VisualStudio\SW_RD_Referrals.jpg", "SiteAssets/Img");
            increaseCarePlansWPHeight(sitrUrl);

            ConfigureReferralPage(sitrUrl, "ReferralPage", "Referral Page", "900px", "900px", strReferralURL + "/SiteAssets/cePrac_SWReferralPage.html");
            ConfigureReferralPage(sitrUrl, "Referrals", "Referrals", "1400px", "1100px", strReferralURL + "/SiteAssets/formExs2.html");

            //break inheritance for new pages Referrals.aspx and ReferralPage.aspx
            //add new security to Referrals.aspx and ReferralPage.aspx
            //break inheritance for referralMembers and referralRequest
            //give prac_123123 to referralMembers and referralRequest list in referral subsite and give contribute permission
        }

        public void ConfigureReferralPage(string webUrl, string strPageName, string strTitle, string strWPHeight, string strWPWidth, string strContentWPLink)
        {
            SitePublishUtility spUtility = new SitePublishUtility();
            spUtility.InitializePage(webUrl, strPageName, strTitle);
            spUtility.DeleteWebPart(webUrl, strPageName);

            using (ClientContext clientContext = new ClientContext(webUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                Web web = clientContext.Web;
                clientContext.Load(web);
                clientContext.Load(web.ParentWeb);
                clientContext.ExecuteQuery();

                var file = web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + "/Pages/"+ strPageName + ".aspx");
                file.CheckOut();
                clientContext.Load(file);
                clientContext.ExecuteQuery();
                try
                {
                    LimitedWebPartManager olimitedwebpartmanager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);

                    WebPartDefinition wpd1 = olimitedwebpartmanager.ImportWebPart(contentEditorXML(strTitle, strWPHeight, strWPWidth, strContentWPLink));
                    wpd1.WebPart.Title = strTitle;
                    olimitedwebpartmanager.AddWebPart(wpd1.WebPart, "CenterLeftColumn", 1);

                    file.CheckIn("CheckIn - Adding Webparts to "+ strTitle, CheckinType.MajorCheckIn);
                    file.Publish("Publish - Adding Webparts to "+ strTitle);
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
                    //file.CheckIn("Delete webpart", CheckinType.MajorCheckIn);
                    //file.Publish("Delete webpart");
                    //clientContext.Load(file);
                    //clientContext.ExecuteQuery();

                    file.CheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();

                    var wpManager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);
                    var webParts = wpManager.WebParts;
                    clientContext.Load(webParts);
                    clientContext.ExecuteQuery();

                    for(int intLoop = 0; intLoop < wpManager.WebParts.Count; intLoop++)
                    {
                        WebPartDefinition obj = wpManager.WebParts[intLoop];
                        clientContext.Load(obj.WebPart);
                        clientContext.ExecuteQuery();
                        if(obj.WebPart.Title == "Care Plans")
                        {
                            obj.WebPart.Properties["Height"] = "475px";
                            obj.SaveWebPartChanges();
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

        //public void breakSecurityInheritanceAddUser()
        //{
        //    string sitrUrl = "https://sharepointdev.fmc-na-icg.com/bi/fhppp/portal/PM01/93031131279";
        //    var clientContext = new SP.ClientContext(siteUrl);
        //    var oList = clientContext.get_web().get_lists().getByTitle('MyList');

        //    var itemId = 4;
        //    //this.oListItem = oList.get_items().getById(itemId);
        //    this.oListItem = oList.getItemById(itemId);

        //    oListItem.breakRoleInheritance(false, false);

        //    this.oUser = clientContext.get_web().get_currentUser();

        //    var collRoleDefinitionBinding = SP.RoleDefinitionBindingCollection.newObject(clientContext);

        //    collRoleDefinitionBinding.add(clientContext.get_web().get_roleDefinitions().getByType(SP.RoleType.contributor));

        //    oListItem.get_roleAssignments().add(oUser, collRoleDefinitionBinding);
        //    clientContext.load(oUser);
        //    clientContext.load(oListItem);

        //    clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));
        //}
    }
}

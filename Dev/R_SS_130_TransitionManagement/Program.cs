using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using SiteUtility;
using System.Configuration;

namespace R_SS_130_TransitionManagement
{
    public class Program
    {
        static Guid _listGuid = Guid.Empty;
        static string dateHrMin = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
        const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
       
        const string LayoutsFolder = @"C:\Users\ssaleh\Downloads\";
        string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
        string strPortalSiteURL = ConfigurationManager.AppSettings["SP_SiteUrl"];
        public static void Main(string[] args)
        {
            SiteInfoUtility siu = new SiteInfoUtility();
            SiteLogUtility slu = new SiteLogUtility();
            List<Practice> practices = siu.GetAllPractices();
            try
            {
                slu.LoggerInfo_Entry("========================================Release Starts========================================", true);
                int intLoop = 0;
                if (practices != null && practices.Count > 0)
                {
                    foreach (Practice practice in practices)
                    {
                        if (intLoop > 16)
                        {
                            //uploadHospAlertRelatedHTMLfile(practice.NewSiteUrl);
                            Program objP = new Program();
                            objP.TransitionSetup(practice.NewSiteUrl, LayoutsFolder, practice.IsCKCC);
                            Console.WriteLine(intLoop + ". " + practice.Name + "  ..  Hosp Alert file uploaded.");
                            Console.WriteLine("=======================================");
                        }
                        intLoop++;
                    }
                }
            }
            catch (Exception ex)
            {
                slu.LoggerInfo_Entry("Error: " + ex.Message, true);
            }
            finally
            {
                slu.LoggerInfo_Entry(SiteLogUtility.textLine0);
                slu.LoggerInfo_Entry("========================================Release Ends========================================", true);
                SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@freseniusmedicalcare.com");
            }

            //Log.CloseAndFlush();
        }


        public void TransitionSetup(string sitrUrl, string layoutsFolder, bool strIsCKCC)
        {
            try
            {
                String strWebPartName = strIsCKCC ? "Hospital Alerts" : "Care Plans";
                SiteFilesUtility objSiteFiles = new SiteFilesUtility();
                objSiteFiles.DocumentUpload(sitrUrl, @layoutsFolder + "cePrac_CarePlansDataTable.html", "SiteAssets");
                if (strIsCKCC)
                {
                    objSiteFiles.DocumentUpload(sitrUrl, @layoutsFolder + "cePrac_HospitalAlerts.html", "SiteAssets");
                }
                else
                {
                    objSiteFiles.DocumentUpload(sitrUrl, @layoutsFolder + "cePrac_CarePlans.html", "SiteAssets");
                }
                objSiteFiles.DocumentUpload(sitrUrl, @layoutsFolder + "cePrac_TransitionDataTable.html", "SiteAssets");
                objSiteFiles.DocumentUpload(sitrUrl, @layoutsFolder + "TransitionManagement.jpg", "SiteAssets/Img");
                increaseHospitalizationAlertWPHeight(sitrUrl, strWebPartName);
                configureTransitionPage(sitrUrl, "TransitionPlan", "Transition Management", "666px", "1200px", sitrUrl + "SiteAssets/cePrac_TransitionDataTable.html");
                addTransitionPageNavigationNode(sitrUrl);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("TransitionSetup", ex.Message, "Error", strPortalSiteURL);
            }
        }

        public void configureTransitionPage(string webUrl, string strPageName, string strTitle, string strWPHeight, string strWPWidth, string strContentWPLink)
        {
            try
            {
                SitePublishUtility spUtility = new SitePublishUtility();
                spUtility.InitializePage(webUrl, strPageName, strTitle);
                //spUtility.DeleteWebPart(webUrl, strPageName);

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
                SiteLogUtility.CreateLogEntry("ConfigureTransitionPage", ex.Message, "Error", strPortalSiteURL);
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

        public void increaseHospitalizationAlertWPHeight(string webURL, string strWebPartName)
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
                        if (obj.WebPart.Title == strWebPartName)
                        {
                            obj.WebPart.Properties["Height"] = "475px";
                            obj.SaveWebPartChanges();
                            clientContext.ExecuteQuery();
                        }
                    }

                    file.CheckIn("increaseHospitalizationAlertWPHeight webpart", CheckinType.MajorCheckIn);
                    file.Publish("increaseHospitalizationAlertWPHeight webpart");
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("increaseHospitalizationAlertWPHeight", ex.Message, "Error", strPortalSiteURL);
                }
            }
        }

        public void addTransitionPageNavigationNode(string webUrl)
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
                            objNewNode.Title = "Transition Management";
                            objNewNode.Url = webUrl + "Pages/TransitionPlan.aspx";
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
                SiteLogUtility.CreateLogEntry("addTransitionPageNavigationNode", ex.Message, "Error", strPortalSiteURL);
            }
        }



    }
}

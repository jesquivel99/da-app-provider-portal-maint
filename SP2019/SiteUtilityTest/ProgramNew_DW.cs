using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using Serilog;
using SiteUtility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

namespace SiteUtilityTest
{
    class ProgramNew_DW
    {
        ILogger _logger = Log.ForContext<ProgramNew_DW>();

        string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
        string strPortalSiteURL = ConfigurationManager.AppSettings["SP_SiteUrl"];

        public ProgramNew_DW()
        {

        }

        public void InitiateProg()
        {
            _logger.Information("InitiateProg() started...");

            using (ClientContext clientContext = new ClientContext(strPortalSiteURL))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    List<string> MedicalDirectorTINs = Get_TINs_Of_Medical_Director_Practices();

                    //List<string> TelephonicTIN = Get_TINs_Of_Telephonic_Practices();
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext);
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        if (pm.ProgramManager == "10")
                        {
                            foreach (PracticeSite psite in pm.PracticeSiteCollection)
                            {
                                if (psite.PracticeTIN == "650255930")
                                //if (MedicalDirectorTINs.Any(psite.PracticeTIN.Contains))
                                {
                                    Console.WriteLine(psite.PracticeTIN);
                                    if (Medical_Director_Setup(psite.URL + "/"))
                                    //if (CkccEngagement_Setup(psite.URL + "/"))
                                    {
                                        _logger.Information(psite.Name + " setup is completed");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message + " " + ex.StackTrace);
                }

                _logger.Information("Maintenance Tasks Completed Successfully!");
            }
        }
        public bool Medical_Director_Setup(string siteUrl)
        {
            //string urlSiteAssets = @"https://sharepoint.fmc-na-icg.com/bi/fhppp/portal/referral";
            string urlSiteAssets = @"https://sharepointdev.fmc-na-icg.com/bi/fhppp/interimckcc/referral";
            try
            {
                SiteFilesUtility objSiteFiles = new SiteFilesUtility();

                if (!SiteFilesUtility.FileExists(siteUrl, "Pages", "MedicalDirectorTable.aspx"))
                {
                    createAspxPage(siteUrl, "MedicalDirectorTable", "Medical Director Timesheets", "1000px", urlSiteAssets + "/SiteAssets/MedicalDirectorTable.html");
                }

                if (!SiteFilesUtility.FileExists(siteUrl, "Pages", "MedicalDirectorForm.aspx"))
                {
                    createAspxPage(siteUrl, "MedicalDirectorForm", "Medical Director Quarterly Time Sheet", "", urlSiteAssets + "/SiteAssets/MedicalDirectorForm.html");
                }
                addMedicalDirectorNavigationNode(siteUrl);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + " " + ex.StackTrace);
                return false;
            }
        }
        public void addMedicalDirectorNavigationNode(string webUrl)
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

                    NavigationNode newNode = objNodeColl.Where(Node => Node.Title == "Medical Director Timesheet").FirstOrDefault();
                    if (newNode == null) // Add only if Medical Director Timesheet node does not exist
                    {
                        NavigationNode prevNode = objNodeColl.Where(Node => Node.Title == "Quality").FirstOrDefault();

                        NavigationNodeCreationInformation objNewNode = new NavigationNodeCreationInformation();
                        objNewNode.Title = "Medical Director Timesheet";
                        objNewNode.Url = webUrl + "Pages/MedicalDirectorTable.aspx";
                        objNewNode.PreviousNode = prevNode; // Add Medical Director Timesheet node right after Quality

                        objNodeColl.Add(objNewNode);
                        clientContext.ExecuteQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("addSWReferralNavigationNode", ex.Message, "Error", strPortalSiteURL);
            }
        }

        public bool CkccEngagement_Setup(string siteUrl)
        {
            string urlSiteAssets = @"https://sharepoint.fmc-na-icg.com/bi/fhppp/portal/referral";
            //string urlSiteAssets = @"https://sharepointdev.fmc-na-icg.com/bi/fhppp/interimckcc/referral";
            try
            {
                SiteFilesUtility objSiteFiles = new SiteFilesUtility();

                if (!SiteFilesUtility.FileExists(siteUrl, "Pages", "CkccEngagement.aspx"))
                {
                    createAspxPage(siteUrl, "CkccEngagement", "CKCC Engagement", "1000px", urlSiteAssets + "/SiteAssets/CkccEngagement.html");
                }

                updateProgramParticipation(siteUrl);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + " " + ex.StackTrace);
                return false;
            }
        }

        public void createAspxPage(string siteUrl, string strPageName, string strTitle, string strWPWidth, string strContentWPLink)
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
                        _logger.Error(ex.Message + " " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + " " + ex.StackTrace);
            }
        }

        public string contentEditorXML(string webPartTitle, string webPartWidth, string webPartContentLink)
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

        private void updateProgramParticipation(string siteUrl)
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
                    query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>CKCC Engagement</Value></Eq></Where></Query></View>";

                    ListItemCollection items = list.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    if (items.Count > 0)
                    {
                        ListItem payorItem = items.FirstOrDefault();
                        payorItem["ProgramNameText"] = siteUrl + "Pages/CkccEngagement.aspx";
                        payorItem.Update();
                        clientContext.ExecuteQuery();
                    }
                    else
                    {
                        string fileLocation = @"C:\temp\";
                        string fileName = "CKCC_KCEEngagement.png";

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
                        newItem["Title"] = "CKCC/KCE Engagement";

                        newItem["ProgramNameText"] = siteUrl + "Pages/CkccEngagement.aspx";
                        newItem["Thumbnail"] = siteUrl + "Program%20Participation/" + fileName;
                        newItem.Update();
                        newItem.File.CheckIn("Checkin - Create CKCC Engagement Item", CheckinType.OverwriteCheckIn);
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

        public void modifyWebPartProgramParticipation(string webUrl)
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


        public int gridHeight(string siteUrl)
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

        public static List<string> Get_TINs_Of_Telephonic_Practices()
        {
            List<string> TINs = new List<string>();
            TINs.Add("222306589");
            TINs.Add("232912285");
            TINs.Add("454714570");
            TINs.Add("460827168");
            TINs.Add("461993309");
            TINs.Add("462049200");
            TINs.Add("462883010");
            TINs.Add("465597091");
            TINs.Add("475264429");
            TINs.Add("521323183");
            TINs.Add("521965533");
            TINs.Add("541897047");
            TINs.Add("582365505");
            TINs.Add("591427538");
            TINs.Add("721311303");
            TINs.Add("721491011");
            TINs.Add("730937601");
            TINs.Add("812518167");
            TINs.Add("813442551");
            TINs.Add("814322706");
            TINs.Add("822391895");
            TINs.Add("832278740");
            TINs.Add("900854896");
            return TINs;
        }
        public static List<string> Get_TINs_Of_Medical_Director_Practices()
        {
            List<string> TINs = new List<string>();
            TINs.Add("631220194");
            TINs.Add("812808787");
            TINs.Add("561378901");
            TINs.Add("751366650");
            TINs.Add("521323183");
            TINs.Add("521133614");
            TINs.Add("561634662");
            TINs.Add("311477544");
            TINs.Add("860959487");
            TINs.Add("431739852");
            TINs.Add("464495456");
            TINs.Add("611141697");
            TINs.Add("351679014");
            TINs.Add("860990148");
            TINs.Add("462812104");
            TINs.Add("640600391");
            TINs.Add("721311303");
            TINs.Add("264239523");
            TINs.Add("222312675");
            TINs.Add("930751490");
            TINs.Add("221910022");
            TINs.Add("824435212");
            TINs.Add("731498617");
            TINs.Add("272244677");
            TINs.Add("650255930");
            return TINs;
        }
    }
}


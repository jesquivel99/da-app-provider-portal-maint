﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SiteUtility;
using System.Configuration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using Microsoft.SharePoint.Client.WebParts;
using System.Net;
using System.IO;
using System.Xml;
using System.Reflection;

namespace SiteUtilityTest
{
    public class ProgramNew_JE
    {
        string LayoutsFolderMnt = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
        static public List<Practice> practicesIWH = new List<Practice>();
        static public List<Practice> practicesCKCC = new List<Practice>();
        static Guid _listGuid = Guid.Empty;
        public void InitiateProg()
        {
            string releaseName = "Benefit-Quality-Payor";
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            string srcUrlIWH = ConfigurationManager.AppSettings["SP_IWHUrl"];
            string srcUrlCKCC = ConfigurationManager.AppSettings["SP_CKCCUrl"];
            string siteInfoFile = ConfigurationManager.AppSettings["Csv_File"];

            string urlAdminGroup = @"https://sharepointdev.fmc-na-icg.com/bi/fhppp/portal/PM08";

            SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            SiteLogUtility.Log_Entry("\n\n=============Release Starts=============", true);

            List<PMData> pmData = initPMDataToList(urlAdminGroup);

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    SiteLogUtility.Log_Entry("\n\n=============[ Get all Portal Practice Data ]=============", true);
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext, practicesIWH, practicesCKCC, pmData);

                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - Start]=============", true);
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        foreach (PracticeSite psite in pm.PracticeSiteCollection)
                        {
                            if (psite.URL.Contains("90566631579"))
                            {
                                SiteLogUtility.LogPracDetail(psite);

                                SiteFilesUtility sfu = new SiteFilesUtility();
                                sfu.uploadProgramPracticeSupportFilesWoDialysisStarts(psite);
                                modifyWebPartProgramParticipation(psite.URL, psite);

                                uploadMultiPartSupportingFiles(psite.URL, psite);

                                Init_Benefit(psite);

                                Init_Quality(psite);

                                Init_Payor(psite);

                                SiteNavigateUtility.ClearQuickNavigationRecent(psite.URL);
                            }
                        }
                    }
                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - End]=============", true);
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

        private void Init_Payor(PracticeSite practiceSite)
        {
            SiteLogUtility.Log_Entry("Init_Payor - In Progress...");
            bool ConfigSuccess = false;
            PublishingPage PPage = null;

            SiteFilesUtility sfUtility = new SiteFilesUtility();
            SitePublishUtility spUtility = new SitePublishUtility();
            SiteListUtility slUtility = new SiteListUtility();
            PracticeCView practiceCView = new PracticeCView();

            try
            {
                ProvisionList(practiceSite, slUtility, slUtility.listNamePayorEducationIwh, practiceCView);
                CreateFolder(practiceSite, slUtility.listNamePayorEducationIwh, slUtility.listFolder1PayorEducationIwh);
                //ProvisionList(practiceSite, slUtility, slUtility.listNamePayorEducationCkcc, practiceCView);
                //CreateFolder(practiceSite, slUtility.listNamePayorEducationCkcc, slUtility.listFolder1PayorEducationCkcc);

                spUtility.InitializePage(practiceSite.URL, slUtility.pageNamePayorEducation, slUtility.pageTitlePayorEducation);
                spUtility.DeleteWebPart(practiceSite.URL, slUtility.pageNamePayorEducation);
                sfUtility.DocumentUpload(practiceSite.URL, LayoutsFolderMnt + "PayorEducation_MultiTab.js", "SiteAssets");
                sfUtility.DocumentUpload(practiceSite.URL, LayoutsFolderMnt + "jquery-ui.theme.css", "SiteAssets");
                ConfigSuccess = ConfigurePayorEducationPage(practiceSite.URL, practiceSite);
                if (ConfigSuccess)
                {
                    if(practiceSite.IsIWH == "true")
                    {
                        modifyView(practiceSite.URL, slUtility.pageNamePayorEducation + ".aspx", slUtility.webpartPayorEducationIwh);
                    }
                    //if (practiceSite.IsCKCC == "true")
                    //{
                    //    modifyView(practiceSite.URL, slUtility.pageNamePayorEducation + ".aspx", slUtility.webpartPayorEducationCkcc);
                    //}
                }
                SP_Update_ProgramParticipation(practiceSite.URL, slUtility.pageNamePayorEducation, "Payor Program Education Resources Coming Soon", "Payor Program Education Resources", "EducationReviewPro.JPG");
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_Payor", ex.Message, "Error", practiceSite.URL);
            }
        }

        private void Init_Quality(PracticeSite practiceSite)
        {
            SiteLogUtility.Log_Entry("Init_Quality - In Progress...");
            bool ConfigSuccess = false;
            PublishingPage PPage = null; 
            
            SiteFilesUtility sfu = new SiteFilesUtility();
            SitePublishUtility spUtility = new SitePublishUtility();
            SiteListUtility slUtility = new SiteListUtility();
            PracticeCView practiceCView = new PracticeCView();

            try
            {
                ProvisionList(practiceSite, slUtility, slUtility.listNameQualityIwh, practiceCView);
                CreateFolder(practiceSite, slUtility.listNameQualityIwh, slUtility.listFolder1QualityIwh);
                //CreateFolder(practiceSite, slUtility.listNameQualityIwh, slUtility.listFolder2QualityIwh);
                //CreateFolder(practiceSite, slUtility.listNameQualityIwh, slUtility.listFolder3QualityIwh);

                ProvisionList(practiceSite, slUtility, slUtility.listNameQualityCkcc, practiceCView);
                CreateFolder(practiceSite, slUtility.listNameQualityCkcc, slUtility.listFolder1QualityCkcc);
                //CreateFolder(practiceSite, slUtility.listNameQualityCkcc, slUtility.listFolder2QualityCkcc);
                //CreateFolder(practiceSite, slUtility.listNameQualityCkcc, slUtility.listFolder3QualityCkcc);

                spUtility.DeleteWebPart(practiceSite.URL, slUtility.pageNameQuality);
                sfu.DocumentUpload(practiceSite.URL, LayoutsFolderMnt + "Quality_MultiTab.js", "SiteAssets");
                sfu.DocumentUpload(practiceSite.URL, LayoutsFolderMnt + "jquery-ui.theme.css", "SiteAssets");
                sfu.uploadImageSupportingFilesSingleImage(practiceSite.URL, "Quality.jpg");
                sfu.uploadHtmlSupportingFilesSingleFile(practiceSite.URL, "cePrac_Quality.html");
                ConfigSuccess = ConfigureQualityPage(practiceSite.URL, practiceSite);
                if (ConfigSuccess)
                {
                    if(practiceSite.IsIWH == "true")
                    {
                        modifyView(practiceSite.URL, slUtility.pageNameQuality + ".aspx", slUtility.webpartQualityIwh);
                    }
                    if (practiceSite.IsCKCC == "true")
                    {
                        modifyView(practiceSite.URL, slUtility.pageNameQuality + ".aspx", slUtility.webpartQualityCkcc);
                    }
                }

                // ONLY UNCOMMENT IF PERFORMING ROLLBACK ON QUALITY PAGE...
                //ConfigureQualityRollbackPage(practiceSite.URL, practiceSite);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_Quality", ex.Message, "Error", practiceSite.URL);
            }
        }

        private void Init_Benefit(PracticeSite practiceSite)
        {
            SiteLogUtility.Log_Entry("Init_Benefit - In Progress...");
            bool ConfigSuccess = false;
            PublishingPage PPage = null;

            SiteFilesUtility sfUtility = new SiteFilesUtility();
            SitePublishUtility spUtility = new SitePublishUtility();
            SiteListUtility slUtility = new SiteListUtility();
            PracticeCView practiceCView = new PracticeCView();

            try
            {
                ProvisionList(practiceSite, slUtility, slUtility.listNameBenefitEnhancementCkcc, practiceCView);
                CreateFolder(practiceSite, slUtility.listNameBenefitEnhancementCkcc, slUtility.listFolder1BenefitEnhancementCkcc);
                spUtility.InitializePage(practiceSite.URL, slUtility.pageNameBenefitEnhancement, "Benefit Enhancement");
                spUtility.DeleteWebPart(practiceSite.URL, slUtility.pageNameBenefitEnhancement);
                sfUtility.DocumentUpload(practiceSite.URL, LayoutsFolderMnt + "BenefitEnhancement_MultiTab.js", "SiteAssets");
                sfUtility.DocumentUpload(practiceSite.URL, LayoutsFolderMnt + "jquery-ui.theme.css", "SiteAssets");
                ConfigSuccess = ConfigureBenefitEnhancementPage(practiceSite.URL, practiceSite);
                if (ConfigSuccess)
                {
                    if(practiceSite.IsCKCC == "true")
                    {
                        modifyView(practiceSite.URL, slUtility.pageNameBenefitEnhancement + ".aspx", slUtility.webpartBenefitEnhancementCkcc);
                    }
                }
                SP_Update_ProgramParticipation(practiceSite.URL, slUtility.pageNameBenefitEnhancement, "CKCC/KCE Coming Soon", "CKCC/KCE", "KCEckcc.JPG");
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_Benefit", ex.Message, "Error", practiceSite.URL);
            }
        }

        public void modifyView(string webUrl, string strPageName = "Home.aspx", string strWebPartTitle = "Practice Documents")
        {
            SiteLogUtility.Log_Entry("modifyView - In Progress...");
            SiteListUtility slu = new SiteListUtility();
            using (ClientContext clientContext = new ClientContext(webUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                {
                    Web w = clientContext.Web;
                    bool blnWebPartExists = false;
                    List list = w.Lists.GetByTitle("Documents");
                    if (strWebPartTitle == slu.webpartBenefitEnhancementCkcc)
                    {
                        list = w.Lists.GetByTitle(slu.listNameBenefitEnhancementCkcc);
                    }
                    else if (strWebPartTitle == slu.webpartPayorEducationIwh)
                    {
                        list = w.Lists.GetByTitle(slu.listNamePayorEducationIwh);
                    }
                    //else if (strWebPartTitle == slu.webpartPayorEducationCkcc)
                    //{
                    //    list = w.Lists.GetByTitle(slu.listNamePayorEducationCkcc);
                    //}
                    else if (strWebPartTitle == slu.webpartQualityIwh)   // Quality
                    {
                        list = w.Lists.GetByTitle(slu.listNameQualityIwh);
                    }
                    else if (strWebPartTitle == slu.webpartQualityCkcc)   // Quality
                    {
                        list = w.Lists.GetByTitle(slu.listNameQualityCkcc);
                    }

                    clientContext.Load(w);
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();

                    var file = w.GetFileByServerRelativeUrl(w.ServerRelativeUrl + "/Pages/" + strPageName);
                    file.CheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();

                    try
                    {
                        var wpManager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);
                        var webparts = wpManager.WebParts;
                        clientContext.Load(webparts);
                        clientContext.ExecuteQuery();

                        string[] viewFields = { "Type", "LinkFilename", "Modified" };

                        if (webparts.Count > 0)
                        {
                            foreach (var webpart in webparts)
                            {
                                clientContext.Load(webpart.WebPart.Properties);
                                clientContext.ExecuteQuery();
                                var propValues = webpart.WebPart.Properties.FieldValues;
                                if (propValues["Title"].Equals(strWebPartTitle))
                                {
                                    blnWebPartExists = true;
                                    var listView = list.Views.GetById(webpart.Id);
                                    clientContext.Load(listView);
                                    clientContext.ExecuteQuery();

                                    listView.ViewFields.RemoveAll();
                                    foreach (var viewField in viewFields)
                                    {
                                        listView.ViewFields.Add(viewField);
                                    }

                                    listView.ViewQuery = "<OrderBy><FieldRef Name='ID' /></OrderBy><Where><IsNotNull><FieldRef Name='ID' /></IsNotNull></Where>";
                                    listView.Update();
                                    clientContext.ExecuteQuery();
                                    file.CheckIn("Removed Extra view in Document library", CheckinType.MajorCheckIn);
                                    file.Publish("Removed Extra view in Document library");
                                    clientContext.Load(file);
                                    w.Update();
                                    clientContext.ExecuteQuery();
                                    break;
                                }
                            }
                        }
                        if (!blnWebPartExists)
                        {
                            file.CheckIn("Removed Extra view in Document library", CheckinType.MajorCheckIn);
                            file.Publish("Removed Extra view in Document library");
                            clientContext.Load(file);
                            w.Update();
                            clientContext.ExecuteQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("Quality - modifyView", ex.Message, "Error", webUrl);
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
            }
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

        public static void ProvisionList(PracticeSite psite, SiteListUtility siUtility, string listName, PracticeCView pracCView)
        {
            SiteLogUtility.Log_Entry("ProvisionList - In Progress...");
            if (!DoesListExist(psite.URL, listName))
            {
                _listGuid = siUtility.CreateDocumentLibrary(listName, psite.URL, psite);
            }
            if (_listGuid != Guid.Empty)
            {
                //Check to see if Content Type Management needs to be Enabled
                //CheckContentTypeManagement(wUrl);

                //Run Content Type setup            
                //ContentTypes.Init(wUrl, _listGuid);

                //Run Folder setup
                //Folders.Init(wUrl, _listGuid);

                //Setup Views
                PracticeCViews practiceCViews = new PracticeCViews();
                pracCView.ViewName = "PageViewer";
                pracCView.RefreshView = true;

                PracticeCViewField practiceCViewField0 = new PracticeCViewField();
                practiceCViewField0.FieldName = "DocIcon";
                PracticeCViewField practiceCViewField1 = new PracticeCViewField();
                practiceCViewField1.FieldName = "LinkFilename";
                PracticeCViewField practiceCViewField2 = new PracticeCViewField();
                practiceCViewField2.FieldName = "Modified";
                
                PracticeCViewFields practiceCViewFields = new PracticeCViewFields();
                practiceCViewFields.Fields = new PracticeCViewField[] { practiceCViewField0, practiceCViewField1, practiceCViewField2 };
                pracCView.ViewFields = practiceCViewFields;

                practiceCViews.View = new PracticeCView[] { pracCView };

                ViewsInit(psite.URL, _listGuid, practiceCViews);
                
                //Setup Subfolders
                //SubFolders.Init(wUrl, _listGuid);
            }
        }
        private static bool DoesListExist(string wUrl, string listName)
        {
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                {
                    ListCollection lists = clientContext.Web.Lists;
                    clientContext.Load(lists);
                    clientContext.ExecuteQuery();

                    bool bListFound = false;
                    if (lists != null && lists.Count > 0)
                    {
                        foreach (List list in lists)
                        {
                            if (list.Title == listName)
                            {
                                _listGuid = list.Id;
                                bListFound = true;
                                break;
                            }
                        }
                    }

                    return bListFound;
                }
            }
        }
        public static bool ConfigureBenefitEnhancementPage(string webUrl, PracticeSite pracSite)
        {
            SiteLogUtility.Log_Entry("ConfigureBenefitEnhancement - In Progress...");
            SiteListUtility slu = new SiteListUtility();
            bool outcome = false;
            string clink = string.Empty;
            string scntPx = "666";

            using (ClientContext clientContext = new ClientContext(webUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                Web web = clientContext.Web;
                clientContext.Load(web, w => w.ServerRelativeUrl);
                clientContext.ExecuteQuery();
                var file = clientContext.Web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + "/Pages/" + slu.pageNameBenefitEnhancement + ".aspx");
                file.CheckOut();
                try
                {
                    clientContext.Load(file);
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    try
                    {
                        LimitedWebPartManager olimitedwebpartmanager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);

                        WebPartDefinition wpd1 = olimitedwebpartmanager.ImportWebPart(contentEditorXML("Multi Tab", "600px", "700px", web.Url + "/SiteAssets/BenefitEnhancement_MultiTab.js"));
                        wpd1.WebPart.Title = "Multi Tab";
                        olimitedwebpartmanager.AddWebPart(wpd1.WebPart, "CenterLeftColumn", 1);

                        //if (pracSite.IsIWH == "true")
                        //{
                        //    WebPartDefinition wpd5 = olimitedwebpartmanager.ImportWebPart(webPartXML(web.Url + "/Documentsiwh/Forms/PageViewer.aspx"));
                        //    wpd5.WebPart.Title = "Practice Documents IWH";
                        //    olimitedwebpartmanager.AddWebPart(wpd5.WebPart, "CenterColumn", 1);
                        //}
                        if (pracSite.IsCKCC == "true")
                        {
                            WebPartDefinition wpd6 = olimitedwebpartmanager.ImportWebPart(webPartXML(web.Url + "/" + slu.listNameBenefitEnhancementCkcc + "/Forms/PageViewer.aspx"));
                            wpd6.WebPart.Title = slu.webpartBenefitEnhancementCkcc;
                            olimitedwebpartmanager.AddWebPart(wpd6.WebPart, "CenterColumn", 1);
                        }

                        file.CheckIn("Adding ConfigureHomePage webparts", CheckinType.MajorCheckIn);
                        file.Publish("Adding ConfigureHomePage webparts");
                        clientContext.Load(file);
                        web.Update();
                        clientContext.ExecuteQuery();
                        outcome = true;
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("ConfigureBenefitEnhancementPage Update", ex.Message, "Error", webUrl);
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("ConfigureBenefitEnhancementPage", ex.Message, "Error", webUrl);
                    outcome = false;
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
            return outcome;
        }

        public static bool ConfigureQualityPage(string webUrl, PracticeSite pracSite)
        {
            SiteLogUtility.Log_Entry("ConfigureQualityPage - In Progress...");
            SiteListUtility slu = new SiteListUtility();
            bool outcome = false;
            string clink = string.Empty;
            string scntPx = "666";

            using (ClientContext clientContext = new ClientContext(webUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                Web web = clientContext.Web;
                clientContext.Load(web, w => w.ServerRelativeUrl);
                clientContext.ExecuteQuery();
                var file = clientContext.Web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + "/Pages/" + slu.pageNameQuality + ".aspx");
                file.CheckOut();
                try
                {
                    clientContext.Load(file);
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    try
                    {
                        LimitedWebPartManager olimitedwebpartmanager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);

                        WebPartDefinition wpd1 = olimitedwebpartmanager.ImportWebPart(contentEditorXML("Multi Tab", "600px", "700px", web.Url + "/SiteAssets/Quality_MultiTab.js"));
                        wpd1.WebPart.Title = "Multi Tab";
                        olimitedwebpartmanager.AddWebPart(wpd1.WebPart, "CenterLeftColumn", 1);

                        if (pracSite.IsIWH == "true")
                        {
                            WebPartDefinition wpd5 = olimitedwebpartmanager.ImportWebPart(webPartXML(web.Url + "/" + slu.listNameQualityIwh + "/Forms/PageViewer.aspx"));
                            wpd5.WebPart.Title = slu.webpartQualityIwh;
                            olimitedwebpartmanager.AddWebPart(wpd5.WebPart, "CenterColumn", 1);
                        }
                        if (pracSite.IsCKCC == "true")
                        {
                            WebPartDefinition wpd6 = olimitedwebpartmanager.ImportWebPart(webPartXML(web.Url + "/" + slu.listNameQualityCkcc + "/Forms/PageViewer.aspx"));
                            wpd6.WebPart.Title = slu.webpartQualityCkcc;
                            olimitedwebpartmanager.AddWebPart(wpd6.WebPart, "CenterColumn", 1);
                        }

                        file.CheckIn("Adding ConfigureHomePage webparts", CheckinType.MajorCheckIn);
                        file.Publish("Adding ConfigureHomePage webparts");
                        clientContext.Load(file);
                        web.Update();
                        clientContext.ExecuteQuery();
                        outcome = true;
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("ConfigureQualityPage Update", ex.Message, "Error", webUrl);
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("ConfigureQualityPage", ex.Message, "Error", webUrl);
                    outcome = false;
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
            return outcome;
        }
        public static bool ConfigureQualityRollbackPage(string webUrl, PracticeSite pracSite)
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
                var file = clientContext.Web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + "/Pages/Quality.aspx");
                file.CheckOut();
                try
                {
                    clientContext.Load(file);
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    try
                    {
                        LimitedWebPartManager olimitedwebpartmanager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);

                        WebPartDefinition wpd20 = olimitedwebpartmanager.ImportWebPart(contentEditorXML("Coming Soon...", "300px", "400px", web.Url + "/SiteAssets/cePrac_Quality.html"));
                        wpd20.WebPart.Title = "Coming Soon...";
                        olimitedwebpartmanager.AddWebPart(wpd20.WebPart, "CenterLeftColumn", 1);

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
                        SiteLogUtility.CreateLogEntry("ConfigureQualityPage Update", ex.Message, "Error", webUrl);
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("ConfigureQualityPage", ex.Message, "Error", webUrl);
                    outcome = false;
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
            return outcome;
        }

        public static bool ConfigurePayorEducationPage(string webUrl, PracticeSite pracSite)
        {
            SiteLogUtility.Log_Entry("ConfigurePayorEducationPage - In Progress...");
            SiteListUtility slu = new SiteListUtility();
            bool outcome = false;
            string clink = string.Empty;
            string scntPx = "666";

            using (ClientContext clientContext = new ClientContext(webUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                Web web = clientContext.Web;
                clientContext.Load(web, w => w.ServerRelativeUrl);
                clientContext.ExecuteQuery();
                var file = clientContext.Web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + "/Pages/" + slu.pageNamePayorEducation + ".aspx");
                file.CheckOut();
                try
                {
                    clientContext.Load(file);
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    try
                    {
                        LimitedWebPartManager olimitedwebpartmanager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);

                        WebPartDefinition wpd1 = olimitedwebpartmanager.ImportWebPart(contentEditorXML("Multi Tab", "600px", "700px", web.Url + "/SiteAssets/PayorEducation_MultiTab.js"));
                        wpd1.WebPart.Title = "Multi Tab";
                        olimitedwebpartmanager.AddWebPart(wpd1.WebPart, "CenterLeftColumn", 1);

                        if (pracSite.IsIWH == "true")
                        {
                            WebPartDefinition wpd5 = olimitedwebpartmanager.ImportWebPart(webPartXML(web.Url + "/" + slu.listNamePayorEducationIwh + "/Forms/PageViewer.aspx"));
                            wpd5.WebPart.Title = slu.webpartPayorEducationIwh;
                            olimitedwebpartmanager.AddWebPart(wpd5.WebPart, "CenterColumn", 1);
                        }
                        //if (pracSite.IsCKCC == "true")
                        //{
                        //    WebPartDefinition wpd6 = olimitedwebpartmanager.ImportWebPart(webPartXML(web.Url + "/" + slu.listNamePayorEducationCkcc + "/Forms/PageViewer.aspx"));
                        //    wpd6.WebPart.Title = slu.webpartPayorEducationCkcc;
                        //    olimitedwebpartmanager.AddWebPart(wpd6.WebPart, "CenterColumn", 1);
                        //}

                        file.CheckIn("Adding ConfigureHomePage webparts", CheckinType.MajorCheckIn);
                        file.Publish("Adding ConfigureHomePage webparts");
                        clientContext.Load(file);
                        web.Update();
                        clientContext.ExecuteQuery();
                        outcome = true;
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("ConfigurePayorEducationPage Update", ex.Message, "Error", webUrl);
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("ConfigurePayorEducationPage", ex.Message, "Error", webUrl);
                    outcome = false;
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
            return outcome;
        }

        public static string contentEditorXML(string webPartTitle, string webPartHeight, string webPartWidth, string webPartContentLink)
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

        public static string webPartXML(string strListURL)
        {
            string strXML = "";
            strXML = String.Format("<?xml version='1.0' encoding='utf-8' ?>" +
                        "<webParts>" +
                            "<webPart xmlns='http://schemas.microsoft.com/WebPart/v3'>" +
                                "<metaData>" +
                                    "<type name='Microsoft.SharePoint.WebPartPages.XsltListViewWebPart, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c' />" +
                                    "<importErrorMessage>Webdelen kan ikke importeres.</importErrorMessage>" +
                                "</metaData>" +
                                "<data>" +
                                    "<properties>" +
                                        "<property name='ListUrl' type='string'>{0}</property>" +
                                        "<property name='ChromeType' type='chrometype'>TitleOnly</property>" +
                                    "</properties>" +
                                "</data>" +
                            "</webPart>" +
                        "</webParts>", strListURL);
            return strXML;
        }

        public static void ViewsInit(string wUrl, Guid listGuid, PracticeCViews practiceCViews)
        {
            string _wUrl = wUrl;
            Guid _listGuid = listGuid;

            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                try
                {
                    Web w = clientContext.Web;
                    List list = w.Lists.GetById(_listGuid);
                    clientContext.Load(list);
                    clientContext.Load(list.Views);
                    clientContext.Load(list.Fields);
                    clientContext.Load(w);
                    clientContext.ExecuteQuery();

                    foreach (var view in practiceCViews.View)
                    {
                        View vw;
                        if (view.DefaultView && !view.CopyDefaultView)
                        {
                            vw = RefreshViewFieldsToView(list.DefaultView, list, view);
                        }
                        else
                        {
                            vw = ReturnListViewIfExists(list, listGuid, view, false, wUrl);
                            bool undoCheckout = UndoPageViewerCheckout(list, _listGuid, "PageViewer", true, wUrl);

                            if (vw == null)
                            {
                                try
                                {
                                    string query;
                                    System.Collections.Specialized.StringCollection strc = AddAdditionalFieldsToView(list, out query, _listGuid, view, wUrl);

                                    ViewCreationInformation vcI = new ViewCreationInformation();
                                    vcI.Title = view.ViewName;
                                    vcI.Query = query;
                                    vcI.RowLimit = 30;
                                    vcI.SetAsDefaultView = true;
                                    //vcI.ViewFields = new string[strc.Count];
                                    string CommaSeparateColumnNames = "Type,LinkFilename,Modified";
                                    vcI.ViewFields = CommaSeparateColumnNames.Split(',');
                                    vw = list.Views.Add(vcI);
                                    clientContext.Load(vw, v => v.Id, v => v.ViewQuery, v => v.Title, v => v.ViewFields, v => v.ViewType, v => v.DefaultView, v => v.PersonalView, v => v.RowLimit);
                                    clientContext.ExecuteQuery();
                                    //vw = list.Views.Add(view.ViewName, strc, query, 30, true, false);


                                    w.Update(); //Need to be this update or could it be just the List?

                                    if (view.ViewName == "PageViewer" || view.ViewName.StartsWith("WebPart_"))
                                    {
                                        if (view.ViewName == "PageViewer")
                                        {
                                            vw = ReturnListViewIfExists(list, listGuid, view, true, wUrl);
                                            undoCheckout = UndoPageViewerCheckout(list, _listGuid, "PageViewer", true, wUrl);
                                            string wbType = "Standard";
                                            if (view.WebPartRibbonOptions)
                                            { wbType = "Freeform"; }

                                            SetToolbarType(vw, wbType, list, view.WebPartRibbonOptions);
                                        }
                                        else
                                        {
                                            vw = ReturnListViewIfExists(list, listGuid, view, false, wUrl);
                                            undoCheckout = UndoPageViewerCheckout(list, _listGuid, "PageViewer", true, wUrl);
                                        }

                                        if (vw == null)
                                        {
                                            continue;
                                        }
                                        //vw.TabularView = false;
                                        vw.Update();

                                    }
                                    else
                                    {
                                        SetToolbarType(vw, "FreeForm", list, false);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // ignored
                                }
                            }
                            else
                            {
                                if (view.ViewName == "PageViewer")
                                {
                                    vw = ReturnListViewIfExists(list, listGuid, view, true, wUrl);
                                    undoCheckout = UndoPageViewerCheckout(list, _listGuid, "PageViewer", true, wUrl);
                                    string wbType = "Standard";
                                    if (view.WebPartRibbonOptions)
                                    { wbType = "Freeform"; }
                                    //
                                    SetToolbarType(vw, wbType, list, view.WebPartRibbonOptions);
                                    vw.Update();
                                }
                                if (view.RefreshView)
                                {
                                    vw = RefreshViewFieldsToView(list.DefaultView, list, view);
                                    vw.Update();
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("cViews-Init", ex.Message, "Error", wUrl);
                }
            }
        }

        public static View RefreshViewFieldsToView(View vw, List list, PracticeCView practiceCView)
        {
            //Clean view then add fields
            //vw.ViewFields.DeleteAll();
            vw.ViewFields.RemoveAll();
            return AddAdditionalFieldsToView(vw, list, practiceCView);
        }

        public static View AddAdditionalFieldsToView(View vw, List list, PracticeCView practiceCView)
        {
            foreach (PracticeCViewField cf in practiceCView.ViewFields.Fields)
            {
                //Field spf = list.Fields.GetSpField(cf.FieldName);
                Field spf = list.Fields.GetByTitle(cf.FieldName);
                if (spf != null)
                {
                    vw.ViewFields.Add(spf.ToString());
                }
            }

            if (practiceCView.ViewOrderBy.Count() > 0)
            {
                vw.ViewQuery = practiceCView.ViewOrderBy.configure_OrderBy(list);
            }
            return vw;
        }

        public static StringCollection AddAdditionalFieldsToView(List list, out string query, Guid _listGuid, PracticeCView practiceCView, string wURL = "")
        {
            query = string.Empty;
            StringCollection s = new StringCollection();
            try
            {
                if (practiceCView.CopyDefaultView)
                {
                    for (int intLoop = 0; intLoop < list.Views.Count; intLoop++)
                    {
                        if (list.Views[intLoop].DefaultView)
                        {
                            s.Add(list.Views[intLoop].ViewFields.ToString());
                            query = list.Views[intLoop].ViewQuery;
                        }
                    }

                    //s = list.DefaultView.ViewFields.ToStringCollection();
                    //query = list.DefaultView.ViewQuery;
                }
                else
                {
                    using (ClientContext clientContext = new ClientContext(wURL))
                    {
                        clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                        Web w = clientContext.Web;
                        list = w.Lists.GetById(_listGuid);
                        clientContext.Load(list);
                        clientContext.Load(list.Views);
                        clientContext.Load(list.Fields);
                        clientContext.Load(w);
                        clientContext.ExecuteQuery();

                        foreach (PracticeCViewField cf in practiceCView.ViewFields.Fields)
                        {
                            Field spf = list.Fields.GetByTitle(cf.FieldName);
                            try
                            {
                                clientContext.Load(spf);
                                clientContext.ExecuteQuery();
                                if (spf != null)
                                {
                                    //s.Add(spf.EntityPropertyName);
                                    s.Add(spf.InternalName);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        //if (practiceCView.ViewOrderBy.Count() > 0)
                        //{
                        //    query = practiceCView.ViewOrderBy.configure_OrderBy(list);
                        //}
                        //else if (practiceCView.UseEscoiDasFilter)
                        //{
                        //    query = GenerateCalendarViewFilter(practiceCView.Escoid);
                        //}
                        //else if (practiceCView.ViewName == "WebPart")
                        //{
                        //    query = GenerateAnnouncementWebPartViewFilter();
                        //}
                        //else if (practiceCView.ViewName.StartsWith("WebPart_"))
                        //{
                        //    query = GenerateCategoryWebPartFilter(practiceCView.ViewName.Replace("WebPart_", ""));
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
               SiteLogUtility.CreateLogEntry("AddAdditionalFieldsToView", ex.Message, "Error", "");
            }
            return s;
        }

        private static string GenerateCategoryWebPartFilter(string filter)
        {
            StringBuilder a = new StringBuilder();
            a.AppendFormat(@"<Where><Eq><FieldRef Name='Category' /><Value Type='Text'>{0}</Value></Eq></Where>",
                filter);
            return a.ToString();
        }

        private static string GenerateCalendarViewFilter(string escoid)
        {
            StringBuilder a = new StringBuilder();
            a.AppendFormat(
                @"<Where><Contains><FieldRef Name='ESCORegion' /><Value Type='Text'>{0}</Value></Contains></Where>",
                escoid);
            return a.ToString();
        }

        private static string GenerateAnnouncementWebPartViewFilter()
        {
            StringBuilder a = new StringBuilder();
            a.AppendFormat(
                @"<Where><Geq><FieldRef Name='Expires' /><Value IncludeTimeValue='FALSE' Type='DateTime'><Today /></Value></Geq></Where>");
            return a.ToString();
        }

        public static View ReturnListViewIfExists(List list, Guid _listGuid, PracticeCView practiceCView, bool justCreated = false, string wURL = "")
        {
            for (int i = 0; i < list.Views.Count; i++)
            {
                if (list.Views[i].Title.Equals(practiceCView.ViewName))
                {
                    if (justCreated && list.Views[i].Title == "PageViewer")
                    {
                        using (ClientContext clientContext = new ClientContext(wURL))
                        {
                            bool contentExists = false;
                            string checkingMessage = "Checking in back";
                            clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                            Web w = clientContext.Web;
                            list = w.Lists.GetById(_listGuid);
                            clientContext.Load(list);
                            clientContext.Load(list.Views);
                            clientContext.Load(list.Fields);
                            clientContext.Load(w);
                            clientContext.ExecuteQuery();
                            Microsoft.SharePoint.Client.File pvFile = w.GetFileByServerRelativeUrl(list.Views[i].ServerRelativeUrl);
                            try
                            {
                                pvFile.CheckOut();
                                clientContext.Load(pvFile);
                                clientContext.ExecuteQuery();
                                if (pvFile.Exists)
                                {
                                    //string str1 = @"<SharePoint:RssLink runat=""server"" />";
                                    //string str2 = @"<link rel=""stylesheet"" type=""text/css"" href=""/_layouts/15/PageViewerCustom.css"" />";

                                    //FileInformation oFileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, pvFile.ServerRelativeUrl);

                                    //using (System.IO.StreamReader sr = new System.IO.StreamReader(oFileInfo.Stream))
                                    //{
                                    //    string line = sr.ReadToEnd();
                                    //    if (!line.Contains(str2) && line.Contains(str1))
                                    //    {
                                    //        contentExists = true;
                                    //    }
                                    //}
                                    //if (contentExists)
                                    //{
                                    //    using (var stream = new MemoryStream())
                                    //    {
                                    //        using (var writer = new StreamWriter(stream))
                                    //        {
                                    //            writer.WriteLine(str1 + str2);
                                    //            writer.Flush();
                                    //            stream.Position = 0;
                                    //            Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, pvFile.ServerRelativeUrl, stream, true);
                                    //            checkingMessage = "Added PageViewerCustom css link";
                                    //        }
                                    //    }
                                    //}

                                    //pvFile.CheckIn(checkingMessage, CheckinType.MajorCheckIn);
                                    //pvFile.Publish(checkingMessage);
                                    //clientContext.Load(pvFile);
                                    //clientContext.ExecuteQuery();

                                    bool undoCheckout = UndoPageViewerCheckout(list, _listGuid, "PageViewer", true, wURL);
                                }
                            }
                            catch (Exception ex)
                            {
                                SiteLogUtility.CreateLogEntry("ReturnListViewIfExists", ex.Message, "Error", clientContext.Web.ServerRelativeUrl);
                                pvFile.CheckIn(checkingMessage, CheckinType.MajorCheckIn);
                                pvFile.Publish(checkingMessage);
                                clientContext.Load(pvFile);
                                clientContext.ExecuteQuery();
                                clientContext.Dispose();
                                // ignored
                            }
                        }
                        Microsoft.SharePoint.Client.View v = list.Views[i];
                        v.Update();
                    }
                    return list.Views[i];
                }
            }
            return null;
        }

        private static void SetToolbarType(View spView, string toolBarType, List list, bool WebPartRibbonOptions = false)
        {
            try
            {
                spView.GetType().InvokeMember("EnsureFullBlownXmlDocument",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null, spView, null, System.Globalization.CultureInfo.CurrentCulture);
                PropertyInfo nodeProp = spView.GetType().GetProperty("Node",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                XmlNode node = nodeProp.GetValue(spView, null) as XmlNode;
                XmlNode toolbarNode = node.SelectSingleNode("Toolbar");
                if (toolbarNode != null)
                {
                    toolbarNode.Attributes["Type"].Value = toolBarType;
                    if (spView.Title == "PageViewer" && WebPartRibbonOptions)
                    {
                        XmlDocument doc = toolbarNode.OwnerDocument;
                        XmlAttribute xa = doc.CreateAttribute("ShowAlways");
                        xa.Value = "TRUE";
                        toolbarNode.Attributes.SetNamedItem(xa);
                    }

                    // If the toolbartype is Freeform (i.e. Summary Toolbar) then we need to manually 
                    // add some CAML to get it to work.
                    if (String.Compare(toolBarType, "Freeform", true, System.Globalization.CultureInfo.InvariantCulture) ==
                        0)
                    {
                        string newItemString;
                        XmlAttribute positionNode = toolbarNode.OwnerDocument.CreateAttribute("Position");
                        positionNode.Value = "After";
                        toolbarNode.Attributes.Append(positionNode);

                        switch (list.BaseTemplate)
                        {
                            case (int)ListTemplateType.Announcements:
                                newItemString = "announcement";
                                break;
                            case (int)ListTemplateType.Events:
                                newItemString = "event";
                                break;
                            case (int)ListTemplateType.Tasks:
                                newItemString = "task";
                                break;
                            case (int)ListTemplateType.DiscussionBoard:
                                newItemString = "discussion";
                                break;
                            case (int)ListTemplateType.Links:
                                newItemString = "link";
                                break;
                            case (int)ListTemplateType.GenericList:
                                newItemString = "item";
                                break;
                            case (int)ListTemplateType.DocumentLibrary:
                                newItemString = "document";
                                break;
                            default:
                                newItemString = "item";
                                break;
                        }

                        //if (list.BaseTemplate == BaseType.DocumentLibrary)
                        //{
                        //    newItemString = "document";
                        //}

                        // Add the CAML
                        toolbarNode.InnerXml =
                            @"<IfHasRights><RightsChoices><RightsGroup PermAddListItems=""required"" /></RightsChoices><Then><HTML><![CDATA[ <table width=100% cellpadding=0 cellspacing=0 border=0 > <tr> <td colspan=""2"" class=""ms-partline""><IMG src=""/_layouts/images/blank.gif"" width=1 height=1 alt=""""></td> </tr> <tr> <td class=""ms-addnew"" style=""padding-bottom: 3px""> <img src=""/_layouts/images/rect.gif"" alt="""">&nbsp;<a class=""ms-addnew"" ID=""idAddNewItem"" href=""]]></HTML><URL Cmd=""New"" /><HTML><![CDATA["" ONCLICK=""javascript:NewItem(']]></HTML><URL Cmd=""New"" /><HTML><![CDATA[', true);javascript:return false;"" target=""_self"">]]></HTML><HTML>Add new " +
                            newItemString +
                            @"</HTML><HTML><![CDATA[</a> </td> </tr> <tr><td><IMG src=""/_layouts/images/blank.gif"" width=1 height=5 alt=""""></td></tr> </table>]]></HTML></Then></IfHasRights>";
                    }

                    spView.Update();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("cViews-SetToolbarType", ex.Message, "Error", list.ParentWebUrl);
            }
        }

        public static bool modifyWebPartProgramParticipation(string webUrl, PracticeSite practiceSite)
        {
            bool outcome = false;
            string clink = string.Empty;
            int webPartHeight = gridHeight(webUrl, practiceSite);

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
                        outcome = true;
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("modifyWebPart", ex.Message, "Error", webUrl);
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("modifyWebPart", ex.Message, "Error", webUrl);
                    outcome = false;
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
            return outcome;
        }
        public static int gridHeight(string webUrl, PracticeSite site)
        {
            int intCount = -1;
            int[] intHeight = new int[4] { 156, 253, 350, 447 };
            try
            {
                if (site.IsIWH == "true")
                {
                    intCount++;
                }
                if (site.IsCKCC == "true")
                {
                    intCount++;
                    intCount++;  // For Dialysis Starts...
                }
                if (site.IsKC365 == "true")
                {
                    intCount++;
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("gridHeight", ex.Message, "Error", webUrl);
            }
            return intHeight[intCount];
        }
        public static void UpdateWebPartSize(string webURL)
        {
            var pageRelativeUrl = "/Pages/ProgramParticipation.aspx";
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
                            //oWebPart.DeleteWebPart();
                            oWebPart.IsPropertyAvailable("Height");
                            oWebPart.IsPropertyAvailable("Width");
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
        public static List<PMData> initPMDataToList(string adminGroupUrl)
        {
            //SitePMData objSitePMData = new SitePMData();
            List<PMData> pmData = new List<PMData>();
            pmData = SP_GetPortalData_PMData(adminGroupUrl);
            return pmData;
        }
        public static DataTable readPMSiteData()
        {
            try
            {
                string connString = "Data Source=" + ConfigurationManager.AppSettings["SqlServer"]
                        + "; Initial Catalog=" + ConfigurationManager.AppSettings["Database"] + "; Integrated Security=SSPI";

                string query = @"SELECT * FROM [HealthCloud_NightlyProd].[PORTAL].[vwPracticeInfo] ORDER BY GroupID";

                DataTable dtTable = new DataTable();
                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtTable);
                conn.Close();
                da.Dispose();

                return dtTable;
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("readPMSiteData", ex.Message, "Error", "");
                return null;
            }
        }

        public static List<PMData> SP_GetPortalData_PMData(string adminGroupUrl)
        {
            List<PMData> All_PortalData = new List<PMData>();
            List<PMData> CKCC_PMData = new List<PMData>();
            try
            {
                All_PortalData = SP_GetAll_PMData(adminGroupUrl);
                CKCC_PMData = All_PortalData.Where
                    (x => x.ProgramParticipation.Contains("KCE Participation")).ToList();

                //ResultDescription += "[" + Answers_NewReferrals.Count + "] items found in SP => Visible and have answers." + textLine;
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("SP_GetPortalData_PMData", ex.Message, "Error", "");
            }

            return CKCC_PMData;
        }

        public static List<PMData> SP_GetAll_PMData(string urlAdminGrp)
        {
            List<PMData> pmData = new List<PMData>();
            SitePMData sitePMData = new SitePMData();

            using (ClientContext clientContext = new ClientContext(urlAdminGrp))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                List list = clientContext.Web.Lists.GetByTitle("AdminGroup");
                clientContext.Load(list);
                clientContext.ExecuteQuery();
                View view = list.Views.GetByTitle("All Links");

                clientContext.Load(view);
                clientContext.ExecuteQuery();
                CamlQuery query = new CamlQuery();
                query.ViewXml = view.ViewQuery;

                ListItemCollection items = list.GetItems(query);
                clientContext.Load(items);
                clientContext.ExecuteQuery();
                SiteLogUtility.Log_Entry("Total Count: " + items.Count, true);

                foreach (var item in items)
                {
                    PMData pmd = new PMData();


                    SiteLogUtility.Log_Entry(item["PracticeTIN"] + " - " + item["PracticeName"] + " - " + item["ProgramParticipation"], true);
                    //SiteLogUtility.Log_Entry("PracticeName: " + item["PracticeName"], true);
                    //SiteLogUtility.Log_Entry("PracticeTIN: " + item["PracticeTIN"], true);
                    //SiteLogUtility.Log_Entry("ProgramParticipation: " + item["ProgramParticipation"], true);

                    pmd.PracticeName = item["PracticeName"].ToString();
                    pmd.PracticeTIN = item["PracticeTIN"].ToString();
                    pmd.SiteId = item["PracticeTIN"].ToString();
                    pmd.ProgramParticipation = item["ProgramParticipation"].ToString();

                    pmd.IsKC365 = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationKC365) ? "true" : "false";
                    pmd.IsCKCC = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationCKCC) ? "true" : "false";
                    pmd.IsIWH = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationIWH) ? "true" : "false";
                    //sourceSite.SetAttributeValue("IsKC365", Convert.ToInt32(dr["KC365"]) == 0 ? "false" : "true");
                    //sourceSite.SetAttributeValue("kceArea", dr["CKCCArea"]);
                    //sourceSite.SetAttributeValue("IsCKCC", dr["CKCCArea"].ToString() == "" ? "false" : "true");
                    //sourceSite.SetAttributeValue("IsIWH", dr["IWNRegion"].ToString() == "" ? "false" : "true");

                    pmData.Add(pmd);
                }
            }

            return pmData;
        }

        public List<PMData> filterPMDataToList(DataTable pmDT)
        {
            List<PMData> pmData = new List<PMData>();
            return pmData;
        }

        public static void filterPMSiteData(DataTable allData)
        {
            try
            {
                DataTable dtDataNew = allData.Clone();
                DataView view = new DataView(allData);
                DataTable distinctValues = view.ToTable(true, "GroupID");

                for (int intLoop = 0; intLoop < distinctValues.Rows.Count; intLoop++)
                {
                    if (intLoop <= 9)
                    {
                        dtDataNew = allData.AsEnumerable().Where(row => row.Field<Int32>("GroupID") == Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"])).CopyToDataTable();
                        //updateXML(dtDataNew, ConfigurationManager.AppSettings["ConfigURL"] + "PracticeSiteTemplate_PM" + Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"]).ToString("00") + ".config", "PracticeSite20_PM" + Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"]).ToString("00"));
                    }
                    else
                    {
                        dtDataNew = allData.AsEnumerable().Where(row => row.Field<Int32>("GroupID") == Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"])).CopyToDataTable();
                        //updateXML(dtDataNew, ConfigurationManager.AppSettings["ConfigURL"] + "PracticeSiteTemplate_PM" + distinctValues.Rows[intLoop]["GroupID"].ToString() + ".config", "PracticeSite20_PM" + distinctValues.Rows[intLoop]["GroupID"].ToString());
                    }
                    dtDataNew.Rows.Clear();
                }
            }
            catch (Exception ex)
            {

            }
        }

        //public static void updateXML(DataTable dt, string xmlfilePath, string strRegionID)
        //{
        //    try
        //    {
        //        XDocument sourceFile = XDocument.Load(ConfigurationManager.AppSettings["ConfigURL"] + "PracticeSiteTemplate.config");
        //        XDocument xdoc = XDocument.Load(xmlfilePath);
        //        var sourceElementSbsite = sourceFile.Elements("Config").Elements("Sites").Elements("Site").Elements("SubSites").Elements("Site");
        //        var propertyValueSourceEle = sourceFile.Elements("Config").Elements("Sites").Elements("Site").Elements("SubSites").Elements("Site").Elements("SiteSettings").Elements("PropertyBag").Elements("Property");
        //        var sourceSite = sourceElementSbsite.FirstOrDefault();
        //        var propertySourceSite = propertyValueSourceEle.FirstOrDefault();
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            DataRow dr = dt.Rows[i];
        //            sourceSite.SetAttributeValue("SiteName", dr["SiteID"]);
        //            sourceSite.SetAttributeValue("SiteTitle", dr["PracticeName"]);
        //            sourceSite.SetAttributeValue("RegionID", strRegionID);
        //            sourceSite.SetAttributeValue("SiteDescription", dr["PracticeName"] + " is a member of " + strRegionID);
        //            sourceSite.SetAttributeValue("IsKC365", Convert.ToInt32(dr["KC365"]) == 0 ? "false" : "true");
        //            sourceSite.SetAttributeValue("kceArea", dr["CKCCArea"]);
        //            sourceSite.SetAttributeValue("IsCKCC", dr["CKCCArea"].ToString() == "" ? "false" : "true");
        //            sourceSite.SetAttributeValue("IsIWH", dr["IWNRegion"].ToString() == "" ? "false" : "true");
        //            sourceSite.SetAttributeValue("encryptedTIN", dr["EncryptedPracticeTIN"]);
        //            propertySourceSite.SetAttributeValue("PropertyValue", strRegionID);
        //            xdoc.Element("Config").Element("Sites").Element("Site").Element("SubSites").Add(sourceSite);
        //            //xdoc.Element("Config").Element("Sites").Element("Site").Element("SubSites").Element("Site").Element("SiteSettings").Element("PropertyBag").Element("Property").Add(propertySourceSite);
        //            xdoc.Save(xmlfilePath);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        public static View ReturnListViewIfExists(List list, Guid _listGuid, string ViewName, bool justCreated = false, string wURL = "")
        {
            for (int i = 0; i < list.Views.Count; i++)
            {
                if (list.Views[i].Title.Equals(ViewName))
                {
                    if (justCreated && list.Views[i].Title == "PageViewer")
                    {
                        using (ClientContext clientContext = new ClientContext(wURL))
                        {
                            bool contentExists = false;
                            string checkingMessage = "Checking in back";
                            clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                            Web w = clientContext.Web;
                            list = w.Lists.GetById(_listGuid);
                            clientContext.Load(list);
                            clientContext.Load(list.Views);
                            clientContext.Load(list.Fields);
                            clientContext.Load(w);
                            clientContext.ExecuteQuery();
                            Microsoft.SharePoint.Client.File pvFile = w.GetFileByServerRelativeUrl(list.Views[i].ServerRelativeUrl);
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
                        Microsoft.SharePoint.Client.View v = list.Views[i];
                        v.Update();
                    }
                    return list.Views[i];
                }
            }
            return null;
        }

        public static bool UndoPageViewerCheckout(List list, Guid _listGuid, string ViewName, bool justCreated = false, string wURL = "")
        {
            for (int i = 0; i < list.Views.Count; i++)
            {
                if (list.Views[i].Title.Equals(ViewName))
                {
                    if (justCreated && list.Views[i].Title == "PageViewer")
                    {
                        using (ClientContext clientContext = new ClientContext(wURL))
                        {
                            bool contentExists = false;
                            string checkingMessage = "Checking in back";
                            clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                            Web w = clientContext.Web;
                            list = w.Lists.GetById(_listGuid);
                            clientContext.Load(list);
                            clientContext.Load(list.Views);
                            clientContext.Load(list.Fields);
                            clientContext.Load(w);
                            clientContext.ExecuteQuery();
                            Microsoft.SharePoint.Client.File pvFile = w.GetFileByServerRelativeUrl(list.Views[i].ServerRelativeUrl);
                            clientContext.Load(pvFile);
                            User user = pvFile.Author;
                            clientContext.Load(user);
                            clientContext.ExecuteQuery();

                            try
                            {
                                if (pvFile.CheckOutType != CheckOutType.None)
                                {
                                    SiteLogUtility.Log_Entry("-----------------------", true);
                                    SiteLogUtility.Log_Entry(pvFile.Author.Title.ToString(), true);
                                    SiteLogUtility.Log_Entry(pvFile.Author.LoginName.ToString(), true);
                                    SiteLogUtility.Log_Entry(pvFile.CheckOutType.ToString(), true);
                                    SiteLogUtility.Log_Entry(pvFile.Name.ToString(), true);
                                    SiteLogUtility.Log_Entry(pvFile.CheckedOutByUser.ToString(), true);
                                    SiteLogUtility.Log_Entry("", true);

                                    pvFile.UndoCheckOut();
                                    clientContext.Load(pvFile);
                                    clientContext.ExecuteQuery();
                                }

                            }
                            catch (Exception ex)
                            {
                                SiteLogUtility.CreateLogEntry("UndoPageViewerCheckout", ex.Message, "Error", clientContext.Web.ServerRelativeUrl);
                            }
                        }
                        Microsoft.SharePoint.Client.View v = list.Views[i];
                        v.Update();
                    }
                    return true;
                }
            }
            return true;
        }

        public static void CreateFolder(PracticeSite practiceSite, string docListName, string folderName)
        {
            SiteLogUtility.Log_Entry("CreateFolder - In Progress...");
            try
            {
                using (ClientContext clientContext = new ClientContext(practiceSite.URL))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                    List docList = clientContext.Web.Lists.GetByTitle(docListName);
                    FolderCollection folderCollection = docList.RootFolder.Folders;
                    clientContext.Load(folderCollection);
                    clientContext.ExecuteQuery();

                    if (practiceSite.IsCKCC == "true")
                    {
                        Folder parentFolder = docList.RootFolder.Folders.Add(folderName);
                    }

                    clientContext.Load(folderCollection);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("CreateFolder", ex.Message, "Error", practiceSite.URL);
            }
        }
        public static bool SP_Update_ProgramParticipation(string wUrl, string pageName, string searchTitle, string newTitle, string newThumbnail)
        {
            SiteLogUtility.Log_Entry("SP_Update_ProgramParticipation - In Progress...");
            string pageNameAspx = pageName + ".aspx";

            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                try
                {
                    SiteFilesUtility siteFilesUtility = new SiteFilesUtility();
                    string rootWebUrl = siteFilesUtility.GetRootSite(wUrl);
                    string fileName1 = newThumbnail;

                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle("Program Participation");
                    clientContext.Load(web);
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();
                    View view = list.Views.GetByTitle("All Documents");

                    clientContext.Load(view);
                    clientContext.ExecuteQuery();
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = view.ViewQuery;

                    ListItemCollection items = list.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    foreach (var item in items)
                    {

                        if (item["Title"].ToString().Contains(searchTitle))
                        {
                            SiteLogUtility.Log_Entry("BEFORE - ProgramNameText", true);
                            SiteLogUtility.Log_Entry(item["ProgramNameText"].ToString(), true);

                            item.File.CheckOut();
                            clientContext.ExecuteQuery();
                            item["Title"] = newTitle;
                            item["ProgramNameText"] = web.Url + "/Pages/" + pageNameAspx;
                            item["Thumbnail"] = wUrl + "/Program%20Participation/" + fileName1;
                            item.Update();
                            item.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                            clientContext.ExecuteQuery();

                            SiteLogUtility.Log_Entry("AFTER - ProgramNameText", true);
                            SiteLogUtility.Log_Entry(item["ProgramNameText"].ToString(), true);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("SP_Update_ProgramParticipation", ex.Message, "Error", wUrl);
                    return false;
                }
            }

            return true;
        }
        
        public static object lockObjBenefitEnhancement = new object();
        public static object lockObjQuality = new object();
        public static object lockObjPayorEducation = new object();
        public void uploadMultiPartSupportingFiles(string wUrl, PracticeSite practiceSite)
        {
            try
            {
                SiteListUtility slu = new SiteListUtility();
                string strJSContentBenefitEnhancement = "";
                string strJSContentQuality = "";
                string strJSContentPayorEducation = "";
                /*
                 * BenefitEnhancement_MultiTab.js
                 * Quality_MultiTab.js
                 * PayorEducation_MultiTab.js
                 */
                string strJSFileServerPathBenefitEnhancement = LayoutsFolderMnt + "BenefitEnhancement_MultiTab.js";
                string strJSFileServerPathQuality = LayoutsFolderMnt + "Quality_MultiTab.js";
                string strJSFileServerPathPayorEducation = LayoutsFolderMnt + "PayorEducation_MultiTab.js";

                if (practiceSite.IsIWH.Equals("true"))
                {
                    strJSContentQuality = @"var thisTab2 = {title: '" + slu.tabTitleQualityIwh + "',webParts: ['" + slu.webpartQualityIwh + "']};tabConfiguration.push(thisTab2);";
                    strJSContentPayorEducation = @"var thisTab2 = {title: '" + slu.tabTitlePayorEducationIwh + "',webParts: ['" + slu.webpartPayorEducationIwh + "']};tabConfiguration.push(thisTab2);";
                }
                if (practiceSite.IsCKCC.Equals("true"))
                {
                    strJSContentBenefitEnhancement = @"var thisTab2 = {title: '" + slu.tabTitleBenefitEnhancementCkcc + "',webParts: ['" + slu.webpartBenefitEnhancementCkcc + "']};tabConfiguration.push(thisTab2);";
                    
                    strJSContentQuality = strJSContentQuality + @"var thisTab3 = {title: '" + slu.tabTitleQualityCkcc + "',webParts: ['" + slu.webpartQualityCkcc + "']};tabConfiguration.push(thisTab3);";
                    //strJSContentPayorEducation = strJSContentPayorEducation + @"var thisTab3 = {title: '" + slu.tabTitlePayorEducationCkcc + "',webParts: ['" + slu.webpartPayorEducationCkcc + "']};tabConfiguration.push(thisTab3);";
                }

                strJSContentBenefitEnhancement = strJSContentBenefitEnhancement + "//*#funXXXX#*";
                strJSContentQuality = strJSContentQuality + "//*#funXXXX#*";
                strJSContentPayorEducation = strJSContentPayorEducation + "//*#funXXXX#*";

                lock (lockObjBenefitEnhancement)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathBenefitEnhancement).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentBenefitEnhancement;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathBenefitEnhancement, lines);
                }

                lock (lockObjQuality)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathQuality).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentQuality;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathQuality, lines);
                }

                lock (lockObjPayorEducation)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathPayorEducation).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentPayorEducation;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathPayorEducation, lines);
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("uploadMultiPartSupportingFiles", ex.Message, "Error", wUrl);
            }
        }
        
    }
}

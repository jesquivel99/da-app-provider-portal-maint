using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using System.Xml;
using System.Collections.Specialized;
using System.Reflection;
using Microsoft.SharePoint.Client.WebParts;

namespace SiteUtility
{
    public class SiteListUtility
    {
        static Guid _listGuid = Guid.Empty;

        // Benefit Enhancement...
        public string pageNameBenefitEnhancement = "CkccKceResources";
        public string pageTitleBenefitEnhancement = "CKCC/KCE Resources";

        public string listNameBenefitEnhancementCkcc = "BenefitEnhancementCkcc";
        public string listTitleBenefitEnhancementCkcc = "Benefit Enhancement Ckcc";
        public string listFolder1BenefitEnhancementCkcc = "Benefit Enhancement Training";
        public string listFolder2BenefitEnhancementCkcc = "Operating Guides and Documents";
        public string tabTitleBenefitEnhancementCkcc = "CKCC/KCE";
        public string webpartBenefitEnhancementCkcc = "BenefitEnhancement_Ckcc";

        // Quality...
        public string pageNameQuality = "Quality";
        public string pageTitleQuality = "Quality";

        public string listNameQualityIwh = "QualityIwh";
        public string listTitleQualityIwh = "QualityIwh";
        public string listFolder1QualityIwh = "Quality Reporting";
        public string listFolder2QualityIwh = @"Education-Training-Resources";
        public string listFolder3QualityIwh = "Supporting Documentation from Practices";
        public string tabTitleQualityIwh = "Payor Programs";
        public string webpartQualityIwh = "Quality_Iwh";

        public string listNameQualityCkcc = "QualityCkcc";
        public string listTitleQualityCkcc = "QualityCkcc";
        public string listFolder1QualityCkcc = "Quality Reporting";
        public string listFolder2QualityCkcc = @"Education-Training-Resources";
        public string listFolder3QualityCkcc = "Supporting Documentation from Practices";
        public string tabTitleQualityCkcc = "CKCC/KCE";
        public string webpartQualityCkcc = "Quality_Ckcc";

        // Payor Education...
        public string pageNamePayorEducation = "PayorEdResources";
        public string pageTitlePayorEducation = "Payor Education Resources";

        public string listNamePayorEducationIwh = "PayorEdResourcesIwh";
        public string listTitlePayorEducationIwh = "Payor Education Resources Iwh";
        public string listFolder1PayorEducationIwh = "Education";
        public string listFolder2PayorEducationIwh = "CKD Support";
        public string tabTitlePayorEducationIwh = "Payor Programs";
        public string webpartPayorEducationIwh = "PayorEducation_Iwh";

        public string listNamePayorEducationCkcc = "PayorEdResourcesCkcc";
        public string listTitlePayorEducationCkcc = "Payor Education Resources Ckcc";
        public string listFolder1PayorEducationCkcc = "Education";
        public string tabTitlePayorEducationCkcc = "CKCC/KCE";
        public string webpartPayorEducationCkcc = "PayorEducation_Ckcc";

        // Data Exchange...
        public string pageNameDataExchange = "DataExchange";
        public string pageTitleDataExchange = "Data Exchange";

        public string listNameDataExchangeIwh = "Documentsiwh";
        public string listTitleDataExchangeIwh = "Documentsiwh";
        public string tabTitleDataExchangeIwh = "Payor Programs";
        public string webpartDataExchangeIwh = "Practice Documents IWH";
        public string listFolder1DataExchangeIwh = "Explanation of Payment";
        public string listFolder2DataExchangeIwh = "ESRD Practice Rosters";
        public string listFolder3DataExchangeIwh = "Hospital Notifications";
        public string listFolder4DataExchangeIwh = "Other Documents";
        

        public string listNameDataExchangeCkcc = "Documentsckcc";
        public string listTitleDataExchangeCkcc = "DocumentsCkcc";
        public string tabTitleDataExchangeCkcc = "CKCC/KCE";
        public string webpartDataExchangeCkcc = "Practice Documents CKCC";
        public string listFolder1DataExchangeCkcc = "CKD Support";
        public string listFolder2DataExchangeCkcc = "Explanation of Payment";
        public string listFolder3DataExchangeCkcc = "High Risk Progression";
        public string listFolder4DataExchangeCkcc = "Hospital Notifications";
        public string listFolder5DataExchangeCkcc = "KCE Alignment Reports";
        public string listFolder6DataExchangeCkcc = "Metrics";
        public string listFolder7DataExchangeCkcc = "Other Documents";

        // Risk Adjustment...
        public string pageNameRiskAdjustment = "RiskAdjustmentResources";
        public string pageTitleRiskAdjustment = "Risk Adjustment Resources";

        public string listNameRiskAdjustmentIwh = "RiskAdjustment_iwh";
        public string listTitleRiskAdjustmentIwh = "RiskAdjustment_iwh";
        public string tabTitleRiskAdjustmentIwh = "Private Payor Program";
        public string webpartRiskAdjustmentIwh = "Private Payor Program";
        public string listFolder1RiskAdjustmentIwh = "Accurate Documentation Worksheet";
        public string listFolder2RiskAdjustmentIwh = "Medical Chart Reviews";
        

        public string listNameRiskAdjustmentCkcc = "RiskAdjustment_ckcc";
        public string listTitleRiskAdjustmentCkcc = "RiskAdjustment_ckcc";
        public string tabTitleRiskAdjustmentCkcc = "CKCC/KCE Program";
        public string webpartRiskAdjustmentCkcc = "CKCC/KCE Program";
        public string listFolder1RiskAdjustmentCkcc = "Accurate Documentation Worksheet";
        public string listFolder2RiskAdjustmentCkcc = "AWV Clinical Intervention Incentive";
        public string listFolder3RiskAdjustmentCkcc = "Medical Chart Reviews";

        //Program Participation List items...
        public const string progpart_PayorEnrollment = "Payor Enrollment";
        public const string progpart_CkccKceResources = "CKCC/KCE Resources";
        public const string progpart_PayorProgeducation = "Payor Program Education Resources";
        public const string progpart_PatientStatusUpdates = "Patient Status Updates";
        public const string progpart_CkccKceEngagement = "CKCC/KCE Engagement";

        static ILogger logger = Log.ForContext<SiteListUtility>();

        public void CreateList(string strListName, string strWebURL, int listType)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(strWebURL))
                {
                    // The properties of the new custom list
                    ListCreationInformation creationInfo = new ListCreationInformation();
                    creationInfo.Title = strListName;
                    creationInfo.TemplateType = listType;

                    List newList = clientContext.Web.Lists.Add(creationInfo);
                    clientContext.Load(newList);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("CreateList", ex.Message, "Error", strWebURL);
            }
        }
        public void CreateListColumn(string strColumnXML, string strListName, string strWebURL)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(strWebURL))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    List targetList = clientContext.Web.Lists.GetByTitle(strListName);
                    Field oField = targetList.Fields.AddFieldAsXml(strColumnXML, true, AddFieldOptions.AddFieldInternalNameHint);

                    clientContext.Load(oField);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("CreateListColumn", ex.Message, "Error", "");
            }
        }
        public void ProvisionField(Practice psite, string listTitle, string fieldName)
        {
            string _wUrl = psite.NewSiteUrl;
            //Guid _listGuid = listGuid;
            SiteListUtility slu = new SiteListUtility();
            SiteLogUtility siteLogUtility = new SiteLogUtility();

            using (ClientContext clientContext = new ClientContext(_wUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                {
                    try
                    {
                        Web w = clientContext.Web;
                        //List list = w.Lists.GetById(_listGuid);
                        List list = w.Lists.GetByTitle(listTitle);
                        FieldCollection collFd = list.Fields;
                        clientContext.Load(w);
                        clientContext.Load(list);
                        clientContext.Load(collFd);
                        clientContext.ExecuteQuery();

                        bool fieldFound = false;
                        foreach (Field f in collFd)
                        {
                            //siteLogUtility.LoggerInfo_Entry(f.Title);
                            if (f.Title == fieldName)
                            {
                                fieldFound = true;
                                siteLogUtility.LoggerInfo_Entry($"Field Found: {f.Title}", true);
                                break;
                            }
                        }

                        if (fieldFound == false)
                        {
                            siteLogUtility.LoggerInfo_Entry($"Field NOT Found - Creating: {fieldName}", true);
                            //slu.CreateListColumn($"<Field Type='Number' DisplayName='{fieldName}' Name='{fieldName}' />", listTitle, _wUrl);
                            slu.CreateListColumn($"<Field Type='Text' DisplayName='{fieldName}' Name='{fieldName}' />", listTitle, _wUrl);
                        }
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("Provision Field", ex.Message, "Error", "", true);
                        logger.Error(ex.Message);
                    }
                }
            }
        }
        public Guid CreateDocumentLibrary(string strListName, string strWebURL, Practice practiceSite)
        {
            Guid _listGuid = Guid.Empty;
            bool createList = true;

            using (ClientContext clientContext = new ClientContext(strWebURL))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                {
                    Web w = clientContext.Web;
                    try
                    {
                        if (strListName.Contains("iwh"))
                        {
                            //if (practiceSite.siteType != null && practiceSite.siteType.Contains("iwh"))
                            if (practiceSite.IsIWH)
                            {
                                createList = true;
                            }
                            else
                            {
                                createList = false;
                            }
                        }

                        if (strListName.Contains("ckcc"))
                        {
                            //if (practiceSite.siteType != null && practiceSite.siteType.Contains("ckcc"))
                            if (practiceSite.IsCKCC)
                            {
                                createList = true;
                            }
                            else
                            {
                                createList = false;
                            }
                        }

                        if (strListName.Contains("kc365"))
                        {
                            //if (practiceSite.siteType != null && practiceSite.siteType.Contains("kc365"))
                            if (practiceSite.IsKC365)
                            {
                                createList = true;
                            }
                            else
                            {
                                createList = false;
                            }
                        }
                        if (createList)
                        {
                            // The properties of the new document library...
                            ListCreationInformation creationInfo = new ListCreationInformation();
                            creationInfo.Title = strListName;
                            creationInfo.TemplateType = (int)ListTemplateType.DocumentLibrary;

                            List newList = clientContext.Web.Lists.Add(creationInfo);
                            clientContext.Load(newList, o => o.Id);
                            clientContext.ExecuteQuery();
                            _listGuid = newList.Id; 
                        }

                        return _listGuid;

                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("CreateDocumentLibrary", ex.Message, "Error", strWebURL);
                        return Guid.Empty;
                    }
                }
            }
        }
        public static Boolean DoesFolderExist(FolderCollection fc, string fname)
        {
            try
            {
                if (fc != null & fc.Count > 0)
                    return (fc.First(f => f.Name == fname) != null);
                else
                    return false;
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("DoesFolderExist", ex.Message, "Error", "");
                return false;
            }
        }
        public static void CreateFolder(Practice practiceSite, string docListName, string folderName)
        {
            SiteLogUtility.Log_Entry("CreateFolder - In Progress...");
            
            try
            {
                using (ClientContext clientContext = new ClientContext(practiceSite.NewSiteUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                    List docList = clientContext.Web.Lists.GetByTitle(docListName);
                    FolderCollection folderCollection = docList.RootFolder.Folders;
                    clientContext.Load(folderCollection);
                    clientContext.ExecuteQuery();

                    if (!SiteListUtility.DoesFolderExist(folderCollection, folderName))
                    {
                        Folder parentFolder = docList.RootFolder.Folders.Add(folderName);
                        clientContext.Load(folderCollection);
                        clientContext.ExecuteQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("CreateFolder", ex.Message, "Error", "");
            }
        }
        public static void ProvisionList(Practice psite, SiteListUtility siUtility, string listName, PracticeCView pracCView)
        {
            SiteLogUtility.Log_Entry("ProvisionList - In Progress...");
            if (!DoesListExist(psite.NewSiteUrl, listName))
            {
                _listGuid = siUtility.CreateDocumentLibrary(listName, psite.NewSiteUrl, psite);
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

                ViewsInit(psite.NewSiteUrl, _listGuid, practiceCViews);

                //Setup Subfolders
                //SubFolders.Init(wUrl, _listGuid);
            }
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
                    SiteLogUtility.CreateLogEntry("cViews-Init", ex.Message, "Error", "");
                }
            }
        }
        public static void modifyView(string webUrl, string strPageName = "Home.aspx", string strWebPartTitle = "Practice Documents")
        {
            SiteLogUtility.Log_Entry("   modifyView - In Progress...");
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
                    else if (strWebPartTitle == slu.webpartDataExchangeIwh)   // DataExchange
                    {
                        list = w.Lists.GetByTitle(slu.listNameDataExchangeIwh);
                    }
                    else if (strWebPartTitle == slu.webpartDataExchangeCkcc)   // DataExchange
                    {
                        list = w.Lists.GetByTitle(slu.listNameDataExchangeCkcc);
                    }
                    else if (strWebPartTitle == slu.webpartRiskAdjustmentIwh)   // RiskAdjustment
                    {
                        list = w.Lists.GetByTitle(slu.listNameRiskAdjustmentIwh);
                    }
                    else if (strWebPartTitle == slu.webpartRiskAdjustmentCkcc)   // RiskAdjustment
                    {
                        list = w.Lists.GetByTitle(slu.listNameRiskAdjustmentCkcc);
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
                                SiteLogUtility.Log_Entry("WebPart = " + propValues["Title"].ToString(), true);
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
                        SiteLogUtility.CreateLogEntry("Quality - modifyView", ex.Message, "Error", "");
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
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
                SiteLogUtility.CreateLogEntry("cViews-SetToolbarType", ex.Message, "Error", "");
            }
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
                                SiteLogUtility.CreateLogEntry("ReturnListViewIfExists", ex.Message, "Error", "");
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
                                SiteLogUtility.CreateLogEntry("UndoPageViewerCheckout", ex.Message, "Error", "");
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
        public static bool DoesListExist(string wUrl, string listName)
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
        public void CreateListItem(string strListName, string webUrl, List<string> listColumnName,List<string> listItemData)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(webUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web w = clientContext.Web;
                    clientContext.Load(w);
                    clientContext.ExecuteQuery();

                    List oList = clientContext.Web.Lists.GetByTitle(strListName);
                    ListItemCreationInformation oListItemCreationInformation = new ListItemCreationInformation();
                    ListItem oItem = oList.AddItem(oListItemCreationInformation);

                    for (int intLoop = 0; intLoop < listColumnName.Count; intLoop++)
                    {
                        oItem[listColumnName[intLoop]] = listItemData[intLoop];
                        oItem.Update();
                    }
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("CreateListItem", ex.Message, "Error", webUrl);
            }
        }
        public List<Practice> GetAdminList(string sUrl, string strProgramManagerSite = "")
        {
            //SiteInfoUtility siteInfoUtility = new SiteInfoUtility();
            List<Practice> AllAdminList = new List<Practice>();

            using (ClientContext clientContext = new ClientContext(sUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    List adminList = null;
                    Web w = clientContext.Web;
                    clientContext.Load(w);
                    clientContext.Load(w.ParentWeb);
                    
                    if (strProgramManagerSite == "")
                    {
                        adminList = w.Lists.GetByTitle("AdminGroup");
                    }
                    else
                    {
                        adminList = w.Lists.GetByTitle("AdminSiteData");
                    }

                    List oList = adminList;
                    ListItemCollection collListItem = oList.GetItems(CamlQuery.CreateAllItemsQuery());
                    clientContext.Load(oList);
                    
                    
                    if (strProgramManagerSite == "")
                    {
                        clientContext.Load(collListItem, items => items.Include(
                        item => item["PracticeName"],
                        item => item["PracticeTIN"],
                        item => item["ProgramParticipation"],
                        item => item["KCEArea"]));
                    }
                    else
                    {
                        clientContext.Load(collListItem, items => items.Include(
                        item => item["PracticeName"],
                        item => item["PracticeTIN"],
                        item => item["ProgramParticipation"],
                        item => item["KCEArea"],
                        item => item["PracticeManagerSite"]));
                    }
                    
                    clientContext.ExecuteQuery();

                    foreach (ListItem listItem in collListItem)
                    {
                        Practice practice = new Practice();
                        practice.Name = listItem["PracticeName"].ToString();
                        practice.SiteID = listItem["PracticeTIN"].ToString();
                        practice.ProgramParticipation = listItem["ProgramParticipation"].ToString();
                        practice.CKCCArea = listItem["KCEArea"] == null ? string.Empty : listItem["KCEArea"].ToString();
                        practice.PMGroup = String.IsNullOrEmpty(strProgramManagerSite) ? String.Empty : listItem["PracticeManagerSite"].ToString();
                        AllAdminList.Add(practice);
                    }
                    return AllAdminList;
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("CheckAdminList", ex.Message, "Error", "");
                    return null;
                }
            }

        }
        public List<Practice> GetAdminListItems(string pracUrl, string programManagerRef = "")
        {
            SiteListUtility siteListUtility = new SiteListUtility();
            List<Practice> adminListItems;
            try
            {
                string wUrl = pracUrl.Substring(0, pracUrl.LastIndexOf('/'));

                string pmUrl = wUrl.Substring(0, wUrl.LastIndexOf('/'));
                string adminUrl = pmUrl.Substring(0, pmUrl.LastIndexOf('/'));

                if (programManagerRef == "")
                {
                    adminListItems = siteListUtility.GetAdminList(pmUrl); 
                }
                else
                {
                    logger.Information("Program Manager Ref: " + programManagerRef);
                    adminListItems = siteListUtility.GetAdminList(adminUrl, programManagerRef);
                }

                logger.Information("Admin List Count: " + adminListItems.Count());

                return adminListItems;
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetAdminListItems", ex.Message, "Error", "");
                return null;
            }
        }
        public bool CheckAdminList(string sUrl, string siteId, string strProgramManagerSite = "")
        {
            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();
            using (ClientContext ctx = new ClientContext(sUrl))
            {
                ctx.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                Web web = ctx.Web;
                ctx.Load(web.ParentWeb);
                ctx.ExecuteQuery();
                string rootWebUrl = siteInfoUtility.GetRootSite(sUrl);
                sUrl = siteInfoUtility.GetRootSite(sUrl) + web.ParentWeb.ServerRelativeUrl;
            }

            using (ClientContext clientContext = new ClientContext(sUrl))
            {

                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                List adminList = null;
                Web w = clientContext.Web;
                clientContext.Load(w);
                clientContext.ExecuteQuery();

                try
                {
                    if (strProgramManagerSite == "")
                    {
                        adminList = w.Lists.GetByTitle("AdminGroup");
                    }
                    else
                    {
                        adminList = w.Lists.GetByTitle("AdminSiteData");
                    }

                    List oList = adminList;
                    clientContext.Load(oList);
                    clientContext.ExecuteQuery();

                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = @"<View><Query><Where><Eq>" +
                                            "<FieldRef Name='PracticeTIN' />" +
                                            "<Value Type='Text'>" + siteId + "</Value>" +
                                        "</Eq></Where></Query></View>";

                    ListItemCollection collListItem = oList.GetItems(camlQuery);
                    clientContext.Load(collListItem);
                    clientContext.ExecuteQuery();

                    if ((collListItem.Count > 0) == false)
                        return false;

                    foreach (ListItem listItem in collListItem)
                    {
                        if (listItem["PracticeTIN"].Equals(siteId))
                            return true;
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("CheckAdminList", ex.Message, "Error", "");
                    return false;
                }
            }

            return false;
        }
        public void List_AddAdminListItem(string sUrl, Practice site, string strProgramManagerSite = "")
        {
            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();
            using (ClientContext ctx = new ClientContext(sUrl))
            {
                ctx.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                Web web = ctx.Web;
                ctx.Load(web.ParentWeb);
                ctx.ExecuteQuery();
                string rootWebUrl = siteInfoUtility.GetRootSite(sUrl);
                //sUrl = siteInfoUtility.GetRootSite(sUrl) + web.ParentWeb.ServerRelativeUrl;
            }

            try
            {
                using (ClientContext clientContext = new ClientContext(sUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    List adminList = null;
                    Web w = clientContext.Web;
                    clientContext.Load(w);
                    clientContext.ExecuteQuery();

                    if (strProgramManagerSite == "")
                    {
                        adminList = clientContext.Web.Lists.GetByTitle("AdminGroup");
                    }
                    else
                    {
                        adminList = clientContext.Web.Lists.GetByTitle("AdminSiteData");
                    }


                    ListItemCreationInformation oListItemCreationInformation = new ListItemCreationInformation();
                    ListItem oItem = adminList.AddItem(oListItemCreationInformation);

                    FieldUrlValue url = new FieldUrlValue();
                    url.Url = site.NewSiteUrl;
                    url.Description = site.Name;

                    oItem["URL"] = url;
                    oItem["PracticeName"] = site.Name;
                    oItem["PracticeTIN"] = site.SiteID;
                    oItem["ProgramParticipation"] = site.ProgramParticipation;
                    oItem["KCEArea"] = site.CKCCArea;
                    if (strProgramManagerSite != "")
                    {
                        oItem["PracticeManagerSite"] = strProgramManagerSite.Split('/')[strProgramManagerSite.Split('/').Length - 1];
                    }
                    oItem.Update();
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("List_AddItemsAdminGroup", ex.Message, "Error", "");
            }

        }
        public void List_UpdateAdminListItem(string sUrl, Practice site, string strProgramManagerSite = "")
        {
            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();
            SiteLogUtility siteLogUtility = new SiteLogUtility();
            //string sUrl = string.Empty;

            using (ClientContext ctx = new ClientContext(sUrl))
            {
                ctx.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                Web web = ctx.Web;
                ctx.Load(web.ParentWeb);
                ctx.ExecuteQuery();
                string rootWebUrl = siteInfoUtility.GetRootSite(sUrl);
                //sUrl = siteInfoUtility.GetRootSite(sUrl) + web.ParentWeb.ServerRelativeUrl;
            }

            try
            {
                using (ClientContext clientContext = new ClientContext(sUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    List adminList = null;
                    Web w = clientContext.Web;
                    clientContext.Load(w);
                    clientContext.ExecuteQuery();

                    if (strProgramManagerSite == "")
                    {
                        adminList = clientContext.Web.Lists.GetByTitle("AdminGroup");
                    }
                    else
                    {
                        adminList = clientContext.Web.Lists.GetByTitle("AdminSiteData");
                    }

                    List oList = adminList;
                    clientContext.Load(oList);
                    clientContext.ExecuteQuery();

                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = @"<View><Query><Where><Eq>" +
                                            "<FieldRef Name='PracticeTIN' />" +
                                            "<Value Type='Text'>" + site.SiteID + "</Value>" +
                                        "</Eq></Where></Query></View>";

                    ListItemCollection collListItem = oList.GetItems(camlQuery);
                    clientContext.Load(collListItem);
                    clientContext.ExecuteQuery();

                    foreach (ListItem listItem in collListItem)
                    {
                        if (listItem["PracticeTIN"].Equals(site.SiteID))
                        {
                            siteLogUtility.LoggerInfo_Entry("ProgramParticipation BEFORE: " + listItem["ProgramParticipation"]);
                            FieldUrlValue url = new FieldUrlValue();
                            url.Url = site.NewSiteUrl;
                            url.Description = site.Name;

                            listItem["URL"] = url;
                            listItem["PracticeName"] = site.Name;
                            listItem["PracticeTIN"] = site.SiteID;
                            listItem["ProgramParticipation"] = site.ProgramParticipation;
                            listItem["KCEArea"] = site.CKCCArea;
                            if (strProgramManagerSite != "")
                            {
                                listItem["PracticeManagerSite"] = strProgramManagerSite.Split('/')[strProgramManagerSite.Split('/').Length - 1];
                            }
                            siteLogUtility.LoggerInfo_Entry("ProgramParticipation AFTER: " + listItem["ProgramParticipation"]);

                            listItem.Update();
                            clientContext.ExecuteQuery();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("List_UpdateAdminListItem", ex.Message, "Error", "");
            }

        }
        public static bool ConfigureBenefitEnhancementPage(string webUrl, Practice pracSite)
        {
            SiteLogUtility.Log_Entry("   ConfigureBenefitEnhancement - In Progress...");
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

                        if (pracSite.IsCKCC == true)
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
                        SiteLogUtility.CreateLogEntry("ConfigureBenefitEnhancementPage Update", ex.Message, "Error", "");
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("ConfigureBenefitEnhancementPage", ex.Message, "Error", "");
                    outcome = false;
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
            return outcome;
        }
        public static bool ConfigureQualityPage(string webUrl, Practice pracSite)
        {
            SiteLogUtility.Log_Entry("   ConfigureQualityPage - In Progress...");
            SiteListUtility slu = new SiteListUtility();
            bool outcome = false;
            string clink = string.Empty;
            string scntPx = "777";

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
                        olimitedwebpartmanager.AddWebPart(wpd1.WebPart, "CenterColumn", 1);

                        if (pracSite.IsIWH)
                        {
                            WebPartDefinition wpd5 = olimitedwebpartmanager.ImportWebPart(webPartXML(web.Url + "/" + slu.listNameQualityIwh + "/Forms/PageViewer.aspx"));
                            wpd5.WebPart.Title = slu.webpartQualityIwh;
                            olimitedwebpartmanager.AddWebPart(wpd5.WebPart, "CenterColumn", 1);
                        }
                        if (pracSite.IsCKCC)
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
                        SiteLogUtility.CreateLogEntry("ConfigureQualityPage Update", ex.Message, "Error", "");
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("ConfigureQualityPage", ex.Message, "Error", "");
                    outcome = false;
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
            return outcome;
        }
        public static bool ConfigurePayorEducationPage(string webUrl, Practice pracSite)
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

                        if (pracSite.IsIWH)
                        {
                            WebPartDefinition wpd5 = olimitedwebpartmanager.ImportWebPart(webPartXML(web.Url + "/" + slu.listNamePayorEducationIwh + "/Forms/PageViewer.aspx"));
                            wpd5.WebPart.Title = slu.webpartPayorEducationIwh;
                            olimitedwebpartmanager.AddWebPart(wpd5.WebPart, "CenterLeftColumn", 1);
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
                        SiteLogUtility.CreateLogEntry("ConfigurePayorEducationPage Update", ex.Message, "Error", "");
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("ConfigurePayorEducationPage", ex.Message, "Error", "");
                    outcome = false;
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
            return outcome;
        }
        public static bool ConfigureDocumentExchangePage(string webUrl, Practice pracSite)
        {
            SiteLogUtility.Log_Entry("ConfigureDocumentExchangePage - In Progress...");
            SiteListUtility slu = new SiteListUtility();
            bool outcome = false;
            string clink = string.Empty;
            string scntPx = "777";

            using (ClientContext clientContext = new ClientContext(webUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                Web web = clientContext.Web;
                clientContext.Load(web, w => w.ServerRelativeUrl);
                clientContext.ExecuteQuery();
                var file = clientContext.Web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + "/Pages/" + slu.pageNameDataExchange + ".aspx");
                file.CheckOut();
                try
                {
                    clientContext.Load(file);
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    try
                    {
                        LimitedWebPartManager olimitedwebpartmanager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);

                        WebPartDefinition wpd3 = olimitedwebpartmanager.ImportWebPart(contentEditorXML("SupportStyles", "0px", "0px", web.Url + "/SiteAssets/smlcal.js"));
                        wpd3.WebPart.Title = "SupportStyles";
                        olimitedwebpartmanager.AddWebPart(wpd3.WebPart, "Footer", 1);

                        WebPartDefinition wpd2 = olimitedwebpartmanager.ImportWebPart(contentEditorXML("Easy Button", "65px", "300px", web.Url + "/SiteAssets/cePrac_EasyDownload.html"));
                        wpd2.WebPart.Title = "Esay Button";
                        olimitedwebpartmanager.AddWebPart(wpd2.WebPart, "CenterColumn", 1);

                        WebPartDefinition wpd1 = olimitedwebpartmanager.ImportWebPart(contentEditorXML("Multi Tab", "600px", "700px", web.Url + "/SiteAssets/PracticeSiteTemplate_MultiTab.js"));
                        wpd1.WebPart.Title = "Multi Tab";
                        olimitedwebpartmanager.AddWebPart(wpd1.WebPart, "CenterColumn", 1);

                        if (pracSite.IsIWH)
                        {
                            WebPartDefinition wpd5 = olimitedwebpartmanager.ImportWebPart(webPartXML(web.Url + "/" + slu.listNameDataExchangeIwh + "/Forms/PageViewer.aspx"));
                            wpd5.WebPart.Title = slu.webpartDataExchangeIwh;
                            olimitedwebpartmanager.AddWebPart(wpd5.WebPart, "CenterColumn", 1);
                        }

                        if (pracSite.IsCKCC)
                        {
                            WebPartDefinition wpd6 = olimitedwebpartmanager.ImportWebPart(webPartXML(web.Url + "/" + slu.listNameDataExchangeCkcc + "/Forms/PageViewer.aspx"));
                            wpd6.WebPart.Title = slu.webpartDataExchangeCkcc;
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
                        SiteLogUtility.CreateLogEntry("ConfigureDataExchangePage Update", ex.Message, "Error", "");
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("ConfigureDataExchangePage", ex.Message, "Error", "");
                    outcome = false;
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
            return outcome;
        }
        public static bool ConfigureRiskAdjustmentPage(string webUrl, Practice pracSite)
        {
            SiteLogUtility.Log_Entry("ConfigureRiskAdjustmentPage - In Progress...");
            SiteListUtility slu = new SiteListUtility();
            bool outcome = false;
            string clink = string.Empty;
            string scntPx = "777";

            using (ClientContext clientContext = new ClientContext(webUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                Web web = clientContext.Web;
                clientContext.Load(web, w => w.ServerRelativeUrl);
                clientContext.ExecuteQuery();
                var file = clientContext.Web.GetFileByServerRelativeUrl(web.ServerRelativeUrl + "/Pages/" + slu.pageNameRiskAdjustment + ".aspx");
                file.CheckOut();
                try
                {
                    clientContext.Load(file);
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();
                    try
                    {
                        LimitedWebPartManager olimitedwebpartmanager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);

                        WebPartDefinition wpd3 = olimitedwebpartmanager.ImportWebPart(contentEditorXML("SupportStyles", "0px", "0px", web.Url + "/SiteAssets/smlcal.js"));
                        wpd3.WebPart.Title = "SupportStyles";
                        olimitedwebpartmanager.AddWebPart(wpd3.WebPart, "Footer", 1);

                        WebPartDefinition wpd4 = olimitedwebpartmanager.ImportWebPart(contentEditorXML("Multi Tab", "600px", "700px", web.Url + "/SiteAssets/RiskAdjustment.js"));
                        wpd4.WebPart.Title = "Multi Tab";
                        olimitedwebpartmanager.AddWebPart(wpd4.WebPart, "CenterColumn", 1);

                        //WebPartDefinition wpd1 = olimitedwebpartmanager.ImportWebPart(contentEditorXML("Multi Tab", "600px", "700px", web.Url + "/SiteAssets/RiskAdjustment.js"));
                        //wpd1.WebPart.Title = "Multi Tab";
                        //olimitedwebpartmanager.AddWebPart(wpd1.WebPart, "CenterLeftColumn", 1);

                        if (pracSite.IsIWH)
                        {
                            WebPartDefinition wpd5 = olimitedwebpartmanager.ImportWebPart(webPartXML(web.Url + "/" + slu.listNameRiskAdjustmentIwh + "/Forms/PageViewer.aspx"));
                            wpd5.WebPart.Title = slu.webpartRiskAdjustmentIwh;
                            olimitedwebpartmanager.AddWebPart(wpd5.WebPart, "CenterColumn", 1);
                        }

                        if (pracSite.IsCKCC)
                        {
                            WebPartDefinition wpd6 = olimitedwebpartmanager.ImportWebPart(webPartXML(web.Url + "/" + slu.listNameRiskAdjustmentCkcc + "/Forms/PageViewer.aspx"));
                            wpd6.WebPart.Title = slu.webpartRiskAdjustmentCkcc;
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
                        SiteLogUtility.CreateLogEntry("ConfigureRiskAdjustmentPage Update", ex.Message, "Error", "");
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("ConfigureRiskAdjustmentPage", ex.Message, "Error", "");
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
        public static void ListFunction1()
        {
            Console.WriteLine("ListFunction 1");
        }
        public static void ListFunction2()
        {
            Console.WriteLine("ListFunction 2");
        }
    }
}

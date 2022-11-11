using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace SiteUtility
{
    public class SiteListUtility
    {
        // Benefit Enhancement...
        public string pageNameBenefitEnhancement = "CkccKceResources";
        public string pageTitleBenefitEnhancement = "CKCC/KCE Resources";

        public string listNameBenefitEnhancementCkcc = "BenefitEnhancementCkcc";
        public string listTitleBenefitEnhancementCkcc = "Benefit Enhancement Ckcc";
        public string listFolder1BenefitEnhancementCkcc = "Benefit Enhancement Training";
        public string listFolder2BenefitEnhancementCkcc = "Operating Guides and Documents";
        public string tabTitleBenefitEnhancementCkcc = "CKCC/KCE";
        public string webpartBenefitEnhancementCkcc = "BenefitEnhancement_Ckcc";

        //public string pageNameBenefitEnhancement = "BenefitEnhancement";
        //public string pageTitleBenefitEnhancement = "Benefit Enhancement";

        //public string listNameBenefitEnhancementCkcc = "BenefitEnhancementCkcc";
        //public string listTitleBenefitEnhancementCkcc = "Benefit Enhancement Ckcc";
        //public string listFolder1BenefitEnhancementCkcc = "Benefit Enhancement Training";
        //public string tabTitleBenefitEnhancementCkcc = "CKCC/KCE";
        //public string webpartBenefitEnhancementCkcc = "BenefitEnhancement_Ckcc";

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

        //Program Participation List items...
        public const string progpart_PayorEnrollment = "Payor Enrollment";
        public const string progpart_CkccKceResources = "CKCC/KCE Resources";
        public const string progpart_PayorProgeducation = "Payor Program Education Resources";
        public const string progpart_PatientStatusUpdates = "Patient Status Updates";
        public const string progpart_CkccKceEngagement = "CKCC/KCE Engagement";

        static ILogger logger = Log.ForContext<SiteListUtility>();



        public static void ListFunction1()
        {
            Console.WriteLine("ListFunction 1");
        }
        public static void ListFunction2()
        {
            Console.WriteLine("ListFunction 2");
        }

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
                SiteLogUtility.CreateLogEntry("CreateListColumn", ex.Message, "Error", strWebURL);
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
    }
}

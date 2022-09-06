using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
    }
}

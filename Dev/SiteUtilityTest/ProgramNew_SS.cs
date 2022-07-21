using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using SiteUtility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SiteUtilityTest
{
    public class ProgramNew_SS
    {
        string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
        string strPortalSiteURL = ConfigurationManager.AppSettings["SP_SiteUrl"];
        public void InitiateProg()
        {
            string sAdminListName = ConfigurationManager.AppSettings["AdminRootListName"];
            string releaseName = "SiteUtilityTest";
            SiteRootAdminList objRootSite = new SiteRootAdminList();
            SiteDeleteUtility objDeleteSite = new SiteDeleteUtility();
            SiteFilesUtility objFilesSite = new SiteFilesUtility();
            SiteListUtility objListUtility = new SiteListUtility();

            SiteLogUtility.InitLogFile(releaseName, rootUrl, strPortalSiteURL);

            using (ClientContext clientContext = new ClientContext(strPortalSiteURL))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                Console.WriteLine("=============Release Starts=============");
                try
                {
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext);
                    int intLoop = 0;
                    Console.WriteLine("=======================================");
                    Console.WriteLine("***************************************");
                    Console.WriteLine("=======================================");
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        if (pm.ProgramManager != "03" && pm.ProgramManager != "05")
                        {
                            foreach (PracticeSite psite in pm.PracticeSiteCollection)
                            {
                                intLoop++;
                                //setupMedicalAlertDeployment(psite.URL);
                                //setupHospitalizationAlertDeployment(psite.URL);
                                Console.WriteLine(intLoop + ". " + psite.Name + "  ..  Med & Hosp Alert Deployed.");
                                Console.WriteLine("=======================================");
                            }
                        }
                    }
                    Console.WriteLine("=======================================");
                    Console.WriteLine("=======================================");
                    Console.WriteLine("=======================================");
                    Console.WriteLine("=======================================");
                    Console.WriteLine("Dumping Hosp Alert Data in SharePoint List");
                    Console.WriteLine("=======================================");

                    string sHsptlAlertListName = "HospitalizationAlert";
                    List<PracticeMap> PracticesMap = new List<PracticeMap>();
                    PracticesMap = CreatePracticeMap_with_RosterData();
                    insertHospitalizeAlertDataSP(PracticesMap, sHsptlAlertListName);

                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", strPortalSiteURL);
                }

                Console.WriteLine("=======================================");
                Console.WriteLine("3. Maintenance Tasks Complete - Complete");
                Console.WriteLine("=============Release Ends=============");
                SiteLogUtility.CreateLogEntry("PracticeSite-Maint - SiteUtilityTest", "=============Release Ends=============", "Log", strPortalSiteURL);
            }
        }

        public void changeColumnToRichText(string strURL)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(strURL))
                {
                    clientContext.Credentials = new NetworkCredential("spAdmin_Dev", "$5ApjXy9", "Medspring");
                    List targetList = clientContext.Web.Lists.GetByTitle("HospitalizationAlert");

                    Field obField = targetList.Fields.GetByTitle("Diagnosis");
                    obField.TypeAsString = "Note";
                    obField.Update();
                    clientContext.Load(obField);
                    clientContext.ExecuteQuery();

                    //// Get field from list using internal name or display name
                    //Field oField = targetList.Fields.GetByInternalNameOrTitle("FaxType");
                    //Field oFields = targetList.Fields.GetByInternalNameOrTitle("MemberStatus");
                    //oField.DeleteObject();
                    //oFields.DeleteObject();

                    //clientContext.ExecuteQuery();

                    //List olist = clientContext.Web.Lists.GetByTitle("HospitalizationAlert");

                    //olist.DeleteObject();
                    //clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void DeleteColumnsAndList(string strURL)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(strURL))
                {
                    clientContext.Credentials = new NetworkCredential("spAdmin_Dev", "$5ApjXy9", "Medspring");
                    List targetList = clientContext.Web.Lists.GetByTitle("CarePlan");

                    // Get field from list using internal name or display name
                    Field oField = targetList.Fields.GetByInternalNameOrTitle("FaxType");
                    Field oFields = targetList.Fields.GetByInternalNameOrTitle("MemberStatus");
                    oField.DeleteObject();
                    oFields.DeleteObject();

                    clientContext.ExecuteQuery();

                    List olist = clientContext.Web.Lists.GetByTitle("HospitalizationAlert");

                    olist.DeleteObject();
                    clientContext.ExecuteQuery();
                }
            }
            catch(Exception ex)
            {

            }
           
        }

        public void setupMedicalAlertDeployment(string strURL)
        {
            try
            {
                //createCarePlanListColumns(strURL);
                uploadMedAlertRelatedHTMLFile(strURL);
                increaseMedHospAlertWPHeight(strURL, "/Pages/MedicationAlerts.aspx", "Medication Alerts", "/SiteAssets/cePrac_MedAlertDataTable.html");
                modifyMedicalAlertNavigationNode(strURL, "Medication Alert Coming Soon", "Medication Alerts", "/Pages/MedicationAlerts.aspx");
            }
            catch(Exception ex)
            {

            }            
        }

        public void createCarePlanListColumns(string strURL)
        {
            try
            {
                SiteListUtility objListUtility = new SiteListUtility();
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='FaxType' Name='FaxType' />", "CarePlan", strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='MemberStatus' Name='MemberStatus' />", "CarePlan", strURL);
            }
            catch(Exception ex)
            {

            }            
        }

        public void setupHospitalizationAlertDeployment(string strURL)
        {
            try
            {
                string sHsptlAlertListName = "HospitalizationAlert";

                createHospitalizeAlertList(strURL, sHsptlAlertListName);
                uploadHospAlertRelatedHTMLfile(strURL);
                increaseMedHospAlertWPHeight(strURL, "/Pages/HospitalAlerts.aspx", "Hospitalization Alerts", "/SiteAssets/cePrac_HospAlertDataTable.html");
                modifyMedicalAlertNavigationNode(strURL, "Hospitalization Alerts Coming Soon", "Hospitalization Alerts", "/Pages/HospitalAlerts.aspx");
            }
            catch (Exception ex)
            {

            }
        }

        public void createHospitalizeAlertList(string strURL, string sHsptlAlertListName)
        {
            try
            {
                SiteListUtility objListUtility = new SiteListUtility();
                objListUtility.CreateList(sHsptlAlertListName, strURL, (int)ListTemplateType.GenericList);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='GroupID' Name='GroupID' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='PracticeTIN' Name='PracticeTIN' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='MEMBER_MASTER_ROW_ID' Name='MEMBER_MASTER_ROW_ID' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='FirstName' Name='FirstName' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='LastName' Name='LastName' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='DateOfBirth' Name='DateOfBirth' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='DischargeDate' Name='DischargeDate' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='DischargeFacility' Name='DischargeFacility' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='Diagnosis' Name='Diagnosis' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='FacilityType' Name='FacilityType' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='Setting' Name='Setting' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='Nephrologist' Name='Nephrologist' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='DialysisFacility' Name='DialysisFacility' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='_File' Name='_File' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='isIWH' Name='isIWH' />", sHsptlAlertListName, strURL);
                objListUtility.CreateListColumn("<Field Type='Text' DisplayName='isCKCC' Name='isCKCC' />", sHsptlAlertListName, strURL);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("PracticeSite-Maint - createHospitalizeAlertList", ex.Message, "Error", strPortalSiteURL);
            }            
        }

        public void uploadHospAlertRelatedHTMLfile(string strURL)
        {
            try
            {
                SiteFilesUtility objFilesSite = new SiteFilesUtility();
                objFilesSite.DocumentUpload(strURL, @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\cePrac_HospitalAlerts.html", "SiteAssets");
                objFilesSite.DocumentUpload(strURL, @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\cePrac_HospAlertDataTable.html", "SiteAssets");
                objFilesSite.DocumentUpload(strURL, @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Images\HospitalAlerts.jpg", "SiteAssets/Img");
            }
            catch (Exception ex)
            {

            }            
        }

        public void uploadMedAlertRelatedHTMLFile(string strURL)
        {
            try
            {
                SiteFilesUtility objFilesSite = new SiteFilesUtility();
                objFilesSite.DocumentUpload(strURL, @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\cePrac_MedAlertDataTable.html", "SiteAssets");
                objFilesSite.DocumentUpload(strURL, @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\cePrac_MedicationAlerts.html", "SiteAssets");
                objFilesSite.DocumentUpload(strURL, @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Images\MedicationAlerts.jpg", "SiteAssets/Img");
            }
            catch (Exception ex)
            {

            }            
        }

        public void increaseMedHospAlertWPHeight(string webURL, string strPageRelativeUrl, string strTitle, string strContentLink)
        {
            var pageRelativeUrl = strPageRelativeUrl;
            using (ClientContext clientContext = new ClientContext(webURL))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential("spAdmin_Dev", "$5ApjXy9", "Medspring");
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
                        if (obj.WebPart.Title == "Coming Soon...")
                        {
                            obj.WebPart.Properties["Height"] = "666px";
                            obj.WebPart.Properties["Width"] = "1200px";
                            obj.WebPart.Properties["Title"] = strTitle;
                            obj.WebPart.Properties["ContentLink"] = webURL + strContentLink;
                            obj.SaveWebPartChanges();
                            clientContext.ExecuteQuery();
                            break;
                        }
                    }

                    file.CheckIn("increaseMedicationAlertsWPHeight webpart", CheckinType.MajorCheckIn);
                    file.Publish("increaseMedicationAlertsWPHeight webpart");
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    //SiteLogUtility.CreateLogEntry("increaseCarePlansWPHeight", ex.Message, "Error", strPortalSiteURL);
                }
            }
        }

        public void modifyMedicalAlertNavigationNode(string webUrl, string strOldTitle, string strTitle, string strNodeURL)
        {
            try
            {
                bool nodeUpdate = false;
                using (ClientContext clientContext = new ClientContext(webUrl))
                {
                    clientContext.Credentials = new NetworkCredential("spAdmin_Dev", "$5ApjXy9", "Medspring");
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

                            foreach (var childObj in objNav.Children)
                            {
                                if (childObj.Title == strOldTitle)
                                {
                                    childObj.Title = strTitle;
                                    childObj.Url = webUrl + strNodeURL;
                                    childObj.Update();
                                    clientContext.ExecuteQuery();
                                    nodeUpdate = true;
                                    break;
                                }
                            }

                            if (nodeUpdate) break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //SiteLogUtility.CreateLogEntry("addSWReferralNavigationNode", ex.Message, "Error", strPortalSiteURL);
            }
        }

        public void insertHospitalizeAlertDataSP(List<PracticeMap> objPracticesMap, string sHsptlAlertListName)
        {
            try
            {
                string siteURL = "";
                for (int intLoop = 0; intLoop < objPracticesMap.Count; intLoop++)
                {
                    siteURL = strPortalSiteURL + "/" + objPracticesMap[intLoop].GroupID + "/" + objPracticesMap[intLoop].SiteID;
                    siteURL = siteURL + "/";

                    Console.WriteLine(objPracticesMap[intLoop].GroupID + ". " + siteURL);
                    Console.WriteLine("=======================================");

                    using (ClientContext clientContext = new ClientContext(siteURL))
                    {
                        clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                        Web w = clientContext.Web;
                        clientContext.Load(w);
                        clientContext.ExecuteQuery();

                        List oList = clientContext.Web.Lists.GetByTitle(sHsptlAlertListName);
                        ListItemCreationInformation oListItemCreationInformation = new ListItemCreationInformation();

                        for (int intArray = 0; intArray < objPracticesMap[intLoop].RosterDataList.Count; intArray++)
                        {
                            //checking if item already exists in Hospitalization Alert list.
                            if (checkIfHospitalizationRecordExists(siteURL, sHsptlAlertListName, 
                                                                    objPracticesMap[intLoop].RosterDataList[intArray].MEMBER_MASTER_ROW_ID.ToString(), 
                                                                    objPracticesMap[intLoop].RosterDataList[intArray].DischargeDate.ToString(), 
                                                                    objPracticesMap[intLoop].RosterDataList[intArray].Setting.ToString()))
                            {
                                continue;
                            }
                            ListItem oItem = oList.AddItem(oListItemCreationInformation);
                            if(objPracticesMap[intLoop].RosterDataList[intArray].GroupID.ToString()!="")
                                oItem["GroupID"] = objPracticesMap[intLoop].RosterDataList[intArray].GroupID.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray].PracticeTIN.ToString() != "")
                                oItem["PracticeTIN"] = objPracticesMap[intLoop].RosterDataList[intArray].PracticeTIN.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray].MEMBER_MASTER_ROW_ID.ToString() != "")
                                oItem["MEMBER_MASTER_ROW_ID"] = objPracticesMap[intLoop].RosterDataList[intArray].MEMBER_MASTER_ROW_ID.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray].FirstName.ToString() != "")
                                oItem["FirstName"] = objPracticesMap[intLoop].RosterDataList[intArray].FirstName.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray].LastName.ToString() != "")
                                oItem["LastName"] = objPracticesMap[intLoop].RosterDataList[intArray].LastName.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray].DateOfBirth.ToString() != "")
                                oItem["DateOfBirth"] = objPracticesMap[intLoop].RosterDataList[intArray].DateOfBirth.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray].DischargeDate.ToString() != "")
                                oItem["DischargeDate"] = objPracticesMap[intLoop].RosterDataList[intArray].DischargeDate.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray].DischargeFacility.ToString() != "")
                                oItem["DischargeFacility"] = objPracticesMap[intLoop].RosterDataList[intArray].DischargeFacility.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray].Diagnosis.ToString() != "")
                                oItem["Diagnosis"] = objPracticesMap[intLoop].RosterDataList[intArray].Diagnosis.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray].FacilityType.ToString() != "")
                                oItem["FacilityType"] = objPracticesMap[intLoop].RosterDataList[intArray].FacilityType.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray].Setting.ToString() != "")
                                oItem["Setting"] = objPracticesMap[intLoop].RosterDataList[intArray].Setting.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray].Nephrologist.ToString() != "")
                                oItem["Nephrologist"] = objPracticesMap[intLoop].RosterDataList[intArray].Nephrologist.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray].DialysisFacility.ToString() != "")
                                oItem["DialysisFacility"] = objPracticesMap[intLoop].RosterDataList[intArray].DialysisFacility.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray]._File.ToString() != "")
                                oItem["_File"] = objPracticesMap[intLoop].RosterDataList[intArray]._File.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray].isIWH.ToString() != "")
                                oItem["isIWH"] = objPracticesMap[intLoop].RosterDataList[intArray].isIWH.ToString();
                            if (objPracticesMap[intLoop].RosterDataList[intArray].isCKCC.ToString() != "")
                                oItem["isCKCC"] = objPracticesMap[intLoop].RosterDataList[intArray].isCKCC.ToString();
                            oItem.Update();
                        }
                        clientContext.ExecuteQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("insertHospitalizeAlertDataSP", ex.Message, "Error", strPortalSiteURL);
            }
        }

        public bool checkIfHospitalizationRecordExists(string siteURL,string strListName, string strMemberID, string strDischargeDate, string strSettings)
        {
            bool result = false;
            try
            {
                using (ClientContext clientContext = new ClientContext(siteURL))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                   

                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle(strListName);
                    clientContext.Load(web);
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();

                    CamlQuery query = new CamlQuery();

                    query.ViewXml = "<View><Query><Where>" +
                        "<And>" +
                        "<Eq><FieldRef Name = 'MEMBER_MASTER_ROW_ID'/><Value Type = 'Text'>" + strMemberID + "</Value></Eq>" +
                        "<And>" +
                        "<Eq><FieldRef Name = 'Setting'/><Value Type = 'Text'>" + strSettings + "</Value></Eq>" +
                        "<Eq><FieldRef Name = 'DischargeDate'/><Value Type = 'Text'>" + strDischargeDate + "</Value></Eq>" +
                        "</And>" +
                        "</And></Where></Query></View>";

                    ListItemCollection items = list.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    if (items.Count > 0) {
                        result = true;
                    }                    
                }
            }
            catch(Exception ex)
            {
                SiteLogUtility.CreateLogEntry("checkIfHospitalizationRecordExists", ex.Message, "Error", strPortalSiteURL);
            }
            return result;
        }

        public List<PracticeMap> CreatePracticeMap_with_RosterData()
        {
            List<PracticeMap> PracMap = new List<PracticeMap>();
            List<RosterData> rosterList = new List<RosterData>();

            try
            {
                rosterList = SQL_Get_RosterData();

                if (rosterList == null)
                    return PracMap;

                foreach (RosterData rd in rosterList)
                {
                    if (!PracMap.Any(p => p.SiteID.Equals(rd.SiteID)))
                    {
                        PracticeMap pm = new PracticeMap();
                        pm.PracticeTIN = rd.PracticeTIN; //not used
                        pm.isIWH = rd.isIWH;
                        pm.isCKCC = rd.isCKCC;
                        pm.SiteID = rd.SiteID;

                        string PM_number = null;
                        if (rd.GroupID == "1" || rd.GroupID == "01")
                            PM_number = "PM01";
                        else if (rd.GroupID == "2" || rd.GroupID == "02")
                            PM_number = "PM02";
                        else if (rd.GroupID == "3" || rd.GroupID == "03")
                            PM_number = "PM03";
                        else if (rd.GroupID == "4" || rd.GroupID == "04")
                            PM_number = "PM04";
                        else if (rd.GroupID == "5" || rd.GroupID == "05")
                            PM_number = "PM05";
                        else if (rd.GroupID == "6" || rd.GroupID == "06")
                            PM_number = "PM06";
                        else if (rd.GroupID == "7" || rd.GroupID == "07")
                            PM_number = "PM07";
                        else if (rd.GroupID == "8" || rd.GroupID == "08")
                            PM_number = "PM08";
                        else if (rd.GroupID == "9" || rd.GroupID == "09")
                            PM_number = "PM09";
                        else if (rd.GroupID == "10")
                            PM_number = "PM10";
                        else if (rd.GroupID == "11")
                            PM_number = "PM11";
                        else
                            PM_number = "0"; //DoesPracticeSiteExist() is supposed to fail

                        pm.GroupID = PM_number;
                        PracMap.Add(pm);
                    }
                }

                foreach (PracticeMap pm in PracMap)
                {
                    pm.RosterDataList = rosterList.Where(rl => rl.SiteID.Equals(pm.SiteID)).ToList();
                }
            }
            catch (Exception ex)
            {
                

            }
            return PracMap;
        }
        
        public List<RosterData> SQL_Get_RosterData()
        {
            List<RosterData> RosterDataList = new List<RosterData>();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection())
                {
                    //sqlConn.ConnectionString = "Data Source=" + ConfigurationManager.AppSettings["SqlServer"] + "; Initial Catalog=" + ConfigurationManager.AppSettings["Database"] + "; Integrated Security=SSPI";
                    sqlConn.ConnectionString = "Data Source=VH2-SQL-01; Initial Catalog=HealthCloud_NightlyProd; Integrated Security=SSPI";

                    string query = @"SELECT * FROM [HealthCloud_NightlyProd].[Roster].[Hosp_Extract_ForSP_V]";

                    sqlConn.Open();
                    SqlCommand getQuery = new SqlCommand(query, sqlConn);
                    using (SqlDataReader reader = getQuery.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return null;

                        while (reader.Read())
                        {
                            RosterData rd = new RosterData();

                            if (!reader.IsDBNull(reader.GetOrdinal("GroupID")))
                                rd.GroupID = reader["GroupID"].ToString();
                            if (!reader.IsDBNull(reader.GetOrdinal("SiteID")))
                                rd.SiteID = reader["SiteID"].ToString();
                            if (!reader.IsDBNull(reader.GetOrdinal("PracticeTIN")))
                                rd.PracticeTIN = reader["PracticeTIN"].ToString();
                            if (!reader.IsDBNull(reader.GetOrdinal("MEMBER_MASTER_ROW_ID")))
                                rd.MEMBER_MASTER_ROW_ID = reader["MEMBER_MASTER_ROW_ID"].ToString();
                            if (!reader.IsDBNull(reader.GetOrdinal("FirstName")))
                                rd.FirstName = reader["FirstName"].ToString();
                            if (!reader.IsDBNull(reader.GetOrdinal("LastName")))
                                rd.LastName = reader["LastName"].ToString();
                            if (!reader.IsDBNull(reader.GetOrdinal("DischargeFacility")))
                                rd.DischargeFacility = reader["DischargeFacility"].ToString();
                            if (!reader.IsDBNull(reader.GetOrdinal("Diagnosis")))
                                rd.Diagnosis = reader["Diagnosis"].ToString();
                            if (!reader.IsDBNull(reader.GetOrdinal("FacilityType")))
                                rd.FacilityType = reader["FacilityType"].ToString();
                            if (!reader.IsDBNull(reader.GetOrdinal("Setting")))
                                rd.Setting = reader["Setting"].ToString();
                            if (!reader.IsDBNull(reader.GetOrdinal("Nephrologist")))
                                rd.Nephrologist = reader["Nephrologist"].ToString();
                            if (!reader.IsDBNull(reader.GetOrdinal("DialysisFacility")))
                                rd.DialysisFacility = reader["DialysisFacility"].ToString();
                            if (!reader.IsDBNull(reader.GetOrdinal("_File")))
                                rd._File = reader["_File"].ToString();

                            if (!reader.IsDBNull(reader.GetOrdinal("isIWH")))
                                rd.isIWH = Convert.ToInt32(reader["isIWH"]);
                            if (!reader.IsDBNull(reader.GetOrdinal("isCKCC")))
                                rd.isCKCC = Convert.ToInt32(reader["isCKCC"]);

                            DateTime DateOfBirth_DateTime = new DateTime();
                            if (!reader.IsDBNull(reader.GetOrdinal("DateOfBirth")))
                                DateOfBirth_DateTime = reader.GetDateTime(reader.GetOrdinal("DateOfBirth"));
                            if (DateOfBirth_DateTime > DateTime.MinValue && DateOfBirth_DateTime < DateTime.MaxValue)
                                rd.DateOfBirth = DateOfBirth_DateTime.ToString("yyyy-MM-dd");

                            DateTime DischargeDate_DateTime = new DateTime();
                            if (!reader.IsDBNull(reader.GetOrdinal("DischargeDate")))
                                DischargeDate_DateTime = reader.GetDateTime(reader.GetOrdinal("DischargeDate"));
                            if (DischargeDate_DateTime > DateTime.MinValue && DischargeDate_DateTime < DateTime.MaxValue)
                                rd.DischargeDate = DischargeDate_DateTime.ToString("yyyy-MM-dd");

                            if (
                                rd.GroupID == null &&
                                rd.SiteID == null &&
                                rd.MEMBER_MASTER_ROW_ID == null &&
                                rd.FirstName == null &&
                                rd.LastName == null &&
                                rd.DateOfBirth == null &&
                                rd.DischargeDate == null &&
                                rd.DischargeFacility == null &&
                                rd.Diagnosis == null &&
                                rd.FacilityType == null &&
                                rd.Setting == null &&
                                rd.Nephrologist == null &&
                                rd.DialysisFacility == null &&
                                rd._File == null &&
                                rd.PracticeTIN == null
                            )
                            {
                                //do nothing
                            }
                            else
                                RosterDataList.Add(rd);
                        }
                    }
                }
                //RosterDataList.OrderBy(data => Convert.ToInt32(data.MEMBER_MASTER_ROW_ID));
            }
            catch (Exception ex)
            {

            }
            return RosterDataList;
        }
    }

     public class PracticeMap
    {
        public string CensusFolderName;
        public string FileName;
        public string ExcelExtension;
        public int isIWH;
        public int isCKCC;
        public int Status_IWH;
        public int Status_CKCC;
        public int Status_Practice;

        public PracticeMap()
        {
            this.CensusFolderName = "Hospital Notifications";
            this.FileName = "Hospital_Notification_Extracts_" + DateTime.Now.ToString("yyyyMMdd");
            this.ExcelExtension = ".xlsx";
            this.isIWH = 0;
            this.isCKCC = 0;
            this.Status_IWH = 0;
            this.Status_CKCC = 0;
            this.Status_Practice = 0;
        }

        public List<RosterData> RosterDataList = new List<RosterData>();
        public string PracticeSite;
        public string SourceFilePath;
        public string spCensusFolderFileUrl;
        public string spArchiveFileUrl;
        public bool PracticeSite_Exists;
        public string GroupID;
        public string SiteID;
        public bool IWH_CensusFolder_Exists;
        public bool IWH_Archive_Exists;
        public bool CKCC_CensusFolder_Exists;
        public bool CKCC_Archive_Exists;
        public string PracticeTIN;
    }

    public class RosterData
    {
        public RosterData()
        { }

        public int isIWH;
        public int isCKCC;
        public string GroupID;
        public string SiteID;
        public string PracticeTIN;
        public string MEMBER_MASTER_ROW_ID;
        public string FirstName;
        public string LastName;
        public string DateOfBirth;
        public string DischargeDate;
        public string DischargeFacility;
        public string Diagnosis;
        public string FacilityType;
        public string Setting;
        public string Nephrologist;
        public string DialysisFacility;
        public string _File;
    }
}

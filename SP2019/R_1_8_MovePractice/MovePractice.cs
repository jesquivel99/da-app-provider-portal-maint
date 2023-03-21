using System;
using System.Collections.Generic;
using System.Linq;
using SiteUtility;
using Microsoft.SharePoint.Client;
using System.Net;
using System.Globalization;
using Serilog;

namespace R_JE_100_MovePractice
{
    public class MovePractice
    {
        static string dateHrMin = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
        const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
        static ILogger _logger = Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("Logs/maint" + dateHrMin + "_.log", rollingInterval: RollingInterval.Day, shared: true, outputTemplate: outputTemp1)
           .CreateLogger();
        static ILogger logger = _logger.ForContext<MovePractice>();
        const string LayoutsFolder = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
        public void InitiateProg()
        {
            SiteInfoUtility siteInfo = new SiteInfoUtility();

            List<Practice> practices = siteInfo.GetAllPractices();
            try
            {
                LoggerInfo_Entry("\n\n=============[ Deployment - Start]=============", true);

                if (practices != null && practices.Count > 0)
                {
                    foreach (Practice practice in practices)
                    {
                        SiteFilesUtility sfu = new SiteFilesUtility();

                        //HTML Update Files - Deploy 9/09...
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_CarePlansDataTable.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_HospAlertDataTable.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_HospitalAlerts.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_MedAlertDataTable.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_MedicationAlerts.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_ProgramParTableData.html");

                        /*
                        //HTML Files for Landing Page
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_ProgramParticipation.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_ProgramParTableData.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_Home.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_RiskAdjustmentResources.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_CareCoordination.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_InteractiveInsights.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_Quality.html");

                        //HTML Files for CareCoordination Page
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_CarePlans.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_CarePlans_Ckcc.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_HospitalAlerts.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_MedicationAlerts.html");

                        //HTML Files for CarePlans Page
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_CarePlansDataTable.html");

                        //HTML Files - Other
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_HospitalAlerts.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_MedicationAlerts.html");
                        sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_InteractiveInsights.html");

                        UpdateUrlRef(practice, "Program Participation");
                        SitePermissionUtility.RoleAssignment_AddPracUser(practice);
                        SitePermissionUtility.RoleAssignment_AddPracReadOnly(practice);
                        string adminUrl = LoadParentWeb("");
                        UpdateProgramParticipation("", practice, "");
                        UpdateProgramParticipation("", practice, "");
                        SyncSiteDescription(practice.NewSiteUrl, practice.Name);
                        //UpdateLogoUrl()
                        //UpdateProgramMgrUrl()
                        */
                    }
                }
                LoggerInfo_Entry("\n\n=============[ Deployment - End]=============", true);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
            }
            finally
            {
                LoggerInfo_Entry(SiteLogUtility.textLine0, true);
                SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@interwellhealth.com");
            }
            SiteLogUtility.Log_Entry("=============Release Ends=============", true);

        }
        public void InitiateProg(string siteID)
        {
            SiteInfoUtility siteInfo = new SiteInfoUtility();
            SiteFilesUtility sfu = new SiteFilesUtility();

            Practice practice = siteInfo.GetPracticeBySiteID(siteID);
            if (practice != null)
            {
                try
                {
                    Init_MoveUpdatePractice(practice, LayoutsFolder);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
                finally
                {
                    LoggerInfo_Entry(SiteLogUtility.textLine0);
                    SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@interwellhealth.com");
                }
                Log.CloseAndFlush();
            }
        }

        private void Init_MoveUpdatePractice(Practice practice, string layoutsFolder)
        {
            SiteLogUtility siteLogUtility = new SiteLogUtility();
            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();

            try
            {
                siteLogUtility.LoggerInfo_Entry("================ MoveUpdatePractice Deployment Started =====================", true);
                siteLogUtility.LoggerInfo_Entry("================ " + practice.Name + " =====================", true);

                Init_DocUpload(practice, layoutsFolder);
                Init_Permissions(practice, layoutsFolder);
                UpdateUrlRef(practice, "Program Participation");
                Init_DocUpload2(practice, layoutsFolder);

                string pmUrl = LoadParentWeb(practice.NewSiteUrl);
                SiteNavigateUtility.NavigationPracticeMntTop(practice.NewSiteUrl, pmUrl);

                siteInfoUtility.Init_UpdateAllProgramParticipation(practice);
                Init_UpdateProgramParticipation(practice, layoutsFolder);  // Program Participation Item Update - Img File...
                SiteInfoUtility.modifyWebPartProgramParticipation(practice.NewSiteUrl, practice);  // Resize...
                SiteFilesUtility.uploadMultiPartSupportingFilesAll(practice.NewSiteUrl, practice, layoutsFolder);  // JavaScript...

                SiteNavigateUtility.ClearQuickNavigationRecent(practice.NewSiteUrl);
                SiteNavigateUtility.RenameQuickNavigationNode(practice.NewSiteUrl, "Quality Coming Soon", "Quality");


                siteLogUtility.LoggerInfo_Entry("================ Deployment Completed =====================", true);

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                SiteLogUtility.CreateLogEntry("Init_MoveUpdatePractice", ex.Message, "Error", "");
            }
        }

        private void Init_Permissions(Practice practice, string layoutsFolder)
        {
            string pracUserGroup = "Prac_" + practice.TIN + "_User";
            string pracUserROGroup = "Prac_" + practice.TIN + "_ReadOnly";
            try
            {
                // Get all SP Groups from practice site...
                List<string> listWebGroups = SitePermissionUtility.GetWebGroups(practice);

                // Get PM group(s) and remove...
                List<string> pmSiteManagerWebGroups = SitePermissionUtility.GetPMGroupSiteManager(listWebGroups);
                foreach (string item in pmSiteManagerWebGroups)
                {
                    SitePermissionUtility.RoleAssignment_RemovePracUserGroup(item, "Practice Manager Site Permission Level", practice.NewSiteUrl);
                }
                // Get PM ReadOnly group(s) and remove...
                List<string> pmReadOnlyWebGroups = SitePermissionUtility.GetPMGroupReadOnly(listWebGroups);
                foreach (string item in pmReadOnlyWebGroups)
                {
                    SitePermissionUtility.RoleAssignment_RemovePracUserGroup(item, "Read", practice.NewSiteUrl);
                }

                // Get PracUser group(s) and remove...
                List<string> pracUserWebGroups = SitePermissionUtility.GetPracUser(listWebGroups);
                foreach (string item in pracUserWebGroups)
                {
                    SitePermissionUtility.RoleAssignment_RemovePracUserGroup(item, "Practice Site User Permission Level", practice.NewSiteUrl);
                }
                // Get PracUserReadOnly group(s) and remove...
                List<string> pracUserReadOnlyWebGroups = SitePermissionUtility.GetPracUserReadOnly(listWebGroups);
                foreach (string item in pracUserReadOnlyWebGroups)
                {
                    SitePermissionUtility.RoleAssignment_RemovePracUserGroup(item, "Read", practice.NewSiteUrl);
                }

                // Check if Group Exists else Create...
                if (SitePermissionUtility.CheckIfGroupExists(practice.NewSiteUrl, pracUserGroup) == false)
                {
                    SitePermissionUtility.CreateSiteGroup(practice.NewSiteUrl, pracUserGroup, "This is the Sites General Practice User group.");
                }
                if (SitePermissionUtility.CheckIfGroupExists(practice.NewSiteUrl, pracUserROGroup) == false)
                {
                    SitePermissionUtility.CreateSiteGroup(practice.NewSiteUrl, pracUserROGroup, "This is the Sites Practice ReadOnly User group.");
                }

                //Add correct PM and User SP Group Permissions...
                SitePermissionUtility.RoleAssignment_AddSiteManager(practice, practice.NewSiteUrl);
                SitePermissionUtility.RoleAssignment_AddSiteManagerReadOnly(practice, practice.NewSiteUrl);
                SitePermissionUtility.RoleAssignment_AddPracUser(practice);
                SitePermissionUtility.RoleAssignment_AddPracReadOnly(practice);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                SiteLogUtility.CreateLogEntry("Init_Permissions", ex.Message, "Error", "");
            }
        }

        private void Init_UpdateProgramParticipation(Practice practice, string layoutsFolder)
        {
            SiteLogUtility slu = new SiteLogUtility();

            try
            {
                if (practice.IsCKCC)
                {
                    // CKCC/KCE Resources...
                    slu.LoggerInfo_Entry("Adding to Program Participation: " + SitePublishUtility.titleCkccKceResources, true);
                    SiteFilesUtility.updateProgramParticipation(practice.NewSiteUrl, SitePublishUtility.titleCkccKceResources,
                            SitePublishUtility.pageCkccKceResources, layoutsFolder, SitePublishUtility.imgCkccKceResources);

                    // Patient Status Updates...
                    slu.LoggerInfo_Entry("Adding to Program Participation: " + SitePublishUtility.titlePatientStatusUpdates, true);
                    SiteFilesUtility.updateProgramParticipation(practice.NewSiteUrl, SitePublishUtility.titlePatientStatusUpdates,
                            SitePublishUtility.pagePatientStatusUpdates, layoutsFolder, SitePublishUtility.imgPatientStatusUpdates);
                }

                if (practice.IsIWH)
                {
                    // CKCC/KCE Resources...
                    //SiteFilesUtility.updateProgramParticipation(practice.NewSiteUrl, SitePublishUtility.titleCkccKceResources,
                    //        SitePublishUtility.pageCkccKceResources, layoutsFolder, SitePublishUtility.imgCkccKceResources);
                }

                if (practice.IsKC365)
                {
                    // CKCC/KCE Resources...
                    //SiteFilesUtility.updateProgramParticipation(practice.NewSiteUrl, SitePublishUtility.titleCkccKceResources,
                    //        SitePublishUtility.pageCkccKceResources, layoutsFolder, SitePublishUtility.imgCkccKceResources);
                }

                if (practice.IsTelephonic)
                {
                    // CKCC/KCE Resources...
                    //SiteFilesUtility.updateProgramParticipation(practice.NewSiteUrl, SitePublishUtility.titleCkccKceResources,
                    //        SitePublishUtility.pageCkccKceResources, layoutsFolder, SitePublishUtility.imgCkccKceResources);
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_UpdateProgramParticipation", ex.Message, "Error", "");
                logger.Information(ex.Message);
            }
        }

        private void Init_DocUpload(Practice practice, string layoutsFolder)
        {
            SiteFilesUtility sfu = new SiteFilesUtility();

            try
            {
                sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_ProgramParticipation.html", "SiteAssets");
                sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_Home.html", "SiteAssets");
                sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_RiskAdjustmentResources.html", "SiteAssets");
                sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_Quality.html", "SiteAssets");
                sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_InteractiveInsights.html", "SiteAssets");
                sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "FHPIcon.JPG", "SiteAssets");

                //sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_CarePlans.html", "SiteAssets");
                //sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_CarePlansDataTable.html", "SiteAssets");
                //sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_HospitalAlerts.html", "SiteAssets");
                //sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_HospAlertDataTable.html", "SiteAssets");
                //sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_MedicationAlerts.html", "SiteAssets");
                //sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_MedAlertDataTable.html", "SiteAssets");
                //sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_CareCoordination.html", "SiteAssets");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                SiteLogUtility.CreateLogEntry("Init_DocUpload", ex.Message, "Error", "", true);
            }
        }
        private void Init_DocUpload2(Practice practice, string layoutsFolder)
        {
            SiteFilesUtility sfu = new SiteFilesUtility();

            try
            {
                sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_CarePlans.html", "SiteAssets");
                sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_CarePlansDataTable.html", "SiteAssets");
                sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_HospitalAlerts.html", "SiteAssets");
                sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_HospAlertDataTable.html", "SiteAssets");
                sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_MedicationAlerts.html", "SiteAssets");
                sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_MedAlertDataTable.html", "SiteAssets");
                sfu.DocumentUpload(practice.NewSiteUrl, layoutsFolder + "cePrac_CareCoordination.html", "SiteAssets");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                SiteLogUtility.CreateLogEntry("Init_DocUpload", ex.Message, "Error", "", true);
            }
        }
        private static void LoggerInfo_Entry(string logtext, bool consolePrint = false)
        {
            try
            {
                logger.Information(logtext);
                SiteLogUtility.LogList.Add(logtext);
                if (consolePrint)
                {
                    Console.WriteLine(logtext);
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("LoggerInfo_Entry", ex.Message, "Error", "", true);
                logger.Error(ex.Message);
            }
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
        public class PmAssignment
        {
            public PmAssignment()
            {

            }

            public string PMRefId { get; set; }
            public string PMName { get; set; }
            public string PMGroup { get; set; }
        }

        public static List<PmAssignment> GetPmAssignments()
        {
            List<PmAssignment> pmAssignments = new List<PmAssignment>();

            PmAssignment pma1 = new PmAssignment() { PMRefId = "01", PMName = "Angela Korf", PMGroup = "IWNRegion01" };
            PmAssignment pma2 = new PmAssignment() { PMRefId = "02", PMName = "Annie Fambro", PMGroup = "IWNRegion02" };
            PmAssignment pma3 = new PmAssignment() { PMRefId = "03", PMName = "Francisco Calles", PMGroup = "IWNRegion03" };
            PmAssignment pma4 = new PmAssignment() { PMRefId = "04", PMName = "Glenda S Wooten", PMGroup = "IWNRegion04" };
            PmAssignment pma5 = new PmAssignment() { PMRefId = "05", PMName = "Karen Bauer", PMGroup = "IWNRegion05" };
            PmAssignment pma6 = new PmAssignment() { PMRefId = "06", PMName = "Katelyn Minnick", PMGroup = "IWNRegion06" };
            PmAssignment pma7 = new PmAssignment() { PMRefId = "07", PMName = "Linda S Biermann", PMGroup = "IWNRegion07" };
            PmAssignment pma8 = new PmAssignment() { PMRefId = "08", PMName = "Marion C Grantham", PMGroup = "IWNRegion08" };
            PmAssignment pma9 = new PmAssignment() { PMRefId = "09", PMName = "Thomas Locke", PMGroup = "IWNRegion09" };
            PmAssignment pma10 = new PmAssignment() { PMRefId = "10", PMName = "Kristina Dunigan", PMGroup = "IWNRegion10" };
            PmAssignment pma11 = new PmAssignment() { PMRefId = "11", PMName = "Kimberley Bankhead", PMGroup = "IWNRegion11" };

            pmAssignments.Add(pma1);
            pmAssignments.Add(pma2);
            pmAssignments.Add(pma3);
            pmAssignments.Add(pma4);
            pmAssignments.Add(pma5);
            pmAssignments.Add(pma6);
            pmAssignments.Add(pma7);
            pmAssignments.Add(pma8);
            pmAssignments.Add(pma9);
            pmAssignments.Add(pma10);
            pmAssignments.Add(pma11);

            return pmAssignments;
        }
        public static void ProgramMgrSiteGroups(string siteName, string strUrl)
        {
            List<PmAssignment> pmAssignments = new List<PmAssignment>();
            pmAssignments = GetPmAssignments();

            try
            {
                RoleAssignment_AddSiteManager(siteName, pmAssignments, strUrl);
                RoleAssignment_AddSiteManagerReadOnly(siteName, pmAssignments, strUrl);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("cSite - PracSiteGroups", ex.Message, "Error", "");
            }
        }
        private static bool RoleAssignment_AddSiteManager(string siteName, List<PmAssignment> pmAssignment, string strUrl)
        {
            int sStart = siteName.Length - 2;
            string PMid = siteName.Substring(sStart, 2);
            PmAssignment result = pmAssignment.Find(x => x.PMRefId == PMid);

            //string path = siteUrl + pracInfo.SiteMgrRegionRef + "/" + pracInfo.PracticeTIN;
            string path = strUrl;

            try
            {
                using (ClientContext clientContext = new ClientContext(path))
                {
                    Web w = clientContext.Web;
                    clientContext.Load(w);
                    clientContext.ExecuteQuery();

                    //Get by name > RoleDefinition...
                    RoleDefinition roleContributorPM = clientContext.Web.RoleDefinitions.GetByName("Practice Manager Site Permission Level");

                    //Get by name > Group...
                    Group oGroup = w.SiteGroups.GetByName(result.PMGroup + "_SiteManager");

                    RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(clientContext);
                    collRoleDefinitionBinding.Add(roleContributorPM);

                    // Add Group and RoleDefinitionBinding to RoleAssignments...
                    w.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

                    clientContext.Load(oGroup, group => group.Title);
                    clientContext.Load(roleContributorPM, role => role.Name);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("RoleAssignment_AddSiteManager", ex.Message, "Error", "");
                return false;
            }

            return true;
        }

        private static bool RoleAssignment_AddSiteManagerReadOnly(string siteName, List<PmAssignment> pmAssignment, string strUrl)
        {
            int sStart = siteName.Length - 2;
            string PMid = siteName.Substring(sStart, 2);
            PmAssignment result = pmAssignment.Find(x => x.PMRefId == PMid);

            //string path = siteUrl + pracInfo.SiteMgrRegionRef + "/" + pracInfo.PracticeTIN;
            string path = strUrl;

            try
            {
                using (ClientContext clientContext = new ClientContext(path))
                {
                    Web w = clientContext.Web;
                    clientContext.Load(w);
                    clientContext.ExecuteQuery();

                    //Get by name > RoleDefinition...
                    RoleDefinition roleReadOnly = clientContext.Web.RoleDefinitions.GetByName("Read");

                    //Get by name > Group...
                    Group oGroup = w.SiteGroups.GetByName(result.PMGroup + "_ReadOnly");

                    RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(clientContext);
                    collRoleDefinitionBinding.Add(roleReadOnly);

                    // Add Group and RoleDefinitionBinding to RoleAssignments...
                    w.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

                    clientContext.Load(oGroup, group => group.Title);
                    clientContext.Load(roleReadOnly, role => role.Name);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("RoleAssignment_AddSiteManagerReadOnly", ex.Message, "Error", "");
                return false;
            }

            return true;
        }
        public List<string> GetPracUserGroups(List<string> webGroups)
        {
            return webGroups.Where(g => g.StartsWith("Prac_")).ToList();
        }
        public List<string> GetPracUserReadOnly(List<string> webGroups)
        {
            return webGroups.Where(gb => gb.StartsWith("Prac_")).Where(ge => ge.EndsWith("_ReadOnly")).ToList();
        }
        public static string LoadParentWeb(string wUrl)
        {
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    clientContext.Load(clientContext.Web,
                                            web => web.ParentWeb.ServerRelativeUrl,
                                            web => web.ServerRelativeUrl,
                                            web => web.SiteGroups.Include(
                                                sg => sg.Description,
                                                sg => sg.Title));
                    clientContext.ExecuteQuery();

                    SiteInfoUtility siu = new SiteInfoUtility();
                    string rootUrl = siu.GetRootSite(wUrl);

                    SiteLogUtility.Log_Entry("RootWeb: " + rootUrl, true);
                    SiteLogUtility.Log_Entry("ParentWeb: " + clientContext.Web.ParentWeb.ServerRelativeUrl, true);
                    SiteLogUtility.Log_Entry("PracticeWeb: " + clientContext.Web.ServerRelativeUrl, true);

                    return rootUrl + clientContext.Web.ParentWeb.ServerRelativeUrl;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error with LoadParentWeb: " + ex.Message);
                    return null;
                }
            }
        }
        public static void UpdateProgramParticipation(string adminUrl, Practice site, string dbProgramParticipation, string strProgramManagerSite = "")
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(adminUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web w = clientContext.Web;
                    List adminList = null;

                    if (strProgramManagerSite == "")
                    {
                        adminList = clientContext.Web.Lists.GetByTitle("AdminGroup");
                    }
                    else
                    {
                        adminList = clientContext.Web.Lists.GetByTitle("AdminSiteData");
                    }
                    ListItem oItem = null;
                    clientContext.Load(w);

                    CamlQuery oQuery = new CamlQuery();
                    oQuery.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='PracticeTIN' /><Value Type='Text'>" + site.SiteID + "</Value></Eq></Where></Query></View>";

                    ListItemCollection oItems = adminList.GetItems(oQuery);
                    clientContext.Load(oItems);
                    clientContext.ExecuteQuery();

                    oItem = oItems.FirstOrDefault();
                    logger.Debug("Item=" + oItem["URL"].ToString());
                    logger.Debug("Item=" + oItem["PracticeName"].ToString());
                    logger.Debug("Item=" + oItem["PracticeTIN"].ToString());
                    logger.Debug("Item=" + oItem["ProgramParticipation"].ToString());
                    logger.Debug("Item=" + oItem["KCEArea"].ToString());
                    oItem["ProgramParticipation"] = dbProgramParticipation;
                    //if (strProgramManagerSite != "")
                    //{
                    //    logger.Debug("Item=" + oItem["ProgramManagerSite"].ToString());
                    //}
                    oItem.Update();
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error: " + ex.Message);
            }
        }
        private string FormatProgramParticipation(PMData pd)
        {
            string programParticipation = string.Empty;
            SitePMData sitePmd = new SitePMData();

            try
            {
                if (pd.IsIWH == "true")
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePmd.programParticipationIWH : programParticipation + "; " + sitePmd.programParticipationIWH;
                }
                if (pd.IsCKCC == "true")
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePmd.programParticipationCKCC : programParticipation + "; " + sitePmd.programParticipationCKCC;
                }
                if (pd.IsKC365 == "true")
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePmd.programParticipationKC365 : programParticipation + "; " + sitePmd.programParticipationKC365;
                }
                if (pd.IsTeleKC365 == "true")
                {
                    programParticipation = String.IsNullOrEmpty(programParticipation) ? sitePmd.programParticipationTelephonicKC365 : programParticipation + "; " + sitePmd.programParticipationTelephonicKC365;
                }

                return programParticipation;
            }

            catch (Exception ex)
            {
                logger.Error("Error: " + ex.Message);
                return null;
            }
        }
        public static void SyncSiteDescription(string wUrl, string pracName)
        {
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.Load(web.ParentWeb);
                    clientContext.ExecuteQuery();

                    SiteInfoUtility siu = new SiteInfoUtility();
                    string strSiteDesc = GetSiteDescriptionData(siu.GetRootSite(wUrl) + web.ParentWeb.ServerRelativeUrl, pracName);

                    web.Description = strSiteDesc;
                    web.Update();
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();

                    logger.Information(strSiteDesc);
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("SyncSubSiteDescription", ex.Message, "Error", wUrl);
                }
            }
        }

        public static string GetSiteDescriptionData(string wUrl, string SiteTitle)
        {
            string strDescription = "";
            string strParticipationValue = "";
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                try
                {
                    var web = clientContext.Web;
                    clientContext.Load(web.ParentWeb);
                    clientContext.ExecuteQuery();

                    List list = clientContext.Web.Lists.GetByTitle("AdminGroup");
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='PracticeName' /><Value Type='Text'>" + SiteTitle + "</Value></Eq></Where></Query></View>";
                    ListItemCollection items = list.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    if (items.Count > 0)
                    {
                        if (items[0].FieldValues["KCEArea"] != null)
                        {
                            strDescription = SiteTitle + " is a member of " + items[0].FieldValues["KCEArea"].ToString() + ". Program Participation: ";
                        }
                        else
                        {
                            strDescription = SiteTitle + ". Program Participation: ";
                        }
                        if (items[0].FieldValues["ProgramParticipation"] != null)
                        {
                            string[] strParticipationList = items[0].FieldValues["ProgramParticipation"].ToString().Split(';');
                            for (int intLoop = 0; intLoop < strParticipationList.Length; intLoop++)
                            {
                                strParticipationValue = strParticipationValue + " " + (intLoop + 1) + "." + strParticipationList[intLoop].ToString() + ";";
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("SyncSubSiteDescription", ex.Message, "Error", wUrl);
                }
            }
            strDescription = strDescription + " " + strParticipationValue;
            return strDescription;
        }
        public static string changeSiteNameTitleCase(string strSiteName)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string strText = "";
            string[] strSiteNameArr = strSiteName.Split(' ');
            for (int intArray = 0; intArray < strSiteNameArr.Count(); intArray++)
            {
                if ((intArray + 1) != strSiteNameArr.Count())
                {
                    if (strSiteNameArr[intArray].ToString().ToLower() == "and" || strSiteNameArr[intArray].ToString().ToLower() == "of")
                    {
                        strText = strText + " " + strSiteNameArr[intArray].ToString().ToLower();
                    }
                    else
                    {
                        strText = strText + " " + textInfo.ToTitleCase(strSiteNameArr[intArray].ToString().ToLower());
                    }
                }
                else if (strSiteNameArr.Last().Contains('('))
                {
                    strText = strText + " " + strSiteNameArr[intArray].ToString();
                }
                else if (strSiteNameArr.Last().Count() < 5)
                {
                    strText = strText + " " + strSiteNameArr[intArray].ToString();
                }
                else
                {
                    strText = strText + " " + textInfo.ToTitleCase(strSiteNameArr[intArray].ToString().ToLower());
                }
            }
            return strText.Trim();
        }
    }
}

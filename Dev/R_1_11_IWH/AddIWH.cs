using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using SiteUtility;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using Microsoft.SharePoint.Client.WebParts;
using System.Net;
using System.Xml;
using System.Reflection;
using Serilog;

namespace R_1_11_IWH
{
    public class AddIWH
    {
        static Guid _listGuid = Guid.Empty;
        static string dateHrMin = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
        const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
        static ILogger _logger = Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("Logs/maint" + dateHrMin + "_.log", rollingInterval: RollingInterval.Day, shared: false, outputTemplate: outputTemp1)
           .CreateLogger();
        static ILogger logger = _logger.ForContext<AddIWH>();

        public void InitProg()
        {
            SiteInfoUtility siteInfo = new SiteInfoUtility();
            //List<Practice> practices = siteInfo.GetAllPractices();
            List<Practice> practices = siteInfo.GetPracticesByPM("01");
            int CntPrac = 0;


            try
            {
                LoggerInfo_Entry("========================================Release Starts========================================", true);

                //if (practice != null)
                if (practices != null && practices.Count > 0)
                {
                    foreach (Practice practice in practices)
                    {
                        {
                            uploadProgramPracticeSupportFilesIwnPayorEd(practice);                // Image...
                            modifyWebPartProgramParticipation(practice.NewSiteUrl, practice);     // Resize...
                            uploadMultiPartSupportingFilesAll(practice.NewSiteUrl, practice);     // JavaScript...

                            Init_Payor(practice);
                            Init_DataExchange(practice);
                            Init_RiskAdjustment(practice);
                            Init_Quality(practice);

                            LoggerInfo_Entry("Testing: " + practice.Name + " - " + practice.NewSiteUrl);
                            
                            SiteNavigateUtility.ClearQuickNavigationRecent(practice.NewSiteUrl);
                            SiteNavigateUtility.RenameQuickNavigationNode(practice.NewSiteUrl, "Quality Coming Soon", "Quality");
                            CntPrac++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerInfo_Entry("Error: " + ex.Message, true);
            }
            finally
            {
                LoggerInfo_Entry(SiteLogUtility.textLine0);
                LoggerInfo_Entry("Total Practice Count: " + CntPrac, true);
                LoggerInfo_Entry(SiteLogUtility.textLine0);
                LoggerInfo_Entry("========================================Release Ends========================================", true);
                SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@interwellhealth.com");
            }

            Log.CloseAndFlush();
        }
        public void InitProg(string siteId)
        {
            SiteInfoUtility siteInfo = new SiteInfoUtility();
            Practice practice = siteInfo.GetPracticeBySiteID(siteId);
            int CntPrac = 0;

            try
            {
                LoggerInfo_Entry("========================================Release Starts========================================", true);

                if (practice != null)
                //if (practices != null && practices.Count > 0)
                {
                    //foreach (Practice practice in practices)
                    {
                        {
                            //siteInfo.Init_UpdateAllProgramParticipation(practice);
                            //return;

                            uploadProgramPracticeSupportFilesIwnPayorEd(practice);                // Image...
                            modifyWebPartProgramParticipation(practice.NewSiteUrl, practice);     // Resize...
                            uploadMultiPartSupportingFilesAll(practice.NewSiteUrl, practice);     // JavaScript...

                            Init_Payor(practice);
                            Init_DataExchange(practice);
                            Init_RiskAdjustment(practice);
                            Init_Quality(practice);
                            Init_CarePlan(practice);

                            LoggerInfo_Entry("Practice: " + practice.Name + " - " + practice.NewSiteUrl);

                            SiteNavigateUtility.ClearQuickNavigationRecent(practice.NewSiteUrl);
                            SiteNavigateUtility.RenameQuickNavigationNode(practice.NewSiteUrl, "Quality Coming Soon", "Quality");
                            CntPrac++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerInfo_Entry("Error: " + ex.Message, true);
            }
            finally
            {
                LoggerInfo_Entry(SiteLogUtility.textLine0);
                LoggerInfo_Entry("Total Practice Count: " + CntPrac, true);
                LoggerInfo_Entry(SiteLogUtility.textLine0);
                LoggerInfo_Entry("========================================Release Ends========================================", true);
                SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@interwellhealth.com");
            }

            Log.CloseAndFlush();
        }

        private void Init_CarePlan(Practice practice)
        {
            SiteFilesUtility siteFilesUtility = new SiteFilesUtility();
            string LayoutsFolderIwn = @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\Iwn\";

            try
            {
                //check if only IWN
                //upload cePrac_CarePlans.html
                if(practice.IsCKCC == false && practice.IsIWH == true)
                {
                    siteFilesUtility.DocumentUpload(practice.NewSiteUrl, LayoutsFolderIwn + "cePrac_CarePlans.html", "SiteAssets");
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_CarePlan", ex.Message, "Error", "");
            }
        }

        //public void InitiateProgNew2()
        //{
        //    string releaseName = "SiteUtilityTest";
        //    string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
        //    string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];


        //    string runPM = "PM01";
        //    string runPractice = "91930060469";
        //    string urlAdminGroup = siteUrl + "/" + runPM;

        //    SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
        //    logger.Information("========================================Release Starts========================================");

        //    using (ClientContext clientContext = new ClientContext(siteUrl))
        //    {
        //        clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

        //        try
        //        {
        //            logger.Information("-------------[ Read Deployed DB:  " + urlAdminGroup + "  ]-------------");
        //            //SitePMData objSitePMData = new SitePMData();
        //            //DataTable dataTable = objSitePMData.readDBPortalDeployed(runPM);
        //            //List<PMData> pmd = FilterPMData(dataTable);

        //            logger.Information("-------------[ Processing AdminGroup:  " + urlAdminGroup + "  ]-------------");
        //            List<PMData> pmData = SiteInfoUtility.initPMDataToList(urlAdminGroup);

        //            logger.Information("-------------[ Get all Portal Practice Data         ]-------------");
        //            List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext, practicesIWH, practicesCKCC, pmData);

        //            logger.Information("-------------[ Maintenance Tasks - Start            ]-------------");
        //            foreach (ProgramManagerSite pm in practicePMSites)
        //            {
        //                foreach (PracticeSite psite in pm.PracticeSiteCollection)
        //                {
        //                    //if (psite.URL.Contains(runPM))
        //                    if (psite.URL.Contains(runPM) && psite.URL.Contains(runPractice))
        //                    {
        //                        //SiteLogUtility.LogPracDetail(psite);

        //                        SiteFilesUtility sfu = new SiteFilesUtility();
        //                        uploadProgramPracticeSupportFilesIwnPayorEd(psite);    // Image...
        //                        modifyWebPartProgramParticipation(psite.URL, psite);   // Resize...
        //                        uploadMultiPartSupportingFilesAll(psite.URL, psite);   // JavaScript...

        //                        Init_Payor(psite);
        //                        Init_DataExchange(psite);
        //                        Init_RiskAdjustment(psite);
        //                        Init_Quality(psite);

        //                        //PMData beforePmd = (PMData)pmData.Where(x => x.SiteId == psite.SiteId).FirstOrDefault();
        //                        //PMData afterPmd = (PMData)pmd.Where(x => x.SiteId == psite.SiteId).FirstOrDefault();

        //                        //if (afterPmd.IsTeleKC365 == "true")
        //                        //{
        //                        //    logger.Debug("--");
        //                        //    logger.Debug(psite.PracticeName);
        //                        //    logger.Debug(psite.Name + " - " + psite.URL);
        //                        //    logger.Debug("BEFORE:" + beforePmd.ProgramParticipation);
        //                        //    logger.Debug(" AFTER:" + afterPmd.ProgramParticipation);

        //                        //string adminUrl = LoadParentWeb(pm.URL);
        //                        //UpdateProgramParticipation(pm.URL, psite, afterPmd.ProgramParticipation);
        //                        //UpdateProgramParticipation(adminUrl, psite, afterPmd.ProgramParticipation, runPM);
        //                        //SyncSiteDescription(psite.URL, psite.Name);
        //                        //}

        //                    }
        //                }
        //            }
        //            logger.Information("-------------[ Maintenance Tasks - End              ]-------------");
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.Error("Error: " + ex.Message);
        //        }
        //        finally
        //        {
        //            logger.Information(SiteLogUtility.textLine0);
        //            logger.Information(releaseName);
        //            //SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@freseniusmedicalcare.com");
        //        }
        //        logger.Information("========================================Release Ends========================================");
        //    }

        //    Log.CloseAndFlush();
        //}

        private static void LoggerInfo_Entry(string logtext, bool consolePrint = false)
        {
            logger.Information(logtext);
            SiteLogUtility.LogList.Add(logtext);
            if (consolePrint)
            {
                Console.WriteLine(logtext);
            }
        }
        private static void Init_Payor(Practice practiceSite)
        {
            SiteLogUtility.Log_Entry("Init_Payor - In Progress...");
            bool ConfigSuccess = false;
            string LayoutsFolderMnt = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
            string LayoutsFolderIwn = @"M:\FTP Targets\Integrated Care Group\Portal\~Deployment\Pages\Iwn\";

            SiteFilesUtility sfUtility = new SiteFilesUtility();
            SitePublishUtility spUtility = new SitePublishUtility();
            SiteListUtility slUtility = new SiteListUtility();
            PracticeCView practiceCView = new PracticeCView();

            try
            {
                if (practiceSite.IsIWH && SiteListUtility.DoesListExist(practiceSite.NewSiteUrl, slUtility.listNamePayorEducationIwh) == false)
                {
                    SiteListUtility.ProvisionList(practiceSite, slUtility, slUtility.listNamePayorEducationIwh, practiceCView); 
                }
                SiteListUtility.CreateFolder(practiceSite, slUtility.listNamePayorEducationIwh, slUtility.listFolder1PayorEducationIwh);
                SiteListUtility.CreateFolder(practiceSite, slUtility.listNamePayorEducationIwh, slUtility.listFolder2PayorEducationIwh);

                if (!SiteFilesUtility.FileExists(practiceSite.NewSiteUrl, "Pages", slUtility.pageNamePayorEducation + ".aspx"))
                {
                    spUtility.InitializePage(practiceSite.NewSiteUrl, slUtility.pageNamePayorEducation, slUtility.pageTitlePayorEducation); 
                }
                spUtility.DeleteWebPart(practiceSite.NewSiteUrl, slUtility.pageNamePayorEducation);
                sfUtility.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolderMnt + "PayorEducation_MultiTab.js", "SiteAssets");
                sfUtility.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolderMnt + "jquery-ui.theme.css", "SiteAssets");
                ConfigSuccess = ConfigurePayorEducationPage(practiceSite.NewSiteUrl, practiceSite);
                if (ConfigSuccess)
                {
                    if (practiceSite.IsIWH)
                    {
                        modifyView(practiceSite.NewSiteUrl, slUtility.pageNamePayorEducation + ".aspx", slUtility.webpartPayorEducationIwh);
                    }
                }
                SP_Update_ProgramParticipation(practiceSite.NewSiteUrl, slUtility.pageNamePayorEducation, "Payor Program Education Resources Coming Soon", "Payor Program Education Resources", "EducationReviewPro.JPG");
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_Payor", ex.Message, "Error", "");
            }
        }
        private static void Init_Quality(Practice practiceSite)
        {
            SiteLogUtility.Log_Entry("Init_Quality - In Progress...");
            bool ConfigSuccess = false;
            PublishingPage PPage = null;
            string LayoutsFolderMnt = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";

            SiteFilesUtility sfu = new SiteFilesUtility();
            SitePublishUtility spUtility = new SitePublishUtility();
            SiteListUtility slUtility = new SiteListUtility();
            PracticeCView practiceCView = new PracticeCView();

            try
            {
                if (practiceSite.IsIWH && SiteListUtility.DoesListExist(practiceSite.NewSiteUrl, slUtility.listNameQualityIwh) == false)
                {
                    SiteListUtility.ProvisionList(practiceSite, slUtility, slUtility.listNameQualityIwh, practiceCView);
                }
                SiteListUtility.CreateFolder(practiceSite, slUtility.listNameQualityIwh, slUtility.listFolder1QualityIwh);
                SiteListUtility.CreateFolder(practiceSite, slUtility.listNameQualityIwh, slUtility.listFolder2QualityIwh);
                SiteListUtility.CreateFolder(practiceSite, slUtility.listNameQualityIwh, slUtility.listFolder3QualityIwh);

                //if (practiceSite.IsCKCC && SiteListUtility.DoesListExist(practiceSite.NewSiteUrl, slUtility.listNameQualityCkcc) == false)
                //{
                //    SiteListUtility.ProvisionList(practiceSite, slUtility, slUtility.listNameQualityCkcc, practiceCView);
                //}
                //SiteListUtility.CreateFolder(practiceSite, slUtility.listNameQualityCkcc, slUtility.listFolder1QualityCkcc);
                //SiteListUtility.CreateFolder(practiceSite, slUtility.listNameQualityCkcc, slUtility.listFolder2QualityCkcc);
                //SiteListUtility.CreateFolder(practiceSite, slUtility.listNameQualityCkcc, slUtility.listFolder3QualityCkcc);

                if (!SiteFilesUtility.FileExists(practiceSite.NewSiteUrl, "Pages", slUtility.pageNameQuality + ".aspx"))
                {
                    spUtility.InitializePage(practiceSite.NewSiteUrl, slUtility.pageNameQuality, slUtility.pageTitleQuality);
                }
                spUtility.DeleteWebPart(practiceSite.NewSiteUrl, slUtility.pageNameQuality);
                sfu.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolderMnt + "Quality_MultiTab.js", "SiteAssets");
                sfu.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolderMnt + "jquery-ui.theme.css", "SiteAssets");
                sfu.uploadImageSupportingFilesSingleImage(practiceSite.NewSiteUrl, "Quality.jpg");
                sfu.uploadHtmlSupportingFilesSingleFile(practiceSite.NewSiteUrl, "cePrac_Quality.html");
                ConfigSuccess = ConfigureQualityPage(practiceSite.NewSiteUrl, practiceSite);
                if (ConfigSuccess)
                {
                    if (practiceSite.IsIWH)
                    {
                        modifyView(practiceSite.NewSiteUrl, slUtility.pageNameQuality + ".aspx", slUtility.webpartQualityIwh);
                    }
                    if (practiceSite.IsCKCC)
                    {
                        modifyView(practiceSite.NewSiteUrl, slUtility.pageNameQuality + ".aspx", slUtility.webpartQualityCkcc);
                    }
                }

            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_Quality", ex.Message, "Error", "");
            }
        }
        private static void Init_DataExchange(Practice practiceSite)
        {
            SiteLogUtility.Log_Entry("Init_DataExchange - In Progress...");
            bool ConfigSuccess = false;
            string LayoutsFolderMnt = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";

            SiteFilesUtility sfUtility = new SiteFilesUtility();
            SitePublishUtility spUtility = new SitePublishUtility();
            SiteListUtility slUtility = new SiteListUtility();
            PracticeCView practiceCView = new PracticeCView();

            try
            {
                if (practiceSite.IsIWH && SiteListUtility.DoesListExist(practiceSite.NewSiteUrl, slUtility.listNameDataExchangeIwh) == false)
                {
                    SiteListUtility.ProvisionList(practiceSite, slUtility, slUtility.listNameDataExchangeIwh, practiceCView);
                }
                SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeIwh, slUtility.listFolder1DataExchangeIwh);
                SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeIwh, slUtility.listFolder2DataExchangeIwh);
                SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeIwh, slUtility.listFolder3DataExchangeIwh);
                SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeIwh, slUtility.listFolder4DataExchangeIwh);

                //if (practiceSite.IsCKCC && SiteListUtility.DoesListExist(practiceSite.NewSiteUrl, slUtility.listNameDataExchangeCkcc) == false)
                //{
                //    SiteListUtility.ProvisionList(practiceSite, slUtility, slUtility.listNameDataExchangeCkcc, practiceCView);
                //}
                //SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeCkcc, slUtility.listFolder1DataExchangeCkcc);
                //SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeCkcc, slUtility.listFolder2DataExchangeCkcc);
                //SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeCkcc, slUtility.listFolder3DataExchangeCkcc);
                //SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeCkcc, slUtility.listFolder4DataExchangeCkcc);
                //SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeCkcc, slUtility.listFolder5DataExchangeCkcc);
                //SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeCkcc, slUtility.listFolder6DataExchangeCkcc);
                //SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeCkcc, slUtility.listFolder7DataExchangeCkcc);

                if (!SiteFilesUtility.FileExists(practiceSite.NewSiteUrl, "Pages", slUtility.pageNameDataExchange + ".aspx"))
                {
                    spUtility.InitializePage(practiceSite.NewSiteUrl, slUtility.pageNameDataExchange, slUtility.pageTitleDataExchange); 
                }
                spUtility.DeleteWebPart(practiceSite.NewSiteUrl, slUtility.pageNameDataExchange);
                sfUtility.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolderMnt + "PracticeSiteTemplate_MultiTab.js", "SiteAssets");
                sfUtility.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolderMnt + "jquery-ui.theme.css", "SiteAssets");
                ConfigSuccess = ConfigureDocumentExchangePage(practiceSite.NewSiteUrl, practiceSite);
                if (ConfigSuccess)
                {
                    if (practiceSite.IsIWH)
                    {
                        modifyView(practiceSite.NewSiteUrl, slUtility.pageNameDataExchange + ".aspx", slUtility.webpartDataExchangeIwh);
                    }
                    if (practiceSite.IsCKCC)
                    {
                        modifyView(practiceSite.NewSiteUrl, slUtility.pageNameDataExchange + ".aspx", slUtility.webpartDataExchangeCkcc);
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_DataExchange", ex.Message, "Error", "");
            }
            //cntIsIwh++;
        }
        private static void Init_RiskAdjustment(Practice practiceSite)
        {
            SiteLogUtility.Log_Entry("Init_RiskAdjustment - In Progress...");
            bool ConfigSuccess = false;
            PublishingPage PPage = null;
            string LayoutsFolderMnt = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";

            SiteFilesUtility sfUtility = new SiteFilesUtility();
            SitePublishUtility spUtility = new SitePublishUtility();
            SiteListUtility slUtility = new SiteListUtility();
            PracticeCView practiceCView = new PracticeCView();

            try
            {
                if (practiceSite.IsIWH && SiteListUtility.DoesListExist(practiceSite.NewSiteUrl, slUtility.listNameRiskAdjustmentIwh) == false)
                {
                    SiteListUtility.ProvisionList(practiceSite, slUtility, slUtility.listNameRiskAdjustmentIwh, practiceCView);
                }
                SiteListUtility.CreateFolder(practiceSite, slUtility.listNameRiskAdjustmentIwh, slUtility.listFolder1RiskAdjustmentIwh);
                SiteListUtility.CreateFolder(practiceSite, slUtility.listNameRiskAdjustmentIwh, slUtility.listFolder2RiskAdjustmentIwh);

                SitePermissionUtility.BreakRoleInheritanceOnList(practiceSite.NewSiteUrl, slUtility.listNameRiskAdjustmentIwh, "Risk_Adjustment_User", RoleType.Contributor);

                //if (practiceSite.IsCKCC && SiteListUtility.DoesListExist(practiceSite.NewSiteUrl, slUtility.listNameRiskAdjustmentCkcc) == false)
                //{
                //    SiteListUtility.ProvisionList(practiceSite, slUtility, slUtility.listNameRiskAdjustmentCkcc, practiceCView);
                //}
                //SiteListUtility.CreateFolder(practiceSite, slUtility.listNameRiskAdjustmentCkcc, slUtility.listFolder1RiskAdjustmentCkcc);
                //SiteListUtility.CreateFolder(practiceSite, slUtility.listNameRiskAdjustmentCkcc, slUtility.listFolder2RiskAdjustmentCkcc);
                //SiteListUtility.CreateFolder(practiceSite, slUtility.listNameRiskAdjustmentCkcc, slUtility.listFolder3RiskAdjustmentCkcc);

                if (!SiteFilesUtility.FileExists(practiceSite.NewSiteUrl, "Pages", slUtility.pageNameRiskAdjustment + ".aspx"))
                {
                    spUtility.InitializePage(practiceSite.NewSiteUrl, slUtility.pageNameRiskAdjustment, slUtility.pageTitleRiskAdjustment); 
                }
                spUtility.DeleteWebPart(practiceSite.NewSiteUrl, slUtility.pageNameRiskAdjustment);
                sfUtility.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolderMnt + "RiskAdjustment.js", "SiteAssets");
                sfUtility.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolderMnt + "jquery-ui.theme.css", "SiteAssets");
                ConfigSuccess = ConfigureRiskAdjustmentPage(practiceSite.NewSiteUrl, practiceSite);
                if (ConfigSuccess)
                {
                    if (practiceSite.IsIWH)
                    {
                        modifyView(practiceSite.NewSiteUrl, slUtility.pageNameRiskAdjustment + ".aspx", slUtility.webpartRiskAdjustmentIwh);
                    }
                    if (practiceSite.IsCKCC)
                    {
                        modifyView(practiceSite.NewSiteUrl, slUtility.pageNameRiskAdjustment + ".aspx", slUtility.webpartRiskAdjustmentCkcc);
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_RiskAdjustment", ex.Message, "Error", "");
            }
            //cntIsIwh++;
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
        public static bool modifyWebPartProgramParticipation(string webUrl, Practice practiceSite)
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
                        SiteLogUtility.CreateLogEntry("modifyWebPart", ex.Message, "Error", "");
                        outcome = false;
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("modifyWebPart", ex.Message, "Error", "");
                    outcome = false;
                    file.UndoCheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();
                    clientContext.Dispose();
                }
            }
            return outcome;
        }
        public static int gridHeight(string webUrl, Practice site)
        {
            int intCount = -1;
            int[] intHeight = new int[5] { 156, 253, 350, 447, 544 };
            try
            {
                if (site.IsIWH)
                {
                    intCount++;  // Payor Program Education Resources...
                }
                if (site.IsCKCC)
                {
                    intCount++;  // CKCC/KCE Resources...
                    intCount++;  // Patient Status Updates...
                }
                if (site.IsKC365)
                {
                    intCount++;  // Payor Enrollment...
                }
                if (site.IsTelephonic)
                {
                    intCount++;  // CKCC/KCE Engagement...
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("gridHeight", ex.Message, "Error", "");
            }
            return intHeight[intCount];
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

                    Folder parentFolder = docList.RootFolder.Folders.Add(folderName);
                    //if (practiceSite.IsCKCC == "true")
                    //{
                    //    Folder parentFolder = docList.RootFolder.Folders.Add(folderName);
                    //}

                    clientContext.Load(folderCollection);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("CreateFolder", ex.Message, "Error", "");
            }
        }
        public static bool SP_Update_ProgramParticipation(string wUrl, string pageName, string searchTitle, string newTitle, string newThumbnail)
        {
            SiteLogUtility.Log_Entry("   SP_Update_ProgramParticipation - In Progress...");
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
                    SiteLogUtility.CreateLogEntry("SP_Update_ProgramParticipation", ex.Message, "Error", "");
                    return false;
                }
            }

            return true;
        }

        public static object lockObjDataExchange = new object();
        public static object lockObjRiskAdjustment = new object();
        public static object lockObjBenefitEnhancement = new object();
        public static object lockObjQuality = new object();
        public static object lockObjPayorEducation = new object();
        public static void uploadMultiPartSupportingFilesAll(string wUrl, Practice practiceSite)
        {
            string LayoutsFolderMnt = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
            try
            {
                SiteListUtility slu = new SiteListUtility();
                string strJSContentDataExchange = "";
                string strJSContentRiskAdjustment = "";
                string strJSContentBenefitEnhancement = "";
                string strJSContentQuality = "";
                string strJSContentPayorEducation = "";

                string strJSFileServerPathDataExchange = LayoutsFolderMnt + "PracticeSiteTemplate_MultiTab.js";
                string strJSFileServerPathRiskAdjustment = LayoutsFolderMnt + "RiskAdjustment.js";
                string strJSFileServerPathBenefitEnhancement = LayoutsFolderMnt + "BenefitEnhancement_MultiTab.js";
                string strJSFileServerPathQuality = LayoutsFolderMnt + "Quality_MultiTab.js";
                string strJSFileServerPathPayorEducation = LayoutsFolderMnt + "PayorEducation_MultiTab.js";

                //if (practiceSite.IsIWH.Equals("true"))
                if (practiceSite.IsIWH)
                {
                    strJSContentDataExchange = @"var thisTab2 = {title: '" + slu.tabTitleDataExchangeIwh + "',webParts: ['" + slu.webpartDataExchangeIwh + "']};tabConfiguration.push(thisTab2);";
                    strJSContentRiskAdjustment = @"var thisTab2 = {title: '" + slu.tabTitleRiskAdjustmentIwh + "',webParts: ['" + slu.webpartRiskAdjustmentIwh + "']};tabConfiguration.push(thisTab2);";
                    strJSContentQuality = @"var thisTab2 = {title: '" + slu.tabTitleQualityIwh + "',webParts: ['" + slu.webpartQualityIwh + "']};tabConfiguration.push(thisTab2);";
                    strJSContentPayorEducation = @"var thisTab2 = {title: '" + slu.tabTitlePayorEducationIwh + "',webParts: ['" + slu.webpartPayorEducationIwh + "']};tabConfiguration.push(thisTab2);";
                }
                //if (practiceSite.IsCKCC.Equals("true"))
                if (practiceSite.IsCKCC)
                {
                    strJSContentDataExchange = strJSContentDataExchange + @"var thisTab3 = {title: '" + slu.tabTitleDataExchangeCkcc + "',webParts: ['" + slu.webpartDataExchangeCkcc + "']};tabConfiguration.push(thisTab3);";
                    strJSContentRiskAdjustment = strJSContentRiskAdjustment + @"var thisTab3 = {title: '" + slu.tabTitleRiskAdjustmentCkcc + "',webParts: ['" + slu.webpartRiskAdjustmentCkcc + "']};tabConfiguration.push(thisTab3);";
                    strJSContentBenefitEnhancement = @"var thisTab2 = {title: '" + slu.tabTitleBenefitEnhancementCkcc + "',webParts: ['" + slu.webpartBenefitEnhancementCkcc + "']};tabConfiguration.push(thisTab2);";
                    strJSContentQuality = strJSContentQuality + @"var thisTab3 = {title: '" + slu.tabTitleQualityCkcc + "',webParts: ['" + slu.webpartQualityCkcc + "']};tabConfiguration.push(thisTab3);";
                }

                strJSContentDataExchange = strJSContentDataExchange + "//*#funXXXX#*";
                strJSContentRiskAdjustment = strJSContentRiskAdjustment + "//*#funXXXX#*";
                strJSContentBenefitEnhancement = strJSContentBenefitEnhancement + "//*#funXXXX#*";
                strJSContentQuality = strJSContentQuality + "//*#funXXXX#*";
                strJSContentPayorEducation = strJSContentPayorEducation + "//*#funXXXX#*";

                lock (lockObjDataExchange)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathDataExchange).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentDataExchange;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathDataExchange, lines);
                }

                lock (lockObjRiskAdjustment)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathRiskAdjustment).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentRiskAdjustment;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathRiskAdjustment, lines);
                }

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
                SiteLogUtility.CreateLogEntry("uploadMultiPartSupportingFilesAll", ex.Message, "Error", "");
            }
        }
        public static void uploadMultiPartSupportingFilesIwh(string wUrl, PracticeSite practiceSite)
        {
            string LayoutsFolderMnt = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
            try
            {
                SiteListUtility slu = new SiteListUtility();
                string strJSContentDataExchange = "";
                string strJSContentRiskAdjustment = "";
                string strJSContentQuality = "";
                string strJSContentPayorEducation = "";

                string strJSFileServerPathDataExchange = LayoutsFolderMnt + "PracticeSiteTemplate_MultiTab.js";
                string strJSFileServerPathRiskAdjustment = LayoutsFolderMnt + "RiskAdjustment.js";
                string strJSFileServerPathQuality = LayoutsFolderMnt + "Quality_MultiTab.js";
                string strJSFileServerPathPayorEducation = LayoutsFolderMnt + "PayorEducation_MultiTab.js";

                if (practiceSite.IsIWH.Equals("true"))
                {
                    strJSContentDataExchange = @"var thisTab2 = {title: '" + slu.tabTitleDataExchangeIwh + "',webParts: ['" + slu.webpartDataExchangeIwh + "']};tabConfiguration.push(thisTab2);";
                    strJSContentRiskAdjustment = @"var thisTab2 = {title: '" + slu.tabTitleRiskAdjustmentIwh + "',webParts: ['" + slu.webpartRiskAdjustmentIwh + "']};tabConfiguration.push(thisTab2);";
                    strJSContentQuality = @"var thisTab2 = {title: '" + slu.tabTitleQualityIwh + "',webParts: ['" + slu.webpartQualityIwh + "']};tabConfiguration.push(thisTab2);";
                    strJSContentPayorEducation = @"var thisTab2 = {title: '" + slu.tabTitlePayorEducationIwh + "',webParts: ['" + slu.webpartPayorEducationIwh + "']};tabConfiguration.push(thisTab2);";
                }

                strJSContentDataExchange = strJSContentDataExchange + "//*#funXXXX#*";
                strJSContentRiskAdjustment = strJSContentRiskAdjustment + "//*#funXXXX#*";
                strJSContentQuality = strJSContentQuality + "//*#funXXXX#*";
                strJSContentPayorEducation = strJSContentPayorEducation + "//*#funXXXX#*";

                lock (lockObjDataExchange)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathDataExchange).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentDataExchange;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathDataExchange, lines);
                }

                lock (lockObjRiskAdjustment)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathRiskAdjustment).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentRiskAdjustment;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathRiskAdjustment, lines);
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
                SiteLogUtility.CreateLogEntry("uploadMultiPartSupportingFilesAll", ex.Message, "Error", "");
            }
        }
        public static void uploadProgramPracticeSupportFilesIwnPayorEd(Practice practiceSite)
        {
            //string siteType = practiceSite.siteType;

            //if (siteType == "")
            //{
            //    return;
            //}
            string LayoutsFolder = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
            using (ClientContext clientContext = new ClientContext(practiceSite.NewSiteUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    string rootWebUrl = GetRootSite(practiceSite.NewSiteUrl);

                    string LibraryName = "Program Participation";

                    string fileName0 = "EducationReviewPro.JPG";

                    byte[] f0 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName0);

                    FileCreationInformation fc0 = new FileCreationInformation();
                    fc0.Url = fileName0;
                    fc0.Overwrite = true;
                    fc0.Content = f0;

                    List myLibrary = web.Lists.GetByTitle(LibraryName);

                    //if (siteType != null && siteType.Contains("iwh"))
                    if (practiceSite.IsIWH == true)
                    {
                        Microsoft.SharePoint.Client.File newFile0 = myLibrary.RootFolder.Files.Add(fc0);
                        clientContext.Load(newFile0);
                        clientContext.ExecuteQuery();

                        ListItem lItem0 = newFile0.ListItemAllFields;
                        lItem0.File.CheckOut();
                        clientContext.ExecuteQuery();
                        //lItem0["Title"] = "Payor Program Education Resources Coming Soon!";
                        lItem0["Title"] = "Payor Program Education Resources";
                        lItem0["ProgramNameText"] = practiceSite.NewSiteUrl + "/Pages/PayorEdResources.aspx";
                        lItem0["Thumbnail"] = practiceSite.NewSiteUrl + "/Program%20Participation/" + fileName0;
                        lItem0.Update();
                        lItem0.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("uploadProgramPracticeSupportFilesIwnPayorEd", ex.Message, "Error", "");
                }
            }
        }
        public static string GetRootSite(string url)
        {
            Uri uri = new Uri(url.TrimEnd(new[] { '/' }));
            return $"{uri.Scheme}://{ uri.DnsSafeHost}";
        }
    }
    public class PracticeInfo
    {
        public int GroupID { get; set; }
        public string ProgramManager { get; set; }
        public string SiteID { get; set; }
        public string PracticeName { get; set; }
        public string PracticeTIN { get; set; }
        public string PracticeNPI { get; set; }
        public string CKCCArea { get; set; }
        public int IWNRegion { get; set; }
        public int KC365 { get; set; }
        public string EncryptedPracticeTIN { get; set; }
        public PracticeInfo()
        {

        }
    }
}

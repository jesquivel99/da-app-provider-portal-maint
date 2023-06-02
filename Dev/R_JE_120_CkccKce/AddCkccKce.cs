using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using Serilog;
using SiteUtility;

namespace R_JE_120_CkccKce
{
    public class AddCkccKce
    {
        static Guid _listGuid = Guid.Empty;
        static string dateHrMin = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
        const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
        static ILogger _logger = Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("Logs/maint" + dateHrMin + "_.log", rollingInterval: RollingInterval.Day, shared: true, outputTemplate: outputTemp1)
           .CreateLogger();
        static ILogger logger = _logger.ForContext<AddCkccKce>();
        //const string LayoutsFolder = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
        static string LayoutsFolder = ConfigurationManager.AppSettings["LayoutsFolderDeploy"];
        static string LayoutsFolderImg = ConfigurationManager.AppSettings["LayoutsFolderImg"];
        readonly string EmailToMe = ConfigurationManager.AppSettings["EmailStatusToMe"];

        public void InitProg()
        {
            SiteInfoUtility siu = new SiteInfoUtility();
            SiteLogUtility slu = new SiteLogUtility();

            List<Practice> practices = siu.GetAllPractices();

            try
            {
                slu.LoggerInfo_Entry("========================================Release Starts========================================", true);

                if (practices != null && practices.Count > 0)
                {
                    foreach (Practice practice in practices)
                    {
                        Init_AddCkccKce(practice);
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
                SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", EmailToMe);
            }

            Log.CloseAndFlush();
        }
        public void InitProg(string siteId)
        {
            SiteInfoUtility siu = new SiteInfoUtility();
            SiteLogUtility slu = new SiteLogUtility();

            Practice practice = siu.GetPracticeBySiteID(siteId);

            try
            {
                slu.LoggerInfo_Entry("======================================== AddCkccKce Release Starts ========================================", true);

                if (practice != null && practice.IsCKCC)
                {
                    Init_AddCkccKce(practice);
                }
            }
            catch (Exception ex)
            {
                slu.LoggerInfo_Entry("Error: " + ex.Message, true);
            }
            finally
            {
                slu.LoggerInfo_Entry(SiteLogUtility.textLine0);
                slu.LoggerInfo_Entry("======================================== AddCkccKce Release Ends ========================================", true);
                SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", EmailToMe);
            }

            Log.CloseAndFlush();
        }

        private void Init_AddCkccKce(Practice practice)
        {
            SiteInfoUtility siu = new SiteInfoUtility();
            SiteLogUtility slu = new SiteLogUtility();
            try
            {
                siu.Init_UpdateAllProgramParticipation(practice);
                Init_DocUpload(practice);
                Init_UpdateProgramParticipation(practice);  // Program Participation Item Update - Img File...
                SiteInfoUtility.modifyWebPartProgramParticipation(practice.NewSiteUrl, practice);  // Resize...
                SiteFilesUtility.uploadMultiPartSupportingFilesAll(practice.NewSiteUrl, practice);  // JavaScript...

                Init_Benefit(practice);             // CKCC KCE Resources...
                Init_DataExchange(practice);
                Init_RiskAdjustment(practice);
                Init_Quality(practice);

                slu.LoggerInfo_Entry("Practice: " + practice.Name + " - " + practice.NewSiteUrl);

                SiteNavigateUtility.ClearQuickNavigationRecent(practice.NewSiteUrl);
                SiteNavigateUtility.RenameQuickNavigationNode(practice.NewSiteUrl, "Quality Coming Soon", "Quality");
            }
            catch (Exception ex)
            {
                slu.LoggerInfo_Entry(ex.Message, true);
                SiteLogUtility.CreateLogEntry("Init_AddCkccKce", ex.Message, "Error", "", true);
            }
        }

        private void Init_DialysisStarts(Practice practice, string layoutsFolder)
        {
            try
            {
                SiteFilesUtility sfUtility = new SiteFilesUtility();
                SitePublishUtility spUtility = new SitePublishUtility();
                SiteListUtility slu = new SiteListUtility();

                //spUtility.InitializePage(practice.NewSiteUrl, "PatientUpdates", "Patient Status Updates");
                //spUtility.DeleteWebPart(practice.NewSiteUrl, "PatientUpdates");
                //ConfigureDialysisStartsPage(psite.URL, urlSiteAssets, pageName);

                //uploadProgramPracticeSupportFilesDialysisStarts(psite);
                //modifyWebPartProgramParticipation(psite.URL, psite);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_DialysisStarts", ex.Message, "Error", "");
            }
        }

        private void Init_DocUpload(Practice practice)
        {
            SiteLogUtility slu = new SiteLogUtility();
            try
            {
                SiteFilesUtility sfu = new SiteFilesUtility();
                sfu.DocumentUpload(practice.NewSiteUrl, @LayoutsFolder + "cePrac_ProgramParTableData.html", "SiteAssets");
                sfu.DocumentUpload(practice.NewSiteUrl, @LayoutsFolderImg + "SW_RD_Referrals.jpg", "SiteAssets/Img");
                sfu.DocumentUpload(practice.NewSiteUrl, @LayoutsFolderImg + "MedicationAlerts.JPG", "SiteAssets/Img");
                sfu.DocumentUpload(practice.NewSiteUrl, @LayoutsFolderImg + "HospitalAlerts.jpg", "SiteAssets/Img");
                sfu.DocumentUpload(practice.NewSiteUrl, @LayoutsFolderImg + "Quality.jpg", "SiteAssets/Img");
            }
            catch (Exception ex)
            {
                slu.LoggerInfo_Entry(ex.Message, true);
                SiteLogUtility.CreateLogEntry("Init_DocUpload", ex.Message, "Error", "", true);
            }
        }
        private void Init_UpdateProgramParticipation(Practice practice)
        {
            try
            {
                if (practice.IsCKCC)
                {
                    // CKCC/KCE Resources...
                    SiteFilesUtility.updateProgramParticipation(practice.NewSiteUrl, SitePublishUtility.titleCkccKceResources,
                            SitePublishUtility.pageCkccKceResources, LayoutsFolderImg, SitePublishUtility.imgCkccKceResources);

                    // Patient Status Updates...
                    SiteFilesUtility.updateProgramParticipation(practice.NewSiteUrl, SitePublishUtility.titlePatientStatusUpdates,
                            SitePublishUtility.pagePatientStatusUpdates, LayoutsFolderImg, SitePublishUtility.imgPatientStatusUpdates); 
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_UpdateProgramParticipation", ex.Message, "Error", "");
                logger.Information(ex.Message);
            }
        }
        private static void Init_Quality(Practice practiceSite)
        {
            SiteLogUtility.Log_Entry("Init_Quality - In Progress...");
            bool ConfigSuccess = false;

            SiteFilesUtility sfu = new SiteFilesUtility();
            SitePublishUtility spUtility = new SitePublishUtility();
            SiteListUtility slUtility = new SiteListUtility();
            PracticeCView practiceCView = new PracticeCView();

            try
            {
                if (practiceSite.IsCKCC && SiteListUtility.DoesListExist(practiceSite.NewSiteUrl, slUtility.listNameQualityCkcc) == false)
                {
                    SiteListUtility.ProvisionList(practiceSite, slUtility, slUtility.listNameQualityCkcc, practiceCView);
                    SiteListUtility.CreateFolder(practiceSite, slUtility.listNameQualityCkcc, slUtility.listFolder1QualityCkcc);
                    SiteListUtility.CreateFolder(practiceSite, slUtility.listNameQualityCkcc, slUtility.listFolder2QualityCkcc);
                    SiteListUtility.CreateFolder(practiceSite, slUtility.listNameQualityCkcc, slUtility.listFolder3QualityCkcc);
                }

                spUtility.DeleteWebPart(practiceSite.NewSiteUrl, slUtility.pageNameQuality);
                sfu.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolder + "Quality_MultiTab.js", "SiteAssets");
                sfu.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolder + "jquery-ui.theme.css", "SiteAssets");
                sfu.uploadImageSupportingFilesSingleImage(practiceSite.NewSiteUrl, "Quality.jpg");
                sfu.uploadHtmlSupportingFilesSingleFile(practiceSite.NewSiteUrl, "cePrac_Quality.html");
                ConfigSuccess = SiteListUtility.ConfigureQualityPage(practiceSite.NewSiteUrl, practiceSite);
                if (ConfigSuccess)
                {
                    if (practiceSite.IsIWH)
                    {
                        SiteListUtility.modifyView(practiceSite.NewSiteUrl, slUtility.pageNameQuality + ".aspx", slUtility.webpartQualityIwh);
                    }
                    if (practiceSite.IsCKCC)
                    {
                        SiteListUtility.modifyView(practiceSite.NewSiteUrl, slUtility.pageNameQuality + ".aspx", slUtility.webpartQualityCkcc);
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

            SiteFilesUtility sfUtility = new SiteFilesUtility();
            SitePublishUtility spUtility = new SitePublishUtility();
            SiteListUtility slUtility = new SiteListUtility();
            PracticeCView practiceCView = new PracticeCView();

            try
            {
                if (practiceSite.IsCKCC && SiteListUtility.DoesListExist(practiceSite.NewSiteUrl, slUtility.listNameDataExchangeCkcc) == false)
                {
                    SiteListUtility.ProvisionList(practiceSite, slUtility, slUtility.listNameDataExchangeCkcc, practiceCView);
                    SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeCkcc, slUtility.listFolder1DataExchangeCkcc);
                    SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeCkcc, slUtility.listFolder2DataExchangeCkcc);
                    SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeCkcc, slUtility.listFolder3DataExchangeCkcc);
                    SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeCkcc, slUtility.listFolder4DataExchangeCkcc);
                    SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeCkcc, slUtility.listFolder5DataExchangeCkcc);
                    SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeCkcc, slUtility.listFolder6DataExchangeCkcc);
                    SiteListUtility.CreateFolder(practiceSite, slUtility.listNameDataExchangeCkcc, slUtility.listFolder7DataExchangeCkcc); 
                }


                //spUtility.InitializePage(practiceSite.URL, slUtility.pageNameDataExchange, slUtility.pageTitleDataExchange);
                spUtility.DeleteWebPart(practiceSite.NewSiteUrl, slUtility.pageNameDataExchange);
                sfUtility.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolder + "PracticeSiteTemplate_MultiTab.js", "SiteAssets");
                sfUtility.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolder + "jquery-ui.theme.css", "SiteAssets");
                ConfigSuccess = SiteListUtility.ConfigureDocumentExchangePage(practiceSite.NewSiteUrl, practiceSite);
                if (ConfigSuccess)
                {
                    if (practiceSite.IsIWH)
                    {
                        SiteListUtility.modifyView(practiceSite.NewSiteUrl, slUtility.pageNameDataExchange + ".aspx", slUtility.webpartDataExchangeIwh);
                    }
                    if (practiceSite.IsCKCC)
                    {
                        SiteListUtility.modifyView(practiceSite.NewSiteUrl, slUtility.pageNameDataExchange + ".aspx", slUtility.webpartDataExchangeCkcc);
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_DataExchange", ex.Message, "Error", "");
            }
        }
        private static void Init_RiskAdjustment(Practice practiceSite)
        {
            SiteLogUtility.Log_Entry("Init_RiskAdjustment - In Progress...");
            bool ConfigSuccess = false;
            PublishingPage PPage = null;

            SiteFilesUtility sfUtility = new SiteFilesUtility();
            SitePublishUtility spUtility = new SitePublishUtility();
            SiteListUtility slUtility = new SiteListUtility();
            PracticeCView practiceCView = new PracticeCView();

            try
            {
                if (practiceSite.IsCKCC && SiteListUtility.DoesListExist(practiceSite.NewSiteUrl, slUtility.listNameRiskAdjustmentCkcc) == false)
                {
                    SiteListUtility.ProvisionList(practiceSite, slUtility, slUtility.listNameRiskAdjustmentCkcc, practiceCView);
                    SiteListUtility.CreateFolder(practiceSite, slUtility.listNameRiskAdjustmentCkcc, slUtility.listFolder1RiskAdjustmentCkcc);
                    SiteListUtility.CreateFolder(practiceSite, slUtility.listNameRiskAdjustmentCkcc, slUtility.listFolder2RiskAdjustmentCkcc);
                    SiteListUtility.CreateFolder(practiceSite, slUtility.listNameRiskAdjustmentCkcc, slUtility.listFolder3RiskAdjustmentCkcc); 
                }

                //spUtility.InitializePage(practiceSite.URL, slUtility.pageNameRiskAdjustment, slUtility.pageTitleRiskAdjustment);
                spUtility.DeleteWebPart(practiceSite.NewSiteUrl, slUtility.pageNameRiskAdjustment);
                sfUtility.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolder + "RiskAdjustment.js", "SiteAssets");
                sfUtility.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolder + "jquery-ui.theme.css", "SiteAssets");
                ConfigSuccess = SiteListUtility.ConfigureRiskAdjustmentPage(practiceSite.NewSiteUrl, practiceSite);
                if (ConfigSuccess)
                {
                    if (practiceSite.IsIWH)
                    {
                        SiteListUtility.modifyView(practiceSite.NewSiteUrl, slUtility.pageNameRiskAdjustment + ".aspx", slUtility.webpartRiskAdjustmentIwh);
                    }
                    if (practiceSite.IsCKCC)
                    {
                        SiteListUtility.modifyView(practiceSite.NewSiteUrl, slUtility.pageNameRiskAdjustment + ".aspx", slUtility.webpartRiskAdjustmentCkcc);
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_RiskAdjustment", ex.Message, "Error", "");
            }
            //cntIsIwh++;
        }
        private static void Init_Benefit(Practice practiceSite)
        {
            SiteLogUtility.Log_Entry("Init_Benefit - In Progress...");
            bool ConfigSuccess = false;

            SiteFilesUtility sfu = new SiteFilesUtility();
            SitePublishUtility spu = new SitePublishUtility();
            SiteListUtility slu = new SiteListUtility();
            PracticeCView practiceCView = new PracticeCView();

            try
            {
                if (practiceSite.IsCKCC && SiteListUtility.DoesListExist(practiceSite.NewSiteUrl, slu.listNameBenefitEnhancementCkcc) == false)
                {
                    SiteListUtility.ProvisionList(practiceSite, slu, slu.listNameBenefitEnhancementCkcc, practiceCView);
                    SiteListUtility.CreateFolder(practiceSite, slu.listNameBenefitEnhancementCkcc, slu.listFolder1BenefitEnhancementCkcc);
                    SiteListUtility.CreateFolder(practiceSite, slu.listNameBenefitEnhancementCkcc, slu.listFolder2BenefitEnhancementCkcc); 
                }

                if (!SiteFilesUtility.FileExists(practiceSite.NewSiteUrl, "Pages", slu.pageNameBenefitEnhancement + ".aspx"))
                {
                    spu.InitializePage(practiceSite.NewSiteUrl, slu.pageNameBenefitEnhancement, slu.pageTitleBenefitEnhancement);
                }
                spu.DeleteWebPart(practiceSite.NewSiteUrl, slu.pageNameBenefitEnhancement);
                sfu.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolder + "BenefitEnhancement_MultiTab.js", "SiteAssets");
                sfu.DocumentUpload(practiceSite.NewSiteUrl, LayoutsFolder + "jquery-ui.theme.css", "SiteAssets");
                ConfigSuccess = SiteListUtility.ConfigureBenefitEnhancementPage(practiceSite.NewSiteUrl, practiceSite);
                if (ConfigSuccess)
                {
                    if (practiceSite.IsCKCC)
                    {
                        SiteListUtility.modifyView(practiceSite.NewSiteUrl, slu.pageNameBenefitEnhancement + ".aspx", slu.webpartBenefitEnhancementCkcc);
                    }
                }
                SiteFilesUtility.SP_Update_ProgramParticipation(practiceSite.NewSiteUrl, slu.pageNameBenefitEnhancement, "CKCC/KCE Coming Soon", "CKCC/KCE Resources", "KCEckcc.JPG");
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_Benefit", ex.Message, "Error", "");
            }
            //cntIsCkcc++;
        }
    }
}

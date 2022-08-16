using Microsoft.SharePoint.Client;
using Serilog;
using SiteUtility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace R_1_9_AddSortColumn
{
    class Program
    {
        public static Guid _listGuid = Guid.Empty;
        public static List<ProgPart> progParts = new List<ProgPart>();
        public static List<string> listEmail = new List<string>();
        static string dateHrMin = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
        static ILogger logger;
        
        // Used for workaround...
        static public List<Practice> practicesIWH = new List<Practice>();
        static public List<Practice> practicesCKCC = new List<Practice>();
        static public string runPM = "PM04";                // Use as a workaround...
        static public string runPractice = "99590861659";  // Use as a workaround...

        static void Main(string[] args)
        {
            #region LoggerRegion
            const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
            logger = Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .WriteTo.File("Logs/maint" + dateHrMin + "_.log", rollingInterval: RollingInterval.Day, shared: false, outputTemplate: outputTemp1)
               .CreateLogger();

            logger = Log.ForContext<Program>(); 
            #endregion

            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            SiteFilesUtility sfu = new SiteFilesUtility();
            SiteInfoUtility siteInfo = new SiteInfoUtility();
            int CntPrac = 0;

            // Uncomment one of these unless using workaround...
            //List<Practice> practices = siteInfo.GetAllPractices();
            List<Practice> practices = siteInfo.GetPracticesByPM("01");
            //Practice practice = siteInfo.GetPracticeBySiteID("93034041279");

            // Used for workaround...
            //SiteInfoUtility_2 siteInfo_2 = new SiteInfoUtility_2();               // Only use as a workaround ELSE COMMENT...
            //Practice practice = siteInfo_2.GetPracticeBySiteID(runPractice);      // Only use as a workaround ELSE COMMENT...

            LoggerInfo_Entry("========================================Release Starts========================================");

            try
            {
                LoggerInfo_Entry("-------------[ Get Program Participation Sort Order ]-------------", true);
                GetSortParameters("ProgPartSort");

                LoggerInfo_Entry("-------------[ Maintenance Tasks - Start            ]-------------", true);
                //if (practice != null)
                if (practices != null && practices.Count > 0)
                {
                    foreach (Practice practice in practices)
                    {
                        {
                            LoggerInfoBody(practice);

                            bool listExist = DoesListExist(practice, "Program Participation");
                            if (!listExist)
                            {
                                LoggerInfo_Entry("    List Does NOT Exist");
                                continue;
                            }

                            ProvisionField(practice, "Program Participation");
                            UpdateSortCol(practice, "Program Participation");
                            sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_ProgramParTableData.html");
                        }
                    }
                }
                LoggerInfo_Entry("-------------[ Maintenance Tasks - End              ]-------------", true);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
            }
            finally
            {
                LoggerInfo_Entry(SiteLogUtility.textLine0);
                LoggerInfo_Entry("Total Practice Count: " + CntPrac, true);
                LoggerInfo_Entry(SiteLogUtility.textLine0);
                LoggerInfo_Entry("========================================Release Ends========================================", true);
                SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@freseniusmedicalcare.com");
            }
            LoggerInfo_Entry("========================================Release Ends========================================", true);
            Log.CloseAndFlush();
        }

        private static void LoggerInfoBody(Practice practice)
        {
            LoggerInfo_Entry(SiteLogUtility.textLine0, true);
            LoggerInfo_Entry("       Prac Url: " + practice.NewSiteUrl);
            LoggerInfo_Entry("  practice Name: " + practice.Name);
        }

        private static void LoggerInfo_Entry(string logtext, bool consolePrint = false)
        {
            logger.Information(logtext);
            SiteLogUtility.LogList.Add(logtext);
            if (consolePrint)
            {
                Console.WriteLine(logtext);
            }
        }

        public void InitiateProg()
        {
            //string releaseName = "SiteUtilityTest";
            //string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            //string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];


            //string runPM = "PM01";
            //string runPractice = "94910221369";
            //string urlAdminGroup = siteUrl + "/" + runPM;



            //SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            //logger.Information("\n\n=============Release Starts=============");
            ////SiteLogUtility.Log_Entry("\n\n=============Release Starts=============", true);

            //using (ClientContext clientContext = new ClientContext(siteUrl))
            //{
            //    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

            //    try
            //    {
            //        SiteLogUtility.Log_Entry("\n\n=============[ Get Program Participation Sort Order ]=============", true);
            //        GetSortParameters("ProgPartSort");

            //        SiteLogUtility.Log_Entry("\n\n=============[ Get PM AdminGroup ]=============", true);
            //        SiteLogUtility.Log_Entry("Processing AdminGroup:  " + urlAdminGroup, true);
            //        _logger.Information("Processing AdminGroup:  " + urlAdminGroup);
            //        List<PMData> pmData = SiteInfoUtility.initPMDataToList(urlAdminGroup);

            //        SiteLogUtility.Log_Entry("\n\n=============[ Get all Portal Practice Data ]=============", true);
            //        List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext);

            //        SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - Start]=============", true);
            //        foreach (ProgramManagerSite pm in practicePMSites)
            //        {
            //            SiteLogUtility.Log_Entry("\nPM Site: " + pm.PracticeName + " - " + pm.PMURL, true);
            //            foreach (PracticeSite psite in pm.PracticeSiteCollection)
            //            {
            //                //if (psite.URL.Contains(runPM))
            //                if (psite.URL.Contains(runPM) && psite.URL.Contains(runPractice))
            //                {
            //                    bool listExist = DoesListExist(psite, "Program Participation");
            //                    ListAddColumn(psite, "Program Participation");
            //                    UpdateSortCol(psite, "Program Participation");


            //                    // get list
            //                    // check if column exists
            //                    // add column
            //                    // update all items with sort values
            //                    // refresh the view; deploy html 
            //                }
            //            }
            //        }
            //        SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - End]=============", true);
            //    }
            //    catch (Exception ex)
            //    {
            //        SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
            //    }
            //    finally
            //    {
            //        SiteLogUtility.Log_Entry(SiteLogUtility.textLine0, true);
            //        SiteLogUtility.finalLog(releaseName);
            //        SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@freseniusmedicalcare.com");
            //    }
            //    SiteLogUtility.Log_Entry("=============Release Ends=============", true);
            //}
        }
        private static void UpdateSortCol(Practice psite, string strList)
        {
            //ILogger _logger1 = Log.ForContext<Program>();
            try
            {
                using (ClientContext clientContext = new ClientContext(psite.NewSiteUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle(strList);
                    var items = list.GetItems(CamlQuery.CreateAllItemsQuery());

                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    foreach (var item in items)
                    {
                        var fndTitle = item["Title"].ToString();
                        //int sortOrder = GetProgramParticipationSortOrder(fndTitle);
                        ProgPart progPart = progParts.Where(x => x.ColName.Contains(fndTitle)).FirstOrDefault();
                        item["Sort_Order"] = progPart.SortOrder;
                        item.Update();
                        clientContext.ExecuteQuery();
                        LoggerInfo_Entry($">>> {item["Title"]} - Sort Order = {item["Sort_Order"]}", true);
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("UpdateSortCol", ex.Message, "Error", "");
            }
        }
        private string GetProgramParticipationImg(string fndTitle)
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
        private static void GetSortParameters(string paramGroup)
        {
            try
            {
                //NameValueCollection listSection = ConfigurationManager.GetSection(paramGroup) as NameValueCollection;
                NameValueCollection listSection = (NameValueCollection)ConfigurationManager.GetSection(paramGroup);
                string[] listSectionKeys = listSection.AllKeys;

                foreach (var key in listSectionKeys)
                {
                    ProgPart progPart = new ProgPart();
                    progPart.ColName = key;
                    progPart.SortOrder = Convert.ToInt32(listSection[key]);
                    LoggerInfo_Entry(key + " - " + listSection[key]);
                    progParts.Add(progPart);
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetSortParameters", ex.Message, "Error", "");
            }
        }
        private static void ListAddColumn(Practice psite, string strList)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(psite.NewSiteUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle(strList);
                    Field field = list.Fields.AddFieldAsXml("<Field Type='Number' DisplayName='Sort_Order' Name='Sort_Order' />", true, AddFieldOptions.AddFieldInternalNameHint);

                    clientContext.Load(field);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("ListAddColumn", ex.Message, "Error", "");
            }
        }
        private static bool DoesListExist(Practice psite, string listName)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(psite.NewSiteUrl))
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
                                    //LoggerInfo_Entry(psite.Name + " - " + psite.NewSiteUrl);
                                    break;
                                }
                            }
                        }

                        return bListFound;
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("DoesListExist", ex.Message, "Error", "");
                LoggerInfo_Entry("ERROR - DoesListExist: " + ex.Message, true);
            }
            return false;
        }
        private static bool DoesListExistGetGuid(string wUrl, string listName)
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
        public static void ProvisionField(Practice psite, string listTitle)
        {
            string _wUrl = psite.NewSiteUrl;
            //Guid _listGuid = listGuid;
            SiteListUtility slu = new SiteListUtility();

            using (ClientContext clientContext = new ClientContext(_wUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                {
                    try
                    {
                        Web w = clientContext.Web;
                        //List list = w.Lists.GetById(_listGuid);
                        List list = w.Lists.GetByTitle(listTitle);
                        //Field fd = list.Fields.GetByTitle("Sort_Order");
                        FieldCollection collFd = list.Fields;
                        clientContext.Load(w);
                        clientContext.Load(list);
                        clientContext.Load(collFd);
                        clientContext.ExecuteQuery();

                        bool fieldFound = false;
                        foreach (Field f in collFd)
                        {
                            //LoggerInfo_Entry(f.Title);
                            if (f.Title == "Sort_Order")
                            {
                                fieldFound = true;
                                LoggerInfo_Entry("Field Found: Sort_Order", true);
                                break;
                            }
                        }

                        if (fieldFound == false)
                        {
                            LoggerInfo_Entry("Field NOT Found - Creating: Sort_Order", true);
                            slu.CreateListColumn("<Field Type='Number' DisplayName='Sort_Order' Name='Sort_Order' />", listTitle, _wUrl);
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

        //private void EnsureFieldDisplayName()
        //{
        //    using (ClientContext clientContext = new ClientContext(_wUrl))
        //    {
        //        clientContext.Credentials = new NetworkCredential(SpCredential.UserName, SpCredential.Password, SpCredential.Domain);
        //        {
        //            Web w = clientContext.Web;
        //            List l = w.Lists.GetById(_listGuid);
        //            Field f = l.Fields.GetByInternalNameOrTitle(FieldName);
        //            f.Title = DisplayName;
        //            f.Update();
        //            l.Update();
        //            w.Update();
        //            clientContext.ExecuteQuery();
        //        }
        //    }
        //}
        public class ProgPart
        {
            public ProgPart()
            {
            }
            public string ColName { get; set; }
            public int SortOrder { get; set; }
        }

        public class SiteInfoUtility_2
        {
            public List<Practice> AllPractices;
            string strPortalSiteURL = ConfigurationManager.AppSettings["SP_SiteUrl"];

            public SiteInfoUtility_2()
            {
                AllPractices = new List<Practice>();

                // Read All Practice Info from Webs...
                try
                {
                    using (ClientContext clientContext = new ClientContext(strPortalSiteURL))
                    {
                        clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                        try
                        {
                            Practice practice = new Practice();
                            string urlAdminGroup = strPortalSiteURL + "/" + runPM;
                            logger.Information("-------------[ Processing AdminGroup:  " + urlAdminGroup + "  ]-------------");
                            List<PMData> pmData = SiteInfoUtility.initPMDataToList(urlAdminGroup);

                            logger.Information("-------------[ Get all Portal Practice Data         ]-------------");
                            List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext, practicesIWH, practicesCKCC, pmData);

                            logger.Information("-------------[ Adding Data to Practice - Start            ]-------------");
                            foreach (ProgramManagerSite pm in practicePMSites)
                            {
                                foreach (PracticeSite psite in pm.PracticeSiteCollection)
                                {
                                    if (psite.URL.Contains(runPM))
                                    //if (psite.URL.Contains(runPM) && psite.URL.Contains(runPractice))
                                    {
                                        practice.PMGroup = psite.ProgramManager;

                                        practice.PMName = pm.ProgramManagerName;
                                        practice.Name = psite.Name;
                                        practice.SiteID = psite.SiteId;
                                        practice.TIN = psite.PracticeTIN;
                                        practice.NPI = psite.PracticeNPI;
                                        practice.NewSiteUrl = psite.URL;

                                        practice.CKCCArea = "";

                                        if (practice.CKCCArea == "")
                                            practice.IsCKCC = false;
                                        else
                                            practice.IsCKCC = true;

                                        practice.IsCKCC = psite.IsCKCC.Equals("true") ? true : false;
                                        practice.IsIWH = psite.IsIWH.Equals("true") ? true : false;
                                        practice.IsKC365 = psite.IsKC365.Equals("true") ? true : false;
                                        practice.IsTelephonic = psite.IsTeleKC365.Equals("true") ? true : false;

                                        practice.MedicalDirector = "";

                                        AllPractices.Add(practice);
                                    }
                                }
                            }
                            logger.Information("-------------[ Adding Data to Practice - End              ]-------------");
                        }
                        catch (Exception ex)
                        {
                            logger.Error("Error: " + ex.Message);
                        }
                        finally
                        {
                            logger.Information(SiteLogUtility.textLine0);
                            logger.Information("Total Practices: " + AllPractices.Count());
                            //SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@freseniusmedicalcare.com");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            public Practice GetPracticeBySiteID(string siteID)
            {
                return AllPractices.Where(p => p.SiteID == siteID).FirstOrDefault();
            }
        }
    }
}

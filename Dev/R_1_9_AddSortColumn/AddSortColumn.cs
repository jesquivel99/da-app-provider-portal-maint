using Microsoft.SharePoint.Client;
using Serilog;
using SiteUtility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;


namespace R_JE_109_AddSortColumn
{
    public class AddSortColumn
    {
        public static Guid _listGuid = Guid.Empty;
        public static List<ProgPart> progParts = new List<ProgPart>();
        static string dateHrMin = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
        static ILogger logger;
        readonly string EmailToMe = ConfigurationManager.AppSettings["EmailStatusToMe"];

        public void InitiateProg()
        {
            #region LoggerRegion
            const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
            logger = Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .WriteTo.File("Logs/maint" + dateHrMin + "_.log", rollingInterval: RollingInterval.Day, shared: false, outputTemplate: outputTemp1)
               .CreateLogger();

            logger = Log.ForContext<AddSortColumn>(); 
            #endregion

            SiteFilesUtility sfu = new SiteFilesUtility();
            SiteInfoUtility siteInfo = new SiteInfoUtility();
            int CntPrac = 0;
            int CntNoPrac = 0;

            List<Practice> practices = siteInfo.GetPracticesByPM("10");

            try
            {
                LoggerInfo_Entry("-------------[ Get Program Participation Sort Order ]-------------", true);
                GetSortParameters("ProgPartSort");

                LoggerInfo_Entry("-------------[ Deployment Started            ]-------------", true);
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
                                CntNoPrac++;
                                continue;
                            }

                            ProvisionField(practice, "Program Participation");
                            UpdateSortCol(practice, "Program Participation");
                            sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_ProgramParTableData.html");
                            CntPrac++;
                        }
                    }
                }
                LoggerInfo_Entry("-------------[ Deployment Completed              ]-------------", true);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
            }
            finally
            {
                LoggerInfo_Entry(SiteLogUtility.textLine0);
                LoggerInfo_Entry("Total Practice Count: " + CntPrac, true);
                LoggerInfo_Entry("Total Practice Did Not Exist Count: " + CntNoPrac, true);
                LoggerInfo_Entry(SiteLogUtility.textLine0);
                SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", EmailToMe);
            }
            Log.CloseAndFlush();
        }
        public void InitiateProg(string siteID)
        {
            #region LoggerRegion
            const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
            logger = Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .WriteTo.File("Logs/maint" + dateHrMin + "_.log", rollingInterval: RollingInterval.Day, shared: false, outputTemplate: outputTemp1)
               .CreateLogger();

            logger = Log.ForContext<AddSortColumn>();
            #endregion

            SiteInfoUtility siteInfo = new SiteInfoUtility();
            int CntPrac = 0;

            LoggerInfo_Entry("-------------[ AddSortColumn Deployment Started            ]-------------", true);

            Practice practice = siteInfo.GetPracticeBySiteID(siteID);
            if (practice != null)
            {
                try
                {
                    SiteFilesUtility sfu = new SiteFilesUtility();
                    LoggerInfo_Entry("-------------[ Get Program Participation Sort Order ]-------------", true);
                    GetSortParameters("ProgPartSort");

                    LoggerInfoBody(practice);

                    bool listExist = DoesListExist(practice, "Program Participation");
                    if (!listExist)
                    {
                        LoggerInfo_Entry("    List Does NOT Exist");
                        return;
                    }

                    ProvisionField(practice, "Program Participation");
                    UpdateSortCol(practice, "Program Participation");
                    sfu.uploadHtmlSupportingFilesSingleFile(practice.NewSiteUrl, "cePrac_ProgramParTableData.html");
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("AddSortColumn", ex.Message, "Error", "");
                }
                finally
                {
                    LoggerInfo_Entry(SiteLogUtility.textLine0);
                    LoggerInfo_Entry("Total Practice Count: " + CntPrac, true);
                    LoggerInfo_Entry(SiteLogUtility.textLine0);
                    LoggerInfo_Entry("-------------[ AddSortColumn Deployment Started            ]-------------", true);

                    //SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@freseniusmedicalcare.com");
                }
                Log.CloseAndFlush();
            }
            LoggerInfo_Entry("-------------[ Deployment Completed              ]-------------", true);
        }
        private static void LoggerInfoBody(Practice practice)
        {
            LoggerInfo_Entry(SiteLogUtility.textLine0, true);
            LoggerInfo_Entry("       Prac Url: " + practice.NewSiteUrl);
            LoggerInfo_Entry("  practice Name: " + practice.Name);
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
            }
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
        public class ProgPart
        {
            public ProgPart()
            {
            }
            public string ColName { get; set; }
            public int SortOrder { get; set; }
        }
    }
}

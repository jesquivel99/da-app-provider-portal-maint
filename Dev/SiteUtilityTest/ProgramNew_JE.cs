using System;
using SiteUtility;
using System.Configuration;
using Microsoft.SharePoint.Client;
using System.Net;
using Serilog;
using System.Collections.Generic;

namespace SiteUtilityTest
{
    public class ProgramNew_JE
    {
        const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
        static ILogger _logger = Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("Logs/maint" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + "_.log", rollingInterval: RollingInterval.Day, shared: false, outputTemplate: outputTemp1)
           .CreateLogger();
        static ILogger logger = _logger.ForContext<ProgramNew_JE>();
        private Guid _listGuid = Guid.Empty;
        public void InitiateProg()
        {
            string releaseName = "SiteUtilityTest";
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();
            SiteLogUtility siteLogUtility = new SiteLogUtility();

            //List<Practice> practices = siteInfoUtility.GetAllCKCCPractices();
            List<Practice> practices = siteInfoUtility.GetPracticesByPM("01");

            //SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            siteLogUtility.LoggerInfo_Entry("This is the Release Name: " + releaseName);
            siteLogUtility.LoggerInfo_Entry("========================================Release Starts========================================");

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    siteLogUtility.LoggerInfo_Entry("-------------[ Maintenance Tasks - Start            ]-------------");

                    foreach (Practice practice in practices)
                    {
                        // Build xml configuration file...
                        //SiteUtility.SitePMData.InitialConnectDBPortalDeployed("PM06");
                        
                        siteLogUtility.LoggerInfoBody(practice);

                        //SiteNavigateUtility.ClearQuickNavigationRecent(practice.NewSiteUrl);
                        SiteNavigateUtility.RenameQuickNavigationNode(practice.NewSiteUrl, "Hospitalization Alert", "Hospitalization Alerts");
                    }

                    siteLogUtility.LoggerInfo_Entry("-------------[ Maintenance Tasks - End              ]-------------");

                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
                    logger.Error("Error: " + ex.Message);
                }
                finally
                {
                    siteLogUtility.LoggerInfo_Entry(SiteLogUtility.textLine0);
                    //SiteLogUtility.finalLog("Final: " + releaseName);
                    SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@interwellhealth.com");
                }
                siteLogUtility.LoggerInfo_Entry("========================================Release Ends========================================");
            }

            Log.CloseAndFlush();
        }

        public void GetListGuid(string wUrl, string listName)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(wUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    
                    List list = clientContext.Web.Lists.GetByTitle(listName);
                    clientContext.Load(list, o => o.Id, o => o.ContentTypes);
                    clientContext.ExecuteQuery();
                    if (list.Id != Guid.Empty)
                    {
                        _listGuid = list.Id;
                    }
                }
                
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetListGuid", ex.Message, "Error", "");
            }
        }

        public void GetListContentTypes(string wUrl, string listName)
        {
            GetListGuid(wUrl, listName);

            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                ContentTypeCollection contentTypes = clientContext.Web.AvailableContentTypes;
                ListCollection lists = clientContext.Web.Lists;
                Web web = clientContext.Web;
                clientContext.Load(web);
                clientContext.Load(lists);
                clientContext.Load(contentTypes);
                clientContext.ExecuteQuery();

                List list1 = lists.GetById(_listGuid);
                ContentType contentType;
                clientContext.Load(list1);
                clientContext.ExecuteQuery();

                if (DoesContentType_Exist(list1.ContentTypes, "Text"))
                {
                    contentType = RetrieveExistingContentType(list1.ContentTypes, "Text");
                }
            }
        }
        public bool DoesContentType_Exist(ContentTypeCollection spc, string name)
        {
            foreach (ContentType c in spc)
            {
                if (c.Name.ToLower() == name.ToLower())
                {
                    return true;
                }
            }

            return false;
        }
        public ContentType RetrieveExistingContentType(ContentTypeCollection spc, string name)
        {
            foreach (ContentType c in spc)
            {
                if (c.Name.ToLower() == name.ToLower())
                {
                    return c;
                }
            }

            return null;
        }
    }
}

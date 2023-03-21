using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiteUtility;
using Serilog;

namespace R_JE_110_Init_UpdateProgramParticipation
{
    public class UpdateProgramParticipation
    {
        static readonly string dateHrMin = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
        static ILogger logger;
        public void InitProg()
        {
            #region LoggerRegion
            const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
            logger = Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .WriteTo.File("Logs/maint" + dateHrMin + "_.log", rollingInterval: RollingInterval.Day, shared: true, outputTemplate: outputTemp1)
               .CreateLogger();

            logger = Log.ForContext<UpdateProgramParticipation>();
            #endregion

            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();
            SiteLogUtility siteLogUtility = new SiteLogUtility();

            int CntPrac = 0;

            //List<Practice> practices = siteInfoUtility.GetAllPractices();
            List<Practice> practices = siteInfoUtility.GetPracticesByPM("01");

            try
            {
                siteLogUtility.LoggerInfo_Entry("-------------[ Deployment Started            ]-------------", true);
                if (practices != null && practices.Count > 0)
                {
                    foreach (Practice practice in practices)
                    {
                        {
                            {
                                siteLogUtility.LoggerInfoBody(practice);
                                siteInfoUtility.Init_UpdateAllProgramParticipation(practice);
                            }
                            CntPrac++;
                        }
                    }
                }
                siteLogUtility.LoggerInfo_Entry("-------------[ Deployment Completed              ]-------------", true);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("UpdateProgramParticipation - InitProg()", ex.Message, "Error", "");
            }
            finally
            {
                siteLogUtility.LoggerInfo_Entry(SiteLogUtility.textLine0);
                siteLogUtility.LoggerInfo_Entry("Total Practice Count: " + CntPrac, true);
                siteLogUtility.LoggerInfo_Entry(SiteLogUtility.textLine0);
                siteLogUtility.LoggerInfo_Entry("========================================Release Ends========================================", true);
                SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@interwellhealth.com");
            }

            Log.CloseAndFlush();
        }
        public void InitProg(string siteId)
        {
            #region LoggerRegion
            const string outputTemp1 = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
            logger = Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .WriteTo.File("Logs/maint" + dateHrMin + "_.log", rollingInterval: RollingInterval.Day, shared: true, outputTemplate: outputTemp1)
               .CreateLogger();

            logger = Log.ForContext<UpdateProgramParticipation>();
            #endregion

            SiteLogUtility siteLogUtility = new SiteLogUtility();
            SiteInfoUtility siteInfoUtility = new SiteInfoUtility();
            
            Practice practice = siteInfoUtility.GetPracticeBySiteID(siteId);

            try
            {
                siteLogUtility.LoggerInfo_Entry("-------------[ Deployment Started            ]-------------", true);
                if (practice != null)
                {
                    siteLogUtility.LoggerInfoBody(practice);
                    siteInfoUtility.Init_UpdateAllProgramParticipation(practice);
                }
                siteLogUtility.LoggerInfo_Entry("-------------[ Deployment Completed              ]-------------", true);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("UpdateProgramParticipation - InitProg(siteId)", ex.Message, "Error", "");
            }
            finally
            {
                siteLogUtility.LoggerInfo_Entry(SiteLogUtility.textLine0);
                siteLogUtility.LoggerInfo_Entry("========================================Release Ends========================================", true);
                //SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@freseniusmedicalcare.com");
            }

            Log.CloseAndFlush();
        }

    }
}

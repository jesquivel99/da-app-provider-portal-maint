using R_DW_100_CarePlanHtmlUpdate;
using R_JE_100_MovePractice;
using R_JE_109_AddSortColumn;


namespace SiteUtilityTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //CarePlanHtmlUpdate carePlanHtmlUpdate = new CarePlanHtmlUpdate();
            //carePlanHtmlUpdate.InitiateProg();

            AddSortColumn addSortColumn = new AddSortColumn();
            addSortColumn.InitiateProg("94711764549");

            //--------------------------------------------------------
            // Run Maintenance Code to Complete a new site Deployment
            //--------------------------------------------------------
            // CompleteNewSiteDeployment("siteID");


            //const string outputTemp = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
            //ILogger logger = Log.Logger = new LoggerConfiguration()
            //   .MinimumLevel.Debug()
            //   .Enrich.FromLogContext()
            //   .WriteTo.Console()
            //   .WriteTo.File("Logs/Nabeel/ex_.log", rollingInterval: RollingInterval.Day, shared: true, outputTemplate: outputTemp)
            //   .CreateLogger();
            //objProgramNew test = new SiteInfoUtilityTest();
            //objProgramNew.InitiateProg();
            //objProgramNew.ReferralSetup();

            //ProgramNew_SS objProgramNew2 = new ProgramNew_SS();
            //objProgramNew2.InitiateProg();

            //ProgramNew_AA objProgramNew_AA = new ProgramNew_AA();
            //objProgramNew_AA.InitiateProg();

            //ProgramNew_NA objProgramNew_NA = new ProgramNew_NA(logger);
            //objProgramNew_NA.InitiateProg();

        }
        static void CompleteNewSiteDeployment(string siteID)
        {
            //DialysisStart dialysisStart = new DialysisStart();
            //dialysisStart.InitiateProg(siteID);
            //BenefitQualityPayor benefitQualityPayor = new BenefitQualityPayor();
            //benefitQualityPayor.InitiateProg(siteID);
            //CarePlanHtmlUpdate carePlanHtmlUpdate = new CarePlanHtmlUpdate();
            //carePlanHtmlUpdate.InitiateProg(siteID);
        }
    }
}

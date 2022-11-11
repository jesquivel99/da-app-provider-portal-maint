using R_DW_100_CarePlanHtmlUpdate;
using R_JE_100_MovePractice;
using R_JE_109_AddSortColumn;
using R_1_10_CkccEngagement;
using R_1_11_IWH;
using R_JE_110_Init_UpdateProgramParticipation;


namespace SiteUtilityTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //CarePlanHtmlUpdate carePlanHtmlUpdate = new CarePlanHtmlUpdate();
            //carePlanHtmlUpdate.InitiateProg();

            //MovePractice movePractice = new MovePractice();
            //movePractice.InitiateProg("94711764549");

            UpdateProgramParticipation updateProgramParticipation = new UpdateProgramParticipation();
            updateProgramParticipation.InitProg();

            //--------------------------------------------------------
            // Run Maintenance Code to Complete a new site Deployment
            //--------------------------------------------------------
            //CompleteNewSiteDeployment("98357241959");


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
            //AddIWH addIWH = new AddIWH();
            //addIWH.InitProg(siteID);
            //AddSortColumn addSortColumn = new AddSortColumn();
            //addSortColumn.InitiateProg(siteID);
            //CkccEngagement ckccEngagement = new CkccEngagement();
            //ckccEngagement.InitiateProg(siteID);
            //DialysisStart dialysisStart = new DialysisStart();
            //dialysisStart.InitiateProg(siteID);
            //BenefitQualityPayor benefitQualityPayor = new BenefitQualityPayor();
            //benefitQualityPayor.InitiateProg(siteID);
            //CarePlanHtmlUpdate carePlanHtmlUpdate = new CarePlanHtmlUpdate();
            //carePlanHtmlUpdate.InitiateProg(siteID);
        }
    }
}

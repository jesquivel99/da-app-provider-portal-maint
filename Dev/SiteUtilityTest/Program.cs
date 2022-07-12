using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteUtility;
using System.Configuration;
using Microsoft.SharePoint.Client;
using System.Net;
using Serilog;

namespace SiteUtilityTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //const string outputTemp = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}";
            //ILogger logger = Log.Logger = new LoggerConfiguration()
            //   .MinimumLevel.Debug()
            //   .Enrich.FromLogContext()
            //   .WriteTo.Console()
            //   .WriteTo.File("Logs/Nabeel/ex_.log", rollingInterval: RollingInterval.Day, shared: true, outputTemplate: outputTemp)
            //   .CreateLogger();
            ProgramNew objProgramNew = new ProgramNew();
            objProgramNew.InitiateProg();
            //objProgramNew.ReferralSetup();
            

            //ProgramNew2 objProgramNew2 = new ProgramNew2();
            //objProgramNew2.InitiateProgNew2();

            //ProgramNew_AA objProgramNew_AA = new ProgramNew_AA();
            //objProgramNew_AA.InitiateProg();

            //ProgramNew_JE objProgramNew_JE = new ProgramNew_JE();
            //objProgramNew_JE.InitiateProg();

            //ProgramNew_NA objProgramNew_NA = new ProgramNew_NA(logger);
            //objProgramNew_NA.InitiateProg();


            //SitePMData.initialConnectDBPortal("02");
            //Log.CloseAndFlush();
        }
    }
}

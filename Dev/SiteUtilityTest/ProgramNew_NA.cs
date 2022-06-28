using Microsoft.SharePoint.Client;
using Serilog;
using SiteUtility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SiteUtilityTest
{
    class ProgramNew_NA
    {
        private readonly ILogger _logger;
        ////dev
        //string rootUrl = "https://sharepointdev.fmc-na-icg.com";
        //string strPortalSiteURL = "https://sharepointdev.fmc-na-icg.com/bi/fhppp/portal";
        //string strReferralURL = "https://sharepointdev.fmc-na-icg.com/bi/fhppp/interimckcc/referral"; //NO SLASH AT THE END

        ///*
        ////PROD
        //string rootUrl = "http://vh2-sp-01/";
        //string strPortalSiteURL = "http://vh2-sp-01/bi/fhppp/portal";
        //string strReferralURL = "http://vh2-sp-01/bi/fhppp/portal/referral"; //NO SLASH AT THE END
        //*/

        //string ResultLog = "=============Release Starts=============\r\n";
        //string textLine = "\r\n=======================================\r\n";

        public ProgramNew_NA(ILogger logger)
        {
            _logger = logger.ForContext<ProgramNew_NA>();
        }

        public void InitiateProg()
        {
            _logger.Information("InitiateProg() started...");
        }

       

        
    }
}

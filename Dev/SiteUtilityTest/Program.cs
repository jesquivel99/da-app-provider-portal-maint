using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteUtility;
using System.Configuration;
using Microsoft.SharePoint.Client;
using System.Net;

namespace SiteUtilityTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ProgramNew objProgramNew = new ProgramNew();
            //objProgramNew.InitiateProg();
            objProgramNew.ReferralSetup();

            //ProgramNew2 objProgramNew2 = new ProgramNew2();
            //objProgramNew2.InitiateProgNew2();
        }
    }
}

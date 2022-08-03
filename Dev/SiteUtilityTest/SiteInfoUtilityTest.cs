using System;
using System.Collections.Generic;
using Serilog;
using SiteUtility;

namespace SiteUtilityTest
{
    class SiteInfoUtilityTest
    {
        public SiteInfoUtilityTest()
        {
            SiteInfoUtility siteInfo = new SiteInfoUtility();

            //List<Practice> practices = siteInfo.GetAllPractices();
            //List<Practice> practices = siteInfo.GetAllCKCCPractices();
            //List<Practice> practices = siteInfo.GetAllIWHPractices();
            List<Practice> practices = siteInfo.GetAllKC365Practices();
            //List<Practice> practices = siteInfo.GetAllTelephonicPractices();
            //List<Practice> practices = siteInfo.GetAllMedicalDirectorPractices();
            if (practices != null && practices.Count > 0)
            {
                foreach (Practice practice in practices)
                {
                    // Do something to the practice
                }
            }
            //Practice practice = siteInfo.GetPracticeByTIN("493796062");
            //Practice practice = siteInfo.GetPracticeByNPI("12345678901");
            //Practice practice = siteInfo.GetPracticeBySiteID("94937960629");
            //if (practice != null)
            //{
            //    // Do something to the practice
            //}
        }
    }
}

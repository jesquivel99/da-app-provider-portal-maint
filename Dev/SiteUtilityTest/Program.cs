using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteUtility;

namespace SiteUtilityTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SiteLogUtility.LogFunction1(); 
            SiteLogUtility.LogFunction2(); 
            SitePublishUtility.PublishFunction1();
            SitePublishUtility.PublishFunction2();
            SiteListUtility.ListFunction1();
            SiteListUtility.ListFunction2();
        }
    }
}

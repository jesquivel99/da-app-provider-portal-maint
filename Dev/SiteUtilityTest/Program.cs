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
            // APPROACH:
            //  1. Get all subwebs...populate the appropriate classes
            //  2. Get all existing objects/assets...populate the appropriate classes
            //  3. Do Something...
            

            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            
            using(ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                Console.WriteLine("=============Release Starts=============");

                //  1. Get all subwebs...populate the appropriate classes
                List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext);
                foreach (ProgramManagerSite pm in practicePMSites)
                {
                    Console.WriteLine($"     Program Mgr - {pm.ProgramManagerName}");
                    foreach (PracticeSite psite in pm.PracticeSiteCollection)
                    {
                        Console.WriteLine($"       Practice Site - {psite.Name}");
                    }
                }

                //  TO-DO:
                //  2. Get all existing objects/assets...populate the appropriate classes
                foreach (ProgramManagerSite pm in practicePMSites)
                {
                    foreach (PracticeSite psite in pm.PracticeSiteCollection)
                    {
                        // Do Something...
                    }
                }
                Console.WriteLine("2. GetAllObjects - Complete");


                //  3. Maintenance Tasks...
                foreach (ProgramManagerSite pm in practicePMSites)
                {
                    foreach (PracticeSite psite in pm.PracticeSiteCollection)
                    {
                        // Task - Examples 1...
                        //SiteLogUtility.LogFunction1();
                        //SiteLogUtility.LogFunction2();
                        //SitePublishUtility.PublishFunction1();
                        //SitePublishUtility.PublishFunction2();
                        //SiteListUtility.ListFunction1();
                        //SiteListUtility.ListFunction2();

                        // Task - Examples 2...
                        //PracticeSiteLibrary.PublishPage(psite, "Home.aspx", "Pages");
                        //PracticeSiteLibrary.PublishPage(psite, "cePrac_Home.html", "SiteAssets");
                        //PracticeSiteLibrary.PublishPage(psite, "Hospital.aspx", "Pages");
                        //PracticeSiteLibrary.PublishPage(psite, "cePrac_Hospital.html", "SiteAssets");
                    }
                }
                Console.WriteLine("3. Maintenance Tasks Complete - Complete");

                Console.WriteLine("=============Release Ends=============");
            }
            
        }
    }
}

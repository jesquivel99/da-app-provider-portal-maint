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
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            string releaseName = "SiteUtilityTest";

            SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            
            using(ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                SiteLogUtility.LogText = "=============Release Starts=============";
                SiteLogUtility.Log_Entry(SiteLogUtility.LogText);
                Console.WriteLine(SiteLogUtility.LogText);

                try
                {
                    //  Get all Practice Data...
                    SiteLogUtility.LogText = "=============[ Get all Practice Data ]=============";
                    SiteLogUtility.Log_Entry(SiteLogUtility.LogText);
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext);

                    //  Maintenance Tasks...
                    SiteLogUtility.LogText = "\n\n=============[ Maintenance Tasks ]=============";
                    SiteLogUtility.Log_Entry(SiteLogUtility.LogText);
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        foreach (PracticeSite psite in pm.PracticeSiteCollection)
                        {
                            // Get and Remove SP Groups...
                            //SitePermissionUtility.GetSpGroups(pm, psite);
                            //SitePermissionUtility.RemoveAllSpGroups(pm, psite);
                            SitePermissionUtility.RemoveSingleSpGroup(psite.PracUserReadOnlyPermission, psite.URL);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", siteUrl);
                }
                finally
                {
                    SiteLogUtility.Log_ProcessLogs(SiteLogUtility.logEntryList);

                    // Append all LogList items to log file...
                    System.IO.File.AppendAllLines(SiteLogUtility.LogFileName, SiteLogUtility.LogList);

                    SiteLogUtility.LogText = $"PracticeSiteMaint - {releaseName} \n   Complete";
                    SiteLogUtility.Log_Entry(SiteLogUtility.LogText);
                    Console.WriteLine(SiteLogUtility.textLine);
                    Console.WriteLine(SiteLogUtility.LogText);

                    //Log_EmailToMe();
                }

                Console.WriteLine("=============Release Ends=============");
            }

        }
    }
}

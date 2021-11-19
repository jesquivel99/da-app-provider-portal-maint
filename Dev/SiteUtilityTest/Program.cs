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
            objProgramNew.InitiateProg();
            //string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            //string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            //string siteInfoFile = ConfigurationManager.AppSettings["Csv_File"];
            //string releaseName = "SiteUtilityTest";

            //SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            
            //using(ClientContext clientContext = new ClientContext(siteUrl))
            //{
            //    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
            //    SiteLogUtility.Log_Entry("=============Release Starts=============", true);

            //    try
            //    {
            //        //  Get all Practice Data...
            //        SiteLogUtility.Log_Entry("=============[ Get all Practice Data ]=============", true);
            //        List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext);

            //        //  Maintenance Tasks...
            //        SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks ]=============", true);
            //        foreach (ProgramManagerSite pm in practicePMSites)
            //        {
            //            foreach (PracticeSite psite in pm.PracticeSiteCollection)
            //            {
            //                // Get and Remove SP Groups...
            //                //SitePermissionUtility.GetWebGroups(psite);
            //                //SitePermissionUtility.RemoveSingleSpGroup(psite.PracUserReadOnlyPermission, psite.PracUserReadOnlyPermissionDesc, psite.URL);

            //                if (psite.URL.Contains("94910221369") || psite.URL.Contains("91101941279"))
            //                {
            //                    SiteNavigateUtility.NavigationPracticeMnt(psite.URL, pm.PMURL);
            //                }

            //                if (psite.URL.Contains("94910221369") || psite.URL.Contains("91101941279"))
            //                {
            //                    SiteLogUtility.Log_Entry("Adding RoleAssignments - AddPortalBusinessAdminUserReadOnly, AddRiskAdjustmentUserReadOnly", true);
            //                    SitePermissionUtility.RoleAssignment_AddPortalBusinessAdminUserReadOnly(psite);
            //                    SitePermissionUtility.RoleAssignment_AddRiskAdjustmentUserReadOnly(psite);
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", siteUrl);
            //    }
            //    finally
            //    {
            //        SiteLogUtility.finalLog(releaseName);
            //    }
            //    SiteLogUtility.Log_Entry("=============Release Ends=============", true);
            //}
        }
    }
}

﻿using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using SiteUtility;
using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SiteUtilityTest
{
    public class ProgramNew_Vignesh
    {
        //dev
        string rootUrl = "https://sharepointdev.fmc-na-icg.com";
        string strPortalSiteURL = "https://sharepointdev.fmc-na-icg.com/bi/fhppp/portal";
        string strReferralURL = "https://sharepointdev.fmc-na-icg.com/bi/fhppp/interimckcc/referral"; //NO SLASH AT THE END

        /*
        //PROD
        string rootUrl = "http://vh2-sp-01/";
        string strPortalSiteURL = "http://vh2-sp-01/bi/fhppp/portal";
        string strReferralURL = "http://vh2-sp-01/bi/fhppp/portal/referral"; //NO SLASH AT THE END
        */

        string ResultLog = "=============Release Starts=============\r\n";
        string textLine = "\r\n=======================================\r\n";

        public void InitiateProg()
        {
            string sAdminListName = ConfigurationManager.AppSettings["AdminRootListName"];
            string releaseName = "SiteUtilityTest";
            SiteRootAdminList objRootSite = new SiteRootAdminList();
            SiteDeleteUtility objDeleteSite = new SiteDeleteUtility();
            SiteFilesUtility objFilesSite = new SiteFilesUtility();

            SiteLogUtility.InitLogFile(releaseName, rootUrl, strPortalSiteURL);

            using (ClientContext clientContext = new ClientContext(strPortalSiteURL))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext);
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        //if (pm.ProgramManager == "08")
                        //{
                            foreach (PracticeSite psite in pm.PracticeSiteCollection)
                            {
                                List<PMData> pmd = SiteInfoUtility.SP_GetAll_PMData(pm.URL, psite.SiteId);
                                if (pmd.Count > 0)
                                {
                                    if (pmd[0].IsCKCC == "true")
                                    {
                                        ReferralSetup(psite.URL + "/");

                                        ResultLog += textLine + psite.Name + "\r\n" + psite.URL + "\r\nSite is CKCC - Setup is Complete;" + textLine;
                                    }
                                    else
                                    {
                                        ResultLog += textLine + psite.Name + "\r\n" + psite.URL + "\r\nSite is NOT CKCC; No changes made;" + textLine;
                                    }
                                }
                            }
                        //}
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", strPortalSiteURL);
                }

                ResultLog += textLine + "=============Release Ends=============";
                string fileName = "ResultLog_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".txt";
                string filePath = Path.Combine(@"C:\SVK\OptStart", fileName);
                using (StreamWriter sw = new StreamWriter(filePath))
                    sw.WriteLine(ResultLog);
            }
        }

        public void ReferralSetup(string sitrUrl)
        {
            try
            {
                string strSiteID = getSiteID(sitrUrl);
                addSecurityGroupToList(strReferralURL, "Prac_" + strSiteID + "_User", "DialysisStarts", "Contribute");
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("ReferralSetup", ex.Message, "Error", strPortalSiteURL);
            }
        }

        public string getSiteID(string strURL)
        {
            string strRealSiteID = string.Empty;
            try
            {
                string[] strResult = strURL.Split('/');
                string strSiteID = strResult[strResult.Length - 2];
                char[] charArray = strSiteID.Substring(1, strSiteID.Length - 2).ToArray();
                Array.Reverse(charArray);
                strRealSiteID = new string(charArray);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("getSiteID", ex.Message, "Error", strPortalSiteURL);
            }
            return strRealSiteID;
        }

        public void addSecurityGroupToList(string strURL, string strSecurityGroupName, string strListName, string strPermissionType)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(strURL))
                {
                    List targetList = clientContext.Web.Lists.GetByTitle(strListName);
                    clientContext.Load(targetList, target => target.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();

                    if (targetList.HasUniqueRoleAssignments)
                    {
                        // Write group name to be added in the list
                        Group group = clientContext.Web.SiteGroups.GetByName(strSecurityGroupName);
                        RoleDefinitionBindingCollection roleDefCollection = new RoleDefinitionBindingCollection(clientContext);

                        // Set the permission level of the group for this particular list
                        RoleDefinition readDef = clientContext.Web.RoleDefinitions.GetByName(strPermissionType);
                        roleDefCollection.Add(readDef);

                        Principal userGroup = group;
                        RoleAssignment roleAssign = targetList.RoleAssignments.Add(userGroup, roleDefCollection);

                        clientContext.Load(roleAssign);
                        roleAssign.Update();
                        clientContext.ExecuteQuery();
                    }
                    else
                        ResultLog += "\r\nCannot add permissions - does NOT HasUniqueRoleAssignments;" + textLine;
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("addSecurityGroupToList", ex.Message, "Error", strPortalSiteURL);
            }
        }

    }
}

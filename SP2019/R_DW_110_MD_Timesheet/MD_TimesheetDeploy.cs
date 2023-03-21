using System;
using System.Linq;
using System.Net;
using Microsoft.SharePoint.Client;
using SiteUtility;

namespace R_DW_110_MD_Timesheet
{
    public class MD_TimesheetDeploy
    {
        public void InitiateProg(string siteID)
        {
            SiteInfoUtility siteInfo = new SiteInfoUtility();
            SiteLogUtility slu = new SiteLogUtility();

            Practice practice = siteInfo.GetPracticeBySiteID(siteID);

            Medical_Director_Setup(practice.NewSiteUrl);
        }

        public bool Medical_Director_Setup(string siteUrl)
        {
            //string urlSiteAssets = @"https://sharepoint.fmc-na-icg.com/bi/fhppp/portal/referral";
            string urlSiteAssets = @"https://sharepointdev.fmc-na-icg.com/bi/fhppp/interimckcc/referral";
            try
            {
                SitePublishUtility objSitePublish = new SitePublishUtility();

                if (!SiteFilesUtility.FileExists(siteUrl, "Pages", "MedicalDirectorTable.aspx"))
                {
                    SitePublishUtility.CreateAspxPage(siteUrl, "MedicalDirectorTable", "Medical Director Timesheets", "1000px", urlSiteAssets + "/SiteAssets/MedicalDirectorTable.html");
                }

                if (!SiteFilesUtility.FileExists(siteUrl, "Pages", "MedicalDirectorForm.aspx"))
                {
                    SitePublishUtility.CreateAspxPage(siteUrl, "MedicalDirectorForm", "Medical Director Quarterly Time Sheet", "", urlSiteAssets + "/SiteAssets/MedicalDirectorForm.html");
                }
                AddMedicalDirectorNavigationNode(siteUrl);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void AddMedicalDirectorNavigationNode(string webUrl)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(webUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;
                    NavigationNodeCollection objNodeColl = clientContext.Web.Navigation.QuickLaunch;
                    clientContext.Load(web);
                    clientContext.Load(web.ParentWeb);
                    clientContext.ExecuteQuery();

                    clientContext.Load(objNodeColl);
                    clientContext.ExecuteQuery();

                    NavigationNode newNode = objNodeColl.Where(Node => Node.Title == "Medical Director Timesheet").FirstOrDefault();
                    if (newNode == null) // Add only if Medical Director Timesheet node does not exist
                    {
                        NavigationNode prevNode = objNodeColl.Where(Node => Node.Title.Contains("Quality")).FirstOrDefault();

                        NavigationNodeCreationInformation objNewNode = new NavigationNodeCreationInformation();
                        objNewNode.Title = "Medical Director Timesheet";
                        objNewNode.Url = webUrl + "Pages/MedicalDirectorTable.aspx";
                        objNewNode.PreviousNode = prevNode; // Add Medical Director Timesheet node right after Quality

                        objNodeColl.Add(objNewNode);
                        clientContext.ExecuteQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                //SiteLogUtility.CreateLogEntry("addSWReferralNavigationNode", ex.Message, "Error", strPortalSiteURL);
            }
        }
    }
}


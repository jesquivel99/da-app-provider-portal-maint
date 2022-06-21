using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SiteUtility;
using System.Configuration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using Microsoft.SharePoint.Client.WebParts;
using System.Net;
using System.IO;
using System.Xml;
using System.Reflection;
using SP = Microsoft.SharePoint.Client;
using System.Net.Mail;

namespace R_1_8_MovePractice
{
    public class Program
    {
        public static Guid _listGuid = Guid.Empty;
        static void Main(string[] args)
        {
        }

        public void InitiateProg()
        {
            string releaseName = "SiteUtilityTest";
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];


            string runPM = "PM06";
            string runPractice = "92869520159";
            string urlAdminGroup = siteUrl + "/" + runPM;



            SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            SiteLogUtility.Log_Entry("\n\n=============Release Starts=============", true);

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    //Test Start...
                    //return;
                    //Test End...

                    SiteLogUtility.Log_Entry("\n\n=============[ Get all Portal Practice Data ]=============", true);
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext);

                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - Start]=============", true);
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        SiteLogUtility.Log_Entry("\nPM Site: " + pm.PracticeName + " - " + pm.PMURL, true);
                        foreach (PracticeSite psite in pm.PracticeSiteCollection)
                        {
                            //if (psite.URL.Contains(runPM))
                            if (psite.URL.Contains(runPM) && psite.URL.Contains(runPractice))
                            {
                                SiteFilesUtility sfu = new SiteFilesUtility();
                                UpdateUrlRef(psite, "Program Participation");

                                //HTML Files for Landing Page
                                sfu.uploadHtmlSupportingFilesSingleFile(psite.URL, "cePrac_ProgramParticipation.html");
                                sfu.uploadHtmlSupportingFilesSingleFile(psite.URL, "cePrac_Home.html");
                                sfu.uploadHtmlSupportingFilesSingleFile(psite.URL, "cePrac_RiskAdjustmentResources.html");
                                sfu.uploadHtmlSupportingFilesSingleFile(psite.URL, "cePrac_CareCoordination.html");
                                sfu.uploadHtmlSupportingFilesSingleFile(psite.URL, "cePrac_InteractiveInsights.html");
                                sfu.uploadHtmlSupportingFilesSingleFile(psite.URL, "cePrac_Quality.html");

                                //HTML Files for CareCoordination Page
                                sfu.uploadHtmlSupportingFilesSingleFile(psite.URL, "cePrac_CarePlans.html");
                                sfu.uploadHtmlSupportingFilesSingleFile(psite.URL, "cePrac_CarePlans_Ckcc.html");
                                sfu.uploadHtmlSupportingFilesSingleFile(psite.URL, "cePrac_HospitalAlerts.html");
                                sfu.uploadHtmlSupportingFilesSingleFile(psite.URL, "cePrac_MedicationAlerts.html");

                                //HTML Files for CarePlans Page
                                sfu.uploadHtmlSupportingFilesSingleFile(psite.URL, "cePrac_CarePlansDataTable.html");

                                //HTML Files - Other
                                sfu.uploadHtmlSupportingFilesSingleFile(psite.URL, "cePrac_HospitalAlerts.html");
                                sfu.uploadHtmlSupportingFilesSingleFile(psite.URL, "cePrac_MedicationAlerts.html");
                                sfu.uploadHtmlSupportingFilesSingleFile(psite.URL, "cePrac_InteractiveInsights.html");
                            }
                        }
                    }
                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - End]=============", true);
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", "");
                }
                finally
                {
                    SiteLogUtility.Log_Entry(SiteLogUtility.textLine0, true);
                    SiteLogUtility.finalLog(releaseName);
                    SiteLogUtility.email_toMe(String.Join("\n", SiteLogUtility.LogList), "LogFile", "james.esquivel@freseniusmedicalcare.com");
                }
                SiteLogUtility.Log_Entry("=============Release Ends=============", true);
            }
        }

        private void UpdateUrlRef(PracticeSite psite, string strList)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(psite.URL))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle(strList);
                    var items = list.GetItems(CamlQuery.CreateAllItemsQuery());

                    clientContext.Load(items);
                    clientContext.ExecuteQuery();


                    foreach (var item in items)
                    {
                        var fndTitle = item["Title"].ToString();
                        string thumbNail = GetProgramParticipationImg(fndTitle);
                        item["Thumbnail"] = psite.URL + "/Program%20Participation/" + thumbNail;
                        item.Update();
                        clientContext.ExecuteQuery();
                        SiteLogUtility.Log_Entry($">>> {item["Title"]} - Thumbnail = {item["Thumbnail"]}", true);
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("UpdateSortCol", ex.Message, "Error", "");
            }
        }
        private string GetProgramParticipationImg(string fndTitle)
        {
            string thumbNail = string.Empty;
            try
            {
                switch (fndTitle)
                {
                    case SiteListUtility.progpart_PayorEnrollment:
                        thumbNail = "PracticeReferrals.JPG";
                        break;
                    case SiteListUtility.progpart_CkccKceResources:
                        thumbNail = "KCEckcc.JPG";
                        break;
                    case SiteListUtility.progpart_PayorProgeducation:
                        thumbNail = "EducationReviewPro.JPG";
                        break;
                    case SiteListUtility.progpart_PatientStatusUpdates:
                        thumbNail = "optimalstarts.jpg";
                        break;
                    case SiteListUtility.progpart_CkccKceEngagement:
                        thumbNail = "CKCC_KCEEngagement.png";
                        break;


                    default:
                        thumbNail = "";
                        break;
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetProgramParticipationImg", ex.Message, "Error", "");
            }
            return thumbNail;
        }
    }
}

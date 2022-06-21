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

namespace SiteUtilityTest
{
    public class ProgramNew_JE
    {
        public static Guid _listGuid = Guid.Empty;
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
                                // get list
                                // check if column exists
                                // add column
                                // update all items with sort values
                                // refresh the view

                                //bool listExist = DoesListExist(psite, "Program Participation");
                                //ListAddColumn(psite, "Program Participation");
                                //UpdateSortCol(psite, "Program Participation");





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

                                //GetPropertyBag(psite.URL);
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

        private void UpdateSortCol(PracticeSite psite, string strList)
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
                        int sortOrder = GetProgramParticipationSortOrder(fndTitle);
                        item["Sort_Order"] = sortOrder;
                        item.Update();
                        clientContext.ExecuteQuery();
                        SiteLogUtility.Log_Entry($">>> {item["Title"]} - Sort Order = {item["Sort_Order"]}", true);
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

        private int GetProgramParticipationSortOrder(string fndTitle)
        {
            int sortorder = 0;
            try
            {
                switch (fndTitle)
                {
                    case SiteListUtility.progpart_PayorEnrollment:
                        sortorder = 10;
                        break;
                    case SiteListUtility.progpart_CkccKceResources:
                        sortorder = 20;
                        break;
                    case SiteListUtility.progpart_CkccKceEngagement:
                        sortorder = 30;
                        break;
                    case SiteListUtility.progpart_PatientStatusUpdates:
                        sortorder = 40;
                        break;
                    case SiteListUtility.progpart_PayorProgeducation:
                        sortorder = 50;
                        break;
                    default:
                        sortorder = 10;
                        break;
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetProgramParticipationSortOrder", ex.Message, "Error", "");
            }
            return sortorder;
        }

        private void ListAddColumn(PracticeSite psite, string strList)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(psite.URL))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle(strList);
                    Field field = list.Fields.AddFieldAsXml("<Field Type='Number' DisplayName='Sort_Order' Name='Sort_Order' />", true, AddFieldOptions.AddFieldInternalNameHint);

                    clientContext.Load(field);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("ListAddColumn", ex.Message, "Error", "");
            }
        }

        private static bool DoesListExist(PracticeSite psite, string listName)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(psite.URL))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    {
                        ListCollection lists = clientContext.Web.Lists;
                        clientContext.Load(lists);
                        clientContext.ExecuteQuery();

                        bool bListFound = false;

                        if (lists != null && lists.Count > 0)
                        {
                            foreach (List list in lists)
                            {
                                if (list.Title == listName)
                                {
                                    _listGuid = list.Id;
                                    bListFound = true;
                                    SiteLogUtility.Log_Entry(psite.Name + " - " + psite.URL, true);
                                    break;
                                }
                            }
                        }

                        return bListFound;
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("DoesListExist", ex.Message, "Error", "");
            }
            return false;
        }

        private static bool DoesListExistGetGuid(string wUrl, string listName)
        {
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                {
                    ListCollection lists = clientContext.Web.Lists;
                    clientContext.Load(lists);
                    clientContext.ExecuteQuery();

                    bool bListFound = false;
                    if (lists != null && lists.Count > 0)
                    {
                        foreach (List list in lists)
                        {
                            if (list.Title == listName)
                            {
                                _listGuid = list.Id;
                                bListFound = true;
                                break;
                            }
                        }
                    }

                    return bListFound;
                }
            }
        }

        public static string urlRelativeReferral_Prod = @"/bi/fhppp/portal/referral/";
        public static string urlRelativeReferral_Dev = @"/bi/fhppp/interimckcc/referral/";
        
        public static void LoadParentWebTest(PracticeSite site)
        {
            using (ClientContext clientContext = new ClientContext(site.URL))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    clientContext.Load(clientContext.Web,
                                            web => web.ParentWeb.ServerRelativeUrl,
                                            web => web.ServerRelativeUrl,
                                            web => web.SiteGroups.Include(
                                                sg => sg.Description,
                                                sg => sg.Title));
                    clientContext.ExecuteQuery();

                    SiteInfoUtility siu = new SiteInfoUtility();
                    string rootUrl = siu.GetRootSite(site.URL);
                    string urlReferralSite = rootUrl;
                    if (rootUrl.Contains("sharepointdev"))
                    {
                        urlReferralSite = rootUrl + urlRelativeReferral_Dev;
                    }
                    else
                    {
                        urlReferralSite = rootUrl + urlRelativeReferral_Prod;
                    }
                    
                    SiteLogUtility.Log_Entry("RootWeb: " + rootUrl);
                    SiteLogUtility.Log_Entry("ParentWeb: " + clientContext.Web.ParentWeb.ServerRelativeUrl);
                    SiteLogUtility.Log_Entry("PracticeWeb: " + clientContext.Web.ServerRelativeUrl);
                    SiteLogUtility.Log_Entry("ReferralWeb: " + urlReferralSite);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error with LoadParentWebTest: " + ex.Message);
                }
            }
        }
        
        public static void GetPropertyBag(string wUrl)
        {
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                Web w = clientContext.Web;
                clientContext.Load(w);
                clientContext.Load(w.AllProperties);
                clientContext.ExecuteQuery();
                try
                {
                    foreach (var item in w.AllProperties.FieldValues)
                    {
                        SiteLogUtility.Log_Entry("Key: " + item.Key + " - Value: " + item.Value, true);
                    }
                    
                    //int c = Properties.Count();
                    //for (int i = 0; i < c; i++)
                    //{
                    //    string key = Properties[i].PropertyName;

                    //    if (w.AllProperties.FieldValues.ContainsKey(key) && Properties[i].PropertyValue != (string)w.AllProperties[key])
                    //    {
                    //        w.AllProperties[key] = Properties[i].PropertyValue;
                    //        w.Update();
                    //        clientContext.ExecuteQuery();
                    //    }
                    //    else
                    //    {
                    //        //w.AllProperties.Add(key, Properties[i].PropertyValue);
                    //        w.AllProperties[key] = Properties[i].PropertyValue;
                    //        w.Update();
                    //        clientContext.ExecuteQuery();
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("SetupPropertyBag", ex.Message, "Error", "");
                }
            }
        }

        private static bool DeleteDeploymentErrorItems()
        {
            int count = 0;
            try
            {
                using (ClientContext clientContext = new ClientContext("https://sharepointdev.fmc-na-icg.com/bi/fhppp/portal"))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                    Web web = clientContext.Web;
                    //List list = web.Lists.GetByTitle("DeploymentErrors");

                    List rList = web.Lists.GetByTitle("DeploymentErrors");

                    //CamlQuery camlQuery = new CamlQuery();
                    //camlQuery.ViewXml = "<View Scope='RecursiveAll'><RowLimit>4999</RowLimit></View>";


                    CamlQuery camlQuery = new CamlQuery();
                    //camlQuery.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='Method' /><Value Type='Text'>" + "GetRootFolders" + "</Value></Eq></Where></Query><RowLimit>4999</RowLimit></View>";
                    camlQuery.ViewXml = "<View><RowLimit>4999</RowLimit></View>";

                    List<ListItem> items = new List<ListItem>();
                    do
                    {
                        ListItemCollection colxn = rList.GetItems(camlQuery);
                        clientContext.Load(colxn);
                        clientContext.ExecuteQuery();
                        items.AddRange(colxn);
                        camlQuery.ListItemCollectionPosition = colxn.ListItemCollectionPosition;
                    } while (camlQuery.ListItemCollectionPosition != null);

                    if (items.Count < 1)
                        return false;
                    foreach (ListItem li in items)
                    {
                        //li.DeleteObject();
                        //SiteLogUtility.Log_Entry(li.DisplayName, true);
                        SiteLogUtility.Log_Entry(li["Method"].ToString(), true);

                        clientContext.ExecuteQuery();
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
            }
            return true;
        }
    }
}

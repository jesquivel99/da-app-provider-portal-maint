using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteUtility;
using System.Configuration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using System.Net;
using System.IO;

namespace R_1_5_ModifyView
{
    public class Program
    {
        static public List<Practice> practicesIWH = new List<Practice>();
        static public List<Practice> practicesCKCC = new List<Practice>();
        static Guid _listGuid = Guid.Empty;
        static void Main(string[] args)
        {
            string releaseName = "ModifyView";
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            string srcUrlIWH = ConfigurationManager.AppSettings["SP_IWHUrl"];
            string srcUrlCKCC = ConfigurationManager.AppSettings["SP_CKCCUrl"];
            string siteInfoFile = ConfigurationManager.AppSettings["Csv_File"];

            string urlAdminGroup = @"https://sharepointdev.fmc-na-icg.com/bi/fhppp/portal/PM02";

            SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);
            SiteLogUtility.Log_Entry("\n\n=============Release Starts=============", true);

            List<PMData> pmData = initPMDataToList(urlAdminGroup);

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    SiteLogUtility.Log_Entry("\n\n=============[ Get all Portal Practice Data ]=============", true);
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext, practicesIWH, practicesCKCC, pmData);

                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - Start]=============", true);
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        foreach (PracticeSite psite in pm.PracticeSiteCollection)
                        {
                            if (psite.URL.Contains("92125344289") && psite.IsCKCC.Equals("true"))
                            {
                                SiteLogUtility.LogPracDetail(psite);

                                //Modify View...
                                if(psite.IsCKCC == "true")
                                {
                                    Init_ModifyView_CKCC(psite.URL);
                                }

                                if(psite.IsIWH == "true")
                                {
                                    Init_ModifyView_IWH(psite.URL);
                                }
                            }
                        }
                    }
                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks - End]=============", true);
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", siteUrl);
                }
                finally
                {
                    SiteLogUtility.finalLog(releaseName);
                }
                SiteLogUtility.Log_Entry("=============Release Ends=============", true);
            }
        }

        private static void Init_ModifyView_IWH(string wUrl)
        {
            try
            {
                modifyView(wUrl, "DataExchange.aspx", "Practice Documents IWH");
                modifyView(wUrl, "RiskAdjustmentResources.aspx", "Private Payor Program");
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_ModifyView_IWH", ex.Message, "Error", wUrl);
            }
        }

        private static void Init_ModifyView_CKCC(string wUrl)
        {
            try
            {
                modifyView(wUrl, "DataExchange.aspx", "Practice Documents CKCC");
                modifyView(wUrl, "RiskAdjustmentResources.aspx", "CKCC/KCE Program");
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("Init_ModifyView_CKCC", ex.Message, "Error", wUrl);
            }
        }

        public static List<PMData> initPMDataToList(string adminGroupUrl)
        {
            //SitePMData objSitePMData = new SitePMData();
            List<PMData> pmData = new List<PMData>();
            pmData = SP_GetPortalData_PMData(adminGroupUrl);
            return pmData;
        }

        public static List<PMData> SP_GetPortalData_PMData(string adminGroupUrl)
        {
            List<PMData> All_PortalData = new List<PMData>();
            List<PMData> CKCC_PMData = new List<PMData>();
            try
            {
                All_PortalData = SP_GetAll_PMData(adminGroupUrl);
                CKCC_PMData = All_PortalData.Where
                    (x => x.ProgramParticipation.Contains("KCE Participation")).ToList();

                //ResultDescription += "[" + Answers_NewReferrals.Count + "] items found in SP => Visible and have answers." + textLine;
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("SP_GetPortalData_PMData", ex.Message, "Error", "");
            }

            return CKCC_PMData;
        }

        public static List<PMData> SP_GetAll_PMData(string urlAdminGrp)
        {
            List<PMData> pmData = new List<PMData>();
            SitePMData sitePMData = new SitePMData();

            using (ClientContext clientContext = new ClientContext(urlAdminGrp))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                List list = clientContext.Web.Lists.GetByTitle("AdminGroup");
                clientContext.Load(list);
                clientContext.ExecuteQuery();
                View view = list.Views.GetByTitle("All Links");

                clientContext.Load(view);
                clientContext.ExecuteQuery();
                CamlQuery query = new CamlQuery();
                query.ViewXml = view.ViewQuery;

                ListItemCollection items = list.GetItems(query);
                clientContext.Load(items);
                clientContext.ExecuteQuery();
                SiteLogUtility.Log_Entry("Total Count: " + items.Count, true);

                foreach (var item in items)
                {
                    PMData pmd = new PMData();


                    SiteLogUtility.Log_Entry(item["PracticeTIN"] + " - " + item["PracticeName"] + " - " + item["ProgramParticipation"], true);
                    //SiteLogUtility.Log_Entry("PracticeName: " + item["PracticeName"], true);
                    //SiteLogUtility.Log_Entry("PracticeTIN: " + item["PracticeTIN"], true);
                    //SiteLogUtility.Log_Entry("ProgramParticipation: " + item["ProgramParticipation"], true);

                    pmd.PracticeName = item["PracticeName"].ToString();
                    pmd.PracticeTIN = item["PracticeTIN"].ToString();
                    pmd.SiteId = item["PracticeTIN"].ToString();
                    pmd.ProgramParticipation = item["ProgramParticipation"].ToString();

                    pmd.IsKC365 = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationKC365) ? "true" : "false";
                    pmd.IsCKCC = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationCKCC) ? "true" : "false";
                    pmd.IsIWH = item["ProgramParticipation"].ToString().Contains(sitePMData.programParticipationIWH) ? "true" : "false";
                    //sourceSite.SetAttributeValue("IsKC365", Convert.ToInt32(dr["KC365"]) == 0 ? "false" : "true");
                    //sourceSite.SetAttributeValue("kceArea", dr["CKCCArea"]);
                    //sourceSite.SetAttributeValue("IsCKCC", dr["CKCCArea"].ToString() == "" ? "false" : "true");
                    //sourceSite.SetAttributeValue("IsIWH", dr["IWNRegion"].ToString() == "" ? "false" : "true");

                    pmData.Add(pmd);
                }
            }

            return pmData;
        }

        public static void modifyView(string webUrl, string strPageName = "Home.aspx", string strWebPartTitle = "Practice Documents")
        {
            using (ClientContext clientContext = new ClientContext(webUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                {
                    Web w = clientContext.Web;
                    bool blnWebPartExists = false;
                    List list = w.Lists.GetByTitle("Documents");
                    if (strWebPartTitle == "Practice Documents IWH")
                    {
                        list = w.Lists.GetByTitle("Documentsiwh");
                    }
                    else if (strWebPartTitle == "Practice Documents CKCC")
                    {
                        list = w.Lists.GetByTitle("Documentsckcc");
                    }
                    else if (strWebPartTitle == "CKCC/KCE Program")        // Risk Adjustment tab
                    {
                        list = w.Lists.GetByTitle("RiskAdjustment_ckcc");
                    }
                    else if (strWebPartTitle == "Private Payor Program")   // Risk Adjustment tab
                    {
                        list = w.Lists.GetByTitle("RiskAdjustment_iwh");
                    }

                    SiteLogUtility.Log_Entry("Webpart Title: " + strWebPartTitle, true);

                    clientContext.Load(w);
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();
                    _listGuid = list.Id;

                    // May need to use this method...
                    //bool undoCheckout = UndoPageViewerCheckout(list, _listGuid, "PageViewer", true, webUrl);

                    var file = w.GetFileByServerRelativeUrl(w.ServerRelativeUrl + "/Pages/" + strPageName);
                    file.CheckOut();
                    clientContext.Load(file);
                    clientContext.ExecuteQuery();

                    try
                    {
                        var wpManager = file.GetLimitedWebPartManager(Microsoft.SharePoint.Client.WebParts.PersonalizationScope.Shared);
                        var webparts = wpManager.WebParts;
                        clientContext.Load(webparts);
                        clientContext.ExecuteQuery();

                        string[] viewFields = { "Type", "LinkFilename", "Modified" };

                        if (webparts.Count > 0)
                        {
                            foreach (var webpart in webparts)
                            {
                                clientContext.Load(webpart.WebPart.Properties);
                                clientContext.ExecuteQuery();
                                var propValues = webpart.WebPart.Properties.FieldValues;
                                if (propValues["Title"].Equals(strWebPartTitle))
                                {
                                    blnWebPartExists = true;
                                    var listView = list.Views.GetById(webpart.Id);
                                    clientContext.Load(listView);
                                    clientContext.ExecuteQuery();

                                    listView.ViewFields.RemoveAll();
                                    foreach (var viewField in viewFields)
                                    {
                                        listView.ViewFields.Add(viewField);
                                    }

                                    listView.ViewQuery = "<OrderBy><FieldRef Name='ID' /></OrderBy><Where><IsNotNull><FieldRef Name='ID' /></IsNotNull></Where>";
                                    listView.Update();
                                    clientContext.ExecuteQuery();
                                    file.CheckIn("Adding Modified to view in Document library", CheckinType.MajorCheckIn);
                                    file.Publish("Adding Modified to view in Document library");
                                    clientContext.Load(file);
                                    w.Update();
                                    clientContext.ExecuteQuery();
                                    break;
                                }
                            }
                        }
                        if (!blnWebPartExists)
                        {
                            file.CheckIn("Adding Modified to view in Document library", CheckinType.MajorCheckIn);
                            file.Publish("Adding Modified to view in Document library");
                            clientContext.Load(file);
                            w.Update();
                            clientContext.ExecuteQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("CreatePracticeHomePage - modifyView", ex.Message, "Error", webUrl);
                        file.UndoCheckOut();
                        clientContext.Load(file);
                        clientContext.ExecuteQuery();
                        clientContext.Dispose();
                    }
                }
            }
        }

        public static bool UndoPageViewerCheckout(List list, Guid _listGuid, string ViewName, bool justCreated = false, string wURL = "")
        {
            for (int i = 0; i < list.Views.Count; i++)
            {
                if (list.Views[i].Title.Equals(ViewName))
                {
                    if (justCreated && list.Views[i].Title == "PageViewer")
                    {
                        using (ClientContext clientContext = new ClientContext(wURL))
                        {
                            bool contentExists = false;
                            string checkingMessage = "Checking in back";
                            clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                            Web w = clientContext.Web;
                            list = w.Lists.GetById(_listGuid);
                            clientContext.Load(list);
                            clientContext.Load(list.Views);
                            clientContext.Load(list.Fields);
                            clientContext.Load(w);
                            clientContext.ExecuteQuery();
                            Microsoft.SharePoint.Client.File pvFile = w.GetFileByServerRelativeUrl(list.Views[i].ServerRelativeUrl);
                            clientContext.Load(pvFile);
                            User user = pvFile.Author;
                            clientContext.Load(user);
                            clientContext.ExecuteQuery();

                            try
                            {
                                if (pvFile.CheckOutType != CheckOutType.None)
                                {
                                    SiteLogUtility.Log_Entry("-----------------------", true);
                                    SiteLogUtility.Log_Entry(pvFile.Author.Title.ToString(), true);
                                    SiteLogUtility.Log_Entry(pvFile.Author.LoginName.ToString(), true);
                                    SiteLogUtility.Log_Entry(pvFile.CheckOutType.ToString(), true);
                                    SiteLogUtility.Log_Entry(pvFile.Name.ToString(), true);
                                    SiteLogUtility.Log_Entry(pvFile.CheckedOutByUser.ToString(), true);
                                    SiteLogUtility.Log_Entry("", true);

                                    pvFile.UndoCheckOut();
                                    clientContext.Load(pvFile);
                                    clientContext.ExecuteQuery();
                                }

                            }
                            catch (Exception ex)
                            {
                                SiteLogUtility.CreateLogEntry("UndoPageViewerCheckout", ex.Message, "Error", clientContext.Web.ServerRelativeUrl);
                            }
                        }
                        Microsoft.SharePoint.Client.View v = list.Views[i];
                        v.Update();
                    }
                    return true;
                }
            }
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using SPNavPub = Microsoft.SharePoint.Client.Publishing.Navigation;

namespace SiteUtility
{
    public class SiteNavigateUtility
    {
        public partial class NavPracPage
        {
            public class NavPracPageItems
            {
                public string NavPageName;
                public string NavPageFileName;
                public int NavPageSort;
                public int NavPageStatus;

                public string navPracPageLanding = "Landing Page";
                public string navPracPageDataExchange = "Data Exchange";
                public string navPracPageRiskAdjustment = "Risk Adjustment Resources";
                public string navPracPageProgramParticipation = "Program Participation";
                public string navPracPageCareCoordination = "Care Coordination";
                public string navPracPageInteractiveInsights = "Interactive Insights Coming Soon";
                public string navPracPageQuality = "Quality";

                public string navPracPageFileLanding = "Home.aspx";
                public string navPracPageFileDataExchange = "DataExchange.aspx";
                public string navPracPageFileRiskAdjustment = "RiskAdjustmentResources.aspx";
                public string navPracPageFileProgramParticipation = "ProgramParticipation.aspx";
                public string navPracPageFileCareCoordination = "CareCoordination.aspx";
                public string navPracPageFileInteractiveInsights = "InteractiveInsights.aspx";
                public string navPracPageFileQuality = "Quality.aspx";
            }

            public class NavPracPageTopItems
            {
                public string NavPageName;
                public string NavPageFileName;
                public string NavPageUrl;
                public int NavPageSort;
                public int NavPageStatus;

                public string navPracPagePM = "Program Manager";
                public string navPracPageFilePM = "Home.aspx";
                public string navPracPagePmUrl = String.Empty;

                public string navPracPageAdmin = "Admin";
                public string navPracPageFileAdmin = "Admin.aspx";
                public string navPracPageAdminUrl = String.Empty;

            }
        }
        public partial class Navigation_AddPagesToNode
        {
            List<NavPracPage.NavPracPageItems> newNavPracItems = new List<NavPracPage.NavPracPageItems>();
            public Navigation_AddPagesToNode(string webUrl)
            {

                try
                {
                    NavPracPage.NavPracPageItems pageLanding = new NavPracPage.NavPracPageItems();
                    pageLanding.NavPageName = pageLanding.navPracPageLanding;
                    pageLanding.NavPageFileName = pageLanding.navPracPageFileLanding;
                    pageLanding.NavPageSort = 1;
                    pageLanding.NavPageStatus = 1;
                    newNavPracItems.Add(pageLanding);

                    NavPracPage.NavPracPageItems pageProgramParticipation = new NavPracPage.NavPracPageItems();
                    pageProgramParticipation.NavPageName = pageProgramParticipation.navPracPageProgramParticipation;
                    pageProgramParticipation.NavPageFileName = pageProgramParticipation.navPracPageFileProgramParticipation;
                    pageProgramParticipation.NavPageSort = 2;
                    pageProgramParticipation.NavPageStatus = 1;
                    newNavPracItems.Add(pageProgramParticipation);

                    NavPracPage.NavPracPageItems pageDataExchange = new NavPracPage.NavPracPageItems();
                    pageDataExchange.NavPageName = pageDataExchange.navPracPageDataExchange;
                    pageDataExchange.NavPageFileName = pageDataExchange.navPracPageFileDataExchange;
                    pageDataExchange.NavPageSort = 3;
                    pageDataExchange.NavPageStatus = 1;
                    newNavPracItems.Add(pageDataExchange);

                    NavPracPage.NavPracPageItems pageRiskAdjustment = new NavPracPage.NavPracPageItems();
                    pageRiskAdjustment.NavPageName = pageRiskAdjustment.navPracPageRiskAdjustment;
                    pageRiskAdjustment.NavPageFileName = pageRiskAdjustment.navPracPageFileRiskAdjustment;
                    pageRiskAdjustment.NavPageSort = 4;
                    pageRiskAdjustment.NavPageStatus = 1;
                    newNavPracItems.Add(pageRiskAdjustment);



                    NavPracPage.NavPracPageItems pageCareCoordination = new NavPracPage.NavPracPageItems();
                    pageCareCoordination.NavPageName = pageCareCoordination.navPracPageCareCoordination;
                    pageCareCoordination.NavPageFileName = pageCareCoordination.navPracPageFileCareCoordination;
                    pageCareCoordination.NavPageSort = 10;
                    pageCareCoordination.NavPageStatus = 1;
                    newNavPracItems.Add(pageCareCoordination);

                    NavPracPage.NavPracPageItems pageInteractiveInsights = new NavPracPage.NavPracPageItems();
                    pageInteractiveInsights.NavPageName = pageInteractiveInsights.navPracPageInteractiveInsights;
                    pageInteractiveInsights.NavPageFileName = pageInteractiveInsights.navPracPageFileInteractiveInsights;
                    pageInteractiveInsights.NavPageSort = 11;
                    pageInteractiveInsights.NavPageStatus = 9;
                    newNavPracItems.Add(pageInteractiveInsights);

                    NavPracPage.NavPracPageItems pageQuality = new NavPracPage.NavPracPageItems();
                    pageQuality.NavPageName = pageQuality.navPracPageQuality;
                    pageQuality.NavPageFileName = pageQuality.navPracPageFileQuality;
                    pageQuality.NavPageSort = 12;
                    pageQuality.NavPageStatus = 9;
                    newNavPracItems.Add(pageQuality);


                    var sortedNavPracItems = newNavPracItems.OrderBy(x => x.NavPageStatus).ThenBy(x => x.NavPageSort).ToList();
                    foreach (NavPracPage.NavPracPageItems navItem in sortedNavPracItems)
                    {
                        AddLeftNav_Pages(navItem, webUrl, webUrl);
                    }

                    AddChildNodeToLeftNav(webUrl);
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("Navigation_AddPagesToNode", ex.Message, "Error", "");
                }
            }


            private static void AddLeftNav_Pages(NavPracPage.NavPracPageItems navItem, string webUrl, string pracUrl)
            {
                try
                {
                    using (ClientContext clientContext = new ClientContext(webUrl))
                    {
                        clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                        {
                            Web w = clientContext.Web;
                            clientContext.Load(w);
                            NavigationNodeCollection qlNavNodeColl = w.Navigation.QuickLaunch;
                            clientContext.Load(qlNavNodeColl);
                            clientContext.ExecuteQuery();

                            NavigationNodeCreationInformation navCreateInfo = new NavigationNodeCreationInformation();
                            navCreateInfo.Title = navItem.NavPageName;
                            navCreateInfo.Url = pracUrl + @"Pages/" + navItem.NavPageFileName;
                            navCreateInfo.AsLastNode = true;
                            qlNavNodeColl.Add(navCreateInfo);
                            clientContext.ExecuteQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("AddLeftNav_Pages", ex.Message, "Error", "");

                }
            }
            private void AddChildNodeToLeftNav(string webUrl)
            {
                try
                {
                    using (ClientContext clientContext = new ClientContext(webUrl))
                    {
                        clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                        {
                            Web w = clientContext.Web;
                            clientContext.Load(w);
                            NavigationNodeCollection qlNavNodeColl = w.Navigation.QuickLaunch;
                            clientContext.Load(qlNavNodeColl);
                            clientContext.ExecuteQuery();

                            NavigationNode careCoordinationNode = qlNavNodeColl.Where(n => n.Url.ToString().Contains("CareCoordination")).FirstOrDefault();

                            if (careCoordinationNode != null)
                            {
                                NavigationNodeCreationInformation navCarePlansCreateInfo = new NavigationNodeCreationInformation();
                                navCarePlansCreateInfo.Title = "Care Plans";
                                navCarePlansCreateInfo.Url = webUrl + @"Pages/CarePlans.aspx";
                                navCarePlansCreateInfo.AsLastNode = true;
                                careCoordinationNode.Children.Add(navCarePlansCreateInfo);

                                NavigationNodeCreationInformation navHospitalCreateInfo = new NavigationNodeCreationInformation();
                                navHospitalCreateInfo.Title = "Hospitalization Alerts";
                                navHospitalCreateInfo.Url = webUrl + @"Pages/HospitalAlerts.aspx";
                                navHospitalCreateInfo.AsLastNode = true;
                                careCoordinationNode.Children.Add(navHospitalCreateInfo);

                                NavigationNodeCreationInformation navMedicationCreateInfo = new NavigationNodeCreationInformation();
                                navMedicationCreateInfo.Title = "Medication Alerts";
                                navMedicationCreateInfo.Url = webUrl + @"Pages/MedicationAlerts.aspx";
                                navMedicationCreateInfo.AsLastNode = true;
                                careCoordinationNode.Children.Add(navMedicationCreateInfo);

                                clientContext.ExecuteQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("AddChildNodeToLeftNav", ex.Message, "Error", "");
                }
            }

            private void Mnt_AddChildNodeToLeftNav(string webUrl, string navName)
            {
                try
                {
                    using (ClientContext clientContext = new ClientContext(webUrl))
                    {
                        clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                        {
                            Web w = clientContext.Web;
                            clientContext.Load(w);
                            NavigationNodeCollection qlNavNodeColl = w.Navigation.QuickLaunch;
                            clientContext.Load(qlNavNodeColl);
                            clientContext.ExecuteQuery();

                            NavigationNode parentNode = qlNavNodeColl.Where(n => n.Url.ToString().Contains(navName)).FirstOrDefault();

                            if (parentNode != null)
                            {
                                NavigationNodeCreationInformation navCarePlansCreateInfo = new NavigationNodeCreationInformation();
                                navCarePlansCreateInfo.Title = "Care Plans";
                                navCarePlansCreateInfo.Url = webUrl + @"/Pages/CarePlans.aspx";
                                navCarePlansCreateInfo.AsLastNode = true;
                                parentNode.Children.Add(navCarePlansCreateInfo);

                                NavigationNodeCreationInformation navHospitalCreateInfo = new NavigationNodeCreationInformation();
                                navHospitalCreateInfo.Title = "Hospitalization Alerts";
                                navHospitalCreateInfo.Url = webUrl + @"/Pages/HospitalAlerts.aspx";
                                navHospitalCreateInfo.AsLastNode = true;
                                parentNode.Children.Add(navHospitalCreateInfo);

                                NavigationNodeCreationInformation navMedicationCreateInfo = new NavigationNodeCreationInformation();
                                navMedicationCreateInfo.Title = "Medication Alert";
                                navMedicationCreateInfo.Url = webUrl + @"/Pages/MedicationAlerts.aspx";
                                navMedicationCreateInfo.AsLastNode = true;
                                parentNode.Children.Add(navMedicationCreateInfo);

                                clientContext.ExecuteQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("AddChildNodeToLeftNav", ex.Message, "Error", "");
                }
            }

        }

        public class TopNavigation_InitialAdjustmentPublishing
        {
            public TopNavigation_InitialAdjustmentPublishing(string sUrl)
            {
                using (ClientContext clientContext = new ClientContext(sUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    {
                        try
                        {
                            Web w = clientContext.Web;
                            var navigation = new ClientPortalNavigation(w);

                            navigation.GlobalIncludePages = false;
                            navigation.GlobalIncludeSubSites = false;
                            navigation.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            SiteLogUtility.CreateLogEntry("TopNavigation_InitialAdjustmentPublishing", ex.Message, "Error", "");
                            clientContext.Dispose();
                        }
                    }
                }
            }
        }

        public class ClearTopNavigation
        {
            public ClearTopNavigation(string webUrl)
            {
                try
                {
                    using (ClientContext clientContext = new ClientContext(webUrl))
                    {
                        clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                        {
                            Web w = clientContext.Web;

                            try
                            {
                                NavigationNodeCollection collTopNode = w.Navigation.TopNavigationBar;
                                clientContext.Load(collTopNode);
                                clientContext.ExecuteQuery();

                                if (collTopNode.Count > 0)
                                {
                                    ClearNavigation(webUrl, collTopNode);
                                }
                            }
                            catch (Exception ex)
                            {
                                SiteLogUtility.CreateLogEntry("ClearQuickNavigation", ex.Message, "Error", "");
                                clientContext.Dispose();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("CNavigation - ClearTopNavigation", ex.Message, "Error", "");
                }
            }

            private static void ClearNavigation(string webUrl, NavigationNodeCollection nodes)
            {
                using (ClientContext clientContext = new ClientContext(webUrl))
                {
                    try
                    {
                        clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                        {
                            var w = clientContext.Web;
                            clientContext.Load(w);
                            clientContext.ExecuteQuery();

                            NavigationNodeCollection qlNodes = clientContext.Web.Navigation.TopNavigationBar;
                            clientContext.Load(qlNodes);
                            clientContext.ExecuteQuery();

                            List<NavigationNode> qlNodesToDelete = new List<NavigationNode>();

                            foreach (var node in qlNodes)
                            {
                                // Delete child nodes...
                                clientContext.Load(node.Children);
                                clientContext.ExecuteQuery();
                                node.Children.ToList().ForEach(nodeChild => nodeChild.DeleteObject());
                                clientContext.ExecuteQuery();

                                // Add node to List to delete...
                                qlNodesToDelete.Add(node);
                            }

                            // Delete nodes from quick launch...
                            foreach (NavigationNode nodeItem in qlNodesToDelete)
                            {
                                clientContext.Load(nodeItem);
                                clientContext.ExecuteQuery();
                                nodeItem.DeleteObject();
                                clientContext.ExecuteQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("ClearNavigation", ex.Message, "Error", "");
                        clientContext.Dispose();
                        throw;
                    }

                }
            }
        }

        public class Navigation_AddToTopNode
        {
            List<NavPracPage.NavPracPageTopItems> newNavPracItems = new List<NavPracPage.NavPracPageTopItems>();
            public Navigation_AddToTopNode(string webUrl, string urlPMSite)
            {
                string adminUrl = string.Empty;

                try
                {
                    NavPracPage.NavPracPageTopItems pagePM = new NavPracPage.NavPracPageTopItems();
                    pagePM.NavPageName = pagePM.navPracPagePM;
                    pagePM.NavPageFileName = pagePM.navPracPageFilePM;
                    pagePM.NavPageUrl = urlPMSite;
                    pagePM.NavPageSort = 1;
                    pagePM.NavPageStatus = 1;
                    newNavPracItems.Add(pagePM);

                    adminUrl = SiteInfoUtility.LoadParentWeb(urlPMSite);

                    NavPracPage.NavPracPageTopItems pageAdmin = new NavPracPage.NavPracPageTopItems();
                    pageAdmin.NavPageName = pageAdmin.navPracPageAdmin;
                    pageAdmin.NavPageFileName = pageAdmin.navPracPageFileAdmin;
                    pageAdmin.NavPageUrl = adminUrl;
                    pageAdmin.NavPageSort = 2;
                    pageAdmin.NavPageStatus = 1;
                    newNavPracItems.Add(pageAdmin);

                    var sortedNavPracItems2 = newNavPracItems.OrderBy(x => x.NavPageStatus).ThenBy(x => x.NavPageSort).ToList();
                    foreach (NavPracPage.NavPracPageTopItems navItem in sortedNavPracItems2)
                    {
                        AddTopNav_Pages(navItem, webUrl, urlPMSite);
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("Navigation_AddToTopNode", ex.Message, "Error", "");
                }
            }

            private static void AddTopNav_Pages(NavPracPage.NavPracPageTopItems navItem, string webUrl, string pmUrl)
            {
                try
                {
                    using (ClientContext clientContext = new ClientContext(webUrl))
                    {
                        clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                        {
                            Web w = clientContext.Web;
                            clientContext.Load(w);
                            NavigationNodeCollection qlNavNodeColl = w.Navigation.TopNavigationBar;
                            clientContext.Load(qlNavNodeColl);
                            clientContext.ExecuteQuery();

                            NavigationNodeCreationInformation navCreateInfo = new NavigationNodeCreationInformation();
                            navCreateInfo.Title = navItem.NavPageName;
                            //navCreateInfo.Url = pmUrl + @"/Pages/" + navItem.NavPageFileName;
                            navCreateInfo.Url = navItem.NavPageUrl + @"/Pages/" + navItem.NavPageFileName;
                            navCreateInfo.AsLastNode = true;
                            qlNavNodeColl.Add(navCreateInfo);
                            clientContext.ExecuteQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("AddTopNav_Pages", ex.Message, "Error", "");

                }
            }
        }

        public static void NavigationInit(string pracUrl)
        {

        }

        /// <summary>
        /// Call this method to execute same methods as Template Deployment Practice Navigation
        /// </summary>
        /// <param name="pracUrl"></param>
        /// <param name="pmUrl"></param>
        public static void NavigationPracticeMnt(string pracUrl, string pmUrl)
        {
            try
            {
                QuickLaunch_InitialAdjustment(pracUrl);
                QuickLaunch_InitialAdjustmentPublishing(pracUrl);
                ClearQuickNavigation(pracUrl);
                new Navigation_AddPagesToNode(pracUrl);

                new TopNavigation_InitialAdjustmentPublishing(pracUrl);
                new ClearTopNavigation(pracUrl);
                new Navigation_AddToTopNode(pracUrl, pmUrl);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("NavigationMnt", ex.Message, "Error", "");
            }
        }

        /// <summary>
        /// Call this method to execute same methods as Template Deployment Practice Navigation
        /// </summary>
        /// <param name="pracUrl"></param>
        /// <param name="pmUrl"></param>
        public static void NavigationPracticeMntTop(string pracUrl, string pmUrl)
        {
            try
            {
                //QuickLaunch_InitialAdjustment(pracUrl);
                //QuickLaunch_InitialAdjustmentPublishing(pracUrl);
                //new Navigation_AddPagesToNode(pracUrl);

                new TopNavigation_InitialAdjustmentPublishing(pracUrl);
                new ClearTopNavigation(pracUrl);
                new Navigation_AddToTopNode(pracUrl, pmUrl);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("NavigationMnt", ex.Message, "Error", "");
            }
        }

        public static void ClearQuickNavigation(string wUrl)
        {
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                {
                    Web w = clientContext.Web;

                    try
                    {
                        PublishingWeb pweb = PublishingWeb.GetPublishingWeb(clientContext, w);
                        SPNavPub.WebNavigationSettings wnavs = new SPNavPub.WebNavigationSettings(clientContext, w);
                        NavigationNodeCollection collQuickLaunchNode = pweb.Web.Navigation.QuickLaunch;
                        clientContext.Load(collQuickLaunchNode);
                        clientContext.ExecuteQuery();

                        if (collQuickLaunchNode.Count > 0)
                        {
                            ClearNavigation(wUrl, collQuickLaunchNode);
                            //wnavs.Update();
                            //pweb.Update();
                            //w.Update();
                        }
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("ClearNavigation", ex.Message, "Error", "");
                    }
                }
            }
        }

        private static void ClearNavigation(string webUrl, NavigationNodeCollection nodes)
        {
            try
            {
                string strPortal = "portal";
                using (ClientContext clientContext = new ClientContext(webUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    {
                        var w = clientContext.Web;
                        clientContext.Load(w);
                        clientContext.ExecuteQuery();

                        NavigationNodeCollection qlNodes = clientContext.Web.Navigation.QuickLaunch;
                        clientContext.Load(qlNodes);
                        clientContext.ExecuteQuery();

                        foreach (var node in qlNodes)
                        {
                            if (w.Url.ToLower().Contains(strPortal))
                            {
                                clientContext.Load(node.Children);
                                clientContext.ExecuteQuery();
                                node.Children.ToList().ForEach(nodeChild => nodeChild.DeleteObject());
                                clientContext.ExecuteQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("ClearNavigation", ex.Message, "Error", "");
            }
        }

        public static void ClearQuickNavigationRecent(string wUrl)
        {
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                {
                    Web w = clientContext.Web;

                    try
                    {
                        PublishingWeb pweb = PublishingWeb.GetPublishingWeb(clientContext, w);
                        SPNavPub.WebNavigationSettings wnavs = new SPNavPub.WebNavigationSettings(clientContext, w);
                        NavigationNodeCollection collQuickLaunchNode = pweb.Web.Navigation.QuickLaunch;
                        clientContext.Load(collQuickLaunchNode);
                        clientContext.ExecuteQuery();

                        if (collQuickLaunchNode.Count > 0)
                        {
                            ClearNavigationRecent(wUrl, collQuickLaunchNode);
                            //wnavs.Update();
                            //pweb.Update();
                            //w.Update();
                        }
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("ClearQuickNavigationRecent", ex.Message, "Error", "");
                    }
                }
            }
        }

        private static void ClearNavigationRecent(string webUrl, NavigationNodeCollection nodes)
        {
            try
            {
                string strNodeToRemove = "Recent";
                using (ClientContext clientContext = new ClientContext(webUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    {
                        var w = clientContext.Web;
                        clientContext.Load(w);
                        clientContext.ExecuteQuery();

                        NavigationNodeCollection qlNodes = clientContext.Web.Navigation.QuickLaunch;
                        clientContext.Load(qlNodes);
                        clientContext.ExecuteQuery();

                        List<NavigationNode> qlNodesToDelete = new List<NavigationNode>();

                        foreach (var node in qlNodes)
                        {
                            if (node.Title.Contains(strNodeToRemove))
                            {
                                // Delete child nodes...
                                clientContext.Load(node.Children);
                                clientContext.ExecuteQuery();
                                node.Children.ToList().ForEach(nodeChild => nodeChild.DeleteObject());
                                clientContext.ExecuteQuery();

                                // Add node to List to delete...
                                qlNodesToDelete.Add(node);
                            }
                        }

                        // Delete nodes from quick launch...
                        foreach (NavigationNode nodeItem in qlNodesToDelete)
                        {
                            clientContext.Load(nodeItem);
                            clientContext.ExecuteQuery();
                            nodeItem.DeleteObject();
                            clientContext.ExecuteQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("ClearNavigationRecent", ex.Message, "Error", "");
            }
        }

        public static void RenameQuickNavigationNode(string wUrl, string nodeSearchName, string nodeNewName)
        {
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                {
                    Web w = clientContext.Web;

                    try
                    {
                        PublishingWeb pweb = PublishingWeb.GetPublishingWeb(clientContext, w);
                        SPNavPub.WebNavigationSettings wnavs = new SPNavPub.WebNavigationSettings(clientContext, w);
                        NavigationNodeCollection collQuickLaunchNode = pweb.Web.Navigation.QuickLaunch;
                        clientContext.Load(collQuickLaunchNode);
                        clientContext.ExecuteQuery();

                        if (collQuickLaunchNode.Count > 0)
                        {
                            RenameNavigationNode(wUrl, collQuickLaunchNode, nodeSearchName, nodeNewName);
                        }
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("RenameQuickNavigationNode", ex.Message, "Error", "");
                    }
                }
            }
        }

        private static void RenameNavigationNode(string webUrl, NavigationNodeCollection nodes, string nodeSearch, string nodeNewName)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(webUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    {
                        var w = clientContext.Web;
                        clientContext.Load(w);
                        clientContext.ExecuteQuery();

                        NavigationNodeCollection qlNodes = clientContext.Web.Navigation.QuickLaunch;
                        clientContext.Load(qlNodes);
                        clientContext.ExecuteQuery();

                        foreach (var node in qlNodes)
                        {
                            if (node.Title.Contains(nodeSearch))
                            {
                                // Rename child nodes...
                                node.Title = nodeNewName;
                                node.Update();
                                clientContext.ExecuteQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("RenameNavigationNode", ex.Message, "Error", "");
            }
        }

        public static void QuickLaunch_InitialAdjustmentPublishing(string sUrl)
        {
            using (ClientContext clientContext = new ClientContext(sUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                {
                    try
                    {
                        Web w = clientContext.Web;
                        var navigation = new ClientPortalNavigation(w);

                        navigation.CurrentIncludePages = false;
                        navigation.CurrentIncludeSubSites = false;
                        navigation.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("QuickLaunch_InitialAdjustmentPublishing", ex.Message, "Error", "");
                        clientContext.Dispose();
                    }
                }
            }
        }

        public static bool QuickLaunch_InitialAdjustment(string pracUrl)
        {
            using (ClientContext clientContext = new ClientContext(pracUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                {
                    try
                    {
                        Web w = clientContext.Web;
                        clientContext.Load(w);
                        clientContext.ExecuteQuery();

                        NavigationNodeCollection qlNodes = clientContext.Web.Navigation.QuickLaunch;
                        clientContext.Load(qlNodes);
                        clientContext.ExecuteQuery();

                        if (qlNodes.Count > 1)
                        {
                            int icount = qlNodes.Count - 1;
                            for (int i = 0; icount >= i; icount--)
                            {
                                NavigationNode nav;
                                nav = w.Navigation.QuickLaunch[i];
                                if (nav != null)
                                {
                                    //w.Navigation.QuickLaunch.Delete(nav);
                                    nav.DeleteObject();
                                    clientContext.ExecuteQuery();
                                }
                            }
                        }
                        w.Update();
                        clientContext.ExecuteQuery();
                    }
                    catch(Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("QuickLaunch_InitialAdjustment", ex.Message, "Error", "");
                        clientContext.Dispose();
                    }
                }
            }
            return true;
        }

        public static bool QuickLaunch_Print(string pracUrl)
        {
            using (ClientContext clientContext = new ClientContext(pracUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                {
                    try
                    {
                        Web w = clientContext.Web;
                        clientContext.Load(w);
                        clientContext.ExecuteQuery();

                        NavigationNodeCollection qlNodes = clientContext.Web.Navigation.QuickLaunch;
                        clientContext.Load(qlNodes);
                        clientContext.ExecuteQuery();

                        if (qlNodes.Count > 1)
                        {
                            foreach (NavigationNode node in qlNodes)
                            {
                                SiteLogUtility.Log_Entry($"{node.Title} - {node.Url}", true);
                            }
                            
                            //int icount = qlNodes.Count - 1;
                            //for (int i = 0; icount >= i; icount--)
                            //{
                            //    NavigationNode nav;
                            //    nav = w.Navigation.QuickLaunch[i];
                            //    if (nav != null)
                            //    {
                            //        //clientContext.ExecuteQuery();
                            //        SiteLogUtility.Log_Entry($"{nav.Title} - {nav.Url}", true);
                            //    }
                            //}
                        }
                        w.Update();
                        clientContext.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.CreateLogEntry("QuickLaunch_InitialAdjustment", ex.Message, "Error", "");
                        clientContext.Dispose();
                    }
                }
            }
            return true;
        }
    }
}

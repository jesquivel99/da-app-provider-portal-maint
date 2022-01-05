using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;

namespace PracticeSiteFileMigration
{
    //public class Practice
    //{
    //    public string PMGroup;
    //    public string Name;
    //    public string TIN;
    //    public string SiteID;
    //    public string NewSiteUrl;
    //    public string ExistingSiteUrl;
    //    public PracticeType Type;
    //    public Practice()
    //    {
    //    }
    //}

    //public enum PracticeType { IWH, iCKCC };
    //public enum FolderType { IWH, iCKCC, BOTH };
    //public enum SpServer { DEV, PROD };
    //public class PracticeUltility
    //{
    //    public const string SpUserName = "spAdmin";
    //    public const string SpPassword = "Xfw4E9fcis6nj5";
    //    public const string SpUserDomain = "medspring";

    //    public List<Practice> practicesIWH = new List<Practice>();
    //    public List<Practice> practicesCKCC = new List<Practice>();
    //    public SpServer SpDeployServer;

    //    public PracticeUltility(SpServer spServer)
    //    {
    //        SpDeployServer = spServer;
    //    }

    //    public List<Practice> GetAllPracticeNewSites(ClientContext clientContext, List<Practice> practices, PracticeType practiceType)
    //    {
    //        string srcUrlIWH = "https://sharepointdev.fmc-na-icg.com/bi/fhppp/iwn/";
    //        string srcUrlCKCC = "https://sharepointdev.fmc-na-icg.com/bi/fhppp/interimckcc/";

    //        if (SpDeployServer == SpServer.PROD)
    //        {
    //            srcUrlIWH = "https://sharepoint.fmc-na-icg.com/bi/fhppp/iwn/";
    //            srcUrlCKCC = "https://sharepoint.fmc-na-icg.com/bi/fhppp/interimckcc/";
    //        }

    //        string rootURL = "";

    //        using (ClientContext clientContextIWH = new ClientContext(srcUrlIWH))
    //        {
    //            clientContextIWH.Credentials = new NetworkCredential(SpUserName, SpPassword, SpUserDomain);
    //            practicesIWH = GetAllPracticeExistingSites(clientContextIWH, practicesIWH, PracticeType.IWH);
    //        }
    //        using (ClientContext clientContextCKCC = new ClientContext(srcUrlCKCC))
    //        {
    //            clientContextCKCC.Credentials = new NetworkCredential(SpUserName, SpPassword, SpUserDomain);
    //            practicesCKCC = GetAllPracticeExistingSites(clientContextCKCC, practicesCKCC, PracticeType.iCKCC);
    //        }

    //        clientContext.Load(clientContext.Web);
    //        clientContext.Load(clientContext.Web.Webs);
    //        clientContext.ExecuteQuery();

    //        int index = clientContext.Web.Url.IndexOf("portal");
    //        rootURL = clientContext.Web.Url.Substring(0, index);

    //        foreach (Web web in clientContext.Web.Webs)
    //        {
    //            if (Char.IsDigit(web.Url.Last()))
    //            {
    //                using (ClientContext clientContext0 = new ClientContext(web.Url))
    //                {
    //                    clientContext0.Load(clientContext0.Web);
    //                    clientContext0.Load(clientContext0.Web.Webs);
    //                    clientContext0.ExecuteQuery();

    //                    if (clientContext0.Web.Url.Contains("/PM"))
    //                    {
    //                        string group = clientContext0.Web.Url.Substring(clientContext0.Web.Url.Length - 2);
    //                        //if (group != "06") continue;

    //                        foreach (Web web0 in clientContext0.Web.Webs)
    //                        {
    //                            Practice practice = new Practice();

    //                            practice.PMGroup = group;
    //                            practice.Name = web0.Title;
    //                            practice.NewSiteUrl = web0.Url;
    //                            practice.Type = practiceType;
    //                            practice.SiteID = practice.NewSiteUrl.Substring(practice.NewSiteUrl.Length - 11); //"9" + Reverse(practice.TIN) + "9";
    //                            practice.TIN = Reverse(practice.SiteID.Substring(1, practice.SiteID.Length - 2));
    //                            practice.ExistingSiteUrl = MapExistingSite(practice.TIN);

    //                            practices.Add(practice);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        return practices;
    //    }
    //    private static List<Practice> GetAllPracticeExistingSites(ClientContext clientContext, List<Practice> practices, PracticeType practiceType)
    //    {
    //        clientContext.Load(clientContext.Web);
    //        clientContext.Load(clientContext.Web.Webs);
    //        clientContext.ExecuteQuery();

    //        foreach (Web web in clientContext.Web.Webs)
    //        {
    //            if (Char.IsDigit(web.Url.Last()))
    //            {
    //                using (ClientContext clientContext0 = new ClientContext(web.Url))
    //                {
    //                    clientContext0.Load(clientContext0.Web);
    //                    clientContext0.Load(clientContext0.Web.Webs);
    //                    clientContext0.ExecuteQuery();

    //                    if (clientContext0.Web.Url.Contains("/ICKCCGroup") || clientContext0.Web.Url.Contains("/iwn"))
    //                    {
    //                        string group = clientContext0.Web.Url.Substring(clientContext0.Web.Url.Length - 2);

    //                        if (group.CompareTo("12") < 0)
    //                        {
    //                            foreach (Web web0 in clientContext0.Web.Webs)
    //                            {
    //                                Practice practice = new Practice();
    //                                practice.ExistingSiteUrl = web0.Url;
    //                                practices.Add(practice);
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        return practices;
    //    }

    //    private string MapExistingSite(string TIN)
    //    {
    //        Practice practice = practicesIWH.Where(p => p.ExistingSiteUrl.Contains(TIN)).FirstOrDefault();
    //        if (practice == null)
    //            practice = practicesCKCC.Where(p => p.ExistingSiteUrl.Contains(TIN)).FirstOrDefault();

    //        if (practice == null)
    //        {
    //            Console.WriteLine(TIN);
    //            return "";
    //        }
    //        else
    //            return practice.ExistingSiteUrl;
    //    }
    //    private string Reverse(string s)
    //    {
    //        char[] charArray = s.ToCharArray();
    //        Array.Reverse(charArray);
    //        return new string(charArray);
    //    }
    //}
}

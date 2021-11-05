using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.SharePoint.Client;

namespace SiteUtility
{
    public class SiteInfoUtility
    {
        public static List<ProgramManagerSite> getSubWebs(string path, string rootUrl)
        {
            List<ProgramManagerSite> pmSites = new List<ProgramManagerSite>();
            List<PracticeSite> practices = new List<PracticeSite>();
            try
            {
                using (ClientContext ctx = new ClientContext(path))
                {
                    Web web = ctx.Web;
                    ctx.Load(web, w => w.Webs,
                                       w => w.Title,
                                       w => w.Description,
                                       w => w.ServerRelativeUrl,
                                       w => w.Url,
                                       w => w.Navigation);
                    ctx.ExecuteQuery();

                    foreach (Web w in web.Webs)
                    {
                        string newpath = rootUrl + w.ServerRelativeUrl;
                        Console.WriteLine(newpath);

                        getSubWebs(newpath, rootUrl);

                        PracticeSite prac = new PracticeSite();
                        prac.Name = w.Title;
                        prac.URL = w.Url;
                        practices.Add(prac);
                    }
                    ProgramManagerSite pmsite = new ProgramManagerSite();
                    pmsite.URL = web.Url;
                    pmSites.Add(pmsite);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return pmSites;
        }

        public static void GetPMPracticeDetails(ClientContext clientContext)
        {
            clientContext.Load(clientContext.Web.Webs);
            clientContext.ExecuteQuery();

            foreach (Web web in clientContext.Web.Webs)
            {
                if (Char.IsDigit(web.Url.Last()))
                {
                    using (ClientContext clientContext0 = new ClientContext(web.Url))
                    {
                        clientContext0.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                        clientContext0.Load(clientContext0.Web.Webs);
                        clientContext0.ExecuteQuery();

                        foreach (Web web0 in clientContext0.Web.Webs)
                        {
                            //Practice practice = new Practice();
                            //practice.Name = web0.Title;
                            //practice.Url = web0.Url;
                            //practice.Type = practiceType;
                            //Practices.Add(practice);
                        }
                    }
                }
            }
        }

        public static List<ProgramManagerSite> GetAllPracticeDetails(ClientContext clientContext)
        {
            clientContext.Load(clientContext.Web.Webs);
            clientContext.ExecuteQuery();

            List<ProgramManagerSite> pmSites = new List<ProgramManagerSite>();

            foreach (Web web in clientContext.Web.Webs)
            {

                ProgramManagerSite pmSite = new ProgramManagerSite();
                pmSite.ProgramManagerName = web.Url;
                pmSite.PracticeSiteCollection = new List<PracticeSite>();

                using (ClientContext clientContext0 = new ClientContext(web.Url))
                {
                    clientContext0.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    clientContext0.Load(clientContext0.Web.Webs);
                    clientContext0.ExecuteQuery();

                    foreach (Web web0 in clientContext0.Web.Webs)
                    {
                        PracticeSite practiceSite = new PracticeSite();
                        practiceSite.Name = web0.Title;
                        practiceSite.URL = web0.Url;
                        pmSite.PracticeSiteCollection.Add(practiceSite);

                        //Practice practice = new Practice();
                        //practice.Name = web0.Title;
                        //practice.Url = web0.Url;
                        //practice.Type = practiceType;
                        //Practices.Add(practice);
                    }
                }
                pmSites.Add(pmSite);
            }
            Console.WriteLine("1. GetAllPracticeDetails - Complete");
            return pmSites;
        }
    }
}

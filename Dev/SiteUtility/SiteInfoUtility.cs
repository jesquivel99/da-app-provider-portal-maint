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
            clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

            clientContext.Load(clientContext.Web.Webs);
            clientContext.ExecuteQuery();
            string strUrl = clientContext.Url;

            List<ProgramManagerSite> pmSites = new List<ProgramManagerSite>();

            try
            {
                foreach (Web web in clientContext.Web.Webs)
                {
                    if (web.Url.Contains("admingroup01") == false)
                    {
                        PmAssignment pmAssignments = GetPM(web.Url);
                        ProgramManagerSite pmSite = new ProgramManagerSite();
                        pmSite.ProgramManagerName = pmAssignments.PMName;
                        pmSite.PMURL = web.Url;
                        pmSite.URL = web.Url;
                        pmSite.ProgramManager = pmAssignments.PMRefId;
                        pmSite.IWNSiteMgrPermission = pmAssignments.PMGroup + "_SiteManager";
                        pmSite.IWNSiteMgrReadOnlyPermission = pmAssignments.PMGroup + "_ReadOnly";
                        pmSite.PracticeSiteCollection = new List<PracticeSite>();

                        SiteLogUtility.Log_Entry(SiteLogUtility.textLine);
                        SiteLogUtility.Log_Entry($"{pmSite.ProgramManagerName} - {pmSite.ProgramManager}");
                        SiteLogUtility.Log_Entry(pmSite.PMURL);

                        using (ClientContext clientContext0 = new ClientContext(web.Url))
                        {
                            clientContext0.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                            clientContext0.Load(clientContext0.Web.Webs);
                            clientContext0.ExecuteQuery();

                            foreach (Web web0 in clientContext0.Web.Webs)
                            {
                                string siteId = GetPracSiteName(web0.Url);
                                siteId = DecryptPTIN(siteId);
                                PracticeSite practiceSite = new PracticeSite();
                                practiceSite.Name = web0.Title;
                                practiceSite.URL = web0.Url;
                                practiceSite.PracticeTIN = siteId;
                                practiceSite.PracUserPermission = $"Prac_{siteId}_User";
                                practiceSite.PracUserReadOnlyPermission = $"Prac_{siteId}_ReadOnly";
                                pmSite.PracticeSiteCollection.Add(practiceSite);

                                SiteLogUtility.Log_Entry(practiceSite.Name);
                                SiteLogUtility.Log_Entry(practiceSite.URL);
                            }
                        }

                        if (pmSite.PMURL.Contains("admingroup01") == false)
                        {
                            pmSites.Add(pmSite);
                            SiteLogUtility.Log_Entry($"Total Practices:  {pmSite.PracticeSiteCollection.Count}");
                        } 
                    }
                    
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetAllPracticeDetails", ex.Message, "Error", strUrl);
                throw;
            }
            
            SiteLogUtility.Log_Entry("1. GetAllPracticeDetails - Complete", true);
            return pmSites;
        }


        /// <summary>
        /// This method is called by GetPM(string sUrl).
        /// 
        /// Returns a List of PmAssignment class which will be used as a cross-reference table to find a match from the PM SiteName.
        /// EXAMPLE:
        ///   If the URL is https://sharepointdev.fmc-na-icg.com/bi/fhppp/portal/PracticeSite20_PM01
        ///   the last two characters of the URL (01 in this example) will be compared to the PMRefId and return the match.
        ///   
        ///   The PMGroup is hard-coded with 'IWNRegionXX' so it can be used with the existing SPGroups
        /// </summary>
        /// <returns> List<PmAssignment> </returns>
        private static List<PmAssignment> GetPmAssignments()
        {
            List<PmAssignment> pmAssignments = new List<PmAssignment>();

            PmAssignment pma1 = new PmAssignment() { PMRefId = "01", PMName = "Angela Korf", PMGroup = "IWNRegion01" };
            PmAssignment pma2 = new PmAssignment() { PMRefId = "02", PMName = "Annie Fambro", PMGroup = "IWNRegion02" };
            PmAssignment pma3 = new PmAssignment() { PMRefId = "03", PMName = "Francisco Calles", PMGroup = "IWNRegion03" };
            PmAssignment pma4 = new PmAssignment() { PMRefId = "04", PMName = "Glenda S Wooten", PMGroup = "IWNRegion04" };
            PmAssignment pma5 = new PmAssignment() { PMRefId = "05", PMName = "Karen Bauer", PMGroup = "IWNRegion05" };
            PmAssignment pma6 = new PmAssignment() { PMRefId = "06", PMName = "Katelyn Minnick", PMGroup = "IWNRegion06" };
            PmAssignment pma7 = new PmAssignment() { PMRefId = "07", PMName = "Linda S Biermann", PMGroup = "IWNRegion07" };
            PmAssignment pma8 = new PmAssignment() { PMRefId = "08", PMName = "Marion C Grantham", PMGroup = "IWNRegion08" };
            PmAssignment pma9 = new PmAssignment() { PMRefId = "09", PMName = "Thomas Locke", PMGroup = "IWNRegion09" };
            PmAssignment pma10 = new PmAssignment() { PMRefId = "10", PMName = "Kristina Dunigan", PMGroup = "IWNRegion10" };

            pmAssignments.Add(pma1);
            pmAssignments.Add(pma2);
            pmAssignments.Add(pma3);
            pmAssignments.Add(pma4);
            pmAssignments.Add(pma5);
            pmAssignments.Add(pma6);
            pmAssignments.Add(pma7);
            pmAssignments.Add(pma8);
            pmAssignments.Add(pma9);
            pmAssignments.Add(pma10);

            return pmAssignments;
        }

        /// <summary>
        /// This method gets the Program Manager cross-reference data
        /// and the SiteName, to return a PmAssignment class.
        /// 
        /// Needed a method to get the last two characters of the Program Manager URL and return
        /// the Program Manager Name and the name of the SPGroup to be used for permissions.
        /// </summary>
        /// <param name="sUrl"></param>
        /// <returns> PmAssignment class </returns>
        public static PmAssignment GetPM(string sUrl)
        {
            string siteName = string.Empty;
            List<PmAssignment> pmAssignments = new List<PmAssignment>();

            try
            {
                pmAssignments = GetPmAssignments();
                siteName = GetSiteName(sUrl);

                int sStart = siteName.Length - 2;
                string PMid = siteName.Substring(sStart, 2);
                PmAssignment result = pmAssignments.Find(x => x.PMRefId == PMid);

                return result;
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetPM", ex.Message, "Error", "");
                throw;
            }

            
        }

        public class PmAssignment
        {
            public PmAssignment()
            {

            }

            public string PMRefId { get; set; }
            public string PMName { get; set; }
            public string PMGroup { get; set; }
        }

        public static string GetSiteName(string sUrl)
        {
            Uri pracUrl = new Uri(sUrl);
            int segCnt = pracUrl.Segments.Count();
            string siteName = segCnt > 4 ? pracUrl.Segments.Last() : string.Empty;

            return siteName;
        }

        public static string GetPracSiteName(string sUrl)
        {
            Uri pracUrl = new Uri(sUrl);
            int segCnt = pracUrl.Segments.Count();
            string siteName = segCnt > 4 ? pracUrl.Segments.Last() : string.Empty;

            return siteName;
        }

        public static string DecryptPTIN(string s)
        {
            try
            {
                int sLen = s.Length;
                string sFirst = s.Substring(0, 1);
                string sLast = s.Substring(sLen - 1, 1);

                if (sFirst.Equals("9") && sLast.Equals("9"))
                {
                    s = s.Substring(1, sLen - 1);
                    sLen = s.Length;
                    s = s.Substring(0, sLen - 1);
                }

                char[] charArray = s.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("DecryptPTIN", ex.Message, "Error", "");
                return s;
            }
        }

        /// <summary>
        /// Method will receive CSV file input
        /// Utilize existing Classes
        /// </summary>
        public static class GenericTextFileProcessor
        {
            public static List<T> LoadFromTextFile<T>(string filePath) where T : class, new()
            {
                var lines = System.IO.File.ReadAllLines(filePath).ToList();
                List<T> output = new List<T>();
                T entry = new T();
                var cols = entry.GetType().GetProperties();

                // Checks to be sure we have at least one header row and one data row...
                if (lines.Count < 2)
                {
                    throw new IndexOutOfRangeException("The file was either empty or missing.");
                }

                // Splits the header into one column header per entry...
                var headers = lines[0].Split(',');

                // Removes header row from the lines so we don't
                //  have to worry about skipping over that first row.
                lines.RemoveAt(0);

                foreach (var row in lines)
                {
                    entry = new T();
                    var vals = row.Split(',');

                    for (int i = 0; i < headers.Length; i++)
                    {
                        foreach (var col in cols)
                        {
                            if (col.Name == headers[i])
                            {
                                col.SetValue(entry, Convert.ChangeType(vals[i], col.PropertyType));
                            }
                        }
                    }

                    output.Add(entry);
                }

                return output;
            }
        }

        public static void loadFromTextFile()
        {
            // Load records to process into PracticeSite...
            //List<PracticeSite> newSiteInfo = SiteInfoUtility.GenericTextFileProcessor.LoadFromTextFile<PracticeSite>(siteInfoFile);
            //SiteLogUtility.Log_Entry("Will be processed: ");
            //foreach (var item in newSiteInfo)
            //{
            //    Console.WriteLine($"{item.URL}, {item.Name}");
            //    SiteLogUtility.Log_Entry($"{item.URL}, {item.Name}");
            //}
            //Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.SharePoint.Client;

namespace SiteUtility
{
    public class SitePublishUtility
    {
        public static void PublishPage(PracticeSite practiceSite)
        {
            using (ClientContext clientContext = new ClientContext(practiceSite.URL))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                Web web = clientContext.Web;
                clientContext.Load(web);
                //clientContext.ExecuteQuery();

                // This value is NOT List internal name
                List targetList = clientContext.Web.Lists.GetByTitle("Pages");
                clientContext.Load(targetList);
                clientContext.ExecuteQuery();

                Folder folder = targetList.RootFolder;
                FileCollection files = folder.Files;
                clientContext.Load(files);
                clientContext.ExecuteQuery();

                SiteLogUtility.Log_Entry(targetList.EntityTypeName + " - " + practiceSite.URL, true);
                foreach (File f in files)
                {
                    SiteLogUtility.Log_Entry(f.Name, true);
                }
            }
        }
        public static void PublishFunction1()
        {
            Console.WriteLine("PublishFunction 1");
        }
        public static void PublishFunction2()
        {
            Console.WriteLine("PublishFunction 2");
        }
    }
}

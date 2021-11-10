using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SiteUtility
{
    public class SiteDeleteUtility
    {
        public void DeleteSite(string sURL)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(sURL))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    {
                        // Make sure the site you are deleting is no longer in use.
                        // Once a site is deleted, there's no way you can recover the site.
                        clientContext.Web.DeleteObject();
                        clientContext.ExecuteQuery();
                    }

                }
            }
            catch(Exception ex)
            {
                SiteLogUtility.CreateLogEntry("DeleteSite", ex.Message, "Error", sURL);
            }
            
        }
    }
}

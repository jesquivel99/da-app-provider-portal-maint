using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SiteUtility
{
    public class SiteFilesUtility
    {
        public void DocumentUpload(string siteURL, string filePath, string LibraryName)
        {
            using (ClientContext clientContext = new ClientContext(siteURL))
            {
                try
                {
                    string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);

                    FileCreationInformation fcInfo = new FileCreationInformation();
                    fcInfo.Url = fileName;
                    fcInfo.Overwrite = true;
                    fcInfo.Content = System.IO.File.ReadAllBytes(filePath);

                    Web myWeb = clientContext.Web;
                    List myLibrary = myWeb.Lists.GetByTitle(LibraryName);
                    myLibrary.RootFolder.Files.Add(fcInfo);
                    clientContext.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("DocumentUpload", ex.Message, "Error", siteURL);
                }
            }
        }
    }
}

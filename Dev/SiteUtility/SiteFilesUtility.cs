using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SiteUtility
{
    public class SiteFilesUtility
    {
        public void DocumentUpload(string siteURL, string filePath, string LibraryName)
        {
            using (ClientContext clientContext = new ClientContext(siteURL))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

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

                    SiteLogUtility.Log_Entry($"--      Pages Audit: {siteURL}/{LibraryName}/{fileName}", true);
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("DocumentUpload", ex.Message, "Error", siteURL);
                }
            }
        }

        public void uploadImageSupportingFiles(string wUrl)
        {
            string LayoutsFolder = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config";
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    string LibraryName = "Site Assets";
                    List<FileCreationInformation> imgFileCreateInfo = new List<FileCreationInformation>();

                    // Get jpg files...
                    string[] files0 = Directory.GetFiles(Path.Combine(LayoutsFolder, "Img"), "*.*");

                    // Create FileCreateInfo and add to List<>...
                    foreach (var item in files0)
                    {
                        byte[] f0 = System.IO.File.ReadAllBytes(item);
                        FileInfo fileName = new FileInfo(Path.GetFileName(item));

                        FileCreationInformation fc0 = new FileCreationInformation();
                        fc0.Url = fileName.ToString();
                        fc0.Overwrite = true;
                        fc0.Content = f0;

                        imgFileCreateInfo.Add(fc0);
                    }

                    // Get Site Assets
                    List myLibrary = web.Lists.GetByTitle(LibraryName);
                    clientContext.ExecuteQuery();

                    // Upload image files to "Site Assets/Img" folder...
                    clientContext.Load(myLibrary.RootFolder.Folders);
                    clientContext.ExecuteQuery();

                    foreach (Folder SubFolder in myLibrary.RootFolder.Folders)
                    {
                        if (SubFolder.Name.Equals("Img"))
                        {
                            foreach (FileCreationInformation fileCreationInfo in imgFileCreateInfo)
                            {
                                clientContext.Load(SubFolder.Files.Add(fileCreationInfo));
                            }
                            clientContext.ExecuteQuery();
                        }
                    }

                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("uploadImageSupportingFiles", ex.Message, "Error", wUrl);
                }
            }
        }

        public void CreateRedirectPage(string redirUrl)
        {
            string path = @"c:\temp\Home.aspx";
            List<string> lines = new List<string>();

            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                lines.Add("<html>");
                lines.Add("<body>");
                lines.Add("<script>");
                lines.Add(@"(function () {");
                lines.Add($"window.location.replace('{redirUrl}');");
                lines.Add(@"})();");
                lines.Add("</script>");
                lines.Add("</body>");
                lines.Add("</html>");

                System.IO.File.AppendAllLines(path, lines);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("CreateRedirectPage", ex.Message, "Error", "");
            }
        }
    }
}

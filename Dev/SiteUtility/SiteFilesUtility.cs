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
            SiteLogUtility.Log_Entry("   DocumentUpload - In Progress...");
            using (ClientContext clientContext = new ClientContext(siteURL))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);

                    var fileCreationInfo = new FileCreationInformation
                    {
                        Content = System.IO.File.ReadAllBytes(filePath),
                        Overwrite = true,
                        Url = Path.GetFileName(filePath)
                    };
                    var targetFolder = clientContext.Web.GetFolderByServerRelativeUrl(LibraryName);
                    var uploadFile = targetFolder.Files.Add(fileCreationInfo);
                    clientContext.Load(uploadFile);
                    clientContext.ExecuteQuery();
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

        public void uploadImageSupportingFilesSingleImage(string wUrl, string imgFileName)
        {
            SiteLogUtility.Log_Entry("   UploadImageSupportingFilesSingleImage - In Progress...");
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

                        if (fileName.ToString().ToLower() == imgFileName.ToLower())
                        {
                            FileCreationInformation fc0 = new FileCreationInformation();
                            fc0.Url = fileName.ToString();
                            fc0.Overwrite = true;
                            fc0.Content = f0;

                            imgFileCreateInfo.Add(fc0); 
                        }
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
        public void uploadHtmlSupportingFilesSingleFile(string wUrl, string htmlFileName)
        {
            SiteLogUtility.Log_Entry("   UploadHtmlSupportingFilesSingleFile - In Progress...");
            string LayoutsFolderMnt = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    string LibraryName = "Site Assets";
                    List<FileCreationInformation> htmlFileCreateInfo = new List<FileCreationInformation>();


                    // Get cePrac html files...
                    string[] cePracFiles = Directory.GetFiles(LayoutsFolderMnt, "cePrac*.html");

                    // File into byte array and add to List<>...
                    foreach (var item in cePracFiles)
                    {
                        byte[] f0 = System.IO.File.ReadAllBytes(item);
                        FileInfo fileName = new FileInfo(Path.GetFileName(item));

                        if (fileName.ToString().ToLower() == htmlFileName.ToLower())
                        {
                            FileCreationInformation fc0 = new FileCreationInformation();
                            fc0.Url = fileName.ToString();
                            fc0.Overwrite = true;
                            fc0.Content = f0;

                            htmlFileCreateInfo.Add(fc0);
                        }
                    }

                    // Get Site Assets
                    List myLibrary = web.Lists.GetByTitle(LibraryName);
                    clientContext.ExecuteQuery();

                    // Upload html files to "Site Assets" folder...
                    clientContext.Load(myLibrary.RootFolder);
                    clientContext.ExecuteQuery();

                    foreach (FileCreationInformation fileCreationInfo in htmlFileCreateInfo)
                    {
                        clientContext.Load(myLibrary.RootFolder.Files.Add(fileCreationInfo));
                    }
                    clientContext.ExecuteQuery();

                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("uploadHtmlSupportingFiles", ex.Message, "Error", wUrl);
                }
            }
        }
        public void uploadProgramPracticeSupportFilesWoDialysisStarts(PracticeSite practiceSite)
        {
            string siteType = practiceSite.siteType;

            if (siteType == "")
            {
                return;
            }
            string LayoutsFolder = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
            using (ClientContext clientContext = new ClientContext(practiceSite.URL))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    string rootWebUrl = GetRootSite(practiceSite.URL);

                    string LibraryName = "Program Participation";

                    //string fileName0 = "EducationReviewPro.JPG";
                    string fileName1 = "KCEckcc.JPG";
                    //string fileName2 = "PracticeReferrals.JPG";

                    //byte[] f0 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName0);
                    byte[] f1 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName1);
                    //byte[] f2 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName2);

                    //FileCreationInformation fc0 = new FileCreationInformation();
                    //fc0.Url = fileName0;
                    //fc0.Overwrite = true;
                    //fc0.Content = f0;

                    FileCreationInformation fc1 = new FileCreationInformation();
                    fc1.Url = fileName1;
                    fc1.Overwrite = true;
                    fc1.Content = f1;

                    //FileCreationInformation fc2 = new FileCreationInformation();
                    //fc2.Url = fileName2;
                    //fc2.Overwrite = true;
                    //fc2.Content = f2;

                    List myLibrary = web.Lists.GetByTitle(LibraryName);

                    //if (siteType != null && siteType.Contains("kc365"))
                    //{
                    //    Microsoft.SharePoint.Client.File newFile2 = myLibrary.RootFolder.Files.Add(fc2);
                    //    clientContext.Load(newFile2);
                    //    clientContext.ExecuteQuery();

                    //    ListItem lItem2 = newFile2.ListItemAllFields;
                    //    lItem2.File.CheckOut();
                    //    clientContext.ExecuteQuery();
                    //    lItem2["Title"] = "Payor Enrollment";
                    //    lItem2["ProgramNameText"] = rootWebUrl + "/bi/fhppp/iwn/EnrollmentReferrals/SitePages/ReferralSearch.aspx?qsptine=" + practiceSite.EncryptedPracticeTIN;
                    //    lItem2["Thumbnail"] = practiceSite.URL + "/Program%20Participation/" + fileName2;
                    //    lItem2.Update();
                    //    lItem2.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                    //    clientContext.ExecuteQuery();
                    //}

                    if (siteType != null && siteType.Contains("ckcc"))
                    {
                        Microsoft.SharePoint.Client.File newFile1 = myLibrary.RootFolder.Files.Add(fc1);
                        clientContext.Load(newFile1);
                        clientContext.ExecuteQuery();

                        ListItem lItem1 = newFile1.ListItemAllFields;
                        lItem1.File.CheckOut();
                        clientContext.ExecuteQuery();
                        //lItem1["Title"] = "CKCC/KCE Coming Soon!";
                        lItem1["Title"] = "CKCC/KCE";
                        lItem1["ProgramNameText"] = practiceSite.URL + "/Pages/ProgramParticipation.aspx";
                        lItem1["Thumbnail"] = practiceSite.URL + "/Program%20Participation/" + fileName1;
                        lItem1.Update();
                        lItem1.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();
                    }

                    //if (siteType != null && siteType.Contains("iwh"))
                    //{
                    //    Microsoft.SharePoint.Client.File newFile0 = myLibrary.RootFolder.Files.Add(fc0);
                    //    clientContext.Load(newFile0);
                    //    clientContext.ExecuteQuery();

                    //    ListItem lItem0 = newFile0.ListItemAllFields;
                    //    lItem0.File.CheckOut();
                    //    clientContext.ExecuteQuery();
                    //    lItem0["Title"] = "Payor Program Education Resources Coming Soon!";
                    //    //lItem0["Title"] = "Payor Program Education Resources";
                    //    lItem0["ProgramNameText"] = practiceSite.URL + "/Pages/ProgramParticipation.aspx";
                    //    lItem0["Thumbnail"] = practiceSite.URL + "/Program%20Participation/" + fileName0;
                    //    lItem0.Update();
                    //    lItem0.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                    //    clientContext.ExecuteQuery();
                    //}
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("uploadProgramPracticeSupportFiles", ex.Message, "Error", practiceSite.URL);
                }
            }
        }
        public void uploadProgramPracticeSupportFiles_SingleFile(PracticeSite practiceSite, string imgName, string imgUrl, string progrPracTitle)
        {
            string siteType = practiceSite.siteType;

            if (siteType == "")
            {
                return;
            }
            string LayoutsFolder = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
            using (ClientContext clientContext = new ClientContext(practiceSite.URL))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    string rootWebUrl = GetRootSite(practiceSite.URL);

                    string LibraryName = "Program Participation";

                    string fileName0 = "EducationReviewPro.JPG";
                    string fileName1 = "KCEckcc.JPG";
                    //string fileName2 = "PracticeReferrals.JPG";

                    byte[] f0 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName0);
                    byte[] f1 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName1);
                    //byte[] f2 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName2);

                    FileCreationInformation fc0 = new FileCreationInformation();
                    fc0.Url = fileName0;
                    fc0.Overwrite = true;
                    fc0.Content = f0;

                    FileCreationInformation fc1 = new FileCreationInformation();
                    fc1.Url = fileName1;
                    fc1.Overwrite = true;
                    fc1.Content = f1;

                    //FileCreationInformation fc2 = new FileCreationInformation();
                    //fc2.Url = fileName2;
                    //fc2.Overwrite = true;
                    //fc2.Content = f2;

                    List myLibrary = web.Lists.GetByTitle(LibraryName);

                    //if (siteType != null && siteType.Contains("kc365"))
                    //{
                    //    Microsoft.SharePoint.Client.File newFile2 = myLibrary.RootFolder.Files.Add(fc2);
                    //    clientContext.Load(newFile2);
                    //    clientContext.ExecuteQuery();

                    //    ListItem lItem2 = newFile2.ListItemAllFields;
                    //    lItem2.File.CheckOut();
                    //    clientContext.ExecuteQuery();
                    //    lItem2["Title"] = "Payor Enrollment";
                    //    lItem2["ProgramNameText"] = rootWebUrl + "/bi/fhppp/iwn/EnrollmentReferrals/SitePages/ReferralSearch.aspx?qsptine=" + practiceSite.EncryptedPracticeTIN;
                    //    lItem2["Thumbnail"] = practiceSite.URL + "/Program%20Participation/" + fileName2;
                    //    lItem2.Update();
                    //    lItem2.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                    //    clientContext.ExecuteQuery();
                    //}

                    if (siteType != null && siteType.Contains("ckcc"))
                    {
                        Microsoft.SharePoint.Client.File newFile1 = myLibrary.RootFolder.Files.Add(fc1);
                        clientContext.Load(newFile1);
                        clientContext.ExecuteQuery();

                        ListItem lItem1 = newFile1.ListItemAllFields;
                        lItem1.File.CheckOut();
                        clientContext.ExecuteQuery();
                        lItem1["Title"] = "CKCC/KCE Coming Soon!";
                        //lItem1["Title"] = "CKCC/KCE";
                        lItem1["ProgramNameText"] = practiceSite.URL + "/Pages/ProgramParticipation.aspx";
                        lItem1["Thumbnail"] = practiceSite.URL + "/Program%20Participation/" + fileName1;
                        lItem1.Update();
                        lItem1.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();
                    }

                    if (siteType != null && siteType.Contains("iwh"))
                    {
                        Microsoft.SharePoint.Client.File newFile0 = myLibrary.RootFolder.Files.Add(fc0);
                        clientContext.Load(newFile0);
                        clientContext.ExecuteQuery();

                        ListItem lItem0 = newFile0.ListItemAllFields;
                        lItem0.File.CheckOut();
                        clientContext.ExecuteQuery();
                        lItem0["Title"] = "Payor Program Education Resources Coming Soon!";
                        //lItem0["Title"] = "Payor Program Education Resources";
                        lItem0["ProgramNameText"] = practiceSite.URL + "/Pages/ProgramParticipation.aspx";
                        lItem0["Thumbnail"] = practiceSite.URL + "/Program%20Participation/" + fileName0;
                        lItem0.Update();
                        lItem0.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("uploadProgramPracticeSupportFiles", ex.Message, "Error", practiceSite.URL);
                }
            }
        }
        public void uploadProgramPracticeSupportFiles(PracticeSite practiceSite)
        {
            string siteType = practiceSite.siteType;
            
            if (siteType == "")
            {
                return;
            }
            string LayoutsFolder = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
            using (ClientContext clientContext = new ClientContext(practiceSite.URL))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    string rootWebUrl = GetRootSite(practiceSite.URL);

                    string LibraryName = "Program Participation";
                    string fileName0 = "EducationReviewPro.JPG";
                    string fileName1 = "KCEckcc.JPG";
                    string fileName2 = "PracticeReferrals.JPG";
                    string fileName3 = "optimalstarts.jpg";

                    byte[] f0 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName0);
                    byte[] f1 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName1);
                    byte[] f2 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName2);
                    byte[] f3 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName3);

                    FileCreationInformation fc0 = new FileCreationInformation();
                    fc0.Url = fileName0;
                    fc0.Overwrite = true;
                    fc0.Content = f0;

                    FileCreationInformation fc1 = new FileCreationInformation();
                    fc1.Url = fileName1;
                    fc1.Overwrite = true;
                    fc1.Content = f1;

                    FileCreationInformation fc2 = new FileCreationInformation();
                    fc2.Url = fileName2;
                    fc2.Overwrite = true;
                    fc2.Content = f2;

                    FileCreationInformation fc3 = new FileCreationInformation();
                    fc3.Url = fileName3;
                    fc3.Overwrite = true;
                    fc3.Content = f3;
                    List myLibrary = web.Lists.GetByTitle(LibraryName);


                    if (siteType != null && siteType.Contains("kc365"))
                    {
                        Microsoft.SharePoint.Client.File newFile2 = myLibrary.RootFolder.Files.Add(fc2);
                        clientContext.Load(newFile2);
                        clientContext.ExecuteQuery();

                        ListItem lItem2 = newFile2.ListItemAllFields;
                        lItem2.File.CheckOut();
                        clientContext.ExecuteQuery();
                        lItem2["Title"] = "Payor Enrollment";
                        lItem2["ProgramNameText"] = rootWebUrl + "/bi/fhppp/iwn/EnrollmentReferrals/SitePages/ReferralSearch.aspx?qsptine=" + practiceSite.EncryptedPracticeTIN;
                        lItem2["Thumbnail"] = practiceSite.URL + "/Program%20Participation/" + fileName2;
                        lItem2.Update();
                        lItem2.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();
                    }

                    if (siteType != null && siteType.Contains("ckcc"))
                    {
                        Microsoft.SharePoint.Client.File newFile1 = myLibrary.RootFolder.Files.Add(fc1);
                        clientContext.Load(newFile1);
                        clientContext.ExecuteQuery();

                        ListItem lItem1 = newFile1.ListItemAllFields;
                        lItem1.File.CheckOut();
                        clientContext.ExecuteQuery();
                        //lItem1["Title"] = "CKCC/KCE Coming Soon!";
                        lItem1["Title"] = "CKCC/KCE";
                        lItem1["ProgramNameText"] = practiceSite.URL + "/Pages/ProgramParticipation.aspx";
                        lItem1["Thumbnail"] = practiceSite.URL + "/Program%20Participation/" + fileName1;
                        lItem1.Update();
                        lItem1.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();


                        Microsoft.SharePoint.Client.File newFile3 = myLibrary.RootFolder.Files.Add(fc3);
                        clientContext.Load(newFile3);
                        clientContext.ExecuteQuery();

                        ListItem lItem3 = newFile3.ListItemAllFields;
                        lItem3.File.CheckOut();
                        clientContext.ExecuteQuery();
                        //lItem3["Title"] = "Optimal Starts Coming Soon!";
                        lItem3["Title"] = "Dialysis Starts";
                        lItem3["ProgramNameText"] = practiceSite.URL + "/Pages/OptimalStart.aspx";
                        lItem3["Thumbnail"] = practiceSite.URL + "/Program%20Participation/" + fileName3;
                        lItem3.Update();
                        lItem3.File.CheckIn("Checkin - Create OptimalStart item", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();
                    }

                    if (siteType != null && siteType.Contains("iwh"))
                    {
                        Microsoft.SharePoint.Client.File newFile0 = myLibrary.RootFolder.Files.Add(fc0);
                        clientContext.Load(newFile0);
                        clientContext.ExecuteQuery();

                        ListItem lItem0 = newFile0.ListItemAllFields;
                        lItem0.File.CheckOut();
                        clientContext.ExecuteQuery();
                        //lItem0["Title"] = "Payor Program Education Resources Coming Soon!";
                        lItem0["Title"] = "Payor Program Education Resources";
                        lItem0["ProgramNameText"] = practiceSite.URL + "/Pages/ProgramParticipation.aspx";
                        lItem0["Thumbnail"] = practiceSite.URL + "/Program%20Participation/" + fileName0;
                        lItem0.Update();
                        lItem0.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("uploadProgramPracticeSupportFiles", ex.Message, "Error", practiceSite.URL);
                }
            }
        }

        public string GetRootSite(string url)
        {
            Uri uri = new Uri(url.TrimEnd(new[] { '/' }));
            return $"{uri.Scheme}://{ uri.DnsSafeHost}";
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

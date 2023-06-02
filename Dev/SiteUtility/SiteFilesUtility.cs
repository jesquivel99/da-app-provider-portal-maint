using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Serilog;
using System.Configuration;

namespace SiteUtility
{
    public class SiteFilesUtility
    {
        static ILogger _logger = Log.ForContext<SiteFilesUtility>();
        static readonly string LayoutsFolderDeploy = ConfigurationManager.AppSettings["LayoutsFolderDeploy"];
        static readonly string LayoutsFolderDeployIwn = ConfigurationManager.AppSettings["LayoutsFolderIwn"];
        static readonly string LayoutsFolderDeployImg = ConfigurationManager.AppSettings["LayoutsFolderImg"];
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
                    SiteLogUtility.CreateLogEntry("DocumentUpload", ex.Message, "Error", "");
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
            SiteLogUtility.Log_Entry("   UploadHtmlSupportingFilesSingleFile - " + htmlFileName);
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    string LibraryName = "Site Assets";
                    List<FileCreationInformation> htmlFileCreateInfo = new List<FileCreationInformation>();


                    // Get cePrac html files...
                    string[] cePracFiles = Directory.GetFiles(LayoutsFolderDeploy, "cePrac*.html");

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
        public void uploadProgramPracticeSupportFiles(Practice practiceSite, string LayoutsFolder)
        {
            SiteListUtility siteListUtility = new SiteListUtility();
            //string LayoutsFolder = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
            using (ClientContext clientContext = new ClientContext(practiceSite.NewSiteUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    string rootWebUrl = GetRootSite(practiceSite.NewSiteUrl);

                    string LibraryName = "Program Participation";

                    string fileName0 = SitePublishUtility.imgPayorProgramEdResources; //"EducationReviewPro.JPG";
                    string fileName1 = SitePublishUtility.imgCkccKceResources; //"KCEckcc.JPG";
                    string fileName2 = SitePublishUtility.imgPayorEnrollment; //"PracticeReferrals.JPG";
                    string fileName3 = SitePublishUtility.imgPatientStatusUpdates; //"optimalstarts.jpg";
                    string fileName4 = SitePublishUtility.imgCkccEngagement; //"optimalstarts.jpg";

                    byte[] f0 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName0);
                    byte[] f1 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName1);
                    byte[] f2 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName2);
                    byte[] f3 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName3);
                    byte[] f4 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName4);

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

                    FileCreationInformation fc4 = new FileCreationInformation();
                    fc4.Url = fileName4;
                    fc4.Overwrite = true;
                    fc4.Content = f4;

                    List myLibrary = web.Lists.GetByTitle(LibraryName);

                    if (practiceSite.IsKC365)
                    {
                        Microsoft.SharePoint.Client.File newFile2 = myLibrary.RootFolder.Files.Add(fc2);
                        clientContext.Load(newFile2);
                        clientContext.ExecuteQuery();

                        ListItem lItem2 = newFile2.ListItemAllFields;
                        lItem2.File.CheckOut();
                        clientContext.ExecuteQuery();
                        lItem2["Title"] = "Payor Enrollment";
                        lItem2["ProgramNameText"] = practiceSite.NewSiteUrl + "/Pages/" + SitePublishUtility.pagePayorEnrollment + ".aspx";
                        lItem2["Thumbnail"] = practiceSite.NewSiteUrl + "/Program%20Participation/" + fileName2;
                        lItem2.Update();
                        lItem2.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();
                    }

                    if (practiceSite.IsCKCC)
                    {
                        Microsoft.SharePoint.Client.File newFile1 = myLibrary.RootFolder.Files.Add(fc1);
                        clientContext.Load(newFile1);
                        clientContext.ExecuteQuery();

                        ListItem lItem1 = newFile1.ListItemAllFields;
                        lItem1.File.CheckOut();
                        clientContext.ExecuteQuery();
                        lItem1["Title"] = "CKCC/KCE";
                        lItem1["ProgramNameText"] = practiceSite.NewSiteUrl + "/Pages/" + SitePublishUtility.pageCkccKceResources + ".aspx";
                        lItem1["Thumbnail"] = practiceSite.NewSiteUrl + "/Program%20Participation/" + fileName1;
                        lItem1.Update();
                        lItem1.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();


                        Microsoft.SharePoint.Client.File newFile3 = myLibrary.RootFolder.Files.Add(fc3);
                        clientContext.Load(newFile3);
                        clientContext.ExecuteQuery();

                        ListItem lItem3 = newFile3.ListItemAllFields;
                        lItem3.File.CheckOut();
                        clientContext.ExecuteQuery();
                        lItem3["Title"] = "Dialysis Starts";
                        lItem3["ProgramNameText"] = practiceSite.NewSiteUrl + "/Pages/" + SitePublishUtility.pagePatientStatusUpdates + ".aspx";
                        lItem3["Thumbnail"] = practiceSite.NewSiteUrl + "/Program%20Participation/" + fileName3;
                        lItem3.Update();
                        lItem3.File.CheckIn("Checkin - Create OptimalStart item", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();
                    }

                    if (practiceSite.IsIWH)
                    {
                        Microsoft.SharePoint.Client.File newFile0 = myLibrary.RootFolder.Files.Add(fc0);
                        clientContext.Load(newFile0);
                        clientContext.ExecuteQuery();

                        ListItem lItem0 = newFile0.ListItemAllFields;
                        lItem0.File.CheckOut();
                        clientContext.ExecuteQuery();
                        lItem0["Title"] = "Payor Program Education Resources";
                        lItem0["ProgramNameText"] = practiceSite.NewSiteUrl + "/Pages/" + SitePublishUtility.pagePayorProgramEdResources + ".aspx";
                        lItem0["Thumbnail"] = practiceSite.NewSiteUrl + "/Program%20Participation/" + fileName0;
                        lItem0.Update();
                        lItem0.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();
                    }

                    if (practiceSite.IsTelephonic)
                    {
                        Microsoft.SharePoint.Client.File newFile4 = myLibrary.RootFolder.Files.Add(fc4);
                        clientContext.Load(newFile4);
                        clientContext.ExecuteQuery();

                        ListItem lItem0 = newFile4.ListItemAllFields;
                        lItem0.File.CheckOut();
                        clientContext.ExecuteQuery();
                        lItem0["Title"] = SitePublishUtility.titleCkccEngagement;
                        lItem0["ProgramNameText"] = practiceSite.NewSiteUrl + "/Pages/ProgramParticipation.aspx";
                        lItem0["Thumbnail"] = practiceSite.NewSiteUrl + "/Program%20Participation/" + fileName0;
                        lItem0.Update();
                        lItem0.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("uploadProgramPracticeSupportFiles", ex.Message, "Error", "");
                }
            }
        }
        private void UploadFilesToSubFolder(string wUrl)
        {
            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    string LibraryName = "Site Assets";
                    List<FileCreationInformation> imgFileCreateInfo = new List<FileCreationInformation>();

                    // Get jpg files...
                    string[] files0 = Directory.GetFiles(Path.Combine(LayoutsFolderDeployImg, "Img"), "*.*");

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
                    SiteLogUtility.CreateLogEntry("uploadImageSupportingFiles", ex.Message, "Error", "");
                }
            }
        }
        public static void updateProgramParticipation(string siteUrl, string progPartTitle, string progPartPage, string LayoutsFolder, string progPartImgFile)
        {
            //string LayoutsFolder = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                Web web = clientContext.Web;
                clientContext.Load(web);
                clientContext.ExecuteQuery();

                try
                {
                    List list = web.Lists.GetByTitle("Program Participation");
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();

                    CamlQuery query = new CamlQuery();
                    query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + 
                        progPartTitle + 
                        "</Value></Eq></Where></Query></View>";

                    ListItemCollection items = list.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    if (items.Count > 0)
                    {
                        ListItem progPartItem = items.FirstOrDefault();
                        progPartItem["ProgramNameText"] = siteUrl + "Pages/" + progPartPage + ".aspx";
                        progPartItem.Update();
                        clientContext.ExecuteQuery();
                    }
                    else
                    {
                        string fileLocation = LayoutsFolder;
                        string fileName = progPartImgFile;

                        byte[] f = System.IO.File.ReadAllBytes(fileLocation + fileName);

                        FileCreationInformation fc = new FileCreationInformation();
                        fc.Url = fileName;
                        fc.Overwrite = true;
                        fc.Content = f;

                        Microsoft.SharePoint.Client.File newFile = list.RootFolder.Files.Add(fc);
                        clientContext.Load(newFile);
                        clientContext.ExecuteQuery();

                        ListItem newItem = newFile.ListItemAllFields;
                        newItem.File.CheckOut();
                        clientContext.ExecuteQuery();
                        newItem["Title"] = progPartTitle;

                        newItem["ProgramNameText"] = siteUrl + "Pages/" + progPartPage + ".aspx";
                        newItem["Thumbnail"] = siteUrl + "Program%20Participation/" + fileName;
                        newItem.Update();
                        newItem.File.CheckIn("Checkin - Create Program Participation Item " + progPartTitle, CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();

                        //modifyWebPartProgramParticipation(siteUrl);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message + " " + ex.StackTrace);
                }
            }
        }
        public static bool SP_Update_ProgramParticipation(string wUrl, string pageName, string searchTitle, string newTitle, string newThumbnail)
        {
            SiteLogUtility.Log_Entry("   SP_Update_ProgramParticipation - In Progress...");
            string pageNameAspx = pageName + ".aspx";

            using (ClientContext clientContext = new ClientContext(wUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                try
                {
                    SiteFilesUtility siteFilesUtility = new SiteFilesUtility();
                    string rootWebUrl = siteFilesUtility.GetRootSite(wUrl);
                    string fileName1 = newThumbnail;

                    Web web = clientContext.Web;
                    List list = web.Lists.GetByTitle("Program Participation");
                    clientContext.Load(web);
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();
                    View view = list.Views.GetByTitle("All Documents");

                    clientContext.Load(view);
                    clientContext.ExecuteQuery();
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = view.ViewQuery;

                    ListItemCollection items = list.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    foreach (var item in items)
                    {

                        if (item["Title"].ToString().Contains(searchTitle))
                        {
                            SiteLogUtility.Log_Entry("BEFORE - ProgramNameText", true);
                            SiteLogUtility.Log_Entry(item["ProgramNameText"].ToString(), true);

                            item.File.CheckOut();
                            clientContext.ExecuteQuery();
                            item["Title"] = newTitle;
                            item["ProgramNameText"] = web.Url + "/Pages/" + pageNameAspx;
                            item["Thumbnail"] = wUrl + "/Program%20Participation/" + fileName1;
                            item.Update();
                            item.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                            clientContext.ExecuteQuery();

                            SiteLogUtility.Log_Entry("AFTER - ProgramNameText", true);
                            SiteLogUtility.Log_Entry(item["ProgramNameText"].ToString(), true);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("SP_Update_ProgramParticipation", ex.Message, "Error", "");
                    return false;
                }
            }

            return true;
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

        private static void UseRecursiveMethodToGetAllItems(string pracUrl, string libName, string folderName)
        {
            using (ClientContext context = new ClientContext(pracUrl))
            {
                context.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                FolderCollection rootFolders = context.Web.GetFolderByServerRelativeUrl(libName).Folders;
                context.Load(rootFolders, folders => folders.Include(f => f.ListItemAllFields));
                context.ExecuteQuery();
                foreach (var folder in rootFolders)
                {
                    GetFilesAndFolders(context, folder, folderName);
                }

                Console.ReadLine();
            }
        }

        private static void GetFilesAndFolders(ClientContext context, Folder folder, string folderName)
        {
            if (folder != null && folder.ListItemAllFields.FieldValues.Count > 0)
            {
                if (folder.ListItemAllFields.FieldValues["FileLeafRef"].ToString() == folderName)
                {
                    Console.WriteLine($"Folder - {folder.ListItemAllFields.FieldValues["FileLeafRef"]} - FOUND THE FOLDER!");
                }
                else
                {
                    Console.WriteLine($"Folder - {folder.ListItemAllFields.FieldValues["FileLeafRef"]}");
                }

                var fileCollection = folder.Files;
                context.Load(fileCollection, files => files.Include(f => f.ListItemAllFields));
                context.ExecuteQuery();

                foreach (var file in fileCollection)
                {
                    Console.WriteLine($" -> {file.ListItemAllFields.FieldValues["FileLeafRef"]}");
                }

                var subFolderCollection = folder.Folders;
                context.Load(subFolderCollection, folders => folders.Include(f => f.ListItemAllFields));
                context.ExecuteQuery();
                foreach (var subFolder in subFolderCollection)
                {
                    GetFilesAndFolders(context, subFolder, folderName);
                }
            }
        }

        public void GetAllCheckedOutFilesInLibrary(string pracUrl, string listName)
        {
            try
            {
                using (var ctx = new ClientContext(pracUrl))
                {
                    ctx.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    ctx.Load(ctx.Web, a => a.Lists);
                    ctx.ExecuteQuery();

                    List list = ctx.Web.Lists.GetByTitle(listName);
                    ListItemCollection items = list.GetItems(
                        new CamlQuery()
                        {
                            ViewXml = @"<View Scope='RecursiveAll'><Query><Where><IsNotNull><FieldRef Name='File_x0020_Type' /></IsNotNull></Where></Query></View>"
                        });
                    ctx.Load(items, a => a.IncludeWithDefaultProperties(item => item.File, item => item.File.CheckedOutByUser, item => item.File.Author));
                    ctx.ExecuteQuery();
                    foreach (var item in items)
                    {
                        //if (item.File.CheckOutType != CheckOutType.None)
                        {
                            SiteLogUtility.Log_Entry("File: " + item["FileRef"].ToString().Split('/').LastOrDefault(), true);
                            SiteLogUtility.Log_Entry("                Author: " + item.File.Author.Title, true);
                            //SiteLogUtility.Log_Entry("        Checked-Out By: " + item.File.CheckedOutByUser.Title, true);
                            //SiteLogUtility.Log_Entry("Checked-Out User Email: " + item.File.CheckedOutByUser.Email, true);
                            SiteLogUtility.Log_Entry("Last Modified: " + DateTime.Parse(item["Last_x0020_Modified"].ToString()), true);
                            SiteLogUtility.Log_Entry("-----------------------", true);
                            SiteLogUtility.Log_Entry("", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("GetAllCheckedOutFiles", ex.Message, "Error", "");
            }
        }


        public static bool FileExists(string siteUrl, string libName, string fileName)
        {

            using (ClientContext context = new ClientContext(siteUrl))
            {
                Web web = context.Web;
                context.Load(web);
                context.ExecuteQuery();
                string serverRelativeUrl = web.ServerRelativeUrl;
                var file = context.Web.GetFileByServerRelativeUrl(serverRelativeUrl + "/" + libName + "/" + fileName);
                context.Load(file, f => f.Exists);
                try
                {
                    context.ExecuteQuery();

                    if (file.Exists)
                    {
                        return true;
                    }
                    return false;
                }
                catch (ServerUnauthorizedAccessException uae)
                {
                    _logger.Error("You are not allowed to access this file " + uae.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Information("Could not find file {name}", fileName);
                    return false;
                }
            }
               
        }

        public static object lockObjDataExchange = new object();
        public static object lockObjRiskAdjustment = new object();
        public static object lockObjBenefitEnhancement = new object();
        public static object lockObjQuality = new object();
        public static object lockObjPayorEducation = new object();
        public static void uploadMultiPartSupportingFilesAll(string wUrl, Practice practiceSite)
        {
            SiteLogUtility siteLogUtility = new SiteLogUtility();

            string LayoutsFolder = LayoutsFolderDeploy;
            try
            {
                SiteListUtility slu = new SiteListUtility();
                string strJSContentDataExchange = "";
                string strJSContentRiskAdjustment = "";
                string strJSContentBenefitEnhancement = "";
                string strJSContentQuality = "";
                string strJSContentPayorEducation = "";

                string strJSFileServerPathDataExchange = LayoutsFolder + "PracticeSiteTemplate_MultiTab.js";
                string strJSFileServerPathRiskAdjustment = LayoutsFolder + "RiskAdjustment.js";
                string strJSFileServerPathBenefitEnhancement = LayoutsFolder + "BenefitEnhancement_MultiTab.js";
                string strJSFileServerPathQuality = LayoutsFolder + "Quality_MultiTab.js";
                string strJSFileServerPathPayorEducation = LayoutsFolder + "PayorEducation_MultiTab.js";

                if (practiceSite.IsIWH)
                {
                    strJSContentDataExchange = @"var thisTab2 = {title: '" + slu.tabTitleDataExchangeIwh + "',webParts: ['" + slu.webpartDataExchangeIwh + "']};tabConfiguration.push(thisTab2);";
                    strJSContentRiskAdjustment = @"var thisTab2 = {title: '" + slu.tabTitleRiskAdjustmentIwh + "',webParts: ['" + slu.webpartRiskAdjustmentIwh + "']};tabConfiguration.push(thisTab2);";
                    strJSContentQuality = @"var thisTab2 = {title: '" + slu.tabTitleQualityIwh + "',webParts: ['" + slu.webpartQualityIwh + "']};tabConfiguration.push(thisTab2);";
                    strJSContentPayorEducation = @"var thisTab2 = {title: '" + slu.tabTitlePayorEducationIwh + "',webParts: ['" + slu.webpartPayorEducationIwh + "']};tabConfiguration.push(thisTab2);";
                }
                if (practiceSite.IsCKCC)
                {
                    strJSContentDataExchange = strJSContentDataExchange + @"var thisTab3 = {title: '" + slu.tabTitleDataExchangeCkcc + "',webParts: ['" + slu.webpartDataExchangeCkcc + "']};tabConfiguration.push(thisTab3);";
                    strJSContentRiskAdjustment = strJSContentRiskAdjustment + @"var thisTab3 = {title: '" + slu.tabTitleRiskAdjustmentCkcc + "',webParts: ['" + slu.webpartRiskAdjustmentCkcc + "']};tabConfiguration.push(thisTab3);";
                    strJSContentBenefitEnhancement = @"var thisTab2 = {title: '" + slu.tabTitleBenefitEnhancementCkcc + "',webParts: ['" + slu.webpartBenefitEnhancementCkcc + "']};tabConfiguration.push(thisTab2);";
                    strJSContentQuality = strJSContentQuality + @"var thisTab3 = {title: '" + slu.tabTitleQualityCkcc + "',webParts: ['" + slu.webpartQualityCkcc + "']};tabConfiguration.push(thisTab3);";
                }

                strJSContentDataExchange = strJSContentDataExchange + "//*#funXXXX#*";
                strJSContentRiskAdjustment = strJSContentRiskAdjustment + "//*#funXXXX#*";
                strJSContentBenefitEnhancement = strJSContentBenefitEnhancement + "//*#funXXXX#*";
                strJSContentQuality = strJSContentQuality + "//*#funXXXX#*";
                strJSContentPayorEducation = strJSContentPayorEducation + "//*#funXXXX#*";

                lock (lockObjDataExchange)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathDataExchange).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentDataExchange;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathDataExchange, lines);
                    siteLogUtility.LoggerInfo_Entry("Uploaded MultiTab Support File: " + strJSFileServerPathDataExchange);
                }

                lock (lockObjRiskAdjustment)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathRiskAdjustment).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentRiskAdjustment;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathRiskAdjustment, lines);
                    siteLogUtility.LoggerInfo_Entry("Uploaded MultiTab Support File: " + strJSFileServerPathRiskAdjustment);
                }

                lock (lockObjBenefitEnhancement)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathBenefitEnhancement).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentBenefitEnhancement;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathBenefitEnhancement, lines);
                    siteLogUtility.LoggerInfo_Entry("Uploaded MultiTab Support File: " + strJSFileServerPathBenefitEnhancement);
                }

                lock (lockObjQuality)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathQuality).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentQuality;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathQuality, lines);
                    siteLogUtility.LoggerInfo_Entry("Uploaded MultiTab Support File: " + strJSFileServerPathQuality);
                }

                lock (lockObjPayorEducation)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathPayorEducation).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentPayorEducation;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathPayorEducation, lines);
                    siteLogUtility.LoggerInfo_Entry("Uploaded MultiTab Support File: " + strJSFileServerPathPayorEducation);
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("uploadMultiPartSupportingFilesAll", ex.Message, "Error", "");
            }
        }
        public static void UploadMultiPartSupportingFilesIwh(string wUrl, PracticeSite practiceSite)
        {
            string LayoutsFolder = @LayoutsFolderDeploy;
            try
            {
                SiteListUtility slu = new SiteListUtility();
                string strJSContentDataExchange = "";
                string strJSContentRiskAdjustment = "";
                string strJSContentQuality = "";
                string strJSContentPayorEducation = "";

                string strJSFileServerPathDataExchange = LayoutsFolder + "PracticeSiteTemplate_MultiTab.js";
                string strJSFileServerPathRiskAdjustment = LayoutsFolder + "RiskAdjustment.js";
                string strJSFileServerPathQuality = LayoutsFolder + "Quality_MultiTab.js";
                string strJSFileServerPathPayorEducation = LayoutsFolder + "PayorEducation_MultiTab.js";

                if (practiceSite.IsIWH.Equals("true"))
                {
                    strJSContentDataExchange = @"var thisTab2 = {title: '" + slu.tabTitleDataExchangeIwh + "',webParts: ['" + slu.webpartDataExchangeIwh + "']};tabConfiguration.push(thisTab2);";
                    strJSContentRiskAdjustment = @"var thisTab2 = {title: '" + slu.tabTitleRiskAdjustmentIwh + "',webParts: ['" + slu.webpartRiskAdjustmentIwh + "']};tabConfiguration.push(thisTab2);";
                    strJSContentQuality = @"var thisTab2 = {title: '" + slu.tabTitleQualityIwh + "',webParts: ['" + slu.webpartQualityIwh + "']};tabConfiguration.push(thisTab2);";
                    strJSContentPayorEducation = @"var thisTab2 = {title: '" + slu.tabTitlePayorEducationIwh + "',webParts: ['" + slu.webpartPayorEducationIwh + "']};tabConfiguration.push(thisTab2);";
                }

                strJSContentDataExchange = strJSContentDataExchange + "//*#funXXXX#*";
                strJSContentRiskAdjustment = strJSContentRiskAdjustment + "//*#funXXXX#*";
                strJSContentQuality = strJSContentQuality + "//*#funXXXX#*";
                strJSContentPayorEducation = strJSContentPayorEducation + "//*#funXXXX#*";

                lock (lockObjDataExchange)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathDataExchange).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentDataExchange;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathDataExchange, lines);
                }

                lock (lockObjRiskAdjustment)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathRiskAdjustment).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentRiskAdjustment;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathRiskAdjustment, lines);
                }

                lock (lockObjQuality)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathQuality).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentQuality;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathQuality, lines);
                }

                lock (lockObjPayorEducation)
                {
                    List<string> lines = System.IO.File.ReadAllLines(strJSFileServerPathPayorEducation).ToList<string>();
                    for (var i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("//*#funXXXX#*"))
                        {
                            lines[i] = strJSContentPayorEducation;
                        }
                    }
                    System.IO.File.WriteAllLines(strJSFileServerPathPayorEducation, lines);
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("uploadMultiPartSupportingFilesAll", ex.Message, "Error", "");
            }
        }
        public static void uploadProgramPracticeSupportFilesIwnPayorEd(Practice practiceSite)
        {
            SiteFilesUtility siteFilesUtility = new SiteFilesUtility();
            string LayoutsFolder = @"C:\Projects\PracticeSite-Core\Dev\PracticeSiteTemplate\Config\";
            using (ClientContext clientContext = new ClientContext(practiceSite.NewSiteUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    var web = clientContext.Web;
                    string rootWebUrl = siteFilesUtility.GetRootSite(practiceSite.NewSiteUrl);

                    string LibraryName = "Program Participation";

                    string fileName0 = "EducationReviewPro.JPG";

                    byte[] f0 = System.IO.File.ReadAllBytes(LayoutsFolder + fileName0);

                    FileCreationInformation fc0 = new FileCreationInformation();
                    fc0.Url = fileName0;
                    fc0.Overwrite = true;
                    fc0.Content = f0;

                    List myLibrary = web.Lists.GetByTitle(LibraryName);

                    //if (siteType != null && siteType.Contains("iwh"))
                    if (practiceSite.IsIWH == true)
                    {
                        Microsoft.SharePoint.Client.File newFile0 = myLibrary.RootFolder.Files.Add(fc0);
                        clientContext.Load(newFile0);
                        clientContext.ExecuteQuery();

                        ListItem lItem0 = newFile0.ListItemAllFields;
                        lItem0.File.CheckOut();
                        clientContext.ExecuteQuery();
                        //lItem0["Title"] = "Payor Program Education Resources Coming Soon!";
                        lItem0["Title"] = "Payor Program Education Resources";
                        lItem0["ProgramNameText"] = practiceSite.NewSiteUrl + "/Pages/PayorEdResources.aspx";
                        lItem0["Thumbnail"] = practiceSite.NewSiteUrl + "/Program%20Participation/" + fileName0;
                        lItem0.Update();
                        lItem0.File.CheckIn("Z", CheckinType.OverwriteCheckIn);
                        clientContext.ExecuteQuery();
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("uploadProgramPracticeSupportFilesIwnPayorEd", ex.Message, "Error", "");
                }
            }
        }
        public static void UploadHtmlCarePlanPage(string wUrl, bool isCkcc)
        {
            SiteFilesUtility siteFilesUtility = new SiteFilesUtility();
            string layoutsFolder = ConfigurationManager.AppSettings["LayoutsFolderDeploy"];
            string layoutsFolderIwn = ConfigurationManager.AppSettings["LayoutsFolderIwn"];
            try
            {
                if (isCkcc)
                {
                    siteFilesUtility.DocumentUpload(wUrl, layoutsFolder + "cePrac_CarePlans.html", "SiteAssets");
                    siteFilesUtility.DocumentUpload(wUrl, layoutsFolder + "cePrac_HospitalAlerts.html", "SiteAssets");
                }
                else
                {
                    siteFilesUtility.DocumentUpload(wUrl, layoutsFolderIwn + "cePrac_CarePlans.html", "SiteAssets");
                    siteFilesUtility.DocumentUpload(wUrl, layoutsFolderIwn + "cePrac_HospitalAlerts.html", "SiteAssets");
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("UploadHtmlCarePlanPage", ex.Message, "Error", "");
            }
        }

    }
}

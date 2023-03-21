using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;

namespace SiteUtility
{
    
    public class PMData
    {
        public int GroupID { get; set; }
        public string ProgramManager { get; set; }
        public string SiteId { get; set; }
        public string PracticeName { get; set; }
        public string PracticeTIN { get; set; }
        public string PracticeNPI { get; set; }
        public string CKCCArea { get; set; }
        public int IWNRegion { get; set; }
        public int KC365 { get; set; }
        public string EncryptedPracticeTIN { get; set; }
        public string ProgramParticipation { get; set; }
        public string IsIWH { get; set; }
        public string IsCKCC { get; set; }
        public string IsKC365 { get; set; }
        public string IsTeleKC365 { get; set; }
        public string siteType { get; set; }
        public PMData()
        {

        }

        public bool PrintProgramParticipationGroupTotal(List<PMData> pMData)
        {
            try
            {
                var groupPerProgram = pMData
                                .GroupBy(u => u.ProgramParticipation)
                                .Select(grp => new
                                {
                                    Program = grp.Key,
                                    Count = grp.Count(),
                                    pmData = grp.ToList()
                                })
                                .OrderBy(pp => pp.Program)
                                .ToList();

                foreach (var item in groupPerProgram)
                {
                    SiteLogUtility.Log_Entry(item.Program + " = " + item.pmData.Count().ToString(), true);
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("PrintParticipationGroupTotal", ex.Message, "Error", "");
                return false;
            }

            return true;
        }
        public bool PrintProgramParticipationGroupTotal2(List<PMData> pMData)
        {
            try
            {
                var groupPerProgram = pMData
                                .Where(u => u.ProgramParticipation == "KCE Participation")
                                .Select(grp => new
                                {
                                    Program = grp.ProgramParticipation,
                                    pmData = pMData.Count()
                                });

            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("PrintParticipationGroupTotal", ex.Message, "Error", "");
                return false;
            }

            return true;
        }
        public bool PrintProgramParticipationGroupSubTotal(List<PMData> pMData, string pGroup)
        {
            try
            {
                int grpTotal = 0;
                var groupPerProgram1 = pMData
                        .GroupBy(g => g.ProgramParticipation)
                        .Where(fl => fl.Key.Contains(pGroup))
                        .Select(grp => new {
                            Program = grp.Key,
                            Count = grp.Count(), 
                            pmData = grp.ToList()
                        })
                        .OrderBy(pp => pp.Program)
                        .ToList();

                SiteLogUtility.Log_Entry("Group Contains [ " + pGroup + " ]");
                foreach (var item in groupPerProgram1)
                {
                    SiteLogUtility.Log_Entry(item.Program + " = " + item.pmData.Count().ToString(), true);
                    grpTotal = grpTotal + item.Count;
                }
                SiteLogUtility.Log_Entry("GROUP TOTAL: " + grpTotal.ToString(), true);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("PrintParticipationGroupTotal", ex.Message, "Error", "");
                return false;
            }

            return true;
        }
        public int CntProgramParticipationGroupSubTotal(List<PMData> pMData, string pGroup)
        {
            int grpTotal = 0;
            try
            {
                var groupPerProgram1 = pMData
                        .GroupBy(g => g.ProgramParticipation)
                        .Where(fl => fl.Key.Contains(pGroup))
                        .Select(grp => new {
                            Program = grp.Key,
                            Count = grp.Count(),
                            pmData = grp.ToList()
                        })
                        .OrderBy(pp => pp.Program)
                        .ToList();

                foreach (var item in groupPerProgram1)
                {
                    grpTotal = grpTotal + item.Count;
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("CntParticipationGroupTotal", ex.Message, "Error", "");
                return grpTotal;
            }

            return grpTotal;
        }
    }

    public class SitePMData
    {
        public string programParticipationIWH = "InterWell Health";
        public string programParticipationCKCC = "KCE Participation";
        public string programParticipationKC365 = "KC365";
        public string programParticipationTelephonicKC365 = "Telephonic KC365";
        public static void initialConnect()
        {
            SitePMData objSitePMData = new SitePMData();
            objSitePMData.readPMSiteData();
        }
        
        /// <summary>
        /// Method will call readDBPortalPMData with an optional parameter.
        /// Optional parameter is used to filter for a single Program Manager.
        /// Format is "01", "02"...
        /// </summary>
        /// <param name="PMRef"></param>
        public static void initialConnectDBPortal(string PMRef = "")
        {
            SitePMData objSitePMData = new SitePMData();
            objSitePMData.readDBPortalPMData(PMRef);
        }
        public static void InitialConnectDBPortalDeployed(string PMRef = "")
        {
            try
            {
                SitePMData objSitePMData = new SitePMData();
                DataTable dtTable = objSitePMData.readDBPortalDeployed(PMRef);
                objSitePMData.filterPMSiteData(dtTable);
                //createJSONConfig(dtTable);
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("InitialConnectDBPortalDeployed", ex.Message, "Error", "");
            }
        }
        public void readPMSiteData()
        {
            try
            {
                string ds = ConfigurationManager.AppSettings["SqlServer"].ToString();
                string ic = ConfigurationManager.AppSettings["Database"].ToString();
                string connString = "Data Source=" + ConfigurationManager.AppSettings["SqlServer"]
                        + "; Initial Catalog=" + ConfigurationManager.AppSettings["Database"] + "; Integrated Security=SSPI";

                string query = @"SELECT * FROM [HealthCloud_NightlyProd].[PORTAL].[vwPracticeInfo] ORDER BY GroupID";

                DataTable dtTable = new DataTable();
                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtTable);
                conn.Close();
                da.Dispose();
                filterPMSiteData(dtTable);
                //createJSONConfig(dtTable);
            }
            catch (Exception ex)
            {
            }
        }

        public void readDBPortalPMData(string PMRef = "")
        {
            try
            {
                string filterPM = "";
                string ds = ConfigurationManager.AppSettings["SqlServer"].ToString();
                string ic = ConfigurationManager.AppSettings["Database"].ToString();
                string connString = "Data Source=" + ConfigurationManager.AppSettings["SqlServer"]
                        + "; Initial Catalog=" + ConfigurationManager.AppSettings["Database"] + "; Integrated Security=SSPI";
                string query = string.Empty;

                if(PMRef != "")
                {
                    query = @"SELECT * FROM [HealthCloud_NightlyProd].[PORTAL].[vwPracticeInfo] WHERE GroupID = " + PMRef + " ORDER BY GroupID";
                }
                else
                {
                    query = @"SELECT * FROM [HealthCloud_NightlyProd].[PORTAL].[vwPracticeInfo] ORDER BY GroupID";
                }

                DataTable dtTable = new DataTable();
                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtTable);
                conn.Close();
                da.Dispose();
                filterPMSiteData(dtTable);
                createJSONConfig(dtTable);
            }
            catch (Exception ex)
            {
            }
        }

        public DataTable readDBPortalDeployed(string PMRef = "")
        {
            try
            {
                string filterPM = "";
                string ds = ConfigurationManager.AppSettings["SqlServer"].ToString();
                string ic = ConfigurationManager.AppSettings["Database"].ToString();
                string connString = "Data Source=" + ConfigurationManager.AppSettings["SqlServer"]
                        + "; Initial Catalog=" + ConfigurationManager.AppSettings["Database"] + "; Integrated Security=SSPI";
                string query = string.Empty;

                if (PMRef.StartsWith("PM"))
                {
                    PMRef = PMRef.Substring(2);
                }

                if (PMRef != "")
                {
                    query = @"SELECT * FROM [HealthCloud_NightlyProd].[PORTAL].[PracticeInfo_Deployed] WHERE GroupID = " + PMRef + " ORDER BY GroupID";
                }
                else
                {
                    query = @"SELECT * FROM [HealthCloud_NightlyProd].[PORTAL].[PracticeInfo_Deployed] ORDER BY GroupID";
                }

                DataTable dtTable = new DataTable();
                using (SqlConnection sqlConn = new SqlConnection())
                {
                    sqlConn.ConnectionString = connString;

                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand(query, sqlConn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dtTable);
                    sqlConn.Close();
                    da.Dispose();
                    //filterPMSiteData(dtTable);
                    //createJSONConfig(dtTable);
                }
                return dtTable;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void filterPMSiteData(DataTable allData)
        {
            try
            {
                DataTable dtDataNew = allData.Clone();
                DataView view = new DataView(allData);
                DataTable distinctValues = view.ToTable(true, "GroupID");
                for (int intLoop = 0; intLoop < distinctValues.Rows.Count; intLoop++)
                {
                    if (intLoop <= 9)
                    {
                        dtDataNew = allData.AsEnumerable().Where(row => row.Field<Int32>("GroupID") == Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"])).CopyToDataTable();
                        updateXML(dtDataNew, ConfigurationManager.AppSettings["ConfigURL"] + "PracticeSiteTemplate_PM" + Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"]).ToString("00") + ".config", "PM" + Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"]).ToString("00"));
                    }
                    else
                    {
                        dtDataNew = allData.AsEnumerable().Where(row => row.Field<Int32>("GroupID") == Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"])).CopyToDataTable();
                        updateXML(dtDataNew, ConfigurationManager.AppSettings["ConfigURL"] + "PracticeSiteTemplate_PM" + distinctValues.Rows[intLoop]["GroupID"].ToString() + ".config", "PM" + distinctValues.Rows[intLoop]["GroupID"].ToString());
                    }
                    dtDataNew.Rows.Clear();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void updateXML(DataTable dt, string xmlfilePath, string strRegionID)
        {
            try
            {
                XDocument sourceFile = XDocument.Load(ConfigurationManager.AppSettings["ConfigURL"] + "PracticeSiteTemplate.config");
                XDocument xdoc = XDocument.Load(xmlfilePath);
                var sourceElementSbsite = sourceFile.Elements("Config").Elements("Sites").Elements("Site").Elements("SubSites").Elements("Site");
                var propertyValueSourceEle = sourceFile.Elements("Config").Elements("Sites").Elements("Site").Elements("SubSites").Elements("Site").Elements("SiteSettings").Elements("PropertyBag").Elements("Property");
                var sourceSite = sourceElementSbsite.FirstOrDefault();
                var propertySourceSite = propertyValueSourceEle.FirstOrDefault();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    sourceSite.SetAttributeValue("SiteName", dr["SiteID"]);

                    //sourceSite.SetAttributeValue("SiteTitle", dr["PracticeName"]);
                    sourceSite.SetAttributeValue("SiteTitle", changeSiteNameTitleCase(dr["PracticeName"].ToString()));
                    
                    sourceSite.SetAttributeValue("RegionID", strRegionID);
                    
                    //sourceSite.SetAttributeValue("SiteDescription", dr["PracticeName"] + " is a member of " + strRegionID);
                    sourceSite.SetAttributeValue("SiteDescription", changeSiteNameTitleCase(dr["PracticeName"].ToString()) + " is a member of " + strRegionID);
                    
                    sourceSite.SetAttributeValue("IsKC365", Convert.ToInt32(dr["KC365"]) == 0 ? "false" : "true");
                    sourceSite.SetAttributeValue("kceArea", dr["CKCCArea"]);
                    sourceSite.SetAttributeValue("IsCKCC", dr["CKCCArea"].ToString() == "" ? "false" : "true");
                    
                    //sourceSite.SetAttributeValue("IsIWH", dr["IWNRegion"].ToString() == "0" ? "false" : "true");
                    sourceSite.SetAttributeValue("IsIWH", Convert.ToBoolean(dr["IWNRegion"]) == false ? "false" : "true");
                    
                    sourceSite.SetAttributeValue("encryptedTIN", dr["EncryptedPracticeTIN"]);
                    propertySourceSite.SetAttributeValue("PropertyValue", strRegionID);
                    xdoc.Element("Config").Element("Sites").Element("Site").Element("SubSites").Add(sourceSite);
                    //xdoc.Element("Config").Element("Sites").Element("Site").Element("SubSites").Element("Site").Element("SiteSettings").Element("PropertyBag").Element("Property").Add(propertySourceSite);
                    xdoc.Save(xmlfilePath);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void createJSONConfig(DataTable dt)
        {
            try
            {
                string strVariable = "var practices = [";
                string strText = "";
                string strName = "name: ";
                string strNameValue = "";
                string strEncryptedTIN = "encryptedTIN: ";
                string strTINValue = "";
                string strURL = "url: ";
                string strFormatURL = "";
                //string strPracticeSite = "PracticeSite20_PM";
                string strPracticeSite = "/bi/fhppp/portal/PM";
                string strPracticeSiteValue = "";
                DataTable dtDataNew = dt.Clone();
                dtDataNew = dt.AsEnumerable().Where(row => row.Field<Int32>("KC365") != 0).CopyToDataTable();
                for (int intLoop = 0; intLoop < dtDataNew.Rows.Count; intLoop++)
                {
                    if (strText != "")
                    {
                        strText = strText + ",";
                    }
                    if (Convert.ToInt32(dtDataNew.Rows[intLoop]["GroupID"]) <= 9)
                    {
                        strPracticeSiteValue = strPracticeSite + Convert.ToInt32(dtDataNew.Rows[intLoop]["GroupID"]).ToString("00");
                    }
                    else
                    {
                        strPracticeSiteValue = strPracticeSite + dtDataNew.Rows[intLoop]["GroupID"].ToString();
                    }
                    strNameValue = '"' + dtDataNew.Rows[intLoop]["PracticeName"].ToString() + '"';
                    strTINValue = '"' + dtDataNew.Rows[intLoop]["EncryptedPracticeTIN"].ToString() + '"';
                    strFormatURL = '"' + strPracticeSiteValue + "/" + dtDataNew.Rows[intLoop]["SiteID"].ToString() + "/Pages/Home.aspx" + '"';
                    strText = strText + @"{" + strName + strNameValue + "," + strEncryptedTIN + strTINValue + "," + strURL + strFormatURL + "}";
                }
                strVariable = strVariable + strText + "]";

                using (StreamWriter writer = new StreamWriter(ConfigurationManager.AppSettings["PracticeJS"]))
                {
                    writer.WriteLine(strVariable);
                }
            }
            catch (Exception ex)
            {
                //test
            }
        }

        public static string formateSiteName(string strSiteName)
        {
            if (strSiteName.Split(',').Count() > 1)
            {
                return changeSiteNameTitleCaseNxt(strSiteName);
            }
            else
            {
                return changeSiteNameTitleCase(strSiteName);
            }
        }
        public static string changeSiteNameTitleCase(string strSiteName)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string strText = "";
            string[] strSiteNameArr = strSiteName.Split(' ');
            for (int intArray = 0; intArray < strSiteNameArr.Count(); intArray++)
            {
                if ((intArray + 1) != strSiteNameArr.Count())
                {
                    if (strSiteNameArr[intArray].ToString().ToLower() == "and" || strSiteNameArr[intArray].ToString().ToLower() == "of")
                    {
                        strText = strText + " " + strSiteNameArr[intArray].ToString().ToLower();
                    }
                    else
                    {
                        strText = strText + " " + textInfo.ToTitleCase(strSiteNameArr[intArray].ToString().ToLower());
                    }
                }
                else if (strSiteNameArr.Last().Contains('('))
                {
                    strText = strText + " " + strSiteNameArr[intArray].ToString();
                }
                else if (strSiteNameArr.Last().Count() < 5)
                {
                    strText = strText + " " + strSiteNameArr[intArray].ToString();
                }
                else
                {
                    strText = strText + " " + textInfo.ToTitleCase(strSiteNameArr[intArray].ToString().ToLower());
                }
            }
            return strText.Trim();
        }
        public static string changeSiteNameTitleCaseNxt(string strSiteName)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string strNewText = textInfo.ToTitleCase(strSiteName.Split(',')[0].ToLower()) + "," + strSiteName.Split(',')[1].ToString();
            if (strNewText.Contains("Of"))
            {
                strNewText = strNewText.Replace("Of", "of");
            }
            if (strNewText.Contains("And"))
            {
                strNewText = strNewText.Replace("And", "and");
            }
            return strNewText;
        }
    }
}

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
        public string siteType { get; set; }
        public PMData()
        {

        }
    }

    public class SitePMData
    {
        public string programParticipationIWH = "InterWell Health";
        public string programParticipationCKCC = "KCE Participation";
        public string programParticipationKC365 = "KC365";
        public static void initialConnect()
        {
            SitePMData objSitePMData = new SitePMData();
            objSitePMData.readPMSiteData();
        }
        public void readPMSiteData()
        {
            try
            {
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
                        updateXML(dtDataNew, ConfigurationManager.AppSettings["ConfigURL"] + "PracticeSiteTemplate_PM" + Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"]).ToString("00") + ".config", "PracticeSite20_PM" + Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"]).ToString("00"));
                    }
                    else
                    {
                        dtDataNew = allData.AsEnumerable().Where(row => row.Field<Int32>("GroupID") == Convert.ToInt32(distinctValues.Rows[intLoop]["GroupID"])).CopyToDataTable();
                        updateXML(dtDataNew, ConfigurationManager.AppSettings["ConfigURL"] + "PracticeSiteTemplate_PM" + distinctValues.Rows[intLoop]["GroupID"].ToString() + ".config", "PracticeSite20_PM" + distinctValues.Rows[intLoop]["GroupID"].ToString());
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
                    sourceSite.SetAttributeValue("SiteTitle", dr["PracticeName"]);
                    sourceSite.SetAttributeValue("RegionID", strRegionID);
                    sourceSite.SetAttributeValue("SiteDescription", dr["PracticeName"] + " is a member of " + strRegionID);
                    sourceSite.SetAttributeValue("IsKC365", Convert.ToInt32(dr["KC365"]) == 0 ? "false" : "true");
                    sourceSite.SetAttributeValue("kceArea", dr["CKCCArea"]);
                    sourceSite.SetAttributeValue("IsCKCC", dr["CKCCArea"].ToString() == "" ? "false" : "true");
                    sourceSite.SetAttributeValue("IsIWH", dr["IWNRegion"].ToString() == "" ? "false" : "true");
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
                string strPracticeSite = "PracticeSite20_PM";
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
    }
}

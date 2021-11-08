using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using System.Net;
using System.Configuration;
using System.IO;

namespace SiteUtility
{
    public class SiteLogUtility
    {
        private static string LogText = "";
        private static string LogFile = "";
        private static string LogFileName = "";
        private static string FileDir = "";
        private static List<string> FileList = new List<string>();
        private static List<string> LogList = new List<string>();
        public static List<LogInfo> logEntryList = new List<LogInfo>();

        static string ResultDescription = "", ResultDescConcern = "", EmailDesc = "";
        static string textLine = "\n------------------------------\n\n";
        static string textLineSPGroups = "\n-----------------------------------------------------------------------------------------------------\n";

        public class LogInfo
        {
            public LogInfo()
            {

            }

            private static int id = 1;
            private static DateTime FileDate = new DateTime();

            public DateTime LogDateTime { get; set; }
            public string LogEntry { get; set; }
            public string LogType { get; set; }
            public int GenerateId()
            {
                return id++;
            }

        }

        public static void InitLogFile(string maintAppName, string rootUrl, string siteUrl)
        {

            LogText = "PracticeSite-Maint - SiteLogUtility \n   In Progress...";
            Console.WriteLine(textLine);
            Console.WriteLine(LogText);

            LogText = CreateLog(LogFile);
            Log_Entry(LogText);
            LogText = string.Format("                  App Name: {0}", maintAppName);
            Log_Entry(LogText);
            LogText = string.Format("                  Root URL: {0}", rootUrl);
            Log_Entry(LogText);
            LogText = string.Format("                  Site URL: {0}", siteUrl);
            Log_Entry(LogText);
            Log_Entry(textLine);
        }

        public static void CreateLogEntry(string strMethod, string strMessage, string strType, string strURL)
        {
            if (strURL == "")
            {
                strURL = ConfigurationManager.AppSettings["SP_SiteUrl"];
            }
            using (ClientContext clientContext = new ClientContext(strURL))
            {
                clientContext.Credentials = new NetworkCredential("spAdmin_Dev", "$5ApjXy9", "Medspring");
                Web w = clientContext.Web;
                clientContext.Load(w);
                clientContext.ExecuteQuery();

                User theUser = clientContext.Web.EnsureUser("Medspring\\jesquivel");
                clientContext.Load(theUser);
                clientContext.ExecuteQuery();

                //List errorList = clientContext.Site.RootWeb.Lists.GetByTitle("DeploymentErrors");
                List errorList = clientContext.Web.Lists.GetByTitle("DeploymentErrors");
                ListItemCreationInformation oListItemCreationInformation = new ListItemCreationInformation();
                ListItem oItem = errorList.AddItem(oListItemCreationInformation);
                oItem["Title"] = "NewItem";
                oItem["Message"] = strMessage;
                oItem["LogType"] = strType;
                oItem["Method"] = strMethod;
                oItem["Author"] = theUser;
                oItem.Update();
                clientContext.ExecuteQuery();
            }
        }

        public static void Log_Entry(string logtext)
        {
            logEntryList.AddRange(AddLogEntry(logtext));
        }

        public static List<LogInfo> AddLogEntry(string logEntry)
        {
            List<LogInfo> lg = new List<LogInfo>();
            LogInfo li = new LogInfo();

            try
            {
                //Console.WriteLine();
                li.LogDateTime = DateTime.Now;
                li.LogEntry = logEntry;
                li.LogType = "General";

                lg.Add(li);

            }
            catch (Exception ex)
            {
                Console.WriteLine();
            }

            return lg;
        }

        public static int Add_LogInfoToList(LogInfo li)
        {
            int result = 0;
            try
            {
                List<LogInfo> logList = new List<LogInfo>();

                result = 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                throw;
            }
            //Console.WriteLine();

            return result;
        }

        public static int Log_ProcessLogs(List<LogInfo> loglines)
        {
            int count = 0;
            string success = "", failure = "";

            try
            {
                count = loglines.Count;
                if (count < 1)
                {
                    ResultDescription += "Log_ProcessLogs() -> List from logEntryList was empty \n\n";
                    Console.WriteLine(ResultDescription);
                    LogList.Add(ResultDescription);
                    return 0;
                }
                foreach (LogInfo li in loglines)
                {
                    int code = 0;
                    int cntLogLines = 0;

                    LogList.Add(string.Format("{0} - {1}  {2}", li.GenerateId(), li.LogDateTime, li.LogEntry));

                }

                LogList.Add(string.Format(textLine));
                LogList.Add(string.Format("Total Log Lines: {0}", loglines.Count));

            }
            catch (Exception ex)
            {
                //CreateErrLog(ex, "Log_ProcessLogs");
                //ErrorThrown = true;

                throw new Exception("Error Log_PrcessLogs");
            }
            return 1;
        }

        public static string CreateLog(string LogName)
        {
            //string logFolder = Path.Combine(ConfigurationManager.AppSettings["Log_Dir"], "Logs");
            string logFolder = ConfigurationManager.AppSettings["Log_Dir"];
            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);

            string fileName = LogName + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".log";
            string filePath = Path.Combine(logFolder, fileName);

            //using (StreamWriter sw = new StreamWriter(filePath))
            //    sw.WriteLine(ex.ToString());
            LogText = "Log File Created: " + DateTime.Now;
            //AddLogEntry(LogText);
            LogFileName = filePath;

            return LogText;
        }

        public static void CreateFile(string sb, string fileName)
        {
            try
            {
                string outDescFolder = Path.Combine(ConfigurationManager.AppSettings["PrimaryFolder"], "OutDescription");
                if (!Directory.Exists(outDescFolder))
                    Directory.CreateDirectory(outDescFolder);

                fileName += "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".txt";
                string filePath = Path.Combine(outDescFolder, fileName);

                using (StreamWriter sw = new StreamWriter(filePath))
                    sw.WriteLine(sb);

                LogFileName = filePath;
            }
            catch (Exception ex)
            {
                CreateErrLog(ex, "CreateFile");
            }
        }

        public static void CreateErrLog(Exception ex, string LogName)
        {
            string logFolder = Path.Combine(ConfigurationManager.AppSettings["Log_Dir"], "ErrLogs");
            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);

            string fileName = LogName + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".log";
            string filePath = Path.Combine(logFolder, fileName);

            using (StreamWriter sw = new StreamWriter(filePath))
                sw.WriteLine(ex.ToString());

            Log_Entry(ex.ToString());
        }
        public static void LogFunction1()
        {
            Console.WriteLine("LogFunction 1");
        }
        public static void LogFunction2()
        {
            Console.WriteLine("LogFunction 2");
        }
    }
}

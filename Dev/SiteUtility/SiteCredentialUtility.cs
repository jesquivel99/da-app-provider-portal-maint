using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SiteUtility
{
    public class SiteCredentialUtility
    {
        // SharePoint Dev Credentials...
        public static string UserName = "spAdmin_Dev";
        public static string Password = "$5ApjXy9";
        public static string Domain = "Medspring";


        // SharePoint Prod Credentials...
        //public static string UserName = "spAdmin";
        //public static string Password = "Xfw4E9fcis6nj5";
        //public static string Domain = "medspring";

        //public static string UserName = ConfigurationManager.AppSettings["UCN"];
        //public static string Password = ConfigurationManager.AppSettings["UCP"];
        //public static string Domain = ConfigurationManager.AppSettings["UCD"];
    }
}

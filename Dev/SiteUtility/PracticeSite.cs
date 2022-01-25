using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteUtility
{
    public class PracticeSite
    {
        public PracticeSite()
        {

        }
        public enum PracticeType { IWH, iCKCC };
        public enum FolderType { IWH, iCKCC, BOTH };
        public enum SpServer { DEV, PROD };
        public PracticeType Type;
        public string URL { get; set; }
        public string Name { get; set; }
        /// <Notes>
        /// Add any properties we need
        /// </Notes>
        public string PracticeTIN { get; set; }
        public string PracticeName { get; set; }
        public string EncryptedPracticeTIN { get; set; }
        public string PracticeNPI { get; set; }
        public string SiteId { get; set; }

        public string IWNRegion { get; set; }
        public string IWNRegionURL { get; set; }
        public string ProgramManager { get; set; }
        public string ReferralURL { get; set; }
        public string ExistingSiteUrl { get; set; }
        public string RelativeExistingSiteUrl { get; set; }
        public string ExistingSiteNone = "None";
        public string ProgramParticipation { get; set; }
        public string IsIWH { get; set; }
        public string IsCKCC { get; set; }
        public string IsKC365 { get; set; }
        public string siteType { get; set; }
        public string PracUserPermission { get; set; }
        public string PracUserPermissionDesc = "Practice Site User Permission Level";
        public string PracUserReadOnlyPermission { get; set; }
        public string PracUserReadOnlyPermissionDesc = "Read";

        public DateTime RowCreateDate { get; set; }
        public DateTime RowUpdateDate { get; set; }
    }
}

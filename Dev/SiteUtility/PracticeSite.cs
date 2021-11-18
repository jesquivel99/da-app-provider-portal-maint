using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteUtility
{
    public class PracticeSite
    {
        public string URL { get; set; }
        public string Name { get; set; }
        /// <Notes>
        /// Add any properties we need
        /// </Notes>
        public string PracticeTIN { get; set; }
        public string PracticeName { get; set; }
        public string EncryptedPracticeTIN { get; set; }
        public string PracticeNPI { get; set; }

        public string IWNRegion { get; set; }
        public string IWNRegionURL { get; set; }
        public string ProgramManager { get; set; }
        public string ReferralURL { get; set; }

        public string PracUserPermission { get; set; }
        public string PracUserPermissionDesc = "Practice Site User Permission Level";
        public string PracUserReadOnlyPermission { get; set; }
        public string PracUserReadOnlyPermissionDesc = "Read";

        public DateTime RowCreateDate { get; set; }
        public DateTime RowUpdateDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteUtility
{
    public class ProgramManagerSite
    {
        public string URL { get; set; }
        public string ProgramManagerName { get; set; }
        public List<PracticeSite> PracticeSiteCollection { get; set; }
        /// <Notes>
        /// Add any properties we need
        /// </Notes>
        public string PracticeTIN { get; set; }
        public string PracticeName { get; set; }
        public string EncryptedPracticeTIN { get; set; }
        public string PracticeNPI { get; set; }

        public string PM { get; set; }
        public string PMURL { get; set; }
        public string ProgramManager { get; set; }
        public string ReferralURL { get; set; }

        public string IWNSiteMgrPermission { get; set; }
        public string IWNSiteMgrReadOnlyPermission { get; set; }

        public DateTime RowCreateDate { get; set; }
        public DateTime RowUpdateDate { get; set; }
    }

    
}

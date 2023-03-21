using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteUtility
{
    public partial class PracticeCViewFields
    {
        //public PracticeCViewFields()
        //{

        //}

        public Boolean AddViewFields { get; set; }
        public Boolean ArrangeFieldsInGivenOrder { get; set; }
        public PracticeCViewField[] Fields { get; set; }
    }
}

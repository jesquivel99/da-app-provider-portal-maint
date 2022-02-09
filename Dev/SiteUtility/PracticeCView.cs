using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteUtility
{
    public partial class PracticeCView
    {
        //public PracticeCView()
        //{

        //}

        PracticeCViewFields _viewFields;
        PracticeCViewOrderBy _viewOrderBy;

        public string ViewName { get; set; }
        public Boolean DefaultView { get; set; }
        public Boolean CopyDefaultView { get; set; }
        public Boolean UseEscoiDasFilter { get; set; }
        public Boolean RefreshView { get; set; }
        public Boolean WebPartRibbonOptions { get; set; }
        public string Escoid { get; set; }
        public PracticeCViewFields ViewFields { get; set; }
        public PracticeCViewOrderBy ViewOrderBy { get; set; }
    }
}

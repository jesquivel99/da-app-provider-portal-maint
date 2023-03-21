using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace SiteUtility
{
    public partial class PracticeCViewOrderBy
    {
        //public PracticeCViewOrderBy()
        //{

        //}

        public PracticeCViewField[] Fields { get; set; }

        public int Count()
        {
            return Fields.Length;
        }

        public string configure_OrderBy(List list)
        {
            StringBuilder viewOrderString = new StringBuilder();
            try
            {
                viewOrderString.Append("<OrderBy>");
                foreach (PracticeCViewField vob in Fields)
                {
                    Field spf = null;
                    for (int intLoop = 0; intLoop < list.Fields.Count; intLoop++)
                    {
                        if (list.Fields[intLoop].Title == vob.FieldName)
                        {
                            spf = list.Fields[intLoop];
                        }
                    }
                    //if (list.Fields.ContainsField(vob.FieldName)) { spf = list.Fields.GetField(vob.FieldName); }
                    if (spf != null)
                    {
                        viewOrderString.Append(string.Format("<FieldRef Name=\"{0}\" />", spf.EntityPropertyName));
                    }

                }
                viewOrderString.Append("</OrderBy>");
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("configure_OrderBy", ex.Message, "Error", list.ParentWebUrl);
            }
            return viewOrderString.ToString();
        }
    }
}

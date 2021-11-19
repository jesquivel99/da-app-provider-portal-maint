using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SiteUtility
{
    public class SiteListUtility
    {
        public static void ListFunction1()
        {
            Console.WriteLine("ListFunction 1");
        }
        public static void ListFunction2()
        {
            Console.WriteLine("ListFunction 2");
        }

        public void CreateList(string strListName, string strWebURL, int listType)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(strWebURL))
                {
                    // The properties of the new custom list
                    ListCreationInformation creationInfo = new ListCreationInformation();
                    creationInfo.Title = strListName;
                    creationInfo.TemplateType = listType;

                    List newList = clientContext.Web.Lists.Add(creationInfo);
                    clientContext.Load(newList);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("CreateList", ex.Message, "Error", strWebURL);
            }
        }

        public void CreateListColumn(string strColumnXML, string strListName, string strWebURL)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(strWebURL))
                {
                    List targetList = clientContext.Web.Lists.GetByTitle(strListName);
                    Field oField = targetList.Fields.AddFieldAsXml(strColumnXML, true, AddFieldOptions.AddFieldInternalNameHint);

                    clientContext.Load(oField);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("CreateListColumn", ex.Message, "Error", strWebURL);
            }
        }

        public void CreateDocumentLibrary(string strListName, string strWebURL)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(strWebURL))
                {
                    // The properties of the new custom list
                    ListCreationInformation creationInfo = new ListCreationInformation();
                    creationInfo.Title = strListName;
                    creationInfo.TemplateType = (int)ListTemplateType.DocumentLibrary;

                    List newList = clientContext.Web.Lists.Add(creationInfo);
                    clientContext.Load(newList);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("CreateDocumentLibrary", ex.Message, "Error", strWebURL);
            }
        }

        public void CreateListItem(string strListName, string webUrl, List<string> listColumnName,List<string> listItemData)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(webUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web w = clientContext.Web;
                    clientContext.Load(w);
                    clientContext.ExecuteQuery();

                    List oList = clientContext.Web.Lists.GetByTitle(strListName);
                    ListItemCreationInformation oListItemCreationInformation = new ListItemCreationInformation();
                    ListItem oItem = oList.AddItem(oListItemCreationInformation);

                    for (int intLoop = 0; intLoop < listColumnName.Count; intLoop++)
                    {
                        oItem[listColumnName[intLoop]] = listItemData[intLoop];
                        oItem.Update();
                    }
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("CreateListItem", ex.Message, "Error", webUrl);
            }
        }
    }
}

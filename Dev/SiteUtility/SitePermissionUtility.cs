using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.SharePoint.Client;
using Serilog;

namespace SiteUtility
{
    public class SitePermissionUtility
    {
        static ILogger logger = Log.ForContext<SitePermissionUtility>();
        public class PmAssignment
        {
            public PmAssignment()
            {

            }

            public string PMRefId { get; set; }
            public string PMName { get; set; }
            public string PMGroup { get; set; }
        }
        public static bool RoleAssignment_AddPortalBusinessAdminUserReadOnly(PracticeSite pracInfo)
        {
            string pTin = pracInfo.PracticeTIN;

            //string path = siteUrl + pracInfo.SiteMgrRegionRef + "/" + pracInfo.PracticeTIN;
            string path = pracInfo.URL;

            try
            {
                using (ClientContext ctx = new ClientContext(path))
                {
                    ctx.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web w = ctx.Web;
                    ctx.Load(w);
                    ctx.ExecuteQuery();

                    //Get by name > RoleDefinition...
                    RoleDefinition roleReadOnly = w.RoleDefinitions.GetByName("Read");

                    //Get by name > Group...
                    Group oGroup = w.SiteGroups.GetByName("Portal_Business_Admin_User");

                    RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(ctx);
                    collRoleDefinitionBinding.Add(roleReadOnly);

                    // Add Group and RoleDefinitionBinding to RoleAssignments...
                    w.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

                    ctx.Load(oGroup, group => group.Title);
                    ctx.Load(roleReadOnly, role => role.Name);
                    ctx.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                logger.Information(ex.Message);
                SiteLogUtility.CreateLogEntry("RoleAssignment_AddPortalBusinessAdminUserReadOnly", ex.Message, "Error", "");
                return false;
            }

            return true;
        }

        public static bool RoleAssignment_AddRiskAdjustmentUserReadOnly(PracticeSite pracInfo)
        {
            string pTin = pracInfo.PracticeTIN;

            //string path = siteUrl + pracInfo.SiteMgrRegionRef + "/" + pracInfo.PracticeTIN;
            string path = pracInfo.URL;

            try
            {
                using (ClientContext ctx = new ClientContext(path))
                {
                    ctx.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web w = ctx.Web;
                    ctx.Load(w);
                    ctx.ExecuteQuery();

                    //Get by name > RoleDefinition...
                    RoleDefinition roleReadOnly = w.RoleDefinitions.GetByName("Read");

                    //Get by name > Group...
                    Group oGroup = w.SiteGroups.GetByName("Risk_Adjustment_User");

                    RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(ctx);
                    collRoleDefinitionBinding.Add(roleReadOnly);

                    // Add Group and RoleDefinitionBinding to RoleAssignments...
                    w.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

                    ctx.Load(oGroup, group => group.Title);
                    ctx.Load(roleReadOnly, role => role.Name);
                    ctx.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                logger.Information(ex.Message);
                SiteLogUtility.CreateLogEntry("RoleAssignment_AddRiskAdjustmentUserReadOnly", ex.Message, "Error", "");
                return false;
            }

            return true;
        }

        public static bool RoleAssignment_AddPracReadOnly(Practice pracInfo)
        {
            string pTin = pracInfo.TIN;

            //string path = siteUrl + pracInfo.SiteMgrRegionRef + "/" + pracInfo.PracticeTIN;
            string path = pracInfo.NewSiteUrl;

            try
            {
                using (ClientContext ctx = new ClientContext(path))
                {
                    ctx.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web w = ctx.Web;
                    ctx.Load(w);
                    ctx.ExecuteQuery();

                    //Get by name > RoleDefinition...
                    RoleDefinition roleReadOnly = w.RoleDefinitions.GetByName("Read");

                    //Get by name > Group...
                    Group oGroup = w.SiteGroups.GetByName("Prac_" + pTin + "_ReadOnly");

                    RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(ctx);
                    collRoleDefinitionBinding.Add(roleReadOnly);

                    // Add Group and RoleDefinitionBinding to RoleAssignments...
                    w.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

                    ctx.Load(oGroup, group => group.Title);
                    ctx.Load(roleReadOnly, role => role.Name);
                    ctx.ExecuteQuery();

                    logger.Information($"Grant Permissions - Added:  Prac_{pTin}_ReadOnly");
                }
            }
            catch (Exception ex)
            {
                logger.Information(ex.Message);
                SiteLogUtility.CreateLogEntry("RoleAssignment_AddPracReadOnly", ex.Message, "Error", "");
                return false;
            }

            return true;
        }

        public static bool RoleAssignment_AddPracUser(Practice pracInfo)
        {
            string pTin = pracInfo.TIN;

            //string path = siteUrl + pracInfo.SiteMgrRegionRef + "/" + pracInfo.PracticeTIN;
            string path = pracInfo.NewSiteUrl;

            try
            {
                using (ClientContext ctx = new ClientContext(path))
                {
                    ctx.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web w = ctx.Web;
                    ctx.Load(w);
                    ctx.ExecuteQuery();

                    //Get by name > RoleDefinition...
                    RoleDefinition roleReadOnly = w.RoleDefinitions.GetByName("Practice Site User Permission Level");

                    //Get by name > Group...
                    Group oGroup = w.SiteGroups.GetByName("Prac_" + pTin + "_User");

                    RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(ctx);
                    collRoleDefinitionBinding.Add(roleReadOnly);

                    // Add Group and RoleDefinitionBinding to RoleAssignments...
                    w.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

                    ctx.Load(oGroup, group => group.Title);
                    ctx.Load(roleReadOnly, role => role.Name);
                    ctx.ExecuteQuery();

                    logger.Information($"Grant Permissions - Added:  Prac_{pTin}_User");
                }
            }
            catch (Exception ex)
            {
                logger.Information(ex.Message);
                SiteLogUtility.CreateLogEntry("RoleAssignment_AddPracUser", ex.Message, "Error", "");
                return false;
            }

            return true;
        }

        private static bool RoleAssignment_AddSiteManager(string siteName, List<PmAssignment> pmAssignment, string strUrl)
        {
            int sStart = siteName.Length - 2;
            string PMid = siteName.Substring(sStart, 2);
            PmAssignment result = pmAssignment.Find(x => x.PMRefId == PMid);

            //string path = siteUrl + pracInfo.SiteMgrRegionRef + "/" + pracInfo.PracticeTIN;
            string path = strUrl;

            try
            {
                using (ClientContext clientContext = new ClientContext(path))
                {
                    Web w = clientContext.Web;
                    clientContext.Load(w);
                    clientContext.ExecuteQuery();

                    //Get by name > RoleDefinition...
                    RoleDefinition roleContributorPM = clientContext.Web.RoleDefinitions.GetByName("Practice Manager Site Permission Level");

                    //Get by name > Group...
                    Group oGroup = w.SiteGroups.GetByName(result.PMGroup + "_SiteManager");

                    RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(clientContext);
                    collRoleDefinitionBinding.Add(roleContributorPM);

                    // Add Group and RoleDefinitionBinding to RoleAssignments...
                    w.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

                    clientContext.Load(oGroup, group => group.Title);
                    clientContext.Load(roleContributorPM, role => role.Name);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                logger.Information(ex.Message);
                SiteLogUtility.CreateLogEntry("RoleAssignment_AddSiteManager", ex.Message, "Error", "");
                return false;
            }

            return true;
        }

        private static bool RoleAssignment_AddSiteManagerReadOnly(string siteName, List<PmAssignment> pmAssignment, string strUrl)
        {
            int sStart = siteName.Length - 2;
            string PMid = siteName.Substring(sStart, 2);
            PmAssignment result = pmAssignment.Find(x => x.PMRefId == PMid);

            //string path = siteUrl + pracInfo.SiteMgrRegionRef + "/" + pracInfo.PracticeTIN;
            string path = strUrl;

            try
            {
                using (ClientContext clientContext = new ClientContext(path))
                {
                    Web w = clientContext.Web;
                    clientContext.Load(w);
                    clientContext.ExecuteQuery();

                    //Get by name > RoleDefinition...
                    RoleDefinition roleReadOnly = clientContext.Web.RoleDefinitions.GetByName("Read");

                    //Get by name > Group...
                    Group oGroup = w.SiteGroups.GetByName(result.PMGroup + "_ReadOnly");

                    RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(clientContext);
                    collRoleDefinitionBinding.Add(roleReadOnly);

                    // Add Group and RoleDefinitionBinding to RoleAssignments...
                    w.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

                    clientContext.Load(oGroup, group => group.Title);
                    clientContext.Load(roleReadOnly, role => role.Name);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                logger.Information(ex.Message);
                SiteLogUtility.CreateLogEntry("RoleAssignment_AddSiteManagerReadOnly", ex.Message, "Error", "");
                return false;
            }

            return true;
        }

        public static bool GetWebGroups(Practice pracInfo)
        {
            var path = pracInfo.NewSiteUrl;

            try
            {
                using (ClientContext clientContext = new ClientContext(path))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;

                    //Parameters to receive response from the server    
                    //RoleAssignments property should be passed in Load method to get the collection of Groups assigned to the web    
                    clientContext.Load(web, w => w.Title);
                    RoleAssignmentCollection roleAssignmentColl = web.RoleAssignments;

                    //RoleAssignment.Member property returns the group associated to the web  
                    //RoleAssignement.RoleDefinitionBindings property returns the permissions associated to the group for the web  
                    clientContext.Load(roleAssignmentColl,
                        roleAssignement => roleAssignement.Include(
                            r => r.Member,
                            r => r.RoleDefinitionBindings));
                    clientContext.ExecuteQuery();


                    SiteLogUtility.LogText = $"Groups has permission to the Web:  {web.Title}";
                    logger.Information(SiteLogUtility.LogText);

                    SiteLogUtility.LogText = $"Groups Count:  {roleAssignmentColl.Count}";
                    logger.Information(SiteLogUtility.LogText);

                    SiteLogUtility.LogText = "Group with Permissions as follows:  ";
                    logger.Information(SiteLogUtility.LogText);

                    foreach (RoleAssignment grp in roleAssignmentColl)
                    {
                        string strGroup = "";
                        strGroup += $"    {grp.Member.Title} : ";

                        foreach (RoleDefinition rd in grp.RoleDefinitionBindings)
                        {
                            strGroup += $"{rd.Name} ";
                        }
                        logger.Information(strGroup);
                    }
                    //Console.Read();
                }
            }
            catch (Exception ex)
            {
                logger.Information(ex.Message);
                SiteLogUtility.CreateLogEntry("GetWebGroups", ex.Message, "Error", "");
                return false;
            }

            return true;
        }
        private static string GetPermission(string spUserGroup, string permLevel, string sUrl)
        {
            using (ClientContext clientContext = new ClientContext(sUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    clientContext.Load(clientContext.Web,
                        web => web.SiteGroups.Include(
                            g => g.Title,
                            g => g.Id),
                        web => web.RoleAssignments.Include(
                            assignment => assignment.PrincipalId,
                            assignment => assignment.RoleDefinitionBindings.Include(
                                defBindings => defBindings.Name)),
                        web => web.RoleDefinitions.Include(
                            definition => definition.Id,
                            definition => definition.Name,
                            definition => definition.Description));
                    clientContext.ExecuteQuery();

                    RoleDefinition readDef = clientContext.Web.RoleDefinitions.FirstOrDefault(
                            definition => definition.Name == permLevel);
                    Group group = clientContext.Web.SiteGroups.FirstOrDefault(
                            g => g.Title == spUserGroup);
                    if (readDef == null || group == null) return "";

                    foreach (RoleAssignment roleAssignment in clientContext.Web.RoleAssignments)
                    {
                        if (roleAssignment.PrincipalId == group.Id)
                        {
                            SiteLogUtility.Log_Entry($"PrincipalId: {roleAssignment.PrincipalId}  - GroupId: {group.Id}");

                            // If we want to Remove selected Permission
                            //roleAssignment.RoleDefinitionBindings.Remove(readDef);
                        }
                        SiteLogUtility.Log_Entry($"PrincipalId: {roleAssignment.PrincipalId} - RoleDefBindings Cnt: {roleAssignment.RoleDefinitionBindings.Count}");
                        clientContext.ExecuteQuery();
                    }

                }
                catch (Exception ex)
                {
                    logger.Information(ex.Message);
                    SiteLogUtility.CreateLogEntry("GetPermission", ex.Message, "Error", "");
                }
                return "";
            }
        }
    }
}

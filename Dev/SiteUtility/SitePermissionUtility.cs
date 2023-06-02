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
        public SitePermissionUtility()
        {
        }
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

                    logger.Information($"Grant Permissions - Added:  {oGroup.Title} - {roleReadOnly.Name}");
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

                    logger.Information($"Grant Permissions - Added:  {oGroup.Title} - {roleReadOnly.Name}");
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
        public static bool RoleAssignment_AddSiteManager(Practice practice, string strUrl)
        {
            //int sStart = siteName.Length - 2;
            //string PMid = siteName.Substring(sStart, 2);
            //PmAssignment result = pmAssignment.Find(x => x.PMRefId == PMid);
            string pmIWNRegion = "IWNRegion" + practice.PMGroup;

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
                    Group oGroup = w.SiteGroups.GetByName(pmIWNRegion + "_SiteManager");

                    RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(clientContext);
                    collRoleDefinitionBinding.Add(roleContributorPM);

                    // Add Group and RoleDefinitionBinding to RoleAssignments...
                    w.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

                    clientContext.Load(oGroup, group => group.Title);
                    clientContext.Load(roleContributorPM, role => role.Name);
                    clientContext.ExecuteQuery();

                    logger.Information($"Grant Permissions - Added:  {oGroup.Title} - {roleContributorPM.Name}");
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
        public static bool RoleAssignment_AddSiteManagerReadOnly(Practice practice, string strUrl)
        {
            //int sStart = siteName.Length - 2;
            //string PMid = siteName.Substring(sStart, 2);
            //PmAssignment result = pmAssignment.Find(x => x.PMRefId == PMid);
            string pmIWNRegion = "IWNRegion" + practice.PMGroup;

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
                    Group oGroup = w.SiteGroups.GetByName(pmIWNRegion + "_ReadOnly");

                    RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(clientContext);
                    collRoleDefinitionBinding.Add(roleReadOnly);

                    // Add Group and RoleDefinitionBinding to RoleAssignments...
                    w.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

                    clientContext.Load(oGroup, group => group.Title);
                    clientContext.Load(roleReadOnly, role => role.Name);
                    clientContext.ExecuteQuery();

                    logger.Information($"Grant Permissions - Added:  {oGroup.Title} - {roleReadOnly.Name}");
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
        public static string RoleAssignment_RemovePracUserGroup(string spUserGroup, string permLevel, string sUrl)
        {
            SiteLogUtility siteLogUtility = new SiteLogUtility();

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
                            g => g.Title.Equals(spUserGroup));
                    if (readDef == null || group == null) return "";

                    foreach (RoleAssignment roleAssignment in clientContext.Web.RoleAssignments)
                    {
                        if (roleAssignment.PrincipalId == group.Id)
                        {
                            siteLogUtility.LoggerInfo_Entry($"Group will be removed: {group.Title}", true);
                            siteLogUtility.LoggerInfo_Entry($"  PrincipalId: {roleAssignment.PrincipalId}  - GroupId: {group.Id}", true);

                            // If we want to Remove selected Permission
                            roleAssignment.RoleDefinitionBindings.Remove(readDef);
                            roleAssignment.Update();
                            clientContext.ExecuteQuery();

                        }
                        //siteLogUtility.LoggerInfo_Entry($"PrincipalId: {roleAssignment.PrincipalId} - RoleDefBindings Cnt: {roleAssignment.RoleDefinitionBindings.Count}");
                        //clientContext.ExecuteQuery();
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
        public static List<string> GetWebGroups(string wUrl)
        {
            var path = wUrl;
            List<string> listWebGrp = new List<string>();

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
                        listWebGrp.Add(grp.Member.Title);

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
                return null;
            }
            return listWebGrp;
        }
        public static List<string> GetPracUserGroups(List<string> webGroups)
        {
            return webGroups.Where(g => g.StartsWith("Prac_")).ToList();
        }
        public static List<string> GetPracUserReadOnly(List<string> webGroups)
        {
            return webGroups.Where(gb => gb.StartsWith("Prac_")).Where(ge => ge.EndsWith("_ReadOnly")).ToList();
        }
        public static List<string> GetPracUser(List<string> webGroups)
        {
            return webGroups.Where(gb => gb.StartsWith("Prac_")).Where(ge => ge.EndsWith("_User")).ToList();
        }
        public static List<string> GetPMGroupSiteManager(List<string> webGroups)
        {
            return webGroups.Where(gb => gb.StartsWith("IWNRegion")).Where(ge => ge.EndsWith("SiteManager")).ToList();
        }
        public static List<string> GetPMGroupReadOnly(List<string> webGroups)
        {
            return webGroups.Where(gb => gb.StartsWith("IWNRegion")).Where(ge => ge.EndsWith("ReadOnly")).ToList();
        }
        public static string GetPermission(string spUserGroup, string permLevel, string sUrl)
        {
            SiteLogUtility siteLogUtility = new SiteLogUtility();

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
                            siteLogUtility.LoggerInfo_Entry($"PrincipalId: {roleAssignment.PrincipalId}  - GroupId: {group.Id}", true);

                            // If we want to Remove selected Permission
                            //roleAssignment.RoleDefinitionBindings.Remove(readDef);
                        }
                        siteLogUtility.LoggerInfo_Entry($"PrincipalId: {roleAssignment.PrincipalId} - RoleDefBindings Cnt: {roleAssignment.RoleDefinitionBindings.Count}");
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
        public static bool CreateSiteGroup(string pracUrl, string grpTitle, string grpDesc)
        {
            using (ClientContext clientContext = new ClientContext(pracUrl))
            {
                SiteLogUtility siteLogUtility = new SiteLogUtility();
                try
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();

                    GroupCollection groups = clientContext.Web.SiteGroups;
                    GroupCreationInformation grpCreationInformation = new GroupCreationInformation();
                    grpCreationInformation.Title = grpTitle;
                    grpCreationInformation.Description = grpDesc;

                    Group group = groups.Add(grpCreationInformation);
                    clientContext.ExecuteQuery();

                    siteLogUtility.LoggerInfo_Entry(grpTitle + " Group Created");
                    return true;
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("CreateSiteGroup - Error", ex.Message, "Error", pracUrl);
                    return false;
                }

            }
        }
        public static bool CheckIfGroupExists(string pracUrl, string strGroupName)
        {
            using (ClientContext clientContext = new ClientContext(pracUrl))
            {
                try
                {
                    clientContext.Credentials = new NetworkCredential( SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();

                    Group oGroup = web.SiteGroups.GetByName(strGroupName);
                    clientContext.Load(oGroup,
                        group => group.Title,
                        group => group.Description,
                        group => group.Id);
                    clientContext.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("CheckIfGroupExists - Error", ex.Message, "Error", "");
                    return false;
                }
                return true;
            }
        }
        public static bool BreakRoleInheritanceOnList(string practiceURL, string userList, string userGroup, RoleType roleToAdd)
        {
            //
            // Usage - 
            // BreakRoleInheritanceOnList("https://sharepointdev.fmc-na-icg.com/bi/fhppp/portal/PM06/99881985029"
            //                           ,"RiskAdjustment_iwh"
            //                           ,"Risk_Adjustment_User"
            //                           ,RoleType.Contributor);
            //
            try
            {
                using (ClientContext clientContext = new ClientContext(practiceURL))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    clientContext.Load(clientContext.Web.RoleAssignments);
                    clientContext.ExecuteQuery();

                    RoleDefinition roleToAddDefinition = clientContext.Web.RoleDefinitions.GetByType(roleToAdd);

                    if (clientContext.Web.RoleAssignments != null && clientContext.Web.RoleAssignments.Count > 1)
                    {
                        Group group = clientContext.Web.SiteGroups.GetByName(userGroup);
                        clientContext.Load(group);

                        Microsoft.SharePoint.Client.List list = clientContext.Web.Lists.GetByTitle(userList);
                        clientContext.ExecuteQuery();
                        list.BreakRoleInheritance(true, true);
                        RoleDefinitionBindingCollection collRoleDefinitionBindingList = new RoleDefinitionBindingCollection(clientContext);
                        collRoleDefinitionBindingList.Add(roleToAddDefinition);
                        list.RoleAssignments.Add(group, collRoleDefinitionBindingList);

                        clientContext.ExecuteQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("BreakRoleInheritanceOnList - Error", ex.Message, "Error", "");
                return false;
            }

            return true;
        }
        public static void AddSecurityGroupToList(string strURL, string strSecurityGroupName, string strListName, string strPermissionType)
        {
            try
            {
                using (ClientContext clientContext = new ClientContext(strURL))
                {
                    List targetList = clientContext.Web.Lists.GetByTitle(strListName);
                    clientContext.Load(targetList, target => target.HasUniqueRoleAssignments);
                    clientContext.ExecuteQuery();

                    if (targetList.HasUniqueRoleAssignments)
                    {
                        // Write group name to be added in the list
                        Group group = clientContext.Web.SiteGroups.GetByName(strSecurityGroupName);
                        RoleDefinitionBindingCollection roleDefCollection = new RoleDefinitionBindingCollection(clientContext);

                        // Set the permission level of the group for this particular list
                        RoleDefinition readDef = clientContext.Web.RoleDefinitions.GetByName(strPermissionType);
                        roleDefCollection.Add(readDef);

                        Principal userGroup = group;
                        RoleAssignment roleAssign = targetList.RoleAssignments.Add(userGroup, roleDefCollection);

                        clientContext.Load(roleAssign);
                        roleAssign.Update();
                        clientContext.ExecuteQuery();

                        logger.Information("Group {0} Added to {1} with Permission Type = {2}", strSecurityGroupName, strListName, strPermissionType);
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.CreateLogEntry("AddSecurityGroupToList", ex.Message, "Error", "");
            }
        }
    }
}

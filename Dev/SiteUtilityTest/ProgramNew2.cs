using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteUtility;
using System.Configuration;
using Microsoft.SharePoint.Client;
using System.Net;

namespace SiteUtilityTest
{
    public class ProgramNew2
    {
        // AuditMode = true    will NOT execute code to remove SharePoint Permission Groups
        // AuditMode = false   will execute code to remove SharePoint Permission Groups
        public static bool AuditMode = true;
        public void InitiateProgNew2()
        {
            string rootUrl = ConfigurationManager.AppSettings["SP_RootUrl"];
            string siteUrl = ConfigurationManager.AppSettings["SP_SiteUrl"];
            string siteInfoFile = ConfigurationManager.AppSettings["Csv_File"];
            string releaseName = "SiteUtilityTest";

            SiteLogUtility.InitLogFile(releaseName, rootUrl, siteUrl);

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                SiteLogUtility.Log_Entry("=============Release Starts=============", true);

                try
                {
                    //  Get all Practice Data...
                    SiteLogUtility.Log_Entry("=============[ Get all Practice Data ]=============", true);
                    List<ProgramManagerSite> practicePMSites = SiteInfoUtility.GetAllPracticeDetails(clientContext);

                    //  Maintenance Tasks...
                    SiteLogUtility.Log_Entry("\n\n=============[ Maintenance Tasks ]=============", true);
                    foreach (ProgramManagerSite pm in practicePMSites)
                    {
                        foreach (PracticeSite psite in pm.PracticeSiteCollection)
                        {
                            if (psite.URL.Contains("94910221369") || psite.URL.Contains("91101941279"))
                            {
                                SiteNavigateUtility.NavigationPracticeMnt(psite.URL, pm.PMURL);
                            }

                            if (psite.URL.Contains("94910221369") || psite.URL.Contains("91101941279"))
                            {
                                SiteLogUtility.Log_Entry("Adding RoleAssignments - AddPortalBusinessAdminUserReadOnly, AddRiskAdjustmentUserReadOnly", true);
                                RoleAssignment_AddPortalBusinessAdminUserReadOnly(psite);
                                RoleAssignment_AddRiskAdjustmentUserReadOnly(psite);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SiteLogUtility.CreateLogEntry("PracticeSite-Maint - Program", ex.Message, "Error", siteUrl);
                }
                finally
                {
                    SiteLogUtility.finalLog(releaseName);
                }
                SiteLogUtility.Log_Entry("=============Release Ends=============", true);
            }
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
                SiteLogUtility.CreateLogEntry("RoleAssignment_AddRiskAdjustmentUserReadOnly", ex.Message, "Error", "");
                return false;
            }

            return true;
        }

        public static bool GetWebGroups(PracticeSite pracInfo)
        {
            var path = pracInfo.URL;

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
                    SiteLogUtility.Log_Entry(SiteLogUtility.LogText, true);

                    SiteLogUtility.LogText = $"Groups Count:  {roleAssignmentColl.Count}";
                    SiteLogUtility.Log_Entry(SiteLogUtility.LogText, true);

                    SiteLogUtility.LogText = "Group with Permissions as follows:  ";
                    SiteLogUtility.Log_Entry(SiteLogUtility.LogText, true);

                    foreach (RoleAssignment grp in roleAssignmentColl)
                    {
                        string strGroup = "";
                        strGroup += $"    {grp.Member.Title} : ";

                        foreach (RoleDefinition rd in grp.RoleDefinitionBindings)
                        {
                            strGroup += $"{rd.Name} ";
                        }
                        SiteLogUtility.Log_Entry(strGroup, true);
                    }
                    //Console.Read();
                }
            }
            catch (Exception)
            {
                return false;
                //throw;
            }

            return true;
        }


        //------------------------[ Testing all programs below or pulling code to use for above ]----------------------------------
        public static bool GetSpGroups(ProgramManagerSite pmInfo, PracticeSite pracInfo)
        {
            try
            {
                var path = pracInfo.URL;

                SiteLogUtility.LogText = $"Processing:  {path}";
                SiteLogUtility.Log_Entry(SiteLogUtility.LogText, true);

                // Set Permission Property Values...
                //SetPermissionValue(pmInfo, pracInfo);

                using (ClientContext clientContext = new ClientContext(path))
                {
                    bool removePracUserGroup = false;
                    bool removePracReadOnlyGroup = false;
                    bool removeSiteMgrGroup = false;
                    bool removeSiteMgrReadOnlyGroup = false;

                    try
                    {
                        removePracUserGroup = RemoveSpGroups(pracInfo.PracUserPermission, path);
                        removePracReadOnlyGroup = RemoveSpGroups(pracInfo.PracUserReadOnlyPermission, path);
                        removeSiteMgrGroup = RemoveSpGroups(pmInfo.IWNSiteMgrPermission, path);
                        removeSiteMgrReadOnlyGroup = RemoveSpGroups(pmInfo.IWNSiteMgrReadOnlyPermission, path);
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.Log_Entry("GetSpGroups Error: " + ex.ToString(), true);
                    }

                    //finally
                    //{
                    //    SiteLogUtility.Log_Entry("Remove Summary: " +
                    //        "PracUserGroup = " + removePracUserGroup.ToString() + " | " +
                    //        "PracReadOnlyGroup = " + removePracReadOnlyGroup.ToString() + " | " +
                    //        "SiteMgrGroup = " + removeSiteMgrGroup.ToString() + " | " +
                    //        "SiteMgrReadOnly = " + removeSiteMgrReadOnlyGroup.ToString());
                    //}

                }
            }

            catch (Exception ex)
            {
                SiteLogUtility.Log_Entry("GetSpGroups Error: " + ex.ToString(), true);
                return false;
            }

            return true;
        }

        public static bool RemoveAllSpGroups(ProgramManagerSite pmInfo, PracticeSite pracInfo)
        {
            try
            {
                var path = pracInfo.URL;

                SiteLogUtility.LogText = $"Processing:  {path}";
                Console.WriteLine(SiteLogUtility.LogText);
                SiteLogUtility.Log_Entry(SiteLogUtility.LogText);

                using (ClientContext clientContext = new ClientContext(path))
                {
                    bool removePracUserGroup = false;
                    bool removePracReadOnlyGroup = false;
                    bool removeSiteMgrGroup = false;
                    bool removeSiteMgrReadOnlyGroup = false;

                    try
                    {
                        removePracUserGroup = RemoveSpGroups(pracInfo.PracUserPermission, path);
                        removePracReadOnlyGroup = RemoveSpGroups(pracInfo.PracUserReadOnlyPermission, path);
                        removeSiteMgrGroup = RemoveSpGroups(pmInfo.IWNSiteMgrPermission, path);
                        removeSiteMgrReadOnlyGroup = RemoveSpGroups(pmInfo.IWNSiteMgrReadOnlyPermission, path);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("GetSpGroups Error: " + ex.ToString());
                        SiteLogUtility.Log_Entry("GetSpGroups Error: " + ex.ToString());
                    }

                    //finally
                    //{
                    //    SiteLogUtility.Log_Entry("Remove Summary: " +
                    //        "PracUserGroup = " + removePracUserGroup.ToString() + " | " +
                    //        "PracReadOnlyGroup = " + removePracReadOnlyGroup.ToString() + " | " +
                    //        "SiteMgrGroup = " + removeSiteMgrGroup.ToString() + " | " +
                    //        "SiteMgrReadOnly = " + removeSiteMgrReadOnlyGroup.ToString());
                    //}

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("GetSpGroups Error: " + ex.ToString());
                SiteLogUtility.Log_Entry("GetSpGroups Error: " + ex.ToString());
                return false;
            }

            return true;
        }

        public static bool RemoveSingleSpGroup(string spUserGroup, string permLevel, string sUrl)
        {
            try
            {
                SiteLogUtility.LogText = $"Processing:  {sUrl}";
                SiteLogUtility.Log_Entry(SiteLogUtility.LogText, true);

                using (ClientContext clientContext = new ClientContext(sUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                    bool removePracUserGroup = false;
                    string getPracUserPermission = string.Empty;

                    try
                    {
                        getPracUserPermission = GetPermission(spUserGroup, permLevel, sUrl);  //this actually deletes at the moment...
                        removePracUserGroup = RemoveSpGroups(spUserGroup, sUrl);
                    }
                    catch (Exception ex)
                    {
                        SiteLogUtility.Log_Entry("GetSpGroups Error: " + ex.ToString(), true);
                    }

                }
            }

            catch (Exception ex)
            {
                SiteLogUtility.Log_Entry("GetSpGroups Error: " + ex.ToString(), true);
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
                    SiteLogUtility.CreateLogEntry("GetPermission", ex.Message, "Error", sUrl);
                }
                return "";
            }
        }

        private static bool RemoveSpGroups(string spUserGroup, string path)
        {
            using (ClientContext clientContext = new ClientContext(path))
            {
                clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                try
                {
                    Web web = null;
                    web = clientContext.Web;
                    RoleAssignmentCollection assignColl;
                    RoleAssignment roleAssign;

                    string userOrGroup = spUserGroup; //we can give either title or login Name of the user/group.
                    string permissionLevel = "Practice Site User Permission Level"; //we can Give name of any permission level name.

                    clientContext.Load(web.RoleAssignments,
                        roles => roles.Include(
                            r => r.Member,
                            r => r.Member.LoginName,
                            r => r.Member.Title,
                            r => r.RoleDefinitionBindings
                    ));

                    clientContext.ExecuteQuery();

                    assignColl = web.RoleAssignments;

                    //for (int isitePermCount = 0; isitePermCount < assignColl.Count; isitePermCount++)
                    for (int isitePermCount = 0; isitePermCount < assignColl.Count; isitePermCount++)
                    {
                        try
                        {
                            roleAssign = assignColl[isitePermCount];
                            string userLoginName = string.Empty;
                            string userTitle = string.Empty;
                            userLoginName = roleAssign.Member.LoginName;
                            userTitle = roleAssign.Member.Title;

                            if (userTitle == userOrGroup || userLoginName == userOrGroup)

                            {
                                //Get the roledefinition from it’s name
                                RoleDefinition roleDef = web.RoleDefinitions.GetByName(permissionLevel);

                                // If we want to Remove selected Permission
                                //roleAssign.RoleDefinitionBindings.Remove(roleDef);
                                SiteLogUtility.LogText = $"Will be removed: {roleAssign.PrincipalId}";
                                SiteLogUtility.Log_Entry(SiteLogUtility.LogText);

                                // If we want to Add selected Permission
                                //roleAssign.RoleDefinitionBindings.Add(roleDef);
                                //roleAssign.Update();
                                clientContext.ExecuteQuery();

                                Console.WriteLine(SiteLogUtility.LogText);
                            }
                        }

                        catch
                        {
                            return false;
                        }
                    }
                    //Console.ReadLine();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }

        public static bool RoleAssignmentCollection_Add()
        {
            return true;
        }

        public static bool RoleAssignmentCollection_AddGroupReadOnly(PracticeSite pracInfo)
        {
            var path = pracInfo.URL;

            try
            {
                using (ClientContext clientContext = new ClientContext(path))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);
                    Web web = clientContext.Web;

                    Group oGroup = web.SiteGroups.GetByName(pracInfo.PracUserReadOnlyPermission);
                    RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(clientContext);
                    RoleDefinition oRoleDefinition = web.RoleDefinitions.GetByType(RoleType.Reader);
                    collRoleDefinitionBinding.Add(oRoleDefinition);
                    web.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);
                    clientContext.Load(oGroup,
                        g => g.Id,
                        g => g.Title);
                    clientContext.Load(oRoleDefinition,
                        role => role.Id,
                        role => role.Name);
                    clientContext.ExecuteQuery();

                    SiteLogUtility.LogText = $"{oGroup.Title} created and assigned {oRoleDefinition.Name} role.";
                    SiteLogUtility.Log_Entry(SiteLogUtility.LogText, true);
                }
            }
            catch (Exception)
            {
                return false;
                //throw;
            }

            return true;
        }

        

        ///// <summary>
        ///// Set Group SiteManager - Practice Manager Site Permission Level ...
        ///// ICKCCGroup01_SiteManager
        ///// Practice Manager Site Permission Level
        ///// </summary>
        ///// <param name="pracInfo"></param>
        ///// <returns></returns>
        //private static bool RoleAssignment_AddSiteManager(SiteInfo pracInfo, List<PmAssignment> pmAssignment)
        //{
        //    int sStart = pracInfo.PMSiteName.Length - 2;
        //    string PMid = pracInfo.PMSiteName.Substring(sStart, 2);
        //    PmAssignment result = pmAssignment.Find(x => x.PMRefId == PMid);

        //    //string path = siteUrl + pracInfo.SiteMgrRegionRef + "/" + pracInfo.PracticeTIN;
        //    string path = pracInfo.SiteUrl;

        //    try
        //    {
        //        using (ClientContext ctx = new ClientContext(path))
        //        {
        //            Web w = ctx.Web;
        //            ctx.Load(w);
        //            ctx.ExecuteQuery();

        //            //Get by name > RoleDefinition...
        //            RoleDefinition oRoleDefinition = w.RoleDefinitions.GetByName("Practice Manager Site Permission Level");

        //            //Get by name > Group...
        //            //Group oGroup = w.SiteGroups.GetByName(pracInfo.SiteMgrRegionRef + "_SiteManager");
        //            Group oGroup = w.SiteGroups.GetByName(result.PMGroup + "_SiteManager");

        //            RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(ctx);
        //            collRoleDefinitionBinding.Add(oRoleDefinition);

        //            // Add Group and RoleDefinitionBinding to RoleAssignments...
        //            w.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

        //            ctx.Load(oGroup, group => group.Title);
        //            ctx.Load(oRoleDefinition, role => role.Name);
        //            ctx.ExecuteQuery();

        //            Console.WriteLine($"{oGroup.Title} created and assigned {oRoleDefinition.Name} role.");
        //            SiteLogUtility.Log_Entry($"{oGroup.Title} created and assigned {oRoleDefinition.Name} role.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SiteLogUtility.CreateLogEntry("RoleAssignmentCollection", ex.Message, "Error", "");
        //        return false;
        //    }

        //    return true;
        //}

        public static void SetPermissionValue(ProgramManagerSite pmsi, PracticeSite si)
        {
            si.PracUserPermission = "Prac_" + si.PracticeTIN + "_User";
            si.PracUserReadOnlyPermission = "Prac_" + si.PracticeTIN + "_ReadOnly";
            pmsi.IWNSiteMgrPermission = si.IWNRegion + "_SiteManager";
            pmsi.IWNSiteMgrReadOnlyPermission = si.IWNRegion + "_ReadOnly";

        }

        public static void SetPracPermissionValue(ProgramManagerSite pmsi, PracticeSite si)
        {
            si.PracUserPermission = "Prac_" + si.PracticeTIN + "_User";
            si.PracUserReadOnlyPermission = "Prac_" + si.PracticeTIN + "_ReadOnly";
            pmsi.IWNSiteMgrPermission = si.IWNRegion + "_SiteManager";
            pmsi.IWNSiteMgrReadOnlyPermission = si.IWNRegion + "_ReadOnly";

        }

        public static void SetPMPermissionValue(ProgramManagerSite pmsi, PracticeSite si)
        {
            pmsi.IWNSiteMgrPermission = si.IWNRegion + "_SiteManager";
            pmsi.IWNSiteMgrReadOnlyPermission = si.IWNRegion + "_ReadOnly";

        }

        public static void Permissions_BAK()
        {
            //----------------------------------------------------------------------
            //Web web = clientContext.Web;
            //clientContext.Load(web);
            //clientContext.ExecuteQuery();

            //var contributeRole = web.RoleDefinitions.GetByType(RoleType.Contributor);
            //var contributeRole2 = web.RoleDefinitions.GetByName("Read");

            //var group = web.SiteGroups.GetByName(spUserGroup);
            //var groupAssignment = web.RoleAssignments.GetByPrincipalId(group.Id);
            //var roles = groupAssignment.RoleDefinitionBindings;

            //clientContext.Load(group);
            //clientContext.Load(groupAssignment);
            //clientContext.ExecuteQuery();

            //if (roles.Contains(contributeRole2))
            //{
            //    //roles.Remove(contributeRole);
            //    //groupAssignment.Update();
            //    SiteLogUtility.Log_Entry($"{groupAssignment.PrincipalId} - {contributeRole2.Name}");
            //}
            //----------------------------------------------------------------------

            ////////clientContext.Load(
            ////////    clientContext.Web,
            ////////    web => web.SiteGroups.Include(
            ////////        g => g.Title,
            ////////        g => g.Id),
            ////////        web => web.RoleAssignments.Include(

            ////////            assignment => assignment.PrincipalId,
            ////////            assignment => assignment.RoleDefinitionBindings.Include(

            ////////                definitionb => definitionb.Name)),
            ////////        web => web.RoleDefinitions.Include(

            ////////                definition => definition.Name));

            ////////clientContext.ExecuteQuery();

            ////////RoleDefinition readDef = clientContext.Web.RoleDefinitions.FirstOrDefault(
            ////////        definition => definition.Name == "Read");
            ////////Group group = clientContext.Web.SiteGroups.FirstOrDefault(
            ////////        g => g.Title == spUserGroup);
            ////////if (readDef == null || group == null) return false;

            ////////foreach (RoleAssignment roleAssignment in clientContext.Web.RoleAssignments)
            ////////{
            ////////    if(roleAssignment.PrincipalId == group.Id)
            ////////    {
            ////////        SiteLogUtility.Log_Entry($"PrincipalId: {roleAssignment.PrincipalId}  - GroupId: {group.Id}");
            ////////    }
            ////////    SiteLogUtility.Log_Entry($"PrincipalId: {roleAssignment.PrincipalId} - RoleDefBindings Cnt: {roleAssignment.RoleDefinitionBindings.Count}");
            ////////    //clientContext.ExecuteQuery();
            ////////}

            //foreach (var rd in from roleAssignment in clientContext.Web.RoleAssignments
            //                   where roleAssignment.PrincipalId == @group.Id
            //                   from rd in roleAssignment.RoleDefinitionBindings.Where(
            //                       rd => rd.Name == readDef.Name)
            //                   select rd)
            //{
            //    //rd.DeleteObject();
            //    SiteLogUtility.Log_Entry($"Will be deleted: {rd.Name} - {rd.Description} - {rd.Id}");
            //}
            //clientContext.ExecuteQuery();
            //}
            //}
            //catch (Exception ex)
            //{
            //    SiteLogUtility.LogText = path;
            //    SiteLogUtility.Log_Entry(SiteLogUtility.LogText);
            //    SiteLogUtility.Log_Entry("RemoveSpGroups Error: " + ex.Message.ToString());
            //    return false;
            //}
            //return true;
            //}
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.SharePoint.Client;

namespace SiteUtility
{
    public class SitePermissionUtility
    {
        // AuditMode = true    will NOT execute code to remove SharePoint Permission Groups
        // AuditMode = false   will execute code to remove SharePoint Permission Groups
        public static bool AuditMode = true;

        public static bool GetSpGroups(ProgramManagerSite pmInfo, PracticeSite pracInfo)
        {
            try
            {
                var path = pracInfo.URL;

                SiteLogUtility.LogText = $"Processing:  {path}";
                Console.WriteLine(SiteLogUtility.LogText);
                SiteLogUtility.Log_Entry(SiteLogUtility.LogText);

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

        public static bool RemoveSingleSpGroup(string spUserGroup, string sUrl)
        {
            try
            {
                SiteLogUtility.LogText = $"Processing:  {sUrl}";
                Console.WriteLine(SiteLogUtility.LogText);
                SiteLogUtility.Log_Entry(SiteLogUtility.LogText);

                using (ClientContext clientContext = new ClientContext(sUrl))
                {
                    clientContext.Credentials = new NetworkCredential(SiteCredentialUtility.UserName, SiteCredentialUtility.Password, SiteCredentialUtility.Domain);

                    bool removePracUserGroup = false;

                    try
                    {
                        removePracUserGroup = RemoveSpGroups(spUserGroup, sUrl);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("GetSpGroups Error: " + ex.ToString());
                        SiteLogUtility.Log_Entry("GetSpGroups Error: " + ex.ToString());
                    }

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

                

        //private static bool RoleAssignmentCollection_AddGroupReadOnly(PracticeSite pracInfo)
        //{
        //    //string siteUrl = "";
        //    //var path = siteUrl + pracInfo.ICKCCGroup + "/" + pracInfo.PracticeTIN;
        //    var path = pracInfo.URL;

        //    try
        //    {
        //        using (ClientContext clientContext = new ClientContext(path))
        //        {
        //            // Set Group ReadOnly ReadOnly...
        //            // ICKCCGroup01_ReadOnly
        //            // Read

        //            Web oWebsite = clientContext.Web;

        //            //Get by name > Group...
        //            Group oGroup = oWebsite.SiteGroups.GetByName(pracInfo.ICKCCGroup + "_ReadOnly");

        //            RoleDefinitionBindingCollection collRoleDefinitionBinding = new RoleDefinitionBindingCollection(clientContext);

        //            //Get by Type > Read...
        //            RoleDefinition oRoleDefinition = oWebsite.RoleDefinitions.GetByType(RoleType.Reader);

        //            collRoleDefinitionBinding.Add(oRoleDefinition);

        //            oWebsite.RoleAssignments.Add(oGroup, collRoleDefinitionBinding);

        //            clientContext.Load(oGroup,
        //                group => group.Title);

        //            clientContext.Load(oRoleDefinition,
        //                role => role.Name);

        //            clientContext.ExecuteQuery();

        //            Console.WriteLine("{0} created and assigned {1} role.", oGroup.Title, oRoleDefinition.Name);

        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //        //throw;
        //    }

        //    return true;
        //}

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

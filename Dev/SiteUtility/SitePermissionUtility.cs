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
                Console.WriteLine("Processing: " + path);

                // Set Permission Property Values...
                SetPermissionValue(pmInfo, pracInfo);

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

                    finally
                    {
                        SiteLogUtility.Log_Entry("Remove Summary: " +
                            "PracUserGroup = " + removePracUserGroup.ToString() + " | " +
                            "PracReadOnlyGroup = " + removePracReadOnlyGroup.ToString() + " | " +
                            "SiteMgrGroup = " + removeSiteMgrGroup.ToString() + " | " +
                            "SiteMgrReadOnly = " + removeSiteMgrReadOnlyGroup.ToString());
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
            try
            {
                using (ClientContext clientContext = new ClientContext(path))
                {
                    try
                    {
                        // ------------------ Get groups object using Users 
                        // clientContext.Web.RoleAssignments.GetByPrincipal(spUserGroup).DeleteObject();
                        //var web = clientContext.Web;
                        //clientContext.Load(clientContext.Web, a => a.)
                        // Load the group
                        /*clientContext.Load(clientContext.Web,
                                                web => web.SiteGroups.Include(
                                                    g => g.Title,
                                                    g => g.Id),
                                                web => web.RoleAssignments.Include(
                                                    assignment => assignment.PrincipalId,
                                                    assignment => assignment.RoleDefinitionBindings.Include(
                                                        definition => definition.Name)),
                                                web => web.RoleDefinitions.Include(
                                                    definition => definition.Name));
                        clientContext.ExecuteQuery();
                        */
                        //Group myGroup = clientContext.Web.SiteGroups.GetByName(spUserGroup);
                        //myclientContext.Web.RoleAssignments.GetByPrincipal(spUserGroup).DeleteObject();


                        // ------------------ Get groups object using group name 
                        Group oGroup = clientContext.Web.SiteGroups.GetByName(spUserGroup);
                        // Load the group
                        clientContext.Load(oGroup);
                        //clientContext.Load(oGroup, w => w.Id,
                        //                            w => w.Title,
                        //                            w => w.LoginName,
                        //                            w => w.PrincipalType,
                        //                            w => w.Description);
                        clientContext.ExecuteQuery();

                        var oGroupTitle = oGroup.Title;
                        var oGroupLen = oGroup.Title.Length;
                        var oGroupStatus = "";
                        if (oGroupLen > 0)
                        {
                            //if (!AuditMode)
                            //{
                            //    // Remove group
                            //    clientContext.Web.SiteGroups.Remove(oGroup);
                            //    clientContext.ExecuteQuery();

                            //    oGroupStatus = "Removed SP Group: ";
                            //    Console.WriteLine(oGroupStatus + oGroupTitle);
                            //    SiteLogUtility.Log_Entry(oGroupStatus + oGroupTitle);
                            //}

                            if (AuditMode)
                            {
                                oGroupStatus = "Will Remove SP Group: ";
                                Console.WriteLine("SP Group: " + oGroupTitle);
                                SiteLogUtility.Log_Entry("Will Remove SP Group: " + oGroupTitle);
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ClientContext RemoveSpGroups Error: " + ex.Message.ToString());
                        SiteLogUtility.Log_Entry("ClientContext SPUserGroup: " + spUserGroup);
                        SiteLogUtility.Log_Entry("ClientContext RemoveSpGroups Error: " + ex.Message.ToString());
                        //throw;
                    }
                }
            }
            catch (Exception ex)
            {
                SiteLogUtility.Log_Entry("RemoveSpGroups Error: " + ex.Message.ToString());
                //throw;
                return false;
            }
            return true;
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
            si.PracUserPermission = "Prac_" + si.PracticeTIN + "_User";
            si.PracUserReadOnlyPermission = "Prac_" + si.PracticeTIN + "_ReadOnly";
            pmsi.IWNSiteMgrPermission = si.IWNRegion + "_SiteManager";
            pmsi.IWNSiteMgrReadOnlyPermission = si.IWNRegion + "_ReadOnly";

        }
    }
}

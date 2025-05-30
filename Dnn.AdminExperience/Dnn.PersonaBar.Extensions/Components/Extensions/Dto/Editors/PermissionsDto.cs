﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace Dnn.PersonaBar.Extensions.Components.Dto.Editors
{
    using Dnn.PersonaBar.Library.Dto;
    using Dnn.PersonaBar.Library.Helper;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Security.Permissions;
    using Newtonsoft.Json;

    [JsonObject]
    public class PermissionsDto : Permissions
    {
        public PermissionsDto(bool needDefinitions)
            : base(needDefinitions)
        {
            foreach (var role in PermissionProvider.Instance().ImplicitRolesForPages(PortalSettings.Current.PortalId))
            {
                this.EnsureRole(role, true, true);
            }
        }

        [JsonProperty("desktopModuleId")]
        public int DesktopModuleId { get; set; }

        /// <inheritdoc/>
        protected override void LoadPermissionDefinitions()
        {
            foreach (PermissionInfo permission in PermissionController.GetPermissionsByPortalDesktopModule())
            {
                this.PermissionDefinitions.Add(new Permission()
                {
                    PermissionId = permission.PermissionID,
                    PermissionName = permission.PermissionName,
                    FullControl = PermissionHelper.IsFullControl(permission),
                    View = PermissionHelper.IsViewPermisison(permission),
                });
            }
        }
    }
}

﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace Dnn.PersonaBar.Extensions.Components.Dto.Editors
{
    using DotNetNuke.Services.Installer.Packages;
    using Newtonsoft.Json;

    [JsonObject]
    public class AuthSystemPackageDetailDto : PackageInfoDto
    {
        /// <summary>Initializes a new instance of the <see cref="AuthSystemPackageDetailDto"/> class.</summary>
        public AuthSystemPackageDetailDto()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="AuthSystemPackageDetailDto"/> class.</summary>
        /// <param name="portalId">The portal ID.</param>
        /// <param name="package">The package info.</param>
        public AuthSystemPackageDetailDto(int portalId, PackageInfo package)
            : base(portalId, package)
        {
        }

        [JsonProperty("authenticationType")]
        public string AuthenticationType { get; set; }

        [JsonProperty("settingUrl")]
        public string SettingUrl { get; set; }

        [JsonProperty("loginControlSource")]
        public string LoginControlSource { get; set; }

        [JsonProperty("logoffControlSource")]
        public string LogoffControlSource { get; set; }

        [JsonProperty("settingsControlSource")]
        public string SettingsControlSource { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        // special extension roperties
        [JsonProperty("appId")]
        public string AppId { get; set; }

        [JsonProperty("appSecret")]
        public string AppSecret { get; set; }

        [JsonProperty("appEnabled")]
        public bool AppEnabled { get; set; }
    }
}

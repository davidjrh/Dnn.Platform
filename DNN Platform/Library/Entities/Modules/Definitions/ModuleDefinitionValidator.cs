﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Entities.Modules.Definitions
{
    using System;
    using System.IO;
    using System.Web;
    using System.Xml;

    using DotNetNuke.Common;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Services.Localization;

    public enum ModuleDefinitionVersion
    {
        /// <summary>An unknown version.</summary>
        VUnknown = 0,

        /// <summary>Version one.</summary>
        V1 = 1,

        /// <summary>Version two.</summary>
        V2 = 2,

        /// <summary>Version two of a skin.</summary>
        V2_Skin = 3,

        /// <summary>Version two of a provider.</summary>
        V2_Provider = 4,

        /// <summary>Version three.</summary>
        V3 = 5,
    }

    public class ModuleDefinitionValidator : XmlValidatorBase
    {
        public ModuleDefinitionVersion GetModuleDefinitionVersion(Stream xmlStream)
        {
            ModuleDefinitionVersion retValue;
            xmlStream.Seek(0, SeekOrigin.Begin);
            var xmlReader = new XmlTextReader(xmlStream)
            {
                XmlResolver = null,
                DtdProcessing = DtdProcessing.Prohibit,
            };
            xmlReader.MoveToContent();

            // This test assumes provides a simple validation
            switch (xmlReader.LocalName.ToLowerInvariant())
            {
                case "module":
                    retValue = ModuleDefinitionVersion.V1;
                    break;
                case "dotnetnuke":
                    switch (xmlReader.GetAttribute("type"))
                    {
                        case "Module":
                            switch (xmlReader.GetAttribute("version"))
                            {
                                case "2.0":
                                    retValue = ModuleDefinitionVersion.V2;
                                    break;
                                case "3.0":
                                    retValue = ModuleDefinitionVersion.V3;
                                    break;
                                default:
                                    return ModuleDefinitionVersion.VUnknown;
                            }

                            break;
                        case "SkinObject":
                            retValue = ModuleDefinitionVersion.V2_Skin;
                            break;
                        case "Provider":
                            retValue = ModuleDefinitionVersion.V2_Provider;
                            break;
                        default:
                            retValue = ModuleDefinitionVersion.VUnknown;
                            break;
                    }

                    break;
                default:
                    retValue = ModuleDefinitionVersion.VUnknown;
                    break;
            }

            return retValue;
        }

        /// <inheritdoc/>
        public override bool Validate(Stream xmlStream)
        {
            this.SchemaSet.Add(string.Empty, this.GetDnnSchemaPath(xmlStream));
            return base.Validate(xmlStream);
        }

        private static string GetLocalizedString(string key)
        {
            var objPortalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            if (objPortalSettings == null)
            {
                return key;
            }

            return Localization.GetString(key, objPortalSettings);
        }

        private string GetDnnSchemaPath(Stream xmlStream)
        {
            ModuleDefinitionVersion version = this.GetModuleDefinitionVersion(xmlStream);
            string schemaPath = string.Empty;
            switch (version)
            {
                case ModuleDefinitionVersion.V2:
                    schemaPath = "components\\ResourceInstaller\\ModuleDef_V2.xsd";
                    break;
                case ModuleDefinitionVersion.V3:
                    schemaPath = "components\\ResourceInstaller\\ModuleDef_V3.xsd";
                    break;
                case ModuleDefinitionVersion.V2_Skin:
                    schemaPath = "components\\ResourceInstaller\\ModuleDef_V2Skin.xsd";
                    break;
                case ModuleDefinitionVersion.V2_Provider:
                    schemaPath = "components\\ResourceInstaller\\ModuleDef_V2Provider.xsd";
                    break;
                case ModuleDefinitionVersion.VUnknown:
                    throw new Exception(GetLocalizedString("EXCEPTION_LoadFailed"));
            }

            return Path.Combine(Globals.ApplicationMapPath, schemaPath);
        }
    }
}

﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.UI.Modules
{
    using System.Web.UI;

    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Framework;

    public class ReflectedModuleControlFactory : BaseModuleControlFactory
    {
        /// <inheritdoc/>
        public override int Priority => -1;

        /// <inheritdoc/>
        public override bool SupportsControl(ModuleInfo moduleConfiguration, string controlSrc)
        {
            // This factory is used as fallback
            return true;
        }

        /// <inheritdoc/>
        public override Control CreateControl(TemplateControl containerControl, string controlKey, string controlSrc)
        {
            // load from a typename in an assembly ( ie. server control)
            var objType = Reflection.CreateType(controlSrc);
            return containerControl.LoadControl(objType, null);
        }

        /// <inheritdoc/>
        public override Control CreateModuleControl(TemplateControl containerControl, ModuleInfo moduleConfiguration)
        {
            return this.CreateControl(containerControl, string.Empty, moduleConfiguration.ModuleControl.ControlSrc);
        }

        /// <inheritdoc/>
        public override Control CreateSettingsControl(TemplateControl containerControl, ModuleInfo moduleConfiguration, string controlSrc)
        {
            return this.CreateControl(containerControl, string.Empty, controlSrc);
        }
    }
}

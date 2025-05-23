﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Modules.Admin.Users
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.UI.Utilities;

    /// <summary>A settings control for the profile module.</summary>
    public partial class ViewProfileSettings : ModuleSettingsBase
    {
        /// <inheritdoc/>
        public override void LoadSettings()
        {
            try
            {
                ClientAPI.AddButtonConfirm(this.cmdLoadDefault, Localization.GetString("LoadDefault.Confirm", this.LocalResourceFile));
                this.cmdLoadDefault.ToolTip = Localization.GetString("LoadDefault.Help", this.LocalResourceFile);

                if (!this.Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty((string)this.TabModuleSettings["ProfileTemplate"]))
                    {
                        this.txtTemplate.Text = (string)this.TabModuleSettings["ProfileTemplate"];
                    }

                    if (this.Settings.ContainsKey("IncludeButton"))
                    {
                        this.IncludeButton.Checked = Convert.ToBoolean(this.Settings["IncludeButton"]);
                    }
                }
            }
            catch (Exception exc)
            {
                // Module failed to load
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <inheritdoc/>
        public override void UpdateSettings()
        {
            try
            {
                ModuleController.Instance.UpdateTabModuleSetting(this.TabModuleId, "ProfileTemplate", this.txtTemplate.Text);
                ModuleController.Instance.UpdateTabModuleSetting(this.TabModuleId, "IncludeButton", this.IncludeButton.Checked.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception exc)
            {
                // Module failed to load
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <inheritdoc/>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.cmdLoadDefault.Click += this.cmdLoadDefault_Click;
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Breaking Change")]

        // ReSharper disable once InconsistentNaming
        protected void cmdLoadDefault_Click(object sender, EventArgs e)
        {
            this.txtTemplate.Text = Localization.GetString("DefaultTemplate", this.LocalResourceFile);
        }
    }
}

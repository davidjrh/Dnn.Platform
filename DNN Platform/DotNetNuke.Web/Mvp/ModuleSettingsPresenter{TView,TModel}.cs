﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Internal.SourceGenerators;

    /// <summary>Represents a class that is a presenter for a settings controlin a Web Forms Model-View-Presenter application.</summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    [DnnDeprecated(9, 2, 0, "Replace WebFormsMvp and DotNetNuke.Web.Mvp with MVC or SPA patterns instead")]
    public abstract partial class ModuleSettingsPresenter<TView, TModel> : ModulePresenterBase<TView>
        where TView : class, ISettingsView<TModel>
        where TModel : SettingsModel, new()
    {
        /// <summary>Initializes a new instance of the <see cref="ModuleSettingsPresenter{TView, TModel}"/> class.</summary>
        /// <param name="view">The view.</param>
        protected ModuleSettingsPresenter(TView view)
            : base(view)
        {
            view.OnLoadSettings += this.OnLoadSettingsInternal;
            view.OnSaveSettings += this.OnSaveSettingsInternal;
        }

        /// <inheritdoc/>
        protected override void OnLoad()
        {
            base.OnLoad();

            if (this.IsPostBack)
            {
                // Initialize dictionaries as LoadSettings is not called on Postback
                this.View.Model.ModuleSettings = new Dictionary<string, string>();
                this.View.Model.TabModuleSettings = new Dictionary<string, string>();
            }
        }

        protected virtual void LoadSettings()
        {
            this.View.Model.ModuleSettings = new Dictionary<string, string>(
                                            this.ModuleContext.Configuration.ModuleSettings
                                            .Cast<DictionaryEntry>()
                                            .ToDictionary(kvp => (string)kvp.Key, kvp => (string)kvp.Value));

            this.View.Model.TabModuleSettings = new Dictionary<string, string>(
                                            this.ModuleContext.Configuration.TabModuleSettings
                                            .Cast<DictionaryEntry>()
                                            .ToDictionary(kvp => (string)kvp.Key, kvp => (string)kvp.Value));
        }

        protected virtual void SaveSettings()
        {
            var controller = ModuleController.Instance;

            foreach (var setting in this.View.Model.ModuleSettings)
            {
                ModuleController.Instance.UpdateModuleSetting(this.ModuleId, setting.Key, setting.Value);
            }

            foreach (var setting in this.View.Model.TabModuleSettings)
            {
                ModuleController.Instance.UpdateTabModuleSetting(this.ModuleContext.TabModuleId, setting.Key, setting.Value);
            }
        }

        private void OnLoadSettingsInternal(object sender, EventArgs e)
        {
            this.LoadSettings();
        }

        private void OnSaveSettingsInternal(object sender, EventArgs e)
        {
            this.SaveSettings();
        }
    }
}

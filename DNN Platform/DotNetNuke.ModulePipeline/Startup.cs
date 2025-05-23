﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.ModulePipeline
{
    using DotNetNuke.DependencyInjection;
    using DotNetNuke.UI.Modules;
    using Microsoft.Extensions.DependencyInjection;

    /// <inheritdoc/>
    public class Startup : IDnnStartup
    {
        /// <inheritdoc/>
        public void ConfigureServices(IServiceCollection services)
        {
            // MULTI-TARGETTING PIPELINE
            // -------------------------
            // This file multi-targets .NET Framework and .NET Standard,
            // which is needed as DNN migrates to .NET Core. The 'NETFRAMEWORK'
            // pre-processor directives are to fully support Legacy DNN.
            // As the Pipeline is upgraded to be more complaint with
            // .NET Standard 2.0 use the appropriate pre-processor directives.
#if NETFRAMEWORK
            services.AddSingleton<IModuleControlPipeline, ModuleControlPipeline>();
#endif
        }
    }
}

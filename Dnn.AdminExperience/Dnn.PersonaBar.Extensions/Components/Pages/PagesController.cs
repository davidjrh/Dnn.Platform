﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace Dnn.PersonaBar.Pages.Components
{
    using System;

    using DotNetNuke.Common;
    using DotNetNuke.Framework;
    using DotNetNuke.Internal.SourceGenerators;

    using Microsoft.Extensions.DependencyInjection;

    /// <inheritdoc cref="IPagesController"/>
    [DnnDeprecated(10, 0, 0, "Please resolve IPagesController via dependency injection.")]
    public partial class PagesController : ServiceLocator<IPagesController, PagesController>
    {
        /// <inheritdoc/>
        protected override Func<IPagesController> GetFactory()
        {
            return Globals.DependencyProvider.GetRequiredService<IPagesController>;
        }
    }
}

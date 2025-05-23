﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Build.Tasks;

/// <summary>A cake task to generate the ASP.NET MVC package.</summary>
public sealed class PackageAspNetMvc : PackageComponentTask
{
    /// <summary>Initializes a new instance of the <see cref="PackageAspNetMvc"/> class.</summary>
    public PackageAspNetMvc()
        : base("AspNetMvc", "System.Web.Mvc.dll", "Microsoft.AspNetMvc")
    {
    }
}

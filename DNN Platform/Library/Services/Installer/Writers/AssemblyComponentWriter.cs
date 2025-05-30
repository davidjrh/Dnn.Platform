﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Services.Installer.Writers
{
    using System.Collections.Generic;

    using DotNetNuke.Services.Installer.Packages;

    /// <summary>
    /// The AssemblyComponentWriter class handles creating the manifest for Assembly
    /// Component(s).
    /// </summary>
    public class AssemblyComponentWriter : FileComponentWriter
    {
        /// <summary>Initializes a new instance of the <see cref="AssemblyComponentWriter"/> class.</summary>
        /// <param name="basePath">The assembly file base path.</param>
        /// <param name="assemblies">The assembly files.</param>
        /// <param name="package">The package info.</param>
        public AssemblyComponentWriter(string basePath, Dictionary<string, InstallFile> assemblies, PackageInfo package)
            : base(basePath, assemblies, package)
        {
        }

        /// <summary>Gets the name of the Collection Node ("assemblies").</summary>
        /// <value>A String.</value>
        protected override string CollectionNodeName
        {
            get
            {
                return "assemblies";
            }
        }

        /// <summary>Gets the name of the Component Type ("Assembly").</summary>
        /// <value>A String.</value>
        protected override string ComponentType
        {
            get
            {
                return "Assembly";
            }
        }

        /// <summary>Gets the name of the Item Node ("assembly").</summary>
        /// <value>A String.</value>
        protected override string ItemNodeName
        {
            get
            {
                return "assembly";
            }
        }
    }
}

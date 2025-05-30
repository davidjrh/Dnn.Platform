﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Services.FileSystem.FolderMappings
{
    using System.Collections.Generic;

    public interface IFolderMappingsConfigController
    {
        /// <summary>Gets all folder types read from folderMappings config file.</summary>
        IList<FolderTypeConfig> FolderTypes { get; }

        /// <summary>Gets root node in folderMappings config file.</summary>
        string ConfigNode { get; }

        /// <summary>Load data from folderMappings config file.</summary>
        void LoadConfig();

        /// <summary>Save data in folderMappings config file.</summary>
        /// <param name="folderMappinsSettings">The folder mapping settings XML.</param>
        void SaveConfig(string folderMappinsSettings);

        /// <summary>Gets the folderMapping configured for a specific folder.</summary>
        /// <param name="portalId">Portal ID where the folder is.</param>
        /// <param name="folderPath">Specific folder path.</param>
        /// <returns>A <see cref="FolderMappingInfo"/> instance or <see langword="null"/>.</returns>
        FolderMappingInfo GetFolderMapping(int portalId, string folderPath);
    }
}

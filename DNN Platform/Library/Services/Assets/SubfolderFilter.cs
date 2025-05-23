﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Services.Assets
{
    public enum SubfolderFilter
    {
        /// <summary>Exclude subfolders.</summary>
        ExcludeSubfolders = 0,

        /// <summary>Include files of the subfolder only.</summary>
        IncludeSubfoldersFilesOnly = 1,

        /// <summary>Include folders within the subfolder.</summary>
        IncludeSubfoldersFolderStructure = 2,
    }
}

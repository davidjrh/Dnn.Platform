﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace Dnn.PersonaBar.Library
{
    /// <summary>The scope of a menu item.</summary>
    public enum ServiceScope
    {
        /// <summary>the service available for all users.</summary>
        Regular = 0,

        /// <summary>the service only available for admin users.</summary>
        Admin = 1,

        /// <summary>the service only available for host users.</summary>
        Host = 2,
    }
}

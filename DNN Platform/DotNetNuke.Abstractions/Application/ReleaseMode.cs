﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Abstractions.Application
{
    /// <summary>The enumeration of release mode.</summary>
    /// <value>
    /// <list type="bullet">
    ///         <item>None: Not specified for the current release.</item>
    ///         <item>Alpha:Alpha release is an opportunity for customers to get an early look at a particular software feature.</item>
    ///         <item>Beta: Beta release is a mostly completed release,
    ///                 At this point we will have implemented most of the major features planned for a specific release. </item>
    ///         <item>RC: RC release will be the Stable release if there is no major show-stopping bugs,
    ///                 We have gone through all the major test scenarios and are just running through a final set of regression
    ///                 tests and verifying the packaging.</item>
    ///         <item>Stable: Stable release is believed to be ready for use,
    ///                 remember that only stable release can be used in production environment.</item>
    /// </list>
    /// </value>
    public enum ReleaseMode
    {
        /// <summary>Not assigned.</summary>
        None = 0,

        /// <summary>Alpha release.</summary>
        Alpha = 1,

        /// <summary>Beta release.</summary>
        Beta = 2,

        /// <summary>Release candidate.</summary>
        RC = 3,

        /// <summary>Stable release version.</summary>
        Stable = 4,
    }
}

﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Services.ClientCapability
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public interface IClientCapabilityProvider
    {
        /// <summary>Returns ClientCapability based on userAgent.</summary>
        /// <param name="userAgent">The user agent.</param>
        /// <returns>An <see cref="IClientCapability"/> instance.</returns>
        IClientCapability GetClientCapability(string userAgent);

        /// <summary>Returns ClientCapability based on ClientCapabilityId.</summary>
        /// <param name="clientId">The client ID.</param>
        /// <returns>An <see cref="IClientCapability"/> instance.</returns>
        IClientCapability GetClientCapabilityById(string clientId);

        /// <summary>Returns available Capability Values for every Capability Name.</summary>
        /// <returns>Dictionary of Capability Name along with List of possible values of the Capability.</returns>
        /// <example>Capability Name = mobile_browser, value = Safari, Android Webkit.</example>
        IDictionary<string, List<string>> GetAllClientCapabilityValues();

        /// <summary>Returns All available Client Capabilities present.</summary>
        /// <returns>List of IClientCapability present.</returns>
        IQueryable<IClientCapability> GetAllClientCapabilities();

        /// <summary>Returns ClientCapability based on HttpRequest.</summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>An <see cref="IClientCapability"/> instance.</returns>
        IClientCapability GetClientCapability(HttpRequest httpRequest);
    }
}

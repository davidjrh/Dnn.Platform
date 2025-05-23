﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Modules.Journal.Components
{
    using System.Globalization;

    using DotNetNuke.Common;
    using DotNetNuke.Entities.Users;
    using DotNetNuke.Services.Tokens;

    public class ProfilePicPropertyAccess : IPropertyAccess
    {
        private readonly int userId;

        /// <summary>Initializes a new instance of the <see cref="ProfilePicPropertyAccess"/> class.</summary>
        /// <param name="userId">The user ID.</param>
        public ProfilePicPropertyAccess(int userId)
        {
            this.userId = userId;
        }

        /// <inheritdoc/>
        public CacheLevel Cacheability => CacheLevel.notCacheable;

        public int Size { get; set; } = 32;

        /// <inheritdoc/>
        public string GetProperty(string propertyName, string format, CultureInfo formatProvider, UserInfo accessingUser, Scope currentScope, ref bool propertyNotFound)
        {
            if (propertyName.ToLowerInvariant() == "relativeurl")
            {
                int size;
                if (int.TryParse(format, out size))
                {
                    this.Size = size;
                }

                return UserController.Instance.GetUserProfilePictureUrl(this.userId, this.Size, this.Size);
            }

            propertyNotFound = true;
            return string.Empty;
        }
    }
}

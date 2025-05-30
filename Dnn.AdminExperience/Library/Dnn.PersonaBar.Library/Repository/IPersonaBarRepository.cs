﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace Dnn.PersonaBar.Library.Repository
{
    using Dnn.PersonaBar.Library.Model;

    /// <summary>Interface responsible to get the Persona Bar menu structure from the data layer.</summary>
    public interface IPersonaBarRepository
    {
        /// <summary>Gets the menu structure of the persona bar.</summary>
        /// <returns>Persona bar menu structure.</returns>
        PersonaBarMenu GetMenu();

        /// <summary>Save menu item info.</summary>
        /// <param name="item">The menu item.</param>
        void SaveMenuItem(MenuItem item);

        /// <summary>remove a menu item.</summary>
        /// <param name="identifier">The menu item ID.</param>
        void DeleteMenuItem(string identifier);

        /// <summary>Get the menu item by identifier.</summary>
        /// <param name="identifier">The menu item ID.</param>
        /// <returns>A <see cref="MenuItem"/> instance or <see langword="null"/>.</returns>
        MenuItem GetMenuItem(string identifier);

        /// <summary>Get the menu item by menu id.</summary>
        /// <param name="menuId">The menu ID.</param>
        /// <returns>A <see cref="MenuItem"/> instance or <see langword="null"/>.</returns>
        MenuItem GetMenuItem(int menuId);

        /// <summary>Get a menu item's default allowed permissions.</summary>
        /// <param name="menuId">The menu ID.</param>
        /// <returns>The permissions string.</returns>
        string GetMenuDefaultPermissions(int menuId);

        /// <summary>Save a menu item's default allowed permissions.</summary>
        /// <param name="menuItem">The menu item.</param>
        /// <param name="roleNames">The default roles allowed to view the menu item.</param>
        void SaveMenuDefaultPermissions(MenuItem menuItem, string roleNames);

        void UpdateMenuController(string identifier, string controller);
    }
}

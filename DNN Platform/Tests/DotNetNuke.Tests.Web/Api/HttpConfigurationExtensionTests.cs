﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Tests.Web.Api
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using DotNetNuke.Web.Api;
    using NUnit.Framework;

    [TestFixture]
    public class HttpConfigurationExtensionTests
    {
        [Test]
        public void GetTabAndModuleInfoProvidersReturnsEmptyWhenNoProvidersAdded()
        {
            // Arrange
            var configuration = new HttpConfiguration();

            // Act
            var providers = configuration.GetTabAndModuleInfoProviders();

            // Assert
            Assert.That(providers, Is.Empty);
        }

        [Test]
        public void AddTabAndModuleInfoProviderWorksForFirstProvider()
        {
            // Arrange
            var configuration = new HttpConfiguration();

            // Act
            configuration.AddTabAndModuleInfoProvider(new StandardTabAndModuleInfoProvider());

            // Assert
            Assert.That(((IEnumerable<ITabAndModuleInfoProvider>)configuration.Properties["TabAndModuleInfoProvider"]).Count(), Is.EqualTo(1));
        }

        [Test]
        public void AddTabAndModuleInfoProviderWorksForManyProviders()
        {
            // Arrange
            var configuration = new HttpConfiguration();

            // Act
            configuration.AddTabAndModuleInfoProvider(new StandardTabAndModuleInfoProvider());
            configuration.AddTabAndModuleInfoProvider(new StandardTabAndModuleInfoProvider());
            configuration.AddTabAndModuleInfoProvider(new StandardTabAndModuleInfoProvider());

            // Assert
            Assert.That(((IEnumerable<ITabAndModuleInfoProvider>)configuration.Properties["TabAndModuleInfoProvider"]).Count(), Is.EqualTo(3));
        }
    }
}

﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Tests.Web.Mvc.Framework.Modules
{
    using System;
    using System.IO;
    using System.Web.Mvc;

    using DotNetNuke.Tests.Utilities.Fakes;
    using DotNetNuke.Web.Mvc.Framework.ActionResults;
    using DotNetNuke.Web.Mvc.Framework.Modules;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ModuleExecutionEngineTests
    {
        private FakeServiceProvider serviceProvider;

        [SetUp]
        public void Setup()
        {
            this.serviceProvider = FakeServiceProvider.Setup(
                services =>
                {
                    services.AddSingleton<IControllerFactory, DefaultControllerFactory>();
                });
        }

        [TearDown]
        public void TearDown()
        {
            this.serviceProvider.Dispose();
        }

        [Test]
        public void ExecuteModule_Requires_NonNull_ModuleRequestContext()
        {
            var engine = new ModuleExecutionEngine();
            Assert.Throws<ArgumentNullException>(() => engine.ExecuteModule(null));
        }

        [Test]
        public void ExecuteModule_Returns_Null_If_Application_Is_Null()
        {
            // Arrange
            var engine = new ModuleExecutionEngine();
            var requestContext = new ModuleRequestContext() { ModuleApplication = null };

            // Act
            ModuleRequestResult result = engine.ExecuteModule(requestContext);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ExecuteModule_Executes_ModuleApplication_If_Not_Null()
        {
            // Arrange
            var engine = new ModuleExecutionEngine();

            var moduleApp = new Mock<ModuleApplication>();
            var requestContext = new ModuleRequestContext() { ModuleApplication = moduleApp.Object };

            // Act
            engine.ExecuteModule(requestContext);

            // Assert
            moduleApp.Verify(app => app.ExecuteRequest(It.IsAny<ModuleRequestContext>()));
        }

        [Test]
        public void ExecuteModule_Returns_Result_Of_Executing_ModuleApplication()
        {
            // Arrange
            var engine = new ModuleExecutionEngine();
            var expected = new ModuleRequestResult();

            var moduleApp = new Mock<ModuleApplication>();
            moduleApp.Setup(app => app.ExecuteRequest(It.IsAny<ModuleRequestContext>()))
                    .Returns(expected);
            var requestContext = new ModuleRequestContext() { ModuleApplication = moduleApp.Object };

            // Act
            ModuleRequestResult actual = engine.ExecuteModule(requestContext);

            // Assert
            Assert.That(actual, Is.SameAs(expected));
        }

        [Test]
        public void ExecuteModuleResult_Calls_IDnnViewResult_ExecuteResult()
        {
            // Arrange
            var engine = new ModuleExecutionEngine();

            var actionResultMock = new Mock<ActionResult>();

            var viewResultMock = actionResultMock.As<IDnnViewResult>();

            var controllerContext = MockHelper.CreateMockControllerContext();
            var moduleRequestResult = new ModuleRequestResult
            {
                ActionResult = actionResultMock.Object,
                ControllerContext = controllerContext,
            };

            // Act
            engine.ExecuteModuleResult(moduleRequestResult, new StringWriter());

            // Arrange
            viewResultMock.Verify(v => v.ExecuteResult(It.IsAny<ControllerContext>(), It.IsAny<TextWriter>()));
        }

        [Test]
        public void ExecuteModuleResult_Calls_IDnnViewResult_ExecuteResult_With_ModuleRequestResult_ControllerContext()
        {
            // Arrange
            var engine = new ModuleExecutionEngine();

            var actionResultMock = new Mock<ActionResult>();

            var viewResultMock = actionResultMock.As<IDnnViewResult>();

            var controllerContext = MockHelper.CreateMockControllerContext();
            var moduleRequestResult = new ModuleRequestResult
            {
                ActionResult = actionResultMock.Object,
                ControllerContext = controllerContext,
            };

            // Act
            engine.ExecuteModuleResult(moduleRequestResult, new StringWriter());

            // Arrange
            viewResultMock.Verify(v => v.ExecuteResult(controllerContext, It.IsAny<TextWriter>()));
        }
    }
}

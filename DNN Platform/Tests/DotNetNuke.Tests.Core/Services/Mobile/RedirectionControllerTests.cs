﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Tests.Core.Services.Mobile
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Reflection;
    using System.Web;

    using DotNetNuke.Abstractions;
    using DotNetNuke.Abstractions.Application;
    using DotNetNuke.Abstractions.Logging;
    using DotNetNuke.Abstractions.Modules;
    using DotNetNuke.Common.Internal;
    using DotNetNuke.ComponentModel;
    using DotNetNuke.Data;
    using DotNetNuke.Entities.Controllers;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Security.Roles;
    using DotNetNuke.Services.Cache;
    using DotNetNuke.Services.ClientCapability;
    using DotNetNuke.Services.Mobile;
    using DotNetNuke.Tests.Core.Services.ClientCapability;
    using DotNetNuke.Tests.Instance.Utilities;
    using DotNetNuke.Tests.Utilities.Fakes;
    using DotNetNuke.Tests.Utilities.Mocks;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    /// <summary>  Summary description for RedirectionControllerTests.</summary>
    [TestFixture]
    public class RedirectionControllerTests
    {
        public const string iphoneUserAgent = "Mozilla/5.0 (iPod; U; CPU iPhone OS 4_0 like Mac OS X; en-us) AppleWebKit/532.9 (KHTML, like Gecko) Version/4.0.5 Mobile/8A293 Safari/6531.22.7";
        public const string wp7UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows Phone OS 7.0; Trident/3.1; IEMobile/7.0) Asus;Galaxy6";
        public const string msIE8UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; InfoPath.3; Creative AutoUpdate v1.40.02)";
        public const string msIE9UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
        public const string msIE10UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
        public const string fireFox5NT61UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:5.0) Gecko/20110619 Firefox/5.0";
        public const string iPadTabletUserAgent = "Mozilla/5.0 (iPad; U; CPU OS 3_2 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Version/4.0.4 Mobile/7B334b Safari/531.21.10";
        public const string samsungGalaxyTablet = "Mozilla/5.0 (Linux; U; Android 2.2; en-gb; SAMSUNG GT-P1000 Tablet Build/MASTER) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1";
        public const string winTabletPC = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705; Tablet PC 2.0)";
        public const string htcDesireVer1Sub22UserAgent = "Mozilla/5.0 (Linux; U; Android 2.2; sv-se; Desire_A8181 Build/FRF91) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1";
        public const string blackBerry9105V1 = "BlackBerry9105/5.0.0.696 Profile/MIDP-2.1 Configuration/CLDC-1.1 VendorID/133";
        public const string motorolaRIZRSymbianOSOpera865 = "MOTORIZR-Z8/46.00.00 Mozilla/4.0 (compatible; MSIE 6.0; Symbian OS; 356) Opera 8.65 [it] UP.Link/6.3.0.0.0";

        public const int Portal0 = 0;
        public const int Portal1 = 1;
        public const int Portal2 = 2;
        public const int Page1 = 1;
        public const int Page2 = 2;
        public const int Page3 = 3;
        public const int SortOrder1 = 1;
        public const int SortOrder2 = 1;
        public const int SortOrder3 = 1;
        public const string PortalAlias0 = "www.portal0.com";
        public const string PortalAlias1 = "www.portal1.com";
        public const int AnotherPageOnSamePortal = 56;
        public const int DeletedPageOnSamePortal = 59;
        public const int DeletedPageOnSamePortal2 = 94;
        public const int HomePageOnPortal0 = 55;
        public const int HomePageOnPortal1 = 57;
        public const int MobileLandingPage = 91;
        public const int TabletLandingPage = 92;
        public const int AllMobileLandingPage = 93;
        public const bool EnabledFlag = true;
        public const bool DisabledFlag = false;
        public const bool IncludeChildTabsFlag = true;
        public const string ExternalSite = "https://dnncommunity.org";

        private const string DisableMobileRedirectCookieName = "disablemobileredirect";
        private const string DisableRedirectPresistCookieName = "disableredirectpresist";
        private const string DisableMobileRedirectQueryStringName = "nomo";

        private Mock<DataProvider> dataProvider;
        private RedirectionController redirectionController;
        private Mock<ClientCapabilityProvider> clientCapabilityProvider;
        private Mock<IHostController> mockHostController;
        private FakeServiceProvider serviceProvider;

        private DataTable dtRedirections;
        private DataTable dtRules;

        [SetUp]

        public void SetUp()
        {
            ComponentFactory.Container = new SimpleContainer();
            UnitTestHelper.ClearHttpContext();
            this.dataProvider = MockComponentProvider.CreateDataProvider();
            MockComponentProvider.CreateDataCacheProvider();
            MockComponentProvider.CreateEventLogController();
            this.clientCapabilityProvider = MockComponentProvider.CreateNew<ClientCapabilityProvider>();
            this.mockHostController = new Mock<IHostController>();
            this.mockHostController.As<IHostSettingsService>();
            HostController.RegisterInstance(this.mockHostController.Object);
            this.SetupContainer();

            this.redirectionController = new RedirectionController(new PortalController(Mock.Of<IBusinessControllerProvider>()), Mock.Of<IEventLogger>());

            this.SetupDataProvider();
            this.SetupClientCapabilityProvider();
            this.SetupRoleProvider();

            var tabController = TabController.Instance;
            var dataProviderField = tabController.GetType().GetField("dataProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            if (dataProviderField != null)
            {
                dataProviderField.SetValue(tabController, this.dataProvider.Object);
            }
        }

        [TearDown]
        public void TearDown()
        {
            this.serviceProvider.Dispose();
            TestableGlobals.ClearInstance();
            PortalController.ClearInstance();
            CachingProvider.Instance().PurgeCache();
            MockComponentProvider.ResetContainer();
            UnitTestHelper.ClearHttpContext();
            if (this.dtRedirections != null)
            {
                this.dtRedirections.Dispose();
                this.dtRedirections = null;
            }

            if (this.dtRules != null)
            {
                this.dtRules.Dispose();
                this.dtRules = null;
            }

            ComponentFactory.Container = null;
        }

        [Test]

        public void RedirectionController_Save_Valid_Redirection()
        {
            var redirection = new Redirection { Name = "Test R", PortalId = Portal0, SortOrder = 1, SourceTabId = -1, Type = RedirectionType.MobilePhone, TargetType = TargetType.Portal, TargetValue = Portal1 };
            this.redirectionController.Save(redirection);

            var dataReader = this.dataProvider.Object.GetRedirections(Portal0);
            var affectedCount = 0;
            while (dataReader.Read())
            {
                affectedCount++;
            }

            Assert.That(affectedCount, Is.EqualTo(1));
        }

        [Test]

        public void RedirectionController_Save_ValidRedirection_With_Rules()
        {
            var redirection = new Redirection { Name = "Test R", PortalId = Portal0, SortOrder = 1, SourceTabId = -1, IncludeChildTabs = true, Type = RedirectionType.Other, TargetType = TargetType.Portal, TargetValue = Portal1 };
            redirection.MatchRules.Add(new MatchRule { Capability = "Platform", Expression = "IOS" });
            redirection.MatchRules.Add(new MatchRule { Capability = "Version", Expression = "5" });
            this.redirectionController.Save(redirection);

            var dataReader = this.dataProvider.Object.GetRedirections(Portal0);
            var affectedCount = 0;
            while (dataReader.Read())
            {
                affectedCount++;
            }

            Assert.That(affectedCount, Is.EqualTo(1));

            var getRe = this.redirectionController.GetRedirectionsByPortal(Portal0)[0];
            Assert.That(getRe.MatchRules, Has.Count.EqualTo(2));
        }

        [Test]

        public void RedirectionController_GetRedirectionsByPortal_With_Valid_PortalID()
        {
            this.PrepareData();

            IList<IRedirection> list = this.redirectionController.GetRedirectionsByPortal(Portal0);

            Assert.That(list, Has.Count.EqualTo(7));
        }

        [Test]

        public void RedirectionController_Delete_With_ValidID()
        {
            this.PrepareData();
            this.redirectionController.Delete(Portal0, 1);

            IList<IRedirection> list = this.redirectionController.GetRedirectionsByPortal(Portal0);

            Assert.That(list, Has.Count.EqualTo(6));
        }

        [Test]

        public void RedirectionController_PurgeInvalidRedirections_DoNotPurgeRuleForNonDeletetedSource()
        {
            this.dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, SortOrder1, HomePageOnPortal0, IncludeChildTabsFlag, (int)TargetType.Tab, AnotherPageOnSamePortal, EnabledFlag);
            this.redirectionController.PurgeInvalidRedirections(0);
            Assert.That(this.redirectionController.GetRedirectionsByPortal(0), Has.Count.EqualTo(1));
        }

        [Test]

        public void RedirectionController_PurgeInvalidRedirections_DoPurgeRuleForDeletetedSource()
        {
            this.dtRedirections.Rows.Add(new object[] { 1, Portal0, "R1", (int)RedirectionType.MobilePhone, SortOrder1, DeletedPageOnSamePortal2, IncludeChildTabsFlag, (int)TargetType.Tab, AnotherPageOnSamePortal, EnabledFlag });
            this.redirectionController.PurgeInvalidRedirections(0);
            Assert.That(this.redirectionController.GetRedirectionsByPortal(0), Is.Empty);
        }

        [Test]

        public void RedirectionController_PurgeInvalidRedirections_DoPurgeRuleForDeletetedTargetPortal()
        {
            this.dtRedirections.Rows.Add(new object[] { 1, Portal0, "R1", (int)RedirectionType.MobilePhone, SortOrder1, HomePageOnPortal0, IncludeChildTabsFlag, (int)TargetType.Portal, Portal2, EnabledFlag });
            this.redirectionController.PurgeInvalidRedirections(0);
            Assert.That(this.redirectionController.GetRedirectionsByPortal(0), Is.Empty);
        }

        [Test]

        public void RedirectionController_PurgeInvalidRedirections_DoPurgeRuleForDeletetedTargetTab()
        {
            this.dtRedirections.Rows.Add(new object[] { 1, Portal0, "R1", (int)RedirectionType.MobilePhone, SortOrder1, HomePageOnPortal0, IncludeChildTabsFlag, (int)TargetType.Tab, DeletedPageOnSamePortal2, EnabledFlag });
            this.redirectionController.PurgeInvalidRedirections(0);
            Assert.That(this.redirectionController.GetRedirectionsByPortal(0), Is.Empty);
        }

        [Test]

        public void RedirectionController_GetRedirectionUrl_Throws_On_Null_UserAgent()
        {
            Assert.Throws<ArgumentException>(() => this.redirectionController.GetRedirectUrl(null, Portal0, 0));
        }

        [Test]

        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_Redirection_IsNotSet()
        {
            Assert.That(this.redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, HomePageOnPortal0), Is.EqualTo(string.Empty));
        }

        [Test]

        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_Redirection_IsNotEnabled()
        {
            this.PrepareSingleDisabledRedirectionRule();
            Assert.That(this.redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, HomePageOnPortal0), Is.EqualTo(string.Empty));
        }

        [Test]

        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_UserAgent_Is_Desktop()
        {
            this.PrepareData();
            Assert.That(this.redirectionController.GetRedirectUrl(msIE9UserAgent, Portal0, HomePageOnPortal0), Is.EqualTo(string.Empty));
        }

        [Test]

        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_CurrentPage_IsSameAs_TargetPage_OnMobile()
        {
            this.PreparePortalToAnotherPageOnSamePortal();
            Assert.That(this.redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, AnotherPageOnSamePortal), Is.EqualTo(string.Empty));
        }

        [Test]

        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_TargetPage_IsDeleted()
        {
            // prepare rule to a deleted tab on the same portal
            this.dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, AnotherPageOnSamePortal, EnabledFlag, (int)TargetType.Tab, DeletedPageOnSamePortal, 1);
            Assert.That(this.redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, AnotherPageOnSamePortal), Is.EqualTo(string.Empty));
        }

        [Test]

        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_CurrentPortal_IsSameAs_TargetPortal_OnMobile()
        {
            this.PrepareSamePortalToSamePortalRedirectionRule();
            Assert.That(this.redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, AnotherPageOnSamePortal), Is.EqualTo(string.Empty));
        }

        [Test]

        public void RedirectionController_GetRedirectionUrl_Returns_TargetPageOnSamePortal_When_Surfing_HomePage_OnMobile()
        {
            this.PreparePortalToAnotherPageOnSamePortal();
            Assert.That(this.redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 1), Is.EqualTo(this.NavigateUrl(AnotherPageOnSamePortal)));
        }

        // [Test]
        // public void RedirectionController_GetRedirectionUrl_Returns_HomePageOfOtherPortal_When_Surfing_AnyPageOfCurrentPortal_OnMobile()
        // {
        //    PrepareHomePageToHomePageRedirectionRule();
        //    Assert.AreEqual(DotNetNuke.Common.Globals.AddHTTP(PortalAlias1), redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 1));
        //    Assert.AreEqual(DotNetNuke.Common.Globals.AddHTTP(PortalAlias1), redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 2));
        // }
        [Test]

        public void RedirectionController_GetRedirectionUrl_Returns_ExternalSite_When_Surfing_AnyPageOfCurrentPortal_OnMobile()
        {
            this.PrepareExternalSiteRedirectionRule();
            Assert.Multiple(() =>
            {
                Assert.That(this.redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 1), Is.EqualTo(ExternalSite));
                Assert.That(this.redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 2), Is.EqualTo(ExternalSite));
            });
        }

        [Test]

        public void RedirectionController_GetRedirectionUrl_Returns_MobileLanding_ForMobile_And_TabletLanding_ForTablet()
        {
            this.PrepareMobileAndTabletRedirectionRuleWithMobileFirst();
            Assert.Multiple(() =>
            {
                Assert.That(this.redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 1), Is.EqualTo(this.NavigateUrl(MobileLandingPage)));
                Assert.That(this.redirectionController.GetRedirectUrl(iPadTabletUserAgent, Portal0, 1), Is.EqualTo(this.NavigateUrl(TabletLandingPage)));
            });
        }

        [Test]

        public void RedirectionController_GetRedirectionUrl_Returns_TabletLanding_ForTablet_And_MobileLanding_ForMobile()
        {
            this.PrepareMobileAndTabletRedirectionRuleWithAndTabletRedirectionRuleTabletFirst();
            Assert.Multiple(() =>
            {
                Assert.That(this.redirectionController.GetRedirectUrl(iphoneUserAgent, 0, 1), Is.EqualTo(this.NavigateUrl(MobileLandingPage)));
                Assert.That(this.redirectionController.GetRedirectUrl(iPadTabletUserAgent, 0, 1), Is.EqualTo(this.NavigateUrl(TabletLandingPage)));
            });
        }

        [Test]

        public void RedirectionController_GetRedirectionUrl_Returns_SameLandingPage_For_AllMobile()
        {
            this.PrepareAllMobileRedirectionRule();
            string mobileLandingPage = this.redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 1);
            string tabletLandingPage = this.redirectionController.GetRedirectUrl(iPadTabletUserAgent, Portal0, 1);
            Assert.Multiple(() =>
            {
                Assert.That(mobileLandingPage, Is.EqualTo(this.NavigateUrl(AllMobileLandingPage)));
                Assert.That(tabletLandingPage, Is.EqualTo(this.NavigateUrl(AllMobileLandingPage)));
            });
            Assert.That(tabletLandingPage, Is.EqualTo(mobileLandingPage));
        }

        [Test]

        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_Capability_DoesNot_Match()
        {
            this.PrepareOperaBrowserOnSymbianOSRedirectionRule();
            Assert.That(this.redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 1), Is.EqualTo(string.Empty));
        }

        [Test]

        public void RedirectionController_GetRedirectionUrl_Returns_ValidUrl_When_Capability_Matches()
        {
            this.PrepareOperaBrowserOnSymbianOSRedirectionRule();
            Assert.That(this.redirectionController.GetRedirectUrl(motorolaRIZRSymbianOSOpera865, Portal0, 1), Is.EqualTo(this.NavigateUrl(AnotherPageOnSamePortal)));
        }

        [Test]

        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_NotAll_Capability_Matches()
        {
            this.PrepareOperaBrowserOnIPhoneOSRedirectionRule();
            Assert.That(this.redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 1), Is.EqualTo(string.Empty));
        }

        [Test]

        public void RedirectionController_GetFullSiteUrl_With_NoRedirections()
        {
            var url = this.redirectionController.GetFullSiteUrl(Portal0, HomePageOnPortal0);

            Assert.That(url, Is.EqualTo(string.Empty));
        }

        // [Test]
        // public void RedirectionController_GetFullSiteUrl_When_Redirect_Between_Different_Portals()
        // {
        //    dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, -1, EnabledFlag, (int)TargetType.Portal, "1", 1);

        // var url = redirectionController.GetFullSiteUrl(Portal1, HomePageOnPortal1);

        // Assert.AreEqual(Globals.AddHTTP(PortalAlias0), url);
        // }

        // [Test]
        // public void RedirectionController_GetFullSiteUrl_When_Redirect_In_Same_Portal()
        // {
        //    dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, HomePageOnPortal0, EnabledFlag, (int)TargetType.Tab, AnotherPageOnSamePortal, 1);

        // var url = redirectionController.GetFullSiteUrl(Portal1, AnotherPageOnSamePortal);

        // //Assert.AreEqual(string.Empty, url);
        // }
        [Test]

        public void RedirectionController_GetFullSiteUrl_When_Redirect_To_DifferentUrl()
        {
            this.dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, HomePageOnPortal0, EnabledFlag, (int)TargetType.Url, ExternalSite, 1);

            var url = this.redirectionController.GetFullSiteUrl(Portal1, AnotherPageOnSamePortal);

            Assert.That(url, Is.EqualTo(string.Empty));
        }

        [Test]

        public void RedirectionController_GetMobileSiteUrl_With_NoRedirections()
        {
            var url = this.redirectionController.GetMobileSiteUrl(Portal0, HomePageOnPortal0);

            Assert.That(url, Is.EqualTo(string.Empty));
        }

        [Test]

        public void RedirectionController_GetMobileSiteUrl_Returns_Page_Specific_Url_When_Multiple_PageLevel_Redirects_Defined()
        {
            string redirectUrlPage1 = "m.yahoo.com";
            string redirectUrlPage2 = "m.cnn.com";

            // first page goes to one url
            this.dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, Page1, EnabledFlag, (int)TargetType.Url, redirectUrlPage1, 1);

            // second page goes to another url (this is Tablet - it should not matter)
            this.dtRedirections.Rows.Add(2, Portal0, "R2", (int)RedirectionType.Tablet, 2, Page2, EnabledFlag, (int)TargetType.Url, redirectUrlPage2, 1);

            var mobileUrlForPage1 = this.redirectionController.GetMobileSiteUrl(Portal0, Page1);
            var mobileUrlForPage2 = this.redirectionController.GetMobileSiteUrl(Portal0, Page2);
            var mobileUrlForPage3 = this.redirectionController.GetMobileSiteUrl(Portal0, Page3);

            Assert.Multiple(() =>
            {
                // First Page returns link to first url
                Assert.That(mobileUrlForPage1, Is.EqualTo(string.Format("{0}?nomo=0", redirectUrlPage1)));

                // Second Page returns link to second url
                Assert.That(mobileUrlForPage2, Is.EqualTo(string.Format("{0}?nomo=0", redirectUrlPage2)));

                // Third Page returns link to first url - as this is the first found url and third page has no redirect defined
                Assert.That(string.Format("{0}?nomo=0", redirectUrlPage1), Is.EqualTo(mobileUrlForPage3));
            });
        }

        // [Test]
        // public void RedirectionController_GetMobileSiteUrl_Works_When_Page_Redirects_To_Another_Portal()
        // {
        //    //first page goes to one second portal
        //    dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, -1, EnabledFlag, (int)TargetType.Portal, Portal1, 1);

        // var mobileUrlForPage1 = redirectionController.GetMobileSiteUrl(Portal0, Page1);

        // //First Page returns link to home page of other portal
        //    Assert.AreEqual(Globals.AddHTTP(PortalAlias1), mobileUrlForPage1);
        // }
        [Test]

        public void RedirectionController_GetMobileSiteUrl_When_Redirect_To_DifferentUrl()
        {
            this.dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, HomePageOnPortal0, EnabledFlag, (int)TargetType.Url, ExternalSite, 1);

            var url = this.redirectionController.GetMobileSiteUrl(Portal1, AnotherPageOnSamePortal);

            Assert.That(url, Is.EqualTo(string.Empty));
        }

        [Test]
        public void RedirectionController_IsRedirectAllowedForTheSession_In_Normal_Action()
        {
            var app = this.GenerateApplication();

            Assert.That(this.redirectionController.IsRedirectAllowedForTheSession(app), Is.True);
        }

        [Test]
        public void RedirectionController_IsRedirectAllowedForTheSession_With_Nonmo_Param_Set_To_1()
        {
            var app = this.GenerateApplication();
            app.Context.Request.QueryString.Add(DisableMobileRedirectQueryStringName, "1");

            Assert.Multiple(() =>
            {
                Assert.That(this.redirectionController.IsRedirectAllowedForTheSession(app), Is.False);
                Assert.That(app.Request.Cookies[DisableMobileRedirectCookieName], Is.Not.Null);
                Assert.That(app.Request.Cookies[DisableRedirectPresistCookieName], Is.Not.Null);
            });
        }

        [Test]
        public void RedirectionController_IsRedirectAllowedForTheSession_With_Nonmo_Param_Set_To_0()
        {
            var app = this.GenerateApplication();
            app.Context.Request.QueryString.Add(DisableMobileRedirectQueryStringName, "0");

            Assert.That(this.redirectionController.IsRedirectAllowedForTheSession(app), Is.True);
        }

        [Test]
        public void RedirectionController_IsRedirectAllowedForTheSession_With_Nonmo_Param_Set_To_1_And_Then_Setback_To_0()
        {
            var app = this.GenerateApplication();
            app.Context.Request.QueryString.Add(DisableMobileRedirectQueryStringName, "1");
            Assert.That(this.redirectionController.IsRedirectAllowedForTheSession(app), Is.False);

            app.Context.Request.QueryString.Add(DisableMobileRedirectQueryStringName, "0");
            Assert.That(this.redirectionController.IsRedirectAllowedForTheSession(app), Is.True);
        }

        private static IClientCapability GetClientCapabilityCallBack(string userAgent)
        {
            var clientCapability = new TestClientCapability();
            switch (userAgent)
            {
                case iphoneUserAgent:
                    clientCapability.IsMobile = true;
                    clientCapability.Properties.Add("mobile_browser", "Safari");
                    clientCapability.Properties.Add("device_os", "iPhone OS");
                    break;
                case iPadTabletUserAgent:
                    clientCapability.IsTablet = true;
                    clientCapability.Properties.Add("mobile_browser", "Safari");
                    clientCapability.Properties.Add("device_os", "iPhone OS");
                    break;
                case motorolaRIZRSymbianOSOpera865:
                    clientCapability.IsMobile = true;
                    clientCapability.Properties.Add("mobile_browser", "Opera Mini");
                    clientCapability.Properties.Add("device_os", "Symbian OS");
                    break;
            }

            return clientCapability;
        }

        private void SetupContainer()
        {
            var mockNavigationManager = new Mock<INavigationManager>();
            mockNavigationManager.Setup(x => x.NavigateURL(It.IsAny<int>())).Returns<int>(x => this.NavigateUrl(x));
            this.serviceProvider = FakeServiceProvider.Setup(
                services =>
                {
                    services.AddSingleton(mockNavigationManager.Object);
                    services.AddSingleton(this.dataProvider.Object);
                    services.AddSingleton(this.mockHostController.Object);
                    services.AddSingleton((IHostSettingsService)this.mockHostController.Object);
                    services.AddSingleton(this.clientCapabilityProvider.Object);
                });
        }

        private void SetupDataProvider()
        {
            this.dataProvider.Setup(d => d.GetProviderPath()).Returns(string.Empty);

            this.dtRedirections = new DataTable("Redirections");
            var pkCol = this.dtRedirections.Columns.Add("Id", typeof(int));
            this.dtRedirections.Columns.Add("PortalId", typeof(int));
            this.dtRedirections.Columns.Add("Name", typeof(string));
            this.dtRedirections.Columns.Add("Type", typeof(int));
            this.dtRedirections.Columns.Add("SortOrder", typeof(int));
            this.dtRedirections.Columns.Add("SourceTabId", typeof(int));
            this.dtRedirections.Columns.Add("IncludeChildTabs", typeof(bool));
            this.dtRedirections.Columns.Add("TargetType", typeof(int));
            this.dtRedirections.Columns.Add("TargetValue", typeof(object));
            this.dtRedirections.Columns.Add("Enabled", typeof(bool));

            this.dtRedirections.PrimaryKey = new[] { pkCol };

            this.dtRules = new DataTable("Rules");
            var pkCol1 = this.dtRules.Columns.Add("Id", typeof(int));
            this.dtRules.Columns.Add("RedirectionId", typeof(int));
            this.dtRules.Columns.Add("Capability", typeof(string));
            this.dtRules.Columns.Add("Expression", typeof(string));

            this.dtRules.PrimaryKey = new[] { pkCol1 };

            this.dataProvider.Setup(d =>
                                d.SaveRedirection(
                                    It.IsAny<int>(),
                                    It.IsAny<int>(),
                                    It.IsAny<string>(),
                                    It.IsAny<int>(),
                                    It.IsAny<int>(),
                                    It.IsAny<int>(),
                                    It.IsAny<bool>(),
                                    It.IsAny<int>(),
                                    It.IsAny<object>(),
                                    It.IsAny<bool>(),
                                    It.IsAny<int>())).Returns<int, int, string, int, int, int, bool, int, object, bool, int>(
                                                            (id, portalId, name, type, sortOrder, sourceTabId, includeChildTabs, targetType, targetValue, enabled, userId) =>
                                                            {
                                                                if (id == -1)
                                                                {
                                                                    if (this.dtRedirections.Rows.Count == 0)
                                                                    {
                                                                        id = 1;
                                                                    }
                                                                    else
                                                                    {
                                                                        id = Convert.ToInt32(this.dtRedirections.Select(string.Empty, "Id Desc")[0]["Id"]) + 1;
                                                                    }

                                                                    var row = this.dtRedirections.NewRow();
                                                                    row["Id"] = id;
                                                                    row["PortalId"] = portalId;
                                                                    row["name"] = name;
                                                                    row["type"] = type;
                                                                    row["sortOrder"] = sortOrder;
                                                                    row["sourceTabId"] = sourceTabId;
                                                                    row["includeChildTabs"] = includeChildTabs;
                                                                    row["targetType"] = targetType;
                                                                    row["targetValue"] = targetValue;
                                                                    row["enabled"] = enabled;

                                                                    this.dtRedirections.Rows.Add(row);
                                                                }
                                                                else
                                                                {
                                                                    var rows = this.dtRedirections.Select("Id = " + id);
                                                                    if (rows.Length == 1)
                                                                    {
                                                                        var row = rows[0];

                                                                        row["name"] = name;
                                                                        row["type"] = type;
                                                                        row["sortOrder"] = sortOrder;
                                                                        row["sourceTabId"] = sourceTabId;
                                                                        row["includeChildTabs"] = includeChildTabs;
                                                                        row["targetType"] = targetType;
                                                                        row["targetValue"] = targetValue;
                                                                        row["enabled"] = enabled;
                                                                    }
                                                                }

                                                                return id;
                                                            });

            this.dataProvider.Setup(d => d.GetRedirections(It.IsAny<int>())).Returns<int>(this.GetRedirectionsCallBack);
            this.dataProvider.Setup(d => d.DeleteRedirection(It.IsAny<int>())).Callback<int>((id) =>
            {
                var rows = this.dtRedirections.Select("Id = " + id);
                if (rows.Length == 1)
                {
                    this.dtRedirections.Rows.Remove(rows[0]);
                }
            });

            this.dataProvider.Setup(d => d.SaveRedirectionRule(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>())).Callback<int, int, string, string>((id, rid, capbility, expression) =>
                {
                    if (id == -1)
                    {
                        if (this.dtRules.Rows.Count == 0)
                        {
                            id = 1;
                        }
                        else
                        {
                            id = Convert.ToInt32(this.dtRules.Select(string.Empty, "Id Desc")[0]["Id"]) + 1;
                        }

                        var row = this.dtRules.NewRow();
                        row["Id"] = id;
                        row["RedirectionId"] = rid;
                        row["capability"] = capbility;
                        row["expression"] = expression;

                        this.dtRules.Rows.Add(row);
                    }
                    else
                    {
                        var rows = this.dtRules.Select("Id = " + id);
                        if (rows.Length == 1)
                        {
                            var row = rows[0];

                            row["capability"] = capbility;
                            row["expression"] = expression;
                        }
                    }
                });

            this.dataProvider.Setup(d => d.GetRedirectionRules(It.IsAny<int>())).Returns<int>(this.GetRedirectionRulesCallBack);
            this.dataProvider.Setup(d => d.DeleteRedirectionRule(It.IsAny<int>())).Callback<int>((id) =>
            {
                var rows = this.dtRules.Select("Id = " + id);
                if (rows.Length == 1)
                {
                    this.dtRules.Rows.Remove(rows[0]);
                }
            });

            this.dataProvider.Setup(d => d.GetPortals(It.IsAny<string>())).Returns<string>(this.GetPortalsCallBack);
            this.dataProvider.Setup(d => d.GetTabs(It.IsAny<int>())).Returns<int>(this.GetTabsCallBack);
            this.dataProvider.Setup(d => d.GetTab(It.IsAny<int>())).Returns<int>(this.GetTabCallBack);
            this.dataProvider.Setup(d => d.GetTabModules(It.IsAny<int>())).Returns<int>(this.GetTabModulesCallBack);
            this.dataProvider.Setup(d => d.GetPortalSettings(It.IsAny<int>(), It.IsAny<string>())).Returns<int, string>(this.GetPortalSettingsCallBack);
            this.dataProvider.Setup(d => d.GetAllRedirections()).Returns(this.GetAllRedirectionsCallBack);

            var portalDataService = MockComponentProvider.CreateNew<DotNetNuke.Entities.Portals.Data.IDataService>();
            portalDataService.Setup(p => p.GetPortalGroups()).Returns(this.GetPortalGroupsCallBack);
        }

        private void SetupClientCapabilityProvider()
        {
            this.clientCapabilityProvider.Setup(p => p.GetClientCapability(It.IsAny<string>())).Returns<string>(GetClientCapabilityCallBack);
        }

        private void SetupRoleProvider()
        {
            var mockRoleProvider = MockComponentProvider.CreateNew<RoleProvider>();
        }

        private IDataReader GetRedirectionsCallBack(int portalId)
        {
            var dtCheck = this.dtRedirections.Clone();
            foreach (var row in this.dtRedirections.Select("PortalId = " + portalId))
            {
                dtCheck.Rows.Add(row.ItemArray);
            }

            return dtCheck.CreateDataReader();
        }

        private IDataReader GetRedirectionRulesCallBack(int rid)
        {
            var dtCheck = this.dtRules.Clone();
            foreach (var row in this.dtRules.Select("RedirectionId = " + rid))
            {
                dtCheck.Rows.Add(row.ItemArray);
            }

            return dtCheck.CreateDataReader();
        }

        private IDataReader GetPortalsCallBack(string culture)
        {
            return this.GetPortalCallBack(Portal0, DotNetNuke.Services.Localization.Localization.SystemLocale);
        }

        private IDataReader GetPortalCallBack(int portalId, string culture)
        {
            DataTable table = new DataTable("Portal");

            var cols = new string[]
                        {
                            "PortalID", "PortalGroupID", "PortalName", "LogoFile", "FooterText", "ExpiryDate", "UserRegistration", "BannerAdvertising", "AdministratorId", "Currency", "HostFee",
                            "HostSpace", "PageQuota", "UserQuota", "AdministratorRoleId", "RegisteredRoleId", "Description", "KeyWords", "BackgroundFile", "GUID", "PaymentProcessor", "ProcessorUserId",
                            "ProcessorPassword", "SiteLogHistory", "Email", "DefaultLanguage", "TimezoneOffset", "AdminTabId", "HomeDirectory", "SplashTabId", "HomeTabId", "LoginTabId", "RegisterTabId",
                            "UserTabId", "SearchTabId", "Custom404TabId", "Custom500TabId", "TermsTabId", "PrivacyTabId", "SuperTabId", "CreatedByUserID", "CreatedOnDate", "LastModifiedByUserID", "LastModifiedOnDate", "CultureCode",
                        };

            foreach (var col in cols)
            {
                table.Columns.Add(col);
            }

            int homePage = 55;
            if (portalId == Portal0)
            {
                homePage = HomePageOnPortal0;
            }
            else if (portalId == Portal1)
            {
                homePage = HomePageOnPortal1;
            }

            table.Rows.Add(portalId, null, "My Website", "Logo.png", "Copyright 2011 by DotNetNuke Corporation", null, "2", "0", "2", "USD", "0", "0", "0", "0", "0", "1", "My Website", "DotNetNuke, DNN, Content, Management, CMS", null, "1057AC7A-3C08-4849-A3A6-3D2AB4662020", null, null, null, "0", "admin@changeme.invalid", "en-US", "-8", "58", "Portals/0", null, homePage.ToString(), null, null, "57", "56", "-1", "-1", null, null, "7", "-1", "2011-08-25 07:34:11", "-1", "2011-08-25 07:34:29", culture);

            return table.CreateDataReader();
        }

        private DataTable GetTabsDataTable()
        {
            DataTable table = new DataTable("Tabs");

            var cols = new string[]
                        {
                            "TabID", "UniqueId", "VersionGuid", "DefaultLanguageGuid", "LocalizedVersionGuid", "TabOrder", "PortalID", "TabName", "IsVisible", "ParentId", "Level", "IconFile", "IconFileLarge", "DisableLink", "Title", "Description", "KeyWords", "IsDeleted", "SkinSrc", "ContainerSrc", "TabPath", "StartDate", "EndDate", "Url", "HasChildren", "RefreshInterval", "PageHeadText", "IsSecure", "PermanentRedirect", "SiteMapPriority", "ContentItemID", "Content", "ContentTypeID", "ModuleID", "ContentKey", "Indexed", "CultureCode", "CreatedByUserID", "CreatedOnDate", "LastModifiedByUserID", "LastModifiedOnDate", "StateID", "HasBeenPublished", "IsSystem",
                        };

            foreach (var col in cols)
            {
                table.Columns.Add(col);
            }

            table.Rows.Add(HomePageOnPortal1, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "3", Portal1, "HomePageOnPortal1", true, null, "0", null, null, false, string.Empty, string.Empty, string.Empty, false, "[G]Skins/DarkKnight/Home-Mega-Menu.ascx", "[G]Containers/DarkKnight/SubTitle_Grey.ascx", "//HomePageOnPortal1", null, null, string.Empty, false, null, null, false, false, "0.5", "89", "HomePageOnPortal1", "1", "-1", null, false, null, "-1", DateTime.Now, "-1", DateTime.Now, "0", true, false);
            table.Rows.Add(HomePageOnPortal0, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "3", Portal0, "HomePageOnPortal0", true, null, "0", null, null, false, string.Empty, string.Empty, string.Empty, false, "[G]Skins/DarkKnight/Home-Mega-Menu.ascx", "[G]Containers/DarkKnight/SubTitle_Grey.ascx", "//HomePageOnPortal0", null, null, string.Empty, false, null, null, false, false, "0.5", "89", "HomePageOnPortal0", "1", "-1", null, false, null, "-1", DateTime.Now, "-1", DateTime.Now, "0", true, false);
            table.Rows.Add(AnotherPageOnSamePortal, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "4", Portal0, "AnotherPageOnSamePortal", true, null, "0", null, null, false, string.Empty, string.Empty, string.Empty, false, "[G]Skins/DarkKnight/Home-Mega-Menu.ascx", "[G]Containers/DarkKnight/SubTitle_Grey.ascx", "//AnotherPageOnSamePortal", null, null, string.Empty, false, null, null, false, false, "0.5", "89", "HomePageOnPortal0", "1", "-1", null, false, null, "-1", DateTime.Now, "-1", DateTime.Now, "0", true, false);
            table.Rows.Add(MobileLandingPage, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "5", Portal0, "MobileLandingPage", true, null, "0", null, null, false, string.Empty, string.Empty, string.Empty, false, "[G]Skins/DarkKnight/Home-Mega-Menu.ascx", "[G]Containers/DarkKnight/SubTitle_Grey.ascx", "//MobileLandingPage", null, null, string.Empty, false, null, null, false, false, "0.5", "89", "HomePageOnPortal0", "1", "-1", null, false, null, "-1", DateTime.Now, "-1", DateTime.Now, "0", true, false);
            table.Rows.Add(TabletLandingPage, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "6", Portal0, "TabletLandingPage", true, null, "0", null, null, false, string.Empty, string.Empty, string.Empty, false, "[G]Skins/DarkKnight/Home-Mega-Menu.ascx", "[G]Containers/DarkKnight/SubTitle_Grey.ascx", "//TabletLandingPage", null, null, string.Empty, false, null, null, false, false, "0.5", "89", "HomePageOnPortal0", "1", "-1", null, false, null, "-1", DateTime.Now, "-1", DateTime.Now, "0", true, false);
            table.Rows.Add(AllMobileLandingPage, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "7", Portal0, "AllMobileLandingPage", true, null, "0", null, null, false, string.Empty, string.Empty, string.Empty, false, "[G]Skins/DarkKnight/Home-Mega-Menu.ascx", "[G]Containers/DarkKnight/SubTitle_Grey.ascx", "//AllMobileLandingPage", null, null, string.Empty, false, null, null, false, false, "0.5", "89", "HomePageOnPortal0", "1", "-1", null, false, null, "-1", DateTime.Now, "-1", DateTime.Now, "0", true, false);
            table.Rows.Add(DeletedPageOnSamePortal, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "8", Portal0, "A Deleted Page", true, null, "0", null, null, false, string.Empty, string.Empty, string.Empty, true, "[G]Skins/DarkKnight/Home-Mega-Menu.ascx", "[G]Containers/DarkKnight/SubTitle_Grey.ascx", "//DeletedPage", null, null, string.Empty, false, null, null, false, false, "0.5", "90", "Deleted Page", "1", "-1", null, false, null, "-1", DateTime.Now, "-1", DateTime.Now, "0", true, false);

            return table;
        }

        private IDataReader GetTabsCallBack(int portalId)
        {
            var table = this.GetTabsDataTable();
            var newTable = table.Clone();
            foreach (var row in table.Select("PortalID = " + portalId))
            {
                newTable.Rows.Add(row.ItemArray);
            }

            return newTable.CreateDataReader();
        }

        private IDataReader GetTabCallBack(int tabId)
        {
            var table = this.GetTabsDataTable();
            var newTable = table.Clone();
            foreach (var row in table.Select("TabID = " + tabId))
            {
                newTable.Rows.Add(row.ItemArray);
            }

            return newTable.CreateDataReader();
        }

        private IDataReader GetTabModulesCallBack(int tabId)
        {
            DataTable table = new DataTable("TabModules");

            var cols = new string[]
                        {
                            "PortalID", "TabID", "TabModuleID", "ModuleID", "ModuleDefID", "ModuleOrder", "PaneName", "ModuleTitle", "CacheTime", "CacheMethod", "Alignment", "Color", "Border", "IconFile", "AllTabs", "Visibility", "IsDeleted", "Header", "Footer", "StartDate", "EndDate", "ContainerSrc", "DisplayTitle", "DisplayPrint", "DisplaySyndicate", "InheritViewPermissions", "DesktopModuleID", "DefaultCacheTime", "ModuleControlID", "BusinessControllerClass", "IsAdmin", "SupportedFeatures", "ContentItemID", "Content", "ContentTypeID", "ContentKey", "Indexed", "CreatedByUserID", "CreatedOnDate", "LastModifiedByUserID", "LastModifiedOnDate", "LastContentModifiedOnDate", "UniqueId", "VersionGuid", "DefaultLanguageGuid", "LocalizedVersionGuid", "CultureCode",
                        };

            foreach (var col in cols)
            {
                table.Columns.Add(col);
            }

            table.Columns["ModuleID"].DataType = typeof(int);

            var portalId = tabId == HomePageOnPortal0 ? Portal0 : Portal1;

            table.Rows.Add(portalId, tabId, 51, 362, 117, 1, "ContentPane", "DotNetNuke® Enterprise Edition", "3600", "FileModuleCachingProvider", "left", null, null, null, false, "2", false, null, null, null, null, "[G]Containers/DarkKnight/Banner.ascx", true, false, false, false, null, null, "0", true, "75", "1200", "240", "DotNetNuke.Modules.HtmlPro.HtmlTextController", false, "7", "90", "DotNetNuke® Enterprise Edition", "2", null, false, "-1", DateTime.Now, "-1", DateTime.Now, DateTime.Now, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), null);

            return table.CreateDataReader();
        }

        private IDataReader GetPortalSettingsCallBack(int portalId, string culture)
        {
            DataTable table = new DataTable("PortalSettings");

            var cols = new string[]
                        {
                            "SettingName", "SettingValue", "CreatedByUserID", "CreatedOnDate", "LastModifiedByUserID", "LastModifiedOnDate", "CultureCode",
                        };

            foreach (var col in cols)
            {
                table.Columns.Add(col);
            }

            var alias = portalId == Portal0 ? PortalAlias0 : PortalAlias1;

            table.Rows.Add("DefaultPortalAlias", alias, "-1", DateTime.Now, "-1", DateTime.Now, "en-us");

            return table.CreateDataReader();
        }

        private IDataReader GetPortalGroupsCallBack()
        {
            DataTable table = new DataTable("PortalGroups");

            var cols = new string[]
                        {
                            "PortalGroupID", "MasterPortalID", "PortalGroupName", "PortalGroupDescription", "AuthenticationDomain", "CreatedByUserID", "CreatedOnDate", "LastModifiedByUserID", "LastModifiedOnDate",
                        };

            foreach (var col in cols)
            {
                table.Columns.Add(col);
            }

            table.Rows.Add(1, 0, "Portal Group", string.Empty, string.Empty, -1, DateTime.Now, -1, DateTime.Now);

            return table.CreateDataReader();
        }

        private IDataReader GetAllRedirectionsCallBack()
        {
            return this.dtRedirections.CreateDataReader();
        }

        private void PrepareData()
        {
            // id, portalId, name, type, sortOrder, sourceTabId, includeChildTabs, targetType, targetValue, enabled
            this.dtRedirections.Rows.Add(1, Portal0, "R4", (int)RedirectionType.Other, 4, -1, DisabledFlag, (int)TargetType.Portal, "1", EnabledFlag);
            this.dtRedirections.Rows.Add(2, Portal0, "R2", (int)RedirectionType.Tablet, 2, -1, DisabledFlag, (int)TargetType.Portal, "1", EnabledFlag);
            this.dtRedirections.Rows.Add(3, Portal0, "R3", (int)RedirectionType.AllMobile, 3, -1, DisabledFlag, (int)TargetType.Portal, "1", EnabledFlag);
            this.dtRedirections.Rows.Add(4, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, -1, DisabledFlag, (int)TargetType.Portal, "1", EnabledFlag);
            this.dtRedirections.Rows.Add(5, Portal0, "R5", (int)RedirectionType.MobilePhone, 5, HomePageOnPortal0, EnabledFlag, (int)TargetType.Portal, "1", EnabledFlag);
            this.dtRedirections.Rows.Add(6, Portal0, "R6", (int)RedirectionType.MobilePhone, 6, -1, DisabledFlag, (int)TargetType.Tab, HomePageOnPortal0, EnabledFlag);
            this.dtRedirections.Rows.Add(7, Portal0, "R7", (int)RedirectionType.MobilePhone, 7, -1, DisabledFlag, (int)TargetType.Url, ExternalSite, EnabledFlag);

            // id, redirectionId, capability, expression
            this.dtRules.Rows.Add(1, 1, "mobile_browser", "Safari");
            this.dtRules.Rows.Add(2, 1, "device_os_version", "4.0");

            this.dtRedirections.Rows.Add(8, Portal1, "R8", (int)RedirectionType.MobilePhone, 1, -1, EnabledFlag, (int)TargetType.Portal, 2, true);
            this.dtRedirections.Rows.Add(9, Portal1, "R9", (int)RedirectionType.Tablet, 1, -1, EnabledFlag, (int)TargetType.Portal, 2, true);
            this.dtRedirections.Rows.Add(10, Portal1, "R10", (int)RedirectionType.AllMobile, 1, -1, EnabledFlag, (int)TargetType.Portal, 2, true);
        }

        private void PrepareOperaBrowserOnSymbianOSRedirectionRule()
        {
            this.dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.Other, 1, -1, DisabledFlag, (int)TargetType.Tab, AnotherPageOnSamePortal, EnabledFlag);

            // id, redirectionId, capability, expression
            this.dtRules.Rows.Add(1, 1, "mobile_browser", "Opera Mini");
            this.dtRules.Rows.Add(2, 1, "device_os", "Symbian OS");
        }

        private void PrepareOperaBrowserOnIPhoneOSRedirectionRule()
        {
            this.dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.Other, 1, -1, DisabledFlag, (int)TargetType.Tab, AnotherPageOnSamePortal, EnabledFlag);

            // id, redirectionId, capability, expression
            this.dtRules.Rows.Add(1, 1, "mobile_browser", "Opera Mini");
            this.dtRules.Rows.Add(2, 1, "device_os", "iPhone OS");
        }

        private void PreparePortalToAnotherPageOnSamePortal()
        {
            this.dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, -1, DisabledFlag, (int)TargetType.Tab, AnotherPageOnSamePortal, EnabledFlag);
        }

        private void PrepareSamePortalToSamePortalRedirectionRule()
        {
            this.dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, -1, DisabledFlag, (int)TargetType.Portal, Portal0, 1);
        }

        private void PrepareExternalSiteRedirectionRule()
        {
            this.dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 7, -1, DisabledFlag, (int)TargetType.Url, ExternalSite, 1);
        }

        private void PrepareMobileAndTabletRedirectionRuleWithMobileFirst()
        {
            this.dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, -1, DisabledFlag, (int)TargetType.Tab, MobileLandingPage, EnabledFlag);
            this.dtRedirections.Rows.Add(2, Portal0, "R2", (int)RedirectionType.Tablet, 2, -1, DisabledFlag, (int)TargetType.Tab, TabletLandingPage, EnabledFlag);
        }

        private void PrepareMobileAndTabletRedirectionRuleWithAndTabletRedirectionRuleTabletFirst()
        {
            this.dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.Tablet, 1, -1, DisabledFlag, (int)TargetType.Tab, TabletLandingPage, EnabledFlag);
            this.dtRedirections.Rows.Add(2, Portal0, "R2", (int)RedirectionType.MobilePhone, 2, -1, DisabledFlag, (int)TargetType.Tab, MobileLandingPage, EnabledFlag);
        }

        private void PrepareAllMobileRedirectionRule()
        {
            this.dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.AllMobile, 1, -1, DisabledFlag, (int)TargetType.Tab, AllMobileLandingPage, EnabledFlag);
        }

        private void PrepareSingleDisabledRedirectionRule()
        {
            this.dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.AllMobile, 1, -1, DisabledFlag, (int)TargetType.Tab, AllMobileLandingPage, DisabledFlag);
        }

        private HttpApplication GenerateApplication()
        {
            var simulator = new Instance.Utilities.HttpSimulator.HttpSimulator("/", "c:\\");
            simulator.SimulateRequest(new Uri("http://localhost/dnn/Default.aspx"));

            var app = new HttpApplication();

            var requestProp = typeof(NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
            requestProp.SetValue(HttpContext.Current.Request.QueryString, false, null);

            var stateProp = typeof(HttpApplication).GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic);
            stateProp.SetValue(app, HttpContext.Current);

            return app;
        }

        private string NavigateUrl(int tabId)
        {
            return string.Format("/Default.aspx?tabid={0}", tabId);
        }
    }
}

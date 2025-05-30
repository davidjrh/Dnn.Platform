﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Common.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Web.Caching;

    using DotNetNuke.Collections.Internal;
    using DotNetNuke.Entities.Host;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Instrumentation;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Services.Cache;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Log.EventLog;
    using DotNetNuke.Services.OutputCache;

    [Obsolete("Deprecated in DotNetNuke 9.13.8. This type has no known use. Scheduled for removal in v11.0.0.")]
    public enum CoreCacheType
    {
        /// <summary>Host level.</summary>
        Host = 1,

        /// <summary>Portal level.</summary>
        Portal = 2,

        /// <summary>Tab level.</summary>
        Tab = 3,
    }

    /// <summary>The DataCache class is a facade class for the CachingProvider Instance's.</summary>
    public class DataCache
    {
        // Host keys
        public const string SecureHostSettingsCacheKey = "SecureHostSettings";
        public const string UnSecureHostSettingsCacheKey = "UnsecureHostSettings";
        public const string HostSettingsCacheKey = "HostSettings";
        public const CacheItemPriority HostSettingsCachePriority = CacheItemPriority.NotRemovable;
        public const int HostSettingsCacheTimeOut = 20;

        // Portal keys
        public const string PortalAliasCacheKey = "PortalAlias";
        public const CacheItemPriority PortalAliasCachePriority = CacheItemPriority.NotRemovable;
        public const int PortalAliasCacheTimeOut = 200;

        public const string PortalSettingsCacheKey = "PortalSettings{0}{1}";
        public const CacheItemPriority PortalSettingsCachePriority = CacheItemPriority.NotRemovable;
        public const int PortalSettingsCacheTimeOut = 20;

        public const string PortalDictionaryCacheKey = "PortalDictionary";
        public const CacheItemPriority PortalDictionaryCachePriority = CacheItemPriority.High;
        public const int PortalDictionaryTimeOut = 20;

        public const string PortalCacheKey = "Portal{0}_{1}";
        public const CacheItemPriority PortalCachePriority = CacheItemPriority.High;
        public const int PortalCacheTimeOut = 20;

        public const string AllPortalsCacheKey = "AllPortals";
        public const CacheItemPriority AllPortalsCachePriority = CacheItemPriority.High;
        public const int AllPortalsCacheTimeOut = 20;

        public const string PortalUserCountCacheKey = "PortalUserCount{0}";
        public const CacheItemPriority PortalUserCountCachePriority = CacheItemPriority.High;
        public const int PortalUserCountCacheTimeOut = 20;

        public const string PortalGroupsCacheKey = "PortalGroups";
        public const CacheItemPriority PortalGroupsCachePriority = CacheItemPriority.High;
        public const int PortalGroupsCacheTimeOut = 20;

        public const string PortalPermissionCacheKey = "PortalPermission{0}";
        public const CacheItemPriority PortalPermissionCachePriority = CacheItemPriority.High;
        public const int PortalPermissionCacheTimeOut = 20;

        /// <summary> The portal styles cache key.</summary>
        public const string PortalStylesCacheKey = "Dnn_Css_Custom_Properties_{0}";

        // Tab cache keys
        public const string TabCacheKey = "Tab_Tabs{0}";
        public const string TabSettingsCacheKey = "TabSettings{0}";
        public const CacheItemPriority TabCachePriority = CacheItemPriority.High;
        public const int TabCacheTimeOut = 20;
        public const string TabPathCacheKey = "Tab_TabPathDictionary{0}_{1}";
        public const CacheItemPriority TabPathCachePriority = CacheItemPriority.High;
        public const int TabPathCacheTimeOut = 20;
        public const string TabPermissionCacheKey = "Tab_TabPermissions{0}";
        public const CacheItemPriority TabPermissionCachePriority = CacheItemPriority.High;
        public const int TabPermissionCacheTimeOut = 20;
        public const string TabAliasSkinCacheKey = "Tab_TabAliasSkins{0}";
        public const CacheItemPriority TabAliasSkinCachePriority = CacheItemPriority.High;
        public const int TabAliasSkinCacheTimeOut = 20;
        public const string TabCustomAliasCacheKey = "Tab_TabCustomAliases{0}";
        public const CacheItemPriority TabCustomAliasCachePriority = CacheItemPriority.High;
        public const int TabCustomAliasCacheTimeOut = 20;
        public const string TabUrlCacheKey = "Tab_TabUrls{0}";
        public const CacheItemPriority TabUrlCachePriority = CacheItemPriority.High;
        public const int TabUrlCacheTimeOut = 20;
        public const string TabVersionsCacheKey = "Tab_TabVersions{0}";
        public const CacheItemPriority TabVersionsCachePriority = CacheItemPriority.High;
        public const int TabVersionsCacheTimeOut = 20;
        public const string TabVersionDetailsCacheKey = "Tab_TabVersionDetails{0}";
        public const CacheItemPriority TabVersionDetailsCachePriority = CacheItemPriority.High;
        public const int TabVersionDetailsCacheTimeOut = 20;

        public const string AuthenticationServicesCacheKey = "AuthenticationServices";
        public const CacheItemPriority AuthenticationServicesCachePriority = CacheItemPriority.NotRemovable;
        public const int AuthenticationServicesCacheTimeOut = 20;

        public const string DesktopModulePermissionCacheKey = "DesktopModulePermissions";
        public const CacheItemPriority DesktopModulePermissionCachePriority = CacheItemPriority.High;
        public const int DesktopModulePermissionCacheTimeOut = 20;

        public const string DesktopModuleCacheKey = "DesktopModulesByPortal{0}";
        public const CacheItemPriority DesktopModuleCachePriority = CacheItemPriority.High;
        public const int DesktopModuleCacheTimeOut = 20;

        public const string PortalDesktopModuleCacheKey = "PortalDesktopModules{0}";
        public const CacheItemPriority PortalDesktopModuleCachePriority = CacheItemPriority.AboveNormal;
        public const int PortalDesktopModuleCacheTimeOut = 20;

        public const string ModuleDefinitionCacheKey = "ModuleDefinitions";
        public const CacheItemPriority ModuleDefinitionCachePriority = CacheItemPriority.High;
        public const int ModuleDefinitionCacheTimeOut = 20;

        public const string ModuleControlsCacheKey = "ModuleControls";
        public const CacheItemPriority ModuleControlsCachePriority = CacheItemPriority.High;
        public const int ModuleControlsCacheTimeOut = 20;

        public const string TabModuleCacheKey = "TabModules{0}";
        public const string TabModuleSettingsCacheKey = "TabModuleSettings{0}";
        public const string SingleTabModuleCacheKey = "SingleTabModule{0}";
        public const string TabModuleSettingsNameCacheKey = "TabModuleSettingsName:{0}:{1}";
        public const CacheItemPriority TabModuleCachePriority = CacheItemPriority.AboveNormal;
        public const int TabModuleCacheTimeOut = 20;

        public const string PublishedTabModuleCacheKey = "PublishedTabModules{0}";
        public const CacheItemPriority PublishedTabModuleCachePriority = CacheItemPriority.AboveNormal;
        public const int PublishedTabModuleCacheTimeOut = 20;

        public const string ModulePermissionCacheKey = "ModulePermissions{0}";
        public const CacheItemPriority ModulePermissionCachePriority = CacheItemPriority.AboveNormal;
        public const int ModulePermissionCacheTimeOut = 20;

        public const string ModuleCacheKey = "Modules{0}";
        public const string ModuleSettingsCacheKey = "ModuleSettings{0}";
        public const int ModuleCacheTimeOut = 20;
        public const CacheItemPriority ModuleCachePriority = CacheItemPriority.AboveNormal;

        public const string SharedModulesByPortalCacheKey = "SharedModulesByPortal{0}";
        public const int SharedModulesByPortalCacheTimeOut = 20;
        public const CacheItemPriority SharedModulesByPortalCachePriority = CacheItemPriority.Normal;

        public const string ContentItemsCacheKey = "ContentItems{0}";
        public const int ContentItemsCacheTimeOut = 20;
        public const CacheItemPriority ContentItemsCachePriority = CacheItemPriority.Normal;

        public const string SharedModulesWithPortalCacheKey = "SharedModulesWithPortal{0}";
        public const int SharedModulesWithPortalCacheTimeOut = 20;
        public const CacheItemPriority SharedModulesWithPortalCachePriority = CacheItemPriority.Normal;

        public const string FolderCacheKey = "Folders{0}";
        public const int FolderCacheTimeOut = 20;
        public const CacheItemPriority FolderCachePriority = CacheItemPriority.Normal;

        public const string FolderUserCacheKey = "Folders|{0}|{1}|{2}";
        public const int FolderUserCacheTimeOut = 20;
        public const CacheItemPriority FolderUserCachePriority = CacheItemPriority.Normal;

        public const string FolderMappingCacheKey = "FolderMapping|{0}";
        public const int FolderMappingCacheTimeOut = 20;
        public const CacheItemPriority FolderMappingCachePriority = CacheItemPriority.High;

        public const string FolderPermissionCacheKey = "FolderPermissions{0}"; // parent cache dependency key
        public const string FolderPathPermissionCacheKey = "FolderPathPermissions|{0}|{1}";
        public const CacheItemPriority FolderPermissionCachePriority = CacheItemPriority.Normal;
        public const int FolderPermissionCacheTimeOut = 20;

        public const string ListsCacheKey = "Lists{0}";
        public const string ListEntriesCacheKey = "ListEntries|{0}|{1}";
        public const CacheItemPriority ListsCachePriority = CacheItemPriority.Normal;
        public const int ListsCacheTimeOut = 20;

        public const string ProfileDefinitionsCacheKey = "ProfileDefinitions{0}";
        public const int ProfileDefinitionsCacheTimeOut = 20;

        public const string UserCacheKey = "UserInfo|{0}|{1}";
        public const int UserCacheTimeOut = 1;
        public const CacheItemPriority UserCachePriority = CacheItemPriority.Normal;

        public const string UserProfileCacheKey = "UserProfile|{0}|{1}";
        public const int UserProfileCacheTimeOut = UserCacheTimeOut;

        public const string UserNotificationsConversationCountCacheKey = "UserNitifConversationCount|{0}|{1}";
        public const string UserNotificationsCountCacheKey = "UserNotificationsCount|{0}|{1}";
        public const string UserNewThreadsCountCacheKey = "UserNewThreadsCount|{0}|{1}";
        public const int NotificationsCacheTimeInSec = 30;

        public const string UserPersonalizationCacheKey = "UserPersonalization|{0}|{1}";
        public const int UserPersonalizationCacheTimeout = 5;
        public const CacheItemPriority UserPersonalizationCachePriority = CacheItemPriority.Normal;

        public const string UserLookupCacheKey = "UserLookup|{0}";
        public const int UserLookupCacheTimeOut = 20;
        public const CacheItemPriority UserLookupCachePriority = CacheItemPriority.High;

        public const string LocalesCacheKey = "Locales{0}";
        public const CacheItemPriority LocalesCachePriority = CacheItemPriority.Normal;
        public const int LocalesCacheTimeOut = 20;

        public const string SkinDefaultsCacheKey = "SkinDefaults_{0}";
        public const CacheItemPriority SkinDefaultsCachePriority = CacheItemPriority.Normal;
        public const int SkinDefaultsCacheTimeOut = 20;

        public const CacheItemPriority ResourceFilesCachePriority = CacheItemPriority.Normal;
        public const int ResourceFilesCacheTimeOut = 20;

        public const string ResourceFileLookupDictionaryCacheKey = "ResourceFileLookupDictionary";
        public const CacheItemPriority ResourceFileLookupDictionaryCachePriority = CacheItemPriority.NotRemovable;
        public const int ResourceFileLookupDictionaryTimeOut = 200;

        public const string SpaModulesContentHtmlFileCacheKey = "SpaModulesContentHtmlFile|{0}";
        public const string SpaModulesFileExistsCacheKey = "SpaModulesFileExists|{0}";
        public const CacheItemPriority SpaModulesHtmlFileCachePriority = CacheItemPriority.Normal;
        public const int SpaModulesHtmlFileTimeOut = 200;

        public const string SkinsCacheKey = "GetSkins{0}";

        public const string BannersCacheKey = "Banners:{0}:{1}:{2}";
        public const CacheItemPriority BannersCachePriority = CacheItemPriority.Normal;
        public const int BannersCacheTimeOut = 20;

        public const string RedirectionsCacheKey = "Redirections:{0}";
        public const CacheItemPriority RedirectionsCachePriority = CacheItemPriority.Default;
        public const int RedirectionsCacheTimeOut = 20;

        public const string PreviewProfilesCacheKey = "PreviewProfiles:{0}";
        public const CacheItemPriority PreviewProfilesCachePriority = CacheItemPriority.Default;
        public const int PreviewProfilesCacheTimeOut = 20;

        public const string RelationshipTypesCacheKey = "RelationshipTypes";
        public const CacheItemPriority RelationshipTypesCachePriority = CacheItemPriority.Default;
        public const int RelationshipTypesCacheTimeOut = 20;

        public const string RelationshipByPortalIDCacheKey = "RelationshipByPortalID:{0}";
        public const CacheItemPriority RelationshipByPortalIDCachePriority = CacheItemPriority.Default;
        public const int RelationshipByPortalIDCacheTimeOut = 20;

        public const string RolesCacheKey = "Roles:{0}";
        public const CacheItemPriority RolesCachePriority = CacheItemPriority.Default;
        public const int RolesCacheTimeOut = 20;

        public const string RoleGroupsCacheKey = "RoleGroups:{0}";
        public const CacheItemPriority RoleGroupsCachePriority = CacheItemPriority.Default;
        public const int RoleGroupsCacheTimeOut = 20;

        public const string JournalTypesCacheKey = "JournalTypes:{0}";
        public const CacheItemPriority JournalTypesCachePriority = CacheItemPriority.Default;
        public const int JournalTypesTimeOut = 20;

        public const string NotificationTypesCacheKey = "NotificationTypes:{0}";
        public const CacheItemPriority NotificationTypesCachePriority = CacheItemPriority.Default;
        public const int NotificationTypesTimeOut = 20;

        public const string NotificationTypeActionsCacheKey = "NotificationTypeActions:{0}";
        public const string NotificationTypeActionsByNameCacheKey = "NotificationTypeActions:{0}|{1}";
        public const CacheItemPriority NotificationTypeActionsPriority = CacheItemPriority.Default;
        public const int NotificationTypeActionsTimeOut = 20;

        public const string SubscriptionTypesCacheKey = "SubscriptionTypes";
        public const CacheItemPriority SubscriptionTypesCachePriority = CacheItemPriority.Default;
        public const int SubscriptionTypesTimeOut = 20;

        public const string PackagesCacheKey = "Packages_{0}";
        public const string PackageDependenciesCacheKey = "Packages_Dependencies";
        public const CacheItemPriority PackagesCachePriority = CacheItemPriority.Default;
        public const int PackagesCacheTimeout = 20;

        public const string ContentTypesCacheKey = "ContentTypes";
        public const CacheItemPriority ContentTypesCachePriority = CacheItemPriority.AboveNormal;
        public const int ContentTypesCacheTimeOut = 20;

        public const string PermissionsCacheKey = "Permissions";
        public const CacheItemPriority PermissionsCachePriority = CacheItemPriority.AboveNormal;
        public const int PermissionsCacheTimeout = 20;

        public const string PackageTypesCacheKey = "PackagesTypes";
        public const CacheItemPriority PackageTypesCachePriority = CacheItemPriority.AboveNormal;
        public const int PackageTypesCacheTimeout = 20;

        public const string JavaScriptLibrariesCacheKey = "JavaScriptLibraries";
        public const CacheItemPriority JavaScriptLibrariesCachePriority = CacheItemPriority.AboveNormal;
        public const int JavaScriptLibrariesCacheTimeout = 20;

        public const string CaptchaCacheKey = "Captcha_{0}";
        public const CacheItemPriority CaptchaCachePriority = CacheItemPriority.NotRemovable;
        public const int CaptchaCacheTimeout = 2;

        public const string ContentWorkflowCacheKey = "ContentWorkflows:{0}";
        public const string ContentWorkflowStateCacheKey = "ContentWorkflowStates_{0}";
        public const CacheItemPriority WorkflowsCachePriority = CacheItemPriority.Low;
        public const int WorkflowsCacheTimeout = 2;

        public const string ScopeTypesCacheKey = "ScopeTypes";
        public const string VocabularyCacheKey = "Vocabularies";
        public const string TermCacheKey = "Terms_{0}";

        internal const string UserIdListToClearDiskImageCacheKey = "UserIdListToClearDiskImage_{0}";

        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(DataCache));

        private static readonly ReaderWriterLockSlim DictionaryLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<string, object> LockDictionary = new Dictionary<string, object>();

        private static readonly SharedDictionary<string, object> DictionaryCache = new SharedDictionary<string, object>();

        private static readonly TimeSpan FiveSeconds = new TimeSpan(0, 0, 5);

        private static string cachePersistenceEnabled = string.Empty;

        public static bool CachePersistenceEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(cachePersistenceEnabled))
                {
                    cachePersistenceEnabled = Config.GetSetting("EnableCachePersistence") ?? "false";
                }

                return bool.Parse(cachePersistenceEnabled);
            }
        }

        public static void ClearCache()
        {
            CachingProvider.Instance().Clear("Prefix", "DNN_");
            using (DictionaryCache.GetWriteLock())
            {
                DictionaryCache.Clear();
            }

            // log the cache clear event
            var log = new LogInfo { LogTypeKey = EventLogController.EventLogType.CACHE_REFRESH.ToString() };
            log.LogProperties.Add(new LogDetailInfo("*", "Refresh"));
            LogController.Instance.AddLog(log);
        }

        public static void ClearCache(string cachePrefix)
        {
            CachingProvider.Instance().Clear("Prefix", GetDnnCacheKey(cachePrefix));
        }

        public static void ClearFolderCache(int portalId)
        {
            CachingProvider.Instance().Clear("Folder", portalId.ToString());
        }

        public static void ClearHostCache(bool cascade)
        {
            if (cascade)
            {
                ClearCache();
            }
            else
            {
                CachingProvider.Instance().Clear("Host", string.Empty);
            }
        }

        public static void ClearModuleCache(int tabId)
        {
            CachingProvider.Instance().Clear("Module", tabId.ToString());
            Dictionary<int, int> portals = PortalController.GetPortalDictionary();
            if (portals.ContainsKey(tabId))
            {
                Hashtable tabSettings = TabController.Instance.GetTabSettings(tabId);
                if (tabSettings["CacheProvider"] != null && tabSettings["CacheProvider"].ToString().Length > 0)
                {
                    OutputCachingProvider outputProvider = OutputCachingProvider.Instance(tabSettings["CacheProvider"].ToString());
                    if (outputProvider != null)
                    {
                        outputProvider.Remove(tabId);
                    }
                }

                var portalId = portals[tabId];
                RemoveCache(string.Format(SharedModulesByPortalCacheKey, portalId));
                RemoveCache(string.Format(SharedModulesWithPortalCacheKey, portalId));
            }
        }

        public static void ClearModulePermissionsCachesByPortal(int portalId)
        {
            CachingProvider.Instance().Clear("ModulePermissionsByPortal", portalId.ToString());
        }

        public static void ClearPortalCache(int portalId, bool cascade)
        {
            CachingProvider.Instance().Clear(cascade ? "PortalCascade" : "Portal", portalId.ToString());
        }

        public static void ClearTabsCache(int portalId)
        {
            CachingProvider.Instance().Clear("Tab", portalId.ToString());
        }

        public static void ClearDefinitionsCache(int portalId)
        {
            RemoveCache(string.Format(ProfileDefinitionsCacheKey, portalId));
        }

        public static void ClearDesktopModulePermissionsCache()
        {
            RemoveCache(DesktopModulePermissionCacheKey);
        }

        public static void ClearFolderPermissionsCache(int portalId)
        {
            PermissionProvider.ResetCacheDependency(
                portalId,
                () => RemoveCache(string.Format(FolderPermissionCacheKey, portalId)));
        }

        public static void ClearListsCache(int portalId)
        {
            RemoveCache(string.Format(ListsCacheKey, portalId));
        }

        public static void ClearModulePermissionsCache(int tabId)
        {
            RemoveCache(string.Format(ModulePermissionCacheKey, tabId));
        }

        public static void ClearTabPermissionsCache(int portalId)
        {
            RemoveCache(string.Format(TabPermissionCacheKey, portalId));
        }

        public static void ClearPortalPermissionsCache(int portalId)
        {
            RemoveCache(string.Format(PortalPermissionCacheKey, portalId));
        }

        public static void ClearUserCache(int portalId, string username)
        {
            RemoveCache(string.Format(UserCacheKey, portalId, username));
            RemoveCache(string.Format(UserProfileCacheKey, portalId, username));
        }

        public static void ClearPortalUserCountCache(int portalID)
        {
            CachingProvider.Instance().Remove(string.Format(DataCache.PortalUserCountCacheKey, portalID));
        }

        public static void ClearUserPersonalizationCache(int portalId, int userId)
        {
            RemoveCache(string.Format(UserPersonalizationCacheKey, portalId, userId));
        }

        public static void ClearPackagesCache(int portalId)
        {
            RemoveCache(string.Format(PackagesCacheKey, portalId));
        }

        public static TObject GetCachedData<TObject>(CacheItemArgs cacheItemArgs, CacheItemExpiredCallback cacheItemExpired)
        {
            // declare local object and try and retrieve item from the cache
            return GetCachedData<TObject>(cacheItemArgs, cacheItemExpired, false);
        }

        public static TObject GetCache<TObject>(string cacheKey)
        {
            object objObject = GetCache(cacheKey);
            if (objObject == null)
            {
                return default(TObject);
            }

            return (TObject)objObject;
        }

        public static object GetCache(string cacheKey)
        {
            return CachingProvider.Instance().GetItem(GetDnnCacheKey(cacheKey));
        }

        public static void RemoveCache(string cacheKey)
        {
            CachingProvider.Instance().Remove(GetDnnCacheKey(cacheKey));
        }

        public static void RemoveFromPrivateDictionary(string dnnCacheKey)
        {
            using (DictionaryCache.GetWriteLock())
            {
                DictionaryCache.Remove(CleanCacheKey(dnnCacheKey));
            }
        }

        public static void SetCache(string cacheKey, object objObject)
        {
            SetCache(cacheKey, objObject, (DNNCacheDependency)null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        public static void SetCache(string cacheKey, object objObject, DNNCacheDependency objDependency)
        {
            SetCache(cacheKey, objObject, objDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        public static void SetCache(string cacheKey, object objObject, DateTime absoluteExpiration)
        {
            SetCache(cacheKey, objObject, (DNNCacheDependency)null, absoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        public static void SetCache(string cacheKey, object objObject, TimeSpan slidingExpiration)
        {
            SetCache(cacheKey, objObject, (DNNCacheDependency)null, Cache.NoAbsoluteExpiration, slidingExpiration, CacheItemPriority.Normal, null);
        }

        public static void SetCache(string cacheKey, object objObject, DNNCacheDependency objDependency, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            SetCache(cacheKey, objObject, objDependency, absoluteExpiration, slidingExpiration, CacheItemPriority.Normal, null);
        }

        public static void SetCache(string cacheKey, object objObject, DNNCacheDependency objDependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            if (objObject != null)
            {
                // if no OnRemoveCallback value is specified, use the default method
                if (onRemoveCallback == null)
                {
                    onRemoveCallback = ItemRemovedCallback;
                }

                CachingProvider.Instance().Insert(GetDnnCacheKey(cacheKey), objObject, objDependency, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
            }
        }

        internal static void ItemRemovedCallback(string key, object value, CacheItemRemovedReason removedReason)
        {
            // if the item was removed from the cache, log the key and reason to the event log
            try
            {
                if (Globals.Status == Globals.UpgradeStatus.None)
                {
                    var log = new LogInfo();
                    switch (removedReason)
                    {
                        case CacheItemRemovedReason.Removed:
                            log.LogTypeKey = EventLogController.EventLogType.CACHE_REMOVED.ToString();
                            break;
                        case CacheItemRemovedReason.Expired:
                            log.LogTypeKey = EventLogController.EventLogType.CACHE_EXPIRED.ToString();
                            break;
                        case CacheItemRemovedReason.Underused:
                            log.LogTypeKey = EventLogController.EventLogType.CACHE_UNDERUSED.ToString();
                            break;
                        case CacheItemRemovedReason.DependencyChanged:
                            log.LogTypeKey = EventLogController.EventLogType.CACHE_DEPENDENCYCHANGED.ToString();
                            break;
                    }

                    log.LogProperties.Add(new LogDetailInfo(key, removedReason.ToString()));
                    LogController.Instance.AddLog(log);
                }
            }
            catch (Exception exc)
            {
                // Swallow exception
                Logger.Error(exc);
            }
        }

        internal static TObject GetCachedData<TObject>(CacheItemArgs cacheItemArgs, CacheItemExpiredCallback cacheItemExpired, bool storeInDictionary)
        {
            object objObject = storeInDictionary
                                   ? GetCachedDataFromDictionary(cacheItemArgs, cacheItemExpired)
                                   : GetCachedDataFromRuntimeCache(cacheItemArgs, cacheItemExpired);

            // return the object
            if (objObject == null)
            {
                return default(TObject);
            }

            return (TObject)objObject;
        }

        private static string GetDnnCacheKey(string cacheKey)
        {
            return CachingProvider.GetCacheKey(cacheKey);
        }

        private static string CleanCacheKey(string cacheKey)
        {
            return CachingProvider.CleanCacheKey(cacheKey);
        }

        private static object GetCachedDataFromRuntimeCache(CacheItemArgs cacheItemArgs, CacheItemExpiredCallback cacheItemExpired)
        {
            object objObject = GetCache(cacheItemArgs.CacheKey);

            // if item is not cached
            if (objObject == null)
            {
                // Get Unique Lock for cacheKey
                object @lock = GetUniqueLockObject(cacheItemArgs.CacheKey);

                // prevent other threads from entering this block while we regenerate the cache
                lock (@lock)
                {
                    // try to retrieve object from the cache again (in case another thread loaded the object since we first checked)
                    objObject = GetCache(cacheItemArgs.CacheKey);

                    // if object was still not retrieved
                    if (objObject == null)
                    {
                        // get object from data source using delegate
                        try
                        {
                            objObject = cacheItemExpired(cacheItemArgs);
                        }
                        catch (Exception ex)
                        {
                            objObject = null;
                            Exceptions.LogException(ex);
                        }

                        // set cache timeout
                        int timeOut = cacheItemArgs.CacheTimeOut * Convert.ToInt32(Host.PerformanceSetting);

                        // if we retrieved a valid object and we are using caching
                        if (objObject != null && timeOut > 0)
                        {
                            // save the object in the cache
                            SetCache(
                                cacheItemArgs.CacheKey,
                                objObject,
                                cacheItemArgs.CacheDependency,
                                Cache.NoAbsoluteExpiration,
                                TimeSpan.FromMinutes(timeOut),
                                cacheItemArgs.CachePriority,
                                cacheItemArgs.CacheCallback);

                            // check if the item was actually saved in the cache
                            if (GetCache(cacheItemArgs.CacheKey) == null)
                            {
                                // log the event if the item was not saved in the cache ( likely because we are out of memory )
                                var log = new LogInfo { LogTypeKey = EventLogController.EventLogType.CACHE_OVERFLOW.ToString() };
                                log.LogProperties.Add(new LogDetailInfo(cacheItemArgs.CacheKey, "Overflow - Item Not Cached"));
                                LogController.Instance.AddLog(log);
                            }
                        }

                        // This thread won so remove unique Lock from collection
                        RemoveUniqueLockObject(cacheItemArgs.CacheKey);
                    }
                }
            }

            return objObject;
        }

        private static object GetCachedDataFromDictionary(CacheItemArgs cacheItemArgs, CacheItemExpiredCallback cacheItemExpired)
        {
            object cachedObject;

            bool isFound;
            using (DictionaryCache.GetReadLock())
            {
                isFound = DictionaryCache.TryGetValue(cacheItemArgs.CacheKey, out cachedObject);
            }

            if (!isFound)
            {
                // get object from data source using delegate
                try
                {
                    cachedObject = cacheItemExpired != null ? cacheItemExpired(cacheItemArgs) : null;
                }
                catch (Exception ex)
                {
                    cachedObject = null;
                    Exceptions.LogException(ex);
                }

                using (DictionaryCache.GetWriteLock())
                {
                    if (!DictionaryCache.ContainsKey(cacheItemArgs.CacheKey))
                    {
                        if (cachedObject != null)
                        {
                            DictionaryCache[cacheItemArgs.CacheKey] = cachedObject;
                        }
                    }
                }
            }

            return cachedObject;
        }

        private static object GetUniqueLockObject(string key)
        {
            object @lock = null;
            if (DictionaryLock.TryEnterReadLock(FiveSeconds))
            {
                try
                {
                    // Try to get lock Object (for key) from Dictionary
                    if (LockDictionary.ContainsKey(key))
                    {
                        @lock = LockDictionary[key];
                    }
                }
                finally
                {
                    DictionaryLock.ExitReadLock();
                }
            }

            if (@lock == null)
            {
                if (DictionaryLock.TryEnterWriteLock(FiveSeconds))
                {
                    try
                    {
                        // Double check dictionary
                        if (!LockDictionary.ContainsKey(key))
                        {
                            // Create new lock
                            LockDictionary[key] = new object();
                        }

                        // Retrieve lock
                        @lock = LockDictionary[key];
                    }
                    finally
                    {
                        DictionaryLock.ExitWriteLock();
                    }
                }
            }

            return @lock;
        }

        private static void RemoveUniqueLockObject(string key)
        {
            if (!DictionaryLock.TryEnterWriteLock(FiveSeconds))
            {
                return;
            }

            try
            {
                // check dictionary
                if (LockDictionary.ContainsKey(key))
                {
                    // Remove lock
                    LockDictionary.Remove(key);
                }
            }
            finally
            {
                DictionaryLock.ExitWriteLock();
            }
        }
    }
}

﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Services.FileSystem
{
    using System;
    using System.Collections;
    using System.Data;

    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Services.FileSystem.Internal;

    /// <summary>  Represents the FolderMapping object and holds the Properties of that object.</summary>
    [Serializable]
    public class FolderMappingInfo : IHydratable
    {
        private Hashtable folderMappingSettings;

        private string imageUrl;

        /// <summary>Initializes a new instance of the <see cref="FolderMappingInfo"/> class.</summary>
        public FolderMappingInfo()
        {
            this.FolderMappingID = Null.NullInteger;
            this.PortalID = Null.NullInteger;
        }

        /// <summary>Initializes a new instance of the <see cref="FolderMappingInfo"/> class.</summary>
        /// <param name="portalID">The portal ID.</param>
        /// <param name="mappingName">The mapping name.</param>
        /// <param name="folderProviderType">The name of the <see cref="Type"/> of the <see cref="FolderProvider"/>.</param>
        public FolderMappingInfo(int portalID, string mappingName, string folderProviderType)
        {
            this.FolderMappingID = Null.NullInteger;
            this.PortalID = portalID;
            this.MappingName = mappingName;
            this.FolderProviderType = folderProviderType;
        }

        public Hashtable FolderMappingSettings
        {
            get
            {
                if (this.folderMappingSettings == null)
                {
                    if (this.FolderMappingID == Null.NullInteger)
                    {
                        this.folderMappingSettings = new Hashtable();
                    }
                    else
                    {
                        this.folderMappingSettings = FolderMappingController.Instance.GetFolderMappingSettings(this.FolderMappingID);
                    }
                }

                return this.folderMappingSettings;
            }
        }

        public string ImageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(this.imageUrl))
                {
                    this.imageUrl = FolderProvider.Instance(this.FolderProviderType).GetFolderProviderIconPath();
                }

                return this.imageUrl;
            }
        }

        public bool IsEditable
        {
            get
            {
                return !DefaultFolderProviders.GetDefaultProviders().Contains(this.FolderProviderType);
            }
        }

        public int FolderMappingID { get; set; }

        public int PortalID { get; set; }

        public string MappingName { get; set; }

        public string FolderProviderType { get; set; }

        public int Priority { get; set; }

        public bool SyncAllSubFolders
        {
            get
            {
                if (this.FolderMappingSettings.ContainsKey("SyncAllSubFolders"))
                {
                    return bool.Parse(this.FolderMappingSettings["SyncAllSubFolders"].ToString());
                }

                return true;
            }

            set
            {
                this.FolderMappingSettings["SyncAllSubFolders"] = value;
            }
        }

        /// <summary>  Gets or sets and sets the Key ID.</summary>
        public int KeyID
        {
            get
            {
                return this.FolderMappingID;
            }

            set
            {
                this.FolderMappingID = value;
            }
        }

        /// <summary>  Fills a FolderInfo from a Data Reader.</summary>
        /// <param name="dr">The Data Reader to use.</param>
        public void Fill(IDataReader dr)
        {
            this.FolderMappingID = Null.SetNullInteger(dr["FolderMappingID"]);
            this.PortalID = Null.SetNullInteger(dr["PortalID"]);
            this.MappingName = Null.SetNullString(dr["MappingName"]);
            this.FolderProviderType = Null.SetNullString(dr["FolderProviderType"]);
            this.Priority = Null.SetNullInteger(dr["Priority"]);
        }
    }
}

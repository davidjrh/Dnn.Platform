﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Services.OutputCache.Providers
{
    using System;
    using System.Globalization;
    using System.IO;

    using DotNetNuke.Common.Utilities;

    /// <summary>FileResponseFilter implements <see cref="OutputCacheResponseFilter"/> to capture the response into files.</summary>
    public class FileResponseFilter : OutputCacheResponseFilter
    {
        // Private _content As StringBuilder
        private DateTime cacheExpiration;

        /// <summary>Initializes a new instance of the <see cref="FileResponseFilter"/> class.</summary>
        /// <param name="itemId">The tab ID.</param>
        /// <param name="maxVaryByCount">The maximum number of values by which the cached response can vary.</param>
        /// <param name="filterChain">The stream to write into the file.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheDuration">The duration for which the response should be cached.</param>
        internal FileResponseFilter(int itemId, int maxVaryByCount, Stream filterChain, string cacheKey, TimeSpan cacheDuration)
            : base(filterChain, cacheKey, cacheDuration, maxVaryByCount)
        {
            if (maxVaryByCount > -1 && Services.OutputCache.Providers.FileProvider.GetCachedItemCount(itemId) >= maxVaryByCount)
            {
                this.HasErrored = true;
                return;
            }

            this.CachedOutputTempFileName = Services.OutputCache.Providers.FileProvider.GetTempFileName(itemId, cacheKey);
            this.CachedOutputFileName = Services.OutputCache.Providers.FileProvider.GetCachedOutputFileName(itemId, cacheKey);
            this.CachedOutputAttribFileName = Services.OutputCache.Providers.FileProvider.GetAttribFileName(itemId, cacheKey);
            if (File.Exists(this.CachedOutputTempFileName))
            {
                bool fileDeleted = FileSystemUtils.DeleteFileWithWait(this.CachedOutputTempFileName, 100, 200);
                if (fileDeleted == false)
                {
                    this.HasErrored = true;
                }
            }

            if (this.HasErrored == false)
            {
                try
                {
                    this.CaptureStream = new FileStream(this.CachedOutputTempFileName, FileMode.CreateNew, FileAccess.Write);
                }
                catch (Exception)
                {
                    this.HasErrored = true;
                    throw;
                }

                this.cacheExpiration = DateTime.UtcNow.Add(cacheDuration);
                this.HasErrored = false;
            }
        }

        public string CachedOutputTempFileName { get; set; }

        public string CachedOutputFileName { get; set; }

        public string CachedOutputAttribFileName { get; set; }

        public DateTime CacheExpiration
        {
            get
            {
                return this.cacheExpiration;
            }

            set
            {
                this.cacheExpiration = value;
            }
        }

        /// <inheritdoc/>
        public override byte[] StopFiltering(int itemId, bool deleteData)
        {
            if (this.HasErrored)
            {
                return null;
            }

            if (this.CaptureStream != null)
            {
                this.CaptureStream.Close();

                if (File.Exists(this.CachedOutputFileName))
                {
                    FileSystemUtils.DeleteFileWithWait(this.CachedOutputFileName, 100, 200);
                }

                File.Move(this.CachedOutputTempFileName, this.CachedOutputFileName);

                StreamWriter oWrite = File.CreateText(this.CachedOutputAttribFileName);
                oWrite.WriteLine(this.cacheExpiration.ToString(CultureInfo.InvariantCulture));
                oWrite.Close();
            }

            if (deleteData)
            {
                FileSystemUtils.DeleteFileWithWait(this.CachedOutputFileName, 100, 200);
                FileSystemUtils.DeleteFileWithWait(this.CachedOutputAttribFileName, 100, 200);
            }

            return null;
        }
    }
}

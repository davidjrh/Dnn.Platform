﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace Dnn.ExportImport.Components.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Dnn.ExportImport.Components.Common;
    using Dnn.ExportImport.Components.Dto;
    using Dnn.ExportImport.Components.Entities;
    using Dnn.ExportImport.Components.Providers;
    using Dnn.ExportImport.Components.Services;

    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Portals;
    using Newtonsoft.Json;

    /// <summary>The export controller.</summary>
    public class ExportController : BaseController
    {
        /// <summary>Initializes a new instance of the <see cref="ExportController"/> class.</summary>
        [Obsolete("Deprecated in DotNetNuke 10.0.0. Please use overload with IEnumerable<BasePortableService>. Scheduled removal in v12.0.0.")]
        public ExportController()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ExportController"/> class.</summary>
        /// <param name="portableServices">The portable service implementations.</param>
        public ExportController(IEnumerable<BasePortableService> portableServices)
            : base(portableServices)
        {
        }

        /// <summary>Queues an export operation.</summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="exportDto">The export DTO.</param>
        /// <returns>The job ID.</returns>
        public int QueueOperation(int userId, ExportDto exportDto)
        {
            exportDto.ProductSku = DotNetNuke.Application.DotNetNukeContext.Current.Application.SKU;
            exportDto.ProductVersion = Globals.FormatVersion(DotNetNuke.Application.DotNetNukeContext.Current.Application.Version, true);
            var dbTime = DateUtils.GetDatabaseUtcTime();
            exportDto.ToDateUtc = dbTime.AddMilliseconds(-dbTime.Millisecond);
            var directory = dbTime.ToString("yyyy-MM-dd_HH-mm-ss");
            if (exportDto.ExportMode == ExportMode.Differential)
            {
                exportDto.FromDateUtc = this.GetLastJobTime(exportDto.PortalId, JobType.Export);
            }

            var dataObject = JsonConvert.SerializeObject(exportDto);
            exportDto.IsDirty = false; // This should be set to false for new job.
            var jobId = DataProvider.Instance()
                .AddNewJob(
                    exportDto.PortalId,
                    userId,
                    JobType.Export,
                    exportDto.ExportName,
                    exportDto.ExportDescription,
                    directory,
                    dataObject);

            // Run the scheduler if required.
            if (exportDto.RunNow)
            {
                EntitiesController.Instance.RunSchedule();
            }

            this.AddEventLog(exportDto.PortalId, userId, jobId, Constants.LogTypeSiteExport);
            return jobId;
        }

        /// <summary>Creates a package manifest.</summary>
        /// <param name="exportJob">The export job.</param>
        /// <param name="exportFileInfo">The export file info.</param>
        /// <param name="summary">The summary.</param>
        public void CreatePackageManifest(ExportImportJob exportJob, ExportFileInfo exportFileInfo, ImportExportSummary summary)
        {
            var filePath = Path.Combine(ExportFolder, exportJob.Directory, Constants.ExportManifestName);
            var portal = PortalController.Instance.GetPortal(exportJob.PortalId);
            var packageInfo = new ImportPackageInfo
            {
                Summary = summary,
                PackageId = exportJob.Directory,
                Name = exportJob.Name,
                Description = exportJob.Description,
                ExporTime = exportJob.CreatedOnDate,
                PortalName = portal?.PortalName,
            };
            Util.WriteJson(filePath, packageInfo);
        }
    }
}

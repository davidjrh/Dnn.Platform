﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Services.Log.EventLog
{
    using System;

    using DotNetNuke.Services.Scheduling;

    public class PurgeLogBuffer : SchedulerClient
    {
        /// <summary>Initializes a new instance of the <see cref="PurgeLogBuffer"/> class.</summary>
        /// <param name="objScheduleHistoryItem">The schedule history item.</param>
        public PurgeLogBuffer(ScheduleHistoryItem objScheduleHistoryItem)
        {
            this.ScheduleHistoryItem = objScheduleHistoryItem;
        }

        /// <inheritdoc/>
        public override void DoWork()
        {
            try
            {
                // notification that the event is progressing
                this.Progressing(); // OPTIONAL
                LoggingProvider.Instance().PurgeLogBuffer();
                this.ScheduleHistoryItem.Succeeded = true; // REQUIRED
                this.ScheduleHistoryItem.AddLogNote("Purged log entries successfully"); // OPTIONAL
            }
            catch (Exception exc)
            {
                this.ScheduleHistoryItem.Succeeded = false; // REQUIRED
                this.ScheduleHistoryItem.AddLogNote("EXCEPTION: " + exc); // OPTIONAL
                this.Errored(ref exc); // REQUIRED

                // log the exception
                Exceptions.Exceptions.LogException(exc); // OPTIONAL
            }
        }
    }
}

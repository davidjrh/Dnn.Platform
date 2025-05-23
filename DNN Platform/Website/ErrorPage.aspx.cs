﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

// ReSharper disable once CheckNamespace
namespace DotNetNuke.Services.Exceptions
{
    using System;
    using System.Web;
    using System.Web.UI;

    using DotNetNuke.Abstractions.Application;
    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Security;
    using DotNetNuke.Services.FileSystem;

    using Microsoft.Extensions.DependencyInjection;

    using Localization = DotNetNuke.Services.Localization.Localization;

    /// <summary>Trapped errors are redirected to this universal error page, resulting in a graceful display.</summary>
    /// <remarks>
    /// 'get the last server error
    /// 'process this error using the Exception Management Application Block
    /// 'add to a placeholder and place on page
    /// 'catch direct access - No exception was found...you shouldn't end up here unless you go to this aspx page URL directly.
    /// </remarks>
    public partial class ErrorPage : Page
    {
        private readonly IApplicationStatusInfo appStatus;

        /// <summary>Initializes a new instance of the <see cref="ErrorPage"/> class.</summary>
        public ErrorPage()
            : this(null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ErrorPage"/> class.</summary>
        /// <param name="appStatus">The application status.</param>
        public ErrorPage(IApplicationStatusInfo appStatus)
        {
            this.appStatus = appStatus ?? Globals.GetCurrentServiceProvider().GetRequiredService<IApplicationStatusInfo>();
        }

        /// <inheritdoc/>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.DefaultStylesheet.Attributes["href"] = this.ResolveUrl("~/Portals/_default/default.css");
            this.InstallStylesheet.Attributes["href"] = this.ResolveUrl("~/Install/install.css");
        }

        /// <inheritdoc/>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var portalSettings = PortalController.Instance.GetCurrentSettings();
            if (portalSettings != null && !string.IsNullOrEmpty(portalSettings.LogoFile))
            {
                IFileInfo fileInfo = FileManager.Instance.GetFile(portalSettings.PortalId, portalSettings.LogoFile);
                if (fileInfo != null)
                {
                    this.headerImage.ImageUrl = FileManager.Instance.GetUrl(fileInfo);
                }
            }

            this.headerImage.Visible = !string.IsNullOrEmpty(this.headerImage.ImageUrl);

            string localizedMessage;
            var status = this.Request.QueryString["status"];
            if (!string.IsNullOrEmpty(status))
            {
                this.ManageError(status);
            }
            else
            {
                // get the last server error
                var exc = this.Server.GetLastError();
                try
                {
                    if (this.Request.Url.LocalPath.ToLowerInvariant().EndsWith("installwizard.aspx"))
                    {
                        this.ErrorPlaceHolder.Controls.Add(new LiteralControl(HttpUtility.HtmlEncode(exc.ToString())));
                    }
                    else
                    {
                        var lex = new PageLoadException(exc.Message, exc);
                        Exceptions.LogException(lex);
                        localizedMessage = Localization.GetString("Error.Text", Localization.GlobalResourceFile);
                        this.ErrorPlaceHolder.Controls.Add(new ErrorContainer(localizedMessage, lex).Container);
                    }
                }
                catch
                {
                    // No exception was found...you shouldn't end up here
                    // unless you go to this aspx page URL directly
                    localizedMessage = Localization.GetString("UnhandledError.Text", Localization.GlobalResourceFile);
                    this.ErrorPlaceHolder.Controls.Add(new LiteralControl(localizedMessage));
                }

                this.Response.StatusCode = 500;
            }

            localizedMessage = Localization.GetString("Return.Text", Localization.GlobalResourceFile);

            this.hypReturn.Text = localizedMessage;
        }

        private void ManageError(string status)
        {
            string errorMode = Config.GetCustomErrorMode(this.appStatus);

            string errorMessage = HttpUtility.HtmlEncode(this.Request.QueryString["error"]);
            string errorMessage2 = HttpUtility.HtmlEncode(this.Request.QueryString["error2"]);
            string localizedMessage = Localization.GetString(status + ".Error", Localization.GlobalResourceFile);
            if (localizedMessage != null)
            {
                localizedMessage = localizedMessage.Replace("src=\"images/403-3.gif\"", "src=\"" + this.ResolveUrl("~/images/403-3.gif") + "\"");

                if (!string.IsNullOrEmpty(errorMessage2) && ((errorMode == "Off") || ((errorMode == "RemoteOnly") && this.Request.IsLocal)))
                {
                    this.ErrorPlaceHolder.Controls.Add(new LiteralControl(string.Format(localizedMessage, errorMessage2)));
                }
                else
                {
                    this.ErrorPlaceHolder.Controls.Add(new LiteralControl(string.Format(localizedMessage, errorMessage)));
                }
            }

            int statusCode;
            int.TryParse(status, out statusCode);

            if (statusCode > -1)
            {
                this.Response.StatusCode = statusCode;
            }
        }
    }
}

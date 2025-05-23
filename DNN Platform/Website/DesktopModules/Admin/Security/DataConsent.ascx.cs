﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Modules.Admin.Users
{
    using System;
    using System.Web;
    using System.Web.UI;

    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Users;
    using DotNetNuke.Security;
    using DotNetNuke.Services.Log.EventLog;

    /// <summary>A control which handles a user's consent to the site's usage of their data.</summary>
    public partial class DataConsent : UserModuleBase
    {
        public delegate void DataConsentEventHandler(object sender, DataConsentEventArgs e);

        public event DataConsentEventHandler DataConsentCompleted;

        public enum DataConsentStatus
        {
            /// <summary>Consented.</summary>
            Consented = 0,

            /// <summary>Cancelled.</summary>
            Cancelled = 1,

            /// <summary>Removed account.</summary>
            RemovedAccount = 2,

            /// <summary>Failed to remove account.</summary>
            FailedToRemoveAccount = 3,
        }

        public string DeleteMeConfirmString
        {
            get
            {
                switch (this.PortalSettings.DataConsentUserDeleteAction)
                {
                    case PortalSettings.UserDeleteAction.Manual:
                        return this.LocalizeString("ManualDelete.Confirm");
                    case PortalSettings.UserDeleteAction.DelayedHardDelete:
                        return this.LocalizeString("DelayedHardDelete.Confirm");
                    case PortalSettings.UserDeleteAction.HardDelete:
                        return this.LocalizeString("HardDelete.Confirm");
                }

                return string.Empty;
            }
        }

        public void OnDataConsentComplete(DataConsentEventArgs e)
        {
            this.DataConsentCompleted?.Invoke(this, e);
        }

        /// <inheritdoc/>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.cmdCancel.Click += this.CmdCancel_Click;
            this.cmdSubmit.Click += this.CmdSubmit_Click;
            this.cmdDeleteMe.Click += this.CmdDeleteMe_Click;
            this.cmdDeleteMe.Visible = this.PortalSettings.DataConsentUserDeleteAction != PortalSettings.UserDeleteAction.Off;
            if (!this.Page.IsPostBack)
            {
                this.chkAgree.Checked = false;
                this.cmdSubmit.Enabled = false;
                this.pnlNoAgreement.Visible = false;
            }

            this.chkAgree.Attributes.Add(
                "onclick",
                $"document.getElementById({HttpUtility.JavaScriptStringEncode(this.cmdSubmit.ClientID, addDoubleQuotes: true)}).disabled = !this.checked;");
            this.cmdDeleteMe.Attributes.Add(
                "onclick",
                $"if (!confirm({HttpUtility.JavaScriptStringEncode(this.DeleteMeConfirmString, addDoubleQuotes: true)})) this.preventDefault();");
        }

        private void CmdCancel_Click(object sender, EventArgs e)
        {
            this.OnDataConsentComplete(new DataConsentEventArgs(DataConsentStatus.Cancelled));
        }

        private void CmdSubmit_Click(object sender, EventArgs e)
        {
            if (this.chkAgree.Checked)
            {
                UserController.UserAgreedToTerms(this.User);
                this.User.HasAgreedToTerms = true;
                this.OnDataConsentComplete(new DataConsentEventArgs(DataConsentStatus.Consented));
            }
        }

        private void CmdDeleteMe_Click(object sender, EventArgs e)
        {
            var success = false;
            switch (this.PortalSettings.DataConsentUserDeleteAction)
            {
                case PortalSettings.UserDeleteAction.Manual:
                    this.User.Membership.Approved = false;
                    UserController.UpdateUser(this.PortalSettings.PortalId, this.User);
                    UserController.UserRequestsRemoval(this.User, true);
                    success = true;
                    break;
                case PortalSettings.UserDeleteAction.DelayedHardDelete:
                    var user = this.User;
                    success = UserController.DeleteUser(ref user, true, false);
                    UserController.UserRequestsRemoval(this.User, true);
                    break;
                case PortalSettings.UserDeleteAction.HardDelete:
                    success = UserController.RemoveUser(this.User);
                    break;
            }

            if (success)
            {
                PortalSecurity.Instance.SignOut();
                this.OnDataConsentComplete(new DataConsentEventArgs(DataConsentStatus.RemovedAccount));
            }
            else
            {
                this.OnDataConsentComplete(new DataConsentEventArgs(DataConsentStatus.FailedToRemoveAccount));
            }
        }

        /// <summary>
        /// The DataConsentEventArgs class provides a customised EventArgs class for
        /// the DataConsent Event.
        /// </summary>
        public class DataConsentEventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DataConsentEventArgs"/> class.
            /// Constructs a new DataConsentEventArgs.
            /// </summary>
            /// <param name="status">The Data Consent Status.</param>
            public DataConsentEventArgs(DataConsentStatus status)
            {
                this.Status = status;
            }

            /// <summary>Gets or sets the Update Status.</summary>
            public DataConsentStatus Status { get; set; }
        }
    }
}

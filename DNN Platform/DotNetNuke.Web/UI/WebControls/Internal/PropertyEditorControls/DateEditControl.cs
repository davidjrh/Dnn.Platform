﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Web.UI.WebControls.Internal.PropertyEditorControls
{
    using System;
    using System.Collections.Specialized;
    using System.Data.SqlTypes;
    using System.Globalization;
    using System.Web.UI;

    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Instrumentation;
    using DotNetNuke.UI.WebControls;

    /// <summary>
    /// The DateEditControl control provides a standard UI component for editing
    /// date properties.
    /// </summary>
    /// <remarks>
    /// This control is only for internal use, please don't reference it in any other place as it may be removed in future.
    /// </remarks>
    [ToolboxData("<{0}:DateEditControl runat=server></{0}:DateEditControl>")]
    public class DateEditControl : EditControl
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(DateEditControl));
        private DnnDatePicker dateControl;

        /// <inheritdoc/>
        public override string EditControlClientId
        {
            get
            {
                this.EnsureChildControls();
                return this.DateControl.ClientID;
            }
        }

        /// <inheritdoc/>
        public override string ID
        {
            get
            {
                return base.ID + "_control";
            }

            set
            {
                base.ID = value;
            }
        }

        /// <summary>Gets dateValue returns the Date representation of the Value.</summary>
        /// <value>A Date representing the Value.</value>
        protected DateTime DateValue
        {
            get
            {
                DateTime dteValue = Null.NullDate;
                try
                {
                    var dteString = Convert.ToString(this.Value);
                    DateTime.TryParse(dteString, CultureInfo.InvariantCulture, DateTimeStyles.None, out dteValue);
                }
                catch (Exception exc)
                {
                    Logger.Error(exc);
                }

                return dteValue;
            }
        }

        /// <summary>
        /// Gets defaultDateFormat is a string that will be used to format the date in the absence of a
        /// FormatAttribute.
        /// </summary>
        /// <value>A String representing the default format to use to render the date.</value>
        /// <returns>A Format String.</returns>
        protected virtual string DefaultFormat
        {
            get
            {
                return "d";
            }
        }

        /// <summary>Gets format is a string that will be used to format the date in View mode.</summary>
        /// <value>A String representing the format to use to render the date.</value>
        /// <returns>A Format String.</returns>
        protected virtual string Format
        {
            get
            {
                string format = this.DefaultFormat;
                if (this.CustomAttributes != null)
                {
                    foreach (Attribute attribute in this.CustomAttributes)
                    {
                        if (attribute is FormatAttribute)
                        {
                            var formatAtt = (FormatAttribute)attribute;
                            format = formatAtt.Format;
                            break;
                        }
                    }
                }

                return format;
            }
        }

        /// <summary>Gets oldDateValue returns the Date representation of the OldValue.</summary>
        /// <value>A Date representing the OldValue.</value>
        protected DateTime OldDateValue
        {
            get
            {
                DateTime dteValue = Null.NullDate;
                try
                {
                    // Try and cast the value to an DateTime
                    var dteString = this.OldValue as string;
                    if (!string.IsNullOrEmpty(dteString))
                    {
                        dteValue = DateTime.Parse(dteString, CultureInfo.InvariantCulture);
                    }
                }
                catch (Exception exc)
                {
                    Logger.Error(exc);
                }

                return dteValue;
            }
        }

        /// <summary>Gets or sets the Value expressed as a String.</summary>
        protected override string StringValue
        {
            get
            {
                string stringValue = Null.NullString;
                if (this.DateValue.ToUniversalTime().Date != (DateTime)SqlDateTime.MinValue && this.DateValue != Null.NullDate)
                {
                    stringValue = this.DateValue.ToString(this.Format);
                }

                return stringValue;
            }

            set
            {
                this.Value = DateTime.Parse(value);
            }
        }

        private DnnDatePicker DateControl
        {
            get
            {
                if (this.dateControl == null)
                {
                    this.dateControl = new DnnDatePicker();
                }

                return this.dateControl;
            }
        }

        /// <inheritdoc/>
        public override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            this.EnsureChildControls();
            bool dataChanged = false;
            string presentValue = this.StringValue;
            string postedValue = postCollection[postDataKey + "_control"];
            if (!presentValue.Equals(postedValue))
            {
                if (string.IsNullOrEmpty(postedValue))
                {
                    this.Value = Null.NullDate;
                    dataChanged = true;
                }
                else
                {
                    this.Value = DateTime.Parse(postedValue).ToString(CultureInfo.InvariantCulture);
                    dataChanged = true;
                }
            }

            this.LoadDateControls();
            return dataChanged;
        }

        /// <inheritdoc/>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            this.DateControl.ControlStyle.CopyFrom(this.ControlStyle);
            this.DateControl.ID = base.ID + "_control";

            this.Controls.Add(this.DateControl);
        }

        protected virtual void LoadDateControls()
        {
            if (this.DateValue != Null.NullDate)
            {
                this.DateControl.SelectedDate = this.DateValue.Date;
            }
        }

        /// <summary>OnDataChanged is called by the PostBack Handler when the Data has changed.</summary>
        /// <param name="e">An EventArgs object.</param>
        protected override void OnDataChanged(EventArgs e)
        {
            var args = new PropertyEditorEventArgs(this.Name);
            args.Value = this.DateValue;
            args.OldValue = this.OldDateValue;
            args.StringValue = this.DateValue.ToString(CultureInfo.InvariantCulture);
            this.OnValueChanged(args);
        }

        /// <inheritdoc/>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            this.LoadDateControls();

            if (this.Page != null && this.EditMode == PropertyEditorMode.Edit)
            {
                this.Page.RegisterRequiresPostBack(this);
            }
        }

        /// <inheritdoc cref="EditControl.RenderEditMode"/>
        protected override void RenderEditMode(HtmlTextWriter writer)
        {
            this.RenderChildren(writer);
        }

        /// <inheritdoc cref="EditControl.RenderViewMode"/>
        protected override void RenderViewMode(HtmlTextWriter writer)
        {
            this.ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(this.StringValue);
            writer.RenderEndTag();
        }
    }
}

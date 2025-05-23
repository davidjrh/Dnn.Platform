﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.UI.WebControls
{
    using System;
    using System.Web.UI.WebControls;

    /// <summary>The DNNDataGrid control provides an Enhanced Data Grid, that supports other column types.</summary>
    public class DNNDataGrid : DataGrid
    {
        public event DNNDataGridCheckedColumnEventHandler ItemCheckedChanged;

        /// <summary>Called when the grid is Data Bound.</summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnDataBinding(EventArgs e)
        {
            foreach (DataGridColumn column in this.Columns)
            {
                if (ReferenceEquals(column.GetType(), typeof(CheckBoxColumn)))
                {
                    // Manage CheckBox column events
                    var cbColumn = (CheckBoxColumn)column;
                    cbColumn.CheckedChanged += this.OnItemCheckedChanged;
                }
            }
        }

        /// <inheritdoc/>
        protected override void CreateControlHierarchy(bool useDataSource)
        {
            base.CreateControlHierarchy(useDataSource);
        }

        /// <inheritdoc/>
        protected override void PrepareControlHierarchy()
        {
            base.PrepareControlHierarchy();
        }

        /// <summary>Centralised Event that is raised whenever a check box is changed.</summary>
        private void OnItemCheckedChanged(object sender, DNNDataGridCheckChangedEventArgs e)
        {
            if (this.ItemCheckedChanged != null)
            {
                this.ItemCheckedChanged(sender, e);
            }
        }
    }
}

﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace Dnn.PersonaBar.Pages.Components.Prompt.Models
{
    public class PageModel : PageModelBase
    {
        /// <summary>Initializes a new instance of the <see cref="PageModel"/> class.</summary>
        public PageModel()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PageModel"/> class.</summary>
        /// <param name="tab">The tab info.</param>
        public PageModel(DotNetNuke.Entities.Tabs.TabInfo tab)
            : base(tab)
        {
            this.Container = tab.ContainerSrc;
            this.Url = tab.Url;
            this.Keywords = tab.KeyWords;
            this.Description = tab.Description;
        }

        public string Container { get; set; }

        public string Url { get; set; }

        public string Keywords { get; set; }

        public string Description { get; set; }
    }
}

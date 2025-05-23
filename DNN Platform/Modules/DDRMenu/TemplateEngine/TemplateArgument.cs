﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.DDRMenu.TemplateEngine
{
    public class TemplateArgument
    {
        /// <summary>Initializes a new instance of the <see cref="TemplateArgument"/> class.</summary>
        public TemplateArgument()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TemplateArgument"/> class.</summary>
        /// <param name="name">The template argument name.</param>
        /// <param name="value">The template argument value.</param>
        public TemplateArgument(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}

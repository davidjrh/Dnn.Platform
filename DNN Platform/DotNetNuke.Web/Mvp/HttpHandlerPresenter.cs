﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvp
{
    using DotNetNuke.Internal.SourceGenerators;

    using WebFormsMvp;

    /// <summary>Represents a class that is a presenter for an HTTP handler in a Web Forms Model-View-Presenter application.</summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    [DnnDeprecated(9, 2, 0, "Replace WebFormsMvp and DotNetNuke.Web.Mvp with MVC or SPA patterns instead")]
    public abstract partial class HttpHandlerPresenter<TView> : Presenter<TView>
        where TView : class, IHttpHandlerView
    {
        /// <summary>Initializes a new instance of the <see cref="HttpHandlerPresenter{TView}"/> class.</summary>
        /// <param name="view">The view.</param>
        protected HttpHandlerPresenter(TView view)
            : base(view)
        {
        }
    }
}

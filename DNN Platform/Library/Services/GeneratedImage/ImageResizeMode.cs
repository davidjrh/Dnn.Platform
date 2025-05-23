﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Services.GeneratedImage
{
    /// <summary>Enumeration that represent the available resize modes.</summary>
    public enum ImageResizeMode
    {
        /// <summary>Resizes the image with the given width or height without maintaining the aspect ratio.</summary>
        Fill = 0,

        /// <summary>
        /// Fit mode maintains the aspect ratio of the original image while ensuring that the dimensions of the result
        /// do not exceed the maximum values for the resize transformation.
        /// </summary>
        Fit = 1,

        /// <summary>
        /// Crop resizes the image and removes parts of it to ensure that the dimensions of the result are exactly
        /// as specified by the transformation.
        /// </summary>
        Crop = 2,

        /// <summary>
        /// Resizes the image with the given width or height and maintains the aspect ratio. The image will be centered in a
        /// square area of the chosen background color.
        /// </summary>
        FitSquare = 3,
    }
}

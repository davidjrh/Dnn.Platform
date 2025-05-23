﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Modules.NavigationProvider
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;

    using DotNetNuke.Common;
    using DotNetNuke.Framework;
    using DotNetNuke.Internal.SourceGenerators;
    using DotNetNuke.UI.Skins;
    using DotNetNuke.UI.WebControls;

    /// <summary>Provides a renderer for navigation.</summary>
    public abstract partial class NavigationProvider : UserControlBase
    {
        public delegate void NodeClickEventHandler(NavigationEventArgs args);

        public delegate void PopulateOnDemandEventHandler(NavigationEventArgs args);

        public event NodeClickEventHandler NodeClick;

        public event PopulateOnDemandEventHandler PopulateOnDemand;

        public enum Alignment
        {
            /// <summary>Left aligned.</summary>
            Left = 0,

            /// <summary>Right aligned.</summary>
            Right = 1,

            /// <summary>Center aligned.</summary>
            Center = 2,

            /// <summary>Justified alignment.</summary>
            Justify = 3,
        }

        public enum HoverAction
        {
            /// <summary>Expand on hover.</summary>
            Expand = 0,

            /// <summary>No action on hover.</summary>
            None = 1,
        }

        public enum HoverDisplay
        {
            /// <summary>Highlight on hover.</summary>
            Highlight = 0,

            /// <summary>Render an outset border on hover.</summary>
            Outset = 1,

            /// <summary>No display changes on hover.</summary>
            None = 2,
        }

        public enum Orientation
        {
            /// <summary>Horizontal orientation.</summary>
            Horizontal = 0,

            /// <summary>Vertical orientation.</summary>
            Vertical = 1,
        }

        public abstract Control NavigationControl { get; }

        public abstract bool SupportsPopulateOnDemand { get; }

        public abstract string ControlID { get; set; }

        public virtual string PathImage
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string PathSystemImage
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string PathSystemScript
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string WorkImage
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string IndicateChildImageSub
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string IndicateChildImageRoot
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string IndicateChildImageExpandedSub
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string IndicateChildImageExpandedRoot
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public abstract string CSSControl { get; set; }

        public virtual string CSSContainerRoot
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSContainerSub
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSNode
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSNodeRoot
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSIcon
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSNodeHover
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSNodeHoverSub
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSNodeHoverRoot
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSBreak
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSIndicateChildSub
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSIndicateChildRoot
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSBreadCrumbSub
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSBreadCrumbRoot
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSNodeSelectedSub
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSNodeSelectedRoot
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSSeparator
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSLeftSeparator
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSRightSeparator
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSLeftSeparatorSelection
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSRightSeparatorSelection
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSLeftSeparatorBreadCrumb
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string CSSRightSeparatorBreadCrumb
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string StyleBackColor
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string StyleForeColor
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string StyleHighlightColor
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string StyleIconBackColor
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string StyleSelectionBorderColor
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string StyleSelectionColor
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string StyleSelectionForeColor
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual decimal StyleControlHeight
        {
            get
            {
                return 25;
            }

            set
            {
            }
        }

        public virtual decimal StyleBorderWidth
        {
            get
            {
                return 0;
            }

            set
            {
            }
        }

        public virtual decimal StyleNodeHeight
        {
            get
            {
                return 25;
            }

            set
            {
            }
        }

        public virtual decimal StyleIconWidth
        {
            get
            {
                return 0;
            }

            set
            {
            }
        }

        public virtual string StyleFontNames
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual decimal StyleFontSize
        {
            get
            {
                return 0;
            }

            set
            {
            }
        }

        public virtual string StyleFontBold
        {
            get
            {
                return "False";
            }

            set
            {
            }
        }

        public virtual string StyleRoot
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string StyleSub
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string EffectsStyle
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string EffectsTransition
        {
            get
            {
                return "'";
            }

            set
            {
            }
        }

        public virtual double EffectsDuration
        {
            get
            {
                return -1;
            }

            set
            {
            }
        }

        public virtual string EffectsShadowColor
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string EffectsShadowDirection
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual int EffectsShadowStrength
        {
            get
            {
                return -1;
            }

            set
            {
            }
        }

        public virtual Orientation ControlOrientation
        {
            get
            {
                return Orientation.Horizontal;
            }

            set
            {
            }
        }

        public virtual Alignment ControlAlignment
        {
            get
            {
                return Alignment.Left;
            }

            set
            {
            }
        }

        public virtual string ForceDownLevel
        {
            get
            {
                return false.ToString();
            }

            set
            {
            }
        }

        public virtual decimal MouseOutHideDelay
        {
            get
            {
                return -1;
            }

            set
            {
            }
        }

        public virtual HoverDisplay MouseOverDisplay
        {
            get
            {
                return HoverDisplay.Highlight;
            }

            set
            {
            }
        }

        public virtual HoverAction MouseOverAction
        {
            get
            {
                return HoverAction.Expand;
            }

            set
            {
            }
        }

        public virtual string ForceCrawlerDisplay
        {
            get
            {
                return "False";
            }

            set
            {
            }
        }

        public virtual bool IndicateChildren
        {
            get
            {
                return true;
            }

            set
            {
            }
        }

        public virtual string SeparatorHTML
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string SeparatorLeftHTML
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string SeparatorRightHTML
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string SeparatorLeftHTMLActive
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string SeparatorRightHTMLActive
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string SeparatorLeftHTMLBreadCrumb
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string SeparatorRightHTMLBreadCrumb
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string NodeLeftHTMLSub
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string NodeLeftHTMLRoot
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string NodeRightHTMLSub
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string NodeRightHTMLRoot
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string NodeLeftHTMLBreadCrumbSub
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string NodeRightHTMLBreadCrumbSub
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string NodeLeftHTMLBreadCrumbRoot
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual string NodeRightHTMLBreadCrumbRoot
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        public virtual bool PopulateNodesFromClient
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        public virtual List<CustomAttribute> CustomAttributes
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        /// <inheritdoc cref="Instance(System.IServiceProvider,string)"/>
        [DnnDeprecated(10, 0, 0, "Please use overload with IServiceProvider")]
        public static partial NavigationProvider Instance(string friendlyName)
        {
            return Instance(Globals.DependencyProvider, friendlyName);
        }

        public static NavigationProvider Instance(IServiceProvider serviceProvider, string friendlyName)
        {
            return (NavigationProvider)Reflection.CreateObject(serviceProvider, "navigationControl", friendlyName, string.Empty, string.Empty);
        }

        public abstract void Initialize();

        public abstract void Bind(DNNNodeCollection objNodes);

        public virtual void ClearNodes()
        {
        }

        protected void RaiseEvent_NodeClick(DNNNode objNode)
        {
            if (this.NodeClick != null)
            {
                this.NodeClick(new NavigationEventArgs(objNode.ID, objNode));
            }
        }

        protected void RaiseEvent_NodeClick(string strID)
        {
            if (this.NodeClick != null)
            {
                this.NodeClick(new NavigationEventArgs(strID, null));
            }
        }

        protected void RaiseEvent_PopulateOnDemand(DNNNode objNode)
        {
            if (this.PopulateOnDemand != null)
            {
                this.PopulateOnDemand(new NavigationEventArgs(objNode.ID, objNode));
            }
        }

        protected void RaiseEvent_PopulateOnDemand(string strID)
        {
            if (this.PopulateOnDemand != null)
            {
                this.PopulateOnDemand(new NavigationEventArgs(strID, null));
            }
        }
    }
}

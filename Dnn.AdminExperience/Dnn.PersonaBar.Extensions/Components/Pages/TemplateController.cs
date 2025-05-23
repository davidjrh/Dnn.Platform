﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace Dnn.PersonaBar.Pages.Components
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Xml;

    using Dnn.PersonaBar.Pages.Components.Dto;
    using Dnn.PersonaBar.Pages.Components.Exceptions;
    using Dnn.PersonaBar.Pages.Services.Dto;
    using DotNetNuke.Abstractions.Modules;
    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Framework;
    using DotNetNuke.Services.FileSystem;
    using DotNetNuke.Web.UI;

    using Microsoft.Extensions.DependencyInjection;

    public class TemplateController : ServiceLocator<ITemplateController, TemplateController>, ITemplateController
    {
        private const string TemplatesFolderPath = "Templates/";

        private readonly IBusinessControllerProvider businessControllerProvider;
        private readonly ITabController tabController;

        /// <summary>Initializes a new instance of the <see cref="TemplateController"/> class.</summary>
        [Obsolete("Deprecated in DotNetNuke 10.0.0. Please use overload with IServiceProvider. Scheduled removal in v12.0.0.")]
        public TemplateController()
            : this(null)
        {
            this.tabController = TabController.Instance;
        }

        /// <summary>Initializes a new instance of the <see cref="TemplateController"/> class.</summary>
        /// <param name="businessControllerProvider">The business controller provider.</param>
        public TemplateController(IBusinessControllerProvider businessControllerProvider)
        {
            this.businessControllerProvider = businessControllerProvider ?? Globals.DependencyProvider.GetRequiredService<IBusinessControllerProvider>();
            this.tabController = TabController.Instance;
        }

        /// <inheritdoc/>
        public string SaveAsTemplate(PageTemplate template)
        {
            string filename;
            try
            {
                var folder = GetTemplateFolder();

                if (folder == null)
                {
                    folder = CreateTemplateFolder();
                }

                filename = folder.FolderPath + template.Name + ".page.template";
                filename = filename.Replace("/", "\\");

                var xmlTemplate = new XmlDocument { XmlResolver = null };
                var nodePortal = xmlTemplate.AppendChild(xmlTemplate.CreateElement("portal"));
                nodePortal.Attributes?.Append(XmlUtils.CreateAttribute(xmlTemplate, "version", "3.0"));

                // Add template description
                var node = xmlTemplate.CreateElement("description");
                node.InnerXml = HttpUtility.HtmlEncode(template.Description);
                nodePortal.AppendChild(node);

                // Serialize tabs
                var nodeTabs = nodePortal.AppendChild(xmlTemplate.CreateElement("tabs"));
                this.SerializeTab(template, xmlTemplate, nodeTabs);

                // add file to Files table
                using (var fileContent = new MemoryStream(Encoding.UTF8.GetBytes(xmlTemplate.OuterXml)))
                {
                    FileManager.Instance.AddFile(folder, template.Name + ".page.template", fileContent, true, false, "application/octet-stream");
                }
            }
            catch (DotNetNuke.Services.FileSystem.PermissionsNotMetException)
            {
                throw new TemplateException("Error accessing to the templates folder.");
            }
            catch (Exception)
            {
                throw new TemplateException("Error accessing to the templates folder.");
            }

            return filename;
        }

        /// <inheritdoc/>
        public IEnumerable<Template> GetTemplates()
        {
            var portalSettings = PortalController.Instance.GetCurrentPortalSettings();
            var templateFolder = FolderManager.Instance.GetFolder(portalSettings.PortalId, TemplatesFolderPath);

            return this.LoadTemplates(portalSettings.PortalId, templateFolder);
        }

        /// <inheritdoc/>
        public int GetDefaultTemplateId(IEnumerable<Template> templates)
        {
            var firstOrDefault = templates.FirstOrDefault(t => t.Id == "Default");
            if (firstOrDefault != null)
            {
                return firstOrDefault.Value;
            }

            return Null.NullInteger;
        }

        /// <inheritdoc/>
        public void CreatePageFromTemplate(int templateId, TabInfo tab, int portalId)
        {
            // create the page from a template
            if (templateId != Null.NullInteger)
            {
                var xmlDoc = new XmlDocument { XmlResolver = null };
                try
                {
                    // open the XML file
                    var fileId = Convert.ToInt32(templateId);
                    var templateFile = FileManager.Instance.GetFile(fileId);
                    xmlDoc.Load(FileManager.Instance.GetFileContent(templateFile));
                }
                catch (Exception ex)
                {
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                    throw new PageException(Localization.GetString("BadTemplate"));
                }

                TabController.DeserializePanes(this.businessControllerProvider, xmlDoc.SelectSingleNode("//portal/tabs/tab/panes"), tab.PortalID, tab.TabID, PortalTemplateModuleAction.Ignore, new Hashtable());

                // save tab permissions
                RibbonBarManager.DeserializeTabPermissions(xmlDoc.SelectNodes("//portal/tabs/tab/tabpermissions/permission"), tab);

                var tabIndex = 0;
                var exceptions = string.Empty;

                // ReSharper disable once PossibleNullReferenceException
                foreach (XmlNode tabNode in xmlDoc.SelectSingleNode("//portal/tabs").ChildNodes)
                {
                    // Create second tab onward tabs. Note first tab is already created above.
                    if (tabIndex > 0)
                    {
                        try
                        {
                            TabController.DeserializeTab(this.businessControllerProvider, tabNode, null, portalId, PortalTemplateModuleAction.Replace);
                        }
                        catch (Exception ex)
                        {
                            DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                            exceptions += string.Format("Template Tab # {0}. Error {1}<br/>", tabIndex + 1, ex.Message);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(tab.SkinSrc) && !string.IsNullOrEmpty(XmlUtils.GetNodeValue(tabNode, "skinsrc", string.Empty)))
                        {
                            tab.SkinSrc = XmlUtils.GetNodeValue(tabNode, "skinsrc", string.Empty);
                        }

                        if (string.IsNullOrEmpty(tab.ContainerSrc) && !string.IsNullOrEmpty(XmlUtils.GetNodeValue(tabNode, "containersrc", string.Empty)))
                        {
                            tab.ContainerSrc = XmlUtils.GetNodeValue(tabNode, "containersrc", string.Empty);
                        }

                        bool isSecure;
                        if (bool.TryParse(XmlUtils.GetNodeValue(tabNode, "issecure", string.Empty), out isSecure))
                        {
                            tab.IsSecure = isSecure;
                        }

                        TabController.Instance.UpdateTab(tab);
                    }

                    tabIndex++;
                }

                if (!string.IsNullOrEmpty(exceptions))
                {
                    throw new PageException(exceptions);
                }
            }
        }

        /// <inheritdoc/>
        protected override Func<ITemplateController> GetFactory()
        {
            return Globals.DependencyProvider.GetRequiredService<ITemplateController>;
        }

        private static IFolderInfo GetTemplateFolder()
        {
            return FolderManager.Instance.GetFolder(PortalSettings.Current.PortalId, TemplatesFolderPath);
        }

        private static IFolderInfo CreateTemplateFolder()
        {
            return FolderManager.Instance.AddFolder(PortalSettings.Current.PortalId, TemplatesFolderPath);
        }

        private void SerializeTab(PageTemplate template, XmlDocument xmlTemplate, XmlNode nodeTabs)
        {
            var portalSettings = PortalController.Instance.GetCurrentPortalSettings();
            var tab = this.tabController.GetTab(template.TabId, portalSettings.PortalId, false);
            var xmlTab = new XmlDocument { XmlResolver = null };
            var nodeTab = TabController.SerializeTab(this.businessControllerProvider, xmlTab, tab, template.IncludeContent);
            nodeTabs.AppendChild(xmlTemplate.ImportNode(nodeTab, true));
        }

        private IEnumerable<Template> LoadTemplates(int portalId, IFolderInfo templateFolder)
        {
            var templates = new List<Template>();
            if (templateFolder == null)
            {
                return templates;
            }

            templates.Add(new Template
            {
                Id = Localization.GetString("None_Specified"),
                Value = Null.NullInteger,
            });

            var files = Globals.GetFileList(portalId, "page.template", false, templateFolder.FolderPath);
            foreach (FileItem file in files)
            {
                int i;
                int.TryParse(file.Value, out i);
                templates.Add(new Template
                {
                    Id = file.Text.Replace(".page.template", string.Empty),
                    Value = i,
                });
            }

            return templates;
        }
    }
}

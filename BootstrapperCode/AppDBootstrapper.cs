using Apprenda.API.Extension.Bootstrapping;
using Apprenda.API.Extension.CustomProperties;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace APMBootstrapper
{
    class AppDBootstrapper : Apprenda.API.Extension.Bootstrapping.BootstrapperBase
    {
        const string AppDApplicationPoolTemplate = @"<!--<applicationPool name=""##IISAPPLICATIONNAME##"" instrument=""true""/>-->";
        const string AppDApplicationPoolValue = @"<applicationPool name=""##IISAPPLICATIONNAME##"" instrument=""true""/>";
        /// <summary>
        /// If a Custom Property "APM Application Name" exists, open up the application and add the AppDynamics
        /// specific configuration inside its *.config files
        /// </summary>
        public override BootstrappingResult Bootstrap(BootstrappingRequest bootstrappingRequest)
        {
            // if the app is a web application hosted in IIS, we need to drop the AppDynamics HTTP Module in the folder and modify the web.config
            if (bootstrappingRequest.ComponentType == ComponentType.PublicAspNet || bootstrappingRequest.ComponentType == ComponentType.AspNet)
            {
                // to get here, it means that the bootstrap policy matched the requirements of an application and we need to
                // make some changes to it based on its custom properties
                try
                {
                    var appName = bootstrappingRequest.ApplicationAlias; // use the ApplicationAlias as a default                
                    foreach (CustomProperty property in bootstrappingRequest.Properties)
                    {
                        // only save the first property value in case any of these properties are multi-value custom properties                    
                        if (property.Name == "APM Application Name")
                        {
                            foreach (var value in property.Values)
                            {
                                appName = value;
                                if (string.IsNullOrWhiteSpace(appName))
                                {
                                    return BootstrappingResult.Failure(new[] { "The Application Name specified for this application is invalid" });
                                }
                                break;
                            }
                        }
                    }

                    var modifyResult = ModifyConfigFiles(bootstrappingRequest, appName);
                    return !modifyResult.Succeeded ? modifyResult : BootstrappingResult.Success();
                }
                catch (Exception e)
                {
                    return BootstrappingResult.Failure(new[] { e.Message });
                }
            }
            return BootstrappingResult.Success();
        }

        /// <summary>
        /// Find all the files ending in *.config and modify them to add the AppDynamics specific XML elements
        /// Imagine in the future we can use a Custom Property to instruct us which specific .config files to instrument
        /// 
        /// Also, copy the AppD.config file with the subscription ID in the root folder of the application
        /// </summary>
        /// <param name="appName">The name of the application. This name will show up in the AppDynamics monitoring dashboard. The developer specifies this name in the Apprenda Custom Properties for her application</param>        
        private static BootstrappingResult ModifyConfigFiles(BootstrappingRequest bootstrappingRequest, string appName)
        {
            string[] configFiles = Directory.GetFiles(bootstrappingRequest.ComponentPath, "web.config", SearchOption.AllDirectories);
            if (configFiles.Length <= 0)
            {
                return BootstrappingResult.Failure(new[] { "Failed to find a web.config for application " + appName });
            }

            foreach (string file in configFiles)
            {
                var result = ModifyXML(bootstrappingRequest, file, appName);
                if (!result.Succeeded)
                {
                    return result;
                }
            }

            var srcHttpModuleFilePath = Path.Combine(bootstrappingRequest.BootstrapperPath, @"AppDynamicsHttpModule.dll");
            var destinationBasePath = bootstrappingRequest.ComponentPath;

            // if this is a UI, the componentpath will not take you into the root folder of the IIS application. so we need to append it
            destinationBasePath = Path.Combine(destinationBasePath, bootstrappingRequest.ComponentName);

            var dstHttpModuleFilePath = Path.Combine(destinationBasePath, @"bin");
            dstHttpModuleFilePath = Path.Combine(dstHttpModuleFilePath, @"AppDynamicsHttpModule.dll");
            File.Copy(srcHttpModuleFilePath, dstHttpModuleFilePath, true);

            // now copy the config file that has the application name and tier name in there
            // and then open the files and replace the right values in there
            var dstConfigFilePath = Path.Combine(destinationBasePath, @"AppDynamics.config");
            File.Copy(Path.Combine(bootstrappingRequest.BootstrapperPath, @"AppDynamics.config"), dstConfigFilePath, true);

            File.WriteAllText(dstConfigFilePath, Regex.Replace(File.ReadAllText(dstConfigFilePath), "##APPLICATION_NAME##", appName));
            File.WriteAllText(dstConfigFilePath, Regex.Replace(File.ReadAllText(dstConfigFilePath), "##APPLICATION_TIER_NAME##", "Web Sites"));

            return BootstrappingResult.Success();
        }

        /// <summary>
        /// Edit the specified .config file and add the AppDynamics xml elements
        /// Assumes the AppDynamics agent is installed on all the Apprenda nodes and it is set to instrument all applications (IIS-based and non-IIS-based)
        /// Assumes the AppD.config contains at least the following content to instrument the WCF container for Apprenda apps:
        ///     <instrumentation>
        ///         <applications>
        ///             <application name="Apprenda.WCFServiceHost.exe" />
        ///         </applications>
        ///     </instrumentation>
        /// </summary>
        private static BootstrappingResult ModifyXML(BootstrappingRequest bootstrappingRequest, string filePath, string appName)
        {
            // because we need to work with any *.config files, we can't use the webconfigurationmanager class            
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            
            /*
            XmlNode appsettingsNode = xmlDoc.SelectSingleNode("//appSettings");
            if (null == appsettingsNode)
            {
                // we didn't find an appSettings Node - creating it                
                xmlDoc.AppendChild(CreateNewNode("appSettings", xmlDoc));
                appsettingsNode = xmlDoc.SelectSingleNode("//appSettings");
            }
            */

            XmlElement httpModuleElement = (XmlElement)xmlDoc.SelectSingleNode("//system.webServer/modules/add[@name = 'AppDynamicsHttpModule']");
            if (null == httpModuleElement)
            {
                XmlNode webServerNode = xmlDoc.SelectSingleNode("//system.webServer");
                if (null == webServerNode)
                {
                    xmlDoc.AppendChild(CreateNewNode("system.webServer", xmlDoc));
                    webServerNode = xmlDoc.SelectSingleNode("//system.webServer");
                }
                XmlNode modulesNode = xmlDoc.SelectSingleNode("//system.webServer/modules");
                if (null == modulesNode)
                {
                    webServerNode.AppendChild(CreateNewNode("modules", xmlDoc));
                    modulesNode = xmlDoc.SelectSingleNode("//system.webServer/modules");
                }
                
                modulesNode.AppendChild(CreateNewElement("name", "AppDynamicsHttpModule", "type", "AppDynamics.AppDynamicsHttpModule, AppDynamicsHttpModule", xmlDoc, modulesNode));
            }
            else
            {
                // modify an existing value
                httpModuleElement.Attributes["type"].Value = "AppDynamics.AppDynamicsHttpModule, AppDynamicsHttpModule";
            }

            /*
            XmlElement appNameElement = (XmlElement)xmlDoc.SelectSingleNode("//appSettings/add[@key = 'APPDYNAMICS_AGENT_APPLICATION_NAME']");
            XmlElement tierNameElement = (XmlElement)xmlDoc.SelectSingleNode("//appSettings/add[@key = 'APPDYNAMICS_AGENT_TIER_NAME']");

            if (null == appNameElement)
            {
                // value does not exist, let's create it
                appsettingsNode.AppendChild(CreateNewElement("key", "APPDYNAMICS_AGENT_APPLICATION_NAME", "value", appName, xmlDoc, appsettingsNode));
            }
            else
            {
                // modify an existing value
                appNameElement.Attributes["value"].Value = appName;
            }

            if (null == tierNameElement)
            {
                // value does not exist, let's create it
                appsettingsNode.AppendChild(CreateNewElement("key", "APPDYNAMICS_AGENT_TIER_NAME", "value", "Web Sites", xmlDoc, appsettingsNode));
            }
            else
            {
                // modify an existing value
                tierNameElement.Attributes["value"].Value = "Web Sites";
            }      
            */

            xmlDoc.Save(filePath);

            return BootstrappingResult.Success();
        }

        private static XmlNode CreateNewElement(string attribute1Name, string attribute1Value, string attribute2Name, string attribute2Value, XmlDocument xmlDoc, XmlNode node)
        {
            XmlNode newElement = xmlDoc.CreateNode(XmlNodeType.Element, "add", null);
            XmlAttribute attribute1 = xmlDoc.CreateAttribute(attribute1Name);
            attribute1.Value = attribute1Value;
            newElement.Attributes.Append(attribute1);
            XmlAttribute attribute2 = xmlDoc.CreateAttribute(attribute2Name);
            attribute2.Value = attribute2Value;
            newElement.Attributes.Append(attribute2);
            return newElement;
        }

        private static XmlNode CreateNewNode(string nodeName, XmlDocument xmlDoc)
        {
            XmlNode newElement = xmlDoc.CreateNode(XmlNodeType.Element, nodeName, null);
            return newElement;
        }
    }
}


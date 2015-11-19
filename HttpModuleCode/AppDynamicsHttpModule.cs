using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace AppDynamics
{
    public class AppDynamicsHttpModule : IHttpModule
    {
        private string AppDynamicsName;
        private string AppDynamicsTier;

        public void Init(HttpApplication app)
        {
            //string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase); // not used because it returns path in UNC-format
            //string appdynamicsConfigFile = Directory.GetParent(currentDir).FullName;
            string appdynamicsConfigDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] configFiles = Directory.GetFiles(appdynamicsConfigDir, "AppDynamics.config", SearchOption.TopDirectoryOnly);
            if (configFiles.Length != 1)
            {
                throw new Exception("Failed to find the AppDynamics.config file at " + appdynamicsConfigDir);
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFiles[0]);
            XmlNode appsettingsNode = xmlDoc.SelectSingleNode("//appSettings");
            if (null == appsettingsNode)
            {
                throw new Exception("Failed to find the appSettings node in AppDynamics.config file at " + appdynamicsConfigDir);
            }

            XmlElement appNameElement = (XmlElement)xmlDoc.SelectSingleNode("//appSettings/add[@key = 'APPDYNAMICS_AGENT_APPLICATION_NAME']");
            XmlElement tierNameElement = (XmlElement)xmlDoc.SelectSingleNode("//appSettings/add[@key = 'APPDYNAMICS_AGENT_TIER_NAME']");

            if (null == appNameElement || null == tierNameElement)
            {
                throw new Exception("Failed to find the AppDynamics elements in node in AppDynamics.config file at " + appdynamicsConfigDir);
            }

            this.AppDynamicsName = appNameElement.Attributes["value"].Value;
            this.AppDynamicsTier = tierNameElement.Attributes["value"].Value;

            app.BeginRequest += new EventHandler(this.OnBeginRequest);            
        }

        public void Dispose()
        {
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {          
            Environment.SetEnvironmentVariable("APPDYNAMICS_AGENT_APPLICATION_NAME", this.AppDynamicsName, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("APPDYNAMICS_AGENT_TIER_NAME", this.AppDynamicsTier, EnvironmentVariableTarget.Process);
        }
    }
}

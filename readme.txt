<h1>Apprenda-AppDynamics-Integration</h1>

This contains the bootstrap policy code and the relevant artifacts needed to enable/integrate APM from AppDynamics with Apprenda Applications

<h2>Requirements</h2>
- Custom Property "APM Application Name" exists on each Apprenda Application. Each application that wants to enable APM should fill in that custom property. The custom property should apply to Applications, Allows a custom value, and is visible and editable by developers.
- AppDynamics .NET Agent is installed with full instrumentation capabilities for IIS and non-IIS .NET applications on every Apprenda platform node that can host applications. The global AppDynamics config file should enable the instrumentation of all IIS application pools.
- Modify the C:\ProgramData\AppDynamics\DotNetAgent\Config\config.xml to add the following XML so that AppDynamics can monitor the Apprenda WCF self-hosted executable. Add the XML as a child of the <app-agents> node
<standalone-applications>
    <standalone-application executable="Apprenda.WCFServiceHost.exe"><tier name="Web Services"/></standalone-application>
</standalone-applications>

<h2>Additional Information</h2>
Enhancing Apprenda applications to work with your standardized corporate-wide APM tool is simple through the power of Apprenda extensibility and bootstrap policies. To learn more about bootstrap policies, visit the following links:
- http://docs.apprenda.com/5-5/bootstrap-policies
- http://docs.apprenda.com/5-5/custom-bootstrapper
- http://apprenda.com/blog/customize-paas-application-bootstrap-policies/

To get a better understanding of this integration, view the following videos that guide you step-by-step on this integration.
- TBD

<h2>Repository Contents</h2>
- HttpModuleCode contains the code to create an HTTP Module that is used to inject data into the IIS-based applications
- BootstrapperCode contains the code for the Apprenda Bootstrap policy. The bootstrap policy installs the HTTP module for IIS based workloads and creates the necessary web.config properties. For example the following line is added to web.config under the \\system.webserver\modules node
-- <add name="AppDynamicsHttpModule" type="AppDynamics.AppDynamicsHttpModule, AppDynamicsHttpModule" />
- BootstrapperBinariesandFolderStructure contains the individual files and the ZIP file that can be uploaded to Apprenda to create the bootstrap policy (BSP). The operator has full freedom to create the proper triggers for the BSP. In my example, I used logic that keyed off the Custom Property "APM Application Name"
- SampleApprendaApplication contained a sample 3-tier .NET Application that can be uploaded on Apprenda to show off the APM capabilities of AppDynamics. This application has defined in the manifest two Environment Variables that AppDynamics and the BSP require. The application manifest also defines the Custom Property "APM Application Name".
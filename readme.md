<h1>Apprenda-AppDynamics-Integration</h1>

<h2>Getting Started</h2>
<h3>Overview</h3>
As a foundational software layer and application run-time environment, Apprenda removes the complexities of building and delivering modern software applications, enabling enterprises to turn ideas into innovations faster. Apprenda allows you to leverage existing skillsets and investments in infrastructure and developer tools, and provides the catalyst to achieve greater returns and higher productivity. One such existing investment could be Application Performance Monitoring, or APM in short. 

AppDynamics accelerates your company's digital transformation and optimizes every customer experience. As a software analytics company, AppDynamics monitors and manages complex applications, captures metrics and KPIs from your applications, and helps identify and resolve performance and application issues quickly. AppDynamics, through their advanced analytic and user insight capabilities, helps developers of modern applications make sense of the data, performance tune their apps, and make data driven decisions to improve the user experience. The Application Intelligence capabilities of AppDynamics allows developers to understand how end-users are interacting with the Apprenda applications, and what is the experience my end users receive throughout the day.

This integration enhances applications running on Apprenda’s private platform as a service (PaaS) with AppDynamics monitoring. The integration is made possible by the advanced extensibility that Apprenda offers through bootstrap policies.

The power of Apprenda benefits coupled with deep insights offered by advanced monitoring of your application will be a net benefit to your developers, their end users and the overall application experience. If your enterprise has standardized corporate wide on AppDynamics, you can enable it with a few simple clicks and light up advanced monitoring for Apprenda applications.

<h3>Release Notes</h3>
- Version 1.0

<h3>Features</h3>
With AppDynamics monitoring Apprenda applications, developers can:
- Monitor multi-instance distributed applications in real time
- Monitor how the application is interacting with the database and gain access to query latencies
- Monitor performance metrics and the overall health of an application
- Monitor Key Performance Indicators (KPIs) and define custom metrics that provide insight into the internal behavior of the application
- Monitor how end users are using an app and the experience they receive throughout the day
- Quickly diagnose bottlenecks in an application 
- Receive proactive alerts when the application is falling behind its SLA

<h3>License</h3>
Copyright (c) 2016 Apprenda Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

<h2>Installation, Configuration, and Usage</h2>
Open the AppDynamicsInstallationGuideForApprenda.docx file for a step-by-step installation, configuration, and usage guide

<h2>Additional Information</h2>
Application Bootstrap Policies (BSP) on the Apprenda Platform allow Platform Operators to set specific rules governing inclusion of specified libraries with the deployment of .Net UI, WCF/Windows Services, and Java Web Application components. These policies apply comparison logic set by the Platform Operator to match specified libraries to the desired application components. The ability to add and extend the functionality of guest applications through policy is one of the greatest and most powerful features of Apprenda.

Enhancing Apprenda applications to work with your standardized corporate-wide deployment of AppDynamics is simple through the power of Apprenda extensibility and bootstrap policies. To learn more about bootstrap policies, visit the following links:
- http://docs.apprenda.com/6-5/bootstrap-policies
- http://docs.apprenda.com/6-5/custom-bootstrapper
- http://apprenda.com/blog/customize-paas-application-bootstrap-policies/

To get a better understanding of this integration through a step-by-step video turorial, visit one of the two following links:
- https://apprenda.wistia.com/medias/ck8afudkk1
- https://www.youtube.com/watch?v=kKTsi5qsWSs

<h2>Repository Contents</h2>
This GitHub repository contains the bootstrap policy code and the relevant artifacts needed to light up APM from AppDynamics for Apprenda Applications
- HttpModuleCode contains the code to create an HTTP Module that is used to inject data into the IIS-based applications. AppDynamics requires this to create IIS-based applications. The module is injected into the Apprenda application through the bootstrap policy
- BootstrapperCode contains the code for the Apprenda Bootstrap policy. The bootstrap policy installs the HTTP module for IIS based workloads and creates the necessary web.config properties. For example the following line is added to web.config under the ```\\system.webserver\modules``` node
  - ```<add name="AppDynamicsHttpModule" type="AppDynamics.AppDynamicsHttpModule, AppDynamicsHttpModule" />```
- BootstrapperBinariesandFolderStructure contains the individual files and the ZIP file that can be uploaded to Apprenda to create the bootstrap policy (BSP). The operator has full freedom to create the proper triggers for the BSP. In my example, I used logic that keyed off the Custom Property "APM Application Name"
- SampleApprendaApplication contains a sample 3-tier .NET Application that can be uploaded on Apprenda to show off the APM capabilities of AppDynamics. 
  - This application has defined in the manifest two Environment Variables that AppDynamics and the BSP require. 
  - The application manifest also defines the Custom Property "APM Application Name".
- AppDynamicsSampleConfigFile contains a sample AppDynamics config.xml file that you can set up on each Apprenda node.
  - Make sure that once you install the AppDynamics agent and set up the config.xml, it would be highly recommended to reboot the windows node
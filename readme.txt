add to C:\ProgramData\AppDynamics\DotNetAgent\Config\config.xml as a child of the <app-agents> node
<standalone-applications>
    <standalone-application executable="Apprenda.WCFServiceHost.exe"><tier name="Web Services"/></standalone-application>
</standalone-applications>

/* web.config registration of the app
 * <configuration>
    <system.web>
        <httpModules>
            <!-- <add name="AppDynamics" 
                      type="AppDynamics.AppDynamicsHttpModule, AppDynamicsHttpModule" /> -->
        </httpModules>
    </system.web>
</configuration>
 * */

appdynamics.config, a new file in the app
<<>>

create Custom Property APM Application Name \ Applies to Applications\ Allow custom value \ visible+editable by developers

manifest of apprenda app - WCF config
    <services>
	  <service name="TimeCard.Service">
	    <environmentVariables>		
		  <variable name="APPDYNAMICS_AGENT_TIER_NAME" value="WCF Services" />
		  <variable name="APPDYNAMICS_AGENT_APPLICATION_NAME" value="TimecardM2" />
		</environmentVariables>
	  </service>
    </services>	
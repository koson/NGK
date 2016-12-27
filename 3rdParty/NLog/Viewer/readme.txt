http://www.gibraltarsoftware.com/loupe/extensions/nlog

Integration is easy

Just add an assembly reference and a few lines of code or XML.
See details for ASP.NET Webforms, ASP.NET MVC, WinForms, WPF, Services and Entity Framework or read our complete NLog integration documentation.

XML
<nlog>
  <extensions>
    <add assembly="Gibraltar.Agent.NLog" />
  </extensions>
  <targets>
    <target name="Gibraltar" xsi:type="Gibraltar" />
  </targets>
  <rules>
    <logger name="*" minlevel="Trace" writeTo="Gibraltar" />
  </rules>
</nlog>


https://www.youtube.com/watch/?v=RIxww3cmbkM
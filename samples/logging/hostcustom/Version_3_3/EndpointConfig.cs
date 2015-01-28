using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using NServiceBus;

#region Config
public class EndpointConfig : 
    IConfigureThisEndpoint, 
    AsA_Server,
    IWantCustomInitialization, 
    IWantCustomLogging
{
    void IWantCustomLogging.Init()
    {
        var layout = new PatternLayout
                     {
                         ConversionPattern = "%d %-5p %c - %m%n"
                     };
        layout.ActivateOptions();
        var appender = new ConsoleAppender
                       {
                           Layout = layout,
                           Threshold = Level.Info
                       };
        appender.ActivateOptions();

        BasicConfigurator.Configure(appender);

        SetLoggingLibrary.Log4Net();
    }
#endregion

    void IWantCustomInitialization.Init()
    {
        var configure = Configure.With();
        configure.DefineEndpointName("HostCustomLoggingSample");
        configure.DefaultBuilder();
        configure.InMemorySagaPersister();
        configure.UseInMemoryTimeoutPersister();
        configure.InMemorySubscriptionStorage();
        configure.JsonSerializer();
    }
}
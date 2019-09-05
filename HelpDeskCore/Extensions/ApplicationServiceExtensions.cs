using CoreTools.Http;
using HelpDeskCore.Services.Emails;
using HelpDeskCore.Services.Imports;
using HelpDeskCore.Services.Notifications;
using HelpDeskCore.Services.Views;
using HelpDeskCore.Shared.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDeskCore.Extensions
{
  /// <summary>
  /// Provides extension methods for classes that implement the <see cref="IApplicationBuilder"/> and the <see cref="IServiceCollection"/> interfaces.
  /// </summary>
  public static class ApplicationServiceExtensions
  {
    /// <summary>
    /// Adds services used by the application to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The collection to which services are added.</param>
    /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddHelpDeskCoreServices(this IServiceCollection services)
    {
      return services
        .AddTransient<IViewRenderService, ViewRenderService>()
        .AddTransient<IUserDataImporter, UserDataImporter>()
        .AddTransient<IIssueEmailProducer, IssueEmailProducer>()
        .AddTransient<IMessageProducer, SingletonMessageProducer>()
        .AddTransient<IEmailSender, EmailSender>()
        .AddTransient<IMessageConsumer, MessageConsumer>()
        .AddSingleton<Microsoft.Extensions.Hosting.IHostedService, EmailDispatcher>();
    }
  }
}

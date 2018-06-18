using HelpDeskCore.Services.Emails;
using HelpDeskCore.Shared.Messaging;
using Microsoft.Extensions.Logging;

namespace HelpDeskCore.Services.Notifications
{
  /// <summary>
  /// Represents a message producer that uses the singleton <see cref="EmailDispatcher"/> instance as the consumer.
  /// </summary>
  public sealed class SingletonMessageProducer : MessageProducer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SingletonMessageProducer"/> class using the specified parameter.
    /// </summary>
    /// <param name="logger">An object used to report incidences.</param>
    public SingletonMessageProducer(ILogger<SingletonMessageProducer> logger) : base(EmailDispatcher.Instance, logger)
    {
    }
  }
}

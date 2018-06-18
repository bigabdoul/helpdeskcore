namespace HelpDeskCore.Shared.Messaging
{
  /// <summary>
  /// Encapsulates data that controls the behaviour of an instance of the <see cref="MessageConsumer"/> class.
  /// </summary>
  public class MessageDispatchOptions
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDispatchOptions"/> class.
    /// </summary>
    public MessageDispatchOptions()
    {
    }

    /// <summary>
    /// The number of seconds to elapse between each tick of the timer. Use -1 to disable the timer.
    /// </summary>
    public int TimerPeriodInSeconds { get; set; } = 300;

    /// <summary>
    /// The amount of time, in seconds, to delay before the timer is invoked for the first time.
    /// Use 0 to start the timer immediately. To prevent the timer from starting, use -1.
    /// </summary>
    public int TimerInitialDelayInSeconds { get; set; } = 60;
  }
}

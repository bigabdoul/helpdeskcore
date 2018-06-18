using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreTools.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HelpDeskCore.Shared.Messaging
{
    /// <summary>
    /// Represents a service that broadcasts notification messages to subscribers.
    /// </summary>
    public class MessageConsumer : ApplicationService, IMessageConsumer
    {
        #region fields

        Timer _timer;
        bool _timerStarted;
        bool _disabled;
        bool _dispatching;
        readonly MessageDispatchOptions _options;
        readonly object _synclock = new object();
        readonly ConcurrentQueue<INotificationMessage> _msgQ = new ConcurrentQueue<INotificationMessage>();

        /// <summary>
        /// References the logger for this message dispatcher.
        /// </summary>
        protected readonly ILogger<MessageConsumer> Logger;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageConsumer"/> class.
        /// </summary>
        /// <param name="logger">An object used to report incidences.</param>
        /// <param name="options">The options to use for the current <see cref="IMessageConsumer"/>.</param>
        public MessageConsumer(ILogger<MessageConsumer> logger, IOptions<MessageDispatchOptions> options)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        #endregion

        #region events
        
        /// <summary>
        /// Event fired when broadcasting messages in method <see cref="ExecuteAsync(CancellationToken)"/>.
        /// </summary>
        protected event AsyncEventHandler<NotificationEventArgs> Dispatch;

        #endregion

        #region properties

        /// <summary>
        /// If true, will gracefully stop everything the background service is doing right now; otherwise, starts the service.
        /// </summary>
        public virtual bool Disabled
        {
            get => _disabled;
            set
            {
                if (value != _disabled)
                {
                    _disabled = value;
                    if (_disabled)
                    {
                        StopAsync(default(CancellationToken));
                    }
                    else
                    {
                        StartAsync(default(CancellationToken));
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the consumer is busy or not.
        /// </summary>
        public bool Dispatching { get => _dispatching; protected set => _dispatching = value; }

        #endregion

        #region INotificationMessage

        /// <summary>
        /// Enqueues a notification message.
        /// </summary>
        /// <param name="message">The notification message to enqueue.</param>
        public virtual void Enqueue(INotificationMessage message)
        {
            _msgQ.Enqueue(message);
        }

        /// <summary>
        /// Dispatch messages when service inactive.
        /// </summary>
        public virtual void Notify()
        {
            if (!_disabled && !_dispatching)
            {
                Execute();
            }
        }

        /// <summary>
        /// Dequeues and returns all notification messages currently stored in the internal queue.
        /// </summary>
        /// <returns>A collection of <see cref="INotificationMessage"/> objects.</returns>
        public virtual IEnumerable<INotificationMessage> DequeueAll()
        {
            while (_msgQ.Count > 0)
            {
                if (_msgQ.TryDequeue(out var m))
                {
                    yield return m;
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="MessageConsumer"/>.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<INotificationMessage> GetEnumerator() => _msgQ.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Timer

        /// <summary>
        /// Asynchronously stops the background service.
        /// </summary>
        /// <param name="cancellationToken">The token used to cancel an ongoing async operation.</param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            StopTimer();
            return base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously starts the background service.
        /// </summary>
        /// <param name="cancellationToken">The token used to cancel an ongoing async operation.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">The service is currently disabled.</exception>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            if (_disabled) throw new InvalidOperationException("The service is currently disabled.");
            var task = base.StartAsync(cancellationToken);
            StartTimer();
            return task;
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="secondsInterval">The period of the timer. If null, the default options are used.</param>
        /// <returns>A reference to the current <see cref="MessageConsumer"/> instance.</returns>
        public virtual MessageConsumer StartTimer(int? secondsInterval = null)
        {
            if (!_timerStarted)
            {
                lock (_synclock)
                {
                    if (!_timerStarted)
                    {
                        var period = (secondsInterval ?? _options.TimerPeriodInSeconds);
                        var delay = _options.TimerInitialDelayInSeconds;

                        if (period < 0 || delay < 0)
                        {
                            return this;
                        }

                        // convert to milliseconds
                        period *= 1000;
                        delay *= 1000;

                        if (_timer == null)
                        {
                            _timer = new Timer(async (state) =>
                            {
                                await ExecuteAsync(CancellationTokenSource.Token);
                            }, null, delay, period);
                        }
                        else
                        {
                            _timer.Change(0, period);
                        }

                        _timerStarted = true;
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        /// <returns>A reference to the current <see cref="MessageConsumer"/> instance.</returns>
        public virtual MessageConsumer StopTimer()
        {
            if (_timerStarted)
                lock (_synclock)
                {
                    if (_timerStarted)
                    {
                        _timer.Change(Timeout.Infinite, Timeout.Infinite);
                        _timerStarted = false;
                    }
                }

            return this;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Immediately starts dispatching messages.
        /// </summary>
        protected MessageConsumer Execute()
        {
#pragma warning disable 4014
            ExecuteAsync(default(CancellationToken));
#pragma warning restore 4014
            return this;
        }

        /// <summary>
        /// Asynchronously dispatches messages. This method requires a handler for the <see cref="Dispatch"/> event.
        /// </summary>
        /// <param name="cancellationToken">The token used to cancel the operation if desired.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">No handler for event <see cref="Dispatch"/>.</exception>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (_dispatching || _disabled || _msgQ.Count == 0) return;
            if (Dispatch == null) throw new InvalidOperationException($"No handler for event {nameof(Dispatch)}.");

            _dispatching = true;
            try
            {
                var cts = CreateLinkedTokenSource(cancellationToken);
                
                foreach (var m in DequeueAll())
                {
                    await Dispatch(this, new NotificationEventArgs(m));

                    if (cts.IsCancellationRequested)
                        break;
                }

                cts.Token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException ex)
            {
                Logger.LogInformation(ex, $"Task was canceled in {nameof(ExecuteAsync)}.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"An error occured in method {nameof(ExecuteAsync)}.");
            }
            finally
            {
                _dispatching = false;
            }
        }

        #endregion
    }
}

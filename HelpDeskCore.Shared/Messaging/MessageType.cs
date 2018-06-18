namespace HelpDeskCore.Shared.Messaging
{
    /// <summary>
    /// Provides enumerations for message types.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Success message.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Informal message.
        /// </summary>
        Info,

        /// <summary>
        /// Warning message.
        /// </summary>
        Warn,

        /// <summary>
        /// Error message.
        /// </summary>
        Error,
    }
}

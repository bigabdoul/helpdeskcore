namespace HelpDeskCore.Shared.Logging
{
    /// <summary>
    /// Exposes typical properties of an item that can be logged.
    /// </summary>
    public interface ILogItem
    {
        /// <summary>
        /// The log item identifier.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// The name of the logged item.
        /// </summary>
        string Name { get; set; }
    }
}

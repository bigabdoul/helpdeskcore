using HelpDeskCore.Shared.Logging;
using Microsoft.AspNetCore.Identity;

namespace HelpDeskCore.Data.Logging
{
    /// <summary>
    /// Represents a generic system event data container.
    /// </summary>
    /// <typeparam name="TEntity">The type of event data.</typeparam>
    public class SysEventArgs<TEntity> : SysEventArgs where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SysEventArgs{TEntity}"/> class using the specified parameter.
        /// </summary>
        /// <param name="type">The type of the event.</param>
        public SysEventArgs(SysEventType type) : base(type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SysEventArgs{TEntity}"/> class using the specified parameter.
        /// </summary>
        /// <param name="e">An instance of the <see cref="SysEventArgs"/> class to initialize with.</param>
        /// <exception cref="System.InvalidCastException">
        /// Either <see cref="SysEventArgs.Data"/> or <see cref="SysEventArgs.User"/> cannot be converted to the generic entity type or to <see cref="IdentityUser"/> respectively.
        /// </exception>
        public SysEventArgs(SysEventArgs e) : base(e.EventType)
        {
            Data = (TEntity)e.Data;
            User = (IdentityUser)e.User;
            Error = e.Error;
            ObjectState = e.ObjectState;
        }

        /// <summary>
        /// Gets or sets the entity-related event data.
        /// </summary>
        public new TEntity Data { get => (TEntity)base.Data; set => base.Data = value; }

        /// <summary>
        /// Gets or sets the identity user who caused the event.
        /// </summary>
        public new IdentityUser User { get => (IdentityUser)base.User; set => base.User = value; }
    }
}

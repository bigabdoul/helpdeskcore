using System;
using System.Threading.Tasks;

namespace HelpDeskCore.Shared
{
    /// <summary>
    /// A generic delegate suitable for asynchronously handling events.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
    /// <param name="sender">The object that fired the event.</param>
    /// <param name="e">The event data.</param>
    /// <returns></returns>
    public delegate Task AsyncEventHandler<TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs;
}

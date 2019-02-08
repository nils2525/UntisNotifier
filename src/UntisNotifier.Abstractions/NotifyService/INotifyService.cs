using System.Collections;
using System.Collections.Generic;
using UntisNotifier.Abstractions.Models;

namespace UntisNotifier.Abstractions.NotifyService
{
    /// <summary>
    /// Classes that implement that interface
    /// can be notified through the UntisNotifier
    /// </summary>
    public interface INotifyService
    {
        /// <summary>
        /// Is called when lessons have changed.
        /// </summary>
        /// <param name="lessons">lessons that have been changed</param>
        /// <returns>whether it was notified or not</returns>
        bool Notify(IEnumerable<Lesson> lessons);
    }
}
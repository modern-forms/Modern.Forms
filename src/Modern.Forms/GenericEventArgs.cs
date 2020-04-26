using System;

namespace Modern.Forms
{
    /// <summary>
    /// An EventArgs class with a generic Value parameter.
    /// </summary>
    public class EventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the EventArgs class.
        /// </summary>
        public EventArgs (T value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value of the event argument.
        /// </summary>
        public T Value { get; }
    }
}

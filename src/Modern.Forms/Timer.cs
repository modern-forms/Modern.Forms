using System.ComponentModel;
using Modern.WindowKit.Threading;

namespace Modern.Forms
{
    /// <summary>
    /// Implements a timer that raises an event at user-defined intervals.
    /// This timer is intended for UI-related scenarios and raises its <see cref="Tick"/>
    /// event on the UI thread.
    /// </summary>
    /// <remarks>
    /// Use this component when periodic work must be performed on the UI thread,
    /// such as updating UI state, animations, or scheduling lightweight tasks.
    ///
    /// This timer uses <see cref="DispatcherTimer"/> internally and is therefore
    /// integrated with the current UI dispatcher.
    /// </remarks>
    public class Timer : Component
    {
        private DispatcherTimer? dispatcherTimer;
        private int interval = 100;
        private bool enabled;
        private EventHandler? onTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        public Timer ()
        {
        }

        /// <summary>
        /// Occurs when the specified timer interval has elapsed and the timer is enabled.
        /// </summary>
        public event EventHandler Tick {
            add => onTimer += value;
            remove => onTimer -= value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the timer is running.
        /// </summary>
        [DefaultValue (false)]
        public bool Enabled {
            get => enabled;
            set {
                if (enabled == value)
                    return;

                enabled = value;

                if (enabled)
                    StartTimer ();
                else
                    StopTimer ();
            }
        }

        /// <summary>
        /// Gets or sets the time, in milliseconds, between timer ticks.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value is less than 1.
        /// </exception>
        [DefaultValue (100)]
        public int Interval {
            get => interval;
            set {
                ArgumentOutOfRangeException.ThrowIfLessThan (value, 1);

                if (interval == value)
                    return;

                interval = value;

                if (dispatcherTimer is not null)
                    dispatcherTimer.Interval = TimeSpan.FromMilliseconds (interval);
            }
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start () => Enabled = true;

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop () => Enabled = false;

        /// <summary>
        /// Raises the <see cref="Tick"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnTick (EventArgs e)
        {
            onTimer?.Invoke (this, e);
        }

        private void StartTimer ()
        {
            if (dispatcherTimer is null) 
            {
                dispatcherTimer = new DispatcherTimer ();
                dispatcherTimer.Tick += DispatcherTimer_Tick;
            }
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds (interval);
            dispatcherTimer.Start ();
        }

        private void StopTimer ()
        {
            dispatcherTimer?.Stop ();
        }

        private void DispatcherTimer_Tick (object? sender, EventArgs e)
        {
            OnTick (EventArgs.Empty);
        }

        /// <summary>
        /// Releases the resources used by the <see cref="Timer"/>.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release managed resources; otherwise, <see langword="false"/>.
        /// </param>
        protected override void Dispose (bool disposing)
        {
            if (disposing) {
                enabled = false;
                StopTimer ();

                if (dispatcherTimer is not null) {
                    dispatcherTimer.Tick -= DispatcherTimer_Tick;
                    dispatcherTimer = null;
                }

                onTimer = null;
            }

            base.Dispose (disposing);
        }

        /// <summary>
        /// Returns a string that represents the current timer.
        /// </summary>
        /// <returns>A string containing the type name and interval.</returns>
        public override string ToString ()
        {
            return $"{base.ToString ()}, Interval: {Interval}";
        }
    }
}

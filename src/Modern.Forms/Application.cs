using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Avalonia.Input;
using Avalonia.Threading;

namespace Modern.Forms
{
    public static class Application
    {
        private static CancellationTokenSource _mainLoopCancellationTokenSource;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is existing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is existing; otherwise, <c>false</c>.
        /// </value>
        internal static bool IsExiting { get; set; }

        public static void Run (Form form)
        {
            form.Show ();
            Run ((ICloseable)form);
        }

        /// <summary>
        /// Runs the application's main loop until the <see cref="ICloseable"/> is closed.
        /// </summary>
        /// <param name="closable">The closable to track</param>
        public static void Run (ICloseable closable)
        {
            if (_mainLoopCancellationTokenSource != null) {
                throw new Exception ("Run should only called once");
            }

            closable.Closed += (s, e) => AvaloniaExit ();

            _mainLoopCancellationTokenSource = new CancellationTokenSource ();

            Dispatcher.UIThread.MainLoop (_mainLoopCancellationTokenSource.Token);

            // Make sure we call OnExit in case an error happened and Exit() wasn't called explicitly
            if (!IsExiting) {
                OnExit?.Invoke (null, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Exits the application
        /// </summary>
        public static void AvaloniaExit ()
        {
            IsExiting = true;

            //Windows.Clear ();

            OnExit?.Invoke (null, EventArgs.Empty);

            _mainLoopCancellationTokenSource?.Cancel ();
        }

        /// <summary>
        /// Sent when the application is exiting.
        /// </summary>
        public static event EventHandler OnExit;
    }
}

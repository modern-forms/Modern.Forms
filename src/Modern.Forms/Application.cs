using System;
using System.Threading;
using Modern.WindowKit;
using Modern.WindowKit.Threading;

namespace Modern.Forms
{
    /// <summary>
    /// Provides static methods and properties to manage an application, such as methods to start and stop an application.
    /// </summary>
    public static class Application
    {
        private static CancellationTokenSource? _mainLoopCancellationTokenSource;
        private static bool is_exiting;

        /// <summary>
        /// This is the top level active menu, if any.
        /// This is used to hide menus if the user clicks elsewhere on the Form or off the Form.
        /// </summary>
        internal static MenuBase? ActiveMenu { get; set; }

        /// <summary>
        /// Exits the application.
        /// </summary>
        public static void Exit ()
        {
            is_exiting = true;

            OnExit?.Invoke (null, EventArgs.Empty);

            _mainLoopCancellationTokenSource?.Cancel ();
        }

        /// <summary>
        /// Raised when the application is exiting.
        /// </summary>
        public static event EventHandler? OnExit;

        /// <summary>
        /// Begins running a standard application message loop on the current thread, and makes the specified form visible.
        /// </summary>
        /// <param name="mainForm">A Form that represents the form to make visible.</param>
        public static void Run (Form mainForm)
        {
            mainForm.Show ();
            Run ((ICloseable)mainForm);
        }

        /// <summary>
        /// Runs the application's main loop until the <see cref="ICloseable"/> is closed.
        /// </summary>
        /// <param name="closable">The closable to track.</param>
        public static void Run (ICloseable closable)
        {
            if (_mainLoopCancellationTokenSource != null)
                throw new Exception ("Run should only called once");

            closable.Closed += (s, e) => Exit ();

            _mainLoopCancellationTokenSource = new CancellationTokenSource ();

            Dispatcher.UIThread.MainLoop (_mainLoopCancellationTokenSource.Token);

            // Make sure we call OnExit in case an error happened and Exit() wasn't called explicitly
            if (!is_exiting)
                OnExit?.Invoke (null, EventArgs.Empty);
        }

        /// <summary>
        /// Performs the desired Action on the UI thread.
        /// </summary>
        /// <param name="action">The action to perform on the UI thread.</param>
        public static void RunOnUIThread (Action action)
        {
            Dispatcher.UIThread.Post (action);
        }
    }
}

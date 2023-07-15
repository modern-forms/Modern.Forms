using System;

namespace Modern.Forms;

public abstract partial class WindowBase
{
    // This pattern is ugly, but it saves allocations
    // https://docs.microsoft.com/en-us/dotnet/standard/events/how-to-handle-multiple-events-using-event-properties
    private static readonly object s_keyDownEvent = new object ();
    private static readonly object s_keyPressEvent = new object ();
    private static readonly object s_keyUpEvent = new object ();

    /// <summary>
    /// Raised when the user presses down a key.
    /// </summary>
    public event EventHandler<KeyEventArgs>? KeyDown {
        add => Events.AddHandler (s_keyDownEvent, value);
        remove => Events.RemoveHandler (s_keyDownEvent, value);
    }

    /// <summary>
    /// Raised when the user presses a key.
    /// </summary>
    public event EventHandler<KeyPressEventArgs>? KeyPress {
        add => Events.AddHandler (s_keyPressEvent, value);
        remove => Events.RemoveHandler (s_keyPressEvent, value);
    }

    /// <summary>
    /// Raised when the user releases a key.
    /// </summary>
    public event EventHandler<KeyEventArgs>? KeyUp {
        add => Events.AddHandler (s_keyUpEvent, value);
        remove => Events.RemoveHandler (s_keyUpEvent, value);
    }

    /// <summary>
    /// Raises the KeyDown event.
    /// </summary>
    protected virtual void OnKeyDown (KeyEventArgs e) => (Events[s_keyDownEvent] as EventHandler<KeyEventArgs>)?.Invoke (this, e);

    /// <summary>
    /// Raises the KeyPress event.
    /// </summary>
    protected virtual void OnKeyPress (KeyPressEventArgs e) => (Events[s_keyPressEvent] as EventHandler<KeyPressEventArgs>)?.Invoke (this, e);

    /// <summary>
    /// Raises the KeyUp event.
    /// </summary>
    protected virtual void OnKeyUp (KeyEventArgs e) => (Events[s_keyUpEvent] as EventHandler<KeyEventArgs>)?.Invoke (this, e);
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

namespace Modern.Forms;

public partial class Control
{
    // This pattern is ugly, but it saves allocations
    // https://docs.microsoft.com/en-us/dotnet/standard/events/how-to-handle-multiple-events-using-event-properties
    private static readonly object s_autoSizeChangedEvent = new object ();
    private static readonly object s_clickEvent = new object ();
    private static readonly object s_contextMenuChangedEvent = new object ();
    private static readonly object s_controlAddedEvent = new object ();
    private static readonly object s_controlRemovedEvent = new object ();
    private static readonly object s_cursorChangedEvent = new object ();
    private static readonly object s_dockChangedEvent = new object ();
    private static readonly object s_doubleClickEvent = new object ();
    private static readonly object s_enabledChangedEvent = new object ();
    private static readonly object s_gotFocusEvent = new object ();
    private static readonly object s_invalidatedEvent = new object ();
    private static readonly object s_keyDownEvent = new object ();
    private static readonly object s_keyPressEvent = new object ();
    private static readonly object s_keyUpEvent = new object ();
    private static readonly object s_layoutEvent = new object ();
    private static readonly object s_locationChangedEvent = new object ();
    private static readonly object s_marginChangedEvent = new object ();
    private static readonly object s_mouseDownEvent = new object ();
    private static readonly object s_mouseEnterEvent = new object ();
    private static readonly object s_mouseLeaveEvent = new object ();
    private static readonly object s_mouseMoveEvent = new object ();
    private static readonly object s_mouseUpEvent = new object ();
    private static readonly object s_mouseWheelEvent = new object ();
    private static readonly object s_paddingChangedEvent = new object ();
    private static readonly object s_parentEvent = new object ();
    private static readonly object s_resizeEvent = new object ();
    private static readonly object s_sizeChangedEvent = new object ();
    private static readonly object s_tabIndexChangedEvent = new object ();
    private static readonly object s_tabStopChangedEvent = new object ();
    private static readonly object s_textChangedEvent = new object ();
    private static readonly object s_visibleChangedEvent = new object ();

    /// <summary>
    /// Raised when the AutoSize property is changed.
    /// </summary>
    public event EventHandler AutoSizeChanged {
        add => Events.AddHandler (s_autoSizeChangedEvent, value);
        remove => Events.RemoveHandler (s_autoSizeChangedEvent, value);
    }

    /// <summary>
    /// Raised when this control is clicked.
    /// </summary>
    public event EventHandler<MouseEventArgs>? Click {
        add => Events.AddHandler (s_clickEvent, value);
        remove => Events.RemoveHandler (s_clickEvent, value);
    }

    /// <summary>
    /// Raised when the ContextMenu property is changed
    /// </summary>
    public event EventHandler ContextMenuChanged {
        add => Events.AddHandler (s_contextMenuChangedEvent, value);
        remove => Events.RemoveHandler (s_contextMenuChangedEvent, value);
    }

    /// <summary>
    ///  Raised when a new control is added.
    /// </summary>
    public event EventHandler<EventArgs<Control>> ControlAdded {
        add => Events.AddHandler (s_controlAddedEvent, value);
        remove => Events.RemoveHandler (s_controlAddedEvent, value);
    }

    /// <summary>
    ///  Raised when a control is removed.
    /// </summary>
    public event EventHandler<EventArgs<Control>> ControlRemoved {
        add => Events.AddHandler (s_controlRemovedEvent, value);
        remove => Events.RemoveHandler (s_controlRemovedEvent, value);
    }

    /// <summary>
    /// Raised when the Cursor property is changed.
    /// </summary>
    public event EventHandler? CursorChanged {
        add => Events.AddHandler (s_cursorChangedEvent, value);
        remove => Events.RemoveHandler (s_cursorChangedEvent, value);
    }

    /// <summary>
    /// Raised when the Dock property is changed.
    /// </summary>
    public event EventHandler? DockChanged {
        add => Events.AddHandler (s_dockChangedEvent, value);
        remove => Events.RemoveHandler (s_dockChangedEvent, value);
    }

    /// <summary>
    /// Raised when this control is double-clicked.
    /// </summary>
    public event EventHandler<MouseEventArgs>? DoubleClick {
        add => Events.AddHandler (s_doubleClickEvent, value);
        remove => Events.RemoveHandler (s_doubleClickEvent, value);
    }

    /// <summary>
    /// Raised when the Enabled property is changed.
    /// </summary>
    public event EventHandler? EnabledChanged {
        add => Events.AddHandler (s_enabledChangedEvent, value);
        remove => Events.RemoveHandler (s_enabledChangedEvent, value);
    }

    /// <summary>
    /// Raised when the control receives focus.
    /// </summary>
    public event EventHandler? GotFocus {
        add => Events.AddHandler (s_gotFocusEvent, value);
        remove => Events.RemoveHandler (s_gotFocusEvent, value);
    }

    /// <summary>
    /// Raised when the Control is invalidated.
    /// </summary>
    public event EventHandler<EventArgs<Rectangle>>? Invalidated {
        add => Events.AddHandler (s_invalidatedEvent, value);
        remove => Events.RemoveHandler (s_invalidatedEvent, value);
    }

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
    /// Raised when the control performs a layout.
    /// </summary>
    public event EventHandler<LayoutEventArgs>? Layout {
        add => Events.AddHandler (s_layoutEvent, value);
        remove => Events.RemoveHandler (s_layoutEvent, value);
    }

    /// <summary>
    /// Raised when the Location property is changed.
    /// </summary>
    public event EventHandler? LocationChanged {
        add => Events.AddHandler (s_locationChangedEvent, value);
        remove => Events.RemoveHandler (s_locationChangedEvent, value);
    }

    /// <summary>
    /// Raised when the Margin property is changed.
    /// </summary>
    public event EventHandler? MarginChanged {
        add => Events.AddHandler (s_marginChangedEvent, value);
        remove => Events.RemoveHandler (s_marginChangedEvent, value);
    }

    /// <summary>
    /// Raised when a mouse button is pressed.
    /// </summary>
    public event EventHandler<MouseEventArgs>? MouseDown {
        add => Events.AddHandler (s_mouseDownEvent, value);
        remove => Events.RemoveHandler (s_mouseDownEvent, value);
    }

    /// <summary>
    /// Raised when the mouse cursor enters the control.
    /// </summary>
    public event EventHandler<MouseEventArgs>? MouseEnter {
        add => Events.AddHandler (s_mouseEnterEvent, value);
        remove => Events.RemoveHandler (s_mouseEnterEvent, value);
    }

    /// <summary>
    /// Raised when the mouse cursor leaves the control.
    /// </summary>
    public event EventHandler? MouseLeave {
        add => Events.AddHandler (s_mouseLeaveEvent, value);
        remove => Events.RemoveHandler (s_mouseLeaveEvent, value);
    }

    /// <summary>
    /// Raised when the mouse cursor is moved within the control.
    /// </summary>
    public event EventHandler<MouseEventArgs>? MouseMove {
        add => Events.AddHandler (s_mouseMoveEvent, value);
        remove => Events.RemoveHandler (s_mouseMoveEvent, value);
    }

    /// <summary>
    /// Raised when a mouse button ir released.
    /// </summary>
    public event EventHandler<MouseEventArgs>? MouseUp {
        add => Events.AddHandler (s_mouseUpEvent, value);
        remove => Events.RemoveHandler (s_mouseUpEvent, value);
    }

    /// <summary>
    /// Raised when a mouse wheel is rotated.
    /// </summary>
    public event EventHandler<MouseEventArgs>? MouseWheel {
        add => Events.AddHandler (s_mouseWheelEvent, value);
        remove => Events.RemoveHandler (s_mouseWheelEvent, value);
    }

    /// <summary>
    /// Raised when the Padding property is changed.
    /// </summary>
    public event EventHandler? PaddingChanged {
        add => Events.AddHandler (s_paddingChangedEvent, value);
        remove => Events.RemoveHandler (s_paddingChangedEvent, value);
    }

    /// <summary>
    /// Raised when the Parent property is changed.
    /// </summary>
    public event EventHandler ParentChanged {
        add => Events.AddHandler (s_parentEvent, value);
        remove => Events.RemoveHandler (s_parentEvent, value);
    }

    /// <summary>
    ///  Raised when the control is resized.
    /// </summary>
    public event EventHandler Resize {
        add => Events.AddHandler (s_resizeEvent, value);
        remove => Events.RemoveHandler (s_resizeEvent, value);
    }

    /// <summary>
    /// Raised when the Size property is changed.
    /// </summary>
    public event EventHandler? SizeChanged {
        add => Events.AddHandler (s_sizeChangedEvent, value);
        remove => Events.RemoveHandler (s_sizeChangedEvent, value);
    }

    /// <summary>
    /// Raised when the TabIndex property is changed.
    /// </summary>
    public event EventHandler? TabIndexChanged {
        add => Events.AddHandler (s_tabIndexChangedEvent, value);
        remove => Events.RemoveHandler (s_tabIndexChangedEvent, value);
    }

    /// <summary>
    /// Raised when the TabStop property is changed.
    /// </summary>
    public event EventHandler? TabStopChanged {
        add => Events.AddHandler (s_tabStopChangedEvent, value);
        remove => Events.RemoveHandler (s_tabStopChangedEvent, value);
    }

    /// <summary>
    /// Raised when the Text property is changed.
    /// </summary>
    public event EventHandler? TextChanged {
        add => Events.AddHandler (s_textChangedEvent, value);
        remove => Events.RemoveHandler (s_textChangedEvent, value);
    }

    /// <summary>
    /// Raised when the Visisble property is changed.
    /// </summary>
    public event EventHandler? VisibleChanged {
        add => Events.AddHandler (s_visibleChangedEvent, value);
        remove => Events.RemoveHandler (s_visibleChangedEvent, value);
    }
}

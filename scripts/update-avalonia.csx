// This script copies the files we use from Avalonia into the Modern.Forms repo.
// Note this is not automatic, there are still plenty of changes that need to be
// manually reverted before the result will build and can be committed.

using System;
using System.IO;
using System.Text;

// Paths to the local repositories
// This assumes the repositories are siblings, but you can change that here if needed
// - /code
//   - /Avalonia
//   - /Modern.Forms
string avalonia_repo_path = Path.Combine ("..", "..", "Avalonia");
string modern_forms_repo_path = "..";

string avalonia_path = Path.Combine (avalonia_repo_path, "src");
string modern_forms_path = Path.Combine (modern_forms_repo_path, "src", "Modern.Forms", "Avalonia");

CopyFile ("Avalonia.Input/Cursors.cs", "Cursors.cs");
CopyFile ("Avalonia.Base/Threading/Dispatcher.cs", "Dispatcher.cs");
CopyFile ("Avalonia.Base/Threading/DispatcherPriority.cs", "DispatcherPriority.cs");
CopyFile ("Avalonia.Visuals/Platform/IBitmapImpl.cs", "IBitmapImpl.cs");
CopyFile ("Avalonia.Input/Platform/IClipboard.cs", "IClipboard.cs");
CopyFile ("Avalonia.Input/ICloseable.cs", "ICloseable.cs");
CopyFile ("Avalonia.Base/Threading/IDispatcher.cs", "IDispatcher.cs");
CopyFile ("Avalonia.Controls/Platform/Surfaces/IFramebufferPlatformSurface.cs", "IFramebufferPlatformSurface.cs");
CopyFile ("Avalonia.Input/IInputDevice.cs", "IInputDevice.cs");
CopyFile ("Avalonia.Input/IKeyboardDevice.cs", "IKeyboardDevice.cs");
CopyFile ("Avalonia.Visuals/Platform/ILockedFramebuffer.cs", "ILockedFramebuffer.cs");
CopyFile ("Avalonia.Input/IMouseDevice.cs", "IMouseDevice.cs");
CopyFile ("Avalonia.Base/Platform/IPlatformHandle.cs", "IPlatformHandle.cs");
CopyFile ("Avalonia.Base/Platform/IPlatformThreadingInterface.cs", "IPlatformThreadingInterface.cs");
CopyFile ("Avalonia.Input/IPointerDevice.cs", "IPointerDevice.cs");
CopyFile ("Avalonia.Controls/Platform/IPopupImpl.cs", "IPopupImpl.cs");
CopyFile ("Avalonia.Base/Platform/IRuntimePlatform.cs", "IRuntimePlatform.cs");
CopyFile ("Avalonia.Controls/Platform/IScreenImpl.cs", "IScreenImpl.cs");
CopyFile ("Avalonia.Input/Platform/IStandardCursorFactory.cs", "IStandardCursorFactory.cs");
CopyFile ("Avalonia.Controls/Platform/ISystemDialogImpl.cs", "ISystemDialogImpl.cs");
CopyFile ("Avalonia.Controls/Platform/ITopLevelImpl.cs", "ITopLevelImpl.cs");
CopyFile ("Avalonia.Controls/Platform/IWindowBaseImpl.cs", "IWindowBaseImpl.cs");
CopyFile ("Avalonia.Controls/Platform/IWindowImpl.cs", "IWindowImpl.cs");
CopyFile ("Avalonia.Controls/Platform/IWindowingPlatform.cs", "IWindowingPlatform.cs");
CopyFile ("Avalonia.Visuals/Platform/IWriteableBitmapImpl.cs", "IWriteableBitmapImpl.cs");
CopyFile ("Avalonia.Base/Threading/JobRunner.cs", "JobRunner.cs");
CopyFile ("Avalonia.Input/Key.cs", "Key.cs");
CopyFile ("Avalonia.Input/KeyboardDevice.cs", "KeyboardDevice.cs");
CopyFile ("Avalonia.Base/Utilities/MathUtilities.cs", "MathUtilities.cs");
CopyFile ("Avalonia.Input/MouseDevice.cs", "MouseDevice.cs");
CopyFile ("Avalonia.Visuals/Platform/PixelFormat.cs", "PixelFormat.cs");
CopyFile ("Avalonia.Visuals/Media/PixelPoint.cs", "PixelPoint.cs");
CopyFile ("Avalonia.Visuals/Media/PixelRect.cs", "PixelRect.cs");
CopyFile ("Avalonia.Visuals/Media/PixelSize.cs", "PixelSize.cs");
CopyFile ("Avalonia.Base/Platform/PlatformHandle.cs", "PlatformHandle.cs");
CopyFile ("Avalonia.Visuals/Point.cs", "Point.cs");
CopyFile ("Avalonia.Input/Raw/RawInputEventArgs.cs", "RawInputEventArgs.cs");
CopyFile ("Avalonia.Input/Raw/RawKeyEventArgs.cs", "RawKeyEventArgs.cs");
//CopyFile ("Avalonia.Input/Raw/RawMouseEventArgs.cs", "RawMouseEventArgs.cs");
CopyFile ("Avalonia.Input/Raw/RawMouseWheelEventArgs.cs", "RawMouseWheelEventArgs.cs");
CopyFile ("Avalonia.Input/Raw/RawTextInputEventArgs.cs", "RawTextInputEventArgs.cs");
CopyFile ("Avalonia.Visuals/Rect.cs", "Rect.cs");
//CopyFile ("Avalonia.Input/RuntimeInfo.cs", "RuntimeInfo.cs");
CopyFile ("Avalonia.Controls/Platform/Screen.cs", "Screen.cs");
CopyFile ("Avalonia.Controls/Screens.cs", "Screens.cs");
CopyFile ("Avalonia.Visuals/Size.cs", "Size.cs");
CopyFile ("Shared/PlatformSupport/StandardRuntimePlatform.cs", "StandardRuntimePlatform.cs");
CopyFile ("Avalonia.Controls/SystemDialog.cs", "SystemDialog.cs");
CopyFile ("Avalonia.Visuals/Thickness.cs", "Thickness.cs");
CopyFile ("Avalonia.Visuals/Vector.cs", "Vector.cs");
CopyFile ("Avalonia.Controls/WindowEdge.cs", "WindowEdge.cs");
CopyFile ("Avalonia.Controls/WindowState.cs", "WindowState.cs");

// Mac Backend
CopyFile ("Avalonia.Native/AvaloniaNativePlatform.cs", "Avalonia.Mac/AvaloniaNativePlatform.cs");
CopyFile ("Avalonia.Native/AvaloniaNativePlatformExtensions.cs", "Avalonia.Mac/AvaloniaNativePlatformExtensions.cs");
CopyFile ("Avalonia.Native/CallbackBase.cs", "Avalonia.Mac/CallbackBase.cs");
CopyFile ("Avalonia.Native/Cursor.cs", "Avalonia.Mac/Cursor.cs");
CopyFile ("Avalonia.Native/DynLoader.cs", "Avalonia.Mac/DynLoader.cs");
CopyFile ("Avalonia.Native/Helpers.cs", "Avalonia.Mac/Helpers.cs");
CopyFile ("Avalonia.Native/PlatformThreadingInterface.cs", "Avalonia.Mac/PlatformThreadingInterface.cs");
CopyFile ("Avalonia.Native/PopupImpl.cs", "Avalonia.Mac/PopupImpl.cs");
CopyFile ("Avalonia.Native/ScreenImpl.cs", "Avalonia.Mac/ScreenImpl.cs");
CopyFile ("Avalonia.Native/SystemDialogs.cs", "Avalonia.Mac/SystemDialogs.cs");
CopyFile ("Avalonia.Native/WindowImpl.cs", "Avalonia.Mac/WindowImpl.cs");
CopyFile ("Avalonia.Native/WindowImplBase.cs", "Avalonia.Mac/WindowImplBase.cs");

// Windows Backend
CopyFile ("Windows/Avalonia.Win32/CursorFactory.cs", "Avalonia.Win32/CursorFactory.cs");
CopyFile ("Windows/Avalonia.Win32/FramebufferManager.cs", "Avalonia.Win32/FramebufferManager.cs");
CopyFile ("Windows/Avalonia.Win32/Input/KeyInterop.cs", "Avalonia.Win32/KeyInterop.cs");
CopyFile ("Windows/Avalonia.Win32/PlatformConstants.cs", "Avalonia.Win32/PlatformConstants.cs");
CopyFile ("Windows/Avalonia.Win32/PopupImpl.cs", "Avalonia.Win32/PopupImpl.cs");
CopyFile ("Windows/Avalonia.Win32/ScreenImpl.cs", "Avalonia.Win32/ScreenImpl.cs");
CopyFile ("Windows/Avalonia.Win32/SystemDialogImpl.cs", "Avalonia.Win32/SystemDialogImpl.cs");
CopyFile ("Windows/Avalonia.Win32/Win32Platform.cs", "Avalonia.Win32/Win32Platform.cs");
CopyFile ("Windows/Avalonia.Win32/WindowFramebuffer.cs", "Avalonia.Win32/WindowFramebuffer.cs");
CopyFile ("Windows/Avalonia.Win32/WindowImpl.cs", "Avalonia.Win32/WindowImpl.cs");
CopyFile ("Windows/Avalonia.Win32/Input/WindowsKeyboardDevice.cs", "Avalonia.Win32/WindowsKeyboardDevice.cs");
CopyFile ("Windows/Avalonia.Win32/Input/WindowsMouseDevice.cs", "Avalonia.Win32/WindowsMouseDevice.cs");
CopyFile ("Windows/Avalonia.Win32/WinScreen.cs", "Avalonia.Win32/WinScreen.cs");
CopyFile ("Windows/Avalonia.Win32/Interop/UnmanagedMethods.cs", "Avalonia.Win32/Interop/UnmanagedMethods.cs");

// X11 Backend
CopyFile ("Avalonia.X11/NativeDialogs/Gtk.cs", "Avalonia.X11/Gtk.cs");
CopyFile ("Avalonia.X11/NativeDialogs/GtkNativeFileDialogs.cs", "Avalonia.X11/GtkNativeFileDialogs.cs");
CopyFile ("Avalonia.X11/Keysyms.cs", "Avalonia.X11/Keysyms.cs");
CopyFile ("Avalonia.Base/Platform/Interop/Utf8Buffer.cs", "Avalonia.X11/Utf8Buffer.cs");
CopyFile ("Avalonia.X11/X11Atoms.cs", "Avalonia.X11/X11Atoms.cs");
CopyFile ("Avalonia.X11/X11Clipboard.cs", "Avalonia.X11/X11Clipboard.cs");
CopyFile ("Avalonia.X11/X11CursorFactory.cs", "Avalonia.X11/X11CursorFactory.cs");
CopyFile ("Avalonia.X11/X11Enums.cs", "Avalonia.X11/X11Enums.cs");
CopyFile ("Avalonia.X11/X11Exception.cs", "Avalonia.X11/X11Exception.cs");
CopyFile ("Avalonia.X11/X11Framebuffer.cs", "Avalonia.X11/X11Framebuffer.cs");
CopyFile ("Avalonia.X11/X11FramebufferSurface.cs", "Avalonia.X11/X11FramebufferSurface.cs");
CopyFile ("Avalonia.X11/X11Info.cs", "Avalonia.X11/X11Info.cs");
CopyFile ("Avalonia.X11/X11KeyTransform.cs", "Avalonia.X11/X11KeyTransform.cs");
CopyFile ("Avalonia.X11/X11Platform.cs", "Avalonia.X11/X11Platform.cs");
CopyFile ("Avalonia.X11/X11PlatformThreading.cs", "Avalonia.X11/X11PlatformThreading.cs");
CopyFile ("Avalonia.X11/X11Screens.cs", "Avalonia.X11/X11Screens.cs");
CopyFile ("Avalonia.X11/X11Structs.cs", "Avalonia.X11/X11Structs.cs");
CopyFile ("Avalonia.X11/X11Window.cs", "Avalonia.X11/X11Window.cs");
CopyFile ("Avalonia.X11/XError.cs", "Avalonia.X11/XError.cs");
CopyFile ("Avalonia.X11/XI2Manager.cs", "Avalonia.X11/XI2Manager.cs");
CopyFile ("Avalonia.X11/XIStructs.cs", "Avalonia.X11/XIStructs.cs");
CopyFile ("Avalonia.X11/XLib.cs", "Avalonia.X11/XLib.cs");

// Skia Backend
CopyFile ("Skia/Avalonia.Skia/Helpers/PixelFormatHelper.cs", "Avalonia.Skia/PixelFormatHelper.cs");
CopyFile ("Skia/Avalonia.Skia/SkiaOptions.cs", "Avalonia.Skia/SkiaOptions.cs");
CopyFile ("Skia/Avalonia.Skia/SkiaPlatform.cs", "Avalonia.Skia/SkiaPlatform.cs");
CopyFile ("Skia/Avalonia.Skia/SkiaSharpExtensions.cs", "Avalonia.Skia/SkiaSharpExtensions.cs");
CopyFile ("Skia/Avalonia.Skia/WriteableBitmapImpl.cs", "Avalonia.Skia/WriteableBitmapImpl.cs");

private void CopyFile (string src, string dst)
{
    var full_src = Path.Combine (avalonia_path, src);
    var full_dst = Path.Combine (modern_forms_path, dst);

    var text = File.ReadAllText (full_src);

    // Avalonia does not use nullable reference types so we disable checking for these files
    text = $"#nullable disable{Environment.NewLine}{Environment.NewLine}{text}";

    // We do not publicly expose the Avalonia types
    text = text.Replace ("public class", "internal class");
    text = text.Replace ("public abstract class", "internal abstract class");
    text = text.Replace ("public static class", "internal static class");
    text = text.Replace ("public static extern", "internal static extern");
    text = text.Replace ("public enum", "internal enum");
    text = text.Replace ("public interface", "internal interface");
    text = text.Replace ("public struct", "internal struct");
    text = text.Replace ("public readonly struct", "internal readonly struct");
    text = text.Replace ("public unsafe struct", "internal unsafe struct");

    // Some namespaces we do not use
    text = Comment (text, "using Avalonia.Animation");
    text = Comment (text, "using Avalonia.Animation.Animators");
    text = Comment (text, "using Avalonia.Interactivity");
    text = Comment (text, "using Avalonia.Media");
    text = Comment (text, "using Avalonia.OpenGL");
    text = Comment (text, "using Avalonia.Rendering");
    text = Comment (text, "using Avalonia.VisualTree");
    text = Comment (text, "using Avalonia.X11.Glx");
    text = Comment (text, "using JetBrains.Annotations");
    text = Comment (text, "using System.ComponentModel.DataAnnotation");
    text = Comment (text, "using System.Reactive.Disposables");
    text = Comment (text, "using System.Reactive.Linq");

    // We don't use Avalonia's DI
    text = text.Replace ("AvaloniaLocator.Current.GetService<IStandardCursorFactory>()", "AvaloniaGlobals.StandardCursorFactory");
    text = text.Replace ("AvaloniaLocator.Current.GetService<IPlatformThreadingInterface>()", "AvaloniaGlobals.PlatformThreadingInterface");
    text = text.Replace ("AvaloniaLocator.Current.GetService<ISystemDialogImpl>()", "AvaloniaGlobals.SystemDialogImplementation");
    text = text.Replace ("AvaloniaLocator.Current.GetService<IRuntimePlatform>()", "AvaloniaGlobals.RuntimePlatform");
    text = text.Replace ("AvaloniaLocator.Current?.GetService<IRuntimePlatform>()", "AvaloniaGlobals.RuntimePlatform");

    // Other fixups
    text = Comment (text, "Contract.Requires");
    text = Comment (text, "Animation.Animation.RegisterAnimator");
    text = Comment (text, "[Pure]");

    File.WriteAllText (full_dst, text, Encoding.UTF8);
}

// We prefer to comment unused stuff so we can tell stuff we've disabled versus new stuff in diffs
private string Comment (string text, string str) => text.Replace (str, "//" + str);

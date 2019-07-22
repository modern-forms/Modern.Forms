## What is Modern.Forms?

*** **This is just a proof of concept. The demo runs well, but that's all that has been implemented.** ***

Modern.Forms is a cross-platform spiritual successor to Winforms for .NET Core 3.0.

**This is accomplished with:**

* .NET Core 3.0 Preview
* Some infrastructure code from Mono's Winforms (like layouts):
  * https://github.com/mono/mono/tree/master/mcs/class/System.Windows.Forms
* A port of Avalonia's native backends
  * https://github.com/AvaloniaUI/Avalonia
  * Only the base infrastructure is used, basically this gives us a blank Form
* SkiaSharp
  * All new controls are drawn with SkiaSharp

### Motivation

The goal of this proof of concept is to create a spiritual successor to Winforms that is:
* Cross platform (Windows / Mac / Linux)
* Familiar for Winforms developers (ie: not XAML)
  * Sample Form:
    * [MainForm.cs](https://github.com/jpobst/Modern.Forms/blob/master/samples/Explorer/MainForm.cs)
    * [MainForm.Designer.cs](https://github.com/jpobst/Modern.Forms/blob/master/samples/Explorer/MainForm.Designer.cs)
* Great for LOB applications and quick apps
* Updated with modern controls and modern aesthetics

## How to Run

### Windows

* Clone this repository
* Install .NET Core 3.0 Preview 4+
  * https://dotnet.microsoft.com/download/dotnet-core/3.0
* Open `Modern.Forms.sln` in Visual Studio 2019
* Ensure `Explore` is set as the Startup project
* Hit F5

![Windows Screenshot](https://github.com/jpobst/Modern.Forms/blob/master/docs/explorer-windows.png "Windows Screenshot")

### Ubuntu 19.04 AMD64

* Clone this repository
* Install .NET Core 3.0 Preview 4+
  * https://dotnet.microsoft.com/download/dotnet-core/3.0
* Navigate to `samples/Explorer`
* Run `dotnet run`

![Ubuntu Screenshot](https://github.com/jpobst/Modern.Forms/blob/master/docs/explorer-ubuntu.png "Ubuntu Screenshot")

### OSX

The OSX backend from Avalonia has not been ported because I do not have a Mac.  Given the
work done to get the other backends running, it would probably only take a few hours.

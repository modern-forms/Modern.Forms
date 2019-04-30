## What is Modern.Forms?

*** This is just a proof of concept. The demo runs well, but that's all that has been implemented. ***

Modern.Forms is a cross-platform spiritual successor to Winforms for .NET Core 3.0.

**This is accomplished with:**

* .NET Core 3.0 Preview
* A port of Mono's Winforms that runs on .NET Core
  * https://github.com/yzrmn/System.CoreFX.Forms
  * Only the base infrastructure is used, basically this gives us a blank Form
* SkiaSharp
  * All new controls are drawn with SkiaSharp

### Motivation

The goal of this proof of concept is to create a spiritual successor to Winforms that is:
* Cross platform (Windows / Mac / Linux)
* Familiar for Winforms developers (ie: no XAML)
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
* Ensure `Explorer` is set at the Startup project
* Hit F5

![Windows Screenshot](https://github.com/jpobst/Modern.Forms/docs/explorer-windows.png "Windows Screenshot")

### Ubuntu 19.04 AMD64

* Clone this repository
* Install .NET Core 3.0 Preview 4+
  * https://dotnet.microsoft.com/download/dotnet-core/3.0
* Install `libgdiplus.so`
  * `apt install libgidplus`
* Install `libSkiaSharp.so`
  * https://github.com/mono/SkiaSharp/releases/download/v1.68.0/libSkiaSharp.so
  * This can go in `bin/Debug/netcoreapp3.0` or anywhere it can be found
* Install `libMonoPosixHelper` (Bundled in Mono)
  * `apt install mono-runtime-common`
* Navigate to `samples/Explorer`
* Run `dotnet run`

![Ubuntu Screenshot](https://github.com/jpobst/Modern.Forms/docs/explorer-ubuntu.png "Ubuntu Screenshot")

### OSX

I don't have a Mac and it appears neither did the guy who ported `System.CoreFX.Forms`, so it is likely
there will need to be additional porting work to get it functional.  Once `System.CoreFX.Forms` is
successfully running, there should not be any additional changes needed for `Modern.Forms`.
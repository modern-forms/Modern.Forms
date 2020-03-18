## What is Modern.Forms?

*** **This framework is currently in its early stages. It is not intended for production use.** ***

Modern.Forms is a cross-platform spiritual successor to Winforms for .NET Core 3.1+, supporting Windows, Mac, and Linux.

**This is accomplished with:**

* Some infrastructure code from Mono's Winforms (like layouts):
  * https://github.com/mono/mono/tree/master/mcs/class/System.Windows.Forms
* A port of Avalonia's native backends (for interacting with the operating system)
  * https://github.com/AvaloniaUI/Avalonia
* SkiaSharp
  * All controls are fully managed and drawn with SkiaSharp

### Motivation

The goal is to create a spiritual successor to Winforms that is:
* Cross platform (Windows / Mac / Linux)
* Familiar for Winforms developers (ie: not XAML)
  * Sample Form:
    * [MainForm.cs](https://github.com/modern-forms/Modern.Forms/blob/master/samples/Explorer/MainForm.cs)
    * [MainForm.Designer.cs](https://github.com/modern-forms/Modern.Forms/blob/master/samples/Explorer/MainForm.Designer.cs)
* Great for LOB applications and quick apps
* Updated with modern controls and modern aesthetics

## How to Run

### Sample Application

The quickest way to see Modern.Forms in action is through our `ModernDecompiler` sample application, 
which allows you to decompile .NET assemblies. ([Source Code](https://github.com/modern-forms/ModernDecompile))

From a Windows, Mac, or Linux command line with .NET Core 3.1+ installed:
```
dotnet tool install --global ModernDecompile
decompile
```

This will launch the sample application built with Modern.Forms.

![ModernDecompiler Screenshot](https://github.com/jpobst/Modern.Forms/blob/master/docs/modern-decompile.png "ModernDecompiler Screenshot")

### Other Samples

Some smaller samples are available in the Modern.Forms repository:

* `ControlGallery` - Gallery of the controls included in Modern.Forms in action.
* `Explore` - A Windows Explorer clone.

For information on building and running these samples, see [Samples](docs/samples.md).

### Getting Started

To create your own Modern.Forms application, see [Getting Started](docs/getting-started.md).

## Build Status

Windows | Mac - High Sierra | Ubuntu 16.04
-|-|-
[![Build status](https://dev.azure.com/jonathan0207/Modern.Forms/_apis/build/status/Windows)](https://dev.azure.com/jonathan0207/Modern.Forms/_build/latest?definitionId=1) | [![Build status](https://dev.azure.com/jonathan0207/Modern.Forms/_apis/build/status/Mac%20OSX%20-%20High%20Sierra)](https://dev.azure.com/jonathan0207/Modern.Forms/_build/latest?definitionId=4) | [![Build status](https://dev.azure.com/jonathan0207/Modern.Forms/_apis/build/status/Ubuntu%2016.04)](https://dev.azure.com/jonathan0207/Modern.Forms/_build/latest?definitionId=2)

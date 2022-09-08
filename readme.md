## What is Modern.Forms?

*** **This framework is currently in its early stages. Use at your own risk.** ***

Modern.Forms is an open-source cross-platform spiritual successor to Winforms for .NET 6, supporting Windows, Mac, and Linux.

If you are looking for an open-source cross-platform spiritual successor to WPF, see [Avalonia](https://github.com/AvaloniaUI/Avalonia).

### Motivation

The goal is to create a spiritual successor to Winforms that is:
* Cross platform (Windows / Mac / Linux)
* Familiar for Winforms developers (ie: not XAML)
  * Sample Form:
    * [MainForm.cs](https://github.com/modern-forms/Modern.Forms/blob/main/samples/Explorer/MainForm.cs)
    * [MainForm.Designer.cs](https://github.com/modern-forms/Modern.Forms/blob/main/samples/Explorer/MainForm.Designer.cs)
* Great for LOB applications and quick apps
* Updated with modern controls and modern aesthetics

### Getting Started

To create your own Modern.Forms application, see [Getting Started](docs/getting-started.md).

## How to Run

### Sample Application

The quickest way to see Modern.Forms in action is through our `ModernDecompiler` sample application, 
which allows you to decompile .NET assemblies. ([Source Code](https://github.com/modern-forms/ModernDecompile))

From a Windows, Mac, or Linux command line with .NET 6 installed:
```
dotnet tool install --global ModernDecompile
decompile
```

This will launch the sample application built with Modern.Forms.

![ModernDecompiler Screenshot](https://github.com/modern-forms/Modern.Forms/blob/main/docs/modern-decompile.png "ModernDecompiler Screenshot")

### Other Samples

Some smaller samples are available in the Modern.Forms repository:

* [`ControlGallery`](samples/ControlGallery) - Gallery of the controls included in Modern.Forms in action.
* [`Explore`](samples/Explorer) - A Windows Explorer clone.
* [`Outlaw`](samples/Outlaw) - A Microsoft Outlook clone.

For information on building and running these samples, see [Samples](docs/samples.md).

## Build Status

[![.NET Build](https://github.com/modern-forms/Modern.Forms/actions/workflows/dotnet.yml/badge.svg)](https://github.com/modern-forms/Modern.Forms/actions/workflows/dotnet.yml)

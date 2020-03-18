## Getting Started with Modern.Forms

## From Template

The easiest way to get started creating a Modern.Forms application is with our `dotnet` template available from NuGet.

To install and run:
```
dotnet new --install ModernForms.Templates
dotnet new modernforms
dotnet run
```

This will run create and run a basic Hello World Modern.Forms application.

There isn't documentation available yet, but the API should be relatively familiar for developers with Windows.Forms
experience.  A good resource is to look at the source code of our sample applications:
* [ControlGallery](https://github.com/modern-forms/Modern.Forms/tree/master/samples/ControlGallery)
* [Explore](https://github.com/modern-forms/Modern.Forms/tree/master/samples/Explorer)
* [ModernDecompile](https://github.com/modern-forms/ModernDecompile/tree/master/src)

## From Scratch

To turn a regular .NET Core Console Application into a Modern.Forms application, make the following changes.

#### Project File

Ensure the following properties are set:
```
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <Nullable>enable</Nullable>
</PropertyGroup>
```

Add a NuGet reference to `Modern.Forms`:
```
<ItemGroup>
    <PackageReference Include="Modern.Forms" Version="0.1.0" />
</ItemGroup>
```

#### Empty Form

Create an empty Form class:
```csharp
using Modern.Forms;

public class MainForm : Form
{
}
```

#### Program.cs
Call `Application.Run ()` with an instance of your Form:

```csharp
static void Main (string [] args)
{
    Application.Run (new MainForm ());
}
```

Your application should now be ready to run.
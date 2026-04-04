## Contributing to Modern.Forms

Thank you for your interest in contributing to `Modern.Forms`! This document provides guidelines to help you get started.

### Reporting Bugs & Suggesting Features

Please use [GitHub Issues](https://github.com/modern-forms/Modern.Forms/issues) to:

* Report bugs — include steps to reproduce, expected behavior, and the OS you are running on.
* Suggest new features or enhancements.

Before opening a new issue, search existing issues to avoid duplicates.

### Getting Started

#### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet) (or later)
* A C# editor such as Visual Studio 2022, Visual Studio Code, or JetBrains Rider

#### Clone & Build

```
git clone https://github.com/modern-forms/Modern.Forms.git
cd Modern.Forms
dotnet build --configuration Release
```

#### Run Tests

Tests are located in the `tests/Modern.Forms.Tests` project:

```
dotnet test --configuration Release
```

> **Note:** Some tests require a graphical environment and currently only pass on Windows.

#### Run Samples

Sample applications are in the `samples` directory. For example:

```
cd samples/Explorer
dotnet run
```

See [Samples](docs/samples.md) for more details.

### Code Style

This project uses an [`.editorconfig`](.editorconfig) file to enforce a consistent code style. Key conventions include:

* Use **spaces** for indentation (4 spaces for C# files).
* Prefer `var` where possible.
* Avoid `this.` qualification unless necessary.
* Use language keywords instead of framework type names (e.g., `int` instead of `Int32`).
* Nullable reference types are enabled (`<Nullable>enable</Nullable>`).

Before submitting a pull request, verify that your changes pass the formatter:

```
dotnet format --verify-no-changes
```

You can auto-fix formatting issues with:

```
dotnet format
```

### Submitting a Pull Request

1. Fork the repository and create a branch from `main`.
2. Make your changes, keeping the scope focused and the commits clear.
3. Ensure the project builds without warnings in Release configuration — warnings are treated as errors in Release builds.
4. Ensure all existing tests continue to pass.
5. Add or update tests for any new or changed behavior when applicable.
6. Run `dotnet format --verify-no-changes` to confirm your code matches the project style.
7. Open a pull request against `main`. CI will build and test on Windows, macOS, and Ubuntu.

### License

By contributing to `Modern.Forms`, you agree that your contributions will be licensed under the [MIT License](license.md).

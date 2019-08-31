### Scripts

These scripts use [dotnet-script](https://github.com/filipw/dotnet-script).

To install:
```
dotnet tool install -g dotnet-script
```

To run a script:
```
dotnet script update-avalonia.csx
```

#### update-avalonia.csx

This script copies and massages the files we use from Avalonia into the Modern.Forms repo.

Note this is not automatic, there are still plenty of changes that need to be
manually reverted before the result will build and can be committed.  
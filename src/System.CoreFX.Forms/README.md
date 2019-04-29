# FAQ

## What is System.CoreFX.Forms?

*System.CoreFX.Forms* is a port of Mono WinForms to .NET Core 2.1 utilizing the work that has been done for *System.Drawing.Common*. It should serve as an easy and known way to build cross-platform user interfaces or even port existing .NET Framework applications. The project is in a very early state (proof of concept) and is only known to be working on Windows right now.

**Contributions for Unix support are very welcome and probably needed to make this work.**

## Which features are supported?

Almost the full code has been taken over, however there are some things that had to be altered or cut down:

* *WebBrowser* and *Html* classes
* Everything related to *Registry* or *Configuration*
* A few *Mono.Unix* specific calls which are not supported
* Some minor/misc stuff

**Please note that the majority of features are untested!**

using System;
using Xunit;

namespace Modern.Forms.Tests;

public class ApplicationTests
{
    [Fact]
    public void OpenForms ()
    {
        Assert.Equal (0, Application.OpenForms.Count);

        // Creating a Form does not add it to open forms
        var f = new Form ();

        Assert.Equal (0, Application.OpenForms.Count);

        // Showing a Form adds it to open forms
        f.Show ();

        Assert.Equal (1, Application.OpenForms.Count);
        Assert.Equal (f, Application.OpenForms[0]);

        // Showing a dialog Form adds it to open forms
        var f2 = new Form ();
        f2.ShowDialog (f);

        Assert.Equal (2, Application.OpenForms.Count);
        Assert.Equal (f2, Application.OpenForms[1]);

        // Closing the dialog Form removes it from open forms
        f2.Close ();

        // Closing a Form removes it from open forms
        f.Close ();
        Assert.Equal (0, Application.OpenForms.Count);
    }
}

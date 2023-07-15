using System;
using Modern.Forms;

namespace ControlGallery.Panels;

public class FormShortcutsPanel : BasePanel
{
    private readonly Form form;
    private readonly TextBox textbox;

    public FormShortcutsPanel (Form form)
    {
        this.form = form;

        textbox = Controls.Add (new TextBox {
            Left = 10,
            Top = 10,
            Width = 200,
            Height = 400,
            MultiLine = true,
        });

        var clear_button = Controls.Add (new Button {
            Text = "Clear",
            Left = 220,
            Top = 10
        });

        clear_button.Click += (o, e) => textbox.Text = string.Empty;

        form.KeyDown += HandleKeyDown;
        form.KeyUp += HandleKeyUp;
        form.KeyPress += HandleKeyPress;
    }

    public override void UnloadPanel ()
    {
        form.KeyDown -= HandleKeyDown;
        form.KeyUp -= HandleKeyUp;
        form.KeyPress -= HandleKeyPress;
    }

    private void HandleKeyDown (object? sender, KeyEventArgs e)
    {
        textbox.Text += $"KeyDown: '{e.KeyCode}'{Environment.NewLine}";
        e.Handled = true;
    }

    private void HandleKeyUp (object? sender, KeyEventArgs e)
    {
        textbox.Text += $"KeyUp: '{e.KeyCode}'{Environment.NewLine}";
        e.Handled = true;
    }

    private void HandleKeyPress (object? sender, KeyPressEventArgs e)
    {
        textbox.Text += $"KeyPress: '{e.Text}'{Environment.NewLine}";
        e.Handled = true;
    }
}

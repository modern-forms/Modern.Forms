using System;
using Modern.Forms;

namespace ControlGallery.Panels;

public class FormShortcutsPanel : Panel
{
    private readonly TextBox textbox;

    public FormShortcutsPanel (Form form)
    {
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

        form.KeyDown += (s, e) => {
            textbox.Text += $"KeyDown: '{e.KeyCode}'{Environment.NewLine}";
            e.Handled = true;
        };

        form.KeyUp += (s, e) => {
            textbox.Text += $"KeyUp: '{e.KeyCode}'{Environment.NewLine}";
            e.Handled = true;
        };

        form.KeyPress += (s, e) => {
            textbox.Text += $"KeyPress: '{e.Text}'{Environment.NewLine}";
            e.Handled = true;
        };
    }
}

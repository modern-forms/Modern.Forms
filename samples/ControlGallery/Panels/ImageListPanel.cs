using System.Linq;
using Modern.Forms;
using SkiaSharp;

namespace ControlGallery.Panels;

public class ImageListPanel : Panel
{
    public ImageListPanel ()
    {
        var image_list_16 = new ImageList { ImageSize = new SKSize (16, 16) };
        var image_list_32 = new ImageList { ImageSize = new SKSize (32, 32) };

        AddImages (image_list_16);
        AddImages (image_list_32);

        // Create a combo box to toggle between the available images
        var comboBox = Controls.Add (new ComboBox {
            Left = 10,
            Top = 10,
            Width = 200,
        });

        comboBox.Items.AddRange (image_list_16.Images.Keys.ToArray ());
        comboBox.SelectedIndex = 0;

        // Create controls to ensure they display images correctly
        var checkbox_16 = Controls.Add (new CheckBox {
            Text = "CheckBox 16",
            Left = 10,
            Top = 50,
            Width = 200,
            ImageList = image_list_16,
            ImageKey = "button"
        });

        var checkbox_32 = Controls.Add (new CheckBox {
            Text = "CheckBox 32",
            Left = 10,
            Top = 90,
            Width = 200,
            ImageList = image_list_32,
            ImageKey = "button"
        });

        var radiobutton_16 = Controls.Add (new RadioButton {
            Text = "RadioButton 16",
            Left = 10,
            Top = 130,
            Width = 200,
            ImageList = image_list_16,
            ImageKey = "button"
        });

        var radiobutton_32 = Controls.Add (new RadioButton {
            Text = "RadioButton 32",
            Left = 10,
            Top = 170,
            Width = 200,
            ImageList = image_list_32,
            ImageKey = "button"
        });

        var label_16 = Controls.Add (new Label {
            Text = "Label 16",
            Left = 10,
            Top = 210,
            Width = 200,
            ImageList = image_list_16,
            ImageKey = "button"
        });

        var label_32 = Controls.Add (new Label {
            Text = "Label 32",
            Left = 10,
            Top = 250,
            Width = 200,
            ImageList = image_list_32,
            ImageKey = "button"
        });

        var button_16 = Controls.Add (new Button {
            Text = "Button 16",
            Left = 10,
            Top = 290,
            Width = 200,
            ImageList = image_list_16,
            ImageKey = "button"
        });

        var button_32 = Controls.Add (new Button {
            Text = "Button 32",
            Left = 10,
            Top = 330,
            Width = 200,
            ImageList = image_list_32,
            ImageKey = "button"
        });

        comboBox.SelectedIndexChanged += (o, e) => {
            checkbox_16.ImageKey = comboBox.SelectedItem?.ToString ()!;
            checkbox_32.ImageKey = comboBox.SelectedItem?.ToString ()!;
            radiobutton_16.ImageKey = comboBox.SelectedItem?.ToString ()!;
            radiobutton_32.ImageKey = comboBox.SelectedItem?.ToString ()!;
            label_16.ImageKey = comboBox.SelectedItem?.ToString ()!;
            label_32.ImageKey = comboBox.SelectedItem?.ToString ()!;
            button_16.ImageKey = comboBox.SelectedItem?.ToString ()!;
            button_32.ImageKey = comboBox.SelectedItem?.ToString ()!;
        };
    }

    private void AddImages (ImageList imageList)
    {
        imageList.Images.Add ("button", ImageLoader.Get ("button.png"));
        imageList.Images.Add ("cd-burn", ImageLoader.Get ("cd-burn.png"));
        imageList.Images.Add ("compress", ImageLoader.Get ("compress.png"));
        imageList.Images.Add ("copy", ImageLoader.Get ("copy.png"));
        imageList.Images.Add ("cut", ImageLoader.Get ("cut.png"));
        imageList.Images.Add ("delete-red", ImageLoader.Get ("delete-red.png"));
        imageList.Images.Add ("drive", ImageLoader.Get ("drive.png"));
        imageList.Images.Add ("folder", ImageLoader.Get ("folder.png"));
        imageList.Images.Add ("folder-add", ImageLoader.Get ("folder-add.png"));
        imageList.Images.Add ("folder-closed", ImageLoader.Get ("folder-closed.png"));
        imageList.Images.Add ("folder-up", ImageLoader.Get ("folder-up.png"));
        imageList.Images.Add ("layout-folder-pane", ImageLoader.Get ("layout-folder-pane.png"));
        imageList.Images.Add ("mail", ImageLoader.Get ("mail.png"));
        imageList.Images.Add ("new", ImageLoader.Get ("new.png"));
        imageList.Images.Add ("paste", ImageLoader.Get ("paste.png"));
        imageList.Images.Add ("print", ImageLoader.Get ("print.png"));
        imageList.Images.Add ("search", ImageLoader.Get ("search.png"));
        imageList.Images.Add ("swatches", ImageLoader.Get ("swatches.png"));
    }
}

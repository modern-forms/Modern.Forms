using System;
using System.Drawing;
using System.Net.Http;
using Modern.Forms.Renderers;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a PictureBox control.
    /// </summary>
    public class PictureBox : Control
    {
        private static HttpClient? client;

        private SKBitmap? image;
        private string? image_location;
        private PictureBoxSizeMode size_mode;

        /// <summary>
        /// Initializes a new instance of the PictureBox class.
        /// </summary>
        public PictureBox ()
        {
            SetControlBehavior (ControlBehaviors.Selectable, false);
        }

        // Lazily initialize and cache an HttpClient if needed.
        private static HttpClient Client => client ??= new HttpClient ();

        /// <inheritdoc/>
        protected override Size DefaultSize => new Size (100, 50);

       /// <summary>
       /// Gets or sets the image the PictureBox should display.
       /// </summary>
        public SKBitmap? Image {
            get => image;
            set {
                if (image != value) {
                    image = value;
                    IsErrored = false;

                    UpdateSize ();
                }
            }
        }

        /// <summary>
        /// Gets or sets the path or URL of the image the PictureBox should display.
        /// </summary>
        public string? ImageLocation {
            get => image_location;
            set => LoadInternal (value);
        }

        /// <summary>
        /// Gets a value indicating the requested image could not be loaded.
        /// </summary>
        public bool IsErrored { get; private set; }

        /// <summary>
        /// Loads the image at the specified path or URL and sets ImageLocation to it.
        /// </summary>
        public void Load (string url)
        {
            if (string.IsNullOrWhiteSpace (url))
                throw new InvalidOperationException ("ImageLocation not specified.");

            ImageLocation = url;
        }

        // Load image from path or URL and display it.
        private void LoadInternal (string? url)
        {
            if (image_location == url)
                return;

            if (url is null) {
                Image = null;
                return;
            }

            IsErrored = false;
            image_location = url;

            try {
                if (url.Contains ("://"))
                    Image = SKBitmap.Decode (Client.GetStreamAsync (url).Result);
                else
                    Image = SKBitmap.Decode (url);
            } catch (Exception) {
                IsErrored = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicated the sizing mode used.
        /// </summary>
        public PictureBoxSizeMode SizeMode {
            get => size_mode;
            set {
                if (size_mode != value) {
                    size_mode = value;

                    //AutoSize = size_mode == PictureBoxSizeMode.AutoSize;
                    //SetAutoSizeMode (size_mode == PictureBoxSizeMode.AutoSize ? AutoSizeMode.GrowAndShrink : AutoSizeMode.GrowOnly);

                    UpdateSize ();

                    OnSizeModeChanged (EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Raised when the value of the SizeMode property changes.
        /// </summary>
        public event EventHandler? SizeModeChanged;

        /// <inheritdoc/>
        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            RenderManager.Render (this, e);
        }

        /// <summary>
        /// Raises the SizeModeChanged event.
        /// </summary>
        protected void OnSizeModeChanged (EventArgs e) => SizeModeChanged?.Invoke (this, e);

        // Trigger a resizing.
        private void UpdateSize ()
        {
            if (image == null)
                return;

            Parent?.PerformLayout (this, nameof (AutoSize));
        }
    }
}

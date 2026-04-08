namespace Modern.Forms
{
    /// <summary>
    /// Provides data for the <see cref="LinkLabel.LinkClicked"/> event.
    /// </summary>
    public class LinkLabelLinkClickedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkLabelLinkClickedEventArgs"/> class.
        /// </summary>
        /// <param name="link">The link that was clicked.</param>
        public LinkLabelLinkClickedEventArgs (LinkLabel.Link? link)
            : this (link, MouseButtons.Left)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkLabelLinkClickedEventArgs"/> class.
        /// </summary>
        /// <param name="link">The link that was clicked.</param>
        /// <param name="button">The mouse button associated with the click.</param>
        public LinkLabelLinkClickedEventArgs (LinkLabel.Link? link, MouseButtons button)
        {
            Link = link;
            Button = button;
        }

        /// <summary>
        /// Gets the link that was clicked.
        /// </summary>
        public LinkLabel.Link? Link { get; }

        /// <summary>
        /// Gets the mouse button associated with the click.
        /// </summary>
        public MouseButtons Button { get; }
    }
}

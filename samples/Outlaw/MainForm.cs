using System.Drawing;
using Modern.Forms;

namespace Outlaw
{
    public partial class MainForm : Form
    {
        public MainForm ()
        {
            InitializeComponent ();

            PopulateEmailList ();
            email_list.DrawNode += EmailListDrawNode;

            email_list.Style.SelectedItemBackgroundColor = Theme.ControlMidHighColor;

        }

        private void EmailListDrawNode (object? sender, TreeViewDrawEventArgs e)
        {
            var item = (EmailListItem)e.Item;

            if (item.Unread) {
                var bounds = new Rectangle (item.Bounds.Left, item.Bounds.Top + e.LogicalToDeviceUnits (1), e.LogicalToDeviceUnits (3), item.Bounds.Height - e.LogicalToDeviceUnits (2));
                e.Canvas.FillRectangle (bounds, Theme.AccentColor2);
            }

            var line1_bounds = new Rectangle (item.Bounds.Left + e.LogicalToDeviceUnits (12), item.Bounds.Top + e.LogicalToDeviceUnits (3), item.Bounds.Width - e.LogicalToDeviceUnits (80), e.LogicalToDeviceUnits (23));
            var line2_bounds = new Rectangle (item.Bounds.Left + e.LogicalToDeviceUnits (12), line1_bounds.Bottom - e.LogicalToDeviceUnits (3), item.Bounds.Width - e.LogicalToDeviceUnits (16), e.LogicalToDeviceUnits (20));
            var line3_bounds = new Rectangle (item.Bounds.Left + e.LogicalToDeviceUnits (12), line2_bounds.Bottom - e.LogicalToDeviceUnits (3), item.Bounds.Width - e.LogicalToDeviceUnits (16), e.LogicalToDeviceUnits (20));
            var date_bounds = new Rectangle (item.Bounds.Width - e.LogicalToDeviceUnits (80), item.Bounds.Top + e.LogicalToDeviceUnits (3), e.LogicalToDeviceUnits (74), e.LogicalToDeviceUnits (23));

            e.Canvas.DrawText (item.Text, Theme.UIFont, e.LogicalToDeviceUnits (16), line1_bounds, Theme.ForegroundColor, Modern.Forms.ContentAlignment.MiddleLeft, maxLines: e.LogicalToDeviceUnits (1));
            e.Canvas.DrawText (item.Subject, Theme.UIFont, e.LogicalToDeviceUnits (12), line2_bounds, CustomTheme.LighterGrayFont, Modern.Forms.ContentAlignment.MiddleLeft, maxLines: e.LogicalToDeviceUnits (1));
            e.Canvas.DrawText (item.Body, Theme.UIFont, e.LogicalToDeviceUnits (12), line3_bounds, CustomTheme.LighterGrayFont, Modern.Forms.ContentAlignment.MiddleLeft, maxLines: e.LogicalToDeviceUnits (1));
            e.Canvas.DrawText (FormatDateTime (item.ReceiveDate), Theme.UIFont, e.LogicalToDeviceUnits (11), date_bounds, CustomTheme.LighterGrayFont, Modern.Forms.ContentAlignment.MiddleRight, maxLines: e.LogicalToDeviceUnits (1));

            e.Canvas.DrawLine (item.Bounds.Left, item.Bounds.Bottom - e.LogicalToDeviceUnits (1), item.Bounds.Right, item.Bounds.Bottom - e.LogicalToDeviceUnits (1), Theme.ControlMidColor, e.LogicalToDeviceUnits (1));
        }

        private string FormatDateTime (DateTime date)
        {
            if (date.ToShortDateString () == DateTime.Now.ToShortDateString ())
                return date.ToShortTimeString ();

            return date.ToString ("ddd M/d");
        }

        private void PopulateEmailList ()
        {
            email_list.Items.Add (new EmailListItem ("Megan Smith", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime (), true));
            email_list.Items.Add (new EmailListItem ("Greg Simon", "Dinner on Friday", "Are you available for dinner on Friday? Ashley said she is available.", GetNextDateTime (), true));
            email_list.Items.Add (new EmailListItem ("Victor Craig", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Beverly Williams", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime (), true));
            email_list.Items.Add (new EmailListItem ("Morgan Graves", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Megan Smith", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("noreply@marketing.example.com", "Dinner on Friday", "Are you available for dinner on Friday? Ashley said she is available.", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Victor Craig", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Beverly Williams", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Morgan Graves", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Megan Smith", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Greg Simon", "Dinner on Friday", "Are you available for dinner on Friday? Ashley said she is available.", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Victor Craig", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Beverly Williams", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Morgan Graves", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Megan Smith", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Greg Simon", "Dinner on Friday", "Are you available for dinner on Friday? Ashley said she is available.", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Victor Craig", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Beverly Williams", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Morgan Graves", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Megan Smith", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Greg Simon", "Dinner on Friday", "Are you available for dinner on Friday? Ashley said she is available.", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Victor Craig", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Beverly Williams", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
            email_list.Items.Add (new EmailListItem ("Morgan Graves", "New mockups", "Hey I got those new mockups you requested!", GetNextDateTime ()));
        }

        private DateTime GetNextDateTime ()
        {
            hours += Random.Shared.Next (1, 6);

            return DateTime.Now.Subtract (new TimeSpan (hours, Random.Shared.Next (1, 59), Random.Shared.Next (1, 59)));
        }

        private int hours = 0;
    }
}

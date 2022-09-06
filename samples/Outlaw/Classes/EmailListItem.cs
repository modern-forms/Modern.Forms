using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modern.Forms;

namespace Outlaw
{
    internal class EmailListItem : TreeViewItem
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime ReceiveDate { get; set; }
        public bool Unread { get; set; }

        public EmailListItem (string text1, string subject, string body, DateTime receiveDate, bool unread = false)
        {
            Text = text1;
            Subject = subject;
            Body = body;
            ReceiveDate = receiveDate;
            Unread = unread;
        }

        public override Size GetPreferredSize (Size proposedSize)
        {
            var height = LogicalToDeviceUnits (65);

            return new Size (0, height);
        }

        private int LogicalToDeviceUnits (int value) => TreeView?.LogicalToDeviceUnits (value) ?? value;
    }
}

﻿using System;
using System.Drawing;

namespace Modern.Forms.Renderers
{
    /// <summary>
    /// Represents a class that can render a ListBox.
    /// </summary>
    public class ListBoxRenderer : Renderer<ListBox>
    {
        /// <inheritdoc/>
        protected override void Render (ListBox control, PaintEventArgs e)
        {
            for (var i = control.FirstVisibleIndex; i < Math.Min (control.Items.Count, control.FirstVisibleIndex + control.VisibleItemCount + 1); i++) {
                var item = control.Items[i];
                var bounds = control.GetItemRectangle (i);

                RenderItem (control, item, i, bounds, e);
            }

            // If there are no items we still may need to draw a focus rectangle
            if (control.Items.Count == 0 && control.Selected && control.ShowFocusCues) {
                var client = control.ClientRectangle;
                client.Height = control.ScaledItemHeight;

                e.Canvas.DrawFocusRectangle (client, 1);
            }
        }

        /// <summary>
        /// Renders a ListBox item.
        /// </summary>
        protected virtual void RenderItem (ListBox control, object item, int index, Rectangle bounds, PaintEventArgs e)
        {
            // Draw selected background
            if (control.Items.SelectedIndexes.Contains (index))
                e.Canvas.FillRectangle (bounds, Theme.ControlHighlightLowColor);

            // Draw hover background
            else if (control.ShowHover && control.Items.HoveredIndex == index)
                e.Canvas.FillRectangle (bounds, Theme.ControlMidColor);

            // Draw focus rectangle
            if (control.Selected && control.ShowFocusCues && control.Items.FocusedIndex == index)
                e.Canvas.DrawFocusRectangle (bounds, 1);

            // This fixes text positioning for partially shown items
            bounds.Height = control.ScaledItemHeight;
            bounds.Inflate (-4, 0);

            // Draw text
            e.Canvas.DrawText (item.ToString ()!, bounds, control, ContentAlignment.MiddleLeft, maxLines: 1);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Modern.Forms.Design
{
    /// <summary>
    ///  The AdornerWindow is a transparent window that resides ontop of the Designer's Frame. This window is used
    ///  by the BehaviorService to intercept all messages. It also serves as a unified canvas on which to paint Glyphs.
    /// </summary>
    internal class AdornerWindow : Control
    {
        private readonly BehaviorService _behaviorService;

        /// <summary>
        ///  Constructor that parents itself to the Designer Frame and hooks all
        ///  necessary events.
        /// </summary>
        internal AdornerWindow (BehaviorService behaviorService, DesignerFrame designerFrame)
        {
            _behaviorService = behaviorService;
            DesignerFrame = designerFrame;
            Dock = DockStyle.Fill;
            //AllowDrop = true;
            Text = "AdornerWindow";
            //SetStyle (ControlStyles.Opaque, true);

            Dock = DockStyle.Fill;
            SetControlBehavior (ControlBehaviors.Transparent, true);
            designerFrame.Controls.Add (this);

        }

        internal DesignerFrame DesignerFrame { get; }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            _behaviorService.PropagatePaint (e);
        }

        protected override void OnMouseDown (MouseEventArgs e)
        {
            var loc = new Point (e.X - 15, e.Y + 15);

            _behaviorService.PropagateHitTest (loc);

             if (_behaviorService.OnMouseDown (e.Button, loc))
                return;


            DesignerFrame.design_adapter.DoMouseDown (e);
        }
    }
}

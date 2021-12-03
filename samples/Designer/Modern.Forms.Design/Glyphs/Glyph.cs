using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modern.Forms;

namespace Modern.Forms.Design
{
    public abstract class Glyph
    {
        /// <summary>
        ///  This read-only property will return the Bounds associated with
        ///  this Glyph.  The Bounds can be empty.
        /// </summary>
        public virtual Rectangle Bounds => throw new NotImplementedException ();


        /// <summary>
        ///  Abstract method that forces Glyph implementations to provide
        ///  hit test logic.  Given any point - if the Glyph has decided to
        ///  be involved with that location, the Glyph will need to return
        ///  a valid Cursor.  Otherwise, returning null will cause the
        ///  the BehaviorService to simply ignore it.
        /// </summary>
        public abstract Cursor? GetHitTest (Point p);


        /// <summary>
        ///  Abstract method that forces Glyph implementations to provide
        ///  paint logic.  The PaintEventArgs object passed into this method
        ///  contains the Graphics object related to the BehaviorService's
        ///  AdornerWindow.
        /// </summary>
        public abstract void Paint (PaintEventArgs pe);
    }
}

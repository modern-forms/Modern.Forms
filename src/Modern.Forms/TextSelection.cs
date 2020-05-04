using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    /// <summary>
    /// Represents a block of selected text.
    /// </summary>
    public struct TextSelection
    {
        /// <summary>
        ///  Initializes a new instance of the TextSelection struct.
        /// </summary>
        public TextSelection (int start, int end, SKColor color)
        {
            Start = start;
            End = end;
            Color = color;
        }

        /// <summary>
        /// Gets or set the background color of the text selection.
        /// </summary>
        public SKColor Color { get; set; }

        /// <summary>
        /// Gets an empty text selection.
        /// </summary>
        public static TextSelection Empty = new TextSelection ();

        /// <summary>
        /// Gets or sets the end of the text selection.
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Gets a value indicating if the text selection is empty.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty () => Start == End;

        /// <summary>
        /// Gets or sets the beginning of the text selection.
        /// </summary>
        public int Start { get; set; }
    }
}

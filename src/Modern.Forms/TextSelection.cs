using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace Modern.Forms
{
    public struct TextSelection
    {
        public int Start { get; set; }
        public int End { get; set; }
        public SKColor Color { get; set; }

        public TextSelection (int start, int end, SKColor color)
        {
            Start = start;
            End = end;
            Color = color;
        }

        public bool IsEmpty () => Start == End;

        public static TextSelection Empty = new TextSelection ();
    }
}

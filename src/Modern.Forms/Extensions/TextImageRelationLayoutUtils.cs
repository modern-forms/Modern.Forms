using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modern.Forms.Layout;
using SkiaSharp;

namespace Modern.Forms
{
    internal class TextImageRelationLayoutUtils
    {
        public Rectangle Bounds { get; set; }
        public TextImageRelation TextImageRelation { get; set; }

        public SKTypeface? Font { get; set; }
        public string? Text { get; set; }
        public int FontSize { get; set; } = 8;
        public ContentAlignment TextAlign { get; set; }
        public Padding TextPadding { get; set; }

        public Size ImageSize { get; set; }
        public ContentAlignment ImageAlign { get; set; }
        public Padding ImagePadding { get; set; }

        public (Rectangle imageBounds, Rectangle textBounds) Layout ()
        {
            Rectangle text_bounds;
            Rectangle image_bounds;

            var text_size = GetTextSize ().ToSize ();
            text_size.Height++;

            // We only have one thing to layout, or it's overlay so position doesn't matter
            if (ImageSize == Size.Empty || Text?.Length == 0 || TextImageRelation == TextImageRelation.Overlay) {
                var image_size = ImageSize;

                text_bounds = LayoutUtils.Align (text_size, Bounds, TextAlign);
                image_bounds = LayoutUtils.Align (image_size, Bounds, ImageAlign);

                return (image_bounds, text_bounds);
            }

            var max_combined_bounds = Bounds;

            // Combine text & image into one rectangle that we center within maxBounds.
            var combined_size = LayoutUtils.AddAlignedRegion (text_size, ImageSize, TextImageRelation);
            max_combined_bounds.Size = LayoutUtils.UnionSizes (max_combined_bounds.Size, combined_size);
            var combined_bounds = LayoutUtils.Align (combined_size, max_combined_bounds, ContentAlignment.MiddleCenter);

            // ImageEdge indicates whether the combination of ImageAlign and TextImageRelation place
            // the image along the edge of the control.  If so, we can increase the space for text.
            var image_edge = (AnchorStyles)(ImageAlignToRelation (ImageAlign) & TextImageRelation) != AnchorStyles.None;

            // TextEdge indicates whether the combination of TextAlign and TextImageRelation place
            // the text along the edge of the control.  If so, we can increase the space for image.
            var text_edge = (AnchorStyles)(TextAlignToRelation (TextAlign) & TextImageRelation) != AnchorStyles.None;

            if (image_edge) {
                // Just split imageSize off of maxCombinedBounds.
                LayoutUtils.SplitRegion (
                    max_combined_bounds,
                    ImageSize,
                    (AnchorStyles)TextImageRelation,
                    out image_bounds,
                    out text_bounds);
            } else if (text_edge) {
                // Just split textSize off of maxCombinedBounds.
                LayoutUtils.SplitRegion (
                    max_combined_bounds,
                    text_size,
                    (AnchorStyles)LayoutUtils.GetOppositeTextImageRelation (TextImageRelation),
                    out text_bounds,
                    out image_bounds);
            } else {
                // Expand the adjacent regions to maxCombinedBounds (centered) and split the rectangle into
                // imageBounds and textBounds.
                LayoutUtils.SplitRegion (
                    combined_bounds,
                    ImageSize,
                    (AnchorStyles)TextImageRelation,
                    out image_bounds,
                    out text_bounds);
                LayoutUtils.ExpandRegionsToFillBounds (
                    max_combined_bounds,
                    (AnchorStyles)TextImageRelation,
                    ref image_bounds,
                    ref text_bounds);
            }

            // Align text/image within their regions.
            image_bounds = LayoutUtils.Align (ImageSize, image_bounds, ImageAlign);
            text_bounds = LayoutUtils.Align (text_size, text_bounds, TextAlign);

            return (image_bounds, text_bounds);
        }

        private SKSize GetTextSize ()
        {
            if (Text is null || Font is null)
                return SKSize.Empty;

            return TextMeasurer.MeasureText (Text, Font, FontSize);
        }

        // Maps an image align to the set of TextImageRelations that represent the same edge.
        // For example, imageAlign = TopLeft maps to TextImageRelations ImageAboveText (top)
        // and ImageBeforeText (left).
        private static readonly TextImageRelation[] _imageAlignToRelation = new TextImageRelation[]
        {
                /* TopLeft = */       TextImageRelation.ImageAboveText | TextImageRelation.ImageBeforeText,
                /* TopCenter = */     TextImageRelation.ImageAboveText,
                /* TopRight = */      TextImageRelation.ImageAboveText | TextImageRelation.TextBeforeImage,
                /* Invalid */         0,
                /* MiddleLeft = */    TextImageRelation.ImageBeforeText,
                /* MiddleCenter = */  0,
                /* MiddleRight = */   TextImageRelation.TextBeforeImage,
                /* Invalid */         0,
                /* BottomLeft = */    TextImageRelation.TextAboveImage | TextImageRelation.ImageBeforeText,
                /* BottomCenter = */  TextImageRelation.TextAboveImage,
                /* BottomRight = */   TextImageRelation.TextAboveImage | TextImageRelation.TextBeforeImage
        };

        private static TextImageRelation ImageAlignToRelation (ContentAlignment alignment)
            => _imageAlignToRelation[LayoutUtils.ContentAlignmentToIndex (alignment)];

        private static TextImageRelation TextAlignToRelation (ContentAlignment alignment)
            => LayoutUtils.GetOppositeTextImageRelation (ImageAlignToRelation (alignment));
    }
}

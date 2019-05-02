// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2004 Novell, Inc.
//
// Authors:
//	Peter Bartok	pbartok@novell.com
//


// NOT COMPLETE

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace System.Windows.Forms {
	public sealed class ControlPaint {
		#region Local Variables
		static int		RGBMax=255;
		static int		HLSMax=255;
		#endregion	// Local Variables

		#region Private Enumerations


		#region Constructor
		// Prevent a public constructor from being created
		private ControlPaint() {
		}
		#endregion	// Constructor


		#endregion	// Private Enumerations

		#region Helpers
		internal static void Color2HBS(Color color, out int h, out int l, out int s) {
			int	r;
			int	g;
			int	b;
			int	cMax;
			int	cMin;
			int	rDelta;
			int	gDelta;
			int	bDelta;

			r=color.R;
			g=color.G;
			b=color.B;

			cMax = Math.Max(Math.Max(r, g), b);
			cMin = Math.Min(Math.Min(r, g), b);

			l = (((cMax+cMin)*HLSMax)+RGBMax)/(2*RGBMax);

			if (cMax==cMin) {		// Achromatic
				h=0;					// h undefined
				s=0;
				return;
			}

			/* saturation */
			if (l<=(HLSMax/2)) {
				s=(((cMax-cMin)*HLSMax)+((cMax+cMin)/2))/(cMax+cMin);
			} else {
				s=(((cMax-cMin)*HLSMax)+((2*RGBMax-cMax-cMin)/2))/(2*RGBMax-cMax-cMin);
			}

			/* hue */
			rDelta=(((cMax-r)*(HLSMax/6))+((cMax-cMin)/2))/(cMax-cMin);
			gDelta=(((cMax-g)*(HLSMax/6))+((cMax-cMin)/2))/(cMax-cMin);
			bDelta=(((cMax-b)*(HLSMax/6))+((cMax-cMin)/2))/(cMax-cMin);

			if (r == cMax) {
				h=bDelta - gDelta;
			} else if (g == cMax) {
				h=(HLSMax/3) + rDelta - bDelta;
			} else { /* B == cMax */
				h=((2*HLSMax)/3) + gDelta - rDelta;
			}

			if (h<0) {
				h+=HLSMax;
			}

			if (h>HLSMax) {
				h-=HLSMax;
			}
		}

		private static int HueToRGB(int n1, int n2, int hue) {
			if (hue<0) {
				hue+=HLSMax;
			}

			if (hue>HLSMax) {
				hue -= HLSMax;
			}

			/* return r,g, or b value from this tridrant */
			if (hue<(HLSMax/6)) {
				return(n1+(((n2-n1)*hue+(HLSMax/12))/(HLSMax/6)));
			}

			if (hue<(HLSMax/2)) {
				return(n2);
			}

			if (hue<((HLSMax*2)/3)) {
				return(n1+(((n2-n1)*(((HLSMax*2)/3)-hue)+(HLSMax/12))/(HLSMax/6)));
			} else {
				return(n1);
			}
		}

		internal static Color HBS2Color(int hue, int lum, int sat) {
			int	R;
			int	G;
			int	B;
			int	Magic1;
			int	Magic2;

			if (sat == 0) {            /* Achromatic */
				R=G=B=(lum*RGBMax)/HLSMax;
				// FIXME : Should throw exception if hue!=0
			} else {
				if (lum<=(HLSMax/2)) {
					Magic2=(lum*(HLSMax+sat)+(HLSMax/2))/HLSMax;
				} else {
					Magic2=sat+lum-((sat*lum)+(HLSMax/2))/HLSMax;
				}
				Magic1=2*lum-Magic2;

				R = Math.Min(255, (HueToRGB(Magic1,Magic2,hue+(HLSMax/3))*RGBMax+(HLSMax/2))/HLSMax);
				G = Math.Min(255, (HueToRGB(Magic1,Magic2,hue)*RGBMax+(HLSMax/2))/HLSMax);
				B = Math.Min(255, (HueToRGB(Magic1,Magic2,hue-(HLSMax/3))*RGBMax+(HLSMax/2))/HLSMax);
			}
			return (Color.FromArgb(R, G, B));
		}
		#endregion	// Helpers

		#region Public Static Properties
		public static Color ContrastControlDark {
			get { return(SystemColors.ControlDark); }
		}
		#endregion	// Public Static Properties

		#region Public Static Methods
		[MonoTODO ("Not implemented, will throw NotImplementedException")]
		public static IntPtr CreateHBitmap16Bit (Bitmap bitmap, Color background)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO ("Not implemented, will throw NotImplementedException")]
		public static IntPtr CreateHBitmapColorMask (Bitmap bitmap, IntPtr monochromeMask)
		{
			throw new NotImplementedException ();
		}

		[MonoTODO ("Not implemented, will throw NotImplementedException")]
		public static IntPtr CreateHBitmapTransparencyMask (Bitmap bitmap)
		{
			throw new NotImplementedException ();
		}

		public static Color Light(Color baseColor) {
			return Light(baseColor, 0.5f);
		}

		public static Color Light (Color baseColor, float percOfLightLight)
		{
			//if (baseColor.ToArgb () == ThemeEngine.Current.ColorControl.ToArgb ()) {
			//	int r_sub, g_sub, b_sub;
			//	Color color;

			//	if (percOfLightLight <= 0f)
			//		return ThemeEngine.Current.ColorControlLight;

			//	if (percOfLightLight == 1.0f)
			//		return ThemeEngine.Current.ColorControlLightLight;
				
			//	r_sub = ThemeEngine.Current.ColorControlLightLight.R - ThemeEngine.Current.ColorControlLight.R;
			//	g_sub = ThemeEngine.Current.ColorControlLightLight.G - ThemeEngine.Current.ColorControlLight.G;
			//	b_sub = ThemeEngine.Current.ColorControlLightLight.B - ThemeEngine.Current.ColorControlLight.B;

			//	color = Color.FromArgb (ThemeEngine.Current.ColorControlLight.A,
			//			(int) (ThemeEngine.Current.ColorControlLight.R + (r_sub * percOfLightLight)),
			//			(int) (ThemeEngine.Current.ColorControlLight.G + (g_sub * percOfLightLight)),
			//			(int) (ThemeEngine.Current.ColorControlLight.B + (b_sub * percOfLightLight)));
			//	return color;
 		//	}
			
			int H, I, S;

			ControlPaint.Color2HBS (baseColor, out H, out I, out S);
			int NewIntensity = Math.Min (255, I + (int) ((255 - I) * 0.5f * percOfLightLight));
			
			return ControlPaint.HBS2Color (H, NewIntensity, S);
		}

		public static Color LightLight (Color baseColor)
		{
			return Light(baseColor, 1.0f);
		}

		public static Color Dark (Color baseColor)
		{
			return Dark(baseColor, 0.5f);
		}

		public static Color Dark (Color baseColor, float percOfDarkDark)
		{
			//if (baseColor.ToArgb () == ThemeEngine.Current.ColorControl.ToArgb ()) {
				
			//	int r_sub, g_sub, b_sub;
			//	Color color;

			//	if (percOfDarkDark <= 0f)
			//		return ThemeEngine.Current.ColorControlDark;

			//	if (percOfDarkDark == 1.0f)
			//		return ThemeEngine.Current.ColorControlDarkDark;

			//	r_sub = ThemeEngine.Current.ColorControlDarkDark.R - ThemeEngine.Current.ColorControlDark.R;
			//	g_sub = ThemeEngine.Current.ColorControlDarkDark.G - ThemeEngine.Current.ColorControlDark.G;
			//	b_sub = ThemeEngine.Current.ColorControlDarkDark.B - ThemeEngine.Current.ColorControlDark.B;

			//	color = Color.FromArgb (ThemeEngine.Current.ColorControlDark.A,
			//			(int) (ThemeEngine.Current.ColorControlDark.R + (r_sub * percOfDarkDark)),
			//			(int) (ThemeEngine.Current.ColorControlDark.G + (g_sub * percOfDarkDark)),
			//			(int) (ThemeEngine.Current.ColorControlDark.B + (b_sub * percOfDarkDark)));
			//	return color;
 		//	}
		
			int H, I, S;

			ControlPaint.Color2HBS(baseColor, out H, out I, out S);
			int PreIntensity = Math.Max (0, I - (int) (I * 0.333f));
			int NewIntensity = Math.Max (0, PreIntensity - (int) (PreIntensity * percOfDarkDark));
			return ControlPaint.HBS2Color(H, NewIntensity, S);
		}

		public static Color DarkDark (Color baseColor)
		{
			return Dark(baseColor, 1.0f);
		}
		#endregion	// Public Static Methods
	}
}

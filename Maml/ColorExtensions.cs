using System.Drawing;
using Windows.Win32.Foundation;

namespace Maml;

internal static class ColorExtensions
{

    public static COLORREF ToColorRef(this Color color)
    {
        return new COLORREF((uint)((color.B << 16) | (color.G << 8) | (color.R << 0)));
    }

    public static Color FromArgb(uint color) => Color.FromArgb(unchecked((int)color));
}
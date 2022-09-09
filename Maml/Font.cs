using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using static Windows.Win32.PInvoke;

namespace Maml;

[Flags]
public enum FontStyle
{
    Normal      = 0,
    Italic      = 1 << 0,
    Underline   = 1 << 1,
    Strikeout   = 1 << 2,
}

public record FontData
{
    public string Name { get; init; } = "Arial";
    public int Size { get; init; } = 16;
    public int Weight { get; init; } = 400;
    public FontStyle Style { get; init; } = FontStyle.Normal;
}

internal class FontCache
{
    private readonly Dictionary<FontData, HFONT> Fonts = new();

    ~FontCache()
    {
        foreach (var kv in Fonts)
        {
            if (DeleteObject((HGDIOBJ)kv.Value.Value) == 0)
            {
                throw new Exception("DeleteObject() Failed");
            }
        }
    }

    public HFONT GetFont(FontData fontData)
    {
        if (Fonts.TryGetValue(fontData, out var hfont))
        {
            return hfont;
        }

        hfont = AllocateFont(fontData);
        Fonts.Add(fontData, hfont);
        return hfont;
    }

    unsafe private HFONT AllocateFont(FontData fontData)
    {
        fixed (char* pFontName = fontData.Name)
        {
            PWSTR fontNameStr = new(pFontName);
            return CreateFont(
                fontData.Size, 0,
                0, 0,
                fontData.Weight,
                (uint)(fontData.Style & FontStyle.Italic),
                (uint)(fontData.Style & FontStyle.Underline),
                (uint)(fontData.Style & FontStyle.Strikeout),
                (uint)FONT_CHARSET.DEFAULT_CHARSET,
                FONT_OUTPUT_PRECISION.OUT_DEFAULT_PRECIS,
                FONT_CLIP_PRECISION.CLIP_DEFAULT_PRECIS,
                FONT_QUALITY.CLEARTYPE_QUALITY,
                FONT_PITCH_AND_FAMILY.FF_DONTCARE,
                fontNameStr);
        }
    }
}

internal static class HFONTExtensions
{
    internal static HGDIOBJ ToHGDIOBJ(this HFONT hfont) => (HGDIOBJ)hfont.Value;
}

internal static class StringExtensions
{
    internal static PWSTR ToPWSTR(this string str)
    {
        var pStr = Marshal.StringToHGlobalUni(str);
        unsafe
        {
            return new((char*)pStr.ToPointer());
        }
        /*
        unsafe
        {
            fixed (char* pStr = str)
            {
                return new(pStr);
            }
        }
        */
    }
}

using Maml.Math;
using System;
using System.Runtime.InteropServices;

namespace Maml.Graphics;

[StructLayout(LayoutKind.Explicit, Size = 16)]
public partial struct Color : IEquatable<Color>
{
	[FieldOffset(0)] public float R;
	[FieldOffset(4)] public float G;
	[FieldOffset(8)] public float B;
	[FieldOffset(12)] public float A;

	public Color(uint hex)
	{
		R = (float)(((hex & 0xff000000) >> 24) / (double)0xff);
		G = (float)(((hex & 0x00ff0000) >> 16) / (double)0xff);
		B = (float)(((hex & 0x0000ff00) >> 08) / (double)0xff);
		A = (float)(((hex & 0x000000ff) >> 00) / (double)0xff);
	}

	public uint ToUint()
	{
		uint r = (uint)(R * 0xff);
		uint g = (uint)(G * 0xff);
		uint b = (uint)(B * 0xff);
		uint a = (uint)(A * 0xff);
		return r << 24 | g << 16 | b << 8 | a;
	}

	public static Color Lerp(in Color lhs, in Color rhs, in double t) => new()
	{
		R = (float)Unit.Lerp(lhs.R, rhs.R, t),
		G = (float)Unit.Lerp(lhs.G, rhs.G, t),
		B = (float)Unit.Lerp(lhs.B, rhs.B, t),
		A = (float)Unit.Lerp(lhs.A, rhs.A, t),
	};

	public static bool ApproxEqual(in Color lhs, in Color rhs, double epsilon = Unit.Epsilon) =>
		Unit.ApproxEqual(lhs.R, rhs.R, epsilon) &&
		Unit.ApproxEqual(lhs.G, rhs.G, epsilon) &&
		Unit.ApproxEqual(lhs.B, rhs.B, epsilon) &&
		Unit.ApproxEqual(lhs.A, rhs.A, epsilon);

	public override bool Equals(object? obj) => obj is Color color && Equals(color);
	public bool Equals(Color other) => R == other.R && G == other.G && B == other.B && A == other.A;
	public override int GetHashCode() => HashCode.Combine(R, G, B, A);

	public static bool operator ==(Color left, Color right) => left.Equals(right);
	public static bool operator !=(Color left, Color right) => !(left == right);
}

public static class Colors
{
	public static readonly Color Black = new(0x000000ff);
	public static readonly Color Silver = new(0xc0c0c0ff);
	public static readonly Color Gray = new(0x808080ff);
	public static readonly Color White = new(0xffffffff);
	public static readonly Color Maroon = new(0x800000ff);
	public static readonly Color Red = new(0xff0000ff);
	public static readonly Color Purple = new(0x800080ff);
	public static readonly Color Fuchsia = new(0xff00ffff);
	public static readonly Color Green = new(0x008000ff);
	public static readonly Color Lime = new(0x00ff00ff);
	public static readonly Color Olive = new(0x808000ff);
	public static readonly Color Yellow = new(0xffff00ff);
	public static readonly Color Navy = new(0x000080ff);
	public static readonly Color Blue = new(0x0000ffff);
	public static readonly Color Teal = new(0x008080ff);
	public static readonly Color Aqua = new(0x00ffffff);
	public static readonly Color Orange = new(0xffa500ff);
	public static readonly Color AliceBlue = new(0xf0f8ffff);
	public static readonly Color AntiqueWhite = new(0xfaebd7ff);
	public static readonly Color Aquamarine = new(0x7fffd4ff);
	public static readonly Color Azure = new(0xf0ffffff);
	public static readonly Color Beige = new(0xf5f5dcff);
	public static readonly Color Bisque = new(0xffe4c4ff);
	public static readonly Color BlanchedAlmond = new(0xffebcdff);
	public static readonly Color BlueViolet = new(0x8a2be2ff);
	public static readonly Color Brown = new(0xa52a2aff);
	public static readonly Color Burlywood = new(0xdeb887ff);
	public static readonly Color CadetBlue = new(0x5f9ea0ff);
	public static readonly Color Chartreuse = new(0x7fff00ff);
	public static readonly Color Chocolate = new(0xd2691eff);
	public static readonly Color Coral = new(0xff7f50ff);
	public static readonly Color CornflowerBlue = new(0x6495edff);
	public static readonly Color Cornsilk = new(0xfff8dcff);
	public static readonly Color Crimson = new(0xdc143cff);
	public static readonly Color Cyan = new(0x00ffffff);
	public static readonly Color DarkBlue = new(0x00008bff);
	public static readonly Color DarkCyan = new(0x008b8bff);
	public static readonly Color DarkGoldenrod = new(0xb8860bff);
	public static readonly Color DarkGray = new(0xa9a9a9ff);
	public static readonly Color DarkGreen = new(0x006400ff);
	public static readonly Color DarkGrey = new(0xa9a9a9ff);
	public static readonly Color DarkKhaki = new(0xbdb76bff);
	public static readonly Color DarkMagenta = new(0x8b008bff);
	public static readonly Color DarkOliveGreen = new(0x556b2fff);
	public static readonly Color DarkOrange = new(0xff8c00ff);
	public static readonly Color DarkOrchid = new(0x9932ccff);
	public static readonly Color DarkRed = new(0x8b0000ff);
	public static readonly Color DarkSalmon = new(0xe9967aff);
	public static readonly Color DarkSeagreen = new(0x8fbc8fff);
	public static readonly Color DarkSlateBlue = new(0x483d8bff);
	public static readonly Color DarkSlateGray = new(0x2f4f4fff);
	public static readonly Color DarkSlateGrey = new(0x2f4f4fff);
	public static readonly Color DarkTurquoise = new(0x00ced1ff);
	public static readonly Color DarkViolet = new(0x9400d3ff);
	public static readonly Color DeepPink = new(0xff1493ff);
	public static readonly Color DeepSkyBlue = new(0x00bfffff);
	public static readonly Color DimGray = new(0x696969ff);
	public static readonly Color DimGrey = new(0x696969ff);
	public static readonly Color DodgerBlue = new(0x1e90ffff);
	public static readonly Color Firebrick = new(0xb22222ff);
	public static readonly Color FloralWhite = new(0xfffaf0ff);
	public static readonly Color ForestGreen = new(0x228b22ff);
	public static readonly Color Gainsboro = new(0xdcdcdcff);
	public static readonly Color GhostWhite = new(0xf8f8ffff);
	public static readonly Color Gold = new(0xffd700ff);
	public static readonly Color Goldenrod = new(0xdaa520ff);
	public static readonly Color GreenYellow = new(0xadff2fff);
	public static readonly Color Grey = new(0x808080ff);
	public static readonly Color Honeydew = new(0xf0fff0ff);
	public static readonly Color HotPink = new(0xff69b4ff);
	public static readonly Color IndianRed = new(0xcd5c5cff);
	public static readonly Color Indigo = new(0x4b0082ff);
	public static readonly Color Ivory = new(0xfffff0ff);
	public static readonly Color Khaki = new(0xf0e68cff);
	public static readonly Color Lavender = new(0xe6e6faff);
	public static readonly Color LavenderBlush = new(0xfff0f5ff);
	public static readonly Color LawnGreen = new(0x7cfc00ff);
	public static readonly Color LemonChiffon = new(0xfffacdff);
	public static readonly Color LightBlue = new(0xadd8e6ff);
	public static readonly Color LightCoral = new(0xf08080ff);
	public static readonly Color LightCyan = new(0xe0ffffff);
	public static readonly Color LightGoldenrodYellow = new(0xfafad2ff);
	public static readonly Color LightGray = new(0xd3d3d3ff);
	public static readonly Color LightGreen = new(0x90ee90ff);
	public static readonly Color LightGrey = new(0xd3d3d3ff);
	public static readonly Color LightPink = new(0xffb6c1ff);
	public static readonly Color LightSalmon = new(0xffa07aff);
	public static readonly Color LightSeagreen = new(0x20b2aaff);
	public static readonly Color LightSkyblue = new(0x87cefaff);
	public static readonly Color LightSlateGray = new(0x778899ff);
	public static readonly Color LightSlategrey = new(0x778899ff);
	public static readonly Color LightSteelblue = new(0xb0c4deff);
	public static readonly Color LightYellow = new(0xffffe0ff);
	public static readonly Color LimeGreen = new(0x32cd32ff);
	public static readonly Color Linen = new(0xfaf0e6ff);
	public static readonly Color Magenta = new(0xff00ffff);
	public static readonly Color MediumAquamarine = new(0x66cdaaff);
	public static readonly Color MediumBlue = new(0x0000cdff);
	public static readonly Color MediumOrchid = new(0xba55d3ff);
	public static readonly Color MediumPurple = new(0x9370dbff);
	public static readonly Color MediumSeaGreen = new(0x3cb371ff);
	public static readonly Color MediumSlateBlue = new(0x7b68eeff);
	public static readonly Color MediumSpringGreen = new(0x00fa9aff);
	public static readonly Color MediumTurquoise = new(0x48d1ccff);
	public static readonly Color MediumVioletRed = new(0xc71585ff);
	public static readonly Color MidnightBlue = new(0x191970ff);
	public static readonly Color MintCream = new(0xf5fffaff);
	public static readonly Color MistyRose = new(0xffe4e1ff);
	public static readonly Color Moccasin = new(0xffe4b5ff);
	public static readonly Color NavajoWhite = new(0xffdeadff);
	public static readonly Color OldLace = new(0xfdf5e6ff);
	public static readonly Color OliveDrab = new(0x6b8e23ff);
	public static readonly Color OrangeRed = new(0xff4500ff);
	public static readonly Color Orchid = new(0xda70d6ff);
	public static readonly Color PaleGoldenrod = new(0xeee8aaff);
	public static readonly Color PaleGreen = new(0x98fb98ff);
	public static readonly Color PaleTurquoise = new(0xafeeeeff);
	public static readonly Color PaleVioletred = new(0xdb7093ff);
	public static readonly Color PapayaWhip = new(0xffefd5ff);
	public static readonly Color PeachPuff = new(0xffdab9ff);
	public static readonly Color Peru = new(0xcd853fff);
	public static readonly Color Pink = new(0xffc0cbff);
	public static readonly Color Plum = new(0xdda0ddff);
	public static readonly Color PowderBlue = new(0xb0e0e6ff);
	public static readonly Color RosyBrown = new(0xbc8f8fff);
	public static readonly Color RoyalBlue = new(0x4169e1ff);
	public static readonly Color SaddleBrown = new(0x8b4513ff);
	public static readonly Color Salmon = new(0xfa8072ff);
	public static readonly Color SandyBrown = new(0xf4a460ff);
	public static readonly Color SeaGreen = new(0x2e8b57ff);
	public static readonly Color SeaShell = new(0xfff5eeff);
	public static readonly Color Sienna = new(0xa0522dff);
	public static readonly Color SkyBlue = new(0x87ceebff);
	public static readonly Color SlateBlue = new(0x6a5acdff);
	public static readonly Color SlateGray = new(0x708090ff);
	public static readonly Color SlateGrey = new(0x708090ff);
	public static readonly Color Snow = new(0xfffafaff);
	public static readonly Color SpringGreen = new(0x00ff7fff);
	public static readonly Color SteelBlue = new(0x4682b4ff);
	public static readonly Color Tan = new(0xd2b48cff);
	public static readonly Color Thistle = new(0xd8bfd8ff);
	public static readonly Color Tomato = new(0xff6347ff);
	public static readonly Color Transparent = new(0x00000000);
	public static readonly Color Turquoise = new(0x40e0d0ff);
	public static readonly Color Violet = new(0xee82eeff);
	public static readonly Color Wheat = new(0xf5deb3ff);
	public static readonly Color WhiteSmoke = new(0xf5f5f5ff);
	public static readonly Color YellowGreen = new(0x9acd32ff);
	public static readonly Color RebeccaPurple = new(0x663399ff);
}

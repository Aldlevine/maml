using Maml.Math;
using Maml.Observable;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maml.Graphics;

public enum FontStyle
{
	Normal,
	Oblique,
	Italic,
}

public enum FlowDirection
{
	LeftToRight,
	RightToLeft,
}

public struct Font
{
	public string Name;
	public double Size;
	public int Weight;
	public FontStyle Style;
}

public partial class TextGeometry : Geometry
{
	public static BasicProperty<TextGeometry, string> TextProperty = new("");
	public string Text
	{
		get => TextProperty[this].Get();
		set => TextProperty[this].Set(value);
	}

	public static BasicProperty<TextGeometry, Font> FontProperty = new(default);
	public Font Font
	{
		get => FontProperty[this].Get();
		set => FontProperty[this].Set(value);
	}

	public static BasicProperty<TextGeometry, FlowDirection> FlowDirectionProperty = new(default);
	public FlowDirection FlowDirection
	{
		get => FlowDirectionProperty[this].Get();
		set => FlowDirectionProperty[this].Set(value);
	}
}

// public struct TextFormat
// {
// 	public Font Font;
// 	public double LineHeight;
// 	public FlowDirection FlowDirection;
// }
// 
// public partial class TextLayout : Resource
// {
// 	public static BasicProperty<TextLayout, TextFormat> FormatProperty = new(default);
// 	public TextFormat Format
// 	{
// 		get => FormatProperty[this].Get();
// 		set => FormatProperty[this].Set(value);
// 	}
// 
// 	public static BasicProperty<TextLayout, string> TextProperty = new("");
// 	public string Text
// 	{
// 		get => TextProperty[this].Get();
// 		set => TextProperty[this].Set(value);
// 	}
// 
// 	public static BasicProperty<TextLayout, Rect> LayoutRectProperty = new(new());
// 	public Rect LayoutRect
// 	{
// 		get => LayoutRectProperty[this].Get();
// 		set => LayoutRectProperty[this].Set(value);
// 	}
// }

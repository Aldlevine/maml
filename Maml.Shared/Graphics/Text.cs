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

public enum FontWeight
{
	Thin = 100,
	ExtraLight = 200,
	Light = 300,
	SemiLight = 350,
	Normal = 400,
	Medium = 500,
	SemiBold = 600,
	Bold = 700,
	ExtraBold = 800,
	Heavy = 900,
	ExtraHeavy = 950,
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
	public FontWeight Weight;
	public FontStyle Style;
}

public partial class Text : Resource
{
	public static BasicProperty<Text, string> StringProperty = new("")
	{
		Changed = (self) => self.IsDirty = true,
	};
	public string String
	{
		get => StringProperty[this].Get();
		set => StringProperty[this].Set(value);
	}

	public static BasicProperty<Text, Font> FontProperty = new(default)
	{
		Changed = (self) => self.IsDirty = true,
	};
	public Font Font
	{
		get => FontProperty[this].Get();
		set => FontProperty[this].Set(value);
	}

	public static BasicProperty<Text, FlowDirection> FlowDirectionProperty = new(default)
	{
		Changed = (self) => self.IsDirty = true,
	};
	public FlowDirection FlowDirection
	{
		get => FlowDirectionProperty[this].Get();
		set => FlowDirectionProperty[this].Set(value);
	}

	public static BasicProperty<Text, Vector2> SizeProperty = new(default);
	public Vector2 Size
	{
		get => SizeProperty[this].Get();
		private set => SizeProperty[this].Set(value);
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

using Maml.Math;
using Maml.Observable;

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

public enum WrappingMode
{
	Normal,
	None,
	Character,
	Word,
}

public abstract record LineHeight(double Value)
{
	public record Absolute(double Value) : LineHeight(Value);
	public record Relative(double Value) : LineHeight(Value);

	public static implicit operator LineHeight(double value) => new Absolute(value);
	public static implicit operator double(LineHeight lineHeight) => lineHeight.Value;
}

public static class LineheightExtensions
{
	public static LineHeight Absolute(this double self) => new LineHeight.Absolute(self);
	public static LineHeight Relative(this double self) => new LineHeight.Relative(self);
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
	unsafe public Text()
	{
		Engine.Singleton.Window.Measure += (s, r) => GetResource(r);
	}

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

	public static BasicProperty<Text, WrappingMode> WrappingModeProperty = new(WrappingMode.Normal)
	{
		Changed = (self) => self.IsDirty = true,
	};
	public WrappingMode WrappingMode
	{
		get => WrappingModeProperty[this].Get();
		set => WrappingModeProperty[this].Set(value);
	}

	public static BasicProperty<Text, LineHeight> LineHeightProperty = new(new LineHeight.Relative(1.2))
	{
		Changed = (self) => self.IsDirty = true,
	};
	public LineHeight LineHeight
	{
		get => LineHeightProperty[this].Get();
		set => LineHeightProperty[this].Set(value);
	}

	private Vector2 maxSize = new(double.PositiveInfinity, double.PositiveInfinity);
	public static ComputedProperty<Text, Vector2> MaxSizeProperty = new()
	{
		Get = (self) => self.maxSize,
		Set = (self, value) =>
		{
			var lines = self.String.Split('\n').Length;
			var lineHeight = self.LineHeight switch
			{
				LineHeight.Relative => self.LineHeight.Value * self.Font.Size,
				_ => self.LineHeight.Value,
			};

			if (
			(self.Size.X >= value.X && value.X < self.maxSize.X) ||
			(self.Size.Y >= value.Y && value.Y < self.maxSize.Y) ||
			(self.LineCount > lines && self.Size.X <= value.X && value.X > self.maxSize.X) ||
			(self.LineCount * lineHeight > self.Size.Y && self.Size.Y <= value.Y && value.Y > self.maxSize.Y)
			)
			{
				self.IsDirty = true;
			}
			self.maxSize = Vector2.Max(value, Vector2.Zero);
		},
	};
	public Vector2 MaxSize
	{
		get => MaxSizeProperty[this].Get();
		set => MaxSizeProperty[this].Set(value);
	}

	// Computed by layout, but not actually computed properties.

	public static BasicProperty<Text, Vector2> SizeProperty = new(default);
	public Vector2 Size
	{
		get => SizeProperty[this].Get();
		private set => SizeProperty[this].Set(value);
	}

	public static BasicProperty<Text, uint> LineCountProperty = new(default);
	public uint LineCount
	{
		get => LineCountProperty[this].Get();
		private set => LineCountProperty[this].Set(value);
	}
}


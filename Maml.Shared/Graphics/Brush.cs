namespace Maml.Graphics;

public abstract partial class Brush : Resource
{

}

public partial class ColorBrush : Brush
{
	private Color _Color;
	public Color Color
	{
		get => _Color;
		set
		{
			if (_Color != value)
			{
				_Color = value;
				IsDirty = true;
				RaiseChanged(this, new());
			}
		}
	}

	public ColorBrush() : base() { }

	public ColorBrush(ColorBrush colorBrush)
	{
		Color = colorBrush.Color;
	}
}

/*
using Maml.Math;
using System;
using System.Collections.Generic;

namespace Maml.Graphics;

public abstract class Brush { }

// Solid

public class ColorBrush : Brush, IEquatable<ColorBrush?>
{
	public Color Color;

	public override bool Equals(object? obj) => Equals(obj as ColorBrush);
	public bool Equals(ColorBrush? other) => other is not null && EqualityComparer<Color>.Default.Equals(Color, other.Color);
	public override int GetHashCode() => HashCode.Combine(Color);

	public static bool operator ==(ColorBrush? left, ColorBrush? right) => EqualityComparer<ColorBrush>.Default.Equals(left, right);
	public static bool operator !=(ColorBrush? left, ColorBrush? right) => !(left == right);
}

// Gradients

public abstract class GradientBrush : Brush
{
	public ColorStopCollection ColorStops { get; init; } = new();
}

public class LinearGradientBrush : GradientBrush, IEquatable<LinearGradientBrush?>
{
	public Vector2 Start;
	public Vector2 End;

	public override bool Equals(object? obj) => Equals(obj as LinearGradientBrush);
	public bool Equals(LinearGradientBrush? other)
	{
		return other is not null
		 && base.Equals(other)
		 && EqualityComparer<ColorStopCollection>.Default.Equals(ColorStops, other.ColorStops)
		 && EqualityComparer<Vector2>.Default.Equals(Start, other.Start)
		 && EqualityComparer<Vector2>.Default.Equals(End, other.End);
	}

	public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), ColorStops, Start, End);

	public static bool operator ==(LinearGradientBrush? left, LinearGradientBrush? right) => EqualityComparer<LinearGradientBrush>.Default.Equals(left, right);
	public static bool operator !=(LinearGradientBrush? left, LinearGradientBrush? right) => !(left == right);
}

public class RadialGradientBrush : GradientBrush, IEquatable<RadialGradientBrush?>
{
	public Vector2 Origin;
	public Vector2 Offset;
	public Vector2 Radii;

	public override bool Equals(object? obj) => Equals(obj as RadialGradientBrush);
	public bool Equals(RadialGradientBrush? other)
	{
		return other is not null
		 && EqualityComparer<ColorStopCollection>.Default.Equals(ColorStops, other.ColorStops)
		 && EqualityComparer<Vector2>.Default.Equals(Origin, other.Origin)
		 && EqualityComparer<Vector2>.Default.Equals(Offset, other.Offset)
		 && EqualityComparer<Vector2>.Default.Equals(Radii, other.Radii);
	}

	public override int GetHashCode() => HashCode.Combine(ColorStops, Origin, Offset, Radii);

	public static bool operator ==(RadialGradientBrush? left, RadialGradientBrush? right) => EqualityComparer<RadialGradientBrush>.Default.Equals(left, right);
	public static bool operator !=(RadialGradientBrush? left, RadialGradientBrush? right) => !(left == right);
}

// Color Stops

public record struct ColorStop(Color Color, double Offset) { }

public class ColorStopCollection : SortedList<double, ColorStop>, IEquatable<ColorStopCollection?>
{
	public void Add(ColorStop colorStop) => Add(colorStop.Offset, colorStop);

	public override bool Equals(object? obj) => Equals(obj as ColorStopCollection);

	public bool Equals(ColorStopCollection? other)
	{
		return other is not null
		 && EqualityComparer<IComparer<double>>.Default.Equals(Comparer, other.Comparer)
		 && Count == other.Count
		 && EqualityComparer<IList<double>>.Default.Equals(Keys, other.Keys)
		 && EqualityComparer<IList<ColorStop>>.Default.Equals(Values, other.Values);
	}

	public override int GetHashCode() => HashCode.Combine(Comparer, Count, Keys, Values);

	public static bool operator ==(ColorStopCollection? left, ColorStopCollection? right) => EqualityComparer<ColorStopCollection>.Default.Equals(left, right);

	public static bool operator !=(ColorStopCollection? left, ColorStopCollection? right) => !(left == right);
}
*/

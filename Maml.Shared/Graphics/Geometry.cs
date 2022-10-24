using Maml.Math;
using Maml.Observable;

namespace Maml.Graphics;

public abstract partial class Geometry : Resource
{
	public abstract Rect GetBoundingRect();
}

public partial class EllipseGeometry : Geometry
{
	public static BasicProperty<EllipseGeometry, Ellipse> EllipseProperty { get; } = new(default)
	{
		Changed = (self) => self.IsDirty = true,
	};
	public Ellipse Ellipse
	{
		get => EllipseProperty[this].Get();
		set => EllipseProperty[this].Set(value);
	}


	public EllipseGeometry() : base() { }

	public EllipseGeometry(EllipseGeometry ellipseGeometry)
	{
		Ellipse = ellipseGeometry.Ellipse;
	}

	public override Rect GetBoundingRect() => new() { Position = Ellipse.Center - Ellipse.Radius, Size = Ellipse.Radius, };
}

public partial class LineGeometry : Geometry
{
	public static BasicProperty<LineGeometry, Line> LineProperty { get; } = new(default)
	{
		Changed = (self) => self.IsDirty = true,
	};
	public Line Line
	{
		get => LineProperty[this].Get();
		set => LineProperty[this].Set(value);
	}

	public LineGeometry() : base() { }

	public LineGeometry(LineGeometry lineGeometry)
	{
		Line = lineGeometry.Line;
	}
	public override Rect GetBoundingRect() => new() { Position = Vector2.Min(Line.Start, Line.End), End = Vector2.Max(Line.Start, Line.End), };
}

public partial class RectGeometry : Geometry
{
	public static BasicProperty<RectGeometry, Rect> RectProperty { get; } = new(default)
	{
		Changed = (self) => self.IsDirty = true,
	};
	public Rect Rect
	{
		get => RectProperty[this].Get();
		set => RectProperty[this].Set(value);
	}

	public RectGeometry() : base() { }

	public RectGeometry(RectGeometry rectGeometry)
	{
		Rect = rectGeometry.Rect;
	}

	public override Rect GetBoundingRect() => Rect;
}


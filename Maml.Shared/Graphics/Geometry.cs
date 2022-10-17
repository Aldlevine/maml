using Maml.Math;
using Maml.Observable;

namespace Maml.Graphics;

public abstract partial class Geometry : Resource
{
	public abstract Rect GetBoundingRect();
}

public partial class EllipseGeometry : Geometry
{
	private Ellipse ellipse;
	public Ellipse Ellipse
	{
		get => ellipse;
		set
		{
			ellipse = value;
			IsDirty = true;
		}
	}

	public EllipseGeometry() : base() { }

	public EllipseGeometry(EllipseGeometry ellipseGeometry)
	{
		Ellipse = ellipseGeometry.Ellipse;
	}

	public override Rect GetBoundingRect() => new Rect { Position = Ellipse.Center - Ellipse.Radius, Size = Ellipse.Radius, };
}

public partial class LineGeometry : Geometry
{
	private Line line;
	public Line Line
	{
		get => line;
		set
		{
			line = value;
			IsDirty = true;
		}
	}

	public LineGeometry() : base() { }

	public LineGeometry(LineGeometry lineGeometry)
	{
		Line = lineGeometry.Line;
	}
	public override Rect GetBoundingRect() => new Rect { Position = Vector2.Min(Line.Start, Line.End), End = Vector2.Max(Line.Start, Line.End), };
}

public partial class RectGeometry : Geometry
{
	//private Rect rect;
	//public Rect Rect
	//{
	//	get => rect;
	//	set
	//	{
	//		rect = value;
	//		IsDirty = true;
	//	}
	//}
	public static BasicProperty<RectGeometry, Rect> RectProperty = new(default)
	{
		Changed = (self) =>
		{
			self.IsDirty = true;
		},
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

//public partial class TextGeometry : Geometry
//{
//	public static BasicProperty<TextGeometry, string> TextProperty = new("");
//	public string Text
//	{
//		get => TextProperty[this].Get();
//		set => TextProperty[this].Set(value);
//	}

//	public static BasicProperty<TextGeometry, Vector2> MaxSizeProperty = new(Vector2.Zero);
//	public Vector2 MaxSize
//	{
//		get => MaxSizeProperty[this].Get();
//		set => MaxSizeProperty[this].Set(value);
//	}
//}

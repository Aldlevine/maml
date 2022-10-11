using Maml.Math;
using Maml.Observable;

namespace Maml.Graphics;

public abstract partial class Geometry : Resource { }

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
}

public partial class RectGeometry : Geometry
{
	private Rect rect;
	public Rect Rect
	{
		get => rect;
		set
		{
			rect = value;
			IsDirty = true;
		}
	}

	public RectGeometry() : base() { }

	public RectGeometry(RectGeometry rectGeometry)
	{
		Rect = rectGeometry.Rect;
	}
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

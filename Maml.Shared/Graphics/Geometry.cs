using Maml.Math;

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

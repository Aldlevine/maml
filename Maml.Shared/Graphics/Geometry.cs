using Maml.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maml.Graphics;

public abstract partial class Geometry: Resource { }

public partial class EllipseGeometry: Geometry
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
}

public partial class LineGeometry: Geometry
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
}

public partial class RectGeometry: Geometry
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
}

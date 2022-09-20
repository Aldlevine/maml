using System;
using System.Collections.Generic;
using System.Text;

namespace Maml.Geometry;

public partial struct Ellipse
{
	public Vector2 Center { get; set; } = Vector2.Zero;
	public Vector2 Radius { get; set; } = Vector2.Zero;
	public Ellipse() { }
}

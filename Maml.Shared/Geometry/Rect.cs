using System;
using System.Collections.Generic;
using System.Text;

namespace Maml.Geometry;

public partial struct Rect
{
	public Vector2 Position { get; set; } = Vector2.Zero;
	public Vector2 Size { get; set; } = Vector2.Zero;
	public Rect() { }
}


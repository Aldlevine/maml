namespace Maml.Math;

public partial struct Ellipse : IShape
{
	public Vector2 Center { get; set; } = Vector2.Zero;
	public Vector2 Radius { get; set; } = Vector2.Zero;
	public Ellipse() { }

	public bool HasPoint(in Vector2 point)
	{
		var x = point.X - Center.X;
		var y = point.Y - Center.Y;
		var rx = Radius.X;
		var ry = Radius.Y;
		var p = ((x * x) / (rx * rx)) + ((y * y) / (ry * ry));
		return p <= 1;
	}

	public Rect GetBoundingRect(in Transform transform) => transform * new Rect { Position = Center - Radius, Size = Radius * 2 };
}

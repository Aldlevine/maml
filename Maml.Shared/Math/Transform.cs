namespace Maml.Math;

public partial struct Transform
{
	public Vector2 X { get; set; } = Vector2.Right;
	public Vector2 Y { get; set; } = Vector2.Down;
	public Vector2 Origin { get; set; } = Vector2.Zero;

	public Transform(Vector2 x, Vector2 y, Vector2 origin)
	{
		X = x;
		Y = y;
		Origin = origin;
	}

	public static readonly Transform Identity = new(
		new(1, 0),
		new(0, 1),
		new(0, 0));

	public static readonly Transform PixelIdentity = new(
		new(1, 0),
		new(0, 1),
		new(-0.5f, -0.5f));

	public Transform Translated(Vector2 offset) => new Transform(X, Y, Origin + offset);

	public Transform Transformed(Transform other)
	{
		var result = this;
		result.Origin = result.xform(other.Origin);
		result.X = new(result.tdotx(other.X), result.tdoty(other.X));
		result.Y = new(result.tdotx(other.Y), result.tdoty(other.Y));
		return result;
	}

	public Transform Scaled(Vector2 scale) => new()
	{
		X = new(X.X * scale.X, X.Y * scale.Y),
		Y = new(Y.X * scale.X, Y.Y * scale.Y),
		Origin = new Vector2(Origin.X, Origin.Y) * scale,
	};

	// ooooh
	private double tdotx(Vector2 v) => X.X * v.X + Y.X * v.Y;
	private double tdoty(Vector2 v) => X.Y * v.X + Y.Y * v.Y;
	private Vector2 xform(Vector2 v) => new Vector2(tdotx(v), tdoty(v)) + Origin;
}

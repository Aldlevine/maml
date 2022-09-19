namespace Maml.Geometry;

public partial struct Transform
{
	public Vector2 X = Vector2.Right;
	public Vector2 Y = Vector2.Down;
	public Vector2 Origin = Vector2.Zero;

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
}

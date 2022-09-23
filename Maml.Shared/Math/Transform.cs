using System.Numerics;

namespace Maml.Math;


public partial struct Transform
{
	// public Vector2 X { get; set; } = Vector2.Right;
	// public Vector2 Y { get; set; } = Vector2.Down;
	// public Vector2 Origin { get; set; } = Vector2.Zero;

	// public Vector2 X => new Vector2(matrix.M11, matrix.M12);
	// public Vector2 Y => new Vector2(matrix.M21, matrix.M22);
	// public Vector2 Origin => new Vector2(matrix.M31, matrix.M32);
	public Vector2 X
	{
		get => new Vector2(matrix.M11, matrix.M12);
		set
		{
			matrix.M11 = (float)value.X;
			matrix.M12 = (float)value.Y;
		}
	}

	public Vector2 Y
	{
		get => new Vector2(matrix.M21, matrix.M22);
		set
		{
			matrix.M21 = (float)value.X;
			matrix.M22 = (float)value.Y;
		}
	}

	public Vector2 Origin
	{
		get => new Vector2(matrix.M31, matrix.M32);
		set
		{
			matrix.M31 = (float)value.X;
			matrix.M32 = (float)value.Y;
		}
	}

	public Vector2 Scale
	{
		get
		{
			double detSign = System.Math.Sign(matrix.GetDeterminant());
			return new(X.Length(), detSign * Y.Length());
		}
		set
		{
			X = X.Normalized();
			Y = Y.Normalized();
			X *= new Vector2(value.X, value.X);
			Y *= new Vector2(value.Y, value.Y);
		}
	}


	private Matrix3x2 matrix;

	public Transform(Vector2 x, Vector2 y, Vector2 origin)
	{
		// X = x;
		// Y = y;
		// Origin = origin;
		matrix = new((float)x.X, (float)x.Y, (float)y.X, (float)y.Y, (float)origin.X, (float)origin.Y);
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
		// var result = this;
		// result.Origin = result.xform(other.Origin);
		// result.X = new(result.tdotx(other.X), result.tdoty(other.X));
		// result.Y = new(result.tdotx(other.Y), result.tdoty(other.Y));
		// return result;
		var result = this;
		result.matrix = matrix * other.matrix;
		return result;
	}

	public Transform Scaled(Vector2 scale)
	{
		// X = new(X.X * scale.X, X.Y * scale.Y),
		// Y = new(Y.X * scale.X, Y.Y * scale.Y),
		// Origin = new Vector2(Origin.X, Origin.Y) * scale,
		var result = this;
		result.matrix = matrix * Matrix3x2.CreateScale((float)scale.X, (float)scale.Y);
		return result;
	}

	public Transform Rotated(double rotation)
	{
		// var rads = System.Math.IEEERemainder(rotation, double.Tau);
		// var cr = System.Math.Cos(rads);
		// var sr = System.Math.Sin(rads);
		// return this.Transformed(new()
		// {
		// 	X = new(cr, -sr),
		// 	Y = new(sr, cr),
		// 	Origin = Vector2.Zero,
		// });
		var result = this;
		result.matrix = matrix * Matrix3x2.CreateRotation((float)rotation);
		return result;
	}

	// ooooh
	private double tdotx(Vector2 v) => X.X * v.X + Y.X * v.Y;
	private double tdoty(Vector2 v) => X.Y * v.X + Y.Y * v.Y;
	private Vector2 xform(Vector2 v) => new Vector2(tdotx(v), tdoty(v)) + Origin;
}

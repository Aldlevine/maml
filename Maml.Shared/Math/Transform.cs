using System;
using System.Numerics;

namespace Maml.Math;

public partial struct Transform : IEquatable<Transform>
{
	public Vector2 X
	{
		get => new(matrix.M11, matrix.M12);
		set
		{
			matrix.M11 = (float)value.X;
			matrix.M12 = (float)value.Y;
		}
	}

	public Vector2 Y
	{
		get => new(matrix.M21, matrix.M22);
		set
		{
			matrix.M21 = (float)value.X;
			matrix.M22 = (float)value.Y;
		}
	}

	public Vector2 Origin
	{
		get => new(matrix.M31, matrix.M32);
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
			double det = matrix.GetDeterminant();
			double detSign = double.IsNaN(det) switch
			{
				true => 1,
				false => System.Math.Sign(matrix.GetDeterminant()),
			};
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

	public double Rotation
	{
		get => double.Atan2(X.Y, X.X);
		set
		{
			var scale = Scale;
			var cr = double.Cos(value);
			var sr = double.Sin(value);
			X = new(cr, sr);
			Y = new(-sr, cr);
			Scale = scale;
		}
	}


	private Matrix3x2 matrix = Matrix3x2.Identity;

	public Transform()
	{
		matrix = Matrix3x2.Identity;
	}

	public Transform(Vector2 x, Vector2 y, Vector2 origin)
	{
		matrix = new((float)x.X, (float)x.Y, (float)y.X, (float)y.Y, (float)origin.X, (float)origin.Y);
	}

	public static readonly Transform Identity = new(
		new(1, 0),
		new(0, 1),
		new(0, 0));

	public static readonly Transform PixelIdentity = new(
		new(1, 0),
		new(0, 1),
		new(0.5f, 0.5f));

	// public Transform Translated(Vector2 offset) => new Transform { Origin = offset } * this;//this with { Origin = Origin + offset, };
	public Transform Translated(Vector2 offset) => this with { Origin = Origin + offset, };

	public Transform Scaled(Vector2 scale) => new Transform { Scale = scale } * this;//this with { Scale = Scale * scale, };

	public Transform Rotated(double rotation) => new Transform { Rotation = rotation } * this;// this with { Rotation = Rotation + rotation, };

	public Transform Inverse()
	{
		if (!Matrix3x2.Invert(matrix, out var newMatrix))
		{
			throw new ArithmeticException();
		}
		return new Transform
		{
			matrix = newMatrix,
		};
	}

	public static Transform operator *(Transform lhs, Transform rhs)
	{
		return new()
		{
			// matrix = lhs.matrix * rhs.matrix,
			matrix = rhs.matrix * lhs.matrix,
		};
		//double x0 = lhs.tdotx(rhs.X);
		//double x1 = lhs.tdoty(rhs.X);
		//double y0 = lhs.tdotx(rhs.Y);
		//double y1 = lhs.tdoty(rhs.Y);

		//return new()
		//{
		//	X = new(x0, x1),
		//	Y = new(y0, y1),
		//	Origin = lhs * rhs.Origin,
		//};
	}

	public static Vector2 operator *(Transform lhs, Vector2 rhs) => new Vector2(lhs.tdotx(rhs), lhs.tdoty(rhs)) + lhs.Origin;

	public static Rect operator *(Transform lhs, Rect rhs)
	{
		Vector2 x = lhs.X * rhs.Size.X;
		Vector2 y = lhs.Y * rhs.Size.Y;
		Vector2 pos = lhs * rhs.Position;

		Rect rect = default;
		rect.Position = pos;
		rect = rect.ExpandedTo(pos + x);
		rect = rect.ExpandedTo(pos + y);
		rect = rect.ExpandedTo(pos + x + y);
		return rect;
	}

	public static bool operator ==(Transform left, Transform right) => left.Equals(right);
	public static bool operator !=(Transform left, Transform right) => !(left == right);
	public override bool Equals(object? obj) => obj is Transform transform && Equals(transform);
	public bool Equals(Transform other) => matrix.Equals(other.matrix);
	public override int GetHashCode() => HashCode.Combine(matrix);


	private double tdotx(in Vector2 v) => X.X * v.X + Y.X * v.Y;
	private double tdoty(in Vector2 v) => X.Y * v.X + Y.Y * v.Y;

}

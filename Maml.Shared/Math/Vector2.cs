using System;
using Windows.Devices.WiFiDirect;

namespace Maml.Math;

public partial struct Vector2 : IEquatable<Vector2>
{
	public double X { get; set; }
	public double Y { get; set; }

	public readonly static Vector2 Zero = new(0, 0);
	public readonly static Vector2 One = new(1, 1);
	public readonly static Vector2 Left = new(-1, 0);
	public readonly static Vector2 Right = new(1, 0);
	public readonly static Vector2 Up = new(0, -1);
	public readonly static Vector2 Down = new(0, 1);

	public Vector2(double x, double y)
	{
		X = x;
		Y = y;
	}

	public override bool Equals(object? obj) => obj is Vector2 vector && Equals(vector);
	public bool Equals(Vector2 other) => X == other.X && Y == other.Y;
	public override int GetHashCode() => HashCode.Combine(X, Y);

	public static bool operator ==(Vector2 left, Vector2 right) => left.Equals(right);
	public static bool operator !=(Vector2 left, Vector2 right) => !(left == right);

	public override string? ToString() => $"({X},{Y})";

	// Math
	public static Vector2 operator +(Vector2 lhs, Vector2 rhs) => new Vector2(lhs.X + rhs.X, lhs.Y + rhs.Y);
	public static Vector2 operator -(Vector2 lhs, Vector2 rhs) => new Vector2(lhs.X - rhs.X, lhs.Y - rhs.Y);
	public static Vector2 operator *(Vector2 lhs, Vector2 rhs) => new Vector2(lhs.X * rhs.X, lhs.Y * rhs.Y);
	public static Vector2 operator /(Vector2 lhs, Vector2 rhs) => new Vector2(lhs.X / rhs.X, lhs.Y / rhs.Y);

	public static Vector2 operator +(Vector2 lhs, double rhs) => new Vector2(lhs.X + rhs, lhs.Y + rhs);
	public static Vector2 operator -(Vector2 lhs, double rhs) => new Vector2(lhs.X - rhs, lhs.Y - rhs);
	public static Vector2 operator *(Vector2 lhs, double rhs) => new Vector2(lhs.X * rhs, lhs.Y * rhs);
	public static Vector2 operator /(Vector2 lhs, double rhs) => new Vector2(lhs.X / rhs, lhs.Y / rhs);

	public static Vector2 operator -(Vector2 v) => new Vector2(-v.X, -v.Y);

	public double LengthSq() => X * X + Y * Y;
	public double Length() => System.Math.Sqrt(LengthSq());
	public Vector2 Normalized()
	{
		var len = Length();
		return new(X / len, Y / len);
	}

	public static Vector2 Lerp(Vector2 from, Vector2 to, double t)
	{
		return new(Unit.Lerp(from.X, to.X, t), Unit.Lerp(from.Y, to.Y, t));
	}
}

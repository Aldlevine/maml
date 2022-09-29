using System;

namespace Maml.Math;

public partial struct Rect : IShape, IEquatable<Rect>
{
	public Vector2 Position { get; set; } = Vector2.Zero;
	public Vector2 Size { get; set; } = Vector2.Zero;

	public Vector2 End
	{
		get => Position + Size;
		set => Size = (value - Position);
	}

	public Rect() { }

	public Rect ExpandedTo(in Vector2 v)
	{
		Vector2 pos = Position;
		Vector2 end = End;

		if (v.X < pos.X) { pos.X = v.X; }
		if (v.Y < pos.Y) { pos.Y = v.Y; }
		if (v.X > end.X) { end.X = v.X; }
		if (v.Y > end.Y) { end.Y = v.Y; }
		return new()
		{
			Position = pos,
			End = end,
		};
	}

	public bool HasPoint(in Vector2 v)
	{
		var begin = Position;
		var end = End;
		if (v.X < begin.X) { return false; }
		if (v.X > end.X) { return false; }
		if (v.Y < begin.Y) { return false; }
		if (v.Y > end.Y) { return false; }
		return true;
	}

	public Rect GetBoundingRect(in Transform transform) => transform * this;

	public override string? ToString() => $"Rect({Position}, {Size})";
	public override bool Equals(object? obj) => obj is Rect rect && Equals(rect);
	public bool Equals(Rect other) => Position.Equals(other.Position) && Size.Equals(other.Size);
	public override int GetHashCode() => HashCode.Combine(Position, Size);

	public static bool operator ==(Rect left, Rect right) => left.Equals(right);
	public static bool operator !=(Rect left, Rect right) => !(left == right);
}


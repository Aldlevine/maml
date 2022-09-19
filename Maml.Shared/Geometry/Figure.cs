using System.Collections.Generic;

namespace Maml.Geometry;

public abstract record Figure
{
	public Vector2 Origin;

	public record Line : Figure
	{
		public Vector2 Dest;
	}

	public record BezierCurve : Line
	{
		public Vector2 ControlPointIn;
		public Vector2 ControlPointOut;
	}

	public record QuadraticCurve : Line
	{
		public Vector2 ControlPoint;
	}

	public record Arc : Figure
	{
		public double Radius;
		public double StartAngle;
		public double EndAngle;
	}

	public record ArcTo : Line
	{
		public double Radius;
	}

	public record Ellipse : Figure
	{
		public Vector2 Radii;
		public double Rotation;
		public double StartAngle;
		public double EndAngle;
	}

	public record Rect : Figure
	{
		public Vector2 Size;
	}
}

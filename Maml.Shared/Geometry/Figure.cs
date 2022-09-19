namespace Maml.Geometry;

public abstract partial record Figure
{
	public Vector2 Origin;

	public partial record Line : Figure
	{
		public Vector2 Dest;
	}

	public partial record BezierCurve : Line
	{
		public Vector2 ControlPointIn;
		public Vector2 ControlPointOut;
	}

	public partial record QuadraticCurve : Line
	{
		public Vector2 ControlPoint;
	}

	public partial record Arc : Figure
	{
		public double Radius;
		public double StartAngle;
		public double EndAngle;
	}

	public partial record ArcTo : Line
	{
		public double Radius;
	}

	public partial record Ellipse : Figure
	{
		public Vector2 Radii;
		public double Rotation;
		public double StartAngle;
		public double EndAngle;
	}

	public partial record Rect : Figure
	{
		public Vector2 Size;
	}
}

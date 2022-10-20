namespace Maml.Math;

public static class Unit
{
	public const double Epsilon = 0.001;

	public static double Lerp(double from, double to, double t)
	{
		if (t == 0) { return from; }
		if (t == 1) { return to; }
		return from + (t * (to - from));
	}

	public static bool ApproxEqual(double lhs, double rhs, double epsilon = Epsilon) => double.Abs(lhs - rhs) <= epsilon;

	public static double Triangle(double t) => (2 / double.Pi) * double.Asin(double.Sin(t));
}

namespace Maml.Math;

public static class Unit
{
	public static double Lerp(double from, double to, double t)
	{
		if (t == 0) { return from; }
		if (t == 1) { return to; }
		return from + (t * (to - from));
	}

	public static double Triangle(double t)
	{
		return (2 / double.Pi) * double.Asin(double.Sin(t));
	}
}

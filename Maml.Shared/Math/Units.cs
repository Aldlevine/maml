namespace Maml.Math;

public record struct Unit(double Value)
{
	// public abstract double AsDIP { get; }
	// public abstract double AsPP { get; }

	public static double Lerp(double from, double to, double t)
	{
		if (t == 0) { return from; }
		if (t == 1) { return to; }
		return from + (t * (to - from));
	}
}

// public record PP(double Value) : Unit
// {
// 	public override double AsDIP => Value * Program.App.Viewport.DpiRatio;
// 	public override double AsPP => Value;
// }
// 
// public record DIP(double Value) : Unit
// {
// 	public override double AsDIP => Value;
// 	public override double AsPP => Value / Program.App.Viewport.DpiRatio;
// }
// 
// public static class UnitsExtensions
// {
// 	public static DIP DIP(this double value) => new DIP(value);
// 	public static PP PP(this double value) => new PP(value);
// 
// 	public static DIP DIP(this float value) => new DIP(value);
// 	public static PP PP(this float value) => new PP(value);
// 
// 	public static DIP DIP(this int value) => new DIP(value);
// 	public static PP PP(this int value) => new PP(value);
// }


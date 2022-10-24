namespace Maml.Graphics;
public partial struct Color
{
	internal string ToCSSColor() => $"rgba({R * 255}, {G * 255}, {B * 255}, {A})";
}

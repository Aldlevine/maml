using Maml.Observable;

namespace Maml.Graphics;

public abstract partial class Brush : Resource { }

public partial class ColorBrush : Brush
{
	public static BasicProperty<ColorBrush, Color> ColorProperty { get; } = new(default)
	{
		Changed = (ColorBrush self) => self.IsDirty = true,
	};
	public Color Color
	{
		get => ColorProperty[this].Get();
		set => ColorProperty[this].Set(value);
	}

	public ColorBrush() : base() { }

	public ColorBrush(ColorBrush colorBrush)
	{
		Color = colorBrush.Color;
	}
}


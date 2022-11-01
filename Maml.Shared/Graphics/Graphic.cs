using Maml.Math;
using Maml.Observable;
using System;

namespace Maml.Graphics;

public abstract partial class Graphic : ObservableObject
{
	public abstract void Draw(RenderTargetBase renderTarget, Transform transform);
	public abstract Rect GetBoundingRect();
}

public partial class GeometryGraphic : Graphic
{
	public static BasicProperty<GeometryGraphic, Geometry?> GeometryProperty { get; } = new(null);
	public Geometry? Geometry
	{
		get => GeometryProperty[this].Get();
		set => GeometryProperty[this].Set(value);
	}

	public static BasicProperty<GeometryGraphic, DrawLayer[]> DrawLayersProperty { get; } = new(Array.Empty<DrawLayer>());
	public DrawLayer[] DrawLayers
	{
		get => DrawLayersProperty[this].Get();
		set => DrawLayersProperty[this].Set(value);
	}

	public GeometryGraphic() { }

	public GeometryGraphic(GeometryGraphic geometryGraphic)
	{
		Geometry = geometryGraphic.Geometry;
		DrawLayers = geometryGraphic.DrawLayers;
	}

	public override void Draw(RenderTargetBase rt, Transform transform)
	{
		if (Geometry == null) { return; }
		if (DrawLayers.Length == 0) { return; }

		rt.SetTransform(transform);

		foreach (var layer in DrawLayers)
		{
			rt.DrawGeometry(Geometry, layer);
		}
	}

	public override Rect GetBoundingRect()
	{
		if (Geometry == null)
		{
			return new();
		}

		var rect = Geometry.GetBoundingRect();
		var maxThickness = 0;
		foreach (var drawLayer in DrawLayers)
		{
			if (drawLayer is Stroke s)
			{
				maxThickness = int.Max(maxThickness, s.Thickness);
			}
		}
		rect = rect
			.ExpandedTo(rect.Position - (maxThickness + 1))
			.ExpandedTo(rect.End + (maxThickness + 1) * 2);
		return rect;
	}
}

public partial class TextGraphic : Graphic
{
	public TextGraphic()
	{
		Changed += (s, e) => measure();
	}

	unsafe private void measure()
	{
		if (Engine.Singleton.Window?.RenderTarget == null) { return; }
		Text?.GetResource(Engine.Singleton.Window.RenderTarget);
	}

	public static BasicProperty<TextGraphic, Text?> TextProperty { get; } = new(null);
	public Text? Text
	{
		get => TextProperty[this].Get();
		set => TextProperty[this].Set(value);
	}

	public static BasicProperty<TextGraphic, Brush?> BrushProperty { get; } = new(null);
	public Brush? Brush
	{
		get => BrushProperty[this].Get();
		set => BrushProperty[this].Set(value);
	}

	public override void Draw(RenderTargetBase rt, Transform transform)
	{
		if (Text == null) { return; }
		if (Brush == null) { return; }

		rt.SetTransform(transform);
		rt.PushClip(new Rect { Size = Text.Size, });
		rt.DrawText(Text, Brush);
		rt.PopClip();
	}

	public override Rect GetBoundingRect() => new() { Size = Text?.Size ?? Vector2.Zero, };
}

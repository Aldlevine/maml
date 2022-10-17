using Maml.Math;
using Maml.Observable;
using System.Collections.Generic;

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

	// TODO: Emit Changed
	public List<DrawLayer> DrawLayers { get; set; } = new();

	public GeometryGraphic() { }

	public GeometryGraphic(GeometryGraphic geometryGraphic)
	{
		Geometry = geometryGraphic.Geometry;
		DrawLayers = geometryGraphic.DrawLayers;
	}

	public override void Draw(RenderTargetBase rt, Transform transform)
	{
		if (Geometry == null) { return; }
		if (DrawLayers.Count == 0) { return; }

		rt.SetTransform(transform);

		foreach (var layer in DrawLayers)
		{
			rt.DrawGeometry(Geometry, layer);
		}
	}

	public override Rect GetBoundingRect() => Geometry.GetBoundingRect();
}

public partial class TextGraphic : Graphic
{
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

	public override Rect GetBoundingRect() => new Rect { Size = Text.Size, };
}

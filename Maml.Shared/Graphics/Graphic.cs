using Maml.Math;
using Maml.Observable;
using System.Collections.Generic;

namespace Maml.Graphics;

public abstract partial class Graphic : ObservableObject
{
	public abstract void Draw(RenderTargetBase renderTarget, Transform transform);

	// public abstract Rect GetBoundingRect(Transform transform);
}

public partial class GeometryGraphic : Graphic
{
	public static BasicProperty<GeometryGraphic, Geometry?> GeometryProperty = new(null);
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
}

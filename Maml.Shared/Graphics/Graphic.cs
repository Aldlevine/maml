using Maml.Math;
using System.Collections.Generic;

namespace Maml.Graphics;

public abstract partial class Graphic
{
	public abstract void Draw(ViewportBase viewport, Transform transform);

	// public abstract Rect GetBoundingRect(Transform transform);
}

public partial class GeometryGraphic : Graphic
{
	public Geometry? Geometry { get; set; }
	public List<DrawLayer> DrawLayers { get; set; } = new();

	public GeometryGraphic() { }

	public GeometryGraphic(GeometryGraphic geometryGraphic)
	{
		Geometry = geometryGraphic.Geometry;
		DrawLayers = geometryGraphic.DrawLayers;
	}
	
	public override void Draw(ViewportBase vp, Transform transform)
	{
		if (Geometry == null) { return; }

		Transform curXform = vp.GetTransform();
		vp.SetTransform(curXform * transform);

		foreach (var layer in DrawLayers)
		{
			vp.DrawGeometry(Geometry, layer);
		}
		vp.SetTransform(curXform);
	}
}

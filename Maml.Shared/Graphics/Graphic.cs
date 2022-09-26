using Maml.Math;
using System.Collections.Generic;

namespace Maml.Graphics;

public abstract partial class Graphic
{
	public abstract void Draw(ViewportBase viewport, Transform transform);
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
		// foreach (var layer in DrawLayers)
		// {
		// 	switch (layer)
		// 	{
		// 		case Fill l:
		// 			DrawLayer(vp, l);
		// 			break;
		// 		case Stroke l:
		// 			DrawLayer(vp, l);
		// 			break;
		// 	}
		// }
		vp.SetTransform(curXform);
	}
}

using Maml.Events;
using Maml.Math;
using System;
using System.Collections.Generic;

namespace Maml.Graphics;

public abstract partial class Graphic : IChanged
{
	public event EventHandler<ChangedEvent>? Changed;
	public void RaiseChanged(object? sender, ChangedEvent e) => Changed?.Invoke(sender, e);

	public abstract void Draw(RenderTargetBase renderTarget, Transform transform);

	// public abstract Rect GetBoundingRect(Transform transform);
}

public partial class GeometryGraphic : Graphic
{
	private Geometry? geometry;
	public Geometry? Geometry
	{
		get => geometry;
		set
		{
			if (geometry == value) { return; }
			if (geometry != null) { geometry.Changed -= RaiseChanged; }
			geometry = value;
			if (geometry != null) { geometry.Changed += RaiseChanged; }
		}
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

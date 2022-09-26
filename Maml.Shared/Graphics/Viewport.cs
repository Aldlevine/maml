using Maml.Events;
using Maml.Graphics;
using Maml.Math;
using Maml.Scene;
using System;
using System.Threading.Tasks;

namespace Maml.Graphics;

public abstract class ViewportBase
{
	public double DpiRatio => GetDpiRatio();
	public Vector2 Size => GetSize();

	public abstract event EventHandler<ResizeEvent>? Resize;
	public abstract event EventHandler<DrawEvent>? Draw;

	protected abstract Vector2 GetSize();
	protected abstract double GetDpiRatio();

	public abstract void Clear(Color color);
	public abstract Transform GetTransform();
	public abstract void SetTransform(Transform transform);

	// public void DrawGraphic(Graphic graphic, Transform transform) => graphic.Draw(this, transform);
	public void DrawScene(SceneTree sceneTree)
	{
		foreach (var node in sceneTree.Nodes)
		{
			foreach (var c in node.Graphics)
			{
				if (c is GraphicComponent g && g.Graphic != null)
				{
					// DrawGraphic(g.Graphic, node.GlobalTransform * g.Transform);
					g.Graphic.Draw(this, node.GlobalTransform * g.Transform);
				}
			}
		}
	}

	public void DrawGeometry(Geometry geometry, DrawLayer drawLayer)
	{
		switch (drawLayer)
		{
			case Fill l: DrawGeometry(geometry, l); break;
			case Stroke l: DrawGeometry(geometry, l); break;
		}
	}
	public abstract void DrawGeometry(Geometry geometry, Fill fill);
	public abstract void DrawGeometry(Geometry geometry, Stroke stroke);
}

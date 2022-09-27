using Maml.Events;
using Maml.Graphics;
using Maml.Math;
using Maml.Scene;
using System;
using System.Threading.Tasks;

namespace Maml.Graphics;

public abstract class ViewportBase
{
	public abstract event EventHandler<ResizeEvent>? Resize;
	public abstract event EventHandler<DrawEvent>? Draw;

	public abstract Vector2 Size { get; }
	public abstract double DpiRatio { get; }

	public abstract Transform GetTransform();
	public abstract void SetTransform(Transform transform);

	public abstract void Clear(Color color);
	public abstract void DrawGeometry(Geometry geometry, Fill fill);
	public abstract void DrawGeometry(Geometry geometry, Stroke stroke);

	public void DrawScene(SceneTree sceneTree)
	{
		foreach (var node in sceneTree.Nodes)
		{
			foreach (var c in node.Graphics)
			{
				if (c is GraphicComponent g && g.Graphic != null)
				{
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
}

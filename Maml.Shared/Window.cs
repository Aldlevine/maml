using Maml.Events;
using Maml.Graphics;
using Maml.Math;
using Maml.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maml;

public abstract class WindowBase<TRenderTarget> where TRenderTarget: RenderTargetBase
{
	public abstract TRenderTarget RenderTarget { get; init; }

	public abstract Vector2 Size { get; }
	public abstract double DpiRatio { get; }

	public abstract event EventHandler<ResizeEvent>? Resize;
	public abstract event EventHandler<PointerEvent>? PointerMove;
	public abstract event EventHandler<PointerEvent>? PointerDown;
	public abstract event EventHandler<PointerEvent>? PointerUp;
	public abstract event EventHandler<WheelEvent>? Wheel;
	public abstract event EventHandler<KeyEvent>? KeyDown;
	public abstract event EventHandler<KeyEvent>? KeyUp;
	public abstract event EventHandler<FocusEvent>? Focus;
	public abstract event EventHandler<FocusEvent>? Blur;
}

public abstract class RenderTargetBase
{
	public abstract event EventHandler<DrawEvent>? Draw;

	// Platform Specific
	public abstract Transform GetTransform();
	public abstract void SetTransform(Transform transform);

	public abstract void Clear(Color color);
	public abstract void DrawGeometry(Geometry geometry, Fill fill);
	public abstract void DrawGeometry(Geometry geometry, Stroke stroke);

	// Platform Agnostic
	public void DrawGeometry(Geometry geometry, DrawLayer drawLayer)
	{
		switch (drawLayer)
		{
			case Fill l:
				DrawGeometry(geometry, l);
				break;
			case Stroke l:
				DrawGeometry(geometry, l);
				break;
		}
	}

	public void DrawScene(SceneTree sceneTree)
	{
		foreach (var node in sceneTree.Nodes)
		foreach (var c in node.Graphics)
		{
			if (c is GraphicComponent g && g.Graphic != null)
			{
				// g.Graphic.Draw(this, node.GlobalTransform * g.Transform);
			}
		}
	}
}

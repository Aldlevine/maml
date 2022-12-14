using Maml.Graphics;
using Maml.Math;
using System.Collections.Generic;

namespace Maml;

public abstract class RenderTargetBase
{
	// Platform Specific
	public abstract void BeginDraw();
	public abstract void EndDraw();
	public abstract void SetTransform(Transform transform);

	public abstract void Clear(Color color);
	public abstract void ClearRect(Rect rect, Color color);
	public abstract void DrawGeometry(Geometry geometry, Fill fill);
	public abstract void DrawGeometry(Geometry geometry, Stroke stroke);
	public abstract void DrawText(Text text, Brush brush);

	public abstract void PushClip(Rect rect);
	public abstract void PopClip();
	public abstract void PushLayer(IList<Rect> rect);
	public abstract void PopLayer();

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
}

public partial class RenderTarget : RenderTargetBase { }

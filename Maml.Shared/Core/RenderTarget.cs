using Maml.Graphics;
using Maml.Math;
using Maml.Scene;

namespace Maml;

public abstract class RenderTargetBase
{
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
}

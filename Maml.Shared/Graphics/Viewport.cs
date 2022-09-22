using Maml.Events;
using Maml.Math;

namespace Maml.Graphics;

public partial class Viewport
{
	public static readonly Viewport Main = new();

	public double DpiRatio => GetDpiRatio();
	public Vector2 Size => GetSize();

	internal partial Vector2 GetSize();
	internal partial double GetDpiRatio();

	public event EventHandler<ResizeEvent>? Resize;
	public event EventHandler<DrawEvent>? Draw;

	public partial void Clear(Color color);
	public partial void DrawGraphic(Graphic graphic, Transform transform);
	public partial void SetTransform(Transform transform);

	// public partial void BeginDraw();
	// public partial void EndDraw();
	// public partial void Clear(Color color);

	// public partial void PushClip(Path path);
	// public partial void PopClip();

	// public partial void SetTransform(Transform transform);
	// public partial void FillPath(Path path);
	// public partial void StrokePath(Path path);
	// public partial void SetFillBrush(Brush brush);
	// public partial void SetStrokeBrush(Brush brush);
}

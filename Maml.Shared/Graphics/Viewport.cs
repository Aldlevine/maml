using Maml.Events;
using Maml.Math;

namespace Maml.Graphics;

public partial class Viewport
{
	public static readonly Viewport Main = new();

	public double Dpi => GetDpi();
	public Vector2 Size => GetSize();

	internal partial Vector2 GetSize();
	internal partial double GetDpi();

	public event EventHandler<ResizeEvent>? Resize;

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

using Maml.Events;
using Maml.Geometry;
using Maml.UserInput;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maml.Drawing;

public partial class Renderer
{
	public static Vector2 Size { get; private set; }
	public static event Events.EventHandler<ResizeEvent>? Resize;

	public static partial void BeginDraw();
	public static partial void EndDraw();
	public static partial void ClearRect(Figure.Rect rect);

	public static partial void PushClip(Path path);
	public static partial void PopClip();

	public static partial void SetTransform(Transform transform);

	public static partial void FillPath(Path path);
	public static partial void StrokePath(Path path);

	public static partial void SetFillBrush(Brush brush);
	public static partial void SetStrokeBrush(Brush brush);
}

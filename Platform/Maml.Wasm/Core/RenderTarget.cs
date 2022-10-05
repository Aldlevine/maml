using Maml.Graphics;
using Maml.Math;
using System.Runtime.InteropServices.JavaScript;

namespace Maml;
public partial class RenderTarget
{
	#region Abstract
	public override void Clear(Color color) => Clear(CanvasId, color.ToCSSColor());
	public override void DrawGeometry(Geometry geometry, Fill fill)
	{
		switch (geometry)
		{
			case RectGeometry g:
				FillRect(CanvasId, g.Rect.Position.X, g.Rect.Position.Y, g.Rect.Size.X, g.Rect.Size.Y, fill.Brush.GetResource(this));
				break;
			case EllipseGeometry g:
				FillEllipse(CanvasId, g.Ellipse.Center.X, g.Ellipse.Center.Y, g.Ellipse.Radius.X, g.Ellipse.Radius.Y, fill.Brush.GetResource(this));
				break;
			case LineGeometry:
				// We can't fill lines
				break;
		}
	}

	public override void DrawGeometry(Geometry geometry, Stroke stroke)
	{
		switch (geometry)
		{
			case RectGeometry g:
				StrokeRect(CanvasId, g.Rect.Position.X, g.Rect.Position.Y, g.Rect.Size.X, g.Rect.Size.Y, stroke.Brush.GetResource(this), stroke.Thickness);
				break;
			case EllipseGeometry g:
				StrokeEllipse(CanvasId, g.Ellipse.Center.X, g.Ellipse.Center.Y, g.Ellipse.Radius.X, g.Ellipse.Radius.Y, stroke.Brush.GetResource(this), stroke.Thickness);
				break;
			case LineGeometry g:
				StrokeLine(CanvasId, g.Line.Start.X, g.Line.Start.Y, g.Line.End.X, g.Line.End.Y, stroke.Brush.GetResource(this), stroke.Thickness);
				break;
		}
	}

	public override Transform GetTransform() => new(GetTransform(CanvasId));

	public override void SetTransform(Transform transform) => SetTransform(CanvasId, transform.ToDoubleArray());
	#endregion

	#region Internal
	internal int CanvasId = 0;

	[JSImport("clear", "render-target.js")]
	private static partial void Clear(int id, string color);

	[JSImport("fillRect", "render-target.js")]
	private static partial void FillRect(int id, double x, double y, double width, double height, int brushId);

	[JSImport("fillEllipse", "render-target.js")]
	private static partial void FillEllipse(int id, double x, double y, double radiusX, double radiusY, int brushId);

	[JSImport("strokeRect", "render-target.js")]
	// TODO: include stroke style
	private static partial void StrokeRect(int id, double x, double y, double width, double height, int brushId, double thickness);

	[JSImport("strokeEllipse", "render-target.js")]
	// TODO: include stroke style
	private static partial void StrokeEllipse(int id, double x, double y, double width, double height, int brushId, double thickness);

	[JSImport("strokeLine", "render-target.js")]
	// TODO: include stroke style
	private static partial void StrokeLine(int id, double startX, double startY, double endX, double endY, int brushId, double thickness);

	[JSImport("getTransform", "render-target.js")]
	private static partial double[] GetTransform(int id);

	[JSImport("setTransform", "render-target.js")]
	private static partial double[] SetTransform(int id, double[] matrix);

	[JSImport("releaseBrush", "render-target.js")]
	private static partial void ReleaseBrush(int id, int brushId);
	internal void ReleaseBrush(int brushId) => ReleaseBrush(CanvasId, brushId);

	[JSImport("makeColorBrush", "render-target.js")]
	private static partial int MakeColorBrush(int id, string color);
	internal int MakeColorBrush(Color color) => MakeColorBrush(CanvasId, color.ToCSSColor());
	#endregion
}

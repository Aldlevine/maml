using Maml.Graphics;
using Maml.Math;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;

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
				FillRect(g.Rect.Position.X, g.Rect.Position.Y, g.Rect.Size.X, g.Rect.Size.Y, fill.Brush.GetResource(this));
				break;
			case LineGeometry:
				// We can't fill lines
				break;
			default:
				FillGeometry(geometry.GetResource(this), fill.Brush.GetResource(this));
				break;
		}
	}

	public override void DrawGeometry(Geometry geometry, Stroke stroke)
	{
		switch (geometry)
		{
			case RectGeometry g:
				StrokeRect(g.Rect.Position.X, g.Rect.Position.Y, g.Rect.Size.X, g.Rect.Size.Y, stroke.Brush.GetResource(this), stroke.Thickness);
				break;
			default:
				StrokeGeometry(geometry.GetResource(this), stroke.Brush.GetResource(this), stroke.Thickness);
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
	private static partial void FillRect(int id, double x, double y, double w, double h, int brushId);
	private void FillRect(double x, double y, double w, double h, int brushId) => FillRect(CanvasId, x, y, w, h, brushId);

	[JSImport("fillEllipse", "render-target.js")]
	private static partial void FillEllipse(int id, double x, double y, double radiusX, double radiusY, int brushId);

	[JSImport("strokeRect", "render-target.js")]
	private static partial void StrokeRect(int id, double x, double y, double w, double h, int brushId, double thickness);
	private void StrokeRect(double x, double y, double w, double h, int brushId, double thickness) => StrokeRect(CanvasId, x, y, w, h, brushId, thickness);

	[JSImport("getTransform", "render-target.js")]
	private static partial double[] GetTransform(int id);

	[JSImport("setTransform", "render-target.js")]
	private static partial double[] SetTransform(int id, double[] matrix);


	[JSImport("fillGeometry", "render-target.js")]
	private static partial void FillGeometry(int id, int geometryId, int brushId);
	internal void FillGeometry(int geometryId, int brushId) => FillGeometry(CanvasId, geometryId, brushId);

	[JSImport("strokeGeometry", "render-target.js")]
	private static partial void StrokeGeometry(int id, int geometryId, int brushId, double thickness);
	internal void StrokeGeometry(int geometryId, int brushId, double thickness) => StrokeGeometry(CanvasId, geometryId, brushId, thickness);

	[JSImport("releaseGeometry", "render-target.js")]
	private static partial void ReleaseGeometry(int id, int geometryId);
	internal void ReleaseGeometry(int geometryId) => ReleaseGeometry(CanvasId, geometryId);

	[JSImport("makeRectGeometry", "render-target.js")]
	private static partial int MakeRectGeometry(int id, double x, double y, double w, double h);
	internal int MakeRectGeometry(Rect rect) => MakeRectGeometry(CanvasId, rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y);

	[JSImport("makeEllipseGeometry", "render-target.js")]
	private static partial int MakeEllipseGeometry(int id, double x, double y, double radiusX, double radiusY);
	internal int MakeEllipseGeometry(Ellipse ellipse) => MakeEllipseGeometry(CanvasId, ellipse.Center.X, ellipse.Center.Y, ellipse.Radius.X, ellipse.Radius.Y);

	[JSImport("makeLineGeometry", "render-target.js")]
	private static partial int MakeLineGeometry(int id, double startX, double startY, double endX, double endY);
	internal int MakeLineGeometry(Line line) => MakeLineGeometry(CanvasId, line.Start.X, line.Start.Y, line.End.X, line.End.Y);


	[JSImport("releaseBrush", "render-target.js")]
	private static partial void ReleaseBrush(int id, int brushId);
	internal void ReleaseBrush(int brushId) => ReleaseBrush(CanvasId, brushId);

	[JSImport("makeColorBrush", "render-target.js")]
	private static partial int MakeColorBrush(int id, string color);
	internal int MakeColorBrush(Color color) => MakeColorBrush(CanvasId, color.ToCSSColor());

	#endregion
}

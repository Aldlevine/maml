using Maml.Events;
using Maml.Geometry;
using System.Runtime.InteropServices.JavaScript;

namespace Maml.Drawing;

public partial class Renderer
{
	[JSExport]
	internal static void HandleResize(int width, int height)
	{
		Size = new(width, height);
		Resize?.Invoke(new ResizeEvent
		{
			Size = Size,
		});
	}

	[JSImport("beginDraw", "renderer.js")]
	private static partial void JSBeginDraw();
	public static partial void BeginDraw() => JSBeginDraw();

	[JSImport("endDraw", "renderer.js")]
	private static partial void JSEndDraw();
	public static partial void EndDraw() => JSEndDraw();

	[JSImport("clearRect", "renderer.js")]
	private static partial void JSClearRect(int x, int y, int width, int height);
	public static partial void ClearRect(Figure.Rect rect) => JSClearRect((int)rect.Origin.X, (int)rect.Origin.Y, (int)rect.Size.X, (int)rect.Size.Y);

	public static partial void PushClip(Path path) => throw new System.NotImplementedException(nameof(PushClip));

	public static partial void PopClip() => throw new System.NotImplementedException(nameof(PopClip));

	[JSImport("setTransform", "renderer.js")]
	private static partial void JSSetTransform(double xx, double xy, double yx, double yy, double tx, double ty);
	public static partial void SetTransform(Transform transform) => JSSetTransform(transform.X.X, transform.X.Y, transform.Y.X, transform.Y.Y, transform.Origin.X, transform.Origin.Y);

	[JSImport("setStrokeBrush", "renderer.js")]
	private static partial void JSSetStrokeBrush(int id);
	public static partial void SetStrokeBrush(Brush brush)
	{
		UploadBrush(brush);
		JSSetStrokeBrush(brush.GetHashCode());
	}

	[JSImport("setFillBrush", "renderer.js")]
	private static partial void JSSetFillBrush(int id);
	public static partial void SetFillBrush(Brush brush)
	{
		UploadBrush(brush);
		JSSetFillBrush(brush.GetHashCode());
	}

	[JSImport("strokePath", "renderer.js")]
	private static partial void JSStrokePath(int id);
	public static partial void StrokePath(Path path)
	{
		UploadPath(path);
		JSStrokePath(path.ID.GetHashCode());
	}

	[JSImport("fillPath", "renderer.js")]
	private static partial void JSFillPath(int id);
	public static partial void FillPath(Path path)
	{
		UploadPath(path);
		JSFillPath(path.ID.GetHashCode());
	}


	// Path Upload
	private static void UploadPath(Path path)
	{
		if (!path.Dirty) { return; }

		int id = path.ID.GetHashCode();
		Path_New(id);
		foreach (var figure in path.Figures)
		{
			switch (figure)
			{
				case Figure.Arc f:
					Path_Arc(id, f.Origin.X, f.Origin.Y, f.StartAngle, f.EndAngle);
					break;
				case Figure.ArcTo f:
					Path_ArcTo(id, f.Origin.X, f.Origin.Y, f.Dest.X, f.Dest.Y, f.Radius);
					break;
				case Figure.QuadraticCurve f:
					Path_QuadraticCurve(id, f.Origin.X, f.Origin.Y, f.ControlPoint.X, f.ControlPoint.Y, f.Dest.X, f.Dest.Y);
                    break;
				case Figure.BezierCurve f:
					Path_BezierCurve(id, f.Origin.X, f.Origin.Y, f.ControlPointIn.X, f.ControlPointIn.Y, f.ControlPointOut.X, f.ControlPointOut.Y, f.Dest.X, f.Dest.Y);
                    break;
				case Figure.Ellipse f:
					Path_Ellipse(id, f.Origin.X, f.Origin.Y, f.Radii.X, f.Radii.Y, f.Rotation, f.StartAngle, f.EndAngle);
                    break;
				case Figure.Line f:
					Path_Line(id, f.Origin.X, f.Origin.Y, f.Dest.X, f.Dest.Y);
                    break;
				case Figure.Rect f:
					Path_Rect(id, f.Origin.X, f.Origin.Y, f.Size.X, f.Size.Y);
					break;
			}
		}

		path.Dirty = false;
	}

	[JSImport("path_new", "renderer.js")]
	private static partial void Path_New(int id);

	[JSImport("path_arc", "renderer.js")]
	private static partial void Path_Arc(int id, double x, double y, double startAngle, double endAngle);

	[JSImport("path_arcTo", "renderer.js")]
	private static partial void Path_ArcTo(int id, double x1, double y1, double x2, double y2, double radius);

	[JSImport("path_quadraticCurve", "renderer.js")]
	private static partial void Path_QuadraticCurve(int id, double x1, double y1, double cpx, double cpy, double x2, double y2);

	[JSImport("path_bezierCurve", "renderer.js")]
	private static partial void Path_BezierCurve(int id, double x1, double y1, double cpx1, double cpy1, double cpx2, double cpy2, double x2, double y2);

	[JSImport("path_ellipse", "renderer.js")]
	private static partial void Path_Ellipse(int id, double x, double y, double rx, double ry, double rotation, double startAngle, double endAngle);

	[JSImport("path_line", "renderer.js")]
	private static partial void Path_Line(int id, double x1, double y1, double x2, double y2);

	[JSImport("path_rect", "renderer.js")]
	private static partial void Path_Rect(int id, double x, double y, double w, double h);

	// Brush upload
	private static void UploadBrush(Brush brush)
	{
		switch (brush)
		{
			case ColorBrush b:
				Brush_Color_New(b.GetHashCode(), b.Color.ToUint());
				break;
			case LinearGradientBrush b:
				Brush_LinearGradient_New(b.GetHashCode(), b.Start.X, b.Start.Y, b.End.X, b.End.Y);
				break;
			// case RadialGradientBrush b:
			// 	Brush_RadialGradient_New(b.GetHashCode(), b.Origin.X, b.Origin.Y, b.Offset.X, b.Offset.Y, b.Radii.X, b.Radii.Y);
			// 	break;
		}

		{
			if (brush is GradientBrush b)
			{
				foreach (var stop in b.ColorStops)
				{
					Brush_AddColorStop(b.GetHashCode(), stop.Key, stop.Value.Color.ToUint());
				}
			}
		}
	}

	[JSImport("brush_color_new", "renderer.js")]
	private static partial void Brush_Color_New(int id, double color);

	[JSImport("brush_linearGradient_new", "renderer.js")]
	private static partial void Brush_LinearGradient_New(int id, double x1, double y1, double x2, double y2);

	// [JSImport("brush_radialGradient_new", "renderer.js")]
	// private static partial void Brush_RadialGradient_New(int id, float x1, float y1, float r1, float x2, float y2, float r2);

	[JSImport("brush_addColorStop", "renderer.js")]
	private static partial void Brush_AddColorStop(int id, double offset, double color);
}

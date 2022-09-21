﻿using Maml.Events;
using Maml.Math;
using System.Runtime.InteropServices.JavaScript;

namespace Maml.Graphics;

public partial class Viewport
{
	[JSExport]
	internal static void HandleResize(int width, int height)
	{
		Main.Size = new(width, height);
		Main.Resize?.Invoke(new ResizeEvent
		{
			Size = Main.Size,
		});
	}

	[JSImport("beginDraw", "viewport.js")]
	private static partial void JSBeginDraw();
	public partial void BeginDraw() => JSBeginDraw();

	[JSImport("endDraw", "viewport.js")]
	private static partial void JSEndDraw();
	public partial void EndDraw() => JSEndDraw();

	[JSImport("clearRect", "viewport.js")]
	private static partial void JSClearRect(int x, int y, int width, int height);
	public partial void ClearRect(Figure.Rect rect) => JSClearRect((int)rect.Origin.X, (int)rect.Origin.Y, (int)rect.Size.X, (int)rect.Size.Y);

	public partial void PushClip(Path path) => throw new System.NotImplementedException(nameof(PushClip));

	public partial void PopClip() => throw new System.NotImplementedException(nameof(PopClip));

	[JSImport("setTransform", "viewport.js")]
	private static partial void JSSetTransform(double xx, double xy, double yx, double yy, double tx, double ty);
	public partial void SetTransform(Transform transform) => JSSetTransform(transform.X.X, transform.X.Y, transform.Y.X, transform.Y.Y, transform.Origin.X, transform.Origin.Y);

	[JSImport("setStrokeBrush", "viewport.js")]
	private static partial void JSSetStrokeBrush(int id);
	public partial void SetStrokeBrush(Brush brush)
	{
		UploadBrush(brush);
		JSSetStrokeBrush(brush.GetHashCode());
	}

	[JSImport("setFillBrush", "viewport.js")]
	private static partial void JSSetFillBrush(int id);
	public partial void SetFillBrush(Brush brush)
	{
		UploadBrush(brush);
		JSSetFillBrush(brush.GetHashCode());
	}

	[JSImport("strokePath", "viewport.js")]
	private static partial void JSStrokePath(int id);
	public partial void StrokePath(Path path)
	{
		UploadPath(path);
		JSStrokePath(path.ID.GetHashCode());
	}

	[JSImport("fillPath", "viewport.js")]
	private static partial void JSFillPath(int id);
	public partial void FillPath(Path path)
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

	[JSImport("path_new", "viewport.js")]
	private static partial void Path_New(int id);

	[JSImport("path_arc", "viewport.js")]
	private static partial void Path_Arc(int id, double x, double y, double startAngle, double endAngle);

	[JSImport("path_arcTo", "viewport.js")]
	private static partial void Path_ArcTo(int id, double x1, double y1, double x2, double y2, double radius);

	[JSImport("path_quadraticCurve", "viewport.js")]
	private static partial void Path_QuadraticCurve(int id, double x1, double y1, double cpx, double cpy, double x2, double y2);

	[JSImport("path_bezierCurve", "viewport.js")]
	private static partial void Path_BezierCurve(int id, double x1, double y1, double cpx1, double cpy1, double cpx2, double cpy2, double x2, double y2);

	[JSImport("path_ellipse", "viewport.js")]
	private static partial void Path_Ellipse(int id, double x, double y, double rx, double ry, double rotation, double startAngle, double endAngle);

	[JSImport("path_line", "viewport.js")]
	private static partial void Path_Line(int id, double x1, double y1, double x2, double y2);

	[JSImport("path_rect", "viewport.js")]
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

	[JSImport("brush_color_new", "viewport.js")]
	private static partial void Brush_Color_New(int id, double color);

	[JSImport("brush_linearGradient_new", "viewport.js")]
	private static partial void Brush_LinearGradient_New(int id, double x1, double y1, double x2, double y2);

	// [JSImport("brush_radialGradient_new", "viewport.js")]
	// private static partial void Brush_RadialGradient_New(int id, float x1, float y1, float r1, float x2, float y2, float r2);

	[JSImport("brush_addColorStop", "viewport.js")]
	private static partial void Brush_AddColorStop(int id, double offset, double color);
}
using Maml.Graphics;
using Maml.Math;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;

namespace Maml;
public partial class RenderTarget
{
	#region Abstract
	public override void Clear(Color color) => InternalClear(color);
	public override void ClearRect(Rect rect, Color color) => InternalClearRect(rect, color);
	public override void SetTransform(Transform transform) => InternalSetTransform(transform);
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

	public override void DrawText(Text text, Brush brush)
	{
		FillText(text.GetResource(this), brush.GetResource(this));
	}

	public override void PushClip(Rect rect) => InternalPushClip(rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y);

	public override void PopClip() => InternalPopClip();

	public override void PushLayer(IList<Rect> rects) => InternalPushLayer(rects);

	public override void PopLayer() => InternalPopLayer();

	#endregion

	#region Internal
	internal int CanvasId = 0;

	// Draw Commands
	private enum WasmDrawCommand
	{
		Clear,
		ClearRect,
		SetTransform,
		FillRect,
		StrokeRect,
		FillGeometry,
		StrokeGeometry,
		FillText,
		PushClip,
		PopClip,
		PushLayer,
		PopLayer,
	}
	internal List<double> DrawCommandBuffer { get; } = new(10_000);

	public override void BeginDraw()
	{
		DrawCommandBuffer.Clear();
	}

	public override void EndDraw()
	{
		ProcessDrawCommands(DrawCommandBuffer.ToArray());
	}

	private void InternalClear(Color color)
	{
		DrawCommandBuffer.AddRange(new double[]
		{
			(double)WasmDrawCommand.Clear,
			color.R, color.G, color.B, color.A,
		});
	}

	private void InternalClearRect(Rect rect, Color color)
	{
		DrawCommandBuffer.AddRange(new double[]
		{
			(double)WasmDrawCommand.ClearRect,
			rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y,
			color.R, color.G, color.B, color.A,
		});
	}

	private void FillRect(double x, double y, double w, double h, int brushId)
	{
		DrawCommandBuffer.AddRange(new double[] {
			(double)WasmDrawCommand.FillRect,
			x, y, w, h, brushId,
		});
	}

	private void StrokeRect(double x, double y, double w, double h, int brushId, double thickness)
	{
		DrawCommandBuffer.AddRange(new double[] {
			(double)WasmDrawCommand.StrokeRect,
			x, y, w, h, brushId, thickness,
		});
	}

	private void InternalSetTransform(Transform transform)
	{
		DrawCommandBuffer.Add((double)WasmDrawCommand.SetTransform);
		DrawCommandBuffer.AddRange(transform.ToDoubleArray());
	}

	private void FillGeometry(int geometryId, int brushId)
	{
		DrawCommandBuffer.AddRange(new double[]
		{
			(double)WasmDrawCommand.FillGeometry,
			geometryId, brushId,
		});
	}

	private void StrokeGeometry(int geometryId, int brushId, double thickness)
	{
		DrawCommandBuffer.AddRange(new double[]
		{
			(double)WasmDrawCommand.StrokeGeometry,
			geometryId, brushId, thickness,
		});
	}

	private void FillText(int textId, int brushId)
	{
		DrawCommandBuffer.AddRange(new double[]
		{
			(double)WasmDrawCommand.FillText,
			textId, brushId,
		});
	}

	private void InternalPushClip(double x, double y, double w, double h)
	{
		DrawCommandBuffer.AddRange(new double[]
		{
			(double)WasmDrawCommand.PushClip,
			x, y, w, h,
		});
	}

	private void InternalPopClip()
	{
		DrawCommandBuffer.AddRange(new double[]
		{
			(double)WasmDrawCommand.PopClip,
		});
	}

	private void InternalPushLayer(IList<Rect> rects)
	{
		DrawCommandBuffer.Add((double)WasmDrawCommand.PushLayer);
		DrawCommandBuffer.Add(rects.Count);
		foreach (var rect in rects)
		{
			DrawCommandBuffer.Add(rect.Position.X);
			DrawCommandBuffer.Add(rect.Position.Y);
			DrawCommandBuffer.Add(rect.Size.X);
			DrawCommandBuffer.Add(rect.Size.Y);
		}
	}

	private void InternalPopLayer()
	{
		DrawCommandBuffer.AddRange(new double[]
		{
			(double)WasmDrawCommand.PopLayer,
		});
	}

	[JSImport("processDrawCommands", "render-target.js")]
	private static partial void ProcessDrawCommands(int canvasId, [JSMarshalAs<JSType.Array<JSType.Number>>] double[] commandBuffer);
	internal void ProcessDrawCommands(double[] commandBuffer) => ProcessDrawCommands(CanvasId, commandBuffer);

	// Resource commands

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


	[JSImport("releaseText", "render-target.js")]
	private static partial void ReleaseText(int id, int textId);
	internal void ReleaseText(int textId) => ReleaseText(CanvasId, textId);

	[JSImport("makeText", "render-target.js")]
	private static partial double[] MakeText(
		int id,
		string text,
		int wrappingMode,
		double lineHeight,
		string fontName,
		double fontSize,
		int fontStyle,
		int fontWeight,
		double maxSizeX,
		double maxSizeY);
	internal (int id, uint lineCount, Vector2 size) MakeText(Text text)
	{
		double lineHeight = text.LineHeight switch
		{
			LineHeight.Relative => text.LineHeight.Value * text.Font.Size,
			_ => text.LineHeight.Value
		};
		var data = MakeText(
			CanvasId,
			text.String,
			(int)text.WrappingMode,
			lineHeight,
			text.Font.Name,
			text.Font.Size,
			(int)text.Font.Style,
			(int)text.Font.Weight,
			text.MaxSize.X,
			text.MaxSize.Y);
		return ((int)data[0], (uint)data[1], new Vector2(data[2], data[3]));
	}

	#endregion
}

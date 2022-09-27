using Maml.Events;
using Maml.Graphics;
using Maml.Math;
using Maml.Scene;
using System.Collections.Generic;

namespace Maml;

public class LineGrid : Node
{
	public LineGrid()
	{
		Engine.Singleton.Window.Resize += HandleResize;
		UpdateSize(Engine.Singleton.Window.Size);
	}

	private static List<DrawLayer> lineDrawLayersMinor = new()
	{
		new Stroke(new ColorBrush { Color = new Color(0x666666ff) with { A = 0.25f } }, 1),
	};

	private static List<DrawLayer> lineDrawLayersMajor = new()
	{
		new Stroke(new ColorBrush { Color = new Color(0x666666ff) with { A = 0.5f } }, 1),
	};

	private static LineGeometry lineGeoX = new()
	{
		Line = new() { Start = new(0, 0), End = new(0, 0) },
	};
	private static LineGeometry lineGeoY = new()
	{
		Line = new() { Start = new(0, 0), End = new(0, 0) },
	};

	private static GeometryGraphic lineGfxMinorX = new()
	{
		Geometry = lineGeoX,
		DrawLayers = lineDrawLayersMinor,
	};

	private static GeometryGraphic lineGfxMinorY = new()
	{
		Geometry = lineGeoY,
		DrawLayers = lineDrawLayersMinor,
	};

	private static GeometryGraphic lineGfxMajorX = new()
	{
		Geometry = lineGeoX,
		DrawLayers = lineDrawLayersMajor,
	};

	private static GeometryGraphic lineGfxMajorY = new()
	{
		Geometry = lineGeoY,
		DrawLayers = lineDrawLayersMajor,
	};

	private void UpdateSize(Vector2 size)
	{
		Graphics.RemoveRange(0, Graphics.Count);

		lineGeoX.Line = new Line { Start = new(0, 0), End = new(0, size.Y), };
		for (int x = 0; x < size.X; x += 20)
		{
			var lineGfx = (x % 100) switch
			{
				0 => lineGfxMajorX,
				_ => lineGfxMinorX,
			};

			Graphics.Add(new GraphicComponent
			{
				Graphic = lineGfx,
				Transform = Transform.Identity.Translated(new(x, 0))
			});
		}

		lineGeoY.Line = new Line { Start = new(0, 0), End = new(size.X, 0), };
		for (int y = 0; y < size.Y; y += 20)
		{
			var lineGfx = (y % 100) switch
			{
				0 => lineGfxMajorY,
				_ => lineGfxMinorY,
			};

			Graphics.Add(new GraphicComponent
			{
				Graphic = lineGfx,
				Transform = Transform.Identity.Translated(new(0, y))
			});
		}
	}

	private void HandleResize(object? sender, ResizeEvent evt)
	{
		UpdateSize(evt.Size);
	}
}

using Maml.Animation;
using Maml.Events;
using Maml.Graphics;
using Maml.Math;
using Maml.Scene;
using System;
using System.Collections.Generic;
using Windows.Graphics.Holographic;

namespace Maml;

public class LineGrid : Node
{
	#region Configuration
	public Vector2 MinorSpacing { get; set; } = new(20, 20);
	public Vector2 MajorInterval { get; set; } = new(5, 5);
	public Vector2 Size { get; set; }
	#endregion

	#region Resources
	public List<DrawLayer> LineDrawLayersMinor = lineDrawLayersMinor;
	public List<DrawLayer> LineDrawLayersMajor = lineDrawLayersMajor;

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

	private GeometryGraphic lineGfxMinorX = new()
	{
		Geometry = lineGeoX,
		DrawLayers = lineDrawLayersMinor,
	};

	private GeometryGraphic lineGfxMinorY = new()
	{
		Geometry = lineGeoY,
		DrawLayers = lineDrawLayersMinor,
	};

	private GeometryGraphic lineGfxMajorX = new()
	{
		Geometry = lineGeoX,
		DrawLayers = lineDrawLayersMajor,
	};

	private GeometryGraphic lineGfxMajorY = new()
	{
		Geometry = lineGeoY,
		DrawLayers = lineDrawLayersMajor,
	};
	#endregion

	public LineGrid()
	{
		// We are going to remove these in favor of triggering updates based on property changes!
		Window.Resize += (s, e) =>
		{
			Size = e.Size;
			Update();
		};

		// We are going to remove these in favor of triggering updates based on property changes!
		Animator.NextFrame += (s, e) =>
		{
			Size = Window.Size;
			Update();
		};
	}

	private void Update()
	{
		lineGfxMinorX.DrawLayers = LineDrawLayersMinor;
		lineGfxMinorY.DrawLayers = LineDrawLayersMinor;
		lineGfxMajorX.DrawLayers = LineDrawLayersMajor;
		lineGfxMajorY.DrawLayers = LineDrawLayersMajor;

		Graphics.RemoveRange(0, Graphics.Count);

		lineGeoX.Line = new Line { Start = new(0, 0), End = new(0, Size.Y), };
		for (int x = 0; x < Size.X; x += (int)MinorSpacing.X)
		{
			var lineGfx = (x % (MinorSpacing.X * MajorInterval.X)) switch
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

		lineGeoY.Line = new Line { Start = new(0, 0), End = new(Size.X, 0), };
		for (int y = 0; y < Size.Y; y += (int)MinorSpacing.Y)
		{
			var lineGfx = (y % (MinorSpacing.Y * MajorInterval.Y)) switch
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
}

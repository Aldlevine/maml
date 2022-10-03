﻿using Maml.Animation;
using Maml.Graphics;
using Maml.Math;
using Maml.Observable;
using Maml.Scene;
using System;
using System.Collections.Generic;

namespace Maml;

public class LineGrid : Node
{
	#region Resources
	private readonly static List<DrawLayer> lineDrawLayersMinor = new()
	{
		new Stroke(new ColorBrush { Color = new Color(0x666666ff) with { A = 0.25f } }, 1),
	};

	private readonly static List<DrawLayer> lineDrawLayersMajor = new()
	{
		new Stroke(new ColorBrush { Color = new Color(0x666666ff) with { A = 0.5f } }, 1),
	};

	private LineGeometry lineGeoX = new()
	{
		Line = new() { Start = new(0, 0), End = new(0, 0) },
	};
	private LineGeometry lineGeoY = new()
	{
		Line = new() { Start = new(0, 0), End = new(0, 0) },
	};

	private GeometryGraphic lineGfxMinorX = new()
	{
		// Geometry = lineGeoX,
		DrawLayers = lineDrawLayersMinor,
	};

	private GeometryGraphic lineGfxMinorY = new()
	{
		// Geometry = lineGeoY,
		DrawLayers = lineDrawLayersMinor,
	};

	private GeometryGraphic lineGfxMajorX = new()
	{
		// Geometry = lineGeoX,
		DrawLayers = lineDrawLayersMajor,
	};

	private GeometryGraphic lineGfxMajorY = new()
	{
		// Geometry = lineGeoY,
		DrawLayers = lineDrawLayersMajor,
	};
	#endregion

	#region Configuration
	public static BasicProperty<LineGrid, Vector2> MinorSpacingProperty = new(new(20, 20));
	public Vector2 MinorSpacing
	{
		get => MinorSpacingProperty[this].Get();
		set => MinorSpacingProperty[this].Set(value);
	}

	public static BasicProperty<LineGrid, Vector2> MajorIntervalProperty = new(new(5, 5));
	public Vector2 MajorInterval
	{
		get => MajorIntervalProperty[this].Get();
		set => MajorIntervalProperty[this].Set(value);
	}

	public static BasicProperty<LineGrid, Vector2> SizeProperty = new(new(100, 100));
	public Vector2 Size
	{
		get => SizeProperty[this].Get();
		set => SizeProperty[this].Set(value);
	}

	public static BasicProperty<LineGrid, List<DrawLayer>> LineDrawLayersMinorProperty = new(lineDrawLayersMinor);
	public List<DrawLayer> LineDrawLayersMinor
	{
		get => LineDrawLayersMinorProperty[this].Get();
		set => LineDrawLayersMinorProperty[this].Set(value);
	}

	public static BasicProperty<LineGrid, List<DrawLayer>> LineDrawLayersMajorProperty = new(lineDrawLayersMajor);
	public List<DrawLayer> LineDrawLayersMajor
	{
		get => LineDrawLayersMajorProperty[this].Get();
		set => LineDrawLayersMajorProperty[this].Set(value);
	}

	// public Transform MyTransform { get; set; } = Transform.Identity;
	#endregion

	public LineGrid()
	{
		// Because we can't assign from instance members in the initializer
		lineGfxMinorX.Geometry = lineGeoX;
		lineGfxMajorX.Geometry = lineGeoX;
		lineGfxMinorY.Geometry = lineGeoY;
		lineGfxMajorY.Geometry = lineGeoY;

		// TODO: we will move this to the property initializer
		LineDrawLayersMinorProperty[this].Changed += (s, v) => Engine.QueueDeferred(UpdateDrawLayers);
		LineDrawLayersMajorProperty[this].Changed += (s, v) => Engine.QueueDeferred(UpdateDrawLayers);
		Engine.QueueDeferred(UpdateDrawLayers);

		// TODO: we will move this to the property initializer
		SizeProperty[this].Changed += (s, v) => Engine.QueueDeferred(UpdateGrid);
		MinorSpacingProperty[this].Changed += (s, v) => Engine.QueueDeferred(UpdateGrid);
		MajorIntervalProperty[this].Changed += (s, v) => Engine.QueueDeferred(UpdateGrid);
		Engine.QueueDeferred(UpdateGrid);
	}

	private void UpdateDrawLayers()
	{
		lineGfxMinorX.DrawLayers = LineDrawLayersMinor;
		lineGfxMinorY.DrawLayers = LineDrawLayersMinor;
		lineGfxMajorX.DrawLayers = LineDrawLayersMajor;
		lineGfxMajorY.DrawLayers = LineDrawLayersMajor;
	}

	private void UpdateGrid()
	{
		foreach (var child in Children)
		{
			child.Dispose();
		}
		Children.Clear();

		lineGeoX.Line = new Line { Start = new(0, 0), End = new(0, Size.Y), };
		for (int x = 0; x < Size.X; x += (int)MinorSpacing.X)
		{
			var lineGfx = (x % (MinorSpacing.X * MajorInterval.X)) switch
			{
				0 => lineGfxMajorX,
				_ => lineGfxMinorX,
			};

			Children.Add(new GraphicNode
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

			Children.Add(new GraphicNode
			{
				Graphic = lineGfx,
				Transform = Transform.Identity.Translated(new(0, y))
			});
		}
	}
}

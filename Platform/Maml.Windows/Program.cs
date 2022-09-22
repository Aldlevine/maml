﻿using Maml.Events;
using Maml.Graphics;
using Maml.Math;
using Maml.Scene;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using static Windows.Win32.PInvoke;

[assembly: MetadataUpdateHandler(typeof(Maml.Program))]

namespace Maml;

internal static class Program
{
	public static App? App;

	// Resources
	private static List<DrawLayer> pointerDrawLayers = new()
	{
		new Fill(new ColorBrush { Color = Colors.RebeccaPurple with { A = 0.25f } }),
		new Stroke(new ColorBrush { Color = Colors.Cyan }, 7),
		new Stroke(new ColorBrush { Color = Colors.Green }, 3),
	};

	private static List<DrawLayer> lineDrawLayersMinor = new()
	{
		new Stroke(new ColorBrush { Color = Colors.DarkSlateBlue with { A = 0.125f } }, 3),
		new Stroke(new ColorBrush { Color = new Color(0x666666ff) with { A = 0.25f } }, 1),
	};

	private static List<DrawLayer> lineDrawLayersMajor = new()
	{
		new Stroke(new ColorBrush { Color = Colors.DarkSlateBlue with { A = 0.25f } }, 5),
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

	private static Node pointerNode = new Node
	{
		Name = "PointerNode",
		Graphics = new() {
			new GeometryGraphic
			{
				Geometry = new RectGeometry { Rect = new() { Position = new(-75, -75), Size = new(50, 50) }, },
				DrawLayers = pointerDrawLayers,
			},
			new GeometryGraphic
			{
				Geometry = new EllipseGeometry { Ellipse = new() { Center = new(50, 50), Radius = new(25, 25) }, },
				DrawLayers = pointerDrawLayers,
			},
			new GeometryGraphic
			{
				Geometry = new LineGeometry { Line = new() { Start = new(-25, -25), End = new(31.5, 31.5) }, },
				DrawLayers = pointerDrawLayers,
			}
		},
	};

	private static Node gridNode = new Node
	{
		Name = "GridNode",
		Graphics = new() { }
	};

	private static SceneTree sceneTree = new SceneTree
	{
		Root = new Node
		{
			Name = "Root",
			Children = new() { gridNode, pointerNode, }
		}
	};

	private static int Main(string[] args)
	{
		// Because we only care about pointer events
		EnableMouseInPointer(true);

		// Attach our console window if requested
		if (args.Length > 0)
		{
			bool consoleMode = Boolean.Parse(args[0]);
			if (consoleMode)
			{
				if (!AttachConsole(unchecked((uint)-1)))
				{
					AllocConsole();
				}
			}
		}

		App = new();

		// hookup events
		App.Viewport.Draw += Draw;
		App.Animator.Frame += Frame;
		App.Viewport.Resize += Resize;
		Input.PointerMove += PointerMove;

		App.RunMessageLoop();

		return 0;
	}

	// This code keeps the grid up to date with the window size
	private static void Resize(ResizeEvent evt)
	{
		gridNode.Graphics.RemoveRange(0, gridNode.Graphics.Count);

		lineGeoX.Line = new Line { Start = new(0, 0), End = new(0, evt.Size.Y), };
		for (int x = 0; x < evt.Size.X; x += 20)
		{
			var lineGfx = (x % 100) switch
			{
				0 => lineGfxMajorX,
				_ => lineGfxMinorX,
			};
			gridNode.Graphics.Add(new(lineGfx) { Transform = Transform.Identity.Translated(new(x, 0)) });
		}

		lineGeoY.Line = new Line { Start = new(0, 0), End = new(evt.Size.X, 0), };
		for (int y = 0; y < evt.Size.Y; y += 20)
		{
			var lineGfx = (y % 100) switch
			{
				0 => lineGfxMajorY,
				_ => lineGfxMinorY,
			};
			gridNode.Graphics.Add(new(lineGfx) { Transform = Transform.Identity.Translated(new(0, y)) });
		}
	}

	// This code animates the pointer node transform
	private static void Frame(FrameEvent evt)
	{
		pointerNode.Transform = Transform.Identity.Rotated(evt.Delta.TotalSeconds).Transformed(pointerNode.Transform);
	}

	// This code moves the pointer node to the pointer position
	private static void PointerMove(PointerEvent evt)
	{
		pointerNode.Transform = pointerNode.Transform with { Origin = evt.Position };
	}

	// This code draws the scene tree to the viewport
	private static void Draw(DrawEvent evt)
	{
		var vp = evt.Viewport;

		if (vp == null)
		{
			return;
		}

		vp.Clear(new Color(0x333333ff));
		vp.SetTransform(Transform.PixelIdentity);

		foreach (var node in sceneTree.Nodes)
		{
			foreach (var n in node.Graphics)
			{
				var g = n.Graphic;
				var gxform = n.Transform;
				gxform = node.Transform.Transformed(gxform);
				vp.DrawGraphic(n.Graphic, gxform);
			}
		}
	}
}

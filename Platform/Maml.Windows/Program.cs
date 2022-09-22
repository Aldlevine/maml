using Maml.Animation;
using Maml.Events;
using Maml.Graphics;
using Maml.Math;
using Maml.UserInput;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using static Windows.Win32.PInvoke;

[assembly: MetadataUpdateHandler(typeof(Maml.Program))]

namespace Maml;

internal static class Program
{
	public static App? App;

	private static Vector2 pointerPosition;

	private static int Main(string[] args)
	{
		EnableMouseInPointer(true);

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
		App.Viewport.Draw += Draw;
		App.Animator.Frame += Frame;
		Input.PointerMove += (e) => pointerPosition = e.Position;
		App.RunMessageLoop();

		return 0;
	}

	// Custom drawing stuff

	private static GeometryGraphic ellipseGfx = new()
	{
		Transform = Transform.PixelIdentity,
		Geometry = new EllipseGeometry()
		{
			Ellipse = new() { Center = new(50, 50), Radius = new(25, 25) },
		},
		DrawLayers = new()
		{
			new Fill(new ColorBrush { Color = Colors.RebeccaPurple with { A = 0.25f } }),
			new Stroke(new ColorBrush { Color = Colors.Cyan }, 7),
			new Stroke(new ColorBrush { Color = Colors.Green }, 3),
		},
	};

	private static GeometryGraphic rectGfx = new()
	{
		Transform = Transform.PixelIdentity,
		Geometry = new RectGeometry
		{
			Rect = new() { Position = new(-75, -75), Size = new(50, 50) },
		},
		DrawLayers = new()
		{
			new Fill(new ColorBrush { Color = Colors.RebeccaPurple with { A = 0.25f } }),
			new Stroke(new ColorBrush { Color = Colors.Cyan }, 7),
			new Stroke(new ColorBrush { Color = Colors.Green }, 3),
		},
	};

	private static GeometryGraphic lineGfx = new()
	{
		Transform = Transform.PixelIdentity,
		Geometry = new LineGeometry()
		{
			Line = new() { Start = new(-25, -25), End = new(31.5, 31.5) }
		},
		DrawLayers = new()
		{
			new Stroke(new ColorBrush { Color = Colors.Cyan }, 7),
			new Stroke(new ColorBrush { Color = Colors.Green }, 3),
		}
	};


	private static List<DrawLayer> drawLayersMinor = new()
	{
		new Stroke(new ColorBrush{Color = Colors.DarkBlue with { A = 0.125f } }, 5),
		new Stroke(new ColorBrush{Color = Colors.LightBlue with { A = 0.25f } }, 1),
	};

	private static List<DrawLayer> drawLayersMajor = new()
	{
		new Stroke(new ColorBrush{Color = Colors.DarkBlue with { A = 0.25f } }, 5),
		new Stroke(new ColorBrush{Color = Colors.LightBlue with { A = 0.5f } }, 1),
	};

	private static GeometryGraphic lineGfxX = new()
	{
		Geometry = new LineGeometry
		{
			Line = new() { Start = new(0, 0), End = new(0, 0) },
		},
	};

	private static GeometryGraphic lineGfxY = new()
	{
		Geometry = new LineGeometry
		{
			Line = new() { Start = new(0, 0), End = new(0, 0) },
		},
	};

	private static double rotation = 0;
	unsafe private static void Frame(FrameEvent evt)
	{
		rotation += double.Tau * evt.Delta.TotalSeconds / 2;
	}

	private static void Draw(DrawEvent evt)
	{
		var vp = evt.Viewport!;

		vp.Clear(Colors.DarkSlateGray);
		vp.SetTransform(Transform.PixelIdentity);

		// Draw Vertical Lines
		((LineGeometry)lineGfxX.Geometry!).Line = new Line { Start = new(0, 0), End = new(0, vp.Size.Y) };
		for (int x = 0; x < vp.Size.X; x += 20)
		{
			lineGfxX.Transform = Transform.Identity.Translated(new Vector2(x, 0));
			if (x % 100 == 0)
			{
				lineGfxX.DrawLayers = drawLayersMajor;
			}
			else
			{
				lineGfxX.DrawLayers = drawLayersMinor;
			}
			vp.DrawGraphic(lineGfxX);
		}

		// Draw Horizontal Lines
		((LineGeometry)lineGfxY.Geometry!).Line = new Line { Start = new(0, 0), End = new(vp.Size.X, 0) };
		for (int y = 0; y < vp.Size.Y; y += 20)
		{
			lineGfxY.Transform = Transform.Identity.Translated(new Vector2(0, y));
			if (y % 100 == 0)
			{
				lineGfxY.DrawLayers = drawLayersMajor;
			}
			else
			{
				lineGfxY.DrawLayers = drawLayersMinor;
			}
			vp.DrawGraphic(lineGfxY);
		}

		// Transform Graphics
		if (Program.App != null)
		{
			rectGfx.Transform = Transform.Identity.Rotated(rotation).Translated(pointerPosition);
			ellipseGfx.Transform = Transform.Identity.Rotated(rotation).Translated(pointerPosition);
			lineGfx.Transform = Transform.Identity.Rotated(rotation).Translated(pointerPosition);
		}

		// Populate Array of Graphics
		Graphic[] graphics = new[]
		{
			ellipseGfx,
			rectGfx,
			lineGfx,
		};

		// Draw Graphics
		foreach (var gfx in graphics)
		{
			vp.DrawGraphic(gfx);
		}
	}
}

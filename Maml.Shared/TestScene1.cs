using Maml.Graphics;
using Maml.Math;
using Maml.Observable;
using Maml.Scene;
using System;

namespace Maml;

internal class TestScene1 : Node
{
	private LineGrid mainGrid { get; } = default!;
	private LineGrid rotatingGrid { get; } = default!;
	private Node centeringNode { get; } = default!;
	private Node rotatingNode { get; } = default!;
	private Node reticleNode { get; } = default!;

	private Binding<WindowBase, Vector2> gridSizeBinding { get; } = default!;

	public TestScene1() : base()
	{
		gridSizeBinding =
		WindowBase.SizeProperty[Window].With((Vector2 size) =>
		{
			var max = double.Max(size.X, size.Y);
			var side = double.Sqrt(2 * max * max);
			side = double.Ceiling(side / rotatingGrid.MinorSpacing.X * 2) * rotatingGrid.MinorSpacing.X * 2;
			return new Vector2(side, side);
		});

		Children = new()
		{
			(mainGrid = new LineGrid
			{
				MinorSpacing = new(20, 20),
				MajorInterval = new(5, 5),
				LineDrawLayersMajor = new()
				{
					new Stroke(new ColorBrush { Color = Colors.BlueViolet with { A = 0.25f } }, 3),
					new Stroke(new ColorBrush { Color = Colors.Lime with { A = 0.25f } }, 1),
				},
				LineDrawLayersMinor = new()
				{
					new Stroke(new ColorBrush { Color = Colors.BlueViolet with { A = 0.25f } }, 1),
				},
				Transform = Transform.PixelIdentity,
				[LineGrid.SizeProperty] = WindowBase.SizeProperty[Window],
			}),

			(centeringNode = new Node
			{
				[OriginProperty] = WindowBase.SizeProperty[Window].With((Vector2 v) => v / 2),
				Children = new()
				{
					(rotatingNode = new Node
					{
						Children = new()
						{
							(rotatingGrid = new LineGrid
							{
								MinorSpacing = new(40, 40),
								MajorInterval = new(10000, 10000),
								LineDrawLayersMinor = new()
								{
									new Stroke(new ColorBrush { Color = Colors.DarkViolet, }, 3),
								},
								[LineGrid.SizeProperty] = gridSizeBinding,
								[OriginProperty] = gridSizeBinding.With((Vector2 v) => v / -2),
							}),
						},
					}),

					(reticleNode = new Node
					{
						Children = new()
						{
							(new GraphicNode
							{
								Graphic = new GeometryGraphic
								{
									Geometry = new EllipseGeometry
									{
										Ellipse = new() { Radius = new(13, 13), },
									},
									DrawLayers = new()
									{
										new Stroke(new ColorBrush { Color = Colors.AntiqueWhite, }, 3),
									},
								},
							}),
							(new GraphicNode
							{
								Graphic = new GeometryGraphic
								{
									Geometry = new EllipseGeometry
									{
										Ellipse = new() { Radius = new(11, 11), },
									},
									DrawLayers = new()
									{
										new Fill(new ColorBrush { Color = Colors.Red with { A = 0.5f, }, }),
										new Stroke(new ColorBrush { Color = Colors.Red, }, 3),
									},
								},
							}),
						},
					}),
				},
			}),

			(new GraphicNode
			{
				Graphic = new GeometryGraphic
				{
					Geometry = new TextGeometry
					{
						Text = "This is one helluva piece of things!",
						Font = new()
						{
							Name = "Segoe UI",
							Size = 16,
							Style = FontStyle.Normal,
							Weight = 400,
						},
					},
					DrawLayers = new()
					{
						//new Stroke(new ColorBrush { Color = Colors.Red, }, 4),
						new Fill(new ColorBrush { Color = Colors.White, }),
					},
				},
				Transform = Transform.PixelIdentity with { Origin = new(100, 100), },
			}),
		};

		for (int i = 0; i < 100; i++)
		{
			Children.Add(new TwirlyNode
			{
				Origin = new(
					Random.Shared.Next((int)Window.Size.X),
					Random.Shared.Next((int)Window.Size.Y)),
			});
		}

		DateTime startTick = DateTime.Now;
		Animator.Frame += (s, e) =>
		{
			switch (e.FrameState)
			{
				case Animation.FrameState.Enter:
					startTick = e.Tick;
					break;
				case Animation.FrameState.Play:
					rotatingNode.Transform = Transform.Identity.Rotated(-(startTick - e.Tick).TotalSeconds * 0.1);
					break;
			}
		};

	}
}

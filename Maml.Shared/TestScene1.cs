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

		Text text = new Text
		{
			String = "This is one helluva piece of things! 😊🔫\nAlso, this is cool...",
			//String = "File	Edit	View	Git	Project	Build	Debug",
			Font = new()
			{
				Name = "Cascadia Mono",
				Size = 10.0 * (96.0 / 72.0),
				Style = FontStyle.Normal,
				Weight = FontWeight.Normal,
			},
		};

		Window.PointerDown += (s, e) =>
		{
			if (e.Button == Events.PointerButton.Right)
			{
				text.String = "Oh My Goodness!!!!!";
				text.Font = text.Font with { Name = "Segoe Script", Size = 32, Weight = FontWeight.ExtraHeavy, };
			}
			else if (e.Button == Events.PointerButton.Left)
			{
				text.String = "This is one helluva piece of things! 😊🔫\nAlso, this is cool...";
				text.Font = new()
				{
					Name = "Cascadia Mono",
					Size = 10.0 * (96.0 / 72.0),
					Style = FontStyle.Normal,
					Weight = FontWeight.Normal,
				};
			}
		};

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
					Geometry = new RectGeometry
					{
						[RectGeometry.RectProperty] = Text.SizeProperty[text].With<Rect>((size) =>
						{
							return new Rect { Position = new(-4.5, -4.5), Size = Vector2.Round(size) + new Vector2(8, 8), };
						}),
					},
					DrawLayers = new()
					{
						new Fill(new ColorBrush { Color = new Color(0x333333ff), }),
						new Stroke(new ColorBrush { Color = Colors.White, }, 1),
					},
				},
				Transform = Transform.Identity with { Origin = new(8, 8), },
				Children = new()
				{
					//(new GraphicNode
					//{
					//	Graphic = new TextGraphic
					//	{
					//		Text = text,
					//		Brush = new ColorBrush { Color = Colors.Green, },
					//	},
					//	Transform = Transform.PixelIdentity.Translated(new(-1, 0)),
					//}),
					//(new GraphicNode
					//{
					//	Graphic = new TextGraphic
					//	{
					//		Text = text,
					//		Brush = new ColorBrush { Color = Colors.Red, },
					//	},
					//	Transform = Transform.PixelIdentity.Translated(new(1, 0)),
					//}),
					(new GraphicNode
					{
						Graphic = new TextGraphic
						{
							Text = text,
							Brush = new ColorBrush { Color = Colors.White, },
						},
						Transform = Transform.Identity,
					}),
				},
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

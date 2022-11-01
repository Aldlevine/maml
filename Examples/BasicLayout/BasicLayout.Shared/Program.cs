using Maml;
using Maml.Graphics;
using Maml.Math;
using Maml.Observable;
using Maml.Scene;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net.Http.Headers;

namespace BasicLayout;

internal static class Program
{
	private static Engine Engine { get; } = Engine.Singleton;

	private static void Main()
	{
		Engine.Initialize();
		Engine.Window.SceneTree.Root = new MainScene();
		Engine.Run();
		Engine.Dispose();
	}
}

internal class MainScene : Node
{
	private static DrawLayer[] drawLayers { get; } = new DrawLayer[]
	{
		//new Fill(new ColorBrush { Color = new Color(0x121212ff), }),
		//new Stroke(new ColorBrush { Color = new Color(0x444444ff), }, 1),
		new Fill(new ColorBrush { Color = new Color(0x121212ff), }),
		new Stroke(new ColorBrush { Color = Colors.Red, }, 1),
	};

	private VBox vbox;
	private RectBox rb1, rb2, rb3;
	private Text text;

	private ComputedProperty<MainScene, Vector2> headerSizeProp = new()
	{
		Get = (self) => new Vector2(self.vbox.Size.X, self.text.Size.Y + (self.rb1.Padding + self.rb1.Margin) * 2),
		Dependencies = (self) => new Binding[]
		{
			Text.SizeProperty[self.text],
			VBox.SizeProperty[self.vbox],
			RectBox.PaddingProperty[self.rb1],
			RectBox.MarginProperty[self.rb1],
		},
	};
	private Vector2 headerSize => headerSizeProp[this].Get();

	public MainScene()
	{
		var windowSize = WindowBase.SizeProperty[Window];
		Dictionary<string, Node> nodes = new();

		Children = new()
		{
			(vbox = new VBox()
			{
				[Box.SizeProperty] = WindowBase.SizeProperty[Window],
				Origin = new(0.5, 0.5),
				Children = new()
				{
					(rb1 = new RectBox()
					{
						DrawLayers = drawLayers,
						Margin = 4,
						Padding = 4,
						Children = new()
						{
							new GraphicNode
							{
								Graphic = new TextGraphic
								{
									Text = (text = new Text
									{
										String = "Hello world, this is some awesome text!\nWhich is pretty cool!",
										LineHeight = 1.2.Relative(),
										Font = new Font
										{
											Name = "Arial",
											Size = 16,
											Weight = FontWeight.Normal,
										},
									}),
									Brush = new ColorBrush { Color = Colors.White, },
								},
								Origin = new(8 - 0.5, 8 - 0.5),
							}
						}
					}),

					(rb2 = new RectBox()
					{
						DrawLayers = drawLayers,
						Margin = 4,
					}),

					(rb3 = new RectBox()
					{
						DrawLayers = drawLayers,
						Margin = 4,
					}),
				}
			}),
		};

		//Box.SizeProperty[rb1].BindTo(Box.SizeProperty[vbox].With(s => new Vector2(s.X, 32)));
		Box.SizeProperty[rb1].BindTo(headerSizeProp[this]);
		Box.SizeProperty[rb2].BindTo(Box.SizeProperty[vbox].With(s => new Vector2(s.X, (s.Y - headerSize.Y) * 0.75)));
		Box.SizeProperty[rb3].BindTo(Box.SizeProperty[vbox].With(s => new Vector2(s.X, (s.Y - headerSize.Y) * 0.25)));
		Text.MaxSizeProperty[text].BindTo(Box.SizeProperty[rb1].With(s => (s - (rb1.Padding + rb1.Margin) * 2) with { Y = double.PositiveInfinity }));
	}
}


internal class RectBox : Box, IGraphicNode
{
	public static BasicProperty<RectBox, DrawLayer[]> DrawLayersProperty = new(Array.Empty<DrawLayer>());
	public DrawLayer[] DrawLayers
	{
		get => DrawLayersProperty[this].Get();
		set => DrawLayersProperty[this].Set(value);
	}

	public static BasicProperty<RectBox, double> MarginProperty = new(0);
	public double Margin
	{
		get => MarginProperty[this].Get();
		set => MarginProperty[this].Set(value);
	}

	public static BasicProperty<RectBox, double> PaddingProperty = new(0);
	public double Padding
	{
		get => PaddingProperty[this].Get();
		set => PaddingProperty[this].Set(value);
	}

	private GeometryGraphic Graphic;

	public RectBox(): base()
	{
		Graphic = new()
		{
			[GeometryGraphic.DrawLayersProperty] = DrawLayersProperty[this],
			Geometry = new RectGeometry
			{
				[RectGeometry.RectProperty] = RectProperty[this].With(r => r.GrownBy(-Margin)),
			},
		};

		Changed += (s, p) => NeedsRedraw = (p != Node.VisibleInTreeProperty) && VisibleInTree;
	}

	public bool NeedsRedraw { get; set; } = true;
	public void Draw(RenderTarget renderTarget)
	{
		if (Graphic?.Geometry == null) { return; }

		renderTarget.SetTransform(GlobalTransform);
		foreach (var layer in DrawLayers)
		{
			renderTarget.DrawGeometry(Graphic.Geometry, layer);
		}
	}

	public Rect PreviousBoundingRect { get; set; } = new();
	public Rect GetBoundingRect() => GlobalTransform * Graphic?.GetBoundingRect() ?? new Rect();
}

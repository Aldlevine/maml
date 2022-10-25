using Maml;
using Maml.Animation;
using Maml.Events;
using Maml.Graphics;
using Maml.Math;
using Maml.Scene;
using System;

public class TwirlyNode : Node
{
	private Node bigCircles { get; }
	private Node smallCircles { get; }
	public TwirlyNode()
	{
		PointerDown += HandlePointerDown;
		PointerEnter += HandlePointerEnter;
		PointerExit += HandlePointerExit;

		HitShape = new Ellipse()
		{
			Radius = new(30, 30),
		};

		Children = new()
		{
			(bigCircles = new Node
			{
				Children = new()
				{
					new GraphicNode
					{
						Graphic = baseGfx,
						Transform = new() { Origin = new(-15, 0), },
					},
					new GraphicNode
					{
						Graphic = baseGfx,
						Transform = new() { Origin = new(+15, 0), },
					},
					new GraphicNode
					{
						Graphic = baseGfx,
						Transform = new() { Origin = new(0, -15), },
					},
					new GraphicNode
					{
						Graphic = baseGfx,
						Transform = new() { Origin = new(0, +15), },
					},

					(smallCircles = new Node
					{
						Children = new()
						{
							new GraphicNode
							{
								Graphic = baseGfx,
								Transform = new() { Scale = new(0.75, 0.75), Origin = new(-25, 0), },
							},
							new GraphicNode
							{
								Graphic = baseGfx,
								Transform = new() { Scale = new(0.75, 0.75), Origin = new(+25, 0), },
							},
							new GraphicNode
							{
								Graphic = baseGfx,
								Transform = new() { Scale = new(0.75, 0.75), Origin = new(0, -25), },
							},
							new GraphicNode
							{
								Graphic = baseGfx,
								Transform = new() { Scale = new(0.75, 0.75), Origin = new(0, +25), },
							},
						},
					}),
				},
			}),
			(new GraphicNode
			{
				Graphic = (hitRectGfx = new GeometryGraphic
				{
					Geometry = new EllipseGeometry { Ellipse = (Ellipse)HitShape, },
					DrawLayers = hitRectHidden,
				}),
			}),
		};

		Engine.Singleton.Animator.Frame += Pulse;
	}

	#region Resources
	private static DrawLayer[] defaultDrawLayers { get; } = new[]
	{
		new Stroke(new ColorBrush { Color = Colors.DarkOrange }, 7),
		new Stroke(new ColorBrush { Color = Colors.HotPink }, 3),
	};

	private static DrawLayer[] selectedDrawLayers { get; } = new[]
	{
		new Stroke(new ColorBrush { Color = Colors.PaleGreen }, 7),
		new Stroke(new ColorBrush { Color = Colors.DarkGoldenrod }, 3),
	};

	private DrawLayer[] animatedDrawLayers { get; } = new[]
	{
		defaultDrawLayers[0] with { Brush = new ColorBrush((ColorBrush)defaultDrawLayers[0].Brush)},
		defaultDrawLayers[1] with { Brush = new ColorBrush((ColorBrush)defaultDrawLayers[1].Brush)},
	};

	private static EllipseGeometry baseGeo { get; } = new()
	{
		Ellipse = new() { Radius = new(10, 10), }
	};

	private GeometryGraphic hitRectGfx { get; }
	private DrawLayer[] hitRectVisible { get; } = Array.Empty<DrawLayer>();
	private DrawLayer[] hitRectHidden { get; } = Array.Empty<DrawLayer>();

	private GeometryGraphic baseGfx { get; } = new()
	{
		Geometry = baseGeo,
		DrawLayers = defaultDrawLayers,
	};

	#endregion

	#region Animations
	private void Spin(object? sender, FrameEvent evt)
	{

		switch (evt.FrameState)
		{
			case FrameState.Enter:
				{
					hitRectGfx.DrawLayers = hitRectVisible;
				}
				break;

			case FrameState.Exit:
				{
					hitRectGfx.DrawLayers = hitRectHidden;
				}
				break;

			case FrameState.Play:
				{
					bigCircles.Transform = bigCircles.Transform.Rotated(evt.Delta.TotalSeconds * 5);
					smallCircles.Transform = smallCircles.Transform.Rotated(evt.Delta.TotalSeconds * -10);
				}
				break;
		}
	}

	//private static readonly Vector2 minScale = Vector2.One;
	//private static readonly Vector2 maxScale = new(2, 2);
	//private double pulsePhase = Random.Shared.NextDouble() * double.Tau;
	//private DateTime pulseTick = DateTime.Now;
	private void Pulse(object? sender, FrameEvent evt)
	{
		switch (evt.FrameState)
		{
			case FrameState.Enter:
				{
					//pulseTick = evt.Tick;
				}
				break;

			case FrameState.Exit:
				{
					//pulsePhase = 0;
				}
				break;

			case FrameState.Play:
				{
					//// var t = double.Sin((evt.Tick - pulseTick).TotalSeconds * 5 + pulsePhase) * 0.5 + 0.5;
					//var t = Unit.Triangle((evt.Tick - pulseTick).TotalSeconds * 5 + pulsePhase) * 0.5 + 0.5;
					//var targetScale = Vector2.Lerp(minScale, maxScale, t);
					//var scale = Vector2.Lerp(Transform.Scale, targetScale, double.Clamp(evt.Delta.TotalSeconds * 10, 0, 1));
					//Transform = Transform with { Scale = scale, };
				}
				break;
		}
	}

	private void ResetScale(object? sender, FrameEvent evt)
	{
		switch (evt.FrameState)
		{
			case FrameState.Enter:
				break;

			case FrameState.Exit:
				break;

			case FrameState.Play:
				{
					var scale = Transform.Scale;
					if (double.Abs(scale.X - 1) < 0.01 && double.Abs(scale.Y - 1) < 0.01)
					{
						scale = Vector2.One;
						Animator.Frame -= ResetScale;
					}
					else
					{
						scale = Vector2.Lerp(scale, Vector2.One, double.Clamp(evt.Delta.TotalSeconds * 10, 0, 1));
					}
					Transform = Transform with { Scale = scale, };
				}
				break;
		}

	}

	private void ShowSelect(object? sender, FrameEvent evt)
	{
		switch (evt.FrameState)
		{
			case FrameState.Enter:
				{
					baseGfx.DrawLayers = animatedDrawLayers;
				}
				break;

			case FrameState.Exit:
				{
					baseGfx.DrawLayers = defaultDrawLayers;
				}
				break;

			case FrameState.Play:
				{
					bool changed = false;
					for (int i = 0; i < animatedDrawLayers.Length; i++)
					{
						if (animatedDrawLayers[i].Brush is not ColorBrush animatedBrush) { continue; }
						if (defaultDrawLayers[i].Brush is not ColorBrush) { continue; }
						if (selectedDrawLayers[i].Brush is not ColorBrush selectedBrush) { continue; }
						animatedBrush.Color = Color.Lerp(animatedBrush.Color, selectedBrush.Color, double.Clamp(evt.Delta.TotalSeconds * 7.5, 0, 1));
						if (Color.ApproxEqual(animatedBrush.Color, selectedBrush.Color, 0.1))
						{
							changed = true;
						}
					}
					if (changed)
					{
						baseGfx[GeometryGraphic.DrawLayersProperty].SetDirty();
					}
				}
				break;
		}
	}

	private void HideSelect(object? sender, FrameEvent evt)
	{
		switch (evt.FrameState)
		{
			case FrameState.Enter:
				{
					baseGfx.DrawLayers = animatedDrawLayers;
				}
				break;

			case FrameState.Exit:
				{
					baseGfx.DrawLayers = defaultDrawLayers;
				}
				break;

			case FrameState.Play:
				{
					bool changed = false;
					for (int i = 0; i < animatedDrawLayers.Length; i++)
					{
						if (animatedDrawLayers[i].Brush is not ColorBrush animatedBrush) { continue; }
						if (defaultDrawLayers[i].Brush is not ColorBrush defaultBrush) { continue; }
						if (selectedDrawLayers[i].Brush is not ColorBrush) { continue; }
						animatedBrush.Color = Color.Lerp(animatedBrush.Color, defaultBrush.Color, double.Clamp(evt.Delta.TotalSeconds * 0.125, 0, 1));
						if (!Color.ApproxEqual(animatedBrush.Color, defaultBrush.Color, 0.1))
						{
							changed = true;
						}
					}
					if (changed)
					{
						baseGfx[GeometryGraphic.DrawLayersProperty].SetDirty();
					}
				}
				break;
		}
	}
	#endregion

	#region Events
	private void HandlePointerDown(object? sender, PointerEvent evt)
	{
		if (evt.Button == PointerButton.Left)
		{
			Window.PointerUp += HandlePointerUp;
			Window.PointerMove += HandlePointerMove;

			// Remove animations
			Animator.Frame -= Pulse;
			Animator.Frame -= HideSelect;

			// Add animations
			Animator.Frame += Spin;
			Animator.Frame += ResetScale;
			Animator.Frame += ShowSelect;

			Parent?.Children.Add(this);
		}
	}

	private void HandlePointerMove(object? sender, PointerEvent evt) => GlobalTransform = GlobalTransform.Translated(evt.PositionDelta);

	private void HandlePointerUp(object? sender, PointerEvent evt)
	{
		if (evt.Button == PointerButton.Left)
		{
			Window.PointerUp -= HandlePointerUp;
			Window.PointerMove -= HandlePointerMove;

			// Remove animations
			Animator.Frame -= Spin;
			Animator.Frame -= ResetScale;
			Animator.Frame -= ShowSelect;

			// Add animations
			Animator.Frame += Pulse;
			Animator.Frame += HideSelect;
		}
	}

	private void HandlePointerEnter(object? sender, PointerEvent evt)
	{
		if ((evt.ButtonMask & PointerButton.Left) > 0)
		{
			Animator.Frame -= HideSelect;
			Animator.Frame += ShowSelect;
		}
	}

	private void HandlePointerExit(object? sender, PointerEvent evt)
	{
		Animator.Frame -= ShowSelect;
		Animator.Frame += HideSelect;
	}
	#endregion
}


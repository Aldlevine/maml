using Maml.Animation;
using Maml.Events;
using Maml.Graphics;
using Maml.Math;
using Maml.Scene;
using System;
using System.Collections.Generic;

namespace Maml;

public class TwirlyNode : Node
{
	private Node bigCircles;
	private Node smallCircles;
	public TwirlyNode()
	{
		GraphicComponent gfxComponent = new()
		{
			Graphic = ellipseGfx,
		};

		PointerDown += OnPointerDown;
		PointerEnter += OnPointerEnter;
		PointerExit += OnPointerExit;

		HitShape = new Ellipse()
		{
			Radius = new(30, 30),
		};

		Children = new()
		{
			(bigCircles = new Node
			{
				Graphics = new()
				{
					new GraphicComponent(gfxComponent)
					{
						Transform = new() { Origin = new(-15, 0), },
					},
					new GraphicComponent(gfxComponent)
					{
						Transform = new() { Origin = new(+15, 0), },
					},
					new GraphicComponent(gfxComponent)
					{
						Transform = new() { Origin = new(0, -15), },
					},
					new GraphicComponent(gfxComponent)
					{
						Transform = new() { Origin = new(0, +15), },
					},
				},

				Children = new()
				{
					(smallCircles = new Node
					{
						Graphics = new()
						{
							(new GraphicComponent(gfxComponent)
							{
								Transform = new Transform { Scale = new(0.75, 0.75), Origin = new(+25, 0), }
							}),
							(new GraphicComponent(gfxComponent)
							{
								Transform = new Transform { Scale = new(0.75, 0.75), Origin = new(-25, 0), }
							}),
							(new GraphicComponent(gfxComponent)
							{
								Transform = new Transform { Scale = new(0.75, 0.75), Origin = new(0, +25), }
							}),
							(new GraphicComponent(gfxComponent)
							{
								Transform = new Transform { Scale = new(0.75, 0.75), Origin = new(0, -25), }
							}),
						},
					}),
				},
			}),
			(new Node
			{
				Graphics = new()
				{
					(new GraphicComponent
					{
						Graphic = (hitRectGfx = new GeometryGraphic
						{
							Geometry = new EllipseGeometry { Ellipse = (Ellipse)HitShape, },
							DrawLayers = hitRectHidden,
						}),
					}),
				},
			}),
		};

		Engine.Singleton.Animator.Frame += Pulse;
	}

	#region Resources
	private static List<DrawLayer> defaultDrawLayers = new()
	{
		new Stroke(new ColorBrush { Color = Colors.DarkOrange }, 7),
		new Stroke(new ColorBrush { Color = Colors.HotPink }, 3),
	};

	private static List<DrawLayer> selectedDrawLayers = new()
	{
		new Stroke(new ColorBrush { Color = Colors.PaleGreen }, 7),
		new Stroke(new ColorBrush { Color = Colors.DarkGoldenrod }, 3),
	};

	private List<DrawLayer> animatedDrawLayers = new()
	{
		defaultDrawLayers[0] with { Brush = new ColorBrush((ColorBrush)defaultDrawLayers[0].Brush)},
		defaultDrawLayers[1] with { Brush = new ColorBrush((ColorBrush)defaultDrawLayers[1].Brush)},
	};

	private static EllipseGeometry ellipseGeo = new()
	{
		Ellipse = new() { Radius = new(10, 10), }
	};

	private GeometryGraphic hitRectGfx;
	private List<DrawLayer> hitRectVisible = new() { };
	private List<DrawLayer> hitRectHidden = new() { };

	private GeometryGraphic ellipseGfx = new()
	{
		Geometry = ellipseGeo,
		DrawLayers = new()
		{
			defaultDrawLayers[0],
			defaultDrawLayers[1],
		},
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

	private static readonly Vector2 minScale = Vector2.One;
	private static readonly Vector2 maxScale = new(2, 2);
	private double pulsePhase = Random.Shared.NextDouble() * double.Tau;
	private DateTime pulseTick = DateTime.Now;
	private void Pulse(object? sender, FrameEvent evt)
	{
		switch (evt.FrameState)
		{
			case FrameState.Enter:
				{
					pulseTick = evt.Tick;
				}
				break;

			case FrameState.Exit:
				{
					pulsePhase = 0;
				}
				break;

			case FrameState.Play:
				{
					// var t = double.Sin((evt.Tick - pulseTick).TotalSeconds * 5 + pulsePhase) * 0.5 + 0.5;
					var t = Unit.Triangle((evt.Tick - pulseTick).TotalSeconds * 5 + pulsePhase) * 0.5 + 0.5;
					var targetScale = Vector2.Lerp(minScale, maxScale, t);
					var scale = Vector2.Lerp(Transform.Scale, targetScale, evt.Delta.TotalSeconds * 10);
					Transform = Transform with { Scale = scale, };
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
						Engine.Singleton.Animator.Frame -= ResetScale;
					}
					else
					{
						scale = Vector2.Lerp(scale, Vector2.One, double.Min(evt.Delta.TotalSeconds * 10, 1));
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
					ellipseGfx.DrawLayers.Clear();
					ellipseGfx.DrawLayers.AddRange(animatedDrawLayers);
				}
				break;

			case FrameState.Exit:
				{
					ellipseGfx.DrawLayers.Clear();
					ellipseGfx.DrawLayers.AddRange(defaultDrawLayers);
				}
				break;

			case FrameState.Play:
				{
					for (int i = 0; i < animatedDrawLayers.Count; i++)
					{
						if (animatedDrawLayers[i].Brush is not ColorBrush animatedBrush) { continue; }
						if (defaultDrawLayers[i].Brush is not ColorBrush defaultBrush) { continue; }
						if (selectedDrawLayers[i].Brush is not ColorBrush selectedBrush) { continue; }
						animatedBrush.Color = Color.Lerp(animatedBrush.Color, selectedBrush.Color, evt.Delta.TotalSeconds * 7.5);
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
					ellipseGfx.DrawLayers.Clear();
					ellipseGfx.DrawLayers.AddRange(animatedDrawLayers);
				}
				break;

			case FrameState.Exit:
				{
					ellipseGfx.DrawLayers.Clear();
					ellipseGfx.DrawLayers.AddRange(defaultDrawLayers);
				}
				break;

			case FrameState.Play:
				{
					for (int i = 0; i < animatedDrawLayers.Count; i++)
					{
						if (animatedDrawLayers[i].Brush is not ColorBrush animatedBrush) { continue; }
						if (defaultDrawLayers[i].Brush is not ColorBrush defaultBrush) { continue; }
						if (selectedDrawLayers[i].Brush is not ColorBrush selectedBrush) { continue; }
						animatedBrush.Color = Color.Lerp(animatedBrush.Color, defaultBrush.Color, evt.Delta.TotalSeconds * 0.125);
					}
				}
				break;
		}
	}
	#endregion

	#region Events
	private void OnPointerDown(object? sender, PointerEvent evt)
	{
		if (evt.Button == PointerButton.Left)
		{
			Engine.Singleton.Window.PointerUp += OnPointerUp;
			Engine.Singleton.Window.PointerMove += OnPointerMove;

			// Remove animations
			Engine.Singleton.Animator.Frame -= Pulse;
			Engine.Singleton.Animator.Frame -= HideSelect;

			// Add animations
			Engine.Singleton.Animator.Frame += Spin;
			Engine.Singleton.Animator.Frame += ResetScale;
			Engine.Singleton.Animator.Frame += ShowSelect;

			Parent?.Children.Add(this);
		}
	}

	private void OnPointerMove(object? sender, PointerEvent evt)
	{
		Transform = Transform.Translated(evt.PositionDelta);
	}

	private void OnPointerUp(object? sender, PointerEvent evt)
	{
		if (evt.Button == PointerButton.Left)
		{
			Engine.Singleton.Window.PointerUp -= OnPointerUp;
			Engine.Singleton.Window.PointerMove -= OnPointerMove;

			// Remove animations
			Engine.Singleton.Animator.Frame -= Spin;
			Engine.Singleton.Animator.Frame -= ResetScale;
			Engine.Singleton.Animator.Frame -= ShowSelect;

			// Add animations
			Engine.Singleton.Animator.Frame += Pulse;
			Engine.Singleton.Animator.Frame += HideSelect;

		}
	}

	private void OnPointerEnter(object? sender, PointerEvent evt)
	{
		if ((evt.ButtonMask & PointerButton.Left) > 0)
		{
			Engine.Singleton.Animator.Frame -= HideSelect;
			Engine.Singleton.Animator.Frame += ShowSelect;
		}
	}

	private void OnPointerExit(object? sender, PointerEvent evt)
	{
		Engine.Singleton.Animator.Frame -= ShowSelect;
		Engine.Singleton.Animator.Frame += HideSelect;
	}
	#endregion
}


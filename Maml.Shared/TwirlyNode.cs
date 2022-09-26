using Maml.Animation;
using Maml.Events;
using Maml.Graphics;
using Maml.Math;
using Maml.Scene;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace Maml;

public class TwirlyNode : Node
{
	private static List<DrawLayer> drawLayers = new()
	{
		new Fill(new ColorBrush { Color = Colors.RebeccaPurple with { A = 0.25f } }),
		new Stroke(new ColorBrush { Color = Colors.HotPink }, 7),
		new Stroke(new ColorBrush { Color = Colors.DarkMagenta }, 3),
	};

	private static GeometryGraphic ellipseGfx = new()
	{
		Geometry = new EllipseGeometry { Ellipse = new() { Radius = new(10, 10) }, },
		DrawLayers = drawLayers,
	};

	private static GraphicComponent gfxComponent = new()
	{
		Graphic = ellipseGfx,
	};

	private Node bigCircles;
	private Node smallCircles;
	// private ColorBrush hitRectBrush;

	private GeometryGraphic hitRectGfx;
	private List<DrawLayer> hitRectVisible = new() { new Stroke(new ColorBrush { Color = Colors.Blue }, 2), };
	private List<DrawLayer> hitRectHidden = new() { new Stroke(new ColorBrush { Color = Colors.Blue with { A = 0.5f} }, 2), };

	public TwirlyNode()
	{

		PointerDown += OnPointerDown;

		HitRect = new()
		{
			Position = new(-30, -30),
			End = new(30, 30),
		};

		Graphics = new()
		{
			(new GraphicComponent
			{
				Graphic = (hitRectGfx = new GeometryGraphic
				{
					Geometry = new RectGeometry { Rect = HitRect, },
					DrawLayers = hitRectHidden,
				}),
			}),
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
		};

		Animator.Singleton.Frame += Pulse;
	}

	private void Spin(object? sender, FrameEvent evt)
	{

		switch (evt.FrameState)
		{
			case FrameState.Enter:
				{
					// hitRectBrush.Color = Colors.Blue;
					hitRectGfx.DrawLayers = hitRectVisible;
				}
				break;

			case FrameState.Exit:
				{
					// hitRectBrush.Color = Colors.Transparent;
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
		if (evt.FrameState == FrameState.Enter)
		{
			pulseTick = evt.Tick;
			// pulsePhase = ((Transform.Origin.X / Program.App?.Viewport.Size.X) ?? 0) * double.Tau * (Random.Shared.NextDouble() * 0.5 + 1);
		}

		else if (evt.FrameState == FrameState.Exit)
		{
			pulsePhase = 0;
		}

		else if (evt.FrameState == FrameState.Play)
		{
			var t = double.Sin((evt.Tick - pulseTick).TotalSeconds * 5 + pulsePhase) * 0.5 + 0.5;
			var targetScale = Vector2.Lerp(minScale, maxScale, t);
			var scale = Vector2.Lerp(bigCircles.Transform.Scale, targetScale, evt.Delta.TotalSeconds * 10);
			bigCircles.Transform = bigCircles.Transform with { Scale = scale, };
		}
	}


	private void ResetScale(object? sender, FrameEvent evt)
	{
		var scale = bigCircles.Transform.Scale;
		if (double.Abs(scale.X - 1) < 0.01 && double.Abs(scale.Y - 1) < 0.01)
		{
			scale = Vector2.One;
			Animator.Singleton.Frame -= ResetScale;
		}
		else
		{
			scale = Vector2.Lerp(scale, Vector2.One, double.Min(evt.Delta.TotalSeconds * 10, 1));
		}
		bigCircles.Transform = bigCircles.Transform with { Scale = scale, };
	}


	private void OnPointerDown(object? sender, PointerEvent evt)
	{
		if (evt.Button == PointerButton.Left)
		{
			Input.PointerUp += OnPointerUp;
			Input.PointerMove += OnPointerMove;
			Animator.Singleton.Frame += Spin;
			Animator.Singleton.Frame -= Pulse;
			Animator.Singleton.Frame += ResetScale;
		}
	}

	private void OnPointerMove(object? sender, PointerEvent evt)
	{
		Transform = Transform.Translated(evt.Delta);
	}

	private void OnPointerUp(object? sender, PointerEvent evt)
	{
		if (evt.Button == PointerButton.Left)
		{
			Input.PointerUp -= OnPointerUp;
			Input.PointerMove -= OnPointerMove;
			Animator.Singleton.Frame -= Spin;
			Animator.Singleton.Frame += Pulse;
			Animator.Singleton.Frame -= ResetScale;
		}
	}
}


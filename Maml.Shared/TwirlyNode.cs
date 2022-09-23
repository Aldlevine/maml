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
		new Stroke(new ColorBrush { Color = Colors.Cyan }, 7),
		new Stroke(new ColorBrush { Color = Colors.Green }, 3),
	};

	private static GeometryGraphic ellipseGfx = new()
	{
		Geometry = new EllipseGeometry { Ellipse = new() { Radius = new(10, 10) }, },
		DrawLayers = drawLayers,
	};

	private List<GraphicComponent> gfx = new()
	{
		new GraphicComponent
		{
			Graphic = ellipseGfx,
			Transform = Transform.Identity.Translated(new(-15, 0)),
		},
		new GraphicComponent
		{
			Graphic = ellipseGfx,
			Transform = Transform.Identity.Translated(new(+15, 0)),
		},
		new GraphicComponent
		{
			Graphic = ellipseGfx,
			Transform = Transform.Identity.Translated(new(0, -15)),
		},
		new GraphicComponent
		{
			Graphic = ellipseGfx,
			Transform = Transform.Identity.Translated(new(0, +15)),
		},
	};

	private InputComponent inp = new()
	{
		HitRect = new Rect { Position = new(-25, -25), Size = new(50, 50), },
	};

	public override List<GraphicComponent> Graphics => gfx;
	public override List<InputComponent> Inputs => new() { inp };

	public TwirlyNode()
	{
		inp.PointerDown += PointerDown;
		Animator.Singleton.Frame += Pulse;
	}

	private void Spin(FrameEvent evt)
	{
		Transform = Transform.Identity.Rotated(evt.Delta.TotalSeconds * 10).Transformed(Transform);
	}

	private TimeSpan pulseTime;
	private void Pulse(FrameEvent evt)
	{
		pulseTime += evt.Delta;
		var scale = (System.Math.Sin(pulseTime.TotalSeconds * 5) + 2) * 0.5;
		Transform = Transform with { Scale = new(scale, scale), };
	}

	private void PointerDown(PointerEvent evt)
	{
		if (evt.Button == PointerButton.Left)
		{
			inp.PointerUp += PointerUp;
			inp.PointerDown -= PointerDown;
			inp.PointerMove += PointerMove;
			Animator.Singleton.Frame += Spin;
			Animator.Singleton.Frame -= Pulse;
		}
	}

	private void PointerMove(PointerEvent evt)
	{
		Transform = Transform.Translated(evt.Delta);
	}

	private void PointerUp(PointerEvent evt)
	{
		if (evt.Button == PointerButton.Left)
		{
			inp.PointerUp -= PointerUp;
			inp.PointerDown += PointerDown;
			inp.PointerMove -= PointerMove;
			Animator.Singleton.Frame -= Spin;
			Animator.Singleton.Frame += Pulse;
		}
	}
}


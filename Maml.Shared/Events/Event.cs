using System;
using Maml.Graphics;
using Maml.Math;
using Maml.Events;
using System.Runtime.Intrinsics;

namespace Maml.Events;


public record Event { }

public record InitEvent : Event { };

public record KeyEvent : Event
{
	public VirtualKey VirtualKey;
	public bool Pressed;
	public bool Echo;
}

public record ResizeEvent : Event
{
	public Vector2 Size;
}

public record DrawEvent : Event
{
	public Viewport? Viewport;
}

public record PointerEvent : Event
{
	public Vector2 Position;
	public Vector2 Delta;
	public PointerButton Button;
	public PointerButton ButtonMask;
}

public record WheelEvent : PointerEvent
{
	public Vector2 Delta;
}

public record FocusEvent : Event
{
	public bool Focused;
}

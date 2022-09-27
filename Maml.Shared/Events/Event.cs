using Maml.Math;

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
	public RenderTargetBase? RenderTarget;
}

public record PointerEvent : Event
{
	public Vector2 Position;
	public Vector2 PositionDelta;
	public PointerButton Button;
	public PointerButton ButtonMask;
}

public record WheelEvent : PointerEvent
{
	public Vector2 WheelDelta;
}

public record FocusEvent : Event
{
	public bool Focused;
}

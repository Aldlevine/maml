using Maml.Graphics;
using Maml.Math;
using Maml.UserInput;

namespace Maml.Events;

public delegate void EventHandler<T>(T evt) where T : Event;

public record Event
{

}

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

public record FrameEvent : Event
{
	public double Delta;
}

public record PointerEvent : Event
{
	public Vector2 Position;
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

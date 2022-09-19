using Maml.Geometry;
using Maml.UserInput;

namespace Maml.Events;

public delegate void EventHandler<T>(T evt) where T : Event;

public record Event
{

}

public record KeyEvent: Event
{
	public VirtualKey VirtualKey;
	public bool Pressed;
	public bool Echo;
}

public record ResizeEvent: Event
{
	// public int Width;
	// public int Height;
	public Vector2 Size;
}

public record PointerEvent: Event
{
	// public int X;
	// public int Y;
	public Vector2 Position;
	public PointerButton Button;
	public PointerButton ButtonMask;
}

public record WheelEvent: PointerEvent
{
	// public double DX;
	// public double DY;
	public Vector2 Delta;
}

public record FocusEvent: Event
{
	public bool Focused;
}

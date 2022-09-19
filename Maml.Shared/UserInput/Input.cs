using Maml.Events;

namespace Maml.UserInput;

public static partial class Input
{
	public static event EventHandler<PointerEvent>? PointerMove;
	public static event EventHandler<PointerEvent>? PointerDown;
	public static event EventHandler<PointerEvent>? PointerUp;
	public static event EventHandler<WheelEvent>? Wheel;
	public static event EventHandler<KeyEvent>? KeyDown;
	public static event EventHandler<KeyEvent>? KeyUp;
	public static event EventHandler<FocusEvent>? Focus;
	public static event EventHandler<FocusEvent>? Blur;
}

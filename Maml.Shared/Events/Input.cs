using System;

namespace Maml.Events;

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

	public static void Emit(string name, Event evt)
	{
		switch (name)
		{
			case nameof(PointerMove):
				PointerMove?.Invoke(null, (PointerEvent)evt);
				break;
			case nameof(PointerDown):
				PointerDown?.Invoke(null, (PointerEvent)evt);
				break;
			case nameof(PointerUp):
				PointerUp?.Invoke(null, (PointerEvent)evt);
				break;
			case nameof(Wheel):
				Wheel?.Invoke(null, (WheelEvent)evt);
				break;
			case nameof(KeyDown):
				KeyDown?.Invoke(null, (KeyEvent)evt);
				break;
			case nameof(KeyUp):
				KeyUp?.Invoke(null, (KeyEvent)evt);
				break;
			case nameof(Focus):
				Focus?.Invoke(null, (FocusEvent)evt);
				break;
			case nameof(Blur):
				Blur?.Invoke(null, (FocusEvent)evt);
				break;
		}
	}
}

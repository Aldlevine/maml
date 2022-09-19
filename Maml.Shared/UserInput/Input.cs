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

	public static void Emit(string name, Event evt)
	{
		switch (name)
		{
			case nameof(PointerMove):
				PointerMove?.Invoke((PointerEvent)evt);
				break;
			case nameof(PointerDown):
				PointerDown?.Invoke((PointerEvent)evt);
				break;
			case nameof(PointerUp):
				PointerUp?.Invoke((PointerEvent)evt);
				break;
			case nameof(Wheel):
				Wheel?.Invoke((WheelEvent)evt);
				break;
			case nameof(KeyDown):
				KeyDown?.Invoke((KeyEvent)evt);
				break;
			case nameof(KeyUp):
				KeyUp?.Invoke((KeyEvent)evt);
				break;
			case nameof(Focus):
				Focus?.Invoke((FocusEvent)evt);
				break;
			case nameof(Blur):
				Blur?.Invoke((FocusEvent)evt);
				break;
		}
		// var @event = typeof(Input).GetEvent(name, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
		// @event?.RaiseMethod?.Invoke(null, new[] { evt });

		// var @event = typeof(Input).GetEvent(name);
		// @event?.GetRaiseMethod()?.Invoke(null, new[] { evt });
	}
}

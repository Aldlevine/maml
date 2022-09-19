using Maml.Events;
using System.Runtime.InteropServices.JavaScript;

namespace Maml.UserInput;

public static partial class Input
{

	[JSExport]
	internal static void HandlePointerMove(int x, int y, int iButtonMask)
	{
		PointerButton buttonMask = (PointerButton)iButtonMask;
		PointerMove?.Invoke(new PointerEvent
		{
			Position = new(x, y),
			ButtonMask = buttonMask,
		});
	}

	[JSExport]
	internal static void HandlePointerDown(int x, int y, int iButton, int iButtonMask)
	{
		PointerButton button = (PointerButton)iButton;
		if (button == PointerButton.Right) { button = PointerButton.Middle; }
		else if (button == PointerButton.Middle) { button = PointerButton.Right; }

		PointerButton buttonMask = (PointerButton)iButtonMask;
		PointerDown?.Invoke(new PointerEvent
		{
			Position = new(x, y),
			Button = button,
			ButtonMask = buttonMask,
		});
	}

	[JSExport]
	internal static void HandlePointerUp(int x, int y, int iButton, int iButtonMask)
	{
		PointerButton button = (PointerButton)iButton;
		if (button == PointerButton.Right) { button = PointerButton.Middle; }
		else if (button == PointerButton.Middle) { button = PointerButton.Right; }

		PointerButton buttonMask = (PointerButton)iButtonMask;
		PointerUp?.Invoke(new PointerEvent
		{
			Position = new(x, y),
			Button = button,
			ButtonMask = buttonMask,
		});
	}

	[JSExport]
	internal static void HandleWheel(int x, int y, double dx, double dy)
	{
		Wheel?.Invoke(new WheelEvent
		{
			Position = new(x, y),
			Delta = new(dx, dy),
		});
	}

	[JSExport]
	internal static void HandleKeyDown(string key, bool echo)
	{
		VirtualKey vk = InputHelpers.JSKeyToVirtualKey(key);
		KeyDown?.Invoke(new KeyEvent
		{
			VirtualKey = vk,
			Echo = echo,
			Pressed = true,
		});
	}

	[JSExport]
	internal static void HandleKeyUp(string key, bool echo)
	{
		VirtualKey vk = InputHelpers.JSKeyToVirtualKey(key);
		KeyUp?.Invoke(new KeyEvent
		{
			VirtualKey = vk,
			Echo = echo,
			Pressed = false,
		});
	}

	[JSExport]
	internal static void HandleFocus()
	{
		Focus?.Invoke(new FocusEvent
		{
			Focused = true,
		});
	}

	[JSExport]
	internal static void HandleBlur()
	{
		Blur?.Invoke(new FocusEvent
		{
			Focused = false,
		});
	}

	[JSImport("report", "input.js")]
	internal static partial void Report(string id, string value);
}

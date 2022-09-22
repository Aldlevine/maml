using Maml.Math;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.Pointer;
using static Maml.Utils.Bits;
using static Windows.Win32.PInvoke;

namespace Maml.UserInput;

public partial class Input
{
	// internal static void HandlePointerMove(int x, int y, int iButtonMask)
	internal static void HandlePointerMove(WPARAM wParam, LPARAM lParam)
	{
		if (Program.App == null)
		{
			return;
		}

		uint pointerId = (uint)LoWord(wParam);
		GetPointerInfo(pointerId, out var pointerInfo);
		double dpiRatio = 1.0 / Program.App.Viewport.DpiRatio;
		var pointerPosition = new Vector2(
			pointerInfo.ptPixelLocation.X - Program.App.windowPosition.X,
			pointerInfo.ptPixelLocation.Y - Program.App.windowPosition.Y);
		pointerPosition *= new Vector2(dpiRatio, dpiRatio);
		PointerButton buttonMask = PointerButton.None;
		if ((pointerInfo.pointerFlags & POINTER_FLAGS.POINTER_FLAG_FIRSTBUTTON) > 0) { buttonMask |= PointerButton.Left; }
		if ((pointerInfo.pointerFlags & POINTER_FLAGS.POINTER_FLAG_SECONDBUTTON) > 0) { buttonMask |= PointerButton.Right; }
		if ((pointerInfo.pointerFlags & POINTER_FLAGS.POINTER_FLAG_THIRDBUTTON) > 0) { buttonMask |= PointerButton.Middle; }
		if ((pointerInfo.pointerFlags & POINTER_FLAGS.POINTER_FLAG_FOURTHBUTTON) > 0) { buttonMask |= PointerButton.Back; }
		if ((pointerInfo.pointerFlags & POINTER_FLAGS.POINTER_FLAG_FIFTHBUTTON) > 0) { buttonMask |= PointerButton.Forward; }
		PointerMove?.Invoke(new Events.PointerEvent
		{
			Position = pointerPosition,
			ButtonMask = buttonMask,
		});
	}
}

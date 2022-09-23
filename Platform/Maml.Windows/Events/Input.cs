﻿using Maml.Math;
using System;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.Pointer;
using static Maml.Utils.Bits;
using static Windows.Win32.PInvoke;

namespace Maml.Events;

public partial class Input
{
	private static PointerButton previousButtonState = PointerButton.None;
	private static Vector2 previousPointerPosition = Vector2.Zero;

	internal static void HandlePointer(WPARAM wParam, LPARAM lParam)
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

		bool hasButtonChange = false;
		foreach (var button in (PointerButton[])Enum.GetValues(typeof(PointerButton)))
		{
			if ((buttonMask & button) > 0 && (previousButtonState & button) == 0)
			{
				PointerDown?.Invoke(new Events.PointerEvent
				{
					Position = pointerPosition,
					Button = button,
				});
				hasButtonChange = true;
			}
			else if ((buttonMask & button) == 0 && (previousButtonState & button) > 0)
			{
				PointerUp?.Invoke(new Events.PointerEvent
				{
					Position = pointerPosition,
					Button = button,
				});
				hasButtonChange = true;
			}
		}

		if (pointerPosition != previousPointerPosition || !hasButtonChange)
		{
			PointerMove?.Invoke(new Events.PointerEvent
			{
				Position = pointerPosition,
				Delta = pointerPosition - previousPointerPosition,
				ButtonMask = buttonMask,
			});
		}

		previousButtonState = buttonMask;
		previousPointerPosition = pointerPosition;
	}
}

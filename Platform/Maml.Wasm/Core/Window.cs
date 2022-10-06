using Maml.Events;
using Maml.Math;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;

namespace Maml;
public partial class Window : WindowBase
{
	public override RenderTarget? RenderTarget { get; }

	public override double DpiRatio => throw new NotImplementedException();

	public override event EventHandler<ResizeEvent>? Resize;
	public override event EventHandler<PointerEvent>? PointerMove;
	public override event EventHandler<PointerEvent>? PointerDown;
	public override event EventHandler<PointerEvent>? PointerUp;
	public override event EventHandler<WheelEvent>? Wheel;
	public override event EventHandler<KeyEvent>? KeyDown;
	public override event EventHandler<KeyEvent>? KeyUp;
	public override event EventHandler<FocusEvent>? Focus;
	public override event EventHandler<FocusEvent>? Blur;
	public override event EventHandler<DrawEvent>? Draw;

	private Vector2 windowSize = baseWindowSize;
	protected override Vector2 GetSize() => windowSize;

	internal static Vector2 baseWindowSize { get; private set; } = Vector2.Zero;

	public Window()
	{
		RenderTarget = new RenderTarget() { CanvasId = 0, };
	}

	[JSExport]
	internal static void HandleResize(int windowID, int width, int height)
	{
		baseWindowSize = new(width, height);
		GetWindow(windowID)?.HandleResize(width, height);
	}

	private void HandleResize(int width, int height)
	{
		windowSize = new(width, height);
		Resize?.Invoke(this, new() { Size = Size, });
		SizeProperty[this].SetDirty();
		Update();
	}

	private Vector2 previousPointerPosition = Vector2.Zero;

	[JSExport]
	[SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "Not supported for JSExport")]
	internal static void HandlePointerMove(int windowID, int positionX, int positionY, int iButton, int iButtonMask)
	{
		GetWindow(windowID)?.HandlePointerMove(positionX, positionY, iButton, iButtonMask);
	}
	private void HandlePointerMove(int positionX, int positionY, int iButton, int iButtonMask)
	{
		var position = new Vector2(positionX, positionY);
		var button = jsButtonToPointerButton(iButton);
		var buttonMask = jsButtonMaskToPointerButton(iButtonMask);
		PointerMove?.Invoke(this, new()
		{
			Position = position,
			PositionDelta = position - previousPointerPosition,
			Button = button,
			ButtonMask = buttonMask,
		});
		previousPointerPosition = position;
		//Update();
	}

	[JSExport]
	[SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "Not supported for JSExport")]
	internal static void HandlePointerDown(int windowID, int positionX, int positionY, int iButton, int iButtonMask)
	{
		GetWindow(windowID)?.HandlePointerDown(positionX, positionY, iButton, iButtonMask);
	}
	private void HandlePointerDown(int positionX, int positionY, int iButton, int iButtonMask)
	{
		var position = new Vector2(positionX, positionY);
		var button = jsButtonToPointerButton(iButton);
		var buttonMask = jsButtonMaskToPointerButton(iButtonMask);
		PointerDown?.Invoke(this, new()
		{
			Position = position,
			Button = button,
			ButtonMask = buttonMask,
		});
		previousPointerPosition = position;
		//Update();
	}

	[JSExport]
	[SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "Not supported for JSExport")]
	internal static void HandlePointerUp(int windowID, int positionX, int positionY, int iButton, int iButtonMask)
	{
		GetWindow(windowID)?.HandlePointerUp(positionX, positionY, iButton, iButtonMask);
	}
	private void HandlePointerUp(int positionX, int positionY, int iButton, int iButtonMask)
	{
		var position = new Vector2(positionX, positionY);
		var button = jsButtonToPointerButton(iButton);
		var buttonMask = jsButtonMaskToPointerButton(iButtonMask);
		PointerUp?.Invoke(this, new()
		{
			Position = position,
			Button = button,
			ButtonMask = buttonMask,
		});
		previousPointerPosition = position;
		//Update();
	}

	private static PointerButton jsButtonToPointerButton(int iButton)
	{
		PointerButton button = (PointerButton)(1 << iButton);
		if (button == PointerButton.Right) { button = PointerButton.Middle; }
		else if (button == PointerButton.Middle) { button = PointerButton.Right; }
		else if (button < 0) { button = PointerButton.None; }
		return button;
	}

	private static PointerButton jsButtonMaskToPointerButton(int iButtonMask) => (PointerButton)iButtonMask;
}

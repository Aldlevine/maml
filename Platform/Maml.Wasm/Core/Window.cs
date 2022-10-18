using Maml.Events;
using Maml.Math;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;

namespace Maml;
public partial class Window : WindowBase
{
	public override RenderTarget? RenderTarget { get; }

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
	protected override Vector2 GetPixelSize() => windowSize;
	private static Vector2 baseWindowSize { get; set; } = Vector2.Zero;


	private double dpiRatio = baseDpiRatio;
	protected override double GetDpiRatio() => dpiRatio;
	private static double baseDpiRatio { get; set; } = 1;


	public Window()
	{
		RenderTarget = new RenderTarget() { CanvasId = 0, };
	}

	[JSExport]
	internal static void HandleResize(int windowID, int width, int height, double dpiRatio)
	{
		baseWindowSize = new(width, height);
		baseDpiRatio = dpiRatio;
		GetWindow(windowID)?.HandleResize(width, height, dpiRatio);
	}

	private void HandleResize(int width, int height, double dpiRatio)
	{
		windowSize = new(width, height);
		this.dpiRatio = dpiRatio;
		Resize?.Invoke(this, new() { Size = PixelSize, });
		PixelSizeProperty[this].SetDirty();
		DpiRatioProperty[this].SetDirty();
		SceneTree.updateRegion = new Rect { Size = Size, };
		//Update();
	}

	private Vector2 previousPointerPosition = Vector2.Zero;

	[JSExport]
	[SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "Not supported for JSExport")]
	internal static void HandlePointerMove(int windowID, double positionX, double positionY, int iButton, int iButtonMask)
	{
		GetWindow(windowID)?.HandlePointerMove(positionX, positionY, iButton, iButtonMask);
	}
	private void HandlePointerMove(double positionX, double positionY, int iButton, int iButtonMask)
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
	internal static void HandlePointerDown(int windowID, double positionX, double positionY, int iButton, int iButtonMask)
	{
		GetWindow(windowID)?.HandlePointerDown(positionX, positionY, iButton, iButtonMask);
	}
	private void HandlePointerDown(double positionX, double positionY, int iButton, int iButtonMask)
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
	internal static void HandlePointerUp(int windowID, double positionX, double positionY, int iButton, int iButtonMask)
	{
		GetWindow(windowID)?.HandlePointerUp(positionX, positionY, iButton, iButtonMask);
	}
	private void HandlePointerUp(double positionX, double positionY, int iButton, int iButtonMask)
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

using Maml.Events;
using Maml.Math;
using Maml.Scene;
using System;

namespace Maml;

public abstract class WindowBase<TRenderTarget> where TRenderTarget : RenderTargetBase
{
	public abstract TRenderTarget? RenderTarget { get; }
	public SceneTree SceneTree { get; init; } = new();

	public abstract Vector2 Size { get; }
	public abstract double DpiRatio { get; }

	public abstract event EventHandler<ResizeEvent>? Resize;
	public abstract event EventHandler<PointerEvent>? PointerMove;
	public abstract event EventHandler<PointerEvent>? PointerDown;
	public abstract event EventHandler<PointerEvent>? PointerUp;
	public abstract event EventHandler<WheelEvent>? Wheel;
	public abstract event EventHandler<KeyEvent>? KeyDown;
	public abstract event EventHandler<KeyEvent>? KeyUp;
	public abstract event EventHandler<FocusEvent>? Focus;
	public abstract event EventHandler<FocusEvent>? Blur;
	public abstract event EventHandler<DrawEvent>? Draw;
}

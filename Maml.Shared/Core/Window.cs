using Maml.Events;
using Maml.Math;
using Maml.Observable;
using Maml.Scene;
using System;

namespace Maml;

public abstract class WindowBase<TRenderTarget> : ObservableObject where TRenderTarget : RenderTargetBase
{
	public abstract TRenderTarget? RenderTarget { get; }
	public SceneTree SceneTree { get; init; } = new();

	public static ComputedProperty<WindowBase<TRenderTarget>, Vector2> SizeProperty = new()
	{
		Get = (WindowBase<TRenderTarget> window) => window.GetSize(),
		Cached = false,
	};
	public Vector2 Size => SizeProperty[this].Get();
	protected abstract Vector2 GetSize();
	// public Vector2 Size { get; protected set; }
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

	public WindowBase()
	{
		Engine.Singleton.Animator.NextFrame += (s, e) =>
		{
			SizeProperty[this].SetDirty(this, Size);
		};
	}


}

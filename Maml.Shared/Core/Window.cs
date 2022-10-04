using Maml.Events;
using Maml.Math;
using Maml.Observable;
using Maml.Scene;
using System;

namespace Maml;

public abstract class WindowBase : ObservableObject
{
	public abstract RenderTarget? RenderTarget { get; }
	public SceneTree SceneTree { get; init; } = new();

	// TODO: this might need to be implementation specific
	public static ComputedProperty<WindowBase, Vector2> SizeProperty = new()
	{
		Get = (window) => window.GetSize(),
		Cached = false,
	};
	public Vector2 Size => SizeProperty[this].Get();
	protected abstract Vector2 GetSize();
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
			SizeProperty[this].SetDirty();
		};
	}
}

public partial class Window : WindowBase { }

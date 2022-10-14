using Maml.Events;
using Maml.Graphics;
using Maml.Math;
using Maml.Observable;
using Maml.Scene;
using System;
using System.Collections.Generic;

namespace Maml;

public abstract class WindowBase : ObservableObject
{
	public abstract RenderTarget? RenderTarget { get; }
	public SceneTree SceneTree { get; init; } = new();

	// TODO: this might need to be implementation specific
	public static ComputedProperty<WindowBase, Vector2> PixelSizeProperty { get; } = new()
	{
		Get = (window) => window.GetPixelSize(),
		Cached = false,
	};
	public Vector2 PixelSize => PixelSizeProperty[this].Get();
	protected abstract Vector2 GetPixelSize();

	public static ComputedProperty<WindowBase, double> DpiRatioProperty { get; } = new()
	{
		Get = (window) => window.GetDpiRatio(),
	};
	public double DpiRatio => DpiRatioProperty[this].Get();
	protected abstract double GetDpiRatio();

	public static ComputedProperty<WindowBase, Vector2> SizeProperty { get; } = new()
	{
		Get = (window) => window.GetPixelSize() / window.GetDpiRatio(),
		Cached = false,
		Dependencies = (window) => new Binding[]
		{
			PixelSizeProperty[window],
			DpiRatioProperty[window]
		},
	};
	public Vector2 Size => SizeProperty[this].Get();


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

	private static int CurrentWindowID { get; set; } = 0;
	protected int windowID = CurrentWindowID++;
	// TODO: This should be weak ref
	private static readonly Dictionary<int, Window> Windows = new();
	protected static void RegisterWindow(Window window) => Windows[window.windowID] = window;
	protected static Window? GetWindow(int id)
	{
		if (Windows.TryGetValue(id, out var window))
		{
			return window;
		}
		return null;
	}

	public void Update()
	{
		if (RenderTarget == null) { return; }
		RenderTarget.BeginDraw();
		RenderTarget.Clear(new Color(0x333333ff));
		SceneTree.Draw(RenderTarget);
		RenderTarget.EndDraw();
	}

	public WindowBase()
	{
		RegisterWindow((Window)this);
		Engine.Singleton.Animator.NextFrame += (s, e) => PixelSizeProperty[this].SetDirty();
	}

	~WindowBase()
	{
		Windows.Remove(windowID);
	}
}

public partial class Window : WindowBase { }

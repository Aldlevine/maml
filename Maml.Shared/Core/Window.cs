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
	//public abstract event EventHandler<DrawEvent>? Draw;

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

	//internal Rect UpdateRect = new Rect();
	internal List<Rect> UpdateRect = new();
	public void PushUpdateRect(Rect rect)
	{
		if (rect.Size == Vector2.Zero)
		{
			return;
		}

		//if (UpdateRect.Size == Vector2.Zero)
		//{
		//	UpdateRect = rect;
		//}
		//else
		//{
		//	UpdateRect = UpdateRect.MergedWith(rect);
		//}

		//foreach (var r in UpdateRect)
		//{
		//	if (r.Intersects(rect))
		//	{
		//		mergedRect = r.MergedWith(rect);
		//		break;
		//	}
		//}
		rect.Position = Vector2.Floor(rect.Position);
		rect.Size = Vector2.Ceiling(rect.Size);

		for (int i = 0; i < UpdateRect.Count; i++)
		{
			var r = UpdateRect[i];
			if (r.Intersects(rect))
			{
				rect = rect.MergedWith(r);
				UpdateRect.RemoveAt(i);
				i--;
			}
		}

		UpdateRect.Add(rect);
	}

	//public Rect ComputeSceneUpdateRect()
	public List<Rect> ComputeSceneUpdateRect()
	{
		foreach (var node in SceneTree.Nodes)
		{
			if (node is GraphicNode graphicNode && graphicNode.NeedsRedraw)
			{
				PushUpdateRect(graphicNode.PreviousBoundingRect);
				PushUpdateRect(graphicNode.PreviousBoundingRect = graphicNode.GetBoundingRect());
			}
		}
		return UpdateRect;
	}

	public void Draw()
	{
		if (RenderTarget == null) { return; }
		RenderTarget.BeginDraw();
		RenderTarget.SetTransform(Transform.Identity);
		//RenderTarget.PushClip(new() {
		//	Position = Vector2.Floor(UpdateRect.Position),
		//	Size = Vector2.Ceiling(UpdateRect.Size),
		//});

		if (UpdateRect.Count > 0)
		{
			RenderTarget.PushLayer(UpdateRect.ToArray());
			foreach (var r in UpdateRect)
			{
				RenderTarget.ClearRect(new Color(0x333333ff), r);
			}
		}

		//RenderTarget.Clear(new Color(0x333333ff));

		SceneTree.Draw(RenderTarget, UpdateRect);

		if (UpdateRect.Count > 0)
		{
			RenderTarget.PopLayer();
		}
		RenderTarget.EndDraw();

		UpdateRect = new();
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

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

	public event EventHandler<RenderTarget>? Measure;

	public abstract event EventHandler<ResizeEvent>? Resize;
	public abstract event EventHandler<PointerEvent>? PointerMove;
	public abstract event EventHandler<PointerEvent>? PointerDown;
	public abstract event EventHandler<PointerEvent>? PointerUp;
	public abstract event EventHandler<WheelEvent>? Wheel;
	public abstract event EventHandler<KeyEvent>? KeyDown;
	public abstract event EventHandler<KeyEvent>? KeyUp;
	public abstract event EventHandler<FocusEvent>? Focus;
	public abstract event EventHandler<FocusEvent>? Blur;

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

	internal List<Rect> UpdateRegion = new();
	public void PushUpdateRect(Rect rect)
	{
		//if (rect.Size == Vector2.Zero || !rect.Intersects(new Rect { Size = Size, }))
		//{
		//	return;
		//}

		//if (UpdateRect.Size == Vector2.Zero)
		//{
		//	UpdateRect = rect;
		//}
		//else
		//{
		//	UpdateRect = UpdateRect.MergedWith(rect);
		//}

		if (rect.Size == Vector2.Zero || !rect.Intersects(new Rect { Size = Size, }))
		{
			return;
		}

		if (UpdateRegion.Count == 0)
		{
			UpdateRegion.Add(rect);
		}
		else
		{
			bool shouldTryMerge = true;
			while (shouldTryMerge)
			{
				shouldTryMerge = false;
				for (int i = 0; i < UpdateRegion.Count; i++)
				{
					if (rect.Intersects(UpdateRegion[i]))
					{
						rect = rect.MergedWith(UpdateRegion[i]);
						UpdateRegion.RemoveAt(i--);
						shouldTryMerge = true;
					}
				}
			}
			UpdateRegion.Add(rect);
		}
	}

	public IList<Rect> ComputeSceneUpdateRegion()
	{
		foreach (var node in SceneTree.Nodes)
		{
			if (node is IGraphicNode graphicNode && graphicNode.NeedsRedraw)
			{
				PushUpdateRect(graphicNode.PreviousBoundingRect);
				PushUpdateRect(graphicNode.PreviousBoundingRect = graphicNode.GetBoundingRect());
			}
		}
		return UpdateRegion;
	}

	public void InvokeMeasure()
	{
		if (RenderTarget != null)
		{
			Measure?.Invoke(this, RenderTarget);
		}
	}

	public void Draw()
	{
		if (RenderTarget == null) { return; }

		RenderTarget.BeginDraw();
		RenderTarget.SetTransform(Transform.Identity);
		Rect updateRect = Rect.Merge(UpdateRegion.ToArray());
		RenderTarget.PushClip(new()
		{
			Position = Vector2.Floor(updateRect.Position),
			Size = Vector2.Ceiling(updateRect.Size),
		});
		//RenderTarget.PushLayer(UpdateRegion);
		RenderTarget.Clear(new Color(0x333333ff));

		//SceneTree.Draw(RenderTarget, UpdateRect);
		SceneTree.Draw(RenderTarget, updateRect);

		RenderTarget.PopClip();
		//RenderTarget.PopLayer();

		RenderTarget.EndDraw();

		//UpdateRect = new();
		UpdateRegion.Clear();
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

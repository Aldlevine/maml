using Maml.Events;
using Maml.Graphics;
using Maml.Math;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Maml.Scene;

public interface IComponent { }

public class GraphicComponent : IComponent
{
	public Transform Transform { get; set; } = Transform.Identity;
	public Graphic? Graphic { get; set; } = default;

	public static implicit operator GraphicComponent(Graphic graphic) => new GraphicComponent { Graphic = graphic, };
}


public class InputComponent : IComponent
{
	// public event Events.EventHandler<PointerEvent>? PointerUp = default;
	// public event Events.EventHandler<PointerEvent>? PointerDown = default;
	// public event Events.EventHandler<PointerEvent>? PointerMove = default;

	private Mutex pointerUpLock = new();
	private event Events.EventHandler<PointerEvent>? pointerUp = default;
	public event Events.EventHandler<PointerEvent>? PointerUp
	{
		add
		{
			lock(pointerUpLock)
			{
				pointerUp += value;
				Input.PointerUp -= OnPointerUp;
				Input.PointerUp += OnPointerUp;
			}
		}
		remove
		{
			lock(pointerUpLock)
			{
				pointerUp -= value;
				if (pointerUp?.GetInvocationList().Length == 0)
				{
					Input.PointerUp -= OnPointerUp;
				}
			}
		}
	}

	private Mutex pointerDownLock = new();
	private event Events.EventHandler<PointerEvent>? pointerDown = default;
	public event Events.EventHandler<PointerEvent>? PointerDown
	{
		add
		{
			lock(pointerDownLock)
			{
				pointerDown += value;
				Input.PointerDown -= OnPointerDown;
				Input.PointerDown += OnPointerDown;
			}
		}
		remove
		{
			lock(pointerDownLock)
			{
				pointerDown -= value;
				if (pointerDown?.GetInvocationList().Length == 0)
				{
					Input.PointerDown -= OnPointerDown;
				}
			}
		}
	}

	private Mutex pointerMoveLock = new();
	private event Events.EventHandler<PointerEvent>? pointerMove = default;
	public event Events.EventHandler<PointerEvent>? PointerMove
	{
		add
		{
			lock(pointerMoveLock)
			{
				pointerMove += value;
				Input.PointerMove -= OnPointerMove;
				Input.PointerMove += OnPointerMove;
			}
		}
		remove
		{
			lock(pointerMoveLock)
			{
				pointerMove -= value;
				if (pointerMove?.GetInvocationList().Length == 0)
				{
					Input.PointerMove -= OnPointerMove;
				}
			}
		}
	}

	public Transform Transform { get; set; } = Transform.Identity;
	public Rect HitRect { get; set; } = default;

	private void OnPointerMove(PointerEvent evt)
	{
		pointerMove?.Invoke(evt);
	}

	private void OnPointerDown(PointerEvent evt)
	{
		pointerDown?.Invoke(evt);
	}

	private void OnPointerUp(PointerEvent evt)
	{
		pointerUp?.Invoke(evt);
	}
}

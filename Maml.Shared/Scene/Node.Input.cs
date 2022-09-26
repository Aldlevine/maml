using Maml.Events;
using System;
using System.Threading;

namespace Maml.Scene;

public partial class Node
{
	private Mutex pointerUpLock = new();
	private event EventHandler<PointerEvent>? pointerUp = default;
	public event EventHandler<PointerEvent>? PointerUp
	{
		add
		{
			lock(pointerUpLock)
			{
				pointerUp += value;
				Input.PointerUp -= HandlePointerUp;
				Input.PointerUp += HandlePointerUp;
			}
		}
		remove
		{
			lock(pointerUpLock)
			{
				pointerUp -= value;
				if (pointerUp?.GetInvocationList().Length == 0)
				{
					Input.PointerUp -= HandlePointerUp;
				}
			}
		}
	}

	private Mutex pointerDownLock = new();
	private event EventHandler<PointerEvent>? pointerDown = default;
	public event EventHandler<PointerEvent>? PointerDown
	{
		add
		{
			lock(pointerDownLock)
			{
				pointerDown += value;
				Input.PointerDown -= HandlePointerDown;
				Input.PointerDown += HandlePointerDown;
			}
		}
		remove
		{
			lock(pointerDownLock)
			{
				pointerDown -= value;
				if (pointerDown?.GetInvocationList().Length == 0)
				{
					Input.PointerDown -= HandlePointerDown;
				}
			}
		}
	}

	private Mutex pointerMoveLock = new();
	private event EventHandler<PointerEvent>? pointerMove = default;
	public event EventHandler<PointerEvent>? PointerMove
	{
		add
		{
			lock(pointerMoveLock)
			{
				pointerMove += value;
				Input.PointerMove -= HandlePointerMove;
				Input.PointerMove += HandlePointerMove;
			}
		}
		remove
		{
			lock(pointerMoveLock)
			{
				pointerMove -= value;
				if (pointerMove?.GetInvocationList().Length == 0)
				{
					Input.PointerMove -= HandlePointerMove;
				}
			}
		}
	}

	private void HandlePointerUp(object? sender, PointerEvent evt)
	{
		if (HitRect.HasPoint(GlobalTransform.Inverse() * evt.Position))
		{
			pointerUp?.Invoke(this, evt);
		}
	}

	private void HandlePointerDown(object? sender, PointerEvent evt)
	{
		if (HitRect.HasPoint(GlobalTransform.Inverse() * evt.Position))
		{
			pointerDown?.Invoke(this, evt);
		}
	}

	private void HandlePointerMove(object? sender, PointerEvent evt)
	{
		if (HitRect.HasPoint(GlobalTransform.Inverse() * evt.Position))
		{
			pointerMove?.Invoke(this, evt);
		}
	}
}

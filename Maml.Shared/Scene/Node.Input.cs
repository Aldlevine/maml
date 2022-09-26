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
				if (pointerExit?.GetInvocationList().Length == 0)
				if (pointerEnter?.GetInvocationList().Length == 0)
				if (pointerMove?.GetInvocationList().Length == 0)
				{
					Input.PointerMove -= HandlePointerMove;
				}
			}
		}
	}

	private Mutex pointerEnterLock = new();
	private event EventHandler<PointerEvent>? pointerEnter = default;
	public event EventHandler<PointerEvent>? PointerEnter
	{
		add
		{
			lock(pointerEnterLock)
			{
				pointerEnter += value;
				Input.PointerMove -= HandlePointerMove;
				Input.PointerMove += HandlePointerMove;
			}
		}
		remove
		{
			lock(pointerEnterLock)
			{
				pointerEnter -= value;
				if (pointerExit?.GetInvocationList().Length == 0)
				if (pointerEnter?.GetInvocationList().Length == 0)
				if (pointerMove?.GetInvocationList().Length == 0)
				{
					Input.PointerMove -= HandlePointerMove;
				}
			}
		}
	}

	private Mutex pointerExitLock = new();
	private event EventHandler<PointerEvent>? pointerExit = default;
	public event EventHandler<PointerEvent>? PointerExit
	{
		add
		{
			lock(pointerExitLock)
			{
				pointerExit += value;
				Input.PointerMove -= HandlePointerMove;
				Input.PointerMove += HandlePointerMove;
			}
		}
		remove
		{
			lock(pointerExitLock)
			{
				pointerExit -= value;
				if (pointerExit?.GetInvocationList().Length == 0)
				if (pointerEnter?.GetInvocationList().Length == 0)
				if (pointerMove?.GetInvocationList().Length == 0)
				{
					Input.PointerMove -= HandlePointerMove;
				}
			}
		}
	}

	private void HandlePointerUp(object? sender, PointerEvent evt)
	{
		if (HitShape.HasPoint(GlobalTransform.Inverse() * evt.Position))
		{
			pointerUp?.Invoke(this, evt);
		}
	}

	private void HandlePointerDown(object? sender, PointerEvent evt)
	{
		if (HitShape.HasPoint(GlobalTransform.Inverse() * evt.Position))
		{
			pointerDown?.Invoke(this, evt);
		}
	}

	private bool hasPointer = false;
	private void HandlePointerMove(object? sender, PointerEvent evt)
	{
		if (HitShape.HasPoint(GlobalTransform.Inverse() * evt.Position))
		{
			pointerMove?.Invoke(this, evt);
			if (!hasPointer)
			{
				hasPointer = true;
				pointerEnter?.Invoke(this, evt);
			}
		}
		else if (hasPointer)
		{
			hasPointer = false;
			pointerExit?.Invoke(this, evt);
		}
	}
}

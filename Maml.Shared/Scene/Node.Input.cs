using Maml.Events;
using System;

namespace Maml.Scene;

public partial class Node
{
	public EventHandler<PointerEvent> OnPointerUp { init => PointerUp += value; }
	private object pointerUpLock { get; } = new();
	private event EventHandler<PointerEvent>? pointerUp = default;
	public event EventHandler<PointerEvent>? PointerUp
	{
		add
		{
			lock (pointerUpLock)
			{
				pointerUp += value;
				Window.PointerUp -= HandlePointerUp;
				Window.PointerUp += HandlePointerUp;
			}
		}
		remove
		{
			lock (pointerUpLock)
			{
				pointerUp -= value;
				if (pointerUp?.GetInvocationList().Length == 0)
				{
					Window.PointerUp -= HandlePointerUp;
				}
			}
		}
	}

	public EventHandler<PointerEvent> OnPointerDown { init => PointerDown += value; }
	private object pointerDownLock { get; } = new();
	private event EventHandler<PointerEvent>? pointerDown = default;
	public event EventHandler<PointerEvent>? PointerDown
	{
		add
		{
			lock (pointerDownLock)
			{
				pointerDown += value;
				Window.PointerDown -= HandlePointerDown;
				Window.PointerDown += HandlePointerDown;
			}
		}
		remove
		{
			lock (pointerDownLock)
			{
				pointerDown -= value;
				if (pointerDown?.GetInvocationList().Length == 0)
				{
					Window.PointerDown -= HandlePointerDown;
				}
			}
		}
	}

	public EventHandler<PointerEvent> OnPointerMove { init => PointerMove += value; }
	private object pointerMoveLock { get; } = new();
	private event EventHandler<PointerEvent>? pointerMove = default;
	public event EventHandler<PointerEvent>? PointerMove
	{
		add
		{
			lock (pointerMoveLock)
			{
				pointerMove += value;
				Window.PointerMove -= HandlePointerMove;
				Window.PointerMove += HandlePointerMove;
			}
		}
		remove
		{
			lock (pointerMoveLock)
			{
				pointerMove -= value;
				if ((pointerExit?.GetInvocationList().Length ?? 0) == 0)
					if ((pointerEnter?.GetInvocationList().Length ?? 0) == 0)
						if ((pointerMove?.GetInvocationList().Length ?? 0) == 0)
						{
							Window.PointerMove -= HandlePointerMove;
						}
			}
		}
	}

	public EventHandler<PointerEvent> OnPointerEnter { init => PointerEnter += value; }
	private object pointerEnterLock { get; } = new();
	private event EventHandler<PointerEvent>? pointerEnter = default;
	public event EventHandler<PointerEvent>? PointerEnter
	{
		add
		{
			lock (pointerEnterLock)
			{
				pointerEnter += value;
				Window.PointerMove -= HandlePointerMove;
				Window.PointerMove += HandlePointerMove;
			}
		}
		remove
		{
			lock (pointerEnterLock)
			{
				pointerEnter -= value;
				if ((pointerExit?.GetInvocationList().Length ?? 0) == 0)
					if ((pointerEnter?.GetInvocationList().Length ?? 0) == 0)
						if ((pointerMove?.GetInvocationList().Length ?? 0) == 0)
						{
							Window.PointerMove -= HandlePointerMove;
						}
			}
		}
	}

	public EventHandler<PointerEvent> OnPointerExit { init => PointerExit += value; }
	private object pointerExitLock { get; } = new();
	private event EventHandler<PointerEvent>? pointerExit = default;
	public event EventHandler<PointerEvent>? PointerExit
	{
		add
		{
			lock (pointerExitLock)
			{
				pointerExit += value;
				Window.PointerMove -= HandlePointerMove;
				Window.PointerMove += HandlePointerMove;
			}
		}
		remove
		{
			lock (pointerExitLock)
			{
				pointerExit -= value;
				if ((pointerExit?.GetInvocationList().Length ?? 0) == 0)
					if ((pointerEnter?.GetInvocationList().Length ?? 0) == 0)
						if ((pointerMove?.GetInvocationList().Length ?? 0) == 0)
						{
							Window.PointerMove -= HandlePointerMove;
						}
			}
		}
	}

	private void HandlePointerUp(object? sender, PointerEvent evt)
	{
		if (HitShape?.HasPoint(GlobalTransform.Inverse() * evt.Position) ?? false)
		{
			pointerUp?.Invoke(this, evt);
		}
	}

	private void HandlePointerDown(object? sender, PointerEvent evt)
	{
		if (HitShape?.HasPoint(GlobalTransform.Inverse() * evt.Position) ?? false)
		{
			pointerDown?.Invoke(this, evt);
		}
	}

	private bool hasPointer = false;
	private void HandlePointerMove(object? sender, PointerEvent evt)
	{
		if (HitShape?.HasPoint(GlobalTransform.Inverse() * evt.Position) ?? false)
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

using Maml.Animation;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Maml;

public abstract class EngineBase
{
	public abstract Window Window { get; protected set; }
	public Animator Animator { get; } = new();
	public abstract void Run();

	public virtual void Initialize() { }
	public virtual void Dispose() { }

	internal Mutex EventMutex = new();

	private Queue<Action> deferredQueue { get; init; } = new();
	public void QueueDeferred(Action action)
	{
		if (deferredQueue.Contains(action))
		{
			return;
		}
		deferredQueue.Enqueue(action);
	}

	internal void ProcessDeferred()
	{
		while (deferredQueue.TryDequeue(out var action))
		{
			action.Invoke();
		}
	}
}

public partial class Engine : EngineBase
{
	private static Engine? singleton { get; set; }
	public static Engine Singleton => singleton ?? (singleton = new Engine());
}

using Maml.Events;
using System;
using System.Threading;

namespace Maml.Animation;

public partial class Animator
{
	private static Animator singleton = new();
	public static Animator Singleton => singleton ??= new();

	public event Maml.Events.EventHandler<FrameEvent>? Frame;

	private DateTime lastTick = DateTime.Now;
	private DateTime tick = DateTime.Now;
	internal Mutex tickMutex = new();

	internal void Tick()
	{
		tickMutex.WaitOne();

		tick = DateTime.Now;
		Frame?.Invoke(new() { Delta = tick - lastTick });
		lastTick = tick;

		tickMutex.ReleaseMutex();
	}
}

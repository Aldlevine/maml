using Maml.Events;
using System;

namespace Maml.Animation;

public partial class Animator
{
	public event Maml.Events.EventHandler<FrameEvent>? Frame;

	private DateTime lastTick = DateTime.Now;
	private DateTime lastFrame = DateTime.Now;
	internal bool ticking = false;

	internal void Tick()
	{
		if (ticking) { return; }
		ticking = true;
		var tick = DateTime.Now;

		while (tick - lastFrame > TimeSpan.FromMilliseconds(1))
		// while ((tick - lastFrame).Ticks > 0)
		{
			Frame?.Invoke(new() { Delta = tick - lastFrame });
			lastFrame = tick;
		}

		// lastTick = tick;
		ticking = false;
	}
}

using System;
using System.Timers;

namespace Maml.Animation;

public partial class Animator
{
	private DateTime lastTick = DateTime.Now;

	private Timer timer = new()
	{
		Interval = double.Epsilon,
		AutoReset = true,
		Enabled = true,
	};

	internal bool ticking = false;
	internal void Tick()
	{
		if (ticking) { return;  }
		ticking = true;
		var tick = DateTime.Now;
		Frame?.Invoke(new() { Delta = tick - lastTick });
		lastTick = tick;
		ticking = false;
	}

	public void StartFrameLoop()
	{
		lastTick = DateTime.Now;
		timer.Elapsed += (s, e) =>
		{
			Frame?.Invoke(new() { Delta = e.SignalTime - lastTick });
			lastTick = e.SignalTime;
		};
	}

	public void StopFrameLoop()
	{
		timer.Stop();
	}
}

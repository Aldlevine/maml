using Maml.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Maml.Animation;

public enum FrameState
{
	Enter,
	Exit,
	Play,
}

public record FrameEvent : Event
{
	public FrameState FrameState;
	public DateTime Tick;
	public TimeSpan Delta;
}

public partial class Animator
{
	private static Animator singleton = new();
	public static Animator Singleton => singleton ??= new();

	private Mutex frameMutex = new();
	private event EventHandler<FrameEvent>? frame;
	public event EventHandler<FrameEvent>? Frame
	{
		add
		{
			lock (frameMutex)
			{
				frame += value;
				value?.Invoke(this, new()
				{
					FrameState = FrameState.Enter,
					Tick = tick,
					Delta = delta,
				});
				value?.Invoke(this, new()
				{
					FrameState = FrameState.Play,
					Tick = tick,
					Delta = delta,
				});
			}
		}
		remove
		{
			lock (frameMutex)
			{
				if (Array.IndexOf(frame.GetInvocationList(), value) > -1)
				{
					frame -= value;
					value?.Invoke(this, new()
					{
						FrameState = FrameState.Exit,
						Tick = tick,
						Delta = delta
					});
				}
			}
		}
	}


	private static readonly TimeSpan minDelta = TimeSpan.FromMilliseconds(4);
	private static readonly TimeSpan maxDelta = TimeSpan.FromMilliseconds(256);
	private TimeSpan delta = default;
	private DateTime tick = DateTime.Now;
	private DateTime lastTick = DateTime.Now;

	private Mutex tickMutex = new();
	internal void Tick()
	{
		if (frame == null) { return; }

		lock (tickMutex)
		{
			tick = DateTime.Now;
			delta = tick - lastTick;

			Parallel.ForEach(frame.GetInvocationList(), (inv, state) =>
			{
				((EventHandler<FrameEvent>)inv).Invoke(this, new()
				{
					FrameState = FrameState.Play,
					Tick = tick,
					Delta = delta,
				});
			});

			lastTick = tick;
		}
	}


	private Thread? tickerLoopThread;
	private ManualResetEventSlim tickerEvent = new();
	private bool tickerAlive = true;
	private void TickerLoop()
	{
		while (tickerAlive)
		{
			tickerEvent.Wait();
			Tick();
			Thread.Sleep(1);
		}
	}

	public void StartTicker()
	{
		if (tickerLoopThread == null)
		{
			tickerLoopThread = new Thread(new ThreadStart(TickerLoop));
			tickerLoopThread.Start();
		}
		tickerEvent.Set();
	}
	public void StopTicker() => tickerEvent.Reset();

	public void Dispose()
	{
		tickerAlive = false;
		tickerEvent.Set();
		tickerLoopThread?.Join();
	}
}

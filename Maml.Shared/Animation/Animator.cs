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

public class Animator
{
	private Mutex frameMutex { get; } = new();
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
				if (frame != null && Array.IndexOf(frame.GetInvocationList(), value) > -1)
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

	public event EventHandler<FrameEvent>? NextFrame;

	private TimeSpan delta = default;
	private DateTime tick = DateTime.Now;
	private DateTime lastTick = DateTime.Now;

	internal void Tick()
	{
		if (frame == null) { return; }

		lock (Engine.Singleton.EventMutex)
		{
			tick = DateTime.Now;
			delta = tick - lastTick;

			FrameEvent evt = new()
			{
				FrameState = FrameState.Play,
				Tick = tick,
				Delta = delta,
			};

			Parallel.ForEach(frame.GetInvocationList(), (inv, state) => ((EventHandler<FrameEvent>)inv).Invoke(this, evt));
			//frame?.Invoke(this, evt);

			NextFrame?.Invoke(this, evt);
			NextFrame = null;

			lastTick = tick;

			Engine.Singleton.ProcessDeferred();
		}
	}


	private Thread? tickerLoopThread;
	private ManualResetEventSlim tickerEvent { get; } = new();
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

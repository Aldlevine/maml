﻿using System;
using System.Threading;

namespace Maml.Animation;

public partial class Animator
{

	private Thread? frameLoopThread;
	private bool running;

	public void StartFrameLoop()
	{
		running = true;
		frameLoopThread = new(new ThreadStart(FrameLoop));
		frameLoopThread.Start();
	}

	public void StopFrameLoop()
	{
		running = false;
	}

	private void FrameLoop()
	{
		double lastFrameTime = DateTime.Now.Ticks / 10_000_000.0;
		while (running)
		{
			double frameTime = DateTime.Now.Ticks / 10_000_000.0;

			Frame?.Invoke(new()
			{
				Delta = frameTime - lastFrameTime,
			});

			int ftMs = (int)(frameTime * 1000);
			int lftMs = (int)(lastFrameTime * 1000);
			int deltaMs = ftMs - lftMs;

			lastFrameTime = frameTime;

			if (deltaMs < 12)
			{
				Thread.Sleep(12 - deltaMs);
			}
		}
	}
}

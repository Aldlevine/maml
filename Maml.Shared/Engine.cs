using Maml.Animation;
using Maml.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maml;

public abstract class EngineBase<TWindow, TRenderTarget>
	where TRenderTarget: RenderTargetBase
	where TWindow: WindowBase<TRenderTarget>
{
	public abstract TWindow Window { get; init; }
	public Animator Animator { get; init; } = new();
	public abstract void Run();
}

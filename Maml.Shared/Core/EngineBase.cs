using Maml.Animation;
using Maml.Events;
using Maml.Graphics;

namespace Maml;

public abstract class EngineBase<TWindow, TRenderTarget>
	where TRenderTarget : RenderTargetBase
	where TWindow : WindowBase<TRenderTarget>
{
	public abstract TWindow Window { get; protected set; }
	public Animator Animator { get; } = new();
	public abstract void Run();

	public virtual void Initialize()
	{
		Window.Draw += HandleDraw;
	}

	private void HandleDraw(object? sender, DrawEvent e)
	{
		if (e.RenderTarget is RenderTarget rt)
		{
			rt.Clear(new Color(0x333333ff));
			rt.DrawScene(Window.SceneTree);
		}
	}
}

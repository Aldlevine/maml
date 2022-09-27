using Maml.Animation;
using Maml.Events;
using Maml.Graphics;
using Windows.Foundation.Metadata;

namespace Maml;

public abstract class EngineBase<TWindow, TRenderTarget>
	where TRenderTarget : RenderTargetBase
	where TWindow : WindowBase<TRenderTarget>
{
	public abstract TWindow Window { get; protected set; }
	public Animator Animator { get; } = new();
	public abstract void Run();

	public virtual void Initialize() { }
}

public partial class Engine
{
	private static Engine? singleton { get; set; }
	public static Engine Singleton => singleton ?? (singleton = new Engine());
}

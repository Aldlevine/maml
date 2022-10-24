using Maml.Animation;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;

namespace Maml;

public partial class Engine
{
	public override void Run()
	{
		// All we need is to handle the first tick/draw
		// Everything else is hooked up through JS
		Animator.Tick();
		Window?.PushUpdateRect(new Math.Rect { Size = Math.Vector2.One * 100_000, });
		Window?.Draw();
	}

	public override void Initialize()
	{
		Window = new();

		Animator.LateFrame += Frame;

		base.Initialize();
	}

	private void Frame(object? source, FrameEvent evt)
	{
		ProcessDeferred();
		Window?.ComputeSceneUpdateRegion();
		Window?.Draw();
	}

	[JSExport]
	[SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "Not supported for JSExport")]
	private static void HandleAnimationFrame(int _1 /* id */)
	{
		Singleton.HandleAnimationFrame();
	}
	private void HandleAnimationFrame() => Animator.Tick();
}

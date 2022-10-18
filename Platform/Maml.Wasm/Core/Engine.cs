using Maml.Animation;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;

namespace Maml;

public partial class Engine
{
	public override void Run()
	{
		// This might be a noop because everything is handled though static interop
	}

	public override void Initialize()
	{
		Window = new();

		base.Initialize();
	}

	[JSExport]
	[SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "Not supported for JSExport")]
	private static void HandleAnimationFrame(int _1 /* id */)
	{
		Singleton.HandleAnimationFrame();
	}
	private void HandleAnimationFrame()
	{
		Animator.Tick();
		Window?.ComputeSceneUpdateRect();
		Window?.Draw();
	}
}

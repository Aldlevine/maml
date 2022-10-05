using Maml.Animation;
using Maml.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

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

		Animator.Frame += Frame;

		base.Initialize();
	}

	private void Frame(object? sender, FrameEvent e)
	{
		// Draw my stuff here
		if (Window.RenderTarget != null)
		{
			Window.RenderTarget.Clear(new Color(0x333333ff));
			Window.SceneTree.Draw(Window.RenderTarget);
		}
	}

	[JSExport]
	[SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "Not supported for JSExport")]
	private static void HandleAnimationFrame(int _1 /* id */)
	{
		Singleton.HandleAnimationFrame();
	}
	private void HandleAnimationFrame() => Animator.Tick();
}
